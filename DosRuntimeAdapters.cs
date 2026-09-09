using System.Diagnostics;

namespace Pi1ElviraEditor;

/// <summary>Read-only host evidence for identification. Never launches a game.</summary>
internal sealed record DosRuntimeHostEvidence(string FileName, string? ProductName, string? FileVersion);

internal sealed record DosRuntimeProbeResult(bool Success, string Output, string Error, int ExitCode);

/// <summary>Injectable probe seam: production spawns the candidate with a
/// version flag under a timeout; tests substitute a fake. Probing never
/// launches a game and never mutates GameRoot.</summary>
internal interface IDosRuntimeProbeRunner
{
    DosRuntimeProbeResult Probe(string executable, IReadOnlyList<string> arguments, int timeoutMilliseconds);
}

internal sealed class SystemDosRuntimeProbeRunner : IDosRuntimeProbeRunner
{
    internal const int DefaultTimeoutMilliseconds = 10000;

    public DosRuntimeProbeResult Probe(string executable, IReadOnlyList<string> arguments, int timeoutMilliseconds)
    {
        try
        {
            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                FileName = executable,
                WorkingDirectory = Path.GetDirectoryName(Path.GetFullPath(executable)) ?? string.Empty,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };
            foreach (string argument in arguments)
                process.StartInfo.ArgumentList.Add(argument);
            if (!process.Start())
                return new(false, string.Empty, "Process did not start.", -1);
            bool exited = process.WaitForExit(timeoutMilliseconds);
            if (!exited)
            {
                try { process.Kill(entireProcessTree: true); } catch { }
                return new(false, string.Empty, "Probe timed out.", -1);
            }
            string output = string.Empty;
            string error = string.Empty;
            try { output = process.StandardOutput.ReadToEnd(); } catch { }
            try { error = process.StandardError.ReadToEnd(); } catch { }
            return new(true, output ?? string.Empty, error ?? string.Empty, process.ExitCode);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            return new(false, string.Empty, ex.Message, -1);
        }
    }
}

internal static class DosRuntimeHostEvidenceReader
{
    internal static DosRuntimeHostEvidence Read(string executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath))
            return new(string.Empty, null, null);
        string fileName = Path.GetFileName(executablePath);
        try
        {
            FileVersionInfo version = FileVersionInfo.GetVersionInfo(executablePath);
            return new(fileName, version.ProductName, version.FileVersion);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return new(fileName, null, null);
        }
    }
}

/// <summary>Host-specific adapter: owns probe semantics and launch-plan
/// syntax for exactly one DOSBox family. Version-switch syntax is never
/// guessed globally.</summary>
internal interface IDosRuntimeAdapter
{
    DosRuntimeKind Kind { get; }
    string FamilyDisplayName { get; }
    IReadOnlyList<string> ExecutableFileNames { get; }
    IReadOnlyList<string> VersionArguments { get; }
    bool Identifies(DosRuntimeHostEvidence evidence);
    string ParseVersion(string probeOutput);
    IReadOnlyList<string> BuildArguments(DosRuntimeLaunchRequest request);
}

/// <summary>Launch-plan inputs shared by all adapters.</summary>
internal sealed record DosRuntimeLaunchRequest(
    string VariantWorkingDirectory,
    string DosExecutable,
    string DataFile,
    string SoundSwitch,
    IReadOnlyList<string> BaseConfigFiles);

internal sealed class DosBoxClassicAdapter : IDosRuntimeAdapter
{
    public DosRuntimeKind Kind => DosRuntimeKind.DosBoxClassic;
    public string FamilyDisplayName => "DOSBox Classic";
    public IReadOnlyList<string> ExecutableFileNames => ["dosbox.exe"];
    // Classic manual: dosbox -version outputs version and exits.
    public IReadOnlyList<string> VersionArguments => ["-version"];

    public bool Identifies(DosRuntimeHostEvidence evidence)
    {
        if (evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase)) return false;
        if ((evidence.ProductName ?? string.Empty).Contains("Staging", StringComparison.OrdinalIgnoreCase)) return false;
        if ((evidence.ProductName ?? string.Empty).Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase)) return false;
        if ((evidence.ProductName ?? string.Empty).Contains("DOSBox", StringComparison.OrdinalIgnoreCase)) return true;
        return evidence.FileName.Equals("dosbox.exe", StringComparison.OrdinalIgnoreCase);
    }

    public string ParseVersion(string probeOutput)
    {
        string first = (probeOutput ?? string.Empty).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() ?? string.Empty;
        return string.IsNullOrWhiteSpace(first) ? "unknown" : first.Length > 64 ? first[..64] : first;
    }

    public IReadOnlyList<string> BuildArguments(DosRuntimeLaunchRequest request)
    {
        var arguments = new List<string>();
        foreach (string config in request.BaseConfigFiles)
        {
            arguments.Add("-conf");
            arguments.Add(config);
        }
        // Classic manual: multiple -c commands run in order before name.
        arguments.Add("-c");
        arguments.Add($"mount C \"{request.VariantWorkingDirectory}\"");
        arguments.Add("-c");
        arguments.Add("C:");
        arguments.Add("-c");
        arguments.Add($"{request.DosExecutable} {request.DataFile} {request.SoundSwitch}".Trim());
        arguments.Add("-c");
        arguments.Add("exit");
        // Classic manual: -exit closes DOSBox when the DOS application ends.
        arguments.Add("-exit");
        return arguments;
    }
}

