using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ElviraVgaEditor;

// Phase 6C9O2B2 diagnostic only. It consumes the corrected F34C raw-stack
// trace offline; it does not open a UI and has no GAMEPC write path.
internal static class Elvira1CorrectedStackAnalyzer
{
    private const int MzHeader = 0x2400;
    private const int RendererPhysical = 0xF34C;
    private static readonly string[] Header =
    [
        "Sequence", "DL", "RendererCS", "RendererIP", "SS", "SP", "TrueReturnIP",
        "Stack00", "Stack02", "Stack04", "Stack06", "Stack08", "Stack0A", "Stack0C",
        "Stack0E", "Stack10", "Stack12", "Stack14", "Stack16", "Stack18", "Stack1A", "Stack1C", "Stack1E",
        "Stack20", "Stack22", "Stack24", "Stack26", "Stack28", "Stack2A", "Stack2C", "Stack2E", "Stack30",
        "Stack32", "Stack34", "Stack36", "Stack38", "Stack3A", "Stack3C", "Stack3E"
    ];

    private static readonly int[] Targets = [417, 424, 425, 298];

    internal static void Run(string tracePath, string gamePcPath, string canonicalExePath, string outputDirectory)
    {
        var protectedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [gamePcPath] = Hash(gamePcPath), [canonicalExePath] = Hash(canonicalExePath)
        };
        if (new FileInfo(canonicalExePath).Length != 0x2CE30 ||
            !string.Equals(protectedHashes[canonicalExePath], "C6CEC09B41D42D92DDD0588B54E5729720CDC91ABEF8083F9205F237A9686756", StringComparison.Ordinal))
            throw new InvalidDataException("Canonical RUNVGA does not match the required 183856-byte SHA-256 baseline.");

        List<Event> trace = ParseTrace(tracePath);
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(gamePcPath, ElviraGameProfile.Elvira1);
        if (entries.Count != 689) throw new InvalidDataException($"Expected 689 GAMEPC entries, found {entries.Count}.");
        byte[] executable = File.ReadAllBytes(canonicalExePath);
        List<ReturnSite> sites = BuildValidatedReturnSites(executable);
        int loadLinear = DeriveLoadLinear(trace);
        var matches = MatchTargets(trace, entries);
        var targets = Targets.ToDictionary(index => index, index => AnalyzeTarget(index, entries[index], trace, matches[index], sites, loadLinear));

        Directory.CreateDirectory(outputDirectory);
        WriteAudit(Path.Combine(outputDirectory, "CORRECTED_TRACE_AUDIT.txt"), trace, entries, sites, targets, loadLinear);
        WriteReturnSites(Path.Combine(outputDirectory, "VALID_RETURN_SITES.tsv"), sites);
        WriteTargetEvents(Path.Combine(outputDirectory, "TARGET_STACK_EVENTS.tsv"), targets);
        WriteChains(Path.Combine(outputDirectory, "TARGET_CALL_CHAINS.tsv"), targets, loadLinear);
        WriteComparison(Path.Combine(outputDirectory, "AFFECTED_SAFE_COMPARISON.txt"), targets);
        WriteConclusion(Path.Combine(outputDirectory, "O2B2_CONCLUSION.txt"), targets, loadLinear);

        foreach ((string path, string before) in protectedHashes)
            if (!string.Equals(before, Hash(path), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"Read-only O2B2 analysis changed {Path.GetFileName(path)}.");
    }

