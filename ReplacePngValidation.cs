using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ElviraVgaEditor;

internal enum ReplacePngValidationFailureKind
{
    None,
    InvalidImage,
    DimensionMismatch,
    PaletteUnavailable,
    PaletteMismatch
}

internal sealed record PaletteValidationEntry(int Index, Color Color)
{
    public string Hex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
    public string Rgb => $"{Color.R}, {Color.G}, {Color.B}";
}

internal sealed record InvalidPngColor(Color Color, int PixelCount, Point FirstOccurrence)
{
    public string Hex => $"#{Color.R:X2}{Color.G:X2}{Color.B:X2}";
    public string Rgb => $"{Color.R}, {Color.G}, {Color.B}";
}

/// <summary>Complete, non-mutating validation result for a candidate replacement PNG.</summary>
internal sealed record ReplacePngValidationResult(
    ReplacePngValidationFailureKind FailureKind,
    int ExpectedWidth,
    int ExpectedHeight,
    int? ActualWidth,
    int? ActualHeight,
    string PaletteIdentity,
    IReadOnlyList<PaletteValidationEntry> PaletteEntries,
    IReadOnlyList<InvalidPngColor> InvalidColors,
    int InvalidPixelCount,
    byte[]? Indices)
{
    public bool IsValid => FailureKind == ReplacePngValidationFailureKind.None;
}

/// <summary>Strict, exact RGB validator for Graphics -&gt; Replace PNG.  It performs
/// no palette selection, conversion, quantization, or edit-state mutation.</summary>
internal static class ReplacePngValidator
{
    public static ReplacePngValidationResult Validate(
        string pngPath,
        int expectedWidth,
        int expectedHeight,
        string paletteIdentity,
        IReadOnlyList<Color>? activePalette)
    {
        IReadOnlyList<PaletteValidationEntry> entries = CreatePaletteEntries(activePalette);
        if (activePalette is null || activePalette.Count < 16)
            return new(ReplacePngValidationFailureKind.PaletteUnavailable, expectedWidth, expectedHeight, null, null,
                paletteIdentity, entries, Array.Empty<InvalidPngColor>(), 0, null);

        try
        {
            using var bitmap = new Bitmap(pngPath);
            if (bitmap.RawFormat.Guid != ImageFormat.Png.Guid)
                return new(ReplacePngValidationFailureKind.InvalidImage, expectedWidth, expectedHeight, null, null,
                    paletteIdentity, entries, Array.Empty<InvalidPngColor>(), 0, null);

            if (bitmap.Width != expectedWidth || bitmap.Height != expectedHeight)
                return new(ReplacePngValidationFailureKind.DimensionMismatch, expectedWidth, expectedHeight, bitmap.Width, bitmap.Height,
                    paletteIdentity, entries, Array.Empty<InvalidPngColor>(), 0, null);

            var indices = new byte[checked(expectedWidth * expectedHeight)];
            var invalid = new Dictionary<(byte R, byte G, byte B), InvalidColorAccumulator>();

            for (int y = 0; y < expectedHeight; y++)
            for (int x = 0; x < expectedWidth; x++)
            {
                Color color = bitmap.GetPixel(x, y);

                // Preserve the former conversion semantics: pixels with alpha below
                // 128 use engine index 0; otherwise RGB must match a usable palette
                // color exactly.  Duplicate RGB entries deterministically use the
                // first matching non-transparent index.
                if (color.A < 128)
                {
                    indices[y * expectedWidth + x] = 0;
                    continue;
                }

                int index = FindExact(activePalette, color);
                if (index >= 0)
                {
                    indices[y * expectedWidth + x] = (byte)index;
                    continue;
                }

                var key = (color.R, color.G, color.B);
                if (!invalid.TryGetValue(key, out InvalidColorAccumulator? accumulator))
                    invalid.Add(key, accumulator = new InvalidColorAccumulator(color, new Point(x, y)));
                accumulator.PixelCount++;
            }

            IReadOnlyList<InvalidPngColor> invalidColors = invalid.Values
                .Select(value => new InvalidPngColor(value.Color, value.PixelCount, value.FirstOccurrence))
                .OrderByDescending(value => value.PixelCount)
                .ThenBy(value => value.Hex, StringComparer.Ordinal)
                .ToArray();

            return invalidColors.Count == 0
                ? new(ReplacePngValidationFailureKind.None, expectedWidth, expectedHeight, bitmap.Width, bitmap.Height,
                    paletteIdentity, entries, invalidColors, 0, indices)
                : new(ReplacePngValidationFailureKind.PaletteMismatch, expectedWidth, expectedHeight, bitmap.Width, bitmap.Height,
                    paletteIdentity, entries, invalidColors, invalidColors.Sum(value => value.PixelCount), null);
        }
        catch (Exception ex) when (ex is ArgumentException or ExternalException or OutOfMemoryException or FileNotFoundException or UnauthorizedAccessException)
        {
            return new(ReplacePngValidationFailureKind.InvalidImage, expectedWidth, expectedHeight, null, null,
                paletteIdentity, entries, Array.Empty<InvalidPngColor>(), 0, null);
        }
    }

    internal static bool TryCommitValidatedReplacement(
        IDictionary<int, string> edits,
        int imageId,
        string pngPath,
        ReplacePngValidationResult validation)
    {
        if (!validation.IsValid) return false;
        edits[imageId] = pngPath;
        return true;
    }

    private static IReadOnlyList<PaletteValidationEntry> CreatePaletteEntries(IReadOnlyList<Color>? palette)
        => palette is null ? Array.Empty<PaletteValidationEntry>() : palette
            .Take(16).Select((color, index) => new PaletteValidationEntry(index, color)).ToArray();

    private static int FindExact(IReadOnlyList<Color> palette, Color color)
    {
        for (int index = 1; index < Math.Min(16, palette.Count); index++)
        {
            Color candidate = palette[index];
            if (candidate.R == color.R && candidate.G == color.G && candidate.B == color.B)
                return index;
        }
        return -1;
    }

    private sealed class InvalidColorAccumulator
    {
        public InvalidColorAccumulator(Color color, Point firstOccurrence) { Color = color; FirstOccurrence = firstOccurrence; }
        public Color Color { get; }
        public Point FirstOccurrence { get; }
        public int PixelCount { get; set; }
    }
}
