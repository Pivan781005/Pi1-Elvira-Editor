namespace Pi1ElviraEditor;

/// <summary>
/// Immutable, read-only description of one opened physical game installation.
/// It intentionally has no mutable active-variant state. Variant selection is
/// represented separately so this installation/baseline context stays stable.
/// </summary>
internal sealed class ProjectContext
{
    internal ProjectContext(
        GameInstallation installation,
        EditorStorageLayout storageLayout,
        PristineManifest pristineManifest,
        BaselineValidationResult baselineValidation)
    {
        Installation = installation ?? throw new ArgumentNullException(nameof(installation));
        StorageLayout = storageLayout ?? throw new ArgumentNullException(nameof(storageLayout));
        PristineManifest = pristineManifest ?? throw new ArgumentNullException(nameof(pristineManifest));
        BaselineValidation = baselineValidation ?? throw new ArgumentNullException(nameof(baselineValidation));

        if (installation.Game is not ElviraGameProfile.Elvira1 and not ElviraGameProfile.Elvira2)
            throw new ArgumentException("A supported game profile is required.", nameof(installation));
        if (!storageLayout.GameRoot.Equals(installation.NormalizedPath, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Installation and storage roots must identify the same physical installation.", nameof(storageLayout));
        if (!pristineManifest.GameId.Equals(PristineManifestService.GameIdFor(installation.Game), StringComparison.Ordinal))
            throw new ArgumentException("The pristine manifest must match the resolved game profile.", nameof(pristineManifest));
        if (baselineValidation.Status != BaselineValidationStatus.MatchesBaseline)
            throw new ArgumentException("A ready project context requires a matching pristine baseline.", nameof(baselineValidation));
    }

    public GameInstallation Installation { get; }
    public string GameRoot => StorageLayout.GameRoot;
    public ElviraGameProfile GameProfile => Installation.Game;
    public GameProfileInfo GameProfileInfo => GameProfileInfo.For(GameProfile);
    public EditorStorageLayout StorageLayout { get; }
    public PristineManifest PristineManifest { get; }
    public string BaselineFingerprint => PristineManifest.BaselineFingerprint;
    public BaselineValidationResult BaselineValidation { get; }
    public string ProjectRoot => StorageLayout.ProjectRoot;

}

internal enum ProjectContextOpenStatus
{
    Success,
    InvalidGameRoot,
    GameNotRecognized,
    ManifestMissing,
    ManifestInvalid,
    UnsupportedManifestSchema,
    GameIdMismatch,
    BaselineMismatch,
    UnsupportedExecutableVersion
}

internal sealed record ProjectContextOpenResult(
    ProjectContextOpenStatus Status,
    ProjectContext? Context = null,
    string? Detail = null)
{
    public bool IsSuccess => Status == ProjectContextOpenStatus.Success && Context is not null;
}

/// <summary>
/// Opens only an already-valid installation. This loader never initializes or
/// repairs a baseline and never creates any editor-owned directory.
/// </summary>
internal sealed class ProjectContextLoader
{
    public ProjectContextOpenResult Open(string? gameRoot)
    {
        if (string.IsNullOrWhiteSpace(gameRoot))
            return new(ProjectContextOpenStatus.InvalidGameRoot, Detail: "A game root is required.");

        EditorStorageLayout layout;
        try { layout = new EditorStorageLayout(gameRoot); }
        catch (ArgumentException ex) { return new(ProjectContextOpenStatus.InvalidGameRoot, Detail: ex.Message); }
        if (!Directory.Exists(layout.GameRoot))
            return new(ProjectContextOpenStatus.InvalidGameRoot, Detail: "The game root does not exist.");

        if (!GameInstallationValidator.TryValidate(layout.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
            return ResolveUnsupportedInstallation(layout);

        var manifests = new PristineManifestService(layout, installation.Game);
        PristineManifestLoadResult loaded = PristineManifestService.LoadManifestForContext(manifests.ManifestPath);
        ProjectContextOpenResult? loadFailure = MapManifestLoadFailure(loaded);
        if (loadFailure is not null) return loadFailure;
        PristineManifest manifest = loaded.Manifest!;
        if (!manifest.GameId.Equals(PristineManifestService.GameIdFor(installation.Game), StringComparison.Ordinal))
            return new(ProjectContextOpenStatus.GameIdMismatch, Detail: "The pristine manifest belongs to another game profile.");

        BaselineValidationResult validation = manifests.ValidateBaseline();
        if (validation.Status == BaselineValidationStatus.UnsupportedExecutableVersion)
            return new(ProjectContextOpenStatus.UnsupportedExecutableVersion, Detail: "A critical executable is not the supported frozen version.");
        if (validation.Status != BaselineValidationStatus.MatchesBaseline)
        {
            BaselineValidationEntry? difference = validation.Entries.FirstOrDefault(entry => entry.Status != BaselineValidationStatus.MatchesBaseline);
            string detail = difference is null ? validation.Status.ToString() : $"{difference.Status}: {difference.RelativePath}";
            return new(ProjectContextOpenStatus.BaselineMismatch, Detail: detail);
        }
        if (!manifests.HasSupportedFrozenInputsReadOnly())
            return new(ProjectContextOpenStatus.UnsupportedExecutableVersion, Detail: "Critical executable or GAMEPC input is unsupported.");

        return new(ProjectContextOpenStatus.Success, new ProjectContext(installation, layout, manifest, validation));
    }

    private static ProjectContextOpenResult ResolveUnsupportedInstallation(EditorStorageLayout layout)
    {
        // A valid existing manifest plus the physical shape of its declared
        // installation distinguishes an unsupported executable from a random
        // folder, without ever accepting that manifest as an open context.
        PristineManifestLoadResult loaded = PristineManifestService.LoadManifestForContext(
            Path.Combine(layout.BaselineRoot, PristineManifestService.ManifestFileName));
        if (loaded.Status == PristineManifestLoadStatus.Success && loaded.Manifest is not null &&
            HasDeclaredGameInputs(layout.GameRoot, loaded.Manifest.GameId))
            return new(ProjectContextOpenStatus.UnsupportedExecutableVersion, Detail: "The declared installation has an unsupported executable version.");
        return new(ProjectContextOpenStatus.GameNotRecognized, Detail: "No supported Elvira installation was recognized.");
    }

    private static bool HasDeclaredGameInputs(string root, string gameId) =>
        File.Exists(Path.Combine(root, "GAMEPC")) &&
        Directory.EnumerateFiles(root, "*2.VGA", SearchOption.TopDirectoryOnly).Any() &&
        (gameId == "Elvira1"
            ? File.Exists(Path.Combine(root, Elvira1ProductionProfile.ActiveVgaExecutable)) && File.Exists(Path.Combine(root, Elvira1ProductionProfile.ActiveEgaExecutable))
            : gameId == "Elvira2" && File.Exists(Path.Combine(root, Elvira2ProductionProfile.ActiveExecutable)));

    private static ProjectContextOpenResult? MapManifestLoadFailure(PristineManifestLoadResult result) => result.Status switch
    {
        PristineManifestLoadStatus.Success => null,
        PristineManifestLoadStatus.Missing => new(ProjectContextOpenStatus.ManifestMissing),
        PristineManifestLoadStatus.UnsupportedSchema => new(ProjectContextOpenStatus.UnsupportedManifestSchema),
        _ => new(ProjectContextOpenStatus.ManifestInvalid)
    };
}