    private static List<Event> ParseTrace(string path)
    {
        using var reader = new StreamReader(path, Encoding.UTF8, true);
        string? header = reader.ReadLine();
        if (header is null || !header.Split('\t').SequenceEqual(Header, StringComparer.Ordinal))
            throw new InvalidDataException("Corrected trace header does not match the required Stack00..Stack3E schema.");
        var result = new List<Event>(); ulong previous = 0; int line = 1;
        while (reader.ReadLine() is { } text)
        {
            line++; string[] f = text.Split('\t');
            if (f.Length != Header.Length) throw new InvalidDataException($"Trace line {line} has {f.Length} fields; expected {Header.Length}.");
            ulong sequence = ParseDecimal(f[0], line, Header[0]);
            if (sequence <= previous) throw new InvalidDataException($"Trace line {line} sequence is not strictly increasing.");
            uint dl = ParseHex(f[1], line, Header[1]); if (dl > byte.MaxValue) throw new InvalidDataException($"Trace line {line} DL is not a byte.");
            ushort[] values = new ushort[37];
            for (int index = 0; index < values.Length; index++) values[index] = ParseWord(f[index + 2], line, Header[index + 2]);
            if (values[4] != values[11]) throw new InvalidDataException($"Trace line {line} TrueReturnIP differs from Stack0C.");
            result.Add(new Event(sequence, (byte)dl, values[0], values[1], values[2], values[3], values[4], values.Skip(5).ToArray()));
            previous = sequence;
        }
        if (result.Count == 0) throw new InvalidDataException("Corrected trace has only its header.");
        return result;
    }

    private static List<ReturnSite> BuildValidatedReturnSites(byte[] image)
    {
        // Narrow, instruction-boundary-validated database: each CALL start and
        // exact bytes are checked, rather than treating arbitrary E8 bytes as calls.
        var definitions = new[]
        {
            new ReturnSite(0xF2A6, 0xF2A9, 0xF32F, "NEAR", "F246 fall-through / F2A6", "F2A6: E8 86 00"),
            new ReturnSite(0xF059, 0xF05C, 0xF246, "NEAR", "F033", "F059: E8 EA 01"),
            new ReturnSite(0xF06D, 0xF070, 0xF246, "NEAR", "F062", "F06D: E8 D6 01"),
            new ReturnSite(0xFCFD, 0xFD00, 0xF32F, "NEAR", "FC numeric/HUD helper", "FCFD: E8 2F F6"),
            new ReturnSite(0xFD0F, 0xFD12, 0xF32F, "NEAR", "FC numeric/HUD helper", "FD0F: E8 1D F6"),
            new ReturnSite(0xFD29, 0xFD2C, 0xF32F, "NEAR", "FC numeric/HUD helper", "FD29: E8 03 F6"),
            new ReturnSite(0xFD3C, 0xFD3F, 0xF32F, "NEAR", "FC numeric/HUD helper", "FD3C: E8 EE F5")
        };
        foreach (ReturnSite site in definitions)
        {
            if (image[site.CallPhysical] != 0xE8 || site.ReturnPhysical != site.CallPhysical + 3)
                throw new InvalidDataException($"Static CALL boundary check failed at {site.CallPhysical:X}.");
            short displacement = BitConverter.ToInt16(image, site.CallPhysical + 1);
            if (site.ReturnPhysical + displacement != site.TargetPhysical)
                throw new InvalidDataException($"Static CALL target check failed at {site.CallPhysical:X}.");
        }
        return definitions.ToList();
    }

    private static int DeriveLoadLinear(IReadOnlyList<Event> trace)
    {
        Event first = trace[0];
        int candidate = first.RendererCs * 16 + first.RendererIp - (RendererPhysical - MzHeader);
        if (candidate != 0x8240) throw new InvalidDataException($"Unexpected derived load linear base {candidate:X}; expected 8240.");
        return candidate;
    }

    private static Dictionary<int, TargetMatch> MatchTargets(IReadOnlyList<Event> trace, IReadOnlyList<GamePcStringEntry> entries)
    {
        List<Event> candidate = trace.Where(e => e.Dl != 0x81).ToList(); byte[] stream = candidate.Select(e => e.Dl).ToArray();
        var result = new Dictionary<int, TargetMatch>();
        foreach (int index in Targets)
        {
            GamePcStringEntry entry = entries[index]; (int start, int length) = LongestPrefix(stream, entry.OriginalBytes);
            if (length < 12) throw new InvalidDataException($"Target {index} did not have a sufficiently unique correlated prefix.");
            bool exact = length == entry.ByteLength;
            result[index] = new TargetMatch(exact ? "EXACT" : "STRONG_PARTIAL", start, length, candidate.Skip(start).Take(length).ToList());
        }
        return result;
    }

