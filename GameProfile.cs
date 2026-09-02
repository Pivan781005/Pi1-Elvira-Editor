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
            "Elvira: Mistress of the Dark",
            null,
            "No fixed Elvira I runtime text-length limit is currently proven; byte counts are informational."),
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
            "Detect from supported executable signatures and resource characteristics; manual override is always available.")
    };
}

internal static class GameProfileDetector
{
    public static ElviraGameProfile Detect(string directory, int zoneCount, int? gamePcStringCount)
    {
        if (GameInstallationValidator.TryValidate(directory, InstallationDiscoverySource.Manual, out GameInstallation? installation) && installation is not null)
            return installation.Game;

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

}
