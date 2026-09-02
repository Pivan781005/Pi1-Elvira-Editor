namespace ElviraVgaEditor;

internal enum CompositeBuildMode { PristineOnly, Full }
internal enum CompositeBuildStage { ValidateContext, ValidateBaseline, ValidateProjectInputs, ValidateVariant, PrepareDisposableVariant, ApplyDataTransformations, ApplyGraphicsTransformations, ApplyFontTransformations, ApplyRuntimeUiTransformations, ApplyExecutableTransformation, ValidateOutput }
internal enum CompositeBuildStatus { Success, PreflightFailed, BaselineInvalid, VariantInvalid, DisposableBuildFailed, TransformationFailed, ValidationFailed, NotFullyConfigured }
internal sealed record CompositeBuildCapability(CompositeBuildStage Stage, string Name, bool Configured, string Detail);
internal sealed record CompositeBuildPlan(string GameId, BuiltInVariantId VariantId, VariantRuntimeKind RuntimeKind, string ProjectCode, string VariantRoot, string BaselineFingerprint, CompositeBuildMode Mode, IReadOnlyList<CompositeBuildStage> Stages, IReadOnlyList<CompositeBuildCapability> Capabilities);
internal sealed record CompositeBuildStageResult(CompositeBuildStage Stage, string Name, bool Executed, bool Succeeded, string Detail);
internal sealed record CompositeBuildResult(CompositeBuildStatus Status, CompositeBuildPlan Plan, IReadOnlyList<CompositeBuildStageResult> Stages);

internal interface ICompositeBuildStep
{
    CompositeBuildStage Stage { get; }
    string Name { get; }
    bool AppliesTo(VariantContext variant);
    string? Preflight(ProjectContext project, VariantContext variant);
    string? Execute(ProjectContext project, VariantContext variant);
}

/// <summary>Single deterministic build boundary. It owns no binary-specific patch logic.</summary>
internal sealed class CompositeBuildService
{
    private static readonly CompositeBuildStage[] OrderedStages = Enum.GetValues<CompositeBuildStage>();
    private readonly DisposableVariantBuildService _disposable;
    private readonly VariantDirectoryService _directories;
    private readonly IReadOnlyList<ICompositeBuildStep> _steps;
    private readonly Func<ProjectContext, VariantContext, IReadOnlyList<ICompositeBuildStep>>? _stepProvider;
    private readonly Func<ProjectContext, VariantContext, IReadOnlyList<string>>? _runtimeArtifactProvider;
    private readonly Func<ProjectContext, VariantContext, string>? _projectVariantProvider;

    public CompositeBuildService(DisposableVariantBuildService disposable, VariantDirectoryService directories, IEnumerable<ICompositeBuildStep>? steps = null,
        Func<ProjectContext, VariantContext, IReadOnlyList<ICompositeBuildStep>>? stepProvider = null,
        Func<ProjectContext, VariantContext, IReadOnlyList<string>>? runtimeArtifactProvider = null,
        Func<ProjectContext, VariantContext, string>? projectVariantProvider = null)
    {
        _disposable = disposable ?? throw new ArgumentNullException(nameof(disposable)); _directories = directories ?? throw new ArgumentNullException(nameof(directories));
        _steps = (steps ?? []).OrderBy(step => step.Stage).ThenBy(step => step.Name, StringComparer.Ordinal).ToArray();
        _stepProvider = stepProvider;
        _runtimeArtifactProvider = runtimeArtifactProvider;
        _projectVariantProvider = projectVariantProvider;
    }

    public CompositeBuildPlan CreatePlan(ProjectContext project, VariantContext variant, CompositeBuildMode mode)
    {
        Validate(project, variant);
        return new(PristineManifestService.GameIdFor(project.GameProfile), variant.VariantId, variant.RuntimeKind,
            GetProjectVariantCode(project, variant), _directories.GetVariantDirectoryPath(project, variant), project.BaselineFingerprint, mode, OrderedStages, BuildCapabilities(project, variant));
    }

