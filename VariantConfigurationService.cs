using System.Text;

namespace Pi1ElviraEditor;

internal sealed record VariantEntry(string DisplayName, string DataFile, bool Enabled, int Order, string Code = "", string ExeFile = "")
{
    /// <summary>Stable human-friendly label for selection controls
    /// (never the record debug representation): DisplayName (Code) — ExeFile.</summary>
    internal string DisplayLabel => string.IsNullOrWhiteSpace(Code) || string.IsNullOrWhiteSpace(ExeFile)
        ? $"{DisplayName} ({DataFile})"
        : $"{DisplayName} ({Code}) — {ExeFile}";

    public override string ToString() => DisplayLabel;
}

internal sealed record VariantEntryStatus(VariantEntry Entry, bool IsAvailable);

/// <summary>
/// In-memory, installation-local list of explicit launchable data-file variants.
/// It never discovers filename patterns and never touches a data file while loading config.
/// </summary>
internal sealed class VariantCatalog
{
    private readonly List<VariantEntry> _entries;

    internal VariantCatalog(string installationDirectory, ElviraGameProfile game, IEnumerable<VariantEntry> entries, bool configurationExists, IEnumerable<string>? loadWarnings = null)
    {
        InstallationDirectory = Path.GetFullPath(installationDirectory);
        Game = game;
        _entries = entries.Select(entry => VariantNaming.Normalize(entry, game)).ToList();
        ConfigurationExists = configurationExists;
        LoadWarnings = (loadWarnings ?? []).ToArray();
        NormalizeOrder();
    }

    internal VariantCatalog(string installationDirectory, IEnumerable<VariantEntry> entries, bool configurationExists, IEnumerable<string>? loadWarnings = null)
        : this(installationDirectory, ElviraGameProfile.Unknown, entries, configurationExists, loadWarnings)
    {
    }

    public string InstallationDirectory { get; }
    public ElviraGameProfile Game { get; }
    public bool ConfigurationExists { get; }
    public IReadOnlyList<string> LoadWarnings { get; }
    public IReadOnlyList<VariantEntry> Entries => _entries;
    public LauncherSettings LauncherAuthoring { get; set; } = LauncherSettings.Default;

    public VariantEntry Add(string displayName, string code, string dataFile, string exeFile, bool enabled = true)
    {
        VariantEntry entry = CreateValidated(displayName, code, dataFile, exeFile, enabled, _entries.Count + 1, exceptDataFile: null);
        _entries.Add(entry);
        NormalizeOrder();
        return _entries[^1];
    }

    public VariantEntry Add(string displayName, string dataFile, bool enabled = true) =>
        Add(displayName, VariantNaming.DeriveCode(dataFile), dataFile, VariantNaming.DeriveExeFile(VariantNaming.DeriveCode(dataFile), Game), enabled);

    public void Edit(string existingDataFile, string displayName, string dataFile, bool enabled)
    {
        int index = FindIndex(existingDataFile);
        if (index < 0) throw new InvalidOperationException($"Variant was not found: {existingDataFile}");
        VariantEntry old = _entries[index];
        _entries[index] = CreateValidated(displayName, old.Code, dataFile, old.ExeFile, enabled, old.Order, old.DataFile);
        NormalizeOrder();
    }

    public bool Remove(string dataFile)
    {
        int index = FindIndex(dataFile);
        if (index < 0) return false;
        _entries.RemoveAt(index);
        NormalizeOrder();
        return true;
    }

    public void SetEnabled(string dataFile, bool enabled)
    {
        int index = FindIndex(dataFile);
        if (index < 0) throw new InvalidOperationException($"Variant was not found: {dataFile}");
        _entries[index] = _entries[index] with { Enabled = enabled };
    }

    public bool MoveUp(string dataFile) => Move(dataFile, -1);
    public bool MoveDown(string dataFile) => Move(dataFile, 1);

    public bool CanMoveUp(string dataFile) => CanMove(dataFile, -1);
    public bool CanMoveDown(string dataFile) => CanMove(dataFile, 1);

