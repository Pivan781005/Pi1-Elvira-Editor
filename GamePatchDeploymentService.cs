using System.Security.Cryptography;

namespace ElviraVgaEditor;

internal sealed record ProductionDeploymentResult(
    ElviraGame Game,
    string ActiveExecutable,
    string OriginalExecutable,
    string ActiveGamePc,
    string OriginalGamePc,
    string ExecutableSha256);

/// <summary>
/// Production activation is intentionally separate from font construction: build and validate a
/// temporary image first, then preserve pristine O-files once and replace only active files.
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
        string activeGamePc = Path.Combine(directory, "GAMEPC");
        string originalGamePc = Path.Combine(directory, "GAMEPCO");

        bool haveOriginalExe = File.Exists(originalExe);
        bool haveOriginalGamePc = File.Exists(originalGamePc);
        if (haveOriginalExe != haveOriginalGamePc)
            throw new InvalidOperationException("Partial original-backup state detected (only one O-file exists). No files were changed; restore or complete the installation manually.");
        if (!File.Exists(activeExe) || !File.Exists(activeGamePc))
            throw new FileNotFoundException("The active executable or GAMEPC is missing. No files were changed.");

        string bootstrapSource = haveOriginalExe ? originalExe : activeExe;
        ValidateBootstrapSource(loaded.Game, bootstrapSource);

        string stamp = Guid.NewGuid().ToString("N");
        string exeTemp = Path.Combine(directory, ".elvira_font_" + stamp + ".exe.tmp");
        string gamePcTemp = Path.Combine(directory, ".elvira_gamepc_" + stamp + ".tmp");
        string exePrevious = Path.Combine(directory, ".elvira_previous_" + stamp + ".exe");
        string gamePcPrevious = Path.Combine(directory, ".elvira_previous_" + stamp + ".gamepc");
        try
        {
            if (loaded.Game == ElviraGame.Elvira1)
                RunVgaBootstrapService.CreateExtendedCp852(bootstrapSource, exeTemp, glyphs);
            else
                RunItBootstrapService.CreateExtendedCp852(bootstrapSource, exeTemp, glyphs);

            // Text editing owns GAMEPC content; deployment keeps its current byte stream but moves
            // it through a validated temporary file so executable and resource activation share one transaction.
            File.Copy(activeGamePc, gamePcTemp, false);
            ValidateGenerated(loaded.Game, exeTemp);
            if (new FileInfo(gamePcTemp).Length == 0) throw new InvalidDataException("GAMEPC temporary output is empty.");

            if (!haveOriginalExe)
            {
                File.Move(activeExe, originalExe); // never overwrites an O-file
                File.Move(activeGamePc, originalGamePc);
            }
            else
            {
                // Retain previous active files until both replacements have succeeded.
                File.Move(activeExe, exePrevious);
                File.Move(activeGamePc, gamePcPrevious);
            }

            File.Move(exeTemp, activeExe);
            File.Move(gamePcTemp, activeGamePc);
            DeleteIfExists(exePrevious);
            DeleteIfExists(gamePcPrevious);
            return new ProductionDeploymentResult(loaded.Game, activeExe, originalExe, activeGamePc, originalGamePc, HashFile(activeExe));
        }
        catch
        {
            // Restore a pre-existing active pair whenever a replacement had already started.
            if (!File.Exists(activeExe) && File.Exists(exePrevious)) File.Move(exePrevious, activeExe);
            if (!File.Exists(activeGamePc) && File.Exists(gamePcPrevious)) File.Move(gamePcPrevious, activeGamePc);
            // First activation remains recoverable: the O-file is the original and is never replaced.
            if (!File.Exists(activeExe) && File.Exists(originalExe) && !haveOriginalExe) File.Copy(originalExe, activeExe);
            if (!File.Exists(activeGamePc) && File.Exists(originalGamePc) && !haveOriginalGamePc) File.Copy(originalGamePc, activeGamePc);
            throw;
        }
        finally
        {
            DeleteIfExists(exeTemp); DeleteIfExists(gamePcTemp); DeleteIfExists(exePrevious); DeleteIfExists(gamePcPrevious);
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
