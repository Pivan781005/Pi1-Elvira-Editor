using System.Text.Json;
using Microsoft.Win32;

namespace Pi1ElviraEditor;

internal sealed record DosRuntimeSelectedHost(string ExecutablePath, DosRuntimeKind Kind);

/// <summary>Per-user DOS host selection store beside installations.json. It
/// never writes into an installation directory.</summary>
internal sealed class DosRuntimeSettingsStore
{
    private readonly string _settingsPath;

    public DosRuntimeSettingsStore(string? settingsPath = null)
    {
        _settingsPath = settingsPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Pi1ElviraVgaEditor",
            "dos-runtime.json");
    }

    public string SettingsPath => _settingsPath;

    public DosRuntimeSelectedHost? Load()
    {
        try
        {
            if (!File.Exists(_settingsPath)) return null;
            Persisted? saved = JsonSerializer.Deserialize<Persisted>(File.ReadAllText(_settingsPath));
            if (saved is null || string.IsNullOrWhiteSpace(saved.ExecutablePath)) return null;
            return new DosRuntimeSelectedHost(saved.ExecutablePath, saved.Kind);
        }
        catch (IOException) { return null; }
        catch (JsonException) { return null; }
    }

    public void Save(string executablePath, DosRuntimeKind kind)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(executablePath);
        var data = new Persisted { ExecutablePath = executablePath, Kind = kind };
        string? directory = Path.GetDirectoryName(_settingsPath);
        if (string.IsNullOrWhiteSpace(directory)) throw new IOException("The application settings directory is unavailable.");
        Directory.CreateDirectory(directory);
        string temporary = Path.Combine(directory, ".dos-runtime." + Guid.NewGuid().ToString("N") + ".tmp");
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

    public void Clear()
    {
        try { if (File.Exists(_settingsPath)) File.Delete(_settingsPath); } catch (IOException) { }
    }

    private sealed class Persisted
    {
        public string? ExecutablePath { get; set; }
        public DosRuntimeKind Kind { get; set; }
    }
}

/// <summary>Filesystem seam for bounded DOS runtime discovery. Tests inject a
/// fake; production touches only fixed candidate paths (never a VARIANTS
/// walk, never a disk scan).</summary>
internal sealed record DosRuntimeUninstallEntry(string DisplayName, string? InstallLocation, string? DisplayIcon);

internal interface IDosRuntimeFileSystem
{
    bool FileExists(string path);
    IEnumerable<string> EnumerateTopFiles(string directory, string pattern);
    string? GetEnvironmentVariable(string name);
    string GetProgramFiles();
    string GetProgramFilesX86();
    string GetLocalAppData();
    IEnumerable<DosRuntimeUninstallEntry> GetUninstallEntries();
}

internal sealed class SystemDosRuntimeFileSystem : IDosRuntimeFileSystem
{
    public bool FileExists(string path)
    {
        try { return File.Exists(path); } catch { return false; }
    }

    public IEnumerable<string> EnumerateTopFiles(string directory, string pattern)
    {
        try { return Directory.EnumerateFiles(directory, pattern, SearchOption.TopDirectoryOnly).ToArray(); }
        catch { return []; }
    }

    public string? GetEnvironmentVariable(string name) => Environment.GetEnvironmentVariable(name);

    public string GetProgramFiles() => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

    public string GetProgramFilesX86() => Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

