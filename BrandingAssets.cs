namespace Pi1ElviraEditor;

// ASCII technical identifiers only. User-visible brand (with Greek pi) lives
// in AppInfo.ProductName and UI text, never in resource names or file names.
internal static class BrandingAssets
{
    internal const string SplashResourceName = "Pi1ElviraEditor.Assets.Branding.Pi1SplashScreen.png";

    internal static Stream OpenSplashStream()
    {
        Stream? stream = typeof(BrandingAssets).Assembly.GetManifestResourceStream(SplashResourceName);
        if (stream is null)
            throw new InvalidDataException($"Embedded splash resource '{SplashResourceName}' was not found.");
        return stream;
    }

    internal static Image LoadSplashImage()
    {
        using Stream stream = OpenSplashStream();
        // Clone out of the resource stream so the stream can be disposed
        // immediately; the caller owns the returned image.
        using Image provisional = Image.FromStream(stream);
        return new Bitmap(provisional);
    }

    internal static bool SplashResourceExists()
        => typeof(BrandingAssets).Assembly.GetManifestResourceNames().Contains(SplashResourceName, StringComparer.Ordinal);

    // Parses a Windows .ico file header and returns embedded frame dimensions.
    // Used by the branding smoke to prove the multi-resolution icon without
    // requiring shell icon extraction for every size.
    internal static IReadOnlyList<Size> GetIcoFrameSizes(string icoPath)
    {
        byte[] data = File.ReadAllBytes(icoPath);
        if (data.Length < 6)
            throw new InvalidDataException("ICO file is too small.");
        ushort reserved = BitConverter.ToUInt16(data, 0);
        ushort type = BitConverter.ToUInt16(data, 2);
        ushort count = BitConverter.ToUInt16(data, 4);
        if (reserved != 0 || type != 1 || count == 0)
            throw new InvalidDataException("ICO file header is invalid.");
        if (data.Length < 6 + count * 16)
            throw new InvalidDataException("ICO directory is truncated.");
        var sizes = new List<Size>(count);
        for (int i = 0; i < count; i++)
        {
            int offset = 6 + i * 16;
            int width = data[offset] == 0 ? 256 : data[offset];
            int height = data[offset + 1] == 0 ? 256 : data[offset + 1];
            if (width <= 0 || height <= 0 || width > 256 || height > 256)
                throw new InvalidDataException("ICO frame dimensions are invalid.");
            sizes.Add(new Size(width, height));
        }
        return sizes;
    }
}
