using System.Security.Cryptography;

namespace Pi1ElviraEditor;

internal enum RunItBootstrapState { Unsupported, OriginalPacked, CanonicalUnpackedAscii98, ExtendedCp852 }

internal sealed record RunItBootstrapResult(string OutputPath, string Sha256, bool UsedEditedFont);

/// <summary>
/// Elvira II V2 split-font bootstrap. LOW glyphs stay in the verified original
/// 0x20..0x81 table; HIGH glyphs occupy initialized SS:0000..03EF. The helper at
/// SS:03F0 renders HIGH glyphs from CS:SI while retaining ES as the VGA output segment.
/// </summary>
internal static class RunItBootstrapService
{
    internal const int PackedSize = 0x13F20;
    internal const int CanonicalSize = 0x27A20;
    internal const int ExtendedSize = 0x288C1;
    internal const int HeaderSize = 0x1400;
    internal const int OriginalFontOffset = 0x168CA;
    internal const int OriginalFontGlyphCount = 98;
    internal const int HighFontOffset = 0x28480;
    internal const int HighFirstByte = 0x82;
    internal const int HighFontSize = 0x3F0;
    private const byte GoldenHighFontSeed = 0x29;
    private const byte GoldenHighFontStep = 0x49;
    internal const int HelperOffset = 0x28870;
    internal const int HelperSize = 81;
    internal const int RendererOffset = 0x7B89;
    internal const int RendererRegionSize = 20;
    internal const string PackedSha256 = "8E9E9C327E02B464E0A956F1544017C45C94C32E1E86732EB17FDED8BBC32FD8";
    internal const string CanonicalSha256 = "7FDE00D641D3BDE82B1732DAD59D21CE19F48573CBEFAD6C64C11754660EB415";
    internal const string CanonicalModuleSha256 = "65413F0A3D99FB20AF1C1B4F2E6A5BDD99768EAA6C6A833C215E3F432F9F4FF4";
    internal const string OriginalFontSha256 = "68C1F23840028438CD9975C5CD79F1861AAF7A009B6F5C65D95CE0B243DDB893";
    /// <summary>
    /// R9D trust anchor for a V2 image. This is a derived value, not a new supported
    /// identity: the frozen canonical image with the documented V2 fixed patches applied
    /// (header words, extended renderer) and the documented mutable LOW font region
    /// (OriginalFontOffset, 98x8 bytes) zeroed, SHA-256 hashed. Any byte outside the
    /// documented mutable font regions therefore fails <see cref="IsTrustedV2PatchTarget"/>.
    /// </summary>
    internal const string TrustedMaskedSha256 = "D82200F218F0B181356E602AB8AA8253DBC17B72D0A2B6865B6423B3034CF4DB";
    // Original renderer lookup / loop prefix at physical 0x7B89.
    internal static readonly byte[] OriginalRenderer = Convert.FromHexString("B60080EA20D1E2D1E2D1E2BECA2A03F28E061206");
    // Exact V2 thunk. It transfers HIGH codes to SS:03F0 and falls through to the old loop for LOW codes.
    internal static readonly byte[] ExtendedRenderer = Convert.FromHexString("0E68ED038CDE81C668145668F003CB9090909090");
    // Exact V2 helper, recovered from the working runtime image. It restores DS and skips the old loop on return.
    internal static readonly byte[] HighRendererHelper = Convert.FromHexString(
        "B60080EA20D1E2D1E2D1E2BECA2A03F29C81FA1003723481EEDA2D8E0612069D268A1580E2F002DAB408B906002E8A04D0E0730326881D47E2F64681C73A01FECC75E78BDC36C7070F04CB8E0612069DCB");

    public static RunItBootstrapState DetectState(string path)
    {
        if (!File.Exists(path)) return RunItBootstrapState.Unsupported;
        byte[] data = File.ReadAllBytes(path);
        if (!IsMz(data)) return RunItBootstrapState.Unsupported;
        string hash = Hash(data);
        if (data.Length == PackedSize && hash == PackedSha256) return RunItBootstrapState.OriginalPacked;
        if (data.Length == CanonicalSize && hash == CanonicalSha256) return RunItBootstrapState.CanonicalUnpackedAscii98;
        return IsExtended(data) ? RunItBootstrapState.ExtendedCp852 : RunItBootstrapState.Unsupported;
    }

