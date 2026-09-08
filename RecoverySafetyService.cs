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
        // Exactly one edition is removed; siblings coexist untouched. When no
        // edition output exists but legacy flat output does, the explicit
        // button press authorizes its removal (nothing is ever deleted silently).
        VariantDirectoryOperationResult edition = _directories.ValidateOwnedVariantEditionDirectory(project, variant, projectCode);
        if (edition.Status == VariantDirectoryOperationStatus.AlreadyValid)
        {
            VariantDirectoryOperationResult result = _directories.DeleteVariantEditionDirectory(project, variant, projectCode);
            return result.Status == VariantDirectoryOperationStatus.Deleted
                ? new(true, "Editor-owned runtime+edition output removed.", result)
                : new(false, "Variant edition directory was not removed: " + result.Status + ".", result);
        }
        if (edition.Status != VariantDirectoryOperationStatus.NotFound)
            return new(false, "Variant edition directory was not removed: " + edition.Status + ".", edition);
        VariantDirectoryService.VariantLegacyFlatInfo legacy = _directories.DetectLegacyFlatVariant(project, variant);
        if (!legacy.HasLegacyFiles && !legacy.HasMatchingMarker)
            return new(false, "Variant directory was not removed: " + VariantDirectoryOperationStatus.NotFound + ".", edition);
        if (legacy.HasOwnedEditions)
            return new(false, "Legacy flat output shares its runtime root with edition outputs; remove the editions first.", edition);
        VariantDirectoryOperationResult removed = _directories.DeleteLegacyFlatVariantDirectory(project, variant);
        return removed.Status == VariantDirectoryOperationStatus.Deleted
            ? new(true, "Legacy flat owned variant output removed explicitly.", removed)
            : new(false, "Legacy flat output was not removed: " + removed.Status + ".", removed);
    }

    public RecoverySafetyOperationResult RebuildOwnedVariant(ProjectContext project, VariantContext variant, string projectCode)
    {
        // Ensure-then-build: the button owns its target directory, so a first
        // build works without a manual directory step. Ensure never copies
        // payload and never touches sibling editions or legacy flat outputs.
        VariantDirectoryOperationResult ensured = _directories.EnsureVariantEditionDirectory(project, variant, projectCode);
        if (ensured.Status != VariantDirectoryOperationStatus.Created && ensured.Status != VariantDirectoryOperationStatus.AlreadyValid)
            return new(false, "Variant rebuild requires an editor-owned runtime+edition directory: " + ensured.Status + ".", ensured);
        CompositeBuildResult build = _composite.Build(project, variant, CompositeBuildMode.Full);
        return build.Status == CompositeBuildStatus.Success
            ? new(true, "Owned variant rebuilt.", ensured, build)
            : new(false, "Variant rebuild did not complete: " + build.Status + ".", ensured, build);
    }

    /// <summary>Read-only adoption census for legacy flat owned outputs
    /// against one explicit target edition. Verifies marker provenance,
    /// manifest identity (including the expected project code), and every
    /// payload byte without moving, deleting or creating anything.</summary>
    public RecoverySafetyOperationResult VerifyLegacyAdoption(ProjectContext project, VariantContext variant, string projectCode)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(variant);
        LegacyAdoptionPlan? plan = InspectLegacyAdoption(project, variant, projectCode, out string detail);
        return plan is null ? new(false, detail) : new(true, detail);
    }

    /// <summary>Controlled, explicit adoption of a proven legacy flat owned
    /// output into its runtime+edition directory. All-or-nothing: every
    /// manifest-listed file is hash-verified BEFORE anything moves; unknown
    /// files, hash mismatches, missing files, an existing edition output,
    /// an unproven marker, or a manifest code different from the selected
    /// edition abort with everything left untouched. Verified
    /// files (payload + manifest) are moved — never copied — into the
    /// edition directory, the legacy root marker is removed, and the root is
    /// pruned when emptied. Stale editor *.tmp leftovers are removed with
    /// the emptied root only. Genuine foreign contents are never adopted.</summary>
    public RecoverySafetyOperationResult AdoptLegacyFlatVariant(ProjectContext project, VariantContext variant, string projectCode)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(variant);
        LegacyAdoptionPlan? plan = InspectLegacyAdoption(project, variant, projectCode, out string inspection);
        if (plan is null) return new(false, inspection);
        VariantDirectoryOperationResult ensured = _directories.EnsureVariantEditionDirectory(project, variant, plan.ProjectCode);
        if (ensured.Status != VariantDirectoryOperationStatus.Created)
            return new(false, "Legacy adoption refused: the edition output appeared concurrently: " + ensured.Status + ".", ensured);
        try
        {
            foreach ((string relative, string source) in plan.PayloadFiles)
            {
                string destination = StrictChild(plan.EditionRoot, relative);
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                if (File.Exists(destination))
                    throw new InvalidDataException("Legacy adoption refused: edition target is not empty: " + relative);
                File.Move(source, destination);
            }
            string manifestDestination = Path.Combine(plan.EditionRoot, VariantManifestService.FileName);
            if (File.Exists(manifestDestination))
                throw new InvalidDataException("Legacy adoption refused: edition manifest already exists.");
            File.Move(plan.ManifestPath, manifestDestination);
            File.Delete(Path.Combine(plan.RuntimeRoot, VariantDirectoryService.OwnershipMarkerFileName));
            PruneAdoptedRoot(plan.RuntimeRoot);
            VariantDirectoryOperationResult owned = _directories.ValidateOwnedVariantEditionDirectory(project, variant, plan.ProjectCode);
            if (owned.Status != VariantDirectoryOperationStatus.AlreadyValid)
                throw new InvalidDataException("Adopted edition output did not validate: " + owned.Status + ".");
            VariantManifest reread = VariantManifestService.Read(plan.EditionRoot);
            if (!reread.ProjectCode.Equals(plan.ProjectCode, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Adopted manifest identity diverged after the move.");
            return new(true, $"Legacy flat output adopted into VARIANTS\\{variant.DirectoryKey}\\{plan.ProjectCode}: {plan.PayloadFiles.Count} files verified and moved.", owned);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        {
            return new(false, "Legacy adoption stopped, earlier moves were preserved in place and nothing was deleted: " + ex.Message + " " + inspection, ensured);
        }
    }

    private sealed record LegacyAdoptionPlan(
        string RuntimeRoot,
        string EditionRoot,
        string ProjectCode,
        string ManifestPath,
        IReadOnlyList<(string Relative, string Source)> PayloadFiles);

    /// <summary>Shared verification phase (also used read-only by Verify).
    /// Returns null with a precise reason unless provenance is fully proven
    /// for the explicitly selected target edition: matching schema-1 root
    /// marker, matching manifest identity whose project code equals the
    /// selected edition, absent edition target, every on-disk file
    /// manifest-known, every manifest file present with size+SHA-256 intact.
    /// Coexisting owned editions for OTHER codes do not block adoption;
    /// plain game-content subdirectories are payload, never siblings.</summary>
    private LegacyAdoptionPlan? InspectLegacyAdoption(ProjectContext project, VariantContext variant, string projectCode, out string detail)
    {
        detail = string.Empty;
        string expectedCode;
        try { expectedCode = ProjectVariantOwnership.NormalizeCode(project, projectCode); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            detail = "Legacy adoption refused: the selected edition is not valid.";
            return null;
        }
        VariantDirectoryService.VariantLegacyFlatInfo legacy = _directories.DetectLegacyFlatVariant(project, variant);
        if (!legacy.HasLegacyFiles || !legacy.HasMatchingMarker)
        {
            detail = !legacy.HasLegacyFiles
                ? "No legacy flat output to adopt."
                : "Legacy adoption refused: the runtime root marker does not match this project/runtime (foreign contents stay untouched).";
            return null;
        }
        string runtimeRoot = _directories.GetVariantDirectoryPath(project, variant);
        VariantManifest manifest;
        try { manifest = VariantManifestService.Read(runtimeRoot); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or InvalidDataException)
        {
            detail = "Legacy adoption refused: no readable variant manifest proves provenance.";
            return null;
        }
        if (!manifest.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal) ||
            !manifest.VariantId.Equals(variant.VariantId.ToString(), StringComparison.Ordinal) ||
            !manifest.RuntimeKind.Equals(variant.RuntimeKind.ToString(), StringComparison.Ordinal) ||
            !manifest.DirectoryKey.Equals(variant.DirectoryKey, StringComparison.Ordinal) ||
            !manifest.BaselineFingerprint.Equals(project.BaselineFingerprint, StringComparison.Ordinal))
        {
            detail = "Legacy adoption refused: the manifest identity does not prove this project/runtime/baseline.";
            return null;
        }
        string code;
        try { code = ProjectVariantOwnership.NormalizeCode(project, manifest.ProjectCode); }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            detail = "Legacy adoption refused: the manifest project code is not a valid edition.";
            return null;
        }
        if (!code.Equals(expectedCode, StringComparison.Ordinal))
        {
            detail = "Legacy adoption refused: the manifest project code '" + code + "' does not match the selected edition '" + expectedCode + "'.";
            return null;
        }
        string editionRoot = _directories.GetVariantEditionDirectoryPath(project, variant, code);
        if (Directory.Exists(editionRoot))
        {
            detail = "Legacy adoption refused: the edition output already exists.";
            return null;
        }
        var expected = manifest.OutputArtifacts.Select(artifact => artifact.RelativePath.Replace('/', Path.DirectorySeparatorChar)).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var onDisk = new List<(string Relative, string Full)>();
        foreach (string file in Directory.EnumerateFiles(runtimeRoot, "*", SearchOption.AllDirectories))
        {
            string name = Path.GetFileName(file);
            string relative = Path.GetRelativePath(runtimeRoot, file);
            if (relative.Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)) continue;
            if (relative.Equals(VariantManifestService.FileName, StringComparison.OrdinalIgnoreCase)) continue;
            if (name.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase)) continue;
            onDisk.Add((relative, file));
        }
        string? unknown = onDisk.Select(entry => entry.Relative).FirstOrDefault(relative => !expected.Contains(relative));
        if (unknown is not null)
        {
            detail = "Legacy adoption refused: unknown file is not manifest-listed: " + unknown;
            return null;
        }
        var payload = new List<(string Relative, string Source)>();
        foreach (VariantManifestArtifact artifact in manifest.OutputArtifacts)
        {
            string relative = artifact.RelativePath.Replace('/', Path.DirectorySeparatorChar);
            string source = StrictChild(runtimeRoot, relative);
            if (!File.Exists(source))
            {
                detail = "Legacy adoption refused: manifest-listed file is missing: " + artifact.RelativePath;
                return null;
            }
            if (new FileInfo(source).Length != artifact.Size || !HashFile(source).Equals(artifact.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                detail = "Legacy adoption refused: payload hash mismatch: " + artifact.RelativePath;
                return null;
            }
            payload.Add((relative, source));
        }
        foreach (VariantManifestRuntimeArtifact runtime in manifest.RuntimeArtifacts)
        {
            if (!runtime.Present || runtime.Size <= 0 || string.IsNullOrWhiteSpace(runtime.Sha256) ||
                !expected.Contains(runtime.RelativePath.Replace('/', Path.DirectorySeparatorChar)))
            {
                detail = "Legacy adoption refused: the manifest runtime artifacts are inconsistent.";
                return null;
            }
        }
        payload.Sort((left, right) => StringComparer.OrdinalIgnoreCase.Compare(left.Relative, right.Relative));
        detail = $"Adoptable legacy flat output at VARIANTS\\{variant.DirectoryKey} (edition {code}): {payload.Count} files verified.";
        return new(runtimeRoot, editionRoot, code, Path.Combine(runtimeRoot, VariantManifestService.FileName), payload);
    }

    private static string StrictChild(string parent, string relative)
    {
        string normalizedParent = Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string full = Path.GetFullPath(Path.Combine(parent, relative));
        if (!full.StartsWith(normalizedParent, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Adoption path escaped its validated root.");
        return full;
    }

    private static void PruneAdoptedRoot(string runtimeRoot)
    {
        try
        {
            // Stale editor temps are removed at every level; directory
            // skeleton left behind by the move is removed bottom-up. Anything
            // else (never the case after a verified census) keeps the root.
            foreach (string tmp in Directory.EnumerateFiles(runtimeRoot, "*.tmp", SearchOption.AllDirectories))
                File.Delete(tmp);
            PruneEmptyDirectories(runtimeRoot);
            if (!Directory.EnumerateFileSystemEntries(runtimeRoot).Any()) Directory.Delete(runtimeRoot, false);
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    private static void PruneEmptyDirectories(string directory)
    {
        foreach (string child in Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly))
        {
            PruneEmptyDirectories(child);
            try
            {
                if (!Directory.EnumerateFileSystemEntries(child).Any()) Directory.Delete(child, false);
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
        }
    }

    private static string HashFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream));
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
            else if (directory.Classification == VariantDirectoryClassification.LegacyFlatOwned)
                items.Add(new(RestorePlanItemKind.ExternalDifferencePreserved, EditorStorageLayout.VariantsDirectoryName + "\\" + directory.DirectoryKey, "Legacy flat owned output is preserved; remove it explicitly via Recovery, never silently."));
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
