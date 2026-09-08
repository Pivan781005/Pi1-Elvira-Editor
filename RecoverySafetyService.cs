namespace Pi1ElviraEditor;

internal sealed record RecoverySafetyInspection(
    BaselineValidationResult Baseline,
    IReadOnlyList<VariantDirectoryInfo> Variants,
    RecoveryReversePreview ReversePreview);

internal sealed record RecoveryReversePreview(
    BaselineValidationResult Baseline,
    IReadOnlyList<VariantDirectoryInfo> OwnedVariants,
    IReadOnlyList<VariantDirectoryInfo> ForeignOrInvalidVariants,
    string LauncherBackupPath,
    bool LauncherBackupAvailable,
    string Detail);

internal sealed record RecoverySafetyOperationResult(bool Succeeded, string Detail, VariantDirectoryOperationResult? VariantResult = null, CompositeBuildResult? BuildResult = null);

internal enum RestorePlanItemKind { RestoreMutable, RemoveOwnedVariant, RemoveKnownEditorArtifact, ExternalDifferencePreserved, NoAction, Blocked }
internal sealed record RestorePlanItem(RestorePlanItemKind Kind, string RelativePath, string Detail, BuiltInVariantId? VariantId = null, string? ProjectCode = null);
internal sealed record RestorePlan(string GameRoot, string BaselineFingerprint, IReadOnlyList<RestorePlanItem> Items)
{
    public bool IsBlocked => Items.Any(item => item.Kind == RestorePlanItemKind.Blocked);
    public int ActionCount => Items.Count(item => item.Kind is RestorePlanItemKind.RestoreMutable or RestorePlanItemKind.RemoveOwnedVariant or RestorePlanItemKind.RemoveKnownEditorArtifact);
}
internal sealed record RestorePlanExecutionResult(bool Succeeded, string Detail, RestorePlan Plan);

/// <summary>Explicit Recovery & Safety operations. This service never creates
/// or recaptures a pristine baseline and never adopts foreign VARIANTS entries.
/// R6T restore execution consumes only a freshly revalidated preview plan.</summary>
internal sealed class RecoverySafetyService
{
    private static readonly HashSet<string> RemovableKnownEditorArtifacts = new(StringComparer.OrdinalIgnoreCase)
    {
        "PI1MENU.COM", "PI1SND.BAT", "ELVIRA_MODS.INI", "ELVIRA.BAK", "ELVOVR.BAK", "TMP.TXT"
    };
    private readonly VariantDirectoryService _directories;
    private readonly CompositeBuildService _composite;

    public RecoverySafetyService(VariantDirectoryService directories, CompositeBuildService composite)
    {
        _directories = directories ?? throw new ArgumentNullException(nameof(directories));
        _composite = composite ?? throw new ArgumentNullException(nameof(composite));
    }

    public RecoverySafetyInspection Inspect(ProjectContext project)
    {
        ArgumentNullException.ThrowIfNull(project);
        BaselineValidationResult baseline = new PristineManifestService(project.StorageLayout, project.GameProfile).ValidateBaseline();
        IReadOnlyList<VariantDirectoryInfo> variants = _directories.Enumerate(project, VariantContextCatalog.CreateBuiltIns(project));
        return new(baseline, variants, CreateReversePreview(project, baseline, variants));
    }