    public static RunItBootstrapResult CreateExtendedCp852(string sourcePath, string destinationPath, IReadOnlyList<GlyphModel> glyphs)
    {
        // R9D: a missing source is a different result from an unsupported one.
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("RUNIT source is missing; bootstrap was blocked. No files were changed.", sourcePath);
        RunItBootstrapState state = DetectState(sourcePath);
        if (state == RunItBootstrapState.ExtendedCp852)
            throw new InvalidOperationException("This RUNIT.EXE is already Extended CP852; regenerate from RUNITO.EXE instead.");
        if (state is not (RunItBootstrapState.OriginalPacked or RunItBootstrapState.CanonicalUnpackedAscii98))
            throw new InvalidDataException("Unsupported or modified RUNIT.EXE.");
        if (Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(destinationPath), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Bootstrap never overwrites its input. Choose a new output filename.");
        if (File.Exists(destinationPath)) throw new IOException("The destination already exists. Choose a new output filename.");

        byte[] canonical = state == RunItBootstrapState.OriginalPacked
            ? UnpackCanonicalOriginal(File.ReadAllBytes(sourcePath))
            : File.ReadAllBytes(sourcePath);
        byte[] output = BuildExtended(canonical, CreateFontImage(canonical, glyphs, out bool usedEdited));
        ValidateExtended(output);
        File.WriteAllBytes(destinationPath, output);
        return new RunItBootstrapResult(destinationPath, Hash(output), usedEdited);
    }

    /// <summary>Builds a deterministic V2 image from the verified canonical image and a 256-slot CP852 image.</summary>
    internal static byte[] BuildExtended(byte[] canonical, byte[] fontImage)
    {
        ValidateCanonical(canonical);
        if (fontImage.Length != 0x800) throw new InvalidDataException("CP852 font image must contain exactly 2048 bytes.");

        byte[] result = new byte[ExtendedSize]; // deterministic zero fill through the MZ image tail
        Array.Copy(canonical, result, canonical.Length);
        Require(result, RendererOffset, OriginalRenderer, "renderer");
        Array.Copy(ExtendedRenderer, 0, result, RendererOffset, ExtendedRenderer.Length);

        // LOW direct byte mapping: character 0x20..0x81 -> original native 98x8 table.
        Array.Copy(fontImage, 0x20 * 8, result, OriginalFontOffset, OriginalFontGlyphCount * 8);
        // Preserve the patched full-cell erase mask even if a caller supplies a malformed image.
        Array.Copy(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes, 0, result,
            OriginalFontOffset + (FontSlotMetadata.HudEraseGlyph - 0x20) * 8, RunVgaFontService.GlyphBytes);
        // HIGH direct byte mapping: character 0x82..0xFF -> initialized initial-stack segment.
        Array.Copy(fontImage, HighFirstByte * 8, result, HighFontOffset, HighFontSize);
        Array.Copy(HighRendererHelper, 0, result, HelperOffset, HelperSize);

        // 0x288C1 bytes means e_cp=0x145, e_cblp=0x0C1. All other V2 header geometry remains canonical.
        WriteU16(result, 0x02, 0x00C1);
        WriteU16(result, 0x04, 0x0145);
        ValidateExtended(result);
        return result;
    }

    internal static byte[] CreateFontImage(byte[] canonical, IReadOnlyList<GlyphModel> glyphs, out bool usedEdited)
    {
        ValidateCanonical(canonical);
        if (glyphs.Count != 256) throw new InvalidDataException("The editor must provide all 256 CP852 glyph slots.");
        byte[] image = new byte[0x800];
        Array.Copy(canonical, OriginalFontOffset, image, 0x20 * 8, OriginalFontGlyphCount * 8);
        // The verified untouched V2 baseline uses the deterministic initialized HIGH seed
        // (0x29 + n*0x49), not the editor's illustrative fallback glyphs. Reproducing it
        // exactly keeps the no-import bootstrap byte-identical to the golden V2 image.
        for (int index = 0; index < HighFontSize; index++)
            image[HighFirstByte * 8 + index] = unchecked((byte)(GoldenHighFontSeed + index * GoldenHighFontStep));

        usedEdited = glyphs.Any(g => g.HasEdited && g.ByteValue != FontSlotMetadata.HudEraseGlyph);
        foreach (GlyphModel glyph in glyphs)
        {
            if (glyph.ByteValue is < 0x20 or > 0xFF || !glyph.HasEdited) continue;
            if (glyph.ByteValue == FontSlotMetadata.HudEraseGlyph) continue;
            byte[] bitmap = glyph.HasEdited ? glyph.Edited : glyph.Original;
            if (bitmap.Length != 8) throw new InvalidDataException($"Glyph 0x{glyph.ByteValue:X2} is not an 8-row bitmap.");
            Array.Copy(bitmap, 0, image, glyph.ByteValue * 8, 8);
        }
        return image;
    }

    internal static byte[] UnpackCanonicalOriginal(byte[] packed)
    {
        if (packed.Length != PackedSize || Hash(packed) != PackedSha256)
            throw new InvalidDataException("Unsupported or modified RUNIT.EXE.");
        if (!IsMz(packed)) throw new InvalidDataException("RUNIT.EXE is not an MZ executable.");
        int headerParagraphs = ReadU16(packed, 0x08);
        int exepackOffset = (headerParagraphs + ReadU16(packed, 0x16)) * 16;
        if (exepackOffset < headerParagraphs * 16 || exepackOffset + 18 > packed.Length) throw new InvalidDataException("EXEPACK header lies outside RUNIT.EXE.");
        int destinationLength = ReadU16(packed, exepackOffset + 12) * 16;
        int rb0 = ReadU16(packed, exepackOffset + 14), rb1 = ReadU16(packed, exepackOffset + 16);
        if (rb0 != 0x4252 && rb1 != 0x4252) throw new InvalidDataException("EXEPACK RB signature was not found.");

        byte[] stream = packed[(headerParagraphs * 16)..exepackOffset].Reverse().ToArray();
        byte[] module = new byte[destinationLength];
        int source = 0; while (source < stream.Length && stream[source] == 0xFF) source++;
        int destination = 0; bool stop = false;
        while (!stop && source < stream.Length)
        {
            if (source + 3 > stream.Length) throw new InvalidDataException("Truncated EXEPACK command stream.");
            byte opcode = stream[source++]; int count = (stream[source++] << 8) | stream[source++];
            if (destination + count > module.Length) throw new InvalidDataException("EXEPACK command exceeds output buffer.");
            if ((opcode & 0xFE) == 0xB0)
            {
                if (source >= stream.Length) throw new InvalidDataException("Truncated EXEPACK fill command.");
                Array.Fill(module, stream[source++], destination, count);
            }
            else if ((opcode & 0xFE) == 0xB2)
            {
                if (source + count > stream.Length) throw new InvalidDataException("Truncated EXEPACK copy command.");
                Array.Copy(stream, source, module, destination, count); source += count;
            }
            else throw new InvalidDataException($"Unsupported EXEPACK opcode 0x{opcode:X2}.");
            destination += count; stop = (opcode & 1) != 0;
        }
        int remaining = stream.Length - source;
        if (remaining > module.Length - destination) throw new InvalidDataException("EXEPACK trailing data exceeds output buffer.");
        Array.Copy(stream, source, module, destination, remaining); Array.Reverse(module, 0, destination + remaining);
        if (module.Length != 0x26620 || Hash(module) != CanonicalModuleSha256) throw new InvalidDataException("RUNIT.EXE unpacked load module does not match the supported canonical profile.");

        int corruptText = IndexOf(packed, "Packed file is corrupt"u8.ToArray(), exepackOffset);
        if (corruptText < 0) throw new InvalidDataException("EXEPACK relocation marker was not found.");
        int relocationSource = corruptText + "Packed file is corrupt"u8.Length;
        var relocations = new List<(ushort Offset, ushort Segment)>();
        for (int segmentIndex = 0; segmentIndex < 16; segmentIndex++)
        {
            ushort count = ReadU16(packed, relocationSource); relocationSource += 2;
            for (int i = 0; i < count; i++) { relocations.Add((ReadU16(packed, relocationSource), (ushort)(segmentIndex * 0x1000))); relocationSource += 2; }
        }
        if (relocations.Count != 0x4B7) throw new InvalidDataException("Unexpected RUNIT.EXE relocation count.");
        byte[] result = new byte[CanonicalSize];
        WriteU16(result, 0x00, 0x5A4D); WriteU16(result, 0x02, 0x0020); WriteU16(result, 0x04, 0x013E); WriteU16(result, 0x06, 0x04B7);
        WriteU16(result, 0x08, 0x0140); WriteU16(result, 0x0A, 0x0157); WriteU16(result, 0x0C, 0xFFFF); WriteU16(result, 0x0E, 0x2708);
        WriteU16(result, 0x10, 0x0B00); WriteU16(result, 0x12, 0x0000); WriteU16(result, 0x14, 0x0010); WriteU16(result, 0x16, 0x0C47); WriteU16(result, 0x18, 0x001C);
        int relocationOffset = 0x1C;
        foreach ((ushort offset, ushort segment) in relocations) { WriteU16(result, relocationOffset, offset); WriteU16(result, relocationOffset + 2, segment); relocationOffset += 4; }
        Array.Copy(module, 0, result, HeaderSize, module.Length); ValidateCanonical(result); return result;
    }

    internal static void ValidateCanonical(byte[] data)
    {
        if (data.Length != CanonicalSize || Hash(data) != CanonicalSha256) throw new InvalidDataException("Unpacked RUNIT.EXE does not match the verified canonical Elvira II baseline.");
        if (!IsMz(data) || ReadU16(data, 0x08) != 0x0140 || ReadU16(data, 0x0A) != 0x0157 || ReadU16(data, 0x06) != 0x04B7 || ReadU16(data, 0x14) != 0x0010 || ReadU16(data, 0x16) != 0x0C47) throw new InvalidDataException("Canonical RUNIT.EXE MZ invariants failed.");
        if (Hash(data.AsSpan(HeaderSize, 0x26620)) != CanonicalModuleSha256 || Hash(data.AsSpan(OriginalFontOffset, OriginalFontGlyphCount * 8)) != OriginalFontSha256) throw new InvalidDataException("Canonical RUNIT.EXE invariants failed.");
    }

    internal static void ValidateExtended(byte[] data)
    {
        if (!IsExtended(data)) throw new InvalidDataException("Generated V2 split-font RUNIT.EXE failed structural validation.");
        if (!At(data, OriginalFontOffset + (FontSlotMetadata.HudEraseGlyph - 0x20) * 8, FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes))
            throw new InvalidDataException("Generated V2 split-font RUNIT.EXE does not preserve the reserved Elvira II HUD erase glyph 0x81.");
    }

    /// <summary>
    /// R9D trusted-patch check for a V2 image. Structural V2 recognition alone never
    /// authorizes a mutation: every byte outside the documented mutable font regions
    /// (LOW table at OriginalFontOffset, HIGH table plus fixed helper in the appended
    /// tail) must reproduce the frozen canonical image with the documented fixed
    /// patches, proven by <see cref="TrustedMaskedSha256"/>. A single modified code
    /// byte fails while legitimate edited fonts keep passing.
    /// </summary>
    internal static bool IsTrustedV2PatchTarget(byte[] image)
    {
        if (!IsExtended(image)) return false;
        try { ValidateExtended(image); }
        catch (InvalidDataException) { return false; }
        // Mirror the TrustedMaskedSha256 derivation exactly: the fixed V2 header words
        // and extended renderer must already be present (never restored here), and only
        // the documented mutable LOW font region is excluded before hashing.
        byte[] prefix = image[0..CanonicalSize].ToArray();
        if (ReadU16(prefix, 0x02) != 0x00C1 || ReadU16(prefix, 0x04) != 0x0145) return false;
        if (!At(prefix, RendererOffset, ExtendedRenderer)) return false;
        Array.Clear(prefix, OriginalFontOffset, OriginalFontGlyphCount * RunVgaFontService.GlyphBytes);
        return Hash(prefix) == TrustedMaskedSha256;
    }

    internal static bool IsExtended(byte[] data) =>
        data.Length == ExtendedSize && IsMz(data) && ReadU16(data, 0x02) == 0x00C1 && ReadU16(data, 0x04) == 0x0145 &&
        ReadU16(data, 0x06) == 0x04B7 && ReadU16(data, 0x08) == 0x0140 && ReadU16(data, 0x0A) == 0x0157 &&
        ReadU16(data, 0x0E) == 0x2708 && ReadU16(data, 0x10) == 0x0B00 && ReadU16(data, 0x14) == 0x0010 && ReadU16(data, 0x16) == 0x0C47 &&
        At(data, RendererOffset, ExtendedRenderer) && At(data, HelperOffset, HighRendererHelper) && HighFontOffset + HighFontSize == HelperOffset && HelperOffset + HelperSize == data.Length;

    private static bool IsMz(byte[] data) => data.Length >= 28 && ReadU16(data, 0) == 0x5A4D;
    private static ushort ReadU16(byte[] data, int offset) => (ushort)(data[offset] | (data[offset + 1] << 8));
    private static void WriteU16(byte[] data, int offset, ushort value) { data[offset] = (byte)value; data[offset + 1] = (byte)(value >> 8); }
    private static string Hash(byte[] data) => Convert.ToHexString(SHA256.HashData(data));
    private static string Hash(ReadOnlySpan<byte> data) => Convert.ToHexString(SHA256.HashData(data));
    private static bool At(byte[] data, int offset, byte[] expected) => offset >= 0 && offset + expected.Length <= data.Length && data.AsSpan(offset, expected.Length).SequenceEqual(expected);
    private static void Require(byte[] data, int offset, byte[] expected, string what) { if (!At(data, offset, expected)) throw new InvalidDataException($"Unsupported or modified RUNIT.EXE: {what} bytes differ."); }
    private static int IndexOf(byte[] haystack, byte[] needle, int start) { for (int i = start; i <= haystack.Length - needle.Length; i++) if (haystack.AsSpan(i, needle.Length).SequenceEqual(needle)) return i; return -1; }
}
