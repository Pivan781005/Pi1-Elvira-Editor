namespace Pi1ElviraEditor;

// Bootstrap semantic keys. R7B will move translations to JSON without changing
// these control-facing identities.
internal static class UiLocalizationKeys
{
    internal const string NoGameSelected = "App.NoGameSelected";
    internal const string FindGamesOrBrowseFolder = "App.FindOrBrowse";
    internal const string RecoveryActionsUnavailable = "Recovery.ActionsUnavailable";
}

internal sealed class UiLocaleChangedEventArgs : EventArgs
{
    internal UiLocaleChangedEventArgs(string previousLocaleId, string currentLocaleId)
    {
        PreviousLocaleId = previousLocaleId;
        CurrentLocaleId = currentLocaleId;
    }

    internal string PreviousLocaleId { get; }
    internal string CurrentLocaleId { get; }
}

// Deliberately independent of game installations, variants, text encodings, and
// font profiles. It is a small provider-agnostic bootstrap for R7A.
internal sealed class UiLocalizationService
{
    private readonly Func<string, string, string> _lookup;

    internal UiLocalizationService(Func<string, string, string> lookup, string defaultLocaleId = "en")
    {
        _lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
        CurrentLocaleId = NormalizeLocaleId(defaultLocaleId);
    }

    internal string CurrentLocaleId { get; private set; }
    internal event EventHandler<UiLocaleChangedEventArgs>? LocaleChanged;

    internal string Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return string.Empty;
        return _lookup(CurrentLocaleId, key) ?? key;
    }

    internal void SetLocale(string localeId)
    {
        string normalized = NormalizeLocaleId(localeId);
        if (normalized == CurrentLocaleId) return;
        string previous = CurrentLocaleId;
        CurrentLocaleId = normalized;
        LocaleChanged?.Invoke(this, new UiLocaleChangedEventArgs(previous, normalized));
    }

    private static string NormalizeLocaleId(string localeId)
    {
        if (string.IsNullOrWhiteSpace(localeId)) throw new ArgumentException("Locale identity is required.", nameof(localeId));
        return localeId.Trim().ToLowerInvariant();
    }
}
