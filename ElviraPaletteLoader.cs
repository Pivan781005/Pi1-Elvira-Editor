using System.Buffers.Binary;
using System.Drawing;

namespace Pi1ElviraEditor;

internal sealed record ElviraPaletteBank(int Index, Color[] Colors)
{
    public override string ToString() => $"{UiText.Get("PaletteWord")} {Index:D3}";
}

internal static class ElviraPaletteLoader
{
    // Elvira 1 DOS vc22_setPalette: when palette id < 1000,
    // only colors 0..12 come from xNN1.VGA. Colors 13..31 are fixed.
    private static readonly byte[] ExtraColors =
    {
        40,  0,  0,   24, 24, 16,   48, 48, 40,
         0,  0,  0,   16,  0,  0,    8,  8,  0,
        48, 24,  0,   56, 40,  0,    0,  0, 24,
         8, 16, 24,   24, 32, 40,   16, 24,  0,
        24,  8,  0,   16, 16,  0,   40, 40, 32,
        32, 32, 24,   40,  0,  0,   24, 24, 16,
        48, 48, 40
    };

    public static List<ElviraPaletteBank> Load(string vga1Path)
    {
        byte[] data = File.ReadAllBytes(vga1Path);
        if (data.Length < 12)
            throw new InvalidDataException($"{Path.GetFileName(vga1Path)} je príliš malý.");

        // xNN1.VGA stores the actual palette-bank count in its BE header at +0x02.
        // Bytes after the declared table belong to other VGA resource structures;
        // they are not additional 0x20-byte palette banks.
        int count = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(2, 2));
        int paletteBase = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(6, 2));
        if (count <= 0 || count > 1000 || paletteBase < 0 || (long)paletteBase + (long)count * 32 > data.Length)
            throw new InvalidDataException(
                $"Neplatná palette table/count hlavička v {Path.GetFileName(vga1Path)} (count={count}, offset=0x{paletteBase:X}).");

        var result = new List<ElviraPaletteBank>();

        for (int bank = 0; bank < count; bank++)
        {
            int p = paletteBase + bank * 32;
            if (p + 32 > data.Length)
                break;

            Color[] colors = new Color[16];

            // First 13 colors are zone palette values, 12-bit RGB packed as 0xRGB.
            for (int i = 0; i < 13; i++)
            {
                ushort c = BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(p + i * 2, 2));
                int r = ((c & 0x0F00) >> 8) * 32;
                int g = ((c & 0x00F0) >> 4) * 32;
                int b = ((c & 0x000F) >> 0) * 32;

                // Original data normally uses 0..7 component values.
                colors[i] = Color.FromArgb(255, ClampByte(r), ClampByte(g), ClampByte(b));
            }

            // Colors 13..15 are the first three fixed Elvira extra colors.
            for (int i = 13; i < 16; i++)
            {
                int extra = (i - 13) * 3;
                colors[i] = Color.FromArgb(
                    255,
                    ClampByte(ExtraColors[extra + 0] * 4),
                    ClampByte(ExtraColors[extra + 1] * 4),
                    ClampByte(ExtraColors[extra + 2] * 4));
            }

            result.Add(new ElviraPaletteBank(bank, colors));
        }

        return result;
    }

    public static Color[] DiagnosticPalette()
        => new[]
        {
            Color.FromArgb(0, 0, 0, 0),
            Color.FromArgb(255, 0, 0, 170),
            Color.FromArgb(255, 0, 170, 0),
            Color.FromArgb(255, 0, 170, 170),
            Color.FromArgb(255, 170, 0, 0),
            Color.FromArgb(255, 170, 0, 170),
            Color.FromArgb(255, 170, 85, 0),
            Color.FromArgb(255, 170, 170, 170),
            Color.FromArgb(255, 85, 85, 85),
            Color.FromArgb(255, 85, 85, 255),
            Color.FromArgb(255, 85, 255, 85),
            Color.FromArgb(255, 85, 255, 255),
            Color.FromArgb(255, 255, 85, 85),
            Color.FromArgb(255, 255, 85, 255),
            Color.FromArgb(255, 255, 255, 85),
            Color.FromArgb(255, 255, 255, 255)
        };

    private static int ClampByte(int v) => Math.Max(0, Math.Min(255, v));
}
