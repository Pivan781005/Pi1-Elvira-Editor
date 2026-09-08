using System.Text.Json;

namespace Pi1ElviraEditor;

internal sealed record AboutDocument(string Title, string Body);

internal static class AboutDocumentService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    internal static AboutDocument Load(UiLanguage language) => Load(language switch { UiLanguage.Slovak => "sk", UiLanguage.Czech => "cs", _ => "en" });

    internal static AboutDocument Load(string localeId)
    {
        AboutDocument english = Read("en");
        if (localeId.Equals("en", StringComparison.OrdinalIgnoreCase)) return english;
        AboutDocument? selected = TryRead(localeId);
        return selected is null ? english : new AboutDocument(
            string.IsNullOrWhiteSpace(selected.Title) ? english.Title : selected.Title,
            string.IsNullOrWhiteSpace(selected.Body) ? english.Body : selected.Body);
    }

    internal static void ValidateAll()
    {
        foreach (string localeId in new[] { "en", "sk", "cs" })
        {
            AboutDocument document = Load(localeId);
            if (string.IsNullOrWhiteSpace(document.Title) || string.IsNullOrWhiteSpace(document.Body) || !document.Body.Contains(AppInfo.ProductName, StringComparison.Ordinal))
                throw new InvalidDataException($"{localeId} About document is incomplete.");
        }
    }

    private static AboutDocument Read(string localeId)
    {
        AboutDocument? document = JsonSerializer.Deserialize<AboutDocument>(File.ReadAllText(PathFor(localeId)), JsonOptions);
        return document ?? throw new InvalidDataException($"About document '{localeId}' is invalid.");
    }

    private static AboutDocument? TryRead(string localeId)
    {
        string path = PathFor(localeId);
        return File.Exists(path) ? Read(localeId) : null;
    }

    private static string PathFor(string localeId) => Path.Combine(AppContext.BaseDirectory, "Locales", "About", localeId + ".json");
}
