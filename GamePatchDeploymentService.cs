using System.Security.Cryptography;

namespace ElviraVgaEditor;

internal sealed record ProductionDeploymentResult(
    ElviraGame Game,
    string ActiveExecutable,
    string OriginalExecutable,
    string ExecutableSha256);

/// <summary>
/// Production activation is intentionally separate from font construction: build and validate a
/// temporary image first, then preserve the target executable's pristine O-file once and replace
/// only that active executable. Font deployment never writes GAMEPC.
/// </summary>
internal static class GamePatchDeploymentService
{
    public static ProductionDeploymentResult Deploy(FontLoadResult loaded, IReadOnlyList<GlyphModel> glyphs)
    {
        if (loaded.Game is not (ElviraGame.Elvira1 or ElviraGame.Elvira2))
            throw new InvalidDataException("The executable game type is unknown; deployment is disabled.");

        string directory = Path.GetDirectoryName(Path.GetFullPath(loaded.SourcePath)) ?? throw new IOException("Game directory is unavailable.");
        string activeExe = Path.Combine(directory, loaded.Game == ElviraGame.Elvira1 ? "RUNVGA.EXE" : "RUNIT.EXE");
        string originalExe = Path.Combine(directory, loaded.Game == ElviraGame.Elvira1 ? "RUNVGAO.EXE" : "RUNITO.EXE");

        bool haveOriginalExe = File.Exists(originalExe);
        if (!File.Exists(activeExe))
            throw new FileNotFoundException($"Cannot update font: target executable is missing: {activeExe}. No files were changed.", activeExe);

        string bootstrapSource = haveOriginalExe ? originalExe : activeExe;
        ValidateBootstrapSource(loaded.Game, bootstrapSource);

        string stamp = Guid.NewGuid().ToString("N");
        string exeTemp = Path.Combine(directory, ".elvira_font_" + stamp + ".exe.tmp");
        string exePrevious = Path.Combine(directory, ".elvira_previous_" + stamp + ".exe");
        try
        {
            if (loaded.Game == ElviraGame.Elvira1)
                RunVgaBootstrapService.CreateExtendedCp852(bootstrapSource, exeTemp, glyphs);
            else
                RunItBootstrapService.CreateExtendedCp852(bootstrapSource, exeTemp, glyphs);

            ValidateGenerated(loaded.Game, exeTemp);

            if (!haveOriginalExe)
            {
                File.Move(activeExe, originalExe); // never overwrites an O-file
            }
            else
            {
                // A valid existing O-file is the normal repeated-write state.
                File.Move(activeExe, exePrevious);
            }

            File.Move(exeTemp, activeExe);
            DeleteIfExists(exePrevious);
            return new ProductionDeploymentResult(loaded.Game, activeExe, originalExe, HashFile(activeExe));
        }
        catch
        {
            // Restore the pre-existing active target whenever replacement had already started.
            if (!File.Exists(activeExe) && File.Exists(exePrevious)) File.Move(exePrevious, activeExe);
            // First activation remains recoverable: the O-file is the original and is never replaced.
            if (!File.Exists(activeExe) && File.Exists(originalExe) && !haveOriginalExe) File.Copy(originalExe, activeExe);
            throw;
        }
        finally
        {
            DeleteIfExists(exeTemp); DeleteIfExists(exePrevious);
        }
    }

    private static void ValidateBootstrapSource(ElviraGame game, string path)
    {
        bool valid = game == ElviraGame.Elvira1
            ? RunVgaBootstrapService.DetectState(path) is RunVgaBootstrapState.OriginalPacked or RunVgaBootstrapState.UnpackedBaseline
            : RunItBootstrapService.DetectState(path) is RunItBootstrapState.OriginalPacked or RunItBootstrapState.CanonicalUnpackedAscii98;
        if (!valid) throw new InvalidDataException("The authoritative O-file/active source is not a supported pristine original. No files were changed.");
    }

    private static void ValidateGenerated(ElviraGame game, string path)
    {
        bool valid = game == ElviraGame.Elvira1
            ? RunVgaBootstrapService.DetectState(path) == RunVgaBootstrapState.ExtendedCp852V5
            : RunItBootstrapService.DetectState(path) == RunItBootstrapState.ExtendedCp852;
        if (!valid) throw new InvalidDataException("The temporary patched executable failed structural validation.");
    }

    private static void DeleteIfExists(string path) { if (File.Exists(path)) File.Delete(path); }
    private static string HashFile(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
}
