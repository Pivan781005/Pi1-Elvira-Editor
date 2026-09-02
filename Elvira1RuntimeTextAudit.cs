using System.Text;

namespace ElviraVgaEditor;

// O1 diagnostic only.  It deliberately does not alter the production Runtime
// classifier: code-flow evidence is incomplete, so unproven entries remain
// UNCLASSIFIED rather than inheriting a limit from their byte length.
internal static class Elvira1RuntimeTextAudit
{
    private const string ObservedTruncationPath = "TextPath_ObservedTruncation";
    private const string HistoricalSafePath = "TextPath_HistoricalSafe";

    public static void Run(string gameDirectory, string outputDirectory)
    {
        string englishPath = Path.Combine(gameDirectory, "GAMEPC");
        string slovakPath = Path.Combine(gameDirectory, "GAMEPCSK");
        string runVgaPath = Path.Combine(gameDirectory, "RUNVGA.EXE");
        Encoding encoding = GamePcTextEditor.GetEncoding("CP852");
        List<GamePcStringEntry> english = GamePcTextEditor.LoadEntries(englishPath, ElviraGameProfile.Elvira1);
        List<GamePcStringEntry>? slovak = File.Exists(slovakPath) ? GamePcTextEditor.LoadEntries(slovakPath, ElviraGameProfile.Elvira1) : null;
        if (english.Count != 689 || slovak is not null && slovak.Count != english.Count)
            throw new InvalidDataException("Elvira I GAMEPC logical-entry count does not match the proven 689-entry model.");

        var before = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [englishPath] = Hash(englishPath), [runVgaPath] = Hash(runVgaPath)
        };
        if (slovak is not null) before[slovakPath] = Hash(slovakPath);
        Directory.CreateDirectory(outputDirectory);
        WriteClassification(Path.Combine(outputDirectory, "E1_RUNTIME_TEXT_CLASSIFICATION.tsv"), english, slovak, encoding);
        WritePaths(Path.Combine(outputDirectory, "E1_RUNTIME_TEXT_PATHS.tsv"));
        WriteReport(Path.Combine(outputDirectory, "E1_RUNTIME_TEXT_TRUNCATION_AUDIT.txt"), english, slovak, encoding, before);
        foreach ((string path, string hash) in before)
            if (!string.Equals(hash, Hash(path), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"Read-only O1 audit changed {Path.GetFileName(path)}.");
    }

    private static void WriteClassification(string path, IReadOnlyList<GamePcStringEntry> english, IReadOnlyList<GamePcStringEntry>? slovak, Encoding encoding)
    {
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("Index\tEnglishBytes\tSlovakBytes\tDeltaBytes\tCurrentRuntimeStatus\tRuntimeClass\tRuntimePath\tHardLimitBytes\tSafePayloadBytes\tRiskStatus\tCodeEvidence\tConfidence\tNotes\tEnglishText\tSlovakText");
        for (int index = 0; index < english.Count; index++)
        {
            string en = english[index].Decode(encoding); string? sk = slovak?[index].Decode(encoding);
            int? skBytes = slovak?[index].ByteLength;
            TextDiagnosticResult current = Elvira1TextMetadata.Evaluate(index, sk ?? en, encoding, ignored: false);
            Classification classification = Classify(index);
            string delta = skBytes is int bytes ? (bytes - english[index].ByteLength).ToString() : "";
            writer.WriteLine(string.Join('\t', new[]
            {
                index.ToString(), english[index].ByteLength.ToString(), skBytes?.ToString() ?? "", delta,
                current.Kind.ToString(), classification.RuntimeClass, classification.Path, "", "", classification.RiskStatus,
                classification.Evidence, classification.Confidence, classification.Notes, Tsv(en), Tsv(sk ?? "")
            }));
        }
    }

    private static void WritePaths(string path)
    {
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("RuntimePath\tCodeAddress\tPhysicalOffset\tConsumerType\tBufferSize\tSafePayloadBytes\tCopySemantics\tRendererOrNextStage\tKnownIndices\tConfidence\tNotes");
        writer.WriteLine($"{ObservedTruncationPath}\tUNKNOWN\tUNKNOWN\tGAMEPC text consumer not statically isolated\tUNKNOWN\tUNKNOWN\tUNKNOWN\tUNKNOWN\t417,424\tPROVEN runtime test; UNKNOWN code flow\tObserved truncation establishes a constrained context, but not a 0x60 buffer or usable byte count.");
        writer.WriteLine($"{HistoricalSafePath}\tUNKNOWN\tUNKNOWN\tGAMEPC text consumer not statically isolated\tUNKNOWN\tUNKNOWN\tUNKNOWN\tUNKNOWN\t298,425,627\tSTRONG EVIDENCE\tHistorical safe overrides disprove a universal 96-byte GAMEPC limit; their distinct code paths are not statically proven in O1.");
    }

