using System.Text;

namespace ElviraVgaEditor;

internal sealed record GamePcStringEntry(int Index, int Offset, int ByteLength, byte[] OriginalBytes)
{
    public string Decode(Encoding enc) => enc.GetString(OriginalBytes);
}

internal static class GamePcTextEditor
{
    private const int StringTableStart = 0x12;
    private const int StringTableLastStart = 0x3C00;

    public static List<GamePcStringEntry> LoadEntries(string gamePcPath)
    {
        byte[] data = File.ReadAllBytes(gamePcPath);
        var list = new List<GamePcStringEntry>();

        int pos = StringTableStart;
        int index = 0;

        // A string may START before 0x3C00 and continue past it.
        // Therefore 0x3C00 is only a limit for starting a NEW entry,
        // not a hard truncation boundary for the current string.
        while (pos < data.Length && pos < StringTableLastStart)
        {
            int start = pos;

            // GAMEPC begins with two non-text prefix/metadata bytes at 0x12-0x13.
            // In the Slovak file these decode visually as "=Ó".
            // Preserve them in the file and exclude them from editable text.
            if (index == 0 && pos == StringTableStart)
            {
                int prefixBytes = Math.Min(2, data.Length - pos);
                pos += prefixBytes;
                start = pos;
            }

            // Read the complete NUL-terminated string, even if the last
            // string crosses the 0x3C00 boundary.
            while (pos < data.Length && data[pos] != 0)
                pos++;

            int len = pos - start;
            byte[] bytes = data.AsSpan(start, len).ToArray();
            list.Add(new GamePcStringEntry(index, start, len, bytes));

            index++;

            // If this entry ended at/after 0x3C00, it was the final text entry.
            // Do not interpret following bytecode/data as more strings.
            int terminatorOffset = pos;
            if (terminatorOffset >= StringTableLastStart)
                break;

            // Skip the terminating NUL.
            if (pos < data.Length)
                pos++;
        }

        return list;
    }

    public static void ValidateText(string text, Encoding enc)
    {
        if (text.IndexOf('\0') >= 0)
            throw new InvalidOperationException(UiText.Get("EmbeddedNulError"));

        Encoding strict = Encoding.GetEncoding(enc.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
        try
        {
            byte[] bytes = strict.GetBytes(text);
            string roundTrip = strict.GetString(bytes);
            if (!string.Equals(text, roundTrip, StringComparison.Ordinal))
                throw new InvalidOperationException(UiText.Get("EncodingRoundTripError"));
        }
        catch (EncoderFallbackException)
        {
            throw new InvalidOperationException(UiText.Get("EncodingUnsupportedChar"));
        }
    }

    internal static byte[] BuildEditedData(
        string dataFilePath,
        IReadOnlyDictionary<int, string> edited,
        IReadOnlyList<GamePcStringEntry> entries,
        Encoding enc)
    {
        byte[] data = File.ReadAllBytes(dataFilePath);

        foreach (var kvp in edited)
        {
            var entry = entries.FirstOrDefault(e => e.Index == kvp.Key)
                ?? throw new InvalidOperationException($"Unknown string index {kvp.Key}.");

            ValidateText(kvp.Value, enc);
            byte[] newBytes = enc.GetBytes(kvp.Value);
            if (newBytes.Length > entry.ByteLength)
            {
                throw new InvalidOperationException(
                    $"{UiText.Get("TooLong")} Index {entry.Index}: {newBytes.Length}>{entry.ByteLength}");
            }

            Array.Copy(newBytes, 0, data, entry.Offset, newBytes.Length);

            // Clear unused bytes inside the original slot.
            for (int i = newBytes.Length; i < entry.ByteLength; i++)
                data[entry.Offset + i] = 0;

            // Keep the original NUL terminator position intact.
            int terminator = entry.Offset + entry.ByteLength;
            if (terminator < data.Length)
                data[terminator] = 0;
        }

        return data;
    }

    public static void SaveInPlace(
        string dataFilePath,
        IReadOnlyDictionary<int, string> edited,
        IReadOnlyList<GamePcStringEntry> entries,
        Encoding enc)
    {
        byte[] data = BuildEditedData(dataFilePath, edited, entries, enc);
        GameDataFileService.WriteCurrent(dataFilePath, data, entries.Count);
    }

    internal static void ValidateSerializedData(string temporaryPath, int expectedEntryCount)
    {
        var verify = LoadEntries(temporaryPath);
        if (verify.Count != expectedEntryCount)
            throw new InvalidOperationException(string.Format(UiText.Get("StringCountChanged"), expectedEntryCount, verify.Count));
    }

    public static Encoding GetEncoding(string name)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        return name switch
        {
            "CP852" => Encoding.GetEncoding(852),
            "Windows-1250" => Encoding.GetEncoding(1250),
            "Latin1/Raw" => Encoding.Latin1,
            _ => Encoding.GetEncoding(852)
        };
    }
}

