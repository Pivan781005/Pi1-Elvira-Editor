using System.Diagnostics;

namespace Pi1ElviraEditor;

internal enum VariantExecutionMode { Run, Debug }
internal sealed record VariantDebugConfiguration(string HostExecutable, IReadOnlyList<string> HostArguments)
{
    public static VariantDebugConfiguration Unavailable { get; } = new(string.Empty, []);
    public bool IsConfigured => !string.IsNullOrWhiteSpace(HostExecutable) && Path.IsPathRooted(HostExecutable);

    /// <summary>Optional machine-local developer configuration. It is never
    /// persisted in project or game state and has no hard-coded DOSBox path.</summary>
    public static VariantDebugConfiguration FromEnvironment()
    {
        string? host = Environment.GetEnvironmentVariable("PI1_DEBUG_HOST");
        return string.IsNullOrWhiteSpace(host) ? Unavailable : new(host.Trim(), []);
    }
}
internal sealed record VariantExecutionResult(bool Started, VariantLaunchReadiness Readiness, string Detail, ProcessStartInfo? StartInfo = null);

/// <summary>Narrow process seam for smoke tests. Production uses the system
/// runner; tests record ProcessStartInfo and start no external process.</summary>
internal interface IVariantProcessRunner { void Start(ProcessStartInfo startInfo); }
internal sealed class SystemVariantProcessRunner : IVariantProcessRunner
{
    public void Start(ProcessStartInfo startInfo)
    {
        if (Process.Start(startInfo) is null) throw new InvalidOperationException("Process.Start returned no process.");
    }
}

/// <summary>Executes only an already-resolved, LaunchReady semantic target.
/// R9F V8.6e P0 rule: a DOS game executable is a DOS TARGET and is never
/// passed to Windows CreateProcess as the host executable. Run always goes
/// through a DOS runtime launch plan whose FileName is the probed DOS host;
/// the legacy direct-DOS Run path fails closed. Debug keeps its explicit
/// developer host. It never invokes CompositeBuildService and never searches
/// game-root files.</summary>
internal sealed class VariantExecutionService
{
    private readonly IVariantProcessRunner _runner;
    private readonly VariantDebugConfiguration _debug;

    public VariantExecutionService(IVariantProcessRunner? runner = null, VariantDebugConfiguration? debug = null)
    {
        _runner = runner ?? new SystemVariantProcessRunner();
        _debug = debug ?? VariantDebugConfiguration.Unavailable;
    }

    /// <summary>Build-side availability only. Debug requires its configured
    /// developer host. Run NEVER reports direct executable availability:
    /// a DOS target requires a DOS runtime launch plan/host (see
    /// DosRuntimeExecutionReadiness); LaunchReady alone never means Windows
    /// Run is available. Use ExecutePlan for the only valid Run path.</summary>
    public bool IsAvailable(VariantLaunchTarget target, VariantExecutionMode mode)
    {
        if (target.Readiness != VariantLaunchReadiness.LaunchReady) return false;
        if (mode == VariantExecutionMode.Run) return false;
        return _debug.IsConfigured;
    }

    /// <summary>Build-side launch-readiness without any host claim. True when
    /// the semantic target is LaunchReady, regardless of DOS host or debug
    /// host configuration.</summary>
    public bool IsBuildReady(VariantLaunchTarget target) =>
        target.Readiness == VariantLaunchReadiness.LaunchReady;

    public VariantExecutionResult Execute(VariantLaunchTarget target, VariantExecutionMode mode)
    {
        if (mode == VariantExecutionMode.Run)
            throw new InvalidOperationException("Run requires a DOS runtime launch plan. A DOS executable must never be started directly by Windows.");
        if (!IsAvailable(target, mode))
        {
            string reason = target.Readiness != VariantLaunchReadiness.LaunchReady
                ? "Variant is not ready to run. Build status: " + target.Readiness + "."
                : "Debug host is not configured.";
            return new(false, target.Readiness, reason);
        }
        try
        {
            ProcessStartInfo start = CreateStartInfo(target, mode);
            _runner.Start(start);
            return new(true, target.Readiness, "Started.", start);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException or System.ComponentModel.Win32Exception)
        {
            return new(false, target.Readiness, "Unable to start variant: " + ex.Message);
        }
    }

    /// <summary>Authoritative Run entry point: starts the DOS host from the
    /// plan. ProcessStartInfo.FileName is always the host executable.</summary>
    public VariantExecutionResult ExecutePlan(DosRuntimeLaunchPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        try
        {
            ProcessStartInfo start = CreatePlanStartInfo(plan);
            _runner.Start(start);
            return new(true, VariantLaunchReadiness.LaunchReady, "Started in " + plan.HostDisplayName + ".", start);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or IOException or UnauthorizedAccessException or System.ComponentModel.Win32Exception)
        {
            return new(false, VariantLaunchReadiness.BuildIncomplete, "Unable to start variant: " + ex.Message);
        }
    }

