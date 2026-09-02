namespace ElviraVgaEditor;

/// <summary>
/// Identifies the editable project/mod selection independently from the DOS
/// runtime.  EN is the immutable baseline projection and deliberately has no
/// writable project-state directory.
/// </summary>
internal static class ProjectVariantOwnership
{
    internal const string OriginalCode = "EN";
    private const string OwnedProjectsDirectoryName = "Projects";

    internal static bool IsOriginal(string? code) => string.Equals(code, OriginalCode, StringComparison.OrdinalIgnoreCase);

    internal static string NormalizeCode(ProjectContext project, string? code)
    {
        ArgumentNullException.ThrowIfNull(project);
        if (IsOriginal(code)) return OriginalCode;
        return TranslationProjectService.ValidateProjectCode(code, project.GameProfile);
    }

    internal static string GetOwnedProjectRoot(ProjectContext project, string code)
    {
        string normalized = NormalizeCode(project, code);
        if (IsOriginal(normalized))
            throw new InvalidOperationException("The Original project is the immutable baseline and has no editable project-state directory.");

        string root = Path.GetFullPath(Path.Combine(project.ProjectRoot, OwnedProjectsDirectoryName, normalized));
        string parent = Path.GetFullPath(Path.Combine(project.ProjectRoot, OwnedProjectsDirectoryName));
        if (!root.StartsWith(parent + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Project variant storage escaped the editor-owned project root.");
        return root;
    }

    internal static string GetOwnedStatePath(ProjectContext project, string code, string fileName) =>
        Path.Combine(GetOwnedProjectRoot(project, code), Path.GetFileName(fileName));

    /// <summary>Legacy R6/R8 state was runtime-wide at Project\&lt;file&gt;.
    /// It is intentionally only detectable here: it is never projected into a
    /// selected project variant without explicit ownership assignment.</summary>
    internal static string GetLegacyUnassignedPath(ProjectContext project, string fileName) =>
        Path.Combine(project.ProjectRoot, Path.GetFileName(fileName));

    internal static bool HasLegacyUnassignedState(ProjectContext project, string fileName) =>
        File.Exists(GetLegacyUnassignedPath(project, fileName));
}

/// <summary>Handles only the old runtime-wide state files. They are retained
/// as unassigned evidence until a user explicitly assigns them to one editable
/// project. No load/projection path consults these files.</summary>
internal sealed class LegacyProjectStateService
{
    private static readonly string[] StateFiles =
    [
        GraphicsVariantService.FileName,
        FontVariantService.FileName,
        RuntimeUiTextService.FileName
    ];

    internal IReadOnlyList<string> DetectUnassigned(ProjectContext project) =>
        StateFiles.Where(file => ProjectVariantOwnership.HasLegacyUnassignedState(project, file)).ToArray();

    internal IReadOnlyList<string> AssignAllToProject(ProjectContext project, string projectVariantCode)
    {
        string code = ProjectVariantOwnership.NormalizeCode(project, projectVariantCode);
        if (ProjectVariantOwnership.IsOriginal(code))
            throw new InvalidOperationException("Legacy editable state cannot be assigned to the immutable Original project.");

        var assigned = new List<string>();
        foreach (string file in DetectUnassigned(project))
        {
            string source = ProjectVariantOwnership.GetLegacyUnassignedPath(project, file);
            string destination = ProjectVariantOwnership.GetOwnedStatePath(project, code, file);
            if (File.Exists(destination))
                throw new IOException($"The selected project already owns {file}; legacy state was not assigned.");
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(source, destination, overwrite: false);

            string archive = Path.Combine(project.ProjectRoot, "Legacy", "Assigned", code, file);
            Directory.CreateDirectory(Path.GetDirectoryName(archive)!);
            if (File.Exists(archive))
                throw new IOException($"Legacy archive already contains {file}; legacy state was not assigned.");
            File.Move(source, archive);
            assigned.Add(file);
        }
        return assigned;
    }
}
