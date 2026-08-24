using System.Security.Cryptography;
using System.Text;

namespace ElviraVgaEditor;

internal enum FontSourceType { RunVga, RunEga }
internal enum RunVgaFontLayout { Unknown, OriginalPackedAscii80, OriginalPackedAscii98, OriginalAscii98, ExtendedCp852V5, ExtendedCp852RunIt }
internal enum ElviraGame { Unknown, Elvira1, Elvira2 }

// Reserved slots are game/layout metadata, not a generic CP852 rule. Both games
// use 0x81 as a HUD erase mask, but their executable layouts remain independent.
internal static class FontSlotMetadata
{
    internal const int HudEraseGlyph = 0x81;
    private static readonly byte[] Elvira1HudEraseBytes = Convert.FromHexString("00FCFCFCFCFCFC00");
    private static readonly byte[] Elvira2HudEraseBytes = Convert.FromHexString("00FCFCFCFCFCFC00");
    private static readonly int[] Elvira1ReservedSlots = [HudEraseGlyph];
    private static readonly int[] Elvira2RunItReservedSlots = [HudEraseGlyph];
    private static readonly int[] NoReservedSlots = [];

    public static IReadOnlyList<int> GetReservedSlots(ElviraGame game, RunVgaFontLayout layout) =>
        (game, layout) switch
        {
            (ElviraGame.Elvira1, RunVgaFontLayout.OriginalPackedAscii98) or
            (ElviraGame.Elvira1, RunVgaFontLayout.OriginalAscii98) or
            (ElviraGame.Elvira1, RunVgaFontLayout.ExtendedCp852V5) => Elvira1ReservedSlots,
            (ElviraGame.Elvira2, RunVgaFontLayout.ExtendedCp852RunIt) => Elvira2RunItReservedSlots,
            _ => NoReservedSlots
        };

    public static bool IsReserved(FontLoadResult? loaded, int code) =>
        loaded is not null && GetReservedSlots(loaded.Game, loaded.Layout).Contains(code);

    public static byte[] GetCanonicalBytes(FontLoadResult loaded, int code)
    {
        if (!IsReserved(loaded, code))
            throw new InvalidOperationException($"Glyph 0x{code:X2} is not reserved for this game/layout.");
        return loaded.Game == ElviraGame.Elvira1 ? Elvira1HudEraseBytes.ToArray() : Elvira2HudEraseBytes.ToArray();
    }

    public static void RestoreReservedSourceGlyphs(FontLoadResult loaded)
    {
        foreach (int code in loaded.ReservedGlyphSlots)
            loaded.Glyphs[code].LoadFromSource(GetCanonicalBytes(loaded, code));
    }
}

internal sealed class FontLoadResult
{
    public required string SourcePath { get; init; }
    public FontSourceType SourceType { get; init; } = FontSourceType.RunVga;
    public RunVgaFontLayout Layout { get; init; }
    public int FontOffset { get; init; }
    public int FirstByteValue { get; init; }
    public int LastByteValue { get; init; } = 255;
    public int LoadedGlyphCount { get; init; }
    public required List<GlyphModel> Glyphs { get; init; }
    public required string DetectionDetails { get; init; }
    public ElviraGame Game { get; init; } = ElviraGame.Unknown;
    // CP852 editing exposes printable/game-addressable slots 0x20..0xFF. The
    // V5 table still physically retains all 256 byte-indexed records.
    public int PhysicalSlotCount => HasFullCp852Font ? 0x100 - RunVgaFontService.OriginalFirstChar : LoadedGlyphCount;
    public IReadOnlyList<int> ReservedGlyphSlots => FontSlotMetadata.GetReservedSlots(Game, Layout);
    public int EditableGlyphCount => Math.Max(0, PhysicalSlotCount - ReservedGlyphSlots.Count);
    public bool CanApply => (Layout == RunVgaFontLayout.OriginalAscii98 || Layout == RunVgaFontLayout.ExtendedCp852V5 || Layout == RunVgaFontLayout.ExtendedCp852RunIt) && FontOffset >= 0 && LoadedGlyphCount > 0;
    public bool CanInitializeEdited => Layout != RunVgaFontLayout.Unknown && LoadedGlyphCount > 0;
    public bool HasFullCp852Font => Layout is RunVgaFontLayout.ExtendedCp852V5 or RunVgaFontLayout.ExtendedCp852RunIt;
}

