using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Pi1ElviraEditor;

// Read-only diagnostic for Elvira I DOS VGA1/VGA2 pairs. It is deliberately
// not connected to the UI or Automatic palette selection.
internal static class Elvira1VgaScriptAudit
{
    private static readonly Regex Vga1Name = new(@"^\d{2}1\.VGA$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    // Elvira I opcodes are BE UInt16. These are their operand byte counts,
    // reproduced from the Elvira I table in ScummVM's AGOS VGA interpreter.
    private static readonly int[] OperandLength =
    {
        0, 6, 2, 10, 6, 4, 2, 2, 4, 4, 8, 2, 0, 2, 2, 2,
        2, 2, 2, 2, 0, 4, 2, 2, 2, 8, 0, 10, 0, 8, 0, 2,
        2, 0, 0, 0, 0, 2, 4, 2, 4, 4, 0, 0, 2, 2, 2, 4,
        4, 0, 18, 2, 4, 4, 4, 0, 4
    };

    public static void Run(string gameDirectory, string outputDirectory)
    {
        if (!Directory.Exists(gameDirectory))
            throw new DirectoryNotFoundException(gameDirectory);
        Directory.CreateDirectory(outputDirectory);

        var all = new List<PairAudit>();
        foreach (string vga1Path in Directory.EnumerateFiles(gameDirectory, "*.VGA")
                     .Where(path => Vga1Name.IsMatch(Path.GetFileName(path)))
                     .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase))
        {
            string name = Path.GetFileName(vga1Path);
            string vga2Name = name[..2] + "2.VGA";
            string vga2Path = Path.Combine(gameDirectory, vga2Name);
            var pair = new PairAudit(name[..2] + "1", name, vga2Name);
            all.Add(pair);
            if (!File.Exists(vga2Path))
            {
                pair.Error = "Missing corresponding VGA2 file.";
                continue;
            }

            try
            {
                pair.Parse(File.ReadAllBytes(vga1Path), File.ReadAllBytes(vga2Path));
            }
            catch (Exception ex)
            {
                pair.Error = ex.Message;
            }
        }

        WriteEvents(Path.Combine(outputDirectory, "E1_VGA_SCRIPT_EVENTS.tsv"), all);
        WriteMap(Path.Combine(outputDirectory, "E1_VGA_IMAGE_PALETTE_MAP.tsv"), all);
        WriteReport(Path.Combine(outputDirectory, "E1_VGA_PALETTE_AUDIT.txt"), all);

        PairAudit? death = all.SingleOrDefault(p => p.Group == "621");
        bool deathPass = death?.ImageMappings.TryGetValue(3, out ImageMapping? deathMap) == true &&
                         deathMap.Palettes.SetEquals(new[] { 2 });
        if (!deathPass)
            throw new InvalidDataException("Mandatory 621/622 image 0003 -> palette 002 anchor was not reproduced; see audit output.");
    }

    private static void WriteEvents(string path, IEnumerable<PairAudit> all)
    {
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("Group\tScriptContext\tFileOffset\tOpcode\tOperation\tImageId\tPaletteId\tActivePalette\tNotes");
        foreach (PairAudit pair in all)
            foreach (ScriptEvent ev in pair.Events)
                writer.WriteLine(string.Join('\t', new[]
                {
                    Tsv(pair.Group), Tsv(ev.Context), Hex(ev.Offset), ev.Opcode.ToString(CultureInfo.InvariantCulture),
                    Tsv(ev.Operation), NullableId(ev.ImageId), NullableId(ev.PaletteId), NullableId(ev.ActivePalette), Tsv(ev.Notes)
                }));
    }

    private static void WriteMap(string path, IEnumerable<PairAudit> all)
    {
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("Group\tVga1\tVga2\tImageId\tWidth\tHeight\tEncoding\tObservedPalettes\tOccurrenceCount\tClassification\tDrawOffsets\tPaletteSetOffsets\tNotes");
        foreach (PairAudit pair in all.Where(p => p.Parsed))
        {
            foreach (VgaImageEntry image in pair.Images)
            {
                pair.ImageMappings.TryGetValue(image.ImageId, out ImageMapping? mapping);
                string classification = pair.Classify(image.ImageId, image, mapping);
                writer.WriteLine(string.Join('\t', new[]
                {
                    Tsv(pair.Group), pair.Vga1, pair.Vga2, image.ImageId.ToString("D4", CultureInfo.InvariantCulture),
                    image.PixelWidth.ToString(CultureInfo.InvariantCulture), image.Height.ToString(CultureInfo.InvariantCulture),
                    image.Compressed ? "RLE" : "Raw", mapping is null ? "" : string.Join(',', mapping.Palettes.OrderBy(x => x).Select(x => x.ToString("D3", CultureInfo.InvariantCulture))),
                    (mapping?.Occurrences.Count ?? 0).ToString(CultureInfo.InvariantCulture), classification,
                    mapping is null ? "" : string.Join(',', mapping.Occurrences.Select(x => Hex(x.DrawOffset)).Distinct()),
                    mapping is null ? "" : string.Join(',', mapping.Occurrences.Select(x => Hex(x.PaletteOffset)).Where(x => x is not null).Distinct()),
                    Tsv(mapping?.Notes ?? "")
                }));
            }
        }
    }

