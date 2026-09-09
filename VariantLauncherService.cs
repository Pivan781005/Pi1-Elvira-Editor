using System.Text.Json;

namespace Pi1ElviraEditor;

/// <summary>Read-only routing state. A target is never an instruction to run
/// DOS; R6R owns process execution.</summary>
internal enum VariantLaunchReadiness
{
    NoActiveInstallation,
    VariantMissing,
    ForeignOrInvalidVariant,
    BuildIncomplete,
    LaunchReady
}

internal sealed record VariantLaunchTarget(
    string GameId,
    BuiltInVariantId VariantId,
    VariantRuntimeKind RuntimeKind,
    string VariantRoot,
    string WorkingDirectory,
    string ExecutableFile,
    string DataFile,
    VariantDirectoryOperationStatus Ownership,
    bool BuildConfigured,
    VariantLaunchReadiness Readiness,
    string Detail);

internal sealed record LauncherRedirectionPlan(
    string LauncherRelativePath,
    string ActiveLauncherPath,
    string RestoreSourcePath,
    PristineFileClassification Classification,
    VariantLaunchTarget Target,
    bool IsWriteAuthorized,
    string Detail);

/// <summary>Semantic, non-mutating launcher target resolver. It relies only
/// on immutable ProjectContext, VariantContext and ownership validation; it
    /// deliberately never reads backup-named files nor discovers root *SK artifacts.
///
/// R9F V7 authoritative runnable identity is Installation + Game/Profile +
/// Runtime + Edition. Every Mods &amp; Launcher consumer (top summary,
/// edition availability, default-variant choices, launcher readiness,
/// Run/Debug enablement, preview, executable/data lookup, rebuild
/// eligibility) must resolve through ResolveEdition with the same explicit
/// (runtime, edition) pair. The runtime parent (VARIANTS\E1VGA) is a
/// container and is never treated as the selected edition output.</summary>
internal sealed class VariantLauncherService
{
    private readonly VariantDirectoryService _directories;
    private readonly CompositeBuildService _composite;

    public VariantLauncherService(VariantDirectoryService directories, CompositeBuildService composite)
    {
        _directories = directories ?? throw new ArgumentNullException(nameof(directories));
        _composite = composite ?? throw new ArgumentNullException(nameof(composite));
    }

    public VariantLaunchTarget Resolve(ProjectContext? project, VariantContext? variant)
    {
        if (project is null || variant is null)
            return new("None", 0, 0, string.Empty, string.Empty, string.Empty, string.Empty,
                VariantDirectoryOperationStatus.InvalidContext, false, VariantLaunchReadiness.NoActiveInstallation,
                "No game selected. Use Find games... or Browse folder...");
        if (!ReferenceEquals(project, variant.Project))
            return Invalid(project, variant, "VariantContext does not belong to the active ProjectContext.");
        string projectCode;
        try { projectCode = _composite.GetProjectVariantCode(project, variant); }
        catch (Exception ex) when (ex is ArgumentException or InvalidDataException)
        { return Invalid(project, variant, ex.Message); }
        return ResolveEdition(project, variant, projectCode);
    }