internal static class RunVgaFontService
{
    public const int GlyphBytes = 8;
    public const int ExtendedGlyphCount = 256;
    public const int ExtendedFontBytes = ExtendedGlyphCount * GlyphBytes;
    public const int OriginalFirstChar = 32;
    public const int OriginalGlyphCount = 98;
    public const int PackedOriginalFirstChar = 0x2F;
    public const int PackedOriginalLastChar = 0x7E;
    public const int PackedOriginalGlyphCount = PackedOriginalLastChar - PackedOriginalFirstChar + 1;

    // Recovered / runtime-verified Elvira 1 DOS layouts.
    private const int OriginalFontLoadModuleOffset = 0x17E16;
    private const int OriginalRendererPhysicalOffset = 0xF355;
    private const int V5RendererPhysicalOffset = 0xF356;
    private const int KnownExtendedPhysicalOffset = 0x3AE60;
    private const int KnownOriginalPhysicalOffset = 0x1A216;
    // Verified packed Elvira 1 RUNVGA font run. The 0x5E up-arrow glyph stores only 7 scanlines,
    // so subsequent glyph data is shifted by one byte in the packed executable.
    private const int PackedOriginalFontPhysicalOffset = 0x15A69;
    private const int PackedShortGlyphCode = 0x5E;
    private const int RunItOriginalPhysicalOffset = 0x168CA;
    private const int RunItHighPhysicalOffset = RunItBootstrapService.HighFontOffset;

    // Original renderer: mov si,28A6h
    private static readonly byte[] OriginalRendererSignature = Convert.FromHexString("BEA628");

    // Active V5 renderer used by FINAL_TEST / HUD_TEST / production derivatives:
    // mov ax,ds ; add ax,234Fh ; mov ds,ax
    private static readonly byte[] V5RendererSignature = Convert.FromHexString("8CD8054F238ED8");

    private static readonly byte[] LowercaseASignature = Convert.FromHexString("0000609090906800");

