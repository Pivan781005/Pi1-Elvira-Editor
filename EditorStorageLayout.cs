namespace ElviraVgaEditor;

/// <summary>
/// Installation-local, editor-owned storage paths. Construction is pure: it
/// validates and calculates paths only. Later R6 phases own population,
/// manifests, project state, variants, and enforcement of immutability.
/// </summary>
internal sealed class EditorStorageLayout
{
    internal const string EditorDirectoryName = "ElviraEditor";
    internal const string BaselineDirectoryName = "Baseline";
    internal const string MutableDirectoryName = "Mutable";
    internal const string ProjectDirectoryName = "Project";
    internal const string LogsDirectoryName = "Logs";
    internal const string MetadataDirectoryName = "Metadata";
    internal const string VariantsDirectoryName = "VARIANTS";

    public EditorStorageLayout(string gameRoot)
    {
        GameRoot = NormalizeGameRoot(gameRoot);
        EditorRoot = CreateChild(GameRoot, EditorDirectoryName);
        BaselineRoot = CreateChild(EditorRoot, BaselineDirectoryName);
        MutableBackupsRoot = CreateChild(BaselineRoot, MutableDirectoryName);
        ProjectRoot = CreateChild(EditorRoot, ProjectDirectoryName);
        LogsRoot = CreateChild(EditorRoot, LogsDirectoryName);
        MetadataRoot = CreateChild(EditorRoot, MetadataDirectoryName);
        VariantsRoot = CreateChild(GameRoot, VariantsDirectoryName);
        ValidateDerivedRoots();
    }

    /// <summary>Game-owned / launcher-visible installation area.</summary>
    public string GameRoot { get; }

    /// <summary>Editor-owned persistent root beneath <see cref="GameRoot"/>.</summary>
    public string EditorRoot { get; }

    /// <summary>Future pristine-baseline metadata and mutable backups (R6C).</summary>
    public string BaselineRoot { get; }
    public string MutableBackupsRoot { get; }

    /// <summary>Future persistent editable project state (R6D).</summary>
    public string ProjectRoot { get; }

    /// <summary>Future disposable complete runnable builds (R6F/R6G).</summary>
    public string VariantsRoot { get; }

    /// <summary>Future editor/build/diagnostic logs.</summary>
    public string LogsRoot { get; }
    public string MetadataRoot { get; }

    /// <summary>
    /// The only mutating operation supplied by this path model. It is never
    /// called by construction or by any path getter.
    /// </summary>
    public void EnsureDirectories()
    {
        Directory.CreateDirectory(EditorRoot);
        Directory.CreateDirectory(BaselineRoot);
        Directory.CreateDirectory(MutableBackupsRoot);
        Directory.CreateDirectory(ProjectRoot);
        Directory.CreateDirectory(VariantsRoot);
        Directory.CreateDirectory(LogsRoot);
        Directory.CreateDirectory(MetadataRoot);
        Directory.CreateDirectory(VariantsRoot);
    }

    /// <summary>Validates a future variant directory component without creating it.</summary>
    public string GetFutureVariantRoot(string variantName) =>
        CreateChild(VariantsRoot, NormalizeVariantDirectoryName(variantName));

    internal static string NormalizeVariantDirectoryName(string variantName)
    {
        if (string.IsNullOrWhiteSpace(variantName))
            throw new ArgumentException("A variant directory name is required.", nameof(variantName));

        string normalized = variantName.Trim();
        if (normalized is "." or ".." || Path.IsPathRooted(normalized) ||
            normalized.IndexOf(Path.DirectorySeparatorChar) >= 0 ||
            normalized.IndexOf(Path.AltDirectorySeparatorChar) >= 0 ||
            normalized.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
            normalized[^1] is '.' or ' ')
            throw new ArgumentException("Variant directory name is not a safe Windows path component.", nameof(variantName));

        string deviceStem = normalized.Split('.', 2)[0];
        if (IsReservedWindowsDeviceName(deviceStem))
            throw new ArgumentException("Variant directory name is a reserved Windows device name.", nameof(variantName));
        return normalized;
    }

