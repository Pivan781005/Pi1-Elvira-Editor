using System.Security.Cryptography;

namespace Pi1ElviraEditor;

internal enum GamePcBaselineStatus
{
    VerifiedOriginal,
    BaselineCopy
}

internal sealed record GamePcBaselineReference(string Path, GamePcBaselineStatus Status, IReadOnlyList<GamePcStringEntry> Entries);

/// <summary>Read-only access to the frozen GAMEPC baseline already protected by <see cref="ProjectContext"/>.</summary>
internal static class GamePcBaselineService
{
    private const string Elvira1OriginalHash = "C0A1B2690499F51402605E5966593895CA9937C59F95C1A35BE026E57E30A663";
    private const string Elvira2OriginalHash = "484C6C25E4E883E31FC129491D7681BB2330C29C63F10CCBA15EA9F7A4E3BA78";

    /// <summary>
    /// Resolves the frozen GAMEPC baseline already protected by <see cref="ProjectContext"/>.
    /// This normal project workflow is strictly read-only.
    /// </summary>
    public static GamePcBaselineReference LoadProjectBaseline(ProjectContext project)
    {
        ArgumentNullException.ThrowIfNull(project);
        string path = Path.Combine(project.GameRoot, "GAMEPC");
        IReadOnlyList<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(path, project.GameProfile);
        string hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
        bool verified = project.GameProfile switch
        {
            ElviraGameProfile.Elvira1 => hash.Equals(Elvira1OriginalHash, StringComparison.OrdinalIgnoreCase),
            ElviraGameProfile.Elvira2 => hash.Equals(Elvira2OriginalHash, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
        return new GamePcBaselineReference(path, verified ? GamePcBaselineStatus.VerifiedOriginal : GamePcBaselineStatus.BaselineCopy, entries);
    }
}
