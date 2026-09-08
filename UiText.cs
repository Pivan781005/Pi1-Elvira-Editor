namespace Pi1ElviraEditor;

internal enum UiLanguage
{
    Slovak,
    English,
    Czech
}

internal static class UiText
{
    private static JsonUiLocaleProvider JsonLocales = JsonUiLocaleProvider.Discover(DefaultLocaleDirectory);
    private static readonly UiLocalizationService Service = new(Lookup, "en");

    private static string DefaultLocaleDirectory => Path.Combine(AppContext.BaseDirectory, "Locales");

    // UI strings resolve exclusively through the discovered JSON locale catalog.
    public static UiLanguage Language => Service.CurrentLocaleId switch
    {
        "sk" => UiLanguage.Slovak,
        "cs" => UiLanguage.Czech,
        _ => UiLanguage.English
    };
    public static string LocaleId => Service.CurrentLocaleId;
    public static IReadOnlyList<UiLocaleDescriptor> AvailableLocales => JsonLocales.AvailableLocales;
    public static IReadOnlyList<UiLocaleDiscoveryDiagnostic> LocaleDiagnostics => JsonLocales.Diagnostics;
    public static event EventHandler<UiLocaleChangedEventArgs>? LocaleChanged
    {
        add => Service.LocaleChanged += value;
        remove => Service.LocaleChanged -= value;
    }
    public static event EventHandler? LocalesChanged;

    public static void SetLanguage(UiLanguage language) => Service.SetLocale(language switch
    {
        UiLanguage.Slovak => "sk",
        UiLanguage.Czech => "cs",
        _ => "en"
    });
    public static void SetLocale(string localeId) => Service.SetLocale(localeId);
    internal static void RefreshLocaleCatalog(string? localeDirectory = null)
    {
        JsonLocales = JsonUiLocaleProvider.Discover(localeDirectory ?? DefaultLocaleDirectory);
        LocalesChanged?.Invoke(null, EventArgs.Empty);
    }
    public static string Get(string key)
        => Service.Get(key);

    internal static string Get(string localeId, string key) =>
        JsonUiLocaleProvider.IsSemanticKey(key) ? JsonLocales.Resolve(localeId, key).Value : key;

    private static string Lookup(string localeId, string key)
    {
        return JsonUiLocaleProvider.IsSemanticKey(key) ? JsonLocales.Resolve(localeId, key).Value : key;
    }

    public static string GlyphDescription(string english) => english switch
    {
        "Control / non-printable" => Get("GlyphControl"),
        "Letter / digit" => Get("GlyphLetterDigit"),
        "Punctuation" => Get("GlyphPunctuation"),
        "Symbol" => Get("GlyphSymbol"),
        "Whitespace" => Get("GlyphWhitespace"),
        "CP852 slot" => Get("GlyphCp852"),
        _ => english
    };
}
