namespace ElviraVgaEditor;

internal static class ElviraImageDecoder
{
    public static byte[] Decode(byte[] file, VgaImageEntry e)
    {
        if (e.DataOffset >= file.Length) throw new InvalidDataException("Data offset je mimo súboru.");
        if (e.PixelWidth <= 0 || e.Height <= 0) throw new InvalidDataException("Neplatné rozmery.");
        return e.Compressed ? DecodeCompressed(file, e) : DecodeRaw(file, e);
    }

    private static byte[] DecodeRaw(byte[] file, VgaImageEntry e)
    {
        int packedRowBytes = e.WidthUnits * 8;
        int expected = checked(packedRowBytes * e.Height);
        if ((long)e.DataOffset + expected > file.Length)
            throw new InvalidDataException("RAW obraz presahuje EOF.");

        byte[] pixels = new byte[checked(e.PixelWidth * e.Height)];
        int src = (int)e.DataOffset, dst = 0;

        for (int y = 0; y < e.Height; y++)
        {
            for (int x = 0; x < packedRowBytes; x++)
            {
                byte b = file[src++];
                pixels[dst++] = (byte)(b >> 4);
                pixels[dst++] = (byte)(b & 0x0F);
            }
        }
        return pixels;
    }

    private static byte[] DecodeCompressed(byte[] file, VgaImageEntry e)
    {
        int columns = e.WidthUnits * 8;
        byte[] pixels = new byte[checked(e.PixelWidth * e.Height)];

        int src = (int)e.DataOffset;
        sbyte continuation = unchecked((sbyte)0x80);

        for (int col = 0; col < columns; col++)
        {
            byte[] packedColumn = DepackColumn(file, ref src, ref continuation, e.Height);
            int px = col * 2;

            for (int y = 0; y < e.Height; y++)
            {
                byte colors = packedColumn[y];
                int row = y * e.PixelWidth;
                pixels[row + px] = (byte)(colors >> 4);
                pixels[row + px + 1] = (byte)(colors & 0x0F);
            }
        }
        return pixels;
    }

    private static byte[] DepackColumn(byte[] file, ref int src, ref sbyte continuation, int height)
    {
        byte[] output = new byte[height];
        int dst = 0, remaining = height;
        sbyte a = continuation;
        if (a == -128) a = ReadSByte(file, ref src);

        while (true)
        {
            if (a >= 0)
            {
                byte color = ReadByte(file, ref src);
                while (true)
                {
                    output[dst++] = color;
                    remaining--;
                    if (remaining == 0)
                    {
                        a--;
                        if (a < 0) a = -128;
                        else src--;
                        continuation = a;
                        return output;
                    }
                    a--;
                    if (a < 0) break;
                }
            }
            else
            {
                while (true)
                {
                    output[dst++] = ReadByte(file, ref src);
                    remaining--;
                    if (remaining == 0)
                    {
                        a++;
                        if (a == 0) a = -128;
                        continuation = a;
                        return output;
                    }
                    a++;
                    if (a == 0) break;
                }
            }
            a = ReadSByte(file, ref src);
        }
    }

    private static byte ReadByte(byte[] file, ref int pos)
    {
        if ((uint)pos >= (uint)file.Length)
            throw new EndOfStreamException($"RLE stream skončil na 0x{pos:X}.");
        return file[pos++];
    }

    private static sbyte ReadSByte(byte[] file, ref int pos) => unchecked((sbyte)ReadByte(file, ref pos));
}