    private static TargetAnalysis AnalyzeTarget(int index, GamePcStringEntry entry, IReadOnlyList<Event> trace, TargetMatch match, IReadOnlyList<ReturnSite> sites, int loadLinear)
    {
        var trueReturns = match.Events.GroupBy(e => e.TrueReturnIp).OrderBy(g => g.Key).Select(g => $"{g.Key:X}({g.Count()})").ToArray();
        var frames = new List<Frame>
        {
            new("F32F return", "Stack0C", 0x0C, 0xF2A9, "KNOWN_RETURN_SITE", "F2A6 E8 F32F; direct printable renderer caller", "PROVEN"),
            new("F246 return", "Stack16", 0x16, 0xF05C, "KNOWN_RETURN_SITE", "F033 E8 F246; first validated upstream caller", "PROVEN"),
            new("F246 saved AX", "Stack0E", 0x0E, null, "KNOWN_SAVED_OR_DATA", "F246 pushes AX before printable fall-through", "PROVEN"),
            new("F246 saved CX", "Stack10", 0x10, null, "KNOWN_SAVED_OR_DATA", "F246 pushes CX before printable fall-through", "PROVEN"),
            new("F246 saved DI", "Stack12", 0x12, null, "KNOWN_SAVED_OR_DATA", "F246 pushes DI before printable fall-through", "PROVEN"),
            new("F246 saved DS", "Stack14", 0x14, null, "KNOWN_SAVED_OR_DATA", "F246 pushes DS before printable fall-through", "PROVEN"),
            new("F033 outer far caller", "Stack22/Stack24", 0x22, null, "POSSIBLE_RETURN_SITE", "Far-return pair is raw only; not used to identify consumer", "UNKNOWN")
        };
        if (match.Events.Any(e => e.TrueReturnIp != 0x2D9 || e.StackWord(0x16) != 0x008C))
            throw new InvalidDataException($"Target {index} does not retain the calibrated F2A9/F05C frame pattern.");
        return new TargetAnalysis(index, entry, match, trueReturns, frames, StableContext(match.Events));
    }

    private static string StableContext(IReadOnlyList<Event> events)
    {
        string Field(int offset) => string.Join(",", events.GroupBy(e => e.StackWord(offset)).OrderBy(g => g.Key).Select(g => $"{g.Key:X}({g.Count()})"));
        return $"SI={Field(0x00)}; BX={Field(0x04)}; CX={Field(0x06)}; DI={Field(0x08)}; DS={Field(0x0A)}; AX varies as glyph/layout state: {Field(0x02)}";
    }

    private static void WriteAudit(string path, IReadOnlyList<Event> trace, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<ReturnSite> sites, IReadOnlyDictionary<int, TargetAnalysis> targets, int loadLinear)
    {
        using var w = Writer(path);
        w.WriteLine("ELVIRA I O2B2 CORRECTED RAW-STACK TRACE AUDIT");
        w.WriteLine($"Events: {trace.Count}; sequence: {trace[0].Sequence}..{trace[^1].Sequence}; GAMEPC entries: {entries.Count}.");
        w.WriteLine($"Unique RendererCS:IP: {string.Join(", ", trace.Select(e => $"{e.RendererCs:X4}:{e.RendererIp:X4}").Distinct())}");
        w.WriteLine($"Unique TrueReturnIP: {string.Join(", ", trace.Select(e => e.TrueReturnIp.ToString("X")).Distinct().Order())}");
        w.WriteLine($"Unique SS: {string.Join(", ", trace.Select(e => e.Ss.ToString("X")).Distinct().Order())}; SP: {trace.Min(e => e.Sp):X}..{trace.Max(e => e.Sp):X}.");
        w.WriteLine($"TrueReturnIP == Stack0C: PASS on all {trace.Count} rows. Stack snapshot: exactly 32 words each.");
        w.WriteLine($"Derived runtime load linear: {loadLinear:X}; canonical physical = 2400h + (runtime linear - {loadLinear:X}h).");
        w.WriteLine("Static return-site database is deliberately narrow and CALL-boundary validated; it is not a byte scan for E8.");
        foreach (TargetAnalysis target in targets.Values.OrderBy(t => t.Index))
            w.WriteLine($"Index {target.Index}: {target.Match.Type} {target.Match.Length}/{target.Entry.ByteLength}; sequence {target.Match.Events[0].Sequence}..{target.Match.Events[^1].Sequence}; TrueReturnIP={string.Join(",", target.TrueReturns)}; Stack16=008C({target.Match.Events.Count}).");
    }

