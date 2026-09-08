namespace Pi1ElviraEditor;

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
}
