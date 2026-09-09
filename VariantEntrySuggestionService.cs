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
    /// <summary>Authoritative runtime-aware executable mapping for one edition
    /// code: resolved against the CURRENT runtime (source stem + code: E1 VGA
    /// SK to RUNVGASK.EXE, E1 EGA SK to RUNEGASK.EXE, E2 VGA SK to
    /// RUNITSK.EXE), the same rule ActiveProjectBuildIdentity uses for builds.
    /// It is never re-derived from GameProfile alone, so EGA never collapses
    /// to RUNVGA. Arbitrary valid codes work; nothing is hardcoded to SK.</summary>
    internal static string ResolveExeFileForRuntime(ProjectContext? project, VariantContext? variant, string? code)
    {
        string normalized = (code ?? string.Empty).Trim().ToUpperInvariant();
        if (variant is not null)
        {
            if (ProjectVariantOwnership.IsOriginal(normalized))
                return (variant.SourceExecutableName ?? string.Empty).ToUpperInvariant();
            string stem = Path.GetFileNameWithoutExtension(variant.SourceExecutableName ?? string.Empty);
            return (stem + normalized + ".EXE").ToUpperInvariant();
        }
        ElviraGameProfile game = project?.GameProfile ?? ElviraGameProfile.Elvira1;
        string fallbackCode;
        try { fallbackCode = VariantNaming.ValidateCode(normalized); }
        catch (InvalidOperationException) { fallbackCode = ProjectVariantOwnership.OriginalCode; }
        return VariantNaming.DeriveExeFile(fallbackCode, game);
    }

    /// <summary>Production persistence boundary for the Add Variant dialog.
    /// Persists the exact runtime-aware executable mapping (see
    /// ResolveExeFileForRuntime) via the explicit catalog overload instead of
    /// re-deriving from GameProfile. MainForm and the persistence regression
    /// share this method, so the smoke proves the real save path.</summary>
    internal static VariantEntry AddRuntimeAwareEntry(VariantCatalog catalog, string displayName, string dataFile, bool enabled, ProjectContext? project, VariantContext? variant)
    {
        ArgumentNullException.ThrowIfNull(catalog);
        string code = VariantNaming.DeriveCode(dataFile);
        string exeFile = ResolveExeFileForRuntime(project, variant, code);
        return catalog.Add(displayName, code, dataFile, exeFile, enabled);
    }

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
            string runtimeExe = ResolveExeFileForRuntime(project, variant, code);
            exeFile = GameDataFileService.IsDos83FileName(runtimeExe)
                ? runtimeExe
                : translation?.ExeFile?.Trim().ToUpperInvariant() ?? string.Empty;
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
                string runtimeExe = ResolveExeFileForRuntime(project, variant, code);
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
