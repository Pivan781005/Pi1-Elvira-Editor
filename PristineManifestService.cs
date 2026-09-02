using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ElviraVgaEditor;

internal enum PristineManifestInitializationStatus { Initialized, AlreadyValid, BaselineConflict, UnsupportedExecutableVersion, InvalidSource }
internal enum BaselineValidationStatus { MatchesBaseline, ExternalInstallationChange, MissingPristineFile, UnexpectedFile, CurrentDiffersFromBaseline, ManifestMissing, ManifestInvalid, UnsupportedSchema, UnsupportedExecutableVersion }

internal sealed record PristineManifestFile(string RelativePath, long Size, string Sha256, PristineFileClassification Classification, PristineFileRole Role);
internal sealed record PristineManifest(int SchemaVersion, string GameId, string Distribution, string BaselineFingerprint, IReadOnlyList<PristineManifestFile> Files);
internal sealed record PristineManifestInitializationResult(PristineManifestInitializationStatus Status, string ManifestPath, PristineManifest? Manifest = null);
internal sealed record BaselineValidationEntry(string RelativePath, BaselineValidationStatus Status);
internal sealed record BaselineValidationResult(BaselineValidationStatus Status, IReadOnlyList<BaselineValidationEntry> Entries);
internal enum PristineManifestLoadStatus { Success, Missing, ManifestInvalid, UnsupportedSchema }
internal sealed record PristineManifestLoadResult(PristineManifestLoadStatus Status, PristineManifest? Manifest = null);

/// <summary>Explicit persistent baseline manifest and exact mutable-backup foundation. No production service calls it yet.</summary>
internal sealed class PristineManifestService
{
    internal const int Schema = 1;
    internal const string ManifestFileName = "pristine-manifest.json";
    private readonly EditorStorageLayout _layout;
    private readonly ElviraGameProfile _game;

    public PristineManifestService(EditorStorageLayout layout, ElviraGameProfile game)
    {
        _layout = layout ?? throw new ArgumentNullException(nameof(layout));
        _game = game is ElviraGameProfile.Elvira1 or ElviraGameProfile.Elvira2 ? game : throw new ArgumentException("A supported game is required.", nameof(game));
    }

    public string ManifestPath => Path.Combine(_layout.BaselineRoot, ManifestFileName);

    public PristineManifestInitializationResult InitializeBaseline(bool validateFrozenInputs = true)
    {
        if (File.Exists(ManifestPath))
        {
            BaselineValidationResult validation = ValidateBaseline();
            return new(validation.Status == BaselineValidationStatus.MatchesBaseline ? PristineManifestInitializationStatus.AlreadyValid : PristineManifestInitializationStatus.BaselineConflict, ManifestPath);
        }

        IReadOnlyList<PristineInstallationFile> inventory = new PristineInstallationService(_layout).EnumerateReadOnly();
        if (validateFrozenInputs && !HasSupportedFrozenInputs(inventory))
            return new(PristineManifestInitializationStatus.UnsupportedExecutableVersion, ManifestPath);
        PristineManifest manifest = BuildManifest(inventory);
        var createdBackups = new List<string>();
        try
        {
            Directory.CreateDirectory(_layout.BaselineRoot);
            foreach (PristineInstallationFile file in inventory.Where(file => file.Classification == PristineFileClassification.MutableBackedUp))
                BackupMutable(file, createdBackups);
            WriteManifestAtomic(manifest);
            return new(PristineManifestInitializationStatus.Initialized, ManifestPath, manifest);
        }
        catch (IOException) when (File.Exists(ManifestPath))
        {
            return new(PristineManifestInitializationStatus.BaselineConflict, ManifestPath);
        }
        catch
        {
            foreach (string path in createdBackups) if (File.Exists(path)) File.Delete(path);
            throw;
        }
    }

