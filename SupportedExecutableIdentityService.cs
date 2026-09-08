namespace Pi1ElviraEditor;

/// <summary>
/// R9D explicit executable-identity classification for the three binary-dependent
/// supported runtimes (E1 VGA / RUNVGA, E1 EGA / RUNEGA, E2 VGA / RUNIT).
/// Supported identity is grounded exclusively in the frozen size + SHA-256
/// fingerprints already represented by the bootstrap services and production
/// profiles. Compatibility is never inferred from the file name, openability,
/// unpackability, approximate size, or neighboring data files.
/// </summary>
internal enum SupportedExecutableIdentity
{
    /// <summary>The expected executable file is absent. This is distinct from <see cref="Unsupported"/>.</summary>
    Missing,
    /// <summary>The proven original packed executable (frozen size + SHA-256).</summary>
    SupportedPacked,
    /// <summary>The proven canonical unpacked representation (frozen size + SHA-256).</summary>
    SupportedCanonical,
    /// <summary>
    /// A structurally valid derived CP852 output (RUNVGA V5, RUNEGA frozen output,
    /// RUNIT V2). It is a supported installation payload but never a bootstrap source.
    /// </summary>
    GeneratedExtended,
    /// <summary>The file exists but its binary identity matches no proven supported form.</summary>
    Unsupported
}

internal sealed record SupportedExecutableClassification(
    VariantRuntimeKind RuntimeKind,
    string ExecutableFileName,
    SupportedExecutableIdentity Identity,
    long Size,
    string? Sha256)
{
    public bool IsMissing => Identity == SupportedExecutableIdentity.Missing;
    public bool IsUnsupported => Identity == SupportedExecutableIdentity.Unsupported;
    public bool IsSupportedBootstrapSource => Identity is SupportedExecutableIdentity.SupportedPacked or SupportedExecutableIdentity.SupportedCanonical;
    public bool IsSupportedForBinaryUse => Identity is SupportedExecutableIdentity.SupportedPacked or SupportedExecutableIdentity.SupportedCanonical or SupportedExecutableIdentity.GeneratedExtended;
}

/// <summary>
/// Single fail-closed identity authority. It extends the existing per-runtime
/// bootstrap state models (which conflate a missing file with an unsupported
/// one) instead of replacing them. GameRoot is never written; classification is
/// read-only.
/// </summary>
internal static class SupportedExecutableIdentityService
{
    public static SupportedExecutableClassification Classify(VariantRuntimeKind runtime, string path) => runtime switch
    {
        VariantRuntimeKind.Elvira1Vga => ClassifyRunVga(path),
        VariantRuntimeKind.Elvira1Ega => ClassifyRunEga(path),
        VariantRuntimeKind.Elvira2Vga => ClassifyRunIt(path),
        _ => throw new ArgumentException("A supported runtime kind is required.", nameof(runtime))
    };

