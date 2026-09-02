using System.Security.Cryptography;
using System.Text;

namespace ElviraVgaEditor;

// Phase 6C9O2B4 static-only audit: BB7B and its seven validated direct callers.
internal static class Elvira1Bb7bProducerAudit
{
    private const string CanonicalHash = "C6CEC09B41D42D92DDD0588B54E5729720CDC91ABEF8083F9205F237A9686756";

    internal static void Run(string canonicalExePath, string gamePcPath, string gamePcSkPath, string outputDirectory)
    {
        var hashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [canonicalExePath] = Hash(canonicalExePath), [gamePcPath] = Hash(gamePcPath), [gamePcSkPath] = Hash(gamePcSkPath)
        };
        byte[] b = File.ReadAllBytes(canonicalExePath);
        if (b.Length != 0x2CE30 || hashes[canonicalExePath] != CanonicalHash) throw new InvalidDataException("Incorrect canonical RUNVGA baseline.");
        Validate(b);
        Directory.CreateDirectory(outputDirectory);
        WriteStaticAudit(Path.Combine(outputDirectory, "BB7B_STATIC_AUDIT.txt"));
        WriteCallers(Path.Combine(outputDirectory, "BB7B_CALLERS.tsv"));
        WriteArguments(Path.Combine(outputDirectory, "BB7B_ARGUMENT_FLOW.tsv"));
        WriteCount(Path.Combine(outputDirectory, "COUNT_PRODUCER_AUDIT.txt"));
        WriteGamePc(Path.Combine(outputDirectory, "GAMEPC_CONTEXT_AUDIT.txt"));
        WriteConclusion(Path.Combine(outputDirectory, "O2B4_CONCLUSION.txt"));
        foreach ((string path, string hash) in hashes) if (Hash(path) != hash) throw new InvalidDataException($"Read-only B4 audit changed {Path.GetFileName(path)}.");
    }

    private static void Validate(byte[] b)
    {
        Expect(b, 0xBB7B, 0x55, 0x8B, 0xEC); Expect(b, 0xBC3A, 0xCB);
        Expect(b, 0xBC1F, 0x8B, 0x5E, 0x06, 0x83, 0x46, 0x06, 0x01, 0x8A, 0x07, 0x98, 0x50, 0x9A, 0x62, 0x00, 0xBD, 0x0C);
        Expect(b, 0x55DE, 0x55, 0x8B, 0xEC); Expect(b, 0x5754, 0xCB);
        foreach (int call in new[] { 0x560B, 0x566B, 0x569D, 0x56E4, 0x56F4, 0x5710, 0x5728 }) Expect(b, call, 0x9A, 0xEB, 0x04, 0x29, 0x09);
        Expect(b, 0x5598, 0x55, 0x8B, 0xEC); Expect(b, 0x55BE, 0xCB);
    }

    private static void WriteStaticAudit(string path)
    {
        using var w = Writer(path);
        w.WriteLine("ELVIRA I B4 — BB7B POINTER/COUNT PRODUCER AUDIT");
        w.WriteLine("BB7B exact boundary: BB7B..BC3A, FAR procedure (RETF at BC3A). It uses BP frame arguments: [BP+6]=source pointer; [BP+8]=byte count.");
        w.WriteLine("BC1F: mov bx,[bp+6]; BC22: add word [bp+6],1; BC26: mov al,[bx]; BC28: cbw; BC29: push ax; BC2A: call far relocated BB7B target F032; BC2F: add sp,2.");
        w.WriteLine("BBA1..BBA8 decrements [BP+8] and exits its byte loop when it reaches zero. This is a variable argument, not a literal capacity.");
        w.WriteLine("All seven instruction-boundary-validated direct FAR callers are in 55DE..5754. No other BB7B FAR-call encoding exists in the canonical image.");
        w.WriteLine("55DE..5754 is a byte/context producer. It invokes BB7B with count=1 for a stack byte argument, or with count=[0288] and static source DS:1FD4/DS:1848. It is not proven that either source is GAMEPC.");
        w.WriteLine("5598..55BE is a small setter: [BP+6] -> [1FD0], [BP+8] -> [1FD2], then [0288]=0. It initializes producer state but does not establish GAMEPC provenance.");
    }

    private static void WriteCallers(string path)
    {
        using var w = Writer(path); w.WriteLine("CallSite\tContainingRoutine\tNearFar\tPointerArgument\tCountArgument\tProducerEvidence\tConfidence\tNotes");
        w.WriteLine("560B\t55DE..5754\tFAR\tLEA AX,[BP+6]; PUSH AX\tMOV AX,1; PUSH AX\tOne caller byte\tPROVEN\tValid target encoding 9A EB 04 29 09.");
        w.WriteLine("566B\t55DE..5754\tFAR\tMOV AX,1FD4; PUSH AX\tPUSH word [0288]\tStatic producer buffer and mutable count\tPROVEN\tNo fixed clamp at call site.");
        w.WriteLine("569D\t55DE..5754\tFAR\tLEA AX,[BP+6]; PUSH AX\tMOV AX,1; PUSH AX\tOne caller byte\tPROVEN\tControl/byte path.");
        w.WriteLine("56E4\t55DE..5754\tFAR\tMOV AX,1848; PUSH AX\tMOV AX,1; PUSH AX\tStatic one-byte source\tPROVEN\tNo GAMEPC relation proven.");
        w.WriteLine("56F4\t55DE..5754\tFAR\tMOV AX,1FD4; PUSH AX\tPUSH word [0288]\tStatic producer buffer and mutable count\tPROVEN\tNo fixed clamp at call site.");
        w.WriteLine("5710\t55DE..5754\tFAR\tLEA AX,[BP+6]; PUSH AX\tMOV AX,1; PUSH AX\tOne caller byte\tPROVEN\tNo full-string traversal.");
        w.WriteLine("5728\t55DE..5754\tFAR\tLEA AX,[BP+6]; PUSH AX\tMOV AX,1; PUSH AX\tOne caller byte\tPROVEN\tNo full-string traversal.");
    }

    private static void WriteArguments(string path)
    {
        using var w = Writer(path); w.WriteLine("Stage\tInstruction\tPointerForm\tCountForm\tClassification\tConfidence\tNotes");
        w.WriteLine("BB7B input\tBC1F..BC2A\t[BP+6] read then incremented\t[BP+8] decremented at BBA1\tVARIABLE_CONTEXT_LENGTH\tPROVEN\tPer-byte stream; count is not fixed.");
        w.WriteLine("55DE direct-byte calls\t5603/5695/5708/5720 setup\tAddress of caller argument [BP+6]\t1\tSCRIPT_DATA_OR_CONTEXT_BYTE\tPROVEN\tNot a full string pointer.");
        w.WriteLine("55DE buffered calls\t5661/56EA setup\tDS:1FD4\tword [0288]\tTEMP_BUFFER\tPROVEN\t0288 is mutable producer state.");
        w.WriteLine("55DE static call\t56DE setup\tDS:1848\t1\tSTATIC_TEXT_OR_CONTROL\tPROVEN\tNo GAMEPC table linkage.");
        w.WriteLine("5598 setter\t55A5..55B5\t[BP+6] -> [1FD0]\t[BP+8] -> [1FD2]; [0288]=0\tUNKNOWN_UPSTREAM\tPROVEN\tPointer/count source above setter unresolved.");
    }

    private static void WriteCount(string path)
    {
        using var w = Writer(path);
        w.WriteLine("COUNT PRODUCER AUDIT");
        w.WriteLine("PROVEN: BB7B count is [BP+8]. For direct calls from 55DE it is either literal 1 or word [0288].");
        w.WriteLine("PROVEN: 55DE initializes/resets [0288] at 55F5, 56C9, 5734; increments it at 573F; and passes it at 5665/56EE. It is mutable context/buffer state, not a literal limit.");
        w.WriteLine("PROVEN: 55DE has no LODSB/SCASB/manual NUL traversal computing a full length for these BB7B calls. It compares its single [BP+6] byte against control values 0Ch,00h,20h,0Ah.");
        w.WriteLine("PROVEN: 0x60/0x5F/0x61 are not count limits in 55DE..5754 or BB7B. No comparison/clamp to 96,95,97 occurs in the validated producer path.");
        w.WriteLine("COUNT SEMANTICS: VARIABLE_CONTEXT_LENGTH. Full GAMEPC length, substring length, and remainder behavior are UNKNOWN until the caller of 55DE/producer state is connected to target text.");
    }

    private static void WriteGamePc(string path)
    {
        using var w = Writer(path);
        w.WriteLine("GAMEPC CONTEXT AUDIT");
        w.WriteLine("GAMEPC connection: UNKNOWN. No validated instruction in BB7B, 55DE..5754, or 5598..55BE indexes GAMEPC's counted-NUL table or derives BB7B count from a GAMEPC entry length.");
        w.WriteLine("Indices 417, 424, 425, 298: their B2 dynamic frames prove BB7B/F032 usage, but do not identify which of the seven 55DE call sites or which higher producer supplied the observed text bytes. Higher context: UNKNOWN for all four.");
        w.WriteLine("Affected/safe divergence above BB7B: UNKNOWN, not NONE FOUND. The direct producer has multiple context branches, but current trace has no BB7B-entry caller discriminator.");
        w.WriteLine("Remainder behavior: UNKNOWN. The variable count proves BB7B renders exactly the supplied segment; it does not prove whether an upstream remainder is rendered, saved, or discarded.");
    }

    private static void WriteConclusion(string path)
    {
        using var w = Writer(path);
        w.WriteLine("O2B4 CONCLUSION");
        w.WriteLine("BB7B receives variable context segments, not a proven full GAMEPC string. Its immediate producer passes either one-byte arguments or DS:1FD4 with mutable [0288].");
        w.WriteLine("There is no fixed 96/95/97 clamp and no strlen/NUL scan in the validated BB7B producer path: 96_NOT_PRESENT_IN_BB7B_PRODUCER_PATH.");
        w.WriteLine("The exact target-specific producer remains unresolved because B2 captured the F033->BB7B return but not a usable BB7B-entry caller/arguments record. A minimal host trace at BB7B after MOV BP,SP should record caller CS:IP plus SS:[BP+6] pointer and SS:[BP+8] count for target glyph events.");
        w.WriteLine("Production consequence: remove the automatic >96 risk classification; keep runtime diagnosis unavailable/unknown until target pointer/count context is proven.");
    }

    private static void Expect(byte[] b, int offset, params byte[] expected) { for (int i = 0; i < expected.Length; i++) if (b[offset + i] != expected[i]) throw new InvalidDataException($"Signature missing at {offset:X}."); }
    private static StreamWriter Writer(string path) => new(path, false, new UTF8Encoding(false));
    private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
}
