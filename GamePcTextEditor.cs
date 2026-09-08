using System.Buffers.Binary;
using System.Text;

namespace Pi1ElviraEditor;

internal sealed record GamePcStringEntry(int Index, int Offset, int ByteLength, byte[] OriginalBytes)
{
    public string Decode(Encoding enc) => enc.GetString(OriginalBytes);
}

internal static class GamePcTextEditor
{
    // Original Elvira I and Elvira II GAMEPC: a 20-byte header followed by a
    // counted NUL text block. The contract is independently verified against
    // the original files for each game; no logical table is derived from a
    // physical fixed cutoff.
    private const int HeaderSize = 0x14;
    private const int StringCountOffset = 0x0C;
    private const int TextBlockLengthOffset = 0x10;
    private const int MaximumStringCount = 100_000;

    // Unknown files retain the historical compatibility parser. Recognized
    // Elvira I and Elvira II installations always use the verified header
    // contract above.
    private const int LegacyStringTableStart = 0x12;
    private const int LegacyStringTableLastStart = 0x3C00;

    internal static byte? GetTerminalDelimiter(GamePcStringEntry baseEntry)
    {
        if (baseEntry.OriginalBytes.Length == 0) return null;
        byte value = baseEntry.OriginalBytes[^1];
        return value == 0x20 || value < 0x20 ? value : null;
    }

    /// <summary>
    /// Immutable GAMEPCO metadata for the legacy two-choice mouse menu resource.
    /// This deliberately recognizes the original semantic resource (ASCII YES/NO),
    /// not a physical string index: its two token start columns are the engine's
    /// fixed hitbox columns and must survive localization.
    /// </summary>
    internal static bool TryGetFixedHotspotMenuLayout(GamePcStringEntry baseEntry, out int firstStartColumn, out int secondStartColumn)
    {
        firstStartColumn = secondStartColumn = 0;
        ReadOnlySpan<byte> bytes = baseEntry.OriginalBytes;
        int position = 0;
        while (position < bytes.Length && bytes[position] == 0x20) position++;
        firstStartColumn = position;
        int firstStart = position;
        while (position < bytes.Length && IsAsciiUpper(bytes[position])) position++;
        ReadOnlySpan<byte> first = bytes[firstStart..position];
        int gapStart = position;
        while (position < bytes.Length && bytes[position] == 0x20) position++;
        if (position == gapStart) return false;
        secondStartColumn = position;
        int secondStart = position;
        while (position < bytes.Length && IsAsciiUpper(bytes[position])) position++;
        ReadOnlySpan<byte> second = bytes[secondStart..position];
        while (position < bytes.Length && bytes[position] == 0x20) position++;

        return position == bytes.Length && first.SequenceEqual("YES"u8) && second.SequenceEqual("NO"u8);
    }

    private static bool IsAsciiUpper(byte value) => value is >= (byte)'A' and <= (byte)'Z';

    private static bool TryReadTwoVisibleLabels(ReadOnlySpan<byte> bytes, out ReadOnlySpan<byte> first, out ReadOnlySpan<byte> second)
    {
        first = second = default;
        int position = 0;
        while (position < bytes.Length && bytes[position] == 0x20) position++;
        int firstStart = position;
        while (position < bytes.Length && bytes[position] > 0x20) position++;
        if (position == firstStart) return false;
        first = bytes[firstStart..position];
        while (position < bytes.Length && bytes[position] == 0x20) position++;
        int secondStart = position;
        while (position < bytes.Length && bytes[position] > 0x20) position++;
        if (position == secondStart) return false;
        second = bytes[secondStart..position];
        while (position < bytes.Length && bytes[position] == 0x20) position++;
        return position == bytes.Length;
    }

    private static byte[] ApplyFixedHotspotMenuLayout(GamePcStringEntry baseEntry, byte[] bytes)
    {
        if (!TryGetFixedHotspotMenuLayout(baseEntry, out int firstStart, out int secondStart) ||
            !TryReadTwoVisibleLabels(bytes, out ReadOnlySpan<byte> first, out ReadOnlySpan<byte> second))
            return bytes;

        int gap = secondStart - (firstStart + first.Length);
        if (gap < 1)
            throw new InvalidOperationException($"String index {baseEntry.Index}: first fixed-hotspot label is too wide for the original second-label column.");

        byte[] aligned = new byte[firstStart + first.Length + gap + second.Length];
        first.CopyTo(aligned.AsSpan(firstStart));
        second.CopyTo(aligned.AsSpan(secondStart));
        aligned.AsSpan(0, firstStart).Fill(0x20);
        aligned.AsSpan(firstStart + first.Length, gap).Fill(0x20);
        return aligned;
    }

