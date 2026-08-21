namespace ElviraVgaEditor;

internal enum UiLanguage
{
    Slovak,
    English,
    Czech
}

internal static class UiText
{
    private static UiLanguage _language = UiLanguage.English;
    public static UiLanguage Language => _language;

    public static void SetLanguage(UiLanguage language) => _language = language;

    private static readonly Dictionary<string, string[]> T = new()
    {
        ["AppTitle"] = new[] { "π1 Elvira I&II VGA Editor v1.0", "π1 Elvira I&II VGA Editor v1.0", "π1 Elvira I&II VGA Editor v1.0" },
        ["About"] = new[] { "O programe", "About", "O programu" },
        ["AboutText"] = new[]
        {
            "π1 Elvira I&II VGA Editor v1.0\n\nNeoficiálny fanúšikovský nástroj pre Elvira I a Elvira II.\nVGA resource editor + GAMEPC text editor.\n\nLicencia: GPL-3.0",
            "π1 Elvira I&II VGA Editor v1.0\n\nUnofficial fan-made tool for Elvira I and Elvira II.\nVGA resource editor + GAMEPC text editor.\n\nLicense: GPL-3.0",
            "π1 Elvira I&II VGA Editor v1.0\n\nNeoficiální fanouškovský nástroj pro Elvira I a Elvira II.\nVGA resource editor + GAMEPC text editor.\n\nLicence: GPL-3.0"
        },
        ["Browse"] = new[] { "Vybrať adresár...", "Browse folder...", "Vybrat adresář..." },
        ["Refresh"] = new[] { "Obnoviť", "Refresh", "Obnovit" },
        ["Palette"] = new[] { "Paleta:", "Palette:", "Paleta:" },
        ["PaletteWord"] = new[] { "Paleta", "Palette", "Paleta" },
        ["DiagnosticPalette"] = new[] { "Diagnostická", "Diagnostic", "Diagnostická" },
        ["PixelZoom"] = new[] { "Pixelové zväčšenie", "Pixel zoom", "Pixelové zvětšení" },
        ["Language"] = new[] { "Jazyk:", "Language:", "Jazyk:" },
        ["VgaTab"] = new[] { "VGA Editor", "VGA Editor", "VGA Editor" },
        ["TextTab"] = new[] { "Editor textov", "Text Editor", "Editor textů" },
        ["OpenTextEditor"] = new[] { "Textový editor", "Text editor", "Textový editor" },
        ["ReplacePng"] = new[] { "Nahradiť PNG...", "Replace PNG...", "Nahradit PNG..." },
        ["CancelEdit"] = new[] { "Zrušiť edit", "Cancel edit", "Zrušit edit" },
        ["ExportPng"] = new[] { "Export PNG...", "Export PNG...", "Export PNG..." },
        ["ApplyGame"] = new[] { "APLIKOVAŤ DO HRY", "APPLY TO GAME", "APLIKOVAT DO HRY" },
        ["RestoreOriginal"] = new[] { "Obnoviť originál", "Restore original", "Obnovit originál" },
        ["Zoom"] = new[] { "Zoom:", "Zoom:", "Zoom:" },
        ["Id"] = new[] { "ID", "ID", "ID" },
        ["Offset"] = new[] { "Offset", "Offset", "Offset" },
        ["Type"] = new[] { "Typ", "Type", "Typ" },
        ["Resolution"] = new[] { "Rozlíšenie", "Size", "Rozlišení" },
        ["Flags"] = new[] { "Flags", "Flags", "Flags" },
        ["Edit"] = new[] { "Edit", "Edit", "Edit" },
        ["TextIndex"] = new[] { "Index", "Index", "Index" },
        ["TextOffset"] = new[] { "File offset", "File offset", "File offset" },
        ["Length"] = new[] { "Dĺžka", "Length", "Délka" },
        ["Text"] = new[] { "Text", "Text", "Text" },
        ["Search"] = new[] { "Hľadať:", "Search:", "Hledat:" },
        ["Encoding"] = new[] { "Kódovanie:", "Encoding:", "Kódování:" },
        ["SaveTexts"] = new[] { "ULOŽIŤ GAMEPC", "SAVE GAMEPC", "ULOŽIT GAMEPC" },
        ["ReloadTexts"] = new[] { "Načítať GAMEPC", "Reload GAMEPC", "Načíst GAMEPC" },
        ["TooLong"] = new[] { "Text je dlhší než pôvodný priestor.", "Text is longer than the original slot.", "Text je delší než původní prostor." },
        ["NoGamepc"] = new[] { "GAMEPC nebol nájdený.", "GAMEPC was not found.", "GAMEPC nebyl nalezen." },
        ["Saved"] = new[] { "Uložené.", "Saved.", "Uloženo." },
        ["BackupMade"] = new[] { "Záloha vytvorená.", "Backup created.", "Záloha vytvořena." },
        ["ZonesFound"] = new[] { "Nájdených zón: {0}", "Zones found: {0}", "Nalezených zón: {0}" },
        ["EntriesStatus"] = new[] { "{0} entries: {1} | endian: {2}", "{0} entries: {1} | endian: {2}", "{0} entries: {1} | endian: {2}" },
        ["Warning"] = new[] { "Upozornenie", "Warning", "Upozornění" },
        ["OriginalBackupMissing"] = new[] { "Originálna záloha neexistuje. Nie je čo obnoviť.", "The original backup does not exist. There is nothing to restore.", "Původní záloha neexistuje. Není co obnovit." },
        ["ReloadPreview"] = new[] { "Načítať znova", "Reload", "Načíst znovu" },
        ["BackToVga"] = new[] { "Späť na VGA", "Back to VGA", "Zpět na VGA" },
        ["TextSaved"] = new[] { "GAMEPC bol uložený.", "GAMEPC was saved.", "GAMEPC byl uložen." },
    };

    public static string Get(string key)
    {
        if (!T.TryGetValue(key, out var arr))
            return key;

        int i = _language switch
        {
            UiLanguage.Slovak => 0,
            UiLanguage.English => 1,
            UiLanguage.Czech => 2,
            _ => 1
        };

        return arr[i];
    }
}
