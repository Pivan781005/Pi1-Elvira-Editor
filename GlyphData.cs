using System.Text;

namespace Pi1ElviraEditor;

internal sealed class GlyphModel
{
    public int ByteValue { get; }
    public string Character { get; }
    public string Description { get; }
    public byte[] Original { get; private set; } = new byte[8];
    public byte[] Edited => RenderEditedClipped();
    public bool IsLoadedFromSource { get; private set; }
    public bool HasKnownFallbackBitmap { get; private set; }
    public bool HasEdited { get; private set; }
    public int ShiftX { get; private set; }
    public int ShiftY { get; private set; }

    // EDITED is kept as an unbounded logical pixel set. Arrow moves only change ShiftX/ShiftY,
    // so pixels are never destroyed while positioning the glyph. The 8x8 bitmap is clipped
    // only when the current view is rendered for Apply / Save copy / Export.
    private readonly HashSet<(int X, int Y)> _editedPixels = new();

    public GlyphModel(int byteValue, string character, string description)
    {
        ByteValue = byteValue;
        Character = character;
        Description = description;
    }

    public void LoadFromSource(byte[] bitmap)
    {
        if (bitmap.Length != 8) throw new ArgumentException("Elvira glyphs must contain exactly 8 bytes.", nameof(bitmap));
        Original = bitmap.ToArray();
        ClearEdited();
        IsLoadedFromSource = true;
    }

    public void LoadFallback(byte[] bitmap)
    {
        if (bitmap.Length != 8) throw new ArgumentException("Elvira glyphs must contain exactly 8 bytes.", nameof(bitmap));
        Original = bitmap.ToArray();
        ClearEdited();
        HasKnownFallbackBitmap = true;
    }

    public void ReplaceEdited(ReadOnlySpan<byte> bitmap)
    {
        if (bitmap.Length != 8) throw new ArgumentException("Elvira glyphs must contain exactly 8 bytes.", nameof(bitmap));
        // 0x81 is an engine-owned HUD erase glyph, not an editable CP852
        // character. Keep this guard below the UI level so import, copy,
        // reset and any future model caller cannot introduce unsafe bytes.
        // Layout-aware callers must use ReplaceReservedCanonical with the
        // FontSlotMetadata canonical bytes for their loaded layout instead:
        // the default below preserves legacy V5/RUNIT fail-closed behavior.
        if (ByteValue == FontSlotMetadata.HudEraseGlyph)
            bitmap = FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes;
        _editedPixels.Clear();
        ShiftX = 0;
        ShiftY = 0;
        AddBitmapPixels(bitmap, _editedPixels);
        HasEdited = true;
    }

    /// <summary>Layout-aware reserved-slot canonicalization (copy/import/
    /// initialization paths that know the loaded layout). The canonical bytes
    /// must come from FontSlotMetadata.GetCanonicalBytes: patched full-cell
    /// for RUNVGA V5 / RUNIT V2, the frozen original mask for RUNEGA.</summary>
    public void ReplaceReservedCanonical(byte[] canonical)
    {
        if (ByteValue != FontSlotMetadata.HudEraseGlyph)
            throw new InvalidOperationException($"Glyph 0x{ByteValue:X2} is not reserved.");
        if (canonical.Length != 8) throw new ArgumentException("Elvira glyphs must contain exactly 8 bytes.", nameof(canonical));
        _editedPixels.Clear();
        ShiftX = 0;
        ShiftY = 0;
        AddBitmapPixels(canonical, _editedPixels);
        HasEdited = true;
    }

    public void CopyOriginalToEdited()
    {
        ReplaceEdited(Original);
    }

    public void ClearEdited()
    {
        _editedPixels.Clear();
        ShiftX = 0;
        ShiftY = 0;
        HasEdited = false;
    }

    /// <summary>Explicit empty working copy: an 8-byte zero bitmap with
    /// HasEdited set. Distinct from ClearEdited (no working copy): the UI
    /// shows real zero HEX instead of [EMPTY], and later pixel edits modify
    /// the zero bitmap rather than re-cloning Original. Defense in depth:
    /// reserved engine glyphs fail closed here even if a future caller
    /// bypasses the UI gates, using the same authoritative reservation
    /// metadata as the rest of the font system.</summary>
    public void ClearToZero()
    {
        if (ByteValue == FontSlotMetadata.HudEraseGlyph)
            throw new InvalidOperationException("Glyph 0x81 is engine-reserved and cannot be cleared to zero.");
        _editedPixels.Clear();
        ShiftX = 0;
        ShiftY = 0;
        HasEdited = true;
    }

    public void Reset() => CopyOriginalToEdited();

    public void ShiftEdited(int dx, int dy)
    {
        if (!HasEdited || ByteValue == FontSlotMetadata.HudEraseGlyph) return;
        checked
        {
            ShiftX += dx;
            ShiftY += dy;
        }
    }

    public void ResetShift()
    {
        if (ByteValue == FontSlotMetadata.HudEraseGlyph) return;
        ShiftX = 0;
        ShiftY = 0;
    }