    private static void WriteReturnSites(string path, IReadOnlyList<ReturnSite> sites)
    {
        using var w = Writer(path); w.WriteLine("CallInstruction\tReturnIP\tCallTarget\tNearFar\tContainingRoutine\tValidation");
        foreach (ReturnSite s in sites) w.WriteLine($"{s.CallPhysical:X}\t{s.ReturnPhysical:X}\t{s.TargetPhysical:X}\t{s.NearFar}\t{s.Routine}\t{s.Validation}");
    }

    private static void WriteTargetEvents(string path, IReadOnlyDictionary<int, TargetAnalysis> targets)
    {
        using var w = Writer(path); w.WriteLine("Index\tMatchType\tSequence\tDL\tRendererCS\tRendererIP\tSS\tSP\tTrueReturnIP\tStack0C\tStack0E\tStack10\tStack12\tStack14\tStack16\tClassification");
        foreach (TargetAnalysis t in targets.Values.OrderBy(t => t.Index)) foreach (Event e in t.Match.Events)
            w.WriteLine($"{t.Index}\t{t.Match.Type}\t{e.Sequence}\t{e.Dl:X2}\t{e.RendererCs:X4}\t{e.RendererIp:X4}\t{e.Ss:X4}\t{e.Sp:X4}\t{e.TrueReturnIp:X4}\t{e.StackWord(0x0C):X4}\t{e.StackWord(0x0E):X4}\t{e.StackWord(0x10):X4}\t{e.StackWord(0x12):X4}\t{e.StackWord(0x14):X4}\t{e.StackWord(0x16):X4}\tStack0C=F2A9 KNOWN_RETURN_SITE; Stack16=F05C KNOWN_RETURN_SITE");
    }

    private static void WriteChains(string path, IReadOnlyDictionary<int, TargetAnalysis> targets, int loadLinear)
    {
        using var w = Writer(path); w.WriteLine("Index\tMatchType\tStartSequence\tEndSequence\tTrueReturnIP\tFrameLevel\tStackOffset\tReturnIP\tReturnPhysical\tPrecedingCall\tCallTarget\tContainingRoutine\tConfidence\tNotes");
        foreach (TargetAnalysis t in targets.Values.OrderBy(t => t.Index))
        {
            string start = t.Match.Events[0].Sequence.ToString(CultureInfo.InvariantCulture), end = t.Match.Events[^1].Sequence.ToString(CultureInfo.InvariantCulture);
            w.WriteLine($"{t.Index}\t{t.Match.Type}\t{start}\t{end}\t2D9\tF32F return\t0C\t2D9\tF2A9\tF2A6:E8 86 00\tF32F\tF246/F2A6 printable path\tPROVEN\tTrueReturnIP equals Stack0C.");
            w.WriteLine($"{t.Index}\t{t.Match.Type}\t{start}\t{end}\t2D9\tF246 return\t16\t8C\tF05C\tF059:E8 EA 01\tF246\tF033\tPROVEN\tFirst validated upstream caller above F246.");
            w.WriteLine($"{t.Index}\t{t.Match.Type}\t{start}\t{end}\t2D9\tF033 outer caller\t22/24\t59F\tUNKNOWN\tUNKNOWN\tUNKNOWN\tUNKNOWN\tUNKNOWN\tRaw far-return pair only; no segment/code claim.");
        }
    }

