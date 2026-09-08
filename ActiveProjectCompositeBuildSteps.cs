namespace Pi1ElviraEditor;

/// <summary>Explicit snapshot of editor project state consumed by one authorized
/// composite build. It is never persisted in the pristine game root.</summary>
internal sealed record ActiveProjectBuildInput(
    TranslationProjectVariant Translation,
    GraphicsProjectState Graphics,
    RuntimeUiTextState RuntimeUi,
    FontProjectState? Font = null);

internal static class ActiveProjectCompositeBuildFactory
{
    internal static IReadOnlyList<ICompositeBuildStep> Create(VariantDirectoryService directories,
        TranslationProjectService translations, GraphicsVariantService graphics, ActiveProjectBuildInput input)
    {
        ArgumentNullException.ThrowIfNull(directories); ArgumentNullException.ThrowIfNull(translations);
        ArgumentNullException.ThrowIfNull(graphics); ArgumentNullException.ThrowIfNull(input);
        return
        [
            new SelectedTranslationProjectBuildStep(translations, directories, input.Translation),
            new GraphicsProjectMaterializationBuildStep(directories, graphics, input.Graphics, input.Translation.Code),
            new FontProjectBuildStep(input.Font),
            new RuntimeUiProjectBuildStep(input.RuntimeUi),
            new TranslationAwareExecutableBuildStep(directories, input.Translation, input.RuntimeUi)
        ];
    }
}

internal static class ActiveProjectBuildIdentity
{
    internal static string ExecutableName(VariantContext variant, TranslationProjectVariant translation)
    {
        if (translation.Code.Equals("EN", StringComparison.OrdinalIgnoreCase)) return variant.SourceExecutableName;
        string stem = Path.GetFileNameWithoutExtension(variant.SourceExecutableName);
        string name = (stem + translation.Code + ".EXE").ToUpperInvariant();
        if (!GameDataFileService.IsDos83FileName(name)) throw new InvalidDataException("Selected translation executable name is not DOS 8.3 safe.");
        return name;
    }
}

/// <summary>Materializes the already-selected project translation. English is
/// intentionally the disposable baseline GAMEPC/RUN*.EXE pair.</summary>
internal sealed class SelectedTranslationProjectBuildStep : ICompositeBuildStep
{
    private readonly TranslationProjectService _translations;
    private readonly VariantDirectoryService _directories;
    private readonly TranslationProjectVariant _translation;
    internal SelectedTranslationProjectBuildStep(TranslationProjectService translations, VariantDirectoryService directories, TranslationProjectVariant translation)
    { _translations = translations; _directories = directories; _translation = translation; }
    public CompositeBuildStage Stage => CompositeBuildStage.ApplyDataTransformations;
    public string Name => "Selected project translation data";
    public bool AppliesTo(VariantContext variant) => variant is not null;
    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        if (project is null || variant is null || !ReferenceEquals(project, variant.Project)) return "Selected translation requires the exact active project and runtime.";
        if (!GameDataFileService.IsDos83FileName(_translation.DataFile) || !GameDataFileService.IsDos83FileName(_translation.ExeFile)) return "Selected translation output names are not DOS 8.3 safe.";
        if (_translation.Code.Equals("EN", StringComparison.OrdinalIgnoreCase)) return null;
        TranslationProjectLoadResult loaded = _translations.Load(project);
        if (!loaded.IsSuccess) return loaded.Detail ?? "Translation project data is invalid.";
        return loaded.State!.Variants.Any(item => item.Code.Equals(_translation.Code, StringComparison.OrdinalIgnoreCase) &&
            item.DataFile.Equals(_translation.DataFile, StringComparison.OrdinalIgnoreCase) && item.ExeFile.Equals(_translation.ExeFile, StringComparison.OrdinalIgnoreCase) &&
            item.Edits.OrderBy(edit => edit.Key).SequenceEqual(_translation.Edits.OrderBy(edit => edit.Key)))
            ? null : "The selected translation no longer matches persisted project state.";
    }
    public string? Execute(ProjectContext project, VariantContext variant)
    {
        try
        {
            if (!_translation.Code.Equals("EN", StringComparison.OrdinalIgnoreCase))
                _ = _translations.MaterializeOwnedVariantDataFile(project, variant, _directories, _translation);
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException)
        { return "Selected translation materialization failed: " + ex.Message; }
    }
}

