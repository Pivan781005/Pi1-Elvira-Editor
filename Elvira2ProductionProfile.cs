namespace ElviraVgaEditor;

/// <summary>
/// Frozen, evidence-backed Elvira II production facts.  This is metadata and
/// invariant validation only; it does not deploy, compose, or localize a game.
/// Those storage/orchestration concerns remain explicitly deferred to R6.
/// </summary>
internal static class Elvira2ProductionProfile
{
    internal const string ActiveGamePc = "GAMEPC";
    internal const string ImmutableGamePc = "GAMEPCO";
    internal const string GeneratedSlovakGamePc = "GAMEPCSK";
    internal const int OriginalGamePcSize = 125702;
    internal const string OriginalGamePcSha256 = "484C6C25E4E883E31FC129491D7681BB2330C29C63F10CCBA15EA9F7A4E3BA78";

    // Elvira II has one supported VGA executable model: RUNIT only.
    internal const string ActiveExecutable = "RUNIT.EXE";
    internal const string ImmutableExecutable = "RUNITO.EXE";
    internal const string GeneratedSlovakExecutable = "RUNITSK.EXE";

    internal static readonly ExecutableFingerprint PackedOriginal = new(RunItBootstrapService.PackedSize, RunItBootstrapService.PackedSha256);
    internal static readonly ExecutableFingerprint CanonicalUnpacked = new(RunItBootstrapService.CanonicalSize, RunItBootstrapService.CanonicalSha256);
    internal static readonly ExecutableFingerprint DeterministicFontEnabled = new(RunItBootstrapService.ExtendedSize,
        "BAB9D7DDE86ABD42C3A728952E736A4C7541CF86F34C3301D2FFCECC905B46BD");

    internal static readonly FrozenGamePcDescriptor GamePc = new(
        ActiveGamePc, ImmutableGamePc, GeneratedSlovakGamePc, OriginalGamePcSha256);

    internal const string DeploymentContract =
        "GamePatchDeploymentService validates a temporary V2 image, preserves RUNITO.EXE once, replaces only RUNIT.EXE, and never writes GAMEPC.";
    internal const string DiscoveryContract =
        "Installation discovery requires GAMEPC, a VGA zone, and RUNIT in OriginalPacked, CanonicalUnpackedAscii98, or ExtendedCp852 state; no Elvira II EGA executable is modeled.";
    internal const string VariantContract =
        "VariantNaming and TranslationVariantService bind GAMEPCSK to RUNITSK.EXE and create from immutable GAMEPCO/RUNITO sources without overwriting destinations.";

    internal static readonly FrozenFontDescriptor LowFont = new(
        RunItBootstrapService.OriginalFontOffset, 0x154CA, RunItBootstrapService.OriginalFontGlyphCount * RunVgaFontService.GlyphBytes,
        RelativeSegment: null, HudBytes: FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes);

    internal static readonly FrozenFontDescriptor HighFont = new(
        RunItBootstrapService.HighFontOffset, 0x27080, RunItBootstrapService.HighFontSize,
        RelativeSegment: 0x2708);

    internal static readonly FrozenFontDescriptor Helper = new(
        RunItBootstrapService.HelperOffset, 0x27470, RunItBootstrapService.HelperSize,
        RelativeSegment: 0x2708);