    public ProcessStartInfo CreatePlanStartInfo(DosRuntimeLaunchPlan plan)
    {
        ArgumentNullException.ThrowIfNull(plan);
        if (string.IsNullOrWhiteSpace(plan.HostExecutable) || !Path.IsPathRooted(plan.HostExecutable) || !File.Exists(plan.HostExecutable))
            throw new InvalidOperationException("DOS runtime host executable is missing.");
        if (string.IsNullOrWhiteSpace(plan.VariantWorkingDirectory) || !Directory.Exists(plan.VariantWorkingDirectory))
            throw new InvalidOperationException("Variant working directory is missing.");
        if (!GameDataFileService.IsDos83FileName(plan.DosExecutable + ".EXE") || !GameDataFileService.IsDos83FileName(plan.DataFile))
            throw new InvalidOperationException("Launch plan DOS names are invalid.");
        string dosPath = SafeChild(plan.VariantWorkingDirectory, plan.DosExecutable + ".EXE");
        string dataPath = SafeChild(plan.VariantWorkingDirectory, plan.DataFile);
        if (!File.Exists(dosPath)) throw new FileNotFoundException("Generated variant executable is missing.", dosPath);
        if (!File.Exists(dataPath)) throw new FileNotFoundException("Generated variant data file is missing.", dataPath);
        foreach (string argument in plan.HostArguments)
        {
            if (string.IsNullOrWhiteSpace(argument) || argument.Any(char.IsControl))
                throw new ArgumentException("DOS runtime launch argument is invalid.");
        }
        var start = new ProcessStartInfo
        {
            FileName = plan.HostExecutable,
            WorkingDirectory = plan.VariantWorkingDirectory,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (string argument in plan.HostArguments)
            start.ArgumentList.Add(argument);
        return start;
    }

    public ProcessStartInfo CreateStartInfo(VariantLaunchTarget target, VariantExecutionMode mode)
    {
        if (mode == VariantExecutionMode.Run)
            throw new InvalidOperationException("Run requires a DOS runtime launch plan. A DOS executable must never be started directly by Windows.");
        ValidateTarget(target);
        string executablePath = SafeChild(target.WorkingDirectory, target.ExecutableFile);
        string dataPath = SafeChild(target.WorkingDirectory, target.DataFile);
        if (!File.Exists(executablePath)) throw new FileNotFoundException("Generated variant executable is missing.", executablePath);
        if (!File.Exists(dataPath)) throw new FileNotFoundException("Generated variant data file is missing.", dataPath);
        if (!_debug.IsConfigured) throw new InvalidOperationException("Debug host is not configured.");
        if (!File.Exists(_debug.HostExecutable)) throw new FileNotFoundException("Configured debug host is missing.", _debug.HostExecutable);
        var debug = new ProcessStartInfo { FileName = _debug.HostExecutable, WorkingDirectory = target.WorkingDirectory, UseShellExecute = false };
        foreach (string argument in _debug.HostArguments) debug.ArgumentList.Add(ValidateHostArgument(argument));
        debug.ArgumentList.Add(executablePath);
        debug.ArgumentList.Add(target.DataFile);
        return debug;
    }

    private static void ValidateTarget(VariantLaunchTarget target)
    {
        if (target.Readiness != VariantLaunchReadiness.LaunchReady)
            throw new InvalidOperationException("Variant is not launch-ready.");
        if (target.Ownership != VariantDirectoryOperationStatus.AlreadyValid || string.IsNullOrWhiteSpace(target.WorkingDirectory) ||
            !Directory.Exists(target.WorkingDirectory) || !GameDataFileService.IsDos83FileName(target.ExecutableFile) ||
            !target.ExecutableFile.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase) || !GameDataFileService.IsDos83FileName(target.DataFile))
            throw new InvalidOperationException("Launch target is invalid.");
    }

    private static string SafeChild(string parent, string component)
    {
        if (Path.GetFileName(component) != component || component.IndexOfAny(['\\', '/', '"']) >= 0)
            throw new ArgumentException("DOS command component is invalid.", nameof(component));
        string child = Path.GetFullPath(Path.Combine(parent, component));
        string normalized = Path.GetFullPath(parent).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        if (!child.StartsWith(normalized, StringComparison.OrdinalIgnoreCase)) throw new ArgumentException("Launch component escaped variant root.");
        return child;
    }

    private static string ValidateHostArgument(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Any(char.IsControl)) throw new ArgumentException("Debug host argument is invalid.");
        return value;
    }
}
