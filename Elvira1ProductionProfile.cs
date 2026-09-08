namespace Pi1ElviraEditor;

/// <summary>
/// Frozen, evidence-backed Elvira I production facts. This is metadata only:
/// it deliberately does not deploy, rename, or patch a game installation.
/// Storage/deployment orchestration remains future R6 work.
/// </summary>
internal static class Elvira1ProductionProfile
{
    internal const string ActiveGamePc = "GAMEPC";
    internal const string GeneratedSlovakGamePc = "GAMEPCSK";
    internal const int OriginalGamePcSize = 135332;

    internal const string ActiveVgaExecutable = "RUNVGA.EXE";
    internal const string GeneratedSlovakVgaExecutable = "RUNVGASK.EXE";

    internal const string ActiveEgaExecutable = "RUNEGA.EXE";
    internal const string GeneratedSlovakEgaExecutable = "RUNEGASK.EXE";

    internal const string LauncherFile = "ELVIRA.BAT";
    internal const string LauncherBackupFile = "ELVIRA.BAK";
    internal const string SoundStateFile = "PI1SND.BAT";
    internal const string MenuHelperFile = "PI1MENU.COM";
    internal const string VariantCatalogFile = "ELVIRA_MODS.INI";
    internal const string GogOverlayDirectory = "cloud_saves";
    internal const string GogOverlayBackupFile = "ELVOVR.BAK";

    internal static readonly FrozenExecutableDescriptor RunVga = new(
        Renderer: "VGA",
        ActiveFileName: ActiveVgaExecutable,
        GeneratedSlovakFileName: GeneratedSlovakVgaExecutable,
        PackedOriginal: new ExecutableFingerprint(0x18C39, RunVgaBootstrapService.PackedSha256),
        CanonicalUnpacked: new ExecutableFingerprint(0x2CE30, RunVgaBootstrapService.BaselineSha256),
        Font: new FrozenFontDescriptor(0x3AE60, 0x38A60, 0x800, RelativeSegment: null,
            RunVgaBootstrapService.V5FontOffset, HudBytes: FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes),
        RuntimeUi: FrozenRuntimeUiDescriptor.RunVgaProduction,
        InitialStack: null);

    internal static readonly FrozenExecutableDescriptor RunEga = new(
        Renderer: "EGA",
        ActiveFileName: ActiveEgaExecutable,
        GeneratedSlovakFileName: GeneratedSlovakEgaExecutable,
        PackedOriginal: new ExecutableFingerprint(0x198EF, "FE596D7DB1CEFB643F2C2BEC1EC5342CC1F7DBA2F47AFE3512B6C6FF8DE05EA1"),
        CanonicalUnpacked: new ExecutableFingerprint(0x25CD0, "A15243583A1774BAF8F77DF599063675ED9EF36A573A35CF6F2504705B3BAB56"),
        Font: new FrozenFontDescriptor(0x32400, 0x30000, 0x800, RelativeSegment: 0x3000,
            RendererSegmentOperandPhysicalOffset: 0xFA51, HudBytes: FontSlotMetadata.OriginalHudEraseGlyphBytes),
        RuntimeUi: FrozenRuntimeUiDescriptor.RuneGaProduction,
        InitialStack: new FrozenStackDescriptor(0x2502, 0x0A00));

    internal static readonly FrozenGamePcDescriptor GamePc = new(
        ActiveGamePc, GeneratedSlovakGamePc,
        "C0A1B2690499F51402605E5966593895CA9937C59F95C1A35BE026E57E30A663");

    internal static readonly FrozenLauncherCompanionDescriptor Launcher = new(
        LauncherFile, LauncherBackupFile, SoundStateFile, MenuHelperFile, VariantCatalogFile,
        GogOverlayDirectory, GogOverlayBackupFile, ModFiles: []);

    // Compatibility metadata only. This is deliberately not a UI warning or a deployment decision.
    internal static readonly FrozenCompatibilityNote RuneGaCompatibility = new(
        "DOSBox-X", "Some characters may render incorrectly in the current test environment.", "UNKNOWN",
        ["DOSBox Staging: PASS", "DOSBox 0.74-2: PASS", "RUNVGA/VGA in DOSBox-X: PASS"]);

