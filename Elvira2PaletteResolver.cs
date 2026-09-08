namespace Pi1ElviraEditor;

// Production adapter around the E2 structural script parser.  It reads only
// the active VGA1/VGA2 pair and caches by both resources' identities.
internal sealed class Elvira2PaletteResolver
{
    private readonly Dictionary<ResourceIdentity, CacheEntry> _cache = new();

    public PaletteResolution Resolve(string vga1Path, string vga2Path, int imageId)
    {
        try
        {
            ResourceIdentity identity = ResourceIdentity.Create(vga1Path, vga2Path);
            if (!_cache.TryGetValue(identity, out CacheEntry? entry))
            {
                try
                {
                    var pair = new Elvira2VgaScriptAudit.Pair("", Path.GetFileName(vga1Path), Path.GetFileName(vga2Path));
                    pair.Parse(File.ReadAllBytes(vga1Path), File.ReadAllBytes(vga2Path));
                    entry = new CacheEntry(pair, null);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException)
                {
                    entry = new CacheEntry(null, PaletteResolution.Invalid(ex.Message));
                }
                _cache[identity] = entry;
            }
            return entry.Failure ?? entry.Pair!.Resolve(imageId);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException)
        {
            return PaletteResolution.Invalid(ex.Message);
        }
    }

    internal static PaletteResolution ResolveForBytes(byte[] vga1, byte[] vga2, int imageId)
    {
        try
        {
            var pair = new Elvira2VgaScriptAudit.Pair("test", "test1.VGA", "test2.VGA");
            pair.Parse(vga1, vga2);
            return pair.Resolve(imageId);
        }
        catch (Exception ex) when (ex is InvalidDataException or ArgumentException)
        {
            return PaletteResolution.Invalid(ex.Message);
        }
    }

    private sealed record CacheEntry(Elvira2VgaScriptAudit.Pair? Pair, PaletteResolution? Failure);

    private sealed record ResourceIdentity(string Vga1Path, long Vga1Length, DateTime Vga1WriteUtc, string Vga2Path, long Vga2Length, DateTime Vga2WriteUtc)
    {
        public static ResourceIdentity Create(string vga1Path, string vga2Path)
        {
            var one = new FileInfo(vga1Path);
            var two = new FileInfo(vga2Path);
            if (!one.Exists || !two.Exists) throw new FileNotFoundException("The paired Elvira II VGA resource is unavailable.");
            return new ResourceIdentity(Path.GetFullPath(vga1Path), one.Length, one.LastWriteTimeUtc, Path.GetFullPath(vga2Path), two.Length, two.LastWriteTimeUtc);
        }
    }
}