/// <summary>Rebuilds only the project-owned image replacements applicable to
/// the selected runtime, inside its marker-validated variant directory.</summary>
internal sealed class GraphicsProjectMaterializationBuildStep : ICompositeBuildStep
{
    private readonly VariantDirectoryService _directories;
    private readonly GraphicsVariantService _graphics;
    private readonly GraphicsProjectState _state;
    private readonly string _projectCode;
    internal GraphicsProjectMaterializationBuildStep(VariantDirectoryService directories, GraphicsVariantService graphics, GraphicsProjectState state, string projectCode)
    { _directories = directories; _graphics = graphics; _state = state; _projectCode = projectCode; }
    public CompositeBuildStage Stage => CompositeBuildStage.ApplyGraphicsTransformations;
    public string Name => "Project graphics edits";
    public bool AppliesTo(VariantContext variant) => variant is not null;
    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        try
        {
            foreach (GraphicsVariantProjection projection in _graphics.GetGraphicsEditsForVariant(project, _state, variant))
                if (!File.Exists(projection.Edit.ReplacementPngPath)) return "Graphics replacement is missing: " + projection.Edit.ReplacementPngPath;
            return null;
        }
        catch (Exception ex) when (ex is ArgumentException or IOException) { return "Graphics project input is invalid: " + ex.Message; }
    }
    public string? Execute(ProjectContext project, VariantContext variant)
    {
        try
        {
            string root = _directories.GetVariantEditionDirectoryPath(project, variant, _projectCode);
            foreach (IGrouping<string, GraphicsVariantProjection> group in _graphics.GetGraphicsEditsForVariant(project, _state, variant)
                .GroupBy(item => item.Edit.Identity.ResourceFileName, StringComparer.OrdinalIgnoreCase).OrderBy(item => item.Key, StringComparer.OrdinalIgnoreCase))
            {
                string target = Path.Combine(root, group.Key);
                if (!File.Exists(target)) return "Variant graphics source is missing: " + group.Key;
                Dictionary<int, string> edits = group.OrderBy(item => item.Edit.Identity.ImageId).ToDictionary(item => item.Edit.Identity.ImageId, item => item.Edit.ReplacementPngPath);
                IReadOnlyDictionary<int, System.Drawing.Color[]> palettes = group.ToDictionary(item => item.Edit.Identity.ImageId,
                    item => ResolvePalette(project, target, item.Edit.Identity.ImageId));
                string temporary = target + ".pi1-build.tmp";
                try
                {
                    _ = new VgaFileRebuilder().Rebuild(target, edits, temporary,
                        entry => palettes.TryGetValue(entry.ImageId, out System.Drawing.Color[]? palette) ? palette : null);
                    _ = new VgaImageTableParser(File.ReadAllBytes(temporary)).Parse();
                    File.Move(temporary, target, true);
                }
                finally { if (File.Exists(temporary)) File.Delete(temporary); }
            }
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        { return "Graphics materialization failed: " + ex.Message; }
    }

    private static System.Drawing.Color[] ResolvePalette(ProjectContext project, string resourcePath, int imageId)
    {
        string name = Path.GetFileName(resourcePath);
        if (name.Length < 3) throw new InvalidDataException("Graphics resource has no paired palette identity.");
        string palettePath = Path.Combine(Path.GetDirectoryName(resourcePath)!, name[..2] + "1.VGA");
        if (!File.Exists(palettePath)) throw new InvalidDataException("Paired palette resource is missing: " + Path.GetFileName(palettePath));
        IReadOnlyList<ElviraPaletteBank> banks = ElviraPaletteLoader.Load(palettePath);
        PaletteResolution resolution = project.GameProfile switch
        {
            ElviraGameProfile.Elvira1 => new Elvira1PaletteResolver().Resolve(palettePath, resourcePath, imageId),
            ElviraGameProfile.Elvira2 => new Elvira2PaletteResolver().Resolve(palettePath, resourcePath, imageId),
            _ => PaletteResolution.Unresolved()
        };
        int bank = Elvira1PaletteResolver.EffectivePaletteBank(null, resolution, banks.Count);
        if (banks.Count == 0 || bank < 0 || bank >= banks.Count) throw new InvalidDataException("No usable palette bank is available for " + name + " image " + imageId + ".");
        return banks[bank].Colors;
    }
}

