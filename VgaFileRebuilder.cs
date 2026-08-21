using System.Buffers.Binary;

namespace ElviraVgaEditor;

internal sealed class VgaFileRebuilder
{
    public EncodeReport Rebuild(string sourceVga, IReadOnlyDictionary<int, string> edits, string outputVga)
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

                    byte[] pixels = PaletteTools.ReadIndices(edits[master.ImageId], master.PixelWidth, master.Height);
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
