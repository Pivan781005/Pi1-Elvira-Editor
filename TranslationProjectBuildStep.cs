namespace ElviraVgaEditor;

/// <summary>
/// Explicit data-projection step for one project-owned translation.  It is not
/// selected implicitly by the UI: callers must supply the semantic language
/// code as part of an authorized composite build plan.
/// </summary>
internal sealed class TranslationProjectBuildStep : ICompositeBuildStep
{
    private readonly TranslationProjectService _translations;
    private readonly VariantDirectoryService _directories;
    private readonly string _code;

    internal TranslationProjectBuildStep(TranslationProjectService translations, VariantDirectoryService directories, string code)
    {
        _translations = translations ?? throw new ArgumentNullException(nameof(translations));
        _directories = directories ?? throw new ArgumentNullException(nameof(directories));
        _code = code ?? throw new ArgumentNullException(nameof(code));
    }

    public CompositeBuildStage Stage => CompositeBuildStage.ApplyDataTransformations;
    public string Name => "Project translation data";
    public bool AppliesTo(VariantContext variant) => variant is not null;

    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        if (project is null || variant is null || !ReferenceEquals(project, variant.Project)) return "Translation data requires the exact active ProjectContext and VariantContext.";
        TranslationProjectLoadResult loaded = _translations.Load(project);
        if (!loaded.IsSuccess) return loaded.Detail ?? "Translation project data is invalid.";
        try
        {
            string code = TranslationProjectService.ValidateProjectCode(_code, project.GameProfile);
            return loaded.State!.Variants.Any(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase))
                ? null : $"No project translation exists for code {code}.";
        }
        catch (InvalidOperationException ex) { return ex.Message; }
    }

    public string? Execute(ProjectContext project, VariantContext variant)
    {
        try
        {
            TranslationProjectLoadResult loaded = _translations.Load(project);
            if (!loaded.IsSuccess) return loaded.Detail ?? "Translation project data is invalid.";
            string code = TranslationProjectService.ValidateProjectCode(_code, project.GameProfile);
            TranslationProjectVariant translation = loaded.State!.Variants.Single(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            _ = _translations.MaterializeOwnedVariantDataFile(project, variant, _directories, translation);
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException or ArgumentException)
        {
            return "Translation data build failed: " + ex.Message;
        }
    }
}
