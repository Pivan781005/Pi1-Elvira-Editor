namespace Pi1ElviraEditor;

/// <summary>R9F V8.6e per-refresh projection reuse for Mods &amp; Launcher.
/// One refresh cycle resolves each required authoritative runtime+edition
/// projection exactly once and reuses it across the Variant Manager, the
/// edition/catalog presentation, the top readiness summary, Run enablement
/// and recovery presentation. This is NOT a stale global cache: the snapshot
/// lives only for one refresh; saving project state, building, switching
/// runtime/edition/installation always builds a fresh snapshot, so later
/// project changes are never hidden. All correctness checks (ownership,
/// manifest, hashes, InputFingerprint) still run inside each resolution.
/// </summary>
internal sealed record ModsLauncherRefreshSnapshot(
    string EditionCode,
    IReadOnlyDictionary<BuiltInVariantId, VariantBuildStatusProjection> StatusByVariant,
    VariantLaunchTarget ActiveTarget,
    LauncherRedirectionPlan ActivePlan,
    IReadOnlyList<VariantEntry> GridEntries)
{
    internal VariantBuildStatusProjection? StatusFor(BuiltInVariantId variantId) =>
        StatusByVariant.TryGetValue(variantId, out VariantBuildStatusProjection? projection) ? projection : null;
}

internal static class ModsLauncherRefreshSnapshotBuilder
{
    /// <summary>Builds one snapshot. The inspect/resolve delegates default to
    /// the production services; tests inject counting wrappers to prove each
    /// required projection resolves exactly once per refresh.</summary>
    internal static ModsLauncherRefreshSnapshot Build(
        ProjectContext project,
        IReadOnlyList<VariantContext> variants,
        VariantContext activeVariant,
        string editionCode,
        Func<ProjectContext, VariantContext, string, VariantBuildStatusProjection> inspect,
        Func<ProjectContext, VariantContext, string, VariantLaunchTarget> resolve,
        Func<ProjectContext, VariantContext, VariantLaunchTarget, LauncherRedirectionPlan> redirection,
        Func<IReadOnlyList<VariantEntry>> gridEntries)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(activeVariant);
        ArgumentNullException.ThrowIfNull(inspect);
        VariantLaunchTarget target = resolve(project, activeVariant, editionCode);
        var byVariant = new Dictionary<BuiltInVariantId, VariantBuildStatusProjection>();
        foreach (VariantContext variant in variants)
        {
            if (!ReferenceEquals(project, variant.Project)) continue;
            if (byVariant.ContainsKey(variant.VariantId)) continue;
            byVariant[variant.VariantId] = inspect(project, variant, editionCode);
        }
        LauncherRedirectionPlan plan = redirection(project, activeVariant, target);
        return new ModsLauncherRefreshSnapshot(editionCode, byVariant, target, plan, gridEntries());
    }
}
