using System.Text.Json;

namespace Pi1ElviraEditor;

internal enum InstallationDiscoverySource
{
    Remembered,
    FastDiscovery,
    Manual
}

/// <summary>One validated VGA installation. The display text is never parsed back into data.</summary>
internal sealed record GameInstallation(
    ElviraGameProfile Game,
    string InstallationPath,
    InstallationDiscoverySource DiscoverySource)
{
    public string NormalizedPath => InstallationPathNormalizer.Normalize(InstallationPath);

    public string DisplayText => $"{GameProfileInfo.For(Game).DisplayName} — {InstallationPath}";
}

internal sealed class InstallationSelectionItem
{
    public InstallationSelectionItem(GameInstallation? installation) => Installation = installation;

    public GameInstallation? Installation { get; }

    public override string ToString() => Installation?.DisplayText ?? UiText.Get("NoSupportedGameSelected");
}

internal static class InstallationPathNormalizer
{
    public static string Normalize(string path)
    {
        // This is deliberately a strict operation for actual filesystem paths.
        // The selector's "no supported game" item is represented by null, never
        // by an empty path, and callers must handle that state before normalizing.
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("An installation path is required.", nameof(path));
        string full = Path.GetFullPath(path.Trim());
        string root = Path.GetPathRoot(full) ?? string.Empty;
        return full.Length > root.Length
            ? full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            : full;
    }
}

/// <summary>Path comparison for an optional active installation.</summary>
internal static class InstallationSelectionState
{
    public static bool IsSameActiveInstallation(GameInstallation installation, string? activeInstallationPath)
    {
        // The selector sentinel deliberately has no path. Do not send it to
        // the strict filesystem-path normalizer merely to perform a comparison.
        return !string.IsNullOrWhiteSpace(activeInstallationPath) &&
            installation.NormalizedPath.Equals(InstallationPathNormalizer.Normalize(activeInstallationPath), StringComparison.OrdinalIgnoreCase);
    }
}

/// <summary>
/// Strictly validates the two supported VGA installations. Directory names are deliberately
/// ignored: a candidate must contain GAMEPC, a VGA zone file, and an executable accepted by
/// the editor's existing verified signature/layout detectors.
/// </summary>
internal static class GameInstallationValidator
{
    public static bool TryValidate(string? directory, InstallationDiscoverySource source, out GameInstallation? installation)
    {
        installation = null;
        if (string.IsNullOrWhiteSpace(directory)) return false;

        string normalized;
        try { normalized = InstallationPathNormalizer.Normalize(directory); }
        catch (Exception) { return false; }
        if (!Directory.Exists(normalized) || !File.Exists(Path.Combine(normalized, "GAMEPC"))) return false;

        try
        {
            bool hasVgaZone = Directory.EnumerateFiles(normalized, "*2.VGA", SearchOption.TopDirectoryOnly)
                .Any(path => Path.GetFileName(path).Length == 7 &&
                             char.IsDigit(Path.GetFileName(path)[0]) &&
                             char.IsDigit(Path.GetFileName(path)[1]));
            if (!hasVgaZone) return false;

            string runIt = Path.Combine(normalized, "RUNIT.EXE");
            if (RunItBootstrapService.DetectState(runIt) is RunItBootstrapState.OriginalPacked or
                RunItBootstrapState.CanonicalUnpackedAscii98 or RunItBootstrapState.ExtendedCp852)
            {
                installation = new GameInstallation(ElviraGameProfile.Elvira2, normalized, source);
                return true;
            }

            string runVga = Path.Combine(normalized, "RUNVGA.EXE");
            if (RunVgaBootstrapService.DetectState(runVga) is RunVgaBootstrapState.OriginalPacked or
                RunVgaBootstrapState.UnpackedBaseline or RunVgaBootstrapState.ExtendedCp852V5)
            {
                installation = new GameInstallation(ElviraGameProfile.Elvira1, normalized, source);
                return true;
            }
        }
        catch (UnauthorizedAccessException) { }
        catch (IOException) { }

        // RUNEGA on its own intentionally reaches this unsupported result.
        return false;
    }
}

internal sealed record InstallationSettings(IReadOnlyList<SavedInstallation> KnownInstallations, string? LastSelectedPath)
{
    public static InstallationSettings Empty { get; } = new(Array.Empty<SavedInstallation>(), null);
}

internal sealed record SavedInstallation(string Path, InstallationDiscoverySource Source);

/// <summary>Small per-user settings store; it never writes into an installation directory.</summary>
internal sealed class InstallationSettingsStore
{
    private readonly string _settingsPath;

