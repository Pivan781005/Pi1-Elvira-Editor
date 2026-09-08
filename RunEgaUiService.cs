using System.Text;

namespace Pi1ElviraEditor;

/// <summary>EGA Runtime UI contract failure kind. Mirrors the VGA Pause
/// distinction (fixed-column shift vs hotspot damage) so tests stay
/// deterministic across both runtimes.</summary>
internal enum RunEgaUiFailureKind
{
    None,
    Empty,
    Encoding,
    TooLong,
    Newline,
    FixedColumn,
    Hotspot
}

/// <summary>Support classification from the R9F record audit (§10).</summary>
internal enum RunEgaUiContractKind
{
    /// <summary>Single message after a frozen prefix; NUL-terminated.</summary>
    SafeSimpleText,
    /// <summary>Editable field plus frozen separators/tails (buttons, input
    /// geometry); NUL only at the very end of the record.</summary>
    SafeStructuredText
}

/// <summary>Per-subfield editability. Message fields are freely editable
/// within their byte limits. Hotspot button labels are editable ONLY as
/// fixed-width slots: TEXT MAY CHANGE, HOTSPOT GEOMETRY MUST NOT CHANGE
/// (shorter labels are space-padded, overlong labels are rejected, columns
/// never move). Mid-record questions and input geometry stay frozen until
/// layout proof exists.</summary>
internal enum RunEgaUiSemanticRole
{
    EditableSafe,
    /// <summary>Editable label with a frozen slot width (pad shorter,
    /// reject longer). Neighboring columns never move.</summary>
    EditableFixedWidth,
    ReadOnlyGeometryUnproven,
    ReadOnlyHotspotProtected,
    ReadOnlyOther
}

internal sealed record RunEgaUiSemanticField(
    string Key,
    string Label,
    string Value,
    RunEgaUiSemanticRole Role,
    string ReadOnlyReason);

/// <summary>
/// Frozen per-record edit contract for Elvira I RUNEGA Runtime UI.
/// Proven by binary against the canonical image (SHA-256
/// A1524358...B3BAB56): English sources at the frozen offsets below,
/// materialized into the output bank at 0x33400+BankOffset with full Length
/// (payload + NUL + zero padding). Variable-length fields (1..max, no
/// padding): shorter values shift later segments left IN THE SPAN but keep
/// identical line/column screen geometry because the DOS renderer parses CR
/// separators and all glyphs are fixed-width. NUL occurs exactly once, at the
/// end. Hotspot button labels live in frozen fixed-width slots (see
/// ButtonSlots): their glyphs may change but their line, starting column,
/// width and neighbors never do. Mid-record questions and input-field
/// geometry stay frozen. Nothing here is borrowed from RUNVGA: separate
/// spans, separate offsets, separate table.
/// </summary>
internal sealed record RunEgaUiFieldContract(
    RuntimeUiLogicalRecordId Id,
    string DisplayName,
    int SourceOffset,
    int BankOffset,
    int Length,
    string Prefix,
    string Tail,
    int MaxFieldBytes,
    string[]? ButtonKeywords,
    string? ButtonLine,
    RunEgaUiContractKind Kind);

internal static class RunEgaUiService
{
    internal const int UiBankPhysical = 0x33400;

    internal static readonly IReadOnlyList<RunEgaUiFieldContract> Contracts =
    [
        new(RuntimeUiLogicalRecordId.PauseMenu, "Pause.menu", 0x1B261, 0x00DD, 42,
            "     ", "\r\r\r Continue      Quit", 14,
            ["Continue", "Quit"], " Continue      Quit", RunEgaUiContractKind.SafeStructuredText),
        new(RuntimeUiLogicalRecordId.ConfirmGeneric, "Confirm.generic", 0x1B288, 0x0107, 39,
            "    ", "\r\r\r     Yes       No", 14,
            ["Yes", "No"], "     Yes       No", RunEgaUiContractKind.SafeStructuredText),
        new(RuntimeUiLogicalRecordId.SavePrompt, "Save.prompt", 0x1B2B0, 0x012E, 51,
            "\r ", "\r\r   ", 43,
            null, null, RunEgaUiContractKind.SafeStructuredText),
        new(RuntimeUiLogicalRecordId.SaveFailure, "Save.failed", 0x1B2E4, 0x0161, 18,
            "\r    ", "", 12,
            null, null, RunEgaUiContractKind.SafeSimpleText),
        new(RuntimeUiLogicalRecordId.LoadFailure, "Restore.loadFailed", 0x1B2F6, 0x0173, 18,
            "\r    ", "", 12,
            null, null, RunEgaUiContractKind.SafeSimpleText),
        new(RuntimeUiLogicalRecordId.FileNotFound, "Restore.fileNotFound", 0x1B308, 0x0185, 19,
            "\r  ", "", 15,
            null, null, RunEgaUiContractKind.SafeSimpleText),
        new(RuntimeUiLogicalRecordId.TryAnotherDisk, "Disk.retry", 0x1B31C, 0x0198, 21,
            "\r  ", "", 17,
            null, null, RunEgaUiContractKind.SafeSimpleText),
        new(RuntimeUiLogicalRecordId.SaveOverwrite, "Save.overwrite", 0x1B332, 0x01AD, 62,
            "\r ", "\r\r    Overwrite it ?\r\r     Yes       No", 21,
            ["Yes", "No"], "     Yes       No", RunEgaUiContractKind.SafeStructuredText),
    ];