    private static void WriteReport(string path, IReadOnlyList<GamePcStringEntry> english, IReadOnlyList<GamePcStringEntry>? slovak, Encoding encoding, IReadOnlyDictionary<string, string> hashes)
    {
        var anchors = new[] { 417, 424, 298, 425, 627 }.Select(i => Anchor(i, english, slovak, encoding)).ToArray();
        using var writer = new StreamWriter(path, false, new UTF8Encoding(false));
        writer.WriteLine("ELVIRA I RUNTIME TEXT TRUNCATION AUDIT — O1"); writer.WriteLine();
        writer.WriteLine("Scope: read-only analysis of English GAMEPC, optional GAMEPCSK, the existing production diagnostic, and pre-existing runtime evidence. CZ was not read.");
        writer.WriteLine("English GAMEPC authority: 689 entries; strict counted-NUL parser. GAMEPC storage/repacking was not re-investigated."); writer.WriteLine();
        writer.WriteLine("CURRENT PRODUCTION DIAGNOSTIC (SOURCE INSPECTION)");
        writer.WriteLine("TextDiagnostics.cs: Elvira1TextMetadata.DialogueLimit = 96; ConfirmedAffected = {417,424}; ConfirmedSafe = {298,425,627}.");
        writer.WriteLine("Evaluate() marks affected entries as ConfirmedRisk only when their current bytes exceed 96; marks safe overrides independently of length; every other >96 entry is PossibleRisk. It therefore mixes a byte-length heuristic with five historical context overrides."); writer.WriteLine();
        writer.WriteLine("WHAT 96 MEANS");
        writer.WriteLine("PROVEN: a 96-byte CP852 threshold was observed in prior runtime testing for the contexts represented by indices 417 and 424.");
        writer.WriteLine("UNKNOWN: no pre-existing static artifact or isolated RUNVGA consumer in this O1 scope proves a 0x60-byte destination, a copy count, terminator handling, a physical code address, or whether 96 is usable payload. Accordingly safe payload is UNKNOWN, not 95 or 96.");
        writer.WriteLine("PROVEN: it is not a universal GAMEPC storage or rendering limit, because historical safe entries 298, 425 and 627 cross the old threshold through different contexts."); writer.WriteLine();
        writer.WriteLine("STATIC CONSUMER RESULT");
        writer.WriteLine("No GAMEPC index-to-consumer routine was statically proven without broad executable reverse engineering. Two neutral evidence classes are recorded in E1_RUNTIME_TEXT_PATHS.tsv; they are not asserted to be binary routines.");
        writer.WriteLine("Additional fixed limits: UNKNOWN. No additional byte limit is claimed."); writer.WriteLine();
        writer.WriteLine("ANCHORS");
        foreach (string anchor in anchors) writer.WriteLine(anchor);
        writer.WriteLine();
        writer.WriteLine("CONSERVATIVE 689-INDEX CLASSIFICATION");
        writer.WriteLine("HARD_LIMIT: 0. SAFE_CONTEXT: 3 (298,425,627). MULTI_CONTEXT: 0. RISK_UNKNOWN_LIMIT: 2 (417,424). UNCLASSIFIED: 684.");
        writer.WriteLine("No class was assigned from byte length alone. For current GAMEPCSK: PROVEN over a real hard limit = 0; at a proven boundary = 0; entries requiring runtime evidence = 686."); writer.WriteLine();
        writer.WriteLine("DYNAMIC TRACE DECISION: TARGETED_RUNTIME_TRACE_REQUIRED");
        writer.WriteLine("Minimal O2 objective: run a controlled invocation for index 417 (then 424 and one safe control), capture the GAMEPC string pointer/index at the first consumer, every destination write and byte/count argument through the next renderer call, and identify the destination buffer base plus terminating-NUL write. Break/capture addresses are UNKNOWN until that first consumer is isolated; inventing offsets would be unsound.");
        writer.WriteLine("Production recommendation for a later phase: replace the broad >96 PossibleRisk heuristic only after that trace maps specific script/reference contexts to a proven consumer and safe payload. Keep the five historical anchors as evidence annotations, not a complete static classifier."); writer.WriteLine();
        writer.WriteLine("READ-ONLY HASHES (before and after audit matched):"); foreach ((string file, string hash) in hashes) writer.WriteLine($"{Path.GetFileName(file)} {hash}");
    }

    private static string Anchor(int index, IReadOnlyList<GamePcStringEntry> english, IReadOnlyList<GamePcStringEntry>? slovak, Encoding encoding)
    {
        Classification c = Classify(index); string sk = slovak is null ? "absent" : slovak[index].ByteLength.ToString();
        return $"{index}: EN={english[index].ByteLength} bytes; SK={sk}; class={c.RuntimeClass}; path={c.Path}; limit=UNKNOWN; evidence={c.Confidence}. {c.Notes}";
    }

    private static Classification Classify(int index) => index switch
    {
        417 or 424 => new("RISK_UNKNOWN_LIMIT", ObservedTruncationPath, "NEEDS_RUNTIME_EVIDENCE", "Historical runtime case", "PROVEN runtime test / UNKNOWN code flow", "Observed truncation; exact mechanism and limit remain unproven."),
        298 or 425 or 627 => new("SAFE_CONTEXT", HistoricalSafePath, "N/A", "Historical safe override", "STRONG EVIDENCE", "Safe evidence disproves the universal 96-byte model; exact consumer is not statically isolated."),
        _ => new("UNCLASSIFIED", "UNKNOWN", "NEEDS_RUNTIME_EVIDENCE", "No static path association", "UNKNOWN", "Length alone does not determine runtime class.")
    };

    private static string Hash(string path) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));
    private static string Tsv(string value) => value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ').Replace('"', '\'');
    private sealed record Classification(string RuntimeClass, string Path, string RiskStatus, string Evidence, string Confidence, string Notes);
}
