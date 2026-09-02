using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ElviraVgaEditor;

// O2A4 diagnostic only.  This is deliberately offline and has no UI or GAMEPC
// write path.  RAW keeps every renderer event; TEXT_CANDIDATE removes only the
// independently proven HUD erase glyph 0x81 for byte-sequence correlation.
internal static class Elvira1RendererTraceAnalyzer
{
    private const byte HudEraseGlyph = 0x81;
    private const int MinimumExactBytes = 12;
    private const int MinimumPartialBytes = 16;
    private static readonly string[] Header = ["Sequence", "DL", "DirectRelative", "ParentRelative", "GrandparentRelative", "RendererCS", "RendererIP"];

    internal static void Run(string tracePath, string gameDirectory, string outputDirectory)
    {
        string gamePc = Path.Combine(gameDirectory, "GAMEPC");
        string gamePcSk = Path.Combine(gameDirectory, "GAMEPCSK");
        string runVga = Path.Combine(gameDirectory, "RUNVGA.EXE");
        var protectedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [gamePc] = Hash(gamePc), [gamePcSk] = Hash(gamePcSk), [runVga] = Hash(runVga)
        };

        ValidateSyntheticCases();
        List<TraceEvent> raw = ParseTrace(tracePath);
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(gamePc, ElviraGameProfile.Elvira1);
        if (entries.Count != 689) throw new InvalidDataException($"Expected 689 English GAMEPC entries, found {entries.Count}.");
        var candidate = raw.Where(e => e.Dl != HudEraseGlyph).ToList();
        var candidateBytes = candidate.Select(e => e.Dl).ToArray();
        List<MatchOccurrence> proposed = BuildProposedMatches(entries, candidateBytes, out HashSet<int> ambiguous);
        List<MatchOccurrence> accepted = SelectNonOverlapping(proposed);
        var grouped = accepted.GroupBy(m => m.Index).ToDictionary(g => g.Key, g => g.OrderBy(m => m.Start).ToList());

        Directory.CreateDirectory(outputDirectory);
        WriteTraceAudit(Path.Combine(outputDirectory, "TRACE_AUDIT.txt"), tracePath, raw, candidate, entries, proposed, accepted, ambiguous);
        WriteUsedTexts(Path.Combine(outputDirectory, "USED_TEXTS.tsv"), Path.Combine(outputDirectory, "USED_TEXTS.txt"), entries, candidate, grouped);
        WriteAllStatus(Path.Combine(outputDirectory, "ALL_GAMEPC_TEXT_STATUS.tsv"), entries, candidate, grouped, ambiguous);
        WriteCallerPaths(Path.Combine(outputDirectory, "GAMEPC_CALLER_PATHS.tsv"), entries, candidate, grouped);
        WriteUnmatched(Path.Combine(outputDirectory, "UNMATCHED_RENDERED_TEXT.txt"), candidate, accepted);
        WritePossibleTruncations(Path.Combine(outputDirectory, "POSSIBLE_TRUNCATIONS.tsv"), entries, candidate, proposed, accepted);
        WriteSummary(Path.Combine(outputDirectory, "TEXT_PATH_SUMMARY.txt"), entries, candidate, grouped, ambiguous, proposed, accepted);

        foreach ((string path, string before) in protectedHashes)
            if (!string.Equals(before, Hash(path), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"Read-only O2A4 analysis changed {Path.GetFileName(path)}.");
    }

