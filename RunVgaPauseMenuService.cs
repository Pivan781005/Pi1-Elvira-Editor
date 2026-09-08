using System.Text;

namespace Pi1ElviraEditor;

/// <summary>Pause.menu failure kind for exact validation. Distinct fixed-column
/// vs hotspot kinds keep tests F and G deterministic.</summary>
internal enum RunVgaPauseFailureKind
{
    None,
    Encoding,
    TooLong,
    Newline,
    FixedColumn,
    Hotspot
}

/// <summary>
/// Frozen Pause.menu physical storage and layout model for Elvira I RUNVGA.
/// Proven by binary against the canonical unpacked baseline (SHA-256
/// C6CEC09B...9686756): file offset 0x1A625 holds 38 CP852 bytes plus a
/// mandatory NUL (39 bytes total, 0x1A625..0x1A64B inclusive). The next
/// logical record (Confirm.generic) starts immediately at 0x1A64C (gap 0),
/// so in-place capacity is exactly 38 chars + NUL; shorter overrides are
/// zero-padded to 39. Runtime code at ~0xCCF4 issues MOV DI,0x2CB5
/// (DS base 0x1557-relocated: 0x2400 + 0x15570 + 0x2CB5 = 0x1A625) then a
/// far renderer call at ~0xCCF7. No RUNEGA bytes are borrowed and no generic
/// bank is manufactured. Phase 1 is in-place only: no pointer/code patching,
/// no relocation.
/// </summary>
internal static class RunVgaPauseMenuService
{
    internal const int PauseFileOffset = 0x1A625;
    internal const int PauseSpanLengthIncludingNul = 39;
    internal const int PauseMaxCharsExcludingNul = 38;
    internal const int PauseRuntimeSourceAddress = 0x2CB5;
    internal const int PauseRendererApprox = 0xCCF7;
    internal const string PauseHotspotLine = " Continue      Quit";
    internal const string PauseLeadingSpaces = "     ";
    internal const string PauseSeparators = "\r\r\r";
    /// <summary>Phase 1 title field: at most 11 CP852 bytes (one byte per
    /// representable character). The frozen 5-space indent, separators and
    /// hotspot line fill the remaining 27 bytes of the 38-char record.</summary>
    internal const int PauseMaxTitleBytes = 11;

    internal static readonly byte[] PauseOriginalSpan = Convert.FromHexString(
        "202020202047616D65205061757365640D0D0D20436F6E74696E75652020202020205175697400");

    internal static string PauseOriginalText =>
        GamePcTextEditor.GetEncoding("CP852").GetString(PauseOriginalSpan[..^1]);

