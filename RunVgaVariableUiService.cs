using System.Text;

namespace Pi1ElviraEditor;

/// <summary>Failure kind for variable-width RUNVGA structured records.
/// VisualCollision (a first label reaching the frozen second anchor) and
/// RowOverflow (exceeding the proven one-line visual width) are geometry
/// failures, never English-length limits.</summary>
internal enum RunVgaVariableFailureKind
{
    None,
    Empty,
    Encoding,
    Newline,
    FixedColumn,
    Hotspot,
    VisualCollision,
    RowOverflow,
    /// <summary>Defensive only: composers report geometry (collision/row
    /// overflow) instead of storage limits, since storage is free.</summary>
    TooLong
}

/// <summary>Materialization path for one validated VGA record: InPlace when
/// the composed payload fits the historical English envelope, otherwise
/// Appended (deterministic thunk + appended record).</summary>
internal enum RunVgaMaterializationKind
{
    InPlace,
    Appended
}

/// <summary>Production variable-width model for the three human-proven-live
/// RUNVGA structured records (Pause.menu, Confirm.generic, Save.overwrite).
/// Proven live (DOSBox, RVGBTN POC): translated labels render and operate
/// with FIXED start anchors and lengths differing from English; the
/// clickable hotspot is separate fixed geometry (Koniec char 5 clickable,
/// char 6 not; Nie fully clickable). Rules: TEXT MAY HAVE VARIABLE LENGTH;
/// ANCHOR / STRUCTURAL GEOMETRY REMAINS FROZEN; HOTSPOT IS NOT DERIVED FROM
/// TRANSLATED STRING LENGTH. Proven one-line visual widths: Pause option
/// line 21 columns, Confirm/Overwrite button line 18 columns, message lines
/// 21 columns (widest live-rendered content in this dialog family; policy
/// bound, reported as such). First labels must keep >=1 separating space
/// before the frozen second anchor. Storage is free (appended record when
/// the envelope is exceeded); NOTHING here applies to RUNEGA.</summary>
internal static class RunVgaVariableUiService
{
    internal const int PauseSpanOffset = 0x1A625;
    internal const int PauseSpanLength = 39;
    internal const int ConfirmSpanOffset = 0x1A64C;
    internal const int ConfirmSpanLength = 39;
    internal const int OverwriteSpanOffset = 0x1A6F6;
    internal const int OverwriteSpanLength = 62;

    internal const int PauseOptionRowMaxCols = 21;
    internal const int ConfirmButtonRowMaxCols = 18;
    internal const int MessageLineMaxCols = 21;

    internal const int PauseTitleMaxCols = 16;
    internal const int PauseContinueMaxCols = 13;
    internal const int PauseQuitMaxCols = 6;
    internal const int ConfirmPromptMaxCols = 17;
    internal const int ConfirmYesMaxCols = 9;
    internal const int ConfirmNoMaxCols = 3;
    internal const int OverwriteMessageMaxCols = 20;
    internal const int OverwriteQuestionMaxCols = 17;

    internal const int PauseContinueColumn = 1;
    internal const int PauseQuitColumn = 15;
    internal const int ConfirmYesColumn = 5;
    internal const int ConfirmNoColumn = 15;

    internal const string PauseIndent = "     ";
    internal const string PauseSeparators = "\r\r\r";
    internal const string PauseButtonIndent = " ";
    internal const string ConfirmIndent = "    ";
    internal const string OverwritePrefix = "\r ";
    internal const string OverwriteQuestionIndent = "    ";
    internal const string OverwriteQuestionFrozen = "Overwrite it ?";
    internal const string ButtonIndent = "     ";

    internal static bool IsVariableRecord(RuntimeUiLogicalRecordId id) => id is
        RuntimeUiLogicalRecordId.PauseMenu or
        RuntimeUiLogicalRecordId.ConfirmGeneric or
        RuntimeUiLogicalRecordId.SaveOverwrite;

    internal static int SpanOffset(RuntimeUiLogicalRecordId id) => id switch
    {
        RuntimeUiLogicalRecordId.PauseMenu => PauseSpanOffset,
        RuntimeUiLogicalRecordId.ConfirmGeneric => ConfirmSpanOffset,
        RuntimeUiLogicalRecordId.SaveOverwrite => OverwriteSpanOffset,
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };

    internal static int SpanLength(RuntimeUiLogicalRecordId id) => id switch
    {
        RuntimeUiLogicalRecordId.PauseMenu => PauseSpanLength,
        RuntimeUiLogicalRecordId.ConfirmGeneric => ConfirmSpanLength,
        RuntimeUiLogicalRecordId.SaveOverwrite => OverwriteSpanLength,
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };

    internal static int Cols(string value) => GamePcTextEditor.GetEncoding("CP852").GetByteCount(value);