    public string GetLocalAppData() => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public IEnumerable<DosRuntimeUninstallEntry> GetUninstallEntries()
    {
        var entries = new List<DosRuntimeUninstallEntry>();
        foreach (RegistryKey root in new[] { Registry.LocalMachine, Registry.CurrentUser })
        {
            foreach (string view in new[] { @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall" })
            {
                try
                {
                    using RegistryKey? key = root.OpenSubKey(view);
                    if (key is null) continue;
                    foreach (string sub in key.GetSubKeyNames())
                    {
                        try
                        {
                            using RegistryKey? app = key.OpenSubKey(sub);
                            if (app is null) continue;
                            string? name = app.GetValue("DisplayName") as string;
                            if (string.IsNullOrWhiteSpace(name)) continue;
                            entries.Add(new(name, app.GetValue("InstallLocation") as string, app.GetValue("DisplayIcon") as string));
                        }
                        catch { }
                    }
                }
                catch { }
            }
        }
        return entries;
    }
}

/// <summary>Bounded, deterministic DOS runtime discovery with session-level
/// caching. It inspects fixed known locations only: GOG-bundled paths near
/// the installation, PATH entries, fixed Program Files relatives, uninstall
/// registration, and the remembered selection. It never walks VARIANTS, so
/// foreign/pre-release residue can never become a host.</summary>
internal sealed class DosRuntimeDiscoveryService
{
    internal static readonly IReadOnlyList<string> HostFileNames = ["dosbox.exe", "dosbox-x.exe"];

    private readonly IDosRuntimeFileSystem _files;
    private readonly IDosRuntimeProbeRunner _prober;
    private readonly Func<string, DosRuntimeHostEvidence> _evidenceReader;

    private string? _cachedRoot;
    private IReadOnlyList<DosRuntimeCandidate>? _cached;

    internal int ProbeCountForTest => _probeCount;
    private int _probeCount;

    public DosRuntimeDiscoveryService(
        IDosRuntimeFileSystem? files = null,
        IDosRuntimeProbeRunner? prober = null,
        Func<string, DosRuntimeHostEvidence>? evidenceReader = null)
    {
        _files = files ?? new SystemDosRuntimeFileSystem();
        _prober = prober ?? new SystemDosRuntimeProbeRunner();
        _evidenceReader = evidenceReader ?? DosRuntimeHostEvidenceReader.Read;
    }

    /// <summary>Session-cached discovery. Re-probes only on explicit refresh,
    /// a changed installation root, or when the caller bypasses the cache.</summary>
    public IReadOnlyList<DosRuntimeCandidate> Discover(string? gameRoot, bool refresh = false)
    {
        string key = (gameRoot ?? string.Empty).ToUpperInvariant();
        if (!refresh && _cached is not null && _cachedRoot == key)
            return _cached;
        IReadOnlyList<DosRuntimeCandidate> found = DiscoverUncached(gameRoot);
        _cachedRoot = key;
        _cached = found;
        return found;
    }

    public void Invalidate() => _cached = null;

    private IReadOnlyList<DosRuntimeCandidate> DiscoverUncached(string? gameRoot)
    {
        var paths = new List<(string Path, DosRuntimeSource Source)>();
        if (!string.IsNullOrWhiteSpace(gameRoot))
        {
            // GOG-bundled fixed locations only. VARIANTS is never enumerated:
            // foreign residue must never become an installed host.
            foreach (string name in new[] { "DOSBox.exe", "dosbox.exe", "dosbox-x.exe" })
                AddCandidate(paths, Path.Combine(gameRoot, "DOSBOX", name), DosRuntimeSource.GogBundled);
            foreach (string file in _files.EnumerateTopFiles(gameRoot, "dosbox*.exe"))
                AddCandidate(paths, file, DosRuntimeSource.GogBundled);
        }
        string? pathVariable = _files.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrWhiteSpace(pathVariable))
        {
            foreach (string directory in pathVariable.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
            {
                string trimmed = directory.Trim().Trim('"');
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                foreach (string name in HostFileNames)
                    AddCandidate(paths, Path.Combine(trimmed, name), DosRuntimeSource.SystemPath);
            }
        }
        foreach (string root in new[] { _files.GetProgramFiles(), _files.GetProgramFilesX86(), _files.GetLocalAppData() })
        {
            if (string.IsNullOrWhiteSpace(root)) continue;
            AddCandidate(paths, Path.Combine(root, "DOSBox", "DOSBox.exe"), DosRuntimeSource.ProgramFiles);
            AddCandidate(paths, Path.Combine(root, "DOSBox Staging", "dosbox.exe"), DosRuntimeSource.ProgramFiles);
            AddCandidate(paths, Path.Combine(root, "DOSBox-X", "dosbox-x.exe"), DosRuntimeSource.ProgramFiles);
        }
        try
        {
            foreach (DosRuntimeUninstallEntry entry in _files.GetUninstallEntries())
            {
                if (!(entry.DisplayName ?? string.Empty).Contains("DOSBox", StringComparison.OrdinalIgnoreCase)) continue;
                foreach (string? baseDir in new[] { entry.InstallLocation, entry.DisplayIcon is not null ? Path.GetDirectoryName(entry.DisplayIcon) : null })
                {
                    if (string.IsNullOrWhiteSpace(baseDir)) continue;
                    foreach (string name in HostFileNames)
                        AddCandidate(paths, Path.Combine(baseDir, name), DosRuntimeSource.Registry);
                }
            }
        }
        catch { }

        var candidates = new List<DosRuntimeCandidate>();
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach ((string path, DosRuntimeSource source) in paths)
        {
            string full;
            try { full = Path.GetFullPath(path); }
            catch { continue; }
            if (!seen.Add(full)) continue;
            if (!_files.FileExists(full)) continue;
            candidates.Add(ProbeCandidate(full, source));
        }
        return candidates
            .OrderBy(item => item.Compatibility != DosRuntimeCompatibility.Compatible)
            .ThenBy(item => item.Kind)
            .ThenBy(item => item.ExecutablePath, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void AddCandidate(List<(string Path, DosRuntimeSource Source)> paths, string path, DosRuntimeSource source)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        paths.Add((path, source));
    }

    internal DosRuntimeCandidate ProbeCandidate(string executablePath, DosRuntimeSource source)
    {
        DosRuntimeHostEvidence evidence;
        try { evidence = _evidenceReader(executablePath); }
        catch { evidence = new DosRuntimeHostEvidence(Path.GetFileName(executablePath), null, null); }
        IDosRuntimeAdapter? adapter = DosRuntimeAdapters.Identify(evidence);
        if (adapter is null)
            return new(DosRuntimeKind.Unknown, Path.GetFileName(executablePath), string.Empty, executablePath, source,
                DosRuntimeCompatibility.Unrecognized, "Not a recognized DOSBox-family host.");
        _probeCount++;
        DosRuntimeProbeResult probe;
        try { probe = _prober.Probe(executablePath, adapter.VersionArguments, SystemDosRuntimeProbeRunner.DefaultTimeoutMilliseconds); }
        catch { probe = new(false, string.Empty, "Probe failed.", -1); }
        if (!probe.Success)
            return new(adapter.Kind, adapter.FamilyDisplayName, string.Empty, executablePath, source,
                DosRuntimeCompatibility.Incompatible, "Version probe failed: " + probe.Error);
        string combined = string.IsNullOrWhiteSpace(probe.Output) ? probe.Error : probe.Output;
        if (string.IsNullOrWhiteSpace(combined))
            return new(adapter.Kind, adapter.FamilyDisplayName, string.Empty, executablePath, source,
                DosRuntimeCompatibility.Incompatible, "Version probe produced no output.");
        return new(adapter.Kind, adapter.FamilyDisplayName, adapter.ParseVersion(combined), executablePath, source,
            DosRuntimeCompatibility.Compatible, "Probed successfully.");
    }

    /// <summary>Manual Browse entry point: probes exactly the user-chosen
    /// executable. Compatible hosts are accepted; anything else fails closed
    /// and never becomes runnable.</summary>
    public DosRuntimeCandidate ProbeUserSelection(string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath) || !_files.FileExists(executablePath))
            return new(DosRuntimeKind.Unknown, Path.GetFileName(executablePath), string.Empty, executablePath ?? string.Empty,
                DosRuntimeSource.UserBrowse, DosRuntimeCompatibility.Unrecognized, "Selected file does not exist.");
        string full;
        try { full = Path.GetFullPath(executablePath); }
        catch { return new(DosRuntimeKind.Unknown, Path.GetFileName(executablePath), string.Empty, executablePath, DosRuntimeSource.UserBrowse, DosRuntimeCompatibility.Unrecognized, "Selected path is invalid."); }
        return ProbeCandidate(full, DosRuntimeSource.UserBrowse);
    }
}
