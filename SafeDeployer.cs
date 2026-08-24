namespace ElviraVgaEditor;

/// <summary>Shared immutable-original/temporary-active replacement primitives for GAMEPC and VGA resources.</summary>
internal static class SafeDeployer
{
    /// <summary>GAMEPC -> GAMEPCO; 012.VGA -> 012O.VGA. Existing O-files are never overwritten.</summary>
    public static string OriginalBackupPath(string activePath)
    {
        string directory = Path.GetDirectoryName(Path.GetFullPath(activePath)) ?? ".";
        string name = Path.GetFileName(activePath);
        if (name.Equals("GAMEPC", StringComparison.OrdinalIgnoreCase)) return Path.Combine(directory, "GAMEPCO");
        string extension = Path.GetExtension(name);
        if (extension.Equals(".VGA", StringComparison.OrdinalIgnoreCase))
            return Path.Combine(directory, Path.GetFileNameWithoutExtension(name) + "O" + extension);
        throw new InvalidOperationException("Immutable O-file naming is defined only for GAMEPC and VGA resources.");
    }

    /// <summary>Rebuilds, parses, and transactionally installs a VGA file. Merely reading a VGA never calls this method.</summary>
    public static void Deploy(string sourceVga, IReadOnlyDictionary<int, string> edits)
    {
        if (!File.Exists(sourceVga)) throw new FileNotFoundException("Active VGA file is missing.", sourceVga);
        _ = new VgaImageTableParser(File.ReadAllBytes(sourceVga)).Parse();
        string tempPatch = TemporaryPath(sourceVga, "vga");
        try
        {
            new VgaFileRebuilder().Rebuild(sourceVga, edits, tempPatch);
            _ = new VgaImageTableParser(File.ReadAllBytes(tempPatch)).Parse();
            ReplaceActiveWithPrepared(sourceVga, tempPatch);
        }
        finally { DeleteIfExists(tempPatch); }
    }

    /// <summary>Installs a validated temporary GAMEPC/VGA file while preserving the first active version as an O-file.</summary>
    public static void ReplaceActiveWithPrepared(string activePath, string preparedPath)
    {
        if (!File.Exists(activePath)) throw new FileNotFoundException("Active file is missing.", activePath);
        if (!File.Exists(preparedPath)) throw new FileNotFoundException("Prepared output is missing.", preparedPath);
        string original = OriginalBackupPath(activePath);
        if (File.Exists(original) && new FileInfo(original).Length == 0)
            throw new InvalidDataException($"Cannot update {Path.GetFileName(activePath)}: its immutable original backup {Path.GetFileName(original)} is empty and unsafe. No files were changed.");
        string rollback = TemporaryPath(activePath, "rollback");
        bool moved = false;
        try
        {
            // Prepared output was validated by its caller; capture the original only after that validation.
            if (!File.Exists(original)) File.Copy(activePath, original, overwrite: false);
            File.Move(activePath, rollback);
            moved = true;
            File.Move(preparedPath, activePath);
            DeleteIfExists(rollback);
        }
        catch
        {
            if (!File.Exists(activePath) && moved && File.Exists(rollback)) File.Move(rollback, activePath);
            throw;
        }
        finally { DeleteIfExists(rollback); }
    }

    /// <summary>Future-compatible restore: copies immutable O bytes back without deleting or changing the O-file.</summary>
    public static void RestoreOriginal(string activePath)
    {
        string original = OriginalBackupPath(activePath);
        if (!File.Exists(original)) throw new FileNotFoundException("Original O-file backup was not found.", original);
        string prepared = TemporaryPath(activePath, "restore");
        try { File.Copy(original, prepared, true); ReplaceActiveWithPrepared(activePath, prepared); }
        finally { DeleteIfExists(prepared); }
    }

    private static string TemporaryPath(string activePath, string kind) => activePath + ".pi1_" + kind + "_" + Guid.NewGuid().ToString("N") + ".tmp";
    private static void DeleteIfExists(string path) { if (File.Exists(path)) File.Delete(path); }
}
