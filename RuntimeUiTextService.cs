using System.Text.Json;
using System.Text.Encodings.Web;

namespace ElviraVgaEditor;

/// <summary>Stable project identities for runtime-owned UI messages.  These
/// deliberately describe messages, not executable offsets or renderer slots.</summary>
internal enum RuntimeUiLogicalRecordId
{
    PauseMenu,
    ConfirmGeneric,
    SavePrompt,
    SaveFailure,
    LoadFailure,
    FileNotFound,
    TryAnotherDisk,
    SaveOverwrite
}

internal enum RuntimeUiMappingReadiness { SupportedAndMapped, KnownButMappingIncomplete, Unsupported }
internal enum RuntimeUiEvidenceStatus { Proven, ProvenByBinary, ProvenLive, StrongEvidence, Inferred, Unknown }
internal enum RuntimeUiTextOrigin { FrozenDefault, ProjectOverride, FrozenDefaultUnavailable }
internal enum RuntimeUiTextLoadStatus { Success, InvalidJson, UnsupportedSchema, GameMismatch, DuplicateLogicalId, UnknownLogicalId, InvalidDocument }

internal sealed record RuntimeUiTextDefinition(
    RuntimeUiLogicalRecordId LogicalRecordId,
    string DisplayName,
    string? FrozenDefaultText,
    IReadOnlyList<VariantRuntimeKind> SupportedRuntimes);

internal sealed record RuntimeUiRuntimeProjection(
    RuntimeUiLogicalRecordId LogicalRecordId,
    string DisplayName,
    string? EffectiveText,
    RuntimeUiTextOrigin TextOrigin,
    bool IsOverridden,
    bool AppliesToRuntime,
    RuntimeUiMappingReadiness MappingReadiness,
    RuntimeUiEvidenceStatus EvidenceStatus);

internal sealed record RuntimeUiTextOverride(RuntimeUiLogicalRecordId LogicalRecordId, string Text);

/// <summary>Immutable project-owned state. It is shared by all runtime variants
/// of a game; variants are projections, never storage identities.</summary>
internal sealed record RuntimeUiTextState(string GameId, IReadOnlyList<RuntimeUiTextOverride> Overrides)
{
    internal static RuntimeUiTextState Empty(ElviraGameProfile game) => new(PristineManifestService.GameIdFor(game), []);
}

internal sealed record RuntimeUiTextLoadResult(RuntimeUiTextLoadStatus Status, RuntimeUiTextState? State = null, string? Detail = null)
{
    public bool IsSuccess => Status == RuntimeUiTextLoadStatus.Success && State is not null;
}

internal sealed record RuntimeUiTextSaveResult(bool Succeeded, string Path, string? Detail = null);

/// <summary>
/// Project-only runtime UI text storage. It intentionally knows no executable
/// writer, byte encoding, layout allocator, or route repair. Frozen profile
/// descriptors remain the authority for those later stages.
/// </summary>
internal sealed class RuntimeUiTextService
{
    internal const int SchemaVersion = 1;
    internal const string FileName = "runtime-ui.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private sealed record PersistedDocument(int SchemaVersion, string GameId, IReadOnlyList<PersistedRecord> Records);
    private sealed record PersistedRecord(string LogicalRecordId, string Text);

    public string GetPath(ProjectContext project) => Path.Combine(RequireProject(project).ProjectRoot, FileName);
    public string GetPath(ProjectContext project, string projectVariantCode) =>
        ProjectVariantOwnership.GetOwnedStatePath(RequireProject(project), projectVariantCode, FileName);

    public IReadOnlyList<RuntimeUiTextDefinition> GetDefinitions(ProjectContext project) => DefinitionsFor(RequireProject(project).GameProfile);