    public static FontLoadResult LoadRunVga(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("RUNVGA path is empty.", nameof(path));
        if (!File.Exists(path)) throw new FileNotFoundException("RUNVGA.EXE was not found.", path);

        byte[] data = File.ReadAllBytes(path);
        ValidateMz(data);

        // Elvira II RUNIT is deliberately detected before Elvira I signatures. Its 256-slot
        // table is at a different physical address and its renderer has a different patch.
        RunItBootstrapState runItState = RunItBootstrapService.DetectState(path);
        if (runItState == RunItBootstrapState.ExtendedCp852)
        {
            return BuildRunItV2Result(path, data);
        }
        if (runItState == RunItBootstrapState.CanonicalUnpackedAscii98)
        {
            return BuildResult(path, RunVgaFontLayout.OriginalAscii98, RunItOriginalPhysicalOffset, OriginalFirstChar, OriginalGlyphCount, data,
                "Canonical unpacked Elvira II RUNIT detected. Loading the native 98-glyph table from physical 0x168CA.", ElviraGame.Elvira2);
        }
        if (runItState == RunItBootstrapState.OriginalPacked)
        {
            // Opening is read-only: unpack only in memory, validate the canonical profile, then
            // expose the real native LOW glyphs. No O-file or game file is created/changed here.
            byte[] canonical = RunItBootstrapService.UnpackCanonicalOriginal(data);
            return BuildResult(path, RunVgaFontLayout.OriginalPackedAscii98, RunItOriginalPhysicalOffset, OriginalFirstChar, OriginalGlyphCount, canonical,
                "Verified packed Elvira II RUNIT.EXE. The canonical image was unpacked in memory and its real native 98-glyph LOW table was loaded from canonical physical 0x168CA.", ElviraGame.Elvira2);
        }

        // Structure/hash detection is authoritative even when a packed Elvira I build does not
        // expose the optional directly-readable preview run.
        RunVgaBootstrapState runVgaState = RunVgaBootstrapService.DetectState(path);
        if (runVgaState == RunVgaBootstrapState.OriginalPacked)
        {
            // Match the Elvira II packed-preview behavior: reconstruct the exact verified
            // canonical image in memory only. The source file is never written or deployed.
            byte[] canonical = RunVgaBootstrapService.UnpackVerifiedOriginal(data);
            FontLoadResult result = BuildResult(path, RunVgaFontLayout.OriginalPackedAscii98,
                KnownOriginalPhysicalOffset, OriginalFirstChar, OriginalGlyphCount, canonical,
                "Verified packed Elvira I RUNVGA.EXE. The canonical image was unpacked in memory and its real native 98-glyph table was loaded from canonical physical 0x1A216.",
                ElviraGame.Elvira1);
            FontSlotMetadata.RestoreReservedSourceGlyphs(result);
            return result;
        }

        // IMPORTANT: Detect the active renderer first. Patched V5 executables intentionally
        // retain the old 98-glyph table as orphaned data at 0x1A216. Looking for glyph data
        // before checking the renderer therefore produces a false "Original 98-glyph" result.
        if (SignatureAt(data, V5RendererPhysicalOffset, V5RendererSignature))
        {
            ValidateExtendedTableBounds(data, KnownExtendedPhysicalOffset);
            FontLoadResult result = BuildResult(
                path,
                RunVgaFontLayout.ExtendedCp852V5,
                KnownExtendedPhysicalOffset,
                0,
                ExtendedGlyphCount,
                data,
                $"Active V5 CP852 renderer detected at physical 0x{V5RendererPhysicalOffset:X} " +
                $"(signature {Convert.ToHexString(V5RendererSignature)}). " +
                $"Loading the active 256×8 font table from physical 0x{KnownExtendedPhysicalOffset:X}. " +
                "The historical 98-glyph table may still exist in this EXE but is not used by the patched renderer.");
            FontSlotMetadata.RestoreReservedSourceGlyphs(result);
            return result;
        }

        int headerSize = ReadUInt16LE(data, 8) * 16;
        int originalPhysical = headerSize + OriginalFontLoadModuleOffset;

        if (SignatureAt(data, OriginalRendererPhysicalOffset, OriginalRendererSignature) && ValidateOriginalTable(data, originalPhysical))
        {
            return BuildResult(
                path,
                RunVgaFontLayout.OriginalAscii98,
                originalPhysical,
                OriginalFirstChar,
                OriginalGlyphCount,
                data,
                $"Original RUNVGA renderer detected at physical 0x{OriginalRendererPhysicalOffset:X}. " +
                $"Loading the native 98-glyph table (codes 0x20-0x81) from physical 0x{originalPhysical:X}. " +
                "This range contains SPACE, punctuation, digits 0-9, A-Z and a-z. Codes outside 0x20-0x81 are not present as native glyph slots.");
        }

        // Some unpacked original variants move the renderer code while retaining the verified
        // native table at physical 0x1A216. Validate the table itself, but never scan the file
        // for an arbitrary 'a' glyph. V5 was already checked above, so its orphaned table is
        // not selected here when the active V5 renderer signature is present.
        if (ValidateOriginalTable(data, KnownOriginalPhysicalOffset))
        {
            return BuildResult(
                path,
                RunVgaFontLayout.OriginalAscii98,
                KnownOriginalPhysicalOffset,
                OriginalFirstChar,
                OriginalGlyphCount,
                data,
                $"Verified original 98-glyph table found at physical 0x{KnownOriginalPhysicalOffset:X}. " +
                "The exact original renderer signature differs in this build, so detection used the strongly validated table layout. " +
                "Native codes 0x20-0x81 include punctuation, digits 0-9, A-Z and a-z.");
        }

        // Known packed Elvira 1 RUNVGA builds keep the printable /..~ glyph run directly
        // readable in the packed executable. This gives us the real 0-9 / A-Z / a-z glyphs
        // without executing or modifying the DOS self-unpacker. Validate multiple known ASCII
        // glyphs before accepting the layout so an unrelated EXE cannot be misidentified.
        if (ValidatePackedOriginalFont(data))
        {
            return BuildPackedOriginalPreviewResult(path, data);
        }

        string packedHint = data.Length < KnownOriginalPhysicalOffset + OriginalGlyphCount * GlyphBytes
            ? " This executable is smaller than the known unpacked font location and is likely a packed/compressed original RUNVGA. No verified directly-readable packed font run was found in this variant."
            : string.Empty;

        return UnknownResult(path,
            "DOS MZ executable recognized, but no verified supported RUNVGA renderer/font layout was found." + packedHint + " " +
            "The editor intentionally does not scan for an arbitrary 'a' bitmap or assume the last 2048 bytes are a font, because both methods can select orphaned data or appended hooks. " +
            "Writes are disabled for unknown layouts.");
    }

