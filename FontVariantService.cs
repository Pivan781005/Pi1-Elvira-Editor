using System.Text.Json;

namespace ElviraVgaEditor;

internal enum FontEditScope { Shared, RuntimeSpecific }
internal enum FontApplicabilityStatus { Shared, RuntimeSpecific, Unsupported, Protected }
internal enum FontRuntimeBank { Full, Low, High, Protected, Unsupported }
internal enum FontProjectLoadStatus { Success, InvalidJson, UnsupportedSchema, GameMismatch, DuplicateGlyph, InvalidRecord, ProtectedGlyph }

/// <summary>Stable project identity for a displayable DOS font glyph. Physical
/// offsets, executable names and variant directories are intentionally excluded.</summary>
internal sealed record FontProjectGlyphIdentity
{
    public FontProjectGlyphIdentity(int byteValue)
    {
        if (byteValue is < 0x20 or > 0xFF) throw new ArgumentOutOfRangeException(nameof(byteValue), "Only displayable CP852 byte slots are project glyphs.");
        ByteValue = byteValue;
    }
    public int ByteValue { get; }
}

internal sealed record FontProjectEdit
{
    public FontProjectEdit(FontProjectGlyphIdentity identity, string bitmapBase64, FontEditScope scope, VariantRuntimeKind? runtimeKind)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));
        BitmapBase64 = ValidateBitmap(bitmapBase64);
        Scope = scope;
        RuntimeKind = runtimeKind;
        if ((scope == FontEditScope.Shared && runtimeKind is not null) || (scope == FontEditScope.RuntimeSpecific && runtimeKind is null))
            throw new ArgumentException("Font edit scope and runtime kind are inconsistent.");
    }
    public FontProjectGlyphIdentity Identity { get; }
    public string BitmapBase64 { get; }
    public FontEditScope Scope { get; }
    public VariantRuntimeKind? RuntimeKind { get; }
    public byte[] Bitmap => Convert.FromBase64String(BitmapBase64);

    public static FontProjectEdit Create(FontProjectGlyphIdentity identity, ReadOnlySpan<byte> bitmap, FontEditScope scope, VariantRuntimeKind? runtimeKind)
    {
        if (bitmap.Length != RunVgaFontService.GlyphBytes) throw new ArgumentException("A font glyph must contain exactly eight rows.", nameof(bitmap));
        return new(identity, Convert.ToBase64String(bitmap), scope, runtimeKind);
    }

    private static string ValidateBitmap(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Glyph bitmap is required.", nameof(value));
        try
        {
            if (Convert.FromBase64String(value).Length != RunVgaFontService.GlyphBytes)
                throw new ArgumentException("Glyph bitmap must contain exactly eight rows.", nameof(value));
            return value;
        }
        catch (FormatException ex) { throw new ArgumentException("Glyph bitmap is not valid base64.", nameof(value), ex); }
    }
}

internal sealed record FontProjectState(string GameId, IReadOnlyList<FontProjectEdit> Edits)
{
    internal static FontProjectState Empty(ElviraGameProfile game) => new(PristineManifestService.GameIdFor(game), []);
}
internal sealed record FontProjectLoadResult(FontProjectLoadStatus Status, FontProjectState? State = null, string? Detail = null)
{ public bool IsSuccess => Status == FontProjectLoadStatus.Success && State is not null; }
internal sealed record FontProjectSaveResult(bool Succeeded, string Path, string? Detail = null);
internal sealed record FontSlotProjection(int ByteValue, FontRuntimeBank Bank, FontApplicabilityStatus Status);
internal sealed record FontVariantProjection(FontProjectEdit Edit, FontRuntimeBank Bank, FontApplicabilityStatus Applicability);

/// <summary>Project-only font edit metadata and read-only runtime projection.
/// This type does not load an executable, create a variant, or invoke a bootstrap service.</summary>
internal sealed class FontVariantService
{
    internal const int SchemaVersion = 1;
    internal const string FileName = "font-edits.json";
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private sealed record Document(int SchemaVersion, string GameId, IReadOnlyList<Record> Edits);
    private sealed record Record(int ByteValue, string BitmapBase64, FontEditScope Scope, VariantRuntimeKind? RuntimeKind);