    /// <summary>Explicit metadata-only recapture for an accepted classification-policy correction.</summary>
    public PristineManifestInitializationResult RecaptureBaselineForMetadataCorrection()
    {
        if (ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
            return new(PristineManifestInitializationStatus.BaselineConflict, ManifestPath);
        IReadOnlyList<PristineInstallationFile> inventory = new PristineInstallationService(_layout).EnumerateReadOnly();
        if (!HasSupportedFrozenInputs(inventory)) return new(PristineManifestInitializationStatus.UnsupportedExecutableVersion, ManifestPath);
        PristineManifest manifest = BuildManifest(inventory);
        var createdBackups = new List<string>();
        string temporaryManifest = Path.Combine(_layout.BaselineRoot, ".recapture." + Guid.NewGuid().ToString("N") + ".tmp");
        string previousManifest = Path.Combine(_layout.BaselineRoot, ".previous." + Guid.NewGuid().ToString("N") + ".json");
        bool committed = false;
        try
        {
            foreach (PristineInstallationFile file in inventory.Where(file => file.Classification == PristineFileClassification.MutableBackedUp)) BackupMutable(file, createdBackups);
            WriteManifestTemporary(manifest, temporaryManifest);
            File.Move(ManifestPath, previousManifest, false);
            try { File.Move(temporaryManifest, ManifestPath, false); }
            catch { File.Move(previousManifest, ManifestPath, false); throw; }
            File.Delete(previousManifest);
            committed = true;
            return new(PristineManifestInitializationStatus.Initialized, ManifestPath, manifest);
        }
        catch
        {
            foreach (string path in createdBackups) if (File.Exists(path)) File.Delete(path);
            if (!committed && File.Exists(previousManifest) && !File.Exists(ManifestPath)) File.Move(previousManifest, ManifestPath, false);
            throw;
        }
        finally
        {
            if (File.Exists(temporaryManifest)) File.Delete(temporaryManifest);
            if (committed && File.Exists(previousManifest)) File.Delete(previousManifest);
        }
    }

    public BaselineValidationResult ValidateBaseline()
    {
        if (!File.Exists(ManifestPath)) return new(BaselineValidationStatus.ManifestMissing, []);
        PristineManifest manifest;
        try { manifest = LoadManifest(ManifestPath, _game); }
        catch (UnsupportedSchemaException) { return new(BaselineValidationStatus.UnsupportedSchema, []); }
        catch { return new(BaselineValidationStatus.ManifestInvalid, []); }

        IReadOnlyList<PristineInstallationFile> current = new PristineInstallationService(_layout).EnumerateReadOnly();
        var actual = current.Where(file => file.Classification != PristineFileClassification.GeneratedIgnored).ToDictionary(file => file.RelativePath, StringComparer.OrdinalIgnoreCase);
        var entries = new List<BaselineValidationEntry>();
        foreach (PristineManifestFile expected in manifest.Files)
        {
            if (!actual.Remove(expected.RelativePath, out PristineInstallationFile? found)) { entries.Add(new(expected.RelativePath, BaselineValidationStatus.MissingPristineFile)); continue; }
            if (found.Size != expected.Size || !found.Sha256.Equals(expected.Sha256, StringComparison.OrdinalIgnoreCase))
            {
                BaselineValidationStatus status = expected.Classification == PristineFileClassification.MutableBackedUp
                    ? BaselineValidationStatus.CurrentDiffersFromBaseline
                    : IsCriticalExecutable(expected.RelativePath) ? BaselineValidationStatus.UnsupportedExecutableVersion : BaselineValidationStatus.ExternalInstallationChange;
                entries.Add(new(expected.RelativePath, status));
            }
            else entries.Add(new(expected.RelativePath, BaselineValidationStatus.MatchesBaseline));
        }
        entries.AddRange(actual.Values.Select(file => new BaselineValidationEntry(file.RelativePath, BaselineValidationStatus.UnexpectedFile)));
        BaselineValidationStatus aggregate = entries.All(item => item.Status == BaselineValidationStatus.MatchesBaseline)
            ? BaselineValidationStatus.MatchesBaseline : entries.First(item => item.Status != BaselineValidationStatus.MatchesBaseline).Status;
        return new(aggregate, entries.OrderBy(item => item.RelativePath, StringComparer.OrdinalIgnoreCase).ToArray());
    }

    /// <summary>Read-only manifest parser for ProjectContext loading. It does not write or adopt anything.</summary>
    internal static PristineManifestLoadResult LoadManifestForContext(string path)
    {
        if (!File.Exists(path)) return new(PristineManifestLoadStatus.Missing);
        try
        {
            PristineManifest? manifest = JsonSerializer.Deserialize<PristineManifest>(File.ReadAllText(path));
            if (manifest is null) return new(PristineManifestLoadStatus.ManifestInvalid);
            if (manifest.SchemaVersion != Schema) return new(PristineManifestLoadStatus.UnsupportedSchema);
            if (manifest.GameId is not "Elvira1" and not "Elvira2" || manifest.Distribution != "Unknown")
                return new(PristineManifestLoadStatus.ManifestInvalid);
            ValidateFiles(manifest.Files);
            if (!manifest.BaselineFingerprint.Equals(Fingerprint(manifest.Files), StringComparison.OrdinalIgnoreCase))
                return new(PristineManifestLoadStatus.ManifestInvalid);
            return new(PristineManifestLoadStatus.Success, manifest);
        }
        catch (JsonException) { return new(PristineManifestLoadStatus.ManifestInvalid); }
        catch (IOException) { return new(PristineManifestLoadStatus.ManifestInvalid); }
        catch (InvalidDataException) { return new(PristineManifestLoadStatus.ManifestInvalid); }
    }

    public static PristineManifest BuildManifest(IEnumerable<PristineInstallationFile> inventory)
    {
        PristineManifestFile[] files = inventory.Where(file => file.Classification != PristineFileClassification.GeneratedIgnored)
            .Select(file => new PristineManifestFile(file.RelativePath, file.Size, file.Sha256, file.Classification, file.Role))
            .OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase).ToArray();
        ValidateFiles(files);
        string game = "Unspecified"; // caller replaces this through WithGame below.
        return new(Schema, game, "Unknown", Fingerprint(files), files);
    }

