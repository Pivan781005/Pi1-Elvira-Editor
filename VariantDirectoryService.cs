using System.Text.Json;

namespace Pi1ElviraEditor;

internal enum VariantDirectoryOperationStatus
{
    Created,
    AlreadyValid,
    Cleared,
    Deleted,
    NotFound,
    ForeignDirectoryConflict,
    OwnershipConflict,
    PathConflict,
    ReparsePointConflict,
    InvalidContext
}

internal enum VariantDirectoryClassification
{
    KnownOwnedVariant,
    ForeignDirectory,
    OwnershipConflict,
    InvalidDirectoryName,
    ReparsePointConflict,
    LegacyFlatOwned
}

internal sealed record VariantDirectoryOperationResult(VariantDirectoryOperationStatus Status, string Path);
internal sealed record VariantDirectoryInfo(string Path, string DirectoryKey, VariantDirectoryClassification Classification, BuiltInVariantId? VariantId = null, string? ProjectCode = null);

/// <summary>
/// Minimal safety marker, not a build/variant manifest. It contains no path,
/// timestamp, payload data, or mutable build state. Schema 1 marks a runtime
/// root; schema 2 additionally binds one edition (project code).
/// </summary>
internal sealed record VariantDirectoryOwnershipMarker(
    int SchemaVersion,
    string Product,
    string GameId,
    string VariantId,
    string DirectoryKey,
    string BaselineFingerprint,
    string ProjectCode = "");

/// <summary>
/// Explicit lifecycle for empty editor-owned directories immediately below
/// VARIANTS. It never copies game payload and never accepts caller paths.
///
/// R9F V3 runnable identity is Installation + Project + Edition + Runtime:
/// each (runtime, edition) pair owns VARIANTS\&lt;RuntimeKey&gt;\&lt;EditionCode&gt;
/// (for example VARIANTS\E1VGA\SK). SK and S1 builds for the same runtime
/// coexist; building one edition never deletes another. The runtime root
/// itself stays a plain container. Pre-R9F flat outputs (files directly in
/// VARIANTS\E1VGA) are detected as legacy, never reinterpreted, and never
/// silently deleted: they require explicit removal.
/// </summary>
internal sealed class VariantDirectoryService
{
    internal const string OwnershipMarkerFileName = ".pi1-variant-owner.json";
    internal const int OwnershipMarkerSchema = 1;
    internal const int EditionOwnershipMarkerSchema = 2;
    private const string OwnershipMarkerProduct = "Pi1ElviraVariantDirectory";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public string GetVariantDirectoryPath(ProjectContext project, VariantContext variant)
    {
        ValidateAssociation(project, variant);
        string key = VariantContext.ValidateDirectoryKey(variant.DirectoryKey);
        if (!key.Equals(variant.DirectoryKey, StringComparison.Ordinal))
            throw new ArgumentException("Variant directory key normalization changed the immutable context value.", nameof(variant));
        string root = NormalizeVariantsRoot(project);
        string path = Path.GetFullPath(Path.Combine(root, key));
        if (!IsStrictChild(path, root)) throw new InvalidDataException("Variant directory escaped VARIANTS.");
        return path;
    }

    /// <summary>Authoritative owned output identity for one runnable:
    /// VARIANTS\&lt;RuntimeKey&gt;\&lt;EditionCode&gt;. Edition subdirectories
    /// coexist; no build ever writes loose files into the runtime root.</summary>
    public string GetVariantEditionDirectoryPath(ProjectContext project, VariantContext variant, string projectCode)
    {
        ValidateAssociation(project, variant);
        string code = ProjectVariantOwnership.NormalizeCode(project, projectCode);
        string runtimeRoot = GetVariantDirectoryPath(project, variant);
        string path = Path.GetFullPath(Path.Combine(runtimeRoot, code));
        if (!IsStrictChild(path, runtimeRoot)) throw new InvalidDataException("Variant edition directory escaped its runtime root.");
        return path;
    }

