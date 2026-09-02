using System.Security.Cryptography;
using System.Text;

namespace ElviraVgaEditor;

// Phase 6C9O2B3 static-only audit. The byte checks deliberately cover only the
// F032 adapter, its two direct far callers, and the proven renderer dispatch.
internal static class Elvira1TextConsumerF033Audit
{
    private const string CanonicalHash = "C6CEC09B41D42D92DDD0588B54E5729720CDC91ABEF8083F9205F237A9686756";

    internal static void Run(string canonicalExePath, string gamePcPath, string gamePcSkPath, string outputDirectory)
    {
        var protectedHashes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [canonicalExePath] = Hash(canonicalExePath), [gamePcPath] = Hash(gamePcPath), [gamePcSkPath] = Hash(gamePcSkPath)
        };
        byte[] image = File.ReadAllBytes(canonicalExePath);
        if (image.Length != 0x2CE30 || protectedHashes[canonicalExePath] != CanonicalHash)
            throw new InvalidDataException("Canonical RUNVGA baseline is not the required 0x2CE30-byte image.");
        Validate(image);

        Directory.CreateDirectory(outputDirectory);
        WriteStaticAudit(Path.Combine(outputDirectory, "F033_STATIC_AUDIT.txt"));
        WriteCallers(Path.Combine(outputDirectory, "F033_CALLERS.tsv"));
        WriteTextFlow(Path.Combine(outputDirectory, "F033_TEXT_FLOW.tsv"));
        WriteTermination(Path.Combine(outputDirectory, "TEXT_TERMINATION_AND_LAYOUT.tsv"));
        WriteLimitConclusion(Path.Combine(outputDirectory, "RUNTIME_LIMIT_CONCLUSION.txt"));
        WriteConclusion(Path.Combine(outputDirectory, "O2B3_CONCLUSION.txt"));

