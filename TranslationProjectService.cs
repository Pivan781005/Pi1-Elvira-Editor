using System.Text.Json;

namespace ElviraVgaEditor;

/// <summary>
/// Project-owned Game Text translation state.  It deliberately stores logical
/// edits rather than GAMEPC clones: the installation's GAMEPC remains the
/// immutable read-only source and runnable data is materialized only in an
/// already-owned disposable variant directory.
/// </summary>
internal sealed record TranslationProjectVariant(
    string DisplayName,
    string Code,
    string DataFile,
    string ExeFile,
    IReadOnlyDictionary<int, string> Edits);

internal sealed record TranslationProjectState(string GameId, IReadOnlyList<TranslationProjectVariant> Variants)
{
    internal static TranslationProjectState Empty(ElviraGameProfile game) => new(PristineManifestService.GameIdFor(game), []);
}

internal enum TranslationProjectLoadStatus { Success, InvalidDocument }
internal sealed record TranslationProjectLoadResult(TranslationProjectLoadStatus Status, TranslationProjectState? State = null, string? Detail = null)
{
    internal bool IsSuccess => Status == TranslationProjectLoadStatus.Success && State is not null;
}

internal sealed class TranslationProjectService
{
    internal const string FileName = "text-translations.json";
    private const int SchemaVersion = 1;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private sealed record PersistedDocument(int SchemaVersion, string GameId, IReadOnlyList<PersistedVariant> Variants);
    private sealed record PersistedVariant(string DisplayName, string Code, string DataFile, string ExeFile, IReadOnlyList<PersistedEdit> Edits);
    private sealed record PersistedEdit(int Index, string Text);

    internal string GetPath(ProjectContext project) => Path.Combine(RequireProject(project).ProjectRoot, FileName);

