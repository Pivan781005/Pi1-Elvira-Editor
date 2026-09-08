using System.Text.Json;
using System.Text.Encodings.Web;

namespace Pi1ElviraEditor;

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
internal enum RuntimeUiTextLoadStatus { Success, InvalidJson, UnsupportedSchema, GameMismatch, DuplicateLogicalId, UnknownLogicalId, UnknownRuntime, InvalidDocument }

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

/// <summary>Runtime-scoped project override (schema v2). An EGA override is
/// never a VGA override even when the logical record ID is identical:
/// (Runtime, LogicalRecordId) is the storage identity.</summary>
internal sealed record RuntimeUiTextOverride(VariantRuntimeKind Runtime, RuntimeUiLogicalRecordId LogicalRecordId, string Text);

/// <summary>Immutable project-owned state, scoped per editable project
/// variant (SK/S1/...). Overrides are keyed by (Runtime, LogicalRecordId);
/// switching runtime projects only that runtime's state.</summary>
internal sealed record RuntimeUiTextState(
    string GameId,
    IReadOnlyList<RuntimeUiTextOverride> Overrides)
{
    internal static RuntimeUiTextState Empty(ElviraGameProfile game) =>
        new(PristineManifestService.GameIdFor(game), []);
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
/// States carry runtime-scoped overrides only; there is no migration from
/// pre-release storage models: schema v1 files and v2 residue payloads fail
/// closed on load instead of being converted or silently dropped.
/// </summary>
internal sealed class RuntimeUiTextService
{
    internal const int SchemaVersion = 2;
    internal const string FileName = "runtime-ui.json";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    private sealed record PersistedDocument(int SchemaVersion, string GameId, IReadOnlyList<PersistedRecord> Records);
    private sealed record PersistedRecord(string Runtime, string LogicalRecordId, string Text);

    public string GetPath(ProjectContext project, string projectVariantCode) =>
        ProjectVariantOwnership.GetOwnedStatePath(RequireProject(project), projectVariantCode, FileName);

    public IReadOnlyList<RuntimeUiTextDefinition> GetDefinitions(ProjectContext project) => DefinitionsFor(RequireProject(project).GameProfile);

    public RuntimeUiTextLoadResult Load(ProjectContext project, string projectVariantCode)
    {
        project = RequireProject(project);
        if (ProjectVariantOwnership.IsOriginal(projectVariantCode)) return new(RuntimeUiTextLoadStatus.Success, RuntimeUiTextState.Empty(project.GameProfile));
        return LoadPath(project, GetPath(project, projectVariantCode));
    }

    public RuntimeUiTextState SetOverride(ProjectContext project, RuntimeUiTextState state, VariantRuntimeKind runtime, RuntimeUiLogicalRecordId logicalRecordId, string text)
    {
        project = RequireProject(project); ValidateState(project, state); RequireDefinition(project.GameProfile, logicalRecordId);
        RequireSupportedRuntime(project.GameProfile, runtime, logicalRecordId);
        if (text is null) throw new ArgumentNullException(nameof(text));
        var values = state.Overrides.Where(value => value.Runtime != runtime || value.LogicalRecordId != logicalRecordId).Append(new(runtime, logicalRecordId, text))
            .OrderBy(value => value.Runtime).ThenBy(value => value.LogicalRecordId).ToArray();
        return state with { Overrides = values };
    }

    public RuntimeUiTextState RemoveOverride(ProjectContext project, RuntimeUiTextState state, VariantRuntimeKind runtime, RuntimeUiLogicalRecordId logicalRecordId)
    {
        project = RequireProject(project); ValidateState(project, state); RequireDefinition(project.GameProfile, logicalRecordId);
        return state with { Overrides = state.Overrides.Where(value => value.Runtime != runtime || value.LogicalRecordId != logicalRecordId).OrderBy(value => value.Runtime).ThenBy(value => value.LogicalRecordId).ToArray() };
    }

    public IReadOnlyList<RuntimeUiRuntimeProjection> GetEffectiveRecords(ProjectContext project, RuntimeUiTextState state) =>
        ProjectRecords(RequireProject(project), null, state);

    public IReadOnlyList<RuntimeUiRuntimeProjection> GetEffectiveRecords(VariantContext variant, RuntimeUiTextState state)
    {
        if (variant is null) throw new ArgumentNullException(nameof(variant));
        return ProjectRecords(variant.Project, variant.RuntimeKind, state);
    }

    /// <summary>Overrides applying to exactly one runtime. Builds consume
    /// only this projection; overrides for other runtimes never leak in.</summary>
    public IReadOnlyList<RuntimeUiTextOverride> GetOverridesForRuntime(RuntimeUiTextState state, VariantRuntimeKind runtime)
    {
        if (state is null) throw new ArgumentNullException(nameof(state));
        return state.Overrides.Where(value => value.Runtime == runtime).OrderBy(value => value.LogicalRecordId).ToArray();
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
        return LoadFile(project, path);
    }

    private RuntimeUiTextLoadResult LoadFile(ProjectContext project, string path)
    {
        string json;
        try { json = File.ReadAllText(path); }
        catch (IOException ex) { return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: ex.Message); }
        int? schema = PeekSchemaVersion(json);
        if (schema is null)
            return new(RuntimeUiTextLoadStatus.InvalidJson, Detail: UiText.Get("RuntimeUi.Detail.DocumentIncomplete"));
        if (schema != SchemaVersion)
        {
            if (schema <= 0)
                return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: UiText.Get("RuntimeUi.Detail.DocumentIncomplete"));
            return new(RuntimeUiTextLoadStatus.UnsupportedSchema, Detail: string.Format(UiText.Get("RuntimeUi.Detail.UnsupportedSchema"), schema));
        }
        if (HasUnassignedResiduePayload(json))
            return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: UiText.Get("RuntimeUi.Detail.LegacyResidueUnsupported"));
        try
        {
            PersistedDocument? document = JsonSerializer.Deserialize<PersistedDocument>(json, JsonOptions);
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

    private static int? PeekSchemaVersion(string json)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            if (document.RootElement.TryGetProperty("schemaVersion", out JsonElement version) && version.ValueKind == JsonValueKind.Number)
                return version.GetInt32();
            return -1;
        }
        catch (JsonException) { return null; }
    }

    /// <summary>Fail-closed guard for pre-release v2 residue payloads: files
    /// carrying an unassignedLegacy array are rejected instead of being
    /// silently dropped or migrated.</summary>
    private static bool HasUnassignedResiduePayload(string json)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty("unassignedLegacy", out JsonElement residue) &&
                residue.ValueKind == JsonValueKind.Array && residue.GetArrayLength() > 0;
        }
        catch (JsonException) { return false; }
    }

    private RuntimeUiTextSaveResult SavePath(ProjectContext project, RuntimeUiTextState state, string path)
    {
        ValidateState(project, state);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            return WriteValidated(project, state, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }

    /// <summary>Writes validated state only. There is no migration from
    /// pre-release storage models and no backup file.</summary>
    private static RuntimeUiTextSaveResult WriteValidated(ProjectContext project, RuntimeUiTextState state, string path)
    {
        try
        {
            var document = new PersistedDocument(SchemaVersion, state.GameId,
                state.Overrides.OrderBy(value => value.Runtime).ThenBy(value => value.LogicalRecordId)
                    .Select(value => new PersistedRecord(value.Runtime.ToString(), value.LogicalRecordId.ToString(), value.Text)).ToArray());
            // Validate the exact serializable output before touching storage.
            string payload = JsonSerializer.Serialize(document, JsonOptions);
            using (JsonDocument.Parse(payload)) { }
            string temporary = path + ".tmp";
            try
            {
                File.WriteAllText(temporary, payload);
                File.Move(temporary, path, true);
            }
            finally { if (File.Exists(temporary)) File.Delete(temporary); }
            return new(true, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }

    internal static bool IsEditableForRuntime(ElviraGameProfile game, VariantRuntimeKind runtime, RuntimeUiLogicalRecordId id) =>
        (game, runtime, id) switch
        {
            (ElviraGameProfile.Elvira1, VariantRuntimeKind.Elvira1Ega, _) => RunEgaUiService.IsEditable(id),
            (ElviraGameProfile.Elvira1, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu) => true,
            (ElviraGameProfile.Elvira2, VariantRuntimeKind.Elvira2Vga, RuntimeUiLogicalRecordId.SaveFailure) => true,
            _ => false
        };

    private static bool TryValidateForRuntime(ElviraGameProfile game, VariantRuntimeKind runtime, RuntimeUiLogicalRecordId id, string text)
    {
        if (text is null || text.IndexOf('\0') >= 0) return false;
        try
        {
            if (game == ElviraGameProfile.Elvira1 && runtime == VariantRuntimeKind.Elvira1Ega)
                return RunEgaUiService.TryValidateStored(text, id, out _, out _);
            if (game == ElviraGameProfile.Elvira1 && runtime == VariantRuntimeKind.Elvira1Vga && id == RuntimeUiLogicalRecordId.PauseMenu)
                return RunVgaPauseMenuService.TryEncodePauseOverride(text, out _, out _, out _);
            if (game == ElviraGameProfile.Elvira2 && runtime == VariantRuntimeKind.Elvira2Vga && id == RuntimeUiLogicalRecordId.SaveFailure)
            {
                System.Text.Encoding source = GamePcTextEditor.GetEncoding("CP852");
                System.Text.Encoding strict = System.Text.Encoding.GetEncoding(source.CodePage, System.Text.EncoderFallback.ExceptionFallback, System.Text.DecoderFallback.ExceptionFallback);
                byte[] encoded = strict.GetBytes(text);
                return string.Equals(text, strict.GetString(encoded), StringComparison.Ordinal);
            }
        }
        catch (Exception ex) when (ex is ArgumentException or System.Text.EncoderFallbackException) { return false; }
        return false;
    }

    private static RuntimeUiTextLoadResult ValidatePersisted(ElviraGameProfile game, PersistedDocument document)
    {
        var values = new List<RuntimeUiTextOverride>(); var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (PersistedRecord record in document.Records)
        {
            if (record is null || string.IsNullOrWhiteSpace(record.Runtime) || string.IsNullOrWhiteSpace(record.LogicalRecordId) || record.Text is null)
                return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: UiText.Get("RuntimeUi.Detail.RecordIncomplete"));
            if (!Enum.TryParse(record.Runtime, true, out VariantRuntimeKind runtime))
                return new(RuntimeUiTextLoadStatus.UnknownRuntime, Detail: string.Format(UiText.Get("RuntimeUi.Detail.UnknownRuntime"), record.Runtime));
            if (!Enum.TryParse(record.LogicalRecordId, true, out RuntimeUiLogicalRecordId id) || !DefinitionsFor(game).Any(definition => definition.LogicalRecordId == id))
                return new(RuntimeUiTextLoadStatus.UnknownLogicalId, Detail: string.Format(UiText.Get("RuntimeUi.Detail.UnknownLogicalId"), record.LogicalRecordId));
            if (!DefinitionsFor(game).Single(definition => definition.LogicalRecordId == id).SupportedRuntimes.Contains(runtime))
                return new(RuntimeUiTextLoadStatus.InvalidDocument, Detail: string.Format(UiText.Get("RuntimeUi.Detail.RuntimeNotSupported"), record.Runtime, record.LogicalRecordId));
            if (!seen.Add(runtime + "|" + id)) return new(RuntimeUiTextLoadStatus.DuplicateLogicalId, Detail: string.Format(UiText.Get("RuntimeUi.Detail.DuplicateLogicalId"), record.Runtime + "/" + record.LogicalRecordId));
            values.Add(new(runtime, id, record.Text));
        }
        return new(RuntimeUiTextLoadStatus.Success, new(PristineManifestService.GameIdFor(game),
            values.OrderBy(value => value.Runtime).ThenBy(value => value.LogicalRecordId).ToArray()));
    }

    private static IReadOnlyList<RuntimeUiRuntimeProjection> ProjectRecords(ProjectContext project, VariantRuntimeKind? runtime, RuntimeUiTextState state)
    {
        ValidateState(project, state);
        // Runtime-scoped identity: only overrides stored for the inspected
        // runtime project into its view. EGA state never leaks into VGA.
        var overrides = state.Overrides.Where(value => runtime is null || value.Runtime == runtime.Value)
            .ToDictionary(value => value.LogicalRecordId);
        return DefinitionsFor(project.GameProfile).Select(definition =>
        {
            bool applies = runtime is null || definition.SupportedRuntimes.Contains(runtime.Value);
            RuntimeUiTextOverride? value = null;
            bool overridden = runtime is not null && overrides.TryGetValue(definition.LogicalRecordId, out value) && value is not null;
            string? text = overridden && value is not null ? value.Text : definition.FrozenDefaultText;
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
        // storage spans or capacities, except the three human-proven-live
        // variable-width records below, whose A-class direct routes plus live
        // DOSBox acceptance (RVGBTN POC: anchored variable labels, separate
        // hotspot geometry) authorize production materialization. All other
        // RUNVGA records stay KnownButMappingIncomplete. EGA spans must
        // never be borrowed for RUNVGA capacities. Route evidence, layout safety,
        // and build materialization remain three separate concepts.
        if (runtime == VariantRuntimeKind.Elvira1Ega)
            return (RuntimeUiMappingReadiness.SupportedAndMapped, RunEgaEvidence(id));
        if (runtime == VariantRuntimeKind.Elvira1Vga && RunVgaVariableUiService.IsVariableRecord(id))
            return (RuntimeUiMappingReadiness.SupportedAndMapped, RuntimeUiEvidenceStatus.ProvenLive);
        return (RuntimeUiMappingReadiness.KnownButMappingIncomplete, RunVgaEvidence(id));
    }

    private static RuntimeUiEvidenceStatus RunEgaEvidence(RuntimeUiLogicalRecordId id) =>
        RunEgaUiService.Evidence(id) switch
        {
            FrozenRuntimeEvidence.ProvenLive => RuntimeUiEvidenceStatus.ProvenLive,
            _ => RuntimeUiEvidenceStatus.ProvenByBinary
        };

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
    private static void RequireSupportedRuntime(ElviraGameProfile game, VariantRuntimeKind runtime, RuntimeUiLogicalRecordId id)
    {
        RuntimeUiTextDefinition definition = DefinitionsFor(game).Single(item => item.LogicalRecordId == id);
        if (!definition.SupportedRuntimes.Contains(runtime))
            throw new ArgumentException($"Logical runtime UI record '{id}' does not apply to runtime '{runtime}'.", nameof(runtime));
    }
    private static void ValidateState(ProjectContext project, RuntimeUiTextState? state)
    {
        if (state is null) throw new ArgumentNullException(nameof(state));
        if (!state.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) throw new ArgumentException("Runtime UI text state belongs to another game.", nameof(state));
        if (state.Overrides.GroupBy(value => (value.Runtime, value.LogicalRecordId)).Any(group => group.Count() != 1)) throw new ArgumentException("Runtime UI text state contains duplicate runtime records.", nameof(state));
        foreach (RuntimeUiTextOverride value in state.Overrides) { RequireDefinition(project.GameProfile, value.LogicalRecordId); if (value.Text is null) throw new ArgumentException("Runtime UI override text must be non-null.", nameof(state)); }
    }
}
