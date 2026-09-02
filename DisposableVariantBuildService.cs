using System.Security.Cryptography;

namespace ElviraVgaEditor;

internal enum DisposableVariantBuildStatus
{
    Success, InvalidContext, BaselineInvalid, VariantDirectoryMissing,
    VariantOwnershipConflict, MutableBackupMissing, MutableBackupMismatch,
    SourceFileMissing, SourceHashMismatch, CopyFailure, VerificationFailure
}

internal sealed record DisposableVariantBuildResult(DisposableVariantBuildStatus Status, string VariantRoot, int PayloadFileCount = 0, int VgaFileCount = 0);
internal sealed record DisposableVariantBuildSource(PristineManifestFile ManifestFile, string SourcePath);

/// <summary>Manifest-driven, pristine-only complete tree copy for an owned variant root.</summary>
internal sealed class DisposableVariantBuildService
{
    private readonly VariantDirectoryService _directories;

    public DisposableVariantBuildService(VariantDirectoryService directories) => _directories = directories ?? throw new ArgumentNullException(nameof(directories));

    public DisposableVariantBuildResult Build(ProjectContext project, VariantContext variant)
    {
        VariantDirectoryOperationResult owned = _directories.ValidateOwnedVariantDirectory(project, variant);
        string root = owned.Path;
        if (owned.Status == VariantDirectoryOperationStatus.NotFound) return new(DisposableVariantBuildStatus.VariantDirectoryMissing, root);
        if (owned.Status == VariantDirectoryOperationStatus.InvalidContext) return new(DisposableVariantBuildStatus.InvalidContext, root);
        if (owned.Status != VariantDirectoryOperationStatus.AlreadyValid) return new(DisposableVariantBuildStatus.VariantOwnershipConflict, root);

        if (new PristineManifestService(project.StorageLayout, project.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
            return new(DisposableVariantBuildStatus.BaselineInvalid, root);
        (DisposableVariantBuildStatus Status, IReadOnlyList<DisposableVariantBuildSource> Sources) preflight = ValidateSources(project);
        if (preflight.Status != DisposableVariantBuildStatus.Success) return new(preflight.Status, root);

        try
        {
            VariantDirectoryOperationResult cleared = _directories.ClearVariantDirectory(project, variant);
            if (cleared.Status != VariantDirectoryOperationStatus.Cleared) return new(DisposableVariantBuildStatus.VariantOwnershipConflict, root);
            foreach (DisposableVariantBuildSource source in preflight.Sources) CopySource(root, source);
        }
        catch (IOException) { return new(DisposableVariantBuildStatus.CopyFailure, root); }
        catch (UnauthorizedAccessException) { return new(DisposableVariantBuildStatus.CopyFailure, root); }

        return VerifyOutput(project, variant, preflight.Sources);
    }

    /// <summary>Read-only source-resolution primitive; useful for testing mutable backup semantics.</summary>
    internal static (DisposableVariantBuildStatus Status, IReadOnlyList<DisposableVariantBuildSource> Sources) ValidateSources(ProjectContext project)
    {
        if (project is null) return (DisposableVariantBuildStatus.InvalidContext, []);
        var sources = new List<DisposableVariantBuildSource>();
        foreach (PristineManifestFile file in project.PristineManifest.Files)
        {
            if (file.Classification == PristineFileClassification.GeneratedIgnored) continue;
            string source = file.Classification == PristineFileClassification.MutableBackedUp
                ? SafeChild(project.StorageLayout.MutableBackupsRoot, file.RelativePath)
                : SafeChild(project.GameRoot, file.RelativePath);
            if (!File.Exists(source)) return (file.Classification == PristineFileClassification.MutableBackedUp ? DisposableVariantBuildStatus.MutableBackupMissing : DisposableVariantBuildStatus.SourceFileMissing, []);
            if (IsReparsePoint(source)) return (file.Classification == PristineFileClassification.MutableBackedUp ? DisposableVariantBuildStatus.MutableBackupMismatch : DisposableVariantBuildStatus.SourceHashMismatch, []);
            FileInfo info = new(source);
            if (info.Length != file.Size || !Hash(source).Equals(file.Sha256, StringComparison.OrdinalIgnoreCase))
                return (file.Classification == PristineFileClassification.MutableBackedUp ? DisposableVariantBuildStatus.MutableBackupMismatch : DisposableVariantBuildStatus.SourceHashMismatch, []);
            sources.Add(new(file, source));
        }
        return (DisposableVariantBuildStatus.Success, sources);
    }

    private DisposableVariantBuildResult VerifyOutput(ProjectContext project, VariantContext variant, IReadOnlyList<DisposableVariantBuildSource> sources)
    {
        VariantDirectoryOperationResult owned = _directories.ValidateOwnedVariantDirectory(project, variant);
        if (owned.Status != VariantDirectoryOperationStatus.AlreadyValid) return new(DisposableVariantBuildStatus.VerificationFailure, owned.Path);
        foreach (DisposableVariantBuildSource source in sources)
        {
            string destination = SafeChild(owned.Path, source.ManifestFile.RelativePath);
            if (!File.Exists(destination) || IsReparsePoint(destination)) return new(DisposableVariantBuildStatus.VerificationFailure, owned.Path);
            FileInfo info = new(destination);
            if (info.Length != source.ManifestFile.Size || !Hash(destination).Equals(source.ManifestFile.Sha256, StringComparison.OrdinalIgnoreCase))
                return new(DisposableVariantBuildStatus.VerificationFailure, owned.Path);
        }
        var actual = Directory.EnumerateFiles(owned.Path, "*", SearchOption.AllDirectories)
            .Where(path => !Path.GetFileName(path).Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase))
            .Select(path => Path.GetRelativePath(owned.Path, path).Replace('/', '\\')).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var expected = sources.Select(source => source.ManifestFile.RelativePath).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (!actual.SetEquals(expected)) return new(DisposableVariantBuildStatus.VerificationFailure, owned.Path);
        return new(DisposableVariantBuildStatus.Success, owned.Path, sources.Count, sources.Count(source => source.ManifestFile.RelativePath.EndsWith(".VGA", StringComparison.OrdinalIgnoreCase)));
    }

    private static void CopySource(string variantRoot, DisposableVariantBuildSource source)
    {
        string destination = SafeChild(variantRoot, source.ManifestFile.RelativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        File.Copy(source.SourcePath, destination, false);
    }

    private static string SafeChild(string parent, string relative)
    {
        string path = Path.GetFullPath(Path.Combine(parent, relative));
        string normalizedParent = Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!path.StartsWith(normalizedParent, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Manifest path escaped its validated root.");
        return path;
    }

    private static bool IsReparsePoint(string path) => (File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0;
    private static string Hash(string path) { using FileStream stream = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(stream)); }
}