    public static PristineManifest LoadManifest(string path, ElviraGameProfile expectedGame)
    {
        PristineManifestLoadResult result = LoadManifestForContext(path);
        if (result.Status == PristineManifestLoadStatus.UnsupportedSchema) throw new UnsupportedSchemaException();
        if (result.Status != PristineManifestLoadStatus.Success || result.Manifest is null) throw new InvalidDataException("Manifest is invalid.");
        if (result.Manifest.GameId != GameId(expectedGame)) throw new InvalidDataException("Manifest game/distribution is invalid.");
        return result.Manifest;
    }

    internal bool HasSupportedFrozenInputsReadOnly() =>
        HasSupportedFrozenInputs(new PristineInstallationService(_layout).EnumerateReadOnly());

    private PristineManifest BuildManifest(IReadOnlyList<PristineInstallationFile> inventory)
    {
        PristineManifest draft = BuildManifest((IEnumerable<PristineInstallationFile>)inventory);
        return draft with { GameId = GameId(_game) };
    }

    private void BackupMutable(PristineInstallationFile file, ICollection<string> created)
    {
        string path = SafeMutablePath(file.RelativePath); string? parent = Path.GetDirectoryName(path);
        if (File.Exists(path))
        {
            if (new FileInfo(path).Length == file.Size && Hash(path).Equals(file.Sha256, StringComparison.OrdinalIgnoreCase)) return;
            throw new IOException("Mutable baseline backup conflicts with existing bytes.");
        }
        Directory.CreateDirectory(parent!);
        string temp = path + ".tmp." + Guid.NewGuid().ToString("N");
        try { File.Copy(file.AbsolutePath, temp, false); if (Hash(temp) != file.Sha256) throw new IOException("Mutable backup hash mismatch."); File.Move(temp, path, false); created.Add(path); }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }

