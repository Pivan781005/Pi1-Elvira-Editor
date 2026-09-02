using System.Text.Json;

namespace ElviraVgaEditor;

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
    ReparsePointConflict
}

internal sealed record VariantDirectoryOperationResult(VariantDirectoryOperationStatus Status, string Path);
internal sealed record VariantDirectoryInfo(string Path, string DirectoryKey, VariantDirectoryClassification Classification, BuiltInVariantId? VariantId = null);

/// <summary>
/// Minimal safety marker, not a build/variant manifest. It contains no path,
/// timestamp, payload data, or mutable build state.
/// </summary>
internal sealed record VariantDirectoryOwnershipMarker(
    int SchemaVersion,
    string Product,
    string GameId,
    string VariantId,
    string DirectoryKey,
    string BaselineFingerprint);

/// <summary>
/// Explicit lifecycle for empty editor-owned directories immediately below
/// VARIANTS. It never copies game payload and never accepts caller paths.
/// </summary>
internal sealed class VariantDirectoryService
{
    internal const string OwnershipMarkerFileName = ".pi1-variant-owner.json";
    internal const int OwnershipMarkerSchema = 1;
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
            output.Add(new(path, key, ClassifyOwnership(path, project, expected) switch
            {
                OwnershipState.Matching => VariantDirectoryClassification.KnownOwnedVariant,
                OwnershipState.Missing => VariantDirectoryClassification.ForeignDirectory,
                _ => VariantDirectoryClassification.OwnershipConflict
            }, expected.VariantId));
        }
        return output.OrderBy(item => item.DirectoryKey, StringComparer.OrdinalIgnoreCase).ToArray();
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