    internal TranslationProjectLoadResult Load(ProjectContext project)
    {
        project = RequireProject(project);
        string path = GetPath(project);
        if (!File.Exists(path)) return new(TranslationProjectLoadStatus.Success, TranslationProjectState.Empty(project.GameProfile));
        try
        {
            PersistedDocument? document = JsonSerializer.Deserialize<PersistedDocument>(File.ReadAllText(path), JsonOptions);
            if (document is null || document.SchemaVersion != SchemaVersion ||
                !document.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal) ||
                document.Variants is null)
                return new(TranslationProjectLoadStatus.InvalidDocument, Detail: "Translation project data is invalid.");

            TranslationProjectVariant[] variants = document.Variants.Select(item => FromPersisted(project.GameProfile, item)).OrderBy(item => item.Code, StringComparer.Ordinal).ToArray();
            if (variants.GroupBy(item => item.Code, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() != 1))
                return new(TranslationProjectLoadStatus.InvalidDocument, Detail: "Translation project contains duplicate codes.");
            return new(TranslationProjectLoadStatus.Success, new(document.GameId, variants));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or InvalidOperationException or ArgumentException)
        {
            return new(TranslationProjectLoadStatus.InvalidDocument, Detail: ex.Message);
        }
    }

    internal TranslationProjectVariant Create(ProjectContext project, TranslationProjectState state, string displayName, string code, IReadOnlyDictionary<int, string> edits)
    {
        project = RequireProject(project); ValidateState(project, state);
        VariantEntry candidate = VariantNaming.Create(displayName, ValidateProjectCode(code, project.GameProfile), project.GameProfile, true, 1);
        if (state.Variants.Any(item => item.Code.Equals(candidate.Code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Translation code {candidate.Code} already exists in {FileName}.");

        // A pre-existing legacy root artifact is never adopted or overwritten.
        string rootData = Path.Combine(project.GameRoot, candidate.DataFile);
        string rootExe = Path.Combine(project.GameRoot, candidate.ExeFile);
        if (File.Exists(rootData) || File.Exists(rootExe))
            throw new IOException($"Translation output conflicts with existing installation file(s): " +
                string.Join(", ", new[] { rootData, rootExe }.Where(File.Exists).Select(Path.GetFileName)) + ".");

        return new TranslationProjectVariant(candidate.DisplayName, candidate.Code, candidate.DataFile, candidate.ExeFile, NormalizeEdits(edits));
    }

    internal TranslationProjectState Add(TranslationProjectState state, TranslationProjectVariant translation)
    {
        if (state.Variants.Any(item => item.Code.Equals(translation.Code, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Translation code {translation.Code} already exists in {FileName}.");
        return state with { Variants = state.Variants.Append(translation).OrderBy(item => item.Code, StringComparer.Ordinal).ToArray() };
    }

    internal TranslationProjectState UpsertWorkingEdits(ProjectContext project, TranslationProjectState state, string code, IReadOnlyDictionary<int, string> edits)
    {
        project = RequireProject(project); ValidateState(project, state);
        string normalizedCode = code.Equals("EN", StringComparison.OrdinalIgnoreCase) ? "EN" : ValidateProjectCode(code, project.GameProfile);
        TranslationProjectVariant? existing = state.Variants.SingleOrDefault(item => item.Code.Equals(normalizedCode, StringComparison.OrdinalIgnoreCase));
        TranslationProjectVariant replacement = existing ?? CreateWorkingVariant(project.GameProfile, normalizedCode);
        replacement = replacement with { Edits = NormalizeEdits(edits) };
        return state with
        {
            Variants = state.Variants.Where(item => !item.Code.Equals(normalizedCode, StringComparison.OrdinalIgnoreCase))
                .Append(replacement).OrderBy(item => item.Code, StringComparer.Ordinal).ToArray()
        };
    }

    internal void Save(ProjectContext project, TranslationProjectState state)
    {
        project = RequireProject(project); ValidateState(project, state);
        string path = GetPath(project);
        Directory.CreateDirectory(project.ProjectRoot);
        PersistedDocument document = new(SchemaVersion, state.GameId, state.Variants.OrderBy(item => item.Code, StringComparer.Ordinal).Select(ToPersisted).ToArray());
        string temporary = path + ".tmp";
        try
        {
            File.WriteAllText(temporary, JsonSerializer.Serialize(document, JsonOptions));
            File.Move(temporary, path, overwrite: true);
        }
        finally { try { if (File.Exists(temporary)) File.Delete(temporary); } catch { } }
    }

    /// <summary>Creates an explicit non-runnable data export below ProjectRoot.
    /// Save As must never use the pristine installation as its destination.</summary>
    internal string ExportProjectDataFile(ProjectContext project, string fileName, IReadOnlyDictionary<int, string> edits)
    {
        project = RequireProject(project);
        string name = Path.GetFileName(fileName ?? string.Empty).ToUpperInvariant();
        if (!GameDataFileService.IsDos83FileName(name)) throw new InvalidOperationException("Export name must be a DOS-compatible 8.3 filename.");
        string directory = Path.Combine(project.ProjectRoot, "Exports");
        string destination = Path.Combine(directory, name);
        if (File.Exists(destination)) throw new IOException($"Project export already exists and will not be overwritten: {destination}");
        string source = Path.Combine(project.GameRoot, "GAMEPC");
        IReadOnlyList<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(source, project.GameProfile);
        byte[] data = GamePcTextEditor.BuildEditedData(source, edits, entries, GamePcTextEditor.GetEncoding("CP852"), project.GameProfile, entries);
        Directory.CreateDirectory(directory);
        string temporary = destination + ".tmp";
        try
        {
            File.WriteAllBytes(temporary, data);
            GamePcTextEditor.ValidateSerializedData(temporary, entries.Count, project.GameProfile);
            File.Move(temporary, destination);
            return destination;
        }
        finally { try { if (File.Exists(temporary)) File.Delete(temporary); } catch { } }
    }

    /// <summary>Explicit build-only projection.  The destination is validated as the
    /// exact editor-owned runtime directory; no installation-root file is ever written.</summary>
    internal string MaterializeOwnedVariantDataFile(ProjectContext project, VariantContext runtime, VariantDirectoryService directories, TranslationProjectVariant translation)
    {
        project = RequireProject(project);
        if (runtime is null || !ReferenceEquals(runtime.Project, project)) throw new ArgumentException("Runtime must belong to the project.", nameof(runtime));
        if (directories.ValidateOwnedVariantDirectory(project, runtime).Status != VariantDirectoryOperationStatus.AlreadyValid)
            throw new InvalidOperationException("The target runtime directory is not editor-owned and ready.");
        string source = Path.Combine(project.GameRoot, runtime.LogicalDataFileName);
        IReadOnlyList<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(source, project.GameProfile);
        byte[] data = GamePcTextEditor.BuildEditedData(source, translation.Edits, entries, GamePcTextEditor.GetEncoding("CP852"), project.GameProfile, entries);
        string destination = Path.Combine(directories.GetVariantDirectoryPath(project, runtime), translation.DataFile);
        if (File.Exists(destination)) throw new IOException($"Generated translation output already exists: {destination}");
        string temporary = destination + ".tmp";
        try
        {
            File.WriteAllBytes(temporary, data);
            GamePcTextEditor.ValidateSerializedData(temporary, entries.Count, project.GameProfile);
            File.Move(temporary, destination);
            return destination;
        }
        finally { try { if (File.Exists(temporary)) File.Delete(temporary); } catch { } }
    }

    internal static int GetMaximumCodeLength(ElviraGameProfile game)
    {
        string exeStem = game == ElviraGameProfile.Elvira2 ? "RUNIT" : "RUNVGA";
        return Math.Min(8 - "GAMEPC".Length, 8 - exeStem.Length);
    }

    internal static string ValidateProjectCode(string? code, ElviraGameProfile game)
    {
        string value = (code ?? string.Empty).Trim().ToUpperInvariant();
        int maximum = GetMaximumCodeLength(game);
        if (value.Length != maximum || !value.All(value => value is >= 'A' and <= 'Z' or >= '0' and <= '9'))
            throw new InvalidOperationException($"Translation code must contain exactly {maximum} ASCII letters or digits.");
        return value;
    }

    private static TranslationProjectVariant CreateWorkingVariant(ElviraGameProfile game, string code)
    {
        VariantEntry entry = code == "EN" ? VariantNaming.Create("English", "EN", game, true, 1) : VariantNaming.Create(code, code, game, true, 1);
        return new(entry.DisplayName, entry.Code, entry.DataFile, entry.ExeFile, new Dictionary<int, string>());
    }

    private static TranslationProjectVariant FromPersisted(ElviraGameProfile game, PersistedVariant item)
    {
        string code = item.Code.Equals("EN", StringComparison.OrdinalIgnoreCase) ? "EN" : ValidateProjectCode(item.Code, game);
        VariantEntry expected = VariantNaming.Create(item.DisplayName, code, game, true, 1);
        if (!item.DataFile.Equals(expected.DataFile, StringComparison.OrdinalIgnoreCase) || !item.ExeFile.Equals(expected.ExeFile, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Translation project filenames do not match its semantic code.");
        if (item.Edits is null || item.Edits.GroupBy(edit => edit.Index).Any(group => group.Count() != 1) || item.Edits.Any(edit => edit.Index < 0 || edit.Text is null))
            throw new InvalidOperationException("Translation project edits are invalid.");
        return new(expected.DisplayName, expected.Code, expected.DataFile, expected.ExeFile,
            item.Edits.OrderBy(edit => edit.Index).ToDictionary(edit => edit.Index, edit => edit.Text));
    }

    private static PersistedVariant ToPersisted(TranslationProjectVariant item) => new(item.DisplayName, item.Code, item.DataFile, item.ExeFile,
        NormalizeEdits(item.Edits).OrderBy(edit => edit.Key).Select(edit => new PersistedEdit(edit.Key, edit.Value)).ToArray());

    private static IReadOnlyDictionary<int, string> NormalizeEdits(IReadOnlyDictionary<int, string>? edits) =>
        (edits ?? new Dictionary<int, string>()).OrderBy(edit => edit.Key).ToDictionary(edit => edit.Key, edit => edit.Value ?? throw new ArgumentException("Translation text must be non-null.", nameof(edits)));

    private static void ValidateState(ProjectContext project, TranslationProjectState? state)
    {
        if (state is null || !state.GameId.Equals(PristineManifestService.GameIdFor(project.GameProfile), StringComparison.Ordinal))
            throw new ArgumentException("Translation project state does not belong to this game.", nameof(state));
    }

    private static ProjectContext RequireProject(ProjectContext? project) => project ?? throw new ArgumentNullException(nameof(project));
}