    /// <summary>
    /// Frozen RUNVGA route evidence. Route addresses are approximate historical
    /// call sites (direct renderer calls and shared dispatchers). Pause.menu is
    /// PROVEN LIVE: relocation was tested live with "Game Paused - Relocation
    /// Test" visibly appearing in the running game, and Continue/Quit hotspot
    /// behavior was debugged live. All other listed routes are at minimum
    /// PROVEN BY BINARY. Layout flags are R4B constraints, not edit contracts.
    /// </summary>
    internal static readonly FrozenRunVgaRouteEvidence RunVgaRoutes = new(
        [
            new("Pause.menu", 0x1A625, 0x2CB5, RunVgaRouteSourceKind.DataAddress, FrozenRuntimeUiDispatch.Direct, 0xCCF7, FrozenRuntimeEvidence.ProvenLive, "FORMATTED|HOTSPOT_SENSITIVE|SINGLE_LINE_REQUIRED|FIXED_COLUMN_REQUIRED"),
            new("Confirm.generic", 0x1A64C, 0x2CDC, RunVgaRouteSourceKind.DataAddress, FrozenRuntimeUiDispatch.Direct, 0xCD29, FrozenRuntimeEvidence.ProvenByBinary, "FORMATTED|HOTSPOT_SENSITIVE|FIXED_COLUMN_REQUIRED"),
            new("Save.prompt", 0x1A674, 0x2D04, RunVgaRouteSourceKind.DataAddress, FrozenRuntimeUiDispatch.Direct, 0xCB1B, FrozenRuntimeEvidence.ProvenByBinary, "FORMATTED"),
            new("Save.failed", 0x1A6A8, 0x2D38, RunVgaRouteSourceKind.DataAddress, FrozenRuntimeUiDispatch.FirstSharedDispatch, 0xCC5A, FrozenRuntimeEvidence.ProvenByBinary, "FORMATTED"),
            new("Restore.loadFailed", 0x1A6BA, 0x2D4A, RunVgaRouteSourceKind.DataAddress, FrozenRuntimeUiDispatch.FirstSharedDispatch, 0xCC5A, FrozenRuntimeEvidence.ProvenByBinary, "FORMATTED"),
            new("Restore.fileNotFound", 0x1A6CC, 0x2D5C, RunVgaRouteSourceKind.BreakpointAddress, FrozenRuntimeUiDispatch.SecondSharedDispatch, 0xCC60, FrozenRuntimeEvidence.ProvenByBinary, "FORMATTED"),
            new("Disk.retry", 0x1A6E0, 0x2D70, RunVgaRouteSourceKind.BreakpointAddress, FrozenRuntimeUiDispatch.SecondSharedDispatch, 0xCC60, FrozenRuntimeEvidence.ProvenByBinary, "FORMATTED"),
            new("Save.overwrite", 0x1A6F6, 0x2D86, RunVgaRouteSourceKind.DataAddress, FrozenRuntimeUiDispatch.Direct, 0xCBFC, FrozenRuntimeEvidence.ProvenByBinary, "FORMATTED|HOTSPOT_SENSITIVE|FIXED_COLUMN_REQUIRED")
        ],
        [
            new("Save.failed", "Disk.retry"),
            new("Restore.loadFailed", "Restore.fileNotFound")
        ]);

