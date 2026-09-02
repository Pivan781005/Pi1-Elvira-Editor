using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ElviraVgaEditor;

/// <summary>Deterministic, editor-owned description of one completed variant
/// output. This file is diagnostic metadata, never evidence of launch readiness.
/// </summary>
internal sealed record VariantManifestArtifact(string RelativePath, long Size, string Sha256);
internal sealed record VariantManifestRuntimeArtifact(string RelativePath, bool Present, long Size, string Sha256);
internal sealed record VariantManifest(
    int SchemaVersion,
    string GameId,
    string VariantId,
    string RuntimeKind,
    string ProjectCode,
    string DirectoryKey,
    string BaselineFingerprint,
    string BuildState,
    IReadOnlyList<VariantManifestRuntimeArtifact> RuntimeArtifacts,
    IReadOnlyList<VariantManifestArtifact> OutputArtifacts);

internal static class VariantManifestService
{
    internal const string FileName = "variant-manifest.json";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public static VariantManifest Write(ProjectContext project, VariantContext variant, VariantDirectoryService directories, CompositeBuildMode mode, IReadOnlyList<string>? runtimeArtifactNames = null, string? projectCode = null)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(variant);
        ArgumentNullException.ThrowIfNull(directories);
        if (!ReferenceEquals(project, variant.Project))
            throw new ArgumentException("VariantContext does not belong to the project.", nameof(variant));

        VariantDirectoryOperationResult owned = directories.ValidateOwnedVariantDirectory(project, variant);
        if (owned.Status != VariantDirectoryOperationStatus.AlreadyValid)
            throw new InvalidDataException("A variant manifest may be written only to an editor-owned variant directory.");

        VariantManifest manifest = Create(project, variant, owned.Path, mode, runtimeArtifactNames, projectCode);
        string destination = Path.Combine(owned.Path, FileName);
        string temporary = destination + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(manifest, JsonOptions), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            File.Move(temporary, destination, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
        return manifest;
    }

    public static VariantManifest Read(string variantRoot)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(variantRoot);
        string path = Path.Combine(variantRoot, FileName);
        VariantManifest? manifest = JsonSerializer.Deserialize<VariantManifest>(File.ReadAllText(path), JsonOptions);
        return manifest ?? throw new InvalidDataException("Variant manifest is empty or invalid.");
    }

    private static VariantManifest Create(ProjectContext project, VariantContext variant, string root, CompositeBuildMode mode, IReadOnlyList<string>? runtimeArtifactNames, string? projectCode)
    {
        VariantManifestArtifact[] output = Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories)
            .Where(path => !IsEditorMetadata(path))
            .Select(path => CreateArtifact(root, path))
            .OrderBy(artifact => artifact.RelativePath, StringComparer.Ordinal)
            .ToArray();
        Dictionary<string, VariantManifestArtifact> byPath = output.ToDictionary(artifact => artifact.RelativePath, StringComparer.OrdinalIgnoreCase);
        string[] runtimePaths = (runtimeArtifactNames ?? [variant.GeneratedExecutableName, variant.LogicalDataFileName])
            .Select(name => Path.GetFileName(name ?? string.Empty).ToUpperInvariant())
            .Where(GameDataFileService.IsDos83FileName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();
        if (runtimePaths.Length != 2) throw new InvalidDataException("A variant manifest requires exactly two distinct DOS runtime artifacts.");
        VariantManifestRuntimeArtifact[] runtime = runtimePaths.Select(path => byPath.TryGetValue(path, out VariantManifestArtifact? artifact)
            ? new VariantManifestRuntimeArtifact(path, true, artifact.Size, artifact.Sha256)
            : new VariantManifestRuntimeArtifact(path, false, 0, string.Empty)).ToArray();
        return new VariantManifest(
            SchemaVersion: 1,
            GameId: PristineManifestService.GameIdFor(project.GameProfile),
            VariantId: variant.VariantId.ToString(),
            RuntimeKind: variant.RuntimeKind.ToString(),
            ProjectCode: ProjectVariantOwnership.NormalizeCode(project, projectCode ?? ProjectVariantOwnership.OriginalCode),
            DirectoryKey: variant.DirectoryKey,
            BaselineFingerprint: project.BaselineFingerprint,
            BuildState: mode == CompositeBuildMode.Full ? "Full" : "PristineOnly",
            RuntimeArtifacts: runtime,
            OutputArtifacts: output);
    }

    private static VariantManifestArtifact CreateArtifact(string root, string path)
    {
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
            throw new InvalidDataException("Variant manifest cannot include a reparse-point artifact.");
        using FileStream stream = File.OpenRead(path);
        return new(Path.GetRelativePath(root, path).Replace('\\', '/'), new FileInfo(path).Length, Convert.ToHexString(SHA256.HashData(stream)));
    }

    private static bool IsEditorMetadata(string path)
    {
        string name = Path.GetFileName(path);
        return name.Equals(FileName, StringComparison.OrdinalIgnoreCase) ||
            name.Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase);
    }
}
