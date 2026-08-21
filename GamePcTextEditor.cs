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

    public static void SaveInPlace(
        string gamePcPath,
        IReadOnlyDictionary<int, string> edited,
        IReadOnlyList<GamePcStringEntry> entries,
        Encoding enc)
    {
        byte[] data = File.ReadAllBytes(gamePcPath);

        foreach (var kvp in edited)
        {
            var entry = entries.FirstOrDefault(e => e.Index == kvp.Key)
                ?? throw new InvalidOperationException($"Unknown string index {kvp.Key}.");

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

        Backup(gamePcPath);
        File.WriteAllBytes(gamePcPath, data);
    }

    private static void Backup(string path)
    {
        string original = path + ".bak_original";
        string previous = path + ".bak_previous";

        if (!File.Exists(original))
            File.Copy(path, original, overwrite: false);

        File.Copy(path, previous, overwrite: true);
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