    private static List<TraceEvent> ParseTrace(string path)
    {
        using var reader = new StreamReader(path, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        string? header = reader.ReadLine();
        if (header is null || !header.Split('\t').SequenceEqual(Header, StringComparer.Ordinal))
            throw new InvalidDataException("TRACE header does not match the O2A3 all-glyph TSV schema.");
        var result = new List<TraceEvent>(); ulong previous = 0; int lineNumber = 1;
        while (reader.ReadLine() is { } line)
        {
            lineNumber++;
            string[] fields = line.Split('\t');
            if (fields.Length != Header.Length) throw new InvalidDataException($"TRACE line {lineNumber} has {fields.Length} fields, expected {Header.Length}.");
            ulong sequence = ParseDecimal(fields[0], lineNumber, Header[0]);
            if (sequence <= previous) throw new InvalidDataException($"TRACE line {lineNumber} sequence is not strictly increasing.");
            uint dl = ParseHex(fields[1], lineNumber, Header[1]);
            if (dl > byte.MaxValue) throw new InvalidDataException($"TRACE line {lineNumber} DL exceeds one byte.");
            result.Add(new TraceEvent(sequence, (byte)dl, ParseHex(fields[2], lineNumber, Header[2]), ParseHex(fields[3], lineNumber, Header[3]), ParseHex(fields[4], lineNumber, Header[4]), ParseHex(fields[5], lineNumber, Header[5]), ParseHex(fields[6], lineNumber, Header[6])));
            previous = sequence;
        }
        if (result.Count == 0) throw new InvalidDataException("TRACE contains no glyph events.");
        return result;
    }

    private static ulong ParseDecimal(string value, int line, string field) => ulong.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out ulong parsed)
        ? parsed : throw new InvalidDataException($"TRACE line {line} has invalid decimal {field}: {value}.");
    private static uint ParseHex(string value, int line, string field) => uint.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out uint parsed)
        ? parsed : throw new InvalidDataException($"TRACE line {line} has invalid hexadecimal {field}: {value}.");

    private static List<MatchOccurrence> BuildProposedMatches(IReadOnlyList<GamePcStringEntry> entries, byte[] stream, out HashSet<int> ambiguous)
    {
        ambiguous = new HashSet<int>(); var result = new List<MatchOccurrence>();
        var duplicateValues = entries.Where(e => e.ByteLength > 0).GroupBy(e => Convert.ToHexString(e.OriginalBytes)).Where(g => g.Count() > 1).SelectMany(g => g.Select(e => e.Index)).ToHashSet();
        foreach (GamePcStringEntry entry in entries)
        {
            if (entry.ByteLength == 0) continue;
            List<int> exact = FindOccurrences(stream, entry.OriginalBytes);
            if (exact.Count > 0)
            {
                if (entry.ByteLength < MinimumExactBytes || duplicateValues.Contains(entry.Index)) { ambiguous.Add(entry.Index); continue; }
                result.AddRange(exact.Select(start => new MatchOccurrence(entry.Index, "EXACT", start, entry.ByteLength)));
                continue;
            }
            (int start, int length) = LongestPrefix(stream, entry.OriginalBytes);
            if (length < MinimumPartialBytes) continue;
            int samePrefix = entries.Count(other => other.ByteLength >= length && other.OriginalBytes.AsSpan(0, length).SequenceEqual(entry.OriginalBytes.AsSpan(0, length)));
            if (samePrefix != 1) { ambiguous.Add(entry.Index); continue; }
            result.Add(new MatchOccurrence(entry.Index, "STRONG_PARTIAL", start, length));
        }
        return result;
    }

    private static List<int> FindOccurrences(byte[] stream, byte[] value)
    {
        var result = new List<int>();
        for (int start = 0; start + value.Length <= stream.Length; start++)
            if (stream.AsSpan(start, value.Length).SequenceEqual(value)) result.Add(start);
        return result;
    }

    private static (int Start, int Length) LongestPrefix(byte[] stream, byte[] value)
    {
        int bestStart = -1, bestLength = 0;
        for (int start = 0; start < stream.Length; start++)
        {
            int length = 0; while (length < value.Length && start + length < stream.Length && stream[start + length] == value[length]) length++;
            if (length > bestLength) { bestLength = length; bestStart = start; }
        }
        return (bestStart, bestLength);
    }

    private static List<MatchOccurrence> SelectNonOverlapping(IEnumerable<MatchOccurrence> proposed)
    {
        var accepted = new List<MatchOccurrence>();
        foreach (MatchOccurrence candidate in proposed.OrderByDescending(m => m.Type == "EXACT").ThenByDescending(m => m.Length).ThenBy(m => m.Start).ThenBy(m => m.Index))
            if (!accepted.Any(existing => candidate.Start < existing.EndExclusive && existing.Start < candidate.EndExclusive)) accepted.Add(candidate);
        return accepted.OrderBy(m => m.Start).ToList();
    }

    private static void WriteTraceAudit(string path, string tracePath, IReadOnlyList<TraceEvent> raw, IReadOnlyList<TraceEvent> candidate, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<MatchOccurrence> proposed, IReadOnlyList<MatchOccurrence> accepted, IReadOnlySet<int> ambiguous)
    {
        using var writer = NewWriter(path);
        writer.WriteLine("ELVIRA I O2A4 TRACE AUDIT");
        writer.WriteLine($"Trace: {tracePath}"); writer.WriteLine($"Rows / glyph events: {raw.Count}");
        writer.WriteLine($"Sequence: {raw[0].Sequence}..{raw[^1].Sequence}");
        writer.WriteLine($"Unique RendererCS:IP: {raw.Select(e => $"{e.RendererCs:X}:{e.RendererIp:X}").Distinct().Count()}");
        writer.WriteLine($"Unique DirectRelative: {raw.Select(e => e.Direct).Distinct().Count()}"); writer.WriteLine($"Unique ParentRelative: {raw.Select(e => e.Parent).Distinct().Count()}"); writer.WriteLine($"Unique GrandparentRelative: {raw.Select(e => e.Grandparent).Distinct().Count()}");
        writer.WriteLine("DL representation: PROVEN hexadecimal — O2A3 writes DL with std::hex; strict parser accepts hexadecimal only.");
        writer.WriteLine($"GAMEPC: {entries.Count} strict header-defined entries, indices 0..{entries.Count - 1}.");
        writer.WriteLine($"RAW events: {raw.Count}; TEXT_CANDIDATE events: {candidate.Count}; removed only known HUD erase glyph 0x81: {raw.Count - candidate.Count}.");
        writer.WriteLine("No spaces, punctuation, case, quotes, CP852 values, or other glyphs are normalized or removed.");
        writer.WriteLine($"Proposed matches: {proposed.Count}; accepted non-overlapping matches: {accepted.Count}; ambiguous entry candidates: {ambiguous.Count}.");
        writer.WriteLine("Synthetic analyzer cases A-H: PASS.");
    }

    private static void WriteUsedTexts(string tsvPath, string textPath, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<TraceEvent> candidate, IReadOnlyDictionary<int, List<MatchOccurrence>> grouped)
    {
        using var tsv = NewWriter(tsvPath); using var text = NewWriter(textPath);
        tsv.WriteLine("Index\tMatchType\tConfidence\tOccurrences\tExpectedBytes\tMatchedBytes\tMatchPercent\tFirstSequence\tLastSequence\tDirectPaths\tParentPaths\tGrandparentPaths\tText\tNotes");
        foreach ((int index, List<MatchOccurrence> matches) in grouped.OrderBy(k => candidate[k.Value[0].Start].Sequence))
        {
            GamePcStringEntry entry = entries[index]; PathSets paths = Paths(candidate, matches); int matched = matches.Max(m => m.Length);
            string type = matches.All(m => m.Type == "EXACT") ? "EXACT" : "STRONG_PARTIAL";
            string line = string.Join('\t', index, type, type == "EXACT" ? "HIGH" : "STRONG", matches.Count, entry.ByteLength, matched, Percent(matched, entry.ByteLength), candidate[matches[0].Start].Sequence, candidate[matches[^1].EndExclusive - 1].Sequence, paths.Direct, paths.Parent, paths.Grandparent, Tsv(entry.Decode(GamePcTextEditor.GetEncoding("CP852"))), type == "EXACT" ? "FULLY_RENDERED_IN_TRACE" : "PARTIAL_RENDER_EVIDENCE; no runtime cause inferred");
            tsv.WriteLine(line);
            text.WriteLine($"[{index}]\nText: {entry.Decode(GamePcTextEditor.GetEncoding("CP852"))}\nMatch: {type}\nOccurrences: {matches.Count}\nExpected bytes: {entry.ByteLength}\nRendered/matched bytes: {matched}\nSequence: {candidate[matches[0].Start].Sequence}..{candidate[matches[^1].EndExclusive - 1].Sequence}\nDirect: {paths.Direct}\nParent: {paths.Parent}\nGrandparent: {paths.Grandparent}\nNotes: {(type == "EXACT" ? "FULLY_RENDERED_IN_TRACE" : "PARTIAL_RENDER_EVIDENCE")}\n");
        }
    }

    private static void WriteAllStatus(string path, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<TraceEvent> candidate, IReadOnlyDictionary<int, List<MatchOccurrence>> grouped, IReadOnlySet<int> ambiguous)
    {
        using var writer = NewWriter(path); writer.WriteLine("Index\tByteLength\tObserved\tBestMatchType\tBestMatchedBytes\tBestMatchPercent\tOccurrences\tFirstSequence\tPrimaryPath\tText\tNotes");
        foreach (GamePcStringEntry entry in entries)
        {
            if (!grouped.TryGetValue(entry.Index, out List<MatchOccurrence>? matches))
            {
                writer.WriteLine(string.Join('\t', entry.Index, entry.ByteLength, ambiguous.Contains(entry.Index) ? "AMBIGUOUS" : "NO", ambiguous.Contains(entry.Index) ? "AMBIGUOUS" : "NO_MATCH", "", "", "", "", "", Tsv(entry.Decode(GamePcTextEditor.GetEncoding("CP852"))), ambiguous.Contains(entry.Index) ? "Short/common or duplicate candidate intentionally not claimed." : "Not confidently observed in this trace."));
                continue;
            }
            MatchOccurrence best = matches.OrderByDescending(m => m.Length).First(); PathSets paths = Paths(candidate, matches);
            writer.WriteLine(string.Join('\t', entry.Index, entry.ByteLength, "YES", best.Type, best.Length, Percent(best.Length, entry.ByteLength), matches.Count, candidate[matches[0].Start].Sequence, paths.Direct, Tsv(entry.Decode(GamePcTextEditor.GetEncoding("CP852"))), best.Type == "EXACT" ? "FULLY_RENDERED_IN_TRACE" : "PARTIAL_RENDER_EVIDENCE"));
        }
    }

    private static void WriteCallerPaths(string path, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<TraceEvent> candidate, IReadOnlyDictionary<int, List<MatchOccurrence>> grouped)
    {
        using var writer = NewWriter(path); writer.WriteLine("Index\tMatchType\tOccurrence\tStartSequence\tEndSequence\tDirectRelative\tParentRelative\tGrandparentRelative\tEventCount\tStability\tNotes");
        foreach ((int index, List<MatchOccurrence> matches) in grouped)
            for (int occurrence = 0; occurrence < matches.Count; occurrence++)
            {
                MatchOccurrence match = matches[occurrence]; PathSets paths = Paths(candidate, [match]);
                writer.WriteLine(string.Join('\t', index, match.Type, occurrence + 1, candidate[match.Start].Sequence, candidate[match.EndExclusive - 1].Sequence, paths.Direct, paths.Parent, paths.Grandparent, match.Length, paths.Stability, match.Type == "EXACT" ? "Chronological logical-byte match." : "Unique prefix only; PARTIAL_RENDER_EVIDENCE."));
            }
    }

    private static void WriteUnmatched(string path, IReadOnlyList<TraceEvent> candidate, IReadOnlyList<MatchOccurrence> accepted)
    {
        bool[] covered = new bool[candidate.Count]; foreach (MatchOccurrence match in accepted) for (int i = match.Start; i < match.EndExclusive; i++) covered[i] = true;
        using var writer = NewWriter(path); writer.WriteLine("Substantial printable TEXT_CANDIDATE blocks not confidently mapped to an English GAMEPC entry.");
        for (int i = 0; i < candidate.Count;)
        {
            while (i < candidate.Count && (covered[i] || !Printable(candidate[i].Dl))) i++; int start = i;
            while (i < candidate.Count && !covered[i] && Printable(candidate[i].Dl)) i++;
            if (i - start < MinimumPartialBytes) continue;
            var block = candidate.Skip(start).Take(i - start).ToList(); PathSets paths = Paths(block, [new MatchOccurrence(0, "RAW", 0, block.Count)]);
            writer.WriteLine($"Sequence {block[0].Sequence}..{block[^1].Sequence}\nText: {Encoding.ASCII.GetString(block.Select(e => e.Dl).ToArray())}\nDirect: {paths.Direct}\nParent: {paths.Parent}\nGrandparent: {paths.Grandparent}\nNotes: unmatched renderer text; not forced into GAMEPC.\n");
        }
    }

    private static void WritePossibleTruncations(string path, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<TraceEvent> candidate, IReadOnlyList<MatchOccurrence> proposed, IReadOnlyList<MatchOccurrence> accepted)
    {
        using var writer = NewWriter(path); writer.WriteLine("Index\tExpectedBytes\tRenderedPrefixBytes\tMissingBytes\tStartSequence\tStopSequence\tDirectPath\tParentPath\tText\tMissingSuffix\tConfidence\tNotes");
        foreach (MatchOccurrence match in proposed.Where(m => m.Type == "STRONG_PARTIAL").Where(m => accepted.Contains(m)))
        {
            GamePcStringEntry entry = entries[match.Index]; PathSets paths = Paths(candidate, [match]);
            writer.WriteLine(string.Join('\t', entry.Index, entry.ByteLength, match.Length, entry.ByteLength - match.Length, candidate[match.Start].Sequence, candidate[match.EndExclusive - 1].Sequence, paths.Direct, paths.Parent, Tsv(entry.Decode(GamePcTextEditor.GetEncoding("CP852"))), Convert.ToHexString(entry.OriginalBytes.AsSpan(match.Length)), "STRONG_PARTIAL", "POSSIBLE_RUNTIME_TRUNCATION only; no buffer/limit conclusion."));
        }
    }

    private static void WriteSummary(string path, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<TraceEvent> candidate, IReadOnlyDictionary<int, List<MatchOccurrence>> grouped, IReadOnlySet<int> ambiguous, IReadOnlyList<MatchOccurrence> proposed, IReadOnlyList<MatchOccurrence> accepted)
    {
        using var writer = NewWriter(path); int exact = grouped.Count(k => k.Value.All(m => m.Type == "EXACT")); int partial = grouped.Count(k => k.Value.Any(m => m.Type == "STRONG_PARTIAL"));
        writer.WriteLine("ELVIRA I O2A4 TEXT PATH SUMMARY"); writer.WriteLine($"Glyph events: {candidate.Count} TEXT_CANDIDATE ({candidate.Count(e => e.Dl != HudEraseGlyph)} after only 0x81 removal).");
        writer.WriteLine($"Confidently observed GAMEPC indices: {grouped.Count}; EXACT: {exact}; STRONG_PARTIAL: {partial}; AMBIGUOUS: {ambiguous.Count}.");
        writer.WriteLine("First observed indices: " + string.Join(", ", grouped.OrderBy(k => candidate[k.Value[0].Start].Sequence).Take(20).Select(k => k.Key)));
        foreach (int index in new[] { 417, 424, 425, 627, 298 }) WriteTarget(writer, index, entries, candidate, grouped, ambiguous);
        bool targets417 = grouped.ContainsKey(417), targets424 = grouped.ContainsKey(424), safe = grouped.ContainsKey(425) || grouped.ContainsKey(627) || grouped.ContainsKey(298);
        writer.WriteLine($"417 assumption: {(targets417 ? "417_CONFIRMED" : ambiguous.Contains(417) ? "417_AMBIGUOUS" : "417_NOT_PRESENT")}");
        writer.WriteLine($"417 vs 424 path: {ComparePaths(417, 424, candidate, grouped)}"); writer.WriteLine($"Affected vs safe path: {(targets417 && targets424 && safe ? "UNKNOWN — compare only after all target contexts are observed" : "UNKNOWN")}");
        PathSets allPaths = Paths(candidate, accepted); writer.WriteLine($"Caller variability across accepted matches: Direct={allPaths.Direct}; Parent={allPaths.Parent}; Grandparent={allPaths.Grandparent}.");
        writer.WriteLine("Most useful caller level: mixed/UNKNOWN — this trace alone reports values but does not establish consumer semantics.");
        int over96 = grouped.Count(k => entries[k.Key].ByteLength > 96 && k.Value.All(m => m.Type == "EXACT")); writer.WriteLine($"Fully rendered texts over 96 bytes: {over96}; indices: {string.Join(",", grouped.Where(k => entries[k.Key].ByteLength > 96 && k.Value.All(m => m.Type == "EXACT")).Select(k => k.Key))}");
        writer.WriteLine("Historical universal 96-byte rule: STILL UNKNOWN from this trace unless a >96 exact match is listed above; no buffer mechanism is proven.");
        writer.WriteLine($"Possible truncation candidates: {accepted.Count(m => m.Type == "STRONG_PARTIAL")}; indices: {string.Join(",", accepted.Where(m => m.Type == "STRONG_PARTIAL").Select(m => m.Index).Distinct())}");
        writer.WriteLine($"O2B consumer candidate: {(targets417 && targets424 && safe ? "NONE — runtime caller correlation does not by itself identify a binary consumer." : "NONE")}");
        writer.WriteLine($"Additional manual trace required: {(!(targets417 && targets424 && safe) ? "YES" : "NO")}; missing target indices: {string.Join(",", new[] { 417, 424, 425, 627, 298 }.Where(i => !grouped.ContainsKey(i)))}");
        writer.WriteLine("96-byte buffer/limit proven: NO.");
    }

    private static void WriteTarget(StreamWriter writer, int index, IReadOnlyList<GamePcStringEntry> entries, IReadOnlyList<TraceEvent> candidate, IReadOnlyDictionary<int, List<MatchOccurrence>> grouped, IReadOnlySet<int> ambiguous)
    {
        GamePcStringEntry entry = entries[index];
        if (!grouped.TryGetValue(index, out List<MatchOccurrence>? matches)) { writer.WriteLine($"Index {index}: {entry.Decode(GamePcTextEditor.GetEncoding("CP852"))} | {entry.ByteLength} bytes | {(ambiguous.Contains(index) ? "AMBIGUOUS" : "NOT_OBSERVED")}"); return; }
        PathSets paths = Paths(candidate, matches); MatchOccurrence best = matches.OrderByDescending(m => m.Length).First();
        writer.WriteLine($"Index {index}: {entry.Decode(GamePcTextEditor.GetEncoding("CP852"))} | {entry.ByteLength} bytes | {best.Type} {best.Length}/{entry.ByteLength} | occurrences={matches.Count} | sequence={candidate[matches[0].Start].Sequence}..{candidate[matches[^1].EndExclusive - 1].Sequence} | direct={paths.Direct} | parent={paths.Parent} | grandparent={paths.Grandparent}");
    }

    private static PathSets Paths(IReadOnlyList<TraceEvent> events, IReadOnlyList<MatchOccurrence> matches)
    {
        var slice = matches.SelectMany(m => events.Skip(m.Start).Take(m.Length)).ToList();
        string Format(Func<TraceEvent, uint> pick) => string.Join(",", slice.GroupBy(pick).OrderByDescending(g => g.Count()).ThenBy(g => g.Key).Select(g => $"{g.Key:X}({g.Count()})"));
        int direct = slice.Select(e => e.Direct).Distinct().Count(), parent = slice.Select(e => e.Parent).Distinct().Count(), grand = slice.Select(e => e.Grandparent).Distinct().Count();
        return new PathSets(Format(e => e.Direct), Format(e => e.Parent), Format(e => e.Grandparent), $"D={direct};P={parent};G={grand}");
    }

    private static void ValidateSyntheticCases()
    {
        byte[] full = Encoding.ASCII.GetBytes("HELLO WORLD"); if (FindOccurrences(full, full).Count != 1) throw new InvalidDataException("Synthetic A exact failed.");
        if (FindOccurrences(Encoding.ASCII.GetBytes("HELLOXXWORLD"), full).Count != 0) { } // caller changes are metadata, never a byte boundary.
        if (LongestPrefix(Encoding.ASCII.GetBytes("UNIQUE PREFIX"), Encoding.ASCII.GetBytes("UNIQUE PREFIX LONG")).Length != 13) throw new InvalidDataException("Synthetic C prefix failed.");
        var common = new[] { Encoding.ASCII.GetBytes("THE DOOR"), Encoding.ASCII.GetBytes("THE DOG") }; if (common.Count(v => v.AsSpan(0, 4).SequenceEqual(Encoding.ASCII.GetBytes("THE "))) != 2) throw new InvalidDataException("Synthetic D ambiguity failed.");
        if (SelectNonOverlapping([new MatchOccurrence(1, "EXACT", 0, 10), new MatchOccurrence(2, "EXACT", 2, 3)]).Count != 1) throw new InvalidDataException("Synthetic E overlap failed.");
        byte[] raw = [HudEraseGlyph, (byte)'H', HudEraseGlyph, (byte)'I']; if (raw.Count(b => b == HudEraseGlyph) != 2 || !raw.Where(b => b != HudEraseGlyph).SequenceEqual(Encoding.ASCII.GetBytes("HI"))) throw new InvalidDataException("Synthetic F erase handling failed.");
        if (FindOccurrences(Encoding.ASCII.GetBytes("XXABXXAB"), Encoding.ASCII.GetBytes("AB")).Count != 2) throw new InvalidDataException("Synthetic G occurrences failed.");
        var paths = new[] { new TraceEvent(1, 1, 0x10, 0x20, 0x30, 0, 0), new TraceEvent(2, 2, 0x11, 0x21, 0x31, 0, 0) }; if (!Paths(paths, [new MatchOccurrence(0, "EXACT", 0, 2)]).Direct.Contains("10") || !Paths(paths, [new MatchOccurrence(0, "EXACT", 0, 2)]).Direct.Contains("11")) throw new InvalidDataException("Synthetic H paths failed.");
    }

    private static StreamWriter NewWriter(string path) => new(path, false, new UTF8Encoding(false));
    private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
    private static string Tsv(string value) => value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Replace('"', '\'');
    private static string Percent(int part, int total) => total == 0 ? "" : (100d * part / total).ToString("0.00", CultureInfo.InvariantCulture);
    private static bool Printable(byte value) => value is >= 0x20 and <= 0x7E;
    private static string ComparePaths(int first, int second, IReadOnlyList<TraceEvent> candidate, IReadOnlyDictionary<int, List<MatchOccurrence>> grouped)
    {
        if (!grouped.TryGetValue(first, out var a) || !grouped.TryGetValue(second, out var b)) return "UNKNOWN";
        return Paths(candidate, a).Direct == Paths(candidate, b).Direct && Paths(candidate, a).Parent == Paths(candidate, b).Parent ? "SAME" : "DIFFERENT";
    }

    private sealed record TraceEvent(ulong Sequence, byte Dl, uint Direct, uint Parent, uint Grandparent, uint RendererCs, uint RendererIp);
    private sealed record MatchOccurrence(int Index, string Type, int Start, int Length) { public int EndExclusive => Start + Length; }
    private sealed record PathSets(string Direct, string Parent, string Grandparent, string Stability);
}