    internal static RunEgaUiFieldContract Contract(RuntimeUiLogicalRecordId id) =>
        Contracts.SingleOrDefault(contract => contract.Id == id)
        ?? throw new ArgumentOutOfRangeException(nameof(id));

    internal static bool IsEditable(RuntimeUiLogicalRecordId id) =>
        Contracts.Any(contract => contract.Id == id);

    /// <summary>Frozen hotspot button slot: a translatable label confined to
    /// an exact button-line cell. TEXT MAY CHANGE; HOTSPOT GEOMETRY MUST NOT:
    /// the starting <see cref="Column"/> within the button line and the
    /// <see cref="Width"/> in CP852 bytes are frozen. Shorter labels are
    /// space-padded to the width; overlong labels are rejected, never
    /// truncated; neighboring columns never move.</summary>
    internal sealed record RunEgaUiButtonSlot(string Key, string DefaultLabel, int Width, int Column);

    /// <summary>Frozen button-line geometry, measured from the authoritative
    /// English originals (fixed-width CP852, 0-based columns in the line):
    /// Pause line " Continue␣␣␣␣␣␣Quit" (19): Continue cols 1-8, Quit cols
    /// 15-18. Confirm/Overwrite line "␣␣␣␣␣Yes␣␣␣␣␣␣␣No" (17): Yes cols 5-7,
    /// No cols 15-16. Indents and gaps are frozen literals.</summary>
    internal static class RunEgaUiButtons
    {
        internal const string PauseIndent = " ";
        internal const string PauseGap = "      ";
        internal const string ConfirmIndent = "     ";
        internal const string ConfirmGap = "       ";
        internal const string OverwriteQuestionLine = "    Overwrite it ?";

        internal static IReadOnlyList<RunEgaUiButtonSlot> Slots(RuntimeUiLogicalRecordId id) => id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu =>
                [new("continue", "Continue", 8, 1), new("quit", "Quit", 4, 15)],
            RuntimeUiLogicalRecordId.ConfirmGeneric =>
                [new("yes", "Yes", 3, 5), new("no", "No", 2, 15)],
            RuntimeUiLogicalRecordId.SaveOverwrite =>
                [new("yes", "Yes", 3, 5), new("no", "No", 2, 15)],
            _ => []
        };

        internal static string DefaultLabel(RuntimeUiLogicalRecordId id, string key) =>
            Slots(id).Single(slot => slot.Key.Equals(key, StringComparison.Ordinal)).DefaultLabel;

        internal static string PauseButtonLine(string @continue, string quit) =>
            PauseIndent + @continue + PauseGap + quit;

