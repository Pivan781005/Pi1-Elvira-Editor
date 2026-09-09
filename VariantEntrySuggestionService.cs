namespace Pi1ElviraEditor;

/// <summary>R9F V8.6d narrow testable surface for Add Variant defaults. It
/// derives a non-empty display name, an authoritative DOS 8.3 data filename,
/// and the current runtime executable mapping from the selected
/// edition/runtime without requiring internal knowledge. It never creates
/// catalog entries; it only suggests them.</summary>
internal sealed record VariantEntrySuggestion(
    string DisplayName,
    string DataFile,
    string ExeFile,
    bool Enabled,
    bool DuplicateExists,
    string Detail);

internal static class VariantEntrySuggestionService
{
    internal static VariantEntrySuggestion Suggest(
        ProjectContext? project,
        VariantContext? variant,
        string? editionCode,
        TranslationProjectState? translationState,
        VariantCatalog? catalog)
    {
        string code = "EN";
        try
        {
            if (project is not null)
                code = ProjectVariantOwnership.NormalizeCode(project, editionCode ?? ProjectVariantOwnership.OriginalCode);
            else if (!string.IsNullOrWhiteSpace(editionCode))
                code = editionCode.Trim().ToUpperInvariant();
        }
        catch (ArgumentException) { code = (editionCode ?? "EN").Trim().ToUpperInvariant(); }
        catch (InvalidOperationException) { code = (editionCode ?? "EN").Trim().ToUpperInvariant(); }

        TranslationProjectVariant? translation = translationState?.Variants
            .SingleOrDefault(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

        string displayName = translation?.DisplayName?.Trim() ?? string.Empty;
        string dataFile = translation?.DataFile?.Trim().ToUpperInvariant() ?? string.Empty;
        // The executable is always derived for the CURRENT runtime from the
        // edition code (RUNVGA+SK=RUNVGASK, RUNEGA+SK=RUNEGASK,
        // RUNIT+SK=RUNITSK). The stored translation ExeFile is VGA-biased and
        // must not leak into an EGA suggestion.
        string exeFile;
        if (variant is not null && !ProjectVariantOwnership.IsOriginal(code))
        {
            try
            {
                string stem = Path.GetFileNameWithoutExtension(variant.SourceExecutableName ?? "RUNVGA.EXE");
                exeFile = (stem + code + ".EXE").ToUpperInvariant();
                if (!GameDataFileService.IsDos83FileName(exeFile))
                    exeFile = translation?.ExeFile?.Trim().ToUpperInvariant() ?? string.Empty;
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                exeFile = translation?.ExeFile?.Trim().ToUpperInvariant() ?? string.Empty;
            }
        }
        else if (variant is not null && ProjectVariantOwnership.IsOriginal(code))
        {
            exeFile = (variant.SourceExecutableName ?? string.Empty).ToUpperInvariant();
        }
        else
        {
            exeFile = translation?.ExeFile?.Trim().ToUpperInvariant() ?? string.Empty;
        }

        if (translation is null && project is not null && variant is not null && !ProjectVariantOwnership.IsOriginal(code))
        {
            // No persisted translation for this edition yet: derive the
            // authoritative filenames from the code so the user never has to
            // guess them. Display name falls back to the code itself. The
            // executable stays runtime-specific (see above).
            try
            {
                VariantEntry derived = VariantNaming.Create(code, code, project.GameProfile, true, 1);
                displayName = code;
                dataFile = derived.DataFile;
                string stem = Path.GetFileNameWithoutExtension(variant.SourceExecutableName ?? "RUNVGA.EXE");
                string runtimeExe = (stem + code + ".EXE").ToUpperInvariant();
                exeFile = GameDataFileService.IsDos83FileName(runtimeExe) ? runtimeExe : derived.ExeFile;
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                displayName = code;
                dataFile = string.Empty;
                exeFile = string.Empty;
            }
        }

        if (string.IsNullOrWhiteSpace(displayName))
            displayName = code;
        if (variant is not null && string.IsNullOrWhiteSpace(exeFile) && !string.IsNullOrWhiteSpace(dataFile))
        {
            try { exeFile = VariantNaming.DeriveExeFile(VariantNaming.DeriveCode(dataFile), project?.GameProfile ?? ElviraGameProfile.Elvira1); }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException) { exeFile = string.Empty; }
        }

        bool duplicate = false;
        if (catalog is not null && !string.IsNullOrWhiteSpace(dataFile))
            duplicate = catalog.Entries.Any(entry => entry.DataFile.Equals(dataFile, StringComparison.OrdinalIgnoreCase));

        string detail = duplicate
            ? $"Entry {dataFile} already exists."
            : string.IsNullOrWhiteSpace(dataFile)
                ? "No edition selected."
                : $"Suggested {dataFile} for edition {code}.";
        return new(displayName, dataFile, exeFile, true, duplicate, detail);
    }
}
