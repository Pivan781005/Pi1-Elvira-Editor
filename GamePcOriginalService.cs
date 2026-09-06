using System.Security.Cryptography;

namespace ElviraVgaEditor;

internal enum GamePcOriginalStatus
{
    VerifiedOriginal,
    BaselineCopy
}

internal sealed record GamePcOriginalReference(string Path, GamePcOriginalStatus Status, IReadOnlyList<GamePcStringEntry> Entries);

/// <summary>Read-only access to the frozen GAMEPC baseline already protected by <see cref="ProjectContext"/>.</summary>
internal static class GamePcOriginalService
{
    internal const string OriginalFileName = "GAMEPCO";
    private const string Elvira1OriginalHash = "C0A1B2690499F51402605E5966593895CA9937C59F95C1A35BE026E57E30A663";
    private const string Elvira2OriginalHash = "484C6C25E4E883E31FC129491D7681BB2330C29C63F10CCBA15EA9F7A4E3BA78";

    /// <summary>Recognizes only the two frozen historical root-side GAMEPCO
    /// copies.  This supports migration-era inventory compatibility without
    /// allowing an arbitrary file named GAMEPCO to bypass baseline validation.</summary>
    internal static bool IsVerifiedLegacyRootOriginal(string path)
    {
        if (!Path.GetFileName(path).Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase) || !File.Exists(path))
            return false;
        string hash;
        try { using FileStream stream = File.OpenRead(path); hash = Convert.ToHexString(SHA256.HashData(stream)); }
        catch { return false; }
        return hash.Equals(Elvira1OriginalHash, StringComparison.OrdinalIgnoreCase) ||
            hash.Equals(Elvira2OriginalHash, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Resolves the frozen GAMEPC already protected by <see cref="ProjectContext"/>.
    /// This normal project workflow is strictly read-only and never creates a
    /// root-side GAMEPCO file. The retired GAMEPCO migration helper was removed
    /// in R10; historical root-side GAMEPCO copies are recognized read-only by
    /// <see cref="IsVerifiedLegacyRootOriginal"/> for inventory compatibility.
    /// </summary>
    public static GamePcOriginalReference LoadProjectBaseline(ProjectContext project)
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
        return new GamePcOriginalReference(path, verified ? GamePcOriginalStatus.VerifiedOriginal : GamePcOriginalStatus.BaselineCopy, entries);
    }

    public static void RejectProtectedPath(string path)
    {
        if (Path.GetFileName(path).Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("GAMEPCO is an immutable original reference and cannot be written.");
    }
}
