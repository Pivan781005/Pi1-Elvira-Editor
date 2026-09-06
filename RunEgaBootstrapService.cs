using System.Security.Cryptography;

namespace ElviraVgaEditor;

/// <summary>
/// Frozen R4 RUNEGA image builder.  It is deliberately path-only: callers own
/// deployment and this type never creates a backup or discovers a game root.
/// </summary>
internal static class RunEgaBootstrapService
{
    internal const int PackedSize = 0x198EF, CanonicalSize = 0x25CD0, OutputSize = 0x33C00;
    internal const string PackedSha256 = "FE596D7DB1CEFB643F2C2BEC1EC5342CC1F7DBA2F47AFE3512B6C6FF8DE05EA1";
    internal const string CanonicalSha256 = "A15243583A1774BAF8F77DF599063675ED9EF36A573A35CF6F2504705B3BAB56";
    private const int Header = 0x2400, Font = 0x32400, Ui = 0x33400;

    internal static bool IsPacked(byte[] image) => image.Length == PackedSize && Hash(image) == PackedSha256;
    internal static bool IsCanonical(byte[] image) => image.Length == CanonicalSize && Hash(image) == CanonicalSha256;

    internal static string CreateFrozenCp852(string sourcePath, string destinationPath, IReadOnlyList<GlyphModel> glyphs)
    {
        // R9D: a missing source is a different result from an unsupported one.
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("RUNEGA source is missing; bootstrap was blocked. No files were changed.", sourcePath);
        if (Path.GetFullPath(sourcePath).Equals(Path.GetFullPath(destinationPath), StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("RUNEGA bootstrap never overwrites its source.");
        if (File.Exists(destinationPath)) throw new IOException("RUNEGA destination already exists.");
        byte[] source = File.ReadAllBytes(sourcePath);
        if (IsPacked(source)) source = CanonicalizePacked(source);
        else if (!IsCanonical(source)) throw new InvalidDataException("Unsupported RUNEGA source fingerprint.");
        ValidateCanonical(source);
        byte[] output = Build(source, glyphs);
        string dir = Path.GetDirectoryName(Path.GetFullPath(destinationPath)) ?? throw new IOException("Output directory unavailable.");
        Directory.CreateDirectory(dir);
        string tmp = Path.Combine(dir, "." + Path.GetFileName(destinationPath) + "." + Guid.NewGuid().ToString("N") + ".tmp");
        try { File.WriteAllBytes(tmp, output); File.Move(tmp, destinationPath); }
        finally { if (File.Exists(tmp)) File.Delete(tmp); }
        ValidateOutput(File.ReadAllBytes(destinationPath));
        return Hash(output);
    }

    internal static byte[] CanonicalizePacked(byte[] packed)
    {
        byte[] canonical = ExepackCanonicalizer.Unpack(packed, PackedSize, PackedSha256, "RUNEGA", image => { W16(image, 0x0A, 0x0195); W16(image, 0x12, 0x627A); });
        if (!IsCanonical(canonical)) throw new InvalidDataException($"RUNEGA EXEPACK canonical identity mismatch: {canonical.Length} bytes SHA-256 {Hash(canonical)}, SS={U16(canonical,0x0E):X4}, minalloc={U16(canonical,0x0A):X4}.");
        ValidateCanonical(canonical);
        return canonical;
    }

    internal static byte[] Build(byte[] canonical, IReadOnlyList<GlyphModel> glyphs)
    {
        ValidateCanonical(canonical);
        if (glyphs.Count != 256) throw new InvalidDataException("RUNEGA needs 256 CP852 slots.");
        var image = new byte[OutputSize];
        Array.Copy(canonical, image, canonical.Length);
        // R4 preserves the original BSS/workspace through module 2481F; the rejected
        // 30800 bank stays zero and is never used.  Font/UI banks are fixed profile data.
        byte[] native = canonical[0x1AE4A..0x1B15A];
        for (int code = 0x20; code <= 0x81; code++) Array.Copy(native, (code - 0x20) * 8, image, Font + code * 8, 8);
        foreach (GlyphModel g in glyphs)
        {
            if (g.ByteValue <= 0x1F || g.ByteValue == FontSlotMetadata.HudEraseGlyph || (!g.HasEdited && !g.HasKnownFallbackBitmap)) continue;
            Array.Copy(g.HasEdited ? g.Edited : g.Original, 0, image, Font + g.ByteValue * 8, 8);
        }
        Array.Copy(FontSlotMetadata.OriginalHudEraseGlyphBytes, 0, image, Font + FontSlotMetadata.HudEraseGlyph * 8, 8);
        // R4 moves the initial stack beyond the retained font image.
        W16(image, 0x0E, 0x2502);
        // The accepted persistent-font patch is only the relocated segment operand.
        Require(image, 0xFA4A, Convert.FromHexString("80EA20D1E2D1E2D1E2BF0A29"), "RUNEGA renderer prefix");
        Convert.FromHexString("B103D3E28BFAB800308ED883").CopyTo(image, 0xFA4A);
        W16(image, 0xFA51, 0x3000);
        BuildRuntimeUi(canonical, image);
        W16(image, 2, 0); W16(image, 4, (ushort)(OutputSize / 512));
        ValidateOutput(image);
        return image;
    }

    private static void BuildRuntimeUi(byte[] source, byte[] image)
    {
        // Final R4 descriptor order, using fixed source offsets rather than English scanning.
        (int Start, int Site, int End, ushort Selector)[] r =
        [ (0x1B261,0xCDD8,0x2D48,0xF000),(0x1B288,0xCE0A,0x2D70,0xF001),
          (0x1B2B0,0xCBFC,0x2DA3,0xF002),(0x1B2E4,0xCD03,0x2DB6,0xF003),
          (0x1B2F6,0xCD1A,0x2DC8,0xF004),(0x1B308,0xCD1D,0x2DDB,0xF005),
          (0x1B31C,0xCD06,0x2DF1,0xF006),(0x1B332,0xCCDD,0x2E30,0xF007) ];
        FrozenRuntimeUiDescriptor d = Elvira1ProductionProfile.RunEga.RuntimeUi;
        for (int i=0;i<r.Length;i++)
        {
            FrozenRuntimeUiRecord frozen = d.RuntimeRecords[i];
            if (frozen.Selector != r[i].Selector || frozen.PointerSitePhysicalOffset != r[i].Site) throw new InvalidDataException("Frozen RUNEGA UI descriptor mismatch.");
            int end=r[i].Start; while (source[end] != 0) end++; end++;
            if (end-r[i].Start > frozen.Length) throw new InvalidDataException("Frozen RUNEGA UI record exceeds its bank descriptor.");
            // The source English PAUSE record is three bytes shorter than the frozen
            // production envelope.  Preserve its bytes and its NUL; unused envelope
            // bytes remain zero.  A later project text stage supplies a translated
            // payload of the descriptor's full length before this low-level builder runs.
            Array.Copy(source,r[i].Start,image,Ui+frozen.BankOffset,end-r[i].Start);
            if (image[r[i].Site] != frozen.PointerOpcode) throw new InvalidDataException("Frozen RUNEGA UI pointer opcode mismatch.");
            W16(image,r[i].Site+1,r[i].Selector);
        }
        // The R4 common bridge, including its one new relocation word.
        Convert.FromHexString("BBBE0253EABF02290D").CopyTo(image, Ui + d.BridgeOffset);
        AddRelocation(image, (ushort)(d.BridgeOffset + 7), 0x3100);
    }

    internal static void ValidateCanonical(byte[] image)
    {
        if (!IsCanonical(image) || !IsMz(image) || U16(image,8)!=0x240 || U16(image,6)!=0x8E1 || U16(image,0x0E)!=0x2482 || U16(image,0x10)!=0x0A00)
            throw new InvalidDataException("RUNEGA canonical frozen identity/MZ structure failed.");
    }
    internal static void ValidateOutput(byte[] image)
    {
        Elvira1ProductionProfile.VerifyFrozenInvariants();
        FrozenExecutableDescriptor p=Elvira1ProductionProfile.RunEga; FrozenRuntimeUiDescriptor ui=p.RuntimeUi;
        if (image.Length!=OutputSize || !IsMz(image) || U16(image,2)!=0 || U16(image,4)!=OutputSize/512 || U16(image,0xFA51)!=0x3000)
            throw new InvalidDataException("RUNEGA output MZ/font layout failed.");
        if (!image.AsSpan(Font+FontSlotMetadata.HudEraseGlyph*8,8).SequenceEqual(p.Font.HudBytes!) ||
            !image.AsSpan(Ui+ui.BridgeOffset,9).SequenceEqual(Convert.FromHexString("BBBE0253EABF02290D")) ||
            !HasRelocation(image, Ui+ui.BridgeRelocationOffset)) throw new InvalidDataException("RUNEGA output frozen bank/bridge validation failed.");
        foreach(FrozenRuntimeUiRecord r in ui.RuntimeRecords)
            if (U16(image,r.PointerSitePhysicalOffset+1)!=r.Selector || !image.AsSpan(Ui+r.BankOffset,r.Length).Contains((byte)0)) throw new InvalidDataException("RUNEGA UI descriptor output mismatch.");
    }
    private static void AddRelocation(byte[] b,ushort off,ushort seg){int n=U16(b,6), at=U16(b,0x18)+n*4;if(at+4>U16(b,8)*16)throw new InvalidDataException("RUNEGA relocation table is full.");W16(b,at,off);W16(b,at+2,seg);W16(b,6,(ushort)(n+1));}
    private static bool HasRelocation(byte[] b,int physical){int h=U16(b,8)*16,n=U16(b,6),t=U16(b,0x18);for(int i=0;i<n;i++){int e=t+i*4;if(h+U16(b,e+2)*16+U16(b,e)==physical)return true;}return false;}
    private static void Require(byte[] b,int at,byte[] expected,string n){if(!b.AsSpan(at,expected.Length).SequenceEqual(expected))throw new InvalidDataException("Unexpected "+n+".");}
    private static bool IsMz(byte[] b)=>b.Length>=28&&b[0]=='M'&&b[1]=='Z'; private static ushort U16(byte[] b,int o)=>(ushort)(b[o]|b[o+1]<<8); private static void W16(byte[] b,int o,ushort v){b[o]=(byte)v;b[o+1]=(byte)(v>>8);} private static string Hash(byte[] b)=>Convert.ToHexString(SHA256.HashData(b));
}
