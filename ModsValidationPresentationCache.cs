namespace Pi1ElviraEditor;

/// <summary>R9F V8.6g passive-presentation validation cache for Mods tab.
/// Caches ValidateBaseline and ValidateSources results across Mods opens,
/// validated by file size+mtime stats (no hashing) on each reuse. Any stat
/// mismatch recomputes fresh (fail-closed). Only used during passive
/// OpenModsLauncher presentation scope; Build/Rebuild/Verify/Restore always
/// compute authoritative fresh results (bypassing the cache for reads) and
/// refresh the cache afterwards. Presentation cache is never authorization.</summary>
internal static class ModsValidationPresentationCache
{
    [ThreadStatic]
    internal static bool IsPresentationScope;

    private static readonly object Gate = new();
    private static readonly Dictionary<string, BaselineEntry> Baselines = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, SourcesEntry> Sources = new(StringComparer.OrdinalIgnoreCase);

    private sealed record BaselineEntry(
        Dictionary<string, (long Length, DateTime Mtime)> Stats,
        BaselineValidationResult Result);

    private sealed record SourcesEntry(
        Dictionary<string, (long Length, DateTime Mtime)> Stats,
        DisposableVariantBuildStatus Status,
        IReadOnlyList<DisposableVariantBuildSource> SourceList);

    internal static bool TryGetBaseline(string gameRoot, EditorStorageLayout layout, out BaselineValidationResult result)
    {
        result = null!;
        if (!IsPresentationScope) return false;
        string key;
        try { key = Path.GetFullPath(gameRoot).ToUpperInvariant(); }
        catch { return false; }
        BaselineEntry? entry;
        lock (Gate)
        {
            if (!Baselines.TryGetValue(key, out entry) || entry is null) return false;
        }
        if (!StatsMatch(layout.GameRoot, entry.Stats)) return false;
        result = entry.Result;
        return true;
    }

    internal static void StoreBaseline(string gameRoot, EditorStorageLayout layout, BaselineValidationResult result)
    {
        string key;
        try { key = Path.GetFullPath(gameRoot).ToUpperInvariant(); }
        catch { return; }
        var stats = CaptureStats(layout.GameRoot);
        if (stats is null) return;
        lock (Gate) Baselines[key] = new BaselineEntry(stats, result);
    }

    internal static bool TryGetSources(ProjectContext project, out DisposableVariantBuildStatus status, out IReadOnlyList<DisposableVariantBuildSource> sources)
    {
        status = DisposableVariantBuildStatus.InvalidContext;
        sources = [];
        if (!IsPresentationScope || project is null) return false;
        string key;
        try { key = Path.GetFullPath(project.GameRoot).ToUpperInvariant(); }
        catch { return false; }
        SourcesEntry? entry;
        lock (Gate)
        {
            if (!Sources.TryGetValue(key, out entry) || entry is null) return false;
        }
        if (!StatsMatch(project.StorageLayout.GameRoot, entry.Stats)) return false;
        // Manifest identity must still match; a recaptured baseline changes fingerprint.
        // Compare expected sizes against current manifest to avoid cross-manifest reuse.
        if (entry.SourceList.Count != project.PristineManifest.Files.Count(file => file.Classification != PristineFileClassification.GeneratedIgnored))
            return false;
        status = entry.Status;
        sources = entry.SourceList;
        return true;
    }

    internal static void StoreSources(ProjectContext project, DisposableVariantBuildStatus status, IReadOnlyList<DisposableVariantBuildSource> sources)
    {
        if (project is null) return;
        string key;
        try { key = Path.GetFullPath(project.GameRoot).ToUpperInvariant(); }
        catch { return; }
        var stats = CaptureStats(project.StorageLayout.GameRoot);
        if (stats is null) return;
        lock (Gate) Sources[key] = new SourcesEntry(stats, status, sources);
    }

    internal static void InvalidateGameRoot(string? gameRoot)
    {
        if (string.IsNullOrWhiteSpace(gameRoot)) return;
        string key;
        try { key = Path.GetFullPath(gameRoot).ToUpperInvariant(); }
        catch { return; }
        lock (Gate)
        {
            Baselines.Remove(key);
            Sources.Remove(key);
        }
    }

    private static Dictionary<string, (long Length, DateTime Mtime)>? CaptureStats(string gameRoot)
    {
        try
        {
            var map = new Dictionary<string, (long, DateTime)>(StringComparer.OrdinalIgnoreCase);
            foreach (string file in EnumerateGameFiles(gameRoot))
            {
                var info = new FileInfo(file);
                if (!info.Exists) return null;
                map[Path.GetFullPath(file).ToUpperInvariant()] = (info.Length, info.LastWriteTimeUtc);
            }
            return map;
        }
        catch { return null; }
    }

    private static bool StatsMatch(string gameRoot, Dictionary<string, (long Length, DateTime Mtime)> expected)
    {
        try
        {
            var actual = new Dictionary<string, (long Length, DateTime Mtime)>(StringComparer.OrdinalIgnoreCase);
            foreach (string file in EnumerateGameFiles(gameRoot))
            {
                var info = new FileInfo(file);
                if (!info.Exists) return false;
                actual[Path.GetFullPath(file).ToUpperInvariant()] = (info.Length, info.LastWriteTimeUtc);
            }
            if (actual.Count != expected.Count) return false;
            foreach (var pair in expected)
            {
                if (!actual.TryGetValue(pair.Key, out var have)) return false;
                if (have.Length != pair.Value.Length || have.Mtime != pair.Value.Mtime) return false;
            }
            return true;
        }
        catch { return false; }
    }

    private static IEnumerable<string> EnumerateGameFiles(string gameRoot)
    {
        // Mirror PristineInstallationService exclusions (ElviraEditor/, VARIANTS/) without hashing.
        var stack = new Stack<string>();
        stack.Push(gameRoot);
        while (stack.Count > 0)
        {
            string dir = stack.Pop();
            string[] files = [];
            string[] dirs = [];
            try
            {
                files = Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly);
                dirs = Directory.GetDirectories(dir, "*", SearchOption.TopDirectoryOnly);
            }
            catch { continue; }
            foreach (string file in files) yield return file;
            foreach (string child in dirs)
            {
                string relative;
                try { relative = Path.GetRelativePath(gameRoot, child).Replace('/', '\\'); }
                catch { continue; }
                if (relative.StartsWith(EditorStorageLayout.EditorDirectoryName + "\\", StringComparison.OrdinalIgnoreCase) ||
                    relative.StartsWith(EditorStorageLayout.VariantsDirectoryName + "\\", StringComparison.OrdinalIgnoreCase) ||
                    relative.Equals(EditorStorageLayout.EditorDirectoryName, StringComparison.OrdinalIgnoreCase) ||
                    relative.Equals(EditorStorageLayout.VariantsDirectoryName, StringComparison.OrdinalIgnoreCase))
                    continue;
                FileAttributes attributes;
                try { attributes = File.GetAttributes(child); }
                catch { continue; }
                if ((attributes & FileAttributes.ReparsePoint) != 0) continue;
                stack.Push(child);
            }
        }
    }
}