/// <summary>Font project-state gate. VGA and EGA/RUNIT font edits materialize
/// inside the owned variant executable bootstrap; this stage performs no byte
/// mutation itself. Elvira I EGA has no proven font writer, so any
/// EGA-applicable saved edit fails the build closed here instead of being
/// silently dropped. Pristine GameRoot is never touched.</summary>
internal sealed class FontProjectBuildStep : ICompositeBuildStep
{
    private readonly FontProjectState? _font;
    internal FontProjectBuildStep(FontProjectState? font) => _font = font;
    public CompositeBuildStage Stage => CompositeBuildStage.ApplyFontTransformations;
    public string Name => "Project font edits";
    public bool AppliesTo(VariantContext variant) => variant is not null;
    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        try
        {
            FontProjectState state = _font ?? FontProjectState.Empty(project.GameProfile);
            IReadOnlyList<FontVariantProjection> applicable =
                new FontVariantService().GetFontEditsForVariant(project, state, variant);
            if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega && applicable.Count > 0)
                return UiText.Get("Font.EgaBuildNotMaterialized");
            return null;
        }
        catch (ArgumentException ex) { return "Font project state is invalid: " + ex.Message; }
    }
    public string? Execute(ProjectContext project, VariantContext variant) => null;
}

internal sealed class RuntimeUiProjectBuildStep : ICompositeBuildStep
{
    private readonly RuntimeUiTextState _state;
    internal RuntimeUiProjectBuildStep(RuntimeUiTextState state) => _state = state;
    public CompositeBuildStage Stage => CompositeBuildStage.ApplyRuntimeUiTransformations;
    public string Name => "Runtime UI project state";
    public bool AppliesTo(VariantContext variant) => variant is not null;
    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        IReadOnlyList<RuntimeUiTextOverride> scoped = Scoped(project, variant);
        if (scoped.Count == 0) return null;
        // R9F V5: the three proven-live variable-width VGA records may pass
        // preflight when their overrides validate (geometry + encoding).
        // Only overrides stored for THIS runtime are consumed; EGA state
        // never leaks into a VGA build and vice versa.
        if (variant is null || variant.RuntimeKind != VariantRuntimeKind.Elvira1Vga)
        {
            // R9F RUNEGA: overrides are allowed only for audited records whose
            // stored values satisfy their frozen edit contracts. Anything else
            // (including legacy unstructured values) fails with its reason.
            if (variant is not null && variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega)
            {
                foreach (RuntimeUiTextOverride value in scoped.OrderBy(item => item.LogicalRecordId))
                {
                    if (!RunEgaUiService.IsEditable(value.LogicalRecordId))
                        return UiText.Get("RuntimeUi.BuildNotMaterialized");
                    if (!RunEgaUiService.TryValidateStored(value.Text, value.LogicalRecordId, out _, out string egaDetail))
                        return egaDetail;
                }
                return null;
            }
            return UiText.Get("RuntimeUi.BuildNotMaterialized");
        }
        foreach (RuntimeUiTextOverride value in scoped)
        {
            if (!RunVgaVariableUiService.IsVariableRecord(value.LogicalRecordId))
                return UiText.Get("RuntimeUi.BuildNotMaterialized");
            if (!RunVgaVariableUiService.TryValidateStored(value.Text, value.LogicalRecordId, out _, out string detail))
                return detail;
        }
        return null;
    }
    public string? Execute(ProjectContext project, VariantContext variant) => null;

    private IReadOnlyList<RuntimeUiTextOverride> Scoped(ProjectContext project, VariantContext variant)
    {
        if (variant is null) return [];
        return _state.Overrides.Where(value => value.Runtime == variant.RuntimeKind).OrderBy(value => value.LogicalRecordId).ToArray();
    }
}

