using System.Text.Json;

namespace Pi1ElviraEditor;

internal enum UiLocaleValidationSeverity { Warning, Error }

internal sealed record UiLocaleValidationDiagnostic(
    UiLocaleValidationSeverity Severity,
    string Code,
    string? Key,
    string Detail);

internal sealed record UiLocaleValidationReport(
    string LocaleId,
    string SourcePath,
    bool IsCanonical,
    bool IsValid,
    int TranslatedKeys,
    int CanonicalKeyCount,
    IReadOnlyList<string> MissingKeys,
    IReadOnlyList<string> ExtraKeys,
    IReadOnlyList<UiLocaleValidationDiagnostic> Diagnostics)
{
    internal decimal CompletionPercent => CanonicalKeyCount == 0 ? 0m : decimal.Round(100m * TranslatedKeys / CanonicalKeyCount, 1);
}

internal sealed record UiLocaleValidationCatalog(
    UiLocaleValidationReport? Canonical,
    IReadOnlyList<UiLocaleValidationReport> Reports,
    IReadOnlyList<UiLocaleValidationDiagnostic> Diagnostics);

internal sealed class UiLocaleValidationService
{
    internal UiLocaleValidationCatalog ValidateDirectory(string localeDirectory) =>
        ValidateFiles(Directory.Exists(localeDirectory)
            ? Directory.EnumerateFiles(localeDirectory, "*.json", SearchOption.TopDirectoryOnly)
            : Array.Empty<string>());

    internal UiLocaleValidationCatalog ValidateFiles(IEnumerable<string> paths)
    {
        string[] orderedPaths = paths.OrderBy(Path.GetFileName, StringComparer.Ordinal).ThenBy(path => path, StringComparer.Ordinal).ToArray();
        string? canonicalPath = orderedPaths.FirstOrDefault(path => Path.GetFileName(path).Equals("en.json", StringComparison.OrdinalIgnoreCase));
        var catalogDiagnostics = new List<UiLocaleValidationDiagnostic>();
        if (canonicalPath is null)
        {
            catalogDiagnostics.Add(new(UiLocaleValidationSeverity.Error, "CanonicalMissing", null, "Canonical en.json was not found."));
            return new UiLocaleValidationCatalog(null, [], catalogDiagnostics);
        }

        ParsedLocale canonicalParsed = Parse(canonicalPath, "en", isCanonical: true, duplicateIdentity: false);
        UiLocaleValidationReport canonical = BuildReport(canonicalParsed, canonicalParsed.Values.Keys.ToHashSet(StringComparer.Ordinal));
        if (!canonical.IsValid)
            catalogDiagnostics.Add(new(UiLocaleValidationSeverity.Error, "CanonicalInvalid", null, "Canonical en.json has structural validation errors."));

        var seenIds = new HashSet<string>(StringComparer.Ordinal);
        var reports = new List<UiLocaleValidationReport>();
        foreach (string path in orderedPaths)
        {
            string id = Path.GetFileNameWithoutExtension(path).Trim().ToLowerInvariant();
            bool duplicate = !seenIds.Add(id);
            ParsedLocale parsed = path.Equals(canonicalPath, StringComparison.OrdinalIgnoreCase)
                ? canonicalParsed
                : Parse(path, id, isCanonical: false, duplicate);
            reports.Add(BuildReport(parsed, canonicalParsed.Values.Keys.ToHashSet(StringComparer.Ordinal)));
        }
        return new UiLocaleValidationCatalog(canonical, reports, catalogDiagnostics);
    }

