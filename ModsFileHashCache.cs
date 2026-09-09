using System.Security.Cryptography;

namespace Pi1ElviraEditor;

/// <summary>R9F V8.6g one-refresh file-hash reuse for passive Mods presentation.
/// During a single synchronous OpenModsLauncher refresh, pristine GameRoot
/// files are hashed once and reused by ValidateBaseline and ValidateSources.
/// Entries are validated by length + mtime before reuse; any mid-refresh
/// change recomputes. The scope lives only for one refresh; Build/Rebuild/
/// Verify/Restore outside the scope always hash fresh. Presentation cache is
/// never used as safety authorization.</summary>
internal static class ModsFileHashCache
{
    [ThreadStatic]
    private static Dictionary<string, Entry>? _current;

    [ThreadStatic]
    private static int _depth;

    private sealed record Entry(long Length, DateTime LastWriteUtc, string Sha256);

    public static IDisposable BeginRefresh()
    {
        _depth++;
        _current ??= new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);
        return new Scope();
    }

    private sealed class Scope : IDisposable
    {
        private bool _disposed;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _depth--;
            if (_depth <= 0)
            {
                _depth = 0;
                _current = null;
            }
        }
    }

    internal static bool TryGet(string absolutePath, out string sha256, out long length)
    {
        sha256 = string.Empty;
        length = 0;
        var map = _current;
        if (map is null) return false;
        string key;
        try { key = Path.GetFullPath(absolutePath); }
        catch { return false; }
        if (!map.TryGetValue(key, out Entry? entry)) return false;
        try
        {
            var info = new FileInfo(key);
            if (!info.Exists || info.Length != entry.Length || info.LastWriteTimeUtc != entry.LastWriteUtc)
            {
                map.Remove(key);
                return false;
            }
            sha256 = entry.Sha256;
            length = entry.Length;
            return true;
        }
        catch { return false; }
    }

    internal static string GetSha256(string absolutePath)
    {
        var map = _current;
        if (map is not null)
        {
            string key;
            try { key = Path.GetFullPath(absolutePath); }
            catch { return HashUncached(absolutePath); }
            if (map.TryGetValue(key, out Entry? entry))
            {
                try
                {
                    var info = new FileInfo(key);
                    if (info.Exists && info.Length == entry.Length && info.LastWriteTimeUtc == entry.LastWriteUtc)
                        return entry.Sha256;
                    map.Remove(key);
                }
                catch { }
            }
            string computed = HashUncached(key);
            try
            {
                var info = new FileInfo(key);
                map[key] = new Entry(info.Length, info.LastWriteTimeUtc, computed);
            }
            catch { }
            return computed;
        }
        return HashUncached(absolutePath);
    }

    private static string HashUncached(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
}
