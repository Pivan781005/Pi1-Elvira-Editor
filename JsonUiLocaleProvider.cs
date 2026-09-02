using System.Text.Json;

namespace ElviraVgaEditor;

internal sealed record UiLocaleDescriptor(string Id, string SourcePath)
{
    public override string ToString() => Id;
}

internal sealed record UiLocaleDiscoveryDiagnostic(string SourcePath, string Detail);
internal enum UiLocaleLookupSource { SelectedLocale, EnglishFallback, MissingKey }
internal sealed record UiLocaleLookup(string Value, UiLocaleLookupSource Source);

internal sealed class JsonUiLocaleProvider
{
    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> _locales;

    private JsonUiLocaleProvider(
        IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> locales,
        IReadOnlyList<UiLocaleDescriptor> availableLocales,
        IReadOnlyList<UiLocaleDiscoveryDiagnostic> diagnostics)
    {
        _locales = locales;
        AvailableLocales = availableLocales;
        Diagnostics = diagnostics;
    }

    internal IReadOnlyList<UiLocaleDescriptor> AvailableLocales { get; }
    internal IReadOnlyList<UiLocaleDiscoveryDiagnostic> Diagnostics { get; }

    internal static JsonUiLocaleProvider Discover(string localeDirectory) =>
        DiscoverFiles(Directory.Exists(localeDirectory)
            ? Directory.EnumerateFiles(localeDirectory, "*.json", SearchOption.TopDirectoryOnly)
            : Array.Empty<string>());

    // Separate file enumeration makes duplicate-identity handling testable even
    // on a case-insensitive filesystem where two identical names cannot coexist.
    internal static JsonUiLocaleProvider DiscoverFiles(IEnumerable<string> paths)
    {
        var locales = new Dictionary<string, IReadOnlyDictionary<string, string>>(StringComparer.Ordinal);
        var descriptors = new List<UiLocaleDescriptor>();
        var diagnostics = new List<UiLocaleDiscoveryDiagnostic>();
        foreach (string path in paths.OrderBy(Path.GetFileName, StringComparer.Ordinal).ThenBy(path => path, StringComparer.Ordinal))
        {
            string id = Path.GetFileNameWithoutExtension(path).Trim().ToLowerInvariant();
            if (!IsLocaleIdentity(id))
            {
                diagnostics.Add(new UiLocaleDiscoveryDiagnostic(path, "Ignored: filename is not a valid locale identity."));
                continue;
            }
            try
            {
                IReadOnlyDictionary<string, string> values = LoadLocaleFile(path);
                if (values.Count == 0) throw new InvalidDataException("Locale file contains no keys.");
                if (!locales.TryAdd(id, values))
                {
                    diagnostics.Add(new UiLocaleDiscoveryDiagnostic(path, $"Skipped: duplicate locale identity '{id}'."));
                    continue;
                }
                descriptors.Add(new UiLocaleDescriptor(id, path));
            }
            catch (InvalidDataException ex)
            {
                diagnostics.Add(new UiLocaleDiscoveryDiagnostic(path, "Skipped: " + ex.Message));
            }
        }
        UiLocaleDescriptor[] ordered = descriptors.OrderBy(locale => locale.Id, StringComparer.Ordinal).ToArray();
        if (!locales.ContainsKey("en"))
            diagnostics.Add(new UiLocaleDiscoveryDiagnostic("Locales", "Canonical English locale 'en' was not discovered."));
        return new JsonUiLocaleProvider(locales, ordered, diagnostics);
    }

    internal bool TryGet(string localeId, string key, out string value)
    {
        value = string.Empty;
        return _locales.TryGetValue(localeId, out IReadOnlyDictionary<string, string>? values) && values.TryGetValue(key, out value!);
    }

    internal UiLocaleLookup Resolve(string localeId, string key)
    {
        if (TryGet(localeId, key, out string selected)) return new UiLocaleLookup(selected, UiLocaleLookupSource.SelectedLocale);
        if (TryGet("en", key, out string english)) return new UiLocaleLookup(english, UiLocaleLookupSource.EnglishFallback);
        return new UiLocaleLookup(key, UiLocaleLookupSource.MissingKey);
    }

    internal static IReadOnlyDictionary<string, string> LoadLocaleFile(string path)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(path));
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                throw new InvalidDataException($"Locale file '{Path.GetFileName(path)}' must contain one JSON object.");

            var values = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                if (!IsSemanticKey(property.Name))
                    throw new InvalidDataException($"Locale file '{Path.GetFileName(path)}' has invalid key '{property.Name}'.");
                if (property.Value.ValueKind != JsonValueKind.String)
                    throw new InvalidDataException($"Locale file '{Path.GetFileName(path)}' key '{property.Name}' must have a string value.");
                if (!values.TryAdd(property.Name, property.Value.GetString() ?? string.Empty))
                    throw new InvalidDataException($"Locale file '{Path.GetFileName(path)}' contains duplicate key '{property.Name}'.");
            }
            return values;
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Locale file '{Path.GetFileName(path)}' is malformed JSON.", ex);
        }
        catch (IOException ex)
        {
            throw new InvalidDataException($"Locale file '{Path.GetFileName(path)}' could not be read.", ex);
        }
    }

    private static bool IsLocaleIdentity(string id) =>
        !string.IsNullOrWhiteSpace(id) && id.Length <= 24 && char.IsLetter(id[0]) &&
        id.All(character => char.IsLetterOrDigit(character) || character == '-');

    internal static bool IsSemanticKey(string key) =>
        !string.IsNullOrWhiteSpace(key) && char.IsLetter(key[0]) &&
        key.All(character => char.IsLetterOrDigit(character) || character is '.' or '_');
}