    public RecoverySafetyOperationResult RestoreOriginalLauncher(ProjectContext project)
    {
        ArgumentNullException.ThrowIfNull(project);
        string launcher = LauncherName(project);
        PristineManifestFile? declared = project.PristineManifest.Files.SingleOrDefault(file => file.RelativePath.Equals(launcher, StringComparison.OrdinalIgnoreCase));
        if (declared?.Classification != PristineFileClassification.MutableBackedUp)
            return new(false, "The active launcher is not declared MutableBackedUp by the pristine manifest.");
        string source = Path.Combine(project.StorageLayout.MutableBackupsRoot, launcher);
        string destination = Path.Combine(project.GameRoot, launcher);
        if (!File.Exists(source)) return new(false, "Baseline mutable launcher backup is missing.");
        if (!IsStrictChild(source, project.StorageLayout.MutableBackupsRoot) || !IsStrictChild(destination, project.GameRoot))
            return new(false, "Launcher restore path is invalid.");
        string temporary = Path.Combine(project.GameRoot, ".pi1-restore-" + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.Copy(source, temporary, overwrite: false);
            File.Move(temporary, destination, overwrite: true);
            return new(true, "Original launcher restored from Baseline\\Mutable.");
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return new(false, "Launcher restore failed: " + ex.Message);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    public RecoverySafetyOperationResult RemoveOwnedVariant(ProjectContext project, VariantContext variant, string projectCode)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(variant);
        // Exactly one edition is removed; siblings coexist untouched.
        VariantDirectoryOperationResult edition = _directories.ValidateOwnedVariantEditionDirectory(project, variant, projectCode);
        if (edition.Status == VariantDirectoryOperationStatus.AlreadyValid)
        {
            VariantDirectoryOperationResult result = _directories.DeleteVariantEditionDirectory(project, variant, projectCode);
            return result.Status == VariantDirectoryOperationStatus.Deleted
                ? new(true, "Editor-owned runtime+edition output removed.", result)
                : new(false, "Variant edition directory was not removed: " + result.Status + ".", result);
        }
        return new(false, "Variant edition directory was not removed: " + edition.Status + ".", edition);
    }

    public RecoverySafetyOperationResult RebuildOwnedVariant(ProjectContext project, VariantContext variant, string projectCode)
    {
        // Ensure-then-build: the button owns its target directory, so a first
        // build works without a manual directory step. Ensure never copies
        // payload and never touches sibling editions or foreign files.
        VariantDirectoryOperationResult ensured = _directories.EnsureVariantEditionDirectory(project, variant, projectCode);
        if (ensured.Status != VariantDirectoryOperationStatus.Created && ensured.Status != VariantDirectoryOperationStatus.AlreadyValid)
            return new(false, "Variant rebuild requires an editor-owned runtime+edition directory: " + ensured.Status + ".", ensured);
        CompositeBuildResult build = _composite.Build(project, variant, CompositeBuildMode.Full);
        return build.Status == CompositeBuildStatus.Success
            ? new(true, "Owned variant rebuilt.", ensured, build)
            : new(false, "Variant rebuild did not complete: " + build.Status + ".", ensured, build);
    }

    /// <summary>Builds the complete non-mutating R6T plan. A changed mutable
    /// file is actionable only when its declared baseline backup is present
    /// and byte-identical to the manifest; all unrelated changes are retained.
    /// </summary>
    public RestorePlan CreateRestorePlan(ProjectContext project)
    {
        ArgumentNullException.ThrowIfNull(project);
        var items = new List<RestorePlanItem>();
        var manifest = new PristineManifestService(project.StorageLayout, project.GameProfile);
        BaselineValidationResult validation = manifest.ValidateBaseline();
        var entries = validation.Entries.ToDictionary(item => item.RelativePath, StringComparer.OrdinalIgnoreCase);
        foreach (PristineManifestFile declared in project.PristineManifest.Files.OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase))
        {
            entries.TryGetValue(declared.RelativePath, out BaselineValidationEntry? state);
            if (declared.Classification == PristineFileClassification.MutableBackedUp && state?.Status == BaselineValidationStatus.CurrentDiffersFromBaseline)
            {
                string backup = Path.Combine(project.StorageLayout.MutableBackupsRoot, declared.RelativePath);
                items.Add(ValidMutableBackup(backup, declared)
                    ? new(RestorePlanItemKind.RestoreMutable, declared.RelativePath, "Restore from Baseline\\Mutable.")
                    : new(RestorePlanItemKind.Blocked, declared.RelativePath, "Declared mutable backup is missing or corrupt."));
            }
            else if (state is null || state.Status == BaselineValidationStatus.MatchesBaseline)
                items.Add(new(RestorePlanItemKind.NoAction, declared.RelativePath, "Matches pristine baseline."));
            else
                items.Add(new(RestorePlanItemKind.ExternalDifferencePreserved, declared.RelativePath, "External immutable/pristine difference is preserved."));
        }
        foreach (BaselineValidationEntry unexpected in validation.Entries
            .Where(entry => !project.PristineManifest.Files.Any(file => file.RelativePath.Equals(entry.RelativePath, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(entry => entry.RelativePath, StringComparer.OrdinalIgnoreCase))
        {
            items.Add(new(RestorePlanItemKind.ExternalDifferencePreserved, unexpected.RelativePath, "Unexpected external file is preserved."));
        }

        foreach (PristineInstallationFile file in new PristineInstallationService(project.StorageLayout).EnumerateReadOnly()
            .Where(file => file.Classification == PristineFileClassification.GeneratedIgnored)
            .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase))
        {
            if (IsNarrowRemovableEditorArtifact(file.RelativePath))
                items.Add(new(RestorePlanItemKind.RemoveKnownEditorArtifact, file.RelativePath, "Recognized editor-generated root artifact."));
            else
                items.Add(new(RestorePlanItemKind.NoAction, file.RelativePath, "GeneratedIgnored policy does not authorize removal."));
        }

        IReadOnlyList<VariantContext> known = VariantContextCatalog.CreateBuiltIns(project);
        foreach (VariantDirectoryInfo directory in _directories.Enumerate(project, known).OrderBy(item => item.DirectoryKey, StringComparer.OrdinalIgnoreCase).ThenBy(item => item.ProjectCode, StringComparer.Ordinal))
        {
            if (directory.Classification == VariantDirectoryClassification.KnownOwnedVariant && directory.VariantId is { } variantId)
            {
                string relative = EditorStorageLayout.VariantsDirectoryName + "\\" + directory.DirectoryKey
                    + (directory.ProjectCode is null ? string.Empty : "\\" + directory.ProjectCode);
                items.Add(directory.ProjectCode is null
                    ? new(RestorePlanItemKind.RemoveOwnedVariant, relative, "Marker-validated editor-owned runtime root.", variantId)
                    : new(RestorePlanItemKind.RemoveOwnedVariant, relative, "Marker-validated editor-owned runtime+edition output.", variantId, directory.ProjectCode));
            }
            else
                items.Add(new(RestorePlanItemKind.ExternalDifferencePreserved, EditorStorageLayout.VariantsDirectoryName + "\\" + directory.DirectoryKey, "Foreign or invalid variant is preserved."));
        }
        return new(project.GameRoot, project.BaselineFingerprint, items.OrderBy(item => item.RelativePath, StringComparer.OrdinalIgnoreCase).ThenBy(item => item.Kind).ToArray());
    }

    /// <summary>Executes only a previously previewed, still-identical plan.
    /// It performs a fresh plan comparison before the first write so changed
    /// state or a corrupt mutable backup cannot cause partial guessing.</summary>
    public RestorePlanExecutionResult ExecuteRestorePlan(ProjectContext project, RestorePlan preview)
    {
        ArgumentNullException.ThrowIfNull(project); ArgumentNullException.ThrowIfNull(preview);
        RestorePlan current = CreateRestorePlan(project);
        if (!preview.GameRoot.Equals(project.GameRoot, StringComparison.OrdinalIgnoreCase) || preview.BaselineFingerprint != project.BaselineFingerprint || !SamePlan(preview, current))
            return new(false, "Restore preview is stale; review the current plan before executing.", current);
        if (preview.IsBlocked) return new(false, "Restore is blocked; declared mutable backup integrity must be repaired manually.", preview);

        IReadOnlyDictionary<BuiltInVariantId, VariantContext> variants = VariantContextCatalog.CreateBuiltIns(project).ToDictionary(variant => variant.VariantId);
        foreach (RestorePlanItem item in preview.Items)
        {
            switch (item.Kind)
            {
                case RestorePlanItemKind.RestoreMutable:
                    RestoreDeclaredMutable(project, item.RelativePath);
                    break;
                case RestorePlanItemKind.RemoveKnownEditorArtifact:
                    DeleteKnownEditorArtifact(project, item.RelativePath);
                    break;
                case RestorePlanItemKind.RemoveOwnedVariant:
                    if (item.VariantId is not { } id || !variants.TryGetValue(id, out VariantContext? variant))
                        return new(false, "Restore plan has an invalid variant identity.", preview);
                    VariantDirectoryOperationResult deleted = item.ProjectCode is null
                        ? _directories.DeleteVariantDirectory(project, variant)
                        : _directories.DeleteVariantEditionDirectory(project, variant, item.ProjectCode);
                    if (deleted.Status != VariantDirectoryOperationStatus.Deleted)
                        return new(false, "Owned variant removal stopped: " + deleted.Status + ".", preview);
                    break;
            }
        }
        return new(true, preview.ActionCount == 0 ? "No restore actions were required." : "Planned editor-owned restore actions completed.", CreateRestorePlan(project));
    }

    private RecoveryReversePreview CreateReversePreview(ProjectContext project, BaselineValidationResult baseline, IReadOnlyList<VariantDirectoryInfo> variants)
    {
        string backup = Path.Combine(project.StorageLayout.MutableBackupsRoot, LauncherName(project));
        VariantDirectoryInfo[] owned = variants.Where(item => item.Classification == VariantDirectoryClassification.KnownOwnedVariant).ToArray();
        VariantDirectoryInfo[] foreign = variants.Where(item => item.Classification != VariantDirectoryClassification.KnownOwnedVariant).ToArray();
        string detail = baseline.Status != BaselineValidationStatus.MatchesBaseline
            ? "Preview only: external/pristine differences must be reviewed; nothing will be deleted."
            : "Preview only: R6T owns final restore semantics.";
        return new(baseline, owned, foreign, backup, File.Exists(backup), detail);
    }

    private static string LauncherName(ProjectContext project) => project.GameProfile == ElviraGameProfile.Elvira1 ? "ELVIRA.BAT" : "CERBERUS.BAT";

    private static bool IsNarrowRemovableEditorArtifact(string relative) =>
        !relative.Contains('\\') && RemovableKnownEditorArtifacts.Contains(relative);

    private static bool ValidMutableBackup(string backup, PristineManifestFile declared)
    {
        if (!File.Exists(backup) || new FileInfo(backup).Length != declared.Size) return false;
        using FileStream stream = File.OpenRead(backup);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)).Equals(declared.Sha256, StringComparison.OrdinalIgnoreCase);
    }