        foreach ((string path, string before) in protectedHashes)
            if (Hash(path) != before) throw new InvalidDataException($"Read-only B3 audit changed {Path.GetFileName(path)}.");
    }

    private static void Validate(byte[] b)
    {
        Expect(b, 0xF032, 0x55, 0x8B, 0xEC, 0x57, 0x56, 0x1E, 0x06); // prologue
        Expect(b, 0xF059, 0xE8, 0xEA, 0x01);                         // -> F246
        Expect(b, 0xF061, 0xCB);                                     // RETF
        Expect(b, 0xF062, 0x52, 0xB6, 0x00);                         // separate F062 entry
        Expect(b, 0xBC2A, 0x9A, 0x62, 0x00, 0xBD, 0x0C);             // -> relocated F032
        Expect(b, 0xBF9C, 0x9A, 0x62, 0x00, 0xBD, 0x0C);             // -> relocated F032
        Expect(b, 0xBC1F, 0x8B, 0x5E, 0x06, 0x83, 0x46, 0x06, 0x01, 0x8A, 0x07, 0x98, 0x50);
        Expect(b, 0xBB7B, 0x55, 0x8B, 0xEC);                         // byte-loop routine start
        Expect(b, 0xBC3A, 0xCB);                                     // byte-loop routine end
        Expect(b, 0xF2A6, 0xE8, 0x86, 0x00);                         // F2A6 -> F32F
        if (0xF2A9 + BitConverter.ToInt16(b, 0xF2A7) != 0xF32F || 0xF05C + BitConverter.ToInt16(b, 0xF05A) != 0xF246)
            throw new InvalidDataException("Validated lower renderer calls do not resolve to their expected targets.");
    }

    private static void WriteStaticAudit(string path)
    {
        using var w = Writer(path);
        w.WriteLine("ELVIRA I B3 — TARGETED STATIC AUDIT OF F033");
        w.WriteLine("Canonical RUNVGA: physical 0x00000..0x2CE2F, SHA-256 C6CEC09B41D42D92DDD0588B54E5729720CDC91ABEF8083F9205F237A9686756.");
        w.WriteLine();
        w.WriteLine("ROUTINE BOUNDARY — PROVEN BY BINARY");
        w.WriteLine("Observed runtime entry: F033. Actual routine: F032..F061. Prologue F032: push bp; mov bp,sp; push di; push si; push ds; push es. Epilogue F05C..F061: pop es; pop ds; pop si; pop di; pop bp; retf.");
        w.WriteLine("F062 begins a separate routine (push dx; mov dh,0; mov dl,[di]; ... call F246), so F032/F033 and F062 are adjacent separate entry paths sharing F246.");
        w.WriteLine();
        w.WriteLine("COMPACT ANNOTATED FLOW — PROVEN BY BINARY");
        w.WriteLine("F039 mov si,[28F2]; F03D mov di,[2772]; F041 cmp si,di; F043 je F05C. This is a state/identity gate, not source-text traversal.");
        w.WriteLine("F045 mov ax,1557; F048 mov ds,ax; F04A test byte [si+1],1; F04F jne F056; F051 call far 0A3A:029A. This is a state-flag gate/helper.");
        w.WriteLine("F056 mov dx,[bp+6]; F059 call F246. Therefore F032 receives one 16-bit argument and supplies its low byte (DL) to the character dispatcher; it does not receive a text pointer.");
        w.WriteLine("F05C..F061 restore registers and RETF. There is no loop, byte counter, copy, NUL test, width test, row test, page test, or 0x60/0x5F/0x61 immediate-limit instruction in F032.");
        w.WriteLine();
        w.WriteLine("DIRECT TEXT-LOOP CALLER — PROVEN BY BINARY");
        w.WriteLine("BB7B..BC3A is the relevant direct caller routine. At BC1F it loads BX=[BP+6], increments [BP+6], loads AL=[BX], CBW, pushes AX, then BC2A CALL FAR relocated F032 and cleans two bytes at BC2F.");
        w.WriteLine("At BBA1 it decrements [BP+8] and branches out when zero. Thus the immediate caller streams bytes from its caller-supplied pointer under a caller-supplied count. It does not prove that pointer is GAMEPC, but it proves no staging copy occurs between that pointer read and F032.");
        w.WriteLine("The routine tests its first source byte against 0Ch at BBB3..BBB8 and has renderer-state gates; this is control/state handling, not a 96-byte capacity.");
        w.WriteLine();
        w.WriteLine("LOWER DISPATCH — PROVEN BY BINARY");
        w.WriteLine("F246 pushes DS/DI/CX/AX and dispatches controls 0Ch,0Dh,0Ah,08h; printable bytes reach the F2A6 path. F2A6 calls F32F at F2A6:E8 86 00; F34C is inside F32F.");
        w.WriteLine("F2A6/F246 contain glyph-layout state updates, but this audit finds no source-byte count, no text staging, and no page continuation/discard rule on the proven F032/BB7B path.");
    }

    private static void WriteCallers(string path)
    {
        using var w = Writer(path); w.WriteLine("CallSite\tTarget\tNearFar\tContainingRoutine\tArgumentSetup\tTextPointerSource\tConfidence\tNotes");
        w.WriteLine("BC2A\tF032 (relocated 0CBD:0062)\tFAR\tBB7B..BC3A\tBC1F loads BX=[BP+6], increments it, AL=[BX], CBW, push AX\t[BP+6] byte-stream pointer; origin above BB7B UNKNOWN\tPROVEN\tValidated raw far-call encoding; runtime Stack22/24 returned to BC2F.");
        w.WriteLine("BF9C\tF032 (relocated 0CBD:0062)\tFAR\tBF2C..UNKNOWN\tB898 push 000Ch immediately before call\tNo text pointer; control-character invocation\tPROVEN\tNot the correlated printable text loop.");
    }

    private static void WriteTextFlow(string path)
    {
        using var w = Writer(path); w.WriteLine("Stage\tPhysicalOffset\tRoutine\tInputPointer\tOutputPointer\tCounter\tLimit\tTermination\tContinuation\tConfidence\tNotes");
        w.WriteLine("Immediate byte source\tBC1F\tBB7B..BC3A\tBX=[BP+6], then [BP+6]++\tAL then AX argument\t[BP+8] decremented at BBA1\tCaller-supplied count; no literal 96\tCount reaches zero; first-byte 0Ch control gate\tReturns to its caller; no page continuation proven\tPROVEN\tDirect source byte is passed without staging.");
        w.WriteLine("Character adapter\tF032/F056\tF032..F061\tNo pointer; DX=[BP+6] character word\tDL to F246\tNONE\tNONE\tState gate may bypass F246\tRETF\tPROVEN\tOne F246 call per F032 invocation.");
        w.WriteLine("Dispatcher/layout\tF246/F2A6\tF246..F32F\tDL character; SI renderer state\tF32F bitmap output\tRenderer state fields\tGlyph/line layout fields, not source count\tControls 0C/0D/0A/08 and printable dispatch\tLayout continuation internal; no source discard proven\tPROVEN\tDo not infer word wrapping/page semantics from this limited static path.");
    }

    private static void WriteTermination(string path)
    {
        using var w = Writer(path); w.WriteLine("Routine\tInstruction\tCondition\tType\tValue\tEffect\tConsumesSource\tPreservesRemainder\tConfidence\tNotes");
        w.WriteLine("BB7B..BC3A\tBBA1..BBA8\t[BP+8] decremented then compared with zero\tCALLER_COUNT_END\tCaller supplied\tLeaves byte loop\tYES\tUNKNOWN\tPROVEN\tNot a fixed engine byte capacity.");
        w.WriteLine("BB7B..BC3A\tBBB3..BBB8\tfirst source byte equals 0Ch\tCONTROL_CODE\t0Ch\tBranches out before stream setup\tReads first byte\tUNKNOWN\tPROVEN\tNo 96 relation.");
        w.WriteLine("F032\tF041..F043\tSI equals DI\tSTATE_GATE\tEquality\tReturns without F246\tNO\tYES\tPROVEN\tNot text length.");
        w.WriteLine("F246\tF24A..F26A\tDL equals 0Ch/0Dh/0Ah/08h or below 20h\tCONTROL_CODE\tControl bytes\tDispatches/ignores control character\tOne supplied character\tN/A\tPROVEN\tNo NUL/string loop in F032 path.");
        w.WriteLine("F062 (separate)\tF067..F071\tDL loaded from [DI] equals 00h\tEND_STRING\t00h\tEnds F062 NUL loop\tYES\tN/A\tPROVEN\tNot dynamically present in B2 target frames.");
        w.WriteLine("F032/BB7B relevant ranges\tBB7B..BC3A, F032..F061\tNo decoded literal 60h/5Fh/61h limit\tBYTE_LIMIT\tNONE\tNONE\tN/A\tN/A\tPROVEN\tBBBD=E9 displacement 60h; BC36/F05F=POP DI opcode 5Fh; F2C8=EB displacement 61h.");
    }

    private static void WriteLimitConclusion(string path)
    {
        using var w = Writer(path);
        w.WriteLine("RUNTIME LIMIT CONCLUSION — 96_NOT_PRESENT_ON_TARGET_PATH");
        w.WriteLine("PROVEN: no 96/95/97 immediate, source-byte capacity, staging buffer, or NUL-copy loop exists in F032 or its validated immediate printable caller BB7B..BC3A.");
        w.WriteLine("PROVEN: BB7B uses a variable caller-supplied count [BP+8], not 96. F032 is one-character dispatch only.");
        w.WriteLine("UNKNOWN: whether the pointer/count supplied to BB7B is direct GAMEPC data, a script-selected subrange, or a formatted buffer; identifying that needs an audit above BB7B, not a broad executable scan.");
        w.WriteLine("O2A4 partial prefixes are not PROVEN truncation: safe 425/298 and affected 417/424 share the same adapter/caller path. Classification: LIKELY_ANALYZER_ARTIFACT / UNKNOWN rendering-boundary evidence, not byte-limit proof.");
        w.WriteLine("Current GAMEPCSK: PROVEN_AT_RISK=0; AT_BOUNDARY=0, because no technical 96-byte rule is proven on this path.");
        w.WriteLine("Production >96 heuristic: REPLACE; it must not classify a text as broken from byte count alone. Evidence-based interim model: no automatic length-risk classification until an upstream BB7B pointer/count context proves a specific constraint.");
    }

    private static void WriteConclusion(string path)
    {
        using var w = Writer(path);
        w.WriteLine("O2B3 CONCLUSION");
        w.WriteLine("F033 is not a text consumer. It is the observed interior of far routine F032..F061, a one-character state-gated adapter into F246.");
        w.WriteLine("The true immediate text-loop consumer is BB7B..BC3A. It streams [BP+6] one byte at a time, increments that pointer, and uses variable [BP+8] as its count. The B2 raw far frame maps exactly to BC2F after this routine's F032 call.");
        w.WriteLine("No fixed 96-byte buffer/payload/layout rule is present in the audited target path. 96_NOT_PRESENT_ON_TARGET_PATH.");
        w.WriteLine("Affected and safe target fragments cannot be distinguished through BB7B/F032: all share the same recovered direct path. The next narrow static target is the routine(s) that supply BB7B's pointer and count, if a production classification model is still required.");
        w.WriteLine("Additional runtime trace: NO for this conclusion. Further dynamic evidence is needed only if static caller enumeration above BB7B cannot connect pointer/count context to GAMEPC/script selection.");
    }

    private static void Expect(byte[] bytes, int offset, params byte[] expected)
    {
        for (int i = 0; i < expected.Length; i++) if (bytes[offset + i] != expected[i]) throw new InvalidDataException($"Expected byte signature missing at {offset:X}.");
    }
    private static StreamWriter Writer(string path) => new(path, false, new UTF8Encoding(false));
    private static string Hash(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
}