    internal static FrozenRuntimeEvidence RunVgaRouteEvidence(RuntimeUiLogicalRecordId id) =>
        RunVgaRoutes.Route(id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => "Pause.menu",
            RuntimeUiLogicalRecordId.ConfirmGeneric => "Confirm.generic",
            RuntimeUiLogicalRecordId.SavePrompt => "Save.prompt",
            RuntimeUiLogicalRecordId.SaveFailure => "Save.failed",
            RuntimeUiLogicalRecordId.LoadFailure => "Restore.loadFailed",
            RuntimeUiLogicalRecordId.FileNotFound => "Restore.fileNotFound",
            RuntimeUiLogicalRecordId.TryAnotherDisk => "Disk.retry",
            RuntimeUiLogicalRecordId.SaveOverwrite => "Save.overwrite",
            _ => throw new ArgumentOutOfRangeException(nameof(id))
        }).Evidence;

    // Deferred R6 scope; intentionally descriptive, with no deployment implementation here.
    internal static readonly IReadOnlyList<string> DeferredR6Items =
    [
        "Separate EGA/VGA variant dimension.",
        "RUNEGA deployment integration.",
        "RUNEGA installation discovery integration.",
        "ProjectContext/VariantContext and global active-variant refactor.",
        "Variants directory and CompositeBuildService integration.",
        "Danger Zone and UI localization integration."
    ];

    internal static void VerifyFrozenInvariants()
    {
        foreach (string name in new[]
        {
            ActiveGamePc, GeneratedSlovakGamePc,
            ActiveVgaExecutable, GeneratedSlovakVgaExecutable,
            ActiveEgaExecutable, GeneratedSlovakEgaExecutable,
            LauncherFile, LauncherBackupFile, SoundStateFile, MenuHelperFile
        })
        {
            if (!GameDataFileService.IsDos83FileName(name))
                throw new InvalidDataException($"Frozen Elvira I name is not DOS 8.3: {name}");
        }

        if (OriginalGamePcSize != 135332 || GamePc.OriginalSha256 != "C0A1B2690499F51402605E5966593895CA9937C59F95C1A35BE026E57E30A663" ||
            !RunVga.PackedOriginal.Matches(RunVgaBootstrapService.PackedSize, RunVgaBootstrapService.PackedSha256) ||
            !RunVga.CanonicalUnpacked.Matches(RunVgaBootstrapService.BaselineSize, RunVgaBootstrapService.BaselineSha256))
            throw new InvalidDataException("RUNVGA frozen fingerprints diverge from the verified bootstrap descriptor.");
        if (!RunEga.Font.IsPersistentBank(0x30000, 0x307FF, 0x3000) ||
            RunEga.Font.HudBytes is null ||
            !RunEga.Font.HudBytes.SequenceEqual(FontSlotMetadata.OriginalHudEraseGlyphBytes))
            throw new InvalidDataException("RUNEGA frozen persistent font-bank metadata is invalid.");
        if (RunEga.InitialStack is not { Ss: 0x2502, Sp: 0x0A00 })
            throw new InvalidDataException("RUNEGA initial stack metadata is invalid.");
        VerifyRuneGaRuntimeUi(RunEga.RuntimeUi);
        VerifyRunVgaRoutes(RunVgaRoutes);
        if (RunEga.PackedOriginal.Matches(0x198F0, RunEga.PackedOriginal.Sha256) ||
            RunEga.CanonicalUnpacked.Matches(0x25CD1, RunEga.CanonicalUnpacked.Sha256))
            throw new InvalidDataException("RUNEGA fingerprint matching must reject incorrect file sizes.");
        if (Launcher.GogOverlayDirectory != GogOverlayDirectory || Launcher.GogOverlayBackupFile != GogOverlayBackupFile)
            throw new InvalidDataException("GOG overlay companion metadata is invalid.");
        if (RuneGaCompatibility is not { Environment: "DOSBox-X", Cause: "UNKNOWN" } ||
            RuneGaCompatibility.CrossCheckResults.Count != 3 ||
            !RuneGaCompatibility.CrossCheckResults.Contains("DOSBox Staging: PASS") ||
            !RuneGaCompatibility.CrossCheckResults.Contains("DOSBox 0.74-2: PASS") ||
            !RuneGaCompatibility.CrossCheckResults.Contains("RUNVGA/VGA in DOSBox-X: PASS"))
            throw new InvalidDataException("RUNEGA compatibility metadata is invalid.");
        if (Launcher.ModFiles.Count != 0)
            throw new InvalidDataException("No Elvira I .MOD dependency is proven in the current installation inventory.");
    }

    private static void VerifyRuneGaRuntimeUi(FrozenRuntimeUiDescriptor runtimeUi)
    {
        if (!runtimeUi.IsFrozen || !runtimeUi.IsPersistentBank(0x31000, 0x317FF, 0x33400, 0x33BFF, 0x3100) ||
            runtimeUi.BridgeOffset != 0x00D4 || runtimeUi.BridgeRelocationOffset != 0x00DB ||
            runtimeUi.RuntimeRecords.Count != 8 || runtimeUi.RejectedCandidateRanges.Count != 1 ||
            runtimeUi.RejectedCandidateRanges[0] is not { ModuleStart: 0x30800, ModuleEndInclusive: 0x30FFF, Classification: "REJECTED_RUNTIME_WRITTEN" })
            throw new InvalidDataException("RUNEGA frozen runtime UI-bank metadata is invalid.");
        if (runtimeUi.ModuleStart <= RunEga.Font.ModuleOffset + RunEga.Font.ByteLength - 1 ||
            runtimeUi.RejectedCandidateRanges[0].ModuleEndInclusive >= runtimeUi.ModuleStart)
            throw new InvalidDataException("RUNEGA UI bank overlaps a font bank or accepts the rejected candidate.");

        FrozenRuntimeUiRecord[] expected =
        [
            new("Pause.menu", 0xF000, 0x00DD, 42, 0xCDD8, 0xBF, 0x2D21, 0x2D48, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenLive),
            new("Confirm.generic", 0xF001, 0x0107, 39, 0xCE0A, 0xBF, 0x2D48, 0x2D70, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenByBinary),
            new("Save.prompt", 0xF002, 0x012E, 51, 0xCBFC, 0xBF, 0x2D70, 0x2DA3, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenLive),
            new("Save.failed", 0xF003, 0x0161, 18, 0xCD03, 0xBF, 0x2DA4, 0x2DB6, FrozenRuntimeUiDispatch.FirstSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Restore.loadFailed", 0xF004, 0x0173, 18, 0xCD1A, 0xBF, 0x2DB6, 0x2DC8, FrozenRuntimeUiDispatch.FirstSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Restore.fileNotFound", 0xF005, 0x0185, 19, 0xCD1D, 0xBD, 0x2DC8, 0x2DDB, FrozenRuntimeUiDispatch.SecondSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Disk.retry", 0xF006, 0x0198, 21, 0xCD06, 0xBD, 0x2DDC, 0x2DF1, FrozenRuntimeUiDispatch.SecondSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Save.overwrite", 0xF007, 0x01AD, 62, 0xCCDD, 0xBF, 0x2DF2, 0x2E30, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenByBinary)
        ];
        if (!runtimeUi.RuntimeRecords.SequenceEqual(expected))
            throw new InvalidDataException("RUNEGA runtime UI record metadata diverges from the frozen evidence map.");
        if (runtimeUi.RuntimeRecords.Select(record => record.Selector).Distinct().Count() != 8 ||
            !runtimeUi.RuntimeRecords.Select(record => record.Selector).Order().SequenceEqual(Enumerable.Range(0xF000, 8)))
            throw new InvalidDataException("RUNEGA runtime UI selectors must be unique and contiguous F000..F007.");
    }

    private static void VerifyRunVgaRoutes(FrozenRunVgaRouteEvidence routes)
    {
        string[] names = ["Pause.menu", "Confirm.generic", "Save.prompt", "Save.failed", "Restore.loadFailed", "Restore.fileNotFound", "Disk.retry", "Save.overwrite"];
        if (!routes.Routes.Select(route => route.Name).SequenceEqual(names, StringComparer.Ordinal))
            throw new InvalidDataException("RUNVGA frozen route evidence diverges from the proven record map.");
        if (routes.Route("Pause.menu").Evidence != FrozenRuntimeEvidence.ProvenLive ||
            routes.Routes.Where(route => !route.Name.Equals("Pause.menu", StringComparison.Ordinal)).Any(route => route.Evidence != FrozenRuntimeEvidence.ProvenByBinary))
            throw new InvalidDataException("RUNVGA route evidence levels diverge from the frozen debugger/binary findings.");
        if (routes.ProvenPairs.Count != 2 ||
            routes.ProvenPairs[0] is not { From: "Save.failed", To: "Disk.retry" } ||
            routes.ProvenPairs[1] is not { From: "Restore.loadFailed", To: "Restore.fileNotFound" })
            throw new InvalidDataException("RUNVGA SAVE/RESTORE route pairing diverges from the proven binary map.");
        if (routes.Routes.Any(route => route.OriginalBlockStart <= 0 || route.RuntimeSourceAddress <= 0 || route.RouteAddress <= 0 || string.IsNullOrWhiteSpace(route.LayoutConstraints)))
            throw new InvalidDataException("RUNVGA route evidence is missing source, route, or layout-constraint metadata.");
    }
}