    public InstallationSettingsStore(string? settingsPath = null)
    {
        _settingsPath = settingsPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Pi1ElviraVgaEditor",
            "installations.json");
    }

    public string SettingsPath => _settingsPath;

    public InstallationSettings Load()
    {
        try
        {
            if (!File.Exists(_settingsPath)) return InstallationSettings.Empty;
            PersistedInstallationSettings? saved = JsonSerializer.Deserialize<PersistedInstallationSettings>(File.ReadAllText(_settingsPath));
            if (saved is null) return InstallationSettings.Empty;
            var known = (saved.KnownInstallations ?? [])
                .Where(item => !string.IsNullOrWhiteSpace(item.Path))
                .Select(item => new SavedInstallation(item.Path, item.Source))
                .ToArray();
            return new InstallationSettings(known, saved.LastSelectedPath);
        }
        catch (IOException) { return InstallationSettings.Empty; }
        catch (JsonException) { return InstallationSettings.Empty; }
    }

    public void Save(IEnumerable<GameInstallation> installations, string? lastSelectedPath)
    {
        var unique = installations
            .GroupBy(item => item.NormalizedPath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .Select(item => new SavedInstallation(item.InstallationPath, item.DiscoverySource))
            .ToArray();
        // A null last selection is the explicit persisted representation of the
        // selector's "no supported game" sentinel. Never serialize an empty path.
        var data = new PersistedInstallationSettings
        {
            KnownInstallations = unique,
            LastSelectedPath = string.IsNullOrWhiteSpace(lastSelectedPath) ? null : lastSelectedPath
        };
        string? directory = Path.GetDirectoryName(_settingsPath);
        if (string.IsNullOrWhiteSpace(directory)) throw new IOException("The application settings directory is unavailable.");
        Directory.CreateDirectory(directory);
        string temporary = Path.Combine(directory, ".installations." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
            File.Move(temporary, _settingsPath, true);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }
    }

    private sealed class PersistedInstallationSettings
    {
        public SavedInstallation[]? KnownInstallations { get; set; }
        public string? LastSelectedPath { get; set; }
    }
}

/// <summary>Bounded, directory-only discovery. It never performs recursive fixed-drive scanning.</summary>
internal static class InstallationDiscoveryService
{
    internal static int FastDiscoveryCallCount { get; private set; }

    internal static void ResetDiagnostics() => FastDiscoveryCallCount = 0;

    public static IReadOnlyList<GameInstallation> DiscoverFast(IEnumerable<string>? rememberedPaths = null, IEnumerable<string>? knownRoots = null)
    {
        FastDiscoveryCallCount++;
        var candidates = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string path in rememberedPaths ?? []) AddCandidate(candidates, path);
        foreach (string root in knownRoots ?? GetDefaultRoots())
        {
            AddCandidate(candidates, root);
            foreach (string child in EnumerateDirectories(root))
            {
                AddCandidate(candidates, child);
                foreach (string grandchild in EnumerateDirectories(child)) AddCandidate(candidates, grandchild);
            }
        }

        var installations = new List<GameInstallation>();
        foreach (string candidate in candidates)
            if (GameInstallationValidator.TryValidate(candidate, InstallationDiscoverySource.FastDiscovery, out GameInstallation? installation) && installation is not null)
                installations.Add(installation);

        return installations
            .GroupBy(item => item.NormalizedPath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .OrderBy(item => item.Game)
            .ThenBy(item => item.InstallationPath, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static IEnumerable<string> GetDefaultRoots()
    {
        var roots = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        string systemRoot = Path.GetPathRoot(Environment.SystemDirectory) ?? string.Empty;
        if (!string.IsNullOrWhiteSpace(systemRoot))
        {
            roots.Add(Path.Combine(systemRoot, "GOG Games"));
            roots.Add(Path.Combine(systemRoot, "Games"));
            roots.Add(Path.Combine(systemRoot, "Games", "GOG"));
        }
        foreach (Environment.SpecialFolder folder in new[] { Environment.SpecialFolder.ProgramFiles, Environment.SpecialFolder.ProgramFilesX86 })
        {
            string path = Environment.GetFolderPath(folder);
            if (!string.IsNullOrWhiteSpace(path))
            {
                roots.Add(Path.Combine(path, "GOG Galaxy", "Games"));
                roots.Add(Path.Combine(path, "GOG Games"));
            }
        }
        return roots;
    }

    private static IEnumerable<string> EnumerateDirectories(string root)
    {
        try { return Directory.Exists(root) ? Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly).ToArray() : []; }
        catch (UnauthorizedAccessException) { return []; }
        catch (IOException) { return []; }
    }

    private static void AddCandidate(ISet<string> candidates, string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        try { candidates.Add(InstallationPathNormalizer.Normalize(path)); }
        catch (Exception) { }
    }
}
