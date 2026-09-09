using System.Diagnostics;

namespace Pi1ElviraEditor;

/// <summary>Read-only host evidence for identification. Never launches a game.
/// Version-resource fields beyond ProductName/FileVersion are optional so
/// older call sites keep compiling; DOSBox-X positive identification relies
/// on the extended metadata.</summary>
internal sealed record DosRuntimeHostEvidence(
    string FileName,
    string? ProductName,
    string? FileVersion,
    string? ProductVersion = null,
    string? OriginalFilename = null,
    string? FileDescription = null);

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
            return new(fileName, version.ProductName, version.FileVersion,
                version.ProductVersion, version.OriginalFilename, version.FileDescription);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return new(fileName, null, null);
        }
    }
}

/// <summary>Host-specific adapter: owns probe semantics and launch-plan
/// syntax for exactly one DOSBox family. Version-switch syntax is never
/// guessed globally. Filename is only a candidate hint; Compatible requires
/// positive family-specific probe validation via IsValidProbe.</summary>
internal interface IDosRuntimeAdapter
{
    DosRuntimeKind Kind { get; }
    string FamilyDisplayName { get; }
    IReadOnlyList<string> ExecutableFileNames { get; }
    IReadOnlyList<string> VersionArguments { get; }
    bool Identifies(DosRuntimeHostEvidence evidence);
    /// <summary>R9F V8.6f: positive family-specific probe validation.
    /// Requires acceptable completion (Success + exit 0), positive
    /// family output with parseable version, and no contradictory family
    /// evidence. Filename alone never validates.</summary>
    bool IsValidProbe(DosRuntimeHostEvidence evidence, DosRuntimeProbeResult probe);
    /// <summary>R9F V8.6j: non-interactive metadata validation. False for
    /// families that require a process version probe (Classic/Staging); true
    /// only for DOSBox-X, which must never be executed merely to identify it
    /// on Windows (its -version path opens an interactive console).</summary>
    bool SupportsMetadataValidation { get; }
    /// <summary>R9F V8.6j: positively identifies the family from authoritative
    /// executable metadata without spawning any process. Returns false with a
    /// diagnostic detail for filename-only, unrelated, contradictory, or
    /// otherwise unusable metadata. Never falls back to process execution.</summary>
    bool TryValidateMetadata(DosRuntimeHostEvidence evidence, out string version, out string detail);
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

/// <summary>R9F V8.6k deterministic user-facing version normalization for
/// Windows FileVersionInfo values. Fully numeric comma-separated versions
/// (e.g. real GOG Classic "0,74,2,1") display dotted ("0.74.2.1"); already
/// dotted values pass through; anything without a usable dotted token yields
/// "unknown". Stable, parseable, no timestamps, no process execution.</summary>
internal static class DosRuntimeMetadataVersions
{
    internal static string Normalize(string? raw)
    {
        string text = (raw ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(text)) return "unknown";
        string compact = text.Replace(" ", string.Empty);
        if (System.Text.RegularExpressions.Regex.IsMatch(compact, @"^\d+(,\d+)+$"))
            return compact.Replace(',', '.');
        if (System.Text.RegularExpressions.Regex.IsMatch(text, @"^\d+(\.\d+)*$"))
            return text.Length > 64 ? text[..64] : text;
        System.Text.RegularExpressions.Match match =
            System.Text.RegularExpressions.Regex.Match(text, @"\d+(\.\d+)+");
        if (!match.Success) return "unknown";
        string token = match.Value;
        return token.Length > 64 ? token[..64] : token;
    }
}

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

