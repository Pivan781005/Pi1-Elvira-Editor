namespace ElviraVgaEditor;

/// <summary>Variant-local adapter for the frozen RUNEGA production image builder.</summary>
internal sealed class RunEgaCompositeBuildStep : ICompositeBuildStep
{
    private readonly VariantDirectoryService _directories;
    public RunEgaCompositeBuildStep(VariantDirectoryService directories) => _directories = directories ?? throw new ArgumentNullException(nameof(directories));
    public CompositeBuildStage Stage => CompositeBuildStage.ApplyExecutableTransformation;
    public string Name => "RUNEGA frozen CP852 bootstrap";
    public bool AppliesTo(VariantContext variant) => variant is not null && variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega;

    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        if (project is null || variant is null || !ReferenceEquals(project, variant.Project) || project.GameProfile != ElviraGameProfile.Elvira1 || !AppliesTo(variant)) return "RUNEGA bootstrap requires the exact Elvira I EGA VariantContext.";
        try { Elvira1ProductionProfile.VerifyFrozenInvariants(); } catch (Exception ex) { return "Frozen RUNEGA profile is invalid: " + ex.Message; }
        if (!variant.SourceExecutableName.Equals(Elvira1ProductionProfile.ActiveEgaExecutable, StringComparison.OrdinalIgnoreCase) || !variant.GeneratedExecutableName.Equals(Elvira1ProductionProfile.GeneratedSlovakEgaExecutable, StringComparison.OrdinalIgnoreCase) || !GameDataFileService.IsDos83FileName(variant.GeneratedExecutableName)) return "RUNEGA variant filenames do not match the frozen profile.";
        var sources = DisposableVariantBuildService.ValidateSources(project);
        if (sources.Status != DisposableVariantBuildStatus.Success) return "Pristine RUNEGA source preflight failed: " + sources.Status;
        DisposableVariantBuildSource? source = sources.Sources.SingleOrDefault(item => item.ManifestFile.RelativePath.Equals(Elvira1ProductionProfile.ActiveEgaExecutable, StringComparison.OrdinalIgnoreCase));
        return source is null || !Elvira1ProductionProfile.RunEga.PackedOriginal.Matches((int)source.ManifestFile.Size, source.ManifestFile.Sha256) ? "The manifest does not expose the frozen packed RUNEGA source." : null;
    }

    public string? Execute(ProjectContext project, VariantContext variant)
    {
        try
        {
            string root = _directories.GetVariantDirectoryPath(project, variant), source = Path.Combine(root, Elvira1ProductionProfile.ActiveEgaExecutable), output = Path.Combine(root, Elvira1ProductionProfile.GeneratedSlovakEgaExecutable);
            if (!File.Exists(source) || !RunEgaBootstrapService.IsPacked(File.ReadAllBytes(source))) return "Fresh variant RUNEGA.EXE is not the frozen packed original.";
            if (File.Exists(output)) return "Fresh variant already contains generated RUNEGASK.EXE.";
            string hash = RunEgaBootstrapService.CreateFrozenCp852(source, output, GlyphRepository.CreateAllCp852Slots());
            byte[] image = File.ReadAllBytes(output); RunEgaBootstrapService.ValidateOutput(image);
            if (image.Length != RunEgaBootstrapService.OutputSize || hash != "C4028BAB9A35B247008197A5753C5C5185F2DEBFF54DCB099178E318830F90EE") return "Generated RUNEGASK.EXE diverged from the frozen production image.";
            return null;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException) { return "RUNEGA bootstrap failed: " + ex.Message; }
    }
}