    private static bool ValidatePackedOriginalFont(byte[] data)
    {
        // The packed run needs 639 bytes: 79 normal 8-byte glyphs plus one 7-byte glyph.
        const int packedBytes = (PackedOriginalGlyphCount - 1) * GlyphBytes + 7;
        if (PackedOriginalFontPhysicalOffset < 0 || PackedOriginalFontPhysicalOffset + packedBytes > data.Length)
            return false;

        // Cross-check several glyphs against the verified original unpacked shapes.
        // This is deliberately much stronger than matching one glyph somewhere in the EXE.
        return PackedGlyphEquals(data, 0x41, Convert.FromHexString("2050507050508800")) &&
               PackedGlyphEquals(data, 0x61, Convert.FromHexString("0000609090906800")) &&
               PackedGlyphLooksNonEmpty(data, 0x30) &&
               PackedGlyphLooksNonEmpty(data, 0x39) &&
               PackedGlyphLooksNonEmpty(data, 0x5A) &&
               PackedGlyphLooksNonEmpty(data, 0x7A);
    }

    private static int PackedGlyphOffset(int code)
    {
        if (code < PackedOriginalFirstChar || code > PackedOriginalLastChar) return -1;
        int index = code - PackedOriginalFirstChar;
        int offset = PackedOriginalFontPhysicalOffset + index * GlyphBytes;
        if (code > PackedShortGlyphCode) offset -= 1;
        return offset;
    }

    private static byte[] ReadPackedGlyph(byte[] data, int code)
    {
        int offset = PackedGlyphOffset(code);
        if (offset < 0) return new byte[GlyphBytes];
        byte[] glyph = new byte[GlyphBytes];
        int storedRows = code == PackedShortGlyphCode ? 7 : 8;
        Array.Copy(data, offset, glyph, 0, storedRows);
        return glyph;
    }

    private static bool PackedGlyphEquals(byte[] data, int code, byte[] expected)
        => ReadPackedGlyph(data, code).SequenceEqual(expected);

    private static bool PackedGlyphLooksNonEmpty(byte[] data, int code)
        => ReadPackedGlyph(data, code).Any(b => b != 0);

    private static FontLoadResult BuildPackedOriginalPreviewResult(string path, byte[] data)
    {
        List<GlyphModel> glyphs = GlyphRepository.CreateAllCp852Slots().ToList();
        for (int code = PackedOriginalFirstChar; code <= PackedOriginalLastChar; code++)
            glyphs[code].LoadFromSource(ReadPackedGlyph(data, code));

        return new FontLoadResult
        {
            SourcePath = path,
            Layout = RunVgaFontLayout.OriginalPackedAscii80,
            FontOffset = PackedOriginalFontPhysicalOffset,
            FirstByteValue = PackedOriginalFirstChar,
            LoadedGlyphCount = PackedOriginalGlyphCount,
            Glyphs = glyphs,
            DetectionDetails =
                $"Packed original RUNVGA font run detected at physical 0x{PackedOriginalFontPhysicalOffset:X}. " +
                "Loaded the real 80 printable glyphs 0x2F-0x7E directly from the packed executable, including digits 0-9, A-Z and a-z. " +
                "Glyph 0x5E is stored as 7 scanlines in this packed layout, so the loader compensates for its one-byte shift. " +
                "This is a read/edit working source only: direct Apply to the packed EXE is disabled until packed-runtime writeback is proven."
        };
    }

