namespace Pi1ElviraEditor;

/// <summary>Variant-local adapter for the proven RUNIT V2 split-font base only.</summary>
internal sealed class RunItCompositeBuildStep : ICompositeBuildStep
{
    private readonly VariantDirectoryService _directories;
    private readonly string _projectCode;
    public RunItCompositeBuildStep(VariantDirectoryService directories, string projectCode = "EN") => (_directories, _projectCode) = (directories ?? throw new ArgumentNullException(nameof(directories)), projectCode);
    public CompositeBuildStage Stage => CompositeBuildStage.ApplyExecutableTransformation;
    public string Name => "RUNIT V2 split-font bootstrap";
    public bool AppliesTo(VariantContext variant) => variant is not null && variant.RuntimeKind == VariantRuntimeKind.Elvira2Vga;
    public string? Preflight(ProjectContext project, VariantContext variant)
    {
        if(project is null||variant is null||!ReferenceEquals(project,variant.Project)||project.GameProfile!=ElviraGameProfile.Elvira2||!AppliesTo(variant))return "RUNIT bootstrap requires the exact Elvira II VGA VariantContext.";
        try{Elvira2ProductionProfile.VerifyFrozenInvariants();}catch(Exception ex){return "Frozen RUNIT profile is invalid: "+ex.Message;}
        if(!variant.SourceExecutableName.Equals(Elvira2ProductionProfile.ActiveExecutable,StringComparison.OrdinalIgnoreCase)||!variant.GeneratedExecutableName.Equals(Elvira2ProductionProfile.GeneratedSlovakExecutable,StringComparison.OrdinalIgnoreCase)||!GameDataFileService.IsDos83FileName(variant.GeneratedExecutableName))return "RUNIT variant filenames do not match the frozen profile.";
        var sources=DisposableVariantBuildService.ValidateSources(project);if(sources.Status!=DisposableVariantBuildStatus.Success)return "Pristine RUNIT source preflight failed: "+sources.Status;
        DisposableVariantBuildSource? source=sources.Sources.SingleOrDefault(x=>x.ManifestFile.RelativePath.Equals(Elvira2ProductionProfile.ActiveExecutable,StringComparison.OrdinalIgnoreCase));
        return source is null||!Elvira2ProductionProfile.PackedOriginal.Matches((int)source.ManifestFile.Size,source.ManifestFile.Sha256)?"The manifest does not expose the frozen packed RUNIT source.":null;
    }
    public string? Execute(ProjectContext project,VariantContext variant)
    {
        try{string root=_directories.GetVariantEditionDirectoryPath(project,variant,_projectCode),source=Path.Combine(root,Elvira2ProductionProfile.ActiveExecutable),output=Path.Combine(root,Elvira2ProductionProfile.GeneratedSlovakExecutable);
        // R9D: fail closed with a Missing vs Unsupported distinction; never patch an unknown binary.
        SupportedExecutableClassification identity=SupportedExecutableIdentityService.ClassifyRunIt(source);
        if(identity.Identity!=SupportedExecutableIdentity.SupportedPacked)return SupportedExecutableIdentityService.DescribeBlocked(identity,"RUNIT bootstrap");
        if(File.Exists(output))return "Fresh variant already contains generated RUNITSK.EXE.";RunItBootstrapResult built=RunItBootstrapService.CreateExtendedCp852(source,output,GlyphRepository.CreateAllCp852Slots());byte[] image=File.ReadAllBytes(built.OutputPath);RunItBootstrapService.ValidateExtended(image);if(image.Length!=RunItBootstrapService.ExtendedSize||built.Sha256!=Elvira2ProductionProfile.DeterministicFontEnabled.Sha256)return "Generated RUNITSK.EXE diverged from the frozen V2 image.";return null;}
        catch(Exception ex)when(ex is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException){return "RUNIT bootstrap failed: "+ex.Message;}
    }
}