    internal static void VerifySmokeInvariants()
    {
        VerifyLayout(@"C:\Games\GOG\Elvira", @"C:\Games\GOG\Elvira\ElviraEditor", @"C:\Games\GOG\Elvira\VARIANTS");
        VerifyLayout(@"C:\Games\GOG\Elvira II\", @"C:\Games\GOG\Elvira II\ElviraEditor", @"C:\Games\GOG\Elvira II\VARIANTS");

        string neverCreated = Path.Combine(Path.GetTempPath(), "Pi1ElviraStorageLayoutSmoke", Guid.NewGuid().ToString("N"));
        if (Directory.Exists(neverCreated)) throw new InvalidDataException("Storage smoke test root unexpectedly exists.");
        _ = new EditorStorageLayout(neverCreated);
        if (Directory.Exists(neverCreated)) throw new InvalidDataException("Pure storage-layout construction created a directory.");

        AssertRejected(() => new EditorStorageLayout("relative-game-root"));
        AssertRejected(() => new EditorStorageLayout(@"C:\Games\GOG\Elvira\..\Other"));
        AssertRejected(() => new EditorStorageLayout(""));
        AssertRejected(() => new EditorStorageLayout("\0"));
        foreach (string unsafeName in new[] { ".", "..", "nested\\name", "C:\\rooted", "CON", "variant." })
            AssertRejected(() => NormalizeVariantDirectoryName(unsafeName));
        if (NormalizeVariantDirectoryName("Slovak CP852") != "Slovak CP852")
            throw new InvalidDataException("Safe variant directory normalization changed its deterministic representation.");
    }

    private static void VerifyLayout(string gameRoot, string expectedEditorRoot, string expectedVariantsRoot)
    {
        var layout = new EditorStorageLayout(gameRoot);
        if (!layout.EditorRoot.Equals(expectedEditorRoot, StringComparison.OrdinalIgnoreCase) ||
            layout.BaselineRoot != Path.Combine(expectedEditorRoot, BaselineDirectoryName) ||
            layout.MutableBackupsRoot != Path.Combine(expectedEditorRoot, BaselineDirectoryName, MutableDirectoryName) ||
            layout.ProjectRoot != Path.Combine(expectedEditorRoot, ProjectDirectoryName) ||
            layout.VariantsRoot != expectedVariantsRoot ||
            layout.LogsRoot != Path.Combine(expectedEditorRoot, LogsDirectoryName) ||
            layout.MetadataRoot != Path.Combine(expectedEditorRoot, MetadataDirectoryName))
            throw new InvalidDataException("Storage layout did not derive the expected deterministic roots.");
    }

    private static string NormalizeGameRoot(string gameRoot)
    {
        if (string.IsNullOrWhiteSpace(gameRoot) || !Path.IsPathRooted(gameRoot.Trim()))
            throw new ArgumentException("Game root must be a non-empty absolute Windows path.", nameof(gameRoot));

        string supplied = gameRoot.Trim();
        if (supplied.Split([Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar], StringSplitOptions.RemoveEmptyEntries)
            .Any(segment => segment is "." or ".."))
            throw new ArgumentException("Game root must not contain relative traversal segments.", nameof(gameRoot));

        string full;
        try { full = Path.GetFullPath(supplied); }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        { throw new ArgumentException("Game root is not a valid full Windows path.", nameof(gameRoot), ex); }

        string root = Path.GetPathRoot(full) ?? string.Empty;
        full = full.Length > root.Length
            ? full.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
            : full;
        if (full.Equals(root, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Game root cannot be a volume root.", nameof(gameRoot));
        return full;
    }

    private static string CreateChild(string parent, string childName)
    {
        string child = Path.GetFullPath(Path.Combine(parent, childName));
        if (!IsStrictChildOf(child, parent))
            throw new InvalidDataException("Derived storage path escaped its owning root.");
        return child;
    }

    private void ValidateDerivedRoots()
    {
        if (PathsEqual(EditorRoot, GameRoot))
            throw new InvalidDataException("Editor root must not equal game root.");

        string[] roots = [BaselineRoot, ProjectRoot, LogsRoot, MetadataRoot];
        if (roots.Distinct(StringComparer.OrdinalIgnoreCase).Count() != roots.Length)
            throw new InvalidDataException("Storage child roots must be unique after normalization.");
        foreach (string child in roots)
        {
            if (!IsStrictChildOf(child, EditorRoot))
                throw new InvalidDataException("Storage child root escaped ElviraEditor.");
            if (roots.Any(other => !PathsEqual(other, child) && IsStrictChildOf(child, other)))
                throw new InvalidDataException("Storage child roots must not overlap.");
        }
        if (!IsStrictChildOf(MutableBackupsRoot, BaselineRoot) || !IsStrictChildOf(VariantsRoot, GameRoot) ||
            IsStrictChildOf(VariantsRoot, EditorRoot) || IsStrictChildOf(EditorRoot, VariantsRoot))
            throw new InvalidDataException("Baseline or DOS-visible variants root has an invalid ownership relationship.");
    }

    private static bool IsStrictChildOf(string candidate, string parent) =>
        candidate.StartsWith(EnsureTrailingSeparator(parent), StringComparison.OrdinalIgnoreCase);

    private static string EnsureTrailingSeparator(string path) => path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar)
        ? path : path + Path.DirectorySeparatorChar;

    private static bool PathsEqual(string left, string right) => left.Equals(right, StringComparison.OrdinalIgnoreCase);

    private static bool IsReservedWindowsDeviceName(string name) =>
        name.Equals("CON", StringComparison.OrdinalIgnoreCase) || name.Equals("PRN", StringComparison.OrdinalIgnoreCase) ||
        name.Equals("AUX", StringComparison.OrdinalIgnoreCase) || name.Equals("NUL", StringComparison.OrdinalIgnoreCase) ||
        (name.Length == 4 && (name.StartsWith("COM", StringComparison.OrdinalIgnoreCase) || name.StartsWith("LPT", StringComparison.OrdinalIgnoreCase)) &&
         name[3] is >= '1' and <= '9');

    private static void AssertRejected(Action action)
    {
        try { action(); }
        catch (ArgumentException) { return; }
        throw new InvalidDataException("Storage path safety smoke accepted an unsafe input.");
    }
}
