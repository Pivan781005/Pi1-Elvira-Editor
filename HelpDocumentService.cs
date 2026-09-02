using System.Text.Json;

namespace ElviraVgaEditor;

internal sealed record HelpTopic(string Id, string Group, string Title, string Body);
internal sealed record HelpDocument(string LocaleId, IReadOnlyList<HelpTopic> Topics);
internal sealed record HelpDocumentFile(IReadOnlyList<HelpTopic>? Topics);

/// <summary>Loads localized product documentation separately from short UI
/// strings. English is the canonical topic and field fallback.</summary>
internal static class HelpDocumentService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    internal static HelpDocument Load(UiLanguage language) => Load(LocaleId(language));

    internal static HelpDocument Load(string localeId)
    {
        IReadOnlyList<HelpTopic> english = Read("en");
        IReadOnlyList<HelpTopic> selected = localeId.Equals("en", StringComparison.OrdinalIgnoreCase) ? english : TryRead(localeId) ?? [];
        var selectedById = selected.GroupBy(topic => topic.Id, StringComparer.Ordinal).ToDictionary(group => group.Key, group => group.Single(), StringComparer.Ordinal);
        HelpTopic[] resolved = english.Select(englishTopic => selectedById.TryGetValue(englishTopic.Id, out HelpTopic? translation)
            ? new HelpTopic(englishTopic.Id,
                string.IsNullOrWhiteSpace(translation.Group) ? englishTopic.Group : translation.Group,
                string.IsNullOrWhiteSpace(translation.Title) ? englishTopic.Title : translation.Title,
                string.IsNullOrWhiteSpace(translation.Body) ? englishTopic.Body : translation.Body)
            : englishTopic).ToArray();
        return new(localeId, resolved);
    }

    internal static void ValidateAll()
    {
        HelpDocument english = Load("en");
        if (english.Topics.Count < 18 || english.Topics.Select(topic => topic.Id).Distinct(StringComparer.Ordinal).Count() != english.Topics.Count ||
            !english.Topics.Select(topic => topic.Group).Distinct(StringComparer.Ordinal).SequenceEqual(new[] { "Common", "Elvira I", "Elvira II" }))
            throw new InvalidDataException("English Help document does not define the required deterministic hierarchy.");
        foreach (string localeId in new[] { "sk", "cs" })
        {
            HelpDocument document = Load(localeId);
            if (!document.Topics.Select(topic => topic.Id).SequenceEqual(english.Topics.Select(topic => topic.Id)) ||
                document.Topics.Any(topic => string.IsNullOrWhiteSpace(topic.Title) || string.IsNullOrWhiteSpace(topic.Body)))
                throw new InvalidDataException($"{localeId} Help fallback did not resolve the canonical topic structure.");
        }
    }

    private static IReadOnlyList<HelpTopic> Read(string localeId)
    {
        HelpDocumentFile? document = JsonSerializer.Deserialize<HelpDocumentFile>(File.ReadAllText(DocumentPath(localeId)), JsonOptions);
        HelpTopic[] topics = document?.Topics?.ToArray() ?? throw new InvalidDataException($"Help document '{localeId}' has no topics.");
        ValidateRaw(topics, localeId);
        return topics;
    }

    private static IReadOnlyList<HelpTopic>? TryRead(string localeId)
    {
        string path = DocumentPath(localeId);
        return File.Exists(path) ? Read(localeId) : null;
    }

    private static void ValidateRaw(IReadOnlyList<HelpTopic> topics, string localeId)
    {
        if (topics.Count == 0 || topics.Any(topic => string.IsNullOrWhiteSpace(topic.Id)) ||
            topics.GroupBy(topic => topic.Id, StringComparer.Ordinal).Any(group => group.Count() != 1))
            throw new InvalidDataException($"Help document '{localeId}' has invalid or duplicate topic identities.");
    }

    private static string LocaleId(UiLanguage language) => language switch
    {
        UiLanguage.Slovak => "sk", UiLanguage.Czech => "cs", _ => "en"
    };

    private static string DocumentPath(string localeId) => Path.Combine(AppContext.BaseDirectory, "Locales", "Help", localeId + ".json");
}