    private static UiLocaleValidationReport BuildReport(ParsedLocale parsed, HashSet<string> canonicalKeys)
    {
        List<string> missing = canonicalKeys.Except(parsed.Values.Keys, StringComparer.Ordinal).OrderBy(key => key, StringComparer.Ordinal).ToList();
        List<string> extra = parsed.Values.Keys.Except(canonicalKeys, StringComparer.Ordinal).OrderBy(key => key, StringComparer.Ordinal).ToList();
        var diagnostics = new List<UiLocaleValidationDiagnostic>(parsed.Diagnostics);
        diagnostics.AddRange(missing.Select(key => new UiLocaleValidationDiagnostic(UiLocaleValidationSeverity.Warning, "MissingKey", key, "Translation falls back to English.")));
        diagnostics.AddRange(extra.Select(key => new UiLocaleValidationDiagnostic(UiLocaleValidationSeverity.Warning, "ExtraKey", key, "Key is not present in canonical en.json.")));
        if (parsed.IsCanonical && missing.Count != 0)
            diagnostics.Add(new(UiLocaleValidationSeverity.Error, "CanonicalIncomplete", null, "Canonical en.json is missing reference keys."));
        int translated = canonicalKeys.Count(key => parsed.Values.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value));
        return new UiLocaleValidationReport(parsed.Id, parsed.Path, parsed.IsCanonical,
            diagnostics.All(diagnostic => diagnostic.Severity != UiLocaleValidationSeverity.Error), translated, canonicalKeys.Count,
            missing, extra, diagnostics.OrderBy(diagnostic => diagnostic.Severity).ThenBy(diagnostic => diagnostic.Code, StringComparer.Ordinal).ThenBy(diagnostic => diagnostic.Key, StringComparer.Ordinal).ToArray());
    }

    private static ParsedLocale Parse(string path, string id, bool isCanonical, bool duplicateIdentity)
    {
        var diagnostics = new List<UiLocaleValidationDiagnostic>();
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        if (!IsLocaleIdentity(id))
            diagnostics.Add(new(UiLocaleValidationSeverity.Error, "InvalidLocaleIdentity", null, "Filename is not a valid locale identity."));
        if (duplicateIdentity)
            diagnostics.Add(new(UiLocaleValidationSeverity.Error, "DuplicateLocaleIdentity", null, "Another locale file already uses this identity."));
        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllBytes(path));
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                diagnostics.Add(new(UiLocaleValidationSeverity.Error, "InvalidRoot", null, "Locale JSON must contain one object."));
                return new ParsedLocale(id, path, isCanonical, values, diagnostics);
            }
            foreach (JsonProperty property in document.RootElement.EnumerateObject())
            {
                if (!JsonUiLocaleProvider.IsSemanticKey(property.Name))
                {
                    diagnostics.Add(new(UiLocaleValidationSeverity.Error, "InvalidKey", property.Name, "Key is not a valid semantic localization key."));
                    continue;
                }
                if (property.Value.ValueKind != JsonValueKind.String)
                {
                    diagnostics.Add(new(UiLocaleValidationSeverity.Error, "InvalidValueType", property.Name, "Translation must be a JSON string."));
                    continue;
                }
                if (!values.TryAdd(property.Name, property.Value.GetString() ?? string.Empty))
                {
                    diagnostics.Add(new(UiLocaleValidationSeverity.Error, "DuplicateKey", property.Name, "Locale JSON contains the key more than once."));
                    continue;
                }
                if (string.IsNullOrWhiteSpace(values[property.Name]))
                    diagnostics.Add(new(UiLocaleValidationSeverity.Warning, "EmptyTranslation", property.Name, "Translation is empty and falls back to English."));
            }
        }
        catch (JsonException)
        {
            diagnostics.Add(new(UiLocaleValidationSeverity.Error, "MalformedJson", null, "Locale JSON is malformed."));
        }
        catch (IOException)
        {
            diagnostics.Add(new(UiLocaleValidationSeverity.Error, "UnreadableFile", null, "Locale JSON could not be read."));
        }
        return new ParsedLocale(id, path, isCanonical, values, diagnostics);
    }

    private static bool IsLocaleIdentity(string id) =>
        !string.IsNullOrWhiteSpace(id) && id.Length <= 24 && char.IsLetter(id[0]) &&
        id.All(character => char.IsLetterOrDigit(character) || character == '-');

    private sealed record ParsedLocale(string Id, string Path, bool IsCanonical, Dictionary<string, string> Values, IReadOnlyList<UiLocaleValidationDiagnostic> Diagnostics);
}