    internal static string ToEditableText(GamePcStringEntry entry, GamePcStringEntry baseEntry, Encoding enc, ElviraGameProfile profile)
    {
        string text = entry.Decode(enc);
        byte? delimiter = profile == ElviraGameProfile.Elvira1 ? GetTerminalDelimiter(baseEntry) : null;
        return delimiter.HasValue && entry.OriginalBytes.Length > 0 && entry.OriginalBytes[^1] == delimiter.Value
            ? text[..^1]
            : text;
    }

    public static List<GamePcStringEntry> LoadEntries(string gamePcPath, ElviraGameProfile profile = ElviraGameProfile.Unknown)
    {
        byte[] data = File.ReadAllBytes(gamePcPath);
        return profile switch
        {
            ElviraGameProfile.Elvira1 => LoadHeaderDefinedEntries(data, "Elvira I"),
            ElviraGameProfile.Elvira2 => LoadHeaderDefinedEntries(data, "Elvira II"),
            _ => LoadLegacyEntries(data)
        };
    }

    private static List<GamePcStringEntry> LoadHeaderDefinedEntries(byte[] data, string gameName)
    {
        if (data.Length < HeaderSize)
            throw new InvalidDataException($"{gameName} GAMEPC is shorter than the 0x{HeaderSize:X} byte header.");

        int count = ReadHeaderUInt32(data, StringCountOffset, "string count", gameName);
        int textBlockLength = ReadHeaderUInt32(data, TextBlockLengthOffset, "text-block length", gameName);
        if (count > MaximumStringCount)
            throw new InvalidDataException($"{gameName} GAMEPC string count {count} exceeds the supported safety limit.");
        if (count > textBlockLength)
            throw new InvalidDataException($"{gameName} GAMEPC string count cannot exceed the NUL-terminated text-block length.");

        int textEndExclusive;
        try
        {
            textEndExclusive = checked(HeaderSize + textBlockLength);
        }
        catch (OverflowException ex)
        {
            throw new InvalidDataException($"{gameName} GAMEPC text-block end overflows the supported file range.", ex);
        }
        if (textEndExclusive > data.Length)
            throw new InvalidDataException($"{gameName} GAMEPC text block ends at 0x{textEndExclusive:X}, beyond EOF 0x{data.Length:X}.");

        var list = new List<GamePcStringEntry>();
        int pos = HeaderSize;
        for (int index = 0; index < count; index++)
        {
            if (pos >= textEndExclusive)
                throw new InvalidDataException($"{gameName} GAMEPC text block ended before header-counted string index {index}.");

            int start = pos;
            while (pos < textEndExclusive && data[pos] != 0)
                pos++;
            if (pos >= textEndExclusive)
                throw new InvalidDataException($"{gameName} GAMEPC string index {index} has no NUL terminator inside the declared text block.");

            int len = pos - start;
            list.Add(new GamePcStringEntry(index, start, len, data.AsSpan(start, len).ToArray()));
            pos++; // NUL terminator; zero-length entries are intentional logical slots.
        }

        if (pos != textEndExclusive)
            throw new InvalidDataException($"{gameName} GAMEPC parsed text ends at 0x{pos:X}, but the header declares 0x{textEndExclusive:X}.");

        return list;
    }