    /// <summary>Authoritative original full record decoded from a game root
    /// (packed canonicalizes in memory; unpacked/V5 direct).</summary>
    internal static bool TryDecodeOriginalFromGameRoot(string gameRoot, RuntimeUiLogicalRecordId id, out string? text, out string? error)
    {
        text = null; error = null;
        if (!IsVariableRecord(id)) throw new ArgumentOutOfRangeException(nameof(id));
        try
        {
            string candidate = Path.Combine(gameRoot, Elvira1ProductionProfile.ActiveVgaExecutable);
            if (!File.Exists(candidate))
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
            byte[] data = File.ReadAllBytes(candidate);
            RunVgaBootstrapState state = RunVgaBootstrapService.DetectState(candidate);
            byte[] image = state switch
            {
                RunVgaBootstrapState.UnpackedBaseline => data,
                RunVgaBootstrapState.ExtendedCp852V5 => data,
                RunVgaBootstrapState.OriginalPacked => RunVgaBootstrapService.UnpackVerifiedOriginal(data),
                _ => []
            };
            if (image.Length == 0)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
            return TryDecodeSpan(image, id, out text, out error);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
    }

    internal static bool TryDecodeSpan(byte[] image, RuntimeUiLogicalRecordId id, out string? text, out string? error)
    {
        text = null; error = null;
        int offset = SpanOffset(id);
        int length = SpanLength(id);
        if (image is null || image.Length < offset + length)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        byte[] span = image.AsSpan(offset, length).ToArray();
        int nul = Array.IndexOf(span, (byte)0x00);
        if (nul < 0)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        for (int i = nul + 1; i < span.Length; i++)
            if (span[i] != 0x00)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            string decoded = strict.GetString(span, 0, nul);
            if (!string.Equals(decoded, strict.GetString(strict.GetBytes(decoded)), StringComparison.Ordinal))
            { error = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
            text = decoded;
            return true;
        }
        catch (EncoderFallbackException)
        { error = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
    }

    /// <summary>Semantic decomposition into display rows (message rows plus
    /// variable button rows with geometry hints). Parse-only.</summary>
    internal static bool TryDecompose(RuntimeUiLogicalRecordId id, string? fullRecord, out IReadOnlyList<RunEgaUiSemanticField> rows)
    {
        rows = [];
        if (string.IsNullOrEmpty(fullRecord)) return false;
        if (!TryParse(id, fullRecord, out IReadOnlyDictionary<string, string>? fields) || fields is null) return false;
        string hint = UiText.Get("RuntimeUi.Detail.VgaVariableButtonHint");
        List<RunEgaUiSemanticField> output = id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu =>
            [
                new("title", UiText.Get("RuntimeUi.Semantic.Title"), fields["title"], RunEgaUiSemanticRole.EditableSafe, string.Empty),
                new("continue", UiText.Get("RuntimeUi.Semantic.Continue"), fields["continue"], RunEgaUiSemanticRole.EditableSafe, hint),
                new("quit", UiText.Get("RuntimeUi.Semantic.Quit"), fields["quit"], RunEgaUiSemanticRole.EditableSafe, hint),
            ],
            RuntimeUiLogicalRecordId.ConfirmGeneric =>
            [
                new("prompt", UiText.Get("RuntimeUi.Semantic.Prompt"), fields["prompt"], RunEgaUiSemanticRole.EditableSafe, string.Empty),
                new("yes", UiText.Get("RuntimeUi.Semantic.Yes"), fields["yes"], RunEgaUiSemanticRole.EditableSafe, hint),
                new("no", UiText.Get("RuntimeUi.Semantic.No"), fields["no"], RunEgaUiSemanticRole.EditableSafe, hint),
            ],
            RuntimeUiLogicalRecordId.SaveOverwrite =>
            [
                new("message", UiText.Get("RuntimeUi.Semantic.Message"), fields["message"], RunEgaUiSemanticRole.EditableSafe, string.Empty),
                new("question", UiText.Get("RuntimeUi.Semantic.Question"), fields["question"], RunEgaUiSemanticRole.EditableSafe, string.Empty),
                new("yes", UiText.Get("RuntimeUi.Semantic.Yes"), fields["yes"], RunEgaUiSemanticRole.EditableSafe, hint),
                new("no", UiText.Get("RuntimeUi.Semantic.No"), fields["no"], RunEgaUiSemanticRole.EditableSafe, hint),
            ],
            _ => [],
        };
        if (output.Count == 0) return false;
        rows = output;
        return true;
    }

    /// <summary>Strict structural parse of a full record into semantic
    /// fields. Separators, indents and anchors are frozen; label glyphs and
    /// the inter-button gap vary (gap = spaces holding the second anchor).</summary>
    internal static bool TryParse(RuntimeUiLogicalRecordId id, string fullRecord, out IReadOnlyDictionary<string, string>? fields)
    {
        fields = null;
        if (string.IsNullOrEmpty(fullRecord)) return false;
        Dictionary<string, string> map = new(StringComparer.Ordinal);
        switch (id)
        {
            case RuntimeUiLogicalRecordId.PauseMenu:
                if (!fullRecord.StartsWith(PauseIndent, StringComparison.Ordinal)) return false;
                {
                    string rest = fullRecord.Substring(PauseIndent.Length);
                    int cr = rest.IndexOf('\r');
                    if (cr < 0 || !rest.Substring(cr).StartsWith(PauseSeparators, StringComparison.Ordinal)) return false;
                    string line = rest.Substring(cr + PauseSeparators.Length);
                    if (!TrySplitButtonLine(line, PauseButtonIndent.Length, PauseContinueColumn, PauseQuitColumn, out string cont, out string quit)) return false;
                    map["title"] = rest.Substring(0, cr);
                    map["continue"] = cont;
                    map["quit"] = quit;
                }
                break;
            case RuntimeUiLogicalRecordId.ConfirmGeneric:
                if (!fullRecord.StartsWith(ConfirmIndent, StringComparison.Ordinal)) return false;
                {
                    string rest = fullRecord.Substring(ConfirmIndent.Length);
                    int cr = rest.IndexOf('\r');
                    if (cr < 0 || !rest.Substring(cr).StartsWith(PauseSeparators, StringComparison.Ordinal)) return false;
                    string line = rest.Substring(cr + PauseSeparators.Length);
                    if (!TrySplitButtonLine(line, ButtonIndent.Length, ConfirmYesColumn, ConfirmNoColumn, out string yes, out string no)) return false;
                    map["prompt"] = rest.Substring(0, cr);
                    map["yes"] = yes;
                    map["no"] = no;
                }
                break;
            case RuntimeUiLogicalRecordId.SaveOverwrite:
                if (!fullRecord.StartsWith(OverwritePrefix, StringComparison.Ordinal)) return false;
                {
                    string rest = fullRecord.Substring(OverwritePrefix.Length);
                    int cr = rest.IndexOf('\r');
                    if (cr < 0 || !rest.Substring(cr).StartsWith("\r\r", StringComparison.Ordinal)) return false;
                    string afterFirst = rest.Substring(cr + 2);
                    if (!afterFirst.StartsWith(OverwriteQuestionIndent, StringComparison.Ordinal)) return false;
                    string questionRest = afterFirst.Substring(OverwriteQuestionIndent.Length);
                    int cr2 = questionRest.IndexOf('\r');
                    if (cr2 < 0 || !questionRest.Substring(cr2).StartsWith("\r\r", StringComparison.Ordinal)) return false;
                    string line = questionRest.Substring(cr2 + 2);
                    if (!TrySplitButtonLine(line, ButtonIndent.Length, ConfirmYesColumn, ConfirmNoColumn, out string yes, out string no)) return false;
                    map["message"] = rest.Substring(0, cr);
                    map["question"] = questionRest.Substring(0, cr2);
                    map["yes"] = yes;
                    map["no"] = no;
                }
                break;
            default:
                return false;
        }
        fields = map;
        return true;
    }

    /// <summary>Splits a button line by frozen anchors: exactly
    /// <paramref name="indentCols"/> leading spaces, first label from
    /// <paramref name="firstColumn"/>, second label from
    /// <paramref name="secondColumn"/> to end of line. The gap between
    /// labels must be >=1 space (collision otherwise); it flexes so the
    /// second anchor never moves. No length maxima here: geometry checks
    /// belong to validation, keeping parse and policy separate.</summary>
    private static bool TrySplitButtonLine(string line, int indentCols, int firstColumn, int secondColumn, out string first, out string second)
    {
        first = string.Empty; second = string.Empty;
        if (line.Length <= secondColumn || firstColumn < indentCols || secondColumn <= firstColumn) return false;
        for (int i = 0; i < indentCols; i++)
            if (line[i] != ' ') return false;
        if (line.Length <= firstColumn) return false;
        int gapStart = firstColumn;
        while (gapStart < secondColumn && line[gapStart] != ' ') gapStart++;
        first = line.Substring(firstColumn, gapStart - firstColumn);
        if (first.Length == 0) return false;
        for (int i = gapStart; i < secondColumn; i++)
            if (line[i] != ' ') return false;
        if (gapStart == secondColumn) return false;
        second = line.Substring(secondColumn);
        if (second.Length == 0 || second.IndexOfAny(['\r', '\n', '\0']) >= 0) return false;
        return true;
    }

    /// <summary>Validates + composes semantic fields into the physical full
    /// record. Button labels are trimmed; the gap flexes with spaces so the
    /// second anchor never moves. Unknown fields throw.</summary>
    internal static bool TryCompose(RuntimeUiLogicalRecordId id, IReadOnlyDictionary<string, string>? edits, out string fullRecord, out RunVgaVariableFailureKind failure, out string detail)
    {
        fullRecord = string.Empty; failure = RunVgaVariableFailureKind.None; detail = string.Empty;
        if (!IsVariableRecord(id)) throw new ArgumentOutOfRangeException(nameof(id));
        if (edits is null) throw new ArgumentNullException(nameof(edits));
        string Get(string key)
        {
            if (!edits.TryGetValue(key, out string? value) || value is null)
                throw new ArgumentException("Missing semantic field '" + key + "'.", nameof(edits));
            return value;
        }
        var fields = new Dictionary<string, string>(StringComparer.Ordinal);
        switch (id)
        {
            case RuntimeUiLogicalRecordId.PauseMenu:
                if (!CheckLabel(Get("title"), out string title, out failure, out detail)) return false;
                if (!CheckLabel(Get("continue"), out string cont, out failure, out detail)) return false;
                if (!CheckLabel(Get("quit"), out string quit, out failure, out detail)) return false;
                fields["title"] = title; fields["continue"] = cont; fields["quit"] = quit;
                break;
            case RuntimeUiLogicalRecordId.ConfirmGeneric:
                if (!CheckLabel(Get("prompt"), out string prompt, out failure, out detail)) return false;
                if (!CheckLabel(Get("yes"), out string yes, out failure, out detail)) return false;
                if (!CheckLabel(Get("no"), out string no, out failure, out detail)) return false;
                fields["prompt"] = prompt; fields["yes"] = yes; fields["no"] = no;
                break;
            case RuntimeUiLogicalRecordId.SaveOverwrite:
                if (!CheckLabel(Get("message"), out string message, out failure, out detail)) return false;
                if (!CheckLabel(Get("question"), out string question, out failure, out detail)) return false;
                if (!CheckLabel(Get("yes"), out string yes2, out failure, out detail)) return false;
                if (!CheckLabel(Get("no"), out string no2, out failure, out detail)) return false;
                fields["message"] = message; fields["question"] = question; fields["yes"] = yes2; fields["no"] = no2;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(id));
        }
        // Unknown-field guard: exactly the known keys, no more.
        int known = id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => 3,
            RuntimeUiLogicalRecordId.ConfirmGeneric => 3,
            RuntimeUiLogicalRecordId.SaveOverwrite => 4,
            _ => 0
        };
        if (edits.Count != known)
            throw new ArgumentException("Unexpected semantic field set for '" + id + "'.", nameof(edits));
        if (!ValidateGeometry(id, fields, out failure, out detail)) return false;
        fullRecord = BuildSegments(id, fields);
        return true;
    }

    /// <summary>Pure segment splice (no validation): prefix + fields +
    /// separators + indent + first + flex gap + second. The gap holds the
    /// second anchor fixed regardless of first-label length.</summary>
    private static string BuildSegments(RuntimeUiLogicalRecordId id, IReadOnlyDictionary<string, string> fields)
    {
        switch (id)
        {
            case RuntimeUiLogicalRecordId.PauseMenu:
                return PauseIndent + fields["title"] + PauseSeparators + PauseButtonIndent + fields["continue"]
                    + new string(' ', PauseQuitColumn - PauseContinueColumn - Cols(fields["continue"])) + fields["quit"];
            case RuntimeUiLogicalRecordId.ConfirmGeneric:
                return ConfirmIndent + fields["prompt"] + PauseSeparators + ButtonIndent + fields["yes"]
                    + new string(' ', ConfirmNoColumn - ConfirmYesColumn - Cols(fields["yes"])) + fields["no"];
            case RuntimeUiLogicalRecordId.SaveOverwrite:
                return OverwritePrefix + fields["message"] + "\r\r" + OverwriteQuestionIndent + fields["question"]
                    + "\r\r" + ButtonIndent + fields["yes"]
                    + new string(' ', ConfirmNoColumn - ConfirmYesColumn - Cols(fields["yes"])) + fields["no"];
            default:
                throw new ArgumentOutOfRangeException(nameof(id));
        }
    }

    /// <summary>Geometry policy shared by composition and stored-state
    /// validation: non-empty single-line fields, message-line bounds,
    /// first-label collision (gap >= 1 before the frozen second anchor),
    /// second-label row bounds. No English lengths anywhere.</summary>
    private static bool ValidateGeometry(RuntimeUiLogicalRecordId id, IReadOnlyDictionary<string, string> fields, out RunVgaVariableFailureKind failure, out string detail)
    {
        failure = RunVgaVariableFailureKind.None; detail = string.Empty;
        foreach ((string key, string value) in fields)
        {
            if (value.Length == 0)
            { failure = RunVgaVariableFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
            if (value.IndexOfAny(['\r', '\n']) >= 0)
            { failure = RunVgaVariableFailureKind.Newline; detail = UiText.Get("RuntimeUi.Detail.PauseNewline"); return false; }
        }
        switch (id)
        {
            case RuntimeUiLogicalRecordId.PauseMenu:
                if (Cols(fields["title"]) > PauseTitleMaxCols || 5 + Cols(fields["title"]) > MessageLineMaxCols)
                { failure = RunVgaVariableFailureKind.RowOverflow; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaRowOverflow"), MessageLineMaxCols); return false; }
                if (Cols(fields["continue"]) > PauseContinueMaxCols || PauseContinueColumn + Cols(fields["continue"]) >= PauseQuitColumn)
                { failure = RunVgaVariableFailureKind.VisualCollision; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaButtonCollision"), "Continue", "Quit"); return false; }
                if (Cols(fields["quit"]) > PauseQuitMaxCols || PauseQuitColumn + Cols(fields["quit"]) > PauseOptionRowMaxCols)
                { failure = RunVgaVariableFailureKind.RowOverflow; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaRowOverflow"), PauseOptionRowMaxCols); return false; }
                return true;
            case RuntimeUiLogicalRecordId.ConfirmGeneric:
                if (Cols(fields["prompt"]) > ConfirmPromptMaxCols || 4 + Cols(fields["prompt"]) > MessageLineMaxCols)
                { failure = RunVgaVariableFailureKind.RowOverflow; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaRowOverflow"), MessageLineMaxCols); return false; }
                if (Cols(fields["yes"]) > ConfirmYesMaxCols || ConfirmYesColumn + Cols(fields["yes"]) >= ConfirmNoColumn)
                { failure = RunVgaVariableFailureKind.VisualCollision; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaButtonCollision"), "Yes", "No"); return false; }
                if (Cols(fields["no"]) > ConfirmNoMaxCols || ConfirmNoColumn + Cols(fields["no"]) > ConfirmButtonRowMaxCols)
                { failure = RunVgaVariableFailureKind.RowOverflow; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaRowOverflow"), ConfirmButtonRowMaxCols); return false; }
                return true;
            case RuntimeUiLogicalRecordId.SaveOverwrite:
                if (Cols(fields["message"]) > OverwriteMessageMaxCols || 1 + Cols(fields["message"]) > MessageLineMaxCols)
                { failure = RunVgaVariableFailureKind.RowOverflow; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaRowOverflow"), MessageLineMaxCols); return false; }
                if (Cols(fields["question"]) > OverwriteQuestionMaxCols || 4 + Cols(fields["question"]) > MessageLineMaxCols)
                { failure = RunVgaVariableFailureKind.RowOverflow; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaRowOverflow"), MessageLineMaxCols); return false; }
                if (Cols(fields["yes"]) > ConfirmYesMaxCols || ConfirmYesColumn + Cols(fields["yes"]) >= ConfirmNoColumn)
                { failure = RunVgaVariableFailureKind.VisualCollision; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaButtonCollision"), "Yes", "No"); return false; }
                if (Cols(fields["no"]) > ConfirmNoMaxCols || ConfirmNoColumn + Cols(fields["no"]) > ConfirmButtonRowMaxCols)
                { failure = RunVgaVariableFailureKind.RowOverflow; detail = string.Format(UiText.Get("RuntimeUi.Detail.VgaRowOverflow"), ConfirmButtonRowMaxCols); return false; }
                return true;
            default:
                throw new ArgumentOutOfRangeException(nameof(id));
        }
    }

    /// <summary>Trims + strict-CP852-encodes one semantic label (no
    /// CR/LF/NUL, non-empty, no HUD glyph). Length budgets are enforced by
    /// the geometry checks, never here.</summary>
    private static bool CheckLabel(string raw, out string label, out RunVgaVariableFailureKind failure, out string detail)
    {
        label = (raw ?? string.Empty).Trim();
        failure = RunVgaVariableFailureKind.None; detail = string.Empty;
        if (label.Length == 0)
        { failure = RunVgaVariableFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (label.IndexOf('\0') >= 0)
        { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        byte[] encoded;
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            encoded = strict.GetBytes(label);
            if (!string.Equals(label, strict.GetString(encoded), StringComparison.Ordinal))
            { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        if (Array.FindIndex(encoded, value => value < 0x20 || value == 0x7F) >= 0)
        { failure = RunVgaVariableFailureKind.Newline; detail = UiText.Get("RuntimeUi.Detail.PauseNewline"); return false; }
        if (Array.IndexOf(encoded, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
        { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        return true;
    }

    /// <summary>Contract validation of a composed full record: frozen
    /// structure, anchors, one-line rows, column budgets. Used by grids,
    /// preflight and the materializer.</summary>
    internal static bool TryValidateStored(string? fullRecord, RuntimeUiLogicalRecordId id, out RunVgaVariableFailureKind failure, out string detail)
    {
        failure = RunVgaVariableFailureKind.None; detail = string.Empty;
        if (!IsVariableRecord(id)) throw new ArgumentOutOfRangeException(nameof(id));
        if (string.IsNullOrEmpty(fullRecord))
        { failure = RunVgaVariableFailureKind.Empty; detail = UiText.Get("RuntimeUi.Detail.EgaFieldEmpty"); return false; }
        if (fullRecord.IndexOf('\0') >= 0)
        { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            byte[] payload = strict.GetBytes(fullRecord);
            if (!string.Equals(fullRecord, strict.GetString(payload), StringComparison.Ordinal))
            { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
            if (Array.IndexOf(payload, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
            { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunVgaVariableFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        if (!TryParse(id, fullRecord, out IReadOnlyDictionary<string, string>? fields) || fields is null)
        {
            bool hasBoth = id switch
            {
                RuntimeUiLogicalRecordId.PauseMenu => fullRecord.Contains("Continue", StringComparison.Ordinal) && fullRecord.Contains("Quit", StringComparison.Ordinal),
                _ => fullRecord.Contains("Yes", StringComparison.Ordinal) && fullRecord.Contains("No", StringComparison.Ordinal)
            };
            if (hasBoth)
            { failure = RunVgaVariableFailureKind.FixedColumn; detail = UiText.Get("RuntimeUi.Detail.PauseFixedColumn"); return false; }
            failure = RunVgaVariableFailureKind.Hotspot; detail = UiText.Get("RuntimeUi.Detail.PauseHotspot"); return false;
        }
        // Re-validate parsed fields through the shared geometry policy so
        // stored state (including hand-written JSON) meets the same rules,
        // then require canonical composition (columns hold, but only
        // canonical gap structure persists).
        var edits = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach ((string key, string value) in fields) edits[key] = value;
        if (!ValidateGeometry(id, edits, out failure, out detail)) return false;
        if (!BuildSegments(id, edits).Equals(fullRecord, StringComparison.Ordinal))
        {
            failure = RunVgaVariableFailureKind.FixedColumn; detail = UiText.Get("RuntimeUi.Detail.PauseFixedColumn"); return false;
        }
        return true;
    }

    /// <summary>Migration for old VGA Pause title-only overrides: the saved
    /// title carries over; Continue/Quit reset to the English originals per
    /// the frozen contract. Returns false for non-title-only shapes.</summary>
    internal static bool TryMigratePauseTitle(string? fullRecord, out string title, out string migrated)
    {
        title = string.Empty; migrated = string.Empty;
        if (!RunVgaPauseMenuService.TryExtractTitle(fullRecord, out string? extracted) || extracted is null) return false;
        title = extracted;
        var edits = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["title"] = title,
            ["continue"] = "Continue",
            ["quit"] = "Quit"
        };
        return TryCompose(RuntimeUiLogicalRecordId.PauseMenu, edits, out migrated, out _, out _);
    }

    /// <summary>In-place decision: payload+NUL fits the historical envelope.</summary>
    internal static RunVgaMaterializationKind Decide(RuntimeUiLogicalRecordId id, string fullRecord)
    {
        int bytes = GamePcTextEditor.GetEncoding("CP852").GetByteCount(fullRecord);
        return checked(bytes + 1) <= SpanLength(id) ? RunVgaMaterializationKind.InPlace : RunVgaMaterializationKind.Appended;
    }

    /// <summary>Builds the exact in-place span (payload + NUL + zero pad).</summary>
    internal static bool TryBuildInPlaceSpan(RuntimeUiLogicalRecordId id, string fullRecord, out byte[] span)
    {
        span = [];
        if (!TryValidateStored(fullRecord, id, out _, out _) || Decide(id, fullRecord) != RunVgaMaterializationKind.InPlace) return false;
        byte[] payload = GamePcTextEditor.GetEncoding("CP852").GetBytes(fullRecord);
        span = new byte[SpanLength(id)];
        Array.Copy(payload, span, payload.Length);
        span[payload.Length] = 0x00;
        return true;
    }

    /// <summary>Deterministic appended-record materialization for overflow
    /// records, promoting the human-proven RVGBTN POC architecture: one
    /// per-record thunk (CS:DI loop into the outer renderer, post-call DI
    /// restore) plus NUL-terminated relocated records in fixed slots.
    /// Fixed slot map (append-relative): Pause thunk 0x00 / text 0xC0,
    /// Confirm thunk 0x40 / text 0x100, Overwrite thunk 0x80 / text 0x140;
    /// append envelope 0x200. Only overflow records occupy slots, at fixed
    /// offsets regardless of subset, so identical inputs always produce
    /// identical bytes. Fitting records stay in place; original source
    /// records are never patched when appending.</summary>
    internal static class Appender
    {
        internal const int AppendSize = 0x200;
        internal const int AppendSegment = 0x3926;
        internal const int ThunkPause = 0x00;
        internal const int ThunkConfirm = 0x40;
        internal const int ThunkOverwrite = 0x80;
        internal const int TextPause = 0xC0;
        internal const int TextConfirm = 0x100;
        internal const int TextOverwrite = 0x140;

        internal const int CallPause = 0xCCF7;
        internal const int CallConfirm = 0xCD29;
        internal const int CallOverwrite = 0xCBFC;

        internal const int PostDiPause = 0x2CDC;
        internal const int PostDiConfirm = 0x2D03;
        internal const int PostDiOverwrite = 0x2DC4;

        internal static int ThunkOffset(RuntimeUiLogicalRecordId id) => id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => ThunkPause,
            RuntimeUiLogicalRecordId.ConfirmGeneric => ThunkConfirm,
            RuntimeUiLogicalRecordId.SaveOverwrite => ThunkOverwrite,
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };

        internal static int TextOffset(RuntimeUiLogicalRecordId id) => id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => TextPause,
            RuntimeUiLogicalRecordId.ConfirmGeneric => TextConfirm,
            RuntimeUiLogicalRecordId.SaveOverwrite => TextOverwrite,
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };

        internal static int CallSite(RuntimeUiLogicalRecordId id) => id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => CallPause,
            RuntimeUiLogicalRecordId.ConfirmGeneric => CallConfirm,
            RuntimeUiLogicalRecordId.SaveOverwrite => CallOverwrite,
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };

        internal static int PostDi(RuntimeUiLogicalRecordId id) => id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => PostDiPause,
            RuntimeUiLogicalRecordId.ConfirmGeneric => PostDiConfirm,
            RuntimeUiLogicalRecordId.SaveOverwrite => PostDiOverwrite,
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        };

        internal static byte[] ThunkBytes(int textOffset, int postDi) => new byte[]
        {
            0x52, 0xB6, 0x00, 0xBF, (byte)textOffset, (byte)(textOffset >> 8),
            0x2E, 0x8A, 0x15, 0x47, 0x0A, 0xD2, 0x74, 0x06, 0x0E, 0xE8, 0x07, 0x00,
            0xEB, 0xF2, 0xBF, (byte)postDi, (byte)(postDi >> 8), 0x5A, 0xCB,
            0xBB, 0x75, 0x02, 0x53, 0xEA, 0x76, 0x02, 0xBD, 0x0C
        };

        private static ushort U16(byte[] image, int offset) => (ushort)(image[offset] | (image[offset + 1] << 8));

        private static void W16(byte[] image, int offset, ushort value)
        {
            image[offset] = (byte)value;
            image[offset + 1] = (byte)(value >> 8);
        }

        private static bool HasRelocation(byte[] image, int physical)
        {
            int header = U16(image, 8) * 16;
            int count = U16(image, 6);
            int table = U16(image, 0x18);
            for (int i = 0; i < count; i++)
            {
                int entry = table + i * 4;
                if (header + U16(image, entry + 2) * 16 + U16(image, entry) == physical) return true;
            }
            return false;
        }

        /// <summary>Applies validated VGA overrides to a frozen V5 image:
        /// fitting records in place, overflow records via one deterministic
        /// append. All overrides validate before any byte changes (atomic);
        /// the result re-validates structurally with an explicit whitelist.
        /// Pristine GameRoot is never touched (caller-owned output only).</summary>
        internal static byte[] Materialize(byte[] v5image, IReadOnlyDictionary<RuntimeUiLogicalRecordId, string> overrides)
        {
            if (v5image is null) throw new ArgumentNullException(nameof(v5image));
            if (v5image.Length != RunVgaBootstrapService.V5Size)
                throw new InvalidDataException("Variable VGA materialization requires the frozen V5 executable image.");
            if (overrides is null) throw new ArgumentNullException(nameof(overrides));
            var ordered = overrides.OrderBy(pair => pair.Key).Select(pair => (Id: pair.Key, Full: pair.Value)).ToArray();
            foreach ((RuntimeUiLogicalRecordId id, string full) in ordered)
            {
                if (!IsVariableRecord(id))
                    throw new InvalidDataException("Record '" + id + "' is not a variable-width VGA record.");
                if (!TryValidateStored(full, id, out _, out string detail))
                    throw new InvalidDataException("Invalid VGA override for '" + id + "': " + detail);
            }
            var overflow = ordered.Where(pair => Decide(pair.Id, pair.Full) == RunVgaMaterializationKind.Appended).ToArray();
            byte[] before = (byte[])v5image.Clone();
            byte[] after;
            if (overflow.Length == 0)
            {
                // In-place only: spans, no MZ/relocation/margin changes.
                after = (byte[])v5image.Clone();
                foreach ((RuntimeUiLogicalRecordId id, string full) in ordered)
                {
                    if (!TryBuildInPlaceSpan(id, full, out byte[] span))
                        throw new InvalidDataException("In-place VGA materialization failed for '" + id + "'.");
                    Array.Copy(span, 0, after, SpanOffset(id), span.Length);
                }
                VerifyOnlyVgaSpansChanged(before, after, ordered.Select(pair => pair.Id).ToArray());
            }
            else
            {
                after = ApplyAppended(before, ordered, overflow);
            }
            // Round-trip: every override decodes back exactly.
            var appendedIds = overflow.Select(pair => pair.Id).ToHashSet();
            foreach ((RuntimeUiLogicalRecordId id, string full) in ordered)
            {
                if (!TryDecodeMaterialized(after, id, appendedIds.Contains(id), out string? roundTrip, out _))
                    throw new InvalidDataException("Materialized VGA record did not decode for '" + id + "'.");
                if (!roundTrip!.Equals(full, StringComparison.Ordinal))
                    throw new InvalidDataException("Materialized VGA record diverged for '" + id + "'.");
            }
            return after;
        }

        private static byte[] ApplyAppended(byte[] before, (RuntimeUiLogicalRecordId Id, string Full)[] ordered, (RuntimeUiLogicalRecordId Id, string Full)[] overflow)
        {
            int header = U16(before, 8) * 16;
            if (header != 0x2400 || before.Length - header != 0x39260)
                throw new InvalidDataException("Variable VGA materialization requires the frozen V5 module geometry.");
            if (!before.AsSpan(0x11638, 4).SequenceEqual(new byte[] { 0x81, 0xC3, 0x80, 0x00 }))
                throw new InvalidDataException("VGA startup resize sequence diverged; margin fix refused.");
            int relocCount = U16(before, 6);
            int relocTable = U16(before, 0x18);
            if (relocTable + (relocCount + overflow.Length) * 4 > header)
                throw new InvalidDataException("VGA relocation table is full.");
            var overflowIds = overflow.Select(pair => pair.Id).ToHashSet();
            // Pre-verify every callsite still holds the original call plus
            // its retained relocation (fail closed on double-apply).
            foreach ((RuntimeUiLogicalRecordId id, _) in overflow)
            {
                int site = CallSite(id);
                if (!before.AsSpan(site, 5).SequenceEqual(new byte[] { 0x9A, 0x92, 0x00, 0xBD, 0x0C }))
                    throw new InvalidDataException("VGA callsite 0x" + site.ToString("X") + " is not the original call.");
                if (!HasRelocation(before, site + 3))
                    throw new InvalidDataException("VGA callsite relocation missing at 0x" + (site + 3).ToString("X") + ".");
            }
            int appendBase = before.Length;
            var grown = new byte[before.Length + AppendSize];
            Array.Copy(before, grown, before.Length);
            foreach ((RuntimeUiLogicalRecordId id, string full) in overflow)
            {
                byte[] thunk = ThunkBytes(TextOffset(id), PostDi(id));
                Array.Copy(thunk, 0, grown, appendBase + ThunkOffset(id), thunk.Length);
                byte[] payload = GamePcTextEditor.GetEncoding("CP852").GetBytes(full);
                if (TextOffset(id) + payload.Length + 1 > AppendSize)
                    throw new InvalidDataException("VGA appended record exceeds its slot for '" + id + "'.");
                Array.Copy(payload, 0, grown, appendBase + TextOffset(id), payload.Length);
                grown[appendBase + TextOffset(id) + payload.Length] = 0x00;
            }
            foreach ((RuntimeUiLogicalRecordId id, _) in overflow)
            {
                int site = CallSite(id);
                int target = ThunkOffset(id);
                grown[site] = 0x9A;
                grown[site + 1] = (byte)target;
                grown[site + 2] = (byte)((target >> 8) & 0xFF);
                grown[site + 3] = (byte)(AppendSegment & 0xFF);
                grown[site + 4] = (byte)((AppendSegment >> 8) & 0xFF);
                int at = relocTable + U16(grown, 6) * 4;
                W16(grown, at, (ushort)(ThunkOffset(id) + 0x20));
                W16(grown, at + 2, (ushort)(AppendSegment & 0xFFFF));
                W16(grown, 6, (ushort)(U16(grown, 6) + 1));
            }
            grown[0x1163A] = 0xA0;
            W16(grown, 2, (ushort)(grown.Length % 512));
            W16(grown, 4, (ushort)((grown.Length + 511) / 512));
            VerifyAppended(before, grown, ordered, overflowIds);
            return grown;
        }

        private static bool TryDecodeMaterialized(byte[] image, RuntimeUiLogicalRecordId id, bool appended, out string? text, out string? error)
        {
            text = null; error = null;
            if (!appended) return TryDecodeSpan(image, id, out text, out error);
            try
            {
                int appendBase = RunVgaBootstrapService.V5Size;
                int start = appendBase + TextOffset(id);
                int end = start;
                while (image[end] != 0x00) end++;
                Encoding source = GamePcTextEditor.GetEncoding("CP852");
                Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                text = strict.GetString(image, start, end - start);
                return true;
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException or ArgumentException or EncoderFallbackException)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        }

        internal static void VerifyOnlyVgaSpansChanged(byte[] before, byte[] after, IReadOnlyCollection<RuntimeUiLogicalRecordId> used)
        {
            if (before is null || after is null) throw new ArgumentNullException(before is null ? nameof(before) : nameof(after));
            if (before.Length != after.Length)
                throw new InvalidDataException("VGA materialization must not change executable size on the in-place path.");
            if (before.Length != RunVgaBootstrapService.V5Size)
                throw new InvalidDataException("VGA materialization requires the frozen V5 executable image.");
            bool Inside(int index)
            {
                foreach (RuntimeUiLogicalRecordId id in used)
                    if (index >= SpanOffset(id) && index < SpanOffset(id) + SpanLength(id))
                        return true;
                return false;
            }
            for (int i = 0; i < before.Length; i++)
                if (!Inside(i) && before[i] != after[i])
                    throw new InvalidDataException($"VGA materialization touched byte 0x{i:X} outside the authorized spans.");
            if (after[0] != (byte)'M' || after[1] != (byte)'Z')
                throw new InvalidDataException("Patched V5 image is not a DOS executable.");
            for (int i = RunVgaBootstrapService.V5FontOffset; i < RunVgaBootstrapService.V5FontOffset + RunVgaBootstrapService.V5FontSize; i++)
                if (before[i] != after[i])
                    throw new InvalidDataException("VGA materialization must not modify font bytes.");
        }

        /// <summary>Explicit mutation whitelist for the appended path: used
        /// in-place spans, the 0x200 append region, patched callsites (5
        /// bytes each), the margin byte, new relocation entries, and MZ
        /// size/count fields. Everything else must be byte-identical,
        /// including font/HUD bytes and shared failure dispatchers.</summary>
        internal static void VerifyAppended(byte[] before, byte[] after, (RuntimeUiLogicalRecordId Id, string Full)[] ordered, HashSet<RuntimeUiLogicalRecordId> appended)
        {
            if (before is null || after is null) throw new ArgumentNullException(before is null ? nameof(before) : nameof(after));
            if (after.Length != before.Length + AppendSize)
                throw new InvalidDataException("VGA appended output has the wrong size.");
            var allowed = new HashSet<int>();
            foreach ((RuntimeUiLogicalRecordId id, _) in ordered)
            {
                if (appended.Contains(id)) continue;
                for (int i = SpanOffset(id); i < SpanOffset(id) + SpanLength(id); i++) allowed.Add(i);
            }
            for (int i = before.Length; i < after.Length; i++) allowed.Add(i);
            foreach (RuntimeUiLogicalRecordId id in appended)
                for (int i = CallSite(id); i < CallSite(id) + 5; i++) allowed.Add(i);
            allowed.Add(0x1163A);
            int table = U16(before, 0x18);
            int countBefore = U16(before, 6);
            for (int i = table + countBefore * 4; i < table + U16(after, 6) * 4; i++) allowed.Add(i);
            foreach (int field in new[] { 2, 3, 4, 5, 6, 7 }) allowed.Add(field);
            int min = Math.Min(before.Length, after.Length);
            for (int i = 0; i < min; i++)
                if (!allowed.Contains(i) && before[i] != after[i])
                    throw new InvalidDataException($"VGA appended materialization touched byte 0x{i:X} outside the whitelist.");
            if (after[0] != (byte)'M' || after[1] != (byte)'Z')
                throw new InvalidDataException("Patched V5 image is not a DOS executable.");
            for (int i = RunVgaBootstrapService.V5FontOffset; i < RunVgaBootstrapService.V5FontOffset + RunVgaBootstrapService.V5FontSize; i++)
                if (before[i] != after[i])
                    throw new InvalidDataException("VGA appended materialization must not modify font bytes.");
            int declared = U16(after, 2) == 0 ? U16(after, 4) * 512 : (U16(after, 4) - 1) * 512 + U16(after, 2);
            if (declared != after.Length)
                throw new InvalidDataException("VGA appended output has an invalid MZ declared size.");
        }
    }
}