internal sealed record ExecutableFingerprint(int Size, string Sha256)
{
    internal bool Matches(int size, string sha256) => Size == size && Sha256.Equals(sha256, StringComparison.OrdinalIgnoreCase);
}

internal sealed record FrozenFontDescriptor(
    int PhysicalFileOffset,
    int ModuleOffset,
    int ByteLength,
    int? RelativeSegment,
    int? RendererSegmentOperandPhysicalOffset = null,
    byte[]? HudBytes = null)
{
    internal bool IsPersistentBank(int start, int endInclusive, int relativeSegment) =>
        ModuleOffset == start && ByteLength == endInclusive - start + 1 && RelativeSegment == relativeSegment;
}

internal sealed record FrozenStackDescriptor(int Ss, int Sp);

internal enum FrozenRuntimeUiDispatch { Direct, FirstSharedDispatch, SecondSharedDispatch }
internal enum FrozenRuntimeEvidence { ProvenLive, ProvenByBinary }

/// <summary>How a RUNVGA runtime source address was established.</summary>
internal enum RunVgaRouteSourceKind { DataAddress, BreakpointAddress }

/// <summary>
/// Frozen RUNVGA runtime-route evidence. This describes binary routes only:
/// original text-block starts, runtime source addresses (DI data addresses or
/// BP breakpoint addresses), approximate renderer/dispatcher call sites, and the
/// R4B layout-constraint flags. It deliberately carries no storage spans or
/// capacities, so it can never be borrowed as an edit-size contract. Route
/// evidence, layout safety, and build materialization are three separate
/// concepts: proven routes here do not imply safe arbitrary lengths.
/// </summary>
internal sealed record FrozenRunVgaRouteRecord(
    string Name,
    int OriginalBlockStart,
    int RuntimeSourceAddress,
    RunVgaRouteSourceKind RuntimeSourceKind,
    FrozenRuntimeUiDispatch Dispatch,
    int RouteAddress,
    FrozenRuntimeEvidence Evidence,
    string LayoutConstraints);

