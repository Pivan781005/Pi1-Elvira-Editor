using System.Buffers.Binary;

namespace Pi1ElviraEditor;

internal sealed class VgaImageTableParser
{
    private readonly byte[] _data;
    public VgaImageTableParser(byte[] data) => _data = data;

    public ParsedTable Parse()
    {
        if (_data.Length < 16)
            throw new InvalidDataException("VGA2 súbor je príliš malý.");

        var be = TryParse(OffsetEndian.BigEndian);
        var le = TryParse(OffsetEndian.LittleEndian);
        var best = be.Score >= le.Score ? be : le;

        if (best.Score < 2 || best.Entries.Count == 0)
            throw new InvalidDataException($"Nepodarilo sa nájsť image table. BE={be.Score}, LE={le.Score}");

        return new ParsedTable(best.Endian, best.FirstDataOffset, best.Entries);
    }

    private Candidate TryParse(OffsetEndian endian)
    {
        uint firstData = uint.MaxValue;
        int plausible = 0;
        int probeEntries = Math.Min(_data.Length / 8, 256);

        for (int i = 1; i < probeEntries; i++)
        {
            int p = i * 8;
            uint off = ReadOffset(p, endian);
            byte h = _data[p + 5];
            ushort widthField = BinaryPrimitives.ReadUInt16BigEndian(_data.AsSpan(p + 6, 2));

            if (IsPlausibleEntry(off, h, widthField, p))
            {
                plausible++;
                if (off < firstData) firstData = off;
            }
        }

        if (firstData == uint.MaxValue || firstData < 16 || firstData > _data.Length)
            return new Candidate(endian, 0, new(), -100);

        int entryCount = (int)(firstData / 8);
        if (entryCount <= 1 || entryCount > _data.Length / 8)
            return new Candidate(endian, firstData, new(), -100);

        var entries = new List<VgaImageEntry>(entryCount);
        int valid = 0, invalid = 0;
        uint previous = 0;

        for (int i = 0; i < entryCount; i++)
        {
            int p = i * 8;
            uint off = ReadOffset(p, endian);
            byte flags = _data[p + 4];
            byte h = _data[p + 5];
            ushort widthField = BinaryPrimitives.ReadUInt16BigEndian(_data.AsSpan(p + 6, 2));
            int units = widthField / 16;
            int pxWidth = units * 16;
            bool compressed = (flags & 0x80) != 0;

            entries.Add(new VgaImageEntry(i, off, flags, h, widthField, units, pxWidth, compressed));

            if (i == 0) continue;
            if (off == 0 && h == 0 && widthField == 0) continue;

            if (IsPlausibleEntry(off, h, widthField, p))
            {
                valid++;
                if (previous != 0 && off < previous) invalid++;
                previous = off;
            }
            else invalid++;
        }

        int score = plausible + valid * 3 - invalid * 4;
        if (firstData % 8 == 0) score += 10;
        return new Candidate(endian, firstData, entries, score);
    }

    private bool IsPlausibleEntry(uint offset, byte height, ushort widthField, int tablePos)
    {
        if (offset == 0 || offset >= _data.Length || offset < tablePos + 8 || height == 0) return false;
        if (widthField == 0 || widthField % 16 != 0) return false;
        int units = widthField / 16;
        return units > 0 && units <= 64 && height <= 240;
    }

    private uint ReadOffset(int p, OffsetEndian endian)
        => endian == OffsetEndian.BigEndian
            ? BinaryPrimitives.ReadUInt32BigEndian(_data.AsSpan(p, 4))
            : BinaryPrimitives.ReadUInt32LittleEndian(_data.AsSpan(p, 4));

    private sealed record Candidate(OffsetEndian Endian, uint FirstDataOffset, List<VgaImageEntry> Entries, int Score);
}