        internal static string ConfirmButtonLine(string yes, string no) =>
            ConfirmIndent + yes + ConfirmGap + no;
    }

    internal static FrozenRuntimeEvidence Evidence(RuntimeUiLogicalRecordId id) => id switch
    {
        RuntimeUiLogicalRecordId.PauseMenu => FrozenRuntimeEvidence.ProvenLive,
        RuntimeUiLogicalRecordId.ConfirmGeneric => FrozenRuntimeEvidence.ProvenByBinary,
        RuntimeUiLogicalRecordId.SavePrompt => FrozenRuntimeEvidence.ProvenLive,
        RuntimeUiLogicalRecordId.SaveFailure => FrozenRuntimeEvidence.ProvenByBinary,
        RuntimeUiLogicalRecordId.LoadFailure => FrozenRuntimeEvidence.ProvenByBinary,
        RuntimeUiLogicalRecordId.FileNotFound => FrozenRuntimeEvidence.ProvenByBinary,
        RuntimeUiLogicalRecordId.TryAnotherDisk => FrozenRuntimeEvidence.ProvenByBinary,
        RuntimeUiLogicalRecordId.SaveOverwrite => FrozenRuntimeEvidence.ProvenByBinary,
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };

    /// <summary>Authoritative original decode from a supported RUNEGA form
    /// (packed canonicalizes with SHA validation; canonical direct). Reads
    /// only inside the frozen source span; NUL must occur within Length.</summary>
    internal static bool TryDecodeOriginal(string runegaPath, RuntimeUiLogicalRecordId id, out string? text, out string? error)
    {
        text = null; error = null;
        try
        {
            if (string.IsNullOrWhiteSpace(runegaPath) || !File.Exists(runegaPath))
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
            byte[] data = File.ReadAllBytes(runegaPath);
            byte[] canonical = data.Length == RunEgaBootstrapService.PackedSize
                ? RunEgaBootstrapService.CanonicalizePacked(data)
                : data;
            RunEgaBootstrapService.ValidateCanonical(canonical);
            RunEgaUiFieldContract contract = Contract(id);
            if (contract.SourceOffset + contract.Length > canonical.Length)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
            byte[] window = canonical.AsSpan(contract.SourceOffset, contract.Length).ToArray();
            int nul = Array.IndexOf(window, (byte)0x00);
            if (nul < 0)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
            return StrictDecode(window[..nul], out text, out error);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentOutOfRangeException)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
    }

    internal static bool TryDecodeOriginalFromGameRoot(string gameRoot, RuntimeUiLogicalRecordId id, out string? text, out string? error)
    {
        text = null; error = null;
        try
        {
            return TryDecodeOriginal(Path.Combine(gameRoot, Elvira1ProductionProfile.ActiveEgaExecutable), id, out text, out error);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
    }

    /// <summary>Batch decode for grid display: one canonicalization for all
    /// eight records. Missing entries map to null (unavailable), never throw.</summary>
    internal static IReadOnlyDictionary<RuntimeUiLogicalRecordId, string?> TryDecodeAllFromGameRoot(string gameRoot)
    {
        var result = new Dictionary<RuntimeUiLogicalRecordId, string?>();
        try
        {
            string path = Path.Combine(gameRoot, Elvira1ProductionProfile.ActiveEgaExecutable);
            if (!File.Exists(path)) return result;
            byte[] data = File.ReadAllBytes(path);
            byte[] canonical = data.Length == RunEgaBootstrapService.PackedSize
                ? RunEgaBootstrapService.CanonicalizePacked(data)
                : data;
            RunEgaBootstrapService.ValidateCanonical(canonical);
            foreach (RunEgaUiFieldContract contract in Contracts)
            {
                string? text = null;
                if (contract.SourceOffset + contract.Length <= canonical.Length)
                {
                    byte[] window = canonical.AsSpan(contract.SourceOffset, contract.Length).ToArray();
                    int nul = Array.IndexOf(window, (byte)0x00);
                    if (nul >= 0 && StrictDecode(window[..nul], out string? decoded, out _))
                        text = decoded;
                }
                result[contract.Id] = text;
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentOutOfRangeException)
        {
        }
        return result;
    }

    /// <summary>Decode of a materialized bank span (output images only).</summary>
    internal static bool TryDecodeBankSpan(byte[] image, RuntimeUiLogicalRecordId id, out string? text, out string? error)
    {
        text = null; error = null;
        RunEgaUiFieldContract contract = Contract(id);
        if (image is null || image.Length < UiBankPhysical + contract.BankOffset + contract.Length)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        byte[] span = image.AsSpan(UiBankPhysical + contract.BankOffset, contract.Length).ToArray();
        int nul = Array.IndexOf(span, (byte)0x00);
        if (nul < 0)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        for (int i = nul + 1; i < span.Length; i++)
            if (span[i] != 0x00)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        return StrictDecode(span[..nul], out text, out error);
    }

    /// <summary>Button-aware field extraction (parse-only): for button
    /// records the message field is parsed from frozen segments, so
    /// translated button labels no longer hide the field. Returns false for
    /// broken structure (the caller then shows the stored value raw).</summary>
    internal static bool TryExtractField(string? fullRecord, RuntimeUiLogicalRecordId id, out string? field)
    {
        field = null;
        if (string.IsNullOrEmpty(fullRecord)) return false;
        RunEgaUiFieldContract contract = Contract(id);
        if (RunEgaUiButtons.Slots(id).Count == 0)
        {
            if (!fullRecord.StartsWith(contract.Prefix, StringComparison.Ordinal)) return false;
            if (contract.Tail.Length > 0 && !fullRecord.EndsWith(contract.Tail, StringComparison.Ordinal)) return false;
            field = fullRecord.Substring(contract.Prefix.Length, fullRecord.Length - contract.Prefix.Length - contract.Tail.Length);
            return true;
        }
        return TrySplitStructured(fullRecord, id, out field, out _, out _, out _);
    }

    /// <summary>Button-label extraction (parse-only): trimmed labels keyed
    /// by slot ("continue"/"quit"/"yes"/"no"). Buttonless records yield an
    /// empty map. Returns false for broken structure.</summary>
    internal static bool TryExtractButtons(string? fullRecord, RuntimeUiLogicalRecordId id, out IReadOnlyDictionary<string, string>? buttons)
    {
        buttons = null;
        if (string.IsNullOrEmpty(fullRecord)) return false;
        if (RunEgaUiButtons.Slots(id).Count == 0)
        {
            // Buttonless records keep the legacy affix gate so unstructured
            // legacy values stay visible-raw instead of misparsed.
            RunEgaUiFieldContract contract = Contract(id);
            if (!fullRecord.StartsWith(contract.Prefix, StringComparison.Ordinal)) return false;
            if (contract.Tail.Length > 0 && !fullRecord.EndsWith(contract.Tail, StringComparison.Ordinal)) return false;
            buttons = new Dictionary<string, string>();
            return true;
        }
        if (!TrySplitStructured(fullRecord, id, out _, out IReadOnlyDictionary<string, string>? raw, out _, out _)) return false;
        buttons = raw!.ToDictionary(pair => pair.Key, pair => pair.Value.Trim(), StringComparer.Ordinal);
        return true;
    }

    /// <summary>Splits a button record into its message field and RAW button
    /// slot values (unpadded by the caller). Segment order, separators,
    /// indents and gaps are frozen; only field/button glyph bytes vary.
    /// Structural damage is reported through the frozen failure
    /// classification (fixed-column vs hotspot), never thrown.</summary>
    private static bool TrySplitStructured(string fullRecord, RuntimeUiLogicalRecordId id, out string? field, out IReadOnlyDictionary<string, string>? slots, out RunEgaUiFailureKind failure, out string detail)
    {
        field = null; slots = null; failure = RunEgaUiFailureKind.None; detail = string.Empty;
        RunEgaUiFieldContract contract = Contract(id);
        if (!fullRecord.StartsWith(contract.Prefix, StringComparison.Ordinal))
            return StructuralFailure(fullRecord, contract, out failure, out detail);
        string rest = fullRecord.Substring(contract.Prefix.Length);
        int cr = rest.IndexOf('\r');
        if (cr < 0)
            return StructuralFailure(fullRecord, contract, out failure, out detail);
        field = rest.Substring(0, cr);
        string tail = rest.Substring(cr);
        string line;
        if (id == RuntimeUiLogicalRecordId.PauseMenu)
        {
            if (!tail.StartsWith("\r\r\r", StringComparison.Ordinal))
                return StructuralFailure(fullRecord, contract, out failure, out detail);
            line = tail.Substring(3);
            if (line.Length != 1 + 8 + 6 + 4 || line[0] != ' ' || !line.Substring(9, 6).Equals("      ", StringComparison.Ordinal))
                return StructuralFailure(fullRecord, contract, out failure, out detail);
            slots = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["continue"] = line.Substring(1, 8),
                ["quit"] = line.Substring(15, 4)
            };
            return true;
        }
        if (id == RuntimeUiLogicalRecordId.ConfirmGeneric)
        {
            if (!tail.StartsWith("\r\r\r", StringComparison.Ordinal))
                return StructuralFailure(fullRecord, contract, out failure, out detail);
            return SplitConfirmButtonLine(fullRecord, contract, tail.Substring(3), out slots, out failure, out detail);
        }
        if (id == RuntimeUiLogicalRecordId.SaveOverwrite)
        {
            if (!tail.StartsWith("\r\r", StringComparison.Ordinal))
                return StructuralFailure(fullRecord, contract, out failure, out detail);
            string afterFirst = tail.Substring(2);
            if (!afterFirst.StartsWith(RunEgaUiButtons.OverwriteQuestionLine, StringComparison.Ordinal))
                return StructuralFailure(fullRecord, contract, out failure, out detail);
            string afterQuestion = afterFirst.Substring(RunEgaUiButtons.OverwriteQuestionLine.Length);
            if (!afterQuestion.StartsWith("\r\r", StringComparison.Ordinal))
                return StructuralFailure(fullRecord, contract, out failure, out detail);
            return SplitConfirmButtonLine(fullRecord, contract, afterQuestion.Substring(2), out slots, out failure, out detail);
        }
        return StructuralFailure(fullRecord, contract, out failure, out detail);
    }

    private static bool SplitConfirmButtonLine(string fullRecord, RunEgaUiFieldContract contract, string line, out IReadOnlyDictionary<string, string>? slots, out RunEgaUiFailureKind failure, out string detail)
    {
        slots = null; failure = RunEgaUiFailureKind.None; detail = string.Empty;
        if (line.Length != 5 + 3 + 7 + 2 || !line.StartsWith("     ", StringComparison.Ordinal) || !line.Substring(8, 7).Equals("       ", StringComparison.Ordinal))
            return StructuralFailure(fullRecord, contract, out failure, out detail);
        slots = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["yes"] = line.Substring(5, 3),
            ["no"] = line.Substring(15, 2)
        };
        return true;
    }

    /// <summary>Semantic decomposition of one full record into named
    /// subfields (parse-only, never re-encodes). The editable message field
    /// comes first; hotspot button labels follow as fixed-width editable
    /// rows (current values extracted, trimmed); frozen questions/geometry
    /// stay read-only with localized reasons. Unknown shapes return a single
    /// read-only row so nothing is ever hidden.</summary>
    internal static IReadOnlyList<RunEgaUiSemanticField> Decompose(string? fullRecord, RuntimeUiLogicalRecordId id)
    {
        if (!TryExtractField(fullRecord, id, out string? field) || field is null)
            return [new("raw", UiText.Get("RuntimeUi.Semantic.RawRecord"), fullRecord ?? string.Empty,
                RunEgaUiSemanticRole.ReadOnlyOther, UiText.Get("RuntimeUi.Detail.EgaStructure"))];
        if (!TryExtractButtons(fullRecord, id, out IReadOnlyDictionary<string, string>? buttons) || buttons is null)
            return [new("raw", UiText.Get("RuntimeUi.Semantic.RawRecord"), fullRecord ?? string.Empty,
                RunEgaUiSemanticRole.ReadOnlyOther, UiText.Get("RuntimeUi.Detail.EgaStructure"))];
        string ButtonValue(string key) => buttons.TryGetValue(key, out string? value) ? value : RunEgaUiButtons.DefaultLabel(id, key);
        string ButtonHint(string key)
        {
            RunEgaUiButtonSlot slot = RunEgaUiButtons.Slots(id).Single(item => item.Key.Equals(key, StringComparison.Ordinal));
            return string.Format(UiText.Get("RuntimeUi.Detail.EgaButtonTooWide"), slot.DefaultLabel, slot.Width);
        }
        var rows = new List<RunEgaUiSemanticField>();
        switch (id)
        {
            case RuntimeUiLogicalRecordId.PauseMenu:
                rows.Add(new("title", UiText.Get("RuntimeUi.Semantic.Title"), field, RunEgaUiSemanticRole.EditableSafe, string.Empty));
                rows.Add(new("continue", UiText.Get("RuntimeUi.Semantic.Continue"), ButtonValue("continue"), RunEgaUiSemanticRole.EditableFixedWidth, ButtonHint("continue")));
                rows.Add(new("quit", UiText.Get("RuntimeUi.Semantic.Quit"), ButtonValue("quit"), RunEgaUiSemanticRole.EditableFixedWidth, ButtonHint("quit")));
                break;
            case RuntimeUiLogicalRecordId.ConfirmGeneric:
                rows.Add(new("prompt", UiText.Get("RuntimeUi.Semantic.Prompt"), field, RunEgaUiSemanticRole.EditableSafe, string.Empty));
                rows.Add(new("yes", UiText.Get("RuntimeUi.Semantic.Yes"), ButtonValue("yes"), RunEgaUiSemanticRole.EditableFixedWidth, ButtonHint("yes")));
                rows.Add(new("no", UiText.Get("RuntimeUi.Semantic.No"), ButtonValue("no"), RunEgaUiSemanticRole.EditableFixedWidth, ButtonHint("no")));
                break;
            case RuntimeUiLogicalRecordId.SavePrompt:
                rows.Add(new("message", UiText.Get("RuntimeUi.Semantic.Message"), field, RunEgaUiSemanticRole.EditableSafe, string.Empty));
                rows.Add(new("input", UiText.Get("RuntimeUi.Semantic.InputField"), UiText.Get("RuntimeUi.Semantic.InputFieldFrozen"),
                    RunEgaUiSemanticRole.ReadOnlyGeometryUnproven, UiText.Get("RuntimeUi.Detail.SemanticGeometryUnproven")));
                break;
            case RuntimeUiLogicalRecordId.SaveOverwrite:
                rows.Add(new("message", UiText.Get("RuntimeUi.Semantic.Message"), field, RunEgaUiSemanticRole.EditableSafe, string.Empty));
                rows.Add(new("question", UiText.Get("RuntimeUi.Semantic.Question"), "Overwrite it ?",
                    RunEgaUiSemanticRole.ReadOnlyGeometryUnproven, UiText.Get("RuntimeUi.Detail.SemanticGeometryUnproven")));
                rows.Add(new("yes", UiText.Get("RuntimeUi.Semantic.Yes"), ButtonValue("yes"), RunEgaUiSemanticRole.EditableFixedWidth, ButtonHint("yes")));
                rows.Add(new("no", UiText.Get("RuntimeUi.Semantic.No"), ButtonValue("no"), RunEgaUiSemanticRole.EditableFixedWidth, ButtonHint("no")));
                break;
            default:
                rows.Add(new("message", UiText.Get("RuntimeUi.Semantic.Message"), field, RunEgaUiSemanticRole.EditableSafe, string.Empty));
                break;
        }
        return rows;
    }

    /// <summary>Full-record composer with translated hotspot button labels.
    /// The message field follows the same rules as <see
    /// cref="TryComposeField"/>; each supplied button label is trimmed,
    /// strictly CP852-encoded, and space-padded to its frozen slot width so
    /// every column stays byte-identical. Overlong labels are rejected, never
    /// truncated. Null/missing entries keep the English defaults. Unknown
    /// button keys throw; records without slots reject any supplied labels.</summary>
    internal static bool TryComposeRecord(RuntimeUiLogicalRecordId id, string? field, IReadOnlyDictionary<string, string>? buttons, out string fullRecord, out RunEgaUiFailureKind failure, out string detail)
    {
        fullRecord = string.Empty; failure = RunEgaUiFailureKind.None; detail = string.Empty;
        RunEgaUiFieldContract contract = Contract(id);
        IReadOnlyList<RunEgaUiButtonSlot> slots = RunEgaUiButtons.Slots(id);
        if (slots.Count == 0)
        {
            if (buttons is not null && buttons.Count > 0)
                throw new ArgumentException("Record '" + id + "' has no button slots.", nameof(buttons));
            return TryComposeField(id, field, out fullRecord, out failure, out detail);
        }
        if (buttons is not null)
        {
            foreach (string key in buttons.Keys)
                if (!slots.Any(slot => slot.Key.Equals(key, StringComparison.Ordinal)))
                    throw new ArgumentException("Unknown button slot '" + key + "' for record '" + id + "'.", nameof(buttons));
        }
        if (!ValidateComposeField(contract, field, out _, out failure, out detail)) return false;
        var padded = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (RunEgaUiButtonSlot slot in slots)
        {
            string label = buttons is not null && buttons.TryGetValue(slot.Key, out string? custom) && custom is not null
                ? custom.Trim()
                : slot.DefaultLabel;
            if (!ValidateButtonLabel(slot, label, out string cell, out failure, out detail)) return false;
            padded[slot.Key] = cell;
        }
        string composed = id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => contract.Prefix + field
                + "\r\r\r" + RunEgaUiButtons.PauseButtonLine(padded["continue"], padded["quit"]),
            RuntimeUiLogicalRecordId.ConfirmGeneric => contract.Prefix + field
                + "\r\r\r" + RunEgaUiButtons.ConfirmButtonLine(padded["yes"], padded["no"]),
            RuntimeUiLogicalRecordId.SaveOverwrite => contract.Prefix + field
                + "\r\r" + RunEgaUiButtons.OverwriteQuestionLine
                + "\r\r" + RunEgaUiButtons.ConfirmButtonLine(padded["yes"], padded["no"]),
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };
        if (!TryValidateStored(composed, id, out RunEgaUiFailureKind innerFailure, out string innerDetail))
        { failure = innerFailure; detail = innerDetail; return false; }
        fullRecord = composed;
        return true;
    }

    /// <summary>Message-field rules shared by both composers (empty, NUL,
    /// strict CP852, controls, HUD glyph, envelope field width).</summary>
    private static bool ValidateComposeField(RunEgaUiFieldContract contract, string? field, out byte[] encoded, out RunEgaUiFailureKind failure, out string detail)
    {
        encoded = []; failure = RunEgaUiFailureKind.None; detail = string.Empty;
        if (string.IsNullOrEmpty(field))
        { failure = RunEgaUiFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (field.IndexOf('\0') >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            encoded = strict.GetBytes(field);
            if (!string.Equals(field, strict.GetString(encoded), StringComparison.Ordinal))
            { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        int control = Array.FindIndex(encoded, value => value < 0x20 || value == 0x7F);
        if (control >= 0)
        { failure = RunEgaUiFailureKind.Newline; detail = string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), encoded[control]); return false; }
        if (Array.IndexOf(encoded, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        if (encoded.Length > contract.MaxFieldBytes)
        {
            string composedTooLong = contract.Prefix + field + contract.Tail;
            byte[] fullTooLong = GamePcTextEditor.GetEncoding("CP852").GetBytes(composedTooLong);
            failure = RunEgaUiFailureKind.TooLong;
            detail = string.Format(UiText.Get("RuntimeUi.Detail.RecordCapacityExceeded"), fullTooLong.Length + 1, contract.Length);
            return false;
        }
        return true;
    }

    /// <summary>Fixed-width button-label rules: trimmed, non-empty, strict
    /// CP852 without controls, at most the frozen slot width, padded with
    /// trailing spaces to the exact width. A label that would move any
    /// column fails as a fixed-column violation, never truncates.</summary>
    private static bool ValidateButtonLabel(RunEgaUiButtonSlot slot, string label, out string cell, out RunEgaUiFailureKind failure, out string detail)
    {
        cell = string.Empty; failure = RunEgaUiFailureKind.None; detail = string.Empty;
        if (string.IsNullOrWhiteSpace(label))
        { failure = RunEgaUiFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (label.IndexOf('\0') >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        byte[] encoded;
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            encoded = strict.GetBytes(label);
            if (!string.Equals(label, strict.GetString(encoded), StringComparison.Ordinal))
            { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        int control = Array.FindIndex(encoded, value => value < 0x20 || value == 0x7F);
        if (control >= 0)
        { failure = RunEgaUiFailureKind.Newline; detail = string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), encoded[control]); return false; }
        if (Array.IndexOf(encoded, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        if (encoded.Length > slot.Width)
        { failure = RunEgaUiFailureKind.FixedColumn; detail = string.Format(UiText.Get("RuntimeUi.Detail.EgaButtonTooWide"), label, slot.Width); return false; }
        cell = label + new string(' ', slot.Width - label.Length);
        if (GamePcTextEditor.GetEncoding("CP852").GetByteCount(cell) != slot.Width)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        return true;
    }

    /// <summary>Grid edit path: validates a user-typed field and composes the
    /// frozen full record for project persistence. No padding is invented:
    /// shorter fields shift later segments left in the span while keeping
    /// identical line/column geometry; NUL terminates once at the end.</summary>
    internal static bool TryComposeField(RuntimeUiLogicalRecordId id, string? field, out string fullRecord, out RunEgaUiFailureKind failure, out string detail)
    {
        fullRecord = string.Empty; failure = RunEgaUiFailureKind.None; detail = string.Empty;
        RunEgaUiFieldContract contract = Contract(id);
        if (string.IsNullOrEmpty(field))
        { failure = RunEgaUiFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (field.IndexOf('\0') >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        byte[] encoded;
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            encoded = strict.GetBytes(field);
            if (!string.Equals(field, strict.GetString(encoded), StringComparison.Ordinal))
            { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        int control = Array.FindIndex(encoded, value => value < 0x20 || value == 0x7F);
        if (control >= 0)
        { failure = RunEgaUiFailureKind.Newline; detail = string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), encoded[control]); return false; }
        if (Array.IndexOf(encoded, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        if (encoded.Length > contract.MaxFieldBytes)
        {
            string composedTooLong = contract.Prefix + field + contract.Tail;
            byte[] fullTooLong = GamePcTextEditor.GetEncoding("CP852").GetBytes(composedTooLong);
            failure = RunEgaUiFailureKind.TooLong;
            detail = string.Format(UiText.Get("RuntimeUi.Detail.RecordCapacityExceeded"), fullTooLong.Length + 1, contract.Length);
            return false;
        }
        string composed = contract.Prefix + field + contract.Tail;
        if (!TryValidateStored(composed, id, out RunEgaUiFailureKind innerFailure, out string innerDetail))
        { failure = innerFailure; detail = innerDetail; return false; }
        fullRecord = composed;
        return true;
    }

    /// <summary>Contract validation of a persisted full record (project state,
    /// preflight, materializer). Enforces frozen affixes, field rules and span
    /// capacity; legacy unstructured values fail with a clear hotspot error
    /// instead of a misleading capacity pass.</summary>
    internal static bool TryValidateStored(string? fullRecord, RuntimeUiLogicalRecordId id, out RunEgaUiFailureKind failure, out string detail)
    {
        failure = RunEgaUiFailureKind.None; detail = string.Empty;
        RunEgaUiFieldContract contract = Contract(id);
        if (string.IsNullOrEmpty(fullRecord))
        { failure = RunEgaUiFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (fullRecord.IndexOf('\0') >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        byte[] payload;
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            payload = strict.GetBytes(fullRecord);
            if (!string.Equals(fullRecord, strict.GetString(payload), StringComparison.Ordinal))
            { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        int control = Array.FindIndex(payload, value => value != 0x0D && (value < 0x20 || value == 0x7F));
        if (control >= 0)
        { failure = RunEgaUiFailureKind.Newline; detail = string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), payload[control]); return false; }
        if (Array.IndexOf(payload, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
        { failure = RunEgaUiFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        if (checked(payload.Length + 1) > contract.Length)
        { failure = RunEgaUiFailureKind.TooLong; detail = string.Format(UiText.Get("RuntimeUi.Detail.RecordCapacityExceeded"), payload.Length + 1, contract.Length); return false; }
        if (RunEgaUiButtons.Slots(id).Count > 0)
            return ValidateStructured(fullRecord, id, contract, out failure, out detail);
        if (!fullRecord.StartsWith(contract.Prefix, StringComparison.Ordinal) ||
            (contract.Tail.Length > 0 && !fullRecord.EndsWith(contract.Tail, StringComparison.Ordinal)))
            return StructuralFailure(fullRecord, contract, out failure, out detail);
        string field = fullRecord.Substring(contract.Prefix.Length, fullRecord.Length - contract.Prefix.Length - contract.Tail.Length);
        if (field.Length == 0)
        { failure = RunEgaUiFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (field.IndexOf('\r') >= 0 || field.IndexOf('\n') >= 0)
            return StructuralFailure(fullRecord, contract, out failure, out detail);
        if (GamePcTextEditor.GetEncoding("CP852").GetByteCount(field) > contract.MaxFieldBytes)
        { failure = RunEgaUiFailureKind.TooLong; detail = string.Format(UiText.Get("RuntimeUi.Detail.RecordCapacityExceeded"), payload.Length + 1, contract.Length); return false; }
        return true;
    }

    /// <summary>Structural validation for button records: frozen segments
    /// plus message-field rules plus fixed-width button-slot rules. Button
    /// glyphs may be translated; lines, columns, widths and neighbors never
    /// move. Slot violations fail as fixed-column (overlong) or empty, never
    /// truncate.</summary>
    private static bool ValidateStructured(string fullRecord, RuntimeUiLogicalRecordId id, RunEgaUiFieldContract contract, out RunEgaUiFailureKind failure, out string detail)
    {
        failure = RunEgaUiFailureKind.None; detail = string.Empty;
        if (!TrySplitStructured(fullRecord, id, out string? field, out IReadOnlyDictionary<string, string>? raw, out failure, out detail) || field is null || raw is null)
            return false;
        if (field.Length == 0)
        { failure = RunEgaUiFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (field.IndexOf('\r') >= 0 || field.IndexOf('\n') >= 0)
            return StructuralFailure(fullRecord, contract, out failure, out detail);
        if (GamePcTextEditor.GetEncoding("CP852").GetByteCount(field) > contract.MaxFieldBytes)
        {
            byte[] payload = GamePcTextEditor.GetEncoding("CP852").GetBytes(fullRecord);
            failure = RunEgaUiFailureKind.TooLong; detail = string.Format(UiText.Get("RuntimeUi.Detail.RecordCapacityExceeded"), payload.Length + 1, contract.Length); return false;
        }
        foreach (RunEgaUiButtonSlot slot in RunEgaUiButtons.Slots(id))
        {
            string cell = raw.TryGetValue(slot.Key, out string? value) ? value : string.Empty;
            string label = cell.Trim();
            if (label.Length == 0)
            { failure = RunEgaUiFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
            if (GamePcTextEditor.GetEncoding("CP852").GetByteCount(label) > slot.Width)
            { failure = RunEgaUiFailureKind.FixedColumn; detail = string.Format(UiText.Get("RuntimeUi.Detail.EgaButtonTooWide"), label, slot.Width); return false; }
            if (GamePcTextEditor.GetEncoding("CP852").GetByteCount(cell) != slot.Width)
                return StructuralFailure(fullRecord, contract, out failure, out detail);
        }
        return true;
    }

    private static bool StructuralFailure(string fullRecord, RunEgaUiFieldContract contract, out RunEgaUiFailureKind failure, out string detail)
    {
        failure = RunEgaUiFailureKind.Hotspot; detail = string.Empty;
        if (contract.ButtonKeywords is { Length: > 0 } && contract.ButtonLine is not null &&
            contract.ButtonKeywords.All(keyword => fullRecord.Contains(keyword, StringComparison.Ordinal)))
        {
            failure = RunEgaUiFailureKind.FixedColumn;
            detail = string.Format(UiText.Get("RuntimeUi.Detail.EgaButtons"), contract.ButtonLine);
            return false;
        }
        detail = UiText.Get("RuntimeUi.Detail.EgaStructure");
        return false;
    }

    /// <summary>Builds the exact output span (payload + NUL + zero padding).</summary>
    internal static bool TryBuildSpan(RuntimeUiLogicalRecordId id, string fullRecord, out byte[] span)
    {
        span = [];
        RunEgaUiFieldContract contract = Contract(id);
        if (!TryValidateStored(fullRecord, id, out _, out _)) return false;
        byte[] payload = GamePcTextEditor.GetEncoding("CP852").GetBytes(fullRecord);
        span = new byte[contract.Length];
        Array.Copy(payload, span, payload.Length);
        span[payload.Length] = 0x00;
        return true;
    }

    /// <summary>Validates ALL overrides before writing ANY (fail atomic).</summary>
    internal static void ApplyOverridesToImage(byte[] image, IReadOnlyList<RuntimeUiTextOverride> overrides)
    {
        if (image is null) throw new ArgumentNullException(nameof(image));
        if (image.Length != RunEgaBootstrapService.OutputSize)
            throw new InvalidDataException("RUNEGA Runtime UI materialization requires the frozen output image.");
        if (overrides is null) throw new ArgumentNullException(nameof(overrides));
        var spans = new List<(int Offset, byte[] Span)>();
        foreach (RuntimeUiTextOverride value in overrides)
        {
            RunEgaUiFieldContract contract = Contract(value.LogicalRecordId);
            if (!TryBuildSpan(value.LogicalRecordId, value.Text, out byte[] span))
                throw new InvalidDataException("Invalid RUNEGA Runtime UI override for " + contract.DisplayName + ".");
            spans.Add((UiBankPhysical + contract.BankOffset, span));
        }
        foreach ((int offset, byte[] span) in spans)
            Array.Copy(span, 0, image, offset, span.Length);
    }

    internal static void VerifyOnlyEgaSpansChanged(byte[] before, byte[] after, IReadOnlyList<RunEgaUiFieldContract> enabled)
    {
        if (before is null || after is null) throw new ArgumentNullException(before is null ? nameof(before) : nameof(after));
        if (before.Length != after.Length)
            throw new InvalidDataException("RUNEGA Runtime UI materialization must not change executable size.");
        if (before.Length != RunEgaBootstrapService.OutputSize)
            throw new InvalidDataException("RUNEGA Runtime UI materialization requires the frozen output image.");
        bool Inside(int index)
        {
            foreach (RunEgaUiFieldContract contract in enabled)
                if (index >= UiBankPhysical + contract.BankOffset && index < UiBankPhysical + contract.BankOffset + contract.Length)
                    return true;
            return false;
        }
        for (int i = 0; i < before.Length; i++)
            if (!Inside(i) && before[i] != after[i])
                throw new InvalidDataException($"RUNEGA Runtime UI materialization touched byte 0x{i:X} outside the authorized spans.");
        if (after[0] != (byte)'M' || after[1] != (byte)'Z')
            throw new InvalidDataException("Patched RUNEGA image is not a DOS executable.");
        // Font bank must be untouched by a Runtime UI patch (subset check
        // above already covers it; this names the invariant explicitly).
        FrozenFontDescriptor font = Elvira1ProductionProfile.RunEga.Font;
        for (int i = font.PhysicalFileOffset; i < font.PhysicalFileOffset + font.ByteLength; i++)
            if (before[i] != after[i])
                throw new InvalidDataException("RUNEGA Runtime UI materialization must not modify font bytes.");
    }

    private static bool StrictDecode(byte[] payload, out string? text, out string? error)
    {
        text = null; error = null;
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            string decoded = strict.GetString(payload);
            if (!string.Equals(decoded, strict.GetString(strict.GetBytes(decoded)), StringComparison.Ordinal))
            { error = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
            text = decoded;
            return true;
        }
        catch (EncoderFallbackException)
        { error = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
    }
}