internal sealed record FrozenRunVgaRoutePair(string From, string To);

/// <summary>
/// Owner-supplied frozen RUNVGA route evidence from historical debugger and
/// reverse-engineering work. Pause.menu relocation was tested live (modified
/// text visibly appeared in the running game; Continue/Quit hotspot behavior
/// was debugged live). SAVE/RESTORE pairings are proven by binary. Route
/// addresses are approximate call sites; exact debugger-step counts were never
/// recorded and must not be invented.
/// </summary>
internal sealed record FrozenRunVgaRouteEvidence(
    IReadOnlyList<FrozenRunVgaRouteRecord> Routes,
    IReadOnlyList<FrozenRunVgaRoutePair> ProvenPairs)
{
    internal FrozenRunVgaRouteRecord Route(string name) =>
        Routes.SingleOrDefault(route => route.Name.Equals(name, StringComparison.Ordinal))
        ?? throw new InvalidDataException($"Frozen RUNVGA route evidence has no record '{name}'.");
}

internal sealed record FrozenRuntimeUiRecord(
    string Name, int Selector, int BankOffset, int Length, int PointerSitePhysicalOffset,
    byte PointerOpcode, int OriginalPointer, int OriginalEnd, FrozenRuntimeUiDispatch Dispatch, FrozenRuntimeEvidence Evidence);

internal sealed record FrozenRejectedMemoryRange(int ModuleStart, int ModuleEndInclusive, string Classification);

