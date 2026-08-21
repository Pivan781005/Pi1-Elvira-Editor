namespace ElviraVgaEditor;

internal enum OffsetEndian
{
    BigEndian,
    LittleEndian
}

internal sealed record VgaImageEntry(
    int ImageId,
    uint DataOffset,
    byte HeaderFlags,
    byte Height,
    ushort WidthField,
    int WidthUnits,
    int PixelWidth,
    bool Compressed);

internal sealed record ParsedTable(
    OffsetEndian Endian,
    uint FirstDataOffset,
    List<VgaImageEntry> Entries);

internal sealed record EncodeReport(
    OffsetEndian Endian,
    int TableEntries,
    int EditedPngs,
    int ReplacedBlocks,
    int OldSize,
    int NewSize);
