using System.Security.Cryptography;

namespace ElviraVgaEditor;

internal enum RunVgaBootstrapState { Unsupported, OriginalPacked, UnpackedBaseline, ExtendedCp852V5 }

internal sealed record RunVgaBootstrapResult(string OutputPath, string Sha256, bool UsedEditedFont);

/// <summary>
/// Strict converter for one verified English Elvira I RUNVGA build. It never writes its input.
/// </summary>
internal static class RunVgaBootstrapService
{
    internal const int PackedSize = 0x18C39;
    internal const int BaselineSize = 0x2CE30;
    internal const int V5Size = 0x3B660;
    internal const int V5FontOffset = 0x3AE60;
    internal const int V5FontSize = 0x800;
    internal const string PackedSha256 = "70878FA8C1F2CEB7B2C624AD4DE36C8CBDA10AEDA65A7DA4991C2334B282FFD1";
    internal const string BaselineSha256 = "C6CEC09B41D42D92DDD0588B54E5729720CDC91ABEF8083F9205F237A9686756";
    internal const string HistoricalV5Sha256 = "1C386CCFAF544949A286AD40524F67792B1E3D0F11856EBB5ACE6A9498A0B6CB";
    internal const string HistoricalV5TableSha256 = "007953A94CBA8BDA13F88BDEF63158FDE6978282B5679AEC9260A6EF386F75B8";

    private static readonly byte[] OriginalRenderer = Convert.FromHexString("80EA20D1E2D1E2D1E2BEA62803F28E06E800B408");
    private static readonly byte[] V5Renderer = Convert.FromHexString("B103D3E28BF28E06E8008CD8054F238ED8B40890");
    private static readonly byte[] OriginalResizeCave = new byte[10];
    private static readonly byte[] V5ResizeCave = Convert.FromHexString("81C38000B44ACD21EB6A");
    private static readonly byte[] OriginalResizeRedirect = Convert.FromHexString("B44ACD21");
    private static readonly byte[] V5ResizeRedirect = Convert.FromHexString("EB8E9090");

    public static RunVgaBootstrapState DetectState(string path)
    {
        if (!File.Exists(path)) return RunVgaBootstrapState.Unsupported;
        byte[] data = File.ReadAllBytes(path);
        if (!IsMz(data)) return RunVgaBootstrapState.Unsupported;
        string hash = Hash(data);
        if (data.Length == PackedSize && hash == PackedSha256) return RunVgaBootstrapState.OriginalPacked;
        if (data.Length == BaselineSize && hash == BaselineSha256) return RunVgaBootstrapState.UnpackedBaseline;
        return IsV5(data) ? RunVgaBootstrapState.ExtendedCp852V5 : RunVgaBootstrapState.Unsupported;
    }