    public VariantDirectoryOperationResult EnsureVariantEditionDirectory(ProjectContext project, VariantContext variant, string projectCode)
    {
        string path;
        try { path = GetVariantEditionDirectoryPath(project, variant, projectCode); }
        catch (ArgumentException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        catch (InvalidDataException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }

        string runtimeRoot;
        try { runtimeRoot = GetVariantDirectoryPath(project, variant); }
        catch (ArgumentException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        if (File.Exists(runtimeRoot) && !Directory.Exists(runtimeRoot)) return new(VariantDirectoryOperationStatus.PathConflict, path);
        if (File.Exists(path) && !Directory.Exists(path)) return new(VariantDirectoryOperationStatus.PathConflict, path);
        if (Directory.Exists(path))
        {
            if (IsReparsePoint(path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, path);
            return new(ClassifyEditionOwnership(path, project, variant, ProjectVariantOwnership.NormalizeCode(project, projectCode)) switch
            {
                OwnershipState.Matching => VariantDirectoryOperationStatus.AlreadyValid,
                OwnershipState.Missing => VariantDirectoryOperationStatus.ForeignDirectoryConflict,
                _ => VariantDirectoryOperationStatus.OwnershipConflict
            }, path);
        }

        bool createdTarget = false;
        try
        {
            bool runtimeExisted = Directory.Exists(runtimeRoot);
            Directory.CreateDirectory(runtimeRoot);
            if (IsReparsePoint(runtimeRoot)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, path);
            string code = ProjectVariantOwnership.NormalizeCode(project, projectCode);
            if (runtimeExisted && !IsAdoptableRuntimeRoot(project, variant, runtimeRoot))
                return new(VariantDirectoryOperationStatus.ForeignDirectoryConflict, path);
            if (!runtimeExisted || ClassifyOwnership(runtimeRoot, project, variant) != OwnershipState.Matching)
            {
                // New or unmarked-but-adoptable roots receive the runtime
                // container marker; foreign payload is never adopted (see above).
                if (!runtimeExisted || !Directory.EnumerateFileSystemEntries(runtimeRoot).Any())
                    WriteMarkerAtomic(runtimeRoot, CreateMarker(project, variant));
            }
            Directory.CreateDirectory(path); createdTarget = true;
            WriteMarkerAtomic(path, CreateEditionMarker(project, variant, code));
            if (ClassifyEditionOwnership(path, project, variant, code) != OwnershipState.Matching)
                throw new InvalidDataException("The newly written edition ownership marker did not validate.");
            return new(VariantDirectoryOperationStatus.Created, path);
        }
        catch
        {
            if (createdTarget && Directory.Exists(path) && !IsReparsePoint(path) && !Directory.EnumerateFileSystemEntries(path).Any())
                Directory.Delete(path, false);
            throw;
        }
    }

    /// <summary>An existing runtime root may host a new edition only when it
    /// is already owned (valid container marker, legacy files may coexist),
    /// empty (safe to adopt), or holds nothing but valid edition outputs of
    /// the same variant/baseline. Foreign payload is never adopted.</summary>
    private static bool IsAdoptableRuntimeRoot(ProjectContext project, VariantContext variant, string runtimeRoot)
    {
        if (ClassifyOwnership(runtimeRoot, project, variant) == OwnershipState.Matching) return true;
        List<string> entries;
        try { entries = Directory.EnumerateFileSystemEntries(runtimeRoot, "*", SearchOption.TopDirectoryOnly).ToList(); }
        catch (IOException) { return false; }
        catch (UnauthorizedAccessException) { return false; }
        if (entries.Count == 0) return true;
        foreach (string entry in entries)
        {
            if (!Directory.Exists(entry) || IsReparsePoint(entry)) return false;
            string code = Path.GetFileName(entry);
            string normalized;
            try { normalized = ProjectVariantOwnership.NormalizeCode(project, code); }
            catch (ArgumentException) { return false; }
            catch (InvalidOperationException) { return false; }
            if (ClassifyEditionOwnership(entry, project, variant, normalized) != OwnershipState.Matching) return false;
        }
        return true;
    }

    public VariantDirectoryOperationResult ValidateOwnedVariantEditionDirectory(ProjectContext project, VariantContext variant, string projectCode)
    {
        string path;
        try { path = GetVariantEditionDirectoryPath(project, variant, projectCode); }
        catch (ArgumentException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        catch (InvalidDataException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        if (File.Exists(path) && !Directory.Exists(path)) return new(VariantDirectoryOperationStatus.PathConflict, path);
        if (!Directory.Exists(path)) return new(VariantDirectoryOperationStatus.NotFound, path);
        if (IsReparsePoint(path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, path);
        string code;
        try { code = ProjectVariantOwnership.NormalizeCode(project, projectCode); }
        catch (ArgumentException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        catch (InvalidOperationException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        return new(ClassifyEditionOwnership(path, project, variant, code) switch
        {
            OwnershipState.Matching => VariantDirectoryOperationStatus.AlreadyValid,
            OwnershipState.Missing => VariantDirectoryOperationStatus.ForeignDirectoryConflict,
            _ => VariantDirectoryOperationStatus.OwnershipConflict
        }, path);
    }

    public VariantDirectoryOperationResult ClearVariantEditionDirectory(ProjectContext project, VariantContext variant, string projectCode)
    {
        VariantDirectoryOperationResult checkedDirectory = ValidateOwnedVariantEditionDirectory(project, variant, projectCode);
        if (checkedDirectory.Status != VariantDirectoryOperationStatus.AlreadyValid) return checkedDirectory;
        if (ContainsReparsePoint(checkedDirectory.Path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, checkedDirectory.Path);
        DeleteContentsExceptRootMarker(checkedDirectory.Path, isRoot: true);
        string code = ProjectVariantOwnership.NormalizeCode(project, projectCode);
        if (ClassifyEditionOwnership(checkedDirectory.Path, project, variant, code) != OwnershipState.Matching)
            throw new InvalidDataException("Clear damaged the edition ownership marker.");
        return new(VariantDirectoryOperationStatus.Cleared, checkedDirectory.Path);
    }

    /// <summary>Removes exactly one owned edition directory. Sibling editions
    /// are never touched. An emptied runtime root (marker only) is pruned;
    /// VARIANTS itself is never deleted.</summary>
    public VariantDirectoryOperationResult DeleteVariantEditionDirectory(ProjectContext project, VariantContext variant, string projectCode)
    {
        VariantDirectoryOperationResult checkedDirectory = ValidateOwnedVariantEditionDirectory(project, variant, projectCode);
        if (checkedDirectory.Status != VariantDirectoryOperationStatus.AlreadyValid) return checkedDirectory;
        if (ContainsReparsePoint(checkedDirectory.Path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, checkedDirectory.Path);
        DeleteContentsExceptRootMarker(checkedDirectory.Path, isRoot: true);
        File.Delete(Path.Combine(checkedDirectory.Path, OwnershipMarkerFileName));
        Directory.Delete(checkedDirectory.Path, false);
        PruneEmptyRuntimeRoot(project, variant);
        return new(VariantDirectoryOperationStatus.Deleted, checkedDirectory.Path);
    }

    public VariantDirectoryOperationResult EnsureVariantDirectory(ProjectContext project, VariantContext variant)
    {
        string path;
        try { path = GetVariantDirectoryPath(project, variant); }
        catch (ArgumentException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }

        string root = project.StorageLayout.VariantsRoot;
        if (File.Exists(root) && !Directory.Exists(root)) return new(VariantDirectoryOperationStatus.PathConflict, path);
        if (Directory.Exists(root) && IsReparsePoint(root)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, path);
        if (File.Exists(path) && !Directory.Exists(path)) return new(VariantDirectoryOperationStatus.PathConflict, path);
        if (Directory.Exists(path))
        {
            if (IsReparsePoint(path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, path);
            return new(ClassifyOwnership(path, project, variant) switch
            {
                OwnershipState.Matching => VariantDirectoryOperationStatus.AlreadyValid,
                OwnershipState.Missing => VariantDirectoryOperationStatus.ForeignDirectoryConflict,
                _ => VariantDirectoryOperationStatus.OwnershipConflict
            }, path);
        }

        bool createdTarget = false;
        try
        {
            Directory.CreateDirectory(root);
            Directory.CreateDirectory(path); createdTarget = true;
            WriteMarkerAtomic(path, CreateMarker(project, variant));
            if (ClassifyOwnership(path, project, variant) != OwnershipState.Matching)
                throw new InvalidDataException("The newly written ownership marker did not validate.");
            return new(VariantDirectoryOperationStatus.Created, path);
        }
        catch
        {
            if (createdTarget && Directory.Exists(path) && !IsReparsePoint(path) && !Directory.EnumerateFileSystemEntries(path).Any())
                Directory.Delete(path, false);
            throw;
        }
    }

    public VariantDirectoryOperationResult ClearVariantDirectory(ProjectContext project, VariantContext variant)
    {
        VariantDirectoryOperationResult checkedDirectory = ValidateOwnedDirectory(project, variant);
        if (checkedDirectory.Status != VariantDirectoryOperationStatus.AlreadyValid) return checkedDirectory;
        if (ContainsReparsePoint(checkedDirectory.Path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, checkedDirectory.Path);
        DeleteContentsExceptRootMarker(checkedDirectory.Path, isRoot: true);
        if (ClassifyOwnership(checkedDirectory.Path, project, variant) != OwnershipState.Matching)
            throw new InvalidDataException("Clear damaged the ownership marker.");
        return new(VariantDirectoryOperationStatus.Cleared, checkedDirectory.Path);
    }

    /// <summary>Read-only ownership validation used by the disposable-build stage.</summary>
    public VariantDirectoryOperationResult ValidateOwnedVariantDirectory(ProjectContext project, VariantContext variant) =>
        ValidateOwnedDirectory(project, variant);

    public VariantDirectoryOperationResult DeleteVariantDirectory(ProjectContext project, VariantContext variant)
    {
        VariantDirectoryOperationResult checkedDirectory = ValidateOwnedDirectory(project, variant);
        if (checkedDirectory.Status != VariantDirectoryOperationStatus.AlreadyValid) return checkedDirectory;
        if (ContainsReparsePoint(checkedDirectory.Path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, checkedDirectory.Path);
        DeleteContentsExceptRootMarker(checkedDirectory.Path, isRoot: true);
        File.Delete(Path.Combine(checkedDirectory.Path, OwnershipMarkerFileName));
        Directory.Delete(checkedDirectory.Path, false);
        // Deliberately never delete VARIANTS itself, even if it became empty.
        return new(VariantDirectoryOperationStatus.Deleted, checkedDirectory.Path);
    }

    public IReadOnlyList<VariantDirectoryInfo> Enumerate(ProjectContext project, IEnumerable<VariantContext> knownVariants)
    {
        if (project is null) throw new ArgumentNullException(nameof(project));
        VariantContext[] variants = knownVariants?.ToArray() ?? throw new ArgumentNullException(nameof(knownVariants));
        foreach (VariantContext variant in variants) ValidateAssociation(project, variant);
        if (variants.Select(variant => variant.DirectoryKey).Distinct(StringComparer.OrdinalIgnoreCase).Count() != variants.Length)
            throw new ArgumentException("Known variant contexts have a case-insensitive directory-key collision.", nameof(knownVariants));

        string root = project.StorageLayout.VariantsRoot;
        if (!Directory.Exists(root)) return [];
        if (IsReparsePoint(root)) return [new(root, string.Empty, VariantDirectoryClassification.ReparsePointConflict)];
        var output = new List<VariantDirectoryInfo>();
        foreach (string path in Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly))
        {
            string key = Path.GetFileName(path);
            try { _ = VariantContext.ValidateDirectoryKey(key); }
            catch (ArgumentException) { output.Add(new(path, key, VariantDirectoryClassification.InvalidDirectoryName)); continue; }
            if (IsReparsePoint(path)) { output.Add(new(path, key, VariantDirectoryClassification.ReparsePointConflict)); continue; }
            VariantContext? expected = variants.SingleOrDefault(variant => variant.DirectoryKey.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (expected is null) { output.Add(new(path, key, VariantDirectoryClassification.ForeignDirectory)); continue; }
            EnumerateRuntimeRoot(project, expected, path, output);
        }
        return output.OrderBy(item => item.DirectoryKey, StringComparer.OrdinalIgnoreCase).ThenBy(item => item.ProjectCode, StringComparer.Ordinal).ToArray();
    }

    /// <summary>Lists owned edition outputs inside one runtime root plus any
    /// legacy flat residue (files directly in the root). The runtime root
    /// itself is a container and yields no entry.</summary>
    private static void EnumerateRuntimeRoot(ProjectContext project, VariantContext variant, string runtimePath, List<VariantDirectoryInfo> output)
    {
        bool hasEditions = false;
        foreach (string child in Directory.EnumerateDirectories(runtimePath, "*", SearchOption.TopDirectoryOnly))
        {
            string code = Path.GetFileName(child);
            if (IsReparsePoint(child)) { output.Add(new(child, variant.DirectoryKey, VariantDirectoryClassification.ReparsePointConflict, variant.VariantId, code)); continue; }
            string normalized;
            try { normalized = ProjectVariantOwnership.NormalizeCode(project, code); }
            catch (ArgumentException) { output.Add(new(child, variant.DirectoryKey, VariantDirectoryClassification.ForeignDirectory, variant.VariantId, code)); continue; }
            catch (InvalidOperationException) { output.Add(new(child, variant.DirectoryKey, VariantDirectoryClassification.ForeignDirectory, variant.VariantId, code)); continue; }
            hasEditions = true;
            output.Add(new(child, variant.DirectoryKey, ClassifyEditionOwnership(child, project, variant, normalized) switch
            {
                OwnershipState.Matching => VariantDirectoryClassification.KnownOwnedVariant,
                OwnershipState.Missing => VariantDirectoryClassification.ForeignDirectory,
                _ => VariantDirectoryClassification.OwnershipConflict
            }, variant.VariantId, normalized));
        }
        VariantLegacyFlatInfo legacy = InspectLegacyFlat(project, variant, runtimePath);
        if (legacy.HasLegacyFiles)
        {
            // Residue files directly in the root: owned legacy when the
            // container marker matches (reported, never deleted implicitly),
            // otherwise foreign. Edition outputs alongside are listed above.
            output.Add(new(runtimePath, variant.DirectoryKey, legacy.HasMatchingMarker
                ? VariantDirectoryClassification.LegacyFlatOwned : VariantDirectoryClassification.ForeignDirectory,
                legacy.HasMatchingMarker ? variant.VariantId : null, legacy.ManifestProjectCode));
        }
        else if (!hasEditions)
        {
            // No editions and no residue: fall back to the historical
            // runtime-level marker classification so empty owned roots stay visible.
            output.Add(new(runtimePath, variant.DirectoryKey, ClassifyOwnership(runtimePath, project, variant) switch
            {
                OwnershipState.Matching => VariantDirectoryClassification.KnownOwnedVariant,
                OwnershipState.Missing => VariantDirectoryClassification.ForeignDirectory,
                _ => VariantDirectoryClassification.OwnershipConflict
            }, variant.VariantId));
        }
    }

    private VariantDirectoryOperationResult ValidateOwnedDirectory(ProjectContext project, VariantContext variant)
    {
        string path;
        try { path = GetVariantDirectoryPath(project, variant); }
        catch (ArgumentException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        if (File.Exists(path) && !Directory.Exists(path)) return new(VariantDirectoryOperationStatus.PathConflict, path);
        if (!Directory.Exists(path)) return new(VariantDirectoryOperationStatus.NotFound, path);
        if (IsReparsePoint(path)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, path);
        return new(ClassifyOwnership(path, project, variant) switch
        {
            OwnershipState.Matching => VariantDirectoryOperationStatus.AlreadyValid,
            OwnershipState.Missing => VariantDirectoryOperationStatus.ForeignDirectoryConflict,
            _ => VariantDirectoryOperationStatus.OwnershipConflict
        }, path);
    }

    private static VariantDirectoryOwnershipMarker CreateMarker(ProjectContext project, VariantContext variant) => new(
        OwnershipMarkerSchema,
        OwnershipMarkerProduct,
        PristineManifestService.GameIdFor(project.GameProfile),
        variant.VariantId.ToString(),
        variant.DirectoryKey,
        project.BaselineFingerprint);

    /// <summary>Pre-R9F flat-output evidence. Legacy files are payload files
    /// directly in the runtime root (ownership marker and temp files
    /// excluded). They are reported, never reinterpreted or deleted implicitly.
    /// HasOwnedEditions is true only for child directories carrying a valid
    /// edition marker for this project/runtime/baseline; plain game-content
    /// subdirectories (DOSBOX, cloud_saves, …) do NOT count.</summary>
    internal sealed record VariantLegacyFlatInfo(
        bool HasLegacyFiles,
        bool HasMatchingMarker,
        bool HasOwnedEditions,
        string? ManifestProjectCode,
        IReadOnlyList<string> FileNames);

    /// <summary>Detects legacy flat outputs without touching anything.</summary>
    public VariantLegacyFlatInfo DetectLegacyFlatVariant(ProjectContext project, VariantContext variant)
    {
        ValidateAssociation(project, variant);
        string runtimePath;
        try { runtimePath = GetVariantDirectoryPath(project, variant); }
        catch (ArgumentException) { return new(false, false, false, null, []); }
        if (!Directory.Exists(runtimePath) || IsReparsePoint(runtimePath)) return new(false, false, false, null, []);
        return InspectLegacyFlat(project, variant, runtimePath);
    }

    /// <summary>Explicit, user-authorized removal of legacy flat outputs.
    /// Refuses when owned edition outputs coexist so siblings are protected;
    /// unknown/foreign contents are never touched.</summary>
    public VariantDirectoryOperationResult DeleteLegacyFlatVariantDirectory(ProjectContext project, VariantContext variant)
    {
        ValidateAssociation(project, variant);
        string runtimePath;
        try { runtimePath = GetVariantDirectoryPath(project, variant); }
        catch (ArgumentException) { return new(VariantDirectoryOperationStatus.InvalidContext, string.Empty); }
        if (!Directory.Exists(runtimePath)) return new(VariantDirectoryOperationStatus.NotFound, runtimePath);
        if (IsReparsePoint(runtimePath)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, runtimePath);
        VariantLegacyFlatInfo legacy = InspectLegacyFlat(project, variant, runtimePath);
        if (!legacy.HasMatchingMarker) return new(VariantDirectoryOperationStatus.ForeignDirectoryConflict, runtimePath);
        if (legacy.HasOwnedEditions) return new(VariantDirectoryOperationStatus.OwnershipConflict, runtimePath);
        if (ContainsReparsePoint(runtimePath)) return new(VariantDirectoryOperationStatus.ReparsePointConflict, runtimePath);
        DeleteContentsExceptRootMarker(runtimePath, isRoot: true);
        File.Delete(Path.Combine(runtimePath, OwnershipMarkerFileName));
        Directory.Delete(runtimePath, false);
        return new(VariantDirectoryOperationStatus.Deleted, runtimePath);
    }

    private static VariantLegacyFlatInfo InspectLegacyFlat(ProjectContext project, VariantContext variant, string runtimePath)
    {
        bool matching = ClassifyOwnership(runtimePath, project, variant) == OwnershipState.Matching;
        bool ownedEditions = HasOwnedEditionSubdirectory(project, variant, runtimePath);
        var files = new List<string>();
        try
        {
            foreach (string file in Directory.EnumerateFiles(runtimePath, "*", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileName(file);
                if (name.Equals(OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)) continue;
                if (name.EndsWith(".tmp", StringComparison.OrdinalIgnoreCase)) continue;
                files.Add(name);
            }
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
        files.Sort(StringComparer.OrdinalIgnoreCase);
        return new(files.Count > 0, matching, ownedEditions, ReadLegacyManifestProjectCode(runtimePath), files);
    }

    /// <summary>True only when a child directory parses as an edition code
    /// AND carries a valid edition ownership marker for this exact
    /// project/runtime/baseline. Plain game-content subdirectories
    /// (DOSBOX, cloud_saves, set_config, …) never count: they are payload,
    /// not sibling editions.</summary>
    private static bool HasOwnedEditionSubdirectory(ProjectContext project, VariantContext variant, string runtimePath)
    {
        string[] children;
        try { children = Directory.EnumerateDirectories(runtimePath, "*", SearchOption.TopDirectoryOnly).ToArray(); }
        catch (IOException) { return false; }
        catch (UnauthorizedAccessException) { return false; }
        foreach (string child in children)
        {
            if (IsReparsePoint(child)) continue;
            string code;
            try { code = ProjectVariantOwnership.NormalizeCode(project, Path.GetFileName(child)); }
            catch (ArgumentException) { continue; }
            catch (InvalidOperationException) { continue; }
            if (ClassifyEditionOwnership(child, project, variant, code) == OwnershipState.Matching) return true;
        }
        return false;
    }

    private static string? ReadLegacyManifestProjectCode(string runtimePath)
    {
        try
        {
            string manifest = Path.Combine(runtimePath, VariantManifestService.FileName);
            if (!File.Exists(manifest)) return null;
            VariantManifest parsed = VariantManifestService.Read(runtimePath);
            return string.IsNullOrWhiteSpace(parsed.ProjectCode) ? null : parsed.ProjectCode;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Text.Json.JsonException or InvalidDataException)
        {
            return null;
        }
    }

    private static VariantDirectoryOwnershipMarker CreateEditionMarker(ProjectContext project, VariantContext variant, string projectCode) => new(
        EditionOwnershipMarkerSchema,
        OwnershipMarkerProduct,
        PristineManifestService.GameIdFor(project.GameProfile),
        variant.VariantId.ToString(),
        variant.DirectoryKey,
        project.BaselineFingerprint,
        projectCode);

    private static OwnershipState ClassifyEditionOwnership(string directory, ProjectContext project, VariantContext variant, string projectCode)
    {
        string markerPath = Path.Combine(directory, OwnershipMarkerFileName);
        if (!File.Exists(markerPath)) return OwnershipState.Missing;
        try
        {
            VariantDirectoryOwnershipMarker? marker = JsonSerializer.Deserialize<VariantDirectoryOwnershipMarker>(File.ReadAllText(markerPath));
            return marker is not null && marker.SchemaVersion == EditionOwnershipMarkerSchema && marker.Product == OwnershipMarkerProduct &&
                marker.GameId == PristineManifestService.GameIdFor(project.GameProfile) && marker.VariantId == variant.VariantId.ToString() &&
                marker.DirectoryKey == variant.DirectoryKey && marker.BaselineFingerprint == project.BaselineFingerprint &&
                marker.ProjectCode.Equals(projectCode, StringComparison.Ordinal)
                ? OwnershipState.Matching : OwnershipState.Mismatch;
        }
        catch (IOException) { return OwnershipState.Mismatch; }
        catch (JsonException) { return OwnershipState.Mismatch; }
    }

    private void PruneEmptyRuntimeRoot(ProjectContext project, VariantContext variant)
    {
        string runtimePath;
        try { runtimePath = GetVariantDirectoryPath(project, variant); }
        catch (ArgumentException) { return; }
        catch (InvalidDataException) { return; }
        try
        {
            if (!Directory.Exists(runtimePath) || IsReparsePoint(runtimePath)) return;
            if (Directory.EnumerateFileSystemEntries(runtimePath).All(entry =>
                Path.GetFileName(entry).Equals(OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)))
            {
                foreach (string file in Directory.EnumerateFiles(runtimePath, "*", SearchOption.TopDirectoryOnly))
                    File.Delete(file);
                Directory.Delete(runtimePath, false);
            }
        }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
    }

    private static void WriteMarkerAtomic(string directory, VariantDirectoryOwnershipMarker marker)
    {
        string markerPath = Path.Combine(directory, OwnershipMarkerFileName);
        string temporary = Path.Combine(directory, ".owner." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(marker, JsonOptions));
            File.Move(temporary, markerPath, false);
        }
        finally { if (File.Exists(temporary)) File.Delete(temporary); }
    }

    private static OwnershipState ClassifyOwnership(string directory, ProjectContext project, VariantContext variant)
    {
        string markerPath = Path.Combine(directory, OwnershipMarkerFileName);
        if (!File.Exists(markerPath)) return OwnershipState.Missing;
        try
        {
            VariantDirectoryOwnershipMarker? marker = JsonSerializer.Deserialize<VariantDirectoryOwnershipMarker>(File.ReadAllText(markerPath));
            return marker is not null && marker.SchemaVersion == OwnershipMarkerSchema && marker.Product == OwnershipMarkerProduct &&
                marker.GameId == PristineManifestService.GameIdFor(project.GameProfile) && marker.VariantId == variant.VariantId.ToString() &&
                marker.DirectoryKey == variant.DirectoryKey && marker.BaselineFingerprint == project.BaselineFingerprint
                ? OwnershipState.Matching : OwnershipState.Mismatch;
        }
        catch (IOException) { return OwnershipState.Mismatch; }
        catch (JsonException) { return OwnershipState.Mismatch; }
    }

    private static void ValidateAssociation(ProjectContext project, VariantContext variant)
    {
        if (project is null) throw new ArgumentNullException(nameof(project));
        if (variant is null || !ReferenceEquals(project, variant.Project))
            throw new ArgumentException("VariantContext must belong to the exact ProjectContext instance.", nameof(variant));
    }

    private static string NormalizeVariantsRoot(ProjectContext project)
    {
        string root = Path.GetFullPath(project.StorageLayout.VariantsRoot);
        if (!root.Equals(project.StorageLayout.VariantsRoot, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Variants root is not canonical.");
        return root;
    }

    private static bool ContainsReparsePoint(string directory)
    {
        foreach (string entry in Directory.EnumerateFileSystemEntries(directory, "*", SearchOption.TopDirectoryOnly))
        {
            if (IsReparsePoint(entry)) return true;
            if (Directory.Exists(entry) && ContainsReparsePoint(entry)) return true;
        }
        return false;
    }

    private static void DeleteContentsExceptRootMarker(string directory, bool isRoot)
    {
        foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly))
            if (!isRoot || !Path.GetFileName(file).Equals(OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)) File.Delete(file);
        foreach (string child in Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly))
        {
            DeleteContentsExceptRootMarker(child, isRoot: false);
            Directory.Delete(child, false);
        }
    }

    private static bool IsReparsePoint(string path) => (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;
    private static bool IsStrictChild(string candidate, string parent) => candidate.StartsWith(parent.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    private enum OwnershipState { Matching, Missing, Mismatch }
}
