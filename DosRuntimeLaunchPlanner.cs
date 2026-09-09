namespace Pi1ElviraEditor;

/// <summary>R9F V8.6e DOS launch planning. Builds an inspectable launch plan
/// from an already-resolved LaunchReady target plus a probed host candidate.
/// GOG configuration files are READ-ONLY inputs: never edited, overwritten,
/// or copied. Only host CLI tokens (-c style) carry the launch.</summary>
internal static class DosRuntimeLaunchPlanner
{
    /// <summary>GOG-derived default sound switch for both games, taken from
    /// the pristine launcher BATs (ELVIRA.BAT: runvga gamepc /s,
    /// CERBERUS.BAT: runit gamepc /s). Used only when no PI1SND choice exists.</summary>
    internal const string GogDefaultSoundSwitch = "/s";

    internal static DosRuntimeLaunchPlan BuildPlan(DosRuntimeCandidate host, VariantLaunchTarget target, string gameRoot)
    {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(target);
        if (!host.IsRunnable)
            throw new InvalidOperationException("The selected DOS runtime host is not compatible: " + (host.Detail ?? host.DisplayName));
        if (target.Readiness != VariantLaunchReadiness.LaunchReady)
            throw new InvalidOperationException("Variant is not launch-ready.");
        if (target.Ownership != VariantDirectoryOperationStatus.AlreadyValid || string.IsNullOrWhiteSpace(target.WorkingDirectory) ||
            !Directory.Exists(target.WorkingDirectory) || !GameDataFileService.IsDos83FileName(target.ExecutableFile) ||
            !target.ExecutableFile.EndsWith(".EXE", StringComparison.OrdinalIgnoreCase) || !GameDataFileService.IsDos83FileName(target.DataFile))
            throw new InvalidOperationException("Launch target is invalid.");
        string executablePath = SafeChild(target.WorkingDirectory, target.ExecutableFile);
        string dataPath = SafeChild(target.WorkingDirectory, target.DataFile);
        if (!File.Exists(executablePath)) throw new FileNotFoundException("Generated variant executable is missing.", executablePath);
        if (!File.Exists(dataPath)) throw new FileNotFoundException("Generated variant data file is missing.", dataPath);

        IDosRuntimeAdapter adapter = DosRuntimeAdapters.All.FirstOrDefault(item => item.Kind == host.Kind)
            ?? throw new InvalidOperationException("No launch adapter exists for the selected DOS runtime host.");
        IReadOnlyList<string> configs = SelectBaseConfigs(gameRoot, target);
        string sound = ResolveSoundSwitch(gameRoot);
        string dosCommand = Path.GetFileNameWithoutExtension(target.ExecutableFile);
        var request = new DosRuntimeLaunchRequest(target.WorkingDirectory, dosCommand, target.DataFile, sound, configs);
        IReadOnlyList<string> arguments = adapter.BuildArguments(request);
        foreach (string argument in arguments)
        {
            if (string.IsNullOrWhiteSpace(argument) || argument.Any(char.IsControl))
                throw new InvalidDataException("DOS runtime launch argument is invalid.");
        }
        string summary = $"{host.DisplayName} -> {dosCommand} {target.DataFile} [{Path.GetFileName(target.WorkingDirectory)}]";
        return new DosRuntimeLaunchPlan(host.Kind, host.DisplayName, host.ExecutablePath,
            host.Version, target.WorkingDirectory, dosCommand, target.DataFile, configs, arguments, summary);
    }

    /// <summary>Read-only GOG settings base, game-specific. The single.conf
    /// companion is deliberately EXCLUDED: its [autoexec] launches the GOG
    /// menu (mount + ELVIRA.BAT), which would run before our commands.</summary>
    internal static IReadOnlyList<string> SelectBaseConfigs(string? gameRoot, VariantLaunchTarget target)
    {
        if (string.IsNullOrWhiteSpace(gameRoot)) return [];
        string settings = target.GameId.Equals("Elvira2", StringComparison.OrdinalIgnoreCase)
            ? Path.Combine(gameRoot, "dosbox_elvira2.conf")
            : Path.Combine(gameRoot, "dosbox_elvira.conf");
        try
        {
            return File.Exists(settings) ? [settings] : [];
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return [];
        }
    }

    /// <summary>Existing editor sound semantics first (PI1SND.BAT SET
    /// PI1SND=/x, whitelisted shape), GOG BAT default /s otherwise. Never
    /// invented beyond these two derived sources.</summary>
    internal static string ResolveSoundSwitch(string? gameRoot)
    {
        if (!string.IsNullOrWhiteSpace(gameRoot))
        {
            try
            {
                string pi1Snd = Path.Combine(gameRoot, "PI1SND.BAT");
                if (File.Exists(pi1Snd))
                {
                    foreach (string line in File.ReadAllLines(pi1Snd))
                    {
                        string trimmed = line.Trim();
                        if (!trimmed.StartsWith("SET PI1SND=", StringComparison.OrdinalIgnoreCase)) continue;
                        string value = trimmed["SET PI1SND=".Length..].Trim();
                        if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^/[A-Za-z0-9]{1,3}$"))
                            return value;
                    }
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
            }
        }
        return GogDefaultSoundSwitch;
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
}
