namespace Pi1ElviraEditor;

internal static class ElviraImageEncoder
{
    public static byte[] Encode(VgaImageEntry e, byte[] pixels)
        => e.Compressed ? EncodeCompressed(e, pixels) : EncodeRaw(e, pixels);

    private static byte[] EncodeRaw(VgaImageEntry e, byte[] pixels)
    {
        byte[] output = new byte[checked(e.WidthUnits * 8 * e.Height)];
        int dst = 0;

        for (int y = 0; y < e.Height; y++)
        {
            int row = y * e.PixelWidth;
            for (int x = 0; x < e.PixelWidth; x += 2)
            {
                byte hi = (byte)(pixels[row + x] & 0x0F);
                byte lo = (byte)(pixels[row + x + 1] & 0x0F);
                output[dst++] = (byte)((hi << 4) | lo);
            }
        }
        return output;
    }

    private static byte[] EncodeCompressed(VgaImageEntry e, byte[] pixels)
    {
        using var ms = new MemoryStream();
        int columns = e.WidthUnits * 8;

        for (int col = 0; col < columns; col++)
        {
            byte[] column = new byte[e.Height];
            int px = col * 2;

            for (int y = 0; y < e.Height; y++)
            {
                int row = y * e.PixelWidth;
                column[y] = (byte)(((pixels[row + px] & 0x0F) << 4) | (pixels[row + px + 1] & 0x0F));
            }

            WriteColumn(ms, column);
        }

        return ms.ToArray();
    }

    private static void WriteColumn(Stream output, byte[] src)
    {
        int i = 0;

        while (i < src.Length)
        {
            int run = CountRun(src, i, 128);

            if (run >= 3)
            {
                output.WriteByte((byte)(run - 1));
                output.WriteByte(src[i]);
                i += run;
                continue;
            }

            int literalStart = i;
            int literalLen = 0;

            while (i < src.Length && literalLen < 127)
            {
                run = CountRun(src, i, 128);
                if (run >= 3) break;
                i++;
                literalLen++;
            }

            if (literalLen == 0) continue;

            output.WriteByte(unchecked((byte)(sbyte)(-literalLen)));
            output.Write(src, literalStart, literalLen);
        }
    }

    private static int CountRun(byte[] src, int start, int max)
    {
        byte v = src[start];
        int n = 1;
        while (start + n < src.Length && n < max && src[start + n] == v) n++;
        return n;
    }
}
