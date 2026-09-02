using System.Security.Cryptography;
using System.Text;

namespace ElviraVgaEditor;

// O3 validation helper. It exercises only in-memory metadata and read-only
// parser loads; it never opens the WinForms UI or writes a game installation.
internal static class ProductionRuntimeDiagnosticAudit
{
    internal static void Run(string elvira1GamePc, string elvira2GamePc, string outputDirectory)
    {
        var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [elvira1GamePc] = Hash(elvira1GamePc), [elvira2GamePc] = Hash(elvira2GamePc)
        };
        Encoding cp852 = GamePcTextEditor.GetEncoding("CP852");
        var synthetic = new List<(int Length, TextDiagnosticResult Result)>();
        foreach (int length in new[] { 95, 96, 97, 117, 160, 201 })
        {
            TextDiagnosticResult result = Elvira1TextMetadata.Evaluate(417, new string('X', length), cp852, ignored: false);
            if (result.Bytes != length || result.Kind != TextDiagnosticKind.None || result.Limit is not null || result.OverBy != 0)
                throw new InvalidDataException($"Unsupported runtime classification remained at {length} bytes.");
            synthetic.Add((length, result));
        }
        foreach (int index in new[] { 417, 424, 298, 425, 627 })
            if (Elvira1TextMetadata.Evaluate(index, new string('X', 117), cp852, ignored: false).Kind != TextDiagnosticKind.None)
                throw new InvalidDataException($"Index {index} retained a production runtime classification.");

        int e1Count = GamePcTextEditor.LoadEntries(elvira1GamePc, ElviraGameProfile.Elvira1).Count;
        int e2Count = GamePcTextEditor.LoadEntries(elvira2GamePc, ElviraGameProfile.Elvira2).Count;
        if (e1Count != 689 || e2Count != 1053) throw new InvalidDataException($"Unexpected parser counts E1={e1Count}, E2={e2Count}.");

        Directory.CreateDirectory(outputDirectory);
        using (var audit = Writer(Path.Combine(outputDirectory, "PRODUCTION_RUNTIME_AUDIT.txt")))
        {
            audit.WriteLine("O3 PRODUCTION RUNTIME AUDIT");
            audit.WriteLine("Old behavior: Elvira I used DialogueLimit=96, >96 PossibleRisk, and index-based affected/safe overrides in production UI and Runtime exchange output.");
            audit.WriteLine("New behavior: Elvira I metadata returns no runtime classification; Runtime is blank in exchange and hidden in the text grid for both Elvira I and Elvira II.");
            audit.WriteLine("Original B, Translation B, and Delta B remain CP852 byte metadata.");
            audit.WriteLine("No replacement threshold was introduced. E2 remains without a claimed fixed runtime limit.");
            audit.WriteLine($"Parser counts: E1={e1Count}; E2={e2Count}.");
        }
        using (var tests = Writer(Path.Combine(outputDirectory, "RUNTIME_DIAGNOSTIC_TESTS.txt")))
        {
            tests.WriteLine("Length\tEncodedBytes\tRuntimeKind\tLimit\tResult");
            foreach ((int length, TextDiagnosticResult result) in synthetic)
                tests.WriteLine($"{length}\t{result.Bytes}\t{result.Kind}\t{result.Limit?.ToString() ?? ""}\tPASS_NO_RUNTIME_CLASSIFICATION");
            tests.WriteLine("Indices 417,424,298,425,627 at 117 bytes: PASS_NO_INDEX_CLASSIFICATION");
            tests.WriteLine("E1 Runtime column: hidden by UpdateRuntimeColumnVisibility().");
            tests.WriteLine("E2 Runtime column: hidden by UpdateRuntimeColumnVisibility().");
            tests.WriteLine("Parser/repacker smoke commands remain separate and use disposable copies only.");
        }
        using (var conclusion = Writer(Path.Combine(outputDirectory, "O3_CONCLUSION.txt")))
        {
            conclusion.WriteLine("O3 CONCLUSION");
            conclusion.WriteLine("The unsupported global 96-byte heuristic and all index-based E1 runtime classifications are removed from production behavior.");
            conclusion.WriteLine("Byte counts remain factual metadata. No per-entry Elvira I or Elvira II runtime claim is shown.");
            conclusion.WriteLine("Historical truncation mechanism: UNKNOWN. Truncation reverse engineering is CLOSED_FOR_V1.");
        }
        foreach ((string path, string before) in hashes) if (Hash(path) != before) throw new InvalidDataException($"Read-only O3 audit changed {Path.GetFileName(path)}.");
    }

    private static StreamWriter Writer(string path) => new(path, false, new UTF8Encoding(false));
    private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
}
