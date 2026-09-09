using System.Diagnostics;

namespace Pi1ElviraEditor;

/// <summary>R9F V8.6e explicit DOS runtime model. A DOS game executable is a
/// DOS TARGET and must never be passed directly to Windows CreateProcess.
/// The Windows process executable is always a supported DOS runtime host
/// (DOSBox family); the host then executes the owned runtime+edition target.
/// </summary>
internal enum DosRuntimeKind
{
    Unknown,
    DosBoxClassic,
    DosBoxStaging,
    DosBoxX
}

internal enum DosRuntimeSource
{
    Unknown,
    GogBundled,
    SystemPath,
    ProgramFiles,
    Registry,
    RememberedSelection,
    UserBrowse
}

internal enum DosRuntimeCompatibility
{
    /// <summary>Probed successfully; runnable.</summary>
    Compatible,
    /// <summary>Recognized family but probing failed; never runnable.</summary>
    Incompatible,
    /// <summary>Not a recognized DOS runtime host; never runnable.</summary>
    Unrecognized
}

/// <summary>One discovered DOS runtime host candidate. Immutable.</summary>
internal sealed record DosRuntimeCandidate(
    DosRuntimeKind Kind,
    string DisplayName,
    string Version,
    string ExecutablePath,
    DosRuntimeSource Source,
    DosRuntimeCompatibility Compatibility,
    string Detail)
{
    internal string StableIdentity => Kind + "|" + ExecutablePath.ToUpperInvariant();
    internal bool IsRunnable => Compatibility == DosRuntimeCompatibility.Compatible && File.Exists(ExecutablePath);
}

/// <summary>Machine execution readiness: build truth crossed with DOS host
/// availability. Build fingerprint/status never depends on host selection.
/// </summary>
internal enum DosRuntimeExecutionReadiness
{
    Runnable,
    BuildNotReady,
    HostNotConfigured
}

internal static class DosRuntimeReadiness
{
    internal static DosRuntimeExecutionReadiness Evaluate(VariantLaunchReadiness buildReadiness, bool hostValid) =>
        buildReadiness != VariantLaunchReadiness.LaunchReady
            ? DosRuntimeExecutionReadiness.BuildNotReady
            : !hostValid
                ? DosRuntimeExecutionReadiness.HostNotConfigured
                : DosRuntimeExecutionReadiness.Runnable;

    internal static bool IsRunEnabled(VariantLaunchTarget? target, DosRuntimeCandidate? selectedHost) =>
        target is not null && selectedHost is not null &&
        Evaluate(target.Readiness, selectedHost.IsRunnable) == DosRuntimeExecutionReadiness.Runnable;
}

/// <summary>Inspectable immutable launch plan. The Windows process executable
/// is always the DOS host; the DOS target (owned runtime+edition executable
/// plus data file) travels inside host arguments/commands only.</summary>
internal sealed record DosRuntimeLaunchPlan(
    DosRuntimeKind HostKind,
    string HostDisplayName,
    string HostExecutable,
    string HostVersion,
    string VariantWorkingDirectory,
    string DosExecutable,
    string DataFile,
    IReadOnlyList<string> BaseConfigFiles,
    IReadOnlyList<string> HostArguments,
    string DisplaySummary)
{
    internal ProcessStartInfo ToProcessStartInfo()
    {
        var start = new ProcessStartInfo
        {
            FileName = HostExecutable,
            WorkingDirectory = VariantWorkingDirectory,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (string argument in HostArguments)
            start.ArgumentList.Add(argument);
        return start;
    }
}
