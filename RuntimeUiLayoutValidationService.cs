using System.Text;

namespace Pi1ElviraEditor;

internal enum RuntimeUiLayoutValidationStatus
{
    Valid,
    DefaultTextUnavailable,
    MappingIncomplete,
    UnsupportedRecord,
    EncodingFailure,
    UnsupportedGlyph,
    TextTooLong,
    RecordCapacityExceeded,
    BankCapacityExceeded,
    InvalidFrozenDescriptor,
    FixedColumnViolation,
    HotspotViolation,
    VisualCollision,
    RowOverflow
}

internal sealed record RuntimeUiLayoutValidationResult(
    RuntimeUiLogicalRecordId LogicalRecordId,
    RuntimeUiLayoutValidationStatus Status,
    string Detail,
    byte[] EncodedPayload,
    int PayloadBytesIncludingTerminator,
    int? RecordCapacity,
    int? BankCapacity,
    RuntimeUiMappingReadiness MappingReadiness,
    RuntimeUiEvidenceStatus EvidenceStatus)
{
    public bool IsValid => Status == RuntimeUiLayoutValidationStatus.Valid;
}

internal sealed record RuntimeUiLayoutValidationBatchResult(
    VariantRuntimeKind RuntimeKind,
    IReadOnlyList<RuntimeUiLayoutValidationResult> Records)
{
    public bool IsProductionReady => Records.Count > 0 && Records.All(record => record.IsValid);
}

/// <summary>
/// Read-only production-layout validation for logical RuntimeUiTextService data.
/// R9F V5: the three proven-live variable-width records for Elvira I VGA
/// (Pause.menu, Confirm.generic, Save.overwrite) decode their frozen
/// originals from the game source (packed/unpacked/V5) and validate semantic
/// overrides against frozen anchors with variable label lengths. All other
/// records never scan executable text, create a build, write project state,
/// or turn unknown RUNIT routes into known mappings.
/// </summary>
internal sealed class RuntimeUiLayoutValidationService
{
    private readonly RuntimeUiTextService _texts;

    public RuntimeUiLayoutValidationService(RuntimeUiTextService? texts = null) => _texts = texts ?? new RuntimeUiTextService();

