using System.Security.Cryptography;

namespace Pi1ElviraEditor;

internal enum PristineFileClassification { Immutable, MutableBackedUp, GeneratedIgnored }
internal enum PristineFileRole { Executable, GameData, Resource, Launcher, Config, Save, EditorGenerated, Unknown }
internal enum PristineBaselineComparison { MatchesBaseline, ExternalInstallationChange, MissingPristineFile, UnexpectedFile, SupportedUpdatePendingAdoption, UnsupportedExecutableVersion }
internal enum MutableBackupStatus { DeferredToR6C, AlreadyValid, Conflict }

internal sealed record PristineInstallationFile(
    string RelativePath, string AbsolutePath, long Size, string Sha256,
    PristineFileClassification Classification, PristineFileRole Role);

/// <summary>
/// Read-only whole-installation inventory for a user-supplied clean baseline.
/// It has no manifest persistence, no baseline initialization, and no rollback.
/// </summary>
internal sealed class PristineInstallationService
{
    private readonly EditorStorageLayout _layout;

    public PristineInstallationService(EditorStorageLayout layout)
    {
        _layout = layout ?? throw new ArgumentNullException(nameof(layout));
    }

    public IReadOnlyList<PristineInstallationFile> EnumerateReadOnly()
    {
        var entries = new List<PristineInstallationFile>();
        var paths = EnumerateFilesWithoutReparsePoints(_layout.GameRoot)
            .Select(path => new { Absolute = Path.GetFullPath(path), Relative = NormalizeRelative(path) })
            .Where(item => !IsEditorOwned(item.Relative))
            .OrderBy(item => item.Relative, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        if (paths.Select(item => item.Relative).Distinct(StringComparer.OrdinalIgnoreCase).Count() != paths.Length)
            throw new InvalidDataException("Installation inventory contains a case-insensitive path collision.");

        foreach (var path in paths)
        {
            var info = new FileInfo(path.Absolute);
            (PristineFileClassification classification, PristineFileRole role) = Classify(path.Relative);
            entries.Add(new PristineInstallationFile(path.Relative, path.Absolute, info.Length, HashFile(path.Absolute), classification, role));
        }
        return entries;
    }

    internal static (PristineFileClassification Classification, PristineFileRole Role) Classify(string normalizedRelativePath)
    {
        string file = Path.GetFileName(normalizedRelativePath);
        string upper = normalizedRelativePath.Replace('/', '\\').ToUpperInvariant();
        if (file.Equals("ELVIRA.BAT", StringComparison.OrdinalIgnoreCase) || upper == "CERBERUS.BAT")
            return (PristineFileClassification.MutableBackedUp, PristineFileRole.Launcher);
        if (file.Equals("PI1MENU.COM", StringComparison.OrdinalIgnoreCase) || file.Equals("PI1SND.BAT", StringComparison.OrdinalIgnoreCase) ||
            file.Equals("ELVIRA_MODS.INI", StringComparison.OrdinalIgnoreCase) || file.Equals("ELVIRA.BAK", StringComparison.OrdinalIgnoreCase) ||
            file.Equals("ELVOVR.BAK", StringComparison.OrdinalIgnoreCase) || file.Equals("TMP.TXT", StringComparison.OrdinalIgnoreCase) ||
            file.EndsWith(".SAV", StringComparison.OrdinalIgnoreCase))
            return (PristineFileClassification.GeneratedIgnored, file.EndsWith(".SAV", StringComparison.OrdinalIgnoreCase) ? PristineFileRole.Save : PristineFileRole.EditorGenerated);
        if (file.Equals("GAMEPC", StringComparison.OrdinalIgnoreCase)) return (PristineFileClassification.Immutable, PristineFileRole.GameData);
        if (file.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase)) return (PristineFileClassification.Immutable, PristineFileRole.Executable);
        if (file.EndsWith(".VGA", StringComparison.OrdinalIgnoreCase)) return (PristineFileClassification.Immutable, PristineFileRole.Resource);
        return (PristineFileClassification.Immutable, PristineFileRole.Unknown);
    }

    internal static bool IsSupportedFrozenExecutable(string relativePath, string sha256) =>
        relativePath.Replace('/', '\\').ToUpperInvariant() switch
        {
            "RUNVGA.EXE" => sha256.Equals(Elvira1ProductionProfile.RunVga.PackedOriginal.Sha256, StringComparison.OrdinalIgnoreCase),
            "RUNEGA.EXE" => sha256.Equals(Elvira1ProductionProfile.RunEga.PackedOriginal.Sha256, StringComparison.OrdinalIgnoreCase),
            "RUNIT.EXE" => sha256.Equals(Elvira2ProductionProfile.PackedOriginal.Sha256, StringComparison.OrdinalIgnoreCase),
            _ => false
        };

    private string NormalizeRelative(string path)
    {
        string relative = Path.GetRelativePath(_layout.GameRoot, Path.GetFullPath(path));
        if (Path.IsPathRooted(relative) || relative.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries).Any(part => part is "." or ".."))
            throw new InvalidDataException("Inventory path escaped game root.");
        return relative.Replace('/', '\\');
    }

    private static bool IsEditorOwned(string relative) =>
        relative.StartsWith(EditorStorageLayout.EditorDirectoryName + "\\", StringComparison.OrdinalIgnoreCase) ||
        relative.StartsWith(EditorStorageLayout.VariantsDirectoryName + "\\", StringComparison.OrdinalIgnoreCase);

    private static string HashFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static IEnumerable<string> EnumerateFilesWithoutReparsePoints(string directory)
    {
        foreach (string file in Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly)) yield return file;
        foreach (string child in Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly))
        {
            if ((File.GetAttributes(child) & FileAttributes.ReparsePoint) != 0) continue;
            foreach (string file in EnumerateFilesWithoutReparsePoints(child)) yield return file;
        }
    }
}