    public VariantEntry? FindByDataFile(string dataFile)
    {
        string name = Path.GetFileName(dataFile);
        return _entries.FirstOrDefault(e => e.DataFile.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public VariantEntry? FindByDataFilePath(string currentDataFilePath)
    {
        if (string.IsNullOrWhiteSpace(currentDataFilePath)) return null;
        return FindByDataFile(Path.GetFileName(currentDataFilePath));
    }

    public VariantEntryStatus GetStatus(VariantEntry entry) =>
        new(entry, File.Exists(Path.Combine(InstallationDirectory, entry.DataFile)));

    public void NormalizeOrder()
    {
        _entries.Sort((a, b) => a.Order != b.Order
            ? a.Order.CompareTo(b.Order)
            : StringComparer.OrdinalIgnoreCase.Compare(a.DataFile, b.DataFile));
        for (int i = 0; i < _entries.Count; i++) _entries[i] = _entries[i] with { Order = i + 1 };
    }

    private bool Move(string dataFile, int delta)
    {
        int index = FindIndex(dataFile);
        int target = index + delta;
        if (index < 0 || target < 0 || target >= _entries.Count) return false;
        int movedOrder = _entries[index].Order;
        _entries[index] = _entries[index] with { Order = _entries[target].Order };
        _entries[target] = _entries[target] with { Order = movedOrder };
        NormalizeOrder();
        return true;
    }

    private bool CanMove(string dataFile, int delta)
    {
        int index = FindIndex(dataFile);
        int target = index + delta;
        return index >= 0 && target >= 0 && target < _entries.Count;
    }

    private int FindIndex(string dataFile) => _entries.FindIndex(e => e.DataFile.Equals(Path.GetFileName(dataFile), StringComparison.OrdinalIgnoreCase));

    private VariantEntry CreateValidated(string displayName, string code, string dataFile, string exeFile, bool enabled, int order, string? exceptDataFile)
    {
        string name = displayName.Trim();
        if (name.Length == 0) throw new InvalidOperationException("Variant display name is required.");
        if (!VariantConfigurationService.IsValidDataFileName(dataFile))
            throw new InvalidOperationException($"Variant data file is not a DOS-compatible filename: {dataFile}");
        string file = Path.GetFileName(dataFile);
        string normalizedCode = VariantNaming.ValidateCode(code);
        string normalizedExe = VariantNaming.ValidateExeFile(exeFile);
        if (_entries.Any(e => !e.DataFile.Equals(exceptDataFile, StringComparison.OrdinalIgnoreCase) && e.DataFile.Equals(file, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A variant already refers to data file {file}.");
        if (_entries.Any(e => !e.DataFile.Equals(exceptDataFile, StringComparison.OrdinalIgnoreCase) && e.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A variant already uses display name {name}.");
        if (_entries.Any(e => !e.DataFile.Equals(exceptDataFile, StringComparison.OrdinalIgnoreCase) && e.ExeFile.Equals(normalizedExe, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A variant already refers to executable {normalizedExe}.");
        return new VariantEntry(name, file, enabled, order, normalizedCode, normalizedExe);
    }
}

internal static class VariantConfigurationService
{
    internal const string ConfigFileName = "ELVIRA_MODS.INI";
    private static readonly UTF8Encoding Utf8 = new(false);

    public static VariantCatalog Load(string installationDirectory, ElviraGameProfile game = ElviraGameProfile.Unknown)
    {
        string directory = Path.GetFullPath(installationDirectory);
        string path = Path.Combine(directory, ConfigFileName);
        if (!File.Exists(path))
        {
            var defaults = new List<VariantEntry>();
            if (File.Exists(Path.Combine(directory, "GAMEPC")))
                defaults.Add(VariantNaming.Create("English", "EN", game, true, 1));
            foreach (string dataPath in Directory.EnumerateFiles(directory, "GAMEPC??", SearchOption.TopDirectoryOnly))
            {
                string dataFile = Path.GetFileName(dataPath).ToUpperInvariant();
                if (dataFile.Length != 8) continue;
                string code;
                try { code = VariantNaming.ValidateCode(dataFile[6..]); }
                catch { continue; }
                VariantEntry entry = VariantNaming.Create(VariantDisplayName(code), code, game, true, defaults.Count + 1);
                if (File.Exists(Path.Combine(directory, entry.ExeFile))) defaults.Add(entry);
            }
            return new VariantCatalog(directory, game, defaults, configurationExists: false);
        }

        var sections = ParseIni(File.ReadAllLines(path, Utf8));
        var warnings = new List<string>();
        var candidates = new List<(VariantEntry Entry, int Sequence)>();
        int sequence = 0;
        foreach ((string section, Dictionary<string, string> values) in sections)
        {
            if (!section.StartsWith("Variant", StringComparison.OrdinalIgnoreCase)) continue;
            sequence++;
            if (!values.TryGetValue("Name", out string? name) || !values.TryGetValue("DataFile", out string? dataFile))
            {
                warnings.Add($"Ignored {section}: Name and DataFile are required.");
                continue;
            }
            if (!IsValidDataFileName(dataFile))
            {
                warnings.Add($"Ignored {section}: invalid DataFile '{dataFile}'.");
                continue;
            }
            bool enabled = !values.TryGetValue("Enabled", out string? enabledText) || !enabledText.Equals("false", StringComparison.OrdinalIgnoreCase);
            int order = int.TryParse(values.GetValueOrDefault("Order"), out int parsedOrder) && parsedOrder > 0 ? parsedOrder : sequence;
            string trimmedName = name.Trim();
            if (trimmedName.Length == 0)
            {
                warnings.Add($"Ignored {section}: Name is empty.");
                continue;
            }
            string code = values.GetValueOrDefault("Code") ?? VariantNaming.DeriveCode(dataFile);
            string exeFile = values.GetValueOrDefault("ExeFile") ?? VariantNaming.DeriveExeFile(code, game);
            try { candidates.Add((VariantNaming.Normalize(new VariantEntry(trimmedName, Path.GetFileName(dataFile), enabled, order, code, exeFile), game), sequence)); }
            catch (InvalidOperationException ex) { warnings.Add($"Ignored {section}: {ex.Message}"); }
        }

        var accepted = new List<VariantEntry>();
        foreach ((VariantEntry entry, _) in candidates.OrderBy(c => c.Entry.Order).ThenBy(c => c.Sequence))
        {
            if (accepted.Any(e => e.DataFile.Equals(entry.DataFile, StringComparison.OrdinalIgnoreCase)))
            {
                warnings.Add($"Ignored duplicate DataFile '{entry.DataFile}'.");
                continue;
            }
            if (accepted.Any(e => e.DisplayName.Equals(entry.DisplayName, StringComparison.OrdinalIgnoreCase)))
            {
                warnings.Add($"Ignored duplicate Name '{entry.DisplayName}'.");
                continue;
            }
            accepted.Add(entry);
        }
        var catalog = new VariantCatalog(directory, game, accepted, configurationExists: true, warnings);
        Dictionary<string, string>? root = sections.FirstOrDefault(s => s.Section.Equals("Variants", StringComparison.OrdinalIgnoreCase)).Values;
        if (root is not null)
        {
            string launcher = root.GetValueOrDefault("LauncherFile") ?? LauncherSettings.DefaultLauncherFile;
            string modder = root.GetValueOrDefault("ModderName") ?? LauncherSettings.DefaultModderName;
            string defaultVariant = root.GetValueOrDefault("DefaultVariant") ?? "GAMEPC";
            // Phase 4D: legacy AutoStart/AutoStartDelay keys are deliberately accepted then ignored.
            try { catalog.LauncherAuthoring = LauncherService.ValidateSettings(new LauncherSettings(launcher, modder, defaultVariant)); }
            catch (InvalidDataException ex) { warnings.Add("Ignored launcher metadata: " + ex.Message); }
        }
        return catalog;
    }

    public static void Save(VariantCatalog catalog)
    {
        catalog.NormalizeOrder();
        string path = Path.Combine(catalog.InstallationDirectory, ConfigFileName);
        catalog.LauncherAuthoring = LauncherService.ValidateSettings(catalog.LauncherAuthoring);
        var lines = new List<string> { "[Variants]", $"Count={catalog.Entries.Count}", $"LauncherFile={catalog.LauncherAuthoring.LauncherFile}", $"ModderName={catalog.LauncherAuthoring.ModderName}", $"DefaultVariant={catalog.LauncherAuthoring.DefaultVariant}" };
        foreach (VariantEntry entry in catalog.Entries)
        {
            lines.Add(string.Empty);
            lines.Add($"[Variant{entry.Order}]");
            lines.Add($"Name={entry.DisplayName}");
            lines.Add($"Code={entry.Code}");
            lines.Add($"DataFile={entry.DataFile}");
            lines.Add($"ExeFile={entry.ExeFile}");
            lines.Add($"Enabled={(entry.Enabled ? "true" : "false")}");
            lines.Add($"Order={entry.Order}");
        }
        File.WriteAllText(path, string.Join(Environment.NewLine, lines) + Environment.NewLine, Utf8);
    }

    public static bool IsValidDataFileName(string? dataFile) =>
        !string.IsNullOrWhiteSpace(dataFile) && Path.GetFileName(dataFile) == dataFile && GameDataFileService.IsDos83FileName(dataFile);

    private static string VariantDisplayName(string code) => code switch
    {
        "EN" => "English",
        "SK" => "Slovak",
        "CZ" => "Czech",
        _ => code
    };

    private static List<(string Section, Dictionary<string, string> Values)> ParseIni(IEnumerable<string> lines)
    {
        var sections = new List<(string, Dictionary<string, string>)>();
        string? section = null;
        Dictionary<string, string>? values = null;
        foreach (string raw in lines)
        {
            string line = raw.Trim();
            if (line.Length == 0 || line.StartsWith(';') || line.StartsWith('#')) continue;
            if (line.StartsWith('[') && line.EndsWith(']'))
            {
                section = line[1..^1].Trim();
                values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                sections.Add((section, values));
                continue;
            }
            int separator = line.IndexOf('=');
            if (section is null || values is null || separator <= 0) continue;
            values[line[..separator].Trim()] = line[(separator + 1)..].Trim();
        }
        return sections;
    }
}

internal static class VariantNaming
{
    public static VariantEntry Create(string name, string code, ElviraGameProfile game, bool enabled, int order)
    {
        string normalizedCode = ValidateCode(code);
        string data = normalizedCode == "EN" ? "GAMEPC" : "GAMEPC" + normalizedCode;
        return new VariantEntry(name.Trim(), data, enabled, order, normalizedCode, DeriveExeFile(normalizedCode, game));
    }

    public static VariantEntry Normalize(VariantEntry entry, ElviraGameProfile game)
    {
        string code = string.IsNullOrWhiteSpace(entry.Code) ? DeriveCode(entry.DataFile) : ValidateCode(entry.Code);
        string exe = string.IsNullOrWhiteSpace(entry.ExeFile) ? DeriveExeFile(code, game) : ValidateExeFile(entry.ExeFile);
        return entry with { Code = code, ExeFile = exe };
    }

    public static string ValidateCode(string? code)
    {
        string value = (code ?? string.Empty).Trim().ToUpperInvariant();
        if (value.Length is < 2 or > 3 || !value.All(char.IsLetterOrDigit))
            throw new InvalidOperationException("Variant code must contain 2-3 DOS-safe letters or digits.");
        return value;
    }

    public static string ValidateExeFile(string? exeFile)
    {
        string value = Path.GetFileName(exeFile ?? string.Empty).ToUpperInvariant();
        if (value != exeFile?.ToUpperInvariant() || !value.EndsWith(".EXE", StringComparison.Ordinal) || !GameDataFileService.IsDos83FileName(value))
            throw new InvalidOperationException("Variant executable must be a DOS-compatible .EXE filename.");
        return value;
    }

    public static string DeriveCode(string dataFile)
    {
        string file = Path.GetFileName(dataFile).ToUpperInvariant();
        return file.Equals("GAMEPC", StringComparison.Ordinal) ? "EN" : file.StartsWith("GAMEPC", StringComparison.Ordinal) ? ValidateCode(file[6..]) : "EN";
    }

    public static string DeriveExeFile(string code, ElviraGameProfile game)
    {
        string stem = game == ElviraGameProfile.Elvira2 ? "RUNIT" : "RUNVGA";
        return code == "EN" ? stem + ".EXE" : stem + code + ".EXE";
    }
}
