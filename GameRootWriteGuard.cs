namespace Pi1ElviraEditor;

/// <summary>Authoritative destination policy for protected GameRoot content.
/// Original game assets are immutable: normal editor operations must never
/// write, replace, delete, rename, or move them. Only the bounded launcher
/// BAT architecture (which never routes through this guard) may mutate an
/// existing original file. Editor-owned ElviraEditor/ and VARIANTS/ locations
/// plus explicit external exports remain writable.</summary>
internal static class GameRootWriteGuard
{
    /// <summary>Basenames that must never be written through guarded
    /// game-format APIs, in any directory: replacing them anywhere risks
    /// shadowing pristine assets or reintroducing GameRoot mutation through
    /// a future caller.</summary>
    public static bool IsProtectedFileName(string path)
    {
        string name = Path.GetFileName(path);
        if (name.Equals("GAMEPC", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("GAMEPCO", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("RUNVGA.EXE", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("RUNVGAO.EXE", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("RUNEGA.EXE", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("RUNEGAO.EXE", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("RUNIT.EXE", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("RUNITO.EXE", StringComparison.OrdinalIgnoreCase))
            return true;
        if (name.EndsWith(".VGA", StringComparison.OrdinalIgnoreCase))
            return true;
        if (name.EndsWith(".O", StringComparison.OrdinalIgnoreCase))
            return true;
        return false;
    }

    public static void RejectProtectedFileName(string path)
    {
        if (IsProtectedFileName(path))
            throw new InvalidOperationException($"'{Path.GetFileName(path)}' is a protected original game asset name and cannot be written. Save project content and use Build Variant instead. No files were changed.");
    }

    /// <summary>Canonical directory-boundary comparison. Never matches
    /// C:\Game2 as a child of C:\Game.</summary>
    public static bool IsWithinDirectory(string rootDirectory, string path)
    {
        string root = Path.GetFullPath(rootDirectory).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            + Path.DirectorySeparatorChar;
        string full = Path.GetFullPath(path);
        return full.StartsWith(root, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsOwnedEditorLocation(string gameRoot, string path)
    {
        string fullRoot = Path.GetFullPath(gameRoot);
        foreach (string owned in new[] { "ElviraEditor", "VARIANTS" })
        {
            string ownedRoot = Path.Combine(fullRoot, owned);
            if (Path.GetFullPath(path).Equals(ownedRoot, StringComparison.OrdinalIgnoreCase) || IsWithinDirectory(ownedRoot, path))
                return true;
        }
        return false;
    }

    /// <summary>Denies modified game-binary/data output inside the game
    /// installation. External exports and owned editor locations stay
    /// allowed. A null/unknown game root skips the check (callers that know
    /// the active installation must pass it).</summary>
    public static void RejectGameFormatOutputInGameRoot(string? gameRoot, string destinationPath)
    {
        if (string.IsNullOrWhiteSpace(gameRoot))
            return;
        string fullRoot;
        string fullDestination;
        try
        {
            fullRoot = Path.GetFullPath(gameRoot);
            fullDestination = Path.GetFullPath(destinationPath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            throw new InvalidOperationException("Game output destination is not a valid path. No files were changed.");
        }
        if (!IsWithinDirectory(fullRoot, fullDestination))
            return;
        if (IsOwnedEditorLocation(fullRoot, fullDestination))
            return;
        throw new InvalidOperationException($"Modified game content cannot be written inside the game installation ({fullDestination}). Save project content and use Build Variant into owned VARIANTS output. No files were changed.");
    }

    /// <summary>Denies overwriting an existing original file inside the game
    /// installation (used by user-selected export dialogs whose filters could
    /// otherwise be pointed at a protected name).</summary>
    public static void RejectProtectedExistingOriginal(string? gameRoot, string destinationPath)
    {
        if (string.IsNullOrWhiteSpace(gameRoot))
            return;
        string fullDestination;
        try { fullDestination = Path.GetFullPath(destinationPath); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            throw new InvalidOperationException("Export destination is not a valid path. No files were changed.");
        }
        if (!IsWithinDirectory(gameRoot, fullDestination) || IsOwnedEditorLocation(gameRoot, fullDestination))
            return;
        if (File.Exists(fullDestination) || IsProtectedFileName(fullDestination))
            throw new InvalidOperationException($"Export cannot overwrite protected game installation content ({fullDestination}). Choose a path outside the game installation. No files were changed.");
    }
}
