using System.Security.Cryptography;

namespace ElviraVgaEditor;

/// <summary>Variant-local adapter for the frozen RUNVGA V5 bootstrap; it owns no patch logic.</summary>
internal sealed class RunVgaCompositeBuildStep : ICompositeBuildStep
{
    private readonly VariantDirectoryService _directories;

    public RunVgaCompositeBuildStep(VariantDirectoryService directories) => _directories = directories ?? throw new ArgumentNullException(nameof(directories));

    public CompositeBuildStage Stage => CompositeBuildStage.ApplyExecutableTransformation;
    public string Name => "RUNVGA V5 bootstrap";

    public bool AppliesTo(VariantContext variant) => variant is not null && variant.RuntimeKind == VariantRuntimeKind.Elvira1Vga;

    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        if (project is null || variant is null || !ReferenceEquals(project, variant.Project) || project.GameProfile != ElviraGameProfile.Elvira1 || !AppliesTo(variant))
            return "RUNVGA bootstrap requires the exact Elvira I VGA VariantContext.";
        try { Elvira1ProductionProfile.VerifyFrozenInvariants(); }
        catch (Exception ex) { return "Frozen RUNVGA profile is invalid: " + ex.Message; }
        if (!variant.SourceExecutableName.Equals(Elvira1ProductionProfile.ActiveVgaExecutable, StringComparison.OrdinalIgnoreCase) ||
            !variant.GeneratedExecutableName.Equals(Elvira1ProductionProfile.GeneratedSlovakVgaExecutable, StringComparison.OrdinalIgnoreCase) ||
            !GameDataFileService.IsDos83FileName(variant.GeneratedExecutableName))
            return "RUNVGA variant filenames do not match the frozen production profile.";
        var sources = DisposableVariantBuildService.ValidateSources(project);
        if (sources.Status != DisposableVariantBuildStatus.Success) return "Pristine RUNVGA source preflight failed: " + sources.Status;
        DisposableVariantBuildSource? runVga = sources.Sources.SingleOrDefault(source => source.ManifestFile.RelativePath.Equals(Elvira1ProductionProfile.ActiveVgaExecutable, StringComparison.OrdinalIgnoreCase));
        if (runVga is null || !Elvira1ProductionProfile.RunVga.PackedOriginal.Matches((int)runVga.ManifestFile.Size, runVga.ManifestFile.Sha256))
            return "The manifest does not expose the frozen packed RUNVGA source.";
        return null;
    }

    public string? Execute(ProjectContext project, VariantContext variant)
    {
        try
        {
            string root = _directories.GetVariantDirectoryPath(project, variant);
            string source = Path.Combine(root, Elvira1ProductionProfile.ActiveVgaExecutable);
            string output = Path.Combine(root, Elvira1ProductionProfile.GeneratedSlovakVgaExecutable);
            if (RunVgaBootstrapService.DetectState(source) != RunVgaBootstrapState.OriginalPacked)
                return "Fresh variant RUNVGA.EXE is not the exact frozen packed original.";
            if (File.Exists(output)) return "Fresh variant already contains generated RUNVGASK.EXE.";

            byte[] hud = Elvira1ProductionProfile.RunVga.Font.HudBytes ?? throw new InvalidDataException("Frozen RUNVGA HUD glyph is absent.");
            RunVgaBootstrapResult built = RunVgaBootstrapService.CreateExtendedCp852(source, output, GlyphRepository.CreateAllCp852Slots());
            FontLoadResult loaded = RunVgaFontService.LoadRunVga(built.OutputPath);
            if (RunVgaBootstrapService.DetectState(built.OutputPath) != RunVgaBootstrapState.ExtendedCp852V5 ||
                loaded.Layout != RunVgaFontLayout.ExtendedCp852V5 || loaded.LoadedGlyphCount != 256 ||
                !FontSlotMetadata.IsReserved(loaded, FontSlotMetadata.HudEraseGlyph) ||
                !loaded.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(hud) ||
                new FileInfo(built.OutputPath).Length != RunVgaBootstrapService.V5Size)
                return "Generated RUNVGASK.EXE failed frozen V5/font/HUD validation.";
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException)
        {
            return "RUNVGA bootstrap failed: " + ex.Message;
        }
    }
}