    public static SupportedExecutableClassification ClassifyRunVga(string path)
    {
        const VariantRuntimeKind runtime = VariantRuntimeKind.Elvira1Vga;
        if (!File.Exists(path)) return new(runtime, Elvira1ProductionProfile.ActiveVgaExecutable, SupportedExecutableIdentity.Missing, -1, null);
        byte[] image;
        try { image = File.ReadAllBytes(path); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { return new(runtime, Elvira1ProductionProfile.ActiveVgaExecutable, SupportedExecutableIdentity.Unsupported, -1, null); }
        string hash = Hash(image);
        if (image.Length == RunVgaBootstrapService.PackedSize && hash == RunVgaBootstrapService.PackedSha256)
            return new(runtime, Elvira1ProductionProfile.ActiveVgaExecutable, SupportedExecutableIdentity.SupportedPacked, image.Length, hash);
        if (image.Length == RunVgaBootstrapService.BaselineSize && hash == RunVgaBootstrapService.BaselineSha256)
            return new(runtime, Elvira1ProductionProfile.ActiveVgaExecutable, SupportedExecutableIdentity.SupportedCanonical, image.Length, hash);
        // The derived V5 form is recognized structurally (its font payload varies by
        // design); it remains rejected as a bootstrap source by IsSupportedBootstrapSource.
        if (RunVgaBootstrapService.DetectState(path) == RunVgaBootstrapState.ExtendedCp852V5)
            return new(runtime, Elvira1ProductionProfile.ActiveVgaExecutable, SupportedExecutableIdentity.GeneratedExtended, image.Length, hash);
        return new(runtime, Elvira1ProductionProfile.ActiveVgaExecutable, SupportedExecutableIdentity.Unsupported, image.Length, hash);
    }

    public static SupportedExecutableClassification ClassifyRunEga(string path)
    {
        const VariantRuntimeKind runtime = VariantRuntimeKind.Elvira1Ega;
        if (!File.Exists(path)) return new(runtime, Elvira1ProductionProfile.ActiveEgaExecutable, SupportedExecutableIdentity.Missing, -1, null);
        byte[] image;
        try { image = File.ReadAllBytes(path); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { return new(runtime, Elvira1ProductionProfile.ActiveEgaExecutable, SupportedExecutableIdentity.Unsupported, -1, null); }
        string hash = Hash(image);
        if (RunEgaBootstrapService.IsPacked(image))
            return new(runtime, Elvira1ProductionProfile.ActiveEgaExecutable, SupportedExecutableIdentity.SupportedPacked, image.Length, hash);
        if (RunEgaBootstrapService.IsCanonical(image))
            return new(runtime, Elvira1ProductionProfile.ActiveEgaExecutable, SupportedExecutableIdentity.SupportedCanonical, image.Length, hash);
        try
        {
            RunEgaBootstrapService.ValidateOutput(image);
            return new(runtime, Elvira1ProductionProfile.ActiveEgaExecutable, SupportedExecutableIdentity.GeneratedExtended, image.Length, hash);
        }
        catch (InvalidDataException) { }
        return new(runtime, Elvira1ProductionProfile.ActiveEgaExecutable, SupportedExecutableIdentity.Unsupported, image.Length, hash);
    }

    public static SupportedExecutableClassification ClassifyRunIt(string path)
    {
        const VariantRuntimeKind runtime = VariantRuntimeKind.Elvira2Vga;
        if (!File.Exists(path)) return new(runtime, Elvira2ProductionProfile.ActiveExecutable, SupportedExecutableIdentity.Missing, -1, null);
        byte[] image;
        try { image = File.ReadAllBytes(path); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        { return new(runtime, Elvira2ProductionProfile.ActiveExecutable, SupportedExecutableIdentity.Unsupported, -1, null); }
        string hash = Hash(image);
        if (image.Length == RunItBootstrapService.PackedSize && hash == RunItBootstrapService.PackedSha256)
            return new(runtime, Elvira2ProductionProfile.ActiveExecutable, SupportedExecutableIdentity.SupportedPacked, image.Length, hash);
        if (image.Length == RunItBootstrapService.CanonicalSize && hash == RunItBootstrapService.CanonicalSha256)
            return new(runtime, Elvira2ProductionProfile.ActiveExecutable, SupportedExecutableIdentity.SupportedCanonical, image.Length, hash);
        // The derived V2 form is recognized structurally (its font payload varies by
        // design); it remains rejected as a bootstrap source by IsSupportedBootstrapSource.
        if (RunItBootstrapService.DetectState(path) == RunItBootstrapState.ExtendedCp852)
            return new(runtime, Elvira2ProductionProfile.ActiveExecutable, SupportedExecutableIdentity.GeneratedExtended, image.Length, hash);
        return new(runtime, Elvira2ProductionProfile.ActiveExecutable, SupportedExecutableIdentity.Unsupported, image.Length, hash);
    }

    /// <summary>
    /// User-facing fail-closed wording. It identifies the runtime/executable, states
    /// that it is unsupported or modified, and states that the operation was blocked.
    /// Routed through the existing JSON locale catalog (English fallback preserved).
    /// </summary>
    public static string DescribeBlocked(SupportedExecutableClassification classification, string operation)
    {
        string runtime = RuntimeDisplay(classification.RuntimeKind);
        if (classification.IsMissing)
            return string.Format(UiText.Get("Binary.MissingBlocked"), runtime, classification.ExecutableFileName, operation);
        return string.Format(UiText.Get("Binary.UnsupportedBlocked"), runtime, classification.ExecutableFileName, operation);
    }

    /// <summary>
    /// User-facing wording for a generated runtime artifact (for example the data file)
    /// that no longer matches its authorized build output. Unlike <see cref="DescribeBlocked"/>,
    /// it never claims the executable itself is unsupported or modified.
    /// </summary>
    public static string DescribeArtifactMismatch(VariantRuntimeKind runtime, string artifactFileName, string operation) =>
        string.Format(UiText.Get("Binary.ArtifactMismatchBlocked"), RuntimeDisplay(runtime), artifactFileName, operation);

    /// <summary>Fails closed unless the file is a proven packed or canonical-unpacked source.</summary>
    public static void RequireBootstrapSource(SupportedExecutableClassification classification, string operation)
    {
        if (classification.IsMissing)
            throw new FileNotFoundException(string.Format(UiText.Get("Binary.MissingBlocked"), RuntimeDisplay(classification.RuntimeKind), classification.ExecutableFileName, operation), classification.ExecutableFileName);
        if (!classification.IsSupportedBootstrapSource)
            throw new InvalidDataException(DescribeBlocked(classification with { Identity = SupportedExecutableIdentity.Unsupported }, operation));
    }

    /// <summary>Fails closed unless the file is a proven supported or generated-extended form.</summary>
    public static void RequireBinaryUse(SupportedExecutableClassification classification, string operation)
    {
        if (classification.IsMissing)
            throw new FileNotFoundException(string.Format(UiText.Get("Binary.MissingBlocked"), RuntimeDisplay(classification.RuntimeKind), classification.ExecutableFileName, operation), classification.ExecutableFileName);
        if (!classification.IsSupportedForBinaryUse)
            throw new InvalidDataException(DescribeBlocked(classification, operation));
    }

    /// <summary>
    /// R9D trusted-patch gate for mutation operations such as font binary patching.
    /// FORMAT recognition (<see cref="SupportedExecutableIdentity.GeneratedExtended"/>) alone
    /// never authorizes a mutation: packed/canonical forms are trusted by their exact
    /// frozen hashes, while generated V5/V2 forms must additionally prove that every byte
    /// outside their documented mutable font regions reproduces the proven derivation.
    /// No trusted mutation path exists for RUNEGA through this gate.
    /// </summary>
    public static void RequireTrustedPatchTarget(SupportedExecutableClassification classification, string sourcePath, string operation)
    {
        if (classification.IsMissing)
            throw new FileNotFoundException(string.Format(UiText.Get("Binary.MissingBlocked"), RuntimeDisplay(classification.RuntimeKind), classification.ExecutableFileName, operation), classification.ExecutableFileName);
        if (classification.Identity is SupportedExecutableIdentity.SupportedPacked or SupportedExecutableIdentity.SupportedCanonical)
            return;
        if (classification.Identity == SupportedExecutableIdentity.GeneratedExtended && IsTrustedGeneratedPatchTarget(classification.RuntimeKind, sourcePath))
            return;
        throw new InvalidDataException(DescribeBlocked(classification with { Identity = SupportedExecutableIdentity.Unsupported }, operation));
    }

    private static bool IsTrustedGeneratedPatchTarget(VariantRuntimeKind runtime, string sourcePath)
    {
        byte[] image;
        try { image = File.ReadAllBytes(sourcePath); }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return false;
        }
        return runtime switch
        {
            VariantRuntimeKind.Elvira1Vga => RunVgaBootstrapService.IsTrustedV5PatchTarget(image),
            VariantRuntimeKind.Elvira2Vga => RunItBootstrapService.IsTrustedV2PatchTarget(image),
            _ => false
        };
    }

    private static string RuntimeDisplay(VariantRuntimeKind runtime) => runtime switch
    {
        VariantRuntimeKind.Elvira1Vga => "Elvira I VGA",
        VariantRuntimeKind.Elvira1Ega => "Elvira I EGA",
        VariantRuntimeKind.Elvira2Vga => "Elvira II VGA",
        _ => "Unknown runtime"
    };

    private static string Hash(byte[] image) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(image));
}