/// <summary>Uses the frozen runtime bootstrap, then gives the selected
/// translation its semantic executable filename. The data filename is supplied
/// by the launcher; it is not hard-coded into the DOS executable.</summary>
internal sealed class TranslationAwareExecutableBuildStep : ICompositeBuildStep
{
    private readonly VariantDirectoryService _directories;
    private readonly TranslationProjectVariant _translation;
    private readonly RuntimeUiTextState? _runtimeUi;
    internal TranslationAwareExecutableBuildStep(VariantDirectoryService directories, TranslationProjectVariant translation, RuntimeUiTextState? runtimeUi = null)
    { _directories = directories; _translation = translation; _runtimeUi = runtimeUi; }
    public CompositeBuildStage Stage => CompositeBuildStage.ApplyExecutableTransformation;
    public string Name => "Selected translation executable";
    public bool AppliesTo(VariantContext variant) => variant is not null;
    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        IReadOnlyList<RuntimeUiTextOverride> scoped = ScopedUi(variant);
        // R9F V5: the three proven-live variable-width VGA records require a
        // translated (V5) variant. EN baseline builds must fail closed rather
        // than silently ignore them. Only overrides stored for THIS runtime
        // are consumed.
        if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Vga && scoped.Count > 0)
        {
            foreach (RuntimeUiTextOverride value in scoped)
            {
                if (!RunVgaVariableUiService.IsVariableRecord(value.LogicalRecordId))
                    return UiText.Get("RuntimeUi.BuildNotMaterialized");
                if (!RunVgaVariableUiService.TryValidateStored(value.Text, value.LogicalRecordId, out _, out string detail))
                    return detail;
            }
            if (_translation.Code.Equals("EN", StringComparison.OrdinalIgnoreCase))
                return UiText.Get("RuntimeUi.Detail.PauseRequiresTranslatedVariant");
        }
        // R9F RUNEGA: audited records with contract-valid overrides require a
        // translated (RUNEGASK) variant, exactly like VGA Pause above.
        if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega && scoped.Count > 0)
        {
            foreach (RuntimeUiTextOverride value in scoped.OrderBy(item => item.LogicalRecordId))
            {
                if (!RunEgaUiService.IsEditable(value.LogicalRecordId))
                    return UiText.Get("RuntimeUi.BuildNotMaterialized");
                if (!RunEgaUiService.TryValidateStored(value.Text, value.LogicalRecordId, out _, out string detail))
                    return detail;
            }
            if (_translation.Code.Equals("EN", StringComparison.OrdinalIgnoreCase))
                return UiText.Get("RuntimeUi.Detail.EgaRequiresTranslatedVariant");
        }
        if (_translation.Code.Equals("EN", StringComparison.OrdinalIgnoreCase)) return null;
        return CreateBootstrap(variant)?.Preflight(project, variant);
    }
    public string? Execute(ProjectContext project, VariantContext variant)
    {
        IReadOnlyList<RuntimeUiTextOverride> scoped = ScopedUi(variant);
        if (_translation.Code.Equals("EN", StringComparison.OrdinalIgnoreCase))
        {
            if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Vga && scoped.Count > 0)
                return UiText.Get("RuntimeUi.Detail.PauseRequiresTranslatedVariant");
            if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega && scoped.Count > 0)
                return UiText.Get("RuntimeUi.Detail.EgaRequiresTranslatedVariant");
            return null;
        }
        ICompositeBuildStep? bootstrap = CreateBootstrap(variant);
        if (bootstrap is null) return "No executable bootstrap is defined for the selected runtime.";
        string? error = bootstrap.Execute(project, variant);
        if (error is not null) return error;
        try
        {
            string root = _directories.GetVariantEditionDirectoryPath(project, variant, _translation.Code);
            string generated = Path.Combine(root, variant.GeneratedExecutableName);
            string selected = Path.Combine(root, ActiveProjectBuildIdentity.ExecutableName(variant, _translation));
            if (!File.Exists(generated)) return "Frozen bootstrap did not create its generated executable.";
            if (!generated.Equals(selected, StringComparison.OrdinalIgnoreCase)) File.Move(generated, selected, false);
            // R9F V5: apply every validated variable-width VGA override
            // inside the owned runtime+edition V5 output only: fitting
            // records in place, overflow records via one deterministic
            // append. Pristine GameRoot is never touched.
            if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Vga && scoped.Count > 0)
            {
                foreach (RuntimeUiTextOverride value in scoped)
                {
                    if (!RunVgaVariableUiService.IsVariableRecord(value.LogicalRecordId))
                        return UiText.Get("RuntimeUi.BuildNotMaterialized");
                    if (!RunVgaVariableUiService.TryValidateStored(value.Text, value.LogicalRecordId, out _, out string preDetail))
                        return preDetail;
                }
                try
                {
                    byte[] before = File.ReadAllBytes(selected);
                    if (RunVgaBootstrapService.DetectState(selected) != RunVgaBootstrapState.ExtendedCp852V5 || before.Length != RunVgaBootstrapService.V5Size)
                        return "Variable VGA materialization requires the frozen V5 executable image.";
                    var texts = scoped.OrderBy(value => value.LogicalRecordId).ToDictionary(value => value.LogicalRecordId, value => value.Text);
                    byte[] after = RunVgaVariableUiService.Appender.Materialize(before, texts);
                    string temporary = selected + ".vgaui.tmp";
                    try
                    {
                        File.WriteAllBytes(temporary, after);
                        File.Move(temporary, selected, true);
                    }
                    finally { if (File.Exists(temporary)) File.Delete(temporary); }
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
                { return "Variable VGA materialization failed: " + ex.Message; }
            }
            // R9F RUNEGA: apply every validated override inside the owned
            // runtime+edition output only (validated one by one above, written
            // atomically). Pristine GameRoot is never touched.
            if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega && scoped.Count > 0)
            {
                try
                {
                    byte[] before = File.ReadAllBytes(selected);
                    if (before.Length != RunEgaBootstrapService.OutputSize)
                        return "RUNEGA Runtime UI materialization requires the frozen output image.";
                    var ordered = scoped.OrderBy(value => value.LogicalRecordId).ToArray();
                    foreach (RuntimeUiTextOverride value in ordered)
                    {
                        if (!RunEgaUiService.IsEditable(value.LogicalRecordId))
                            return UiText.Get("RuntimeUi.BuildNotMaterialized");
                        if (!RunEgaUiService.TryValidateStored(value.Text, value.LogicalRecordId, out _, out string preDetail))
                            return preDetail;
                    }
                    byte[] after = (byte[])before.Clone();
                    RunEgaUiService.ApplyOverridesToImage(after, ordered);
                    RunEgaUiService.VerifyOnlyEgaSpansChanged(before, after,
                        ordered.Select(value => RunEgaUiService.Contract(value.LogicalRecordId)).ToArray());
                    RunEgaBootstrapService.ValidateOutput(after);
                    string temporary = selected + ".ega-ui.tmp";
                    try
                    {
                        File.WriteAllBytes(temporary, after);
                        File.Move(temporary, selected, true);
                    }
                    finally { if (File.Exists(temporary)) File.Delete(temporary); }
                    byte[] reread = File.ReadAllBytes(selected);
                    foreach (RuntimeUiTextOverride value in ordered)
                    {
                        if (!RunEgaUiService.TryDecodeBankSpan(reread, value.LogicalRecordId, out string? roundTrip, out _))
                            return "Patched RUNEGA Runtime UI did not decode deterministically.";
                        if (!roundTrip!.Equals(value.Text, StringComparison.Ordinal))
                            return "Patched RUNEGA Runtime UI does not contain the expected override text.";
                    }
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
                { return "RUNEGA Runtime UI materialization failed: " + ex.Message; }
            }
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return "Selected executable materialization failed: " + ex.Message; }
    }
    private ICompositeBuildStep? CreateBootstrap(VariantContext variant) => variant.RuntimeKind switch
    {
        VariantRuntimeKind.Elvira1Vga => new RunVgaCompositeBuildStep(_directories, _translation.Code),
        VariantRuntimeKind.Elvira1Ega => new RunEgaCompositeBuildStep(_directories, _translation.Code),
        VariantRuntimeKind.Elvira2Vga => new RunItCompositeBuildStep(_directories, _translation.Code),
        _ => null
    };

    private IReadOnlyList<RuntimeUiTextOverride> ScopedUi(VariantContext variant)
    {
        if (_runtimeUi is null || variant is null) return [];
        return _runtimeUi.Overrides.Where(value => value.Runtime == variant.RuntimeKind).OrderBy(value => value.LogicalRecordId).ToArray();
    }
}
