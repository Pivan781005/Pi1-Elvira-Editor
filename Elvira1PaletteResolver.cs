namespace Pi1ElviraEditor;

internal enum PaletteResolutionKind
{
    Unique,
    Contextual,
    Unresolved,
    Invalid
}

internal sealed record PaletteResolution(PaletteResolutionKind Kind, int? PaletteId, IReadOnlyList<int> CandidatePaletteIds, string Evidence)
{
    public static PaletteResolution Unique(int palette) => new(PaletteResolutionKind.Unique, palette, new[] { palette }, "One statically observed palette.");
    public static PaletteResolution Contextual(IReadOnlyList<int> palettes) => new(PaletteResolutionKind.Contextual, null, palettes, "More than one statically observed palette.");
    public static PaletteResolution Unresolved() => new(PaletteResolutionKind.Unresolved, null, Array.Empty<int>(), "No safe static palette mapping.");
    public static PaletteResolution Invalid(string evidence) => new(PaletteResolutionKind.Invalid, null, Array.Empty<int>(), evidence);
}

// Production adapter around the N5 structural script parser. It derives data
// only from the currently loaded pair; no N5 report, hard-coded mapping, or UI
// dependency is involved.
internal sealed class Elvira1PaletteResolver
{
    private readonly Dictionary<ResourceIdentity, Elvira1VgaScriptAudit.PairAudit> _cache = new();

    public PaletteResolution Resolve(string vga1Path, string vga2Path, int imageId)
    {
        try
        {
            var identity = ResourceIdentity.Create(vga1Path, vga2Path);
            if (!_cache.TryGetValue(identity, out Elvira1VgaScriptAudit.PairAudit? parsed))
            {
                parsed = new Elvira1VgaScriptAudit.PairAudit("", Path.GetFileName(vga1Path), Path.GetFileName(vga2Path));
                parsed.Parse(File.ReadAllBytes(vga1Path), File.ReadAllBytes(vga2Path));
                _cache[identity] = parsed;
            }
            return parsed.Resolve(imageId);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException)
        {
            return PaletteResolution.Invalid(ex.Message);
        }
    }

    public static int EffectivePaletteBank(int? manualPaletteBank, PaletteResolution automatic, int paletteCount)
    {
        if (manualPaletteBank is int manual && manual >= 0 && manual < paletteCount)
            return manual;
        if (automatic.Kind == PaletteResolutionKind.Unique && automatic.PaletteId is int resolved && resolved >= 0 && resolved < paletteCount)
            return resolved;
        return 0;
    }

    internal static PaletteResolution ResolveForBytes(byte[] vga1, byte[] vga2, int imageId)
    {
        try
        {
            var parsed = new Elvira1VgaScriptAudit.PairAudit("test", "test1.VGA", "test2.VGA");
            parsed.Parse(vga1, vga2);
            return parsed.Resolve(imageId);
        }
        catch (Exception ex) when (ex is InvalidDataException or ArgumentException)
        {
            return PaletteResolution.Invalid(ex.Message);
        }
    }

    private sealed record ResourceIdentity(string Vga1Path, long Vga1Length, DateTime Vga1WriteUtc, string Vga2Path, long Vga2Length, DateTime Vga2WriteUtc)
    {
        public static ResourceIdentity Create(string vga1Path, string vga2Path)
        {
            var one = new FileInfo(vga1Path);
            var two = new FileInfo(vga2Path);
            if (!one.Exists || !two.Exists) throw new FileNotFoundException("The paired Elvira I VGA resource is unavailable.");
            return new ResourceIdentity(Path.GetFullPath(vga1Path), one.Length, one.LastWriteTimeUtc, Path.GetFullPath(vga2Path), two.Length, two.LastWriteTimeUtc);
        }
    }
}