    internal static bool TryDecodePause(byte[] image, out string? text, out string? error)
    {
        text = null; error = null;
        if (image is null || image.Length < PauseFileOffset + PauseSpanLengthIncludingNul)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        if (image[0] != (byte)'M' || image[1] != (byte)'Z')
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        byte[] span = image.AsSpan(PauseFileOffset, PauseSpanLengthIncludingNul).ToArray();
        int nul = Array.IndexOf(span, (byte)0x00);
        if (nul < 0)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        // A single NUL terminates the record; any bytes after it within the
        // span must be zero padding (Phase 1 in-place model). Stale nonzero
        // bytes after NUL indicate a malformed span.
        for (int i = nul + 1; i < span.Length; i++)
            if (span[i] != 0x00)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
        byte[] payload = span[..nul];
        if (payload.Any(b => b == 0x81))
        { error = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
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

    internal static bool TryDecodeFromFile(string path, out string? text, out string? error)
    {
        text = null; error = null;
        try
        {
            if (!File.Exists(path))
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
            byte[] data = File.ReadAllBytes(path);
            RunVgaBootstrapState state = RunVgaBootstrapService.DetectState(path);
            byte[] image = state switch
            {
                RunVgaBootstrapState.UnpackedBaseline => data,
                RunVgaBootstrapState.ExtendedCp852V5 => data,
                RunVgaBootstrapState.OriginalPacked => RunVgaBootstrapService.UnpackVerifiedOriginal(data),
                _ => []
            };
            if (image.Length == 0)
            { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
            return TryDecodePause(image, out text, out error);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
    }

    internal static bool TryDecodeFromGameRoot(string gameRoot, out string? text, out string? error)
    {
        text = null; error = null;
        try
        {
            string candidate = Path.Combine(gameRoot, Elvira1ProductionProfile.ActiveVgaExecutable);
            return TryDecodeFromFile(candidate, out text, out error);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        { error = UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"); return false; }
    }

    internal static bool TryEncodePauseOverride(string overrideText, out byte[] span39, out RunVgaPauseFailureKind failure, out string detail)
    {
        span39 = []; failure = RunVgaPauseFailureKind.None; detail = string.Empty;
        if (overrideText is null)
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        if (overrideText.IndexOf('\0') >= 0)
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        if (overrideText.IndexOf('\n') >= 0)
        { failure = RunVgaPauseFailureKind.Newline; detail = UiText.Get("RuntimeUi.Detail.PauseNewline"); return false; }
        byte[] payload;
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            payload = strict.GetBytes(overrideText);
            if (!string.Equals(overrideText, strict.GetString(payload), StringComparison.Ordinal))
            { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        int control = Array.FindIndex(payload, value => value != 0x0D && (value < 0x20 || value == 0x7F));
        if (control >= 0)
        { failure = RunVgaPauseFailureKind.Newline; detail = string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), payload[control]); return false; }
        if (Array.IndexOf(payload, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        int required = checked(payload.Length + 1);
        if (required > PauseSpanLengthIncludingNul)
        { failure = RunVgaPauseFailureKind.TooLong; detail = string.Format(UiText.Get("RuntimeUi.Detail.RecordCapacityExceeded"), required, PauseSpanLengthIncludingNul); return false; }
        // Separator model: exactly three contiguous 0x0D, no other 0x0D.
        int crCount = payload.Count(b => b == 0x0D);
        byte[] triple = [(byte)0x0D, (byte)0x0D, (byte)0x0D];
        if (crCount != 3 || IndexOf(payload, triple) < 0)
        { failure = RunVgaPauseFailureKind.Hotspot; detail = UiText.Get("RuntimeUi.Detail.PauseHotspot"); return false; }
        string decoded = GamePcTextEditor.GetEncoding("CP852").GetString(payload);
        int sep = decoded.IndexOf(PauseSeparators, StringComparison.Ordinal);
        if (sep < 0)
        { failure = RunVgaPauseFailureKind.Hotspot; detail = UiText.Get("RuntimeUi.Detail.PauseHotspot"); return false; }
        string title = decoded[..sep];
        string hotspot = decoded[(sep + PauseSeparators.Length)..];
        if (!hotspot.Equals(PauseHotspotLine, StringComparison.Ordinal))
        {
            // Distinguish column shifts (same keywords, moved) from keyword damage.
            bool hasBoth = hotspot.Contains("Continue", StringComparison.Ordinal) && hotspot.Contains("Quit", StringComparison.Ordinal);
            failure = hasBoth ? RunVgaPauseFailureKind.FixedColumn : RunVgaPauseFailureKind.Hotspot;
            detail = hasBoth ? UiText.Get("RuntimeUi.Detail.PauseFixedColumn") : UiText.Get("RuntimeUi.Detail.PauseHotspot");
            return false;
        }
        if (!title.StartsWith(PauseLeadingSpaces, StringComparison.Ordinal) || title.Length < PauseLeadingSpaces.Length + 1)
        { failure = RunVgaPauseFailureKind.FixedColumn; detail = UiText.Get("RuntimeUi.Detail.PauseFixedColumn"); return false; }
        span39 = new byte[PauseSpanLengthIncludingNul];
        Array.Copy(payload, span39, payload.Length);
        span39[payload.Length] = 0x00;
        return true;
    }

    internal static void ApplyToImage(byte[] v5image, string overrideText)
    {
        if (v5image is null) throw new ArgumentNullException(nameof(v5image));
        if (v5image.Length != RunVgaBootstrapService.V5Size)
            throw new InvalidDataException("Pause.menu materialization requires the frozen V5 executable image.");
        if (!TryEncodePauseOverride(overrideText, out byte[] span, out _, out string detail))
            throw new InvalidDataException("Invalid Pause.menu override: " + detail);
        Array.Copy(span, 0, v5image, PauseFileOffset, PauseSpanLengthIncludingNul);
    }

    /// <summary>
    /// Title-only UX: extracts the semantic title from a persisted full
    /// record for grid display. Parse-only (no re-encoding); returns false
    /// for legacy unstructured values, which the grid then shows raw so no
    /// stored override is ever hidden.
    /// </summary>
    internal static bool TryExtractTitle(string? fullRecord, out string? title)
    {
        title = null;
        if (string.IsNullOrEmpty(fullRecord)) return false;
        int sep = fullRecord.IndexOf(PauseSeparators, StringComparison.Ordinal);
        if (sep < 0 || fullRecord.IndexOf(PauseSeparators, sep + PauseSeparators.Length, StringComparison.Ordinal) >= 0)
            return false;
        if (fullRecord.Count(c => c == '\r') != PauseSeparators.Length || fullRecord.Contains('\n'))
            return false;
        string head = fullRecord[..sep];
        string hotspot = fullRecord[(sep + PauseSeparators.Length)..];
        if (!hotspot.Equals(PauseHotspotLine, StringComparison.Ordinal)) return false;
        if (!head.StartsWith(PauseLeadingSpaces, StringComparison.Ordinal) || head.Length < PauseLeadingSpaces.Length + 1)
            return false;
        title = head[PauseLeadingSpaces.Length..];
        return true;
    }

    /// <summary>
    /// Title-only UX: validates a user-typed title and composes the frozen
    /// full record ("     " + title + separators + hotspot) for project
    /// persistence and materialization. The binary writer and span are
    /// untouched: the composed record re-enters TryEncodePauseOverride as a
    /// defensive safety net. Failure details are localized for status display.
    /// </summary>
    internal static bool TryComposeFromTitle(string? title, out string fullRecord, out RunVgaPauseFailureKind failure, out string detail)
    {
        fullRecord = string.Empty; failure = RunVgaPauseFailureKind.None; detail = string.Empty;
        if (string.IsNullOrEmpty(title))
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.PauseTitleEmpty"); return false; }
        if (title.IndexOf('\0') >= 0)
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        if (title.IndexOf('\r') >= 0 || title.IndexOf('\n') >= 0)
        { failure = RunVgaPauseFailureKind.Newline; detail = UiText.Get("RuntimeUi.Detail.PauseNewline"); return false; }
        byte[] encoded;
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            encoded = strict.GetBytes(title);
            if (!string.Equals(title, strict.GetString(encoded), StringComparison.Ordinal))
            { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
        }
        catch (EncoderFallbackException)
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
        int control = Array.FindIndex(encoded, value => value < 0x20 || value == 0x7F);
        if (control >= 0)
        { failure = RunVgaPauseFailureKind.Newline; detail = string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), encoded[control]); return false; }
        if (Array.IndexOf(encoded, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
        { failure = RunVgaPauseFailureKind.Encoding; detail = UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"); return false; }
        if (encoded.Length > PauseMaxTitleBytes)
        { failure = RunVgaPauseFailureKind.TooLong; detail = string.Format(UiText.Get("RuntimeUi.Detail.PauseTitleTooLong"), encoded.Length); return false; }
        string composed = PauseLeadingSpaces + title + PauseSeparators + PauseHotspotLine;
        if (!TryEncodePauseOverride(composed, out _, out RunVgaPauseFailureKind innerFailure, out string innerDetail))
        { failure = innerFailure; detail = innerDetail; return false; }
        fullRecord = composed;
        return true;
    }

    internal static void VerifyOnlyPauseBytesChanged(byte[] before, byte[] after)
    {
        if (before is null || after is null) throw new ArgumentNullException(before is null ? nameof(before) : nameof(after));
        if (before.Length != after.Length)
            throw new InvalidDataException("Pause.menu materialization must not change executable size.");
        if (before.Length != RunVgaBootstrapService.V5Size)
            throw new InvalidDataException("Pause.menu materialization requires the frozen V5 executable image.");
        for (int i = 0; i < before.Length; i++)
        {
            bool inSpan = i >= PauseFileOffset && i < PauseFileOffset + PauseSpanLengthIncludingNul;
            if (!inSpan && before[i] != after[i])
                throw new InvalidDataException($"Pause.menu materialization touched byte 0x{i:X} outside the authorized span.");
        }
        // A valid override may be byte-identical to the current image (no-op):
        // zero or more in-span differences are allowed. Only outside-span
        // differences fail.
        // V5 structural markers outside the span must remain intact.
        if (after[0] != (byte)'M' || after[1] != (byte)'Z')
            throw new InvalidDataException("Patched V5 image is not a DOS executable.");
        // Font region must be untouched by a Pause patch.
        for (int i = RunVgaBootstrapService.V5FontOffset; i < RunVgaBootstrapService.V5FontOffset + RunVgaBootstrapService.V5FontSize; i++)
            if (before[i] != after[i])
                throw new InvalidDataException("Pause.menu materialization must not modify font bytes.");
    }

    private static int IndexOf(byte[] haystack, byte[] needle)
    {
        for (int i = 0; i <= haystack.Length - needle.Length; i++)
            if (haystack.AsSpan(i, needle.Length).SequenceEqual(needle)) return i;
        return -1;
    }
}