    private void WriteManifestAtomic(PristineManifest manifest)
    {
        string temp = Path.Combine(_layout.BaselineRoot, ".manifest." + Guid.NewGuid().ToString("N") + ".tmp");
        try
        {
            WriteManifestTemporary(manifest, temp);
            File.Move(temp, ManifestPath, false);
        }
        finally { if (File.Exists(temp)) File.Delete(temp); }
    }

    private void WriteManifestTemporary(PristineManifest manifest, string path)
    {
        File.WriteAllText(path, JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true }));
        _ = LoadManifest(path, _game);
    }

    private bool HasSupportedFrozenInputs(IReadOnlyList<PristineInstallationFile> files)
    {
        string gamePcHash = _game == ElviraGameProfile.Elvira1 ? Elvira1ProductionProfile.GamePc.OriginalSha256 : Elvira2ProductionProfile.GamePc.OriginalSha256;
        if (!files.Any(file => file.RelativePath.Equals("GAMEPC", StringComparison.OrdinalIgnoreCase) && file.Sha256.Equals(gamePcHash, StringComparison.OrdinalIgnoreCase))) return false;
        string[] executables = _game == ElviraGameProfile.Elvira1 ? ["RUNVGA.EXE", "RUNEGA.EXE"] : ["RUNIT.EXE"];
        return executables.All(exe => files.Any(file => file.RelativePath.Equals(exe, StringComparison.OrdinalIgnoreCase) && PristineInstallationService.IsSupportedFrozenExecutable(file.RelativePath, file.Sha256)));
    }

    private string SafeMutablePath(string relative)
    {
        ValidateRelative(relative);
        string result = Path.GetFullPath(Path.Combine(_layout.MutableBackupsRoot, relative));
        if (!result.StartsWith(_layout.MutableBackupsRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Mutable backup escaped its root.");
        return result;
    }

    private static void ValidateFiles(IEnumerable<PristineManifestFile> files)
    {
        PristineManifestFile[] entries = files.ToArray();
        foreach (PristineManifestFile file in entries)
        {
            ValidateRelative(file.RelativePath);
            if (file.Size < 0 || file.Sha256.Length != 64 || !file.Sha256.All(Uri.IsHexDigit) || !Enum.IsDefined(file.Classification) || !Enum.IsDefined(file.Role)) throw new InvalidDataException("Manifest file entry is invalid.");
        }
        if (entries.Select(file => file.RelativePath).Distinct(StringComparer.OrdinalIgnoreCase).Count() != entries.Length) throw new InvalidDataException("Manifest contains duplicate Windows paths.");
    }

    private static void ValidateRelative(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || Path.IsPathRooted(path) || path.Split(['\\', '/'], StringSplitOptions.RemoveEmptyEntries).Any(part => part is "." or "..")) throw new InvalidDataException("Manifest path is unsafe.");
    }

    private static string Fingerprint(IEnumerable<PristineManifestFile> files)
    {
        string content = string.Join("\n", files.OrderBy(file => file.RelativePath, StringComparer.OrdinalIgnoreCase)
            .Select(file => $"{file.RelativePath}\u001F{file.Classification}\u001F{file.Role}\u001F{file.Size}\u001F{file.Sha256}"));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
    }

    internal static string GameIdFor(ElviraGameProfile game) => GameId(game);
    private static string GameId(ElviraGameProfile game) => game == ElviraGameProfile.Elvira1 ? "Elvira1" : game == ElviraGameProfile.Elvira2 ? "Elvira2" : throw new InvalidDataException("Invalid game identity.");
    private static bool IsCriticalExecutable(string path) => path.Equals("RUNVGA.EXE", StringComparison.OrdinalIgnoreCase) || path.Equals("RUNEGA.EXE", StringComparison.OrdinalIgnoreCase) || path.Equals("RUNIT.EXE", StringComparison.OrdinalIgnoreCase);
    private static string Hash(string path) { using FileStream stream = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(stream)); }
    private sealed class UnsupportedSchemaException : Exception;
}