    private static void RestoreDeclaredMutable(ProjectContext project, string relative)
    {
        PristineManifestFile declared = project.PristineManifest.Files.Single(file => file.RelativePath.Equals(relative, StringComparison.OrdinalIgnoreCase) && file.Classification == PristineFileClassification.MutableBackedUp);
        string source = Path.Combine(project.StorageLayout.MutableBackupsRoot, relative);
        if (!ValidMutableBackup(source, declared)) throw new InvalidDataException("Mutable baseline backup changed after preview.");
        string target = Path.Combine(project.GameRoot, relative);
        if (!IsStrictChild(source, project.StorageLayout.MutableBackupsRoot) || !IsStrictChild(target, project.GameRoot)) throw new InvalidDataException("Mutable restore path is invalid.");
        string temporary = Path.Combine(project.GameRoot, ".pi1-r6t-" + Guid.NewGuid().ToString("N") + ".tmp");
        try { File.Copy(source, temporary, false); File.Move(temporary, target, true); }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    private static void DeleteKnownEditorArtifact(ProjectContext project, string relative)
    {
        if (!IsNarrowRemovableEditorArtifact(relative)) throw new InvalidDataException("Artifact removal is not authorized by policy.");
        string target = Path.Combine(project.GameRoot, relative);
        if (!IsStrictChild(target, project.GameRoot)) throw new InvalidDataException("Artifact path escaped game root.");
        if (File.Exists(target)) File.Delete(target);
    }

    private static bool SamePlan(RestorePlan left, RestorePlan right) =>
        left.Items.Count == right.Items.Count && left.Items.Zip(right.Items).All(pair => pair.First == pair.Second);

    private static bool IsStrictChild(string path, string parent)
    {
        string normalizedParent = Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return Path.GetFullPath(path).StartsWith(normalizedParent, StringComparison.OrdinalIgnoreCase);
    }
}
