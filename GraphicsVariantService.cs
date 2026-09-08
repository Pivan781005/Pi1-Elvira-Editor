using System.Text.Json;

namespace Pi1ElviraEditor;

internal enum GraphicsEditScope { Shared, RuntimeSpecific }
internal enum GraphicsApplicabilityStatus { Shared, RuntimeSpecific, Unsupported }
internal enum GraphicsProjectLoadStatus { Success, InvalidJson, UnsupportedSchema, GameMismatch, DuplicateIdentity, InvalidRecord }

/// <summary>Stable logical identity for one image in one named game resource.
/// It excludes variant path and binary offsets deliberately.</summary>
internal sealed record GraphicsProjectIdentity
{
    public GraphicsProjectIdentity(string resourceFileName, int imageId)
    {
        ResourceFileName = ValidateResource(resourceFileName);
        ImageId = ValidateImage(imageId);
    }
    public string ResourceFileName { get; }
    public int ImageId { get; }
    private static string ValidateResource(string value)
    {
        string file = Path.GetFileName(value ?? string.Empty).ToUpperInvariant();
        if (!file.Equals(value, StringComparison.OrdinalIgnoreCase) || !GameDataFileService.IsDos83FileName(file) || !file.EndsWith(".VGA", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Graphics resource must be a DOS 8.3 VGA filename.", nameof(value));
        return file;
    }
    private static int ValidateImage(int value) => value >= 0 ? value : throw new ArgumentOutOfRangeException(nameof(value));
}

internal sealed record GraphicsProjectEdit
{
    public GraphicsProjectEdit(GraphicsProjectIdentity identity, string replacementPngPath, GraphicsEditScope scope, VariantRuntimeKind? runtimeKind)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        ReplacementPngPath = ValidatePath(replacementPngPath);
        Scope = scope;
        RuntimeKind = runtimeKind;
        if ((scope == GraphicsEditScope.Shared && runtimeKind is not null) || (scope == GraphicsEditScope.RuntimeSpecific && runtimeKind is null))
            throw new ArgumentException("Graphics edit scope and runtime kind are inconsistent.");
    }
    public GraphicsProjectIdentity Identity { get; }
    public string ReplacementPngPath { get; }
    public GraphicsEditScope Scope { get; }
    public VariantRuntimeKind? RuntimeKind { get; }
    private static string ValidatePath(string value) => string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Replacement PNG path is required.", nameof(value)) : value;
}

internal sealed record GraphicsProjectState(string GameId, IReadOnlyList<GraphicsProjectEdit> Edits)
{
    internal static GraphicsProjectState Empty(ElviraGameProfile game) => new(PristineManifestService.GameIdFor(game), []);
}
internal sealed record GraphicsProjectLoadResult(GraphicsProjectLoadStatus Status, GraphicsProjectState? State = null, string? Detail = null)
{ public bool IsSuccess => Status == GraphicsProjectLoadStatus.Success && State is not null; }
internal sealed record GraphicsProjectSaveResult(bool Succeeded, string Path, string? Detail = null);
internal sealed record GraphicsVariantProjection(GraphicsProjectEdit Edit, GraphicsApplicabilityStatus Applicability);

/// <summary>Project-data and read-only variant projection only. It deliberately
/// does not open, rebuild, or write a VGA resource.</summary>
internal sealed class GraphicsVariantService
{
    internal const int SchemaVersion = 1;
    internal const string FileName = "graphics-edits.json";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private sealed record Document(int SchemaVersion, string GameId, IReadOnlyList<Record> Edits);
    private sealed record Record(string ResourceFileName, int ImageId, string ReplacementPngPath, GraphicsEditScope Scope, VariantRuntimeKind? RuntimeKind);