    public static void SaveCopy(FontLoadResult loaded, string destinationPath)
    {
        if (!loaded.CanApply) throw new InvalidOperationException("Unknown font layout; refusing to create a modified executable.");
        File.Copy(loaded.SourcePath, destinationPath, true);
        WriteEditedGlyphs(loaded, destinationPath);
    }

    public static byte[] ExportEditedFont(FontLoadResult loaded)
    {
        EnsureFullCp852(loaded);
        byte[] result = new byte[ExtendedFontBytes];
        for (int code = 0; code < ExtendedGlyphCount; code++)
        {
            if (!loaded.Glyphs[code].HasEdited)
                throw new InvalidOperationException($"EDITED font is incomplete. Glyph 0x{code:X2} has not been initialized. Use Copy Original → Edited or import a complete 256-glyph font first.");
            byte[] bitmap = FontSlotMetadata.IsReserved(loaded, code)
                ? FontSlotMetadata.GetCanonicalBytes(loaded, code)
                : loaded.Glyphs[code].Edited;
            Array.Copy(bitmap, 0, result, code * GlyphBytes, GlyphBytes);
        }
        return result;
    }

    public static string ExportEditedFont(FontLoadResult loaded, string destinationPath)
    {
        byte[] bytes = ExportEditedFont(loaded);
        File.WriteAllBytes(destinationPath, bytes);
        return Convert.ToHexString(SHA256.HashData(bytes));
    }