    // Metadata is frozen; this is not a production RuntimeUiTextService.
    internal static readonly FrozenRuntimeUiDescriptor RuntimeUi = new(true,
        "RUNIT error-wrapper routing and persistent-bank evidence are frozen; Pause/Quit and GAMEPC UI follow-ups remain deferred.",
        0x34B00, 0x352FF, 0x35F00, 0x366FF, 0x34B0,
        Records:
        [
            new("SAVE_FAILURE", 0xF100, 0, 21, 0x8DC9, 0xBF, 0x2E47, 0x2E5C, FrozenRuntimeUiDispatch.FirstSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("LOAD_FAILURE", 0xF101, 0, 21, 0x8DE0, 0xBF, 0x2E5C, 0x2E72, FrozenRuntimeUiDispatch.FirstSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("FILE_NOT_FOUND", 0xF102, 0, 22, 0x8DE3, 0xBD, 0x2E72, 0x2E88, FrozenRuntimeUiDispatch.SecondSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("TRY_ANOTHER_DISK", 0xF103, 0, 24, 0x8DCC, 0xBD, 0x2E88, 0x2EA0, FrozenRuntimeUiDispatch.SecondSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary)
        ],
        RejectedRanges: [new(0x27B90, 0x2838F, "RUNIT_RUNTIME_UI_CANDIDATE_A_27B90_REJECTED_STARTUP_WRITTEN")]);

    internal static readonly IReadOnlyList<string> DeferredR6Items =
    [
        "Storage model and immutable-original vault refactor.",
        "ProjectContext, VariantContext, variant directories, and global active variant.",
        "Disposable composite builds and CompositeBuildService.",
        "RuntimeUiTextService integration and general launcher refactor.",
        "UI localization and Danger Zone integration."
    ];

    internal static void VerifyFrozenInvariants()
    {
        foreach (string name in new[] { ActiveGamePc, ImmutableGamePc, GeneratedSlovakGamePc, ActiveExecutable, ImmutableExecutable, GeneratedSlovakExecutable })
            if (!GameDataFileService.IsDos83FileName(name))
                throw new InvalidDataException($"Frozen Elvira II name is not DOS 8.3: {name}");

        if (!PackedOriginal.Matches(RunItBootstrapService.PackedSize, RunItBootstrapService.PackedSha256) ||
            !CanonicalUnpacked.Matches(RunItBootstrapService.CanonicalSize, RunItBootstrapService.CanonicalSha256) ||
            DeterministicFontEnabled.Size != RunItBootstrapService.ExtendedSize)
            throw new InvalidDataException("RUNIT frozen fingerprints diverge from the verified bootstrap contract.");
        if (OriginalGamePcSize != 125702 || GamePc.OriginalSha256 != OriginalGamePcSha256 ||
            GamePc.ActiveFileName != ActiveGamePc || GamePc.ImmutableOriginalFileName != ImmutableGamePc || GamePc.GeneratedSlovakFileName != GeneratedSlovakGamePc)
            throw new InvalidDataException("GAMEPC frozen production metadata is invalid.");

        if (LowFont.PhysicalFileOffset != 0x168CA || LowFont.ByteLength != 0x310 ||
            HighFont.PhysicalFileOffset != 0x28480 || HighFont.ByteLength != 0x3F0 ||
            Helper.PhysicalFileOffset != 0x28870 || Helper.ByteLength != 81 ||
            HighFont.PhysicalFileOffset + HighFont.ByteLength != Helper.PhysicalFileOffset ||
            Helper.PhysicalFileOffset + Helper.ByteLength != RunItBootstrapService.ExtendedSize)
            throw new InvalidDataException("RUNIT split-font geometry is invalid.");
        if (LowFont.HudBytes is null || !LowFont.HudBytes.SequenceEqual(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes) ||
            !FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes.SequenceEqual(Convert.FromHexString("FCFCFCFCFCFCFCFC")))
            throw new InvalidDataException("RUNIT HUD erase glyph 0x81 is not frozen.");
        if (RunItBootstrapService.HighFirstByte != 0x82 || RunItBootstrapService.OriginalFontGlyphCount != 98 ||
            RunVgaFontService.GlyphBytes != 8)
            throw new InvalidDataException("RUNIT CP852 byte-slot geometry is invalid.");

        if (!RuntimeUi.IsPersistentBank(0x34B00, 0x352FF, 0x35F00, 0x366FF, 0x34B0) ||
            RuntimeUi.RejectedCandidateRanges.Count != 1 || RuntimeUi.RejectedCandidateRanges[0] is not
                { ModuleStart: 0x27B90, ModuleEndInclusive: 0x2838F, Classification: "RUNIT_RUNTIME_UI_CANDIDATE_A_27B90_REJECTED_STARTUP_WRITTEN" } ||
            RuntimeUi.RuntimeRecords.Count != 4 || RuntimeUi.RuntimeRecords.Any(record => record.Evidence != FrozenRuntimeEvidence.ProvenByBinary))
            throw new InvalidDataException("RUNIT runtime-UI evidence metadata is invalid.");
        if (RuntimeUi.ModuleStart <= Helper.ModuleOffset + Helper.ByteLength - 1 ||
            RuntimeUi.ModuleStart <= HighFont.ModuleOffset + HighFont.ByteLength - 1 ||
            RuntimeUi.ModuleStart <= LowFont.ModuleOffset + LowFont.ByteLength - 1)
            throw new InvalidDataException("RUNIT runtime-UI bank overlaps a frozen font region.");

        FrozenRuntimeUiRecord saveFailure = RuntimeUi.RuntimeRecords.Single(record => record.Name == "SAVE_FAILURE");
        if (saveFailure is not { PointerSitePhysicalOffset: 0x8DC9, PointerOpcode: 0xBF, OriginalPointer: 0x2E47,
            OriginalEnd: 0x2E5C, Dispatch: FrozenRuntimeUiDispatch.FirstSharedDispatch, Evidence: FrozenRuntimeEvidence.ProvenByBinary })
            throw new InvalidDataException("RUNIT SAVE_FAILURE binary route metadata is invalid.");

        // Profile metadata records the established constraints without claiming
        // that deferred GAMEPC resource-map work has been completed.
        if (RuntimeUi.Notes.Contains("Save/Restore frozen", StringComparison.OrdinalIgnoreCase) ||
            RuntimeUi.Notes.Contains("MP resolved", StringComparison.OrdinalIgnoreCase) || DeferredR6Items.Count == 0)
            throw new InvalidDataException("Elvira II deferred-production boundaries are invalid.");
        if (!DeploymentContract.Contains("RUNITO.EXE", StringComparison.Ordinal) ||
            !DiscoveryContract.Contains("RUNIT", StringComparison.Ordinal) || DiscoveryContract.Contains("RUNEGA", StringComparison.Ordinal) ||
            !VariantContract.Contains("GAMEPCSK", StringComparison.Ordinal) || !VariantContract.Contains("RUNITSK.EXE", StringComparison.Ordinal))
            throw new InvalidDataException("Elvira II deployment/discovery/variant metadata is invalid.");
    }
}