    // Legacy runtime-wide path. It is retained only for read-only migration
    // detection and old isolated-service callers; MainForm never projects it.
    public string GetPath(ProjectContext project) => Path.Combine(Require(project).ProjectRoot, FileName);
    public string GetPath(ProjectContext project, string projectVariantCode) =>
        ProjectVariantOwnership.GetOwnedStatePath(Require(project), projectVariantCode, FileName);
    public GraphicsProjectLoadResult Load(ProjectContext project)
    {
        project = Require(project); string path = GetPath(project);
        if (!File.Exists(path)) return new(GraphicsProjectLoadStatus.Success, GraphicsProjectState.Empty(project.GameProfile));
        try
        {
            Document? document = JsonSerializer.Deserialize<Document>(File.ReadAllText(path), JsonOptions);
            if (document is null || document.Edits is null || string.IsNullOrWhiteSpace(document.GameId)) return new(GraphicsProjectLoadStatus.InvalidRecord, Detail: "Graphics project document is incomplete.");
            if (document.SchemaVersion != SchemaVersion) return new(GraphicsProjectLoadStatus.UnsupportedSchema);
            if (!document.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) return new(GraphicsProjectLoadStatus.GameMismatch);
            var edits = new List<GraphicsProjectEdit>(); var identities = new HashSet<GraphicsProjectIdentity>();
            foreach (Record record in document.Edits)
            {
                try
                {
                    var edit = new GraphicsProjectEdit(new(record.ResourceFileName, record.ImageId), record.ReplacementPngPath, record.Scope, record.RuntimeKind);
                    ValidateRuntime(project.GameProfile, edit);
                    if (!identities.Add(edit.Identity)) return new(GraphicsProjectLoadStatus.DuplicateIdentity, Detail: edit.Identity.ToString());
                    edits.Add(edit);
                }
                catch (ArgumentException ex) { return new(GraphicsProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
            }
            return new(GraphicsProjectLoadStatus.Success, new(document.GameId, Sort(edits)));
        }
        catch (JsonException ex) { return new(GraphicsProjectLoadStatus.InvalidJson, Detail: ex.Message); }
        catch (IOException ex) { return new(GraphicsProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
    }

    public GraphicsProjectLoadResult Load(ProjectContext project, string projectVariantCode)
    {
        project = Require(project);
        if (ProjectVariantOwnership.IsOriginal(projectVariantCode))
            return new(GraphicsProjectLoadStatus.Success, GraphicsProjectState.Empty(project.GameProfile));
        return LoadPath(project, GetPath(project, projectVariantCode));
    }

    public GraphicsProjectState SetEdit(ProjectContext project, GraphicsProjectState state, GraphicsProjectEdit edit)
    {
        project = Require(project); ValidateState(project, state); ValidateRuntime(project.GameProfile, edit);
        return state with { Edits = Sort(state.Edits.Where(value => value.Identity != edit.Identity).Append(edit)) };
    }
    public GraphicsProjectState RemoveEdit(ProjectContext project, GraphicsProjectState state, GraphicsProjectIdentity identity)
    {
        project = Require(project); ValidateState(project, state);
        return state with { Edits = Sort(state.Edits.Where(value => value.Identity != identity)) };
    }
    public IReadOnlyList<GraphicsVariantProjection> GetGraphicsEditsForVariant(ProjectContext project, GraphicsProjectState state, VariantContext variant)
    {
        project = Require(project); ValidateState(project, state);
        if (variant is null || !ReferenceEquals(project, variant.Project)) throw new ArgumentException("Variant must belong to this project.", nameof(variant));
        return state.Edits.Select(edit => new GraphicsVariantProjection(edit, GetApplicability(project.GameProfile, edit, variant.RuntimeKind)))
            .Where(item => item.Applicability != GraphicsApplicabilityStatus.Unsupported).ToArray();
    }
    public GraphicsApplicabilityStatus GetApplicability(ElviraGameProfile game, GraphicsProjectEdit edit, VariantRuntimeKind runtime)
    {
        ValidateRuntime(game, edit);
        return edit.Scope == GraphicsEditScope.Shared ? GraphicsApplicabilityStatus.Shared : edit.RuntimeKind == runtime ? GraphicsApplicabilityStatus.RuntimeSpecific : GraphicsApplicabilityStatus.Unsupported;
    }
    public GraphicsProjectSaveResult Save(ProjectContext project, GraphicsProjectState state)
    {
        project = Require(project); ValidateState(project, state); string path = GetPath(project);
        try
        {
            // Creation is intentionally limited to this explicit Save operation.
            // Load/projection remains completely read-only for unopened projects.
            Directory.CreateDirectory(project.ProjectRoot);
            var document = new Document(SchemaVersion, state.GameId, state.Edits.Select(edit => new Record(edit.Identity.ResourceFileName, edit.Identity.ImageId, edit.ReplacementPngPath, edit.Scope, edit.RuntimeKind)).ToArray());
            string temp = path + ".tmp";
            try { File.WriteAllText(temp, JsonSerializer.Serialize(document, JsonOptions)); File.Move(temp, path, true); }
            finally { if (File.Exists(temp)) File.Delete(temp); }
            return new(true, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }

    public GraphicsProjectSaveResult Save(ProjectContext project, string projectVariantCode, GraphicsProjectState state)
    {
        project = Require(project);
        if (ProjectVariantOwnership.IsOriginal(projectVariantCode))
            return new(false, string.Empty, "The Original project is read-only. Create or select an editable project variant first.");
        return SavePath(project, state, GetPath(project, projectVariantCode));
    }

    private GraphicsProjectLoadResult LoadPath(ProjectContext project, string path)
    {
        if (!File.Exists(path)) return new(GraphicsProjectLoadStatus.Success, GraphicsProjectState.Empty(project.GameProfile));
        try
        {
            Document? document = JsonSerializer.Deserialize<Document>(File.ReadAllText(path), JsonOptions);
            if (document is null || document.Edits is null || string.IsNullOrWhiteSpace(document.GameId)) return new(GraphicsProjectLoadStatus.InvalidRecord, Detail: "Graphics project document is incomplete.");
            if (document.SchemaVersion != SchemaVersion) return new(GraphicsProjectLoadStatus.UnsupportedSchema);
            if (!document.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) return new(GraphicsProjectLoadStatus.GameMismatch);
            var edits = new List<GraphicsProjectEdit>(); var identities = new HashSet<GraphicsProjectIdentity>();
            foreach (Record record in document.Edits)
            {
                try
                {
                    var edit = new GraphicsProjectEdit(new(record.ResourceFileName, record.ImageId), record.ReplacementPngPath, record.Scope, record.RuntimeKind);
                    ValidateRuntime(project.GameProfile, edit);
                    if (!identities.Add(edit.Identity)) return new(GraphicsProjectLoadStatus.DuplicateIdentity, Detail: edit.Identity.ToString());
                    edits.Add(edit);
                }
                catch (ArgumentException ex) { return new(GraphicsProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
            }
            return new(GraphicsProjectLoadStatus.Success, new(document.GameId, Sort(edits)));
        }
        catch (JsonException ex) { return new(GraphicsProjectLoadStatus.InvalidJson, Detail: ex.Message); }
        catch (IOException ex) { return new(GraphicsProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
    }

    private GraphicsProjectSaveResult SavePath(ProjectContext project, GraphicsProjectState state, string path)
    {
        ValidateState(project, state);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var document = new Document(SchemaVersion, state.GameId, state.Edits.Select(edit => new Record(edit.Identity.ResourceFileName, edit.Identity.ImageId, edit.ReplacementPngPath, edit.Scope, edit.RuntimeKind)).ToArray());
            string temp = path + ".tmp";
            try { File.WriteAllText(temp, JsonSerializer.Serialize(document, JsonOptions)); File.Move(temp, path, true); }
            finally { if (File.Exists(temp)) File.Delete(temp); }
            return new(true, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }

    private static void ValidateState(ProjectContext project, GraphicsProjectState? state)
    {
        if (state is null || !state.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) throw new ArgumentException("Graphics project state belongs to another game.", nameof(state));
        if (state.Edits.GroupBy(edit => edit.Identity).Any(group => group.Count() != 1)) throw new ArgumentException("Graphics project state contains duplicate identities.", nameof(state));
        foreach (GraphicsProjectEdit edit in state.Edits) ValidateRuntime(project.GameProfile, edit);
    }
    private static void ValidateRuntime(ElviraGameProfile game, GraphicsProjectEdit edit)
    {
        if (edit.Scope == GraphicsEditScope.RuntimeSpecific && edit.RuntimeKind is not null && !IsRuntimeForGame(game, edit.RuntimeKind.Value))
            throw new ArgumentException("Graphics edit runtime is incompatible with the project game profile.", nameof(edit));
    }
    private static bool IsRuntimeForGame(ElviraGameProfile game, VariantRuntimeKind runtime) => (game, runtime) switch
    {
        (ElviraGameProfile.Elvira1, VariantRuntimeKind.Elvira1Vga or VariantRuntimeKind.Elvira1Ega) => true,
        (ElviraGameProfile.Elvira2, VariantRuntimeKind.Elvira2Vga) => true,
        _ => false
    };
    private static IReadOnlyList<GraphicsProjectEdit> Sort(IEnumerable<GraphicsProjectEdit> values) => values.OrderBy(value => value.Identity.ResourceFileName, StringComparer.Ordinal).ThenBy(value => value.Identity.ImageId).ToArray();
    private static ProjectContext Require(ProjectContext? project) => project ?? throw new ArgumentNullException(nameof(project));
}