    /// <summary>R9F V8.6f Classic positive validation. Accepts genuine
    /// Classic/GOG "DOSBox version X.Y" output (Product may be absent for
    /// old GOG). Rejects Staging, DOSBox-X, "Unknown option", and arbitrary
    /// renamed outputs. All supported Classic hosts return exit 0 for
    /// "-version" per the Classic manual; any other code fails closed.</summary>
    public bool IsValidProbe(DosRuntimeHostEvidence evidence, DosRuntimeProbeResult probe)
    {
        if (evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase)) return false;
        string product = evidence.ProductName ?? string.Empty;
        if (product.Contains("Staging", StringComparison.OrdinalIgnoreCase)) return false;
        if (product.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase)) return false;
        if (probe is not { Success: true }) return false;
        // Real Classic/Staging/X version probes exit 0. No supported host
        // legitimately returns another code; accept only 0.
        if (probe.ExitCode != 0) return false;
        string combined = (probe.Output ?? string.Empty) + "\n" + (probe.Error ?? string.Empty);
        if (combined.Contains("staging", StringComparison.OrdinalIgnoreCase)) return false;
        if (combined.Contains("dosbox-x", StringComparison.OrdinalIgnoreCase)) return false;
        if (System.Text.RegularExpressions.Regex.IsMatch(combined, @"dosbox\s+x\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(combined, @"DOSBox\s+version\s+\d+\.\d+", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) return false;
        return true;
    }

    public string ParseVersion(string probeOutput)
    {
        foreach (string line in (probeOutput ?? string.Empty).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmed = line.Trim();
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"DOSBox\s+version\s+\d+\.\d+", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                return trimmed.Length > 64 ? trimmed[..64] : trimmed;
        }
        return "unknown";
    }

    public bool SupportsMetadataValidation => true;

    /// <summary>R9F V8.6k non-interactive DOSBox Classic identification.
    /// Positive contract: ProductName and/or FileDescription positively
    /// identifies DOSBox, is NOT Staging, is NOT DOSBox-X, a usable
    /// FileVersion/ProductVersion exists, and the executable/original filename
    /// corroborates (dosbox.exe) without ever being sole evidence. Never
    /// spawns dosbox.exe -version and never falls back to it.</summary>
    public bool TryValidateMetadata(DosRuntimeHostEvidence evidence, out string version, out string detail)
    {
        version = string.Empty;
        string product = evidence.ProductName ?? string.Empty;
        string description = evidence.FileDescription ?? string.Empty;
        if ((product.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase) ||
                description.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase)) ||
            System.Text.RegularExpressions.Regex.IsMatch(product + " " + description, @"dosbox\s+x\b", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
        {
            detail = "Executable metadata does not positively identify DOSBox Classic.";
            return false;
        }
        if (product.Contains("Staging", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("Staging", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not positively identify DOSBox Classic.";
            return false;
        }
        if (!product.Contains("DOSBox", StringComparison.OrdinalIgnoreCase) &&
            !description.Contains("DOSBox", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not positively identify DOSBox Classic.";
            return false;
        }
        string normalized = DosRuntimeMetadataVersions.Normalize(
            !string.IsNullOrWhiteSpace(evidence.FileVersion) ? evidence.FileVersion : evidence.ProductVersion);
        if (normalized.Equals("unknown", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not contain a usable version.";
            return false;
        }
        if (!evidence.FileName.Equals("dosbox.exe", StringComparison.OrdinalIgnoreCase) &&
            !(evidence.OriginalFilename ?? string.Empty).Equals("dosbox.exe", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not positively identify DOSBox Classic.";
            return false;
        }
        version = normalized;
        detail = "Identified from executable metadata.";
        return true;
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

    /// <summary>R9F V8.6l shared Staging family marker: DOSBox plus optional
    /// whitespace/hyphen plus Staging (DOSBox Staging, DOSBox-Staging, any
    /// capitalization) in authoritative metadata. The generic English word
    /// "Staging" alone (Video Staging Utility, Deployment Staging Tool) never
    /// identifies the DOSBox family. Used identically by Identify and
    /// TryValidateMetadata so hint and validation share one semantics.</summary>
    internal static bool HasStagingFamilyMarker(string? value) =>
        !string.IsNullOrEmpty(value) &&
        System.Text.RegularExpressions.Regex.IsMatch(value, @"dosbox[\s\-]*staging", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

    public bool Identifies(DosRuntimeHostEvidence evidence)
    {
        if (evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase)) return false;
        return HasStagingFamilyMarker(evidence.ProductName) || HasStagingFamilyMarker(evidence.FileDescription);
    }

    /// <summary>R9F V8.6f Staging positive validation. Requires a
    /// Staging-specific probe ("dosbox-staging" + version) and rejects
    /// Classic/X outputs. Metadata contradiction rejects: a Classic-positive
    /// Product ("DOSBox" without "Staging") or an X-positive Product never
    /// becomes Staging via probe alone. Null/unrelated Product without a
    /// contradictory family marker may still validate via a strong Staging
    /// probe. Exit 0 only.</summary>
    public bool IsValidProbe(DosRuntimeHostEvidence evidence, DosRuntimeProbeResult probe)
    {
        if (evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase)) return false;
        string product = evidence.ProductName ?? string.Empty;
        if (product.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase)) return false;
        if (product.Contains("DOSBox", StringComparison.OrdinalIgnoreCase) &&
            !product.Contains("Staging", StringComparison.OrdinalIgnoreCase)) return false;
        if (probe is not { Success: true }) return false;
        if (probe.ExitCode != 0) return false;
        string combined = (probe.Output ?? string.Empty) + "\n" + (probe.Error ?? string.Empty);
        if (combined.Contains("dosbox-x", StringComparison.OrdinalIgnoreCase)) return false;
        if (!combined.Contains("staging", StringComparison.OrdinalIgnoreCase)) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(combined, @"dosbox[-\s]?staging", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(combined, @"\d+\.\d+")) return false;
        return true;
    }

    public string ParseVersion(string probeOutput)
    {
        foreach (string line in (probeOutput ?? string.Empty).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmed = line.Trim();
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"dosbox[-\s]?staging", System.Text.RegularExpressions.RegexOptions.IgnoreCase) &&
                System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"\d+\.\d+"))
                return trimmed.Length > 64 ? trimmed[..64] : trimmed;
        }
        return "unknown";
    }

    public bool SupportsMetadataValidation => true;

    /// <summary>R9F V8.6k non-interactive DOSBox Staging identification.
    /// Positive contract: ProductName and/or FileDescription carries the
    /// shared DOSBox-family Staging marker (see HasStagingFamilyMarker), a
    /// usable FileVersion/ProductVersion exists, no DOSBox-X contradiction and
    /// no Classic-only identity contradiction exist, and the
    /// executable/original filename may corroborate without ever being sole
    /// evidence. Never spawns dosbox.exe --version and never falls back to it.</summary>
    public bool TryValidateMetadata(DosRuntimeHostEvidence evidence, out string version, out string detail)
    {
        version = string.Empty;
        string product = evidence.ProductName ?? string.Empty;
        string description = evidence.FileDescription ?? string.Empty;
        if (product.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase) ||
            description.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not positively identify DOSBox Staging.";
            return false;
        }
        if (!HasStagingFamilyMarker(product) && !HasStagingFamilyMarker(description))
        {
            detail = "Executable metadata does not positively identify DOSBox Staging.";
            return false;
        }
        string normalized = DosRuntimeMetadataVersions.Normalize(
            !string.IsNullOrWhiteSpace(evidence.FileVersion) ? evidence.FileVersion : evidence.ProductVersion);
        if (normalized.Equals("unknown", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not contain a usable version.";
            return false;
        }
        version = normalized;
        detail = "Identified from executable metadata.";
        return true;
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
    // R9F V8.6j: retained for interface compatibility only. Windows discovery
    // and Browse NEVER execute dosbox-x.exe -version: the real implementation
    // opens its own interactive console ("Press ENTER key to continue") for
    // that flag, so host identification uses authoritative executable metadata
    // instead (see TryValidateMetadata).
    public IReadOnlyList<string> VersionArguments => ["-version"];

    public bool Identifies(DosRuntimeHostEvidence evidence)
    {
        if (evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase)) return true;
        return (evidence.ProductName ?? string.Empty).Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>R9F V8.6f X positive validation. Filename dosbox-x.exe is only
    /// a hint: Compatible additionally requires a DOSBox-X-specific probe
    /// ("DOSBox-X" + version). Rejects Staging/Classic outputs and
    /// contradictory Classic/Staging metadata. Exit 0 only.
    /// R9F V8.6j: retained for non-Windows or explicitly non-interactive
    /// callers only; Windows discovery and Browse use TryValidateMetadata and
    /// never spawn dosbox-x.exe merely to identify it.</summary>
    public bool IsValidProbe(DosRuntimeHostEvidence evidence, DosRuntimeProbeResult probe)
    {
        string product = evidence.ProductName ?? string.Empty;
        if (product.Contains("Staging", StringComparison.OrdinalIgnoreCase) &&
            !product.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase)) return false;
        if (product.Contains("DOSBox", StringComparison.OrdinalIgnoreCase) &&
            !product.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase) &&
            !product.Contains("Staging", StringComparison.OrdinalIgnoreCase)) return false;
        if (probe is not { Success: true }) return false;
        if (probe.ExitCode != 0) return false;
        string combined = (probe.Output ?? string.Empty) + "\n" + (probe.Error ?? string.Empty);
        if (combined.Contains("staging", StringComparison.OrdinalIgnoreCase)) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(combined, @"DOSBox[\s\-]*X", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) return false;
        if (!System.Text.RegularExpressions.Regex.IsMatch(combined, @"\d+\.\d+")) return false;
        return true;
    }

    public string ParseVersion(string probeOutput)
    {
        foreach (string line in (probeOutput ?? string.Empty).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries))
        {
            string trimmed = line.Trim();
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"DOSBox[\s\-]*X", System.Text.RegularExpressions.RegexOptions.IgnoreCase) &&
                System.Text.RegularExpressions.Regex.IsMatch(trimmed, @"\d+\.\d+"))
                return trimmed.Length > 64 ? trimmed[..64] : trimmed;
        }
        return "unknown";
    }

    public bool SupportsMetadataValidation => true;

    /// <summary>R9F V8.6j non-interactive DOSBox-X identification from
    /// authoritative version-resource metadata. Positive contract: ProductName
    /// contains "DOSBox-X" (e.g. real "DOSBox-X DOS Emulator"), a usable
    /// version exists in FileVersion/ProductVersion, no Staging contradiction
    /// exists, and FileName/OriginalFilename/FileDescription corroborate the
    /// family. Filename alone is never sufficient. Never spawns a process and
    /// never falls back to -version when metadata is insufficient.</summary>
    public bool TryValidateMetadata(DosRuntimeHostEvidence evidence, out string version, out string detail)
    {
        version = string.Empty;
        string product = evidence.ProductName ?? string.Empty;
        if (!product.Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not positively identify DOSBox-X.";
            return false;
        }
        if (product.Contains("Staging", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not positively identify DOSBox-X.";
            return false;
        }
        string rawVersion = !string.IsNullOrWhiteSpace(evidence.FileVersion)
            ? evidence.FileVersion
            : evidence.ProductVersion ?? string.Empty;
        string normalized = DosRuntimeMetadataVersions.Normalize(rawVersion);
        if (normalized.Equals("unknown", StringComparison.OrdinalIgnoreCase))
        {
            detail = "Executable metadata does not contain a usable version.";
            return false;
        }
        bool nameCorroborates =
            evidence.FileName.Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase) ||
            (evidence.OriginalFilename ?? string.Empty).Equals("dosbox-x.exe", StringComparison.OrdinalIgnoreCase) ||
            (evidence.FileDescription ?? string.Empty).Contains("DOSBox-X", StringComparison.OrdinalIgnoreCase);
        if (!nameCorroborates)
        {
            detail = "Executable metadata does not positively identify DOSBox-X.";
            return false;
        }
        version = normalized;
        detail = "Identified from executable metadata (no process probe).";
        return true;
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
