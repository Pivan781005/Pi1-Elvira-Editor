using System.Security.Cryptography;

namespace ElviraVgaEditor;

internal enum GamePcOriginalStatus
{
    VerifiedOriginal,
    BaselineCopy
}

internal sealed record GamePcOriginalReference(string Path, GamePcOriginalStatus Status, IReadOnlyList<GamePcStringEntry> Entries);

/// <summary>Creates GAMEPCO once from a strictly valid active GAMEPC and never rewrites it.</summary>
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
    /// Unlike the retired migration helper below, this normal project workflow is
    /// strictly read-only and never creates a root-side GAMEPCO file.
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

    [Obsolete("Legacy migration helper only. Project workflows must use LoadProjectBaseline.")]
    public static GamePcOriginalReference Ensure(string installationDirectory, ElviraGameProfile profile)
    {
        string directory = Path.GetFullPath(installationDirectory);
        string active = Path.Combine(directory, "GAMEPC");
        string original = Path.Combine(directory, OriginalFileName);
        if (!File.Exists(original))
        {
            // Strict parsing must succeed before the immutable baseline is made.
            _ = GamePcTextEditor.LoadEntries(active, profile);
            string temporary = original + ".pi1_original_" + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                File.Copy(active, temporary, overwrite: false);
                _ = GamePcTextEditor.LoadEntries(temporary, profile);
                File.Move(temporary, original, overwrite: false);
            }
            finally { try { if (File.Exists(temporary)) File.Delete(temporary); } catch { } }
        }

        IReadOnlyList<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(original, profile);
        string hash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(original)));
        bool verified = profile switch
        {
            ElviraGameProfile.Elvira1 => hash.Equals(Elvira1OriginalHash, StringComparison.OrdinalIgnoreCase),
            ElviraGameProfile.Elvira2 => hash.Equals(Elvira2OriginalHash, StringComparison.OrdinalIgnoreCase),
            _ => false
        };
        return new GamePcOriginalReference(original, verified ? GamePcOriginalStatus.VerifiedOriginal : GamePcOriginalStatus.BaselineCopy, entries);
    }

    public static void RejectProtectedPath(string path)
    {
        if (Path.GetFileName(path).Equals(OriginalFileName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("GAMEPCO is an immutable original reference and cannot be written.");
    }
}
