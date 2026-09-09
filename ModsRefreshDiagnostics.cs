using System.Diagnostics;

namespace Pi1ElviraEditor;

/// <summary>R9F V8.6g narrowly scoped Mods &amp; Launcher steady-state diagnostics.
/// Counts plus elapsed time for the real production OpenModsLauncher path.
/// Dormant by default (no user logging); tests reset/read via Reset/Snapshot.
/// Low-risk: Interlocked counters plus Stopwatch ticks only.</summary>
internal static class ModsRefreshDiagnostics
{
    public static long OpenModsCalls;
    public static long OpenModsTicks;
    public static long SnapshotBuilds;
    public static long InspectEditionCalls;
    public static long InspectEditionTicks;
    public static long ResolveEditionCalls;
    public static long ResolveEditionTicks;
    public static long FingerprintCalls;
    public static long FingerprintTicks;
    public static long ArtifactHashCalls;
    public static long ArtifactHashTicks;
    public static long CreatePlanCalls;
    public static long CreatePlanTicks;
    public static long RedirectionCalls;
    public static long RedirectionTicks;
    public static long RefreshManagerCalls;
    public static long RefreshManagerTicks;
    public static long LowerGridCalls;
    public static long LowerGridTicks;
    public static long RecoveryInspectCalls;
    public static long RecoveryInspectTicks;
    public static long ValidateBaselineCalls;
    public static long ValidateBaselineTicks;
    public static long DosDiscoverCalls;
    public static long DosProbeCalls;
    public static long RecoveryPresentationCalls;
    public static long RecoveryPresentationTicks;
    public static long ValidateSourcesCalls;
    public static long ValidateSourcesTicks;

    public static void Reset()
    {
        OpenModsCalls = 0;
        OpenModsTicks = 0;
        SnapshotBuilds = 0;
        InspectEditionCalls = 0;
        InspectEditionTicks = 0;
        ResolveEditionCalls = 0;
        ResolveEditionTicks = 0;
        FingerprintCalls = 0;
        FingerprintTicks = 0;
        ArtifactHashCalls = 0;
        ArtifactHashTicks = 0;
        CreatePlanCalls = 0;
        CreatePlanTicks = 0;
        RedirectionCalls = 0;
        RedirectionTicks = 0;
        RefreshManagerCalls = 0;
        RefreshManagerTicks = 0;
        LowerGridCalls = 0;
        LowerGridTicks = 0;
        RecoveryInspectCalls = 0;
        RecoveryInspectTicks = 0;
        ValidateBaselineCalls = 0;
        ValidateBaselineTicks = 0;
        DosDiscoverCalls = 0;
        DosProbeCalls = 0;
        RecoveryPresentationCalls = 0;
        RecoveryPresentationTicks = 0;
        ValidateSourcesCalls = 0;
        ValidateSourcesTicks = 0;
    }

    internal readonly struct Scope : IDisposable
    {
        private readonly long _start;
        private readonly Action<long> _addCalls;
        private readonly Action<long> _addTicks;

        public Scope(Action<long> addCalls, Action<long> addTicks)
        {
            _addCalls = addCalls;
            _addTicks = addTicks;
            _start = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            long elapsed = Stopwatch.GetTimestamp() - _start;
            _addCalls(1);
            _addTicks(elapsed);
        }
    }

    internal static Scope MeasureOpenMods() =>
        new(c => OpenModsCalls += c, t => OpenModsTicks += t);

    internal static Scope MeasureInspect() =>
        new(c => InspectEditionCalls += c, t => InspectEditionTicks += t);

    internal static Scope MeasureResolve() =>
        new(c => ResolveEditionCalls += c, t => ResolveEditionTicks += t);

    internal static Scope MeasureFingerprint() =>
        new(c => FingerprintCalls += c, t => FingerprintTicks += t);

    internal static Scope MeasureArtifactHash() =>
        new(c => ArtifactHashCalls += c, t => ArtifactHashTicks += t);

    internal static Scope MeasureCreatePlan() =>
        new(c => CreatePlanCalls += c, t => CreatePlanTicks += t);

    internal static Scope MeasureRedirection() =>
        new(c => RedirectionCalls += c, t => RedirectionTicks += t);

    internal static Scope MeasureRefreshManager() =>
        new(c => RefreshManagerCalls += c, t => RefreshManagerTicks += t);

    internal static Scope MeasureLowerGrid() =>
        new(c => LowerGridCalls += c, t => LowerGridTicks += t);

    internal static Scope MeasureRecoveryInspect() =>
        new(c => RecoveryInspectCalls += c, t => RecoveryInspectTicks += t);

    internal static Scope MeasureValidateBaseline() =>
        new(c => ValidateBaselineCalls += c, t => ValidateBaselineTicks += t);

    internal static Scope MeasureRecoveryPresentation() =>
        new(c => RecoveryPresentationCalls += c, t => RecoveryPresentationTicks += t);

    internal static Scope MeasureValidateSources() =>
        new(c => ValidateSourcesCalls += c, t => ValidateSourcesTicks += t);

    internal static double TicksToMilliseconds(long ticks) =>
        ticks * 1000.0 / Stopwatch.Frequency;

    internal sealed record Snapshot(
        long OpenModsCalls,
        long SnapshotBuilds,
        long InspectEditionCalls,
        long ResolveEditionCalls,
        long FingerprintCalls,
        long ArtifactHashCalls,
        long CreatePlanCalls,
        long RedirectionCalls,
        long RecoveryInspectCalls,
        long ValidateBaselineCalls,
        long DosDiscoverCalls,
        long DosProbeCalls,
        long ValidateSourcesCalls,
        double OpenModsMilliseconds,
        double InspectMilliseconds,
        double ResolveMilliseconds,
        double FingerprintMilliseconds,
        double CreatePlanMilliseconds,
        double RecoveryMilliseconds,
        double ValidateBaselineMilliseconds,
        double ValidateSourcesMilliseconds);

    internal static Snapshot Capture()
    {
        return new(
            OpenModsCalls,
            SnapshotBuilds,
            InspectEditionCalls,
            ResolveEditionCalls,
            FingerprintCalls,
            ArtifactHashCalls,
            CreatePlanCalls,
            RedirectionCalls,
            RecoveryInspectCalls,
            ValidateBaselineCalls,
            DosDiscoverCalls,
            DosProbeCalls,
            ValidateSourcesCalls,
            TicksToMilliseconds(OpenModsTicks),
            TicksToMilliseconds(InspectEditionTicks),
            TicksToMilliseconds(ResolveEditionTicks),
            TicksToMilliseconds(FingerprintTicks),
            TicksToMilliseconds(CreatePlanTicks),
            TicksToMilliseconds(RecoveryInspectTicks),
            TicksToMilliseconds(ValidateBaselineTicks),
            TicksToMilliseconds(ValidateSourcesTicks));
    }
}