    public CompositeBuildResult Build(ProjectContext project, VariantContext variant, CompositeBuildMode mode)
    {
        CompositeBuildPlan plan;
        try { plan = CreatePlan(project, variant, mode); }
        catch (ArgumentException ex) { return new(CompositeBuildStatus.VariantInvalid, EmptyPlan(project, variant, mode), [new(CompositeBuildStage.ValidateContext, "Context", false, false, ex.Message)]); }
        var results = new List<CompositeBuildStageResult>();
        if (new PristineManifestService(project.StorageLayout, project.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
            return Finish(CompositeBuildStatus.BaselineInvalid, results, plan, CompositeBuildStage.ValidateBaseline, "Baseline", false, "Baseline does not match.");
        results.Add(new(CompositeBuildStage.ValidateContext, "Context", true, true, "Valid")); results.Add(new(CompositeBuildStage.ValidateBaseline, "Baseline", true, true, "Valid"));
        IReadOnlyList<ICompositeBuildStep> applicable;
        try { applicable = mode == CompositeBuildMode.Full ? ApplicableSteps(project, variant) : []; }
        catch (ArgumentException ex) { return Finish(CompositeBuildStatus.PreflightFailed, results, plan, CompositeBuildStage.ValidateProjectInputs, "Step configuration", false, ex.Message); }
        foreach (ICompositeBuildStep step in applicable)
        {
            string? error = step.Preflight(project, variant);
            if (error is not null) return Finish(CompositeBuildStatus.PreflightFailed, results, plan, step.Stage, step.Name, false, error);
            results.Add(new(step.Stage, step.Name, true, true, "Preflight passed"));
        }
        if (mode == CompositeBuildMode.Full && !HasRequiredFutureStages(applicable))
            return Finish(CompositeBuildStatus.NotFullyConfigured, results, plan, CompositeBuildStage.ApplyExecutableTransformation, "Future transformations", false, "Required transformation stages are not configured: " + string.Join(", ", MissingFutureStages(applicable)) + ".");
        DisposableVariantBuildResult baseBuild = _disposable.Build(project, variant);
        if (baseBuild.Status != DisposableVariantBuildStatus.Success) return Finish(CompositeBuildStatus.DisposableBuildFailed, results, plan, CompositeBuildStage.PrepareDisposableVariant, "DisposableVariantBuildService", false, baseBuild.Status.ToString());
        results.Add(new(CompositeBuildStage.PrepareDisposableVariant, "DisposableVariantBuildService", true, true, "Pristine tree ready"));
        foreach (ICompositeBuildStep step in applicable)
        {
            string? error = step.Execute(project, variant);
            if (error is not null) return Finish(CompositeBuildStatus.TransformationFailed, results, plan, step.Stage, step.Name, true, error);
            results.Add(new(step.Stage, step.Name, true, true, "Executed"));
        }
        VariantDirectoryOperationResult owned = _directories.ValidateOwnedVariantDirectory(project, variant);
        if (owned.Status != VariantDirectoryOperationStatus.AlreadyValid) return Finish(CompositeBuildStatus.ValidationFailed, results, plan, CompositeBuildStage.ValidateOutput, "Output", true, owned.Status.ToString());
        try { VariantManifestService.Write(project, variant, _directories, mode, GetRuntimeArtifactNames(project, variant), GetProjectVariantCode(project, variant)); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        { return Finish(CompositeBuildStatus.ValidationFailed, results, plan, CompositeBuildStage.ValidateOutput, "Variant manifest", true, ex.Message); }
        return Finish(CompositeBuildStatus.Success, results, plan, CompositeBuildStage.ValidateOutput, "Output", true, "Owned output is structurally valid.");
    }

    internal IReadOnlyList<string> GetRuntimeArtifactNames(ProjectContext project, VariantContext variant)
    {
        Validate(project, variant);
        IReadOnlyList<string> names = _runtimeArtifactProvider?.Invoke(project, variant) ?? [variant.GeneratedExecutableName, variant.LogicalDataFileName];
        string[] normalized = names.Select(name => Path.GetFileName(name ?? string.Empty).ToUpperInvariant()).ToArray();
        if (normalized.Length != 2 || normalized.Any(name => !GameDataFileService.IsDos83FileName(name)) || normalized.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 2)
            throw new ArgumentException("Composite runtime artifacts must be two distinct DOS 8.3 file names.");
        return normalized;
    }

    internal string GetProjectVariantCode(ProjectContext project, VariantContext variant)
    {
        Validate(project, variant);
        return ProjectVariantOwnership.NormalizeCode(project, _projectVariantProvider?.Invoke(project, variant) ?? ProjectVariantOwnership.OriginalCode);
    }

    private IReadOnlyList<ICompositeBuildStep> ApplicableSteps(ProjectContext project, VariantContext variant)
    {
        IEnumerable<ICompositeBuildStep> source = _stepProvider?.Invoke(project, variant) ?? _steps;
        ICompositeBuildStep[] applicable = source.Where(step => step.AppliesTo(variant)).OrderBy(step => step.Stage).ThenBy(step => step.Name, StringComparer.Ordinal).ToArray();
        if (applicable.GroupBy(step => step.Stage).Any(group => group.Count() > 1)) throw new ArgumentException("Duplicate exclusive build steps apply to the same stage/runtime.");
        return applicable;
    }
    private IReadOnlyList<CompositeBuildCapability> BuildCapabilities(ProjectContext project, VariantContext variant)
    {
        var capabilities = new List<CompositeBuildCapability>();
        IEnumerable<ICompositeBuildStep> source = _stepProvider?.Invoke(project, variant) ?? _steps;
        foreach (ICompositeBuildStep step in source.Where(step => step.AppliesTo(variant)).OrderBy(step => step.Stage).ThenBy(step => step.Name, StringComparer.Ordinal))
        {
            try
            {
                string? preflight = step.Preflight(project, variant);
                capabilities.Add(new(step.Stage, step.Name, preflight is null, preflight ?? "Configured"));
            }
            catch (Exception ex) { capabilities.Add(new(step.Stage, step.Name, false, "Preflight threw: " + ex.Message)); }
        }
        return capabilities;
    }
    private static bool HasRequiredFutureStages(IReadOnlyList<ICompositeBuildStep> applicable)
    {
        var configured = applicable.Select(step => step.Stage).ToHashSet();
        return new[]
        {
            CompositeBuildStage.ApplyDataTransformations,
            CompositeBuildStage.ApplyGraphicsTransformations,
            CompositeBuildStage.ApplyFontTransformations,
            CompositeBuildStage.ApplyRuntimeUiTransformations,
            CompositeBuildStage.ApplyExecutableTransformation
        }.All(configured.Contains);
    }
    private static IEnumerable<string> MissingFutureStages(IReadOnlyList<ICompositeBuildStep> applicable)
    {
        var configured = applicable.Select(step => step.Stage).ToHashSet();
        return new[]
        {
            CompositeBuildStage.ApplyDataTransformations,
            CompositeBuildStage.ApplyGraphicsTransformations,
            CompositeBuildStage.ApplyFontTransformations,
            CompositeBuildStage.ApplyRuntimeUiTransformations,
            CompositeBuildStage.ApplyExecutableTransformation
        }.Where(stage => !configured.Contains(stage)).Select(stage => stage.ToString());
    }
    private static void Validate(ProjectContext project, VariantContext variant)
    {
        if (project is null || variant is null || !ReferenceEquals(project, variant.Project)) throw new ArgumentException("Variant must belong to the exact ProjectContext.");
    }
    private static CompositeBuildResult Finish(CompositeBuildStatus status, List<CompositeBuildStageResult> results, CompositeBuildPlan plan, CompositeBuildStage stage, string name, bool executed, string detail) { results.Add(new(stage, name, executed, status == CompositeBuildStatus.Success, detail)); return new(status, plan, results); }
    private static CompositeBuildPlan EmptyPlan(ProjectContext project, VariantContext variant, CompositeBuildMode mode) => new("Invalid", variant?.VariantId ?? 0, variant?.RuntimeKind ?? 0, ProjectVariantOwnership.OriginalCode, string.Empty, project?.BaselineFingerprint ?? string.Empty, mode, OrderedStages, []);
}