internal sealed class DosBoxStagingAdapter : IDosRuntimeAdapter
{
    public DosRuntimeKind Kind => DosRuntimeKind.DosBoxStaging;
    public string FamilyDisplayName => "DOSBox Staging";
    public IReadOnlyList<string> ExecutableFileNames => ["dosbox.exe"];
    // Staging manual: dosbox --version prints version information and exits.
    public IReadOnlyList<string> VersionArguments => ["--version"];

    public bool Identifies(DosRuntimeHostEvidence evidence)
    {
        if (evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase)) return false;
        if ((evidence.ProductName ?? string.Empty).Contains("Staging", StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    public string ParseVersion(string probeOutput)
    {
        foreach (string line in (probeOutput ?? string.Empty).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmed = line.Trim();
            if (trimmed.Contains("taging", StringComparison.OrdinalIgnoreCase) || System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"\d+\.\d+"))
                return trimmed.Length > 64 ? trimmed[..64] : trimmed;
        }
        return "unknown";
    }

    public IReadOnlyList<string> BuildArguments(DosRuntimeLaunchRequest request)
    {
        var arguments = new List<string>();
        // Staging manual: --conf layered, later files override earlier ones.
        arguments.Add("--noprimaryconf");
        arguments.Add("--nolocalconf");
        foreach (string config in request.BaseConfigFiles)
        {
            arguments.Add("--conf");
            arguments.Add(config);
        }
        // Staging manual: multiple -c commands run before PATH handling.
        arguments.Add("-c");
        arguments.Add($"mount C \"{request.VariantWorkingDirectory}\"");
        arguments.Add("-c");
        arguments.Add("C:");
        arguments.Add("-c");
        arguments.Add($"{request.DosExecutable} {request.DataFile} {request.SoundSwitch}".Trim());
        arguments.Add("-c");
        arguments.Add("exit");
        // Staging manual: --exit exits after running -c commands.
        arguments.Add("--exit");
        return arguments;
    }
}

internal sealed class DosBoxXAdapter : IDosRuntimeAdapter
{
    public DosRuntimeKind Kind => DosRuntimeKind.DosBoxX;
    public string FamilyDisplayName => "DOSBox-X";
    public IReadOnlyList<string> ExecutableFileNames => ["dosbox-x.exe"];
    // DOSBox-X manual: -version displays version information and exits.
    public IReadOnlyList<string> VersionArguments => ["-version"];

    public bool Identifies(DosRuntimeHostEvidence evidence)
    {
        if (evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase)) return true;
        return (evidence.ProductName ?? string.Empty).Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase);
    }

    public string ParseVersion(string probeOutput)
    {
        string first = (probeOutput ?? string.Empty).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() ?? string.Empty;
        return string.IsNullOrWhiteSpace(first) ? "unknown" : first.Length > 64 ? first[..64] : first;
    }

    public IReadOnlyList<string> BuildArguments(DosRuntimeLaunchRequest request)
    {
        var arguments = new List<string>();
        // DOSBox-X manual: multiple -conf options are overlaid on each other.
        foreach (string config in request.BaseConfigFiles)
        {
            arguments.Add("-conf");
            arguments.Add(config);
        }
        // DOSBox-X manual: multiple -c commands run before FILE.
        arguments.Add("-c");
        arguments.Add($"mount C \"{request.VariantWorkingDirectory}\"");
        arguments.Add("-c");
        arguments.Add("C:");
        arguments.Add("-c");
        arguments.Add($"{request.DosExecutable} {request.DataFile} {request.SoundSwitch}".Trim());
        arguments.Add("-c");
        arguments.Add("exit");
        // DOSBox-X manual: -exit closes itself when the DOS program ends.
        arguments.Add("-exit");
        return arguments;
    }
}

internal static class DosRuntimeAdapters
{
    internal static readonly IReadOnlyList<IDosRuntimeAdapter> All =
    [
        new DosBoxXAdapter(),
        new DosBoxStagingAdapter(),
        new DosBoxClassicAdapter()
    ];

    /// <summary>X first: dosbox-x.exe must never fall through to Classic, and
    /// Staging metadata must never be claimed by the Classic filename hint.</summary>
    internal static IDosRuntimeAdapter? Identify(DosRuntimeHostEvidence evidence)
    {
        foreach (IDosRuntimeAdapter adapter in All)
        {
            try
            {
                if (adapter.Identifies(evidence)) return adapter;
            }
            catch { }
        }
        return null;
    }
}
