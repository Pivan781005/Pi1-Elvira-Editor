using System.Text;

namespace ElviraVgaEditor;

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
    InvalidFrozenDescriptor
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
/// This class never scans executable text, creates a build, writes project state,
/// or turns unknown RUNIT routes into known mappings.
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