    public RuntimeUiTextLoadResult Load(ProjectContext project)
    {
        project = RequireProject(project);
        string path = GetPath(project);
        if (!File.Exists(path)) return new(RuntimeUiTextLoadStatus.Success, RuntimeUiTextState.Empty(project.GameProfile));
        try
        {
            PersistedDocument? document = JsonSerializer.Deserialize<PersistedDocument>(File.ReadAllText(path), JsonOptions);
            if (document is null || string.IsNullOrWhiteSpace(document.GameId) || document.Records is null)
                return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: UiText.Get("RuntimeUi.Detail.DocumentIncomplete"));
            if (document.SchemaVersion != SchemaVersion)
                return new(RuntimeUiTextLoadStatus.UnsupportedSchema, Detail: string.Format(UiText.Get("RuntimeUi.Detail.UnsupportedSchema"), document.SchemaVersion));
            string expectedGameId = PristineManifestService.GameIdFor(project.GameProfile);
            if (!document.GameId.Equals(expectedGameId, StringComparison.Ordinal))
                return new(RuntimeUiTextLoadStatus.GameMismatch, Detail: UiText.Get("RuntimeUi.Detail.GameMismatch"));
            return ValidatePersisted(project.GameProfile, document);
        }
        catch (JsonException ex) { return new(RuntimeUiTextLoadStatus.InvalidJson, Detail: ex.Message); }
        catch (IOException ex) { return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: ex.Message); }
    }
    public RuntimeUiTextLoadResult Load(ProjectContext project, string projectVariantCode)
    {
        project = RequireProject(project);
        if (ProjectVariantOwnership.IsOriginal(projectVariantCode)) return new(RuntimeUiTextLoadStatus.Success, RuntimeUiTextState.Empty(project.GameProfile));
        return LoadPath(project, GetPath(project, projectVariantCode));
    }

    public RuntimeUiTextState SetOverride(ProjectContext project, RuntimeUiTextState state, RuntimeUiLogicalRecordId logicalRecordId, string text)
    {
        project = RequireProject(project); ValidateState(project, state); RequireDefinition(project.GameProfile, logicalRecordId);
        if (text is null) throw new ArgumentNullException(nameof(text));
        var values = state.Overrides.Where(value => value.LogicalRecordId != logicalRecordId).Append(new(logicalRecordId, text))
            .OrderBy(value => value.LogicalRecordId).ToArray();
        return state with { Overrides = values };
    }

    public RuntimeUiTextState RemoveOverride(ProjectContext project, RuntimeUiTextState state, RuntimeUiLogicalRecordId logicalRecordId)
    {
        project = RequireProject(project); ValidateState(project, state); RequireDefinition(project.GameProfile, logicalRecordId);
        return state with { Overrides = state.Overrides.Where(value => value.LogicalRecordId != logicalRecordId).OrderBy(value => value.LogicalRecordId).ToArray() };
    }

    public IReadOnlyList<RuntimeUiRuntimeProjection> GetEffectiveRecords(ProjectContext project, RuntimeUiTextState state) =>
        ProjectRecords(RequireProject(project), null, state);

    public IReadOnlyList<RuntimeUiRuntimeProjection> GetEffectiveRecords(VariantContext variant, RuntimeUiTextState state)
    {
        if (variant is null) throw new ArgumentNullException(nameof(variant));
        return ProjectRecords(variant.Project, variant.RuntimeKind, state);
    }

    public RuntimeUiTextSaveResult Save(ProjectContext project, RuntimeUiTextState state)
    {
        project = RequireProject(project); ValidateState(project, state);
        string path = GetPath(project);
        try
        {
            // Saving a user override is the explicit operation that may create
            // editor-owned project storage. Load remains read-only.
            Directory.CreateDirectory(project.ProjectRoot);
            var document = new PersistedDocument(SchemaVersion, state.GameId,
                state.Overrides.OrderBy(value => value.LogicalRecordId).Select(value => new PersistedRecord(value.LogicalRecordId.ToString(), value.Text)).ToArray());
            string temporary = path + ".tmp";
            try
            {
                File.WriteAllText(temporary, JsonSerializer.Serialize(document, JsonOptions));
                File.Move(temporary, path, true);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
            return new(true, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }
    public RuntimeUiTextSaveResult Save(ProjectContext project, string projectVariantCode, RuntimeUiTextState state)
    {
        project = RequireProject(project);
        if (ProjectVariantOwnership.IsOriginal(projectVariantCode)) return new(false, string.Empty, UiText.Get("RuntimeUi.Detail.ReadOnlyOriginal"));
        return SavePath(project, state, GetPath(project, projectVariantCode));
    }

    private RuntimeUiTextLoadResult LoadPath(ProjectContext project, string path)
    {
        if (!File.Exists(path)) return new(RuntimeUiTextLoadStatus.Success, RuntimeUiTextState.Empty(project.GameProfile));
        try
        {
            PersistedDocument? document = JsonSerializer.Deserialize<PersistedDocument>(File.ReadAllText(path), JsonOptions);
            if (document is null || string.IsNullOrWhiteSpace(document.GameId) || document.Records is null)
                return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: UiText.Get("RuntimeUi.Detail.DocumentIncomplete"));
            if (document.SchemaVersion != SchemaVersion) return new(RuntimeUiTextLoadStatus.UnsupportedSchema, Detail: string.Format(UiText.Get("RuntimeUi.Detail.UnsupportedSchema"), document.SchemaVersion));
            if (!document.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) return new(RuntimeUiTextLoadStatus.GameMismatch, Detail: UiText.Get("RuntimeUi.Detail.GameMismatch"));
            return ValidatePersisted(project.GameProfile, document);
        }
        catch (JsonException ex) { return new(RuntimeUiTextLoadStatus.InvalidJson, Detail: ex.Message); }
        catch (IOException ex) { return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: ex.Message); }
    }

    private RuntimeUiTextSaveResult SavePath(ProjectContext project, RuntimeUiTextState state, string path)
    {
        ValidateState(project, state);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var document = new PersistedDocument(SchemaVersion, state.GameId,
                state.Overrides.OrderBy(value => value.LogicalRecordId).Select(value => new PersistedRecord(value.LogicalRecordId.ToString(), value.Text)).ToArray());
            string temporary = path + ".tmp";
            try { File.WriteAllText(temporary, JsonSerializer.Serialize(document, JsonOptions)); File.Move(temporary, path, true); }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
            return new(true, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }

    private static RuntimeUiTextLoadResult ValidatePersisted(ElviraGameProfile game, PersistedDocument document)
    {
        var values = new List<RuntimeUiTextOverride>(); var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (PersistedRecord record in document.Records)
        {
            if (record is null || string.IsNullOrWhiteSpace(record.LogicalRecordId) || record.Text is null)
                return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: UiText.Get("RuntimeUi.Detail.RecordIncomplete"));
            if (!seen.Add(record.LogicalRecordId)) return new(RuntimeUiTextLoadStatus.DuplicateLogicalId, Detail: string.Format(UiText.Get("RuntimeUi.Detail.DuplicateLogicalId"), record.LogicalRecordId));
            if (!Enum.TryParse(record.LogicalRecordId, true, out RuntimeUiLogicalRecordId id) || !DefinitionsFor(game).Any(definition => definition.LogicalRecordId == id))
                return new(RuntimeUiTextLoadStatus.UnknownLogicalId, Detail: string.Format(UiText.Get("RuntimeUi.Detail.UnknownLogicalId"), record.LogicalRecordId));
            values.Add(new(id, record.Text));
        }
        return new(RuntimeUiTextLoadStatus.Success, new(PristineManifestService.GameIdFor(game), values.OrderBy(value => value.LogicalRecordId).ToArray()));
    }

    private static IReadOnlyList<RuntimeUiRuntimeProjection> ProjectRecords(ProjectContext project, VariantRuntimeKind? runtime, RuntimeUiTextState state)
    {
        ValidateState(project, state); var overrides = state.Overrides.ToDictionary(value => value.LogicalRecordId);
        return DefinitionsFor(project.GameProfile).Select(definition =>
        {
            bool applies = runtime is null || definition.SupportedRuntimes.Contains(runtime.Value);
            bool overridden = overrides.TryGetValue(definition.LogicalRecordId, out RuntimeUiTextOverride? value);
            string? text = overridden ? value!.Text : definition.FrozenDefaultText;
            RuntimeUiTextOrigin origin = overridden ? RuntimeUiTextOrigin.ProjectOverride : definition.FrozenDefaultText is null ? RuntimeUiTextOrigin.FrozenDefaultUnavailable : RuntimeUiTextOrigin.FrozenDefault;
            (RuntimeUiMappingReadiness readiness, RuntimeUiEvidenceStatus evidence) = MappingFor(project.GameProfile, runtime, definition.LogicalRecordId);
            return new RuntimeUiRuntimeProjection(definition.LogicalRecordId, definition.DisplayName, text, origin, overridden, applies, readiness, evidence);
        }).Where(record => runtime is null || record.AppliesToRuntime).ToArray();
    }

    private static (RuntimeUiMappingReadiness, RuntimeUiEvidenceStatus) MappingFor(ElviraGameProfile game, VariantRuntimeKind? runtime, RuntimeUiLogicalRecordId id)
    {
        if (runtime is null) return (RuntimeUiMappingReadiness.SupportedAndMapped, RuntimeUiEvidenceStatus.Proven);
        if (game == ElviraGameProfile.Elvira2)
            return id == RuntimeUiLogicalRecordId.SaveFailure
                ? (RuntimeUiMappingReadiness.SupportedAndMapped, RuntimeUiEvidenceStatus.ProvenByBinary)
                : (RuntimeUiMappingReadiness.KnownButMappingIncomplete, RuntimeUiEvidenceStatus.Unknown);
        // The shared E1 project identity does not prove identical binary layout.
        // RUNEGA has frozen record spans; RUNVGA has frozen binary ROUTES (source
        // blocks, runtime sources, renderer/dispatcher call sites) but no frozen
        // storage spans or capacities. Readiness therefore stays
        // KnownButMappingIncomplete while evidence comes per record from the
        // frozen RUNVGA route table: Pause.menu is PROVEN LIVE, the other listed
        // routes are PROVEN BY BINARY. EGA spans must never be borrowed for
        // RUNVGA capacities. Route evidence, layout safety, and build
        // materialization remain three separate concepts.
        return runtime == VariantRuntimeKind.Elvira1Ega
            ? (RuntimeUiMappingReadiness.SupportedAndMapped, RuntimeUiEvidenceStatus.ProvenByBinary)
            : (RuntimeUiMappingReadiness.KnownButMappingIncomplete, RunVgaEvidence(id));
    }

    private static RuntimeUiEvidenceStatus RunVgaEvidence(RuntimeUiLogicalRecordId id) =>
        Elvira1ProductionProfile.RunVgaRouteEvidence(id) switch
        {
            FrozenRuntimeEvidence.ProvenLive => RuntimeUiEvidenceStatus.ProvenLive,
            _ => RuntimeUiEvidenceStatus.ProvenByBinary
        };

    private static IReadOnlyList<RuntimeUiTextDefinition> DefinitionsFor(ElviraGameProfile game) => game switch
    {
        ElviraGameProfile.Elvira1 => Elvira1Definitions,
        ElviraGameProfile.Elvira2 => Elvira2Definitions,
        _ => throw new ArgumentException("A supported game profile is required.", nameof(game))
    };

    // Profiles freeze identity/mapping evidence, not decoded Unicode literals.
    // Null therefore means "use canonical binary text until a project override is supplied",
    // never an invented default string. R6M may validate/project literals later.
    private static readonly IReadOnlyList<RuntimeUiTextDefinition> Elvira1Definitions =
    [
        D(RuntimeUiLogicalRecordId.PauseMenu, "Pause.menu", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega),
        D(RuntimeUiLogicalRecordId.ConfirmGeneric, "Confirm.generic", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega),
        D(RuntimeUiLogicalRecordId.SavePrompt, "Save.prompt", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega),
        D(RuntimeUiLogicalRecordId.SaveFailure, "Save.failed", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega),
        D(RuntimeUiLogicalRecordId.LoadFailure, "Restore.loadFailed", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega),
        D(RuntimeUiLogicalRecordId.FileNotFound, "Restore.fileNotFound", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega),
        D(RuntimeUiLogicalRecordId.TryAnotherDisk, "Disk.retry", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega),
        D(RuntimeUiLogicalRecordId.SaveOverwrite, "Save.overwrite", VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega)
    ];
    private static readonly IReadOnlyList<RuntimeUiTextDefinition> Elvira2Definitions =
    [
        D(RuntimeUiLogicalRecordId.SaveFailure, "SAVE_FAILURE", VariantRuntimeKind.Elvira2Vga),
        D(RuntimeUiLogicalRecordId.LoadFailure, "LOAD_FAILURE", VariantRuntimeKind.Elvira2Vga),
        D(RuntimeUiLogicalRecordId.FileNotFound, "FILE_NOT_FOUND", VariantRuntimeKind.Elvira2Vga),
        D(RuntimeUiLogicalRecordId.TryAnotherDisk, "TRY_ANOTHER_DISK", VariantRuntimeKind.Elvira2Vga)
    ];
    private static RuntimeUiTextDefinition D(RuntimeUiLogicalRecordId id, string name, params VariantRuntimeKind[] runtimes) => new(id, name, FrozenDefaultText: null, runtimes);

    private static ProjectContext RequireProject(ProjectContext? project) => project ?? throw new ArgumentNullException(nameof(project));
    private static void RequireDefinition(ElviraGameProfile game, RuntimeUiLogicalRecordId id) { if (!DefinitionsFor(game).Any(definition => definition.LogicalRecordId == id)) throw new ArgumentException("Logical runtime UI record is not supported by this game.", nameof(id)); }
    private static void ValidateState(ProjectContext project, RuntimeUiTextState? state)
    {
        if (state is null) throw new ArgumentNullException(nameof(state));
        if (!state.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) throw new ArgumentException("Runtime UI text state belongs to another game.", nameof(state));
        if (state.Overrides.GroupBy(value => value.LogicalRecordId).Any(group => group.Count() != 1)) throw new ArgumentException("Runtime UI text state contains duplicate logical IDs.", nameof(state));
        foreach (RuntimeUiTextOverride value in state.Overrides) { RequireDefinition(project.GameProfile, value.LogicalRecordId); if (value.Text is null) throw new ArgumentException("Runtime UI override text must be non-null.", nameof(state)); }
    }
}