    private static void WriteReport(string path, IReadOnlyList<PairAudit> all)
    {
        int pairs = all.Count;
        int parsed = all.Count(p => p.Parsed);
        var maps = all.Where(p => p.Parsed).SelectMany(p => p.Images.Select(i => (Pair: p, Image: i, Map: p.ImageMappings.GetValueOrDefault(i.ImageId)))).ToList();
        int unique = maps.Count(x => x.Pair.Classify(x.Image.ImageId, x.Image, x.Map) == "UNIQUE");
        int contextual = maps.Count(x => x.Pair.Classify(x.Image.ImageId, x.Image, x.Map) == "CONTEXTUAL");
        int unresolved = maps.Count(x => x.Pair.Classify(x.Image.ImageId, x.Image, x.Map) == "UNRESOLVED");
        int invalid = all.Sum(p => p.InvalidReferences);
        int malformed = all.Sum(p => p.MalformedBlocks);
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("ELVIRA I STATIC VGA SCRIPT -> IMAGE/PALETTE AUDIT (read-only)");
        writer.WriteLine();
        writer.WriteLine("Method: E1 VGA1 scripts are decoded from the documented directory/common-header entry tables. Opcodes are BE UInt16. Palette state is path-local; conditional paths are forked, while unknown dynamic transfers stop that path rather than inventing a palette.");
        writer.WriteLine("Naming note: installed files are three digits ending in 1/2 (for example 621.VGA/622.VGA); that is the actual pairing used here.");
        writer.WriteLine();
        writer.WriteLine($"Resource pairs found: {pairs}");
        writer.WriteLine($"Successfully parsed pairs: {parsed}");
        writer.WriteLine($"Total image records: {maps.Count}");
        writer.WriteLine($"UNIQUE images: {unique}");
        writer.WriteLine($"CONTEXTUAL images: {contextual}");
        writer.WriteLine($"UNRESOLVED images: {unresolved}");
        writer.WriteLine($"INVALID references: {invalid}");
        writer.WriteLine($"Malformed script blocks: {malformed}");
        writer.WriteLine($"Uniquely resolved: {(maps.Count == 0 ? 0 : unique * 100.0 / maps.Count):F2}%");
        writer.WriteLine();
        foreach (PairAudit failed in all.Where(p => !p.Parsed))
            writer.WriteLine($"PAIR ERROR {failed.Vga1}/{failed.Vga2}: {failed.Error}");
        writer.WriteLine();
        writer.WriteLine("MANDATORY ANCHORS");
        WriteAnchor(writer, all, "621", 3, 2);
        WriteAnchor(writer, all, "071", 56, 5);
        writer.WriteLine();
        writer.WriteLine("CONTEXTUAL / UNRESOLVED IMAGES");
        foreach (var row in maps.Where(x => x.Pair.Classify(x.Image.ImageId, x.Image, x.Map) is "CONTEXTUAL" or "UNRESOLVED"))
        {
            string palettes = row.Map is null ? "none" : string.Join(',', row.Map.Palettes.OrderBy(x => x).Select(x => x.ToString("D3", CultureInfo.InvariantCulture)));
            writer.WriteLine($"{row.Pair.Group}/{row.Pair.Vga2} image {row.Image.ImageId:D4}: {row.Pair.Classify(row.Image.ImageId, row.Image, row.Map)}; palettes={palettes}");
        }
    }

    private static void WriteAnchor(StreamWriter writer, IReadOnlyList<PairAudit> all, string group, int imageId, int expectedPalette)
    {
        PairAudit? pair = all.SingleOrDefault(p => p.Group == group);
        ImageMapping? map = null;
        if (pair is not null)
            pair.ImageMappings.TryGetValue(imageId, out map);
        string result = map is null ? "UNRESOLVED" : map.Palettes.SetEquals(new[] { expectedPalette }) ? "PASS" : map.Palettes.Contains(expectedPalette) ? "CONTEXTUAL" : "FAIL";
        writer.WriteLine($"{group}/{int.Parse(group, CultureInfo.InvariantCulture) + 1:D3} image {imageId:D4} -> palette {expectedPalette:D3}: {result}");
        if (map is not null)
            foreach (Occurrence o in map.Occurrences)
                writer.WriteLine($"  context {o.Context}: SetPalette({o.Palette:D3}) @{Hex(o.PaletteOffset)} -> Draw({imageId:D4}) @{Hex(o.DrawOffset)} ({o.Notes})");
    }