    public static string ImportFontIntoEdited(FontLoadResult target, string sourcePath)
    {
        EnsureFullCp852(target);
        if (!File.Exists(sourcePath)) throw new FileNotFoundException("Font import source was not found.", sourcePath);

        byte[] fontBytes;
        string sourceDescription;
        string extension = Path.GetExtension(sourcePath);

        if (extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
        {
            FontLoadResult imported = LoadRunVga(sourcePath);
            EnsureFullCp852(imported);
            fontBytes = ExportEditedFont(imported);
            sourceDescription = $"V5 RUNVGA font from {Path.GetFileName(sourcePath)}";
        }
        else
        {
            fontBytes = File.ReadAllBytes(sourcePath);
            if (fontBytes.Length != ExtendedFontBytes)
                throw new InvalidDataException($"A raw Elvira/DOS 8-row font must contain exactly {ExtendedFontBytes} bytes (256 glyphs × 8 bytes). This file contains {fontBytes.Length} bytes.");
            sourceDescription = extension.Equals(".f08", StringComparison.OrdinalIgnoreCase)
                ? $"{Path.GetFileName(sourcePath)} [F08]"
                : Path.GetFileName(sourcePath);
        }

        return ImportFontBytesIntoEdited(target, fontBytes, sourceDescription);
    }

    public static string ImportFontBytesIntoEdited(FontLoadResult target, byte[] fontBytes, string sourceDescription)
    {
        EnsureFullCp852(target);
        if (fontBytes.Length != ExtendedFontBytes)
            throw new InvalidDataException($"A complete CP852 font must contain exactly {ExtendedFontBytes} bytes (256 glyphs × 8 bytes). This data contains {fontBytes.Length} bytes.");

        for (int code = 0; code < ExtendedGlyphCount; code++)
        {
            if (FontSlotMetadata.IsReserved(target, code))
                target.Glyphs[code].ReplaceEdited(FontSlotMetadata.GetCanonicalBytes(target, code));
            else
                target.Glyphs[code].ReplaceEdited(fontBytes.AsSpan(code * GlyphBytes, GlyphBytes));
        }

        string hash = Convert.ToHexString(SHA256.HashData(fontBytes));
        return $"Imported {ExtendedGlyphCount} glyphs / {fontBytes.Length} bytes into EDITED from {sourceDescription}. SHA-256: {hash}";
    }

    private static void WriteEditedGlyphs(FontLoadResult loaded, string path)
    {
        byte[] data = File.ReadAllBytes(path);
        foreach (GlyphModel glyph in loaded.Glyphs.Where(g => g.IsLoadedFromSource && g.HasEdited))
        {
            int offset;
            if (loaded.Layout == RunVgaFontLayout.ExtendedCp852RunIt)
            {
                if (glyph.ByteValue is < 0x20 or > 0xFF) continue;
                offset = glyph.ByteValue <= 0x81
                    ? RunItOriginalPhysicalOffset + (glyph.ByteValue - 0x20) * GlyphBytes
                    : RunItHighPhysicalOffset + (glyph.ByteValue - 0x82) * GlyphBytes;
            }
            else
            {
                int relativeIndex = glyph.ByteValue - loaded.FirstByteValue;
                if (relativeIndex < 0 || relativeIndex >= loaded.LoadedGlyphCount) continue;
                offset = loaded.FontOffset + relativeIndex * GlyphBytes;
            }
            if (offset < 0 || offset + GlyphBytes > data.Length)
                throw new InvalidDataException($"Glyph 0x{glyph.ByteValue:X2} points outside the executable.");
            byte[] bitmap = FontSlotMetadata.IsReserved(loaded, glyph.ByteValue)
                ? FontSlotMetadata.GetCanonicalBytes(loaded, glyph.ByteValue)
                : glyph.Edited;
            Array.Copy(bitmap, 0, data, offset, GlyphBytes);
        }
        // Preserve/repair reserved slots even if an old editor version, raw import,
        // or externally modified in-memory model supplied other bytes.
        foreach (int code in loaded.ReservedGlyphSlots)
        {
            int offset = loaded.Layout == RunVgaFontLayout.ExtendedCp852RunIt
                ? RunItOriginalPhysicalOffset + (code - OriginalFirstChar) * GlyphBytes
                : loaded.FontOffset + (code - loaded.FirstByteValue) * GlyphBytes;
            if (offset < 0 || offset + GlyphBytes > data.Length)
                throw new InvalidDataException($"Reserved glyph 0x{code:X2} points outside the executable.");
            Array.Copy(FontSlotMetadata.GetCanonicalBytes(loaded, code), 0, data, offset, GlyphBytes);
        }
        File.WriteAllBytes(path, data);
    }

    private static FontLoadResult BuildResult(string path, RunVgaFontLayout layout, int fontOffset, int firstByteValue, int glyphCount, byte[] data, string details, ElviraGame game = ElviraGame.Elvira1)
    {
        List<GlyphModel> glyphs = GlyphRepository.CreateAllCp852Slots().ToList();
        for (int i = 0; i < glyphCount; i++)
        {
            int value = firstByteValue + i;
            if (value < 0 || value > 255) continue;
            int sourceOffset = fontOffset + i * GlyphBytes;
            if (sourceOffset < 0 || sourceOffset + GlyphBytes > data.Length)
                throw new InvalidDataException("Font table extends beyond RUNVGA.EXE.");
            glyphs[value].LoadFromSource(data.AsSpan(sourceOffset, GlyphBytes).ToArray());
        }
        return new FontLoadResult { SourcePath = path, Layout = layout, FontOffset = fontOffset, FirstByteValue = firstByteValue, LastByteValue = firstByteValue + glyphCount - 1, LoadedGlyphCount = glyphCount, Glyphs = glyphs, DetectionDetails = details, Game = game };
    }

    private static FontLoadResult BuildRunItV2Result(string path, byte[] data)
    {
        List<GlyphModel> glyphs = GlyphRepository.CreateAllCp852Slots().ToList();
        for (int value = 0x20; value <= 0x81; value++)
            glyphs[value].LoadFromSource(data.AsSpan(RunItOriginalPhysicalOffset + (value - 0x20) * GlyphBytes, GlyphBytes).ToArray());
        // Do not trust a pre-v1.3.1 user-patched active EXE: the model always presents the
        // canonical engine erase mask and every save path repairs it.
        glyphs[RunItBootstrapService.ReservedHudEraseGlyph].LoadFromSource(RunItBootstrapService.ReservedHudEraseGlyphBytes);
        for (int value = 0x82; value <= 0xFF; value++)
            glyphs[value].LoadFromSource(data.AsSpan(RunItHighPhysicalOffset + (value - 0x82) * GlyphBytes, GlyphBytes).ToArray());
        return new FontLoadResult
        {
            SourcePath = path, Layout = RunVgaFontLayout.ExtendedCp852RunIt, FontOffset = RunItOriginalPhysicalOffset,
            FirstByteValue = 0x20, LastByteValue = 0xFF, LoadedGlyphCount = 224, Glyphs = glyphs, Game = ElviraGame.Elvira2,
            DetectionDetails = "Elvira II V2 split-font renderer detected: LOW 0x20-0x81 at 0x168CA; HIGH 0x82-0xFF at 0x28480; helper at 0x28870."
        };
    }

    private static FontLoadResult UnknownResult(string path, string details) => new()
    {
        SourcePath = path,
        Layout = RunVgaFontLayout.Unknown,
        FontOffset = -1,
        FirstByteValue = 0,
        LoadedGlyphCount = 0,
        Glyphs = GlyphRepository.CreateAllCp852Slots().ToList(),
        DetectionDetails = details
    };

    private static void ValidateMz(byte[] data)
    {
        if (data.Length < 64 || data[0] != (byte)'M' || data[1] != (byte)'Z')
            throw new InvalidDataException("The selected file is not a DOS MZ executable.");
    }

    private static void ValidateExtendedTableBounds(byte[] data, int offset)
    {
        if (offset < 0 || offset + ExtendedFontBytes > data.Length)
            throw new InvalidDataException($"V5 renderer signature was found, but the expected 2048-byte table at 0x{offset:X} lies outside the executable.");
    }

    private static void EnsureFullCp852(FontLoadResult loaded)
    {
        if (!loaded.HasFullCp852Font)
            throw new InvalidOperationException("Import/export of standalone fonts requires a loaded V5 extended CP852 RUNVGA with all 256 glyphs. Original 98-glyph RUNVGA files can still be edited in-place, but they do not contain a complete 2048-byte CP852 font asset.");
    }

    internal static bool IsReservedHudGlyph(FontLoadResult? loaded, int code) =>
        FontSlotMetadata.IsReserved(loaded, code);

    private static bool ValidateOriginalTable(byte[] data, int offset)
    {
        if (offset < 0 || offset + OriginalGlyphCount * GlyphBytes > data.Length) return false;
        for (int i = 0; i < 8; i++) if (data[offset + i] != 0) return false;
        int aOffset = offset + (97 - OriginalFirstChar) * GlyphBytes;
        if (!SignatureAt(data, aOffset, LowercaseASignature)) return false;
        int after = offset + OriginalGlyphCount * GlyphBytes;
        int len = Math.Min(96, data.Length - after);
        if (len <= 0) return false;
        string tail = Encoding.ASCII.GetString(data, after, len);
        return tail.Contains("Press any key to continue", StringComparison.OrdinalIgnoreCase);
    }

    private static int ReadUInt16LE(byte[] data, int offset) => data[offset] | (data[offset + 1] << 8);
    private static bool SignatureAt(byte[] data, int offset, byte[] sig) => offset >= 0 && offset + sig.Length <= data.Length && data.AsSpan(offset, sig.Length).SequenceEqual(sig);

}
