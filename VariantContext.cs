namespace ElviraVgaEditor;

/// <summary>Stable semantic identities for the built-in runtime variants.</summary>
internal enum BuiltInVariantId { Elvira1Vga, Elvira1Ega, Elvira2Vga }

/// <summary>
/// A closed game-aware runtime discriminator. Its values make impossible
/// combinations such as Elvira II + RUNEGA unrepresentable as a valid catalog
/// definition.
/// </summary>
internal enum VariantRuntimeKind { Elvira1Vga, Elvira1Ega, Elvira2Vga }

/// <summary>
/// Immutable runtime/build selection belonging to one immutable project. It is
/// only identity and metadata: no path is created and no game file is touched.
/// </summary>
internal sealed class VariantContext
{
    internal VariantContext(
        ProjectContext project,
        BuiltInVariantId variantId,
        VariantRuntimeKind runtimeKind,
        string displayName,
        string sourceExecutableName,
        string generatedExecutableName,
        string logicalDataFileName,
        string directoryKey,
        bool enabled = true)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
        VariantId = variantId;
        RuntimeKind = runtimeKind;
        DisplayName = RequireDisplayName(displayName);
        SourceExecutableName = RequireDosFile(sourceExecutableName, nameof(sourceExecutableName));
        GeneratedExecutableName = RequireDosFile(generatedExecutableName, nameof(generatedExecutableName));
        LogicalDataFileName = RequireDosFile(logicalDataFileName, nameof(logicalDataFileName));
        DirectoryKey = ValidateDirectoryKey(directoryKey);
        Enabled = enabled;
        ValidateCompatibility();
    }

    public ProjectContext Project { get; }
    public BuiltInVariantId VariantId { get; }
    public VariantRuntimeKind RuntimeKind { get; }
    public ElviraGameProfile GameProfile => Project.GameProfile;
    public string DisplayName { get; }
    public string SourceExecutableName { get; }
    public string GeneratedExecutableName { get; }

    // This is a logical project-state role, not a legacy VariantEntry identity.
    // Both Elvira I runtime contexts intentionally point to the same GAMEPC role.
    public string LogicalDataFileName { get; }
    public string DirectoryKey { get; }
    public bool Enabled { get; }

    /// <summary>Future R6F path calculation only; it never creates this directory.</summary>
    public string FutureVariantRoot => Project.StorageLayout.GetFutureVariantRoot(DirectoryKey);

    internal static string ValidateDirectoryKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || key != key.Trim() || key.EndsWith('.'))
            throw new ArgumentException("Variant directory key must not contain leading/trailing spaces or a trailing dot.", nameof(key));
        string normalized = EditorStorageLayout.NormalizeVariantDirectoryName(key).ToUpperInvariant();
        if (normalized.Length is < 1 or > 8 || !normalized.All(character => char.IsLetterOrDigit(character) || character == '_'))
            throw new ArgumentException("Variant directory key must be a 1-8 character DOS-safe component.", nameof(key));
        return normalized;
    }

    private void ValidateCompatibility()
    {
        bool valid = (Project.GameProfile, VariantId, RuntimeKind) switch
        {
            (ElviraGameProfile.Elvira1, BuiltInVariantId.Elvira1Vga, VariantRuntimeKind.Elvira1Vga) =>
                SourceExecutableName == Elvira1ProductionProfile.ActiveVgaExecutable && GeneratedExecutableName == Elvira1ProductionProfile.GeneratedSlovakVgaExecutable,
            (ElviraGameProfile.Elvira1, BuiltInVariantId.Elvira1Ega, VariantRuntimeKind.Elvira1Ega) =>
                SourceExecutableName == Elvira1ProductionProfile.ActiveEgaExecutable && GeneratedExecutableName == Elvira1ProductionProfile.GeneratedSlovakEgaExecutable,
            (ElviraGameProfile.Elvira2, BuiltInVariantId.Elvira2Vga, VariantRuntimeKind.Elvira2Vga) =>
                SourceExecutableName == Elvira2ProductionProfile.ActiveExecutable && GeneratedExecutableName == Elvira2ProductionProfile.GeneratedSlovakExecutable,
            _ => false
        };
        if (!valid || !LogicalDataFileName.Equals("GAMEPC", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Variant runtime is incompatible with its project game profile.");
    }

    private static string RequireDisplayName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Variant display name is required.", nameof(value));
        return value.Trim();
    }

    private static string RequireDosFile(string value, string parameterName)
    {
        string file = Path.GetFileName(value ?? string.Empty).ToUpperInvariant();
        if (!file.Equals(value, StringComparison.OrdinalIgnoreCase) || !GameDataFileService.IsDos83FileName(file))
            throw new ArgumentException("Variant filename must be DOS 8.3 safe.", parameterName);
        return file;
    }
}

/// <summary>Immutable caller-owned active selection; there is no global current variant.</summary>
internal sealed record ActiveVariantSelection
{
    public ActiveVariantSelection(ProjectContext project, VariantContext variant)
    {
        Project = project ?? throw new ArgumentNullException(nameof(project));
        Variant = variant ?? throw new ArgumentNullException(nameof(variant));
        if (!ReferenceEquals(Project, Variant.Project))
            throw new ArgumentException("A variant selection must belong to the exact ProjectContext instance.");
    }

    public ProjectContext Project { get; }
    public VariantContext Variant { get; }
}

/// <summary>Authoritative predefined runtime catalog. It never reads ELVIRA_MODS.INI.</summary>
internal static class VariantContextCatalog
{
    public static IReadOnlyList<VariantContext> CreateBuiltIns(ProjectContext project) => project.GameProfile switch
    {
        ElviraGameProfile.Elvira1 => [Create(project, BuiltInVariantId.Elvira1Vga), Create(project, BuiltInVariantId.Elvira1Ega)],
        ElviraGameProfile.Elvira2 => [Create(project, BuiltInVariantId.Elvira2Vga)],
        _ => throw new ArgumentException("A supported project context is required.", nameof(project))
    };

    public static VariantContext Create(ProjectContext project, BuiltInVariantId variantId)
    {
        if (project is null) throw new ArgumentNullException(nameof(project));
        return variantId switch
        {
            BuiltInVariantId.Elvira1Vga when project.GameProfile == ElviraGameProfile.Elvira1 => new(project, variantId, VariantRuntimeKind.Elvira1Vga,
                "Elvira I VGA", Elvira1ProductionProfile.ActiveVgaExecutable, Elvira1ProductionProfile.GeneratedSlovakVgaExecutable, "GAMEPC", "E1VGA"),
            BuiltInVariantId.Elvira1Ega when project.GameProfile == ElviraGameProfile.Elvira1 => new(project, variantId, VariantRuntimeKind.Elvira1Ega,
                "Elvira I EGA", Elvira1ProductionProfile.ActiveEgaExecutable, Elvira1ProductionProfile.GeneratedSlovakEgaExecutable, "GAMEPC", "E1EGA"),
            BuiltInVariantId.Elvira2Vga when project.GameProfile == ElviraGameProfile.Elvira2 => new(project, variantId, VariantRuntimeKind.Elvira2Vga,
                "Elvira II VGA", Elvira2ProductionProfile.ActiveExecutable, Elvira2ProductionProfile.GeneratedSlovakExecutable, "GAMEPC", "E2VGA"),
            _ => throw new ArgumentException("Variant identity is incompatible with this ProjectContext game profile.", nameof(variantId))
        };
    }
}