    public string GetPath(ProjectContext project) => Path.Combine(Require(project).ProjectRoot, FileName);
    public string GetPath(ProjectContext project, string projectVariantCode) =>
        ProjectVariantOwnership.GetOwnedStatePath(Require(project), projectVariantCode, FileName);
    public FontProjectLoadResult Load(ProjectContext project)
    {
        project = Require(project); string path = GetPath(project);
        if (!File.Exists(path)) return new(FontProjectLoadStatus.Success, FontProjectState.Empty(project.GameProfile));
        try
        {
            Document? document = JsonSerializer.Deserialize<Document>(File.ReadAllText(path), JsonOptions);
            if (document is null || document.Edits is null || string.IsNullOrWhiteSpace(document.GameId)) return new(FontProjectLoadStatus.InvalidRecord, Detail: "Font project document is incomplete.");
            if (document.SchemaVersion != SchemaVersion) return new(FontProjectLoadStatus.UnsupportedSchema);
            if (!document.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) return new(FontProjectLoadStatus.GameMismatch);
            var edits = new List<FontProjectEdit>(); var identities = new HashSet<FontProjectGlyphIdentity>();
            foreach (Record record in document.Edits)
            {
                try
                {
                    var edit = new FontProjectEdit(new(record.ByteValue), record.BitmapBase64, record.Scope, record.RuntimeKind);
                    ValidateEdit(project.GameProfile, edit);
                    if (!identities.Add(edit.Identity)) return new(FontProjectLoadStatus.DuplicateGlyph, Detail: $"0x{edit.Identity.ByteValue:X2}");
                    edits.Add(edit);
                }
                catch (InvalidOperationException ex) { return new(FontProjectLoadStatus.ProtectedGlyph, Detail: ex.Message); }
                catch (ArgumentException ex) { return new(FontProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
            }
            return new(FontProjectLoadStatus.Success, new(document.GameId, Sort(edits)));
        }
        catch (JsonException ex) { return new(FontProjectLoadStatus.InvalidJson, Detail: ex.Message); }
        catch (IOException ex) { return new(FontProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
    }
    public FontProjectLoadResult Load(ProjectContext project, string projectVariantCode)
    {
        project = Require(project);
        if (ProjectVariantOwnership.IsOriginal(projectVariantCode)) return new(FontProjectLoadStatus.Success, FontProjectState.Empty(project.GameProfile));
        return LoadPath(project, GetPath(project, projectVariantCode));
    }

    public FontProjectState SetEdit(ProjectContext project, FontProjectState state, FontProjectEdit edit)
    {
        project = Require(project); ValidateState(project, state); ValidateEdit(project.GameProfile, edit);
        return state with { Edits = Sort(state.Edits.Where(value => value.Identity != edit.Identity).Append(edit)) };
    }
    public FontProjectState RemoveEdit(ProjectContext project, FontProjectState state, FontProjectGlyphIdentity identity)
    {
        project = Require(project); ValidateState(project, state);
        return state with { Edits = Sort(state.Edits.Where(value => value.Identity != identity)) };
    }
    public IReadOnlyList<FontVariantProjection> GetFontEditsForVariant(ProjectContext project, FontProjectState state, VariantContext variant)
    {
        project = Require(project); ValidateState(project, state);
        if (variant is null || !ReferenceEquals(project, variant.Project)) throw new ArgumentException("Variant must belong to this project.", nameof(variant));
        return state.Edits.Select(edit =>
        {
            FontSlotProjection slot = GetSlotProjection(variant, edit.Identity.ByteValue);
            FontApplicabilityStatus status = edit.Scope == FontEditScope.Shared ? FontApplicabilityStatus.Shared : edit.RuntimeKind == variant.RuntimeKind ? FontApplicabilityStatus.RuntimeSpecific : FontApplicabilityStatus.Unsupported;
            return new FontVariantProjection(edit, slot.Bank, status);
        }).Where(item => item.Applicability != FontApplicabilityStatus.Unsupported).OrderBy(item => item.Edit.Identity.ByteValue).ToArray();
    }
    public FontSlotProjection GetSlotProjection(VariantContext variant, int byteValue)
    {
        if (variant is null) throw new ArgumentNullException(nameof(variant));
        if (byteValue is < 0x20 or > 0xFF) return new(byteValue, FontRuntimeBank.Unsupported, FontApplicabilityStatus.Unsupported);
        if (byteValue == FontSlotMetadata.HudEraseGlyph) return new(byteValue, FontRuntimeBank.Protected, FontApplicabilityStatus.Protected);
        return variant.RuntimeKind switch
        {
            VariantRuntimeKind.Elvira1Vga or VariantRuntimeKind.Elvira1Ega => new(byteValue, FontRuntimeBank.Full, FontApplicabilityStatus.Shared),
            VariantRuntimeKind.Elvira2Vga when byteValue <= 0x80 => new(byteValue, FontRuntimeBank.Low, FontApplicabilityStatus.Shared),
            VariantRuntimeKind.Elvira2Vga => new(byteValue, FontRuntimeBank.High, FontApplicabilityStatus.Shared),
            _ => new(byteValue, FontRuntimeBank.Unsupported, FontApplicabilityStatus.Unsupported)
        };
    }
    public FontProjectSaveResult Save(ProjectContext project, FontProjectState state)
    {
        project = Require(project); ValidateState(project, state); string path = GetPath(project);
        try
        {
            Directory.CreateDirectory(project.ProjectRoot);
            var document = new Document(SchemaVersion, state.GameId, state.Edits.Select(edit => new Record(edit.Identity.ByteValue, edit.BitmapBase64, edit.Scope, edit.RuntimeKind)).ToArray());
            string temp = path + ".tmp";
            try { File.WriteAllText(temp, JsonSerializer.Serialize(document, JsonOptions)); File.Move(temp, path, true); }
            finally { if (File.Exists(temp)) File.Delete(temp); }
            return new(true, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }
    public FontProjectSaveResult Save(ProjectContext project, string projectVariantCode, FontProjectState state)
    {
        project = Require(project);
        if (ProjectVariantOwnership.IsOriginal(projectVariantCode)) return new(false, string.Empty, "The Original project is read-only. Create or select an editable project variant first.");
        return SavePath(project, state, GetPath(project, projectVariantCode));
    }

    private FontProjectLoadResult LoadPath(ProjectContext project, string path)
    {
        if (!File.Exists(path)) return new(FontProjectLoadStatus.Success, FontProjectState.Empty(project.GameProfile));
        try
        {
            Document? document = JsonSerializer.Deserialize<Document>(File.ReadAllText(path), JsonOptions);
            if (document is null || document.Edits is null || string.IsNullOrWhiteSpace(document.GameId)) return new(FontProjectLoadStatus.InvalidRecord, Detail: "Font project document is incomplete.");
            if (document.SchemaVersion != SchemaVersion) return new(FontProjectLoadStatus.UnsupportedSchema);
            if (!document.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) return new(FontProjectLoadStatus.GameMismatch);
            var edits = new List<FontProjectEdit>(); var identities = new HashSet<FontProjectGlyphIdentity>();
            foreach (Record record in document.Edits)
            {
                try
                {
                    var edit = new FontProjectEdit(new(record.ByteValue), record.BitmapBase64, record.Scope, record.RuntimeKind);
                    ValidateEdit(project.GameProfile, edit);
                    if (!identities.Add(edit.Identity)) return new(FontProjectLoadStatus.DuplicateGlyph, Detail: $"0x{edit.Identity.ByteValue:X2}");
                    edits.Add(edit);
                }
                catch (InvalidOperationException ex) { return new(FontProjectLoadStatus.ProtectedGlyph, Detail: ex.Message); }
                catch (ArgumentException ex) { return new(FontProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
            }
            return new(FontProjectLoadStatus.Success, new(document.GameId, Sort(edits)));
        }
        catch (JsonException ex) { return new(FontProjectLoadStatus.InvalidJson, Detail: ex.Message); }
        catch (IOException ex) { return new(FontProjectLoadStatus.InvalidRecord, Detail: ex.Message); }
    }

    private FontProjectSaveResult SavePath(ProjectContext project, FontProjectState state, string path)
    {
        ValidateState(project, state);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            var document = new Document(SchemaVersion, state.GameId, state.Edits.Select(edit => new Record(edit.Identity.ByteValue, edit.BitmapBase64, edit.Scope, edit.RuntimeKind)).ToArray());
            string temp = path + ".tmp";
            try { File.WriteAllText(temp, JsonSerializer.Serialize(document, JsonOptions)); File.Move(temp, path, true); }
            finally { if (File.Exists(temp)) File.Delete(temp); }
            return new(true, path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException) { return new(false, path, ex.Message); }
    }

    private static void ValidateState(ProjectContext project, FontProjectState? state)
    {
        if (state is null || !state.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal)) throw new ArgumentException("Font project state belongs to another game.", nameof(state));
        if (state.Edits.GroupBy(edit => edit.Identity).Any(group => group.Count() != 1)) throw new ArgumentException("Font project state contains duplicate glyph identities.", nameof(state));
        foreach (FontProjectEdit edit in state.Edits) ValidateEdit(project.GameProfile, edit);
    }
    private static void ValidateEdit(ElviraGameProfile game, FontProjectEdit edit)
    {
        if (edit.Identity.ByteValue == FontSlotMetadata.HudEraseGlyph) throw new InvalidOperationException("Glyph 0x81 is engine-reserved and cannot be a font project edit.");
        if (edit.Scope == FontEditScope.RuntimeSpecific && edit.RuntimeKind is not null && !IsRuntimeForGame(game, edit.RuntimeKind.Value))
            throw new ArgumentException("Font edit runtime is incompatible with the project game profile.", nameof(edit));
    }
    private static bool IsRuntimeForGame(ElviraGameProfile game, VariantRuntimeKind runtime) => (game, runtime) switch
    {
        (ElviraGameProfile.Elvira1, VariantRuntimeKind.Elvira1Vga or VariantRuntimeKind.Elvira1Ega) => true,
        (ElviraGameProfile.Elvira2, VariantRuntimeKind.Elvira2Vga) => true,
        _ => false
    };
    private static IReadOnlyList<FontProjectEdit> Sort(IEnumerable<FontProjectEdit> values) => values.OrderBy(value => value.Identity.ByteValue).ToArray();
    private static ProjectContext Require(ProjectContext? project) => project ?? throw new ArgumentNullException(nameof(project));
}