    private static string Tsv(string value) => value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
    private static string Hex(int? value) => value is null ? "" : $"0x{value.Value:X4}";
    private static string NullableId(int? value) => value?.ToString(CultureInfo.InvariantCulture) ?? "";

    // Shared by the production-safe resolver. This remains an in-memory parser:
    // it never consumes N5 report files and it never writes a game resource.
    internal sealed class PairAudit
    {
        public PairAudit(string group, string vga1, string vga2) { Group = group; Vga1 = vga1; Vga2 = vga2; }
        public string Group { get; }
        public string Vga1 { get; }
        public string Vga2 { get; }
        public bool Parsed { get; private set; }
        public string Error { get; set; } = "";
        public int PaletteCount { get; private set; }
        internal List<VgaImageEntry> Images { get; private set; } = new();
        internal List<ScriptEvent> Events { get; } = new();
        internal Dictionary<int, ImageMapping> ImageMappings { get; } = new();
        internal int InvalidReferences { get; private set; }
        internal int MalformedBlocks { get; private set; }

        public PaletteResolution Resolve(int imageId)
        {
            if (!Parsed)
                return PaletteResolution.Invalid("VGA1/VGA2 parse did not complete.");
            if (imageId < 0 || imageId >= Images.Count || !IsUsableImage(Images[imageId]))
                return PaletteResolution.Invalid("Image ID is outside the valid VGA2 table.");
            if (!ImageMappings.TryGetValue(imageId, out ImageMapping? mapping) || mapping.Palettes.Count == 0)
                return PaletteResolution.Unresolved();
            int[] palettes = mapping.Palettes.OrderBy(x => x).ToArray();
            return palettes.Length == 1
                ? PaletteResolution.Unique(palettes[0])
                : PaletteResolution.Contextual(palettes);
        }

        public void Parse(byte[] vga1, byte[] vga2)
        {
            if (vga1.Length < 32) throw new InvalidDataException("VGA1 too small.");
            PaletteCount = ReadBe16(vga1, 2);
            int paletteBase = ReadBe16(vga1, 6);
            if (PaletteCount <= 0 || paletteBase < 0 || paletteBase + PaletteCount * 32 > vga1.Length)
                throw new InvalidDataException("Malformed VGA1 palette header.");
            Images = new VgaImageTableParser(vga2).Parse().Entries;

            int directory = ReadBe16(vga1, 10);
            int header = directory + 20;
            if (directory < 0 || header + 20 > vga1.Length) throw new InvalidDataException("Malformed VGA1 script directory/common header.");
            int imageCount = ReadBe16(vga1, header + 2);
            int animationCount = ReadBe16(vga1, header + 6);
            int imageTable = ReadBe16(vga1, header + 10);
            int animationTable = ReadBe16(vga1, header + 14);
            ParseEntries(vga1, imageTable, imageCount, "Image");
            ParseEntries(vga1, animationTable, animationCount, "Animation");
            Parsed = true;
        }

        private void ParseEntries(byte[] data, int table, int count, string kind)
        {
            if (count == 0) return;
            if (table < 0 || table + count * 8 > data.Length) throw new InvalidDataException($"Malformed {kind} script entry table.");
            for (int i = 0; i < count; i++)
            {
                int p = table + i * 8;
                int id = ReadBe16(data, p);
                int script = ReadBe16(data, p + 6);
                if (script == 0) continue;
                if (script >= data.Length) { MalformedBlocks++; Events.Add(new ScriptEvent($"{kind}:{id}", script, -1, "Invalid entry", null, null, null, "Script offset outside VGA1.")); continue; }
                Walk(data, script, $"{kind}:{id}");
            }
        }

