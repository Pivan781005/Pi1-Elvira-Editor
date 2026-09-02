namespace ElviraVgaEditor;

/// <summary>Read-only build-status projection for one semantic variant. It
/// deliberately creates plans only; it never invokes CompositeBuildService.Build.
/// </summary>
internal enum VariantBuildStatus { Missing, Invalid, Incomplete, Ready }

internal sealed record VariantBuildStatusProjection(
    VariantBuildStatus Status,
    VariantLaunchTarget Target,
    int ConfiguredCapabilities,
    int CapabilityCount);

internal sealed class VariantBuildStatusService
{
    private readonly CompositeBuildService _composite;
    private readonly VariantLauncherService _launcher;

    public VariantBuildStatusService(CompositeBuildService composite, VariantLauncherService launcher)
    {
        _composite = composite ?? throw new ArgumentNullException(nameof(composite));
        _launcher = launcher ?? throw new ArgumentNullException(nameof(launcher));
    }

    public VariantBuildStatusProjection Inspect(ProjectContext? project, VariantContext? variant)
    {
        VariantLaunchTarget target = _launcher.Resolve(project, variant);
        if (project is null || variant is null || !ReferenceEquals(project, variant.Project) ||
            target.Ownership != VariantDirectoryOperationStatus.AlreadyValid)
            return new(Map(target.Readiness), target, 0, 0);

        CompositeBuildPlan plan = _composite.CreatePlan(project, variant, CompositeBuildMode.Full);
        int configured = plan.Capabilities.Count(capability => capability.Configured);
        return new(Map(target.Readiness), target, configured, plan.Capabilities.Count);
    }

    private static VariantBuildStatus Map(VariantLaunchReadiness readiness) => readiness switch
    {
        VariantLaunchReadiness.LaunchReady => VariantBuildStatus.Ready,
        VariantLaunchReadiness.BuildIncomplete => VariantBuildStatus.Incomplete,
        VariantLaunchReadiness.VariantMissing => VariantBuildStatus.Missing,
        _ => VariantBuildStatus.Invalid
    };
}