internal sealed record FrozenRuntimeUiDescriptor(
    bool IsFrozen,
    string Notes,
    int ModuleStart = 0,
    int ModuleEndInclusive = 0,
    int PhysicalStart = 0,
    int PhysicalEndInclusive = 0,
    int RelativeSegment = 0,
    int BridgeOffset = 0,
    int BridgeRelocationOffset = 0,
    IReadOnlyList<FrozenRuntimeUiRecord>? Records = null,
    IReadOnlyList<FrozenRejectedMemoryRange>? RejectedRanges = null)
{
    internal static FrozenRuntimeUiDescriptor RunVgaProduction { get; } = new(true,
        "Production runtime UI descriptor is implemented by the existing RUNVGA descriptor path; patch-time string scanning is forbidden.");
    internal static FrozenRuntimeUiDescriptor RuneGaProduction { get; } = new(true,
        "RUNEGA_RUNTIME_PERSISTENT_UI_BANK_31000_PROVEN_LIVE.",
        0x31000, 0x317FF, 0x33400, 0x33BFF, 0x3100, 0x00D4, 0x00DB,
        Records:
        [
            new("Pause.menu", 0xF000, 0x00DD, 42, 0xCDD8, 0xBF, 0x2D21, 0x2D48, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenLive),
            new("Confirm.generic", 0xF001, 0x0107, 39, 0xCE0A, 0xBF, 0x2D48, 0x2D70, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenByBinary),
            new("Save.prompt", 0xF002, 0x012E, 51, 0xCBFC, 0xBF, 0x2D70, 0x2DA3, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenLive),
            new("Save.failed", 0xF003, 0x0161, 18, 0xCD03, 0xBF, 0x2DA4, 0x2DB6, FrozenRuntimeUiDispatch.FirstSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Restore.loadFailed", 0xF004, 0x0173, 18, 0xCD1A, 0xBF, 0x2DB6, 0x2DC8, FrozenRuntimeUiDispatch.FirstSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Restore.fileNotFound", 0xF005, 0x0185, 19, 0xCD1D, 0xBD, 0x2DC8, 0x2DDB, FrozenRuntimeUiDispatch.SecondSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Disk.retry", 0xF006, 0x0198, 21, 0xCD06, 0xBD, 0x2DDC, 0x2DF1, FrozenRuntimeUiDispatch.SecondSharedDispatch, FrozenRuntimeEvidence.ProvenByBinary),
            new("Save.overwrite", 0xF007, 0x01AD, 62, 0xCCDD, 0xBF, 0x2DF2, 0x2E30, FrozenRuntimeUiDispatch.Direct, FrozenRuntimeEvidence.ProvenByBinary)
        ],
        RejectedRanges: [new(0x30800, 0x30FFF, "REJECTED_RUNTIME_WRITTEN")]);

    internal IReadOnlyList<FrozenRuntimeUiRecord> RuntimeRecords => Records ?? [];
    internal IReadOnlyList<FrozenRejectedMemoryRange> RejectedCandidateRanges => RejectedRanges ?? [];

    internal bool IsPersistentBank(int moduleStart, int moduleEndInclusive, int physicalStart, int physicalEndInclusive, int relativeSegment) =>
        ModuleStart == moduleStart && ModuleEndInclusive == moduleEndInclusive && PhysicalStart == physicalStart &&
        PhysicalEndInclusive == physicalEndInclusive && RelativeSegment == relativeSegment;
}

internal sealed record FrozenExecutableDescriptor(
    string Renderer,
    string ActiveFileName,
    string GeneratedSlovakFileName,
    ExecutableFingerprint PackedOriginal,
    ExecutableFingerprint CanonicalUnpacked,
    FrozenFontDescriptor Font,
    FrozenRuntimeUiDescriptor RuntimeUi,
    FrozenStackDescriptor? InitialStack);

internal sealed record FrozenGamePcDescriptor(string ActiveFileName, string GeneratedSlovakFileName, string OriginalSha256);

internal sealed record FrozenLauncherCompanionDescriptor(
    string ActiveLauncherFile, string ImmutableBackupFile, string SoundStateFile, string MenuHelperFile, string VariantCatalogFile,
    string GogOverlayDirectory, string GogOverlayBackupFile, IReadOnlyList<string> ModFiles);

internal sealed record FrozenCompatibilityNote(string Environment, string Observation, string Cause, IReadOnlyList<string> CrossCheckResults);
