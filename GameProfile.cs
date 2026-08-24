namespace ElviraVgaEditor;

internal enum ElviraGameProfile
{
    AutoDetect,
    Elvira1,
    Elvira2,
    Unknown
}

internal sealed record GameProfileInfo(
    ElviraGameProfile Profile,
    string DisplayName,
    int? InteractiveDialogueByteLimit,
    string Notes)
{
    public static GameProfileInfo For(ElviraGameProfile profile) => profile switch
    {
        ElviraGameProfile.Elvira1 => new(
            ElviraGameProfile.Elvira1,
            "Elvira I: Mistress of the Dark",
            96,
            "Original DOS interactive NPC dialogue path has an observed 96-byte CP852 word-aware limit. This is not a global GAMEPC limit."),
        ElviraGameProfile.Elvira2 => new(
            ElviraGameProfile.Elvira2,
            "Elvira II: The Jaws of Cerberus",
            null,
            "Elvira II text limits are not assumed to match Elvira I. Validation stays informational until proven."),
        ElviraGameProfile.Unknown => new(
            ElviraGameProfile.Unknown,
            "Unknown / unsupported variant",
            null,
            "Game profile could not be determined reliably."),
        _ => new(
            ElviraGameProfile.AutoDetect,
            "Auto-detect",
            null,
            "Detect from folder name and resource characteristics; manual override is always available.")
    };
}

internal static class GameProfileDetector
{
    public static ElviraGameProfile Detect(string directory, int zoneCount, int? gamePcStringCount)
    {
        string name = Path.GetFileName(directory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        string full = directory;

        if (ContainsElvira2(name) || ContainsElvira2(full))
            return ElviraGameProfile.Elvira2;

        if (ContainsElvira1(name) || ContainsElvira1(full))
            return ElviraGameProfile.Elvira1;

        // Secondary heuristics derived from the currently tested GOG installations.
        // They are intentionally conservative; manual override remains available.
        if (zoneCount >= 85)
            return ElviraGameProfile.Elvira2;

        if (zoneCount > 0 && zoneCount <= 75)
            return ElviraGameProfile.Elvira1;

        if (gamePcStringCount is >= 1000 and <= 1100)
            return ElviraGameProfile.Elvira2;

        if (gamePcStringCount is >= 650 and <= 710)
            return ElviraGameProfile.Elvira1;

        return ElviraGameProfile.Unknown;
    }

    private static bool ContainsElvira2(string value)
    {
        string s = value.ToLowerInvariant();
        return s.Contains("elvira ii") || s.Contains("elvira 2") || s.Contains("elvira2") || s.Contains("jaws of cerberus");
    }

    private static bool ContainsElvira1(string value)
    {
        string s = value.ToLowerInvariant();
        if (!s.Contains("elvira"))
            return false;
        return !ContainsElvira2(value);
    }
}
