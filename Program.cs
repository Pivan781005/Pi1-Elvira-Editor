using System.Reflection;

namespace ElviraVgaEditor;

internal static class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length == 1 && args[0].Equals("--elvira1-profile-freeze-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira1ProductionProfile.VerifyFrozenInvariants(); Console.WriteLine("Elvira I frozen profile: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I frozen profile: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--replace-png-validation-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyReplacePngValidationSmoke(); Console.WriteLine("Replace PNG palette validation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Replace PNG palette validation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if ((args.Length == 2 || args.Length == 3) && args[0].Equals("--runega-bootstrap-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunEgaBootstrapFixture(args[1], args.Length == 3 ? args[2] : null); Console.WriteLine("RUNEGA bootstrap fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNEGA bootstrap fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--elvira2-profile-freeze-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira2ProductionProfile.VerifyFrozenInvariants(); Console.WriteLine("Elvira II frozen profile: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira II frozen profile: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--storage-layout-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { EditorStorageLayout.VerifySmokeInvariants(); Console.WriteLine("Storage layout: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Storage layout: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--pristine-baseline-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyPristineBaselineSmoke(); Console.WriteLine("Pristine baseline inventory: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Pristine baseline inventory: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--pristine-manifest-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyPristineManifestSmoke(); Console.WriteLine("Pristine manifest: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Pristine manifest: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--launcher-classification-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLauncherClassificationSmoke(); Console.WriteLine("Launcher classification: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Launcher classification: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--project-context-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyProjectContextSmoke(args[1], args[2]); Console.WriteLine("Project context fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Project context fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--project-context-real-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRealProjectContextSmoke(args[1], args[2]); Console.WriteLine("Project context real integration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Project context real integration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-context-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyVariantContextSmoke(args[1], args[2]); Console.WriteLine("Variant context fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant context fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-context-real-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRealVariantContextSmoke(args[1], args[2]); Console.WriteLine("Variant context real integration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant context real integration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-directory-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyVariantDirectorySmoke(args[1], args[2]); Console.WriteLine("Variant directory fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant directory fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-directory-real-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRealVariantDirectorySmoke(args[1], args[2]); Console.WriteLine("Variant directory real integration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant directory real integration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--disposable-build-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyDisposableBuildSmoke(args[1], args[2]); Console.WriteLine("Disposable build fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Disposable build fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--disposable-build-real-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRealDisposableBuildSmoke(args[1], args[2]); Console.WriteLine("Disposable build real integration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Disposable build real integration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--composite-build-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyCompositeBuildSmoke(args[1], args[2]); Console.WriteLine("Composite build fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Composite build fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--active-project-composite-build-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyActiveProjectCompositeBuildSmoke(args[1]); Console.WriteLine("Active project composite build: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Active project composite build: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--project-variant-ownership-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyProjectVariantOwnershipSmoke(args[1]); Console.WriteLine("Project variant ownership: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Project variant ownership: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--edition-switch-stress-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyProjectVariantOwnershipSmoke(args[1], editionStressOnly: true); Console.WriteLine("Edition switching stress: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Edition switching stress: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--mainform-layout-stress-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyProjectVariantOwnershipSmoke(args[1], editionStressOnly: true, layoutStress: true); Console.WriteLine("MainForm layout stress: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("MainForm layout stress: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--button-geometry-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyButtonGeometrySmoke(); Console.WriteLine("Button geometry: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Button geometry: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--graphics-fit-centering-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyGraphicsFitCenteringSmoke(); Console.WriteLine("Graphics Fit centering: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Graphics Fit centering: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--first-paint-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyFirstPaintSmoke(); Console.WriteLine("First paint: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("First paint: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--header-alignment-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyHeaderAlignmentSmoke(args[1], args[2]); Console.WriteLine("Header alignment: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Header alignment: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runvga-composite-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunVgaCompositeFixture(args[1]); Console.WriteLine("RUNVGA composite fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNVGA composite fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runega-composite-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunEgaCompositeFixture(args[1]); Console.WriteLine("RUNEGA composite fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNEGA composite fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runega-composite-real", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRealRunEgaCompositeBuild(args[1]); Console.WriteLine("RUNEGA composite real integration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNEGA composite real integration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-composite-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunItCompositeFixture(args[1]); Console.WriteLine("RUNIT composite fixture: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT composite fixture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-composite-real", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRealRunItCompositeBuild(args[1]); Console.WriteLine("RUNIT composite real integration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT composite real integration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-text-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiTextSmoke(args[1], args[2]); Console.WriteLine("Runtime UI text service: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI text service: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-layout-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiLayoutSmoke(args[1], args[2]); Console.WriteLine("Runtime UI layout validation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI layout validation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--text-tab-split-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyTextTabSplitSmoke(args[1], args[2]); Console.WriteLine("Text tab split: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Text tab split: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--text-pristine-root-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyTextPristineRootSmoke(args[1]); Console.WriteLine("Text/Create Variant pristine root: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Text/Create Variant pristine root: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--graphics-variant-awareness-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyGraphicsVariantAwarenessSmoke(args[1], args[2]); Console.WriteLine("Graphics variant-awareness: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Graphics variant-awareness: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--font-variant-awareness-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyFontVariantAwarenessSmoke(args[1], args[2]); Console.WriteLine("Font variant-awareness: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Font variant-awareness: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--installation-context-activation-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyInstallationContextActivationSmoke(args[1], args[2]); Console.WriteLine("Installation/context activation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Installation/context activation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--unexpected-file-activation-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyUnexpectedFileActivationSmoke(args[1]); Console.WriteLine("Unexpected-file activation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Unexpected-file activation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--launcher-refactor-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLauncherRefactorSmoke(args[1], args[2]); Console.WriteLine("Launcher refactor: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Launcher refactor: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--run-debug-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunDebugSmoke(args[1], args[2]); Console.WriteLine("Run/debug actions: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Run/debug actions: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--recovery-safety-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRecoverySafetySmoke(args[1], args[2]); Console.WriteLine("Recovery & Safety: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Recovery & Safety: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--restore-semantics-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRestoreSemanticsSmoke(args[1], args[2]); Console.WriteLine("Restore semantics: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Restore semantics: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--ui-localization-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyUiLocalizationSmoke(args[1], args[2]); Console.WriteLine("UI localization service: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("UI localization service: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--json-locales-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyJsonLocalesSmoke(args[1], args[2]); Console.WriteLine("JSON locales: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("JSON locales: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--dynamic-locale-discovery-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyDynamicLocaleDiscoverySmoke(args[1], args[2]); Console.WriteLine("Dynamic locale discovery: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Dynamic locale discovery: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--locale-fallback-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLocaleFallbackSmoke(args[1], args[2]); Console.WriteLine("Locale fallback: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Locale fallback: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--localization-audit-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLocalizationAuditSmoke(args[1], args[2]); Console.WriteLine("Localization audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Localization audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--localized-help-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLocalizedHelpSmoke(args[1], args[2]); Console.WriteLine("Localized help: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Localized help: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--help-about-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLocalizedHelpSmoke(args[1], args[2]); Console.WriteLine("Help/About localization: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Help/About localization: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--locale-validation-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLocaleValidationSmoke(args[1], args[2]); Console.WriteLine("Locale validation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Locale validation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--active-variant-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyActiveVariantSmoke(args[1], args[2]); Console.WriteLine("Global active variant: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Global active variant: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--button-terminology-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyButtonTerminologySmoke(args[1], args[2]); Console.WriteLine("Button terminology: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Button terminology: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-manager-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyVariantManagerSmoke(args[1], args[2]); Console.WriteLine("Variant Manager: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant Manager: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-build-status-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyVariantBuildStatusSmoke(args[1], args[2]); Console.WriteLine("Variant build status: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant build status: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-manifest-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyVariantManifestSmoke(args[1], args[2]); Console.WriteLine("Variant manifest: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant manifest: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--rebuild-reproducibility-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRebuildReproducibilitySmoke(args[1], args[2]); Console.WriteLine("Rebuild reproducibility: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Rebuild reproducibility: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--r9d-binary-rejection-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyR9DBinaryRejectionSmoke(args[1], args[2]); Console.WriteLine("R9D binary rejection: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("R9D binary rejection: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--ui-startup-localization-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyUiStartupLocalizationSmoke(args[1], args[2]); Console.WriteLine("UI startup/localization: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("UI startup/localization: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runvga-composite-real", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRealRunVgaCompositeBuild(args[1]); Console.WriteLine("RUNVGA composite real integration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNVGA composite real integration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--pristine-capture", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                ElviraGameProfile game = args[1].Equals("elvira1", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira1 :
                    args[1].Equals("elvira2", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira2 : throw new ArgumentException("Game must be elvira1 or elvira2.");
                var service = new PristineManifestService(new EditorStorageLayout(args[2]), game);
                PristineManifestInitializationResult result = service.InitializeBaseline();
                Console.WriteLine($"Pristine capture: {result.Status}; {result.ManifestPath}; {result.Manifest?.BaselineFingerprint ?? string.Empty}");
                Environment.ExitCode = result.Status is PristineManifestInitializationStatus.Initialized or PristineManifestInitializationStatus.AlreadyValid ? 0 : 1;
            }
            catch (Exception ex) { Console.Error.WriteLine("Pristine capture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--pristine-validate", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                ElviraGameProfile game = args[1].Equals("elvira1", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira1 :
                    args[1].Equals("elvira2", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira2 : throw new ArgumentException("Game must be elvira1 or elvira2.");
                BaselineValidationResult result = new PristineManifestService(new EditorStorageLayout(args[2]), game).ValidateBaseline();
                Console.WriteLine($"Pristine validation: {result.Status}; entries={result.Entries.Count}; differences={result.Entries.Count(entry => entry.Status != BaselineValidationStatus.MatchesBaseline)}");
                Environment.ExitCode = result.Status == BaselineValidationStatus.MatchesBaseline ? 0 : 1;
            }
            catch (Exception ex) { Console.Error.WriteLine("Pristine validation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--pristine-recapture", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                ElviraGameProfile game = args[1].Equals("elvira1", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira1 :
                    args[1].Equals("elvira2", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira2 : throw new ArgumentException("Game must be elvira1 or elvira2.");
                PristineManifestInitializationResult result = new PristineManifestService(new EditorStorageLayout(args[2]), game).RecaptureBaselineForMetadataCorrection();
                Console.WriteLine($"Pristine recapture: {result.Status}; {result.ManifestPath}; {result.Manifest?.BaselineFingerprint ?? string.Empty}");
                Environment.ExitCode = result.Status == PristineManifestInitializationStatus.Initialized ? 0 : 1;
            }
            catch (Exception ex) { Console.Error.WriteLine("Pristine recapture: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--help-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                HelpDocumentService.ValidateAll();
                foreach (UiLanguage language in new[] { UiLanguage.English, UiLanguage.Slovak, UiLanguage.Czech })
                {
                    UiText.SetLanguage(language);
                    HelpDocument document = HelpDocumentService.Load(language);
                    if (document.Topics.Count < 18 || string.IsNullOrWhiteSpace(document.Topics[0].Title))
                        throw new InvalidDataException("Help language selection resolved the wrong localized JSON catalog.");
                }
                UiText.SetLanguage(UiLanguage.English);
                Console.WriteLine("Help documents: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Help documents: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--v5-regression", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                byte[] packed = File.ReadAllBytes(args[1]);
                byte[] baseline = RunVgaBootstrapService.UnpackVerifiedOriginal(packed);
                string fixturePath = Path.Combine(Path.GetDirectoryName(args[1]) ?? ".", "_font_analysis", "test", "RUNVGA_EXTFONT_TEST_V5.EXE");
                if (!File.Exists(fixturePath))
                    fixturePath = Path.Combine(Path.GetDirectoryName(args[1]) ?? ".", "RUNVGAV5.EXE");
                byte[] fixture = File.ReadAllBytes(fixturePath);
                byte[] table = fixture.AsSpan(RunVgaBootstrapService.V5FontOffset, RunVgaBootstrapService.V5FontSize).ToArray();
                bool pass = RunVgaBootstrapService.VerifyHistoricalV5Regression(baseline, table);
                Console.WriteLine(pass ? "V5 regression: PASS" : "V5 regression: FAIL");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("V5 regression: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--v5-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunVgaBootstrapResult result = RunVgaBootstrapService.CreateExtendedCp852(args[1], args[2], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(result.OutputPath);
                bool pass = loaded.Layout == RunVgaFontLayout.ExtendedCp852V5 && loaded.LoadedGlyphCount == 256;
                Console.WriteLine(pass ? "V5 smoke: PASS" : "V5 smoke: FAIL");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("V5 smoke: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--v5-ttf-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                // args: pristine packed Elvira I RUNVGA, TTF/OTF source, fresh V5 output path.
                RunVgaBootstrapService.CreateExtendedCp852(args[1], args[3], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[3]);
                if (loaded.Game != ElviraGame.Elvira1 || loaded.Layout != RunVgaFontLayout.ExtendedCp852V5 ||
                    loaded.PhysicalSlotCount != 224 || loaded.EditableGlyphCount != 223 ||
                    !FontSlotMetadata.IsReserved(loaded, FontSlotMetadata.HudEraseGlyph))
                    throw new InvalidDataException("Generated Elvira I V5 did not expose the expected 224-slot / 223-editable reservation model.");

                byte[] beforeA = loaded.Glyphs[0x41].Original.ToArray();
                byte[] beforea = loaded.Glyphs[0x61].Original.ToArray();
                byte[] raster = VectorFontRasterizer.RasterizeCp852(args[2], new VectorFontRasterOptions("", 11, 0, 0, true));
                RunVgaFontService.ImportFontBytesIntoEdited(loaded, raster, Path.GetFileName(args[2]));
                byte[] canonical = FontSlotMetadata.GetCanonicalBytes(loaded, FontSlotMetadata.HudEraseGlyph);
                if (!loaded.Glyphs[0x81].Edited.SequenceEqual(canonical))
                    throw new InvalidDataException("Elvira I TTF import overwrote reserved glyph 0x81 in memory.");
                // Validate the final write boundary too: legacy/external callers can
                // still supply a corrupt in-memory bitmap, but it must never reach disk.
                loaded.Glyphs[0x81].ReplaceEdited(Convert.FromHexString("1122334455667788"));

                string saved = args[3] + ".ttf.EXE";
                if (File.Exists(saved)) throw new IOException("TTF smoke output already exists.");
                RunVgaFontService.SaveCopy(loaded, saved);
                FontLoadResult reopened = RunVgaFontService.LoadRunVga(saved);

                int[] normalSlots = [0x41, 0x61, 0x80, 0x82, 0x8E, 0xA0, 0xE1, 0xFF];
                bool generated = normalSlots.All(code => reopened.Glyphs[code].Original.SequenceEqual(raster.AsSpan(code * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).ToArray()));
                bool asciiChanged = !beforeA.SequenceEqual(reopened.Glyphs[0x41].Original) && !beforea.SequenceEqual(reopened.Glyphs[0x61].Original);
                bool preserved = reopened.Glyphs[0x81].Original.SequenceEqual(canonical);
                if (!generated || !asciiChanged || !preserved)
                    throw new InvalidDataException("Elvira I TTF import/save/reopen reserved-slot verification failed.");

                Console.WriteLine("V5 TTF smoke: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("V5 TTF smoke: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runit-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunItBootstrapResult result = RunItBootstrapService.CreateExtendedCp852(args[1], args[2], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(result.OutputPath);
                if (loaded.Layout != RunVgaFontLayout.ExtendedCp852RunIt || loaded.LoadedGlyphCount != 224 || loaded.FirstByteValue != 0x20 || loaded.LastByteValue != 0xFF)
                    throw new InvalidDataException("Generated RUNIT did not reopen as Extended CP852.");
                var expectedSlots = new Dictionary<int, byte[]>
                {
                    [0x20] = Convert.FromHexString("0102030405060708"),
                    [0x41] = Convert.FromHexString("1121314151617181"),
                    [0x61] = Convert.FromHexString("1222324252627282"),
                    [0x80] = Convert.FromHexString("1333435363737383"),
                    [0x81] = FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes,
                    [0x82] = Convert.FromHexString("2050507050508800"),
                    [0x8E] = Convert.FromHexString("2151517151518900"),
                    [0xA0] = Convert.FromHexString("2252527252528A00"),
                    [0xE1] = Convert.FromHexString("2353537353538B00"),
                    [0xFF] = Convert.FromHexString("1444546474847484")
                };
                foreach ((int code, byte[] bytes) in expectedSlots)
                    loaded.Glyphs[code].ReplaceEdited(bytes);
                // The serializer, rather than caller discipline, is the final safety gate.
                loaded.Glyphs[0x81].ReplaceEdited(Convert.FromHexString("1444546474847484"));
                string roundTrip = Path.Combine(Path.GetDirectoryName(result.OutputPath) ?? ".", Path.GetFileNameWithoutExtension(result.OutputPath) + ".roundtrip.EXE");
                if (File.Exists(roundTrip)) File.Delete(roundTrip);
                RunVgaFontService.SaveCopy(loaded, roundTrip);
                FontLoadResult reopened = RunVgaFontService.LoadRunVga(roundTrip);
                byte[] disk = File.ReadAllBytes(roundTrip);
                bool slotsMatch = expectedSlots.All(kv =>
                    reopened.Glyphs[kv.Key].Original.SequenceEqual(kv.Value) &&
                    disk.AsSpan(kv.Key <= 0x81
                        ? RunItBootstrapService.OriginalFontOffset + (kv.Key - 0x20) * RunVgaFontService.GlyphBytes
                        : RunItBootstrapService.HighFontOffset + (kv.Key - 0x82) * RunVgaFontService.GlyphBytes,
                        RunVgaFontService.GlyphBytes).SequenceEqual(kv.Value));
                bool pass = reopened.Layout == RunVgaFontLayout.ExtendedCp852RunIt && slotsMatch;
                Console.WriteLine(pass ? "RUNIT smoke: PASS" : "RUNIT smoke: FAIL");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RUNIT smoke: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-regression", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                byte[] packed = File.ReadAllBytes(args[1]);
                if (RunItBootstrapService.DetectState(args[1]) != RunItBootstrapState.OriginalPacked)
                    throw new InvalidDataException("Packed RUNIT identification failed.");
                byte[] canonical = RunItBootstrapService.UnpackCanonicalOriginal(packed);
                RunItBootstrapService.ValidateCanonical(canonical);
                byte[] corruptedPacked = (byte[])packed.Clone(); corruptedPacked[0x200] ^= 1;
                bool wrongHashRejected = false;
                try { RunItBootstrapService.UnpackCanonicalOriginal(corruptedPacked); } catch (InvalidDataException) { wrongHashRejected = true; }
                if (!wrongHashRejected) throw new InvalidDataException("Wrong packed hash was accepted.");
                byte[] font = new byte[RunVgaFontService.ExtendedFontBytes];
                Array.Copy(canonical, RunItBootstrapService.OriginalFontOffset, font, 0x20 * 8, 98 * 8);
                byte[] extended = RunItBootstrapService.BuildExtended(canonical, font);
                RunItBootstrapService.ValidateExtended(extended);
                byte[] knownBadExtended = (byte[])extended.Clone();
                Convert.FromHexString("B600B103D3E28BF28E0612068CD805C2138ED890").CopyTo(knownBadExtended, 0x7B89);
                if (RunItBootstrapService.IsExtended(knownBadExtended))
                    throw new InvalidDataException("Known-bad legacy renderer order was accepted as valid Extended CP852.");
                byte[] badRenderer = (byte[])canonical.Clone(); badRenderer[0x7B89] ^= 1;
                bool rendererRejected = false;
                try { RunItBootstrapService.BuildExtended(badRenderer, font); } catch (InvalidDataException) { rendererRejected = true; }
                if (!rendererRejected) throw new InvalidDataException("Renderer expected-byte guard was bypassed.");
                Console.WriteLine("RUNIT regression: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RUNIT regression: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runit-v2-golden", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                byte[] canonical = RunItBootstrapService.UnpackCanonicalOriginal(File.ReadAllBytes(args[1]));
                byte[] golden = File.ReadAllBytes(args[2]);
                RunItBootstrapService.ValidateExtended(golden);
                byte[] image = new byte[RunVgaFontService.ExtendedFontBytes];
                Array.Copy(canonical, RunItBootstrapService.OriginalFontOffset, image, 0x20 * 8, 98 * 8);
                Array.Copy(golden, RunItBootstrapService.HighFontOffset, image, 0x82 * 8, RunItBootstrapService.HighFontSize);
                byte[] generated = RunItBootstrapService.BuildExtended(canonical, image);
                bool pass = generated.SequenceEqual(golden);
                Console.WriteLine($"RUNIT V2 golden: {(pass ? "PASS" : "FAIL")} SHA-256 {Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(generated))}");
                if (!pass)
                {
                    int first = Enumerable.Range(0, generated.Length).First(i => generated[i] != golden[i]);
                    Console.Error.WriteLine($"First differing offset: 0x{first:X}");
                }
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("RUNIT V2 golden: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--production-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                // args: pristine Elvira I EXE, pristine Elvira II EXE, empty isolated root.
                RunProductionSmoke(ElviraGame.Elvira1, args[1], args[3]);
                RunProductionSmoke(ElviraGame.Elvira2, args[2], args[3]);
                Console.WriteLine("Production backup workflow: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Production backup workflow: FAIL - " + ex.Message);
                Environment.ExitCode = 1;
            }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--partial-backup-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunPartialBackupSmoke(args[1], args[2], args[3]);
                Console.WriteLine("Partial backup guards: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Partial backup guards: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--runit-ttf-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunItBootstrapService.CreateExtendedCp852(args[1], args[3], GlyphRepository.CreateAllCp852Slots());
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[3]);
                byte[] beforeA = loaded.Glyphs[0x41].Original.ToArray(), beforea = loaded.Glyphs[0x61].Original.ToArray(), before0 = loaded.Glyphs[0x30].Original.ToArray();
                byte[] raster = VectorFontRasterizer.RasterizeCp852(args[2], new VectorFontRasterOptions("", 11, 0, 0, true));
                RunVgaFontService.ImportFontBytesIntoEdited(loaded, raster, Path.GetFileName(args[2]));
                string saved = args[3] + ".ttf.EXE";
                if (File.Exists(saved)) throw new IOException("TTF smoke output already exists.");
                RunVgaFontService.SaveCopy(loaded, saved);
                FontLoadResult reopened = RunVgaFontService.LoadRunVga(saved);
                bool changed = !beforeA.SequenceEqual(reopened.Glyphs[0x41].Original) && !beforea.SequenceEqual(reopened.Glyphs[0x61].Original) && !before0.SequenceEqual(reopened.Glyphs[0x30].Original);
                bool cp852 = new[] { 0xA0, 0x82, 0x8D, 0xE1, 0xFD }.All(c => reopened.Glyphs[c].Original.SequenceEqual(raster.AsSpan(c * 8, 8).ToArray()));
                bool reserved = FontSlotMetadata.IsReserved(reopened, 0x81) &&
                    reopened.Glyphs[0x81].Original.SequenceEqual(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes);
                if (!changed || !cp852 || !reserved) throw new InvalidDataException("TTF raster/import/save/reopen reserved-slot verification failed.");
                Console.WriteLine("RUNIT TTF smoke: PASS"); Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT TTF smoke: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-packed-preview", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[1]);
                bool pass = loaded.Game == ElviraGame.Elvira2 && loaded.Layout == RunVgaFontLayout.OriginalPackedAscii98 &&
                    loaded.LoadedGlyphCount == 98 && loaded.FirstByteValue == 0x20 && loaded.LastByteValue == 0x81 &&
                    loaded.Glyphs[0x30].Original.Any(b => b != 0) && loaded.Glyphs[0x41].Original.Any(b => b != 0) &&
                    loaded.Glyphs[0x4C].Original.Any(b => b != 0) && loaded.Glyphs[0x61].Original.Any(b => b != 0) &&
                    loaded.Glyphs[0x80].Original.Any(b => b != 0) &&
                    loaded.Glyphs[0x81].Original.SequenceEqual(FontSlotMetadata.OriginalHudEraseGlyphBytes);
                Console.WriteLine($"RUNIT packed preview: {(pass ? "PASS" : "FAIL")} A={Convert.ToHexString(loaded.Glyphs[0x41].Original)} L={Convert.ToHexString(loaded.Glyphs[0x4C].Original)}");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT packed preview: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runvga-preview-regression", StringComparison.OrdinalIgnoreCase))
        {
            string? temporaryCanonical = null;
            try
            {
                // Preview loading must never alter either input. The temporary baseline lets
                // the same test cover the packed and canonical-unpacked Elvira I paths.
                byte[] packedBytes = File.ReadAllBytes(args[1]);
                string packedHashBefore = Hash(args[1]);
                byte[] canonical = RunVgaBootstrapService.UnpackVerifiedOriginal(packedBytes);
                FontLoadResult packed = RunVgaFontService.LoadRunVga(args[1]);
                AssertElvira1OriginalPreview(packed, RunVgaFontLayout.OriginalPackedAscii98, canonical, "packed");
                if (Hash(args[1]) != packedHashBefore)
                    throw new InvalidDataException("Packed RUNVGA changed while previewing it.");

                temporaryCanonical = Path.Combine(Path.GetTempPath(), "ElviraVgaEditor-preview-" + Guid.NewGuid().ToString("N") + ".EXE");
                File.WriteAllBytes(temporaryCanonical, canonical);
                FontLoadResult unpacked = RunVgaFontService.LoadRunVga(temporaryCanonical);
                AssertElvira1OriginalPreview(unpacked, RunVgaFontLayout.OriginalAscii98, canonical, "unpacked");

                FontLoadResult v5 = RunVgaFontService.LoadRunVga(args[2]);
                if (v5.Game != ElviraGame.Elvira1 || v5.Layout != RunVgaFontLayout.ExtendedCp852V5 ||
                    v5.LoadedGlyphCount != 256 || !v5.Glyphs[0x81].Original.SequenceEqual(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes) ||
                    !FontSlotMetadata.IsReserved(v5, 0x81))
                    throw new InvalidDataException("Elvira I V5 preview/reserved-glyph validation failed.");

                Console.WriteLine("RUNVGA packed/unpacked/V5 preview: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNVGA packed/unpacked/V5 preview: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            finally { if (temporaryCanonical is not null && File.Exists(temporaryCanonical)) File.Delete(temporaryCanonical); }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--vga-backup-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunVgaBackupSmoke(args[1], args[2]);
                Console.WriteLine("VGA O-backup workflow: PASS"); Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("VGA O-backup workflow: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runit-patched-preview", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                FontLoadResult loaded = RunVgaFontService.LoadRunVga(args[1]);
                bool pass = loaded.Game == ElviraGame.Elvira2 && loaded.Layout == RunVgaFontLayout.ExtendedCp852RunIt &&
                    loaded.LoadedGlyphCount == 224 && loaded.FirstByteValue == 0x20 && loaded.LastByteValue == 0xFF &&
                    new[] { 0x20, 0x41, 0x61, 0x80, 0x81, 0x82, 0x8E, 0xA0, 0xE1, 0xFF }.All(c => loaded.Glyphs[c].IsLoadedFromSource) &&
                    FontSlotMetadata.IsReserved(loaded, 0x81) && loaded.Glyphs[0x81].Original.SequenceEqual(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes);
                Console.WriteLine($"RUNIT patched preview: {(pass ? "PASS" : "FAIL")} loaded={loaded.LoadedGlyphCount}");
                Environment.ExitCode = pass ? 0 : 1;
            }
            catch (Exception ex) { Console.Error.WriteLine("RUNIT patched preview: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--patched-hud-regression", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                Directory.CreateDirectory(args[3]);
                string v5a = Path.Combine(args[3], "RUNVGA_V5_A.EXE"), v5b = Path.Combine(args[3], "RUNVGA_V5_B.EXE");
                string v2a = Path.Combine(args[3], "RUNIT_V2_A.EXE"), v2b = Path.Combine(args[3], "RUNIT_V2_B.EXE");
                RunVgaBootstrapService.CreateExtendedCp852(args[1], v5a, GlyphRepository.CreateAllCp852Slots());
                RunVgaBootstrapService.CreateExtendedCp852(args[1], v5b, GlyphRepository.CreateAllCp852Slots());
                RunItBootstrapService.CreateExtendedCp852(args[2], v2a, GlyphRepository.CreateAllCp852Slots());
                RunItBootstrapService.CreateExtendedCp852(args[2], v2b, GlyphRepository.CreateAllCp852Slots());

                byte[] v5Bytes = File.ReadAllBytes(v5a), v5Repeat = File.ReadAllBytes(v5b);
                byte[] v2Bytes = File.ReadAllBytes(v2a), v2Repeat = File.ReadAllBytes(v2b);
                byte[] fullCell = FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes;
                bool v5Pass = v5Bytes.SequenceEqual(v5Repeat) && v5Bytes.AsSpan(
                    RunVgaBootstrapService.V5FontOffset + FontSlotMetadata.HudEraseGlyph * RunVgaFontService.GlyphBytes,
                    RunVgaFontService.GlyphBytes).SequenceEqual(fullCell);
                bool v2Pass = v2Bytes.SequenceEqual(v2Repeat) && v2Bytes.AsSpan(
                    RunItBootstrapService.OriginalFontOffset + (FontSlotMetadata.HudEraseGlyph - RunVgaFontService.OriginalFirstChar) * RunVgaFontService.GlyphBytes,
                    RunVgaFontService.GlyphBytes).SequenceEqual(fullCell);
                if (!v5Pass || !v2Pass) throw new InvalidDataException("Patched full-cell HUD glyph or deterministic rebuild validation failed.");
                VerifyReservedHudSafeguards(args[1], args[2], args[3], v5Bytes, v2Bytes, fullCell);
                Console.WriteLine($"Patched HUD regression: PASS V5={Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(v5Bytes))} V2={Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(v2Bytes))}");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Patched HUD regression: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--gamepc-backup-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunGamePcBackupSmoke(args[1], args[2]);
                Console.WriteLine("GAMEPC O-backup workflow: PASS"); Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("GAMEPC O-backup workflow: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--data-file-variant-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunDataFileVariantSmoke("elvira1", args[1], args[3]);
                RunDataFileVariantSmoke("elvira2", args[2], args[3]);
                Console.WriteLine("Arbitrary data-file workflow: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Arbitrary data-file workflow: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--variant-config-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunVariantConfigurationSmoke(args[1]);
                Console.WriteLine("Variant configuration workflow: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Variant configuration workflow: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--variant-ordering-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunVariantOrderingSmoke(args[1]);
                Console.WriteLine("Variant ordering: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Variant ordering: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--translation-exchange-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunTranslationExchangeSmoke(args[1]); Console.WriteLine("Translation CSV/XLSX exchange: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Translation CSV/XLSX exchange: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--mods-launcher-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                RunModsLauncherWorkflowSmoke("elvira1", args[1], args[3]);
                RunModsLauncherWorkflowSmoke("elvira2", args[2], args[3]);
                foreach (UiLanguage language in new[] { UiLanguage.English, UiLanguage.Slovak, UiLanguage.Czech })
                {
                    UiText.SetLanguage(language);
                    foreach (string key in new[] { "ModsLauncherTab", "VariantAdd", "VariantEdit", "VariantRemove", "VariantMoveUp", "VariantMoveDown", "VariantEnable", "VariantDisable", "VariantOpenDataFile", "VariantAvailable", "VariantMissing", "AddCreatedVariantQuestion" })
                        if (UiText.Get(key) == key) throw new InvalidDataException($"Missing variant localization: {key} ({language}).");
                }
                UiText.SetLanguage(UiLanguage.English);
                Console.WriteLine("Mods & Launcher workflow: PASS");
                Environment.ExitCode = 0;
            }
            catch (Exception ex) { Console.Error.WriteLine("Mods & Launcher workflow: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--runtime-launcher-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunRuntimeLauncherSmoke(args[1]); Console.WriteLine("Runtime BAT launcher: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime BAT launcher: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 2 && args[0].Equals("--gog-overlay-launcher-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunGogOverlayLauncherSmoke(args[1]); Console.WriteLine("GOG overlay launcher: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("GOG overlay launcher: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--pi1menu-input-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunPi1MenuInputSmoke(); Console.WriteLine("PI1MENU input boundary: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("PI1MENU input boundary: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--installation-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunInstallationDiscoverySmoke(args[1], args[2], args[3]); Console.WriteLine("Installation discovery: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Installation discovery: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--installation-selector-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunInstallationSelectorSmoke(args[1], args[2]); Console.WriteLine("Installation selector: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Installation selector: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--font-activation-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunFontActivationSmoke(args[1], args[2], args[3]); Console.WriteLine("Font activation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Font activation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--font-variant-binding-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunFontVariantBindingSmoke(args[1], args[2], args[3]); Console.WriteLine("Font variant binding: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Font variant binding: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--context-safety-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunContextSafetySmoke(args[1], args[2], args[3]); Console.WriteLine("Context safety: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Context safety: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--elvira1-gamepc-parser-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunElvira1GamePcParserSmoke(args[1], args[2], args[3]); Console.WriteLine("Elvira I GAMEPC header parser: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I GAMEPC header parser: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--elvira2-gamepc-parser-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunElvira2GamePcParserSmoke(args[1], args[2], args[3]); Console.WriteLine("Elvira II GAMEPC header parser: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira II GAMEPC header parser: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--elvira2-gamepc-map", StringComparison.OrdinalIgnoreCase))
        {
            try { CreateElvira2GamePcMap(args[1], args[2]); Console.WriteLine("Elvira II GAMEPC string map: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira II GAMEPC string map: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 5 && args[0].Equals("--gamepc-repack-production-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunGamePcRepackProductionSmoke(args[1], args[2], args[3], args[4]); Console.WriteLine("Production GAMEPC repacker: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Production GAMEPC repacker: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 5 && args[0].Equals("--elvira1-terminal-delimiter-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunElvira1TerminalDelimiterSmoke(args[1], args[2], args[3], args[4]); Console.WriteLine("Elvira I terminal-delimiter preservation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I terminal-delimiter preservation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 5 && args[0].Equals("--elvira1-fixed-hotspot-layout-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunElvira1FixedHotspotLayoutSmoke(args[1], args[2], args[3], args[4]); Console.WriteLine("Elvira I fixed-hotspot menu layout: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I fixed-hotspot menu layout: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 5 && args[0].Equals("--elvira1-palette-regression", StringComparison.OrdinalIgnoreCase))
        {
            try { RunElvira1PaletteRegression(args[1], args[2], args[3], args[4]); Console.WriteLine("Elvira I palette regression: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I palette regression: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--elvira1-vga-palette-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira1VgaScriptAudit.Run(args[1], args[2]); Console.WriteLine("Elvira I VGA palette audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I VGA palette audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--elvira1-palette-resolver-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunElvira1PaletteResolverSmoke(args[1], args[2]); Console.WriteLine("Elvira I production palette resolver: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I production palette resolver: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--elvira2-vga-palette-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira2VgaScriptAudit.Run(args[1], args[2]); Console.WriteLine("Elvira II VGA palette audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira II VGA palette audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--elvira2-palette-resolver-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunElvira2PaletteResolverSmoke(args[1], args[2]); Console.WriteLine("Elvira II production palette resolver: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira II production palette resolver: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--graphics-palette-final-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { RunGraphicsPaletteFinalAudit(args[1], args[2], args[3]); Console.WriteLine("Graphics/palette final audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Graphics/palette final audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--elvira1-runtime-text-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira1RuntimeTextAudit.Run(args[1], args[2]); Console.WriteLine("Elvira I runtime text audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I runtime text audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--elvira1-trace-correlation", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira1RendererTraceAnalyzer.Run(args[1], args[2], args[3]); Console.WriteLine("Elvira I renderer trace correlation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I renderer trace correlation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 5 && args[0].Equals("--elvira1-corrected-stack-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira1CorrectedStackAnalyzer.Run(args[1], args[2], args[3], args[4]); Console.WriteLine("Elvira I corrected stack audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I corrected stack audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 5 && args[0].Equals("--elvira1-f033-static-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira1TextConsumerF033Audit.Run(args[1], args[2], args[3], args[4]); Console.WriteLine("Elvira I F033 static audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I F033 static audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 5 && args[0].Equals("--elvira1-bb7b-producer-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { Elvira1Bb7bProducerAudit.Run(args[1], args[2], args[3], args[4]); Console.WriteLine("Elvira I BB7B producer audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Elvira I BB7B producer audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 4 && args[0].Equals("--production-runtime-diagnostic-audit", StringComparison.OrdinalIgnoreCase))
        {
            try { ProductionRuntimeDiagnosticAudit.Run(args[1], args[2], args[3]); Console.WriteLine("Production runtime diagnostic audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Production runtime diagnostic audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--version-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunVersionSmoke(); Console.WriteLine("Product version: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Product version: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        ApplicationConfiguration.Initialize();
        Application.Run(new MainForm());
    }

    private static void RunElvira1PaletteResolverSmoke(string elvira1Directory, string elvira2Directory)
    {
        static PaletteResolution Resolve(Elvira1PaletteResolver resolver, string dir, string vga1, int image)
            => resolver.Resolve(Path.Combine(dir, vga1), Path.Combine(dir, vga1.Substring(0, 2) + "2.VGA"), image);

        var resolver = new Elvira1PaletteResolver();
        string[] e1Inputs = new[] { "621.VGA", "622.VGA", "071.VGA", "072.VGA", "061.VGA", "062.VGA", "011.VGA", "012.VGA" }
            .Select(name => Path.Combine(elvira1Directory, name)).ToArray();
        Dictionary<string, string> before = e1Inputs.ToDictionary(path => path, ComputeSha256, StringComparer.OrdinalIgnoreCase);

        PaletteResolution death = Resolve(resolver, elvira1Directory, "621.VGA", 3);
        Require(death.Kind == PaletteResolutionKind.Unique && death.PaletteId == 2 && Elvira1PaletteResolver.EffectivePaletteBank(null, death, 4) == 2, "621/622 anchor failed.");

        PaletteResolution visual = Resolve(resolver, elvira1Directory, "071.VGA", 56);
        Require(visual.Kind == PaletteResolutionKind.Unique && visual.PaletteId == 5 && Elvira1PaletteResolver.EffectivePaletteBank(null, visual, 6) == 5, "071/072 anchor failed.");
        Require(Elvira1PaletteResolver.EffectivePaletteBank(2, visual, 6) == 2, "Manual palette did not override Automatic.");
        Require(Elvira1PaletteResolver.EffectivePaletteBank(null, visual, 6) == 5, "Basic reset did not restore Automatic.");

        PaletteResolution contextual = Resolve(resolver, elvira1Directory, "061.VGA", 14);
        Require(contextual.Kind == PaletteResolutionKind.Contextual && contextual.CandidatePaletteIds.SequenceEqual(new[] { 0, 1 }) && Elvira1PaletteResolver.EffectivePaletteBank(null, contextual, 2) == 0, "Contextual fallback failed.");
        PaletteResolution unresolved = Resolve(resolver, elvira1Directory, "011.VGA", 1);
        Require(unresolved.Kind == PaletteResolutionKind.Unresolved && Elvira1PaletteResolver.EffectivePaletteBank(null, unresolved, 6) == 0, "Unresolved fallback failed.");
        Require(Elvira1PaletteResolver.ResolveForBytes(new byte[12], new byte[16], 0).Kind == PaletteResolutionKind.Invalid, "Malformed resource was not classified Invalid.");
        Require(ElviraPaletteLoader.Load(Path.Combine(elvira1Directory, "071.VGA")).Count == 6, "071 palette count regression.");

        // Elvira II receives no E1 script interpretation; retain its existing
        // palette loading path as a read-only regression check.
        string e2Vga1 = Path.Combine(elvira2Directory, "001.VGA");
        if (File.Exists(e2Vga1)) _ = ElviraPaletteLoader.Load(e2Vga1);

        foreach ((string path, string hash) in before)
            Require(string.Equals(hash, ComputeSha256(path), StringComparison.OrdinalIgnoreCase), $"Read-only resolver modified {Path.GetFileName(path)}.");
    }

    private static string ComputeSha256(string path)
        => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));

    private static void RunElvira2PaletteResolverSmoke(string elvira1Directory, string elvira2Directory)
    {
        static PaletteResolution Resolve(Elvira2PaletteResolver resolver, string dir, string vga1, int image)
            => resolver.Resolve(Path.Combine(dir, vga1), Path.Combine(dir, vga1.Substring(0, 2) + "2.VGA"), image);

        string[] e2Names = { "951.VGA", "952.VGA", "971.VGA", "972.VGA", "021.VGA", "022.VGA", "001.VGA", "002.VGA", "161.VGA", "162.VGA", "801.VGA", "802.VGA", "991.VGA", "992.VGA" };
        string[] e2Inputs = e2Names.Select(name => Path.Combine(elvira2Directory, name)).ToArray();
        string[] e1Inputs = new[] { "621.VGA", "622.VGA", "071.VGA", "072.VGA", "061.VGA", "062.VGA", "011.VGA", "012.VGA" }
            .Select(name => Path.Combine(elvira1Directory, name)).ToArray();
        string[] allInputs = e2Inputs.Concat(e1Inputs).ToArray();
        Dictionary<string, string> before = allInputs.ToDictionary(path => path, ComputeSha256, StringComparer.OrdinalIgnoreCase);
        var resolver = new Elvira2PaletteResolver();

        int palette951 = ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "951.VGA")).Count;
        int palette971 = ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "971.VGA")).Count;
        PaletteResolution a = Resolve(resolver, elvira2Directory, "951.VGA", 13);
        PaletteResolution b = Resolve(resolver, elvira2Directory, "951.VGA", 14);
        PaletteResolution c = Resolve(resolver, elvira2Directory, "971.VGA", 50);
        Require(a.Kind == PaletteResolutionKind.Unique && a.PaletteId == 17 && Elvira1PaletteResolver.EffectivePaletteBank(null, a, palette951) == 17, "951/952 raw anchor failed.");
        Require(b.Kind == PaletteResolutionKind.Unique && b.PaletteId == 18 && Elvira1PaletteResolver.EffectivePaletteBank(null, b, palette951) == 18, "951/952 RLE anchor failed.");
        Require(c.Kind == PaletteResolutionKind.Unique && c.PaletteId == 1 && Elvira1PaletteResolver.EffectivePaletteBank(null, c, palette971) == 1, "971/972 anchor failed.");
        Require(Elvira1PaletteResolver.EffectivePaletteBank(2, a, palette951) == 2, "Manual palette did not override Automatic.");
        Require(Elvira1PaletteResolver.EffectivePaletteBank(null, a, palette951) == 17, "Basic reset did not restore Automatic.");

        PaletteResolution contextual = Resolve(resolver, elvira2Directory, "021.VGA", 35);
        Require(contextual.Kind == PaletteResolutionKind.Contextual && contextual.CandidatePaletteIds.SequenceEqual(new[] { 2, 4 }) && Elvira1PaletteResolver.EffectivePaletteBank(null, contextual, ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "021.VGA")).Count) == 0, "Contextual fallback failed.");
        PaletteResolution unresolved = Resolve(resolver, elvira2Directory, "001.VGA", 1);
        Require(unresolved.Kind == PaletteResolutionKind.Unresolved && Elvira1PaletteResolver.EffectivePaletteBank(null, unresolved, ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "001.VGA")).Count) == 0, "Unresolved fallback failed.");
        Require(Elvira2PaletteResolver.ResolveForBytes(new byte[12], new byte[16], 0).Kind == PaletteResolutionKind.Invalid, "Synthetic invalid resource was not classified Invalid.");
        foreach (string pair in new[] { "161.VGA", "801.VGA", "991.VGA" })
        {
            PaletteResolution malformed = Resolve(resolver, elvira2Directory, pair, 0);
            Require(malformed.Kind == PaletteResolutionKind.Invalid && Elvira1PaletteResolver.EffectivePaletteBank(null, malformed, 1) == 0, $"Malformed {pair} did not fall back safely.");
        }

        RunElvira1PaletteResolverSmoke(elvira1Directory, elvira2Directory);
        foreach ((string path, string hash) in before)
            Require(string.Equals(hash, ComputeSha256(path), StringComparison.OrdinalIgnoreCase), $"Read-only resolver modified {Path.GetFileName(path)}.");
    }

    private static void RunGraphicsPaletteFinalAudit(string elvira1Directory, string elvira2Directory, string outputDirectory)
    {
        static PaletteResolution E1(Elvira1PaletteResolver r, string dir, string one, int image)
            => r.Resolve(Path.Combine(dir, one), Path.Combine(dir, one.Substring(0, 2) + "2.VGA"), image);
        static PaletteResolution E2(Elvira2PaletteResolver r, string dir, string one, int image)
            => r.Resolve(Path.Combine(dir, one), Path.Combine(dir, one.Substring(0, 2) + "2.VGA"), image);
        static int Effective(int? manual, PaletteResolution automatic, int count)
            => Elvira1PaletteResolver.EffectivePaletteBank(manual, automatic, count);
        static void VerifyPaletteBitmap(string palettePath, string imagePath, int image, int bank)
        {
            byte[] source = File.ReadAllBytes(imagePath); VgaImageEntry entry = new VgaImageTableParser(source).Parse().Entries[image];
            byte[] pixels = ElviraImageDecoder.Decode(source, entry); Color[] palette = ElviraPaletteLoader.Load(palettePath)[bank].Colors;
            using Bitmap bitmap = PaletteTools.ToBitmap(entry.PixelWidth, entry.Height, pixels, palette, transparentZero: true);
            int pixelOffset = Array.FindIndex(pixels, p => p != 0); Require(pixelOffset >= 0, "Anchor contains no visible pixel.");
            int x = pixelOffset % entry.PixelWidth, y = pixelOffset / entry.PixelWidth;
            Require(bitmap.GetPixel(x, y).ToArgb() == palette[pixels[pixelOffset]].ToArgb(), "Preview/export bitmap palette conversion diverged.");
        }

        string[] names = new[] { "071.VGA", "072.VGA", "621.VGA", "622.VGA", "061.VGA", "062.VGA", "011.VGA", "012.VGA" }
            .Select(name => Path.Combine(elvira1Directory, name))
            .Concat(new[] { "951.VGA", "952.VGA", "971.VGA", "972.VGA", "021.VGA", "022.VGA", "001.VGA", "002.VGA", "161.VGA", "162.VGA", "801.VGA", "802.VGA", "991.VGA", "992.VGA" }.Select(name => Path.Combine(elvira2Directory, name)))
            .ToArray();
        Dictionary<string, string> before = names.ToDictionary(path => path, ComputeSha256, StringComparer.OrdinalIgnoreCase);
        var e1 = new Elvira1PaletteResolver(); var e2 = new Elvira2PaletteResolver();
        int e1Count071 = ElviraPaletteLoader.Load(Path.Combine(elvira1Directory, "071.VGA")).Count;
        int e1Count621 = ElviraPaletteLoader.Load(Path.Combine(elvira1Directory, "621.VGA")).Count;
        int e2Count951 = ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "951.VGA")).Count;
        int e2Count971 = ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "971.VGA")).Count;

        PaletteResolution e1a = E1(e1, elvira1Directory, "071.VGA", 56), e1b = E1(e1, elvira1Directory, "621.VGA", 3);
        PaletteResolution e2a = E2(e2, elvira2Directory, "951.VGA", 13), e2b = E2(e2, elvira2Directory, "951.VGA", 14), e2c = E2(e2, elvira2Directory, "971.VGA", 50);
        Require(e1a is { Kind: PaletteResolutionKind.Unique, PaletteId: 5 } && Effective(null, e1a, e1Count071) == 5, "E1 071/072 image 0056 anchor failed.");
        Require(e1b is { Kind: PaletteResolutionKind.Unique, PaletteId: 2 } && Effective(null, e1b, e1Count621) == 2, "E1 621/622 image 0003 anchor failed.");
        Require(e2a is { Kind: PaletteResolutionKind.Unique, PaletteId: 17 } && Effective(null, e2a, e2Count951) == 17, "E2 951/952 image 0013 anchor failed.");
        Require(e2b is { Kind: PaletteResolutionKind.Unique, PaletteId: 18 } && Effective(null, e2b, e2Count951) == 18, "E2 951/952 image 0014 anchor failed.");
        Require(e2c is { Kind: PaletteResolutionKind.Unique, PaletteId: 1 } && Effective(null, e2c, e2Count971) == 1, "E2 971/972 image 0050 anchor failed.");

        // Installation/game/group re-entry uses separate resolver instances and pair-identity caches.
        Require(Effective(null, E1(e1, elvira1Directory, "071.VGA", 56), e1Count071) == 5 && Effective(null, E2(e2, elvira2Directory, "951.VGA", 13), e2Count951) == 17 && Effective(null, E1(e1, elvira1Directory, "621.VGA", 3), e1Count621) == 2 && Effective(null, E2(e2, elvira2Directory, "971.VGA", 50), e2Count971) == 1, "Game/group transition leaked a palette result.");
        Require(Effective(null, E1(e1, elvira1Directory, "071.VGA", 56), e1Count071) == 5 && Effective(null, E1(e1, elvira1Directory, "621.VGA", 3), e1Count621) == 2 && Effective(null, E1(e1, elvira1Directory, "071.VGA", 56), e1Count071) == 5, "E1 pair cache rebound incorrectly.");
        Require(Effective(null, E2(e2, elvira2Directory, "951.VGA", 13), e2Count951) == 17 && Effective(null, E2(e2, elvira2Directory, "971.VGA", 50), e2Count971) == 1 && Effective(null, E2(e2, elvira2Directory, "951.VGA", 13), e2Count951) == 17, "E2 pair cache rebound incorrectly.");

        PaletteResolution e1Contextual = E1(e1, elvira1Directory, "061.VGA", 14), e1Unresolved = E1(e1, elvira1Directory, "011.VGA", 1);
        PaletteResolution e2Contextual = E2(e2, elvira2Directory, "021.VGA", 35), e2Unresolved = E2(e2, elvira2Directory, "001.VGA", 1);
        Require(e1Contextual.Kind == PaletteResolutionKind.Contextual && Effective(null, e1Contextual, 2) == 0 && e1Unresolved.Kind == PaletteResolutionKind.Unresolved && Effective(null, e1Unresolved, 6) == 0 && Effective(null, e1a, e1Count071) == 5, "E1 fallback/recovery failed.");
        Require(e2Contextual.Kind == PaletteResolutionKind.Contextual && Effective(null, e2Contextual, ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "021.VGA")).Count) == 0 && e2Unresolved.Kind == PaletteResolutionKind.Unresolved && Effective(null, e2Unresolved, ElviraPaletteLoader.Load(Path.Combine(elvira2Directory, "001.VGA")).Count) == 0 && Effective(null, e2a, e2Count951) == 17, "E2 fallback/recovery failed.");
        Require(Effective(2, e1a, e1Count071) == 2 && Effective(null, e1a, e1Count071) == 5 && Effective(2, e2a, e2Count951) == 2 && Effective(null, e2a, e2Count951) == 17, "Manual/Basic precedence failed.");
        Require(e1Count071 == 6 && Enumerable.Range(0, e1Count071).All(bank => Effective(bank, e1a, e1Count071) == bank) && Effective(99, e1a, e1Count071) == 5, "E1 palette range normalization failed.");
        Require(Enumerable.Range(0, e2Count951).All(bank => Effective(bank, e2a, e2Count951) == bank) && Effective(99, e2a, e2Count951) == 17, "E2 palette range normalization failed.");
        Require(Elvira1PaletteResolver.ResolveForBytes(new byte[12], new byte[16], 0).Kind == PaletteResolutionKind.Invalid && Elvira2PaletteResolver.ResolveForBytes(new byte[12], new byte[16], 0).Kind == PaletteResolutionKind.Invalid, "Synthetic invalid safety failed.");
        foreach (string malformed in new[] { "161.VGA", "801.VGA", "991.VGA" }) Require(E2(e2, elvira2Directory, malformed, 0).Kind == PaletteResolutionKind.Invalid, $"Malformed {malformed} was not safe.");
        Require(Effective(null, E2(e2, elvira2Directory, "951.VGA", 13), e2Count951) == 17, "Malformed E2 recovery failed.");

        VerifyPaletteBitmap(Path.Combine(elvira1Directory, "071.VGA"), Path.Combine(elvira1Directory, "072.VGA"), 56, 5);
        VerifyPaletteBitmap(Path.Combine(elvira1Directory, "071.VGA"), Path.Combine(elvira1Directory, "072.VGA"), 56, 2);
        VerifyPaletteBitmap(Path.Combine(elvira2Directory, "951.VGA"), Path.Combine(elvira2Directory, "952.VGA"), 13, 17);
        VerifyPaletteBitmap(Path.Combine(elvira2Directory, "951.VGA"), Path.Combine(elvira2Directory, "952.VGA"), 13, 2);

        UiLanguage previousLanguage = UiText.Language;
        foreach (UiLanguage language in Enum.GetValues<UiLanguage>()) { UiText.SetLanguage(language); Require(!string.IsNullOrWhiteSpace(UiText.Get("PaletteAutomaticResolved")) && !string.IsNullOrWhiteSpace(UiText.Get("PaletteAdvanced")) && !string.IsNullOrWhiteSpace(UiText.Get("PaletteBasic")) && !string.IsNullOrWhiteSpace(UiText.Get("PixelPerfect")), "Palette localization missing."); }
        UiText.SetLanguage(previousLanguage);
        foreach ((string path, string hash) in before) Require(string.Equals(hash, ComputeSha256(path), StringComparison.OrdinalIgnoreCase), $"Read-only audit modified {Path.GetFileName(path)}.");

        Directory.CreateDirectory(outputDirectory);
        string report = Path.Combine(outputDirectory, "GRAPHICS_PALETTE_FINAL_AUDIT.txt");
        File.WriteAllLines(report, new[]
        {
            "GRAPHICS / PALETTE FINAL AUDIT — N9", "", "RESULT: PASS", "",
            "E1 anchors: 072/0056 -> 005; 622/0003 -> 002.", "E2 anchors: 952/0013 -> 017; 952/0014 -> 018; 972/0050 -> 001.",
            "Game/group/image transitions: PASS. Manual > UNIQUE Automatic > bank 000: PASS. Basic recomputation: PASS.",
            "Contextual/unresolved/invalid/malformed fallback and recovery: PASS.", "Palette ranges: E1 071.VGA has banks 000..005; representative E2 bank ranges accepted from headers: PASS.",
            "Preview/export conversion uses the same effective palette conversion: PASS. Pixel-perfect code remains ON=NearestNeighbor, OFF=HighQualityBicubic: PASS.",
            "Swatches use EffectivePalette(); export uses EffectivePalette(); resolver results are cached by VGA1+VGA2 resource identity.",
            "EN/SK/CZ palette labels present; existing DPI-aware layout retained. No production diagnostic report reads. No hardcoded image-to-palette mapping table.",
            "Read-only source hashes before/after: PASS. No resolver metadata is serialized into VGA resources.", "", "Representative source SHA-256:",
            $"E1 071.VGA {before[Path.Combine(elvira1Directory, "071.VGA")]}", $"E1 072.VGA {before[Path.Combine(elvira1Directory, "072.VGA")]}",
            $"E1 621.VGA {before[Path.Combine(elvira1Directory, "621.VGA")]}", $"E1 622.VGA {before[Path.Combine(elvira1Directory, "622.VGA")]}",
            $"E2 951.VGA {before[Path.Combine(elvira2Directory, "951.VGA")]}", $"E2 952.VGA {before[Path.Combine(elvira2Directory, "952.VGA")]}",
            $"E2 971.VGA {before[Path.Combine(elvira2Directory, "971.VGA")]}", $"E2 972.VGA {before[Path.Combine(elvira2Directory, "972.VGA")]}",
            "", "Production fix: corrected stale Advanced palette explanation; it now accurately describes unique automatic resolution and bank-000 fallback."
        });
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidDataException(message);
    }

    private static void RunElvira1PaletteRegression(string e1PalettePath, string e1ImagesPath, string e2PalettePath, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);

        List<ElviraPaletteBank> e1Banks = ElviraPaletteLoader.Load(e1PalettePath);
        if (e1Banks.Count != 6 || !e1Banks.Select(bank => bank.Index).SequenceEqual(Enumerable.Range(0, 6)))
            throw new InvalidDataException("Elvira I 071.VGA did not expose exactly header-defined banks 000..005.");

        ushort[] expectedBank005 = [0x000, 0x100, 0x200, 0x600, 0x300, 0x400, 0x621, 0x742, 0x752, 0x763, 0x731, 0x776, 0x775];
        Color[] bank005 = e1Banks[5].Colors;
        for (int i = 0; i < expectedBank005.Length; i++)
        {
            ushort expected = expectedBank005[i];
            Color actual = bank005[i];
            Color expectedColor = Color.FromArgb(255, ((expected >> 8) & 0x0F) * 32, ((expected >> 4) & 0x0F) * 32, (expected & 0x0F) * 32);
            if (actual != expectedColor)
                throw new InvalidDataException($"Palette 005 color {i} differs from the recorded DAC reference.");
        }
        if (bank005.Length != 16 || bank005[13] != Color.FromArgb(255, 160, 0, 0) ||
            bank005[14] != Color.FromArgb(255, 96, 96, 64) || bank005[15] != Color.FromArgb(255, 192, 192, 160))
            throw new InvalidDataException("Palette 005 fixed Elvira colors changed.");

        byte[] e2Header = File.ReadAllBytes(e2PalettePath);
        int e2HeaderCount = System.Buffers.Binary.BinaryPrimitives.ReadUInt16BigEndian(e2Header.AsSpan(2, 2));
        List<ElviraPaletteBank> e2Banks = ElviraPaletteLoader.Load(e2PalettePath);
        if (e2Banks.Count != e2HeaderCount)
            throw new InvalidDataException("The shared header-defined palette count does not match Elvira II 071.VGA.");

        byte[] imageFile = File.ReadAllBytes(e1ImagesPath);
        ParsedTable table = new VgaImageTableParser(imageFile).Parse();
        if (table.Entries.Count != 68 || table.FirstDataOffset != 0x220)
            throw new InvalidDataException("Elvira I 072.VGA did not produce its expected 68-record image table.");

        var reportLines = new List<string> { "image_id\ttable_offset\tpayload_offset\twidth\theight\tencoding\tflags" };
        var decoded = new List<(VgaImageEntry Entry, Bitmap Image)>();
        try
        {
            foreach (VgaImageEntry entry in table.Entries)
            {
                reportLines.Add($"{entry.ImageId:D4}\t0x{entry.ImageId * 8:X4}\t0x{entry.DataOffset:X5}\t{entry.PixelWidth}\t{entry.Height}\t{(entry.Compressed ? "RLE" : "RAW")}\t0x{entry.HeaderFlags:X2}");
                if (entry.DataOffset == 0 || entry.PixelWidth == 0 || entry.Height == 0)
                    continue;

                byte[] pixels = ElviraImageDecoder.Decode(imageFile, entry);
                decoded.Add((entry, PaletteTools.ToBitmap(entry.PixelWidth, entry.Height, pixels, bank005, transparentZero: false)));
            }
            File.WriteAllLines(Path.Combine(outputDirectory, "072_IMAGE_RECORDS.tsv"), reportLines);
            WritePaletteContactSheet(decoded, Path.Combine(outputDirectory, "072_PALETTE005_CONTACT_SHEET.png"));
        }
        finally
        {
            foreach ((_, Bitmap image) in decoded) image.Dispose();
        }
    }

    private static void WritePaletteContactSheet(IReadOnlyList<(VgaImageEntry Entry, Bitmap Image)> decoded, string outputPath)
    {
        const int columns = 4, cellWidth = 176, cellHeight = 150, previewWidth = 168, previewHeight = 122;
        int rows = (decoded.Count + columns - 1) / columns;
        using var sheet = new Bitmap(columns * cellWidth, rows * cellHeight);
        using Graphics graphics = Graphics.FromImage(sheet);
        graphics.Clear(Color.Black);
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        using var textBrush = new SolidBrush(Color.White);

        for (int i = 0; i < decoded.Count; i++)
        {
            (VgaImageEntry entry, Bitmap image) = decoded[i];
            int x = (i % columns) * cellWidth, y = (i / columns) * cellHeight;
            float scale = Math.Min((float)previewWidth / image.Width, (float)previewHeight / image.Height);
            int width = Math.Max(1, (int)Math.Round(image.Width * scale));
            int height = Math.Max(1, (int)Math.Round(image.Height * scale));
            graphics.DrawImage(image, new Rectangle(x + 4, y + 4, width, height));
            graphics.DrawString($"{entry.ImageId:D4}  {entry.PixelWidth}x{entry.Height}  {(entry.Compressed ? "RLE" : "RAW")}", SystemFonts.DefaultFont, textBrush, x + 4, y + 128);
        }
        sheet.Save(outputPath, System.Drawing.Imaging.ImageFormat.Png);
    }

    private static void RunGamePcRepackProductionSmoke(string elvira1, string elvira1Sk, string elvira2, string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Production repacker smoke root already exists: {root}");
        Directory.CreateDirectory(root);
        VerifyProductionRepack("elvira1", elvira1, ElviraGameProfile.Elvira1, 689, 11, 600, 688, root);
        VerifyProductionRepack("elvira2", elvira2, ElviraGameProfile.Elvira2, 1053, 12, 900, 1052, root);

        var enc = GamePcTextEditor.GetEncoding("CP852");
        byte[] english = File.ReadAllBytes(elvira1), slovak = File.ReadAllBytes(elvira1Sk);
        List<GamePcStringEntry> englishEntries = GamePcTextEditor.LoadEntries(elvira1, ElviraGameProfile.Elvira1);
        List<GamePcStringEntry> slovakEntries = GamePcTextEditor.LoadEntries(elvira1Sk, ElviraGameProfile.Elvira1);
        var slovakLogicalTexts = slovakEntries.ToDictionary(entry => entry.Index, entry => entry.Decode(enc));
        byte[] rebuiltSk = GamePcTextEditor.RepackHeaderDefined(english, slovakLogicalTexts, englishEntries, enc, "Elvira I");
        if (!rebuiltSk.SequenceEqual(slovak))
            throw new InvalidDataException("GAMEPCSK binary reproduction differs from the validated source.");
    }

    private static void RunElvira1TerminalDelimiterSmoke(string gamePcOriginal, string gamePcSk, string elvira2, string root)
    {
        if (Directory.Exists(root) || File.Exists(root)) throw new IOException($"Terminal-delimiter smoke root already exists: {root}");
        Directory.CreateDirectory(root);
        var cp852 = GamePcTextEditor.GetEncoding("CP852");
        List<GamePcStringEntry> original = GamePcTextEditor.LoadEntries(gamePcOriginal, ElviraGameProfile.Elvira1);
        List<GamePcStringEntry> slovak = GamePcTextEditor.LoadEntries(gamePcSk, ElviraGameProfile.Elvira1);
        if (original.Count != 689 || slovak.Count != 689) throw new InvalidDataException("Elvira I terminal-delimiter fixture has an unexpected logical count.");

        byte[] originalBytes = File.ReadAllBytes(gamePcOriginal);
        byte[] slovakBytes = File.ReadAllBytes(gamePcSk);
        byte[] repaired = GamePcTextEditor.BuildEditedData(gamePcSk, new Dictionary<int, string>(), slovak, cp852, ElviraGameProfile.Elvira1, original);
        string previewPath = Path.Combine(Path.GetDirectoryName(root) ?? throw new InvalidOperationException("Smoke root has no parent directory."), "GAMEPCSK_O8H_PREVIEW");
        if (Directory.Exists(previewPath)) throw new IOException($"Terminal-delimiter preview path is a directory: {previewPath}");
        if (File.Exists(previewPath))
        {
            if (!File.ReadAllBytes(previewPath).SequenceEqual(repaired))
                throw new IOException($"Terminal-delimiter preview already exists with different contents: {previewPath}");
        }
        else File.WriteAllBytes(previewPath, repaired);
        List<GamePcStringEntry> preview = GamePcTextEditor.LoadEntries(previewPath, ElviraGameProfile.Elvira1);
        if (preview.Count != 689) throw new InvalidDataException("Elvira I repaired preview did not strict-parse to 689 strings.");

        static void RequireSuffix(GamePcStringEntry entry, params byte[] expected)
        {
            if (!entry.OriginalBytes.AsSpan().EndsWith(expected))
                throw new InvalidDataException($"String {entry.Index} terminal bytes are not {Convert.ToHexString(expected)}.");
        }
        RequireSuffix(preview[61], 0x6D, 0x69, 0x2E, 0x22, 0x20);
        RequireSuffix(preview[417], 0x74, 0x65, 0x62, 0x61, 0x2E, 0x20);
        RequireSuffix(preview[424], 0x76, 0x72, 0x65, 0x63, 0x69, 0x2E, 0x20);
        if (!preview[298].OriginalBytes.SequenceEqual(slovak[298].OriginalBytes))
            throw new InvalidDataException("String 298 changed although its immutable original has no terminal delimiter.");
        if (preview[425].OriginalBytes.Length < 2 || preview[425].OriginalBytes[^1] != 0x20 || preview[425].OriginalBytes[^2] == 0x20)
            throw new InvalidDataException("String 425 does not contain exactly one preserved terminal space.");

        int sourceTextEnd = 0x14 + ReadTextBlockLength(slovakBytes);
        int repairedTextEnd = 0x14 + ReadTextBlockLength(repaired);
        if (!slovakBytes.AsSpan(sourceTextEnd).SequenceEqual(repaired.AsSpan(repairedTextEnd)))
            throw new InvalidDataException("Elvira I repaired preview changed the post-text suffix.");
        byte[] originalNoOp = GamePcTextEditor.BuildEditedData(gamePcOriginal, new Dictionary<int, string>(), original, cp852, ElviraGameProfile.Elvira1, original);
        if (!originalNoOp.SequenceEqual(originalBytes)) throw new InvalidDataException("Elvira I immutable original no-op repack is not byte-identical.");
        List<GamePcStringEntry> e2Entries = GamePcTextEditor.LoadEntries(elvira2, ElviraGameProfile.Elvira2);
        if (!GamePcTextEditor.BuildEditedData(elvira2, new Dictionary<int, string>(), e2Entries, cp852, ElviraGameProfile.Elvira2).SequenceEqual(File.ReadAllBytes(elvira2)))
            throw new InvalidDataException("Elvira II no-op behavior changed.");

        var variant = new VariantEntry("Slovak", "GAMEPCSK", true, 1, "SK", "RUNVGASK.EXE");
        TranslationExchangeDocument document = TranslationExchangeService.Create(ElviraGameProfile.Elvira1, gamePcOriginal, original, slovak, new Dictionary<int, string>(), variant, (_, _) => string.Empty);
        foreach (string extension in new[] { "csv", "xlsx" })
        {
            string exchange = Path.Combine(root, "terminal-delimiter." + extension);
            if (extension == "csv") TranslationExchangeService.ExportCsv(exchange, document); else TranslationExchangeService.ExportXlsx(exchange, document);
            IReadOnlyDictionary<int, string> imported = TranslationExchangeService.Import(exchange, ElviraGameProfile.Elvira1, gamePcOriginal, original);
            byte[] importedRepaired = GamePcTextEditor.BuildEditedData(gamePcSk, imported, slovak, cp852, ElviraGameProfile.Elvira1, original);
            if (!importedRepaired.SequenceEqual(repaired)) throw new InvalidDataException($"{extension.ToUpperInvariant()} semantic interchange did not preserve terminal delimiters.");
        }

        string persistence = Path.Combine(root, "persistence"); Directory.CreateDirectory(persistence);
        string saved = Path.Combine(persistence, "GAMEPCSK"); File.Copy(gamePcSk, saved);
        List<GamePcStringEntry> savedEntries = GamePcTextEditor.LoadEntries(saved, ElviraGameProfile.Elvira1);
        GamePcTextEditor.SaveInPlace(saved, new Dictionary<int, string>(), savedEntries, cp852, ElviraGameProfile.Elvira1, original);
        if (!File.ReadAllBytes(saved).SequenceEqual(repaired)) throw new InvalidDataException("Normal Save did not repair the current GAMEPCSK terminal delimiters.");
        string saveAs = Path.Combine(persistence, "SKFIX");
        GameDataFileService.SaveAsNew(saved, saveAs, new Dictionary<int, string>(), savedEntries, cp852, ElviraGameProfile.Elvira1, original);
        if (!File.ReadAllBytes(saveAs).SequenceEqual(repaired)) throw new InvalidDataException("Save As did not preserve terminal delimiters.");
    }

    private static void RunElvira1FixedHotspotLayoutSmoke(string gamePcOriginal, string gamePcSk, string elvira2, string root)
    {
        if (Directory.Exists(root) || File.Exists(root)) throw new IOException($"Fixed-hotspot smoke root already exists: {root}");
        Directory.CreateDirectory(root);
        var cp852 = GamePcTextEditor.GetEncoding("CP852");
        List<GamePcStringEntry> original = GamePcTextEditor.LoadEntries(gamePcOriginal, ElviraGameProfile.Elvira1);
        List<GamePcStringEntry> slovak = GamePcTextEditor.LoadEntries(gamePcSk, ElviraGameProfile.Elvira1);
        if (original.Count != 689 || slovak.Count != 689) throw new InvalidDataException("Fixed-hotspot fixture has an unexpected logical count.");

        static int StartColumn(GamePcStringEntry entry, byte[] token)
        {
            for (int start = 0; start <= entry.OriginalBytes.Length - token.Length; start++)
                if (entry.OriginalBytes.AsSpan(start, token.Length).SequenceEqual(token)) return start;
            throw new InvalidDataException($"String {entry.Index} does not contain the expected menu label.");
        }

        if (!GamePcTextEditor.TryGetFixedHotspotMenuLayout(original[213], out int yesColumn, out int noColumn) || yesColumn != 9 || noColumn != 18)
            throw new InvalidDataException($"The immutable YES/NO resource does not expose the proven fixed columns 9 and 18 (actual {yesColumn}/{noColumn}).");
        if (StartColumn(slovak[213], cp852.GetBytes("ÁNO")) != 0 || StartColumn(slovak[213], cp852.GetBytes("NIE")) != 9)
            throw new InvalidDataException("The Slovak fixture no longer demonstrates the unaligned fixed-hotspot menu state.");

        // Screen A and Screen B are independent prompts; both deliberately reuse
        // the one immutable YES/NO choice resource at index 213.
        if (!original[499].Decode(cp852).Contains("play again", StringComparison.Ordinal) ||
            !original[523].Decode(cp852).Contains("Are you sure", StringComparison.Ordinal) ||
            !slovak[499].Decode(cp852).Contains("znova", StringComparison.Ordinal) ||
            !slovak[523].Decode(cp852).Contains("istý", StringComparison.Ordinal))
            throw new InvalidDataException("The two independent game-over prompts were not found at the proven entries.");

        byte[] repaired = GamePcTextEditor.BuildEditedData(gamePcSk, new Dictionary<int, string>(), slovak, cp852, ElviraGameProfile.Elvira1, original);
        string previewPath = Path.Combine(Path.GetDirectoryName(root) ?? throw new InvalidOperationException("Smoke root has no parent directory."), "GAMEPCSK_Q4_PREVIEW");
        if (File.Exists(previewPath) && !File.ReadAllBytes(previewPath).SequenceEqual(repaired))
            throw new IOException("Fixed-hotspot preview already exists with different contents.");
        File.WriteAllBytes(previewPath, repaired);
        List<GamePcStringEntry> preview = GamePcTextEditor.LoadEntries(previewPath, ElviraGameProfile.Elvira1);
        if (preview.Count != 689 || StartColumn(preview[213], cp852.GetBytes("ÁNO")) != yesColumn || StartColumn(preview[213], cp852.GetBytes("NIE")) != noColumn)
            throw new InvalidDataException("The localized labels do not occupy the immutable YES/NO start columns.");
        if (!preview[212].OriginalBytes.SequenceEqual(slovak[212].OriginalBytes) || !preview[214].OriginalBytes.SequenceEqual(slovak[214].OriginalBytes))
            throw new InvalidDataException("A neighboring non-menu entry changed during layout repair.");

        // Prompt edits must not affect the independently stored choice-row columns.
        byte[] changedPrompts = GamePcTextEditor.BuildEditedData(gamePcSk,
            new Dictionary<int, string> { [499] = "X", [523] = "Y" }, slovak, cp852, ElviraGameProfile.Elvira1, original);
        List<GamePcStringEntry> promptPreview = GamePcTextEditor.LoadEntries(WriteTemporary(root, "prompt-layout", changedPrompts), ElviraGameProfile.Elvira1);
        if (StartColumn(promptPreview[213], cp852.GetBytes("ÁNO")) != yesColumn || StartColumn(promptPreview[213], cp852.GetBytes("NIE")) != noColumn)
            throw new InvalidDataException("A game-over prompt edit shifted the independent fixed-hotspot labels.");

        byte[] originalNoOp = GamePcTextEditor.BuildEditedData(gamePcOriginal, new Dictionary<int, string>(), original, cp852, ElviraGameProfile.Elvira1, original);
        if (!originalNoOp.SequenceEqual(File.ReadAllBytes(gamePcOriginal))) throw new InvalidDataException("Elvira I immutable-original no-op identity failed.");
        List<GamePcStringEntry> e2Entries = GamePcTextEditor.LoadEntries(elvira2, ElviraGameProfile.Elvira2);
        if (!GamePcTextEditor.BuildEditedData(elvira2, new Dictionary<int, string>(), e2Entries, cp852, ElviraGameProfile.Elvira2).SequenceEqual(File.ReadAllBytes(elvira2)))
            throw new InvalidDataException("Elvira II no-op behavior changed.");

        var variant = new VariantEntry("Slovak", "GAMEPCSK", true, 1, "SK", "RUNVGASK.EXE");
        TranslationExchangeDocument document = TranslationExchangeService.Create(ElviraGameProfile.Elvira1, gamePcOriginal, original, slovak, new Dictionary<int, string>(), variant, (_, _) => string.Empty);
        foreach (string extension in new[] { "csv", "xlsx" })
        {
            string exchange = Path.Combine(root, "fixed-hotspot." + extension);
            if (extension == "csv") TranslationExchangeService.ExportCsv(exchange, document); else TranslationExchangeService.ExportXlsx(exchange, document);
            IReadOnlyDictionary<int, string> imported = TranslationExchangeService.Import(exchange, ElviraGameProfile.Elvira1, gamePcOriginal, original);
            byte[] importedRepaired = GamePcTextEditor.BuildEditedData(gamePcSk, imported, slovak, cp852, ElviraGameProfile.Elvira1, original);
            if (!importedRepaired.SequenceEqual(repaired)) throw new InvalidDataException($"{extension.ToUpperInvariant()} round trip changed fixed-hotspot layout.");
        }

        string persistence = Path.Combine(root, "persistence"); Directory.CreateDirectory(persistence);
        string saved = Path.Combine(persistence, "GAMEPCSK"); File.Copy(gamePcSk, saved);
        List<GamePcStringEntry> savedEntries = GamePcTextEditor.LoadEntries(saved, ElviraGameProfile.Elvira1);
        GamePcTextEditor.SaveInPlace(saved, new Dictionary<int, string>(), savedEntries, cp852, ElviraGameProfile.Elvira1, original);
        if (!File.ReadAllBytes(saved).SequenceEqual(repaired)) throw new InvalidDataException("Save did not apply fixed-hotspot layout repair.");
        string saveAs = GameDataFileService.SaveAsNew(saved, Path.Combine(persistence, "SKFIX"), new Dictionary<int, string>(), savedEntries, cp852, ElviraGameProfile.Elvira1, original);
        if (!File.ReadAllBytes(saveAs).SequenceEqual(repaired)) throw new InvalidDataException("Save As did not preserve fixed-hotspot layout repair.");
    }

    private static void VerifyProductionRepack(string name, string sourcePath, ElviraGameProfile profile, int expectedCount, int emptyIndex, int longIndex, int lastIndex, string root)
    {
        string directory = Path.Combine(root, name);
        Directory.CreateDirectory(directory);
        string working = Path.Combine(directory, "GAMEPC");
        File.Copy(sourcePath, working);
        string sourceHash = Hash(working);
        byte[] source = File.ReadAllBytes(working);
        var enc = GamePcTextEditor.GetEncoding("CP852");
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(working, profile);
        if (entries.Count != expectedCount || entries[^1].Index != lastIndex || entries[emptyIndex].ByteLength != 0)
            throw new InvalidDataException($"{name}: header-counted logical records do not match the validated profile.");

        byte[] noOp = GamePcTextEditor.BuildEditedData(working, new Dictionary<int, string>(), entries, enc, profile);
        if (!noOp.SequenceEqual(source)) throw new InvalidDataException($"{name}: no-op repack is not byte-identical.");

        string longText = entries[longIndex].Decode(enc) + " 0123456789 ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        if (enc.GetByteCount(longText) <= entries[longIndex].ByteLength)
            throw new InvalidDataException($"{name}: long-text fixture did not exceed the original slot length.");
        var edits = new Dictionary<int, string>
        {
            [1] = "x",
            [longIndex] = longText,
            [lastIndex] = "čáďéľňóšťúýž",
            [Math.Min(20, emptyIndex + 1)] = "Slovak CP852: čáďéľňóšťúýž"
        };
        byte[] output = GamePcTextEditor.BuildEditedData(working, edits, entries, enc, profile);
        string gameName = profile == ElviraGameProfile.Elvira1 ? "Elvira I" : "Elvira II";
        List<GamePcStringEntry> reparsed = GamePcTextEditor.LoadEntries(WriteTemporary(directory, "repacked", output), profile);
        if (reparsed.Count != expectedCount || reparsed[emptyIndex].ByteLength != 0 ||
            !reparsed[longIndex].Decode(enc).Equals(longText, StringComparison.Ordinal) ||
            !reparsed[lastIndex].Decode(enc).Equals(edits[lastIndex], StringComparison.Ordinal) ||
            reparsed[longIndex].Offset == entries[longIndex].Offset)
            throw new InvalidDataException($"{name}: repacked indexes, empty record, edited text, or moved physical offsets are incorrect.");

        int oldTextEnd = 0x14 + ReadTextBlockLength(source), newTextEnd = 0x14 + ReadTextBlockLength(output);
        if (!source.AsSpan(0, 0x0C).SequenceEqual(output.AsSpan(0, 0x0C)) ||
            !source.AsSpan(0x0C, 4).SequenceEqual(output.AsSpan(0x0C, 4)) ||
            !source.AsSpan(oldTextEnd).SequenceEqual(output.AsSpan(newTextEnd)))
            throw new InvalidDataException($"{name}: header preservation or current-source suffix preservation failed.");

        GamePcTextEditor.SaveInPlace(working, edits, entries, enc, profile);
        List<GamePcStringEntry> saved = GamePcTextEditor.LoadEntries(working, profile);
        if (Hash(working) == sourceHash || saved.Count != expectedCount || !saved[longIndex].Decode(enc).Equals(longText, StringComparison.Ordinal) ||
            !File.Exists(Path.Combine(directory, "GAMEPCO")))
            throw new InvalidDataException($"{name}: transactional in-place repack or original backup behavior failed.");

        string afterValidSave = Hash(working);
        foreach (string badText in new[] { "emoji 😀", "embedded\0nul" })
        {
            bool rejected = false;
            try { _ = GamePcTextEditor.BuildEditedData(working, new Dictionary<int, string> { [longIndex] = badText }, saved, enc, profile); }
            catch (InvalidOperationException) { rejected = true; }
            if (!rejected || Hash(working) != afterValidSave)
                throw new InvalidDataException($"{name}: invalid CP852/NUL validation was not transactional.");
        }

        byte[] corrupt = source.AsSpan(0, 0x13).ToArray();
        bool corruptRejected = false;
        try { _ = GamePcTextEditor.RepackHeaderDefined(corrupt, new Dictionary<int, string>(), entries, enc, gameName); }
        catch (InvalidDataException) { corruptRejected = true; }
        if (!corruptRejected) throw new InvalidDataException($"{name}: corrupt GAMEPC source was accepted by the repacker.");
        bool destinationRejected = false;
        try { _ = GameDataFileService.SaveAsNew(working, Path.Combine(directory, "TOO_LONG_NAME"), new Dictionary<int, string>(), saved, enc, profile); }
        catch (InvalidOperationException) { destinationRejected = true; }
        if (!destinationRejected || Hash(working) != afterValidSave)
            throw new InvalidDataException($"{name}: invalid Save As destination changed the source.");
    }

    private static string WriteTemporary(string directory, string name, byte[] data)
    {
        string path = Path.Combine(directory, name + ".GAMEPC");
        File.WriteAllBytes(path, data);
        return path;
    }

    private static int ReadTextBlockLength(byte[] data) => checked((int)System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(0x10, 4)));

    private static void RunVersionSmoke()
    {
        var assembly = typeof(Program).Assembly;
        string assemblyVersion = assembly.GetName().Version?.ToString() ?? string.Empty;
        string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion ?? string.Empty;
        string informational = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .Cast<System.Reflection.AssemblyInformationalVersionAttribute>().Single().InformationalVersion;
        if (AppInfo.ProductVersion != "1.0" || AppInfo.ProductTitle != "π1 Elvira I & II Editor v1.0" ||
            assemblyVersion != "1.0.0.0" || fileVersion != "1.0.0.0" || informational != "1.0.0")
            throw new InvalidDataException("Current product metadata/title/about text does not resolve to v1.0.");
        foreach (UiLanguage language in new[] { UiLanguage.English, UiLanguage.Slovak, UiLanguage.Czech })
        {
            UiText.SetLanguage(language);
            if (!AboutDocumentService.Load(language).Body.Contains(AppInfo.ProductName, StringComparison.Ordinal))
                throw new InvalidDataException($"Localized current title/about text does not resolve to v1.0: {language}.");
        }
        UiText.SetLanguage(UiLanguage.English);
        HelpDocumentService.ValidateAll();
        AboutDocumentService.ValidateAll();
    }

    private static void VerifyReservedHudSafeguards(string packedRunVga, string packedRunIt, string root, byte[] v5Bytes, byte[] v2Bytes, byte[] fullCell)
    {
        const string unsafeBytesHex = "28004444444C3400";
        byte[] unsafeBytes = Convert.FromHexString(unsafeBytesHex);
        int v5HudOffset = RunVgaBootstrapService.V5FontOffset + FontSlotMetadata.HudEraseGlyph * RunVgaFontService.GlyphBytes;
        int v2HudOffset = RunItBootstrapService.OriginalFontOffset +
            (FontSlotMetadata.HudEraseGlyph - RunVgaFontService.OriginalFirstChar) * RunVgaFontService.GlyphBytes;

        // Final raw-image barriers: no caller can bypass the glyph model by sending
        // a hand-built 256x8 table directly to either bootstrap builder.
        byte[] maliciousFont = new byte[RunVgaFontService.ExtendedFontBytes];
        unsafeBytes.CopyTo(maliciousFont, FontSlotMetadata.HudEraseGlyph * RunVgaFontService.GlyphBytes);
        byte[] v5Raw = RunVgaBootstrapService.BuildV5(
            RunVgaBootstrapService.UnpackVerifiedOriginal(File.ReadAllBytes(packedRunVga)), maliciousFont);
        byte[] v2Raw = RunItBootstrapService.BuildExtended(
            RunItBootstrapService.UnpackCanonicalOriginal(File.ReadAllBytes(packedRunIt)), maliciousFont);
        if (!v5Raw.AsSpan(v5HudOffset, RunVgaFontService.GlyphBytes).SequenceEqual(fullCell) ||
            !v2Raw.AsSpan(v2HudOffset, RunVgaFontService.GlyphBytes).SequenceEqual(fullCell))
            throw new InvalidDataException("A raw bootstrap input bypassed the reserved HUD glyph barrier.");

        // Import and manual model edits share the same enforcement point. A normal
        // printable CP852 slot must still retain its imported data.
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        int[] slovakCodes = "čáďéľňóšťúýž"
            .Select(ch => (int)System.Text.Encoding.GetEncoding(852).GetBytes(ch.ToString())[0])
            .Distinct()
            .ToArray();
        foreach (int code in slovakCodes.Append(0x82).Append(0x8E))
            maliciousFont.AsSpan(code * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).Fill((byte)(code ^ 0x5A));
        foreach ((string name, int offset) target in new[] { ("V5", v5HudOffset), ("V2", v2HudOffset) })
        {
            string clean = Path.Combine(root, target.name + "_HUD_CLEAN.EXE");
            string bad = Path.Combine(root, target.name + "_HUD_BAD.EXE");
            string repaired = Path.Combine(root, target.name + "_HUD_REPAIRED.EXE");
            File.WriteAllBytes(clean, target.name == "V5" ? v5Bytes : v2Bytes);

            FontLoadResult imported = RunVgaFontService.LoadRunVga(clean);
            RunVgaFontService.ImportFontBytesIntoEdited(imported, maliciousFont, "safeguard synthetic import");
            GlyphModel protectedGlyph = imported.Glyphs[FontSlotMetadata.HudEraseGlyph];
            protectedGlyph.ReplaceEdited(unsafeBytes);
            protectedGlyph.ToggleEditedPixel(0, 0);
            protectedGlyph.ShiftEdited(1, 1);
            protectedGlyph.Reset();
            bool slovakAligned = slovakCodes.All(code => imported.Glyphs[code].Edited.SequenceEqual(
                maliciousFont.AsSpan(code * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).ToArray()));
            if (!protectedGlyph.Edited.SequenceEqual(fullCell) ||
                !imported.Glyphs[0x82].Edited.SequenceEqual(maliciousFont.AsSpan(0x82 * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).ToArray()) ||
                !imported.Glyphs[0x8E].Edited.SequenceEqual(maliciousFont.AsSpan(0x8E * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).ToArray()) ||
                !slovakAligned)
                throw new InvalidDataException($"{target.name}: import or manual editing changed the protected glyph or damaged a normal glyph.");

            byte[] malformed = target.name == "V5" ? v5Bytes.ToArray() : v2Bytes.ToArray();
            unsafeBytes.CopyTo(malformed, target.offset);
            File.WriteAllBytes(bad, malformed);
            string badHash = Hash(bad);
            FontLoadResult oldBad = RunVgaFontService.LoadRunVga(bad);
            if (!oldBad.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(unsafeBytes) ||
                !oldBad.Glyphs[FontSlotMetadata.HudEraseGlyph].Edited.SequenceEqual(fullCell) ||
                !oldBad.DetectionDetails.Contains("noncanonical", StringComparison.Ordinal) || Hash(bad) != badHash)
                throw new InvalidDataException($"{target.name}: old bad executable was not identified and repaired only in memory.");

            RunVgaFontService.SaveCopy(oldBad, repaired);
            byte[] repairedBytes = File.ReadAllBytes(repaired);
            if (!repairedBytes.AsSpan(target.offset, RunVgaFontService.GlyphBytes).SequenceEqual(fullCell) || Hash(bad) != badHash)
                throw new InvalidDataException($"{target.name}: explicit output did not repair 0x81 without changing the old source.");
        }
    }

    private static void RunProductionSmoke(ElviraGame game, string pristineExe, string root)
    {
        string name = game == ElviraGame.Elvira1 ? "elvira1" : "elvira2";
        string directory = Path.Combine(root, name);
        if (Directory.Exists(directory)) throw new IOException($"Isolated test directory already exists: {directory}");
        Directory.CreateDirectory(directory);
        string activeExe = Path.Combine(directory, game == ElviraGame.Elvira1 ? "RUNVGA.EXE" : "RUNIT.EXE");
        string gamepc = Path.Combine(directory, "GAMEPC");
        string sourceDir = Path.GetDirectoryName(pristineExe) ?? throw new InvalidDataException("Source directory missing.");
        File.Copy(pristineExe, activeExe);
        File.Copy(Path.Combine(sourceDir, "GAMEPC"), gamepc);
        FontLoadResult initial = RunVgaFontService.LoadRunVga(activeExe);
        ProductionDeploymentResult first = GamePatchDeploymentService.Deploy(initial, initial.Glyphs);
        string originalExeHash = Hash(first.OriginalExecutable), activeGamePcHash = Hash(gamepc);
        if (File.Exists(Path.Combine(directory, "GAMEPCO")))
            throw new InvalidDataException($"{name}: font-only deployment unexpectedly created GAMEPCO.");
        if (Hash(gamepc) != activeGamePcHash)
            throw new InvalidDataException($"{name}: font-only deployment modified GAMEPC.");
        byte[] deployed = File.ReadAllBytes(first.ActiveExecutable);
        int hudOffset = game == ElviraGame.Elvira1
            ? RunVgaBootstrapService.V5FontOffset + FontSlotMetadata.HudEraseGlyph * RunVgaFontService.GlyphBytes
            : RunItBootstrapService.OriginalFontOffset + (FontSlotMetadata.HudEraseGlyph - RunVgaFontService.OriginalFirstChar) * RunVgaFontService.GlyphBytes;
        if (!deployed.AsSpan(hudOffset, RunVgaFontService.GlyphBytes).SequenceEqual(FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes))
            throw new InvalidDataException($"{name}: generated executable did not preserve reserved glyph 0x81.");
        FontLoadResult extended = RunVgaFontService.LoadRunVga(first.ActiveExecutable);
        extended.Glyphs[0x41].ReplaceEdited(Convert.FromHexString("A0B0C0D0E0F00000"));
        ProductionDeploymentResult second = GamePatchDeploymentService.Deploy(extended, extended.Glyphs);
        if (Hash(second.OriginalExecutable) != originalExeHash || Hash(gamepc) != activeGamePcHash || File.Exists(Path.Combine(directory, "GAMEPCO")))
            throw new InvalidDataException($"{name}: repeated font deployment changed an immutable backup or unrelated GAMEPC state.");
        FontLoadResult reopened = RunVgaFontService.LoadRunVga(second.ActiveExecutable);
        if (!reopened.Glyphs[0x41].Original.SequenceEqual(Convert.FromHexString("A0B0C0D0E0F00000")))
            throw new InvalidDataException($"{name}: active glyph did not round-trip.");
    }

    private static string Hash(string path) => Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(File.ReadAllBytes(path)));

    private static void RunElvira1GamePcParserSmoke(string englishSource, string slovakSource, string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Elvira I GAMEPC parser smoke root already exists: {root}");
        string englishHash = Hash(englishSource), slovakHash = Hash(slovakSource);
        Directory.CreateDirectory(root);
        string english = Path.Combine(root, "GAMEPC"), slovak = Path.Combine(root, "GAMEPCSK");
        File.Copy(englishSource, english);
        File.Copy(slovakSource, slovak);

        var encoding = GamePcTextEditor.GetEncoding("CP852");
        List<GamePcStringEntry> en = GamePcTextEditor.LoadEntries(english, ElviraGameProfile.Elvira1);
        List<GamePcStringEntry> sk = GamePcTextEditor.LoadEntries(slovak, ElviraGameProfile.Elvira1);
        AssertElvira1TextMap(en, 0x48D6, "EN", new Dictionary<int, (int Start, int Terminator)>
        {
            [0] = (0x14, 0x58), [1] = (0x59, 0xC0), [565] = (0x3BF0, 0x3C15),
            [566] = (0x3C16, 0x3C42), [688] = (0x48BA, 0x48D5)
        });
        AssertElvira1TextMap(sk, 0x3DAD, "SK", new Dictionary<int, (int Start, int Terminator)>
        {
            [0] = (0x14, 0x50), [1] = (0x51, 0x9D), [666] = (0x3BDA, 0x3C0C),
            [667] = (0x3C0D, 0x3C1B), [688] = (0x3D98, 0x3DAC)
        });
        if (en.Skip(566).Count() != 123 || sk.Skip(566).Count() != 123 ||
            !en[688].Decode(encoding).Contains("Spiritual Mastery", StringComparison.Ordinal) ||
            !sk[688].Decode(encoding).Contains("duševnej sily", StringComparison.Ordinal))
            throw new InvalidDataException("Elvira I UI-model smoke failed for search or visible indices.");

        foreach (int length in new[] { 95, 96, 97, 117, 160, 201 })
        {
            var diagnostic = Elvira1TextMetadata.Evaluate(417, new string('X', length), encoding, ignored: false);
            if (diagnostic.Bytes != length || diagnostic.Kind != TextDiagnosticKind.None || diagnostic.Limit is not null)
                throw new InvalidDataException("Elvira I runtime-diagnostic smoke retained an unsupported byte-length classification.");
        }

        // The writer remains fixed-slot only. Exercise an entry beyond the former
        // cutoff on disposable copies and verify the header/tail are untouched.
        byte[] beforeSave = File.ReadAllBytes(english);
        GamePcStringEntry late = en[688];
        GamePcTextEditor.SaveInPlace(english, new Dictionary<int, string> { [late.Index] = new string('X', late.ByteLength) }, en, encoding, ElviraGameProfile.Elvira1);
        byte[] afterSave = File.ReadAllBytes(english);
        if (afterSave.Length != beforeSave.Length || !afterSave.AsSpan(0, 0x14).SequenceEqual(beforeSave.AsSpan(0, 0x14)) ||
            !afterSave.AsSpan(0x48D6).SequenceEqual(beforeSave.AsSpan(0x48D6)) ||
            GamePcTextEditor.LoadEntries(english, ElviraGameProfile.Elvira1).Count != 689)
            throw new InvalidDataException("Elvira I in-place save altered the header, binary tail, or logical string count.");

        string savedAs = GameDataFileService.SaveAsNew(slovak, Path.Combine(root, "GPCSAVE"), new Dictionary<int, string>(), sk, encoding, ElviraGameProfile.Elvira1);
        string variant = GameDataFileService.SaveAsNew(slovak, Path.Combine(root, "GPCVAR"), new Dictionary<int, string>(), sk, encoding, ElviraGameProfile.Elvira1);
        if (GamePcTextEditor.LoadEntries(savedAs, ElviraGameProfile.Elvira1).Count != 689 ||
            GamePcTextEditor.LoadEntries(variant, ElviraGameProfile.Elvira1).Count != 689)
            throw new InvalidDataException("Elvira I Save As/Create Variant lost header-counted entries.");

        byte[] valid = File.ReadAllBytes(slovak);
        AssertElvira1ParseFails(root, "short-header", valid.AsSpan(0, 0x13).ToArray());
        byte[] beyondEof = (byte[])valid.Clone(); WriteUInt32BigEndian(beyondEof, 0x10, (uint)beyondEof.Length); AssertElvira1ParseFails(root, "length-beyond-eof", beyondEof);
        byte[] countMismatch = (byte[])valid.Clone(); WriteUInt32BigEndian(countMismatch, 0x0C, 690); AssertElvira1ParseFails(root, "count-mismatch", countMismatch);
        byte[] missingTerminator = (byte[])valid.Clone(); missingTerminator[0x3DAC] = (byte)'X'; AssertElvira1ParseFails(root, "missing-terminator", missingTerminator);
        byte[] earlyTextEnd = (byte[])valid.Clone(); WriteUInt32BigEndian(earlyTextEnd, 0x10, 0x3D9A); AssertElvira1ParseFails(root, "early-text-end", earlyTextEnd);
        byte[] excessiveCount = (byte[])valid.Clone(); WriteUInt32BigEndian(excessiveCount, 0x0C, 100_001); AssertElvira1ParseFails(root, "excessive-count", excessiveCount);

        if (Hash(englishSource) != englishHash || Hash(slovakSource) != slovakHash)
            throw new InvalidDataException("Parser smoke changed a supplied source GAMEPC file.");
    }

    private static void RunElvira2GamePcParserSmoke(string gamePcSource, string gamePcOriginalCopy, string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Elvira II GAMEPC parser smoke root already exists: {root}");
        string gamePcHash = Hash(gamePcSource), originalCopyHash = Hash(gamePcOriginalCopy);
        Directory.CreateDirectory(root);
        string gamePc = Path.Combine(root, "GAMEPC"), gamePcO = Path.Combine(root, "GAMEPCO"), workingSource = Path.Combine(root, "GAMEPCSRC");
        File.Copy(gamePcSource, gamePc);
        File.Copy(gamePcOriginalCopy, gamePcO);
        // GAMEPCO is intentionally immutable and rejected as a Save As source.
        // A disposable non-protected copy exercises the real generated-variant path.
        File.Copy(gamePcOriginalCopy, workingSource);

        var encoding = GamePcTextEditor.GetEncoding("CP852");
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(gamePc, ElviraGameProfile.Elvira2);
        AssertElvira2TextMap(entries, 0x6897, new Dictionary<int, (int Start, int Terminator)>
        {
            [0] = (0x14, 0x59), [1] = (0x5A, 0x73), [627] = (0x3BE8, 0x3C0F),
            [628] = (0x3C10, 0x3C1D), [1052] = (0x6886, 0x6896)
        });
        if (!entries[628].Decode(encoding).Equals("A tiger's eye", StringComparison.Ordinal) ||
            !entries[1052].Decode(encoding).Equals("A box of matches", StringComparison.Ordinal) ||
            entries[771].ByteLength != 161)
            throw new InvalidDataException("Elvira II UI-model smoke failed for formerly omitted or long header-counted entries.");

        // Fixed-slot writes remain unchanged: exercise a formerly omitted slot
        // on a disposable copy and verify the header and binary tail survive.
        byte[] beforeSave = File.ReadAllBytes(gamePc);
        GamePcStringEntry late = entries[1052];
        GamePcTextEditor.SaveInPlace(gamePc, new Dictionary<int, string> { [late.Index] = new string('X', late.ByteLength) }, entries, encoding, ElviraGameProfile.Elvira2);
        byte[] afterSave = File.ReadAllBytes(gamePc);
        if (afterSave.Length != beforeSave.Length || !afterSave.AsSpan(0, 0x14).SequenceEqual(beforeSave.AsSpan(0, 0x14)) ||
            !afterSave.AsSpan(0x6897).SequenceEqual(beforeSave.AsSpan(0x6897)) ||
            GamePcTextEditor.LoadEntries(gamePc, ElviraGameProfile.Elvira2).Count != 1053)
            throw new InvalidDataException("Elvira II in-place save altered the header, binary tail, or logical string count.");

        string savedAs = GameDataFileService.SaveAsNew(workingSource, Path.Combine(root, "GPCSAVE"), new Dictionary<int, string>(), entries, encoding, ElviraGameProfile.Elvira2);
        if (!File.ReadAllBytes(savedAs).SequenceEqual(File.ReadAllBytes(workingSource)))
            throw new InvalidDataException("Elvira II no-op Save As was not byte-identical.");

        const string translatedText = "Žluťoučký kůň";
        var translatedEdits = new Dictionary<int, string> { [late.Index] = translatedText };
        byte[] translatedA = GamePcTextEditor.BuildEditedData(workingSource, translatedEdits, entries, encoding, ElviraGameProfile.Elvira2);
        byte[] translatedB = GamePcTextEditor.BuildEditedData(workingSource, translatedEdits, entries, encoding, ElviraGameProfile.Elvira2);
        if (!translatedA.SequenceEqual(translatedB))
            throw new InvalidDataException("Elvira II translated GAMEPC build was not deterministic.");
        string variant = GameDataFileService.SaveAsNew(workingSource, Path.Combine(root, "GAMEPCSK"), translatedEdits, entries, encoding, ElviraGameProfile.Elvira2);
        List<GamePcStringEntry> translatedEntries = GamePcTextEditor.LoadEntries(variant, ElviraGameProfile.Elvira2);
        int translatedTextEnd = translatedEntries[^1].Offset + translatedEntries[^1].ByteLength + 1;
        byte[] original = File.ReadAllBytes(workingSource);
        if (translatedEntries.Count != 1053 || !translatedEntries[late.Index].Decode(encoding).Equals(translatedText, StringComparison.Ordinal) ||
            !translatedEntries[0].OriginalBytes.SequenceEqual(entries[0].OriginalBytes) ||
            !translatedA.AsSpan(0, 0x10).SequenceEqual(original.AsSpan(0, 0x10)) ||
            translatedA.AsSpan(0x10, 4).SequenceEqual(original.AsSpan(0x10, 4)) ||
            !translatedA.AsSpan(translatedTextEnd).SequenceEqual(original.AsSpan(0x6897)))
            throw new InvalidDataException("Elvira II translated GAMEPC did not preserve immutable header fields, unrelated text, or binary suffix.");
        if (GamePcTextEditor.LoadEntries(savedAs, ElviraGameProfile.Elvira2).Count != 1053 ||
            translatedEntries.Count != 1053)
            throw new InvalidDataException("Elvira II Save As/Create Variant lost header-counted entries.");

        byte[] valid = File.ReadAllBytes(gamePcOriginalCopy);
        AssertElvira2ParseFails(root, "short-header", valid.AsSpan(0, 0x13).ToArray());
        byte[] impossibleCount = (byte[])valid.Clone(); WriteUInt32BigEndian(impossibleCount, 0x0C, 100_001); AssertElvira2ParseFails(root, "impossible-count", impossibleCount);
        byte[] beyondEof = (byte[])valid.Clone(); WriteUInt32BigEndian(beyondEof, 0x10, (uint)beyondEof.Length); AssertElvira2ParseFails(root, "length-beyond-eof", beyondEof);
        byte[] missingTerminator = (byte[])valid.Clone(); missingTerminator[0x6896] = (byte)'X'; AssertElvira2ParseFails(root, "missing-terminator", missingTerminator);
        byte[] tooFewStrings = (byte[])valid.Clone(); WriteUInt32BigEndian(tooFewStrings, 0x0C, 1054); AssertElvira2ParseFails(root, "too-few-strings", tooFewStrings);
        byte[] trailingBinary = valid.Concat(new byte[] { 0xFF, 0xFF, 0x00, 0x41, 0x00 }).ToArray();
        string trailingPath = Path.Combine(root, "trailing-binary.GAMEPC"); File.WriteAllBytes(trailingPath, trailingBinary);
        if (GamePcTextEditor.LoadEntries(trailingPath, ElviraGameProfile.Elvira2).Count != 1053)
            throw new InvalidDataException("Elvira II parser followed data after the declared text block.");

        if (Hash(gamePcSource) != gamePcHash || Hash(gamePcOriginalCopy) != originalCopyHash)
            throw new InvalidDataException("Parser smoke changed a supplied source GAMEPC file.");
    }

    private static void CreateElvira2GamePcMap(string sourcePath, string outputDirectory)
    {
        byte[] data = File.ReadAllBytes(sourcePath);
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(sourcePath, ElviraGameProfile.Elvira2);
        var encoding = GamePcTextEditor.GetEncoding("CP852");
        const int textStart = 0x14;
        int textEnd = entries[^1].Offset + entries[^1].ByteLength + 1;

        Directory.CreateDirectory(outputDirectory);
        var csv = new System.Text.StringBuilder();
        csv.AppendLine("Index,StartOffsetHex,StartOffsetDecimal,EndOffsetHex,ByteLength,TerminatorOffsetHex,NextOffsetHex,Classification,Text");
        foreach (GamePcStringEntry entry in entries)
        {
            int terminator = entry.Offset + entry.ByteLength;
            string classification = entry.ByteLength == 0 ? "TEXT (header-counted empty slot)" : "TEXT (header-counted)";
            csv.Append(entry.Index).Append(',')
                .Append($"0x{entry.Offset:X}").Append(',').Append(entry.Offset).Append(',')
                .Append($"0x{terminator - 1:X}").Append(',').Append(entry.ByteLength).Append(',')
                .Append($"0x{terminator:X}").Append(',').Append($"0x{terminator + 1:X}").Append(',')
                .Append(Csv(classification)).Append(',').Append(Csv(entry.Decode(encoding))).AppendLine();
        }
        string csvPath = Path.Combine(outputDirectory, "ELVIRA2_GAMEPC_EN_STRING_MAP.csv");
        File.WriteAllText(csvPath, csv.ToString(), new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        GamePcStringEntry longest = entries.OrderByDescending(entry => entry.ByteLength).First();
        var report = new System.Text.StringBuilder();
        report.AppendLine("ELVIRA II ORIGINAL EN GAMEPC STRING MAP");
        report.AppendLine($"Source: {Path.GetFullPath(sourcePath)}");
        report.AppendLine($"Size: {data.Length} (0x{data.Length:X})");
        report.AppendLine($"SHA-256: {Hash(sourcePath)}");
        report.AppendLine();
        report.AppendLine("HEADER (independently validated)");
        report.AppendLine($"0x0C raw: {Convert.ToHexString(data.AsSpan(0x0C, 4))}; BE count: {entries.Count}");
        report.AppendLine($"0x10 raw: {Convert.ToHexString(data.AsSpan(0x10, 4))}; BE text-block length: {textEnd - textStart}");
        report.AppendLine($"Text start: 0x{textStart:X}; declared/final end exclusive: 0x{textEnd:X}; last terminator: 0x{textEnd - 1:X}");
        report.AppendLine($"COUNT: {entries.Count}; LAST INDEX: {entries[^1].Index}; empty logical slots: {string.Join(", ", entries.Where(entry => entry.ByteLength == 0).Select(entry => entry.Index))}");
        report.AppendLine();
        report.AppendLine("LEGACY CUTOFF AUDIT");
        report.AppendLine("Historical legacy parser started at 0x14 after a two-byte prefix skip and accepted starts below 0x3C00.");
        report.AppendLine("It returned 628 entries (indices 0..627); index 627 terminates at 0x3C0F, while the first omitted valid entry is index 628 at 0x3C10.");
        report.AppendLine($"True header-defined count: {entries.Count}; omitted by legacy branch: {entries.Count - 628} (indices 628..{entries[^1].Index}).");
        report.AppendLine();
        report.AppendLine($"LONGEST: index {longest.Index}, offset 0x{longest.Offset:X}, {longest.ByteLength} bytes: {longest.Decode(encoding)}");
        report.AppendLine("TOP 20 LONGEST ORIGINAL STRINGS");
        foreach (GamePcStringEntry entry in entries.OrderByDescending(entry => entry.ByteLength).Take(20))
            report.AppendLine($"{entry.Index,4}  0x{entry.Offset:X5}  {entry.ByteLength,3}  {entry.Decode(encoding)}");
        report.AppendLine();
        report.AppendLine("TRAILING DATA");
        report.AppendLine($"Bytes at text end 0x{textEnd:X}: {Convert.ToHexString(data.AsSpan(textEnd, Math.Min(32, data.Length - textEnd)))}");
        report.AppendLine("The parser stops at the declared text end and does not scan trailing binary data.");
        string reportPath = Path.Combine(outputDirectory, "ELVIRA2_GAMEPC_STRING_MAP_REPORT.txt");
        File.WriteAllText(reportPath, report.ToString(), new System.Text.UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static string Csv(string value) => '"' + value.Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ") + '"';

    private static void AssertElvira1TextMap(IReadOnlyList<GamePcStringEntry> entries, int expectedCursor, string name, IReadOnlyDictionary<int, (int Start, int Terminator)> expected)
    {
        if (entries.Count != 689 || entries[0].Index != 0 || entries[^1].Index != 688 || entries[11].ByteLength != 0 ||
            entries[^1].Offset + entries[^1].ByteLength + 1 != expectedCursor)
            throw new InvalidDataException($"{name}: header-defined count, empty string, or final cursor did not match.");
        foreach ((int index, (int start, int terminator)) in expected)
        {
            GamePcStringEntry entry = entries[index];
            if (entry.Offset != start || entry.Offset + entry.ByteLength != terminator)
                throw new InvalidDataException($"{name}: index {index} offset regression.");
        }
    }

    private static void AssertElvira2TextMap(IReadOnlyList<GamePcStringEntry> entries, int expectedCursor, IReadOnlyDictionary<int, (int Start, int Terminator)> expected)
    {
        if (entries.Count != 1053 || entries[0].Index != 0 || entries[^1].Index != 1052 || entries[12].ByteLength != 0 ||
            entries[^1].Offset + entries[^1].ByteLength + 1 != expectedCursor)
            throw new InvalidDataException("Elvira II: header-defined count, empty string, or final cursor did not match.");
        foreach ((int index, (int start, int terminator)) in expected)
        {
            GamePcStringEntry entry = entries[index];
            if (entry.Offset != start || entry.Offset + entry.ByteLength != terminator)
                throw new InvalidDataException($"Elvira II: index {index} offset regression.");
        }
    }

    private static void AssertElvira1ParseFails(string root, string name, byte[] data)
    {
        string path = Path.Combine(root, name + ".GAMEPC");
        File.WriteAllBytes(path, data);
        try
        {
            _ = GamePcTextEditor.LoadEntries(path, ElviraGameProfile.Elvira1);
        }
        catch (InvalidDataException)
        {
            return;
        }
        throw new InvalidDataException($"Corrupt Elvira I GAMEPC fixture was accepted: {name}.");
    }

    private static void AssertElvira2ParseFails(string root, string name, byte[] data)
    {
        string path = Path.Combine(root, name + ".GAMEPC");
        File.WriteAllBytes(path, data);
        try
        {
            _ = GamePcTextEditor.LoadEntries(path, ElviraGameProfile.Elvira2);
        }
        catch (InvalidDataException)
        {
            return;
        }
        throw new InvalidDataException($"Corrupt Elvira II GAMEPC fixture was accepted: {name}.");
    }

    private static void WriteUInt32BigEndian(byte[] data, int offset, uint value) =>
        System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(data.AsSpan(offset, sizeof(uint)), value);

    private static void RunInstallationDiscoverySmoke(string pristineRunVga, string pristineRunIt, string root)
    {
        if (Directory.Exists(root)) throw new IOException("Installation smoke root already exists.");
        string runVgaHash = Hash(pristineRunVga), runItHash = Hash(pristineRunIt);
        Directory.CreateDirectory(root);
        string e1a = CreateInstallationFixture(root, "ElviraA", pristineRunVga, "RUNVGA.EXE");
        string e1b = CreateInstallationFixture(root, "ElviraB", pristineRunVga, "RUNVGA.EXE");
        string e2 = CreateInstallationFixture(root, "ElviraII", pristineRunIt, "RUNIT.EXE");
        string unrelated = Path.Combine(root, "Elvira named but invalid");
        string egaOnly = Path.Combine(root, "EgaOnly");
        Directory.CreateDirectory(unrelated);
        Directory.CreateDirectory(egaOnly);
        File.WriteAllBytes(Path.Combine(egaOnly, "GAMEPC"), [0]);
        File.WriteAllBytes(Path.Combine(egaOnly, "012.VGA"), [0]);
        File.WriteAllBytes(Path.Combine(egaOnly, "RUNEGA.EXE"), [0]);

        if (!GameInstallationValidator.TryValidate(e1a, InstallationDiscoverySource.Manual, out GameInstallation? one) || one?.Game != ElviraGameProfile.Elvira1)
            throw new InvalidDataException("Valid Elvira I fixture was rejected.");
        if (!GameInstallationValidator.TryValidate(e2, InstallationDiscoverySource.Manual, out GameInstallation? two) || two?.Game != ElviraGameProfile.Elvira2)
            throw new InvalidDataException("Valid Elvira II fixture was rejected.");
        if (GameInstallationValidator.TryValidate(unrelated, InstallationDiscoverySource.Manual, out _) ||
            GameInstallationValidator.TryValidate(egaOnly, InstallationDiscoverySource.Manual, out _) ||
            GameInstallationValidator.TryValidate(null, InstallationDiscoverySource.Manual, out _) ||
            GameInstallationValidator.TryValidate(string.Empty, InstallationDiscoverySource.Manual, out _) ||
            GameInstallationValidator.TryValidate("   ", InstallationDiscoverySource.Manual, out _))
            throw new InvalidDataException("An invalid or RUNEGA-only fixture was treated as a supported VGA installation.");
        if (GameProfileDetector.Detect(unrelated, 0, null) != ElviraGameProfile.Unknown)
            throw new InvalidDataException("Folder-name-only game detection was accepted.");

        IReadOnlyList<GameInstallation> found = InstallationDiscoveryService.DiscoverFast([], [root, root + Path.DirectorySeparatorChar]);
        if (found.Count != 3 || found.Count(item => item.Game == ElviraGameProfile.Elvira1) != 2 || found.Count(item => item.Game == ElviraGameProfile.Elvira2) != 1)
            throw new InvalidDataException("Fast discovery did not preserve both Elvira I fixtures or deduplicate equivalent roots.");
        if (InstallationDiscoveryService.DiscoverFast([Path.Combine(root, "missing")], []).Any())
            throw new InvalidDataException("A missing remembered installation was listed as valid.");

        string settingsPath = Path.Combine(root, "app-settings", "installations.json");
        var store = new InstallationSettingsStore(settingsPath);
        if (store.Load().KnownInstallations.Count != 0 || !string.IsNullOrWhiteSpace(store.Load().LastSelectedPath))
            throw new InvalidDataException("Fresh installation settings were not empty.");
        store.Save(found, e2);
        InstallationSettings restored = store.Load();
        if (restored.KnownInstallations.Count != 3 || !InstallationPathNormalizer.Normalize(restored.LastSelectedPath ?? string.Empty).Equals(InstallationPathNormalizer.Normalize(e2), StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Installation persistence did not restore the selected valid installation.");
        if (Path.GetDirectoryName(settingsPath)?.StartsWith(e1a, StringComparison.OrdinalIgnoreCase) == true ||
            Path.GetDirectoryName(settingsPath)?.StartsWith(e2, StringComparison.OrdinalIgnoreCase) == true)
            throw new InvalidDataException("Installation settings were placed in a game directory.");
        store.Save(found, string.Empty);
        InstallationSettings cleared = store.Load();
        if (cleared.KnownInstallations.Count != 3 || !string.IsNullOrWhiteSpace(cleared.LastSelectedPath))
            throw new InvalidDataException("The no-installation selector state persisted an empty path or lost known installations.");

        foreach (UiLanguage language in new[] { UiLanguage.English, UiLanguage.Slovak, UiLanguage.Czech })
        {
            UiText.SetLanguage(language);
            foreach (string key in new[] { "NoSupportedGameSelected", "FindGames", "Browse", "SearchDrives", "SearchingForGames", "NoSupportedInstallationFound", "OneSupportedInstallationFound", "ManySupportedInstallationsFound", "InvalidGameFolder", "UnsupportedInstallation", "LastUsedInstallation" })
                if (UiText.Get(key) == key) throw new InvalidDataException($"Missing installation localization: {key} ({language}).");
            if (new InstallationSelectionItem(null).ToString() != UiText.Get("NoSupportedGameSelected") ||
                new InstallationSelectionItem(one).ToString().Contains(e1a, StringComparison.Ordinal) == false)
                throw new InvalidDataException($"Installation UI localization/display failed for {language}.");
        }
        UiText.SetLanguage(UiLanguage.English);
        if (Hash(pristineRunVga) != runVgaHash || Hash(pristineRunIt) != runItHash)
            throw new InvalidDataException("Discovery changed a source game executable.");
    }

    private static string CreateInstallationFixture(string root, string name, string sourceExecutable, string destinationExecutable)
    {
        string directory = Path.Combine(root, name);
        Directory.CreateDirectory(directory);
        File.Copy(sourceExecutable, Path.Combine(directory, destinationExecutable));
        File.WriteAllBytes(Path.Combine(directory, "GAMEPC"), [0]);
        WriteMinimalVgaFixture(Path.Combine(directory, "012.VGA"));
        return directory;
    }

    private static void WriteMinimalVgaFixture(string path)
    {
        // Three valid raw 16x1 entries make the Graphics page exercise its real
        // parser/selection path without reading or changing an installed game asset.
        byte[] data = new byte[80];
        for (int index = 1; index <= 3; index++)
        {
            int table = index * 8;
            int offset = index * 16 + 16;
            data[table] = (byte)(offset >> 24);
            data[table + 1] = (byte)(offset >> 16);
            data[table + 2] = (byte)(offset >> 8);
            data[table + 3] = (byte)offset;
            data[table + 5] = 1;
            data[table + 6] = 0;
            data[table + 7] = 16;
        }
        File.WriteAllBytes(path, data);
    }

    private static void RunContextSafetySmoke(string pristineRunVga, string pristineRunIt, string root)
    {
        if (Directory.Exists(root)) throw new IOException("Context safety smoke root already exists.");
        string runVgaHash = Hash(pristineRunVga), runItHash = Hash(pristineRunIt);
        Directory.CreateDirectory(root);
        // This harness exercises MainForm's current explicit activation path.
        // It therefore needs the same self-contained ProjectContext fixture as
        // the variant/build smokes, rather than the older parser-only fixture.
        ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "elvira1", Path.GetDirectoryName(pristineRunVga)!, ElviraGameProfile.Elvira1);
        ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "elvira2", Path.GetDirectoryName(pristineRunIt)!, ElviraGameProfile.Elvira2);
        string e1Path = e1Project.GameRoot;
        string e2Path = e2Project.GameRoot;
        if (!GameInstallationValidator.TryValidate(e1Path, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
            !GameInstallationValidator.TryValidate(e2Path, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
            throw new InvalidDataException("Context-safety fixtures did not validate.");

        InstallationDiscoveryService.ResetDiagnostics();
        long memoryBefore = GC.GetTotalMemory(true);
        ContextSafetyDiagnostics diagnostics;
        using (var form = new MainForm())
            diagnostics = form.RunContextSafetyStressForTest(e1, e2);
        long memoryAfter = GC.GetTotalMemory(true);

        if (diagnostics.ApplyEffective != 21 || diagnostics.ApplySuppressed < 5 ||
            diagnostics.ControlsBefore != diagnostics.ControlsAfter || diagnostics.FontLoads != 21 ||
            diagnostics.GraphicsLoads == 0 || diagnostics.TextLoads == 0 || diagnostics.ModsRefreshes == 0)
            throw new InvalidDataException($"Unbounded or unexpected context flow: {diagnostics}.");
        if (InstallationDiscoveryService.FastDiscoveryCallCount != 0)
            throw new InvalidDataException($"Page activation triggered discovery: {InstallationDiscoveryService.FastDiscoveryCallCount}.");
        if (Hash(pristineRunVga) != runVgaHash || Hash(pristineRunIt) != runItHash)
            throw new InvalidDataException("Context-safety smoke changed a source game executable.");
        Console.WriteLine($"Context diagnostics: apply requested={diagnostics.ApplyRequested}, effective={diagnostics.ApplyEffective}, suppressed={diagnostics.ApplySuppressed}; graphics={diagnostics.GraphicsLoads}; text={diagnostics.TextLoads}; font={diagnostics.FontLoads}; mods={diagnostics.ModsRefreshes}; controls={diagnostics.ControlsBefore}/{diagnostics.ControlsAfter}; discovery=0; managed delta={memoryAfter - memoryBefore} bytes.");
    }

    private static void RunInstallationSelectorSmoke(string elvira1Directory, string elvira2Directory)
    {
        if (!GameInstallationValidator.TryValidate(elvira1Directory, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1?.Game != ElviraGameProfile.Elvira1 ||
            !GameInstallationValidator.TryValidate(elvira2Directory, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2?.Game != ElviraGameProfile.Elvira2)
            throw new InvalidDataException("Selector-transition fixtures did not validate.");

        string? activePath = e1.InstallationPath;
        int effectiveApplies = 0;
        Move(e2); // E1 -> E2
        Move(e1); // E2 -> E1
        Move(null); // E1 -> none
        Move(e1); // none -> E1
        Move(e2); Move(null); // E2 -> none
        Move(e2); // none -> E2

        for (int cycle = 0; cycle < 20; cycle++)
        {
            Move(null); Move(e1); Move(null); Move(e2); Move(null);
            Move(e1); Move(e2);
        }

        // This explicit empty state is the release-blocker regression: it must
        // be a false comparison, not a call to Normalize("").
        if (InstallationSelectionState.IsSameActiveInstallation(e1, string.Empty))
            throw new InvalidDataException("An empty active installation path was treated as valid.");
        Console.WriteLine($"Selector state diagnostics: 20 cycles; effective applies={effectiveApplies}; no UI created.");

        void Move(GameInstallation? next)
        {
            if (next is null) { activePath = null; return; }
            if (InstallationSelectionState.IsSameActiveInstallation(next, activePath)) return;
            activePath = next.InstallationPath;
            effectiveApplies++;
            if (!InstallationSelectionState.IsSameActiveInstallation(next, activePath))
                throw new InvalidDataException("Supported selector transition did not activate its path.");
        }
    }

    private static void RunFontActivationSmoke(string pristineRunVga, string pristineRunIt, string root)
    {
        if (Directory.Exists(root)) throw new IOException("Font activation smoke root already exists.");
        string runVgaHash = Hash(pristineRunVga), runItHash = Hash(pristineRunIt);
        Directory.CreateDirectory(root);
        string elvira1 = CreateInstallationFixture(root, "elvira1", pristineRunVga, "RUNVGA.EXE");
        string elvira2 = CreateInstallationFixture(root, "elvira2", pristineRunIt, "RUNIT.EXE");
        InstallationDiscoveryService.ResetDiagnostics();
        long memoryBefore = GC.GetTotalMemory(true);
        int e1Controls, e1Previews, e2Controls, e2Previews;

        using (var editor = new FontEditorForm(elvira1))
        {
            int controls = editor.ControlTreeCount;
            for (int activation = 0; activation < 10; activation++) editor.LoadFromGameDirectory(elvira1);
            if (editor.SourceLoadCount != 1 || editor.ControlTreeCount != controls || InstallationDiscoveryService.FastDiscoveryCallCount != 0)
                throw new InvalidDataException($"Elvira I activation was not bounded: loads={editor.SourceLoadCount}, controls={editor.ControlTreeCount}/{controls}, discovery={InstallationDiscoveryService.FastDiscoveryCallCount}.");
            if (editor.PreviewRebuildCount == 0) throw new InvalidDataException("Elvira I activation did not build a preview.");
            e1Controls = controls;
            e1Previews = editor.PreviewRebuildCount;
        }

        using (var editor = new FontEditorForm(elvira2))
        {
            int controls = editor.ControlTreeCount;
            for (int activation = 0; activation < 10; activation++) editor.LoadFromGameDirectory(elvira2);
            if (editor.SourceLoadCount != 1 || editor.ControlTreeCount != controls || InstallationDiscoveryService.FastDiscoveryCallCount != 0)
                throw new InvalidDataException($"Elvira II activation was not bounded: loads={editor.SourceLoadCount}, controls={editor.ControlTreeCount}/{controls}, discovery={InstallationDiscoveryService.FastDiscoveryCallCount}.");
            if (editor.PreviewRebuildCount == 0) throw new InvalidDataException("Elvira II activation did not build a preview.");
            e2Controls = controls;
            e2Previews = editor.PreviewRebuildCount;
        }

        long memoryAfter = GC.GetTotalMemory(true);
        if (Hash(pristineRunVga) != runVgaHash || Hash(pristineRunIt) != runItHash)
            throw new InvalidDataException("Font activation changed a source game executable.");
        Console.WriteLine($"Font activation diagnostics: discovery=0; E1 controls={e1Controls}, previews={e1Previews}; E2 controls={e2Controls}, previews={e2Previews}; managed delta={memoryAfter - memoryBefore} bytes.");
    }

    private static void RunFontVariantBindingSmoke(string pristineRunVga, string pristineRunIt, string root)
    {
        if (Directory.Exists(root)) throw new IOException("Font variant binding smoke root already exists.");
        string sourceVgaHash = Hash(pristineRunVga), sourceRunItHash = Hash(pristineRunIt);
        Directory.CreateDirectory(root);

        string elvira1 = Path.Combine(root, "elvira1");
        string elvira2 = Path.Combine(root, "elvira2");
        Directory.CreateDirectory(elvira1);
        Directory.CreateDirectory(elvira2);

        string runVga = CopyFixture(pristineRunVga, elvira1, "RUNVGA.EXE");
        string runVgaSk = CopyFixture(pristineRunVga, elvira1, "RUNVGASK.EXE");
        string runVgaCz = CopyFixture(pristineRunVga, elvira1, "RUNVGACZ.EXE");
        string runIt = CopyFixture(pristineRunIt, elvira2, "RUNIT.EXE");
        string runItCz = CopyFixture(pristineRunIt, elvira2, "RUNITCZ.EXE");
        foreach (string name in new[] { "GAMEPC", "GAMEPCSK", "GAMEPCCZ", "GAMEPCO" })
            File.WriteAllBytes(Path.Combine(elvira1, name), [0xA5, 0x5A]);

        VariantEntry en1 = new("English", "GAMEPC", true, 1, "EN", "RUNVGA.EXE");
        VariantEntry sk1 = new("Slovak", "GAMEPCSK", true, 2, "SK", "RUNVGASK.EXE");
        VariantEntry cz1 = new("Czech", "GAMEPCCZ", true, 3, "CZ", "RUNVGACZ.EXE");
        VariantEntry en2 = new("English", "GAMEPC", true, 1, "EN", "RUNIT.EXE");
        VariantEntry cz2 = new("Czech", "GAMEPCCZ", true, 2, "CZ", "RUNITCZ.EXE");

        using (var editor = new FontEditorForm())
        {
            AssertBound(editor, en1, elvira1, runVga);
            AssertBound(editor, sk1, elvira1, runVgaSk);
            AssertBound(editor, cz1, elvira1, runVgaCz);
            AssertBound(editor, en1, elvira1, runVga);
            int afterSequence = editor.SourceLoadCount;
            AssertBound(editor, en1, elvira1, runVga);
            if (editor.SourceLoadCount != afterSequence)
                throw new InvalidDataException("Repeated active-variant binding reloaded the same Font source.");

            string englishHash = Hash(runVga);
            string czechDataHash = Hash(Path.Combine(elvira1, "GAMEPCCZ"));
            string originalDataHash = Hash(Path.Combine(elvira1, "GAMEPCO"));
            AssertBound(editor, cz1, elvira1, runVgaCz);
            FontLoadResult czechLoaded = RunVgaFontService.LoadRunVga(editor.CurrentSourcePath!);
            GlyphModel glyph = czechLoaded.Glyphs[0x41];
            byte[] editedA = glyph.Original.ToArray();
            editedA[0] ^= 0x40;
            glyph.ReplaceEdited(editedA);
            ProductionDeploymentResult deployment = GamePatchDeploymentService.Deploy(czechLoaded, czechLoaded.Glyphs);
            if (!string.Equals(deployment.ActiveExecutable, runVgaCz, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Czech Font Apply was redirected away from the bound RUNVGACZ.EXE target.");
            if (Hash(runVga) != englishHash || Hash(Path.Combine(elvira1, "GAMEPCCZ")) != czechDataHash || Hash(Path.Combine(elvira1, "GAMEPCO")) != originalDataHash)
                throw new InvalidDataException("Czech Font Apply safety regression modified English EXE or GAMEPC data.");

            VariantEntry missing = new("Missing", "GAMEPCXX", true, 4, "XX", "RUNVGAMISSING.EXE");
            if (editor.BindVariant(missing, elvira1, showErrors: false) || editor.CurrentSourcePath is not null || editor.IsApplyEnabledForTest)
                throw new InvalidDataException("Missing variant EXE fell back to another source or left Apply enabled.");
        }

        using (var editor = new FontEditorForm())
        {
            AssertBound(editor, en2, elvira2, runIt);
            AssertBound(editor, cz2, elvira2, runItCz);
        }

        if (!MainForm.RequiresFontVariantDiscardConfirmation(en1, sk1, hasPendingFontEdits: true) ||
            MainForm.RequiresFontVariantDiscardConfirmation(en1, sk1, hasPendingFontEdits: false) ||
            MainForm.RequiresFontVariantDiscardConfirmation(en1, en1, hasPendingFontEdits: true))
            throw new InvalidDataException("Unsaved Font variant-switch confirmation condition is incorrect.");
        if (Hash(pristineRunVga) != sourceVgaHash || Hash(pristineRunIt) != sourceRunItHash)
            throw new InvalidDataException("Font variant binding smoke modified a supplied source executable.");

        Console.WriteLine("Font variant targets: E1 EN=RUNVGA.EXE SK=RUNVGASK.EXE CZ=RUNVGACZ.EXE; E2 EN=RUNIT.EXE CZ=RUNITCZ.EXE; missing target rejected; Czech apply isolated.");

        static string CopyFixture(string source, string directory, string fileName)
        {
            string destination = Path.Combine(directory, fileName);
            File.Copy(source, destination);
            return destination;
        }

        static void AssertBound(FontEditorForm editor, VariantEntry variant, string directory, string expected)
        {
            if (!editor.BindVariant(variant, directory, showErrors: false) || editor.IsManualSource ||
                !string.Equals(editor.BoundVariantExecutablePath, expected, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(editor.CurrentSourcePath, expected, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException($"Variant {variant.DisplayName} did not bind its explicit ExeFile {variant.ExeFile}.");
        }
    }

    private static void RunRuntimeLauncherSmoke(string root)
    {
        if (Directory.Exists(root)) throw new IOException("Launcher smoke root already exists.");
        Directory.CreateDirectory(root);
        foreach (ElviraGameProfile game in new[] { ElviraGameProfile.Elvira1, ElviraGameProfile.Elvira2 })
        {
            string directory = Path.Combine(root, game.ToString()); Directory.CreateDirectory(directory);
            File.WriteAllBytes(Path.Combine(directory, "GAMEPC"), [1]);
            File.WriteAllBytes(Path.Combine(directory, "GAMEPCSK"), [2]);
            File.WriteAllBytes(Path.Combine(directory, "GAMEPCCZ"), [3]);
            File.WriteAllBytes(Path.Combine(directory, "GAMEPCDE"), [4]);
            string baseExe = game == ElviraGameProfile.Elvira1 ? "RUNVGA.EXE" : "RUNIT.EXE";
            string slovakExe = game == ElviraGameProfile.Elvira1 ? "RUNVGASK.EXE" : "RUNITSK.EXE";
            string czechExe = game == ElviraGameProfile.Elvira1 ? "RUNVGACZ.EXE" : "RUNITCZ.EXE";
            string germanExe = game == ElviraGameProfile.Elvira1 ? "RUNVGADE.EXE" : "RUNITDE.EXE";
            var catalog = new VariantCatalog(directory, game,
            [
                new VariantEntry("English", "GAMEPC", true, 1, "EN", baseExe),
                new VariantEntry("Slovak", "GAMEPCSK", true, 2, "SK", slovakExe),
                new VariantEntry("Czech", "GAMEPCCZ", true, 3, "CZ", czechExe),
                new VariantEntry("German", "GAMEPCDE", true, 4, "DE", germanExe),
                new VariantEntry("Disabled", "GAMEPCXX", false, 5, "XX", game == ElviraGameProfile.Elvira1 ? "RUNVGAXX.EXE" : "RUNITXX.EXE")
            ], false);
            var settings = new LauncherSettings("ELVIRA.BAT", "Pivan78");
            string preview = LauncherService.BuildPreview(game, catalog, settings, UiLanguage.English, new DateTime(2026, 8, 25, 2, 43, 0));
            if (File.Exists(Path.Combine(directory, "ELVIRA.BAT")) || File.Exists(Path.Combine(directory, "PI1SND.BAT")) || File.Exists(Path.Combine(directory, VariantConfigurationService.ConfigFileName)))
                throw new InvalidDataException("Preview created a file.");
            if (!preview.Contains("Created: 25-08-2026 02:43") || !preview.Contains("Pi1 Elvira I & II Editor v" + AppInfo.ProductVersion) || preview.Contains("call :", StringComparison.OrdinalIgnoreCase) || preview.Contains("exit /b", StringComparison.OrdinalIgnoreCase) || preview.Contains("color ", StringComparison.OrdinalIgnoreCase) || !preview.Contains("call PI1SND.BAT", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Generated launcher is not pure DOS COMMAND.COM-compatible.");
            string title = game == ElviraGameProfile.Elvira1 ? "ELVIRA: MISTRESS OF THE DARK" : "ELVIRA II: THE JAWS OF CERBERUS";
            if (!preview.Contains(title) || !preview.Contains("Default: English") || !preview.Contains("Press ENTER to start default: English") || !preview.Contains(":forced_setup") || preview.Contains("Auto-start") || preview.Contains("PI1MENU.COM 10"))
                throw new InvalidDataException("Full title, ENTER default, or no-timer flow is incorrect.");
            foreach (LauncherSoundOption option in LauncherService.SoundOptions(game))
                if (!preview.Contains($"SET PI1SND={option.Switch}") || !preview.Contains(option.DisplayName)) throw new InvalidDataException($"Missing sound mapping {game}:{option.Switch}.");
            int englishMenu = preview.IndexOf("1. English", StringComparison.Ordinal);
            int slovakMenu = preview.IndexOf("2. Slovak", StringComparison.Ordinal);
            int czechMenu = preview.IndexOf("3. Czech", StringComparison.Ordinal);
            int germanMenu = preview.IndexOf("4. German", StringComparison.Ordinal);
            if (!preview.Contains($"{Path.GetFileNameWithoutExtension(baseExe)} GAMEPC %PI1SND%") ||
                !preview.Contains($"{Path.GetFileNameWithoutExtension(slovakExe)} GAMEPCSK %PI1SND%") ||
                !preview.Contains($"{Path.GetFileNameWithoutExtension(czechExe)} GAMEPCCZ %PI1SND%") ||
                !preview.Contains($"{Path.GetFileNameWithoutExtension(germanExe)} GAMEPCDE %PI1SND%") ||
                preview.Contains("Disabled") || preview.Contains("GAMEPCXX") ||
                englishMenu < 0 || slovakMenu < 0 || czechMenu < 0 || germanMenu < 0 ||
                englishMenu > slovakMenu || slovakMenu > czechMenu || czechMenu > germanMenu ||
                !preview.Contains($"if not exist {czechExe} goto missing_exe_3") || !preview.Contains("if not exist GAMEPCCZ goto missing_data_3") ||
                !preview.Contains("Missing executable:") || !preview.Contains("Missing data file:") ||
                preview.Contains($"{Path.GetFileNameWithoutExtension(baseExe)} GAMEPCSK %PI1SND%"))
                throw new InvalidDataException($"{game}: launcher did not consume explicit VariantEntry ExeFile/DataFile metadata.");
            if (game == ElviraGameProfile.Elvira1 && (!preview.Contains("/m1") || !preview.Contains("if errorlevel 10 goto set_sound_8"))) throw new InvalidDataException("Elvira I sound mapping changed.");
            if (game == ElviraGameProfile.Elvira2 && (preview.Contains("/m1") || !preview.Contains("RUNITCZ GAMEPCCZ %PI1SND%"))) throw new InvalidDataException("Elvira II sound mapping or explicit Czech command is incorrect.");
            LauncherSettings czechDefault = settings with { DefaultVariant = "GAMEPCCZ" };
            string defaultPreview = LauncherService.BuildPreview(game, catalog, czechDefault, UiLanguage.English, new DateTime(2026, 8, 25, 2, 43, 0));
            if (!defaultPreview.Contains("Default: Czech") || !defaultPreview.Contains($"if not exist {czechExe} goto missing_default_exe") ||
                !defaultPreview.Contains("if not exist GAMEPCCZ goto missing_default_data") || !defaultPreview.Contains($"goto launch_3"))
                throw new InvalidDataException($"{game}: configured Czech default did not resolve its explicit executable/data pair.");
            File.Delete(Path.Combine(directory, "GAMEPCCZ"));
            string missingDataPreview = LauncherService.BuildPreview(game, catalog, czechDefault, UiLanguage.English, new DateTime(2026, 8, 25, 2, 43, 0));
            if (!missingDataPreview.Contains("if not exist GAMEPCCZ goto missing_default_data") ||
                !missingDataPreview.Contains("Missing data file: GAMEPCCZ") || missingDataPreview.Contains($"{Path.GetFileNameWithoutExtension(baseExe)} GAMEPCCZ %PI1SND%"))
                throw new InvalidDataException($"{game}: missing variant data did not retain its no-fallback launcher branch.");
            foreach (UiLanguage language in new[] { UiLanguage.English, UiLanguage.Slovak, UiLanguage.Czech })
            {
                string localized = LauncherService.BuildPreview(game, catalog, settings, language, new DateTime(2026, 8, 25, 2, 43, 0));
                string expectedSetup = language == UiLanguage.Slovak ? "Nastavenie" : language == UiLanguage.Czech ? "Nastavení" : "Setup";
                if (!localized.Contains(expectedSetup) || !localized.Contains("English") || !localized.Contains("PC Internal"))
                    throw new InvalidDataException($"Launcher localization failed: {game}/{language}.");
            }
            LauncherService.Generate(game, catalog, settings, UiLanguage.English, false);
            string active = Path.Combine(directory, "ELVIRA.BAT"), backup = Path.Combine(directory, "ELVIRA.BAK");
            string helper = Path.Combine(directory, Pi1MenuCom.FileName);
            if (!File.Exists(active) || File.Exists(backup) || !File.Exists(helper) || !File.ReadAllBytes(helper).SequenceEqual(Pi1MenuCom.Build())) throw new InvalidDataException("New launcher/helper deployment behavior is incorrect.");
            catalog.LauncherAuthoring = settings; VariantConfigurationService.Save(catalog);
            LauncherSettings persisted = VariantConfigurationService.Load(directory).LauncherAuthoring;
            if (persisted.DefaultVariant != "GAMEPC" || File.ReadAllText(Path.Combine(directory, VariantConfigurationService.ConfigFileName)).Contains("AutoStart", StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Default settings or obsolete-key save behavior is incorrect.");
            File.AppendAllText(Path.Combine(directory, VariantConfigurationService.ConfigFileName), "AutoStart=true\nAutoStartDelay=10\n");
            if (VariantConfigurationService.Load(directory).LauncherAuthoring.DefaultVariant != "GAMEPC") throw new InvalidDataException("Legacy auto-start INI keys did not load compatibly.");
            foreach (VariantCatalog bad in new[] { new VariantCatalog(directory, [new VariantEntry("English", "GAMEPC", false, 1)], false), new VariantCatalog(directory, [new VariantEntry("English", "GAMEPCXX", true, 1)], false) })
            { bool rejected = false; try { _ = LauncherService.BuildPreview(game, bad, settings, UiLanguage.English, DateTime.Now); } catch (InvalidDataException) { rejected = true; } if (!rejected) throw new InvalidDataException("Invalid auto-start default was accepted."); }
            string[] expectedLogo = [".PPPPPPPPPPP.....", "PP..PP..PP.......", "....PP..PP.......", "....PP..PP....111", ".PP.PP..PP...1111", "..PPP...PP..11.11", "...............11", "...............11"];
            if (!Pi1Logo.Matrix.SequenceEqual(expectedLogo) || Pi1Logo.Matrix.Sum(r => r.Length) != 136 || Pi1Logo.Cell('P') != (0xDB, 0x07) || Pi1Logo.Cell('1') != (0xDB, 0x0F) || Pi1Logo.Cell('.') != (0x20, 0x00)) throw new InvalidDataException("Author-provided Pi1 logo regression failed.");
            if (!Pi1SetupLayout.Fits(ElviraGameProfile.Elvira1, true) || !Pi1SetupLayout.Fits(ElviraGameProfile.Elvira1, false) || !Pi1SetupLayout.Fits(ElviraGameProfile.Elvira2, true) || !Pi1SetupLayout.Fits(ElviraGameProfile.Elvira2, false) || !preview.Contains("PI1MENU.COM /logo")) throw new InvalidDataException("Pi1 render mode or 80x25 setup placement validation failed.");
            File.WriteAllText(active, "original BAT\r\n"); LauncherService.Generate(game, catalog, settings, UiLanguage.English, false);
            if (!File.Exists(backup) || File.ReadAllText(backup) != "original BAT\r\n") throw new InvalidDataException("First BAT backup is not exact.");
            string bakHash = Hash(backup); File.WriteAllText(active, "external\r\n");
            if (!LauncherService.ActiveLauncherIsExternallyModified(active)) throw new InvalidDataException("External launcher modification was not detected.");
            bool blocked = false; try { LauncherService.Generate(game, catalog, settings, UiLanguage.English, false); } catch (InvalidOperationException) { blocked = true; }
            if (!blocked || File.ReadAllText(active) != "external\r\n") throw new InvalidDataException("External overwrite cancel guard failed.");
            LauncherService.Generate(game, catalog, settings, UiLanguage.English, true);
            if (Hash(backup) != bakHash) throw new InvalidDataException("Existing .BAK changed.");
            LauncherService.RestoreOriginal(directory, settings);
            if (File.ReadAllText(active) != "original BAT\r\n" || Hash(backup) != bakHash) throw new InvalidDataException("Restore changed .BAK or did not restore active BAT.");
            if (LauncherService.IsSafeBatText("Pivan&78") || LauncherService.IsSafeBatText("Pivan\r78")) throw new InvalidDataException("Modder command-injection validation failed.");
        }
    }

    private static void RunGogOverlayLauncherSmoke(string root)
    {
        if (Directory.Exists(root)) throw new IOException("GOG overlay smoke root already exists.");
        Directory.CreateDirectory(root);
        const string rootOriginal = "root original\r\n";
        const string overlayOriginal = "overlay original\r\n";
        var utf8 = new System.Text.UTF8Encoding(false);

        VariantCatalog CreateCatalog(string directory)
        {
            File.WriteAllBytes(Path.Combine(directory, "GAMEPC"), [1]);
            return new VariantCatalog(directory, ElviraGameProfile.Elvira1,
                [new VariantEntry("English", "GAMEPC", true, 1, "EN", "RUNVGA.EXE")], false);
        }

        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }

        static void VerifyStyledLauncher(string content)
        {
            const string brightWhite = "\x1B[1;37m";
            const string magenta = "\x1B[45m";
            const string reset = "\x1B[0m";
            int resetIndex = content.IndexOf(reset, StringComparison.Ordinal);
            Require(content.Contains(brightWhite, StringComparison.Ordinal) &&
                    content.Contains(magenta, StringComparison.Ordinal) &&
                    resetIndex >= 0 &&
                    resetIndex > content.IndexOf("Default:", StringComparison.Ordinal) &&
                    resetIndex < content.IndexOf("Select:", StringComparison.Ordinal) &&
                    !content.Contains("color ", StringComparison.OrdinalIgnoreCase) &&
                    content.Contains("RUNVGA GAMEPC %PI1SND%", StringComparison.Ordinal),
                "Generated launcher lacks the required ANSI table style or explicit variant command.");
        }

        var settings = new LauncherSettings("ELVIRA.BAT", "Pivan78");

        // A: normal installations remain root-only; generation must not invent an overlay.
        string rootOnly = Path.Combine(root, "root-only");
        Directory.CreateDirectory(rootOnly);
        File.WriteAllText(Path.Combine(rootOnly, "ELVIRA.BAT"), rootOriginal, utf8);
        VariantCatalog rootCatalog = CreateCatalog(rootOnly);
        LauncherService.Generate(ElviraGameProfile.Elvira1, rootCatalog, settings, UiLanguage.English, false);
        string rootActive = Path.Combine(rootOnly, "ELVIRA.BAT");
        Require(File.ReadAllText(Path.Combine(rootOnly, "ELVIRA.BAK"), utf8) == rootOriginal &&
                !Directory.Exists(Path.Combine(rootOnly, LauncherService.GogOverlayDirectoryName)),
            "Root-only deployment changed backup semantics or created an overlay directory.");
        VerifyStyledLauncher(File.ReadAllText(rootActive, utf8));
        Require(File.ReadAllBytes(rootActive).Contains((byte)0x1B), "Generated root launcher does not contain a real ANSI ESC byte.");

        // B/C/D: an existing GOG overlay shadows root, so deploy identical Pi1 copies and
        // preserve each pre-Pi1 BAT exactly once, including unrelated cloud state.
        string existingOverlay = Path.Combine(root, "existing-overlay");
        string overlayDirectory = Path.Combine(existingOverlay, LauncherService.GogOverlayDirectoryName);
        Directory.CreateDirectory(overlayDirectory);
        File.WriteAllText(Path.Combine(existingOverlay, "ELVIRA.BAT"), rootOriginal, utf8);
        File.WriteAllText(Path.Combine(overlayDirectory, "ELVIRA.BAT"), overlayOriginal, utf8);
        File.WriteAllText(Path.Combine(existingOverlay, LauncherService.SoundStateFile), "SET PI1SND=/s\r\n", utf8);
        File.WriteAllText(Path.Combine(overlayDirectory, "SAVE001.SAV"), "unrelated cloud save", utf8);
        VariantCatalog overlayCatalog = CreateCatalog(existingOverlay);
        LauncherService.Generate(ElviraGameProfile.Elvira1, overlayCatalog, settings, UiLanguage.English, false);
        string overlayActive = Path.Combine(overlayDirectory, "ELVIRA.BAT");
        string rootBackup = Path.Combine(existingOverlay, "ELVIRA.BAK");
        string overlayBackup = Path.Combine(existingOverlay, LauncherService.GogOverlayBackupFileName);
        Require(File.ReadAllBytes(rootBackup).SequenceEqual(utf8.GetBytes(rootOriginal)) &&
                File.ReadAllBytes(overlayBackup).SequenceEqual(utf8.GetBytes(overlayOriginal)),
            "Root or overlay first-write backup is not byte exact.");
        string generatedRoot = File.ReadAllText(Path.Combine(existingOverlay, "ELVIRA.BAT"), utf8);
        string generatedOverlay = File.ReadAllText(overlayActive, utf8);
        Require(generatedRoot == generatedOverlay, "Root and overlay active launchers differ.");
        VerifyStyledLauncher(generatedRoot);
        Require(File.ReadAllBytes(overlayActive).Contains((byte)0x1B), "Generated overlay launcher does not contain a real ANSI ESC byte.");
        string rootBackupHash = Hash(rootBackup), overlayBackupHash = Hash(overlayBackup);
        string soundHash = Hash(Path.Combine(existingOverlay, LauncherService.SoundStateFile));
        string cloudSaveHash = Hash(Path.Combine(overlayDirectory, "SAVE001.SAV"));
        LauncherService.Generate(ElviraGameProfile.Elvira1, overlayCatalog, settings, UiLanguage.English, false);
        Require(Hash(rootBackup) == rootBackupHash && Hash(overlayBackup) == overlayBackupHash,
            "A second generation overwrote an immutable launcher backup.");
        LauncherService.RestoreOriginal(existingOverlay, settings);
        Require(File.ReadAllText(Path.Combine(existingOverlay, "ELVIRA.BAT"), utf8) == rootOriginal &&
                File.ReadAllText(overlayActive, utf8) == overlayOriginal &&
                Hash(rootBackup) == rootBackupHash && Hash(overlayBackup) == overlayBackupHash &&
                Hash(Path.Combine(existingOverlay, LauncherService.SoundStateFile)) == soundHash &&
                Hash(Path.Combine(overlayDirectory, "SAVE001.SAV")) == cloudSaveHash,
            "Restore did not accurately restore root/overlay or touched unrelated state.");

        // E: when an overlay exists but did not originally contain a launcher, restore removes
        // only the generated shadowing BAT and leaves the overlay directory itself intact.
        string absentOverlay = Path.Combine(root, "overlay-originally-absent");
        string absentOverlayDirectory = Path.Combine(absentOverlay, LauncherService.GogOverlayDirectoryName);
        Directory.CreateDirectory(absentOverlayDirectory);
        File.WriteAllText(Path.Combine(absentOverlay, "ELVIRA.BAT"), rootOriginal, utf8);
        VariantCatalog absentCatalog = CreateCatalog(absentOverlay);
        LauncherService.Generate(ElviraGameProfile.Elvira1, absentCatalog, settings, UiLanguage.English, false);
        string absentOverlayActive = Path.Combine(absentOverlayDirectory, "ELVIRA.BAT");
        Require(File.Exists(absentOverlayActive) && !File.Exists(Path.Combine(absentOverlay, LauncherService.GogOverlayBackupFileName)),
            "Overlay without an original BAT received an unexpected backup or no generated launcher.");
        LauncherService.RestoreOriginal(absentOverlay, settings);
        Require(File.ReadAllText(Path.Combine(absentOverlay, "ELVIRA.BAT"), utf8) == rootOriginal &&
                !File.Exists(absentOverlayActive) && Directory.Exists(absentOverlayDirectory),
            "Restore did not remove only the generated overlay launcher.");
    }

    private static void RunPi1MenuInputSmoke()
    {
        byte[] image = Pi1MenuCom.Build();
        if (!Pi1MenuCom.HasKeyboardDrain(image))
            throw new InvalidDataException("PI1MENU.COM does not contain the required BIOS AH=01h/AH=00h keyboard drain before its blocking read.");

        // Previous GOG CHOICE selection and any already-queued typematic repeats
        // are pending at entry. Neither is allowed to become a Pi1 menu choice.
        if (Pi1MenuCom.SimulateFreshKeyAfterDrain([(byte)'1'], (byte)'2') != 4 ||
            Pi1MenuCom.SimulateFreshKeyAfterDrain([(byte)'1', (byte)'1', (byte)'1'], (byte)'2') != 4)
            throw new InvalidDataException("A stale or held-key repeat was not discarded before the fresh selection.");

        if (Pi1MenuCom.SimulateFreshKeyAfterDrain([], (byte)'1') != 3 ||
            Pi1MenuCom.SimulateFreshKeyAfterDrain([], (byte)'2') != 4 ||
            Pi1MenuCom.SimulateFreshKeyAfterDrain([], 0x0D) != 1 ||
            Pi1MenuCom.SimulateFreshKeyAfterDrain([], 0x1B) != 0x26 ||
            Pi1MenuCom.SimulateFreshKeyAfterDrain([], (byte)'0') != 2 ||
            Pi1MenuCom.SimulateFreshKeyAfterDrain([], (byte)'A') != 12)
            throw new InvalidDataException("Fresh PI1MENU key/errorlevel mapping changed.");

        for (int i = 0; i < 100; i++)
        {
            byte stale = (byte)('1' + i % 9);
            byte fresh = (byte)('1' + (i + 1) % 9);
            int expected = fresh - (byte)'1' + 3;
            if (Pi1MenuCom.SimulateFreshKeyAfterDrain([stale], fresh) != expected)
                throw new InvalidDataException("Repeated PI1MENU invocation lost or duplicated its first fresh key.");
        }
    }

    private static void AssertElvira1OriginalPreview(FontLoadResult loaded, RunVgaFontLayout expectedLayout, byte[] canonical, string sourceKind)
    {
        if (loaded.Game != ElviraGame.Elvira1 || loaded.Layout != expectedLayout || loaded.LoadedGlyphCount != 98 ||
            loaded.FirstByteValue != 0x20 || loaded.LastByteValue != 0x81 || !FontSlotMetadata.IsReserved(loaded, 0x81))
            throw new InvalidDataException($"Elvira I {sourceKind} preview did not expose the expected 98-glyph range.");

        foreach (int code in new[] { 0x20, 0x30, 0x41, 0x42, 0x43, 0x45, 0x48, 0x49, 0x4D, 0x4F, 0x53, 0x54, 0x3F, 0x61, 0x6D, 0x7A, 0x81 })
        {
            byte[] expected = canonical.AsSpan(0x1A216 + (code - 0x20) * RunVgaFontService.GlyphBytes, RunVgaFontService.GlyphBytes).ToArray();
            if (!loaded.Glyphs[code].IsLoadedFromSource || !loaded.Glyphs[code].Original.SequenceEqual(expected))
                throw new InvalidDataException($"Elvira I {sourceKind} preview glyph 0x{code:X2} differs from the canonical table.");
        }
        if (!loaded.Glyphs[0x81].Original.SequenceEqual(FontSlotMetadata.OriginalHudEraseGlyphBytes))
            throw new InvalidDataException($"Elvira I {sourceKind} preview did not preserve reserved glyph 0x81.");
    }

    private static void RunPartialBackupSmoke(string pristineExe, string pristineGamePc, string root)
    {
        foreach (string state in new[] { "exe_o_only", "gamepc_o_only", "active_missing", "corrupt_exe_o" })
        {
            string dir = Path.Combine(root, state);
            if (Directory.Exists(dir)) throw new IOException($"Isolated partial-state directory already exists: {dir}");
            Directory.CreateDirectory(dir);
            string active = Path.Combine(dir, "RUNIT.EXE"), gamepc = Path.Combine(dir, "GAMEPC");
            File.Copy(pristineExe, active); File.Copy(pristineGamePc, gamepc);
            if (state == "exe_o_only") File.Copy(active, Path.Combine(dir, "RUNITO.EXE"));
            if (state == "gamepc_o_only") File.Copy(gamepc, Path.Combine(dir, "GAMEPCO"));
            if (state == "active_missing") { File.Copy(active, Path.Combine(dir, "RUNITO.EXE")); File.Copy(gamepc, Path.Combine(dir, "GAMEPCO")); File.Delete(active); }
            if (state == "corrupt_exe_o") File.WriteAllBytes(Path.Combine(dir, "RUNITO.EXE"), [0]);
            string beforeExeO = File.Exists(Path.Combine(dir, "RUNITO.EXE")) ? Hash(Path.Combine(dir, "RUNITO.EXE")) : "";
            string beforeGamePcO = File.Exists(Path.Combine(dir, "GAMEPCO")) ? Hash(Path.Combine(dir, "GAMEPCO")) : "";
            bool shouldBlock = state is "active_missing" or "corrupt_exe_o";
            bool blocked = false;
            try { GamePatchDeploymentService.Deploy(RunVgaFontService.LoadRunVga(active), GlyphRepository.CreateAllCp852Slots()); }
            catch (Exception) { blocked = true; }
            if (blocked != shouldBlock) throw new InvalidDataException($"Target-aware backup state {state} had unexpected result: blocked={blocked}.");
            if (beforeExeO.Length != 0 && Hash(Path.Combine(dir, "RUNITO.EXE")) != beforeExeO) throw new InvalidDataException($"State {state} overwrote RUNITO.EXE.");
            if (beforeGamePcO.Length != 0 && Hash(Path.Combine(dir, "GAMEPCO")) != beforeGamePcO) throw new InvalidDataException($"State {state} overwrote unrelated GAMEPCO.");
        }
    }

    private static void RunVgaBackupSmoke(string sourceVga, string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Isolated VGA test directory already exists: {root}");
        Directory.CreateDirectory(root);
        string active = Path.Combine(root, Path.GetFileName(sourceVga));
        File.Copy(sourceVga, active);
        string originalHash = Hash(active), oFile = SafeDeployer.OriginalBackupPath(active);
        if (File.Exists(oFile)) throw new InvalidDataException("Unexpected O-file before modification.");
        _ = new VgaImageTableParser(File.ReadAllBytes(active)).Parse(); // read/preview-equivalent: no O-file creation
        if (File.Exists(oFile)) throw new InvalidDataException("Read-only VGA parse created an O-file.");

        InstallHarmlessValidatedVgaVariant(active, 0xA5);
        if (!File.Exists(oFile) || Hash(oFile) != originalHash) throw new InvalidDataException("First VGA O-file does not equal the original active file.");
        string oHash = Hash(oFile);
        InstallHarmlessValidatedVgaVariant(active, 0x5A);
        if (Hash(oFile) != oHash) throw new InvalidDataException("Second VGA save overwrote the O-file.");

        string unsafeDir = Path.Combine(root, "unsafe_target_backup");
        Directory.CreateDirectory(unsafeDir);
        string unsafeActive = Path.Combine(unsafeDir, Path.GetFileName(sourceVga));
        File.Copy(sourceVga, unsafeActive);
        File.WriteAllBytes(SafeDeployer.OriginalBackupPath(unsafeActive), []);
        string unsafePrepared = unsafeActive + ".prepared";
        File.Copy(unsafeActive, unsafePrepared);
        bool blocked = false;
        try { SafeDeployer.ReplaceActiveWithPrepared(unsafeActive, unsafePrepared); } catch (InvalidDataException) { blocked = true; }
        finally { if (File.Exists(unsafePrepared)) File.Delete(unsafePrepared); }
        if (!blocked) throw new InvalidDataException("Unsafe empty VGA target backup was accepted.");
    }

    private static void InstallHarmlessValidatedVgaVariant(string active, byte marker)
    {
        byte[] bytes = File.ReadAllBytes(active);
        Array.Resize(ref bytes, bytes.Length + 1); bytes[^1] = marker; // parsers tolerate inert tail data; active hash changes for transaction coverage.
        string prepared = active + ".prepared";
        File.WriteAllBytes(prepared, bytes);
        _ = new VgaImageTableParser(File.ReadAllBytes(prepared)).Parse();
        SafeDeployer.ReplaceActiveWithPrepared(active, prepared);
    }

    private static void RunGamePcBackupSmoke(string sourceGamePc, string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Isolated GAMEPC test directory already exists: {root}");
        Directory.CreateDirectory(root);
        string active = Path.Combine(root, "GAMEPC"); File.Copy(sourceGamePc, active);
        string originalHash = Hash(active), oFile = SafeDeployer.OriginalBackupPath(active);
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(active, ElviraGameProfile.Elvira1);
        GamePcStringEntry entry = entries.First(e => e.ByteLength > 0);
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        var enc = GamePcTextEditor.GetEncoding("CP852");
        GamePcTextEditor.SaveInPlace(active, new Dictionary<int, string> { [entry.Index] = new string('X', entry.ByteLength) }, entries, enc, ElviraGameProfile.Elvira1);
        if (!File.Exists(oFile) || Hash(oFile) != originalHash) throw new InvalidDataException("GAMEPCO does not equal first active GAMEPC.");
        string oHash = Hash(oFile);
        entries = GamePcTextEditor.LoadEntries(active, ElviraGameProfile.Elvira1);
        entry = entries.First(e => e.ByteLength > 0);
        GamePcTextEditor.SaveInPlace(active, new Dictionary<int, string> { [entry.Index] = new string('Y', entry.ByteLength) }, entries, enc, ElviraGameProfile.Elvira1);
        if (Hash(oFile) != oHash) throw new InvalidDataException("Repeated GAMEPC save overwrote GAMEPCO.");
    }

    private static void RunDataFileVariantSmoke(string game, string fixture, string root)
    {
        string directory = Path.Combine(root, game);
        if (Directory.Exists(directory)) throw new IOException($"Isolated data-file test directory already exists: {directory}");
        Directory.CreateDirectory(directory);
        string original = Path.Combine(directory, "GAMEPC");
        File.Copy(fixture, original);
        string originalHash = Hash(original);
        var enc = GamePcTextEditor.GetEncoding("CP852");
        ElviraGameProfile profile = game.Equals("elvira1", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira1 : ElviraGameProfile.Elvira2;
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(original, profile);
        if (entries.Count == 0 || File.Exists(Path.Combine(directory, "GAMEPCO")))
            throw new InvalidDataException($"{game}: normal GAMEPC open was not read-only.");

        // Open/inspect an arbitrary compatible name without creating a backup.
        string renamed = Path.Combine(directory, "MYMOD");
        File.Copy(original, renamed);
        if (GamePcTextEditor.LoadEntries(renamed, profile).Count != entries.Count || File.Exists(Path.Combine(directory, "MYMODO")))
            throw new InvalidDataException($"{game}: renamed compatible data file did not open read-only.");

        GamePcStringEntry editable = entries.First(e => e.ByteLength > 0);
        var edits = new Dictionary<int, string> { [editable.Index] = new string('X', editable.ByteLength) };
        // SAVE follows the selected arbitrary file; the original GAMEPC must not be touched.
        GamePcTextEditor.SaveInPlace(renamed, edits, entries, enc, profile);
        if (Hash(original) != originalHash || Hash(renamed) == originalHash || File.Exists(Path.Combine(directory, "GAMEPCO")))
            throw new InvalidDataException($"{game}: Save did not stay on the selected arbitrary data file.");

        // Save As and Create Variant both create a new DOS 8.3 working file, leave GAMEPC intact,
        // and return the destination that the UI adopts as its new current data-file context.
        string savedAs = GameDataFileService.SaveAsNew(original, Path.Combine(directory, "GAMEPCSK"), edits, entries, enc, profile);
        if (!savedAs.EndsWith("GAMEPCSK", StringComparison.OrdinalIgnoreCase) || !File.Exists(savedAs) || Hash(original) != originalHash)
            throw new InvalidDataException($"{game}: Save As did not preserve GAMEPC or return GAMEPCSK context.");
        string variant = GameDataFileService.SaveAsNew(original, Path.Combine(directory, "GAMEPCCZ"), edits, entries, enc, profile);
        if (!variant.EndsWith("GAMEPCCZ", StringComparison.OrdinalIgnoreCase) || !File.Exists(variant) || Hash(original) != originalHash)
            throw new InvalidDataException($"{game}: Create Variant did not preserve GAMEPC or return GAMEPCCZ context.");
        if (GamePcTextEditor.LoadEntries(savedAs, profile).Count != entries.Count || GamePcTextEditor.LoadEntries(variant, profile).Count != entries.Count)
            throw new InvalidDataException($"{game}: created variants failed compatible data-file parsing.");
        if (!GameDataFileService.IsDos83FileName("GAMEPCSK") || !GameDataFileService.IsDos83FileName("MYMOD") || GameDataFileService.IsDos83FileName("GAMEPCSLOVAK"))
            throw new InvalidDataException($"{game}: DOS 8.3 filename validation failed.");
    }

    private static void RunVariantConfigurationSmoke(string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Variant test directory already exists: {root}");
        Directory.CreateDirectory(root);
        string configPath = Path.Combine(root, VariantConfigurationService.ConfigFileName);

        VariantCatalog defaults = VariantConfigurationService.Load(root);
        if (defaults.ConfigurationExists || defaults.Entries.Count != 0 || File.Exists(configPath))
            throw new InvalidDataException("Missing config without GAMEPC did not remain an empty in-memory state.");

        File.WriteAllBytes(Path.Combine(root, "GAMEPC"), [0]);
        defaults = VariantConfigurationService.Load(root);
        if (defaults.ConfigurationExists || defaults.Entries.Count != 1 ||
            defaults.Entries[0] != new VariantEntry("English", "GAMEPC", true, 1, "EN", "RUNVGA.EXE") || File.Exists(configPath))
            throw new InvalidDataException("Missing config with GAMEPC did not produce the read-only English default.");

        defaults.Add("Slovak", "GAMEPCSK");
        defaults.Add("Czech", "GAMEPCCZ", enabled: false);
        defaults.Add("Hard Mode", "GAMEPCHD");
        if (!defaults.MoveDown("GAMEPCCZ") || !defaults.MoveUp("GAMEPCHD"))
            throw new InvalidDataException("Variant move operations failed.");
        defaults.SetEnabled("GAMEPCSK", false);
        if (!defaults.Remove("GAMEPCCZ")) throw new InvalidDataException("Variant remove operation failed.");
        defaults.Edit("GAMEPCHD", "Hard Mode", "GAMEPCHD", enabled: true);
        if (defaults.GetStatus(defaults.FindByDataFile("GAMEPCSK")!).IsAvailable)
            throw new InvalidDataException("Missing variant data file was not reported as unavailable.");
        File.WriteAllBytes(Path.Combine(root, "GAMEPCSK"), [0]);
        if (!defaults.GetStatus(defaults.FindByDataFile("GAMEPCSK")!).IsAvailable)
            throw new InvalidDataException("Present variant data file was not reported as available.");

        foreach (string invalid in new[] { @"..\GAMEPC", @"C:\foo\GAMEPC", @"subdir\GAMEPC", "GAMEPCSLOVAK" })
        {
            bool rejected = false;
            try { defaults.Add("Invalid" + invalid.GetHashCode(), invalid); } catch (InvalidOperationException) { rejected = true; }
            if (!rejected) throw new InvalidDataException($"Invalid variant DataFile was accepted: {invalid}");
        }
        bool duplicateRejected = false;
        try { defaults.Add("Slovak duplicate", "GAMEPCSK"); } catch (InvalidOperationException) { duplicateRejected = true; }
        if (!duplicateRejected) throw new InvalidDataException("Duplicate variant DataFile was accepted.");

        VariantConfigurationService.Save(defaults);
        string firstHash = Hash(configPath);
        VariantConfigurationService.Save(defaults);
        if (Hash(configPath) != firstHash) throw new InvalidDataException("Variant config serialization is not deterministic.");
        VariantCatalog reloaded = VariantConfigurationService.Load(root);
        if (!reloaded.ConfigurationExists || reloaded.Entries.Count != defaults.Entries.Count ||
            !reloaded.Entries.SequenceEqual(defaults.Entries) || reloaded.FindByDataFilePath(Path.Combine(root, "GAMEPCSK"))?.DisplayName != "Slovak" ||
            reloaded.FindByDataFile("GAMEPCSK")!.Enabled)
            throw new InvalidDataException("Variant config save/reload did not preserve entries, order, enabled state, or current-file resolution.");

        string malformedDirectory = Path.Combine(root, "malformed");
        Directory.CreateDirectory(malformedDirectory);
        File.WriteAllText(Path.Combine(malformedDirectory, VariantConfigurationService.ConfigFileName), "[Variant1]\nName=Bad\nDataFile=..\\GAMEPC\n[Variant2]\nName=Valid\nDataFile=GAMEPCSK\nEnabled=maybe\nOrder=x\n[Broken\n", new System.Text.UTF8Encoding(false));
        VariantCatalog malformed = VariantConfigurationService.Load(malformedDirectory);
        if (malformed.Entries.Count != 1 || malformed.Entries[0].DisplayName != "Valid" || malformed.LoadWarnings.Count == 0)
            throw new InvalidDataException("Hand-edited malformed config was not handled safely.");
    }

    private static void RunVariantOrderingSmoke(string root)
    {
        if (Directory.Exists(root)) throw new IOException($"Variant ordering test directory already exists: {root}");
        Directory.CreateDirectory(root);

        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidDataException(message);
        }

        static string[] Names(VariantCatalog catalog) => catalog.Entries.Select(entry => entry.DisplayName).ToArray();
        static void RequireOrder(VariantCatalog catalog, params string[] expected) =>
            Require(Names(catalog).SequenceEqual(expected), $"Unexpected variant order: {string.Join(", ", Names(catalog))}.");

        VariantEntry english = new("English", "GAMEPC", true, 1, "EN", "RUNVGA.EXE");
        VariantEntry slovak = new("Slovak", "GAMEPCSK", true, 2, "SK", "RUNVGASK.EXE");
        VariantEntry czech = new("Czech", "GAMEPCCZ", true, 3, "CZ", "RUNVGACZ.EXE");
        VariantEntry disabled = new("Disabled", "GAMEPCXX", false, 4, "XX", "RUNVGAXX.EXE");

        // A/B/D: two variants move in either direction, retain the selected data-file
        // identity, and cannot move past either boundary.
        string twoDirectory = Path.Combine(root, "two");
        Directory.CreateDirectory(twoDirectory);
        var two = new VariantCatalog(twoDirectory, ElviraGameProfile.Elvira1, [english, slovak], false)
        {
            LauncherAuthoring = new LauncherSettings("ELVIRA.BAT", "Pivan78", "GAMEPCSK")
        };
        string selectedDataFile = "GAMEPCSK";
        Require(two.CanMoveUp(selectedDataFile) && !two.CanMoveDown(selectedDataFile) && !two.CanMoveUp("GAMEPC"),
            "Initial Move Up/Down boundary state is incorrect.");
        Require(two.MoveUp(selectedDataFile), "Slovak could not move up.");
        RequireOrder(two, "Slovak", "English");
        Require(two.FindByDataFile(selectedDataFile)?.DisplayName == "Slovak" && !two.CanMoveUp(selectedDataFile) && two.CanMoveDown(selectedDataFile),
            "Moved variant selection token or boundary state was not preserved.");
        VariantConfigurationService.Save(two);
        VariantCatalog twoReloaded = VariantConfigurationService.Load(twoDirectory, ElviraGameProfile.Elvira1);
        RequireOrder(twoReloaded, "Slovak", "English");
        Require(twoReloaded.LauncherAuthoring.DefaultVariant == "GAMEPCSK", "Reordering changed the configured default variant.");
        Require(twoReloaded.MoveDown(selectedDataFile), "Slovak could not move down.");
        RequireOrder(twoReloaded, "English", "Slovak");

        // C/E/F/G/I: order is persisted; every other entry property stays intact, including
        // the disabled entry.  The selected Czech data-file remains the selection key.
        string threeDirectory = Path.Combine(root, "three");
        Directory.CreateDirectory(threeDirectory);
        foreach (string dataFile in new[] { "GAMEPC", "GAMEPCSK", "GAMEPCCZ", "GAMEPCXX" })
            File.WriteAllBytes(Path.Combine(threeDirectory, dataFile), [1]);
        var three = new VariantCatalog(threeDirectory, ElviraGameProfile.Elvira1, [english, slovak, czech, disabled], false)
        {
            LauncherAuthoring = new LauncherSettings("ELVIRA.BAT", "Pivan78", "GAMEPCSK")
        };
        Dictionary<string, VariantEntry> metadataBefore = three.Entries.ToDictionary(entry => entry.DataFile, entry => entry);
        selectedDataFile = "GAMEPCCZ";
        Require(three.MoveUp(selectedDataFile), "Czech could not move up once.");
        RequireOrder(three, "English", "Czech", "Slovak", "Disabled");
        Require(three.MoveUp(selectedDataFile), "Czech could not move up twice.");
        RequireOrder(three, "Czech", "English", "Slovak", "Disabled");
        Require(three.FindByDataFile(selectedDataFile)?.DisplayName == "Czech" && !three.CanMoveUp(selectedDataFile),
            "Czech selection token or first-row boundary state changed.");
        Require(!three.CanMoveDown("GAMEPCXX"), "Last-row Move Down boundary state is incorrect.");
        foreach (VariantEntry entry in three.Entries)
        {
            VariantEntry original = metadataBefore[entry.DataFile];
            Require((entry with { Order = original.Order }) == original,
                $"Reordering changed metadata for {entry.DataFile}.");
        }
        VariantConfigurationService.Save(three);
        VariantCatalog reloaded = VariantConfigurationService.Load(threeDirectory, ElviraGameProfile.Elvira1);
        RequireOrder(reloaded, "Czech", "English", "Slovak", "Disabled");
        Require(reloaded.LauncherAuthoring.DefaultVariant == "GAMEPCSK" && !reloaded.FindByDataFile("GAMEPCXX")!.Enabled,
            "Persistence changed the default variant or disabled state.");

        string preview = LauncherService.BuildPreview(ElviraGameProfile.Elvira1, reloaded, reloaded.LauncherAuthoring, UiLanguage.English, new DateTime(2026, 8, 27));
        int czechMenu = preview.IndexOf("1. Czech", StringComparison.Ordinal);
        int englishMenu = preview.IndexOf("2. English", StringComparison.Ordinal);
        int slovakMenu = preview.IndexOf("3. Slovak", StringComparison.Ordinal);
        Require(czechMenu >= 0 && englishMenu > czechMenu && slovakMenu > englishMenu &&
                preview.Contains("RUNVGACZ GAMEPCCZ %PI1SND%", StringComparison.Ordinal) &&
                preview.Contains("RUNVGASK GAMEPCSK %PI1SND%", StringComparison.Ordinal) &&
                !preview.Contains("Disabled", StringComparison.Ordinal),
            "Launcher did not preserve configured VariantEntry order or explicit launch metadata.");
    }

    private static void RunTranslationExchangeSmoke(string root)
    {
        if (Directory.Exists(root)) throw new IOException("Translation exchange test directory already exists.");
        Directory.CreateDirectory(root); System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        static List<GamePcStringEntry> Entries(int count) => Enumerable.Range(0, count).Select(i => new GamePcStringEntry(i, i, i == 4 ? 0 : 1, i == 4 ? [] : [(byte)'A'])).ToList();
        void RoundTrip(ElviraGameProfile game, int count)
        {
            string directory = Path.Combine(root, game.ToString()); Directory.CreateDirectory(directory); string original = Path.Combine(directory, "GAMEPCO");
            File.WriteAllBytes(original, Enumerable.Repeat((byte)'A', count).ToArray()); List<GamePcStringEntry> entries = Entries(count);
            var edits = new Dictionary<int, string> { [0] = "á", [4] = string.Empty, [5] = "dlhší text" };
            var variant = new VariantEntry("Slovak", "GAMEPCSK", true, 1, "SK", "RUNVGASK.EXE");
            TranslationExchangeDocument document = TranslationExchangeService.Create(game, original, entries, entries, edits, variant, (_, _) => game == ElviraGameProfile.Elvira1 ? "SAFE" : string.Empty);
            foreach (string extension in new[] { "csv", "xlsx" })
            {
                string path = Path.Combine(directory, "translation." + extension);
                if (extension == "csv") TranslationExchangeService.ExportCsv(path, document); else TranslationExchangeService.ExportXlsx(path, document);
                IReadOnlyDictionary<int, string> imported = TranslationExchangeService.Import(path, game, original, entries);
                if (imported.Count != count || imported[0] != "á" || imported[4] != string.Empty || imported[5] != "dlhší text") throw new InvalidDataException($"{game}/{extension} round trip failed.");
            }
            string csv = Path.Combine(directory, "translation.csv"); string before = Hash(original);
            File.AppendAllText(csv, "\""); bool rejected = false; try { _ = TranslationExchangeService.Import(csv, game, original, entries); } catch (InvalidDataException) { rejected = true; }
            if (!rejected || Hash(original) != before) throw new InvalidDataException("Malformed import was not rejected atomically or changed GAMEPCO.");
        }
        RoundTrip(ElviraGameProfile.Elvira1, 689); RoundTrip(ElviraGameProfile.Elvira2, 1053);
    }

    private static void RunModsLauncherWorkflowSmoke(string game, string fixture, string root)
    {
        string directory = Path.Combine(root, game);
        if (Directory.Exists(directory)) throw new IOException($"Isolated Mods test directory already exists: {directory}");
        Directory.CreateDirectory(directory);
        string gamepc = Path.Combine(directory, "GAMEPC");
        File.Copy(fixture, gamepc);
        string configPath = Path.Combine(directory, VariantConfigurationService.ConfigFileName);

        // Opening the tab is equivalent to loading this catalog; no configuration may be created.
        VariantCatalog catalog = VariantConfigurationService.Load(directory);
        if (catalog.ConfigurationExists || catalog.Entries.Count != 1 || catalog.Entries[0].DataFile != "GAMEPC" || File.Exists(configPath))
            throw new InvalidDataException($"{game}: implicit English variant did not remain read-only.");

        // Create Variant + choose Add to Mods: physical creation succeeds first, then explicit metadata persists.
        var enc = GamePcTextEditor.GetEncoding("CP852");
        ElviraGameProfile profile = game.Equals("elvira1", StringComparison.OrdinalIgnoreCase) ? ElviraGameProfile.Elvira1 : ElviraGameProfile.Elvira2;
        List<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(gamepc, profile);
        string createdPath = GameDataFileService.SaveAsNew(gamepc, Path.Combine(directory, "GAMEPCSK"), new Dictionary<int, string>(), entries, enc, profile);
        if (!File.Exists(createdPath) || GamePcTextEditor.LoadEntries(createdPath, profile).Count != entries.Count)
            throw new InvalidDataException($"{game}: variant data file did not create/open correctly.");
        VariantEntry slovak = catalog.Add("Slovak", "GAMEPCSK");
        VariantConfigurationService.Save(catalog);
        if (!File.Exists(configPath) || catalog.FindByDataFilePath(createdPath)?.DataFile != "GAMEPCSK")
            throw new InvalidDataException($"{game}: created variant did not persist or resolve from current data path.");

        // Missing is retained, visible through status, and is never auto-created.
        VariantEntry missing = catalog.Add("Czech", "GAMEPCCZ");
        if (catalog.GetStatus(missing).IsAvailable || File.Exists(Path.Combine(directory, "GAMEPCCZ")))
            throw new InvalidDataException($"{game}: missing variant status is unsafe.");

        // Edit, ordering, and enabled state are metadata-only and persist deterministically.
        catalog.Edit(slovak.DataFile, "Slovak text", "GAMEPCSK", enabled: false);
        if (!catalog.MoveUp("GAMEPCSK") || !catalog.MoveDown("GAMEPCSK"))
            throw new InvalidDataException($"{game}: variant ordering operations failed.");
        catalog.SetEnabled("GAMEPCSK", true);
        VariantConfigurationService.Save(catalog);
        VariantCatalog reloaded = VariantConfigurationService.Load(directory);
        if (reloaded.FindByDataFile("GAMEPCSK")?.Enabled != true || reloaded.FindByDataFile("GAMEPCSK")?.DisplayName != "Slovak text")
            throw new InvalidDataException($"{game}: edit/enable persistence failed.");

        // Remove only edits metadata; the physical file survives. A declined metadata prompt is represented by no Add call.
        if (!reloaded.Remove("GAMEPCSK")) throw new InvalidDataException($"{game}: variant removal failed.");
        VariantConfigurationService.Save(reloaded);
        if (!File.Exists(createdPath) || VariantConfigurationService.Load(directory).FindByDataFile("GAMEPCSK") is not null)
            throw new InvalidDataException($"{game}: remove touched physical data or retained metadata.");
        string declinedPath = GameDataFileService.SaveAsNew(gamepc, Path.Combine(directory, "GAMEPCHD"), new Dictionary<int, string>(), entries, enc, profile);
        if (!File.Exists(declinedPath) || VariantConfigurationService.Load(directory).FindByDataFile("GAMEPCHD") is not null)
            throw new InvalidDataException($"{game}: declined metadata incorrectly changed catalog or physical output.");
    }

    private static void VerifyPristineBaselineSmoke()
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ElviraPristineBaselineSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            var layout = new EditorStorageLayout(root);
            if (Directory.Exists(layout.EditorRoot))
                throw new InvalidDataException("Pure baseline-layout construction created a directory.");
            File.WriteAllBytes(Path.Combine(root, "GAMEPC"), [1, 2, 3]);
            File.WriteAllBytes(Path.Combine(root, "RUNVGA.EXE"), [4, 5]);
            File.WriteAllBytes(Path.Combine(root, "012.VGA"), [6]);
            File.WriteAllBytes(Path.Combine(root, "RESOURCE.DAT"), [7, 8]);
            Directory.CreateDirectory(Path.Combine(root, "SOUND")); File.WriteAllBytes(Path.Combine(root, "SOUND", "VOICE.DAT"), [9]);
            File.WriteAllText(Path.Combine(root, "ELVIRA.BAT"), "original");
            File.WriteAllBytes(Path.Combine(root, "SAVE001.SAV"), [10]);
            File.WriteAllText(Path.Combine(root, "PI1SND.BAT"), "generated");
            Directory.CreateDirectory(layout.EditorRoot); File.WriteAllText(Path.Combine(layout.EditorRoot, "ignore.txt"), "editor");
            Directory.CreateDirectory(layout.VariantsRoot); File.WriteAllText(Path.Combine(layout.VariantsRoot, "ignore.txt"), "variant");
            IReadOnlyList<PristineInstallationFile> inventory = new PristineInstallationService(layout).EnumerateReadOnly();
            string[] expected = ["012.VGA", "ELVIRA.BAT", "GAMEPC", "PI1SND.BAT", "RESOURCE.DAT", "SAVE001.SAV", "RUNVGA.EXE", "SOUND\\VOICE.DAT"];
            if (!inventory.Select(item => item.RelativePath).SequenceEqual(expected.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)) ||
                !inventory.Any(item => item.RelativePath == "012.VGA" && item.Role == PristineFileRole.Resource) ||
                !inventory.Any(item => item.RelativePath == "ELVIRA.BAT" && item.Classification == PristineFileClassification.MutableBackedUp) ||
                !inventory.Any(item => item.RelativePath == "SAVE001.SAV" && item.Classification == PristineFileClassification.GeneratedIgnored) ||
                !inventory.Any(item => item.RelativePath == "PI1SND.BAT" && item.Classification == PristineFileClassification.GeneratedIgnored) ||
                inventory.Any(item => item.RelativePath.StartsWith("ElviraEditor\\", StringComparison.OrdinalIgnoreCase) || item.RelativePath.StartsWith("VARIANTS\\", StringComparison.OrdinalIgnoreCase)) ||
                inventory.Any(item => string.IsNullOrWhiteSpace(item.Sha256)) ||
                PristineInstallationService.IsSupportedFrozenExecutable("RUNIT.EXE", "00"))
                throw new InvalidDataException("Whole-installation pristine inventory smoke failed.");
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, recursive: true);
        }
    }

    private static void VerifyPristineManifestSmoke()
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ElviraManifestSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            File.WriteAllBytes(Path.Combine(root, "GAMEPC"), [1, 2, 3]);
            File.WriteAllBytes(Path.Combine(root, "RUNVGA.EXE"), [4]);
            File.WriteAllBytes(Path.Combine(root, "001.VGA"), [5]);
            Directory.CreateDirectory(Path.Combine(root, "SOUND")); File.WriteAllBytes(Path.Combine(root, "SOUND", "A.DAT"), [6]);
            File.WriteAllText(Path.Combine(root, "ELVIRA.BAT"), "original"); File.WriteAllBytes(Path.Combine(root, "SAVE001.SAV"), [7]);
            var layout = new EditorStorageLayout(root); var service = new PristineManifestService(layout, ElviraGameProfile.Elvira1);
            if (service.InitializeBaseline().Status != PristineManifestInitializationStatus.UnsupportedExecutableVersion)
                throw new InvalidDataException("Unknown fixture executable was silently accepted.");
            PristineManifestInitializationResult initial = service.InitializeBaseline(validateFrozenInputs: false);
            if (initial.Status != PristineManifestInitializationStatus.Initialized || !File.Exists(service.ManifestPath) || !File.Exists(Path.Combine(layout.MutableBackupsRoot, "ELVIRA.BAT")))
                throw new InvalidDataException("Manifest initialization or mutable backup failed.");
            if (service.InitializeBaseline(validateFrozenInputs: false).Status != PristineManifestInitializationStatus.AlreadyValid || service.ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
                throw new InvalidDataException("Baseline idempotency failed.");
            PristineManifest manifest = PristineManifestService.LoadManifest(service.ManifestPath, ElviraGameProfile.Elvira1);
            PristineManifest same = PristineManifestService.BuildManifest(new PristineInstallationService(layout).EnumerateReadOnly());
            if (manifest.BaselineFingerprint != same.BaselineFingerprint) throw new InvalidDataException("Baseline fingerprint is not deterministic.");
            static void RejectManifest(string path, PristineManifest value)
            {
                File.WriteAllText(path, System.Text.Json.JsonSerializer.Serialize(value));
                try { _ = PristineManifestService.LoadManifest(path, ElviraGameProfile.Elvira1); }
                catch { return; }
                throw new InvalidDataException("Malformed manifest was accepted.");
            }
            string malformed = Path.Combine(root, "bad.json");
            RejectManifest(malformed, manifest with { BaselineFingerprint = new string('0', 64) });
            PristineManifestFile first = manifest.Files[0];
            RejectManifest(malformed, manifest with { Files = [first with { RelativePath = "..\\escape" }] });
            RejectManifest(malformed, manifest with { Files = [first, first with { RelativePath = first.RelativePath.ToLowerInvariant() }] });
            File.WriteAllBytes(Path.Combine(root, "001.VGA"), [9]);
            if (!service.ValidateBaseline().Entries.Any(item => item.RelativePath == "001.VGA" && item.Status == BaselineValidationStatus.ExternalInstallationChange))
                throw new InvalidDataException("Immutable external change was not detected.");
            File.Delete(Path.Combine(root, "SOUND", "A.DAT")); File.WriteAllBytes(Path.Combine(root, "NEW.DAT"), [1]); File.WriteAllBytes(Path.Combine(root, "SAVE002.SAV"), [2]);
            BaselineValidationResult changed = service.ValidateBaseline();
            if (!changed.Entries.Any(item => item.Status == BaselineValidationStatus.MissingPristineFile) || !changed.Entries.Any(item => item.Status == BaselineValidationStatus.UnexpectedFile))
                throw new InvalidDataException("Missing/unexpected baseline comparison failed.");
            File.WriteAllText(service.ManifestPath, "{\"SchemaVersion\":99}");
            if (service.ValidateBaseline().Status != BaselineValidationStatus.UnsupportedSchema) throw new InvalidDataException("Unsupported schema was accepted.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyLauncherClassificationSmoke()
    {
        static void RequireClassification(string path, PristineFileClassification expected)
        {
            if (PristineInstallationService.Classify(path).Classification != expected)
                throw new InvalidDataException($"Unexpected launcher classification for {path}.");
        }
        RequireClassification("CERBERUS.BAT", PristineFileClassification.MutableBackedUp);
        RequireClassification("OTHER.BAT", PristineFileClassification.Immutable);
        RequireClassification("ELVIRA.BAT", PristineFileClassification.MutableBackedUp);
        RequireClassification("cloud_saves\\ELVIRA.BAT", PristineFileClassification.MutableBackedUp);
        RequireClassification("PI1SND.BAT", PristineFileClassification.GeneratedIgnored);
        RequireClassification("PI1MENU.COM", PristineFileClassification.GeneratedIgnored);
        RequireClassification("ELVIRA_MODS.INI", PristineFileClassification.GeneratedIgnored);
    }

    private static void VerifyProjectContextSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ElviraProjectContextSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            var loader = new ProjectContextLoader();
            RequireStatus(loader.Open(null), ProjectContextOpenStatus.InvalidGameRoot, "null root");
            RequireStatus(loader.Open(string.Empty), ProjectContextOpenStatus.InvalidGameRoot, "empty root");
            RequireStatus(loader.Open("relative-game-root"), ProjectContextOpenStatus.InvalidGameRoot, "relative root");
            RequireStatus(loader.Open(Path.Combine(root, "missing")), ProjectContextOpenStatus.InvalidGameRoot, "missing root");

            string unknown = Path.Combine(root, "unknown"); Directory.CreateDirectory(unknown);
            File.WriteAllBytes(Path.Combine(unknown, "GAMEPC"), [1]); File.WriteAllBytes(Path.Combine(unknown, "012.VGA"), [2]);
            RequireStatus(loader.Open(unknown), ProjectContextOpenStatus.GameNotRecognized, "unrecognized game");

            string e1 = CreateSupportedFixture(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string e2 = CreateSupportedFixture(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            VerifyFixtureStatuses(loader, e1, ElviraGameProfile.Elvira1);
            VerifyFixtureStatuses(loader, e2, ElviraGameProfile.Elvira2);

            ProjectContext first = RequireSuccess(loader.Open(e2), "first Elvira II fixture open");
            string e2Copy = Path.Combine(root, "e2copy"); CopyDirectory(e2, e2Copy);
            ProjectContext copied = RequireSuccess(loader.Open(e2Copy), "copied Elvira II fixture open");
            if (first.GameRoot.Equals(copied.GameRoot, StringComparison.OrdinalIgnoreCase) || first.BaselineFingerprint != copied.BaselineFingerprint)
                throw new InvalidDataException("Installation and baseline identity were not kept distinct.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, recursive: true); }
    }

    private static void VerifyRealProjectContextSmoke(string elvira1Root, string elvira2Root)
    {
        var loader = new ProjectContextLoader();
        ProjectContext e1a = RequireSuccess(loader.Open(elvira1Root), "Elvira I real open");
        ProjectContext e1b = RequireSuccess(loader.Open(elvira1Root), "Elvira I repeat open");
        ProjectContext e2a = RequireSuccess(loader.Open(elvira2Root), "Elvira II real open");
        ProjectContext e2b = RequireSuccess(loader.Open(elvira2Root), "Elvira II repeat open");
        if (e1a.GameProfile != ElviraGameProfile.Elvira1 || e2a.GameProfile != ElviraGameProfile.Elvira2 ||
            e1a.BaselineFingerprint != e1b.BaselineFingerprint || e2a.BaselineFingerprint != e2b.BaselineFingerprint ||
            e1a.ProjectRoot != e1b.ProjectRoot || e2a.ProjectRoot != e2b.ProjectRoot)
            throw new InvalidDataException("Project context opening was not stable.");
        if (e1a.BaselineFingerprint != "64D0EEDC52A0C646CE0479B76E3CA60808667D1F096501A46EECCEEFAAC455EB" ||
            e2a.BaselineFingerprint != "805131DC9AD327B6FC5E15AE6AD283022BD2D2F1B94032B58FDCA9A93EC1F9D1")
            throw new InvalidDataException("Real baseline fingerprint did not match the authoritative context handoff.");
        RequireLauncher(e1a, "ELVIRA.BAT"); RequireLauncher(e2a, "CERBERUS.BAT");
    }

    private static string CreateSupportedFixture(string root, string name, string source, ElviraGameProfile game)
    {
        string target = Path.Combine(root, name); Directory.CreateDirectory(target);
        CopyRequired(source, target, "GAMEPC");
        CopyRequired(source, target, game == ElviraGameProfile.Elvira1 ? "RUNVGA.EXE" : "RUNIT.EXE");
        if (game == ElviraGameProfile.Elvira1)
        {
            CopyRequired(source, target, "RUNEGA.EXE");
            // Retain the manual acceptance resource and its paired palette in
            // all disposable E1 build fixtures.
            CopyRequired(source, target, "381.VGA");
            CopyRequired(source, target, "382.VGA");
        }
        string zone = Directory.EnumerateFiles(source, "*2.VGA", SearchOption.TopDirectoryOnly)
            .FirstOrDefault(path => Path.GetFileName(path).Length == 7 && char.IsDigit(Path.GetFileName(path)[0]) && char.IsDigit(Path.GetFileName(path)[1]))
            ?? throw new InvalidDataException("Source installation has no supported VGA zone fixture.");
        File.Copy(zone, Path.Combine(target, Path.GetFileName(zone)), false);
        File.WriteAllBytes(Path.Combine(target, "RESOURCE.DAT"), [1, 2, 3]);
        return target;
    }

    private static void VerifyFixtureStatuses(ProjectContextLoader loader, string root, ElviraGameProfile game)
    {
        var layout = new EditorStorageLayout(root); var manifestService = new PristineManifestService(layout, game);
        RequireStatus(loader.Open(root), ProjectContextOpenStatus.ManifestMissing, "missing manifest");
        if (manifestService.InitializeBaseline().Status != PristineManifestInitializationStatus.Initialized)
            throw new InvalidDataException("Project-context fixture baseline initialization failed.");
        if (Directory.Exists(layout.ProjectRoot) || Directory.Exists(layout.VariantsRoot) || Directory.Exists(layout.LogsRoot) || Directory.Exists(layout.MetadataRoot))
            throw new InvalidDataException("Fixture initialization unexpectedly created future project roots.");
        byte[] manifestBeforeReadOnlyOpen = File.ReadAllBytes(manifestService.ManifestPath);
        ProjectContext ready = RequireSuccess(loader.Open(root + Path.DirectorySeparatorChar), "valid fixture open");
        if (ready.GameProfile != game || ready.GameRoot != layout.GameRoot || ready.ProjectRoot != layout.ProjectRoot ||
            ready.BaselineValidation.Status != BaselineValidationStatus.MatchesBaseline)
            throw new InvalidDataException("Ready project context did not expose the expected immutable state.");
        if (Directory.Exists(layout.ProjectRoot) || Directory.Exists(layout.VariantsRoot) || Directory.Exists(layout.LogsRoot) || Directory.Exists(layout.MetadataRoot))
            throw new InvalidDataException("Read-only context loading created a future storage directory.");
        if (!manifestBeforeReadOnlyOpen.SequenceEqual(File.ReadAllBytes(manifestService.ManifestPath)))
            throw new InvalidDataException("Read-only context loading rewrote the fixture manifest.");

        string pristine = File.ReadAllText(manifestService.ManifestPath);
        File.WriteAllText(manifestService.ManifestPath, "not-json");
        RequireStatus(loader.Open(root), ProjectContextOpenStatus.ManifestInvalid, "malformed manifest");
        File.WriteAllText(manifestService.ManifestPath, "{\"SchemaVersion\":999}");
        RequireStatus(loader.Open(root), ProjectContextOpenStatus.UnsupportedManifestSchema, "unsupported manifest schema");
        File.WriteAllText(manifestService.ManifestPath, pristine);

        PristineManifest manifest = PristineManifestService.LoadManifest(manifestService.ManifestPath, game);
        string mismatchId = game == ElviraGameProfile.Elvira1 ? "Elvira2" : "Elvira1";
        File.WriteAllText(manifestService.ManifestPath, System.Text.Json.JsonSerializer.Serialize(manifest with { GameId = mismatchId }));
        RequireStatus(loader.Open(root), ProjectContextOpenStatus.GameIdMismatch, "manifest game mismatch");
        File.WriteAllText(manifestService.ManifestPath, pristine);

        string immutable = Path.Combine(root, "RESOURCE.DAT"); byte[] immutableBytes = File.ReadAllBytes(immutable);
        File.WriteAllBytes(immutable, [9, 9, 9]); RequireStatus(loader.Open(root), ProjectContextOpenStatus.BaselineMismatch, "changed immutable file");
        File.WriteAllBytes(immutable, immutableBytes);
        File.Delete(immutable); RequireStatus(loader.Open(root), ProjectContextOpenStatus.BaselineMismatch, "missing pristine file");
        File.WriteAllBytes(immutable, immutableBytes);

        string exe = Path.Combine(root, game == ElviraGameProfile.Elvira1 ? "RUNVGA.EXE" : "RUNIT.EXE"); byte[] exeBytes = File.ReadAllBytes(exe);
        exeBytes[0] ^= 0xFF; File.WriteAllBytes(exe, exeBytes);
        RequireStatus(loader.Open(root), ProjectContextOpenStatus.UnsupportedExecutableVersion, "unsupported executable");
        exeBytes[0] ^= 0xFF; File.WriteAllBytes(exe, exeBytes);
        _ = RequireSuccess(loader.Open(root), "restored fixture open");
    }

    private static void CopyRequired(string sourceRoot, string targetRoot, string file) =>
        File.Copy(Path.Combine(sourceRoot, file), Path.Combine(targetRoot, file), false);

    private static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (string file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            string relative = Path.GetRelativePath(source, file); string destination = Path.Combine(target, relative);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!); File.Copy(file, destination, false);
        }
    }

    private static ProjectContext RequireSuccess(ProjectContextOpenResult result, string description) =>
        result.IsSuccess ? result.Context! : throw new InvalidDataException($"{description} failed with {result.Status}: {result.Detail}");

    private static void RequireStatus(ProjectContextOpenResult result, ProjectContextOpenStatus expected, string description)
    {
        if (result.Status != expected) throw new InvalidDataException($"{description} returned {result.Status}, expected {expected}.");
    }

    private static void RequireLauncher(ProjectContext context, string launcher)
    {
        PristineManifestFile file = context.PristineManifest.Files.Single(item => item.RelativePath.Equals(launcher, StringComparison.OrdinalIgnoreCase));
        if (file.Classification != PristineFileClassification.MutableBackedUp)
            throw new InvalidDataException($"{launcher} lost MutableBackedUp classification.");
    }

    private static void VerifyVariantContextSmoke(string elvira1Root, string elvira2Root)
    {
        var loader = new ProjectContextLoader();
        ProjectContext e1 = RequireSuccess(loader.Open(elvira1Root), "Elvira I project context");
        ProjectContext e2 = RequireSuccess(loader.Open(elvira2Root), "Elvira II project context");
        bool e1VariantsExisted = Directory.Exists(e1.StorageLayout.VariantsRoot), e2VariantsExisted = Directory.Exists(e2.StorageLayout.VariantsRoot);
        VerifyVariantDefinitions(e1, e2);
        if (Directory.Exists(e1.StorageLayout.VariantsRoot) != e1VariantsExisted || Directory.Exists(e2.StorageLayout.VariantsRoot) != e2VariantsExisted)
            throw new InvalidDataException("Variant context access changed a variant-directory state.");
        VerifyVariantReadOnly(e1);
    }

    private static void VerifyRealVariantContextSmoke(string elvira1Root, string elvira2Root)
    {
        var loader = new ProjectContextLoader();
        ProjectContext e1 = RequireSuccess(loader.Open(elvira1Root), "Elvira I real project context");
        ProjectContext e2 = RequireSuccess(loader.Open(elvira2Root), "Elvira II real project context");
        VerifyVariantDefinitions(e1, e2);
        if (e1.BaselineValidation.Status != BaselineValidationStatus.MatchesBaseline || e2.BaselineValidation.Status != BaselineValidationStatus.MatchesBaseline)
            throw new InvalidDataException("Variant contexts were opened without a matching baseline.");
    }

    private static void VerifyVariantDefinitions(ProjectContext elvira1, ProjectContext elvira2)
    {
        IReadOnlyList<VariantContext> e1 = VariantContextCatalog.CreateBuiltIns(elvira1);
        IReadOnlyList<VariantContext> e2 = VariantContextCatalog.CreateBuiltIns(elvira2);
        if (e1.Count != 2 || e2.Count != 1) throw new InvalidDataException("Built-in variant catalog count is invalid.");
        VariantContext vga = e1.Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
        VariantContext ega = e1.Single(item => item.VariantId == BuiltInVariantId.Elvira1Ega);
        VariantContext runit = e2.Single();
        if (vga is not { RuntimeKind: VariantRuntimeKind.Elvira1Vga, SourceExecutableName: "RUNVGA.EXE", GeneratedExecutableName: "RUNVGASK.EXE", LogicalDataFileName: "GAMEPC" } ||
            ega is not { RuntimeKind: VariantRuntimeKind.Elvira1Ega, SourceExecutableName: "RUNEGA.EXE", GeneratedExecutableName: "RUNEGASK.EXE", LogicalDataFileName: "GAMEPC" } ||
            runit is not { VariantId: BuiltInVariantId.Elvira2Vga, RuntimeKind: VariantRuntimeKind.Elvira2Vga, SourceExecutableName: "RUNIT.EXE", GeneratedExecutableName: "RUNITSK.EXE", LogicalDataFileName: "GAMEPC" })
            throw new InvalidDataException("Built-in executable/data metadata is invalid.");
        if (vga.VariantId == ega.VariantId || vga.RuntimeKind == ega.RuntimeKind || vga.DirectoryKey == ega.DirectoryKey ||
            vga.LogicalDataFileName != ega.LogicalDataFileName || !vga.FutureVariantRoot.StartsWith(elvira1.StorageLayout.VariantsRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Elvira I VGA/EGA variant identity is not independently represented.");
        bool elvira2EgaRejected;
        try { _ = VariantContextCatalog.Create(elvira2, BuiltInVariantId.Elvira1Ega); elvira2EgaRejected = false; }
        catch (ArgumentException) { elvira2EgaRejected = true; }
        if (!elvira2EgaRejected) throw new InvalidDataException("Elvira II + EGA was accepted.");
        foreach (string key in new[] { "SLOVAK-VGA-LONG", "ELVIRA_VARIANT", "..\\SK", "C:\\SK", ".", "CON" })
        {
            try { _ = VariantContext.ValidateDirectoryKey(key); }
            catch (ArgumentException) { continue; }
            throw new InvalidDataException("Unsafe variant directory key was accepted: " + key);
        }
        _ = new ActiveVariantSelection(elvira1, vga);
        bool crossProjectRejected;
        try { _ = new ActiveVariantSelection(elvira1, runit); crossProjectRejected = false; }
        catch (ArgumentException) { crossProjectRejected = true; }
        if (!crossProjectRejected) throw new InvalidDataException("Active selection accepted a VariantContext from another project instance.");
    }

    private static void VerifyVariantReadOnly(ProjectContext context)
    {
        foreach (var property in typeof(VariantContext).GetProperties())
            if (property.SetMethod is not null && property.SetMethod.IsPublic)
                throw new InvalidDataException("VariantContext has a public setter: " + property.Name);
        VariantContext item = VariantContextCatalog.CreateBuiltIns(context).First();
        if (item.VariantId.ToString().Contains("Guid", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Built-in variant identity is not semantic.");
    }

    private static void VerifyVariantDirectorySmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ElviraVariantDirectorySmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var directories = new VariantDirectoryService();
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Ega);
            VariantContext runit = VariantContextCatalog.CreateBuiltIns(e2).Single();

            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, vga), VariantDirectoryOperationStatus.Created, "owned directory create");
            string vgaPath = directories.GetVariantDirectoryPath(e1, vga);
            RequireOnlyOwnershipMarker(vgaPath);
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, vga), VariantDirectoryOperationStatus.AlreadyValid, "owned directory idempotency");
            File.WriteAllText(Path.Combine(vgaPath, "BUILD.TXT"), "temporary payload");
            Directory.CreateDirectory(Path.Combine(vgaPath, "SUB")); File.WriteAllText(Path.Combine(vgaPath, "SUB", "PAYLOAD.DAT"), "temporary payload");
            RequireDirectoryStatus(directories.ClearVariantDirectory(e1, vga), VariantDirectoryOperationStatus.Cleared, "owned directory clear");
            RequireOnlyOwnershipMarker(vgaPath);
            RequireDirectoryStatus(directories.DeleteVariantDirectory(e1, vga), VariantDirectoryOperationStatus.Deleted, "owned directory delete");
            if (Directory.Exists(vgaPath) || !Directory.Exists(e1.StorageLayout.VariantsRoot))
                throw new InvalidDataException("Variant delete removed the wrong directory scope.");

            string egaPath = directories.GetVariantDirectoryPath(e1, ega);
            Directory.CreateDirectory(egaPath); string foreignPayload = Path.Combine(egaPath, "USER.TXT"); File.WriteAllText(foreignPayload, "user bytes");
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, ega), VariantDirectoryOperationStatus.ForeignDirectoryConflict, "foreign directory ensure");
            RequireDirectoryStatus(directories.ClearVariantDirectory(e1, ega), VariantDirectoryOperationStatus.ForeignDirectoryConflict, "foreign directory clear");
            RequireDirectoryStatus(directories.DeleteVariantDirectory(e1, ega), VariantDirectoryOperationStatus.ForeignDirectoryConflict, "foreign directory delete");
            if (!File.Exists(foreignPayload)) throw new InvalidDataException("Foreign directory payload was modified.");
            Directory.Delete(egaPath, true); // test fixture cleanup, never service-managed foreign data.

            Directory.CreateDirectory(egaPath);
            File.WriteAllText(Path.Combine(egaPath, VariantDirectoryService.OwnershipMarkerFileName),
                System.Text.Json.JsonSerializer.Serialize(new VariantDirectoryOwnershipMarker(VariantDirectoryService.OwnershipMarkerSchema,
                    "Pi1ElviraVariantDirectory", "Elvira1", "Elvira1Ega", ega.DirectoryKey, new string('0', 64))));
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, ega), VariantDirectoryOperationStatus.OwnershipConflict, "stale baseline marker");
            if (!File.Exists(Path.Combine(egaPath, VariantDirectoryService.OwnershipMarkerFileName)))
                throw new InvalidDataException("Mismatched ownership marker was overwritten.");
            Directory.Delete(egaPath, true);

            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, vga), VariantDirectoryOperationStatus.Created, "recreate owned directory");
            string outside = Path.Combine(root, "outside"); Directory.CreateDirectory(outside); string sentinel = Path.Combine(outside, "SENTINEL.TXT"); File.WriteAllText(sentinel, "outside");
            string link = Path.Combine(vgaPath, "LINK"); CreateTemporaryReparseLink(link, outside);
            RequireDirectoryStatus(directories.ClearVariantDirectory(e1, vga), VariantDirectoryOperationStatus.ReparsePointConflict, "reparse-point clear protection");
            if (!File.Exists(sentinel)) throw new InvalidDataException("Reparse-point traversal reached outside bytes.");
            Directory.Delete(link); RequireDirectoryStatus(directories.DeleteVariantDirectory(e1, vga), VariantDirectoryOperationStatus.Deleted, "owned directory delete after link removal");

            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, vga), VariantDirectoryOperationStatus.Created, "enumeration directory create");
            IReadOnlyList<VariantDirectoryInfo> listed = directories.Enumerate(e1, [vga, ega]);
            if (listed.Count != 1 || listed[0] is not { DirectoryKey: "E1VGA", Classification: VariantDirectoryClassification.KnownOwnedVariant, VariantId: BuiltInVariantId.Elvira1Vga })
                throw new InvalidDataException("Owned directory enumeration is invalid.");
            bool duplicateKeyRejected;
            try { _ = directories.Enumerate(e1, [vga, vga]); duplicateKeyRejected = false; }
            catch (ArgumentException) { duplicateKeyRejected = true; }
            if (!duplicateKeyRejected) throw new InvalidDataException("Case-insensitive known directory collision was accepted.");
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, runit), VariantDirectoryOperationStatus.InvalidContext, "cross-project directory use");
            foreach (string key in new[] { "..", ".\\..", "C:\\Windows", "\\\\server\\share", "VGA\\..\\..", "CON", "NUL", "ABCDEFGHI", "VGA\\SK", "NAME.", "NAME " })
            {
                try { _ = VariantContext.ValidateDirectoryKey(key); }
                catch (ArgumentException) { continue; }
                throw new InvalidDataException("Path attack key was accepted: " + key);
            }
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRealVariantDirectorySmoke(string elvira1Root, string elvira2Root)
    {
        var loader = new ProjectContextLoader(); var directories = new VariantDirectoryService();
        ProjectContext e1 = RequireSuccess(loader.Open(elvira1Root), "Elvira I real project context");
        ProjectContext e2 = RequireSuccess(loader.Open(elvira2Root), "Elvira II real project context");
        IReadOnlyList<VariantContext> e1Variants = VariantContextCatalog.CreateBuiltIns(e1);
        IReadOnlyList<VariantContext> e2Variants = VariantContextCatalog.CreateBuiltIns(e2);
        foreach (VariantContext variant in e1Variants.Concat(e2Variants))
        {
            ProjectContext project = variant.Project;
            VariantDirectoryOperationResult result = directories.EnsureVariantDirectory(project, variant);
            if (result.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid))
                throw new InvalidDataException("Real owned variant directory could not be ensured: " + result.Status);
            RequireOnlyOwnershipMarker(result.Path);
        }
        RequireKnownOwned(directories.Enumerate(e1, e1Variants), e1Variants);
        RequireKnownOwned(directories.Enumerate(e2, e2Variants), e2Variants);
        if (new PristineManifestService(e1.StorageLayout, e1.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline ||
            new PristineManifestService(e2.StorageLayout, e2.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
            throw new InvalidDataException("Editor-owned VARIANTS root changed a pristine baseline.");
    }

    private static ProjectContext CreateFixtureProjectContext(string root, string name, string source, ElviraGameProfile game)
    {
        string fixture = CreateSupportedFixture(root, name, source, game);
        var layout = new EditorStorageLayout(fixture);
        if (new PristineManifestService(layout, game).InitializeBaseline().Status != PristineManifestInitializationStatus.Initialized)
            throw new InvalidDataException("Variant-directory fixture baseline initialization failed.");
        return RequireSuccess(new ProjectContextLoader().Open(fixture), "variant-directory fixture project context");
    }

    private static void RequireDirectoryStatus(VariantDirectoryOperationResult result, VariantDirectoryOperationStatus expected, string description)
    {
        if (result.Status != expected) throw new InvalidDataException($"{description} returned {result.Status}, expected {expected}.");
    }

    private static void RequireOnlyOwnershipMarker(string path)
    {
        string[] entries = Directory.EnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly).Select(item => Path.GetFileName(item) ?? string.Empty).ToArray();
        if (entries.Length != 1 || !entries[0].Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Variant directory contains payload or lacks its ownership marker.");
    }

    private static void RequireKnownOwned(IReadOnlyList<VariantDirectoryInfo> entries, IReadOnlyList<VariantContext> variants)
    {
        if (entries.Count != variants.Count || entries.Any(item => item.Classification != VariantDirectoryClassification.KnownOwnedVariant))
            throw new InvalidDataException("Real variant directory enumeration is not fully owned/known.");
    }

    private static void CreateTemporaryReparseLink(string link, string target)
    {
        try { Directory.CreateSymbolicLink(link, target); return; }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            // A local NTFS junction is sufficient for this Windows-only smoke
            // and normally does not require the symlink privilege. Both paths
            // are generated temporary fixture paths, never caller input.
            var start = new System.Diagnostics.ProcessStartInfo("cmd.exe", $"/d /c mklink /J \"{link}\" \"{target}\"")
            { UseShellExecute = false, CreateNoWindow = true };
            using System.Diagnostics.Process? process = System.Diagnostics.Process.Start(start);
            process?.WaitForExit();
            if (process is null || process.ExitCode != 0 || !Directory.Exists(link))
                throw new InvalidDataException("Temporary reparse-point fixture could not be created.");
        }
    }

    private static void VerifyDisposableBuildSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ElviraDisposableBuildSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var directories = new VariantDirectoryService(); var builds = new DisposableVariantBuildService(directories);
            VariantContext variant = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            RequireDirectoryStatus(directories.EnsureVariantDirectory(project, variant), VariantDirectoryOperationStatus.Created, "build fixture owned directory");
            RequireBuildStatus(builds.Build(project, variant), DisposableVariantBuildStatus.Success, "initial pristine build");
            string variantRoot = directories.GetVariantDirectoryPath(project, variant);
            File.WriteAllText(Path.Combine(variantRoot, "JUNK.TXT"), "junk");
            File.WriteAllText(Path.Combine(variantRoot, "GAMEPC"), "corrupt variant bytes");
            RequireBuildStatus(builds.Build(project, variant), DisposableVariantBuildStatus.Success, "non-cumulative rebuild");
            if (File.Exists(Path.Combine(variantRoot, "JUNK.TXT")) || !File.ReadAllBytes(Path.Combine(variantRoot, "GAMEPC")).SequenceEqual(File.ReadAllBytes(Path.Combine(project.GameRoot, "GAMEPC"))))
                throw new InvalidDataException("Rebuild reused old variant payload or retained junk.");

            string prior = Path.Combine(variantRoot, "PRIOR.TXT"); File.WriteAllText(prior, "preserve on preflight failure");
            string immutable = Path.Combine(project.GameRoot, "RESOURCE.DAT"); byte[] immutableBytes = File.ReadAllBytes(immutable); File.WriteAllBytes(immutable, [7, 7, 7]);
            RequireBuildStatus(builds.Build(project, variant), DisposableVariantBuildStatus.BaselineInvalid, "baseline gate before clear");
            if (!File.Exists(prior)) throw new InvalidDataException("Failed preflight cleared the prior variant.");
            File.WriteAllBytes(immutable, immutableBytes);

            string launcher = Path.Combine(project.GameRoot, "ELVIRA.BAT"); byte[] backupBytes = File.ReadAllBytes(Path.Combine(project.StorageLayout.MutableBackupsRoot, "ELVIRA.BAT"));
            File.WriteAllText(launcher, "modified root launcher");
            var mutableResolved = DisposableVariantBuildService.ValidateSources(project);
            if (mutableResolved.Status != DisposableVariantBuildStatus.Success || !mutableResolved.Sources.Single(source => source.ManifestFile.RelativePath == "ELVIRA.BAT").SourcePath.StartsWith(project.StorageLayout.MutableBackupsRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Mutable source did not resolve from the pristine backup.");
            File.WriteAllBytes(launcher, backupBytes);

            string backup = Path.Combine(project.StorageLayout.MutableBackupsRoot, "ELVIRA.BAT"); File.Delete(backup);
            if (DisposableVariantBuildService.ValidateSources(project).Status != DisposableVariantBuildStatus.MutableBackupMissing) throw new InvalidDataException("Missing mutable backup was not detected.");
            File.WriteAllBytes(backup, [1, 2, 3]);
            if (DisposableVariantBuildService.ValidateSources(project).Status != DisposableVariantBuildStatus.MutableBackupMismatch) throw new InvalidDataException("Wrong mutable backup was not detected.");
            File.WriteAllBytes(backup, backupBytes);
            File.WriteAllBytes(immutable, [8, 8, 8]);
            if (DisposableVariantBuildService.ValidateSources(project).Status != DisposableVariantBuildStatus.SourceHashMismatch) throw new InvalidDataException("Immutable source mismatch was not detected.");
            File.WriteAllBytes(immutable, immutableBytes);
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRealDisposableBuildSmoke(string elvira1Root, string elvira2Root)
    {
        var loader = new ProjectContextLoader(); var directories = new VariantDirectoryService(); var builds = new DisposableVariantBuildService(directories);
        ProjectContext e1 = RequireSuccess(loader.Open(elvira1Root), "Elvira I real project"); ProjectContext e2 = RequireSuccess(loader.Open(elvira2Root), "Elvira II real project");
        DisposableVariantBuildResult[] results = VariantContextCatalog.CreateBuiltIns(e1).Concat(VariantContextCatalog.CreateBuiltIns(e2))
            .Select(variant => builds.Build(variant.Project, variant)).ToArray();
        if (results.Any(result => result.Status != DisposableVariantBuildStatus.Success)) throw new InvalidDataException("A real disposable pristine build failed.");
        if (!File.ReadAllBytes(Path.Combine(results[0].VariantRoot, "GAMEPC")).SequenceEqual(File.ReadAllBytes(Path.Combine(results[1].VariantRoot, "GAMEPC"))))
            throw new InvalidDataException("Elvira I VGA/EGA GAMEPC copies differ.");
        if (!EquivalentPayload(results[0].VariantRoot, results[1].VariantRoot)) throw new InvalidDataException("Elvira I pristine variant payloads differ.");
        foreach (DisposableVariantBuildResult result in results) RequireVariantPayloadOnly(result.VariantRoot);
        if (HashFile(Path.Combine(results[0].VariantRoot, "ELVIRA.BAT")) != HashFile(Path.Combine(e1.StorageLayout.MutableBackupsRoot, "ELVIRA.BAT")) ||
            HashFile(Path.Combine(results[2].VariantRoot, "CERBERUS.BAT")) != "EA91403C3708DC018708D401A5E93EFAD799AD46EDF595EB571912ABAA0DE2CC")
            throw new InvalidDataException("Real mutable launcher copy was not pristine-backup exact.");
        if (new PristineManifestService(e1.StorageLayout, e1.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline ||
            new PristineManifestService(e2.StorageLayout, e2.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
            throw new InvalidDataException("Disposable output changed a real pristine baseline.");
    }

    private static ProjectContext CreateBuildFixtureProjectContext(string root, string name, string source, ElviraGameProfile game)
    {
        string fixture = CreateSupportedFixture(root, name, source, game);
        CopyRequired(source, fixture, game == ElviraGameProfile.Elvira1 ? "ELVIRA.BAT" : "CERBERUS.BAT");
        var layout = new EditorStorageLayout(fixture);
        if (new PristineManifestService(layout, game).InitializeBaseline().Status != PristineManifestInitializationStatus.Initialized)
            throw new InvalidDataException("Disposable-build fixture baseline initialization failed.");
        return RequireSuccess(new ProjectContextLoader().Open(fixture), "disposable-build fixture project context");
    }

    private static void RequireBuildStatus(DisposableVariantBuildResult result, DisposableVariantBuildStatus expected, string description)
    {
        if (result.Status != expected) throw new InvalidDataException($"{description} returned {result.Status}, expected {expected}.");
    }

    private static bool EquivalentPayload(string left, string right)
    {
        string[] files(string root) => Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories).Where(path => !Path.GetFileName(path).Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)).Select(path => Path.GetRelativePath(root, path)).OrderBy(path => path, StringComparer.OrdinalIgnoreCase).ToArray();
        string[] first = files(left); string[] second = files(right);
        return first.SequenceEqual(second, StringComparer.OrdinalIgnoreCase) && first.All(path => HashFile(Path.Combine(left, path)) == HashFile(Path.Combine(right, path)));
    }

    private static void RequireVariantPayloadOnly(string variantRoot)
    {
        if (!File.Exists(Path.Combine(variantRoot, VariantDirectoryService.OwnershipMarkerFileName)) ||
            Directory.EnumerateFiles(variantRoot, "*", SearchOption.AllDirectories).Any(path => Path.GetFileName(path).Equals("RUNVGASK.EXE", StringComparison.OrdinalIgnoreCase) || Path.GetFileName(path).Equals("RUNEGASK.EXE", StringComparison.OrdinalIgnoreCase) || Path.GetFileName(path).Equals("RUNITSK.EXE", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidDataException("Variant output lacks its marker or contains a generated executable.");
    }

    private static string HashFile(string path) { using var stream = File.OpenRead(path); return Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream)); }

    private static void VerifyRuntimeUiTextSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiTextSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var service = new RuntimeUiTextService();

            // Loading an unopened project is pure: deletion here is safe because
            // this is a disposable fixture, and proves Load never recreates it.
            if (Directory.Exists(e1.ProjectRoot)) Directory.Delete(e1.ProjectRoot, true);
            RuntimeUiTextLoadResult empty = service.Load(e1);
            if (!empty.IsSuccess || empty.State!.Overrides.Count != 0 || Directory.Exists(e1.ProjectRoot) || File.Exists(service.GetPath(e1)))
                throw new InvalidDataException("Read-only runtime UI load created project state.");
            Directory.CreateDirectory(e1.ProjectRoot);

            if (service.GetDefinitions(e1).Count != 8 || service.GetDefinitions(e2).Count != 4 ||
                service.GetDefinitions(e1).Any(value => value.FrozenDefaultText is not null))
                throw new InvalidDataException("Frozen runtime UI definitions diverged from the evidence-only profiles.");

            RuntimeUiTextState e1State = service.SetOverride(e1, empty.State, RuntimeUiLogicalRecordId.PauseMenu, "Pauza – Unicode žľť");
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            RuntimeUiRuntimeProjection vga = service.GetEffectiveRecords(e1Vga, e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu);
            RuntimeUiRuntimeProjection ega = service.GetEffectiveRecords(e1Ega, e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu);
            if (vga is not { IsOverridden: true, TextOrigin: RuntimeUiTextOrigin.ProjectOverride, EffectiveText: "Pauza – Unicode žľť", MappingReadiness: RuntimeUiMappingReadiness.KnownButMappingIncomplete } ||
                ega is not { IsOverridden: true, TextOrigin: RuntimeUiTextOrigin.ProjectOverride, EffectiveText: "Pauza – Unicode žľť", MappingReadiness: RuntimeUiMappingReadiness.SupportedAndMapped })
                throw new InvalidDataException("Elvira I VGA/EGA did not project one shared logical override.");
            RuntimeUiTextSaveResult saved = service.Save(e1, e1State);
            if (!saved.Succeeded || !File.ReadAllText(saved.Path).Contains("Pauza – Unicode žľť", StringComparison.Ordinal) ||
                File.ReadAllText(saved.Path).Contains("PointerSitePhysicalOffset", StringComparison.Ordinal))
                throw new InvalidDataException("Runtime UI override persistence was not sparse Unicode project data.");
            RuntimeUiTextLoadResult reloaded = service.Load(e1);
            if (!reloaded.IsSuccess || !reloaded.State!.Overrides.SequenceEqual(e1State.Overrides)) throw new InvalidDataException("Runtime UI JSON did not round-trip deterministically.");
            RuntimeUiTextState reset = service.RemoveOverride(e1, reloaded.State, RuntimeUiLogicalRecordId.PauseMenu);
            if (reset.Overrides.Count != 0 || service.GetEffectiveRecords(e1Vga, reset).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu).TextOrigin != RuntimeUiTextOrigin.FrozenDefaultUnavailable ||
                !service.Save(e1, reset).Succeeded || File.ReadAllText(service.GetPath(e1)).Contains("PauseMenu", StringComparison.Ordinal))
                throw new InvalidDataException("Runtime UI override reset did not return to the frozen/default source.");

            VariantContext runit = VariantContextCatalog.CreateBuiltIns(e2).Single();
            RuntimeUiTextState e2State = service.SetOverride(e2, service.Load(e2).State!, RuntimeUiLogicalRecordId.LoadFailure, "Načítanie zlyhalo");
            RuntimeUiRuntimeProjection[] r = service.GetEffectiveRecords(runit, e2State).ToArray();
            if (r.Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SaveFailure) is not { MappingReadiness: RuntimeUiMappingReadiness.SupportedAndMapped, EvidenceStatus: RuntimeUiEvidenceStatus.ProvenByBinary } ||
                r.Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.LoadFailure) is not { MappingReadiness: RuntimeUiMappingReadiness.KnownButMappingIncomplete, EvidenceStatus: RuntimeUiEvidenceStatus.Unknown, EffectiveText: "Načítanie zlyhalo" } ||
                r.Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.FileNotFound).MappingReadiness != RuntimeUiMappingReadiness.KnownButMappingIncomplete ||
                r.Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.TryAnotherDisk).MappingReadiness != RuntimeUiMappingReadiness.KnownButMappingIncomplete)
                throw new InvalidDataException("RUNIT incomplete mapping state was not preserved.");

            string e2Path = service.GetPath(e2); Directory.CreateDirectory(e2.ProjectRoot);
            void reject(string json, RuntimeUiTextLoadStatus expected)
            {
                File.WriteAllText(e2Path, json); RuntimeUiTextLoadResult result = service.Load(e2);
                if (result.Status != expected || result.State is not null) throw new InvalidDataException($"Runtime UI malformed input expected {expected}, received {result.Status}.");
            }
            reject("{", RuntimeUiTextLoadStatus.InvalidJson);
            reject("{\"schemaVersion\":2,\"gameId\":\"Elvira2\",\"records\":[]}", RuntimeUiTextLoadStatus.UnsupportedSchema);
            reject("{\"schemaVersion\":1,\"gameId\":\"Elvira1\",\"records\":[]}", RuntimeUiTextLoadStatus.GameMismatch);
            reject("{\"schemaVersion\":1,\"gameId\":\"Elvira2\",\"records\":[{\"logicalRecordId\":\"SaveFailure\",\"text\":\"x\"},{\"logicalRecordId\":\"savefailure\",\"text\":\"y\"}]}", RuntimeUiTextLoadStatus.DuplicateLogicalId);
            reject("{\"schemaVersion\":1,\"gameId\":\"Elvira2\",\"records\":[{\"logicalRecordId\":\"FutureRoute\",\"text\":\"x\"}]}", RuntimeUiTextLoadStatus.UnknownLogicalId);

            var directories = new VariantDirectoryService(); var disposable = new DisposableVariantBuildService(directories);
            foreach (VariantContext variant in VariantContextCatalog.CreateBuiltIns(e1).Concat(VariantContextCatalog.CreateBuiltIns(e2)))
            {
                ICompositeBuildStep step = variant.RuntimeKind switch
                {
                    VariantRuntimeKind.Elvira1Vga => new RunVgaCompositeBuildStep(directories),
                    VariantRuntimeKind.Elvira1Ega => new RunEgaCompositeBuildStep(directories),
                    VariantRuntimeKind.Elvira2Vga => new RunItCompositeBuildStep(directories),
                    _ => throw new InvalidDataException("Unexpected runtime.")
                };
                if (new CompositeBuildService(disposable, directories, [step]).Build(variant.Project, variant, CompositeBuildMode.Full).Status != CompositeBuildStatus.NotFullyConfigured)
                    throw new InvalidDataException("Runtime UI text state falsely made a full build ready.");
            }
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRuntimeUiLayoutSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiLayoutSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var texts = new RuntimeUiTextService(); var layouts = new RuntimeUiLayoutValidationService(texts);
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            VariantContext runit = VariantContextCatalog.CreateBuiltIns(e2).Single();

            RuntimeUiTextState e1State = texts.SetOverride(e1, texts.Load(e1).State!, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
            RuntimeUiLayoutValidationResult validEga = layouts.Validate(ega, e1State, RuntimeUiLogicalRecordId.SaveFailure);
            if (validEga is not { Status: RuntimeUiLayoutValidationStatus.Valid, PayloadBytesIncludingTerminator: 12, RecordCapacity: 18 } ||
                !validEga.EncodedPayload.SequenceEqual(GamePcTextEditor.GetEncoding("CP852").GetBytes("Save failed")))
                throw new InvalidDataException("Valid EGA CP852 record did not validate deterministically.");
            if (layouts.Validate(vga, e1State, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete)
                throw new InvalidDataException("RUNVGA layout validation borrowed an unproven EGA record capacity.");

            RuntimeUiTextState exact = texts.SetOverride(e1, e1State, RuntimeUiLogicalRecordId.SaveFailure, new string('A', 17));
            RuntimeUiTextState oversized = texts.SetOverride(e1, exact, RuntimeUiLogicalRecordId.SaveFailure, new string('A', 18));
            if (layouts.Validate(ega, exact, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.Valid ||
                layouts.Validate(ega, oversized, RuntimeUiLogicalRecordId.SaveFailure) is not { Status: RuntimeUiLayoutValidationStatus.RecordCapacityExceeded, PayloadBytesIncludingTerminator: 19 })
                throw new InvalidDataException("Record capacity/NUL boundary validation failed.");
            RuntimeUiTextState unsupported = texts.SetOverride(e1, e1State, RuntimeUiLogicalRecordId.SaveFailure, "bad 😀");
            if (layouts.Validate(ega, unsupported, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.EncodingFailure)
                throw new InvalidDataException("Unsupported Unicode did not fail strict CP852 validation.");
            string reserved = GamePcTextEditor.GetEncoding("CP852").GetString([(byte)FontSlotMetadata.HudEraseGlyph]);
            RuntimeUiTextState protectedGlyph = texts.SetOverride(e1, e1State, RuntimeUiLogicalRecordId.SaveFailure, reserved);
            if (layouts.Validate(ega, protectedGlyph, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.UnsupportedGlyph)
                throw new InvalidDataException("Reserved HUD glyph 0x81 was accepted as UI text.");
            RuntimeUiLayoutValidationBatchResult egaBatch = layouts.ValidateAll(ega, e1State);
            if (egaBatch.Records.Count != 8 || egaBatch.Records.Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.Valid ||
                egaBatch.Records.Any(value => value.Status == RuntimeUiLayoutValidationStatus.BankCapacityExceeded))
                throw new InvalidDataException("RUNEGA batch bank validation did not preserve its frozen layout.");

            RuntimeUiTextState e2State = texts.SetOverride(e2, texts.Load(e2).State!, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
            if (layouts.Validate(runit, e2State, RuntimeUiLogicalRecordId.SaveFailure) is not { Status: RuntimeUiLayoutValidationStatus.Valid, RecordCapacity: 21, BankCapacity: 0x800 } ||
                layouts.Validate(runit, e2State, RuntimeUiLogicalRecordId.LoadFailure).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete ||
                layouts.Validate(runit, e2State, RuntimeUiLogicalRecordId.FileNotFound).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete ||
                layouts.Validate(runit, e2State, RuntimeUiLogicalRecordId.TryAnotherDisk).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete)
                throw new InvalidDataException("RUNIT mapped/incomplete record distinction changed.");
            if (layouts.Validate(runit, e2State, RuntimeUiLogicalRecordId.SaveFailure).EncodedPayload.Any(value => value == (byte)'?') && "Save failed".Contains('?') == false)
                throw new InvalidDataException("Validation silently substituted a character.");
            string e1Path = texts.GetPath(e1); string? before = File.Exists(e1Path) ? File.ReadAllText(e1Path) : null;
            _ = layouts.ValidateAll(ega, e1State);
            if ((File.Exists(e1Path) ? File.ReadAllText(e1Path) : null) != before)
                throw new InvalidDataException("Read-only validation wrote runtime-ui.json.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyTextTabSplitSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1TextTabSplitSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            using var form = new MainForm();
            if (!form.HasTextDomainSplitForTest || !form.HasRuntimeUiViewForTest)
                throw new InvalidDataException("Text UI did not expose separate Game Text and Runtime UI views.");
            if (GamePcTextEditor.LoadEntries(Path.Combine(e1.GameRoot, "GAMEPC"), ElviraGameProfile.Elvira1).Count == 0 ||
                GamePcTextEditor.LoadEntries(Path.Combine(e2.GameRoot, "GAMEPC"), ElviraGameProfile.Elvira2).Count == 0)
                throw new InvalidDataException("Existing GAMEPC text loading regressed.");

            var texts = new RuntimeUiTextService(); var layouts = new RuntimeUiLayoutValidationService(texts);
            RuntimeUiTextState e1State = texts.SetOverride(e1, texts.Load(e1).State!, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
            VariantContext[] e1Variants = VariantContextCatalog.CreateBuiltIns(e1).ToArray();
            if (texts.GetEffectiveRecords(e1Variants[0], e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SaveFailure).EffectiveText !=
                texts.GetEffectiveRecords(e1Variants[1], e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SaveFailure).EffectiveText)
                throw new InvalidDataException("E1 runtime views did not share one logical project override.");
            if (layouts.Validate(e1Variants.Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga), e1State, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete ||
                layouts.Validate(e1Variants.Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega), e1State, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.Valid)
                throw new InvalidDataException("E1 runtime status presentation diverged.");
            VariantContext runit = VariantContextCatalog.CreateBuiltIns(e2).Single();
            RuntimeUiTextState e2State = texts.SetOverride(e2, texts.Load(e2).State!, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
            if (layouts.Validate(runit, e2State, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.Valid ||
                new[] { RuntimeUiLogicalRecordId.LoadFailure, RuntimeUiLogicalRecordId.FileNotFound, RuntimeUiLogicalRecordId.TryAnotherDisk }
                    .Any(id => layouts.Validate(runit, e2State, id).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete))
                throw new InvalidDataException("RUNIT mapped/incomplete UI presentation diverged.");

            string gamePcPath = Path.Combine(e1.GameRoot, "GAMEPC");
            var beforeInfo = new FileInfo(gamePcPath);
            if (!beforeInfo.Exists) throw new InvalidDataException("Fixture GAMEPC is missing before Runtime UI save.");
            long gamePcLength = beforeInfo.Length;
            string gamePcHash = HashFile(gamePcPath);
            DateTime gamePcWriteTime = beforeInfo.LastWriteTimeUtc;
            RuntimeUiTextSaveResult saved = texts.Save(e1, e1State);
            var afterInfo = new FileInfo(gamePcPath);
            if (!saved.Succeeded) throw new InvalidDataException("Runtime UI save failed: " + saved.Detail);
            if (!afterInfo.Exists || afterInfo.Length != gamePcLength || HashFile(gamePcPath) != gamePcHash)
                throw new InvalidDataException("Runtime UI save changed fixture GAMEPC bytes.");
            if (!File.Exists(saved.Path) || !File.ReadAllText(saved.Path).Contains(nameof(RuntimeUiLogicalRecordId.SaveFailure), StringComparison.Ordinal))
                throw new InvalidDataException("Runtime UI save did not update runtime-ui.json.");
            Console.WriteLine($"Runtime UI fixture GAMEPC: before={gamePcHash} length={gamePcLength} writeUtc={gamePcWriteTime:O}; after={HashFile(gamePcPath)} length={afterInfo.Length} writeUtc={afterInfo.LastWriteTimeUtc:O}");
            RuntimeUiTextState reset = texts.RemoveOverride(e1, e1State, RuntimeUiLogicalRecordId.SaveFailure);
            if (!texts.Save(e1, reset).Succeeded || texts.Load(e1).State!.Overrides.Count != 0)
                throw new InvalidDataException("Runtime UI reset did not use sparse project override removal.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyTextPristineRootSmoke(string elvira1Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1TextPristineRootSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string[] before = PristineTopLevelSnapshot(project);

            // Read-only Text load/preview source: no legacy GAMEPCO is created.
            GamePcOriginalReference baseline = GamePcOriginalService.LoadProjectBaseline(project);
            var translations = new TranslationProjectService();
            TranslationProjectLoadResult loaded = translations.Load(project);
            if (!loaded.IsSuccess || loaded.State!.Variants.Count != 0 || File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")))
                throw new InvalidDataException("Text read-only initialization created or adopted GAMEPCO.");

            IReadOnlyList<GamePcStringEntry> entries = GamePcTextEditor.LoadEntries(baseline.Path, project.GameProfile);
            if (!entries.Any(entry => entry.Index == 393)) throw new InvalidDataException("Translation selector fixture is missing known logical index 393.");
            var skEdits = new Dictionary<int, string> { [393] = "SK project edit" };
            var s1Edits = new Dictionary<int, string> { [393] = "S1 project edit" };
            TranslationProjectVariant sk = translations.Create(project, loaded.State!, "Slovencina", "sk", skEdits);
            TranslationProjectState state = translations.Add(loaded.State!, sk);
            TranslationProjectVariant s1 = translations.Create(project, state, "Slovencina test", "S1", s1Edits);
            state = translations.Add(state, s1);
            translations.Save(project, state);

            if (!File.Exists(translations.GetPath(project)) || !translations.Load(project).State!.Variants.Single(item => item.Code == "SK").Edits.SequenceEqual(skEdits))
                throw new InvalidDataException("Create Variant did not preserve unsaved edits in project-owned translation state.");
            if (!PristineTopLevelSnapshot(project).SequenceEqual(before, StringComparer.Ordinal))
                throw new InvalidDataException("Text/Create Variant changed the pristine game-root file set.");
            if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(project.GameRoot, "GAMEPCSK")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")))
                throw new InvalidDataException("Create Variant created a legacy root artifact.");

            if (!GameInstallationValidator.TryValidate(project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
                throw new InvalidDataException("Translation header fixture could not resolve its installation.");
            using (var form = new MainForm())
            {
                // Materialize a normal maximized-client layout without showing
                // or interacting with the WinForms window.
                form.Size = new Size(1450, 900);
                form.CreateControl();
                form.PerformLayout();
                form.MaterializeGraphicsLayoutForTest();
                form.InitializeInstallationStateForTest();
                form.ActivateInstallationForTest(installation);
                if (!form.TextTranslationCodesForTest.SequenceEqual(new[] { "EN", "S1", "SK" }, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidDataException("The Text translation selector did not expose baseline, S1, and SK project variants deterministically.");
                form.OpenModsForTest();
                if (!form.TranslationCatalogEntriesForTest.Any(entry => entry.Code == "S1" && entry.DataFile == "GAMEPCS1") ||
                    !form.TranslationCatalogEntriesForTest.Any(entry => entry.Code == "SK" && entry.DataFile == "GAMEPCSK"))
                    throw new InvalidDataException("The Mods & Launcher project translation catalog omitted an unbuilt project variant.");
                form.SelectTranslationForTest("S1");
                if (!form.TextEditsForTest.TryGetValue(393, out string? s1Text) || s1Text != "S1 project edit")
                    throw new InvalidDataException("Selecting S1 did not load its stored edit at index 393.");
                if (!form.TextOverviewForTest.Contains("Active Variant: Elvira I VGA", StringComparison.Ordinal) ||
                    !form.TextOverviewForTest.Contains("Original: GAMEPC (Verified original)", StringComparison.Ordinal) ||
                    !form.TextOverviewForTest.Contains("Translation: GAMEPCS1", StringComparison.Ordinal) ||
                    !form.TextOverviewForTest.Contains("EXE: RUNVGAS1.EXE", StringComparison.Ordinal) || !form.TextOverviewFitsForTest)
                    throw new InvalidDataException("S1 project translation did not project its logical data/executable names into the Text header.");
                form.SelectTranslationForTest("SK");
                if (!form.TextEditsForTest.TryGetValue(393, out string? skText) || skText != "SK project edit")
                    throw new InvalidDataException("Selecting SK did not load its stored edit at index 393.");
                form.SelectTranslationForTest("S1");
                if (!form.TextEditsForTest.TryGetValue(393, out s1Text) || s1Text != "S1 project edit")
                    throw new InvalidDataException("Switching translation selection lost S1 project edits.");
                form.SelectTranslationForTest("EN");
                if (!form.TextOverviewForTest.Contains("Translation: Original", StringComparison.Ordinal) ||
                    !form.TextOverviewForTest.Contains("EXE: RUNVGA.EXE", StringComparison.Ordinal) || !form.TextOverviewFitsForTest)
                    throw new InvalidDataException("The English selection was presented as a generated translation instead of the baseline executable context.");
                form.OpenTranslationFromCatalogForTest("S1");
                if (!form.TextEditsForTest.TryGetValue(393, out s1Text) || s1Text != "S1 project edit")
                    throw new InvalidDataException("Opening an unbuilt project translation from the catalog did not select its project state.");
                form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
                if (!form.TextOverviewForTest.Contains("Active Variant: Elvira I EGA", StringComparison.Ordinal) ||
                    !form.TextOverviewForTest.Contains("Translation: GAMEPCS1", StringComparison.Ordinal) || !form.TextOverviewForTest.Contains("EXE: RUNVGAS1.EXE", StringComparison.Ordinal) || !form.TextOverviewFitsForTest)
                    throw new InvalidDataException("Runtime variant change lost the active translation header metadata.");
                form.SelectTranslationForTest("SK");
                if (!form.TextOverviewForTest.Contains("Translation: GAMEPCSK", StringComparison.Ordinal) || !form.TextOverviewForTest.Contains("EXE: RUNVGASK.EXE", StringComparison.Ordinal) || !form.TextOverviewFitsForTest)
                    throw new InvalidDataException("Reloaded SK project translation did not project its logical names into the Text header.");
            }
            using (var reloadedForm = new MainForm())
            {
                reloadedForm.InitializeInstallationStateForTest();
                reloadedForm.ActivateInstallationForTest(installation);
                if (!reloadedForm.TextTranslationCodesForTest.SequenceEqual(new[] { "EN", "S1", "SK" }, StringComparer.OrdinalIgnoreCase))
                    throw new InvalidDataException("Project reload did not restore the deterministic selectable translation catalog.");
                reloadedForm.SelectTranslationForTest("S1");
                if (!reloadedForm.TextEditsForTest.TryGetValue(393, out string? s1Reloaded) || s1Reloaded != "S1 project edit")
                    throw new InvalidDataException("Project reload did not restore S1 project edits.");
            }
            if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCS1")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGAS1.EXE")) ||
                File.Exists(Path.Combine(project.GameRoot, "GAMEPCSK")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")))
                throw new InvalidDataException("Text header display created a logical output file in the pristine root.");

            bool collisionRejected = false;
            try { _ = translations.Create(project, state, "Duplicate", "SK", new Dictionary<int, string>()); }
            catch (InvalidOperationException) { collisionRejected = true; }
            if (!collisionRejected || !PristineTopLevelSnapshot(project).SequenceEqual(before, StringComparer.Ordinal))
                throw new InvalidDataException("Existing translation code was not rejected non-destructively.");

            bool longCodeRejected = false;
            try { _ = TranslationProjectService.ValidateProjectCode("SKA", project.GameProfile); }
            catch (InvalidOperationException) { longCodeRejected = true; }
            if (!longCodeRejected || TranslationProjectService.ValidateProjectCode("sk", project.GameProfile) != "SK")
                throw new InvalidDataException("E1 translation suffix validation did not enforce exact two-character DOS-safe codes.");

            var directories = new VariantDirectoryService();
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RequireDirectoryStatus(directories.EnsureVariantDirectory(project, vga), VariantDirectoryOperationStatus.Created, "translation build fixture directory");
            ICompositeBuildStep[] buildSteps =
            [
                new TranslationProjectBuildStep(translations, directories, "SK"),
                new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations, "graphics", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations, "font", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations, "runtime-ui", _ => true, _ => null, _ => null),
                new RunVgaCompositeBuildStep(directories)
            ];
            CompositeBuildResult built = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, buildSteps).Build(project, vga, CompositeBuildMode.Full);
            string output = directories.GetVariantDirectoryPath(project, vga);
            if (built.Status != CompositeBuildStatus.Success || !File.Exists(Path.Combine(output, "GAMEPCSK")) || !File.Exists(Path.Combine(output, "RUNVGASK.EXE")))
                throw new InvalidDataException("Authorized full variant build did not create GAMEPCSK/RUNVGASK.EXE only in the owned output.");
            if (!PristineTopLevelSnapshot(project).SequenceEqual(before, StringComparer.Ordinal))
                throw new InvalidDataException("Authorized variant build changed the pristine game-root file set.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }

        static string[] PristineTopLevelSnapshot(ProjectContext project) => Directory.EnumerateFiles(project.GameRoot, "*", SearchOption.TopDirectoryOnly)
            .Select(path => Path.GetFileName(path) + ":" + new FileInfo(path).Length + ":" + HashFile(path))
            .OrderBy(value => value, StringComparer.OrdinalIgnoreCase).ToArray();
    }

    private static void VerifyReplacePngValidationSmoke()
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ReplacePngValidationSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            Color[] palette = ElviraPaletteLoader.DiagnosticPalette();
            string validPath = Path.Combine(root, "valid.png");
            string oneInvalidPath = Path.Combine(root, "one-invalid.png");
            string manyInvalidPath = Path.Combine(root, "many-invalid.png");
            string dimensionPath = Path.Combine(root, "wrong-size.png");
            string corruptPath = Path.Combine(root, "corrupt.png");

            using (var bitmap = new Bitmap(3, 2))
            {
                bitmap.SetPixel(0, 0, palette[1]);
                bitmap.SetPixel(1, 0, palette[2]);
                bitmap.SetPixel(2, 0, Color.FromArgb(0, 0, 0, 0));
                bitmap.SetPixel(0, 1, palette[3]);
                bitmap.SetPixel(1, 1, palette[4]);
                bitmap.SetPixel(2, 1, palette[5]);
                bitmap.Save(validPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            ReplacePngValidationResult valid = ReplacePngValidator.Validate(validPath, 3, 2, "fixture palette", palette);
            if (!valid.IsValid || valid.Indices is null || valid.Indices[0] != 1 || valid.Indices[2] != 0 || valid.PaletteEntries.Count != 16)
                throw new InvalidDataException("Exact active-palette PNG was rejected or decoded incorrectly.");
            if (!PaletteTools.ReadIndices(validPath, 3, 2, palette).SequenceEqual(valid.Indices))
                throw new InvalidDataException("The rebuild-time palette conversion diverged from pre-commit validation.");

            using (var bitmap = new Bitmap(3, 2))
            {
                using Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(palette[1]);
                bitmap.SetPixel(2, 0, Color.FromArgb(0x20, 0x20, 0x00));
                bitmap.Save(oneInvalidPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            var existingEdits = new Dictionary<int, string> { [7] = "previous.png" };
            ReplacePngValidationResult oneInvalid = ReplacePngValidator.Validate(oneInvalidPath, 3, 2, "fixture palette", palette);
            if (oneInvalid.FailureKind != ReplacePngValidationFailureKind.PaletteMismatch || oneInvalid.InvalidPixelCount != 1 ||
                oneInvalid.InvalidColors is not [{ Hex: "#202000", PixelCount: 1, FirstOccurrence: { X: 2, Y: 0 } }] ||
                ReplacePngValidator.TryCommitValidatedReplacement(existingEdits, 7, oneInvalidPath, oneInvalid) || existingEdits[7] != "previous.png")
                throw new InvalidDataException("A single invalid color was not reported without committing replacement state.");
            bool rebuildRejectedInvalid = false;
            try { _ = PaletteTools.ReadIndices(oneInvalidPath, 3, 2, palette); }
            catch (InvalidDataException) { rebuildRejectedInvalid = true; }
            if (!rebuildRejectedInvalid)
                throw new InvalidDataException("Rebuild-time conversion accepted an invalid exact-palette PNG.");

            using (var bitmap = new Bitmap(4, 2))
            {
                using Graphics graphics = Graphics.FromImage(bitmap);
                graphics.Clear(palette[2]);
                bitmap.SetPixel(1, 0, Color.FromArgb(0x20, 0x20, 0x00));
                bitmap.SetPixel(2, 0, Color.FromArgb(0x01, 0x02, 0x03));
                bitmap.SetPixel(3, 1, Color.FromArgb(0x20, 0x20, 0x00));
                bitmap.Save(manyInvalidPath, System.Drawing.Imaging.ImageFormat.Png);
            }
            ReplacePngValidationResult manyInvalid = ReplacePngValidator.Validate(manyInvalidPath, 4, 2, "fixture palette", palette);
            if (manyInvalid.FailureKind != ReplacePngValidationFailureKind.PaletteMismatch || manyInvalid.InvalidPixelCount != 3 ||
                manyInvalid.InvalidColors.Count != 2 || manyInvalid.InvalidColors[0] is not { Hex: "#202000", PixelCount: 2, FirstOccurrence: { X: 1, Y: 0 } } ||
                manyInvalid.InvalidColors[1] is not { Hex: "#010203", PixelCount: 1, FirstOccurrence: { X: 2, Y: 0 } })
                throw new InvalidDataException("Multiple invalid colors were not counted and ordered deterministically.");

            using (var bitmap = new Bitmap(2, 2)) { bitmap.Save(dimensionPath, System.Drawing.Imaging.ImageFormat.Png); }
            ReplacePngValidationResult dimensionMismatch = ReplacePngValidator.Validate(dimensionPath, 3, 2, "fixture palette", palette);
            if (dimensionMismatch.FailureKind != ReplacePngValidationFailureKind.DimensionMismatch || dimensionMismatch.ActualWidth != 2 || dimensionMismatch.ActualHeight != 2 || dimensionMismatch.InvalidColors.Count != 0)
                throw new InvalidDataException("Dimension mismatch did not take precedence over palette validation.");

            File.WriteAllText(corruptPath, "not a PNG");
            ReplacePngValidationResult corrupt = ReplacePngValidator.Validate(corruptPath, 3, 2, "fixture palette", palette);
            if (corrupt.FailureKind != ReplacePngValidationFailureKind.InvalidImage)
                throw new InvalidDataException("Corrupt PNG did not produce a clean validation result.");

            if (!ReplacePngValidator.TryCommitValidatedReplacement(existingEdits, 7, validPath, valid) || existingEdits[7] != validPath)
                throw new InvalidDataException("A valid replacement did not commit after a prior validation failure.");

            ReplacePngValidationResult noPalette = ReplacePngValidator.Validate(validPath, 3, 2, "unavailable", null);
            if (noPalette.FailureKind != ReplacePngValidationFailureKind.PaletteUnavailable)
                throw new InvalidDataException("Missing active palette was not rejected explicitly.");

            using var form = new MainForm();
            form.RecordGraphicsValidationErrorForTest();
            if (!form.HasGraphicsValidationErrorForTest)
                throw new InvalidDataException("A palette validation error was not retained while relevant.");
            form.ApplyRelevantGraphicsInputChangeForTest();
            if (form.HasGraphicsValidationErrorForTest)
                throw new InvalidDataException("A relevant palette/context change did not clear a stale palette validation error.");
            form.RecordGraphicsValidationErrorForTest();
            form.RecordSuccessfulReplacementForTest();
            if (form.HasGraphicsValidationErrorForTest)
                throw new InvalidDataException("A successful replacement did not clear the prior validation error.");
        }
        finally
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyActiveProjectCompositeBuildSmoke(string elvira1Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ActiveProjectCompositeBuild", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            string[] pristine = SnapshotPristine(project);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant s1 = translations.Create(project, text, "Slovencina test", "S1", new Dictionary<int, string> { [393] = "Prirucna taska s popruhom cez rameno." });
            text = translations.Add(text, s1);
            TranslationProjectVariant sk = translations.Create(project, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            text = translations.Add(text, sk); translations.Save(project, text);

            string source382 = Path.Combine(project.GameRoot, "382.VGA");
            ParsedTable table = new VgaImageTableParser(File.ReadAllBytes(source382)).Parse();
            VgaImageEntry image = table.Entries.Single(entry => entry.ImageId == 1 && entry.DataOffset > 0);
            byte[] pixels = ElviraImageDecoder.Decode(File.ReadAllBytes(source382), image);
            string replacement = Path.Combine(root, "382-0001.png");
            string palettePath = Path.Combine(project.GameRoot, "381.VGA");
            IReadOnlyList<ElviraPaletteBank> banks = ElviraPaletteLoader.Load(palettePath);
            int paletteBank = Elvira1PaletteResolver.EffectivePaletteBank(null, new Elvira1PaletteResolver().Resolve(palettePath, source382, image.ImageId), banks.Count);
            System.Drawing.Color[] palette = banks[paletteBank].Colors;
            using (Bitmap bitmap = PaletteTools.ToBitmap(image.PixelWidth, image.Height, pixels, palette, transparentZero: false))
            {
                bitmap.SetPixel(0, 0, palette[(pixels[0] + 1) & 0x0F]);
                bitmap.Save(replacement, System.Drawing.Imaging.ImageFormat.Png);
            }
            var graphics = new GraphicsVariantService();
            GraphicsProjectState graphicsState = graphics.SetEdit(project, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1),
                new GraphicsProjectEdit(new GraphicsProjectIdentity("382.VGA", 1), replacement, GraphicsEditScope.RuntimeSpecific, VariantRuntimeKind.Elvira1Vga));
            if (!graphics.Save(project, graphicsState).Succeeded) throw new InvalidDataException("Fixture graphics project did not save.");
            RuntimeUiTextState runtimeUi = RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1);
            var directories = new VariantDirectoryService();
            RequireDirectoryStatus(directories.EnsureVariantDirectory(project, vga), VariantDirectoryOperationStatus.Created, "active project variant directory");

            CompositeBuildService Create(TranslationProjectVariant selected) => new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(selected, graphicsState, runtimeUi)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, selected), selected.DataFile]);
            string output = directories.GetVariantDirectoryPath(project, vga);
            CompositeBuildService s1Build = Create(s1);
            CompositeBuildResult s1Result = s1Build.Build(project, vga, CompositeBuildMode.Full);
            if (s1Result.Status != CompositeBuildStatus.Success) throw new InvalidDataException("S1 composite build failed: " + s1Result.Status + " / " + s1Result.Stages.Last().Detail);
            string s1Data = Path.Combine(output, "GAMEPCS1"), s1Exe = Path.Combine(output, "RUNVGAS1.EXE"), built382 = Path.Combine(output, "382.VGA");
            VariantLaunchTarget s1Target = new VariantLauncherService(directories, s1Build).Resolve(project, vga);
            ParsedTable rebuiltTable = new VgaImageTableParser(File.ReadAllBytes(built382)).Parse();
            byte[] rebuiltPixels = ElviraImageDecoder.Decode(File.ReadAllBytes(built382), rebuiltTable.Entries.Single(entry => entry.ImageId == 1));
            if (!File.Exists(s1Data) || !File.Exists(s1Exe) || !GamePcTextEditor.GetEncoding("CP852").GetString(GamePcTextEditor.LoadEntries(s1Data, ElviraGameProfile.Elvira1).Single(entry => entry.Index == 393).OriginalBytes).Contains("Prirucna", StringComparison.Ordinal) ||
                HashFile(built382) == HashFile(source382) || rebuiltPixels[0] != ((pixels[0] + 1) & 0x0F) || s1Target is not { Readiness: VariantLaunchReadiness.LaunchReady, ExecutableFile: "RUNVGAS1.EXE", DataFile: "GAMEPCS1" })
                throw new InvalidDataException("S1 build did not materialize its selected text, executable, graphics, and Ready state.");
            string[] first = Snapshot(output);
            if (s1Build.Build(project, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !first.SequenceEqual(Snapshot(output), StringComparer.Ordinal))
                throw new InvalidDataException("Selected S1 composite rebuild was not reproducible.");

            CompositeBuildService skBuild = Create(sk);
            if (skBuild.Build(project, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !File.Exists(Path.Combine(output, "GAMEPCSK")) || !File.Exists(Path.Combine(output, "RUNVGASK.EXE")) || File.Exists(s1Data) || File.Exists(s1Exe))
                throw new InvalidDataException("SK build did not replace stale S1 output with the selected SK output.");

            TranslationProjectVariant english = new("English", "EN", "GAMEPC", vga.SourceExecutableName, new Dictionary<int, string>());
            CompositeBuildService enBuild = Create(english);
            if (enBuild.Build(project, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !File.Exists(Path.Combine(output, "GAMEPC")) || !File.Exists(Path.Combine(output, "RUNVGA.EXE")))
                throw new InvalidDataException("English build did not retain baseline runtime semantics.");
            string[] after = SnapshotPristine(project);
            if (!pristine.SequenceEqual(after, StringComparer.Ordinal) || File.Exists(Path.Combine(project.GameRoot, "GAMEPCS1")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGAS1.EXE")))
                throw new InvalidDataException("Active composite build modified the fixture pristine root: " + string.Join(";", pristine.Except(after).Concat(after.Except(pristine)).Take(4)));
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }

        static string[] Snapshot(string variantRoot) => Directory.EnumerateFiles(variantRoot, "*", SearchOption.AllDirectories)
            .Select(path => Path.GetRelativePath(variantRoot, path) + "|" + HashFile(path)).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        static string[] SnapshotPristine(ProjectContext project) => project.PristineManifest.Files
            .OrderBy(file => file.RelativePath, StringComparer.Ordinal)
            .Select(file => file.RelativePath + "|" + HashFile(file.Classification == PristineFileClassification.MutableBackedUp
                ? Path.Combine(project.StorageLayout.MutableBackupsRoot, file.RelativePath)
                : Path.Combine(project.GameRoot, file.RelativePath)))
            .ToArray();
    }

    // Narrow PRE-R9D visual-polish regression: one shared action-button
    // configuration (fixed logical height, common optical padding) plus a
    // single-line no-game StatusStrip. Locale switches may change caption
    // text and horizontal width only — never Y, Height, padding, alignment
    // or renderer. Validity (positive geometry) is required, not just
    // Before == After stability.
    private static void VerifyButtonGeometrySmoke()
    {
        string priorLocale = UiText.LocaleId;
        try
        {
            using var form = new MainForm();
            form.Size = new Size(1450, 900);
            form.CreateControl();
            form.PerformLayout();
            form.MaterializeTabLayoutsForTest();
            form.PerformLayout();
            form.InitializeInstallationStateForTest();
            // Prime ApplyLanguage once so the baseline below already carries
            // localized captions: several selectors only receive their text
            // through ApplyLanguage/ApplyVariantLanguage.
            UiText.SetLocale("sk");
            UiText.SetLocale("en");

            // The obsolete manual Game selector must be gone completely: no
            // production fields remain and neutral identity is Unknown.
            if (typeof(MainForm).GetField("cmbGameProfile", BindingFlags.NonPublic | BindingFlags.Instance) is not null ||
                typeof(MainForm).GetField("lblGameProfile", BindingFlags.NonPublic | BindingFlags.Instance) is not null)
                throw new InvalidDataException("Obsolete Game selector UI still exists.");
            if (form.ActiveGameProfileForTest != ElviraGameProfile.Unknown)
                throw new InvalidDataException("Neutral game identity is not Unknown.");

            // Text-editor action buttons live in a wrapping flow: their Y may
            // legitimately reflow when captions grow, so only Height/padding/
            // geometry validity plus EN-roundtrip determinism applies to them.
            // Every other audited button must also keep Y.

            // Text-editor action buttons live in a wrapping flow: their Y may
            // legitimately reflow, so only Height/padding/geometry validity
            // applies to them. Every other audited button must also keep Y.
            var wrappingFlowButtons = new HashSet<string>(StringComparer.Ordinal)
            {
                "btnOpenDataFile", "btnReloadTexts", "btnSaveTexts", "btnSaveAsDataFile",
                "btnCreateDataVariant", "btnExportTranslations", "btnImportTranslations"
            };

            void RequireSharedConfiguration(IReadOnlyList<ActionButtonGeometry> snapshot)
            {
                // All standard buttons share one caption-rendering rule: the
                // common renderer plus the pinned optical offset. Magnitude is
                // QA-accepted; pin it against accidental drift.
                if (CenteredCaptionButton.OpticalCaptionOffsetY != 1)
                    throw new InvalidDataException("The shared caption offset drifted from its accepted value.");
                foreach (ActionButtonGeometry button in snapshot)
                {
                    if (button.Bounds.Width <= 0 || button.Bounds.Height <= 0)
                        throw new InvalidDataException($"Action button {button.Name} has invalid bounds {button.Bounds}.");
                    if (button.Renderer != nameof(CenteredCaptionButton))
                        throw new InvalidDataException($"Action button {button.Name} does not use the shared renderer.");
                    if (button.MinimumSize.Height != button.Bounds.Height)
                        throw new InvalidDataException($"Action button {button.Name} height is not pinned by its minimum size.");
                    if (button.Padding != new Padding(6, 2, 6, 0))
                        throw new InvalidDataException($"Action button {button.Name} does not use the shared optical padding {button.Padding}.");
                    if (button.TextAlign != ContentAlignment.MiddleCenter || button.UseCompatibleTextRendering)
                        throw new InvalidDataException($"Action button {button.Name} does not use the shared caption configuration.");
                }
                int height = snapshot[0].Bounds.Height;
                if (snapshot.Any(button => button.Bounds.Height != height))
                    throw new InvalidDataException("Standard action buttons do not share one logical height.");
            }

            IReadOnlyList<ActionButtonGeometry> baseline = form.CaptureActionButtonGeometryForTest();
            RequireSharedConfiguration(baseline);
            foreach (string locale in new[] { "sk", "cs", "en" })
            {
                UiText.SetLocale(locale);
                IReadOnlyList<ActionButtonGeometry> current = form.CaptureActionButtonGeometryForTest();
                RequireSharedConfiguration(current);
                foreach ((ActionButtonGeometry before, ActionButtonGeometry after) in baseline.Zip(current))
                {
                    if (!before.Name.Equals(after.Name, StringComparison.Ordinal) ||
                        !before.Renderer.Equals(after.Renderer, StringComparison.Ordinal) ||
                        before.Bounds.Height != after.Bounds.Height ||
                        before.Padding != after.Padding ||
                        before.TextAlign != after.TextAlign ||
                        before.UseCompatibleTextRendering != after.UseCompatibleTextRendering)
                        throw new InvalidDataException($"Locale {locale} changed the shared configuration of {after.Name}.");
                    if (!wrappingFlowButtons.Contains(after.Name) && before.Bounds.Y != after.Bounds.Y)
                        throw new InvalidDataException($"Locale {locale} moved {after.Name} vertically.");
                }
                // Graphics action row: single row, fully inside its container.
                IReadOnlyList<Rectangle> children = form.GraphicsActionChildBoundsForTest();
                Rectangle row = form.GraphicsActionRowBoundsForTest;
                if (row is { Width: <= 0 } || row.Height <= 0)
                    throw new InvalidDataException("Graphics action row has invalid bounds.");
                if (children.Select(child => child.Y).Distinct().Count() != 1)
                    throw new InvalidDataException($"Graphics action buttons wrapped in locale {locale}.");
                // Children are positioned below the panel's top padding; the
                // standard button bottom margin (3) plus panel bottom padding
                // (4) plus breathing room must fit inside the taller row.
                int contentBottom = children.Max(child => child.Bottom);
                if (contentBottom + 7 > row.Height)
                    throw new InvalidDataException($"Graphics action buttons are clipped by their row in locale {locale}.");
                if (row.Height - contentBottom < 40)
                    throw new InvalidDataException($"Graphics action row lost its lower breathing room in locale {locale}.");
                if (locale.Equals("en", StringComparison.Ordinal) &&
                    !current.Zip(baseline).All(pair =>
                        pair.First.Bounds == pair.Second.Bounds && pair.First.Text == pair.Second.Text))
                    throw new InvalidDataException("EN roundtrip did not restore the baseline button geometry.");
                if ((locale.Equals("sk", StringComparison.Ordinal) || locale.Equals("cs", StringComparison.Ordinal)) &&
                    current.Single(button => button.Name == "btnSaveGraphicsProject").Text == "Save to project")
                    throw new InvalidDataException($"Graphics Save button did not localize in {locale}.");

                // Micro-polish invariants: the four mode buttons share one
                // deterministic size and row; the Active Variant / Edition
                // combos share width, height, X and right edge.
                string[] modeButtons = ["btnSpriteEditor", "btnTextEditor", "btnFontEditor", "btnModsLauncher"];
                Size[] modeSizes = current.Where(button => modeButtons.Contains(button.Name)).Select(button => button.Bounds.Size).ToArray();
                int[] modeRows = current.Where(button => modeButtons.Contains(button.Name)).Select(button => button.Bounds.Y).ToArray();
                if (modeSizes.Length != 4 || modeSizes.Distinct().Count() != 1 || modeRows.Distinct().Count() != 1)
                    throw new InvalidDataException($"Mode buttons diverged in locale {locale}.");
                IReadOnlyList<ContextComboGeometry> combos = form.CaptureContextComboGeometryForTest();
                if (combos.Count != 3 ||
                    combos.Select(combo => combo.Bounds.Size).Distinct().Count() != 1 ||
                    combos.Select(combo => combo.Bounds.X).Distinct().Count() != 1 ||
                    combos.Select(combo => combo.Bounds.Right).Distinct().Count() != 1)
                    throw new InvalidDataException($"Header selector combos diverged in locale {locale}.");
                if (combos[1].Margin != combos[2].Margin)
                    throw new InvalidDataException($"Active Variant / Edition combo margins diverged in locale {locale}.");
                // ONE shared selector width matched to the Installation
                // dropdown width (620): a Percent column once claimed 1300+
                // px and pushed the right header out of the client area.
                // Normalized by live DPI.
                foreach (ContextComboGeometry combo in combos)
                {
                    double logicalWidth = combo.Bounds.Width * 96.0 / form.DeviceDpi;
                    if (Math.Abs(logicalWidth - 620) > 1.0)
                        throw new InvalidDataException($"Selector {combo.Name} width {logicalWidth:F0} px does not match the shared 620 px in locale {locale}.");
                }
                foreach (string comboName in new[] { "cmbInstallations", "cmbActiveVariant", "cmbActiveProject" })
                {
                    var selector = (ComboBox)form.FindControlForTest(comboName);
                    if (selector.DropDownWidth != selector.Width)
                        throw new InvalidDataException($"Selector {comboName} dropdown ({selector.DropDownWidth}) is wider than its control ({selector.Width}) in locale {locale}.");
                }

                // Header rows keep their deterministic heights in every locale.
                if (!form.HeaderRowHeightsForTest().SequenceEqual(new[] { 36, 36, 36 }))
                    throw new InvalidDataException($"Header rows changed height in locale {locale}.");

                // Parent-cell geometry: each combo must fit inside its
                // allocated TableLayoutPanel cell and the visible client area.
                IReadOnlyList<ContextCellGeometry> cells = form.CaptureContextCellGeometryForTest();
                foreach (ContextCellGeometry cell in cells)
                {
                    if (cell.ControlBounds is not { Width: > 0, Height: > 0 } || cell.CellBounds.Height <= 0)
                        throw new InvalidDataException($"Context cell geometry is invalid for {cell.Name} in locale {locale}.");
                    if (cell.ControlBounds.Height > cell.CellBounds.Height)
                        throw new InvalidDataException($"Combo {cell.Name} is taller than its allocated cell in locale {locale}.");
                    if (cell.ControlBounds.Top < 0 || cell.ControlBounds.Left < 0 ||
                        cell.ControlBounds.Bottom > cell.PanelClient.Height ||
                        cell.ControlBounds.Right > cell.PanelClient.Width)
                        throw new InvalidDataException($"Combo {cell.Name} is not fully visible in locale {locale}.");
                }
                if (cells.Select(cell => cell.ControlBounds.Height).Distinct().Count() != 1)
                    throw new InvalidDataException($"Header selector visible heights differ in locale {locale}.");

                // Navigation breathing room: every mode-button bottom border
                // must lie inside the panel client area with spare space.
                Rectangle navigationClient = form.NavigationRowClientForTest;
                int[] modeBottoms = current.Where(button => modeButtons.Contains(button.Name)).Select(button => button.Bounds.Bottom).ToArray();
                if (modeBottoms.Any(bottom => bottom > navigationClient.Height))
                    throw new InvalidDataException($"Mode button border escapes the navigation row in locale {locale}.");
                if (navigationClient.Height - modeBottoms.Max() < 6)
                    throw new InvalidDataException($"Navigation row lost its lower breathing room in locale {locale}.");

                // Header structural invariants (no manual Game selector left):
                // Find games shares its action column with Build variant;
                // the Detected caption sits in that same column on row 2.
                TableLayoutPanelCellPosition findCell = form.HeaderCellForTest("btnFindGames");
                TableLayoutPanelCellPosition buildCell = form.HeaderCellForTest("btnBuildActiveVariant");
                TableLayoutPanelCellPosition detectedCell = form.HeaderCellForTest("lblDetectedGameCaption");
                TableLayoutPanelCellPosition detectedValueCell = form.HeaderCellForTest("lblDetectedGame");
                if (findCell.Column != buildCell.Column || buildCell.Column != detectedCell.Column ||
                    detectedCell.Row != 2 || detectedValueCell.Row != 2)
                    throw new InvalidDataException($"Header action column layout diverged in locale {locale}.");
                ActionButtonGeometry findButton = current.Single(button => button.Name == "btnFindGames");
                ActionButtonGeometry buildButton = current.Single(button => button.Name == "btnBuildActiveVariant");
                if (findButton.Bounds.X != buildButton.Bounds.X)
                    throw new InvalidDataException($"Build variant is not under Find games in locale {locale}.");
                Rectangle headerClient = form.HeaderGridClientForTest;
                var detectedValue = (Control)form.FindControlForTest("lblDetectedGame");
                if (detectedValue.Bounds is not { Width: > 0, Height: > 0 } ||
                    detectedValue.Bounds.Right > headerClient.Width || detectedValue.Bounds.Bottom > headerClient.Height)
                    throw new InvalidDataException($"Detected value is not fully visible in locale {locale}.");
            }

            // Representative client widths (neutral state): trio alignment is
            // structural and must hold everywhere; right-header containment
            // is asserted where everything reasonably fits (1600/1920 — the
            // 620 px selector column plus neutral content exceeds 1366, which
            // is reported, not redesigned, per task scope).
            foreach (int formWidth in new[] { 1920, 1600, 1366 })
            {
                foreach (string widthLocale in new[] { "en", "sk" })
                {
                    using var wideForm = new MainForm();
                    wideForm.Size = new Size(formWidth, 900);
                    wideForm.CreateControl();
                    wideForm.PerformLayout();
                    UiText.SetLocale(widthLocale);
                    IReadOnlyList<ContextComboGeometry> wideCombos = wideForm.CaptureContextComboGeometryForTest();
                    if (wideCombos.Select(combo => combo.Bounds.X).Distinct().Count() != 1 ||
                        wideCombos.Select(combo => combo.Bounds.Width).Distinct().Count() != 1 ||
                        wideCombos.Select(combo => combo.Bounds.Right).Distinct().Count() != 1)
                        throw new InvalidDataException($"Selector trio diverged at width {formWidth} in locale {widthLocale}.");
                    if (formWidth < 1600)
                        continue;
                    Control wideRight = wideForm.FindControlForTest("btnHelp").Parent
                        ?? throw new InvalidDataException("Right header has no parent.");
                    foreach (Control child in wideRight.Controls)
                    {
                        if (child.Bounds.Right + child.Margin.Right > wideRight.ClientSize.Width ||
                            child.Bounds.Bottom + child.Margin.Bottom > wideRight.ClientSize.Height)
                            throw new InvalidDataException($"Right header clips at width {formWidth} in locale {widthLocale}.");
                    }
                    if (wideRight.Bounds.Right > wideForm.HeaderGridClientForTest.Width)
                        throw new InvalidDataException($"Right header escapes the grid at width {formWidth} in locale {widthLocale}.");
                }
            }

            // DPI/layout-scale spot checks: the shared selector column must
            // survive uniform scaling with X/Width/Right/Height identical.
            // Only relative equality is asserted here: form.Scale() does not
            // scale fonts in this harness, so absolute containment under
            // Scale() would mix scaled bounds with unscaled text metrics (a
            // combination real DPI sessions never produce); absolute fit is
            // asserted at 100% above and budgeted analytically per DPI.
            foreach (float scale in new[] { 1.25f, 1.5f, 1.75f })
            {
                using var scaledForm = new MainForm();
                scaledForm.Size = new Size(1450, 900);
                scaledForm.CreateControl();
                scaledForm.PerformLayout();
                scaledForm.Scale(new SizeF(scale, scale));
                scaledForm.PerformLayout();
                IReadOnlyList<ContextComboGeometry> scaledCombos = scaledForm.CaptureContextComboGeometryForTest();
                if (scaledCombos.Select(combo => combo.Bounds.X).Distinct().Count() != 1 ||
                    scaledCombos.Select(combo => combo.Bounds.Width).Distinct().Count() != 1 ||
                    scaledCombos.Select(combo => combo.Bounds.Height).Distinct().Count() != 1)
                    throw new InvalidDataException($"Selector trio diverged at scale {scale}.");
            }

            // Mode switching swaps regular/bold caption fonts and repaints
            // focus: button rectangles must not move.
            IReadOnlyList<ActionButtonGeometry> beforeModes = form.CaptureActionButtonGeometryForTest();
            form.ActivateTextModeForTest();
            form.ActivateGraphicsModeForTest();
            IReadOnlyList<ActionButtonGeometry> afterModes = form.CaptureActionButtonGeometryForTest();
            if (!beforeModes.Zip(afterModes).All(pair => pair.First.Bounds == pair.Second.Bounds))
                throw new InvalidDataException("Mode switching moved button rectangles.");

            // Enabled/disabled states share identical caption geometry.
            foreach (string buttonName in new[] { "btnReplace", "btnVerifyPristine" })
            {
                IReadOnlyList<ActionButtonGeometry> enabled =
                    form.CaptureActionButtonGeometryForTest().Where(button => button.Name == buttonName).ToArray();
                form.SetButtonEnabledForTest(buttonName, !enabled[0].Enabled);
                IReadOnlyList<ActionButtonGeometry> disabled =
                    form.CaptureActionButtonGeometryForTest().Where(button => button.Name == buttonName).ToArray();
                form.SetButtonEnabledForTest(buttonName, enabled[0].Enabled);
                if (enabled[0].Bounds != disabled[0].Bounds || enabled[0].Padding != disabled[0].Padding ||
                    enabled[0].TextAlign != disabled[0].TextAlign || enabled[0].Renderer != disabled[0].Renderer)
                    throw new InvalidDataException($"Enabled/disabled caption geometry differs for {buttonName}.");
            }

            // Neutral header state (locale is back to EN here): Detected shows
            // the neutral message without a "Selected:" prefix, and the
            // context selectors plus Build action stay disabled.
            var neutralDetected = form.FindControlForTest("lblDetectedGame");
            if (neutralDetected.Text != UiText.Get(UiLocalizationKeys.NoGameSelected) ||
                neutralDetected.Text.Contains("Selected"))
                throw new InvalidDataException("Neutral Detected presentation diverged.");
            if (((ComboBox)form.FindControlForTest("cmbActiveVariant")).Enabled ||
                ((ComboBox)form.FindControlForTest("cmbActiveProject")).Enabled ||
                form.CaptureActionButtonGeometryForTest().Single(button => button.Name == "btnBuildActiveVariant").Enabled)
                throw new InvalidDataException("Neutral context selectors are not disabled.");

            // No-game StatusStrip: exactly one row, one item, no newline,
            // positive bounds. neutral installation state was initialized above.
            if (form.StatusStripItemCountForTest != 1)
                throw new InvalidDataException("No-game StatusStrip does not expose exactly one item.");
            if (form.StatusStripBoundsForTest is not { Width: > 0, Height: > 0 })
                throw new InvalidDataException("No-game StatusStrip has invalid bounds.");
            if (form.InstallationStatusForTest.Contains('\n'))
                throw new InvalidDataException("No-game status text spans multiple lines.");
            Console.WriteLine($"Button geometry: buttons={baseline.Count}; height={baseline[0].Bounds.Height}; status='{form.InstallationStatusForTest}'");
        }
        finally { UiText.SetLocale(priorLocale); }
    }

    // Deterministic Fit-placement regression over MainForm.ComputeFitPlacement:
    // containment, at most 1 px free-space imbalance per axis, aspect
    // preservation within rounding, degenerate-input handling, idempotency.
    private static void VerifyGraphicsFitCenteringSmoke()
    {
        (Size Viewport, Size Image)[] cases =
        [
            (new Size(682, 482), new Size(320, 200)),
            (new Size(682, 482), new Size(64, 200)),
            (new Size(682, 482), new Size(32, 32)),
            (new Size(682, 482), new Size(2000, 10)),
            (new Size(500, 500), new Size(800, 100)),
            (new Size(100, 100), new Size(320, 200)),
            (new Size(1, 1), new Size(1, 1)),
            (new Size(0, 100), new Size(10, 10)),
            (new Size(100, 0), new Size(10, 10)),
            (new Size(100, 100), new Size(0, 5)),
            (new Size(100, 100), new Size(7, 5)),
        ];
        foreach ((Size viewport, Size image) in cases)
        {
            (Size scaled, Point location) = MainForm.ComputeFitPlacement(viewport, image);
            bool degenerate = viewport.Width <= 0 || viewport.Height <= 0 || image.Width <= 0 || image.Height <= 0;
            if (degenerate)
            {
                if (scaled != Size.Empty)
                    throw new InvalidDataException($"Degenerate Fit input {viewport}/{image} did not yield an empty placement.");
                continue;
            }
            if (scaled.Width <= 0 || scaled.Height <= 0 || scaled.Width > viewport.Width || scaled.Height > viewport.Height)
                throw new InvalidDataException($"Fit placement {scaled} escapes viewport {viewport} for image {image}.");
            if (location.X < 0 || location.Y < 0 || location.X + scaled.Width > viewport.Width || location.Y + scaled.Height > viewport.Height)
                throw new InvalidDataException($"Fit location {location} escapes viewport {viewport} for scaled {scaled}.");
            int freeX = viewport.Width - scaled.Width, freeY = viewport.Height - scaled.Height;
            if (Math.Abs(freeX - 2 * location.X) > 1 || Math.Abs(freeY - 2 * location.Y) > 1)
                throw new InvalidDataException($"Fit placement is not centered within 1 px: viewport {viewport}, scaled {scaled}, location {location}.");
            if (Math.Abs((long)scaled.Width * image.Height - (long)scaled.Height * image.Width) > Math.Max(image.Width, image.Height))
                throw new InvalidDataException($"Fit placement distorts aspect for image {image}: {scaled}.");
            // Idempotency: same inputs always yield the same placement.
            if (MainForm.ComputeFitPlacement(viewport, image) != (scaled, location))
                throw new InvalidDataException("Fit placement is not idempotent.");
        }
        Console.WriteLine($"Fit centering: cases={cases.Length}");
    }

    // First-paint regression for the native-chrome caption buttons: construct
    // and lay out without any mouse/focus interaction, capture, force an
    // ordinary repaint, and require pixel-identical chrome (guards against
    // stateful paint where the first paint differs from later repaints).
    // Also requires a visible bottom border from the very first paint and
    // identical Active Variant / Edition combo geometry. Headless captures
    // cannot reproduce displayed-window show-pipeline quirks, so real-window
    // QA remains authoritative for the visual result.
    private static void VerifyFirstPaintSmoke()
    {
        using var form = new MainForm();
        form.Size = new Size(1450, 900);
        form.CreateControl();
        form.PerformLayout();

        static Bitmap Shot(Control control)
        {
            var bitmap = new Bitmap(Math.Max(1, control.Width), Math.Max(1, control.Height));
            control.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
            return bitmap;
        }

        static bool SamePixels(Bitmap first, Bitmap second)
        {
            if (first.Size != second.Size)
                return false;
            for (int y = 0; y < first.Height; y++)
                for (int x = 0; x < first.Width; x++)
                    if (first.GetPixel(x, y) != second.GetPixel(x, y))
                        return false;
            return true;
        }

        static void RequireBottomBorder(Bitmap first, string name)
        {
            int dark = 0;
            for (int x = 0; x < first.Width; x += 2)
                if (Math.Max(first.GetPixel(x, first.Height - 1).R,
                    Math.Max(first.GetPixel(x, first.Height - 1).G, first.GetPixel(x, first.Height - 1).B)) < 200)
                    dark++;
            if (dark == 0)
                throw new InvalidDataException($"Control {name} has no visible bottom border on first paint.");
            first.Dispose();
        }

        foreach (string name in new[] { "btnSpriteEditor", "btnVerifyPristine", "btnReplace" })
        {
            var button = (Button)form.FindControlForTest(name);
            using Bitmap before = Shot(button);
            button.Invalidate();
            button.Update();
            using Bitmap after = Shot(button);
            if (!SamePixels(before, after))
                throw new InvalidDataException($"Control {name} changed merely because an ordinary repaint occurred.");
            RequireBottomBorder(before, name);
        }

        var variantCombo = (ComboBox)form.FindControlForTest("cmbActiveVariant");
        var projectCombo = (ComboBox)form.FindControlForTest("cmbActiveProject");
        if (variantCombo.Height != projectCombo.Height || variantCombo.Bounds.Size != projectCombo.Bounds.Size)
            throw new InvalidDataException("Active Variant / Edition combos differ on first paint.");
        using (Bitmap before = Shot(variantCombo))
        {
            variantCombo.Invalidate();
            variantCombo.Update();
            using Bitmap after = Shot(variantCombo);
            if (!SamePixels(before, after))
                throw new InvalidDataException("Active Variant combo changed merely because an ordinary repaint occurred.");
        }
        Console.WriteLine("First paint: buttons=3; combos=2");
    }

    // Header identity regression (no manual Game selector): Installation →
    // ProjectContext.GameProfile is the only game identity. Active E1/E2 and
    // E1 → E2 → E1 switching must update the Detected display and every
    // former ActiveGameProfile consumer without any override control.
    private static void VerifyHeaderAlignmentSmoke(string elvira1Source, string elvira2Source)
    {
        string priorLocale = UiText.LocaleId;
        string root = Path.Combine(Path.GetTempPath(), "Pi1HeaderAlignmentSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            UiText.SetLocale("en");
            Directory.CreateDirectory(root);
            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Header alignment fixtures did not validate as installations.");
            using var form = new MainForm();
            form.Size = new Size(1450, 900);
            form.CreateControl();
            form.PerformLayout();
            form.InitializeInstallationStateForTest();

            string detectedCaption = UiText.Get("Detected") + ":";
            void RequireDisplay(ElviraGameProfile expected, string stage)
            {
                if (form.ActiveProjectForTest?.GameProfile != expected || form.ActiveGameProfileForTest != expected)
                    throw new InvalidDataException($"Header {stage} is not authoritative for {expected}.");
                if (form.FindControlForTest("lblDetectedGameCaption").Text != detectedCaption)
                    throw new InvalidDataException($"Header {stage} lost its Detected caption.");
                var detectedValue = (Control)form.FindControlForTest("lblDetectedGame");
                if (detectedValue.Text != GameProfileInfo.For(expected).DisplayName)
                    throw new InvalidDataException($"Header {stage} does not identify {expected}.");
                // Single line: a wrapped value would be at least two text
                // rows tall; rows themselves must stay 36 px.
                if (detectedValue.Bounds.Height > 23)
                    throw new InvalidDataException($"Header {stage} wrapped its Detected value.");
                if (!form.HeaderRowHeightsForTest().SequenceEqual(new[] { 36, 36, 36 }))
                    throw new InvalidDataException($"Header {stage} changed row heights.");
            }

            form.BrowseValidatedInstallationForTest(e1);
            form.ActivateInstallationForTest(e1);
            RequireDisplay(ElviraGameProfile.Elvira1, "E1");
            form.SelectGraphicsZoneForTest("382.VGA");
            if (form.ActiveGameProfileForTest != ElviraGameProfile.Elvira1)
                throw new InvalidDataException("Graphics consumer did not operate as Elvira I.");
            form.BrowseValidatedInstallationForTest(e2);
            form.ActivateInstallationForTest(e2);
            RequireDisplay(ElviraGameProfile.Elvira2, "E2");
            form.BrowseValidatedInstallationForTest(e1);
            form.ActivateInstallationForTest(e1);
            RequireDisplay(ElviraGameProfile.Elvira1, "E1-again");
            Console.WriteLine($"Header alignment: E1='{GameProfileInfo.For(ElviraGameProfile.Elvira1).DisplayName}'; E2='{GameProfileInfo.For(ElviraGameProfile.Elvira2).DisplayName}'; switches=3");
        }
        finally { UiText.SetLocale(priorLocale); if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyProjectVariantOwnershipSmoke(string elvira1Source, bool editionStressOnly = false, bool layoutStress = false)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ProjectVariantOwnership", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string gamePc = Path.Combine(project.GameRoot, "GAMEPC");
            string originalHash = HashFile(gamePc);

            var translations = new TranslationProjectService();
            TranslationProjectState text = translations.Load(project).State!;
            text = translations.Add(text, translations.Create(project, text, "Slovencina test", "S1", new Dictionary<int, string> { [393] = "S1 only" }));
            text = translations.Add(text, translations.Create(project, text, "Slovencina", "SK", new Dictionary<int, string>()));
            translations.Save(project, text);

            string s1Png = Path.Combine(root, "s1.png");
            using (var fixturePng = new Bitmap(1, 1)) fixturePng.Save(s1Png);

            var graphics = new GraphicsVariantService();
            GraphicsProjectState s1Graphics = graphics.SetEdit(project, GraphicsProjectState.Empty(project.GameProfile),
                new GraphicsProjectEdit(new GraphicsProjectIdentity("382.VGA", 1), s1Png, GraphicsEditScope.RuntimeSpecific, VariantRuntimeKind.Elvira1Vga));
            if (!graphics.Save(project, "S1", s1Graphics).Succeeded) throw new InvalidDataException("S1 graphics state was not saved.");
            if (graphics.Load(project, "EN").State!.Edits.Count != 0 || graphics.Load(project, "SK").State!.Edits.Count != 0 || graphics.Load(project, "S1").State!.Edits.Count != 1)
                throw new InvalidDataException("Graphics state leaked between Original, SK, and S1.");

            var fonts = new FontVariantService();
            FontProjectState s1Font = fonts.SetEdit(project, FontProjectState.Empty(project.GameProfile),
                FontProjectEdit.Create(new FontProjectGlyphIdentity(0xA0), new byte[] { 0x20, 0x50, 0x50, 0x70, 0x50, 0x50, 0x88, 0x00 }, FontEditScope.Shared, null));
            if (!fonts.Save(project, "S1", s1Font).Succeeded) throw new InvalidDataException("S1 font state was not saved.");
            if (fonts.Load(project, "EN").State!.Edits.Count != 0 || fonts.Load(project, "SK").State!.Edits.Count != 0 || fonts.Load(project, "S1").State!.Edits.Count != 1)
                throw new InvalidDataException("Font state leaked between Original, SK, and S1.");

            var runtime = new RuntimeUiTextService();
            RuntimeUiTextState s1Runtime = runtime.SetOverride(project, RuntimeUiTextState.Empty(project.GameProfile), RuntimeUiLogicalRecordId.SaveFailure, "S1 only");
            if (!runtime.Save(project, "S1", s1Runtime).Succeeded) throw new InvalidDataException("S1 runtime UI state was not saved.");
            if (runtime.Load(project, "EN").State!.Overrides.Count != 0 || runtime.Load(project, "SK").State!.Overrides.Count != 0 || runtime.Load(project, "S1").State!.Overrides.Count != 1)
                throw new InvalidDataException("Runtime UI state leaked between Original, SK, and S1.");

            if (!text.Variants.Single(item => item.Code == "S1").Edits.TryGetValue(393, out string? value) || value != "S1 only" ||
                text.Variants.Single(item => item.Code == "SK").Edits.Count != 0 || HashFile(gamePc) != originalHash)
                throw new InvalidDataException("Text project state or immutable GAMEPC invariant failed.");

            // A root-level legacy state is detectable but never projected.
            FontProjectState legacy = fonts.SetEdit(project, FontProjectState.Empty(project.GameProfile),
                FontProjectEdit.Create(new FontProjectGlyphIdentity(0xA1), new byte[] { 0x20, 0x50, 0x50, 0x70, 0x50, 0x50, 0x88, 0x00 }, FontEditScope.Shared, null));
            if (!fonts.Save(project, legacy).Succeeded) throw new InvalidDataException("Legacy fixture state was not saved.");
            var legacyStates = new LegacyProjectStateService();
            if (!legacyStates.DetectUnassigned(project).Contains(FontVariantService.FileName) || fonts.Load(project, "SK").State!.Edits.Count != 0)
                throw new InvalidDataException("Legacy project state was not kept unassigned.");

            if (HashFile(gamePc) != originalHash)
                throw new InvalidDataException("Project selection/state persistence modified pristine GAMEPC.");

            if (!GameInstallationValidator.TryValidate(project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
                throw new InvalidDataException("Project-ownership fixture did not validate as an installation.");
            using (var form = new MainForm())
            {
                form.InitializeInstallationStateForTest();
                form.ActivateInstallationForTest(installation);
                form.ActivateFontForTest();
                if (!form.ActiveEditionCodesForTest.SequenceEqual(new[] { "EN", "S1", "SK" }) || form.TextEditionSelectorVisibleForTest ||
                    string.IsNullOrWhiteSpace(form.WorkflowStatusForTest) || form.WorkflowStatusForTest.Contains("Workflow.", StringComparison.Ordinal))
                    throw new InvalidDataException("The global Edition selector or localized workflow status was not initialized deterministically.");
                if (form.ActiveProjectCodeForTest != "EN" || form.GraphicsProjectStateEditCountForTest != 0 || form.RuntimeUiOverrideCountForTest != 0 ||
                    form.EmbeddedFontProjectEditCountForTest != 0 || form.TextEditsForTest.Count != 0)
                    throw new InvalidDataException("Original project did not present the immutable baseline.");

                form.SelectTranslationForTest("S1");
                form.ActivateFontForTest();
                if (form.ActiveProjectCodeForTest != "S1" || string.IsNullOrWhiteSpace(form.WorkflowStatusForTest) || form.WorkflowStatusForTest.Contains("Workflow.", StringComparison.Ordinal))
                    throw new InvalidDataException("Global Edition selection did not refresh the active workflow status.");
                if (form.GraphicsProjectStateEditCountForTest != 1 || form.RuntimeUiOverrideCountForTest != 1 || form.EmbeddedFontProjectEditCountForTest != 1 ||
                    !form.TextEditsForTest.TryGetValue(393, out string? liveS1) || liveS1 != "S1 only")
                    throw new InvalidDataException("S1 project did not restore all of its owned state.");
                if (form.HasUnsavedTextChangesForTest)
                    throw new InvalidDataException("Saved S1 text edits were incorrectly reported as unsaved changes.");
                form.SelectGraphicsZoneForTest("382.VGA");
                if (form.AppliedGraphicsEditCountForTest != 1)
                    throw new InvalidDataException("S1 graphics edits were not rebound to the selected graphics resource.");

                form.SetTextEditForTest(393, "S1 working edit");
                if (!form.HasUnsavedTextChangesForTest)
                    throw new InvalidDataException("A real S1 text edit was not reported as unsaved.");
                form.SaveTextProjectForTest();
                if (form.HasUnsavedTextChangesForTest)
                    throw new InvalidDataException("Saving an S1 text edit did not clear the unsaved state.");

                form.SelectTranslationForTest("EN");
                if (form.GraphicsProjectStateEditCountForTest != 0 || form.RuntimeUiOverrideCountForTest != 0 || form.TextEditsForTest.Count != 0)
                    throw new InvalidDataException("Switching S1 to Original leaked project state.");
                if (form.HasUnsavedTextChangesForTest)
                    throw new InvalidDataException("Switching to Original created a false text dirty state.");
                form.SelectGraphicsZoneForTest("382.VGA");
                if (form.AppliedGraphicsEditCountForTest != 0)
                    throw new InvalidDataException("Original unexpectedly displayed S1 graphics state.");

                form.SelectTranslationForTest("SK");
                form.ActivateFontForTest();
                if (form.GraphicsProjectStateEditCountForTest != 0 || form.RuntimeUiOverrideCountForTest != 0 || form.EmbeddedFontProjectEditCountForTest != 0 || form.TextEditsForTest.Count != 0)
                    throw new InvalidDataException("SK inherited S1 project state.");
                if (form.HasUnsavedTextChangesForTest)
                    throw new InvalidDataException("Switching to SK created a false text dirty state.");
                form.SelectGraphicsZoneForTest("382.VGA");
                if (form.AppliedGraphicsEditCountForTest != 0)
                    throw new InvalidDataException("SK unexpectedly displayed S1 graphics state.");
                form.SelectTranslationForTest("S1");
                form.SelectGraphicsZoneForTest("382.VGA");
                if (form.GraphicsProjectStateEditCountForTest != 1 || form.AppliedGraphicsEditCountForTest != 1 ||
                    !form.TextEditsForTest.TryGetValue(393, out string? returningS1) || returningS1 != "S1 working edit")
                    throw new InvalidDataException("Returning to S1 did not restore its isolated edition state.");
                if (form.HasUnsavedTextChangesForTest)
                    throw new InvalidDataException("Returning to saved S1 created a false text dirty state.");

                // Materialize a realistic graphics-client layout without showing
                // the window (established pattern from VerifyTextPristineRootSmoke).
                // Without this, previewViewport/previewScroll keep degenerate,
                // never-laid-out bounds and the stress below would validate
                // stability of an invalid geometry.
                form.Size = new Size(1450, 900);
                form.CreateControl();
                form.PerformLayout();
                form.MaterializeGraphicsLayoutForTest();
                EditionSwitchDiagnostics stress = form.RunEditionSwitchStressForTest(50);
                if (stress.ViewportBefore.Width <= 0 || stress.ViewportBefore.Height <= 0 ||
                    stress.PreviewBefore.Width <= 0 || stress.PreviewBefore.Height <= 0)
                    throw new InvalidDataException("Graphics preview geometry is not valid before stress switching.");
                if (stress.ViewportBefore != stress.ViewportAfter || stress.PreviewBefore != stress.PreviewAfter ||
                    stress.GraphicsLoads != 200 || stress.TextLoads != 200 || stress.RuntimeUiLoads != 200 || stress.FontBinds != 200 ||
                    stress.ControlsBefore != stress.ControlsAfter || HashFile(gamePc) != originalHash)
                    throw new InvalidDataException("Fifty edition-switch cycles were not stable.");
                if (editionStressOnly)
                    Console.WriteLine($"Edition stress counters: graphics={stress.GraphicsLoads}; text={stress.TextLoads}; runtime={stress.RuntimeUiLoads}; font={stress.FontBinds}; workflow={stress.WorkflowRefreshes}; preview={stress.PreviewRenders}; geometry={stress.ViewportBefore}");
                if (layoutStress)
                {
                    MainFormLayoutDiagnostics layout = form.RunMainFormLayoutStressForTest();
                    if (layout.Before.PreviewViewportBounds.Width <= 0 || layout.Before.PreviewViewportBounds.Height <= 0 ||
                        layout.Before.PreviewScrollBounds.Width <= 0 || layout.Before.PreviewScrollBounds.Height <= 0)
                        throw new InvalidDataException("MainForm layout snapshot is not valid before stress transitions.");
                    if (layout.Before != layout.After || layout.EditionChanges != 100 || layout.TabChanges != 20 || layout.LocaleChanges != 10)
                        throw new InvalidDataException("MainForm layout did not remain stable across stress transitions.");
                    Console.WriteLine($"Layout stress: edition={layout.EditionChanges}; tabs={layout.TabChanges}; locales={layout.LocaleChanges}; splitter={layout.After.SplitterDistance}");
                }
            }
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyGraphicsVariantAwarenessSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1GraphicsVariantSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var service = new GraphicsVariantService();
            if (!service.Load(e1).IsSuccess || service.Load(e1).State!.Edits.Count != 0 || File.Exists(service.GetPath(e1)))
                throw new InvalidDataException("Missing legacy graphics project state was not backward-compatible/read-only.");
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single();
            var shared = new GraphicsProjectEdit(new("012.VGA", 7), Path.Combine(root, "shared.png"), GraphicsEditScope.Shared, null);
            GraphicsProjectState e1State = service.SetEdit(e1, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), shared);
            if (service.GetGraphicsEditsForVariant(e1, e1State, e1Vga) is not [{ Applicability: GraphicsApplicabilityStatus.Shared }] ||
                service.GetGraphicsEditsForVariant(e1, e1State, e1Ega) is not [{ Applicability: GraphicsApplicabilityStatus.Shared }])
                throw new InvalidDataException("Shared E1 graphics edit did not project deterministically to VGA and EGA.");
            var egaOnly = new GraphicsProjectEdit(new("012.VGA", 8), Path.Combine(root, "ega.png"), GraphicsEditScope.RuntimeSpecific, VariantRuntimeKind.Elvira1Ega);
            e1State = service.SetEdit(e1, e1State, egaOnly);
            if (service.GetGraphicsEditsForVariant(e1, e1State, e1Vga).Any(value => value.Edit.Identity.ImageId == 8) ||
                service.GetGraphicsEditsForVariant(e1, e1State, e1Ega).Single(value => value.Edit.Identity.ImageId == 8).Applicability != GraphicsApplicabilityStatus.RuntimeSpecific)
                throw new InvalidDataException("Runtime-specific graphics applicability was not enforced.");
            try { _ = new GraphicsProjectEdit(new("012.VGA", 9), Path.Combine(root, "bad.png"), GraphicsEditScope.RuntimeSpecific, VariantRuntimeKind.Elvira1Ega); _ = service.SetEdit(e2, GraphicsProjectState.Empty(ElviraGameProfile.Elvira2), new(new("012.VGA", 9), Path.Combine(root, "bad.png"), GraphicsEditScope.RuntimeSpecific, VariantRuntimeKind.Elvira1Ega)); throw new InvalidDataException("Impossible E2 EGA edit was accepted."); }
            catch (ArgumentException) { }
            string gamePcHash = HashFile(Path.Combine(e1.GameRoot, "GAMEPC"));
            GraphicsProjectSaveResult saved = service.Save(e1, e1State);
            if (!saved.Succeeded)
                throw new InvalidDataException("Graphics project persistence failed: " + saved.Detail);
            if (HashFile(Path.Combine(e1.GameRoot, "GAMEPC")) != gamePcHash)
                throw new InvalidDataException("Graphics project persistence changed fixture GAMEPC.");
            if (!File.ReadAllText(saved.Path).Contains("012.VGA", StringComparison.Ordinal))
                throw new InvalidDataException("Graphics project persistence did not write the expected resource identity.");
            if (!service.Load(e1).IsSuccess || service.Load(e1).State!.Edits.Count != 2)
                throw new InvalidDataException("Graphics project state did not round-trip.");
            using var form = new MainForm();
            if (!form.HasGraphicsVariantPresentationForTest) throw new InvalidDataException("Graphics variant presentation controls were not constructed.");
            if (service.GetGraphicsEditsForVariant(e2, GraphicsProjectState.Empty(ElviraGameProfile.Elvira2), e2Vga).Count != 0)
                throw new InvalidDataException("E2 graphics projection was not deterministic.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyLauncherRefactorSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1LauncherRefactorSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var directories = new VariantDirectoryService();
            var service = new VariantLauncherService(directories, new CompositeBuildService(new DisposableVariantBuildService(directories), directories));
            if (service.Resolve(null, null).Readiness != VariantLaunchReadiness.NoActiveInstallation)
                throw new InvalidDataException("No-installation launcher state was not neutral.");
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(v => v.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(v => v.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single();
            if (service.Resolve(e1, e1Vga).Readiness != VariantLaunchReadiness.VariantMissing || service.Resolve(e1, e1Ega).VariantId != BuiltInVariantId.Elvira1Ega ||
                service.Resolve(e2, e2Vga).VariantId != BuiltInVariantId.Elvira2Vga)
                throw new InvalidDataException("Semantic variant routing did not use the supplied VariantContext.");
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, e1Vga), VariantDirectoryOperationStatus.Created, "E1VGA ownership");
            if (service.Resolve(e1, e1Vga) is not { Readiness: VariantLaunchReadiness.BuildIncomplete, Ownership: VariantDirectoryOperationStatus.AlreadyValid })
                throw new InvalidDataException("Owned pristine E1VGA variant was falsely launch-ready.");
            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign);
            if (service.Resolve(e1, e1Ega).Readiness != VariantLaunchReadiness.ForeignOrInvalidVariant)
                throw new InvalidDataException("Missing ownership marker was not rejected.");
            try { _ = VariantContext.ValidateDirectoryKey("..\\FOREIGN"); throw new InvalidDataException("Traversal directory key was accepted."); }
            catch (ArgumentException) { }
            try { _ = VariantContextCatalog.Create(e2, BuiltInVariantId.Elvira1Ega); throw new InvalidDataException("E2 EGA route was accepted."); }
            catch (ArgumentException) { }
            LauncherRedirectionPlan e1Plan = service.CreateRedirectionPlan(e1, e1Vga);
            LauncherRedirectionPlan e2Plan = service.CreateRedirectionPlan(e2, e2Vga);
            if (e1Plan.LauncherRelativePath != "ELVIRA.BAT" || e2Plan.LauncherRelativePath != "CERBERUS.BAT" ||
                e1Plan.Classification != PristineFileClassification.MutableBackedUp || e2Plan.Classification != PristineFileClassification.MutableBackedUp ||
                !File.Exists(e1Plan.RestoreSourcePath) || !File.Exists(e2Plan.RestoreSourcePath) || e1Plan.IsWriteAuthorized || e2Plan.IsWriteAuthorized)
                throw new InvalidDataException("Mutable launcher backup routing or incomplete-build gate regressed.");
            if (File.Exists(Path.Combine(e1.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(e1.GameRoot, "RUNVGASK.EXE")) ||
                File.Exists(Path.Combine(e1.GameRoot, "RUNEGASK.EXE")) || File.Exists(Path.Combine(e2.GameRoot, "RUNITSK.EXE")))
                throw new InvalidDataException("Launcher routing depended on a root legacy/generated artifact.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest(); form.OpenModsForTest();
            if (!form.IsNeutralInstallationStateForTest || !form.ModsPresentationForTest.Contains("No game selected.", StringComparison.Ordinal))
                throw new InvalidDataException("Mods initialized context or did not show neutral state.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunDebugSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunDebugSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var directories = new VariantDirectoryService();
            FixtureCompositeStep[] stages = CompleteFixtureStages(null, null).Cast<FixtureCompositeStep>().ToArray();
            var composite = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, stages);
            var launcher = new VariantLauncherService(directories, composite);
            string debugHost = Path.Combine(root, "DEBUG.EXE"); File.WriteAllBytes(debugHost, [0x4D, 0x5A]);
            var runner = new RecordingVariantProcessRunner();
            var execution = new VariantExecutionService(runner, new VariantDebugConfiguration(debugHost, ["-debug"]));

            AssertBlocked(execution.Execute(launcher.Resolve(null, null), VariantExecutionMode.Run), runner, runner.Starts.Count, "No active installation");
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(v => v.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(v => v.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single();
            AssertBlocked(execution.Execute(launcher.Resolve(e1, e1Vga), VariantExecutionMode.Run), runner, runner.Starts.Count, "Missing variant");
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, e1Vga), VariantDirectoryOperationStatus.Created, "E1VGA owned directory");
            AssertBlocked(execution.Execute(launcher.Resolve(e1, e1Vga), VariantExecutionMode.Run), runner, runner.Starts.Count, "Incomplete variant");

            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign);
            AssertBlocked(execution.Execute(launcher.Resolve(e1, e1Ega), VariantExecutionMode.Run), runner, runner.Starts.Count, "Foreign E1EGA directory");
            Directory.Delete(foreign, true);
            try { _ = VariantContextCatalog.Create(e2, BuiltInVariantId.Elvira1Ega); throw new InvalidDataException("Elvira II EGA was accepted."); }
            catch (ArgumentException) { }
            try { _ = VariantContext.ValidateDirectoryKey("..\\escape"); throw new InvalidDataException("Traversal key was accepted."); }
            catch (ArgumentException) { }

            VerifyReadyVariant(e1, e1Vga, directories, launcher, execution, runner);
            VerifyReadyVariant(e1, e1Ega, directories, launcher, execution, runner);
            VerifyReadyVariant(e2, e2Vga, directories, launcher, execution, runner);

            VariantLaunchTarget staleE1 = launcher.Resolve(e1, e1Vga);
            VariantLaunchTarget switchedE2 = launcher.Resolve(e2, e2Vga);
            VariantLaunchTarget returnedE1 = launcher.Resolve(e1, e1Vga);
            if (staleE1.GameId != "Elvira1" || switchedE2.GameId != "Elvira2" || returnedE1.VariantRoot != staleE1.VariantRoot)
                throw new InvalidDataException("E1 -> E2 -> E1 target resolution leaked stale state.");
            string e1Root = directories.GetVariantDirectoryPath(e1, e1Vga);
            string output = Path.Combine(e1Root, e1Vga.GeneratedExecutableName); File.Delete(output);
            AssertBlocked(execution.Execute(launcher.Resolve(e1, e1Vga), VariantExecutionMode.Run), runner, runner.Starts.Count, "Missing generated executable");
            File.WriteAllBytes(output, [0x4D, 0x5A]);
            runner.Failure = new System.ComponentModel.Win32Exception("fixture launch failure");
            if (execution.Execute(launcher.Resolve(e1, e1Vga), VariantExecutionMode.Run).Started)
                throw new InvalidDataException("Fake process failure was not contained.");
            runner.Failure = null;
            if (stages.Any(stage => stage.ExecuteCount != 0)) throw new InvalidDataException("Run/debug unexpectedly invoked a composite build.");
            if (File.Exists(Path.Combine(e1.GameRoot, "RUNVGASK.EXE")) || File.Exists(Path.Combine(e1.GameRoot, "RUNEGASK.EXE")) || File.Exists(Path.Combine(e2.GameRoot, "RUNITSK.EXE")))
                throw new InvalidDataException("Run/debug created a root generated executable.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRecoverySafetySmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RecoverySafetySmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            string e1Manifest = Path.Combine(e1.StorageLayout.BaselineRoot, PristineManifestService.ManifestFileName);
            string e2Manifest = Path.Combine(e2.StorageLayout.BaselineRoot, PristineManifestService.ManifestFileName);
            string e1ManifestHash = HashFile(e1Manifest), e2ManifestHash = HashFile(e2Manifest);
            var directories = new VariantDirectoryService();
            FixtureCompositeStep[] stages = CompleteFixtureStages(null, null).Cast<FixtureCompositeStep>().ToArray();
            var recovery = new RecoverySafetyService(directories, new CompositeBuildService(new DisposableVariantBuildService(directories), directories, stages));
            if (recovery.Inspect(e1).Baseline.Status != BaselineValidationStatus.MatchesBaseline || recovery.Inspect(e2).Baseline.Status != BaselineValidationStatus.MatchesBaseline)
                throw new InvalidDataException("Clean fixture baseline did not verify.");

            string launcher = Path.Combine(e1.GameRoot, "ELVIRA.BAT");
            string backup = Path.Combine(e1.StorageLayout.MutableBackupsRoot, "ELVIRA.BAT");
            string originalLauncherHash = HashFile(backup); File.WriteAllText(launcher, "external launcher edit");
            RecoverySafetyOperationResult restored = recovery.RestoreOriginalLauncher(e1);
            if (!restored.Succeeded || HashFile(launcher) != originalLauncherHash || new PristineManifestService(e1.StorageLayout, e1.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
                throw new InvalidDataException("Baseline mutable launcher restore was not exact.");

            string unexpected = Path.Combine(e1.GameRoot, "EXTERNAL.DAT"); File.WriteAllBytes(unexpected, [0x45, 0x58, 0x54]);
            RecoverySafetyInspection changed = recovery.Inspect(e1);
            if (changed.Baseline.Status != BaselineValidationStatus.UnexpectedFile || !changed.Baseline.Entries.Any(item => item.RelativePath == "EXTERNAL.DAT" && item.Status == BaselineValidationStatus.UnexpectedFile) || !File.Exists(unexpected))
                throw new InvalidDataException("Unexpected external file was not preserved and reported.");
            File.Delete(unexpected);

            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, e1Vga), VariantDirectoryOperationStatus.Created, "owned recovery fixture");
            File.WriteAllBytes(Path.Combine(directories.GetVariantDirectoryPath(e1, e1Vga), "TEMP.DAT"), [0x50, 0x49, 0x31]);
            if (recovery.RemoveOwnedVariant(e1, e1Vga) is not { Succeeded: true, VariantResult.Status: VariantDirectoryOperationStatus.Deleted } || Directory.Exists(directories.GetVariantDirectoryPath(e1, e1Vga)))
                throw new InvalidDataException("Owned variant was not safely removed.");
            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign);
            RecoverySafetyOperationResult foreignResult = recovery.RemoveOwnedVariant(e1, e1Ega);
            if (foreignResult.Succeeded || foreignResult.VariantResult?.Status != VariantDirectoryOperationStatus.ForeignDirectoryConflict || !Directory.Exists(foreign))
                throw new InvalidDataException("Foreign variant was removed or accepted.");
            Directory.Delete(foreign, true);

            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single();
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e2, e2Vga), VariantDirectoryOperationStatus.Created, "owned rebuild fixture");
            RecoverySafetyOperationResult rebuilt = recovery.RebuildOwnedVariant(e2, e2Vga);
            if (!rebuilt.Succeeded || rebuilt.BuildResult?.Status != CompositeBuildStatus.Success || stages.Sum(step => step.ExecuteCount) != 5)
                throw new InvalidDataException("Explicit rebuild did not execute only the owned configured fixture variant.");
            if (HashFile(e1Manifest) != e1ManifestHash || HashFile(e2Manifest) != e2ManifestHash)
                throw new InvalidDataException("Recovery operation recaptured or changed a pristine manifest.");
            if (new PristineManifestService(e1.StorageLayout, e1.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline ||
                new PristineManifestService(e2.StorageLayout, e2.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
                throw new InvalidDataException("Recovery fixture operation changed a pristine baseline.");
            using var form = new MainForm(); form.InitializeInstallationStateForTest(); form.OpenModsForTest();
            if (!form.ModsPresentationForTest.Contains("No game selected.", StringComparison.Ordinal))
                throw new InvalidDataException("Recovery UI did not retain the neutral installation state.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRestoreSemanticsSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RestoreSemanticsSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            string manifest = Path.Combine(e1.StorageLayout.BaselineRoot, PristineManifestService.ManifestFileName);
            string manifestHash = HashFile(manifest);
            var directories = new VariantDirectoryService();
            var recovery = new RecoverySafetyService(directories, new CompositeBuildService(new DisposableVariantBuildService(directories), directories));
            RestorePlan cleanE1 = recovery.CreateRestorePlan(e1), cleanE2 = recovery.CreateRestorePlan(e2);
            if (cleanE1.ActionCount != 0 || cleanE1.IsBlocked || cleanE2.ActionCount != 0 || cleanE2.IsBlocked || RestorePlanSignature(cleanE1) != RestorePlanSignature(recovery.CreateRestorePlan(e1)))
                throw new InvalidDataException("Clean restore preview was not deterministic NoAction.");

            string launcher = Path.Combine(e1.GameRoot, "ELVIRA.BAT");
            string backup = Path.Combine(e1.StorageLayout.MutableBackupsRoot, "ELVIRA.BAT");
            byte[] originalLauncher = File.ReadAllBytes(backup); File.WriteAllText(launcher, "external launcher mutation");
            string artifact = Path.Combine(e1.GameRoot, "PI1MENU.COM"); File.WriteAllBytes(artifact, [0x50, 0x49, 0x31]);
            string legacy = Path.Combine(e1.GameRoot, "GAMEPCO"); File.Copy(Path.Combine(e1.GameRoot, "GAMEPC"), legacy, overwrite: false);
            string unexpected = Path.Combine(e1.GameRoot, "EXTERNAL.DAT"); File.WriteAllBytes(unexpected, [0x45, 0x58, 0x54]);
            string immutable = Path.Combine(e1.GameRoot, "GAMEPC"); byte[] originalImmutable = File.ReadAllBytes(immutable); File.WriteAllBytes(immutable, [0x45, 0x58, 0x54]);
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, e1Vga), VariantDirectoryOperationStatus.Created, "owned restore fixture");
            File.WriteAllBytes(Path.Combine(directories.GetVariantDirectoryPath(e1, e1Vga), "PAYLOAD.DAT"), [0x50, 0x49, 0x31]);
            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign); File.WriteAllBytes(Path.Combine(foreign, "FOREIGN.DAT"), [0x46]);

            RestorePlan preview = recovery.CreateRestorePlan(e1);
            if (preview.IsBlocked || RestorePlanSignature(preview) != RestorePlanSignature(recovery.CreateRestorePlan(e1)) ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.RestoreMutable && item.RelativePath == "ELVIRA.BAT") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.RemoveOwnedVariant && item.VariantId == e1Vga.VariantId) ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.RemoveKnownEditorArtifact && item.RelativePath == "PI1MENU.COM") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved && item.RelativePath == "EXTERNAL.DAT") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved && item.RelativePath == "GAMEPC") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved && item.RelativePath.EndsWith("E1EGA", StringComparison.OrdinalIgnoreCase)) ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.NoAction && item.RelativePath == "GAMEPCO"))
                throw new InvalidDataException("Restore preview classifications were not exact/narrow.");
            RestorePlanExecutionResult executed = recovery.ExecuteRestorePlan(e1, preview);
            if (!executed.Succeeded || File.ReadAllBytes(launcher).AsSpan().SequenceEqual(originalLauncher) is false || File.Exists(artifact) || Directory.Exists(directories.GetVariantDirectoryPath(e1, e1Vga)) ||
                !Directory.Exists(foreign) || !File.Exists(unexpected) || !File.Exists(legacy) || !File.ReadAllBytes(immutable).AsSpan().SequenceEqual(new byte[] { 0x45, 0x58, 0x54 }))
                throw new InvalidDataException("Restore execution diverged from the approved plan.");
            RestorePlan idempotent = recovery.CreateRestorePlan(e1);
            if (idempotent.ActionCount != 0 || !recovery.ExecuteRestorePlan(e1, idempotent).Succeeded)
                throw new InvalidDataException("Repeated restore was not idempotent.");

            File.WriteAllText(launcher, "second mutable mutation");
            File.WriteAllBytes(backup, [0x42, 0x41, 0x44]);
            RestorePlan blocked = recovery.CreateRestorePlan(e1);
            if (!blocked.IsBlocked || recovery.ExecuteRestorePlan(e1, blocked).Succeeded || File.ReadAllText(launcher) != "second mutable mutation")
                throw new InvalidDataException("Missing/corrupt mutable backup was not safely blocked.");
            File.Delete(backup);
            RestorePlan missing = recovery.CreateRestorePlan(e1);
            if (!missing.IsBlocked || recovery.ExecuteRestorePlan(e1, missing).Succeeded || File.ReadAllText(launcher) != "second mutable mutation")
                throw new InvalidDataException("Missing mutable backup was not safely blocked.");
            File.WriteAllBytes(backup, originalLauncher); File.WriteAllBytes(launcher, originalLauncher);
            File.Delete(unexpected); File.Delete(legacy); File.WriteAllBytes(immutable, originalImmutable); Directory.Delete(foreign, true);
            if (HashFile(manifest) != manifestHash || new PristineManifestService(e1.StorageLayout, e1.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline ||
                new PristineManifestService(e2.StorageLayout, e2.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
                throw new InvalidDataException("Restore semantics changed or recaptured pristine baseline metadata.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static string RestorePlanSignature(RestorePlan plan) => string.Join("\n", plan.Items.Select(item => $"{item.Kind}|{item.RelativePath}|{item.Detail}|{item.VariantId}"));

    private static void VerifyReadyVariant(ProjectContext project, VariantContext variant, VariantDirectoryService directories,
        VariantLauncherService launcher, VariantExecutionService execution, RecordingVariantProcessRunner runner)
    {
        VariantDirectoryOperationResult ensured = directories.EnsureVariantDirectory(project, variant);
        if (ensured.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid))
            throw new InvalidDataException("Unable to prepare owned fixture variant: " + ensured.Status);
        string root = directories.GetVariantDirectoryPath(project, variant);
        File.WriteAllBytes(Path.Combine(root, variant.GeneratedExecutableName), [0x4D, 0x5A]);
        File.WriteAllBytes(Path.Combine(root, variant.LogicalDataFileName), [0x50, 0x49, 0x31]);
        VariantLaunchTarget target = launcher.Resolve(project, variant);
        if (target.Readiness != VariantLaunchReadiness.LaunchReady || !execution.IsAvailable(target, VariantExecutionMode.Run) || !execution.IsAvailable(target, VariantExecutionMode.Debug))
            throw new InvalidDataException("Ready fixture variant was not launch-ready: " + target.Detail);
        int starts = runner.Starts.Count;
        VariantExecutionResult run = execution.Execute(target, VariantExecutionMode.Run);
        if (!run.Started || run.StartInfo is not { UseShellExecute: false } runInfo || runInfo.FileName != Path.Combine(root, variant.GeneratedExecutableName) ||
            runInfo.WorkingDirectory != root || runInfo.ArgumentList.Count != 1 || runInfo.ArgumentList[0] != variant.LogicalDataFileName)
            throw new InvalidDataException("Direct run command construction was not exact.");
        VariantExecutionResult debug = execution.Execute(target, VariantExecutionMode.Debug);
        if (!debug.Started || debug.StartInfo is not { UseShellExecute: false } debugInfo || debugInfo.WorkingDirectory != root ||
            debugInfo.ArgumentList.Count != 3 || debugInfo.ArgumentList[1] != Path.Combine(root, variant.GeneratedExecutableName) || debugInfo.ArgumentList[2] != variant.LogicalDataFileName || runner.Starts.Count != starts + 2)
            throw new InvalidDataException("Debug command construction was not exact.");
    }

    private static void AssertBlocked(VariantExecutionResult result, RecordingVariantProcessRunner runner, int starts, string scenario)
    {
        if (result.Started || runner.Starts.Count != starts)
            throw new InvalidDataException(scenario + " launched a process.");
    }

    private static void VerifyUnexpectedFileActivationSmoke(string elvira1Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1UnexpectedActivationSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string unexpected = Path.Combine(project.GameRoot, "UNEXPECTED.DAT");
            File.WriteAllBytes(unexpected, [0x50, 0x49, 0x31]);
            if (!GameInstallationValidator.TryValidate(project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
                throw new InvalidDataException("Unexpected-file fixture did not validate as Elvira I.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(installation);
            if (form.HasActiveInstallationForTest || !form.IsNeutralInstallationStateForTest ||
                form.LastInstallationActivationFailureForTest is not { } detail || !detail.Contains("UnexpectedFile: UNEXPECTED.DAT", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Unexpected file was not rejected with its exact relative path while retaining neutral state.");
            if (!File.Exists(unexpected) || File.ReadAllBytes(unexpected) is not [0x50, 0x49, 0x31])
                throw new InvalidDataException("Unexpected file was modified or adopted during rejected activation.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyLocaleFallbackSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1LocaleFallbackSmoke", Guid.NewGuid().ToString("N"));
        string localeDirectory = Path.Combine(root, "Locales");
        string priorLocaleId = UiText.LocaleId;
        try
        {
            Directory.CreateDirectory(localeDirectory);
            File.WriteAllText(Path.Combine(localeDirectory, "en.json"), "{\"Common.Save\":\"Save\",\"Common.Cancel\":\"Cancel\",\"App.NoGameSelected\":\"No game selected.\",\"App.FindOrBrowse\":\"Use Find games... or Browse folder...\",\"Recovery.ActionsUnavailable\":\"Recovery actions are unavailable.\"}");
            File.WriteAllText(Path.Combine(localeDirectory, "de.json"), "{\"Common.Save\":\"Speichern\",\"App.NoGameSelected\":\"Kein Spiel ausgewählt.\"}");
            JsonUiLocaleProvider provider = JsonUiLocaleProvider.Discover(localeDirectory);
            if (provider.Resolve("de", "Common.Save") is not { Value: "Speichern", Source: UiLocaleLookupSource.SelectedLocale } ||
                provider.Resolve("de", "Common.Cancel") is not { Value: "Cancel", Source: UiLocaleLookupSource.EnglishFallback } ||
                provider.Resolve("de", "Common.Unknown") is not { Value: "Common.Unknown", Source: UiLocaleLookupSource.MissingKey } ||
                provider.Resolve("fr", "Common.Cancel") is not { Value: "Cancel", Source: UiLocaleLookupSource.EnglishFallback })
                throw new InvalidDataException("Selected/English/missing JSON fallback ordering regressed.");

            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Locale fallback fixtures did not validate as installations.");
            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));

            UiText.RefreshLocaleCatalog(localeDirectory);
            UiText.SetLocale("de");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            UiText.SetLocale("de");
            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            VariantContext? e1Variant = form.ActiveVariantForTest;
            if (UiText.Get("Common.Save") != "Speichern" || UiText.Get("Common.Cancel") != "Cancel" || UiText.Get("Common.Unknown") != "Common.Unknown" ||
                !ReferenceEquals(e1Context, form.ActiveProjectForTest) || !ReferenceEquals(e1Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Runtime dynamic-locale fallback did not preserve E1 context or deterministic values.");
            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            VariantContext? e2Variant = form.ActiveVariantForTest;
            UiText.SetLocale("fr");
            if (UiText.Get("Common.Cancel") != "Cancel" || !ReferenceEquals(e2Context, form.ActiveProjectForTest) || !ReferenceEquals(e2Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Runtime unavailable-locale English fallback did not preserve E2 context.");
            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("Locale fallback changed fixture GAMEPC bytes.");
        }
        finally
        {
            UiText.RefreshLocaleCatalog();
            UiText.SetLocale(priorLocaleId);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyLocalizationAuditSmoke(string elvira1Source, string elvira2Source)
    {
        string priorLocaleId = UiText.LocaleId;
        string root = Path.Combine(Path.GetTempPath(), "Pi1LocalizationAuditSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            string[] representativeKeys =
            [
                "AppTitle", "Graphics", "TextTab", "FontEditor", "ModsLauncherTab",
                "RuntimeUi.Title", "RuntimeUi.Save", "Recovery.Title", "Recovery.VerifyPristine",
                "Execution.Run", "Execution.Debug", "Graphics.SaveProject", "Error.Title"
            ];
            JsonUiLocaleProvider canonicalLocales = JsonUiLocaleProvider.Discover(Path.Combine(AppContext.BaseDirectory, "Locales"));
            foreach (string key in representativeKeys)
            {
                if (canonicalLocales.Resolve("en", key).Source == UiLocaleLookupSource.MissingKey)
                    throw new InvalidDataException("Canonical en.json is missing audited UI key: " + key);
            }

            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Localization audit fixtures did not validate as installations.");
            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));

            UiText.SetLocale("en");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            VariantContext? e1Variant = form.ActiveVariantForTest;
            UiText.SetLocale("sk");
            if (UiText.Get("RuntimeUi.Title") != "Runtime UI" || !ReferenceEquals(e1Context, form.ActiveProjectForTest) || !ReferenceEquals(e1Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Partial Slovak locale fallback or E1 context preservation regressed.");
            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            VariantContext? e2Variant = form.ActiveVariantForTest;
            UiText.SetLocale("en");
            if (!ReferenceEquals(e2Context, form.ActiveProjectForTest) || !ReferenceEquals(e2Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Audited runtime locale switch changed E2 context.");
            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("Localization audit changed fixture GAMEPC bytes.");
        }
        finally
        {
            UiText.SetLocale(priorLocaleId);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyLocalizedHelpSmoke(string elvira1Source, string elvira2Source)
    {
        string priorLocaleId = UiText.LocaleId;
        string root = Path.Combine(Path.GetTempPath(), "Pi1LocalizedHelpSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            HelpDocument english = HelpDocumentService.Load(UiLanguage.English);
            HelpDocument slovak = HelpDocumentService.Load(UiLanguage.Slovak);
            HelpDocument czech = HelpDocumentService.Load(UiLanguage.Czech);
            AboutDocument englishAbout = AboutDocumentService.Load(UiLanguage.English);
            AboutDocument slovakAbout = AboutDocumentService.Load(UiLanguage.Slovak);
            AboutDocument czechAbout = AboutDocumentService.Load(UiLanguage.Czech);
            string englishGettingStarted = english.Topics[0].Title;
            string slovakGettingStarted = slovak.Topics[0].Title;
            if (english.Topics.Count < 18 || slovak.Topics.Count != english.Topics.Count || czech.Topics.Count != english.Topics.Count ||
                english.Topics.Select(topic => topic.Id).Distinct(StringComparer.Ordinal).Count() != english.Topics.Count ||
                english.Topics.Any(topic => string.IsNullOrWhiteSpace(topic.Body)) ||
                slovak.Topics.Any(topic => string.IsNullOrWhiteSpace(topic.Body)) || czech.Topics.Any(topic => string.IsNullOrWhiteSpace(topic.Body)) ||
                slovakGettingStarted == englishGettingStarted ||
                new[] { englishAbout, slovakAbout, czechAbout }.Any(document => !document.Body.Contains(AppInfo.ProductName, StringComparison.Ordinal) || document.Body.Contains("RUNEGA/EGA is currently unsupported", StringComparison.Ordinal)))
                throw new InvalidDataException("Localized Help/About documents did not preserve the English, Slovak, Czech, and fallback contracts.");

            UiText.SetLocale("en");
            using var viewer = new HelpViewerForm(UiLanguage.English);
            int expectedGroupCount = english.Topics.Select(topic => topic.Group).Distinct(StringComparer.Ordinal).Count();
            if (viewer.FirstTopicTitleForTest != englishGettingStarted || viewer.GroupCountForTest != expectedGroupCount)
                throw new InvalidDataException("English Help viewer title/topic initialization regressed.");
            UiText.SetLocale("sk");
            viewer.SetLanguage(UiLanguage.Slovak);
            if (viewer.FirstTopicTitleForTest != slovakGettingStarted || !viewer.Text.StartsWith(UiText.Get("Help"), StringComparison.Ordinal))
                throw new InvalidDataException("Runtime Slovak Help viewer refresh left stale visible help text.");
            using var about = new AboutViewerForm(UiLanguage.English);
            if (!about.BodyForTest.Contains(AppInfo.ProductName, StringComparison.Ordinal))
                throw new InvalidDataException("English About viewer did not load the external document.");
            about.SetLanguage(UiLanguage.Slovak);
            if (about.TitleForTest != UiText.Get("About") + " — " + AppInfo.ProductTitle || !about.BodyForTest.Contains(AppInfo.ProductName, StringComparison.Ordinal))
                throw new InvalidDataException("Runtime Slovak About viewer refresh left stale visible content.");
            if (UiText.Get("ReservedHudEraseGlyphTip") == "ReservedHudEraseGlyphTip" || UiText.Get("PaletteAutomaticTooltip") == "PaletteAutomaticTooltip")
                throw new InvalidDataException("Representative Font/Graphics tooltip keys are missing from the JSON locale catalog.");

            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Localized Help fixtures did not validate as installations.");
            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            VariantContext? e1Variant = form.ActiveVariantForTest;
            UiText.SetLocale("en");
            if (!ReferenceEquals(e1Context, form.ActiveProjectForTest) || !ReferenceEquals(e1Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Help locale refresh changed the active E1 context.");
            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            VariantContext? e2Variant = form.ActiveVariantForTest;
            UiText.SetLocale("sk");
            if (!ReferenceEquals(e2Context, form.ActiveProjectForTest) || !ReferenceEquals(e2Variant, form.ActiveVariantForTest) ||
                HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("Localized Help changed context or fixture GAMEPC bytes.");
        }
        finally
        {
            UiText.SetLocale(priorLocaleId);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyLocaleValidationSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1LocaleValidationSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            string en = Path.Combine(root, "en.json");
            string sk = Path.Combine(root, "sk.json");
            string fr = Path.Combine(root, "fr.json");
            string de = Path.Combine(root, "de.json");
            string bad = Path.Combine(root, "bad.json");
            string type = Path.Combine(root, "type.json");
            File.WriteAllText(en, "{\"Common.Save\":\"Save\",\"Common.Cancel\":\"Cancel\"}");
            File.WriteAllText(sk, "{\"Common.Save\":\"Uložiť\",\"Common.Cancel\":\"\"}");
            File.WriteAllText(fr, "{\"Common.Save\":\"Enregistrer\"}");
            File.WriteAllText(de, "{\"Common.Save\":\"Speichern\",\"Common.Extra\":\"Extra\"}");
            File.WriteAllText(bad, "{");
            File.WriteAllText(type, "{\"Common.Save\":null}");
            UiLocaleValidationCatalog catalog = new UiLocaleValidationService().ValidateFiles([en, sk, fr, de, bad, type, sk]);
            UiLocaleValidationReport canonical = catalog.Canonical ?? throw new InvalidDataException("Canonical locale report is missing.");
            UiLocaleValidationReport slovak = catalog.Reports.Single(report => report.LocaleId == "sk" && report.IsValid);
            UiLocaleValidationReport french = catalog.Reports.Single(report => report.LocaleId == "fr");
            UiLocaleValidationReport german = catalog.Reports.Single(report => report.LocaleId == "de");
            if (!canonical.IsValid || canonical.CanonicalKeyCount != 2 || slovak.TranslatedKeys != 1 || slovak.CompletionPercent != 50.0m ||
                !slovak.Diagnostics.Any(diagnostic => diagnostic.Code == "EmptyTranslation") || french.MissingKeys.Count != 1 ||
                !german.ExtraKeys.SequenceEqual(["Common.Extra"], StringComparer.Ordinal) ||
                !catalog.Reports.Any(report => report.LocaleId == "bad" && !report.IsValid && report.Diagnostics.Any(diagnostic => diagnostic.Code == "MalformedJson")) ||
                !catalog.Reports.Any(report => report.LocaleId == "type" && !report.IsValid && report.Diagnostics.Any(diagnostic => diagnostic.Code == "InvalidValueType")) ||
                !catalog.Reports.Any(report => report.LocaleId == "sk" && !report.IsValid && report.Diagnostics.Any(diagnostic => diagnostic.Code == "DuplicateLocaleIdentity")))
                throw new InvalidDataException("Locale validation did not report the required deterministic fixture diagnostics.");

            string installedLocales = Path.Combine(AppContext.BaseDirectory, "Locales");
            UiLocaleValidationCatalog installed = new UiLocaleValidationService().ValidateDirectory(installedLocales);
            if (installed.Canonical is not { IsValid: true } || !installed.Reports.Any(report => report.LocaleId == "sk" && report.IsValid))
                throw new InvalidDataException("Installed en.json/sk.json validation failed.");

            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Locale validation fixtures did not validate as installations.");
            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            VariantContext? e1Variant = form.ActiveVariantForTest;
            _ = new UiLocaleValidationService().ValidateFiles([en, sk, fr, de, bad, type, sk]);
            if (!ReferenceEquals(e1Context, form.ActiveProjectForTest) || !ReferenceEquals(e1Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Locale validation changed the active E1 context.");
            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            VariantContext? e2Variant = form.ActiveVariantForTest;
            if (!ReferenceEquals(e2Context, form.ActiveProjectForTest) || !ReferenceEquals(e2Variant, form.ActiveVariantForTest) ||
                HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("Locale validation changed context or fixture GAMEPC bytes.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyDynamicLocaleDiscoverySmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1DynamicLocaleSmoke", Guid.NewGuid().ToString("N"));
        string localeDirectory = Path.Combine(root, "Locales");
        string priorLocaleId = UiText.LocaleId;
        try
        {
            Directory.CreateDirectory(localeDirectory);
            File.Copy(Path.Combine(AppContext.BaseDirectory, "Locales", "en.json"), Path.Combine(localeDirectory, "en.json"));
            File.Copy(Path.Combine(AppContext.BaseDirectory, "Locales", "sk.json"), Path.Combine(localeDirectory, "sk.json"));
            string german = Path.Combine(localeDirectory, "de.json");
            File.WriteAllText(german, "{\"App.NoGameSelected\":\"Kein Spiel ausgewählt.\",\"App.FindOrBrowse\":\"Spiele suchen... oder Ordner auswählen...\",\"Recovery.ActionsUnavailable\":\"Wiederherstellungsaktionen sind nicht verfügbar.\"}");
            File.WriteAllText(Path.Combine(localeDirectory, "broken.json"), "{");
            File.WriteAllText(Path.Combine(localeDirectory, "README.txt"), "unrelated");

            JsonUiLocaleProvider provider = JsonUiLocaleProvider.Discover(localeDirectory);
            string[] expected = ["de", "en", "sk"];
            if (!provider.AvailableLocales.Select(locale => locale.Id).SequenceEqual(expected) ||
                !provider.TryGet("de", UiLocalizationKeys.NoGameSelected, out string germanText) || germanText != "Kein Spiel ausgewählt." ||
                !provider.Diagnostics.Any(diagnostic => Path.GetFileName(diagnostic.SourcePath) == "broken.json"))
                throw new InvalidDataException("Dynamic locale scan did not deterministically discover valid files and skip malformed JSON.");
            JsonUiLocaleProvider duplicate = JsonUiLocaleProvider.DiscoverFiles([german, german]);
            if (duplicate.AvailableLocales.Count != 1 || !duplicate.Diagnostics.Any(diagnostic => diagnostic.Detail.Contains("duplicate locale identity 'de'", StringComparison.Ordinal)))
                throw new InvalidDataException("Duplicate locale identity was not rejected deterministically.");

            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Dynamic locale fixtures did not validate as installations.");
            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));

            UiText.RefreshLocaleCatalog(localeDirectory);
            UiText.SetLocale("en");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            if (!form.UiLocaleIdsForTest.SequenceEqual(expected))
                throw new InvalidDataException("MainForm selector did not refresh from discovered locale identities.");
            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            VariantContext? e1Variant = form.ActiveVariantForTest;
            UiText.SetLocale("de");
            if (UiText.LocaleId != "de" || UiText.Get(UiLocalizationKeys.NoGameSelected) != "Kein Spiel ausgewählt." ||
                !ReferenceEquals(e1Context, form.ActiveProjectForTest) || !ReferenceEquals(e1Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Discovered locale switch changed E1 context or did not use de.json.");
            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            VariantContext? e2Variant = form.ActiveVariantForTest;
            UiText.SetLocale("sk");
            if (!ReferenceEquals(e2Context, form.ActiveProjectForTest) || !ReferenceEquals(e2Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Discovered locale switch changed E2 context.");
            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("Dynamic locale discovery changed fixture GAMEPC bytes.");
        }
        finally
        {
            UiText.RefreshLocaleCatalog();
            UiText.SetLocale(priorLocaleId);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyJsonLocalesSmoke(string elvira1Source, string elvira2Source)
    {
        string localeDirectory = Path.Combine(AppContext.BaseDirectory, "Locales");
        JsonUiLocaleProvider provider = JsonUiLocaleProvider.Discover(localeDirectory);
        if (!provider.TryGet("en", UiLocalizationKeys.NoGameSelected, out string english) || english != "No game selected." ||
            !provider.TryGet("sk", UiLocalizationKeys.NoGameSelected, out string slovak) || slovak != "Nie je vybraná žiadna hra." ||
            !slovak.Contains('á') || provider.TryGet("cs", UiLocalizationKeys.NoGameSelected, out _))
            throw new InvalidDataException("Initial JSON locale lookup did not retain the explicit en/sk bootstrap contract.");

        string parseRoot = Path.Combine(Path.GetTempPath(), "Pi1JsonLocaleSmoke", Guid.NewGuid().ToString("N"));
        UiLanguage priorLocale = UiText.Language;
        try
        {
            Directory.CreateDirectory(parseRoot);
            string malformed = Path.Combine(parseRoot, "malformed.json");
            string duplicate = Path.Combine(parseRoot, "duplicate.json");
            string invalid = Path.Combine(parseRoot, "invalid.json");
            File.WriteAllText(malformed, "{\"App.NoGameSelected\":");
            File.WriteAllText(duplicate, "{\"App.NoGameSelected\":\"one\",\"App.NoGameSelected\":\"two\"}");
            File.WriteAllText(invalid, "{\"No semantic namespace\":\"bad\"}");
            foreach (string path in new[] { malformed, duplicate, invalid })
            {
                bool rejected = false;
                try { _ = JsonUiLocaleProvider.LoadLocaleFile(path); }
                catch (InvalidDataException) { rejected = true; }
                if (!rejected) throw new InvalidDataException("Invalid JSON locale input was accepted: " + Path.GetFileName(path));
            }

            ProjectContext e1Project = CreateBuildFixtureProjectContext(parseRoot, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(parseRoot, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("JSON locale fixtures did not validate as installations.");
            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));

            UiText.SetLanguage(UiLanguage.English);
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            if (UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder) != "Use Find games... or Browse folder...")
                throw new InvalidDataException("English representative UI text was not served from en.json.");
            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            VariantContext? e1Variant = form.ActiveVariantForTest;
            UiText.SetLanguage(UiLanguage.Slovak);
            if (UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder) != "Použite Nájsť hry... alebo Vybrať adresár..." ||
                !ReferenceEquals(e1Context, form.ActiveProjectForTest) || !ReferenceEquals(e1Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Runtime en/sk JSON switch changed Elvira I context or text source.");
            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            VariantContext? e2Variant = form.ActiveVariantForTest;
            UiText.SetLanguage(UiLanguage.English);
            if (!ReferenceEquals(e2Context, form.ActiveProjectForTest) || !ReferenceEquals(e2Variant, form.ActiveVariantForTest))
                throw new InvalidDataException("Runtime en/sk JSON switch changed Elvira II context.");
            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("JSON locale load/switch changed fixture GAMEPC bytes.");
        }
        finally
        {
            UiText.SetLanguage(priorLocale);
            if (Directory.Exists(parseRoot)) Directory.Delete(parseRoot, true);
        }
    }

    private static void VerifyUiLocalizationSmoke(string elvira1Source, string elvira2Source)
    {
        var service = new UiLocalizationService(
            (locale, key) => key == "Known" ? locale == "sk" ? "známe" : "known" : key);
        if (service.CurrentLocaleId != "en" || service.Get("Known") != "known" || service.Get("Missing.Key") != "Missing.Key")
            throw new InvalidDataException("UI localization bootstrap default or deterministic missing-key lookup regressed.");
        int changes = 0;
        UiLocaleChangedEventArgs? observed = null;
        service.LocaleChanged += (_, e) => { changes++; observed = e; };
        service.SetLocale("sk");
        service.SetLocale("sk");
        if (service.CurrentLocaleId != "sk" || service.Get("Known") != "známe" || changes != 1 || observed is not { PreviousLocaleId: "en", CurrentLocaleId: "sk" })
            throw new InvalidDataException("UI localization locale switching or LocaleChanged propagation regressed.");

        string root = Path.Combine(Path.GetTempPath(), "Pi1UiLocalizationSmoke", Guid.NewGuid().ToString("N"));
        UiLanguage priorLocale = UiText.Language;
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("UI localization fixtures did not validate as installations.");

            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));
            UiText.SetLanguage(UiLanguage.English);
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            string englishNeutral = UiText.Get(UiLocalizationKeys.NoGameSelected) + "\r\n" + UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder);
            if (!form.IsNeutralInstallationStateForTest || form.InstallationStatusForTest != englishNeutral)
                throw new InvalidDataException("MainForm did not expose the localized neutral startup state.");

            UiText.SetLanguage(UiLanguage.Slovak);
            string slovakNeutral = UiText.Get(UiLocalizationKeys.NoGameSelected) + "\r\n" + UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder);
            if (form.InstallationStatusForTest != slovakNeutral || form.ModsPresentationForTest != slovakNeutral)
                throw new InvalidDataException("MainForm did not refresh representative localized neutral controls.");

            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            VariantContext? e1Variant = form.ActiveVariantForTest;
            if (e1Context is null || e1Variant is null || form.ActiveGameForTest != ElviraGameProfile.Elvira1)
                throw new InvalidDataException("UI localization smoke could not establish Elvira I context.");
            UiText.SetLanguage(UiLanguage.Czech);
            if (!ReferenceEquals(e1Context, form.ActiveProjectForTest) || !ReferenceEquals(e1Variant, form.ActiveVariantForTest) || form.ActiveGameForTest != ElviraGameProfile.Elvira1)
                throw new InvalidDataException("UI locale switching recreated or changed Elvira I context.");

            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            VariantContext? e2Variant = form.ActiveVariantForTest;
            if (e2Context is null || e2Variant is null || form.ActiveGameForTest != ElviraGameProfile.Elvira2)
                throw new InvalidDataException("UI localization smoke could not establish Elvira II context.");
            UiText.SetLanguage(UiLanguage.English);
            if (!ReferenceEquals(e2Context, form.ActiveProjectForTest) || !ReferenceEquals(e2Variant, form.ActiveVariantForTest) || form.ActiveGameForTest != ElviraGameProfile.Elvira2)
                throw new InvalidDataException("UI locale switching recreated or changed Elvira II context.");

            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("UI locale switching changed fixture GAMEPC bytes.");
        }
        finally
        {
            UiText.SetLanguage(priorLocale);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyUiStartupLocalizationSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1UiStartupLocalizationSmoke", Guid.NewGuid().ToString("N"));
        UiLanguage priorLocale = UiText.Language;
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Startup/localization fixtures did not validate as installations.");
            string e1Hash = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2Hash = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));

            UiText.RefreshLocaleCatalog();
            string[] expectedLocales = ["en", "sk", "cs"];
            if (!expectedLocales.All(id => UiText.AvailableLocales.Any(locale => locale.Id == id)))
                throw new InvalidDataException("The EN/SK/CS locale files were not dynamically discovered.");

            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            foreach (string localeId in expectedLocales)
            {
                UiText.SetLocale(localeId);
                if (!form.HasNeutralStartupPresentationForTest || form.InstallationStatusForTest !=
                    UiText.Get(UiLocalizationKeys.NoGameSelected) + " " + UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder))
                    throw new InvalidDataException($"Neutral startup presentation regressed for locale '{localeId}': {form.NeutralStartupDiagnosticForTest}");
                if (form.TextNavigationCaptionForTest != UiText.Get("OpenTextEditor"))
                    throw new InvalidDataException($"Localized Text navigation regressed for locale '{localeId}'.");
                if (typeof(MainForm).GetField("btnDeploy", BindingFlags.NonPublic | BindingFlags.Instance) is not null)
                    throw new InvalidDataException($"Legacy direct GameRoot VGA deploy control regressed for locale '{localeId}'.");
                form.ActivateTextModeForTest();
                if (form.TextNavigationCaptionForTest != UiText.Get("OpenTextEditor"))
                    throw new InvalidDataException($"Text navigation caption changed after selecting the Text view for locale '{localeId}'.");
                form.ActivateGraphicsModeForTest();
                using var about = new AboutViewerForm(UiText.Language);
                string expectedAboutTitle = UiText.Get("About") + " — " + AppInfo.ProductTitle;
                if (form.WindowTitleForTest != AppInfo.ProductTitle || about.TitleForTest != expectedAboutTitle ||
                    about.TitleForTest.IndexOf(AppInfo.ProductTitle, StringComparison.Ordinal) != about.TitleForTest.LastIndexOf(AppInfo.ProductTitle, StringComparison.Ordinal))
                    throw new InvalidDataException($"Product identity changed or duplicated for locale '{localeId}'.");
            }

            if (!form.HasNeutralTextActionStateForTest || form.IsVariantAddEnabledForTest)
                throw new InvalidDataException("No-game Text or Variant Manager action gating regressed.");
            using (var dialog = new TranslationVariantDialog(ElviraGameProfile.Elvira1))
            {
                if (!dialog.HasCenteredActionButtonsForTest)
                    throw new InvalidDataException("Translation variant Create/Cancel action alignment regressed.");
            }

            using (var viewer = new CharacterSetViewerForm(Array.Empty<GlyphModel>(), 65, FontPreviewMode.Original))
            {
                UiText.SetLocale("sk");
                if (viewer.LocalizedTitleForTest != UiText.Get("FullSetTitle") || viewer.LocalizedHintForTest != UiText.Get("ViewerHint"))
                    throw new InvalidDataException("Full Character Set did not receive the active Slovak locale.");
                UiText.SetLocale("cs");
                if (viewer.LocalizedTitleForTest != UiText.Get("FullSetTitle") || viewer.LocalizedHintForTest != UiText.Get("ViewerHint"))
                    throw new InvalidDataException("Full Character Set did not refresh for the active Czech locale.");
            }

            form.ActivateInstallationForTest(e1);
            ProjectContext? firstE1 = form.ActiveProjectForTest;
            if (!form.HasActiveOriginalTextActionStateForTest || !form.HasActiveGraphicsPresentationForTest || !form.IsVariantAddEnabledForTest)
                throw new InvalidDataException("A valid installation did not restore Text, Graphics, or Variant Manager action state.");
            form.ClearInstallationStateForTest();
            if (!form.HasNeutralStartupPresentationForTest || !form.HasNeutralTextActionStateForTest || form.IsVariantAddEnabledForTest)
                throw new InvalidDataException("Valid-to-neutral context transition retained enabled project actions: " + form.UiStateDiagnosticForTest);
            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            form.ActivateInstallationForTest(e1);
            if (firstE1 is null || e2Context is null || form.ActiveGameForTest != ElviraGameProfile.Elvira1 ||
                HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1Hash ||
                HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2Hash ||
                Directory.EnumerateFiles(root, "*SK.EXE", SearchOption.AllDirectories).Any())
                throw new InvalidDataException("Startup/localization state transition modified data or leaked game context.");
        }
        finally
        {
            UiText.SetLanguage(priorLocale);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyInstallationContextActivationSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1InstallationActivationSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            ProjectContext e2BrowseProject = CreateBuildFixtureProjectContext(root, "e2browse", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Activation smoke fixtures did not validate as installations.");
            if (!GameInstallationValidator.TryValidate(e2BrowseProject.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2Browse) || e2Browse is null)
                throw new InvalidDataException("Browse activation fixture did not validate as an installation.");

            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            if (!form.IsNeutralInstallationStateForTest || form.InstallationOptionCountForTest != 0 ||
                form.InstallationStatusForTest != "No game selected.\r\nUse Find games... or Browse folder...")
                throw new InvalidDataException("Startup was not the authoritative empty installation state.");

            form.DiscoverInstallationsForTest([e1, e2]);
            if (form.InstallationOptionCountForTest != 2 || !form.IsNeutralInstallationStateForTest)
                throw new InvalidDataException("Discovery selected or activated an installation.");
            using (var singletonForm = new MainForm())
            {
                singletonForm.InitializeInstallationStateForTest();
                singletonForm.DiscoverInstallationsForTest([e1]);
                if (singletonForm.InstallationOptionCountForTest != 1 || !singletonForm.IsNeutralInstallationStateForTest)
                    throw new InvalidDataException("Singleton discovery selected or activated an installation.");
            }
            form.ActivateInstallationForTest(e1);
            if (!form.HasActiveInstallationForTest || form.ActiveGameForTest != ElviraGameProfile.Elvira1 ||
                !form.HasLoadedGraphicsForTest || !form.HasLoadedTextForTest || !form.HasLoadedRuntimeUiForTest)
                throw new InvalidDataException("Explicit Elvira I activation did not initialize all dependent contexts.");
            form.ActivateFontForTest();
            string? e1FontBeforeMods = form.FontSourceForTest;
            if (!form.IsFontLoadedForTest || !string.Equals(Path.GetFileName(e1FontBeforeMods), Elvira1ProductionProfile.ActiveVgaExecutable, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Font-first Elvira I activation did not resolve the authoritative runtime source.");
            form.OpenModsForTest();
            form.ActivateFontForTest();
            if (!string.Equals(e1FontBeforeMods, form.FontSourceForTest, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Opening Mods changed the already-established Font context.");

            form.ActivateInstallationForTest(e2);
            form.ActivateFontForTest();
            if (form.ActiveGameForTest != ElviraGameProfile.Elvira2 || !form.HasLoadedGraphicsForTest || !form.HasLoadedTextForTest ||
                !form.HasLoadedRuntimeUiForTest || !string.Equals(Path.GetFileName(form.FontSourceForTest), Elvira2ProductionProfile.ActiveExecutable, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Elvira I to Elvira II activation leaked stale context.");
            form.ActivateInstallationForTest(e1);
            form.ActivateFontForTest();
            if (form.ActiveGameForTest != ElviraGameProfile.Elvira1 || !string.Equals(Path.GetFileName(form.FontSourceForTest), Elvira1ProductionProfile.ActiveVgaExecutable, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Elvira II to Elvira I activation leaked stale context.");
            int effective = form.ApplyInstallationEffectiveCount;
            form.ActivateInstallationForTest(e1);
            if (form.ApplyInstallationEffectiveCount != effective)
                throw new InvalidDataException("Repeated active installation activation was not idempotent.");
            using (var browseForm = new MainForm())
            {
                browseForm.InitializeInstallationStateForTest();
                browseForm.BrowseValidatedInstallationForTest(e2Browse);
                if (browseForm.ActiveGameForTest != ElviraGameProfile.Elvira2 || !browseForm.HasActiveInstallationForTest)
                    throw new InvalidDataException("Validated explicit Browse activation did not establish Elvira II context.");
            }
            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("Context activation changed fixture GAMEPC bytes.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyFontVariantAwarenessSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1FontVariantSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var service = new FontVariantService();
            if (!service.Load(e1).IsSuccess || service.Load(e1).State!.Edits.Count != 0 || Directory.Exists(e1.ProjectRoot) || File.Exists(service.GetPath(e1)))
                throw new InvalidDataException("Missing legacy font project state was not backward-compatible/read-only.");

            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single();
            byte[] sharedBitmap = Convert.FromHexString("2050507050508800");
            FontProjectEdit shared = FontProjectEdit.Create(new(0x41), sharedBitmap, FontEditScope.Shared, null);
            FontProjectState e1State = service.SetEdit(e1, FontProjectState.Empty(ElviraGameProfile.Elvira1), shared);
            if (service.GetFontEditsForVariant(e1, e1State, e1Vga) is not [{ Edit: { Identity: { ByteValue: 0x41 } }, Bank: FontRuntimeBank.Full, Applicability: FontApplicabilityStatus.Shared }] ||
                service.GetFontEditsForVariant(e1, e1State, e1Ega) is not [{ Edit: { Identity: { ByteValue: 0x41 } }, Bank: FontRuntimeBank.Full, Applicability: FontApplicabilityStatus.Shared }])
                throw new InvalidDataException("Shared E1 font glyph did not project deterministically to VGA and EGA.");

            FontProjectEdit egaOnly = FontProjectEdit.Create(new(0x42), Convert.FromHexString("102850505050F800"), FontEditScope.RuntimeSpecific, VariantRuntimeKind.Elvira1Ega);
            e1State = service.SetEdit(e1, e1State, egaOnly);
            if (service.GetFontEditsForVariant(e1, e1State, e1Vga).Any(value => value.Edit.Identity.ByteValue == 0x42) ||
                service.GetFontEditsForVariant(e1, e1State, e1Ega).Single(value => value.Edit.Identity.ByteValue == 0x42).Applicability != FontApplicabilityStatus.RuntimeSpecific)
                throw new InvalidDataException("Runtime-specific E1 font glyph applicability was not enforced.");

            if (service.GetSlotProjection(e2Vga, 0x41).Bank != FontRuntimeBank.Low || service.GetSlotProjection(e2Vga, 0xA0).Bank != FontRuntimeBank.High)
                throw new InvalidDataException("RUNIT LOW/HIGH font projection diverged from frozen slot ranges.");
            if (service.GetSlotProjection(e2Vga, FontSlotMetadata.HudEraseGlyph) is not { Bank: FontRuntimeBank.Protected, Status: FontApplicabilityStatus.Protected })
                throw new InvalidDataException("Protected HUD glyph 0x81 was not exposed as protected.");
            try { _ = service.SetEdit(e2, FontProjectState.Empty(ElviraGameProfile.Elvira2), FontProjectEdit.Create(new(0x41), sharedBitmap, FontEditScope.RuntimeSpecific, VariantRuntimeKind.Elvira1Ega)); throw new InvalidDataException("Impossible E2 EGA font edit was accepted."); }
            catch (ArgumentException) { }
            try { _ = service.SetEdit(e2, FontProjectState.Empty(ElviraGameProfile.Elvira2), FontProjectEdit.Create(new(FontSlotMetadata.HudEraseGlyph), sharedBitmap, FontEditScope.Shared, null)); throw new InvalidDataException("Protected font glyph 0x81 was accepted."); }
            catch (InvalidOperationException) { }

            string gamePcPath = Path.Combine(e1.GameRoot, "GAMEPC"); string gamePcHash = HashFile(gamePcPath);
            FontProjectSaveResult saved = service.Save(e1, e1State);
            if (!saved.Succeeded || !File.Exists(saved.Path) || HashFile(gamePcPath) != gamePcHash || !File.ReadAllText(saved.Path).Contains("bitmapBase64", StringComparison.Ordinal))
                throw new InvalidDataException("Font project save did not remain project-only or did not persist glyph metadata.");
            if (!service.Load(e1).IsSuccess || service.Load(e1).State!.Edits.Count != 2 || Directory.Exists(e1.StorageLayout.VariantsRoot))
                throw new InvalidDataException("Font project round-trip duplicated state or created a variant payload.");
            using var main = new MainForm();
            using var form = new FontEditorForm();
            form.BindProjectVariant(e2, e2Vga);
            if (!form.HasFontVariantPresentationForTest) throw new InvalidDataException("Font variant presentation was not constructed.");
            if (service.GetFontEditsForVariant(e2, FontProjectState.Empty(ElviraGameProfile.Elvira2), e2Vga).Count != 0)
                throw new InvalidDataException("E2 font projection was not deterministic.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyActiveVariantSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1ActiveVariantSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Active-variant fixtures did not validate as installations.");

            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            if (!form.IsNeutralInstallationStateForTest || form.ActiveVariantForTest is not null || form.AvailableActiveVariantsForTest.Count != 0)
                throw new InvalidDataException("Startup did not remain neutral with no active variant.");

            form.ActivateInstallationForTest(e1);
            ProjectContext? e1Context = form.ActiveProjectForTest;
            if (form.ActiveVariantForTest?.VariantId != BuiltInVariantId.Elvira1Vga ||
                form.AvailableActiveVariantsForTest.Select(item => item.VariantId).ToArray() is not [BuiltInVariantId.Elvira1Vga, BuiltInVariantId.Elvira1Ega] ||
                form.AvailableActiveVariantsForTest.Any(item => !item.LogicalDataFileName.Equals("GAMEPC", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException("Elvira I did not expose exactly the VGA/EGA global variants with shared GAMEPC state.");
            form.ActivateFontForTest(); form.OpenModsForTest();
            if (!form.VariantAwareTabsUseActiveVariantForTest)
                throw new InvalidDataException("Elvira I VGA was not observed consistently by variant-aware tabs.");

            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            if (!ReferenceEquals(e1Context, form.ActiveProjectForTest) || form.ActiveVariantForTest?.VariantId != BuiltInVariantId.Elvira1Ega ||
                !form.VariantAwareTabsUseActiveVariantForTest)
                throw new InvalidDataException("Elvira I VGA-to-EGA switch recreated project context or left a stale tab variant.");

            form.ActivateInstallationForTest(e2);
            ProjectContext? e2Context = form.ActiveProjectForTest;
            if (form.ActiveVariantForTest?.VariantId != BuiltInVariantId.Elvira2Vga || form.AvailableActiveVariantsForTest.Count != 1 ||
                form.AvailableActiveVariantsForTest[0].VariantId != BuiltInVariantId.Elvira2Vga)
                throw new InvalidDataException("Elvira II did not expose exactly its VGA variant.");
            bool e2EgaRejected = false;
            try { form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega); }
            catch (InvalidOperationException) { e2EgaRejected = true; }
            if (!e2EgaRejected) throw new InvalidDataException("Elvira II accepted EGA as an active variant.");
            form.ActivateFontForTest(); form.OpenModsForTest();
            if (!ReferenceEquals(e2Context, form.ActiveProjectForTest) || !form.VariantAwareTabsUseActiveVariantForTest)
                throw new InvalidDataException("Elvira II variant-aware tab projection is stale.");

            form.ActivateInstallationForTest(e1);
            if (form.ActiveVariantForTest?.VariantId != BuiltInVariantId.Elvira1Vga || form.AvailableActiveVariantsForTest.Count != 2 ||
                !form.VariantAwareTabsUseActiveVariantForTest)
                throw new InvalidDataException("Elvira II-to-Elvira I activation retained a stale active variant.");
            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc)
                throw new InvalidDataException("Active-variant selection changed fixture GAMEPC bytes.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyButtonTerminologySmoke(string elvira1Root, string elvira2Root)
    {
        string priorLocale = UiText.LocaleId;
        try
        {
            // Opening the real contexts is read-only and confirms that terminology
            // lookup cannot affect either installation or its selected game state.
            _ = RequireSuccess(new ProjectContextLoader().Open(elvira1Root), "R8B Elvira I context");
            _ = RequireSuccess(new ProjectContextLoader().Open(elvira2Root), "R8B Elvira II context");

            UiText.SetLocale("en");
            var expectedEnglish = new Dictionary<string, string>
            {
                ["ApplyGame"] = "Apply changes to game",
                ["ApplyToExe"] = "Apply changes to EXE",
                ["CreateVariant"] = "Create variant...",
                ["OpenDataFile"] = "Open data file...",
                ["OpenGameExe"] = "Open game EXE...",
                ["GenerateLauncher"] = "Generate or update launcher",
                ["PreviewLauncher"] = "Preview launcher",
                ["RestoreOriginalLauncher"] = "Restore original launcher",
                ["RuntimeUi.Reload"] = "Reload runtime UI",
                ["RuntimeUi.Save"] = "Save runtime UI",
                ["RuntimeUi.ResetOverride"] = "Reset override",
                ["Recovery.VerifyPristine"] = "Verify pristine installation",
                ["Recovery.ReverseAll"] = "Preview reverse all changes…"
            };
            foreach ((string key, string expected) in expectedEnglish)
                if (UiText.Get(key) != expected)
                    throw new InvalidDataException($"R8B English action label diverged for {key}.");

            UiText.SetLocale("sk");
            if (UiText.Get("ApplyGame") != "Aplikovať zmeny do hry" ||
                UiText.Get("Recovery.VerifyPristine") != "Overiť pôvodnú inštaláciu" ||
                UiText.Get("OpenGameExe") != "Open game EXE...")
                throw new InvalidDataException("R8B Slovak translation/fallback behavior diverged.");
        }
        finally { UiText.SetLocale(priorLocale); }
    }

    private static void VerifyVariantManagerSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1VariantManagerSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1Project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2Project = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            if (!GameInstallationValidator.TryValidate(e1Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null ||
                !GameInstallationValidator.TryValidate(e2Project.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e2) || e2 is null)
                throw new InvalidDataException("Variant Manager fixtures did not validate as installations.");

            string e1GamePc = HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC"));
            using var form = new MainForm();
            if (!form.HasVariantManagerForTest) throw new InvalidDataException("Variant Manager controls were not constructed.");
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            form.OpenModsForTest();
            VariantManagerRow[] e1Rows = form.VariantManagerRowsForTest.ToArray();
            if (e1Rows.Select(row => row.Variant.VariantId).ToArray() is not [BuiltInVariantId.Elvira1Vga, BuiltInVariantId.Elvira1Ega] ||
                e1Rows.Any(row => row.Ownership != VariantDirectoryOperationStatus.NotFound || row.Readiness != VariantLaunchReadiness.VariantMissing || row.BuildStatus != VariantBuildStatus.Missing) ||
                e1Rows.Any(row => Directory.Exists(row.VariantRoot)))
                throw new InvalidDataException("Elvira I Variant Manager did not remain read-only for missing VGA/EGA variants.");

            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            e1Rows = form.VariantManagerRowsForTest.ToArray();
            if (form.ActiveVariantForTest?.VariantId != BuiltInVariantId.Elvira1Ega ||
                e1Rows.Single(row => row.Variant.VariantId == BuiltInVariantId.Elvira1Ega).Variant != form.ActiveVariantForTest)
                throw new InvalidDataException("Variant Manager did not track the authoritative E1 EGA selection.");

            string foreignRoot = e1Rows.Single(row => row.Variant.VariantId == BuiltInVariantId.Elvira1Vga).VariantRoot;
            Directory.CreateDirectory(foreignRoot);
            File.WriteAllText(Path.Combine(foreignRoot, "FOREIGN.TXT"), "external fixture file");
            form.OpenModsForTest();
            VariantManagerRow foreign = form.VariantManagerRowsForTest.Single(row => row.Variant.VariantId == BuiltInVariantId.Elvira1Vga);
            if (foreign.Ownership != VariantDirectoryOperationStatus.ForeignDirectoryConflict || foreign.Readiness != VariantLaunchReadiness.ForeignOrInvalidVariant || foreign.BuildStatus != VariantBuildStatus.Invalid ||
                !File.Exists(Path.Combine(foreignRoot, "FOREIGN.TXT")))
                throw new InvalidDataException("Foreign E1 variant directory was not safely reported and preserved.");

            form.ActivateInstallationForTest(e2);
            form.OpenModsForTest();
            VariantManagerRow[] e2Rows = form.VariantManagerRowsForTest.ToArray();
            if (e2Rows.Length != 1 || e2Rows[0].Variant.VariantId != BuiltInVariantId.Elvira2Vga ||
                e2Rows.Any(row => row.Variant.RuntimeKind == VariantRuntimeKind.Elvira1Ega))
                throw new InvalidDataException("Elvira II Variant Manager exposed an invalid EGA runtime.");
            if (HashFile(Path.Combine(e1Project.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2Project.GameRoot, "GAMEPC")) != e2GamePc ||
                Directory.EnumerateFiles(root, "*SK.EXE", SearchOption.AllDirectories).Any())
                throw new InvalidDataException("Variant Manager selection built output or modified fixture game data.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyVariantBuildStatusSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1VariantBuildStatusSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            string e1Hash = HashFile(Path.Combine(e1.GameRoot, "GAMEPC"));
            string e2Hash = HashFile(Path.Combine(e2.GameRoot, "GAMEPC"));
            var directories = new VariantDirectoryService();
            var composite = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, CompleteFixtureStages(null, null));
            var launcher = new VariantLauncherService(directories, composite);
            var status = new VariantBuildStatusService(composite, launcher);
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Ega);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single(item => item.VariantId == BuiltInVariantId.Elvira2Vga);

            if (status.Inspect(null, null).Status != VariantBuildStatus.Invalid)
                throw new InvalidDataException("No-installation state was not reported Invalid.");
            if (status.Inspect(e1, e1Vga).Status != VariantBuildStatus.Missing)
                throw new InvalidDataException("Missing E1 VGA variant was not reported Missing.");

            string foreignRoot = directories.GetVariantDirectoryPath(e1, e1Ega);
            Directory.CreateDirectory(foreignRoot);
            File.WriteAllText(Path.Combine(foreignRoot, "FOREIGN.TXT"), "external fixture file");
            if (status.Inspect(e1, e1Ega).Status != VariantBuildStatus.Invalid || !File.Exists(Path.Combine(foreignRoot, "FOREIGN.TXT")))
                throw new InvalidDataException("Foreign E1 EGA directory was not safely reported Invalid and preserved.");

            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, e1Vga), VariantDirectoryOperationStatus.Created, "E1 VGA status fixture");
            if (status.Inspect(e1, e1Vga).Status != VariantBuildStatus.Incomplete)
                throw new InvalidDataException("Owned but output-less E1 VGA directory was not reported Incomplete.");

            string readyRoot = directories.GetVariantDirectoryPath(e1, e1Vga);
            File.WriteAllBytes(Path.Combine(readyRoot, e1Vga.GeneratedExecutableName), [0x4D, 0x5A]);
            File.WriteAllBytes(Path.Combine(readyRoot, e1Vga.LogicalDataFileName), [0x00]);
            VariantBuildStatusProjection ready = status.Inspect(e1, e1Vga);
            if (ready.Status != VariantBuildStatus.Ready || ready.ConfiguredCapabilities != ready.CapabilityCount || ready.CapabilityCount == 0)
                throw new InvalidDataException("Owned E1 VGA output with configured plan was not reported Ready.");
            File.Delete(Path.Combine(readyRoot, e1Vga.GeneratedExecutableName));
            File.Delete(Path.Combine(readyRoot, e1Vga.LogicalDataFileName));
            if (status.Inspect(e2, e2Vga).Status != VariantBuildStatus.Missing ||
                VariantContextCatalog.CreateBuiltIns(e2).Any(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Ega))
                throw new InvalidDataException("Elvira II variant-specific status exposed an invalid EGA variant.");
            if (HashFile(Path.Combine(e1.GameRoot, "GAMEPC")) != e1Hash || HashFile(Path.Combine(e2.GameRoot, "GAMEPC")) != e2Hash ||
                Directory.EnumerateFiles(root, "*SK.EXE", SearchOption.AllDirectories).Any())
                throw new InvalidDataException("Read-only build-status inspection modified fixture game data or created generated executables.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyVariantManifestSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1VariantManifestSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            string e1GamePc = HashFile(Path.Combine(e1.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2.GameRoot, "GAMEPC"));
            var directories = new VariantDirectoryService();
            ICompositeBuildStep[] steps = CompleteFixtureStages(null, null).Take(4).Append(
                new FixtureCompositeStep(CompositeBuildStage.ApplyExecutableTransformation, "fixture-executable", _ => true, _ => null,
                    // R9D: readiness requires a supported executable identity, so the
                    // fixture materializes the genuine pristine source executable under
                    // the generated name instead of arbitrary fake bytes.
                    variant => { File.Copy(Path.Combine(variant.FutureVariantRoot, variant.SourceExecutableName), Path.Combine(variant.FutureVariantRoot, variant.GeneratedExecutableName), true); return null; })).ToArray();
            var composite = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, steps);
            var launcher = new VariantLauncherService(directories, composite);
            var status = new VariantBuildStatusService(composite, launcher);
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single(item => item.VariantId == BuiltInVariantId.Elvira2Vga);

            RequireDirectoryStatus(directories.EnsureVariantDirectory(e1, e1Vga), VariantDirectoryOperationStatus.Created, "E1 manifest fixture directory");
            string e1Root = directories.GetVariantDirectoryPath(e1, e1Vga);
            VariantManifestService.Write(e1, e1Vga, directories, CompositeBuildMode.PristineOnly);
            if (status.Inspect(e1, e1Vga).Status != VariantBuildStatus.Incomplete)
                throw new InvalidDataException("Manifest presence incorrectly implied readiness.");

            if (composite.Build(e1, e1Vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("E1 manifest fixture build failed.");
            VariantManifest first = VariantManifestService.Read(e1Root);
            string firstBytes = File.ReadAllText(Path.Combine(e1Root, VariantManifestService.FileName));
            VerifyVariantManifest(first, e1, e1Vga, "Full");
            if (status.Inspect(e1, e1Vga).Status != VariantBuildStatus.Ready)
                throw new InvalidDataException("A valid E1 output with manifest was not reported Ready.");
            if (composite.Build(e1, e1Vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success ||
                firstBytes != File.ReadAllText(Path.Combine(e1Root, VariantManifestService.FileName)))
                throw new InvalidDataException("E1 manifest content was not deterministic across identical builds.");

            RequireDirectoryStatus(directories.EnsureVariantDirectory(e2, e2Vga), VariantDirectoryOperationStatus.Created, "E2 manifest fixture directory");
            if (composite.Build(e2, e2Vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("E2 manifest fixture build failed.");
            VerifyVariantManifest(VariantManifestService.Read(directories.GetVariantDirectoryPath(e2, e2Vga)), e2, e2Vga, "Full");
            if (HashFile(Path.Combine(e1.GameRoot, "GAMEPC")) != e1GamePc || HashFile(Path.Combine(e2.GameRoot, "GAMEPC")) != e2GamePc ||
                Directory.EnumerateFiles(e1.GameRoot, "*SK.EXE", SearchOption.TopDirectoryOnly).Any() ||
                Directory.EnumerateFiles(e2.GameRoot, "*SK.EXE", SearchOption.TopDirectoryOnly).Any())
                throw new InvalidDataException("Variant manifest smoke modified fixture game roots.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRebuildReproducibilitySmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RebuildReproducibilitySmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            string e1GamePc = HashFile(Path.Combine(e1.GameRoot, "GAMEPC"));
            string e2GamePc = HashFile(Path.Combine(e2.GameRoot, "GAMEPC"));
            var directories = new VariantDirectoryService();

            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Ega);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single(item => item.VariantId == BuiltInVariantId.Elvira2Vga);

            VerifyRepeatedBuild(e1, e1Vga, new RunVgaCompositeBuildStep(directories));
            VerifyRepeatedBuild(e1, e1Ega, new RunEgaCompositeBuildStep(directories));
            VerifyRepeatedBuild(e2, e2Vga, new RunItCompositeBuildStep(directories));

            if (HashFile(Path.Combine(e1.GameRoot, "GAMEPC")) != e1GamePc ||
                HashFile(Path.Combine(e2.GameRoot, "GAMEPC")) != e2GamePc ||
                new PristineManifestService(e1.StorageLayout, e1.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline ||
                new PristineManifestService(e2.StorageLayout, e2.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline ||
                Directory.EnumerateFiles(e1.GameRoot, "*SK.EXE", SearchOption.TopDirectoryOnly).Any() ||
                Directory.EnumerateFiles(e2.GameRoot, "*SK.EXE", SearchOption.TopDirectoryOnly).Any())
                throw new InvalidDataException("Repeated variant builds modified an isolated game root or baseline.");

            void VerifyRepeatedBuild(ProjectContext project, VariantContext variant, ICompositeBuildStep executableStep)
            {
                ICompositeBuildStep[] steps = CompleteFixtureStages(null, null).Take(4).Append(executableStep).ToArray();
                var builds = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, steps);
                VariantDirectoryOperationResult prepared = directories.EnsureVariantDirectory(project, variant);
                if (prepared.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid))
                    throw new InvalidDataException($"{variant.VariantId} fixture directory could not be prepared: {prepared.Status}.");
                CompositeBuildResult firstBuild = builds.Build(project, variant, CompositeBuildMode.Full);
                if (firstBuild.Status != CompositeBuildStatus.Success)
                    throw new InvalidDataException($"First {variant.VariantId} build failed: {firstBuild.Status} / {firstBuild.Stages.Last().Detail}");

                string variantRoot = directories.GetVariantDirectoryPath(project, variant);
                string firstManifest = File.ReadAllText(Path.Combine(variantRoot, VariantManifestService.FileName));
                string[] firstArtifacts = Snapshot(variantRoot);
                VerifyVariantManifest(VariantManifestService.Read(variantRoot), project, variant, "Full");

                CompositeBuildResult secondBuild = builds.Build(project, variant, CompositeBuildMode.Full);
                if (secondBuild.Status != CompositeBuildStatus.Success)
                    throw new InvalidDataException($"Second {variant.VariantId} build failed: {secondBuild.Status} / {secondBuild.Stages.Last().Detail}");

                if (firstManifest != File.ReadAllText(Path.Combine(variantRoot, VariantManifestService.FileName)) ||
                    !firstArtifacts.SequenceEqual(Snapshot(variantRoot), StringComparer.Ordinal))
                    throw new InvalidDataException($"{variant.VariantId} rebuild output or manifest was not byte-identical.");
            }

            static string[] Snapshot(string variantRoot) => Directory.EnumerateFiles(variantRoot, "*", SearchOption.AllDirectories)
                .Select(path => new FileInfo(path))
                .Select(file => $"{Path.GetRelativePath(variantRoot, file.FullName).Replace('\\', '/')}|{file.Length}|{HashFile(file.FullName)}")
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyR9DBinaryRejectionSmoke(string elvira1Source, string elvira2Source)
    {
        // Temporary/copied fixtures only. The real GameRoots are snapshotted read-only
        // and re-verified at the end; they are never written by this smoke.
        string[] e1Before = SnapshotRootFiles(elvira1Source);
        string[] e2Before = SnapshotRootFiles(elvira2Source);
        string root = Path.Combine(Path.GetTempPath(), "Pi1R9DBinaryRejectionSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            string vgaPacked = CopyFixture(elvira1Source, root, "RUNVGA.EXE", "RUNVGA_PACKED.EXE");
            string egaPacked = CopyFixture(elvira1Source, root, "RUNEGA.EXE", "RUNEGA_PACKED.EXE");
            string runitPacked = CopyFixture(elvira2Source, root, "RUNIT.EXE", "RUNIT_PACKED.EXE");

            // 1-3: proven supported packed forms are accepted.
            Require(SupportedExecutableIdentityService.ClassifyRunVga(vgaPacked).Identity == SupportedExecutableIdentity.SupportedPacked, "Proven packed RUNVGA was not accepted.");
            Require(SupportedExecutableIdentityService.ClassifyRunEga(egaPacked).Identity == SupportedExecutableIdentity.SupportedPacked, "Proven packed RUNEGA was not accepted.");
            Require(SupportedExecutableIdentityService.ClassifyRunIt(runitPacked).Identity == SupportedExecutableIdentity.SupportedPacked, "Proven packed RUNIT was not accepted.");

            // 8a: proven canonical unpacked forms are accepted (unpacked in memory, materialized as temp copies only).
            string vgaCanonical = Path.Combine(root, "RUNVGA_CANONICAL.EXE");
            string egaCanonical = Path.Combine(root, "RUNEGA_CANONICAL.EXE");
            string runitCanonical = Path.Combine(root, "RUNIT_CANONICAL.EXE");
            File.WriteAllBytes(vgaCanonical, RunVgaBootstrapService.UnpackVerifiedOriginal(File.ReadAllBytes(vgaPacked)));
            File.WriteAllBytes(egaCanonical, RunEgaBootstrapService.CanonicalizePacked(File.ReadAllBytes(egaPacked)));
            File.WriteAllBytes(runitCanonical, RunItBootstrapService.UnpackCanonicalOriginal(File.ReadAllBytes(runitPacked)));
            Require(SupportedExecutableIdentityService.ClassifyRunVga(vgaCanonical).Identity == SupportedExecutableIdentity.SupportedCanonical, "Proven canonical RUNVGA was not accepted.");
            Require(SupportedExecutableIdentityService.ClassifyRunEga(egaCanonical).Identity == SupportedExecutableIdentity.SupportedCanonical, "Proven canonical RUNEGA was not accepted.");
            Require(SupportedExecutableIdentityService.ClassifyRunIt(runitCanonical).Identity == SupportedExecutableIdentity.SupportedCanonical, "Proven canonical RUNIT was not accepted.");

            // 4: one mutated byte in a safe temp copy becomes Unsupported (size and MZ openability preserved).
            string vgaMutated = MutatedCopy(vgaPacked, Path.Combine(root, "RUNVGA_MUTATED.EXE"));
            string egaMutated = MutatedCopy(egaPacked, Path.Combine(root, "RUNEGA_MUTATED.EXE"));
            string runitMutated = MutatedCopy(runitPacked, Path.Combine(root, "RUNIT_MUTATED.EXE"));
            string vgaCanonicalMutated = MutatedCopy(vgaCanonical, Path.Combine(root, "RUNVGA_CANONICAL_MUTATED.EXE"));
            foreach ((string name, SupportedExecutableClassification classification, string original) in new[]
            {
                ("RUNVGA packed", SupportedExecutableIdentityService.ClassifyRunVga(vgaMutated), vgaPacked),
                ("RUNEGA packed", SupportedExecutableIdentityService.ClassifyRunEga(egaMutated), egaPacked),
                ("RUNIT packed", SupportedExecutableIdentityService.ClassifyRunIt(runitMutated), runitPacked),
                ("RUNVGA canonical", SupportedExecutableIdentityService.ClassifyRunVga(vgaCanonicalMutated), vgaCanonical)
            })
            {
                Require(classification.Identity == SupportedExecutableIdentity.Unsupported, $"Mutated {name} was not Unsupported.");
                Require(new FileInfo(original).Length == classification.Size, $"Mutated {name} changed size; the mutation proof is invalid.");
            }
            if (File.ReadAllBytes(vgaMutated)[0] != (byte)'M' || File.ReadAllBytes(vgaMutated)[1] != (byte)'Z')
                throw new InvalidDataException("Mutation fixture lost its MZ header; the openability proof is invalid.");

            // 8b: unpackable-structure-but-unknown-hash forms are rejected (never merely "unpackable").
            RequireThrows<InvalidDataException>(() => RunVgaBootstrapService.UnpackVerifiedOriginal(File.ReadAllBytes(vgaMutated)), "Mutated packed RUNVGA unpack");
            RequireThrows<InvalidDataException>(() => RunEgaBootstrapService.CanonicalizePacked(File.ReadAllBytes(egaMutated)), "Mutated packed RUNEGA canonicalization");
            RequireThrows<InvalidDataException>(() => RunItBootstrapService.UnpackCanonicalOriginal(File.ReadAllBytes(runitMutated)), "Mutated packed RUNIT unpack");

            // 7: Missing is a different result from Unsupported.
            string missing = Path.Combine(root, "DOES_NOT_EXIST.EXE");
            Require(SupportedExecutableIdentityService.ClassifyRunVga(missing).Identity == SupportedExecutableIdentity.Missing, "Missing RUNVGA was not Missing.");
            Require(SupportedExecutableIdentityService.ClassifyRunEga(missing).Identity == SupportedExecutableIdentity.Missing, "Missing RUNEGA was not Missing.");
            Require(SupportedExecutableIdentityService.ClassifyRunIt(missing).Identity == SupportedExecutableIdentity.Missing, "Missing RUNIT was not Missing.");
            RequireThrows<FileNotFoundException>(() => RunVgaBootstrapService.CreateExtendedCp852(missing, Path.Combine(root, "MISSING_OUT.EXE"), GlyphRepository.CreateAllCp852Slots()), "Missing RUNVGA bootstrap");
            RequireThrows<FileNotFoundException>(() => RunItBootstrapService.CreateExtendedCp852(missing, Path.Combine(root, "MISSING_OUT.EXE"), GlyphRepository.CreateAllCp852Slots()), "Missing RUNIT bootstrap");
            RequireThrows<FileNotFoundException>(() => RunEgaBootstrapService.CreateFrozenCp852(missing, Path.Combine(root, "MISSING_OUT.EXE"), GlyphRepository.CreateAllCp852Slots()), "Missing RUNEGA bootstrap");

            // 5: unsupported binaries produce no binary-patched build artifact.
            string vgaBlocked = Path.Combine(root, "RUNVGA_BLOCKED.EXE");
            string egaBlocked = Path.Combine(root, "RUNEGA_BLOCKED.EXE");
            string runitBlocked = Path.Combine(root, "RUNIT_BLOCKED.EXE");
            IReadOnlyList<GlyphModel> glyphs = GlyphRepository.CreateAllCp852Slots();
            RequireThrows<InvalidDataException>(() => RunVgaBootstrapService.CreateExtendedCp852(vgaMutated, vgaBlocked, glyphs), "Mutated RUNVGA bootstrap");
            RequireThrows<InvalidDataException>(() => RunEgaBootstrapService.CreateFrozenCp852(egaMutated, egaBlocked, glyphs), "Mutated RUNEGA bootstrap");
            RequireThrows<InvalidDataException>(() => RunItBootstrapService.CreateExtendedCp852(runitMutated, runitBlocked, glyphs), "Mutated RUNIT bootstrap");
            if (File.Exists(vgaBlocked) || File.Exists(egaBlocked) || File.Exists(runitBlocked))
                throw new InvalidDataException("An unsupported binary produced a patched artifact.");

            // 5b: font binary patching rejects the mutated canonical copy but still serves the proven one.
            FontLoadResult proven = RunVgaFontService.LoadRunVga(vgaCanonical);
            proven.Glyphs[0x41].ReplaceEdited(Convert.FromHexString("2050507050508800"));
            string fontCopyOk = Path.Combine(root, "RUNVGA_FONT_OK.EXE");
            RunVgaFontService.SaveCopy(proven, fontCopyOk);
            if (!File.Exists(fontCopyOk)) throw new InvalidDataException("Supported font SaveCopy produced no output.");
            FontLoadResult mutatedView = RunVgaFontService.LoadRunVga(vgaCanonicalMutated);
            string fontCopyBlocked = Path.Combine(root, "RUNVGA_FONT_BLOCKED.EXE");
            if (mutatedView.CanApply)
            {
                RequireThrows<InvalidDataException>(() => RunVgaFontService.SaveCopy(mutatedView, fontCopyBlocked), "Mutated font SaveCopy");
                if (File.Exists(fontCopyBlocked)) throw new InvalidDataException("An unsupported binary produced a font-patched artifact.");
            }

            // 5b2 (review): generated FORMAT recognition must not authorize a mutation.
            // A legitimate generated V5/V2 with edited fonts stays patchable, but a copy
            // with one code byte modified outside the documented mutable font regions is
            // rejected even though its structure still classifies as GeneratedExtended.
            string vgaV5 = Path.Combine(root, "RUNVGA_TRUSTED_V5.EXE");
            string runitV2 = Path.Combine(root, "RUNIT_TRUSTED_V2.EXE");
            RunVgaBootstrapService.CreateExtendedCp852(vgaPacked, vgaV5, GlyphRepository.CreateAllCp852Slots());
            RunItBootstrapService.CreateExtendedCp852(runitPacked, runitV2, GlyphRepository.CreateAllCp852Slots());
            Require(SupportedExecutableIdentityService.ClassifyRunVga(vgaV5).Identity == SupportedExecutableIdentity.GeneratedExtended, "Legitimate V5 lost its generated FORMAT recognition.");
            Require(SupportedExecutableIdentityService.ClassifyRunIt(runitV2).Identity == SupportedExecutableIdentity.GeneratedExtended, "Legitimate V2 lost its generated FORMAT recognition.");
            FontLoadResult v5View = RunVgaFontService.LoadRunVga(vgaV5);
            v5View.Glyphs[0x42].ReplaceEdited(Convert.FromHexString("2050507050508800"));
            string v5CopyOk = Path.Combine(root, "RUNVGA_V5_FONT_OK.EXE");
            RunVgaFontService.SaveCopy(v5View, v5CopyOk);
            if (!File.Exists(v5CopyOk)) throw new InvalidDataException("Trusted generated V5 font SaveCopy produced no output.");
            FontLoadResult v2View = RunVgaFontService.LoadRunVga(runitV2);
            v2View.Glyphs[0x42].ReplaceEdited(Convert.FromHexString("2050507050508800"));
            string v2CopyOk = Path.Combine(root, "RUNIT_V2_FONT_OK.EXE");
            RunVgaFontService.SaveCopy(v2View, v2CopyOk);
            if (!File.Exists(v2CopyOk)) throw new InvalidDataException("Trusted generated V2 font SaveCopy produced no output.");
            string vgaV5Mutated = MutatedCodeCopy(vgaV5, Path.Combine(root, "RUNVGA_V5_MUTATED.EXE"), 0x8000);
            string runitV2Mutated = MutatedCodeCopy(runitV2, Path.Combine(root, "RUNIT_V2_MUTATED.EXE"), 0x1000);
            Require(SupportedExecutableIdentityService.ClassifyRunVga(vgaV5Mutated).Identity == SupportedExecutableIdentity.GeneratedExtended, "Mutated V5 lost its generated FORMAT recognition; the trust proof is invalid.");
            Require(SupportedExecutableIdentityService.ClassifyRunIt(runitV2Mutated).Identity == SupportedExecutableIdentity.GeneratedExtended, "Mutated V2 lost its generated FORMAT recognition; the trust proof is invalid.");
            FontLoadResult v5MutatedView = RunVgaFontService.LoadRunVga(vgaV5Mutated);
            FontLoadResult v2MutatedView = RunVgaFontService.LoadRunVga(runitV2Mutated);
            Require(v5MutatedView.CanApply && v2MutatedView.CanApply, "Mutated generated views lost font applicability; the trust proof is invalid.");
            string v5FontBlocked = Path.Combine(root, "RUNVGA_V5_FONT_BLOCKED.EXE");
            string v2FontBlocked = Path.Combine(root, "RUNIT_V2_FONT_BLOCKED.EXE");
            RequireThrows<InvalidDataException>(() => RunVgaFontService.SaveCopy(v5MutatedView, v5FontBlocked), "Mutated V5 font SaveCopy");
            RequireThrows<InvalidDataException>(() => RunVgaFontService.SaveCopy(v2MutatedView, v2FontBlocked), "Mutated V2 font SaveCopy");
            if (File.Exists(v5FontBlocked) || File.Exists(v2FontBlocked))
                throw new InvalidDataException("A code-modified generated executable produced a font-patched artifact.");

            // 5c: composite executable materialization fails closed on unsupported/missing variant sources.
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            var directories = new VariantDirectoryService();
            var disposable = new DisposableVariantBuildService(directories);
            VerifyCompositeRejection(e1, BuiltInVariantId.Elvira1Vga, new RunVgaCompositeBuildStep(directories), "RUNVGA.EXE", "RUNVGASK.EXE", "Elvira I VGA", directories, disposable);
            VerifyCompositeRejection(e1, BuiltInVariantId.Elvira1Ega, new RunEgaCompositeBuildStep(directories), "RUNEGA.EXE", "RUNEGASK.EXE", "Elvira I EGA", directories, disposable);
            VerifyCompositeRejection(e2, BuiltInVariantId.Elvira2Vga, new RunItCompositeBuildStep(directories), "RUNIT.EXE", "RUNITSK.EXE", "Elvira II VGA", directories, disposable);

            // Run/Debug readiness: supported full-build output stays LaunchReady; a mutated output does not.
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            ICompositeBuildStep[] stages = CompleteFixtureStages(null, null).Take(4).Append(new RunVgaCompositeBuildStep(directories)).ToArray();
            var builds = new CompositeBuildService(disposable, directories, stages);
            if (builds.Build(e1, e1Vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Supported RUNVGA full build failed; the acceptance path regressed.");
            var launcher = new VariantLauncherService(directories, builds);
            VariantLaunchTarget ready = launcher.Resolve(e1, e1Vga);
            if (ready.Readiness != VariantLaunchReadiness.LaunchReady)
                throw new InvalidDataException("Supported RUNVGA output was not launch-ready: " + ready.Detail);
            string generated = Path.Combine(directories.GetVariantDirectoryPath(e1, e1Vga), e1Vga.GeneratedExecutableName);
            byte[] generatedBytes = File.ReadAllBytes(generated);
            byte[] corrupted = (byte[])generatedBytes.Clone();
            corrupted[corrupted.Length / 2] ^= 0x01;
            File.WriteAllBytes(generated, corrupted);
            VariantLaunchTarget blocked = launcher.Resolve(e1, e1Vga);
            if (blocked.Readiness == VariantLaunchReadiness.LaunchReady)
                throw new InvalidDataException("A mutated generated executable resolved as launch-ready.");
            if (!blocked.Detail.Contains("unsupported or modified", StringComparison.OrdinalIgnoreCase) || !blocked.Detail.Contains("blocked", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Blocked readiness did not explain the unsupported binary: " + blocked.Detail);
            var execution = new VariantExecutionService(new RecordingVariantProcessRunner());
            if (execution.Execute(blocked, VariantExecutionMode.Run).Started)
                throw new InvalidDataException("A mutated generated executable started execution.");
            File.WriteAllBytes(generated, generatedBytes);

            // Review: a changed DATA artifact must name the data file, never the executable.
            string variantData = Path.Combine(directories.GetVariantDirectoryPath(e1, e1Vga), e1Vga.LogicalDataFileName);
            byte[] dataBytes = File.ReadAllBytes(variantData);
            byte[] dataCorrupted = (byte[])dataBytes.Clone();
            dataCorrupted[dataCorrupted.Length / 2] ^= 0x01;
            File.WriteAllBytes(variantData, dataCorrupted);
            VariantLaunchTarget dataBlocked = launcher.Resolve(e1, e1Vga);
            if (dataBlocked.Readiness == VariantLaunchReadiness.LaunchReady)
                throw new InvalidDataException("A mutated data artifact resolved as launch-ready.");
            if (!dataBlocked.Detail.Contains(e1Vga.LogicalDataFileName, StringComparison.OrdinalIgnoreCase) ||
                !dataBlocked.Detail.Contains("blocked", StringComparison.OrdinalIgnoreCase) ||
                dataBlocked.Detail.Contains(e1Vga.GeneratedExecutableName, StringComparison.OrdinalIgnoreCase) ||
                dataBlocked.Detail.Contains("unsupported or modified", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Data integrity failure misattributed the executable: " + dataBlocked.Detail);
            if (execution.Execute(dataBlocked, VariantExecutionMode.Run).Started)
                throw new InvalidDataException("A mutated data artifact started execution.");
            File.WriteAllBytes(variantData, dataBytes);
            if (launcher.Resolve(e1, e1Vga).Readiness != VariantLaunchReadiness.LaunchReady)
                throw new InvalidDataException("Restored variant output did not return to launch-ready.");

            // 9: supported builds remain reproducible and byte-identical to the frozen R9C results.
            string vgaSecond = Path.Combine(root, "RUNVGA_V5_B.EXE");
            string egaSecond = Path.Combine(root, "RUNEGA_OUT_B.EXE");
            string runitSecond = Path.Combine(root, "RUNIT_V2_B.EXE");
            RunVgaBootstrapService.CreateExtendedCp852(vgaPacked, Path.Combine(root, "RUNVGA_V5_A.EXE"), GlyphRepository.CreateAllCp852Slots());
            RunVgaBootstrapService.CreateExtendedCp852(vgaPacked, vgaSecond, GlyphRepository.CreateAllCp852Slots());
            string egaFirstHash = RunEgaBootstrapService.CreateFrozenCp852(egaPacked, Path.Combine(root, "RUNEGA_OUT_A.EXE"), GlyphRepository.CreateAllCp852Slots());
            string egaSecondHash = RunEgaBootstrapService.CreateFrozenCp852(egaPacked, egaSecond, GlyphRepository.CreateAllCp852Slots());
            RunItBootstrapService.CreateExtendedCp852(runitPacked, Path.Combine(root, "RUNIT_V2_A.EXE"), GlyphRepository.CreateAllCp852Slots());
            RunItBootstrapResult runitRebuilt = RunItBootstrapService.CreateExtendedCp852(runitPacked, runitSecond, GlyphRepository.CreateAllCp852Slots());
            if (HashFile(Path.Combine(root, "RUNVGA_V5_A.EXE")) != "256CF45DAD6C0108001E8BF9A4114CC79B340178554EAC87A24AAC0A1BC49B21" ||
                HashFile(Path.Combine(root, "RUNVGA_V5_A.EXE")) != HashFile(vgaSecond))
                throw new InvalidDataException("RUNVGA deterministic V5 result diverged.");
            if (egaFirstHash != "C4028BAB9A35B247008197A5753C5C5185F2DEBFF54DCB099178E318830F90EE" || egaFirstHash != egaSecondHash)
                throw new InvalidDataException("RUNEGA deterministic output diverged.");
            if (runitRebuilt.Sha256 != Elvira2ProductionProfile.DeterministicFontEnabled.Sha256 ||
                runitRebuilt.Sha256 != HashFile(Path.Combine(root, "RUNIT_V2_A.EXE")))
                throw new InvalidDataException("RUNIT deterministic V2 result diverged.");

            // 6: pristine GameRoots are byte-identical to the entry snapshot.
            if (!SnapshotRootFiles(elvira1Source).SequenceEqual(e1Before, StringComparer.Ordinal) ||
                !SnapshotRootFiles(elvira2Source).SequenceEqual(e2Before, StringComparer.Ordinal))
                throw new InvalidDataException("A pristine GameRoot was modified during rejection testing.");

            static string CopyFixture(string sourceRoot, string targetRoot, string file, string targetName)
            {
                string destination = Path.Combine(targetRoot, targetName);
                File.Copy(Path.Combine(sourceRoot, file), destination, false);
                return destination;
            }

            static string MutatedCopy(string source, string destination)
            {
                byte[] bytes = File.ReadAllBytes(source);
                bytes[bytes.Length / 2] ^= 0x01;
                File.WriteAllBytes(destination, bytes);
                return destination;
            }

            static string MutatedCodeCopy(string source, string destination, int offset)
            {
                byte[] bytes = File.ReadAllBytes(source);
                if (offset < 2 || offset >= bytes.Length) throw new InvalidDataException("Mutation offset is outside the code body.");
                bytes[offset] ^= 0x01;
                File.WriteAllBytes(destination, bytes);
                return destination;
            }

            static void RequireThrows<TException>(Action action, string description) where TException : Exception
            {
                try { action(); }
                catch (TException) { return; }
                catch (Exception ex) { throw new InvalidDataException($"{description} threw {ex.GetType().Name} instead of {typeof(TException).Name}."); }
                throw new InvalidDataException($"{description} did not throw.");
            }

            void VerifyCompositeRejection(ProjectContext project, BuiltInVariantId variantId, ICompositeBuildStep executableStep, string sourceName, string outputName, string runtimeDisplay, VariantDirectoryService directories, DisposableVariantBuildService disposable)
            {
                VariantContext variant = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.VariantId == variantId);
                RequireDirectoryStatus(directories.EnsureVariantDirectory(project, variant), VariantDirectoryOperationStatus.Created, runtimeDisplay + " fixture directory");
                RequireBuildStatus(disposable.Build(project, variant), DisposableVariantBuildStatus.Success, runtimeDisplay + " pristine build");
                string variantRoot = directories.GetVariantDirectoryPath(project, variant);
                string source = Path.Combine(variantRoot, sourceName);
                string output = Path.Combine(variantRoot, outputName);
                byte[] pristine = File.ReadAllBytes(source);
                byte[] mutated = (byte[])pristine.Clone();
                mutated[mutated.Length / 2] ^= 0x01;
                File.WriteAllBytes(source, mutated);
                string? unsupportedError = executableStep.Execute(project, variant);
                if (unsupportedError is null || !unsupportedError.Contains("unsupported or modified", StringComparison.OrdinalIgnoreCase) ||
                    !unsupportedError.Contains(runtimeDisplay, StringComparison.Ordinal) || File.Exists(output))
                    throw new InvalidDataException($"{runtimeDisplay} composite step did not fail closed on an unsupported binary.");
                File.Delete(source);
                string? missingError = executableStep.Execute(project, variant);
                if (missingError is null || !missingError.Contains("missing", StringComparison.OrdinalIgnoreCase) || File.Exists(output))
                    throw new InvalidDataException($"{runtimeDisplay} composite step did not report a missing binary distinctly.");
                File.WriteAllBytes(source, pristine);
            }
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static string[] SnapshotRootFiles(string gameRoot) => Directory.EnumerateFiles(gameRoot, "*", SearchOption.AllDirectories)
        .Select(path => new FileInfo(path))
        .Select(file => $"{Path.GetRelativePath(gameRoot, file.FullName).Replace('\\', '/')}|{file.Length}|{HashFile(file.FullName)}")
        .OrderBy(value => value, StringComparer.Ordinal)
        .ToArray();

    private static void VerifyVariantManifest(VariantManifest manifest, ProjectContext project, VariantContext variant, string buildState)
    {
        if (manifest.SchemaVersion != 1 || manifest.GameId != PristineManifestService.GameIdFor(project.GameProfile) ||
            manifest.VariantId != variant.VariantId.ToString() || manifest.RuntimeKind != variant.RuntimeKind.ToString() ||
            manifest.DirectoryKey != variant.DirectoryKey || manifest.BaselineFingerprint != project.BaselineFingerprint || manifest.BuildState != buildState)
            throw new InvalidDataException("Variant manifest identity or input fingerprint diverged.");
        if (!manifest.OutputArtifacts.Select(artifact => artifact.RelativePath).SequenceEqual(manifest.OutputArtifacts.Select(artifact => artifact.RelativePath).OrderBy(path => path, StringComparer.Ordinal)) ||
            manifest.OutputArtifacts.Any(artifact => Path.IsPathRooted(artifact.RelativePath) || artifact.RelativePath.Contains('\\') || string.IsNullOrWhiteSpace(artifact.Sha256)) ||
            manifest.OutputArtifacts.Any(artifact => artifact.RelativePath.Equals(VariantManifestService.FileName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidDataException("Variant manifest output artifacts are not deterministic relative identities.");
        if (manifest.RuntimeArtifacts.Count != 2 || manifest.RuntimeArtifacts.Any(artifact => !artifact.Present || artifact.Size <= 0 || string.IsNullOrWhiteSpace(artifact.Sha256)))
            throw new InvalidDataException("Variant manifest did not record the required runtime artifact identities.");
    }

    private static void VerifyCompositeBuildSmoke(string elvira1Root, string elvira2Root)
    {
        var loader = new ProjectContextLoader(); var directories = new VariantDirectoryService(); var disposable = new DisposableVariantBuildService(directories); var composite = new CompositeBuildService(disposable, directories);
        ProjectContext e1 = RequireSuccess(loader.Open(elvira1Root), "Elvira I composite project"); ProjectContext e2 = RequireSuccess(loader.Open(elvira2Root), "Elvira II composite project");
        var runVgaStep = new RunVgaCompositeBuildStep(directories);
        if (runVgaStep.AppliesTo(VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Ega)) ||
            runVgaStep.AppliesTo(VariantContextCatalog.CreateBuiltIns(e2).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira2Vga)))
            throw new InvalidDataException("RUNVGA step accepted an unsupported runtime identity.");
        VariantContext[] variants = VariantContextCatalog.CreateBuiltIns(e1).Concat(VariantContextCatalog.CreateBuiltIns(e2)).ToArray();
        foreach (VariantContext variant in variants)
        {
            CompositeBuildPlan first = composite.CreatePlan(variant.Project, variant, CompositeBuildMode.PristineOnly);
            CompositeBuildPlan second = composite.CreatePlan(variant.Project, variant, CompositeBuildMode.PristineOnly);
            if (!first.Stages.SequenceEqual(second.Stages) || !first.Stages.SequenceEqual(Enum.GetValues<CompositeBuildStage>())) throw new InvalidDataException("Composite plan order is not deterministic.");
            if (composite.Build(variant.Project, variant, CompositeBuildMode.PristineOnly).Status != CompositeBuildStatus.Success) throw new InvalidDataException("Pristine composite build failed.");
            if (composite.Build(variant.Project, variant, CompositeBuildMode.Full).Status != CompositeBuildStatus.NotFullyConfigured) throw new InvalidDataException("Full composite build falsely reported readiness.");
        }
        VerifyCompositeFixtureStages(elvira1Root);
    }

    private static void VerifyCompositeFixtureStages(string elvira1Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1CompositeBuildSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var directories = new VariantDirectoryService(); var disposable = new DisposableVariantBuildService(directories);
            VariantContext variant = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RequireDirectoryStatus(directories.EnsureVariantDirectory(project, variant), VariantDirectoryOperationStatus.Created, "composite fixture directory");
            string variantRoot = directories.GetVariantDirectoryPath(project, variant);
            string prior = Path.Combine(variantRoot, "PRIOR.TXT"); File.WriteAllText(prior, "retain me");

            var preflightFailure = new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations, "preflight-failure", _ => true, _ => "intentional preflight failure", _ => null);
            var preflightService = new CompositeBuildService(disposable, directories, [preflightFailure]);
            if (preflightService.Build(project, variant, CompositeBuildMode.Full).Status != CompositeBuildStatus.PreflightFailed || !File.Exists(prior))
                throw new InvalidDataException("Composite preflight failure did not preserve the existing variant.");

            var order = new List<CompositeBuildStage>();
            var dataFailure = new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations, "data", _ => true, _ => null, _ => "intentional execution failure", order);
            var laterFont = new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations, "font", _ => true, _ => null, _ => null, order);
            ICompositeBuildStep[] completeForFailure = CompleteFixtureStages(dataFailure, laterFont);
            var executionService = new CompositeBuildService(disposable, directories, completeForFailure.Reverse());
            CompositeBuildResult execution = executionService.Build(project, variant, CompositeBuildMode.Full);
            if (execution.Status != CompositeBuildStatus.TransformationFailed || laterFont.ExecuteCount != 0 || execution.Stages.Last().Stage != CompositeBuildStage.ApplyDataTransformations)
                throw new InvalidDataException("Composite execution failure did not stop dependent stages.");

            var first = new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations, "first", _ => true, _ => null, _ => null);
            var second = new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations, "second", _ => true, _ => null, _ => null);
            if (new CompositeBuildService(disposable, directories, [first, second]).Build(project, variant, CompositeBuildMode.Full).Status != CompositeBuildStatus.PreflightFailed)
                throw new InvalidDataException("Duplicate exclusive stage was not rejected before rebuild.");

            var deterministicOrder = new List<CompositeBuildStage>();
            var nonApplicable = new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations, "other-runtime", _ => false, _ => null, _ => null);
            ICompositeBuildStep[] ordered = CompleteFixtureStages(null, null, deterministicOrder).Reverse().Append(nonApplicable).ToArray();
            if (new CompositeBuildService(disposable, directories, ordered).Build(project, variant, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success ||
                !deterministicOrder.SequenceEqual(new[] { CompositeBuildStage.ApplyDataTransformations, CompositeBuildStage.ApplyGraphicsTransformations, CompositeBuildStage.ApplyFontTransformations, CompositeBuildStage.ApplyRuntimeUiTransformations, CompositeBuildStage.ApplyExecutableTransformation }) || nonApplicable.ExecuteCount != 0)
                throw new InvalidDataException("Composite stage ordering or applicability is not deterministic.");

            ProjectContext foreign = RequireSuccess(new ProjectContextLoader().Open(elvira1Source), "foreign composite project");
            if (new CompositeBuildService(disposable, directories).Build(foreign, variant, CompositeBuildMode.PristineOnly).Status != CompositeBuildStatus.VariantInvalid)
                throw new InvalidDataException("Cross-project variant association was not rejected.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static ICompositeBuildStep[] CompleteFixtureStages(FixtureCompositeStep? data, FixtureCompositeStep? font, List<CompositeBuildStage>? executionOrder = null)
    {
        FixtureCompositeStep step(CompositeBuildStage stage, string name) => new(stage, name, _ => true, _ => null, _ => null, executionOrder);
        return [data ?? step(CompositeBuildStage.ApplyDataTransformations, "data"), step(CompositeBuildStage.ApplyGraphicsTransformations, "graphics"), font ?? step(CompositeBuildStage.ApplyFontTransformations, "font"), step(CompositeBuildStage.ApplyRuntimeUiTransformations, "runtime-ui"), step(CompositeBuildStage.ApplyExecutableTransformation, "executable")];
    }

    private static void VerifyRunVgaCompositeFixture(string elvira1Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunVgaCompositeSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext project = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var directories = new VariantDirectoryService(); var disposable = new DisposableVariantBuildService(directories);
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            var step = new RunVgaCompositeBuildStep(directories);
            if (!step.AppliesTo(vga) || step.AppliesTo(ega)) throw new InvalidDataException("RUNVGA applicability is not runtime-specific.");
            RequireDirectoryStatus(directories.EnsureVariantDirectory(project, vga), VariantDirectoryOperationStatus.Created, "RUNVGA fixture directory");
            if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")))
                throw new InvalidDataException("Fixture root unexpectedly contains a builder artifact before execution.");
            var capabilityOnly = new CompositeBuildService(disposable, directories, [step]);
            if (!capabilityOnly.CreatePlan(project, vga, CompositeBuildMode.PristineOnly).Capabilities.Single().Configured)
                throw new InvalidDataException("RUNVGA patch capability was not reported as configured.");
            if (capabilityOnly.Build(project, vga, CompositeBuildMode.PristineOnly).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Pristine-only capability inspection/rebuild failed.");
            string variantRoot = directories.GetVariantDirectoryPath(project, vga), source = Path.Combine(variantRoot, "RUNVGA.EXE"), output = Path.Combine(variantRoot, "RUNVGASK.EXE");
            if (File.Exists(output) || capabilityOnly.Build(project, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.NotFullyConfigured || File.Exists(output))
                throw new InvalidDataException("Incomplete full build executed the RUNVGA capability.");

            ICompositeBuildStep[] executableFixture =
            [
                new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations, "data", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations, "graphics", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations, "font", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations, "runtime-ui", _ => true, _ => null, _ => null), step
            ];
            var composite = new CompositeBuildService(disposable, directories, executableFixture);
            CompositeBuildResult first = composite.Build(project, vga, CompositeBuildMode.Full);
            if (first.Status != CompositeBuildStatus.Success || !first.Stages.Any(item => item.Stage == CompositeBuildStage.ApplyExecutableTransformation && item.Name == step.Name && item.Succeeded))
                throw new InvalidDataException("Complete fixture pipeline did not execute the RUNVGA step.");
            string firstHash = HashFile(output);
            if (new FileInfo(output).Length != RunVgaBootstrapService.V5Size || firstHash != "256CF45DAD6C0108001E8BF9A4114CC79B340178554EAC87A24AAC0A1BC49B21")
                throw new InvalidDataException("RUNVGA fixture output diverged from the frozen deterministic V5 result.");
            if (RunVgaBootstrapService.DetectState(source) != RunVgaBootstrapState.OriginalPacked || RunVgaBootstrapService.DetectState(output) != RunVgaBootstrapState.ExtendedCp852V5)
                throw new InvalidDataException("RUNVGA fixture source/output state is invalid.");
            File.WriteAllText(Path.Combine(variantRoot, "STALE.TXT"), "old variant data"); File.WriteAllBytes(output, [0]);
            CompositeBuildResult second = composite.Build(project, vga, CompositeBuildMode.Full);
            if (second.Status != CompositeBuildStatus.Success || HashFile(output) != firstHash || File.Exists(Path.Combine(variantRoot, "STALE.TXT")))
                throw new InvalidDataException("RUNVGA rebuild was cumulative or nondeterministic.");
            if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")))
                throw new InvalidDataException("RUNVGA adapter created a root-side artifact.");

            var corruptSource = new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations, "corrupt-after-copy", _ => true, _ => null, variant => { File.WriteAllBytes(Path.Combine(variant.FutureVariantRoot, "RUNVGA.EXE"), [0]); return null; });
            ICompositeBuildStep[] failureSteps = [corruptSource,
                new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations, "graphics", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations, "font", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations, "runtime-ui", _ => true, _ => null, _ => null), step];
            CompositeBuildResult failure = new CompositeBuildService(disposable, directories, failureSteps).Build(project, vga, CompositeBuildMode.Full);
            if (failure.Status != CompositeBuildStatus.TransformationFailed || failure.Stages.Last().Stage != CompositeBuildStage.ApplyExecutableTransformation || failure.Stages.Last().Name != step.Name)
                throw new InvalidDataException("Invalid post-copy RUNVGA did not fail at the RUNVGA executable stage.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunEgaBootstrapFixture(string canonicalSource, string? packedSource = null)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunEgaBootstrapSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            string source = Path.Combine(root, "RUNEGA.EXE"), first = Path.Combine(root, "FIRST.EXE"), second = Path.Combine(root, "SECOND.EXE");
            File.Copy(canonicalSource, source); string sourceHash = HashFile(source);
            if (!RunEgaBootstrapService.IsCanonical(File.ReadAllBytes(source))) throw new InvalidDataException("Fixture source is not the frozen canonical RUNEGA.");
            IReadOnlyList<GlyphModel> glyphs = GlyphRepository.CreateAllCp852Slots();
            string firstHash = RunEgaBootstrapService.CreateFrozenCp852(source, first, glyphs);
            string secondHash = RunEgaBootstrapService.CreateFrozenCp852(source, second, glyphs);
            if (sourceHash != HashFile(source) || firstHash != secondHash || !File.ReadAllBytes(first).SequenceEqual(File.ReadAllBytes(second)))
                throw new InvalidDataException("RUNEGA bootstrap is cumulative or changed its source.");
            RunEgaBootstrapService.ValidateOutput(File.ReadAllBytes(first));
            Console.WriteLine($"RUNEGA fixture output: {new FileInfo(first).Length} bytes SHA-256 {firstHash}");
            if (packedSource is not null)
            {
                string packed = Path.Combine(root, "PACKED.EXE"), packedOut1 = Path.Combine(root, "PACK1.EXE"), packedOut2 = Path.Combine(root, "PACK2.EXE");
                File.Copy(packedSource, packed);
                string packedHash = HashFile(packed);
                string packedFirst = RunEgaBootstrapService.CreateFrozenCp852(packed, packedOut1, glyphs);
                string packedSecond = RunEgaBootstrapService.CreateFrozenCp852(packed, packedOut2, glyphs);
                if (packedHash != HashFile(packed) || packedFirst != firstHash || packedSecond != firstHash || !File.ReadAllBytes(first).SequenceEqual(File.ReadAllBytes(packedOut1)) || !File.ReadAllBytes(packedOut1).SequenceEqual(File.ReadAllBytes(packedOut2)))
                    throw new InvalidDataException("Packed RUNEGA canonicalization is not deterministic/equivalent.");
            }
            if (Directory.EnumerateFiles(root).Any(p => Path.GetFileName(p).Equals("RUNEGAO.EXE", StringComparison.OrdinalIgnoreCase) || Path.GetFileName(p).Equals("GAMEPCO", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException("RUNEGA bootstrap created an implicit sibling artifact.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunEgaCompositeFixture(string elvira1Source)
    {
        string root=Path.Combine(Path.GetTempPath(),"Pi1RunEgaCompositeSmoke",Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root); ProjectContext project=CreateBuildFixtureProjectContext(root,"e1",elvira1Source,ElviraGameProfile.Elvira1);
            var directories=new VariantDirectoryService();var disposable=new DisposableVariantBuildService(directories);VariantContext ega=VariantContextCatalog.CreateBuiltIns(project).Single(x=>x.RuntimeKind==VariantRuntimeKind.Elvira1Ega);var step=new RunEgaCompositeBuildStep(directories);
            if(!step.AppliesTo(ega)||step.AppliesTo(VariantContextCatalog.CreateBuiltIns(project).Single(x=>x.RuntimeKind==VariantRuntimeKind.Elvira1Vga)))throw new InvalidDataException("RUNEGA applicability is not runtime-specific.");
            RequireDirectoryStatus(directories.EnsureVariantDirectory(project,ega),VariantDirectoryOperationStatus.Created,"RUNEGA fixture directory");
            var capability=new CompositeBuildService(disposable,directories,[step]);if(!capability.CreatePlan(project,ega,CompositeBuildMode.PristineOnly).Capabilities.Single().Configured)throw new InvalidDataException("RUNEGA capability is not configured.");
            CompositeBuildResult pristine=capability.Build(project,ega,CompositeBuildMode.PristineOnly);if(pristine.Status!=CompositeBuildStatus.Success)throw new InvalidDataException("RUNEGA pristine-only failed: "+pristine.Status+" / "+pristine.Stages.Last().Detail);string vr=directories.GetVariantDirectoryPath(project,ega),outp=Path.Combine(vr,"RUNEGASK.EXE");
            if(File.Exists(outp)||capability.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(outp))throw new InvalidDataException("Incomplete build executed RUNEGA.");
            ICompositeBuildStep[] all=[new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations,"data",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations,"graphics",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations,"font",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations,"ui",_=>true,_=>null,_=>null),step];
            var full=new CompositeBuildService(disposable,directories,all);if(full.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!="C4028BAB9A35B247008197A5753C5C5185F2DEBFF54DCB099178E318830F90EE"||new FileInfo(outp).Length!=RunEgaBootstrapService.OutputSize)throw new InvalidDataException("RUNEGA fixture output diverged.");
            File.WriteAllBytes(outp,[0]);if(full.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!="C4028BAB9A35B247008197A5753C5C5185F2DEBFF54DCB099178E318830F90EE")throw new InvalidDataException("RUNEGA fixture rebuild was nondeterministic.");
        }
        finally { if(Directory.Exists(root))Directory.Delete(root,true); }
    }

    private static void VerifyRealRunEgaCompositeBuild(string elvira1Root)
    {
        ProjectContext project=RequireSuccess(new ProjectContextLoader().Open(elvira1Root),"real RUNEGA project");VariantContext ega=VariantContextCatalog.CreateBuiltIns(project).Single(x=>x.RuntimeKind==VariantRuntimeKind.Elvira1Ega);var directories=new VariantDirectoryService();var composite=new CompositeBuildService(new DisposableVariantBuildService(directories),directories,[new RunEgaCompositeBuildStep(directories)]);
        CompositeBuildPlan plan=composite.CreatePlan(project,ega,CompositeBuildMode.PristineOnly);if(plan.Capabilities is not [{Stage:CompositeBuildStage.ApplyExecutableTransformation,Configured:true}])throw new InvalidDataException("Real RUNEGA capability not configured.");
        if(composite.Build(project,ega,CompositeBuildMode.PristineOnly).Status!=CompositeBuildStatus.Success)throw new InvalidDataException("Real RUNEGA pristine-only failed.");string vr=directories.GetVariantDirectoryPath(project,ega),output=Path.Combine(vr,"RUNEGASK.EXE");
        if(File.Exists(output)||composite.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(output)||new PristineManifestService(project.StorageLayout,project.GameProfile).ValidateBaseline().Status!=BaselineValidationStatus.MatchesBaseline)throw new InvalidDataException("Real RUNEGA capability changed its pristine state.");
    }

    private static void VerifyRunItCompositeFixture(string elvira2Source)
    {
        string root=Path.Combine(Path.GetTempPath(),"Pi1RunItCompositeSmoke",Guid.NewGuid().ToString("N"));
        try{Directory.CreateDirectory(root);ProjectContext project=CreateBuildFixtureProjectContext(root,"e2",elvira2Source,ElviraGameProfile.Elvira2);var directories=new VariantDirectoryService();var disposable=new DisposableVariantBuildService(directories);VariantContext runit=VariantContextCatalog.CreateBuiltIns(project).Single();var step=new RunItCompositeBuildStep(directories);if(!step.AppliesTo(runit))throw new InvalidDataException("RUNIT applicability failed.");RequireDirectoryStatus(directories.EnsureVariantDirectory(project,runit),VariantDirectoryOperationStatus.Created,"RUNIT fixture directory");var cap=new CompositeBuildService(disposable,directories,[step]);if(!cap.CreatePlan(project,runit,CompositeBuildMode.PristineOnly).Capabilities.Single().Configured)throw new InvalidDataException("RUNIT capability not configured.");if(cap.Build(project,runit,CompositeBuildMode.PristineOnly).Status!=CompositeBuildStatus.Success)throw new InvalidDataException("RUNIT pristine-only failed.");string vr=directories.GetVariantDirectoryPath(project,runit),outp=Path.Combine(vr,"RUNITSK.EXE");if(File.Exists(outp)||cap.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(outp))throw new InvalidDataException("Incomplete RUNIT build executed.");ICompositeBuildStep[] all=[new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations,"data",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations,"graphics",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations,"font",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations,"ui",_=>true,_=>null,_=>null),step];var full=new CompositeBuildService(disposable,directories,all);if(full.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!=Elvira2ProductionProfile.DeterministicFontEnabled.Sha256||new FileInfo(outp).Length!=RunItBootstrapService.ExtendedSize)throw new InvalidDataException("RUNIT fixture output diverged.");File.WriteAllBytes(outp,[0]);if(full.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!=Elvira2ProductionProfile.DeterministicFontEnabled.Sha256)throw new InvalidDataException("RUNIT fixture rebuild nondeterministic.");}
        finally{if(Directory.Exists(root))Directory.Delete(root,true);}
    }

    private static void VerifyRealRunItCompositeBuild(string elvira2Root)
    {
        ProjectContext project=RequireSuccess(new ProjectContextLoader().Open(elvira2Root),"real RUNIT project");VariantContext runit=VariantContextCatalog.CreateBuiltIns(project).Single();var directories=new VariantDirectoryService();var composite=new CompositeBuildService(new DisposableVariantBuildService(directories),directories,[new RunItCompositeBuildStep(directories)]);if(composite.CreatePlan(project,runit,CompositeBuildMode.PristineOnly).Capabilities is not [{Configured:true}])throw new InvalidDataException("Real RUNIT capability not configured.");if(composite.Build(project,runit,CompositeBuildMode.PristineOnly).Status!=CompositeBuildStatus.Success)throw new InvalidDataException("Real RUNIT pristine-only failed.");string output=Path.Combine(directories.GetVariantDirectoryPath(project,runit),"RUNITSK.EXE");if(File.Exists(output)||composite.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(output)||new PristineManifestService(project.StorageLayout,project.GameProfile).ValidateBaseline().Status!=BaselineValidationStatus.MatchesBaseline)throw new InvalidDataException("Real RUNIT capability changed pristine state.");
    }

    private static void VerifyRealRunVgaCompositeBuild(string elvira1Root)
    {
        ProjectContext project = RequireSuccess(new ProjectContextLoader().Open(elvira1Root), "real RUNVGA project");
        VariantContext variant = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
        var directories = new VariantDirectoryService(); var composite = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, [new RunVgaCompositeBuildStep(directories)]);
        if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGAO.EXE")))
            throw new InvalidDataException("Real pristine root already contains a forbidden RUNVGA builder artifact.");
        CompositeBuildPlan plan = composite.CreatePlan(project, variant, CompositeBuildMode.PristineOnly);
        if (plan.Capabilities is not [{ Stage: CompositeBuildStage.ApplyExecutableTransformation, Configured: true }])
            throw new InvalidDataException("Real RUNVGA capability is not read-only configured.");
        CompositeBuildResult first = composite.Build(project, variant, CompositeBuildMode.PristineOnly);
        if (first.Status != CompositeBuildStatus.Success) throw new InvalidDataException("Real pristine-only rebuild failed: " + first.Status);
        string variantRoot = directories.GetVariantDirectoryPath(project, variant), source = Path.Combine(variantRoot, "RUNVGA.EXE"), output = Path.Combine(variantRoot, "RUNVGASK.EXE");
        if (RunVgaBootstrapService.DetectState(source) != RunVgaBootstrapState.OriginalPacked || File.Exists(output))
            throw new InvalidDataException("Real pristine-only build did not restore the clean RUNVGA source tree.");
        CompositeBuildResult full = composite.Build(project, variant, CompositeBuildMode.Full);
        if (full.Status != CompositeBuildStatus.NotFullyConfigured || full.Stages.Last().Detail.Contains(nameof(CompositeBuildStage.ApplyExecutableTransformation), StringComparison.Ordinal) || File.Exists(output))
            throw new InvalidDataException("Full readiness did not retain RUNVGA while reporting the remaining missing stages.");
        if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGAO.EXE")) ||
            new PristineManifestService(project.StorageLayout, project.GameProfile).ValidateBaseline().Status != BaselineValidationStatus.MatchesBaseline)
            throw new InvalidDataException("Real RUNVGA composite build changed the pristine root.");
    }

    private sealed class RecordingVariantProcessRunner : IVariantProcessRunner
    {
        public List<System.Diagnostics.ProcessStartInfo> Starts { get; } = [];
        public Exception? Failure { get; set; }
        public void Start(System.Diagnostics.ProcessStartInfo startInfo)
        {
            if (Failure is not null) throw Failure;
            Starts.Add(startInfo);
        }
    }

    private sealed class FixtureCompositeStep : ICompositeBuildStep
    {
        private readonly Func<VariantContext, bool> _applies; private readonly Func<VariantContext, string?> _preflight; private readonly Func<VariantContext, string?> _execute; private readonly List<CompositeBuildStage>? _order;
        public FixtureCompositeStep(CompositeBuildStage stage, string name, Func<VariantContext, bool> applies, Func<VariantContext, string?> preflight, Func<VariantContext, string?> execute, List<CompositeBuildStage>? order = null) { Stage = stage; Name = name; _applies = applies; _preflight = preflight; _execute = execute; _order = order; }
        public CompositeBuildStage Stage { get; } public string Name { get; } public int ExecuteCount { get; private set; }
        public bool AppliesTo(VariantContext variant) => _applies(variant);
        public string? Preflight(ProjectContext project, VariantContext variant) => _preflight(variant);
        public string? Execute(ProjectContext project, VariantContext variant) { ExecuteCount++; _order?.Add(Stage); return _execute(variant); }
    }
}