    public static RunVgaBootstrapResult CreateExtendedCp852(string sourcePath, string destinationPath, IReadOnlyList<GlyphModel> glyphs)
    {
        RunVgaBootstrapState state = DetectState(sourcePath);
        if (state == RunVgaBootstrapState.ExtendedCp852V5)
            throw new InvalidOperationException("This RUNVGA is already an Extended CP852 / V5 executable; bootstrap is intentionally disabled.");
        if (state is not (RunVgaBootstrapState.OriginalPacked or RunVgaBootstrapState.UnpackedBaseline))
            throw new InvalidDataException("Unsupported RUNVGA. Bootstrap supports only the exact verified English packed original or the exact verified BAM/V3 unpacked baseline.");
        if (Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(destinationPath), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Bootstrap never overwrites its input. Choose a new output filename.");
        if (File.Exists(destinationPath))
            throw new IOException("The destination already exists. Choose a new filename so the operation remains transactional.");

        byte[] input = File.ReadAllBytes(sourcePath);
        byte[] baseline = state == RunVgaBootstrapState.OriginalPacked ? UnpackVerifiedOriginal(input) : input;
        ValidateBaseline(baseline);
        (byte[] table, bool usedEdited) = CreateFontTable(baseline, glyphs);
        byte[] output = BuildV5(baseline, table);
        ValidateV5(output);

        string directory = Path.GetDirectoryName(Path.GetFullPath(destinationPath)) ?? throw new IOException("Output directory is unavailable.");
        Directory.CreateDirectory(directory);
        string temporary = Path.Combine(directory, "." + Path.GetFileName(destinationPath) + ".bootstrap_" + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            File.WriteAllBytes(temporary, output);
            if (File.Exists(destinationPath)) throw new IOException("The destination was created while bootstrap was running.");
            File.Move(temporary, destinationPath);
        }
        finally
        {
            if (File.Exists(temporary)) File.Delete(temporary);
        }

        // Integration check through the existing V5 detector.
        FontLoadResult loaded = RunVgaFontService.LoadRunVga(destinationPath);
        if (loaded.Layout != RunVgaFontLayout.ExtendedCp852V5 || loaded.LoadedGlyphCount != 256)
            throw new InvalidDataException("The generated executable did not pass the existing V5 font-layout detector.");
        return new RunVgaBootstrapResult(destinationPath, Hash(output), usedEdited);
    }

    // Regression entry point: baseline + historical fixture table must hash to HistoricalV5Sha256.
    public static bool VerifyHistoricalV5Regression(byte[] verifiedBaseline, byte[] historicalTable)
    {
        ValidateBaseline(verifiedBaseline);
        if (historicalTable.Length != V5FontSize || Hash(historicalTable) != HistoricalV5TableSha256) return false;
        return Hash(BuildV5(verifiedBaseline, historicalTable)) == HistoricalV5Sha256;
    }

    internal static byte[] BuildV5(byte[] baseline, byte[] fontTable)
    {
        ValidateBaseline(baseline);
        if (fontTable.Length != V5FontSize) throw new InvalidDataException("The CP852 font table must contain exactly 2048 bytes.");
        var output = new byte[V5Size];
        Array.Copy(baseline, output, BaselineSize);
        WriteU16(output, 0x0002, 0x0060);
        WriteU16(output, 0x0004, 0x01DC);
        WriteU16(output, 0x000A, 0x0000);
        ReplaceExpected(output, 0xF34C, OriginalRenderer, V5Renderer, "renderer");
        ReplaceExpected(output, 0x11638, OriginalResizeCave, V5ResizeCave, "DOS memory-resize hook");
        ReplaceExpected(output, 0x116A8, OriginalResizeRedirect, V5ResizeRedirect, "DOS memory-resize redirect");
        Array.Copy(fontTable, 0, output, V5FontOffset, V5FontSize);
        // Raw callers may supply a complete table directly, so this final image
        // boundary must enforce the engine-owned HUD erase glyph independently
        // of UI/model/import protections.
        Array.Copy(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes, 0, output,
            V5FontOffset + FontSlotMetadata.HudEraseGlyph * RunVgaFontService.GlyphBytes,
            RunVgaFontService.GlyphBytes);
        ValidateV5(output);
        return output;
    }

    internal static byte[] UnpackVerifiedOriginal(byte[] packed)
    {
        if (packed.Length != PackedSize || Hash(packed) != PackedSha256)
            throw new InvalidDataException("The packed RUNVGA hash does not match the one supported English Elvira I original.");
        if (!IsMz(packed)) throw new InvalidDataException("The packed RUNVGA is not an MZ executable.");

        int headerParagraphs = ReadU16(packed, 0x08);
        int packedCs = ReadU16(packed, 0x16);
        int exepackOffset = (headerParagraphs + packedCs) * 16;
        if (exepackOffset < headerParagraphs * 16 || exepackOffset + 18 > packed.Length)
            throw new InvalidDataException("EXEPACK header lies outside the supported input.");
        int destinationLength = ReadU16(packed, exepackOffset + 12) * 16;
        int skipOrSignature = ReadU16(packed, exepackOffset + 14);
        int signature = ReadU16(packed, exepackOffset + 16);
        if (skipOrSignature != 0x4252 && signature != 0x4252)
            throw new InvalidDataException("The supported EXEPACK RB signature was not found.");

        int packedStart = headerParagraphs * 16;
        byte[] stream = packed[packedStart..exepackOffset].Reverse().ToArray();
        byte[] unpacked = new byte[destinationLength];
        int source = 0;
        while (source < stream.Length && stream[source] == 0xFF) source++;
        int destination = 0;
        bool stop = false;
        while (!stop && source < stream.Length)
        {
            if (source + 3 > stream.Length) throw new InvalidDataException("Truncated EXEPACK command stream.");
            byte opcode = stream[source++];
            int count = (stream[source++] << 8) | stream[source++];
            if (count < 0 || destination + count > unpacked.Length) throw new InvalidDataException("EXEPACK command exceeds output buffer.");
            if ((opcode & 0xFE) == 0xB0)
            {
                if (source >= stream.Length) throw new InvalidDataException("Truncated EXEPACK fill command.");
                Array.Fill(unpacked, stream[source++], destination, count);
            }
            else if ((opcode & 0xFE) == 0xB2)
            {
                if (source + count > stream.Length) throw new InvalidDataException("Truncated EXEPACK copy command.");
                Array.Copy(stream, source, unpacked, destination, count);
                source += count;
            }
            else throw new InvalidDataException($"Unsupported EXEPACK opcode 0x{opcode:X2}.");
            destination += count;
            stop = (opcode & 1) != 0;
        }
        int remaining = stream.Length - source;
        if (remaining > unpacked.Length - destination) throw new InvalidDataException("EXEPACK trailing data exceeds output buffer.");
        Array.Copy(stream, source, unpacked, destination, remaining);
        int unpackedLength = destination + remaining;
        Array.Reverse(unpacked, 0, unpackedLength);

        int corruptText = IndexOf(packed, "Packed file is corrupt"u8.ToArray(), exepackOffset);
        if (corruptText < 0) throw new InvalidDataException("EXEPACK relocation marker was not found.");
        int relocationSource = corruptText + "Packed file is corrupt"u8.Length;
        var relocations = new List<(ushort Offset, ushort Segment)>();
        for (int segmentIndex = 0; segmentIndex < 16; segmentIndex++)
        {
            ushort count = ReadU16(packed, relocationSource); relocationSource += 2;
            for (int i = 0; i < count; i++)
            {
                relocations.Add((ReadU16(packed, relocationSource), (ushort)(segmentIndex * 0x1000)));
                relocationSource += 2;
            }
        }
        const int mzHeaderBytes = 28;
        int headerParagraphsNew = ((mzHeaderBytes + relocations.Count * 4) / 16 / 32 + 1) * 32;
        int headerBytesNew = headerParagraphsNew * 16;
        byte[] result = new byte[headerBytesNew + unpackedLength];
        WriteU16(result, 0x00, 0x5A4D);
        WriteU16(result, 0x02, (ushort)(result.Length % 512));
        WriteU16(result, 0x04, (ushort)(result.Length / 512 + (result.Length % 512 == 0 ? 0 : 1)));
        WriteU16(result, 0x06, (ushort)relocations.Count);
        WriteU16(result, 0x08, (ushort)headerParagraphsNew);
        WriteU16(result, 0x0A, ReadU16(packed, 0x0A));
        WriteU16(result, 0x0C, 0xFFFF);
        WriteU16(result, 0x0E, ReadU16(packed, exepackOffset + 10));
        WriteU16(result, 0x10, ReadU16(packed, exepackOffset + 8));
        WriteU16(result, 0x14, ReadU16(packed, exepackOffset));
        WriteU16(result, 0x16, ReadU16(packed, exepackOffset + 2));
        WriteU16(result, 0x18, mzHeaderBytes);
        int relocOffset = mzHeaderBytes;
        foreach ((ushort offset, ushort segment) in relocations)
        {
            WriteU16(result, relocOffset, offset); WriteU16(result, relocOffset + 2, segment); relocOffset += 4;
        }
        Array.Copy(unpacked, 0, result, headerBytesNew, unpackedLength);

        // The historical BAM/V3 baseline preserves these original header values.
        WriteU16(result, 0x0A, 0x01B5);
        WriteU16(result, 0x12, 0xE0BD);
        ValidateBaseline(result);
        return result;
    }

    private static (byte[] Table, bool UsedEdited) CreateFontTable(byte[] baseline, IReadOnlyList<GlyphModel> glyphs)
    {
        ValidateBaseline(baseline);
        if (glyphs.Count != 256) throw new InvalidDataException("The editor must provide all 256 CP852 glyph slots.");
        var table = new byte[V5FontSize];
        // Preserve the authentic 0x20..0x81 Elvira glyphs as the safe default. This keeps
        // ordinary game text readable even before the user imports a complete CP852 font.
        Array.Copy(baseline, 0x1A216, table, 0x20 * 8, 98 * 8);
        bool usedEdited = glyphs.Any(g => g.HasEdited);
        for (int code = 0; code < 256; code++)
        {
            GlyphModel glyph = glyphs.FirstOrDefault(g => g.ByteValue == code)
                ?? throw new InvalidDataException($"Missing CP852 glyph slot 0x{code:X2}.");
            // 0x81 is an engine erase pass and must not be replaced by imported
            // or manually altered glyph data.
            if (FontSlotMetadata.GetReservedSlots(ElviraGame.Elvira1, RunVgaFontLayout.ExtendedCp852V5).Contains(code))
                continue;
            // Unedited, unloaded slots are intentionally left as the zero/default bank.
            // Known editor fallback diacritics are safe defaults for their CP852 slots.
            if (!glyph.HasEdited && !glyph.HasKnownFallbackBitmap) continue;
            byte[] bitmap = glyph.HasEdited ? glyph.Edited : glyph.Original;
            if (bitmap.Length != 8) throw new InvalidDataException($"Glyph 0x{code:X2} is not an 8-row bitmap.");
            Array.Copy(bitmap, 0, table, code * 8, 8);
        }
        Array.Copy(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes, 0, table,
            FontSlotMetadata.HudEraseGlyph * 8, RunVgaFontService.GlyphBytes);
        return (table, usedEdited);
    }

    private static void ValidateBaseline(byte[] data)
    {
        if (data.Length != BaselineSize || Hash(data) != BaselineSha256)
            throw new InvalidDataException("Unpacked RUNVGA does not match the verified BAM/V3 baseline; bootstrap aborted.");
        if (!IsMz(data) || ReadU16(data, 0x08) != 0x0240 || ReadU16(data, 0x02) != 0x0030 || ReadU16(data, 0x04) != 0x0168 || ReadU16(data, 0x0A) != 0x01B5)
            throw new InvalidDataException("Verified baseline MZ invariants failed.");
        Require(data, 0xF34C, OriginalRenderer, "renderer");
        Require(data, 0x11638, OriginalResizeCave, "DOS memory-resize cave");
        Require(data, 0x116A8, OriginalResizeRedirect, "DOS memory-resize redirect");
    }

    private static void ValidateV5(byte[] data)
    {
        if (!IsV5(data)) throw new InvalidDataException("Generated V5 executable failed structural validation.");
        if (!At(data, V5FontOffset + FontSlotMetadata.HudEraseGlyph * RunVgaFontService.GlyphBytes,
            FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes))
            throw new InvalidDataException("Generated V5 executable does not preserve the reserved HUD erase glyph 0x81.");
        for (int i = BaselineSize; i < V5FontOffset; i++)
            if (data[i] != 0) throw new InvalidDataException("V5 zero-padding invariant failed.");
    }

    private static bool IsV5(byte[] data) => data.Length == V5Size && IsMz(data) && ReadU16(data, 0x08) == 0x0240 &&
        ReadU16(data, 0x02) == 0x0060 && ReadU16(data, 0x04) == 0x01DC && ReadU16(data, 0x0A) == 0x0000 &&
        ((ReadU16(data, 0x04) - 1) * 512 + ReadU16(data, 0x02) == V5Size) &&
        At(data, 0xF34C, V5Renderer) && At(data, 0x11638, V5ResizeCave) && At(data, 0x116A8, V5ResizeRedirect) &&
        V5FontOffset + V5FontSize == data.Length;

    private static void ReplaceExpected(byte[] data, int offset, byte[] expected, byte[] replacement, string name)
    {
        Require(data, offset, expected, name);
        Array.Copy(replacement, 0, data, offset, replacement.Length);
    }
    private static void Require(byte[] data, int offset, byte[] expected, string name)
    {
        if (!At(data, offset, expected)) throw new InvalidDataException($"Expected original {name} bytes were not found at 0x{offset:X}; bootstrap aborted.");
    }
    private static bool At(byte[] data, int offset, byte[] expected) => offset >= 0 && offset + expected.Length <= data.Length && data.AsSpan(offset, expected.Length).SequenceEqual(expected);
    private static bool IsMz(byte[] data) => data.Length >= 28 && data[0] == (byte)'M' && data[1] == (byte)'Z';
    private static ushort ReadU16(byte[] data, int offset) => (ushort)(data[offset] | data[offset + 1] << 8);
    private static void WriteU16(byte[] data, int offset, ushort value) { data[offset] = (byte)value; data[offset + 1] = (byte)(value >> 8); }
    private static string Hash(byte[] data) => Convert.ToHexString(SHA256.HashData(data));
    private static int IndexOf(byte[] data, byte[] needle, int start)
    {
        for (int i = start; i <= data.Length - needle.Length; i++) if (data.AsSpan(i, needle.Length).SequenceEqual(needle)) return i;
        return -1;
    }
}
