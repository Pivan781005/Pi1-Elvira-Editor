using System.Text;

namespace ElviraVgaEditor;

internal sealed record VariantEntry(string DisplayName, string DataFile, bool Enabled, int Order);

internal sealed record VariantEntryStatus(VariantEntry Entry, bool IsAvailable);

/// <summary>
/// In-memory, installation-local list of explicit launchable data-file variants.
/// It never discovers filename patterns and never touches a data file while loading config.
/// </summary>
internal sealed class VariantCatalog
{
    private readonly List<VariantEntry> _entries;

    internal VariantCatalog(string installationDirectory, IEnumerable<VariantEntry> entries, bool configurationExists, IEnumerable<string>? loadWarnings = null)
    {
        InstallationDirectory = Path.GetFullPath(installationDirectory);
        _entries = entries.ToList();
        ConfigurationExists = configurationExists;
        LoadWarnings = (loadWarnings ?? []).ToArray();
        NormalizeOrder();
    }

    public string InstallationDirectory { get; }
    public bool ConfigurationExists { get; }
    public IReadOnlyList<string> LoadWarnings { get; }
    public IReadOnlyList<VariantEntry> Entries => _entries;

    public VariantEntry Add(string displayName, string dataFile, bool enabled = true)
    {
        VariantEntry entry = CreateValidated(displayName, dataFile, enabled, _entries.Count + 1, exceptDataFile: null);
        _entries.Add(entry);
        NormalizeOrder();
        return _entries[^1];
    }

    public void Edit(string existingDataFile, string displayName, string dataFile, bool enabled)
    {
        int index = FindIndex(existingDataFile);
        if (index < 0) throw new InvalidOperationException($"Variant was not found: {existingDataFile}");
        VariantEntry old = _entries[index];
        _entries[index] = CreateValidated(displayName, dataFile, enabled, old.Order, old.DataFile);
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
        (_entries[index], _entries[target]) = (_entries[target], _entries[index]);
        NormalizeOrder();
        return true;
    }

    private int FindIndex(string dataFile) => _entries.FindIndex(e => e.DataFile.Equals(Path.GetFileName(dataFile), StringComparison.OrdinalIgnoreCase));

    private VariantEntry CreateValidated(string displayName, string dataFile, bool enabled, int order, string? exceptDataFile)
    {
        string name = displayName.Trim();
        if (name.Length == 0) throw new InvalidOperationException("Variant display name is required.");
        if (!VariantConfigurationService.IsValidDataFileName(dataFile))
            throw new InvalidOperationException($"Variant data file is not a DOS-compatible filename: {dataFile}");
        string file = Path.GetFileName(dataFile);
        if (_entries.Any(e => !e.DataFile.Equals(exceptDataFile, StringComparison.OrdinalIgnoreCase) && e.DataFile.Equals(file, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A variant already refers to data file {file}.");
        if (_entries.Any(e => !e.DataFile.Equals(exceptDataFile, StringComparison.OrdinalIgnoreCase) && e.DisplayName.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"A variant already uses display name {name}.");
        return new VariantEntry(name, file, enabled, order);
    }
}

internal static class VariantConfigurationService
{
    internal const string ConfigFileName = "ELVIRA_MODS.INI";
    private static readonly UTF8Encoding Utf8 = new(false);

    public static VariantCatalog Load(string installationDirectory)
    {
        string directory = Path.GetFullPath(installationDirectory);
        string path = Path.Combine(directory, ConfigFileName);
        if (!File.Exists(path))
        {
            VariantEntry[] defaults = File.Exists(Path.Combine(directory, "GAMEPC"))
                ? [new VariantEntry("English", "GAMEPC", true, 1)]
                : [];
            return new VariantCatalog(directory, defaults, configurationExists: false);
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
            candidates.Add((new VariantEntry(trimmedName, Path.GetFileName(dataFile), enabled, order), sequence));
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
        return new VariantCatalog(directory, accepted, configurationExists: true, warnings);
    }

    public static void Save(VariantCatalog catalog)
    {
        catalog.NormalizeOrder();
        string path = Path.Combine(catalog.InstallationDirectory, ConfigFileName);
        var lines = new List<string> { "[Variants]", $"Count={catalog.Entries.Count}" };
        foreach (VariantEntry entry in catalog.Entries)
        {
            lines.Add(string.Empty);
            lines.Add($"[Variant{entry.Order}]");
            lines.Add($"Name={entry.DisplayName}");
            lines.Add($"DataFile={entry.DataFile}");
            lines.Add($"Enabled={(entry.Enabled ? "true" : "false")}");
            lines.Add($"Order={entry.Order}");
        }
        File.WriteAllText(path, string.Join(Environment.NewLine, lines) + Environment.NewLine, Utf8);
    }

    public static bool IsValidDataFileName(string? dataFile) =>
        !string.IsNullOrWhiteSpace(dataFile) && Path.GetFileName(dataFile) == dataFile && GameDataFileService.IsDos83FileName(dataFile);

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
