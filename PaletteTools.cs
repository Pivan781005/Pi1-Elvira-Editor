using System.Drawing;
using System.Drawing.Imaging;

namespace Pi1ElviraEditor;

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
        => ReadIndices(pngPath, expectedWidth, expectedHeight, DiagnosticPalette);

    public static byte[] ReadIndices(string pngPath, int expectedWidth, int expectedHeight, IReadOnlyList<Color> palette)
    {
        ReplacePngValidationResult validation = ReplacePngValidator.Validate(pngPath, expectedWidth, expectedHeight, "palette", palette);
        if (validation.IsValid && validation.Indices is not null) return validation.Indices;

        throw new InvalidDataException(validation.FailureKind switch
        {
            ReplacePngValidationFailureKind.DimensionMismatch => $"PNG dimensions {validation.ActualWidth}x{validation.ActualHeight} do not match expected {expectedWidth}x{expectedHeight}.",
            ReplacePngValidationFailureKind.PaletteMismatch => $"PNG contains {validation.InvalidPixelCount} pixels outside the active palette.",
            ReplacePngValidationFailureKind.PaletteUnavailable => "The active palette is unavailable.",
            _ => "The selected file is not a supported PNG image."
        });
    }
}