    public RuntimeUiLayoutValidationResult Validate(VariantContext variant, RuntimeUiTextState state, RuntimeUiLogicalRecordId logicalRecordId)
    {
        if (variant is null) throw new ArgumentNullException(nameof(variant));
        RuntimeUiRuntimeProjection? projection = _texts.GetEffectiveRecords(variant, state).SingleOrDefault(value => value.LogicalRecordId == logicalRecordId);
        if (projection is null)
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedRecord, UiText.Get("RuntimeUi.Detail.UnsupportedRecord"), [], 0, null, null, RuntimeUiMappingReadiness.Unsupported, RuntimeUiEvidenceStatus.Unknown);
        // R9F V5: the three human-proven-live RUNVGA structured records
        // (Pause.menu, Confirm.generic, Save.overwrite) validate full
        // semantic records with frozen anchors and variable label lengths;
        // storage over the English envelope routes to appended records.
        if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Vga && RunVgaVariableUiService.IsVariableRecord(logicalRecordId))
            return ValidateRunVgaVariable(variant, projection);
        // R9F RUNEGA: frozen per-record spans with audited edit contracts.
        // Capacity fit alone never implies editability: stored values must
        // satisfy the frozen structure (prefix/separators/buttons), otherwise
        // they fail with a clear hotspot error instead of a misleading Valid.
        if (variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega)
            return ValidateRunEga(variant, projection);
        if (projection.MappingReadiness == RuntimeUiMappingReadiness.KnownButMappingIncomplete)
        {
            // Route evidence and layout safety are separate: proven routes report
            // their evidence while staying blocked; unknown routes stay generic.
            // Neither branch manufactures a capacity or makes anything editable.
            string incompleteDetail = projection.EvidenceStatus is RuntimeUiEvidenceStatus.Proven or RuntimeUiEvidenceStatus.ProvenByBinary or RuntimeUiEvidenceStatus.ProvenLive or RuntimeUiEvidenceStatus.StrongEvidence
                ? UiText.Get("RuntimeUi.Detail.RouteProvenLayoutIncomplete")
                : UiText.Get("RuntimeUi.Detail.MappingIncomplete");
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.MappingIncomplete, incompleteDetail, [], 0, null, BankCapacity(variant), projection.MappingReadiness, projection.EvidenceStatus);
        }
        if (projection.EffectiveText is null)
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.DefaultTextUnavailable, UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"), [], 0, RecordCapacity(variant, logicalRecordId), BankCapacity(variant), projection.MappingReadiness, projection.EvidenceStatus);
        if (!TryGetFrozenRecord(variant, logicalRecordId, out FrozenRuntimeUiRecord? frozen, out string? descriptorError))
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.InvalidFrozenDescriptor, descriptorError!, [], 0, null, BankCapacity(variant), projection.MappingReadiness, projection.EvidenceStatus);
        FrozenRuntimeUiRecord frozenRecord = frozen!;

        if (!TryEncodeStrict(projection.EffectiveText, out byte[] payload, out string? encodingError))
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.EncodingFailure, encodingError!, [], 0, frozenRecord.Length, BankCapacity(variant), projection.MappingReadiness, projection.EvidenceStatus);
        int control = Array.FindIndex(payload, value => value != 0x0D && (value < 0x20 || value == 0x7F));
        if (control >= 0)
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), payload[control]), payload, payload.Length + 1, frozenRecord.Length, BankCapacity(variant), projection.MappingReadiness, projection.EvidenceStatus);
        int reserved = Array.IndexOf(payload, (byte)FontSlotMetadata.HudEraseGlyph);
        if (reserved >= 0)
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"), payload, payload.Length + 1, frozenRecord.Length, BankCapacity(variant), projection.MappingReadiness, projection.EvidenceStatus);

        int required = checked(payload.Length + 1); // NUL is mandatory in every frozen descriptor span.
        if (required > frozenRecord.Length)
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.RecordCapacityExceeded, string.Format(UiText.Get("RuntimeUi.Detail.RecordCapacityExceeded"), required, frozenRecord.Length), payload, required, frozenRecord.Length, BankCapacity(variant), projection.MappingReadiness, projection.EvidenceStatus);
        if (!FitsFrozenBank(variant, frozenRecord, out int bankCapacity, out string? bankError))
            return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.BankCapacityExceeded, bankError!, payload, required, frozenRecord.Length, bankCapacity, projection.MappingReadiness, projection.EvidenceStatus);
        return Result(logicalRecordId, RuntimeUiLayoutValidationStatus.Valid, UiText.Get("RuntimeUi.Detail.Valid"), payload, required, frozenRecord.Length, bankCapacity, projection.MappingReadiness, projection.EvidenceStatus);
    }

    public RuntimeUiLayoutValidationBatchResult ValidateAll(VariantContext variant, RuntimeUiTextState state) =>
        new(variant.RuntimeKind, _texts.GetEffectiveRecords(variant, state).Select(record => Validate(variant, state, record.LogicalRecordId)).ToArray());

    private static RuntimeUiLayoutValidationResult ValidateRunEga(VariantContext variant, RuntimeUiRuntimeProjection projection)
    {
        RunEgaUiFieldContract contract = RunEgaUiService.Contract(projection.LogicalRecordId);
        int? bank = BankCapacity(variant);
        string? text = projection.EffectiveText;
        if (text is null)
        {
            if (!RunEgaUiService.TryDecodeOriginalFromGameRoot(variant.Project.GameRoot, projection.LogicalRecordId, out string? decoded, out _))
                return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.DefaultTextUnavailable, UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"), [], 0, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus);
            text = decoded!;
        }
        if (!TryEncodeStrict(text, out byte[] payload, out string? encodingError))
            return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.EncodingFailure, encodingError!, [], 0, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus);
        int control = Array.FindIndex(payload, value => value != 0x0D && (value < 0x20 || value == 0x7F));
        if (control >= 0)
            return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), payload[control]), payload, payload.Length + 1, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus);
        if (Array.IndexOf(payload, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
            return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"), payload, payload.Length + 1, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus);
        if (!RunEgaUiService.TryValidateStored(text, projection.LogicalRecordId, out RunEgaUiFailureKind failure, out string detail))
        {
            return failure switch
            {
                RunEgaUiFailureKind.TooLong => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.RecordCapacityExceeded, detail, payload, payload.Length + 1, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus),
                RunEgaUiFailureKind.FixedColumn => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.FixedColumnViolation, detail, payload, payload.Length + 1, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus),
                RunEgaUiFailureKind.Hotspot => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.HotspotViolation, detail, payload, payload.Length + 1, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus),
                RunEgaUiFailureKind.Newline => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, detail, payload, payload.Length + 1, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus),
                _ => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.EncodingFailure, detail, [], 0, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus),
            };
        }
        int required = checked(payload.Length + 1);
        return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.Valid, UiText.Get("RuntimeUi.Detail.Valid"), payload, required, contract.Length, bank, projection.MappingReadiness, projection.EvidenceStatus);
    }

    /// <summary>Validation for the three proven-live variable-width VGA
    /// records: full semantic structure with frozen anchors, variable
    /// label lengths, one-line rows. Storage beyond the English envelope is
    /// a materialization concern (appended record), never a validation
    /// failure here: Valid describes geometry + encoding only.</summary>
    private static RuntimeUiLayoutValidationResult ValidateRunVgaVariable(VariantContext variant, RuntimeUiRuntimeProjection projection)
    {
        int capacity = RunVgaVariableUiService.SpanLength(projection.LogicalRecordId);
        string? text = projection.EffectiveText;
        if (text is null)
        {
            if (!RunVgaVariableUiService.TryDecodeOriginalFromGameRoot(variant.Project.GameRoot, projection.LogicalRecordId, out string? decoded, out _))
                return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.DefaultTextUnavailable, UiText.Get("RuntimeUi.Detail.DefaultTextUnavailable"), [], 0, capacity, null, projection.MappingReadiness, projection.EvidenceStatus);
            text = decoded!;
        }
        if (!TryEncodeStrict(text, out byte[] payload, out string? encodingError))
            return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.EncodingFailure, encodingError!, [], 0, capacity, null, projection.MappingReadiness, projection.EvidenceStatus);
        int control = Array.FindIndex(payload, value => value != 0x0D && (value < 0x20 || value == 0x7F));
        if (control >= 0)
            return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, string.Format(UiText.Get("RuntimeUi.Detail.ControlByte"), payload[control]), payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus);
        if (Array.IndexOf(payload, (byte)FontSlotMetadata.HudEraseGlyph) >= 0)
            return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, UiText.Get("RuntimeUi.Detail.ReservedHudGlyph"), payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus);
        if (!RunVgaVariableUiService.TryValidateStored(text, projection.LogicalRecordId, out RunVgaVariableFailureKind failure, out string detail))
        {
            return failure switch
            {
                RunVgaVariableFailureKind.FixedColumn => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.FixedColumnViolation, detail, payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus),
                RunVgaVariableFailureKind.Hotspot => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.HotspotViolation, detail, payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus),
                RunVgaVariableFailureKind.Newline => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.UnsupportedGlyph, detail, payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus),
                RunVgaVariableFailureKind.VisualCollision => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.VisualCollision, detail, payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus),
                RunVgaVariableFailureKind.RowOverflow => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.RowOverflow, detail, payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus),
                RunVgaVariableFailureKind.TooLong => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.RecordCapacityExceeded, detail, payload, payload.Length + 1, capacity, null, projection.MappingReadiness, projection.EvidenceStatus),
                _ => Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.EncodingFailure, detail, [], 0, capacity, null, projection.MappingReadiness, projection.EvidenceStatus),
            };
        }
        int required = checked(payload.Length + 1);
        return Result(projection.LogicalRecordId, RuntimeUiLayoutValidationStatus.Valid, UiText.Get("RuntimeUi.Detail.Valid"), payload, required, capacity, null, projection.MappingReadiness, projection.EvidenceStatus);
    }

    private static bool TryEncodeStrict(string text, out byte[] bytes, out string? error)
    {
        bytes = []; error = null;
        if (text.IndexOf('\0') >= 0) { error = UiText.Get("RuntimeUi.Detail.EmbeddedNul"); return false; }
        try
        {
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            bytes = strict.GetBytes(text);
            if (!string.Equals(text, strict.GetString(bytes), StringComparison.Ordinal)) { error = UiText.Get("RuntimeUi.Detail.Cp852RoundTrip"); return false; }
            return true;
        }
        catch (EncoderFallbackException) { error = UiText.Get("RuntimeUi.Detail.Cp852Unrepresentable"); return false; }
    }

    private static bool TryGetFrozenRecord(VariantContext variant, RuntimeUiLogicalRecordId id, out FrozenRuntimeUiRecord? record, out string? error)
    {
        record = null; error = null;
        FrozenRuntimeUiDescriptor descriptor = variant.RuntimeKind switch
        {
            VariantRuntimeKind.Elvira1Ega => Elvira1ProductionProfile.RunEga.RuntimeUi,
            VariantRuntimeKind.Elvira2Vga => Elvira2ProductionProfile.RuntimeUi,
            // RUNVGA's frozen descriptor intentionally has no per-record spans.
            // It is not safe to borrow EGA capacities merely because the text is shared.
            VariantRuntimeKind.Elvira1Vga => Elvira1ProductionProfile.RunVga.RuntimeUi,
            _ => throw new ArgumentOutOfRangeException(nameof(variant))
        };
        if (!descriptor.IsFrozen) { error = UiText.Get("RuntimeUi.Detail.DescriptorNotFrozen"); return false; }
        string? name = (variant.RuntimeKind, id) switch
        {
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.PauseMenu) => "Pause.menu",
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.ConfirmGeneric) => "Confirm.generic",
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SavePrompt) => "Save.prompt",
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure) => "Save.failed",
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.LoadFailure) => "Restore.loadFailed",
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.FileNotFound) => "Restore.fileNotFound",
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.TryAnotherDisk) => "Disk.retry",
            (VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveOverwrite) => "Save.overwrite",
            (VariantRuntimeKind.Elvira2Vga, RuntimeUiLogicalRecordId.SaveFailure) => "SAVE_FAILURE",
            _ => null
        };
        if (name is null) { error = UiText.Get("RuntimeUi.Detail.NoRecordLayout"); return false; }
        record = descriptor.RuntimeRecords.SingleOrDefault(value => value.Name.Equals(name, StringComparison.Ordinal));
        if (record is null) { error = UiText.Get("RuntimeUi.Detail.MissingRecord"); return false; }
        return true;
    }

    private static int? RecordCapacity(VariantContext variant, RuntimeUiLogicalRecordId id) =>
        TryGetFrozenRecord(variant, id, out FrozenRuntimeUiRecord? record, out _) ? record!.Length : null;

    private static int? BankCapacity(VariantContext variant) => variant.RuntimeKind switch
    {
        VariantRuntimeKind.Elvira1Ega => Elvira1ProductionProfile.RunEga.RuntimeUi.ModuleEndInclusive - Elvira1ProductionProfile.RunEga.RuntimeUi.ModuleStart + 1,
        VariantRuntimeKind.Elvira2Vga => Elvira2ProductionProfile.RuntimeUi.ModuleEndInclusive - Elvira2ProductionProfile.RuntimeUi.ModuleStart + 1,
        _ => null
    };

    private static bool FitsFrozenBank(VariantContext variant, FrozenRuntimeUiRecord record, out int capacity, out string? error)
    {
        capacity = BankCapacity(variant) ?? 0; error = null;
        if (capacity <= 0) { error = UiText.Get("RuntimeUi.Detail.BankCapacityUnavailable"); return false; }
        if (record.BankOffset < 0 || record.Length <= 0 || record.BankOffset + record.Length > capacity)
        { error = UiText.Get("RuntimeUi.Detail.RecordEscapesBank"); return false; }
        return true;
    }

    private static RuntimeUiLayoutValidationResult Result(RuntimeUiLogicalRecordId id, RuntimeUiLayoutValidationStatus status, string detail, byte[] payload, int required, int? recordCapacity, int? bankCapacity, RuntimeUiMappingReadiness readiness, RuntimeUiEvidenceStatus evidence) =>
        new(id, status, detail, payload.ToArray(), required, recordCapacity, bankCapacity, readiness, evidence);
}