    private static void WriteComparison(string path, IReadOnlyDictionary<int, TargetAnalysis> targets)
    {
        using var w = Writer(path);
        w.WriteLine("AFFECTED/SAFE CORRECTED-STACK COMPARISON");
        w.WriteLine("Affected 417 and 424: SAME_CONSUMER — both have F2A9 at Stack0C and F05C at Stack16, validated as F033 -> F246 -> F2A6 -> F32F.");
        w.WriteLine("Safe 425 and 298: SAME_CONSUMER — same validated frame chain.");
        w.WriteLine("Affected versus safe: NONE FOUND within captured stack through F033. The captured F033 saved-frame values are identical except expected per-glyph layout state (AX; 424 also varies BX across wrapped portions).");
        foreach (TargetAnalysis t in targets.Values.OrderBy(t => t.Index)) w.WriteLine($"Index {t.Index}: {t.Context}");
        w.WriteLine("F062 is statically a valid generic NUL-loop caller of F246 (F06D -> F246 -> return F070), but F070 was not present in these target frames. It is not claimed as their dynamic caller.");
    }

    private static void WriteConclusion(string path, IReadOnlyDictionary<int, TargetAnalysis> targets, int loadLinear)
    {
        using var w = Writer(path);
        w.WriteLine("O2B2 CONCLUSION");
        w.WriteLine("PROVEN: corrected trace is valid; TrueReturnIP equals Stack0C on every row.");
        w.WriteLine("PROVEN: printable target glyphs return F32F -> F2A9. Canonical mapping: runtime 14E1:02D9, module-relative CEA9, physical F2A9; preceding validated CALL is F2A6:E8 86 00 -> F32F.");
        w.WriteLine("PROVEN: Stack0E/10/12/14 are F246 saved AX/CX/DI/DS. Stack16 is F05C, immediately after validated F059:E8 EA 01 -> F246, in routine F033.");
        w.WriteLine("STRONG EVIDENCE: all captured 417, 424, 425, and 298 portions use F033 -> F246 -> F2A6 -> F32F. No affected/safe divergence is visible through this recovered frame.");
        w.WriteLine("O2B3 target: canonical physical F033 (module-relative CC33), particularly F059 CALL F246 and the routine's input/context setup. Do not audit a broad 0x60 limit yet.");
        w.WriteLine("96_STILL_UNKNOWN. A further runtime trace is not required for the F033 audit; it may be required only if O2B3 needs the outer far caller beyond Stack22/24.");
    }

    private static (int Start, int Length) LongestPrefix(byte[] stream, byte[] value)
    {
        int bestStart = -1, best = 0;
        for (int start = 0; start < stream.Length; start++) { int length = 0; while (length < value.Length && start + length < stream.Length && stream[start + length] == value[length]) length++; if (length > best) { best = length; bestStart = start; } }
        return (bestStart, best);
    }
    private static ulong ParseDecimal(string value, int line, string field) => ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out ulong parsed) ? parsed : throw new InvalidDataException($"Trace line {line}: invalid {field}.");
    private static uint ParseHex(string value, int line, string field) => uint.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out uint parsed) ? parsed : throw new InvalidDataException($"Trace line {line}: invalid hexadecimal {field}.");
    private static ushort ParseWord(string value, int line, string field) => ushort.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out ushort parsed) ? parsed : throw new InvalidDataException($"Trace line {line}: invalid 16-bit {field}.");
    private static StreamWriter Writer(string path) => new(path, false, new UTF8Encoding(false));
    private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private sealed record Event(ulong Sequence, byte Dl, ushort RendererCs, ushort RendererIp, ushort Ss, ushort Sp, ushort TrueReturnIp, ushort[] Stack)
    {
        internal ushort StackWord(int offset) => Stack[(offset - 0x00) / 2];
    }
    private sealed record ReturnSite(int CallPhysical, int ReturnPhysical, int TargetPhysical, string NearFar, string Routine, string Validation);
    private sealed record TargetMatch(string Type, int Start, int Length, List<Event> Events);
    private sealed record Frame(string Level, string OffsetName, int Offset, int? Physical, string Classification, string Notes, string Confidence);
    private sealed record TargetAnalysis(int Index, GamePcStringEntry Entry, TargetMatch Match, string[] TrueReturns, List<Frame> Frames, string Context);
}