        private void Walk(byte[] data, int start, string context)
        {
            var todo = new Stack<PathState>();
            var seen = new HashSet<(int Pc, int? Palette)>();
            todo.Push(new PathState(start, null, false));
            int budget = 4096;
            while (todo.Count > 0 && budget-- > 0)
            {
                PathState state = todo.Pop();
                if (!seen.Add((state.Pc, state.Palette))) continue;
                int pc = state.Pc;
                int? palette = state.Palette;
                bool uncertain = state.Uncertain;
                while (pc + 2 <= data.Length && budget-- > 0)
                {
                    int opOffset = pc;
                    int opcode = ReadBe16(data, pc); pc += 2;
                    if (opcode == 0) break;
                    if (opcode < 0 || opcode >= OperandLength.Length || pc + OperandLength[opcode] > data.Length)
                    {
                        MalformedBlocks++; Events.Add(new ScriptEvent(context, opOffset, opcode, "Malformed", null, null, palette, "Invalid opcode or truncated operands.")); break;
                    }
                    int operand = pc;
                    int next = pc + OperandLength[opcode];
                    if (opcode == 23)
                    {
                        int raw = ReadBe16(data, operand);
                        int bank = raw >= 1000 ? raw - 1000 : raw;
                        if (bank >= PaletteCount) { InvalidReferences++; Events.Add(new ScriptEvent(context, opOffset, opcode, "SetPalette", null, bank, palette, "Palette outside declared VGA1 range.")); palette = null; uncertain = true; }
                        else { palette = bank; Events.Add(new ScriptEvent(context, opOffset, opcode, "SetPalette", null, bank, palette, raw >= 1000 ? "Elvira I bottom-palette variant." : "")); }
                    }
                    else if (opcode == 10)
                    {
                        int image = ReadBe16(data, operand);
                        if (image < 0 || image >= Images.Count || !IsUsableImage(Images[image]))
                        {
                            InvalidReferences++; Events.Add(new ScriptEvent(context, opOffset, opcode, "Draw", image, null, palette, "Image outside valid VGA2 image table."));
                        }
                        else
                        {
                            Events.Add(new ScriptEvent(context, opOffset, opcode, "Draw", image, null, palette, uncertain ? "Path passed an unknown dynamic transfer." : ""));
                            if (palette is int p) AddOccurrence(image, p, context, opOffset, FindLastPaletteOffset(context, p), uncertain ? "Path-local; dynamic transfer encountered." : "Path-local static sequence.");
                        }
                    }
                    else if (opcode is >= 5 and <= 9 || opcode == 51)
                    {
                        int skip = SkipOne(data, next);
                        if (skip < 0) { MalformedBlocks++; break; }
                        todo.Push(new PathState(skip, palette, true));
                        pc = next; uncertain = true; continue;
                    }
                    else if (opcode == 19)
                    {
                        int target = next + (short)ReadBe16(data, operand);
                        if (target < 0 || target >= data.Length) { MalformedBlocks++; break; }
                        todo.Push(new PathState(target, palette, uncertain));
                        break;
                    }
                    else if (opcode is 2 or 20 or 21 or 22 or 16 or 17 or 18 or 56)
                    {
                        Events.Add(new ScriptEvent(context, opOffset, opcode, "ControlTransfer", null, null, palette, "Dynamic/asynchronous control flow: remaining path not assumed."));
                        break;
                    }
                    pc = next;
                }
            }
            if (budget <= 0) { MalformedBlocks++; Events.Add(new ScriptEvent(context, start, -1, "Budget", null, null, null, "Walker instruction budget exhausted.")); }
        }

        private int? FindLastPaletteOffset(string context, int palette) => Events.LastOrDefault(e => e.Context == context && e.Operation == "SetPalette" && e.PaletteId == palette)?.Offset;
        private void AddOccurrence(int image, int palette, string context, int draw, int? set, string notes)
        {
            if (!ImageMappings.TryGetValue(image, out ImageMapping? mapping)) ImageMappings[image] = mapping = new ImageMapping();
            mapping.Palettes.Add(palette); mapping.Occurrences.Add(new Occurrence(context, palette, set, draw, notes));
            if (!string.IsNullOrEmpty(notes)) mapping.Notes = notes;
        }
        internal string Classify(int imageId, VgaImageEntry image, ImageMapping? mapping)
            => mapping is null || mapping.Palettes.Count == 0 ? "UNRESOLVED" : mapping.Palettes.Count == 1 ? "UNIQUE" : "CONTEXTUAL";
        private static bool IsUsableImage(VgaImageEntry e) => e.DataOffset != 0 && e.Height != 0 && e.WidthField != 0;
        private static int SkipOne(byte[] data, int pc)
        {
            if (pc + 2 > data.Length) return -1;
            int op = ReadBe16(data, pc);
            return op >= 0 && op < OperandLength.Length && pc + 2 + OperandLength[op] <= data.Length ? pc + 2 + OperandLength[op] : -1;
        }
    }

    private static int ReadBe16(byte[] data, int offset) => BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(offset, 2));
    private sealed record PathState(int Pc, int? Palette, bool Uncertain);
    internal sealed record ScriptEvent(string Context, int Offset, int Opcode, string Operation, int? ImageId, int? PaletteId, int? ActivePalette, string Notes);
    internal sealed class ImageMapping { public HashSet<int> Palettes { get; } = new(); public List<Occurrence> Occurrences { get; } = new(); public string Notes { get; set; } = ""; }
    internal sealed record Occurrence(string Context, int Palette, int? PaletteOffset, int DrawOffset, string Notes);
}
