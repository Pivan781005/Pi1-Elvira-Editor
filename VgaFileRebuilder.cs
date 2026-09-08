using System.Buffers.Binary;

namespace Pi1ElviraEditor;

internal sealed class VgaFileRebuilder
{
    public EncodeReport Rebuild(string sourceVga, IReadOnlyDictionary<int, string> edits, string outputVga, IReadOnlyList<System.Drawing.Color>? activePalette = null)
        => Rebuild(sourceVga, edits, outputVga, activePalette is null ? null : _ => activePalette);

    /// <summary>Composite builds resolve the palette for each edited image from
    /// its persisted resource context. A single VGA resource can contain images
    /// whose proven palette banks differ.</summary>
    public EncodeReport Rebuild(string sourceVga, IReadOnlyDictionary<int, string> edits, string outputVga,
        Func<VgaImageEntry, IReadOnlyList<System.Drawing.Color>?>? paletteForImage)
    {
        byte[] original = File.ReadAllBytes(sourceVga);
        var parsed = new VgaImageTableParser(original).Parse();

        var byOffset = parsed.Entries
            .Where(e => e.DataOffset > 0 && e.DataOffset < original.Length)
            .GroupBy(e => e.DataOffset)
            .OrderBy(g => g.Key)
            .ToList();

        var newOffsets = new Dictionary<uint, uint>();
        int replaced = 0;
        byte[] newFile;

        using (var ms = new MemoryStream())
        {
            ms.Write(original, 0, (int)parsed.FirstDataOffset);

            for (int g = 0; g < byOffset.Count; g++)
            {
                uint oldOffset = byOffset[g].Key;
                uint oldEnd = g + 1 < byOffset.Count ? byOffset[g + 1].Key : (uint)original.Length;
                uint newOffset = checked((uint)ms.Position);
                newOffsets[oldOffset] = newOffset;

                var entries = byOffset[g].ToList();
                var editedEntries = entries.Where(e => edits.ContainsKey(e.ImageId)).ToList();

                if (editedEntries.Count > 0)
                {
                    var master = editedEntries[0];
                    foreach (var e in entries)
                        if (e.PixelWidth != master.PixelWidth || e.Height != master.Height || e.Compressed != master.Compressed)
                            throw new InvalidDataException($"Shared offset 0x{oldOffset:X} má nekompatibilné entries.");

                    IReadOnlyList<System.Drawing.Color>? palette = paletteForImage?.Invoke(master);
                    byte[] pixels = paletteForImage is null
                        ? PaletteTools.ReadIndices(edits[master.ImageId], master.PixelWidth, master.Height)
                        : palette is null
                            ? throw new InvalidDataException($"No validated palette is available for image {master.ImageId}.")
                            : PaletteTools.ReadIndices(edits[master.ImageId], master.PixelWidth, master.Height, palette);
                    byte[] encoded = ElviraImageEncoder.Encode(master, pixels);
                    ms.Write(encoded);
                    replaced++;
                }
                else
                {
                    ms.Write(original, (int)oldOffset, (int)(oldEnd - oldOffset));
                }
            }

            newFile = ms.ToArray();
        }

        foreach (var e in parsed.Entries)
        {
            if (e.DataOffset == 0) continue;
            if (!newOffsets.TryGetValue(e.DataOffset, out uint newOffset))
                throw new InvalidDataException($"Chýba remap pre 0x{e.DataOffset:X}.");

            int p = e.ImageId * 8;
            if (parsed.Endian == OffsetEndian.BigEndian)
                BinaryPrimitives.WriteUInt32BigEndian(newFile.AsSpan(p, 4), newOffset);
            else
                BinaryPrimitives.WriteUInt32LittleEndian(newFile.AsSpan(p, 4), newOffset);
        }

        File.WriteAllBytes(outputVga, newFile);

        return new EncodeReport(parsed.Endian, parsed.Entries.Count, edits.Count, replaced, original.Length, newFile.Length);
    }
}
