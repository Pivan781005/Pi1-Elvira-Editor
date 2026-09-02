using System.Diagnostics;

namespace ElviraVgaEditor;

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
/// It never invokes CompositeBuildService and never searches game-root files.</summary>
internal sealed class VariantExecutionService
{
    private readonly IVariantProcessRunner _runner;
    private readonly VariantDebugConfiguration _debug;

    public VariantExecutionService(IVariantProcessRunner? runner = null, VariantDebugConfiguration? debug = null)
    {
        _runner = runner ?? new SystemVariantProcessRunner();
        _debug = debug ?? VariantDebugConfiguration.Unavailable;
    }

    public bool IsAvailable(VariantLaunchTarget target, VariantExecutionMode mode) =>
        target.Readiness == VariantLaunchReadiness.LaunchReady && (mode == VariantExecutionMode.Run || _debug.IsConfigured);

    public VariantExecutionResult Execute(VariantLaunchTarget target, VariantExecutionMode mode)
    {
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

    public ProcessStartInfo CreateStartInfo(VariantLaunchTarget target, VariantExecutionMode mode)
    {
        ValidateTarget(target);
        string executablePath = SafeChild(target.WorkingDirectory, target.ExecutableFile);
        string dataPath = SafeChild(target.WorkingDirectory, target.DataFile);
        if (!File.Exists(executablePath)) throw new FileNotFoundException("Generated variant executable is missing.", executablePath);
        if (!File.Exists(dataPath)) throw new FileNotFoundException("Generated variant data file is missing.", dataPath);
        if (mode == VariantExecutionMode.Run)
        {
            var run = new ProcessStartInfo { FileName = executablePath, WorkingDirectory = target.WorkingDirectory, UseShellExecute = false };
            run.ArgumentList.Add(target.DataFile);
            return run;
        }
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