    public void ToggleEditedPixel(int viewRow, int viewColumn)
    {
        if (!HasEdited || ByteValue == FontSlotMetadata.HudEraseGlyph) return;
        int logicalX = viewColumn - ShiftX;
        int logicalY = viewRow - ShiftY;
        var p = (logicalX, logicalY);
        if (!_editedPixels.Add(p))
            _editedPixels.Remove(p);
    }

    public int OutsidePixelCount
    {
        get
        {
            if (!HasEdited) return 0;
            int count = 0;
            foreach (var p in _editedPixels)
            {
                int x = p.X + ShiftX;
                int y = p.Y + ShiftY;
                if (x < 0 || x > 7 || y < 0 || y > 7)
                    count++;
            }
            return count;
        }
    }

    public bool HasPixelsOutside8x8 => OutsidePixelCount > 0;

    public bool IsModified => HasEdited && !Original.SequenceEqual(RenderEditedClipped());

    private byte[] RenderEditedClipped()
    {
        var rows = new byte[8];
        if (!HasEdited) return rows;

        foreach (var p in _editedPixels)
        {
            int x = p.X + ShiftX;
            int y = p.Y + ShiftY;
            if (x < 0 || x > 7 || y < 0 || y > 7)
                continue;
            rows[y] |= (byte)(1 << (7 - x));
        }
        return rows;
    }

    private static void AddBitmapPixels(ReadOnlySpan<byte> bitmap, HashSet<(int X, int Y)> target)
    {
        for (int row = 0; row < 8; row++)
        {
            byte value = bitmap[row];
            for (int col = 0; col < 8; col++)
            {
                if ((value & (1 << (7 - col))) != 0)
                    target.Add((col, row));
            }
        }
    }
}

internal static class GlyphRepository
{
    public static IReadOnlyList<GlyphModel> CreateAllCp852Slots()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding enc = Encoding.GetEncoding(852);
        var known = KnownSlovakGlyphs();
        var result = new List<GlyphModel>(256);

        for (int value = 0; value <= 255; value++)
        {
            string ch = enc.GetString(new[] { (byte)value });
            string display = IsPrintable(ch) ? ch : "·";
            var model = new GlyphModel(value, display, Describe(value, ch));
            if (known.TryGetValue(ch, out var bytes)) model.LoadFallback(bytes);
            result.Add(model);
        }
        return result;
    }


    public static string GetSlotDisplayText(int value)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding enc = Encoding.GetEncoding(852);
        string ch = enc.GetString(new[] { (byte)value });
        if (value == 32) return "[SPACE]";
        if (value < 32 || value == 127) return $"[0x{value:X2}]";
        if (ch.Length == 0 || char.IsControl(ch[0])) return $"[0x{value:X2}]";
        return $"'{ch}'";
    }

    private static bool IsPrintable(string ch) => ch.Length > 0 && !char.IsControl(ch[0]);

    private static string Describe(int value, string ch)
    {
        if (value < 32 || value == 127) return "Control / non-printable";
        if (char.IsLetterOrDigit(ch.FirstOrDefault())) return "Letter / digit";
        if (char.IsPunctuation(ch.FirstOrDefault())) return "Punctuation";
        if (char.IsSymbol(ch.FirstOrDefault())) return "Symbol";
        if (char.IsWhiteSpace(ch.FirstOrDefault())) return "Whitespace";
        return "CP852 slot";
    }

    private static Dictionary<string, byte[]> KnownSlovakGlyphs()
    {
        var hex = new Dictionary<string, string>
        {
            ["á"]="0810609090906800", ["Á"]="1020505070505088", ["ä"]="5000609090906800", ["Ä"]="5020505070505088",
            ["č"]="2810304840483000", ["Č"]="2830488080804830", ["ď"]="1014709090906800", ["Ď"]="50f04848484848f0",
            ["é"]="1020704870403800", ["É"]="10f84040704040f8", ["í"]="1020602020283000", ["Í"]="1070202020202070",
            ["ĺ"]="5060404040506000", ["Ĺ"]="10e04040404048f8", ["ľ"]="4060504040506000", ["Ľ"]="50e04040404048f8",
            ["ň"]="5020d86848484800", ["Ň"]="50c84868585848c8", ["ó"]="0810304848483000", ["Ó"]="1070888888888870",
            ["ô"]="2050304848483000", ["Ô"]="2070888888888870", ["ŕ"]="1020d8684040e000", ["Ŕ"]="10f04848705048c8",
            ["š"]="2810384030087000", ["Š"]="5070888060108870", ["ť"]="2028702020283000", ["Ť"]="50f8a82020202070",
            ["ú"]="102098909090e800", ["Ú"]="1088505050505020", ["ý"]="1020989090701060", ["Ý"]="10884830204080c0",
            ["ž"]="2810781020407800", ["Ž"]="50f88810204080f8"
        };
        return hex.ToDictionary(k => k.Key, v => HexToBytes(v.Value));
    }

    private static byte[] HexToBytes(string hex)
    {
        var bytes = new byte[8];
        for (int i = 0; i < 8; i++) bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }
}