    private static int ReadHeaderUInt32(byte[] data, int offset, string field, string gameName)
    {
        uint value = BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(offset, sizeof(uint)));
        if (value > int.MaxValue)
            throw new InvalidDataException($"{gameName} GAMEPC {field} exceeds the supported file range.");
        return (int)value;
    }

    private static List<GamePcStringEntry> LoadLegacyEntries(byte[] data)
    {
        var list = new List<GamePcStringEntry>();

        int pos = LegacyStringTableStart;
        int index = 0;

        while (pos < data.Length && pos < LegacyStringTableLastStart)
        {
            int start = pos;

            if (index == 0 && pos == LegacyStringTableStart)
            {
                int prefixBytes = Math.Min(2, data.Length - pos);
                pos += prefixBytes;
                start = pos;
            }

            while (pos < data.Length && data[pos] != 0)
                pos++;

            int len = pos - start;
            byte[] bytes = data.AsSpan(start, len).ToArray();
            list.Add(new GamePcStringEntry(index, start, len, bytes));

            index++;

            int terminatorOffset = pos;
            if (terminatorOffset >= LegacyStringTableLastStart)
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

    /// <summary>
    /// Produces a complete, validated GAMEPC image. Recognized Elvira profiles
    /// use the header-counted text-pool repacker; unknown legacy files retain
    /// the historical fixed-slot compatibility behavior.
    /// </summary>
    internal static byte[] BuildEditedData(
        string dataFilePath,
        IReadOnlyDictionary<int, string> edited,
        IReadOnlyList<GamePcStringEntry> entries,
        Encoding enc,
        ElviraGameProfile profile = ElviraGameProfile.Unknown,
        IReadOnlyList<GamePcStringEntry>? baseEntries = null)
    {
        byte[] data = File.ReadAllBytes(dataFilePath);
        return profile switch
        {
            ElviraGameProfile.Elvira1 => RepackHeaderDefined(data, edited, entries, enc, "Elvira I", baseEntries),
            ElviraGameProfile.Elvira2 => RepackHeaderDefined(data, edited, entries, enc, "Elvira II"),
            _ => BuildLegacyFixedSlotData(data, edited, entries, enc)
        };
    }

    /// <summary>
    /// Pure shared E1/E2 GAMEPC repacker. It retains the source header template
    /// and post-text suffix from the current data file, rebuilds exactly the
    /// header-counted NUL-delimited logical pool, then strict-parses its result.
    /// </summary>
    internal static byte[] RepackHeaderDefined(
        byte[] source,
        IReadOnlyDictionary<int, string> edited,
        IReadOnlyList<GamePcStringEntry> entries,
        Encoding enc,
        string gameName,
        IReadOnlyList<GamePcStringEntry>? baseEntries = null)
    {
        List<GamePcStringEntry> sourceEntries = LoadHeaderDefinedEntries(source, gameName);
        if (entries.Count != sourceEntries.Count || !entries.Select(e => e.Index).SequenceEqual(sourceEntries.Select(e => e.Index)))
            throw new InvalidDataException($"{gameName} GAMEPC logical entry sequence does not match the current source file.");
        if (baseEntries is not null &&
            (baseEntries.Count != sourceEntries.Count || !baseEntries.Select(e => e.Index).SequenceEqual(sourceEntries.Select(e => e.Index))))
            throw new InvalidDataException($"{gameName} GAMEPC immutable-base entry sequence does not match the current source file.");

        foreach (int index in edited.Keys)
            if (index < 0 || index >= sourceEntries.Count)
                throw new InvalidOperationException($"Unknown string index {index}.");

        byte[] header = source.AsSpan(0, HeaderSize).ToArray();
        int sourceTextEnd = HeaderSize + ReadHeaderUInt32(source, TextBlockLengthOffset, "text-block length", gameName);
        byte[] suffix = source.AsSpan(sourceTextEnd).ToArray();
        using var textPool = new MemoryStream();
        var intended = new List<byte[]>(sourceEntries.Count);

        foreach (GamePcStringEntry sourceEntry in sourceEntries)
        {
            byte[] bytes;
            if (edited.TryGetValue(sourceEntry.Index, out string? text))
            {
                try { ValidateText(text, enc); bytes = enc.GetBytes(text); }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidOperationException($"String index {sourceEntry.Index}: {ex.Message}", ex);
                }
            }
            else
            {
                // Reuse untouched source bytes verbatim: this makes a no-op save
                // byte-identical and avoids any accidental encoding normalization.
                bytes = sourceEntry.OriginalBytes;
            }
            if (baseEntries is not null)
            {
                GamePcStringEntry baseEntry = baseEntries[sourceEntry.Index];
                bytes = ApplyFixedHotspotMenuLayout(baseEntry, bytes);
                byte? delimiter = GetTerminalDelimiter(baseEntry);
                if (delimiter.HasValue)
                {
                    if (bytes.Length > 0 && bytes[^1] == delimiter.Value)
                        bytes = bytes[..^1];
                    bytes = [.. bytes, delimiter.Value];
                }
            }
            intended.Add(bytes);
            textPool.Write(bytes);
            textPool.WriteByte(0);
        }

        byte[] pool = textPool.ToArray();
        BinaryPrimitives.WriteUInt32BigEndian(header.AsSpan(TextBlockLengthOffset, sizeof(uint)), checked((uint)pool.Length));
        byte[] output = new byte[checked(HeaderSize + pool.Length + suffix.Length)];
        header.CopyTo(output, 0);
        pool.CopyTo(output, HeaderSize);
        suffix.CopyTo(output, HeaderSize + pool.Length);

        List<GamePcStringEntry> reparsed = LoadHeaderDefinedEntries(output, gameName);
        if (reparsed.Count != intended.Count || !reparsed.Select((entry, index) => entry.OriginalBytes.SequenceEqual(intended[index])).All(equal => equal))
            throw new InvalidDataException($"{gameName} GAMEPC repack validation did not reproduce the intended logical text sequence.");
        return output;
    }

    private static byte[] BuildLegacyFixedSlotData(
        byte[] data,
        IReadOnlyDictionary<int, string> edited,
        IReadOnlyList<GamePcStringEntry> entries,
        Encoding enc)
    {

        foreach (var kvp in edited)
        {
            var entry = entries.FirstOrDefault(e => e.Index == kvp.Key)
                ?? throw new InvalidOperationException($"Unknown string index {kvp.Key}.");

            ValidateText(kvp.Value, enc);
            byte[] newBytes = enc.GetBytes(kvp.Value);
            if (entry.Offset < 0 || entry.ByteLength < 0 || entry.Offset > data.Length - entry.ByteLength - 1)
                throw new InvalidDataException($"String index {entry.Index} does not fit inside the source GAMEPC file.");
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
        Encoding enc,
        ElviraGameProfile profile = ElviraGameProfile.Unknown,
        IReadOnlyList<GamePcStringEntry>? baseEntries = null)
    {
        byte[] data = BuildEditedData(dataFilePath, edited, entries, enc, profile, baseEntries);
        GameDataFileService.WriteCurrent(dataFilePath, data, entries.Count, profile);
    }

    internal static void ValidateSerializedData(string temporaryPath, int expectedEntryCount, ElviraGameProfile profile)
    {
        var verify = LoadEntries(temporaryPath, profile);
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

    public static string SaveAsNew(string sourcePath, string destinationPath, IReadOnlyDictionary<int, string> edits, IReadOnlyList<GamePcStringEntry> entries, Encoding enc, ElviraGameProfile profile = ElviraGameProfile.Unknown, IReadOnlyList<GamePcStringEntry>? baseEntries = null)
    {
        GamePcOriginalService.RejectProtectedPath(sourcePath);
        GamePcOriginalService.RejectProtectedPath(destinationPath);
        GameRootWriteGuard.RejectProtectedFileName(destinationPath);
        ValidateNewVariantDestination(sourcePath, destinationPath);
        byte[] data = GamePcTextEditor.BuildEditedData(sourcePath, edits, entries, enc, profile, baseEntries);
        WriteNew(destinationPath, data, entries.Count, profile);
        return Path.GetFullPath(destinationPath);
    }

    public static void WriteCurrent(string dataFilePath, byte[] data, int expectedEntryCount, ElviraGameProfile profile)
    {
        // Dormant in normal production UI, but a future caller must never reach
        // a protected GameRoot asset through this writer. In-place GAMEPC
        // replacement is obsolete: the SafeDeployer primitive was deleted, so
        // any residual GAMEPC targeting fails here instead of deploying.
        GameRootWriteGuard.RejectProtectedFileName(dataFilePath);
        GamePcOriginalService.RejectProtectedPath(dataFilePath);
        if (!File.Exists(dataFilePath)) throw new FileNotFoundException("Current data file is missing.", dataFilePath);
        string temp = TemporaryPath(dataFilePath, "save");
        try
        {
            File.WriteAllBytes(temp, data);
            GamePcTextEditor.ValidateSerializedData(temp, expectedEntryCount, profile);
            ReplaceWorkingVariant(dataFilePath, temp);
        }
        finally
        {
            try { if (File.Exists(temp)) File.Delete(temp); } catch { }
        }
    }

    private static void ValidateNewVariantDestination(string sourcePath, string destinationPath)
    {
        GamePcOriginalService.RejectProtectedPath(sourcePath);
        GamePcOriginalService.RejectProtectedPath(destinationPath);
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

    private static void WriteNew(string destinationPath, byte[] data, int expectedEntryCount, ElviraGameProfile profile)
    {
        string temp = TemporaryPath(destinationPath, "new");
        try
        {
            File.WriteAllBytes(temp, data);
            GamePcTextEditor.ValidateSerializedData(temp, expectedEntryCount, profile);
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
