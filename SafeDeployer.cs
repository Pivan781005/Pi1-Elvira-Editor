namespace ElviraVgaEditor;

internal static class SafeDeployer
{
    public static string OriginalBackupPath(string vgaPath) => vgaPath + ".bak_original";
    public static string PreviousBackupPath(string vgaPath) => vgaPath + ".bak_previous";

    public static void Deploy(string sourceVga, IReadOnlyDictionary<int, string> edits)
    {
        string originalBackup = OriginalBackupPath(sourceVga);
        string previousBackup = PreviousBackupPath(sourceVga);
        string tempPatch = sourceVga + ".tmp_patch";

        if (!File.Exists(originalBackup))
            File.Copy(sourceVga, originalBackup, overwrite: false);

        if (File.Exists(tempPatch))
            File.Delete(tempPatch);

        new VgaFileRebuilder().Rebuild(sourceVga, edits, tempPatch);

        // Basic sanity parse before swap.
        _ = new VgaImageTableParser(File.ReadAllBytes(tempPatch)).Parse();

        if (File.Exists(previousBackup))
            File.Delete(previousBackup);

        bool sourceMoved = false;

        try
        {
            File.Move(sourceVga, previousBackup);
            sourceMoved = true;
            File.Move(tempPatch, sourceVga);
        }
        catch
        {
            if (File.Exists(sourceVga))
                File.Delete(sourceVga);

            if (sourceMoved && File.Exists(previousBackup))
                File.Move(previousBackup, sourceVga);

            if (File.Exists(tempPatch))
                File.Delete(tempPatch);

            throw;
        }
    }

    public static void RestoreOriginal(string sourceVga)
    {
        string originalBackup = OriginalBackupPath(sourceVga);
        string previousBackup = PreviousBackupPath(sourceVga);

        if (!File.Exists(originalBackup))
            throw new FileNotFoundException("Originálna záloha neexistuje.", originalBackup);

        if (File.Exists(previousBackup))
            File.Delete(previousBackup);

        if (File.Exists(sourceVga))
            File.Move(sourceVga, previousBackup);

        File.Copy(originalBackup, sourceVga, overwrite: true);
    }
}