/// <summary>
/// Target-aware persistence for compatible Elvira data files. Only the literal original
/// GAMEPC participates in the legacy GAMEPCO immutable-original rule; named variants are
/// working files and use transactional replacement without inventing a second backup format.
/// </summary>
internal static class GameDataFileService
{
    public static bool IsDos83FileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) || Path.GetFileName(fileName) != fileName) return false;
        string[] parts = fileName.Split('.');
        if (parts.Length > 2 || parts[0].Length is < 1 or > 8 || (parts.Length == 2 && parts[1].Length is < 1 or > 3)) return false;
        return parts.All(part => part.All(c => char.IsLetterOrDigit(c) || "!#$%&'()-@^_`{}~".Contains(c)));
    }

    public static string SaveAsNew(string sourcePath, string destinationPath, IReadOnlyDictionary<int, string> edits, IReadOnlyList<GamePcStringEntry> entries, Encoding enc)
    {
        ValidateNewVariantDestination(sourcePath, destinationPath);
        byte[] data = GamePcTextEditor.BuildEditedData(sourcePath, edits, entries, enc);
        WriteNew(destinationPath, data, entries.Count);
        return Path.GetFullPath(destinationPath);
    }

    public static void WriteCurrent(string dataFilePath, byte[] data, int expectedEntryCount)
    {
        if (!File.Exists(dataFilePath)) throw new FileNotFoundException("Current data file is missing.", dataFilePath);
        string temp = TemporaryPath(dataFilePath, "save");
        try
        {
            File.WriteAllBytes(temp, data);
            GamePcTextEditor.ValidateSerializedData(temp, expectedEntryCount);
            if (Path.GetFileName(dataFilePath).Equals("GAMEPC", StringComparison.OrdinalIgnoreCase))
                SafeDeployer.ReplaceActiveWithPrepared(dataFilePath, temp);
            else
                ReplaceWorkingVariant(dataFilePath, temp);
        }
        finally
        {
            try { if (File.Exists(temp)) File.Delete(temp); } catch { }
        }
    }

    private static void ValidateNewVariantDestination(string sourcePath, string destinationPath)
    {
        string sourceDirectory = Path.GetDirectoryName(Path.GetFullPath(sourcePath)) ?? throw new IOException("Source directory is unavailable.");
        string destinationDirectory = Path.GetDirectoryName(Path.GetFullPath(destinationPath)) ?? throw new IOException("Destination directory is unavailable.");
        if (!sourceDirectory.Equals(destinationDirectory, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Save As and Create Variant must stay in the current data-file directory.");
        string name = Path.GetFileName(destinationPath);
        if (!IsDos83FileName(name))
            throw new InvalidOperationException($"'{name}' is not a DOS-compatible 8.3 filename.");
        if (File.Exists(destinationPath))
            throw new IOException($"The variant already exists and will not be overwritten: {destinationPath}");
    }

    private static void WriteNew(string destinationPath, byte[] data, int expectedEntryCount)
    {
        string temp = TemporaryPath(destinationPath, "new");
        try
        {
            File.WriteAllBytes(temp, data);
            GamePcTextEditor.ValidateSerializedData(temp, expectedEntryCount);
            File.Move(temp, destinationPath);
        }
        finally { try { if (File.Exists(temp)) File.Delete(temp); } catch { } }
    }

    private static void ReplaceWorkingVariant(string activePath, string preparedPath)
    {
        string rollback = TemporaryPath(activePath, "rollback");
        bool moved = false;
        try
        {
            File.Move(activePath, rollback);
            moved = true;
            File.Move(preparedPath, activePath);
            File.Delete(rollback);
        }
        catch
        {
            if (!File.Exists(activePath) && moved && File.Exists(rollback)) File.Move(rollback, activePath);
            throw;
        }
        finally { try { if (File.Exists(rollback)) File.Delete(rollback); } catch { } }
    }

    private static string TemporaryPath(string targetPath, string kind) => targetPath + ".pi1_" + kind + "_" + Guid.NewGuid().ToString("N") + ".tmp";
}