    /// <summary>Authoritative explicit runnable target:
    /// VARIANTS\&lt;RuntimeKey&gt;\&lt;EditionCode&gt;. Edition subdirectories
    /// coexist; the runtime root itself is never a runnable output.</summary>
    public VariantLaunchTarget ResolveEdition(ProjectContext? project, VariantContext? variant, string editionCode)
    {
        if (project is null || variant is null)
            return new("None", 0, 0, string.Empty, string.Empty, string.Empty, string.Empty,
                VariantDirectoryOperationStatus.InvalidContext, false, VariantLaunchReadiness.NoActiveInstallation,
                "No game selected. Use Find games... or Browse folder...");
        if (!ReferenceEquals(project, variant.Project))
            return Invalid(project, variant, "VariantContext does not belong to the active ProjectContext.");

        string normalizedCode;
        string root;
        try
        {
            normalizedCode = ProjectVariantOwnership.NormalizeCode(project, editionCode);
            root = _directories.GetVariantEditionDirectoryPath(project, variant, normalizedCode);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidDataException or InvalidOperationException)
        { return Invalid(project, variant, ex.Message); }
        VariantDirectoryOperationResult ownership = _directories.ValidateOwnedVariantEditionDirectory(project, variant, normalizedCode);
        if (ownership.Status == VariantDirectoryOperationStatus.NotFound)
            return ResolveMissingEdition(project, variant, normalizedCode);
        if (ownership.Status != VariantDirectoryOperationStatus.AlreadyValid)
            return Make(project, variant, root, ownership.Status, false, VariantLaunchReadiness.ForeignOrInvalidVariant,
                "Variant directory is not owned by this project/runtime: " + ownership.Status + ".");

        CompositeBuildPlan plan = _composite.CreatePlan(project, variant, CompositeBuildMode.Full);
        bool configured = plan.Capabilities.Count > 0 && plan.Capabilities.All(capability => capability.Configured);
        IReadOnlyList<string> artifacts = _composite.GetRuntimeArtifactNames(project, variant);
        string executableName = artifacts.Single(name => name.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase));
        string dataName = artifacts.Single(name => !name.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase));
        string output = Path.Combine(root, executableName);
        string data = Path.Combine(root, dataName);
        bool outputExists = File.Exists(output) && File.Exists(data);
        if (outputExists)
        {
            try
            {
                VariantManifest manifest = VariantManifestService.Read(root);
                if (!manifest.ProjectCode.Equals(normalizedCode, StringComparison.OrdinalIgnoreCase))
                    return Make(project, variant, root, ownership.Status, configured, VariantLaunchReadiness.BuildIncomplete,
                        "The generated output belongs to a different selected project variant.");
                // R9F V8.6d content-based freshness: the built artifact is stale
                // when current project inputs differ from the recorded build
                // provenance. This is a service-level check, not a UI label.
                try
                {
                    string current = ProjectBuildFingerprintService.Compute(project, variant, normalizedCode);
                    if (string.IsNullOrWhiteSpace(manifest.InputFingerprint) ||
                        !manifest.InputFingerprint.Equals(current, StringComparison.OrdinalIgnoreCase))
                        return Make(project, variant, root, ownership.Status, configured, VariantLaunchReadiness.BuildIncomplete,
                            "Project content changed since the last build; rebuild the variant to use the current project state.");
                }
                catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or InvalidDataException or IOException)
                {
                    return Make(project, variant, root, ownership.Status, configured, VariantLaunchReadiness.BuildIncomplete,
                        "Current project state is invalid; rebuild the variant to use the current project state.");
                }
                // R9D: the launch artifacts must still be byte-identical to the authorized
                // build recorded in the manifest, and the executable must retain a supported
                // structural identity. A post-build modification blocks readiness. An
                // executable problem blames the executable; a data-file problem names the
                // data artifact instead and never claims the executable is unsupported.
                SupportedExecutableClassification launched = SupportedExecutableIdentityService.Classify(variant.RuntimeKind, output);
                if (!launched.IsSupportedForBinaryUse || !MatchesRecordedRuntimeHash(manifest, root, executableName))
                    return Make(project, variant, root, ownership.Status, configured, VariantLaunchReadiness.BuildIncomplete,
                        SupportedExecutableIdentityService.DescribeBlocked(launched, "run/debug"));
                if (!MatchesRecordedRuntimeHash(manifest, root, dataName))
                    return Make(project, variant, root, ownership.Status, configured, VariantLaunchReadiness.BuildIncomplete,
                        SupportedExecutableIdentityService.DescribeArtifactMismatch(variant.RuntimeKind, dataName, "run/debug"));
            }
            catch (Exception ex) when (ex is IOException or JsonException or InvalidDataException)
            {
                return Make(project, variant, root, ownership.Status, configured, VariantLaunchReadiness.BuildIncomplete,
                    "The generated output has no valid project-identity manifest.");
            }
        }
        if (!configured || !outputExists)
            return Make(project, variant, root, ownership.Status, configured, VariantLaunchReadiness.BuildIncomplete,
                !configured ? "Full build is not configured." : "Required generated executable or data artifact is missing.");
        return Make(project, variant, root, ownership.Status, true, VariantLaunchReadiness.LaunchReady, "Authorized generated variant output is present.", executableName, dataName);
    }

    public LauncherRedirectionPlan CreateRedirectionPlan(ProjectContext project, VariantContext variant)
    {
        VariantLaunchTarget target = Resolve(project, variant);
        string launcher = project.GameProfile == ElviraGameProfile.Elvira1 ? "ELVIRA.BAT" : "CERBERUS.BAT";
        PristineManifestFile? manifestFile = project.PristineManifest.Files.SingleOrDefault(file => file.RelativePath.Equals(launcher, StringComparison.OrdinalIgnoreCase));
        string active = Path.Combine(project.GameRoot, launcher);
        string restore = Path.Combine(project.StorageLayout.MutableBackupsRoot, launcher);
        bool classified = manifestFile?.Classification == PristineFileClassification.MutableBackedUp;
        bool backup = classified && File.Exists(restore);
        bool authorized = target.Readiness == VariantLaunchReadiness.LaunchReady && classified && backup;
        string detail = !classified ? "Launcher is not declared MutableBackedUp in the pristine manifest."
            : !backup ? "Pristine mutable launcher backup is missing."
            : !authorized ? "Variant is not launch-ready."
            : "Future launcher redirection is authorized; R6Q does not write it.";
        return new(launcher, active, restore, manifestFile?.Classification ?? PristineFileClassification.Immutable, target, authorized, detail);
    }

    /// <summary>Explicit-edition redirection plan. Same identity as
    /// ResolveEdition; the implicit overload resolves the composite code.</summary>
    public LauncherRedirectionPlan CreateRedirectionPlan(ProjectContext project, VariantContext variant, string editionCode)
    {
        VariantLaunchTarget target = ResolveEdition(project, variant, editionCode);
        string launcher = project.GameProfile == ElviraGameProfile.Elvira1 ? "ELVIRA.BAT" : "CERBERUS.BAT";
        PristineManifestFile? manifestFile = project.PristineManifest.Files.SingleOrDefault(file => file.RelativePath.Equals(launcher, StringComparison.OrdinalIgnoreCase));
        string active = Path.Combine(project.GameRoot, launcher);
        string restore = Path.Combine(project.StorageLayout.MutableBackupsRoot, launcher);
        bool classified = manifestFile?.Classification == PristineFileClassification.MutableBackedUp;
        bool backup = classified && File.Exists(restore);
        bool authorized = target.Readiness == VariantLaunchReadiness.LaunchReady && classified && backup;
        string detail = !classified ? "Launcher is not declared MutableBackedUp in the pristine manifest."
            : !backup ? "Pristine mutable launcher backup is missing."
            : !authorized ? "Variant is not launch-ready."
            : "Future launcher redirection is authorized; R6Q does not write it.";
        return new(launcher, active, restore, manifestFile?.Classification ?? PristineFileClassification.Immutable, target, authorized, detail);
    }

    private VariantLaunchTarget ResolveMissingEdition(ProjectContext project, VariantContext variant, string projectCode)
    {
        string root;
        try { root = _directories.GetVariantDirectoryPath(project, variant); }
        catch (Exception ex) when (ex is ArgumentException or InvalidDataException)
        { return Invalid(project, variant, ex.Message); }
        string editionRoot = Path.Combine(root, projectCode);
        if (!Directory.Exists(root))
            return Make(project, variant, editionRoot, VariantDirectoryOperationStatus.NotFound, false, VariantLaunchReadiness.VariantMissing, "Owned variant directory is missing.");
        // A present runtime root without this edition simply means this
        // edition was never built. Loose files directly in the runtime root
        // are not valid v1.0 owned variants: never launched, never deleted.
        if (Directory.EnumerateFiles(root, "*", SearchOption.TopDirectoryOnly).Any(path =>
            {
                string name = Path.GetFileName(path);
                return !name.Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase) &&
                    !name.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase);
            }))
            return Make(project, variant, editionRoot, VariantDirectoryOperationStatus.ForeignDirectoryConflict, false, VariantLaunchReadiness.ForeignOrInvalidVariant,
                "VARIANTS\\" + variant.DirectoryKey + " holds files that are not editor-owned outputs. They were left untouched.");
        return Make(project, variant, editionRoot, VariantDirectoryOperationStatus.NotFound, false, VariantLaunchReadiness.BuildIncomplete,
            "No built output for edition " + projectCode + "; build required.");
    }

    private static VariantLaunchTarget Invalid(ProjectContext project, VariantContext variant, string detail) =>
        Make(project, variant, string.Empty, VariantDirectoryOperationStatus.InvalidContext, false, VariantLaunchReadiness.ForeignOrInvalidVariant, detail);

    /// <summary>
    /// R9D provenance check: the file must still be byte-identical to the authorized
    /// build recorded in the variant manifest. Any post-build modification fails closed.
    /// </summary>
    private static bool MatchesRecordedRuntimeHash(VariantManifest manifest, string root, string fileName)
    {
        VariantManifestRuntimeArtifact? recorded = manifest.RuntimeArtifacts
            .SingleOrDefault(artifact => artifact.RelativePath.Equals(fileName, StringComparison.OrdinalIgnoreCase));
        if (recorded is not { Present: true } || recorded.Size <= 0 || string.IsNullOrWhiteSpace(recorded.Sha256))
            return false;
        string path = Path.Combine(root, fileName);
        try
        {
            if (new FileInfo(path).Length != recorded.Size)
                return false;
            using FileStream stream = File.OpenRead(path);
            return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)).Equals(recorded.Sha256, StringComparison.OrdinalIgnoreCase);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static VariantLaunchTarget Make(ProjectContext project, VariantContext variant, string root, VariantDirectoryOperationStatus ownership,
        bool configured, VariantLaunchReadiness readiness, string detail, string? executableName = null, string? dataName = null) => new(
            PristineManifestService.GameIdFor(project.GameProfile), variant.VariantId, variant.RuntimeKind, root, root,
            executableName ?? variant.GeneratedExecutableName, dataName ?? variant.LogicalDataFileName, ownership, configured, readiness, detail);
}
