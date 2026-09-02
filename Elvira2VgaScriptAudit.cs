using System.Buffers.Binary;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace ElviraVgaEditor;

// N7 read-only diagnostic. Deliberately separate from Elvira I and production UI.
internal static class Elvira2VgaScriptAudit
{
    private static readonly Regex Vga1Name = new(@"^\d{2}1\.VGA$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    // E2/Waxworks BE opcode operand lengths, independently matched against E2 scripts.
    private static readonly int[] OperandLength =
    {
        0,6,2,10,6,4,2,2,4,4,8,2,2,2,2,2,2,2,2,0,4,2,2,2,8,0,10,0,8,0,2,2,
        0,0,0,4,4,4,2,4,4,4,4,2,2,4,2,2,2,2,2,2,2,2,2,4,6,6,0,0,0,0,2,2,0,0
    };

    public static void Run(string gameDirectory, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        var pairs = new List<Pair>();
        foreach (string vga1 in Directory.EnumerateFiles(gameDirectory, "*.VGA").Where(p => Vga1Name.IsMatch(Path.GetFileName(p))).OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
        {
            string name = Path.GetFileName(vga1); string vga2Name = name[..2] + "2.VGA";
            var pair = new Pair(Path.GetFileNameWithoutExtension(vga1), name, vga2Name); pairs.Add(pair);
            string vga2 = Path.Combine(gameDirectory, vga2Name);
            if (!File.Exists(vga2)) { pair.Error = "Missing corresponding VGA2."; continue; }
            try { pair.Parse(File.ReadAllBytes(vga1), File.ReadAllBytes(vga2)); } catch (Exception ex) { pair.Error = ex.Message; }
        }
        WriteEvents(Path.Combine(outputDirectory, "E2_VGA_SCRIPT_EVENTS.tsv"), pairs);
        WriteMap(Path.Combine(outputDirectory, "E2_VGA_IMAGE_PALETTE_MAP.tsv"), pairs);
        WriteReport(Path.Combine(outputDirectory, "E2_VGA_PALETTE_AUDIT.txt"), pairs);
    }

    private static void WriteEvents(string path, IEnumerable<Pair> pairs)
    {
        using var w = new StreamWriter(path, false, new UTF8Encoding(false));
        w.WriteLine("Group\tScriptContext\tFileOffset\tRawOpcode\tOperation\tImageId\tPaletteId\tActivePalette\tNotes");
        foreach (Pair p in pairs) foreach (Event e in p.Events)
            w.WriteLine($"{p.Group}\t{e.Context}\t{Hex(e.Offset)}\t{e.Opcode}\t{e.Operation}\t{Id(e.Image)}\t{Id(e.Palette)}\t{Id(e.Active)}\t{Tsv(e.Notes)}");
    }

    private static void WriteMap(string path, IEnumerable<Pair> pairs)
    {
        using var w = new StreamWriter(path, false, new UTF8Encoding(false));
        w.WriteLine("Group\tVga1\tVga2\tImageId\tWidth\tHeight\tEncoding\tObservedPalettes\tOccurrenceCount\tClassification\tDrawOffsets\tPaletteSetOffsets\tNotes");
        foreach (Pair p in pairs.Where(p => p.Parsed)) foreach (VgaImageEntry image in p.Images)
        {
            p.Mappings.TryGetValue(image.ImageId, out Mapping? map); string kind = p.Classify(image, map);
            string palettes = map is null ? "" : string.Join(',', map.Palettes.OrderBy(x => x).Select(x => x.ToString("D3", CultureInfo.InvariantCulture)));
            w.WriteLine($"{p.Group}\t{p.Vga1}\t{p.Vga2}\t{image.ImageId:D4}\t{image.PixelWidth}\t{image.Height}\t{(image.Compressed ? "RLE" : "Raw")}\t{palettes}\t{map?.Occurrences.Count ?? 0}\t{kind}\t{(map is null ? "" : string.Join(',', map.Occurrences.Select(o => Hex(o.Draw)).Distinct()))}\t{(map is null ? "" : string.Join(',', map.Occurrences.Select(o => Hex(o.Set)).Distinct()))}\t{Tsv(map?.Notes ?? "")}");
        }
    }

    private static void WriteReport(string path, IReadOnlyList<Pair> pairs)
    {
        var rows = pairs.Where(p => p.Parsed).SelectMany(p => p.Images.Select(i => (Pair: p, Image: i, Map: p.Mappings.GetValueOrDefault(i.ImageId)))).ToList();
        int unique = rows.Count(r => r.Pair.Classify(r.Image, r.Map) == "UNIQUE"); int contextual = rows.Count(r => r.Pair.Classify(r.Image, r.Map) == "CONTEXTUAL"); int unresolved = rows.Count(r => r.Pair.Classify(r.Image, r.Map) == "UNRESOLVED");
        using var w = new StreamWriter(path, false, new UTF8Encoding(false));
        w.WriteLine("ELVIRA II STATIC VGA SCRIPT -> IMAGE/PALETTE AUDIT (N7, read-only)"); w.WriteLine();
        w.WriteLine("PROVEN COMPATIBLE STRUCTURE: BE UInt16 instruction stream; VGA1 directory at +0x0A; common header at directory+20; 8-byte image/animation entries with script offset at +6.");
        w.WriteLine("PROVEN DIFFERENCE: E2 palette selection is opcode 22 (0x0016), while E1's SetPalette is opcode 23 (0x0017). E2 uses the E2/WW operand-length table."); w.WriteLine();
        w.WriteLine($"VGA1/VGA2 pairs found: {pairs.Count}"); w.WriteLine($"Successfully parsed pairs: {pairs.Count(p => p.Parsed)}"); w.WriteLine($"Total image records: {rows.Count}"); w.WriteLine($"UNIQUE: {unique}"); w.WriteLine($"CONTEXTUAL: {contextual}"); w.WriteLine($"UNRESOLVED: {unresolved}"); w.WriteLine($"INVALID references: {pairs.Sum(p => p.Invalid)}"); w.WriteLine($"Malformed script blocks: {pairs.Sum(p => p.Malformed)}"); w.WriteLine($"Uniquely resolved: {(rows.Count == 0 ? 0 : unique * 100.0 / rows.Count):F2}%"); w.WriteLine();
        foreach (Pair p in pairs.Where(p => !p.Parsed)) w.WriteLine($"PAIR ERROR {p.Vga1}/{p.Vga2}: {p.Error}");
        w.WriteLine("\nSTRONG STATIC MANUAL CANDIDATES");
        foreach (var r in rows.Where(r => r.Pair.Classify(r.Image, r.Map) == "UNIQUE").OrderByDescending(r => r.Image.PixelWidth * r.Image.Height).ThenBy(r => r.Pair.Group).Take(3))
        {
            Occurrence o = r.Map!.Occurrences[0]; w.WriteLine($"{r.Pair.Group}/{r.Pair.Vga2} image {r.Image.ImageId:D4}, {r.Image.PixelWidth}x{r.Image.Height}, {(r.Image.Compressed ? "RLE" : "Raw")}, palette {o.Palette:D3}; SetPalette @{Hex(o.Set)} -> Draw @{Hex(o.Draw)}; context {o.Context}. Large UNIQUE image with a local static sequence.");
        }
        w.WriteLine("\nCONTEXTUAL IMAGES"); foreach (var r in rows.Where(r => r.Pair.Classify(r.Image, r.Map) == "CONTEXTUAL")) w.WriteLine($"{r.Pair.Group}/{r.Pair.Vga2} image {r.Image.ImageId:D4}: {string.Join(',', r.Map!.Palettes.OrderBy(x => x).Select(x => x.ToString("D3")))}");
        w.WriteLine("\nUNRESOLVED TOTALS BY GROUP"); foreach (var g in rows.Where(r => r.Pair.Classify(r.Image, r.Map) == "UNRESOLVED").GroupBy(r => r.Pair.Group)) w.WriteLine($"{g.Key}: {g.Count()}");
    }

    // Shared structural parser for the diagnostic and production resolver.  It
    // derives mappings only from the currently selected VGA1/VGA2 resource pair.
    internal sealed class Pair
    {
        public Pair(string group, string vga1, string vga2) { Group = group; Vga1 = vga1; Vga2 = vga2; }
        public string Group { get; } public string Vga1 { get; } public string Vga2 { get; } public bool Parsed { get; private set; } public string Error { get; set; } = ""; public int PaletteCount { get; private set; }
        internal List<VgaImageEntry> Images { get; set; } = new(); internal Dictionary<int, Mapping> Mappings { get; } = new(); internal List<Event> Events { get; } = new(); internal int Invalid { get; set; } internal int Malformed { get; set; }
        public void Parse(byte[] one, byte[] two)
        {
            if (one.Length < 32) throw new InvalidDataException("VGA1 too small."); PaletteCount = Be(one, 2); int paletteBase = Be(one, 6);
            if (PaletteCount <= 0 || paletteBase + PaletteCount * 32 > one.Length) throw new InvalidDataException("Malformed E2 palette header."); Images = new VgaImageTableParser(two).Parse().Entries;
            int header = Be(one, 10) + 20; if (header < 0 || header + 20 > one.Length) throw new InvalidDataException("Malformed E2 script directory/header.");
            ParseTable(one, Be(one, header + 10), Be(one, header + 2), "Image"); ParseTable(one, Be(one, header + 14), Be(one, header + 6), "Animation"); Parsed = true;
        }
        public PaletteResolution Resolve(int imageId)
        {
            if (!Parsed) return PaletteResolution.Invalid(string.IsNullOrWhiteSpace(Error) ? "The E2 VGA pair was not parsed." : Error);
            if (imageId < 0 || imageId >= Images.Count || !Usable(Images[imageId]))
                return PaletteResolution.Invalid("Image is outside the usable E2 VGA2 image table.");
            if (!Mappings.TryGetValue(imageId, out Mapping? mapping) || mapping.Palettes.Count == 0)
                return PaletteResolution.Unresolved();
            int[] palettes = mapping.Palettes.OrderBy(p => p).ToArray();
            return palettes.Length == 1 ? PaletteResolution.Unique(palettes[0]) : PaletteResolution.Contextual(palettes);
        }
        private void ParseTable(byte[] d, int table, int count, string kind)
        {
            if (count == 0) return; if (table < 0 || table + count * 8 > d.Length) throw new InvalidDataException($"Malformed {kind} entry table.");
            for (int i = 0; i < count; i++) { int p = table + i * 8; int id = Be(d, p); int script = Be(d, p + 6); if (script == 0) continue; if (script >= d.Length) { Malformed++; continue; } Walk(d, script, $"{kind}:{id}"); }
        }
        private void Walk(byte[] d, int start, string context)
        {
            var pending = new Stack<(int Pc, int? Palette)>(); var seen = new HashSet<(int Pc, int? Palette)>(); pending.Push((start, null)); int budget = 4096;
            while (pending.Count > 0 && budget-- > 0)
            {
                var state = pending.Pop(); if (!seen.Add(state)) continue; int pc = state.Pc; int? active = state.Palette;
                while (pc + 2 <= d.Length && budget-- > 0)
                {
                    int at = pc; int op = Be(d, pc); pc += 2; if (op == 0) break; if (op >= OperandLength.Length || pc + OperandLength[op] > d.Length) { Malformed++; Events.Add(new(context, at, op, "Malformed", null, null, active, "Invalid opcode or truncated operands.")); break; }
                    int arg = pc; int next = pc + OperandLength[op];
                    if (op == 22) { int palette = Be(d, arg); if (palette >= PaletteCount) { Invalid++; active = null; Events.Add(new(context, at, op, "PaletteSelect", null, palette, null, "Outside declared palette range.")); } else { active = palette; Events.Add(new(context, at, op, "PaletteSelect", null, palette, active, "E2 opcode 22.")); } }
                    else if (op == 10) { int image = (short)Be(d, arg); if (image < 0) { Events.Add(new(context, at, op, "DrawVariable", image, null, active, "Negative image operand is a runtime variable reference; no static image mapping.")); } else if (image >= Images.Count || !Usable(Images[image])) { Invalid++; Events.Add(new(context, at, op, "Draw", image, null, active, "Outside valid VGA2 image table.")); } else { Events.Add(new(context, at, op, "Draw", image, null, active, active is null ? "No established palette in this context." : "Path-local static sequence.")); if (active is int p) Add(image, p, context, at); } }
                    else if (op is >= 5 and <= 9 || op is 43 or 44 or 51) { int skip = Skip(d, next); if (skip < 0) { Malformed++; break; } pending.Push((skip, active)); pc = next; continue; }
                    else if (op == 18) { int target = next + (short)Be(d, arg); if (target < 0 || target >= d.Length) { Malformed++; break; } pending.Push((target, active)); break; }
                    else if (op is 2 or 16 or 17 or 19 or 20 or 21 or 25 or 56) { Events.Add(new(context, at, op, "ControlTransfer", null, null, active, "Dynamic/asynchronous path not assumed.")); break; }
                    pc = next;
                }
            }
            if (budget <= 0) { Malformed++; Events.Add(new(context, start, -1, "Budget", null, null, null, "Instruction budget exhausted.")); }
        }
        private void Add(int image, int palette, string context, int draw)
        {
            if (!Mappings.TryGetValue(image, out Mapping? map)) Mappings[image] = map = new Mapping(); int? set = Events.LastOrDefault(e => e.Context == context && e.Operation == "PaletteSelect" && e.Palette == palette)?.Offset; map.Palettes.Add(palette); map.Occurrences.Add(new Occurrence(context, palette, set, draw));
        }
        internal string Classify(VgaImageEntry image, Mapping? map) => map is null || map.Palettes.Count == 0 ? "UNRESOLVED" : map.Palettes.Count == 1 ? "UNIQUE" : "CONTEXTUAL";
        private static bool Usable(VgaImageEntry i) => i.DataOffset != 0 && i.Height != 0 && i.WidthField != 0;
        private static int Skip(byte[] d, int pc) { if (pc + 2 > d.Length) return -1; int op = Be(d, pc); return op < OperandLength.Length && pc + 2 + OperandLength[op] <= d.Length ? pc + 2 + OperandLength[op] : -1; }
    }
    private static int Be(byte[] d, int o) => BinaryPrimitives.ReadUInt16BigEndian(d.AsSpan(o, 2)); private static string Hex(int? n) => n is null ? "" : $"0x{n:X4}"; private static string Id(int? n) => n?.ToString(CultureInfo.InvariantCulture) ?? ""; private static string Tsv(string s) => s.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
    internal sealed record Event(string Context, int Offset, int Opcode, string Operation, int? Image, int? Palette, int? Active, string Notes); internal sealed class Mapping { public HashSet<int> Palettes { get; } = new(); public List<Occurrence> Occurrences { get; } = new(); public string Notes => "Path-local static mapping."; } internal sealed record Occurrence(string Context, int Palette, int? Set, int Draw);
}
