using System.Drawing;
using System.Drawing.Imaging;

namespace ElviraVgaEditor;

internal static class PaletteTools
{
    // Diagnostic palette is kept only for legacy PNG round-trip compatibility.
    public static readonly Color[] DiagnosticPalette = ElviraPaletteLoader.DiagnosticPalette();

    public static Bitmap ToBitmap(int width, int height, byte[] pixels, Color[] palette, bool transparentZero = true)
    {
        if (palette.Length < 16)
            throw new ArgumentException("Paleta musí mať aspoň 16 farieb.");

        var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            int idx = pixels[y * width + x] & 0x0F;
            Color c = palette[idx];

            // In Elvira's normal sprite drawing, color index 0 is transparent
            // unless the draw flags request non-transparent rendering.
            if (idx == 0 && transparentZero)
                c = Color.FromArgb(0, c.R, c.G, c.B);

            bmp.SetPixel(x, y, c);
        }

        return bmp;
    }

    public static byte[] ReadIndices(string pngPath, int expectedWidth, int expectedHeight)
    {
        using var bmp = new Bitmap(pngPath);
        if (bmp.Width != expectedWidth || bmp.Height != expectedHeight)
            throw new InvalidDataException(
                $"PNG má {bmp.Width}x{bmp.Height}, očakáva sa {expectedWidth}x{expectedHeight}.");

        byte[] result = new byte[checked(expectedWidth * expectedHeight)];

        for (int y = 0; y < expectedHeight; y++)
        for (int x = 0; x < expectedWidth; x++)
        {
            Color c = bmp.GetPixel(x, y);

            if (c.A < 128)
            {
                result[y * expectedWidth + x] = 0;
                continue;
            }

            int idx = FindExact(DiagnosticPalette, c);
            if (idx < 0)
                throw new InvalidDataException(
                    $"Neznáma farba na ({x},{y}): #{c.R:X2}{c.G:X2}{c.B:X2}. " +
                    "Replacement PNG zatiaľ musí používať diagnostickú 16-farebnú paletu.");

            result[y * expectedWidth + x] = (byte)idx;
        }

        return result;
    }

    private static int FindExact(Color[] palette, Color c)
    {
        for (int i = 1; i < palette.Length; i++)
        {
            Color p = palette[i];
            if (c.R == p.R && c.G == p.G && c.B == p.B)
                return i;
        }
        return -1;
    }
}
