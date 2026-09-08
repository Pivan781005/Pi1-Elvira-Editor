using System.Reflection;

namespace Pi1ElviraEditor;

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
        if (args.Length == 3 && args[0].Equals("--runvga-pause-menu-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunVgaPauseMenuSmoke(args[1], args[2]); Console.WriteLine("RUNVGA Pause.menu: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNVGA Pause.menu: FAIL - " + ex.Message); Environment.ExitCode = 1; }
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

                temporaryCanonical = Path.Combine(Path.GetTempPath(), "Pi1ElviraEditor-preview-" + Guid.NewGuid().ToString("N") + ".EXE");
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
        if (args.Length == 3 && args[0].Equals("--runtime-ui-grid-reentrancy-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiGridReentrancySmoke(args[1], args[2]); Console.WriteLine("Runtime UI grid reentrancy: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI grid reentrancy: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-grid-edit-safety-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiGridEditSafetySmoke(args[1], args[2]); Console.WriteLine("Runtime UI grid edit safety: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI grid edit safety: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-legacy-unassigned-ux-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiLegacyUnassignedUxSmoke(args[1], args[2]); Console.WriteLine("Runtime UI legacy-unassigned UX: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI legacy-unassigned UX: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-live-localization-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiLiveLocalizationSmoke(args[1], args[2]); Console.WriteLine("Runtime UI live localization: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI live localization: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--font-lazy-working-copy-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyFontLazyWorkingCopySmoke(args[1], args[2]); Console.WriteLine("Font lazy working copy: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Font lazy working copy: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--font-real-click-lazy-working-copy-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyFontRealClickLazyWorkingCopySmoke(args[1], args[2]); Console.WriteLine("Font real-click lazy working copy: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Font real-click lazy working copy: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runega-generated-font-load-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunEgaGeneratedFontLoadSmoke(args[1], args[2]); Console.WriteLine("RUNEGA generated font load: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNEGA generated font load: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--font-clear-glyph-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyFontClearGlyphSmoke(args[1], args[2]); Console.WriteLine("Font clear glyph: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Font clear glyph: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--hud-erase-0x81-invariant-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyHudErase081InvariantSmoke(args[1], args[2]); Console.WriteLine("HUD erase 0x81 invariant: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("HUD erase 0x81 invariant: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-mods-consistency-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiModsConsistencySmoke(args[1], args[2]); Console.WriteLine("Runtime UI and Mods consistency: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI and Mods consistency: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--variant-edition-identity-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyVariantEditionIdentitySmoke(args[1], args[2]); Console.WriteLine("Variant edition identity: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Variant edition identity: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runega-ui-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunEgaUiSmoke(args[1], args[2]); Console.WriteLine("RUNEGA Runtime UI: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNEGA Runtime UI: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runvga-confirm-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunVgaConfirmSmoke(args[1], args[2]); Console.WriteLine("RUNVGA Confirm.generic: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNVGA Confirm.generic: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runvga-overwrite-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunVgaOverwriteSmoke(args[1], args[2]); Console.WriteLine("RUNVGA Save.overwrite: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNVGA Save.overwrite: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--legacy-adopt-realsim-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLegacyAdoptRealSimSmoke(args[1], args[2]); Console.WriteLine("Legacy adoption realsim: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Legacy adoption realsim: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-original-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiOriginalSmoke(args[1], args[2]); Console.WriteLine("Runtime UI Original feedback: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI Original feedback: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runega-selector-audit-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunEgaSelectorAuditSmoke(args[1], args[2]); Console.WriteLine("RUNEGA selector/bridge audit: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNEGA selector/bridge audit: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runega-semantic-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRunEgaSemanticSmoke(args[1], args[2]); Console.WriteLine("RUNEGA semantic fields: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("RUNEGA semantic fields: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-migration-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiMigrationSmoke(args[1], args[2]); Console.WriteLine("Runtime UI schema migration: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI schema migration: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--runtime-ui-isolation-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyRuntimeUiIsolationSmoke(args[1], args[2]); Console.WriteLine("Runtime UI cross-runtime isolation: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Runtime UI cross-runtime isolation: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--legacy-edition-safety-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLegacyEditionSafetySmoke(args[1], args[2]); Console.WriteLine("Legacy edition safety: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Legacy edition safety: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--legacy-adopt-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyLegacyAdoptSmoke(args[1], args[2]); Console.WriteLine("Legacy adoption: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Legacy adoption: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 3 && args[0].Equals("--mods-post-adoption-consistency-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { VerifyModsPostAdoptionConsistencySmoke(args[1], args[2]); Console.WriteLine("Mods post-adoption consistency: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Mods post-adoption consistency: FAIL - " + ex.Message); Environment.ExitCode = 1; }
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
        if (args.Length == 1 && args[0].Equals("--branding-assets-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { string exe = RunBrandingAssetsSmoke(); Console.WriteLine($"Branding assets: PASS exe={exe} icon=validated"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Branding assets: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--startup-splash-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunStartupSplashSmoke(); Console.WriteLine("Startup splash: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Startup splash: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        if (args.Length == 1 && args[0].Equals("--version-smoke", StringComparison.OrdinalIgnoreCase))
        {
            try { RunVersionSmoke(); Console.WriteLine("Product version: PASS"); Environment.ExitCode = 0; }
            catch (Exception ex) { Console.Error.WriteLine("Product version: FAIL - " + ex.Message); Environment.ExitCode = 1; }
            return;
        }
        ApplicationConfiguration.Initialize();
        using var splash = new StartupSplashCoordinator();
        splash.Start();
        MainForm mainForm;
        try
        {
            // MainForm stays on the normal STA UI thread; construction runs
            // concurrently with the dedicated splash STA thread. No sleep
            // occurs before initialization begins.
            mainForm = new MainForm();
        }
        catch
        {
            // The using disposal closes the splash thread promptly; the
            // original exception is preserved for existing error reporting.
            throw;
        }
        using (mainForm)
        {
            bool splashClosed = false;
            mainForm.Shown += (_, _) =>
            {
                if (splashClosed)
                    return;
                splashClosed = true;
                // Minimum-visibility sleep runs off the UI thread so the main
                // window never freezes; the splash thread stays responsive.
                _ = Task.Run(() =>
                {
                    try { splash.SignalMainReady(); }
                    catch { }
                });
            };
            try
            {
                Application.Run(mainForm);
            }
            finally
            {
                // If Shown never fired (startup failure), close promptly.
                // Never swallows the original exception.
                if (!splashClosed)
                {
                    splashClosed = true;
                    try { splash.SignalMainReady(); }
                    catch { }
                }
            }
        }
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

    private static string RunBrandingAssetsSmoke()
    {
        var assembly = typeof(Program).Assembly;
        Require(string.Equals(assembly.GetName().Name, "Pi1ElviraEditor", StringComparison.Ordinal), "Assembly name is not Pi1ElviraEditor.");
        Require(AppInfo.ProductName == "π1 Elvira Editor", "Product name does not resolve to π1 Elvira Editor.");
        Require(AppInfo.ProductTitle == "π1 Elvira Editor v" + AppInfo.ProductVersion, "Product title does not resolve to π1 Elvira Editor.");
        Require(BrandingAssets.SplashResourceExists(), "Splash embedded resource is missing.");
        Require(assembly.GetManifestResourceNames().Contains(BrandingAssets.SplashResourceName, StringComparer.Ordinal), "Splash manifest resource name mismatch.");

        using (Image image = BrandingAssets.LoadSplashImage())
        {
            Require(image.Width > 0 && image.Height > 0, "Splash image has invalid dimensions.");
            double aspect = (double)image.Width / image.Height;
            Require(aspect > 1.0 && aspect < 3.0, $"Splash aspect ratio is insane: {aspect}.");
            foreach (Size working in new[] { new Size(1920, 1080), new Size(3840, 2160), new Size(1366, 768) })
            {
                Size fitted = StartupSplashForm.ComputeSplashSize(image.Size, working);
                Require(fitted.Width <= working.Width && fitted.Height <= working.Height, "Splash fit exceeds the working area.");
                Require(fitted.Width <= image.Width && fitted.Height <= image.Height, "Splash fit upscaled beyond the original artwork.");
                double fittedAspect = (double)fitted.Width / fitted.Height;
                Require(Math.Abs(fittedAspect - aspect) < 0.02, "Splash fit did not preserve the aspect ratio.");
            }
            Size fourK = StartupSplashForm.ComputeSplashSize(image.Size, new Size(3840, 2160));
            Require(fourK.Width <= image.Width && fourK.Height <= image.Height, "4K splash upscaled beyond the original artwork.");
        }

        // Embedded load already proved Downloads and EXE-sidecar PNGs are not
        // runtime dependencies: the image comes purely from the manifest.
        using (Image embedded = BrandingAssets.LoadSplashImage())
        {
            Require(embedded.Width > 0 && embedded.Height > 0, "Embedded splash reload failed.");
        }

        string icoPath = LocateBrandingIco();
        Require(File.Exists(icoPath), $"Application ICO was not found: {icoPath}.");
        IReadOnlyList<Size> frames = BrandingAssets.GetIcoFrameSizes(icoPath);
        foreach (int required in new[] { 16, 32, 48, 256 })
            Require(frames.Any(size => size.Width == required && size.Height == required), $"ICO is missing {required}x{required}.");

        try
        {
            // Explicitly validate the built application EXE. Never use
            // Environment.ProcessPath here: under `dotnet DLL` hosting it
            // points to dotnet.exe and would false-positive on the host icon.
            string exePath = Path.Combine(AppContext.BaseDirectory, "Pi1ElviraEditor.exe");
            Require(File.Exists(exePath), $"Built application EXE was not found: {exePath}.");
            Require(string.Equals(Path.GetFileName(exePath), "Pi1ElviraEditor.exe", StringComparison.OrdinalIgnoreCase), "Smoke is not validating Pi1ElviraEditor.exe.");

            Bitmap exeBitmap;
            try
            {
                using Icon? exeIcon = Icon.ExtractAssociatedIcon(exePath);
                if (exeIcon is null || exeIcon.Width <= 0 || exeIcon.Height <= 0)
                    throw new InvalidDataException("Pi1ElviraEditor.exe has no extractable icon.");
                using Bitmap provisional = exeIcon.ToBitmap();
                exeBitmap = new Bitmap(provisional);
            }
            catch (InvalidDataException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidDataException("Pi1ElviraEditor.exe icon extraction failed: " + ex.Message);
            }

            using (exeBitmap)
            {
                // The extracted EXE icon must pixel-match one configured ICO
                // frame, proving it is the application branding and not a
                // generic host icon.
                bool matchesConfiguredIco = false;
                try
                {
                    foreach (Size frameSize in BrandingAssets.GetIcoFrameSizes(icoPath))
                    {
                        try
                        {
                            using Bitmap frameBitmap = BrandingAssets.LoadIcoFrameBitmap(icoPath, frameSize);
                            if (BitmapsEqual(frameBitmap, exeBitmap))
                            {
                                matchesConfiguredIco = true;
                                break;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
                Require(matchesConfiguredIco, $"Pi1ElviraEditor.exe icon does not match the configured Assets\\Branding\\Pi1ElviraEditor.ico (extracted {exeBitmap.Width}x{exeBitmap.Height}).");

                // Self-check: when hosted via dotnet, prove the validated icon
                // is not merely the dotnet host icon.
                try
                {
                    string? hostPath = Environment.ProcessPath;
                    if (!string.IsNullOrWhiteSpace(hostPath) && File.Exists(hostPath) &&
                        !string.Equals(hostPath, exePath, StringComparison.OrdinalIgnoreCase))
                    {
                        using Icon? hostIcon = Icon.ExtractAssociatedIcon(hostPath);
                        if (hostIcon is not null)
                        {
                            using Bitmap hostBitmap = hostIcon.ToBitmap();
                            if (BitmapsEqual(hostBitmap, exeBitmap))
                                throw new InvalidDataException($"Pi1ElviraEditor.exe icon is indistinguishable from host '{hostPath}'.");
                        }
                    }
                }
                catch (InvalidDataException)
                {
                    throw;
                }
                catch
                {
                }
            }

            string exeDirectory = AppContext.BaseDirectory;
            DateTime currentWrite = File.GetLastWriteTimeUtc(assembly.Location);
            foreach (string oldName in new[] { "Pi1ElviraVgaEditor.exe", "Pi1ElviraVgaEditor.dll", "ElviraVgaEditor.exe", "ElviraVgaEditor.dll" })
            {
                string candidate = Path.Combine(exeDirectory, oldName);
                if (!File.Exists(candidate))
                    continue;
                DateTime oldWrite = File.GetLastWriteTimeUtc(candidate);
                if (oldWrite >= currentWrite - TimeSpan.FromMinutes(2))
                    throw new InvalidDataException($"Fresh old-identity output was produced beside the EXE: {oldName}.");
            }

            return exePath;
        }
        catch (InvalidDataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new InvalidDataException("EXE icon extraction failed: " + ex.Message);
        }
    }

    private static bool BitmapsEqual(Bitmap left, Bitmap right)
    {
        if (left.Size != right.Size)
            return false;
        for (int y = 0; y < left.Height; y++)
            for (int x = 0; x < left.Width; x++)
                if (left.GetPixel(x, y).ToArgb() != right.GetPixel(x, y).ToArgb())
                    return false;
        return true;
    }

    private static string LocateBrandingIco()
    {
        string? directory = AppContext.BaseDirectory;
        for (int level = 0; level < 9 && directory is not null; level++)
        {
            string candidate = Path.Combine(directory, "Assets", "Branding", "Pi1ElviraEditor.ico");
            if (File.Exists(candidate))
                return candidate;
            directory = Directory.GetParent(directory)?.FullName;
        }
        string fromCwd = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Branding", "Pi1ElviraEditor.ico");
        if (File.Exists(fromCwd))
            return fromCwd;
        return Path.Combine(AppContext.BaseDirectory, "Pi1ElviraEditor.ico");
    }

    private static void RunStartupSplashSmoke()
    {
        using (Image image = BrandingAssets.LoadSplashImage())
        {
            Require(image.Width > 0 && image.Height > 0, "Splash bitmap has invalid dimensions.");
            var sampleArea = new Rectangle(0, 0, 1920, 1040);
            using var form = new StartupSplashForm(image, sampleArea);
            Require(form.FormBorderStyle == FormBorderStyle.None, "Splash is not borderless.");
            Require(form.ShowInTaskbar == false, "Splash shows in the taskbar.");
            Require(form.ControlBox == false, "Splash exposes title-bar controls.");
            Require(form.MaximizeBox == false && form.MinimizeBox == false, "Splash exposes resize chrome.");
            Require(form.TopMost, "Splash is not topmost.");
            Require(form.SplashImageSizeForTest.Width == image.Width && form.SplashImageSizeForTest.Height == image.Height, "Splash does not use the expected embedded bitmap.");
            Rectangle bounds = form.ComputedBoundsForTest;
            Require(bounds.Width <= sampleArea.Width && bounds.Height <= sampleArea.Height, "Splash exceeds the working area.");
            Require(bounds.Width <= image.Width && bounds.Height <= image.Height, "Splash upscaled beyond the original artwork.");
            double originalAspect = (double)image.Width / image.Height;
            double boundsAspect = (double)bounds.Width / bounds.Height;
            Require(Math.Abs(originalAspect - boundsAspect) < 0.02, "Splash did not preserve the aspect ratio.");
            Require(bounds.X >= sampleArea.X && bounds.Y >= sampleArea.Y && bounds.Right <= sampleArea.Right && bounds.Bottom <= sampleArea.Bottom, "Splash is not contained in the working area.");
            foreach (Rectangle area in new[] { new Rectangle(0, 0, 1920, 1040), new Rectangle(0, 0, 3840, 2000), new Rectangle(100, 100, 1366, 700) })
            {
                Rectangle computed = StartupSplashForm.ComputeSplashBounds(image.Size, area);
                Require(computed.Width <= area.Width && computed.Height <= area.Height, "Splash policy exceeds the working area.");
                Require(computed.Width <= image.Width && computed.Height <= image.Height, "Splash policy upscaled beyond the original.");
                double computedAspect = (double)computed.Width / computed.Height;
                Require(Math.Abs(computedAspect - originalAspect) < 0.02, "Splash policy did not preserve the aspect ratio.");
                Require(computed.X >= area.X && computed.Y >= area.Y && computed.Right <= area.Right && computed.Bottom <= area.Bottom, "Splash policy is not contained in the working area.");
            }
        }

        var coordinator = new StartupSplashCoordinator(headlessTestMode: true);
        coordinator.Start();
        Require(coordinator.WaitForExit(TimeSpan.FromMilliseconds(100)) == false, "Headless splash exited before the main-ready signal.");
        coordinator.SignalMainReady();
        Require(coordinator.WaitForExit(TimeSpan.FromSeconds(6)), "Splash thread did not exit after the main-ready signal.");
        Require(!coordinator.IsThreadAliveForTest, "Background splash thread survived completion.");
        coordinator.Dispose();
        Require(!coordinator.IsThreadAliveForTest, "Splash thread survived Dispose.");

        var failure = new StartupSplashCoordinator(headlessTestMode: true);
        failure.Start();
        failure.Dispose();
        Require(!failure.IsThreadAliveForTest, "Failure-path splash thread survived Dispose.");
    }

    private static void RunVersionSmoke()
    {
        var assembly = typeof(Program).Assembly;
        string assemblyVersion = assembly.GetName().Version?.ToString() ?? string.Empty;
        string fileVersion = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion ?? string.Empty;
        string informational = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .Cast<System.Reflection.AssemblyInformationalVersionAttribute>().Single().InformationalVersion;
        if (AppInfo.ProductVersion != "1.0" || AppInfo.ProductTitle != "π1 Elvira Editor v1.0" ||
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

            // Fail-closed trust (V8.4 reconciliation): a noncanonical
            // generated image is identified and repaired in memory only.
            // Persisting it via SaveCopy must be BLOCKED by
            // RequireTrustedPatchTarget (tampered V5/V2 are not trusted
            // patch targets); the bad source must remain byte-identical
            // and no repaired file may appear. This intentionally replaces
            // the old expectation of an unsafe repair/write.
            bool blocked = false;
            try { RunVgaFontService.SaveCopy(oldBad, repaired); }
            catch (InvalidDataException ex) when (ex.Message.Contains("blocked", StringComparison.OrdinalIgnoreCase)) { blocked = true; }
            if (!blocked)
                throw new InvalidDataException($"{target.name}: SaveCopy persisted a noncanonical generated image instead of blocking.");
            if (File.Exists(repaired) || Hash(bad) != badHash)
                throw new InvalidDataException($"{target.name}: blocked SaveCopy still wrote output or touched the bad source.");
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
            if (!preview.Contains("Created: 25-08-2026 02:43") || !preview.Contains("Pi1 Elvira Editor v" + AppInfo.ProductVersion) || preview.Contains("call :", StringComparison.OrdinalIgnoreCase) || preview.Contains("exit /b", StringComparison.OrdinalIgnoreCase) || preview.Contains("color ", StringComparison.OrdinalIgnoreCase) || !preview.Contains("call PI1SND.BAT", StringComparison.OrdinalIgnoreCase))
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
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, variant, "EN"), VariantDirectoryOperationStatus.Created, "build fixture owned directory");
            RequireBuildStatus(builds.Build(project, variant, "EN"), DisposableVariantBuildStatus.Success, "initial pristine build");
            string variantRoot = directories.GetVariantEditionDirectoryPath(project, variant, "EN");
            File.WriteAllText(Path.Combine(variantRoot, "JUNK.TXT"), "junk");
            File.WriteAllText(Path.Combine(variantRoot, "GAMEPC"), "corrupt variant bytes");
            RequireBuildStatus(builds.Build(project, variant, "EN"), DisposableVariantBuildStatus.Success, "non-cumulative rebuild");
            if (File.Exists(Path.Combine(variantRoot, "JUNK.TXT")) || !File.ReadAllBytes(Path.Combine(variantRoot, "GAMEPC")).SequenceEqual(File.ReadAllBytes(Path.Combine(project.GameRoot, "GAMEPC"))))
                throw new InvalidDataException("Rebuild reused old variant payload or retained junk.");

            string prior = Path.Combine(variantRoot, "PRIOR.TXT"); File.WriteAllText(prior, "preserve on preflight failure");
            string immutable = Path.Combine(project.GameRoot, "RESOURCE.DAT"); byte[] immutableBytes = File.ReadAllBytes(immutable); File.WriteAllBytes(immutable, [7, 7, 7]);
            RequireBuildStatus(builds.Build(project, variant, "EN"), DisposableVariantBuildStatus.BaselineInvalid, "baseline gate before clear");
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
            foreach (VariantContext prepared in VariantContextCatalog.CreateBuiltIns(e1).Concat(VariantContextCatalog.CreateBuiltIns(e2)))
            {
                VariantDirectoryOperationResult preparedResult = directories.EnsureVariantEditionDirectory(prepared.Project, prepared, "EN");
                if (preparedResult.Status != VariantDirectoryOperationStatus.Created && preparedResult.Status != VariantDirectoryOperationStatus.AlreadyValid)
                    throw new InvalidDataException("Real baseline edition directory returned " + preparedResult.Status + ".");
            }
            DisposableVariantBuildResult[] results = VariantContextCatalog.CreateBuiltIns(e1).Concat(VariantContextCatalog.CreateBuiltIns(e2))
                .Select(variant => builds.Build(variant.Project, variant, "EN")).ToArray();
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

            RuntimeUiTextState e1State = service.SetOverride(e1, empty.State, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "Pauza – Unicode žľť");
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext e1Ega = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            RuntimeUiRuntimeProjection vga = service.GetEffectiveRecords(e1Vga, e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu);
            RuntimeUiRuntimeProjection ega = service.GetEffectiveRecords(e1Ega, e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu);
            if (vga is not { IsOverridden: true, TextOrigin: RuntimeUiTextOrigin.ProjectOverride, EffectiveText: "Pauza – Unicode žľť", MappingReadiness: RuntimeUiMappingReadiness.SupportedAndMapped })
                throw new InvalidDataException("Elvira I VGA did not project its runtime-scoped override.");
            // R9F V3 isolation: a VGA override must never leak into the EGA
            // view even though the logical record ID is identical.
            if (ega is not { IsOverridden: false, EffectiveText: null, MappingReadiness: RuntimeUiMappingReadiness.SupportedAndMapped })
                throw new InvalidDataException("VGA Runtime UI state leaked into the EGA projection.");
            // RUNVGA route evidence is frozen per record: Pause.menu is PROVEN
            // LIVE and R9F Phase 1 maps its 39-byte span (SupportedAndMapped);
            // the other listed binary routes stay KnownButMappingIncomplete
            // with PROVEN BY BINARY evidence. EGA evidence follows the frozen
            // bank descriptor per record (Pause.menu and Save.prompt are
            // PROVEN LIVE there, the rest PROVEN BY BINARY).
            if (vga.EvidenceStatus != RuntimeUiEvidenceStatus.ProvenLive ||
                ega.EvidenceStatus != RuntimeUiEvidenceStatus.ProvenLive)
                throw new InvalidDataException("Elvira I VGA/EGA route evidence regressed.");
            RuntimeUiRuntimeProjection vgaPrompt = service.GetEffectiveRecords(e1Vga, e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SavePrompt);
            if (vgaPrompt is not { MappingReadiness: RuntimeUiMappingReadiness.KnownButMappingIncomplete, EvidenceStatus: RuntimeUiEvidenceStatus.ProvenByBinary })
                throw new InvalidDataException("RUNVGA binary-routed record evidence regressed from PROVEN BY BINARY.");
            // Invalid saved Runtime UI overrides must keep blocking a full
            // configured build (this Pause text lacks the frozen hotspot line);
            // an empty project state must not block it. Valid Pause.menu
            // overrides are covered by the dedicated Pause smoke.
            if (new RuntimeUiProjectBuildStep(e1State).Preflight(e1, e1Vga) is null)
                throw new InvalidDataException("RUNVGA Runtime UI saved-override materialization became silently buildable.");
            if (new RuntimeUiProjectBuildStep(RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1)).Preflight(e1, e1Vga) is not null)
                throw new InvalidDataException("Empty Runtime UI project state falsely blocked a configured build.");
            RuntimeUiTextSaveResult saved = service.Save(e1, e1State);
            if (!saved.Succeeded || !File.ReadAllText(saved.Path).Contains("Pauza – Unicode žľť", StringComparison.Ordinal) ||
                File.ReadAllText(saved.Path).Contains("PointerSitePhysicalOffset", StringComparison.Ordinal))
                throw new InvalidDataException("Runtime UI override persistence was not sparse Unicode project data.");
            RuntimeUiTextLoadResult reloaded = service.Load(e1);
            if (!reloaded.IsSuccess || !reloaded.State!.Overrides.SequenceEqual(e1State.Overrides)) throw new InvalidDataException("Runtime UI JSON did not round-trip deterministically.");
            RuntimeUiTextState reset = service.RemoveOverride(e1, reloaded.State, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu);
            if (reset.Overrides.Count != 0 || service.GetEffectiveRecords(e1Vga, reset).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu).TextOrigin != RuntimeUiTextOrigin.FrozenDefaultUnavailable ||
                !service.Save(e1, reset).Succeeded || File.ReadAllText(service.GetPath(e1)).Contains("PauseMenu", StringComparison.Ordinal))
                throw new InvalidDataException("Runtime UI override reset did not return to the frozen/default source.");

            VariantContext runit = VariantContextCatalog.CreateBuiltIns(e2).Single();
            RuntimeUiTextState e2State = service.SetOverride(e2, service.Load(e2).State!, VariantRuntimeKind.Elvira2Vga, RuntimeUiLogicalRecordId.LoadFailure, "Načítanie zlyhalo");
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
            reject("{\"schemaVersion\":99,\"gameId\":\"Elvira2\",\"records\":[]}", RuntimeUiTextLoadStatus.UnsupportedSchema);
            reject("{\"schemaVersion\":1,\"gameId\":\"Elvira1\",\"records\":[]}", RuntimeUiTextLoadStatus.GameMismatch);
            reject("{\"schemaVersion\":1,\"gameId\":\"Elvira2\",\"records\":[{\"logicalRecordId\":\"SaveFailure\",\"text\":\"x\"},{\"logicalRecordId\":\"savefailure\",\"text\":\"y\"}]}", RuntimeUiTextLoadStatus.DuplicateLogicalId);
            reject("{\"schemaVersion\":1,\"gameId\":\"Elvira2\",\"records\":[{\"logicalRecordId\":\"FutureRoute\",\"text\":\"x\"}]}", RuntimeUiTextLoadStatus.UnknownLogicalId);
            // Schema v2 identity is (runtime, logical record): duplicates and
            // unknown runtimes fail closed with their own reasons.
            reject("{\"schemaVersion\":2,\"gameId\":\"Elvira2\",\"records\":[{\"runtime\":\"Elvira2Vga\",\"logicalRecordId\":\"SaveFailure\",\"text\":\"x\"},{\"runtime\":\"Elvira2Vga\",\"logicalRecordId\":\"SaveFailure\",\"text\":\"y\"}]}", RuntimeUiTextLoadStatus.DuplicateLogicalId);
            reject("{\"schemaVersion\":2,\"gameId\":\"Elvira2\",\"records\":[{\"runtime\":\"FutureRuntime\",\"logicalRecordId\":\"SaveFailure\",\"text\":\"x\"}]}", RuntimeUiTextLoadStatus.UnknownRuntime);
            reject("{\"schemaVersion\":2,\"gameId\":\"Elvira2\",\"records\":[{\"runtime\":\"Elvira2Vga\",\"logicalRecordId\":\"FutureRoute\",\"text\":\"x\"}]}", RuntimeUiTextLoadStatus.UnknownLogicalId);

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

            // R9F EGA contracts: project state stores frozen full records
            // (prefix + field + frozen tail). A raw message without structure
            // no longer validates: capacity fit alone never implies editability.
            RuntimeUiTextState e1State = texts.SetOverride(e1, texts.Load(e1).State!, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    Save failed.");
            RuntimeUiLayoutValidationResult validEga = layouts.Validate(ega, e1State, RuntimeUiLogicalRecordId.SaveFailure);
            if (validEga is not { Status: RuntimeUiLayoutValidationStatus.Valid, PayloadBytesIncludingTerminator: 18, RecordCapacity: 18 } ||
                !validEga.EncodedPayload.SequenceEqual(GamePcTextEditor.GetEncoding("CP852").GetBytes("\r    Save failed.")))
                throw new InvalidDataException("Valid EGA CP852 record did not validate deterministically.");
            RuntimeUiTextState unstructured = texts.SetOverride(e1, texts.Load(e1).State!, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
            if (layouts.Validate(ega, unstructured, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.HotspotViolation)
                throw new InvalidDataException("Unstructured EGA value lost its hotspot failure instead of a misleading Valid.");
            if (layouts.Validate(vga, e1State, RuntimeUiLogicalRecordId.SaveFailure) is not { Status: RuntimeUiLayoutValidationStatus.MappingIncomplete, RecordCapacity: null })
                throw new InvalidDataException("RUNVGA layout validation borrowed an unproven EGA record capacity or manufactured a span.");

            RuntimeUiTextState exact = texts.SetOverride(e1, e1State, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    " + new string('A', 12));
            RuntimeUiTextState oversized = texts.SetOverride(e1, exact, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    " + new string('A', 13));
            if (layouts.Validate(ega, exact, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.Valid ||
                layouts.Validate(ega, oversized, RuntimeUiLogicalRecordId.SaveFailure) is not { Status: RuntimeUiLayoutValidationStatus.RecordCapacityExceeded, PayloadBytesIncludingTerminator: 19 })
                throw new InvalidDataException("Record capacity/NUL boundary validation failed.");
            RuntimeUiTextState unsupported = texts.SetOverride(e1, e1State, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    bad \U0001F600");
            if (layouts.Validate(ega, unsupported, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.EncodingFailure)
                throw new InvalidDataException("Unsupported Unicode did not fail strict CP852 validation.");
            string reserved = "\r    " + GamePcTextEditor.GetEncoding("CP852").GetString([(byte)FontSlotMetadata.HudEraseGlyph]);
            RuntimeUiTextState protectedGlyph = texts.SetOverride(e1, e1State, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, reserved);
            if (layouts.Validate(ega, protectedGlyph, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.UnsupportedGlyph)
                throw new InvalidDataException("Reserved HUD glyph 0x81 was accepted as UI text.");
            RuntimeUiLayoutValidationBatchResult egaBatch = layouts.ValidateAll(ega, e1State);
            if (egaBatch.Records.Count != 8 || egaBatch.Records.Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.Valid ||
                egaBatch.Records.Any(value => value.Status == RuntimeUiLayoutValidationStatus.BankCapacityExceeded))
                throw new InvalidDataException("RUNEGA batch bank validation did not preserve its frozen layout.");

            RuntimeUiTextState e2State = texts.SetOverride(e2, texts.Load(e2).State!, VariantRuntimeKind.Elvira2Vga, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
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

    private static void VerifyRunVgaPauseMenuSmoke(string elvira1Source, string elvira2Source)
    {
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunVgaPauseSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            var texts = new RuntimeUiTextService();
            var layouts = new RuntimeUiLayoutValidationService(texts);
            // A. Pause original text can be read from the supported packed source.
            string packedRunVga = Path.Combine(elvira1Source, Elvira1ProductionProfile.ActiveVgaExecutable);
            if (!RunVgaPauseMenuService.TryDecodeFromFile(packedRunVga, out string? original, out _))
                throw new InvalidDataException("Pause.menu original could not be decoded from the supported RUNVGA source.");
            string expectedOriginal = RunVgaPauseMenuService.PauseOriginalText;
            if (!original!.Equals(expectedOriginal, StringComparison.Ordinal))
                throw new InvalidDataException("Decoded Pause.menu original diverges from frozen bytes.");
            if (!original.Contains("Game Paused", StringComparison.Ordinal) || !original.Contains("Continue", StringComparison.Ordinal) || !original.Contains("Quit", StringComparison.Ordinal))
                throw new InvalidDataException("Decoded Pause.menu original lost its formatted/hotspot content.");

            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RuntimeUiTextState empty = texts.Load(e1).State!;
            string validOverride = "     R9F PAUSE!\r\r\r Continue      Quit";
            RuntimeUiTextState validState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, validOverride);
            // B. Valid Pause override passes validation with the proven capacity.
            RuntimeUiLayoutValidationResult valid = layouts.Validate(vga, validState, RuntimeUiLogicalRecordId.PauseMenu);
            if (valid.Status != RuntimeUiLayoutValidationStatus.Valid || valid.RecordCapacity != RunVgaPauseMenuService.PauseSpanLengthIncludingNul)
                throw new InvalidDataException("Valid Pause.menu override did not validate: " + valid.Status + " / " + valid.Detail);
            // C. Envelope overflow is not a validation failure: geometry that
            // passes routes to appended materialization at build time.
            RuntimeUiTextState tooLong = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "     Game PAUSED!!\r\r\r Continue      Quit");
            if (layouts.Validate(vga, tooLong, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.Valid)
                throw new InvalidDataException("Over-envelope Pause.menu geometry was rejected at validation.");
            if (RunVgaVariableUiService.Decide(RuntimeUiLogicalRecordId.PauseMenu, "     Game PAUSED!!\r\r\r Continue      Quit") != RunVgaMaterializationKind.Appended)
                throw new InvalidDataException("Over-envelope Pause.menu was not routed to appended materialization.");
            // D. Embedded newline fails closed.
            RuntimeUiTextState newline = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "     Game\nPaused\r\r\r Continue      Quit");
            if (layouts.Validate(vga, newline, RuntimeUiLogicalRecordId.PauseMenu).IsValid)
                throw new InvalidDataException("Pause.menu newline override was accepted.");
            // E. Unrepresentable CP852 character fails.
            RuntimeUiTextState emoji = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "     Game \U0001F600\r\r\r Continue      Quit");
            if (layouts.Validate(vga, emoji, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.EncodingFailure)
                throw new InvalidDataException("Pause.menu emoji override was accepted.");
            // F. Fixed-column violation fails (hotspot keywords present but shifted).
            RuntimeUiTextState shifted = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "     Game Paused\r\r\r  Continue     Quit");
            if (layouts.Validate(vga, shifted, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.FixedColumnViolation)
                throw new InvalidDataException("Pause.menu fixed-column shift was accepted.");
            RuntimeUiTextState noIndent = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "Game Paused\r\r\r Continue      Quit");
            if (layouts.Validate(vga, noIndent, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.FixedColumnViolation)
                throw new InvalidDataException("Pause.menu missing leading spaces were accepted.");
            // G. Translated button labels with frozen anchors validate (the
            // old English-width hotspot rule is retired for these records).
            RuntimeUiTextState hotspot = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "     Game Paused\r\r\r Continue      Exit");
            if (layouts.Validate(vga, hotspot, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.Valid)
                throw new InvalidDataException("Translated Quit label was rejected.");
            // G2. Keywordless structural damage still fails as hotspot.
            RuntimeUiTextState damaged = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "     Game Paused\r\r\r Avanti Salir");
            if (layouts.Validate(vga, damaged, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.HotspotViolation)
                throw new InvalidDataException("Keywordless Pause.menu damage was accepted.");
            RuntimeUiTextState separators = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "     Game Paused Continue      Quit");
            if (layouts.Validate(vga, separators, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.FixedColumnViolation)
                throw new InvalidDataException("Pause.menu missing separators were accepted.");
            // T1. Title-only UX: "Pozastavené" (11 CP852 bytes incl. é) composes
            // the frozen record: 5 spaces + title + 3 CRs + frozen hotspot.
            if (!RunVgaPauseMenuService.TryComposeFromTitle("Pozastavené", out string composedTitle, out _, out _))
                throw new InvalidDataException("Valid Pause.menu title was rejected.");
            if (!composedTitle.Equals("     Pozastavené\r\r\r Continue      Quit", StringComparison.Ordinal))
                throw new InvalidDataException("Pause.menu title composition diverged from the frozen layout.");
            if (!RunVgaPauseMenuService.TryEncodePauseOverride(composedTitle, out byte[] titleSpan, out _, out _))
                throw new InvalidDataException("Composed Pause.menu title did not encode.");
            if (titleSpan.Length != RunVgaPauseMenuService.PauseSpanLengthIncludingNul ||
                !titleSpan.AsSpan(0, 5).SequenceEqual("     "u8.ToArray()) ||
                !titleSpan.AsSpan(5 + 11, 3).SequenceEqual([(byte)0x0D, (byte)0x0D, (byte)0x0D]) ||
                !GamePcTextEditor.GetEncoding("CP852").GetString(titleSpan, 5 + 11 + 3, 19).Equals(RunVgaPauseMenuService.PauseHotspotLine, StringComparison.Ordinal))
                throw new InvalidDataException("Composed Pause.menu title broke the 39-byte span anatomy.");
            // T2. Title UX rejections: newline, control, empty (title-only
            // composer keeps its frozen in-place contract). Length beyond
            // English 11 is governed by the proven one-line visual bound
            // (16 title columns) in the variable composer: 12 chars pass
            // (appended), 17 chars fail as row overflow.
            if (RunVgaPauseMenuService.TryComposeFromTitle("Pozastavené!", out _, out _, out _))
                throw new InvalidDataException("12-character Pause.menu title was accepted.");
            if (RunVgaPauseMenuService.TryComposeFromTitle("A\nB", out _, out _, out _) ||
                RunVgaPauseMenuService.TryComposeFromTitle("A\x01B", out _, out _, out _) ||
                RunVgaPauseMenuService.TryComposeFromTitle(string.Empty, out _, out _, out _))
                throw new InvalidDataException("Pause.menu newline/control/empty title was accepted.");
            // T3. Title round-trips: save composed full record, reload, extract
            // the semantic title for grid display.
            RuntimeUiTextState titleState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, composedTitle);
            RuntimeUiTextSaveResult titleSaved = texts.Save(e1, titleState);
            if (!titleSaved.Succeeded) throw new InvalidDataException("Pause.menu title project save failed.");
            RuntimeUiTextLoadResult titleReloaded = texts.Load(e1);
            if (!titleReloaded.IsSuccess ||
                !RunVgaPauseMenuService.TryExtractTitle(titleReloaded.State!.Overrides.Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu).Text, out string? reloadedTitle) ||
                !reloadedTitle!.Equals("Pozastavené", StringComparison.Ordinal))
                throw new InvalidDataException("Pause.menu title did not survive project save/load.");
            RuntimeUiTextState titleReset = texts.RemoveOverride(e1, titleReloaded.State, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu);
            if (titleReset.Overrides.Count != 0 || !texts.Save(e1, titleReset).Succeeded)
                throw new InvalidDataException("Pause.menu title reset regressed.");
            // T4. Legacy full-record overrides stay compatible: extract shows
            // the title and recomposition is byte-identical to storage.
            if (!RunVgaPauseMenuService.TryExtractTitle(validOverride, out string? legacyTitle) ||
                !legacyTitle!.Equals("R9F PAUSE!", StringComparison.Ordinal))
                throw new InvalidDataException("Legacy full-record Pause.menu override lost its title.");
            if (!RunVgaPauseMenuService.TryComposeFromTitle(legacyTitle, out string recomposed, out _, out _) ||
                !recomposed.Equals(validOverride, StringComparison.Ordinal))
                throw new InvalidDataException("Legacy full-record recomposition diverged.");
            // T5. No-op materialization: title "Game Paused" composes the
            // pristine record; applying it to a default V5 may be
            // byte-identical and must still verify with size unchanged.
            if (!RunVgaPauseMenuService.TryComposeFromTitle("Game Paused", out string noopFull, out _, out _))
                throw new InvalidDataException("Pristine Pause.menu title was rejected.");
            if (!noopFull.Equals(RunVgaPauseMenuService.PauseOriginalText, StringComparison.Ordinal))
                throw new InvalidDataException("No-op Pause.menu composition diverged from pristine bytes.");
            string noopSrc = Path.Combine(root, "noop-pristine-runvga.exe");
            string noopV5 = Path.Combine(root, "noop-pristine-v5.exe");
            string noopPristineHash = HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable));
            File.Copy(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable), noopSrc, true);
            RunVgaBootstrapService.CreateExtendedCp852(noopSrc, noopV5, GlyphRepository.CreateAllCp852Slots());
            byte[] noopBefore = File.ReadAllBytes(noopV5);
            byte[] noopAfter = (byte[])noopBefore.Clone();
            RunVgaPauseMenuService.ApplyToImage(noopAfter, noopFull);
            RunVgaPauseMenuService.VerifyOnlyPauseBytesChanged(noopBefore, noopAfter);
            if (noopBefore.Length != noopAfter.Length)
                throw new InvalidDataException("No-op Pause.menu materialization changed executable size.");
            if (HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable)) != noopPristineHash)
                throw new InvalidDataException("No-op Pause.menu check modified the pristine source.");
            // V1. Variable-width Slovak triple composes POC-exact bytes with
            // frozen anchors (no English-width limits anywhere).
            var skPauseEdits = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["title"] = "Pozastavené",
                ["continue"] = "Pokračuj",
                ["quit"] = "Koniec"
            };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.PauseMenu, skPauseEdits, out string skPauseFull, out _, out string skPauseDetail))
                throw new InvalidDataException("Slovak Pause.menu triple was rejected: " + skPauseDetail);
            if (!skPauseFull.Equals("     Pozastavené\r\r\r Pokračuj      Koniec", StringComparison.Ordinal))
                throw new InvalidDataException("Slovak Pause.menu composition diverged from the POC bytes.");
            string skPauseLine = skPauseFull.Split("\r\r\r")[1];
            if (skPauseLine.IndexOf("Pokračuj", StringComparison.Ordinal) != 1 || skPauseLine.IndexOf("Koniec", StringComparison.Ordinal) != 15)
                throw new InvalidDataException("Slovak Pause.menu anchors moved.");
            if (skPauseLine.Contains('\n') || skPauseFull.Split("\r\r\r").Length != 2)
                throw new InvalidDataException("Slovak Pause.menu broke the one-line option row.");
            if (RunVgaVariableUiService.Decide(RuntimeUiLogicalRecordId.PauseMenu, skPauseFull) != RunVgaMaterializationKind.Appended)
                throw new InvalidDataException("Over-envelope Pause.menu was not routed to appended materialization.");
            // V1b. 12-char titles pass the visual bound (appended); 17 fail.
            var twelveTitle = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["title"] = "Pozastavené!",
                ["continue"] = "Continue",
                ["quit"] = "Quit"
            };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.PauseMenu, twelveTitle, out _, out _, out _))
                throw new InvalidDataException("12-character Pause.menu title was rejected.");
            var seventeenTitle = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["title"] = "12345678901234567",
                ["continue"] = "Continue",
                ["quit"] = "Quit"
            };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.PauseMenu, seventeenTitle, out _, out RunVgaVariableFailureKind seventeenFailure, out _))
                throw new InvalidDataException("17-character Pause.menu title was accepted.");
            if (seventeenFailure != RunVgaVariableFailureKind.RowOverflow)
                throw new InvalidDataException("17-character title did not fail as row overflow.");
            // V2. A Continue translation reaching the frozen Quit anchor is
            // rejected as VISUAL COLLISION, never as "max 8 chars".
            var colliding = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["title"] = "Game Paused",
                ["continue"] = "12345678901234",
                ["quit"] = "Quit"
            };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.PauseMenu, colliding, out _, out RunVgaVariableFailureKind collideFailure, out string collideDetail))
                throw new InvalidDataException("Colliding Continue translation was accepted.");
            if (collideFailure != RunVgaVariableFailureKind.VisualCollision || !collideDetail.Contains("Continue", StringComparison.Ordinal) || !collideDetail.Contains("Quit", StringComparison.Ordinal) || collideDetail.Contains("8", StringComparison.Ordinal))
                throw new InvalidDataException("Collision rejection is not anchor-based: " + collideDetail);
            // V3. An excessively long Quit is rejected only by the proven
            // visual row bound (21), never by English length 4.
            var longQuit = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["title"] = "Game Paused",
                ["continue"] = "Continue",
                ["quit"] = "1234567"
            };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.PauseMenu, longQuit, out _, out RunVgaVariableFailureKind quitFailure, out string quitDetail))
                throw new InvalidDataException("Overflowing Quit label was accepted.");
            if (quitFailure != RunVgaVariableFailureKind.RowOverflow || !quitDetail.Contains("21", StringComparison.Ordinal))
                throw new InvalidDataException("Quit rejection is not row-bound-based: " + quitDetail);
            // V5. Legacy title-only migration: saved title carries over,
            // Continue/Quit reset to English originals, byte-identical for
            // title-only states.
            if (!RunVgaVariableUiService.TryMigratePauseTitle(validOverride, out string migratedTitle, out string migratedFull))
                throw new InvalidDataException("Legacy Pause title migration failed.");
            if (!migratedTitle.Equals("R9F PAUSE!", StringComparison.Ordinal) || !migratedFull.Equals(validOverride, StringComparison.Ordinal))
                throw new InvalidDataException("Legacy Pause title migration diverged.");
            // H. Project save/reload roundtrip preserves the override only.
            RuntimeUiTextSaveResult saved = texts.Save(e1, validState);
            if (!saved.Succeeded) throw new InvalidDataException("Pause.menu project save failed: " + saved.Detail);
            RuntimeUiTextLoadResult reloaded = texts.Load(e1);
            if (!reloaded.IsSuccess || !reloaded.State!.Overrides.SequenceEqual(validState.Overrides))
                throw new InvalidDataException("Pause.menu project roundtrip diverged.");
            if (File.ReadAllText(saved.Path).Contains("PointerSitePhysicalOffset", StringComparison.Ordinal))
                throw new InvalidDataException("Pause.menu project state leaked binary layout metadata.");

            // I/J/K/L. CompositeBuild end-to-end (SK translated V5 variant).
            var directories = new VariantDirectoryService();
            var translations = new TranslationProjectService();
            var graphics = new GraphicsVariantService();
            TranslationProjectState textState = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant sk = translations.Create(e1, textState, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            textState = translations.Add(textState, sk); translations.Save(e1, textState);
            GraphicsProjectState graphicsState = GraphicsProjectState.Empty(ElviraGameProfile.Elvira1);
            string pristinePackedHash = HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable));
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, vga, sk.Code), VariantDirectoryOperationStatus.Created, "pause fixture variant directory");
            CompositeBuildService pauseBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, validState)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            CompositeBuildResult pauseResult = pauseBuild.Build(e1, vga, CompositeBuildMode.Full);
            if (pauseResult.Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Pause.menu composite build failed: " + pauseResult.Status + " / " + pauseResult.Stages.Last().Detail);
            string variantRoot = directories.GetVariantEditionDirectoryPath(e1, vga, sk.Code);
            string patchedV5 = Path.Combine(variantRoot, ActiveProjectBuildIdentity.ExecutableName(vga, sk));
            if (!File.Exists(patchedV5))
                throw new InvalidDataException("Patched V5 variant output is missing.");
            byte[] after = File.ReadAllBytes(patchedV5);
            // Independent pristine V5 expectation from the same packed source.
            string tmpSrc = Path.Combine(root, "tmp-pristine-runvga.exe");
            string tmpV5 = Path.Combine(root, "tmp-pristine-v5.exe");
            File.Copy(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable), tmpSrc, true);
            RunVgaBootstrapService.CreateExtendedCp852(tmpSrc, tmpV5, GlyphRepository.CreateAllCp852Slots());
            byte[] beforeV5 = File.ReadAllBytes(tmpV5);
            RunVgaPauseMenuService.VerifyOnlyPauseBytesChanged(beforeV5, after);
            // L. Generated variant contains the expected Pause text.
            if (!RunVgaPauseMenuService.TryDecodePause(after, out string? patchedText, out _))
                throw new InvalidDataException("Patched V5 Pause.menu did not decode.");
            if (!patchedText!.Equals(validOverride, StringComparison.Ordinal))
                throw new InvalidDataException("Patched V5 Pause.menu text mismatch.");
            // V4. Variable-width Slovak Pause materializes via one
            // deterministic append: grown image, patched callsite, thunk,
            // relocated record, relocations, margin fix, whitelist shape.
            RuntimeUiTextState skPauseState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, skPauseFull);
            CompositeBuildService skPauseBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, skPauseState)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skPauseBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Slovak Pause.menu composite build failed.");
            byte[] skPatched = File.ReadAllBytes(patchedV5);
            if (skPatched.Length != beforeV5.Length + 0x200)
                throw new InvalidDataException("Appended Slovak Pause.menu output has the wrong size.");
            if (!skPatched.AsSpan(0xCCF7, 5).SequenceEqual(new byte[] { 0x9A, 0x00, 0x00, 0x26, 0x39 }))
                throw new InvalidDataException("Pause callsite was not redirected to the append thunk.");
            int skAppend = beforeV5.Length;
            byte[] expectedThunk = [0x52, 0xB6, 0x00, 0xBF, 0xC0, 0x00, 0x2E, 0x8A, 0x15, 0x47, 0x0A, 0xD2, 0x74, 0x06, 0x0E, 0xE8, 0x07, 0x00,
                0xEB, 0xF2, 0xBF, 0xDC, 0x2C, 0x5A, 0xCB, 0xBB, 0x75, 0x02, 0x53, 0xEA, 0x76, 0x02, 0xBD, 0x0C];
            if (!skPatched.AsSpan(skAppend, expectedThunk.Length).SequenceEqual(expectedThunk))
                throw new InvalidDataException("Appended Pause thunk diverged from the POC structure.");
            byte[] skPayload = GamePcTextEditor.GetEncoding("CP852").GetBytes(skPauseFull);
            if (!skPatched.AsSpan(skAppend + 0xC0, skPayload.Length).SequenceEqual(skPayload) || skPatched[skAppend + 0xC0 + skPayload.Length] != 0x00)
                throw new InvalidDataException("Appended Pause record diverged from the POC bytes.");
            if (skPatched[0x1163A] != 0xA0)
                throw new InvalidDataException("Appended output missed the memory-margin fix.");
            if (!skPatched.AsSpan(0xCD29, 5).SequenceEqual(new byte[] { 0x9A, 0x92, 0x00, 0xBD, 0x0C }) ||
                !skPatched.AsSpan(0xCBFC, 5).SequenceEqual(new byte[] { 0x9A, 0x92, 0x00, 0xBD, 0x0C }))
                throw new InvalidDataException("Unrelated callsites changed during Pause append.");
            if (!skPatched.AsSpan(0x1A625, 0x1A733 - 0x1A625 + 1).SequenceEqual(beforeV5.AsSpan(0x1A625, 0x1A733 - 0x1A625 + 1)))
                throw new InvalidDataException("Original 8-record block changed during Pause append.");
            string skHash = HashFile(patchedV5);
            if (skPauseBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !HashFile(patchedV5).Equals(skHash, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Slovak Pause.menu rebuild was not deterministic.");
            if (HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable)) != pristinePackedHash)
                throw new InvalidDataException("Slovak Pause.menu build modified the pristine RUNVGA source.");
            // K. Pristine source RUNVGA remains byte-identical.
            if (HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable)) != pristinePackedHash)
                throw new InvalidDataException("Pause.menu build modified the pristine RUNVGA source.");
            // J. A second Runtime UI override FOR THIS RUNTIME fails closed.
            // (EGA-scoped overrides in the same file never block a VGA build.)
            RuntimeUiTextState both = texts.SetOverride(e1, validState, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
            RuntimeUiTextState egaOnly = texts.SetOverride(e1, validState, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    EGA only.");
            CompositeBuildService blockedBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, both)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (blockedBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.PreflightFailed)
                throw new InvalidDataException("Second Runtime UI override did not fail the build closed.");
            // J2. An EGA-scoped override in shared project state does NOT
            // block the VGA build and is never materialized into it.
            CompositeBuildService egaScopedBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, egaOnly)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (egaScopedBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("EGA-scoped override falsely blocked the VGA build.");
            // M. Route evidence unchanged.
            Elvira1ProductionProfile.VerifyFrozenInvariants();
            if (Elvira1ProductionProfile.RunVgaRouteEvidence(RuntimeUiLogicalRecordId.PauseMenu) != FrozenRuntimeEvidence.ProvenLive)
                throw new InvalidDataException("Pause.menu route evidence regressed.");
            foreach (RuntimeUiLogicalRecordId id in new[] { RuntimeUiLogicalRecordId.ConfirmGeneric, RuntimeUiLogicalRecordId.SavePrompt, RuntimeUiLogicalRecordId.SaveFailure, RuntimeUiLogicalRecordId.LoadFailure, RuntimeUiLogicalRecordId.FileNotFound, RuntimeUiLogicalRecordId.TryAnotherDisk, RuntimeUiLogicalRecordId.SaveOverwrite })
                if (Elvira1ProductionProfile.RunVgaRouteEvidence(id) != FrozenRuntimeEvidence.ProvenByBinary)
                    throw new InvalidDataException("RUNVGA route evidence regressed for " + id);
            // V6. Proven-live mapping: the three structured records project
            // SupportedAndMapped/ProvenLive; the other five stay incomplete.
            var mappingTexts = new RuntimeUiTextService();
            RuntimeUiTextState mappingEmpty = mappingTexts.Load(e1).State!;
            foreach (RuntimeUiLogicalRecordId id in new[] { RuntimeUiLogicalRecordId.PauseMenu, RuntimeUiLogicalRecordId.ConfirmGeneric, RuntimeUiLogicalRecordId.SaveOverwrite })
            {
                RuntimeUiRuntimeProjection projection = mappingTexts.GetEffectiveRecords(vga, mappingEmpty).Single(value => value.LogicalRecordId == id);
                if (projection.MappingReadiness != RuntimeUiMappingReadiness.SupportedAndMapped || projection.EvidenceStatus != RuntimeUiEvidenceStatus.ProvenLive)
                    throw new InvalidDataException("Proven-live VGA mapping regressed for " + id);
            }
            foreach (RuntimeUiLogicalRecordId id in new[] { RuntimeUiLogicalRecordId.SavePrompt, RuntimeUiLogicalRecordId.SaveFailure, RuntimeUiLogicalRecordId.LoadFailure, RuntimeUiLogicalRecordId.FileNotFound, RuntimeUiLogicalRecordId.TryAnotherDisk })
            {
                if (mappingTexts.GetEffectiveRecords(vga, mappingEmpty).Single(value => value.LogicalRecordId == id).MappingReadiness != RuntimeUiMappingReadiness.KnownButMappingIncomplete)
                    throw new InvalidDataException("VGA mapping leaked beyond the proven-live records: " + id);
            }
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunVgaConfirmSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V5: Confirm.generic is proven-live variable-width. Slovak
        // values compose POC-exact bytes with frozen anchors; overflow
        // routes to appended materialization deterministically.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunVgaConfirmSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            var texts = new RuntimeUiTextService();
            var layouts = new RuntimeUiLayoutValidationService(texts);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RuntimeUiTextState empty = texts.Load(e1).State!;
            // A. Authoritative original + anchors + semantic parse.
            if (!RunVgaVariableUiService.TryDecodeOriginalFromGameRoot(e1.GameRoot, RuntimeUiLogicalRecordId.ConfirmGeneric, out string? original, out _) ||
                !original!.Equals("    Are you sure ?\r\r\r     Yes       No", StringComparison.Ordinal))
                throw new InvalidDataException("Confirm.generic original diverged.");
            if (!RunVgaVariableUiService.TryParse(RuntimeUiLogicalRecordId.ConfirmGeneric, original, out IReadOnlyDictionary<string, string>? fields) || fields is null ||
                !fields["prompt"].Equals("Are you sure ?", StringComparison.Ordinal) || !fields["yes"].Equals("Yes", StringComparison.Ordinal) || !fields["no"].Equals("No", StringComparison.Ordinal))
                throw new InvalidDataException("Confirm.generic semantic parse diverged.");
            // B. POC-exact record: English prompt with Áno/Nie (40 bytes).
            var pocEdits = new Dictionary<string, string>(StringComparer.Ordinal) { ["prompt"] = "Are you sure ?", ["yes"] = "Áno", ["no"] = "Nie" };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.ConfirmGeneric, pocEdits, out string pocFull, out _, out _))
                throw new InvalidDataException("POC Confirm.generic composition failed.");
            if (!pocFull.Equals("    Are you sure ?\r\r\r     Áno       Nie", StringComparison.Ordinal))
                throw new InvalidDataException("POC Confirm.generic bytes diverged.");
            // C. Slovak triple: anchors frozen, no English 3/2 limitation.
            var skEdits = new Dictionary<string, string>(StringComparer.Ordinal) { ["prompt"] = "Si si istý?", ["yes"] = "Áno", ["no"] = "Nie" };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.ConfirmGeneric, skEdits, out string skFull, out _, out string skDetail))
                throw new InvalidDataException("Slovak Confirm.generic was rejected: " + skDetail);
            if (!skFull.Equals("    Si si istý?\r\r\r     Áno       Nie", StringComparison.Ordinal))
                throw new InvalidDataException("Slovak Confirm.generic bytes diverged.");
            string skLine = skFull.Split("\r\r\r")[1];
            if (skLine.IndexOf("Áno", StringComparison.Ordinal) != 5 || skLine.IndexOf("Nie", StringComparison.Ordinal) != 15)
                throw new InvalidDataException("Slovak Confirm.generic anchors moved.");
            // D. Slovak Confirm fits the envelope: in-place materialization.
            if (RunVgaVariableUiService.Decide(RuntimeUiLogicalRecordId.ConfirmGeneric, skFull) != RunVgaMaterializationKind.InPlace)
                throw new InvalidDataException("Fitting Confirm.generic was not routed in place.");
            RuntimeUiTextState skState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.ConfirmGeneric, skFull);
            if (layouts.Validate(vga, skState, RuntimeUiLogicalRecordId.ConfirmGeneric).Status != RuntimeUiLayoutValidationStatus.Valid)
                throw new InvalidDataException("Slovak Confirm.generic did not validate.");
            // E. Collision (Yes reaching the No anchor) fails as geometry.
            var collide = new Dictionary<string, string>(StringComparer.Ordinal) { ["prompt"] = "Are you sure ?", ["yes"] = "1234567890", ["no"] = "No" };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.ConfirmGeneric, collide, out _, out RunVgaVariableFailureKind collideFailure, out _))
                throw new InvalidDataException("Colliding Yes translation was accepted.");
            if (collideFailure != RunVgaVariableFailureKind.VisualCollision)
                throw new InvalidDataException("Yes collision did not fail as geometry collision.");
            // F. Overlong No (4 chars, row 19 > 18) fails as row overflow.
            var longNo = new Dictionary<string, string>(StringComparer.Ordinal) { ["prompt"] = "Are you sure ?", ["yes"] = "Yes", ["no"] = "Nein" };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.ConfirmGeneric, longNo, out _, out RunVgaVariableFailureKind noFailure, out string noDetail))
                throw new InvalidDataException("Overflowing No label was accepted.");
            if (noFailure != RunVgaVariableFailureKind.RowOverflow || !noDetail.Contains("18", StringComparison.Ordinal))
                throw new InvalidDataException("No rejection is not row-bound-based: " + noDetail);
            // G. Semantic save/reload + Reset round trip.
            if (!texts.Save(e1, skState).Succeeded || !texts.Load(e1).IsSuccess)
                throw new InvalidDataException("Confirm.generic project save/load regressed.");
            RuntimeUiTextState reset = texts.RemoveOverride(e1, texts.Load(e1).State!, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.ConfirmGeneric);
            if (reset.Overrides.Count != 0 || !texts.Save(e1, reset).Succeeded)
                throw new InvalidDataException("Confirm.generic reset regressed.");
            // H. Overflow Confirm (46 bytes) materializes via append.
            var maxEdits = new Dictionary<string, string>(StringComparer.Ordinal) { ["prompt"] = "12345678901234567", ["yes"] = "123456789", ["no"] = "Nie" };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.ConfirmGeneric, maxEdits, out string maxFull, out _, out _))
                throw new InvalidDataException("Max-width Confirm.generic was rejected.");
            if (RunVgaVariableUiService.Decide(RuntimeUiLogicalRecordId.ConfirmGeneric, maxFull) != RunVgaMaterializationKind.Appended)
                throw new InvalidDataException("Over-envelope Confirm.generic was not routed to append.");
            var translations = new TranslationProjectService();
            var graphics = new GraphicsVariantService();
            TranslationProjectState textState = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant sk = translations.Create(e1, textState, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            textState = translations.Add(textState, sk); translations.Save(e1, textState);
            GraphicsProjectState graphicsState = GraphicsProjectState.Empty(ElviraGameProfile.Elvira1);
            string pristinePackedHash = HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable));
            var directories = new VariantDirectoryService();
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, vga, sk.Code), VariantDirectoryOperationStatus.Created, "confirm fixture variant directory");
            RuntimeUiTextState maxState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.ConfirmGeneric, maxFull);
            CompositeBuildService maxBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, maxState)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (maxBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Overflow Confirm.generic composite build failed.");
            string variantRoot = directories.GetVariantEditionDirectoryPath(e1, vga, sk.Code);
            string patchedV5 = Path.Combine(variantRoot, ActiveProjectBuildIdentity.ExecutableName(vga, sk));
            byte[] patched = File.ReadAllBytes(patchedV5);
            string tmpSrc = Path.Combine(root, "tmp-confirm-runvga.exe");
            string tmpV5 = Path.Combine(root, "tmp-confirm-v5.exe");
            File.Copy(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable), tmpSrc, true);
            RunVgaBootstrapService.CreateExtendedCp852(tmpSrc, tmpV5, GlyphRepository.CreateAllCp852Slots());
            byte[] beforeV5 = File.ReadAllBytes(tmpV5);
            if (patched.Length != beforeV5.Length + 0x200)
                throw new InvalidDataException("Appended Confirm.generic output has the wrong size.");
            if (!patched.AsSpan(0xCD29, 5).SequenceEqual(new byte[] { 0x9A, 0x40, 0x00, 0x26, 0x39 }))
                throw new InvalidDataException("Confirm callsite was not redirected to its thunk.");
            if (!patched.AsSpan(0xCCF7, 5).SequenceEqual(new byte[] { 0x9A, 0x92, 0x00, 0xBD, 0x0C }) ||
                !patched.AsSpan(0xCBFC, 5).SequenceEqual(new byte[] { 0x9A, 0x92, 0x00, 0xBD, 0x0C }))
                throw new InvalidDataException("Unrelated callsites changed during Confirm append.");
            byte[] maxPayload = GamePcTextEditor.GetEncoding("CP852").GetBytes(maxFull);
            if (!patched.AsSpan(beforeV5.Length + 0x100, maxPayload.Length).SequenceEqual(maxPayload))
                throw new InvalidDataException("Appended Confirm record diverged.");
            string firstHash = HashFile(patchedV5);
            if (maxBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !HashFile(patchedV5).Equals(firstHash, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Confirm.generic rebuild was not deterministic.");
            if (HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable)) != pristinePackedHash)
                throw new InvalidDataException("Confirm.generic build modified the pristine RUNVGA source.");
            // I. In-place Slovak Confirm build keeps the size and span.
            CompositeBuildService skBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, skState)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("In-place Slovak Confirm.generic build failed.");
            byte[] inPlace = File.ReadAllBytes(patchedV5);
            if (inPlace.Length != beforeV5.Length)
                throw new InvalidDataException("In-place Confirm.generic build changed executable size.");
            if (!RunVgaVariableUiService.TryDecodeSpan(inPlace, RuntimeUiLogicalRecordId.ConfirmGeneric, out string? roundTrip, out _) || !roundTrip!.Equals(skFull, StringComparison.Ordinal))
                throw new InvalidDataException("In-place Confirm.generic round trip diverged.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunVgaOverwriteSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V5: Save.overwrite is proven-live variable-width (message,
        // question, Yes, No). The Slovak triple fits the envelope and stays
        // in place; a max-width triple exercises the append path.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunVgaOverwriteSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            var texts = new RuntimeUiTextService();
            var layouts = new RuntimeUiLayoutValidationService(texts);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RuntimeUiTextState empty = texts.Load(e1).State!;
            // A. Authoritative original + semantic parse.
            if (!RunVgaVariableUiService.TryDecodeOriginalFromGameRoot(e1.GameRoot, RuntimeUiLogicalRecordId.SaveOverwrite, out string? original, out _) ||
                !original!.Equals("\r File already exists.\r\r    Overwrite it ?\r\r     Yes       No", StringComparison.Ordinal))
                throw new InvalidDataException("Save.overwrite original diverged.");
            if (!RunVgaVariableUiService.TryParse(RuntimeUiLogicalRecordId.SaveOverwrite, original, out IReadOnlyDictionary<string, string>? fields) || fields is null ||
                !fields["message"].Equals("File already exists.", StringComparison.Ordinal) || !fields["question"].Equals("Overwrite it ?", StringComparison.Ordinal) ||
                !fields["yes"].Equals("Yes", StringComparison.Ordinal) || !fields["no"].Equals("No", StringComparison.Ordinal))
                throw new InvalidDataException("Save.overwrite semantic parse diverged.");
            // B. POC-exact record (63 bytes).
            var pocEdits = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["message"] = "File already exists.",
                ["question"] = "Overwrite it ?",
                ["yes"] = "Áno",
                ["no"] = "Nie"
            };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.SaveOverwrite, pocEdits, out string pocFull, out _, out _))
                throw new InvalidDataException("POC Save.overwrite composition failed.");
            if (!pocFull.Equals("\r File already exists.\r\r    Overwrite it ?\r\r     Áno       Nie", StringComparison.Ordinal))
                throw new InvalidDataException("POC Save.overwrite bytes diverged.");
            // C. Slovak triple (57 bytes): anchors frozen, in place.
            var skEdits = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["message"] = "Súbor už existuje.",
                ["question"] = "Prepísať?",
                ["yes"] = "Áno",
                ["no"] = "Nie"
            };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.SaveOverwrite, skEdits, out string skFull, out _, out string skDetail))
                throw new InvalidDataException("Slovak Save.overwrite was rejected: " + skDetail);
            if (!skFull.Equals("\r Súbor už existuje.\r\r    Prepísať?\r\r     Áno       Nie", StringComparison.Ordinal))
                throw new InvalidDataException("Slovak Save.overwrite bytes diverged.");
            string skButtonLine = skFull.Split("\r\r")[2];
            if (skButtonLine.IndexOf("Áno", StringComparison.Ordinal) != 5 || skButtonLine.IndexOf("Nie", StringComparison.Ordinal) != 15)
                throw new InvalidDataException("Slovak Save.overwrite anchors moved.");
            if (RunVgaVariableUiService.Decide(RuntimeUiLogicalRecordId.SaveOverwrite, skFull) != RunVgaMaterializationKind.InPlace)
                throw new InvalidDataException("Fitting Save.overwrite was not routed in place.");
            RuntimeUiTextState skState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.SaveOverwrite, skFull);
            if (layouts.Validate(vga, skState, RuntimeUiLogicalRecordId.SaveOverwrite).Status != RuntimeUiLayoutValidationStatus.Valid)
                throw new InvalidDataException("Slovak Save.overwrite did not validate.");
            // D. Collision/overflow geometry (never English 3/2 limits).
            var collide = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["message"] = "File already exists.",
                ["question"] = "Overwrite it ?",
                ["yes"] = "1234567890",
                ["no"] = "No"
            };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.SaveOverwrite, collide, out _, out RunVgaVariableFailureKind collideFailure, out _))
                throw new InvalidDataException("Colliding Yes translation was accepted.");
            if (collideFailure != RunVgaVariableFailureKind.VisualCollision)
                throw new InvalidDataException("Overwrite Yes collision did not fail as geometry collision.");
            var longNo = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["message"] = "File already exists.",
                ["question"] = "Overwrite it ?",
                ["yes"] = "Yes",
                ["no"] = "Nein"
            };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.SaveOverwrite, longNo, out _, out RunVgaVariableFailureKind noFailure, out string noDetail))
                throw new InvalidDataException("Overflowing No label was accepted.");
            if (noFailure != RunVgaVariableFailureKind.RowOverflow || !noDetail.Contains("18", StringComparison.Ordinal))
                throw new InvalidDataException("Overwrite No rejection is not row-bound-based: " + noDetail);
            // E. Semantic save/reload + Reset round trip.
            if (!texts.Save(e1, skState).Succeeded || !texts.Load(e1).IsSuccess)
                throw new InvalidDataException("Save.overwrite project save/load regressed.");
            RuntimeUiTextState reset = texts.RemoveOverride(e1, texts.Load(e1).State!, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.SaveOverwrite);
            if (reset.Overrides.Count != 0 || !texts.Save(e1, reset).Succeeded)
                throw new InvalidDataException("Save.overwrite reset regressed.");
            // F. Max-width triple (66 bytes) exercises the append path.
            var maxEdits = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["message"] = "12345678901234567890",
                ["question"] = "12345678901234567",
                ["yes"] = "123456789",
                ["no"] = "Nie"
            };
            if (!RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.SaveOverwrite, maxEdits, out string maxFull, out _, out _))
                throw new InvalidDataException("Max-width Save.overwrite was rejected.");
            if (RunVgaVariableUiService.Decide(RuntimeUiLogicalRecordId.SaveOverwrite, maxFull) != RunVgaMaterializationKind.Appended)
                throw new InvalidDataException("Over-envelope Save.overwrite was not routed to append.");
            var translations = new TranslationProjectService();
            var graphics = new GraphicsVariantService();
            TranslationProjectState textState = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant sk = translations.Create(e1, textState, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            textState = translations.Add(textState, sk); translations.Save(e1, textState);
            GraphicsProjectState graphicsState = GraphicsProjectState.Empty(ElviraGameProfile.Elvira1);
            string pristinePackedHash = HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable));
            var directories = new VariantDirectoryService();
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, vga, sk.Code), VariantDirectoryOperationStatus.Created, "overwrite fixture variant directory");
            string variantRoot = directories.GetVariantEditionDirectoryPath(e1, vga, sk.Code);
            string patchedV5 = Path.Combine(variantRoot, ActiveProjectBuildIdentity.ExecutableName(vga, sk));
            string tmpSrc = Path.Combine(root, "tmp-overwrite-runvga.exe");
            string tmpV5 = Path.Combine(root, "tmp-overwrite-v5.exe");
            File.Copy(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable), tmpSrc, true);
            RunVgaBootstrapService.CreateExtendedCp852(tmpSrc, tmpV5, GlyphRepository.CreateAllCp852Slots());
            byte[] beforeV5 = File.ReadAllBytes(tmpV5);
            // F1. In-place Slovak build: size unchanged, span round-trips.
            CompositeBuildService skBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, skState)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("In-place Slovak Save.overwrite build failed.");
            byte[] inPlace = File.ReadAllBytes(patchedV5);
            if (inPlace.Length != beforeV5.Length)
                throw new InvalidDataException("In-place Save.overwrite build changed executable size.");
            if (!RunVgaVariableUiService.TryDecodeSpan(inPlace, RuntimeUiLogicalRecordId.SaveOverwrite, out string? roundTrip, out _) || !roundTrip!.Equals(skFull, StringComparison.Ordinal))
                throw new InvalidDataException("In-place Save.overwrite round trip diverged.");
            // F2. Appended max-width build: grown image, Overwrite thunk,
            // deterministic hash, pristine source intact.
            RuntimeUiTextState maxState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.SaveOverwrite, maxFull);
            CompositeBuildService maxBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(sk, graphicsState, maxState)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (maxBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Appended Save.overwrite build failed.");
            byte[] appended = File.ReadAllBytes(patchedV5);
            if (appended.Length != beforeV5.Length + 0x200)
                throw new InvalidDataException("Appended Save.overwrite output has the wrong size.");
            if (!appended.AsSpan(0xCBFC, 5).SequenceEqual(new byte[] { 0x9A, 0x80, 0x00, 0x26, 0x39 }))
                throw new InvalidDataException("Overwrite callsite was not redirected to its thunk.");
            byte[] maxPayload = GamePcTextEditor.GetEncoding("CP852").GetBytes(maxFull);
            if (!appended.AsSpan(beforeV5.Length + 0x140, maxPayload.Length).SequenceEqual(maxPayload))
                throw new InvalidDataException("Appended Save.overwrite record diverged.");
            string maxHash = HashFile(patchedV5);
            if (maxBuild.Build(e1, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !HashFile(patchedV5).Equals(maxHash, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Save.overwrite rebuild was not deterministic.");
            if (HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable)) != pristinePackedHash)
                throw new InvalidDataException("Save.overwrite build modified the pristine RUNVGA source.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRuntimeUiGridReentrancySmoke(string elvira1Source, string elvira2Source)
    {
        // Regression for the interactive InvalidOperationException ("reentrant
        // call to SetCurrentCellAddressCore"): the Runtime UI grid was rebuilt
        // synchronously (Rows.Clear/Add) from inside its own CellEndEdit commit
        // stack. This smoke runs the same logical sequence through the deferred
        // path on an STA message loop and proves: (a) a refresh requested from
        // inside a SelectionChanged transition completes without throwing;
        // (b) duplicate queued requests coalesce into exactly one rebuild (no
        // loops); (c) rows, Pause override, dirty flag, selection and actions
        // stay coherent. Limits: headless unshown form with a created handle;
        // physical mouse/keyboard commit timing is not reproduced, but the
        // queue -> pump -> worker path is line-identical to the crash path.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiReentrancySmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant sk = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Skusobny text." });
            translations.Save(fixture, translations.Add(text, sk));
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Grid reentrancy fixture did not validate as an installation.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            if (form.RuntimeUiGridRowCountForTest != 8 || form.RuntimeUiGridPauseRowCountForTest != 1)
                throw new InvalidDataException("Runtime UI grid did not populate deterministically.");
            if (!form.RuntimeUiGridPauseOriginalForTest.Equals("Game Paused", StringComparison.Ordinal))
                throw new InvalidDataException("Pause.menu title-only original regressed.");
            form.SelectTranslationForTest("SK");
            if (!form.RuntimeUiGridAllOverrideCellsReadOnlyForTest)
                throw new InvalidDataException("R9F V8 grid is not fully read-only.");
            _ = form.Handle; // created handle: BeginInvoke posts to the STA loop from here on
            int baseline = form.RuntimeUiGridRefreshCountForTest;
            // R9F V8: inline grid commits never mutate state. An overlong raw
            // title via the blocked inline path is reverted without touching
            // project state (semantic validation lives in the editor, not here).
            form.ApplyRuntimeUiGridEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "This title is way too long");
            if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest || form.RuntimeUiSaveEnabledForTest)
                throw new InvalidDataException("Blocked inline edit stored project state.");
            PumpRuntimeUiGrid(form, baseline);
            baseline = form.RuntimeUiGridRefreshCountForTest;
            if (!form.RuntimeUiGridPauseOverrideForTest.Equals(string.Empty, StringComparison.Ordinal))
                throw new InvalidDataException("Blocked inline edit was not reverted.");
            // Raw unstructured value via inline is likewise rejected.
            form.ApplyRuntimeUiGridEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "asssss");
            if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest)
                throw new InvalidDataException("Raw inline value was stored.");
            PumpRuntimeUiGrid(form, baseline);
            baseline = form.RuntimeUiGridRefreshCountForTest;
            // Newline raw value via inline is likewise rejected.
            form.ApplyRuntimeUiGridEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "A\nB");
            if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest)
                throw new InvalidDataException("Inline newline was stored.");
            PumpRuntimeUiGrid(form, baseline);
            baseline = form.RuntimeUiGridRefreshCountForTest;
            // Semantic path: a valid title composes and stores.
            string pauseTitle = "R9F PAUSE!";
            form.ApplySemanticEditForTest(RuntimeUiLogicalRecordId.PauseMenu, pauseTitle);
            // Burst of duplicates, including one from inside a grid
            // SelectionChanged transition (fires inside SetCurrentCellAddressCore).
            form.QueueRuntimeUiGridRefreshForTest();
            if (!form.QueueRefreshFromGridTransitionForTest())
                throw new InvalidDataException("Grid transition handler did not run.");
            PumpRuntimeUiGrid(form, baseline);
            if (form.RuntimeUiGridRefreshCountForTest != baseline + 1)
                throw new InvalidDataException("Queued refreshes did not coalesce into exactly one rebuild.");
            if (form.RuntimeUiGridRowCountForTest != 8 || form.RuntimeUiGridPauseRowCountForTest != 1)
                throw new InvalidDataException("Deferred rebuild duplicated or lost rows.");
            if (!form.RuntimeUiGridPauseOverrideForTest.Equals(pauseTitle, StringComparison.Ordinal))
                throw new InvalidDataException("Pause.menu title did not survive the deferred refresh.");
            if (!form.RuntimeUiDirtyForTest || !form.RuntimeUiSaveEnabledForTest || form.RuntimeUiOverrideCountForTest != 1)
                throw new InvalidDataException("Dirty state, actions or project state diverged after refresh.");
            if (form.RuntimeUiGridCurrentIdForTest is not RuntimeUiLogicalRecordId.PauseMenu)
                throw new InvalidDataException("Current record selection was not preserved across refresh.");
            // A second independent cycle still pumps exactly once (no stuck flag).
            int second = form.RuntimeUiGridRefreshCountForTest;
            form.QueueRuntimeUiGridRefreshForTest();
            PumpRuntimeUiGrid(form, second);
            if (form.RuntimeUiGridRefreshCountForTest != second + 1 || form.RuntimeUiGridRowCountForTest != 8)
                throw new InvalidDataException("Second refresh cycle regressed.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRuntimeUiGridEditSafetySmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8: the Runtime UI grid is evidence/selection only. This keeps
        // all raw cells locked while proving known RUNVGA originals remain
        // visible and the structured semantic editor retains the only edit path.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiGridEditSafety", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant sk = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            translations.Save(fixture, translations.Add(text, sk));
            var runtimeUi = new RuntimeUiTextService();
            RuntimeUiTextState legacyState = new(
                PristineManifestService.GameIdFor(ElviraGameProfile.Elvira1),
                [],
                [new(RuntimeUiLogicalRecordId.FileNotFound, "Legacy residue", [VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega], RuntimeUiTextService.ReasonAmbiguous)]);
            if (!runtimeUi.Save(fixture, "SK", legacyState).Succeeded)
                throw new InvalidDataException("Could not create the disposable Runtime UI legacy-residue fixture.");
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
                throw new InvalidDataException("Grid edit-safety fixture did not validate as an installation.");

            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(installation);
            var expectedOriginals = new Dictionary<RuntimeUiLogicalRecordId, string>
            {
                [RuntimeUiLogicalRecordId.PauseMenu] = "Game Paused",
                [RuntimeUiLogicalRecordId.ConfirmGeneric] = "Are you sure ?",
                [RuntimeUiLogicalRecordId.SavePrompt] = "Insert savegame data disk & enter filename:",
                [RuntimeUiLogicalRecordId.SaveFailure] = "Save failed.",
                [RuntimeUiLogicalRecordId.LoadFailure] = "Load failed.",
                [RuntimeUiLogicalRecordId.FileNotFound] = "File not found.",
                [RuntimeUiLogicalRecordId.TryAnotherDisk] = "Try another disk.",
                [RuntimeUiLogicalRecordId.SaveOverwrite] = "File already exists."
            };
            if (form.RuntimeUiGridRowCountForTest != expectedOriginals.Count || !form.RuntimeUiGridAllOverrideCellsReadOnlyForTest)
                throw new InvalidDataException("Original Runtime UI grid is not complete and read-only.");
            foreach ((RuntimeUiLogicalRecordId id, string expected) in expectedOriginals)
            {
                if (!form.RuntimeUiGridOriginalForTest(id).Equals(expected, StringComparison.Ordinal))
                    throw new InvalidDataException("RUNVGA original display diverged: " + id);
                if (!form.RuntimeUiGridCellReadOnlyForTest(id))
                    throw new InvalidDataException("RUNVGA Override cell is editable: " + id);
            }
            if (form.OpenSemanticEditorBlockedReasonForTest(0) != "Original")
                throw new InvalidDataException("Original edition double-click did not report read-only state.");
            form.ApplyRuntimeUiGridEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "raw grid edit");
            if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest ||
                !form.RuntimeUiStatusLabelForTest.Contains("Original", StringComparison.Ordinal))
                throw new InvalidDataException("Original-edition grid edit was not safely blocked.");

            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RuntimeUiTextState empty = RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1);
            var layouts = new RuntimeUiLayoutValidationService(runtimeUi);
            foreach (RuntimeUiLogicalRecordId id in new[]
            {
                RuntimeUiLogicalRecordId.SavePrompt, RuntimeUiLogicalRecordId.SaveFailure,
                RuntimeUiLogicalRecordId.LoadFailure, RuntimeUiLogicalRecordId.FileNotFound,
                RuntimeUiLogicalRecordId.TryAnotherDisk
            })
            {
                if (runtimeUi.GetEffectiveRecords(vga, empty).Single(record => record.LogicalRecordId == id).MappingReadiness != RuntimeUiMappingReadiness.KnownButMappingIncomplete ||
                    layouts.Validate(vga, empty, id).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete)
                    throw new InvalidDataException("Known-but-incomplete RUNVGA record became editable/mapped: " + id);
            }

            form.SelectTranslationForTest("SK");
            if (!form.RuntimeUiGridAllOverrideCellsReadOnlyForTest || form.RuntimeUiGridRowCountForTest != expectedOriginals.Count ||
                !form.RuntimeUiLegacyNoticeVisibleForTest || form.RuntimeUiLegacyRowCountForTest != 0)
                throw new InvalidDataException("Editable-edition Runtime UI grid or separate legacy notice regressed.");
            if (form.OpenSemanticEditorBlockedReasonForTest((int)RuntimeUiLogicalRecordId.SavePrompt) != "ReadOnly")
                throw new InvalidDataException("Unsupported RUNVGA row opened an editor.");
            foreach (RuntimeUiLogicalRecordId id in new[]
            {
                RuntimeUiLogicalRecordId.PauseMenu, RuntimeUiLogicalRecordId.ConfirmGeneric, RuntimeUiLogicalRecordId.SaveOverwrite
            })
            {
                using RuntimeUiSemanticEditorForm? editor = form.BuildSemanticEditorForTest(id);
                if (editor is null)
                    throw new InvalidDataException("Supported RUNVGA editor did not open: " + id);
            }
            form.ApplyRuntimeUiGridEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "asssss");
            if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest)
                throw new InvalidDataException("Raw grid edit bypassed the semantic editor.");

            IReadOnlyDictionary<string, string> acceptedButtons = new Dictionary<string, string>
            {
                ["continue"] = "Pokračovať",
                ["quit"] = "Koniec"
            };
            if (!RuntimeUiSemanticEditorForm.TryComposeSemanticField(VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu,
                "Pozastavené", acceptedButtons, out _, out _))
                throw new InvalidDataException("VGA semantic Pause fields were incorrectly rejected.");
            IReadOnlyDictionary<string, string> rejectedButtons = new Dictionary<string, string>
            {
                ["continue"] = "Pokračovať",
                ["quit"] = "Ukončiť"
            };
            if (RunVgaVariableUiService.TryCompose(RuntimeUiLogicalRecordId.PauseMenu,
                new Dictionary<string, string>(rejectedButtons) { ["title"] = "Pozastavené" }, out _, out RunVgaVariableFailureKind failure, out _) ||
                failure != RunVgaVariableFailureKind.RowOverflow)
                throw new InvalidDataException("VGA Pause 21-column visual boundary regressed.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRuntimeUiLegacyUnassignedUxSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8.1: v1 scalar residue has no runtime identity. It must remain
        // separate from the selected runtime grid until an explicit discard.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiLegacyUnassignedUx", Guid.NewGuid().ToString("N"));
        string priorLocale = UiText.LocaleId;
        try
        {
            UiText.SetLocale("en");
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string vgaPath = Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable);
            string egaPath = Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable);
            string vgaHash = HashFile(vgaPath);
            string egaHash = HashFile(egaPath);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant sk = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            translations.Save(fixture, translations.Add(text, sk));
            var runtimeUi = new RuntimeUiTextService();
            string statePath = runtimeUi.GetPath(fixture, "SK");
            Directory.CreateDirectory(Path.GetDirectoryName(statePath)!);
            string v1Scalar = "{\"schemaVersion\":1,\"gameId\":\"Elvira1\",\"records\":[{\"logicalRecordId\":\"PauseMenu\",\"text\":\"Pozastavené\"},{\"logicalRecordId\":\"SavePrompt\",\"text\":\"Legacy prompt\"}]}";
            File.WriteAllText(statePath, v1Scalar);

            RuntimeUiTextLoadResult migrated = runtimeUi.Load(fixture, "SK");
            if (!migrated.IsSuccess || migrated.State is not { Overrides.Count: 0, UnassignedLegacy.Count: 2 } migratedState ||
                migratedState.UnassignedLegacy.SingleOrDefault(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu) is not { Text: "Pozastavené" })
                throw new InvalidDataException("The v1 Pause.menu scalar was assigned, deleted, or changed during migration.");
            if (File.ReadAllText(statePath) != v1Scalar)
                throw new InvalidDataException("Read-only v1 migration rewrote project state without an explicit user action.");
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            if (runtimeUi.GetEffectiveRecords(vga, migratedState).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu).IsOverridden ||
                runtimeUi.GetEffectiveRecords(ega, migratedState).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu).IsOverridden)
                throw new InvalidDataException("Unassigned v1 Pause.menu scalar leaked into a runtime projection.");

            if (!RuntimeUiSemanticEditorForm.TryComposeSemanticField(VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, "VGA Pause", out string vgaPause, out _) ||
                !RuntimeUiSemanticEditorForm.TryComposeSemanticField(VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "EGA failed", out string egaFailure, out _))
                throw new InvalidDataException("Could not prepare assigned runtime-state preservation fixtures.");
            RuntimeUiTextState prepared = runtimeUi.SetOverride(fixture, migratedState, VariantRuntimeKind.Elvira1Vga, RuntimeUiLogicalRecordId.PauseMenu, vgaPause);
            prepared = runtimeUi.SetOverride(fixture, prepared, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, egaFailure);
            if (!runtimeUi.Save(fixture, "SK", prepared).Succeeded)
                throw new InvalidDataException("Could not save the disposable v2 legacy-residue fixture.");
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
                throw new InvalidDataException("Legacy-unassigned UX fixture did not validate as an installation.");

            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(installation);
            form.SelectTranslationForTest("SK");
            if (form.RuntimeUiGridRowCountForTest != 8 || form.RuntimeUiLegacyRowCountForTest != 0 ||
                !form.RuntimeUiLegacyNoticeVisibleForTest || !form.RuntimeUiLegacyDiscardEnabledForTest || form.RuntimeUiUnassignedCountForTest != 2)
                throw new InvalidDataException("Legacy residue was mixed into the VGA runtime grid or its discard action was unavailable.");
            string notice = form.RuntimeUiLegacyNoticeForTest;
            if (!notice.Contains("unassigned", StringComparison.OrdinalIgnoreCase) || !notice.Contains("not be used when building", StringComparison.OrdinalIgnoreCase) ||
                notice.Contains(UiText.Get("RuntimeUi.OriginalUnavailable"), StringComparison.Ordinal))
                throw new InvalidDataException("Legacy migration notice is unclear or conflates missing source text with missing runtime identity.");
            if (!form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals("VGA Pause", StringComparison.Ordinal))
                throw new InvalidDataException("Assigned VGA override was not preserved beside legacy residue.");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            if (!form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.SaveFailure).Equals("EGA failed", StringComparison.Ordinal) ||
                !form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals(string.Empty, StringComparison.Ordinal))
                throw new InvalidDataException("Runtime-scoped state leaked while legacy residue was present.");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            if (!form.SelectRuntimeUiLegacyOverrideForTest(RuntimeUiLogicalRecordId.PauseMenu))
                throw new InvalidDataException("Legacy migration selection did not retain Pause.menu.");
            form.DiscardSelectedRuntimeUiLegacyOverrideForTest();
            if (form.RuntimeUiUnassignedCountForTest != 1 || !form.RuntimeUiLegacyNoticeVisibleForTest || form.RuntimeUiGridRowCountForTest != 8 ||
                !form.RuntimeUiDirtyForTest || !form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals("VGA Pause", StringComparison.Ordinal))
                throw new InvalidDataException("Selected legacy discard removed runtime state or the unrelated residue.");
            if (!form.SelectRuntimeUiLegacyOverrideForTest(RuntimeUiLogicalRecordId.SavePrompt))
                throw new InvalidDataException("Selected legacy discard did not preserve the unrelated residue.");
            form.DiscardSelectedRuntimeUiLegacyOverrideForTest();
            if (form.RuntimeUiUnassignedCountForTest != 0 || form.RuntimeUiLegacyNoticeVisibleForTest)
                throw new InvalidDataException("Explicit final legacy discard left migration state behind.");
            form.SaveRuntimeUiForTest();
            RuntimeUiTextLoadResult reloaded = runtimeUi.Load(fixture, "SK");
            if (!reloaded.IsSuccess || reloaded.State is not { UnassignedLegacy.Count: 0, Overrides.Count: 2 } persisted ||
                !runtimeUi.GetOverridesForRuntime(persisted, VariantRuntimeKind.Elvira1Vga).Single().Text.Equals(vgaPause, StringComparison.Ordinal) ||
                !runtimeUi.GetOverridesForRuntime(persisted, VariantRuntimeKind.Elvira1Ega).Single().Text.Equals(egaFailure, StringComparison.Ordinal))
                throw new InvalidDataException("Legacy discard did not persist while preserving assigned runtime state.");
            if (HashFile(vgaPath) != vgaHash || HashFile(egaPath) != egaHash || Directory.Exists(Path.Combine(fixture.GameRoot, "VARIANTS")))
                throw new InvalidDataException("Legacy project-state cleanup touched an executable or variant output.");
            _ = elvira2Source;
        }
        finally
        {
            UiText.SetLocale(priorLocale);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyRuntimeUiLiveLocalizationSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8.2: EN<->SK<->CS switches regenerate Runtime UI presentation
        // from existing in-memory state. No reload, no save, no dirty state,
        // no binary/project mutation; selection and column widths survive.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiLiveLocalization", Guid.NewGuid().ToString("N"));
        string priorLocale = UiText.LocaleId;
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant sk = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            translations.Save(fixture, translations.Add(text, sk));
            var runtimeUi = new RuntimeUiTextService();
            RuntimeUiTextState legacyState = new(
                PristineManifestService.GameIdFor(ElviraGameProfile.Elvira1),
                [],
                [new(RuntimeUiLogicalRecordId.SavePrompt, "Legacy prompt", [VariantRuntimeKind.Elvira1Vga, VariantRuntimeKind.Elvira1Ega], RuntimeUiTextService.ReasonAmbiguous)]);
            if (!runtimeUi.Save(fixture, "SK", legacyState).Succeeded)
                throw new InvalidDataException("Could not create the disposable live-localization fixture.");
            string stateBefore = File.ReadAllText(runtimeUi.GetPath(fixture, "SK"));
            string gamePcHash = HashFile(Path.Combine(fixture.GameRoot, "GAMEPC"));
            string vgaHash = HashFile(Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable));
            string egaHash = HashFile(Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable));
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
                throw new InvalidDataException("Live-localization fixture did not validate as an installation.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(installation);
            form.SelectTranslationForTest("SK");
            form.SetRuntimeUiGridColumnWidthForTest("Override", 321);
            if (!form.SelectRuntimeUiRowForTest(RuntimeUiLogicalRecordId.PauseMenu))
                throw new InvalidDataException("Pause.menu row vanished before the locale switch.");
            if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest)
                throw new InvalidDataException("Fixture carries unexpected Runtime UI state.");

            UiText.SetLocale("sk");
            if (form.RuntimeUiGridRowCountForTest != 8)
                throw new InvalidDataException("Locale switch changed the authoritative VGA row set.");
            string skStatus = form.RuntimeUiGridStatusForTest(RuntimeUiLogicalRecordId.PauseMenu);
            string skDetail = form.RuntimeUiGridDetailForTest(RuntimeUiLogicalRecordId.PauseMenu);
            string skLabel = form.RuntimeUiStatusLabelForTest;
            string skNotice = form.RuntimeUiLegacyNoticeForTest;
            if (!skStatus.Equals("Platné", StringComparison.Ordinal) || !skDetail.Contains("zmestí", StringComparison.Ordinal))
                throw new InvalidDataException("SK Runtime UI presentation did not localize: " + skStatus + " / " + skDetail);
            if (!skLabel.Equals(UiText.Get("RuntimeUi.ProjectStateLoaded"), StringComparison.Ordinal))
                throw new InvalidDataException("SK Runtime UI status line did not localize.");
            if (!skNotice.Contains("nepriradenú", StringComparison.OrdinalIgnoreCase) ||
                skNotice.Contains(UiText.Get("RuntimeUi.OriginalUnavailable"), StringComparison.Ordinal))
                throw new InvalidDataException("SK legacy migration notice did not localize cleanly.");
            var counters = (form.GraphicsLoadCount, form.TextLoadCount, form.RuntimeUiLoadCount, form.FontBindCount,
                form.WorkflowRefreshCount, form.PreviewRenderCount, form.ApplyInstallationRequestedCount,
                form.ApplyInstallationEffectiveCount, form.ModsRefreshCount);

            UiText.SetLocale("en");
            string enStatus = form.RuntimeUiGridStatusForTest(RuntimeUiLogicalRecordId.PauseMenu);
            string enDetail = form.RuntimeUiGridDetailForTest(RuntimeUiLogicalRecordId.PauseMenu);
            if (!enStatus.Equals("Valid", StringComparison.Ordinal) || !enDetail.Contains("fits", StringComparison.Ordinal))
                throw new InvalidDataException("EN Runtime UI presentation did not regenerate without reload: " + enStatus + " / " + enDetail);
            if (!form.RuntimeUiGridOriginalForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals("Game Paused", StringComparison.Ordinal) ||
                !form.RuntimeUiGridLogicalNameForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals("Pause.menu", StringComparison.Ordinal))
                throw new InvalidDataException("Locale switch translated source/game data or logical IDs.");
            if (!form.RuntimeUiStatusLabelForTest.Equals(UiText.Get("RuntimeUi.ProjectStateLoaded"), StringComparison.Ordinal) ||
                !form.RuntimeUiLegacyNoticeForTest.Contains("unassigned", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("EN status line or legacy notice did not regenerate.");
            AssertLiveLocalizationPreserved(form, counters, 321);

            UiText.SetLocale("cs");
            string csDetail = form.RuntimeUiGridDetailForTest(RuntimeUiLogicalRecordId.PauseMenu);
            if (!form.RuntimeUiGridStatusForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals("Platné", StringComparison.Ordinal) ||
                !csDetail.Contains("vejde", StringComparison.Ordinal))
                throw new InvalidDataException("CS Runtime UI presentation did not regenerate without reload.");
            AssertLiveLocalizationPreserved(form, counters, 321);

            UiText.SetLocale("sk");
            if (!form.RuntimeUiGridStatusForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals(skStatus, StringComparison.Ordinal) ||
                !form.RuntimeUiGridDetailForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals(skDetail, StringComparison.Ordinal) ||
                !form.RuntimeUiStatusLabelForTest.Equals(skLabel, StringComparison.Ordinal) ||
                !form.RuntimeUiLegacyNoticeForTest.Equals(skNotice, StringComparison.Ordinal))
                throw new InvalidDataException("SK presentation did not round-trip across locale switches.");
            AssertLiveLocalizationPreserved(form, counters, 321);
            if (enDetail.Equals(skDetail, StringComparison.Ordinal) || enDetail.Equals(csDetail, StringComparison.Ordinal) ||
                skDetail.Equals(csDetail, StringComparison.Ordinal))
                throw new InvalidDataException("Localized Detail cells did not actually differ per locale.");

            if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest || !form.RuntimeUiStatusLabelForTest.Equals(skLabel, StringComparison.Ordinal))
                throw new InvalidDataException("Language switches mutated Runtime UI project state.");
            if (!File.ReadAllText(runtimeUi.GetPath(fixture, "SK")).Equals(stateBefore, StringComparison.Ordinal) ||
                HashFile(Path.Combine(fixture.GameRoot, "GAMEPC")) != gamePcHash ||
                HashFile(Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable)) != vgaHash ||
                HashFile(Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable)) != egaHash ||
                Directory.Exists(Path.Combine(fixture.GameRoot, "VARIANTS")))
                throw new InvalidDataException("Language switches touched project, binary, or variant state.");
            _ = elvira2Source;
        }
        finally
        {
            UiText.SetLocale(priorLocale);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void AssertLiveLocalizationPreserved(MainForm form,
        (int Graphics, int Text, int RuntimeUi, int Font, int Workflow, int Preview, int InstallReq, int InstallEff, int Mods) baseline,
        int expectedOverrideWidth)
    {
        var now = (form.GraphicsLoadCount, form.TextLoadCount, form.RuntimeUiLoadCount, form.FontBindCount,
            form.WorkflowRefreshCount, form.PreviewRenderCount, form.ApplyInstallationRequestedCount,
            form.ApplyInstallationEffectiveCount, form.ModsRefreshCount);
        if (!now.Equals(baseline))
            throw new InvalidDataException("A pure locale switch triggered data reload/reactivation: before=" + baseline + " after=" + now + ".");
        if (form.RuntimeUiGridCurrentIdForTest is not RuntimeUiLogicalRecordId.PauseMenu)
            throw new InvalidDataException("Locale switch lost the Runtime UI row selection.");
        if (form.RuntimeUiGridColumnWidthForTest("Override") != expectedOverrideWidth)
            throw new InvalidDataException("Locale switch reset a user-resized Runtime UI column width.");
        if (form.RuntimeUiGridRowCountForTest != 8 || form.RuntimeUiLegacyRowCountForTest != 0)
            throw new InvalidDataException("Locale switch mixed legacy residue into the runtime grid.");
    }

    private static void VerifyFontLazyWorkingCopySmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8.2: the first mutating action on a normal editable glyph
        // clones ORIGINAL into the edited working copy and applies the same
        // mutation in one step. Viewing never creates state; 0x81 stays
        // protected; nothing is written to the executable.
        string root = Path.Combine(Path.GetTempPath(), "Pi1FontLazyWorkingCopy", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string runVgaPath = Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable);
            string gamePcPath = Path.Combine(fixture.GameRoot, "GAMEPC");
            string runVgaHash = HashFile(runVgaPath);
            string gamePcHash = HashFile(gamePcPath);
            using var font = new FontEditorForm();
            font.LoadFromGameDirectory(fixture.GameRoot);
            if (font.SourceLoadCount != 1)
                throw new InvalidDataException("Font source did not load exactly once.");

            // A. Normal editable glyph starts without an edited working copy.
            if (!font.SelectGlyphForTest(0x31))
                throw new InvalidDataException("Editable glyph 0x31 could not be selected.");
            if (font.GlyphHasEditedForTest(0x31))
                throw new InvalidDataException("Viewing a glyph created an edited working copy.");
            byte[] original31 = font.GlyphOriginalForTest(0x31);
            if (original31.Length != 8 || original31.All(value => value == 0))
                throw new InvalidDataException("Glyph 0x31 has no valid original bitmap.");

            // C. First pixel click clones the original and toggles in one step.
            font.ToggleCurrentPixelForTest(0, 0);
            if (!font.GlyphHasEditedForTest(0x31))
                throw new InvalidDataException("First pixel mutation did not create the edited working copy.");
            byte[] edited31 = font.GlyphEditedForTest(0x31);
            for (int row = 0; row < 8; row++)
            {
                byte expected = row == 0 ? (byte)(original31[row] ^ 0x80) : original31[row];
                if (edited31[row] != expected)
                    throw new InvalidDataException("First click was lost or touched more than the intended pixel.");
            }

            // E. First shift mutation on a fresh glyph works in one action.
            using var shifted = new FontEditorForm();
            shifted.LoadFromGameDirectory(fixture.GameRoot);
            if (!shifted.SelectGlyphForTest(0x41) || shifted.GlyphHasEditedForTest(0x41))
                throw new InvalidDataException("Fresh glyph 0x41 is not in view-only state.");
            byte[] original41 = shifted.GlyphOriginalForTest(0x41);
            shifted.ShiftCurrentGlyphForTest(1, 0);
            if (!shifted.GlyphHasEditedForTest(0x41))
                throw new InvalidDataException("First shift mutation did not create the edited working copy.");
            byte[] edited41 = shifted.GlyphEditedForTest(0x41);
            for (int row = 0; row < 8; row++)
            {
                if (edited41[row] != (byte)(original41[row] >> 1))
                    throw new InvalidDataException("Shift was not applied in the same operation as working-copy creation.");
            }

            // F. Merely selecting/viewing a fresh glyph creates nothing.
            if (!shifted.SelectGlyphForTest(0x42) || shifted.GlyphHasEditedForTest(0x42))
                throw new InvalidDataException("Selecting a fresh glyph created edited state.");

            // G. Protected 0x81 cannot auto-create editable state.
            if (!shifted.SelectGlyphForTest(FontSlotMetadata.HudEraseGlyph))
                throw new InvalidDataException("Protected glyph 0x81 could not be selected.");
            shifted.ToggleCurrentPixelForTest(0, 0);
            shifted.ShiftCurrentGlyphForTest(1, 0);
            if (shifted.GlyphHasEditedForTest(FontSlotMetadata.HudEraseGlyph))
                throw new InvalidDataException("Protected glyph 0x81 became editable through lazy creation.");

            // I. Explicit Restore glyph still resets an edited glyph to original.
            if (!font.SelectGlyphForTest(0x31))
                throw new InvalidDataException("Edited glyph 0x31 could not be reselected.");
            font.ResetCurrentGlyphForTest();
            if (!font.GlyphHasEditedForTest(0x31) || !font.GlyphEditedForTest(0x31).SequenceEqual(font.GlyphOriginalForTest(0x31)))
                throw new InvalidDataException("Explicit Restore glyph did not reset the edited glyph to original.");

            // H. First edits are in-memory only: no executable/game mutation.
            if (HashFile(runVgaPath) != runVgaHash || HashFile(gamePcPath) != gamePcHash ||
                Directory.Exists(Path.Combine(fixture.GameRoot, "VARIANTS")))
                throw new InvalidDataException("Lazy working-copy creation wrote to game files.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyFontRealClickLazyWorkingCopySmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8.3: exercises the REAL GlyphMatrixControl input path that V8.2
        // bypassed. V8.2 invoked the toggle handler directly while the matrix
        // itself was configured non-editable (editable = HasEdited), so real
        // mouse clicks never raised PixelToggled. This smoke drives the same
        // hit-testing and OnMouseDown/OnKeyDown entry points a physical
        // click/keypress uses.
        string root = Path.Combine(Path.GetTempPath(), "Pi1FontRealClickLazy", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string runVgaPath = Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable);
            string runVgaHash = HashFile(runVgaPath);
            using var font = new FontEditorForm();
            font.LoadFromGameDirectory(fixture.GameRoot);
            if (font.SourceLoadCount != 1)
                throw new InvalidDataException("Font source did not load exactly once.");

            // A/B. Fresh glyph: no working copy, yet the matrix must already
            // accept gestures — this is the assertion V8.2 could not make.
            if (!font.SelectGlyphForTest(0x62) || font.GlyphHasEditedForTest(0x62))
                throw new InvalidDataException("Glyph 0x62 is not in view-only state.");
            if (!font.EditedMatrixForTest.IsEditableForTest)
                throw new InvalidDataException("EDITED matrix is not editable before a working copy exists; real clicks cannot arrive.");
            byte[] original62 = font.GlyphOriginalForTest(0x62);
            if (original62.Length != 8 || original62.All(value => value == 0))
                throw new InvalidDataException("Glyph 0x62 has no valid original bitmap.");

            // C/D. One real control click creates the copy AND toggles.
            if (!font.EditedMatrixForTest.SimulatePixelClickForTest(2, 3))
                throw new InvalidDataException("The real control path did not accept the click.");
            if (!font.GlyphHasEditedForTest(0x62))
                throw new InvalidDataException("Real first click did not create the edited working copy.");
            byte[] edited62 = font.GlyphEditedForTest(0x62);
            for (int row = 0; row < 8; row++)
            {
                byte expected = row == 2 ? (byte)(original62[row] ^ (1 << (7 - 3))) : original62[row];
                if (edited62[row] != expected)
                    throw new InvalidDataException("The real first click was lost or touched more than the intended pixel.");
            }

            // E. First shift through the real key path, same single action.
            using var shifted = new FontEditorForm();
            shifted.LoadFromGameDirectory(fixture.GameRoot);
            if (!shifted.SelectGlyphForTest(0x78) || shifted.GlyphHasEditedForTest(0x78))
                throw new InvalidDataException("Glyph 0x78 is not in view-only state.");
            if (!shifted.EditedMatrixForTest.IsEditableForTest)
                throw new InvalidDataException("EDITED matrix rejects shift keys before a working copy exists.");
            byte[] original78 = shifted.GlyphOriginalForTest(0x78);
            shifted.EditedMatrixForTest.SimulateShiftKeyForTest(System.Windows.Forms.Keys.Right);
            if (!shifted.GlyphHasEditedForTest(0x78))
                throw new InvalidDataException("Real first shift did not create the edited working copy.");
            byte[] edited78 = shifted.GlyphEditedForTest(0x78);
            for (int row = 0; row < 8; row++)
            {
                if (edited78[row] != (byte)(original78[row] >> 1))
                    throw new InvalidDataException("Shift was not applied in the same operation as working-copy creation.");
            }

            // 6x8 renderer proof at control level: columns 6-7 are inactive,
            // so clicking them requests no mutation and creates no state.
            if (!shifted.SelectGlyphForTest(0x79) || shifted.GlyphHasEditedForTest(0x79))
                throw new InvalidDataException("Glyph 0x79 is not in view-only state.");
            if (!shifted.EditedMatrixForTest.SimulatePixelClickForTest(0, 6))
                throw new InvalidDataException("The real control path unexpectedly rejected an in-grid gesture.");
            if (shifted.GlyphHasEditedForTest(0x79))
                throw new InvalidDataException("A renderer-inactive column click created edited state.");

            // F/G. Viewing alone is clean; explicit Reset still restores.
            if (!font.SelectGlyphForTest(0x62))
                throw new InvalidDataException("Edited glyph 0x62 could not be reselected.");
            font.ResetCurrentGlyphForTest();
            if (!font.GlyphHasEditedForTest(0x62) || !font.GlyphEditedForTest(0x62).SequenceEqual(font.GlyphOriginalForTest(0x62)))
                throw new InvalidDataException("Explicit Reset glyph did not restore the edited glyph to original.");
            if (HashFile(runVgaPath) != runVgaHash || Directory.Exists(Path.Combine(fixture.GameRoot, "VARIANTS")))
                throw new InvalidDataException("Real-click lazy editing wrote to game files.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunEgaGeneratedFontLoadSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8.3: a trusted generated RUNEGASK.EXE must load its real 256x8
        // bank (physical 0x32400) through the same loader the manual Open
        // game EXE path uses, instead of Unknown / Loaded 0 with fallbacks.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunEgaGeneratedFont", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string packedRunEga = Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable);
            string packedHash = HashFile(packedRunEga);
            string generated = Path.Combine(root, "gen", Elvira1ProductionProfile.GeneratedSlovakEgaExecutable);
            RunEgaBootstrapService.CreateFrozenCp852(packedRunEga, generated, GlyphRepository.CreateAllCp852Slots());
            byte[] image = File.ReadAllBytes(generated);

            FontLoadResult loaded = RunVgaFontService.LoadRunVga(generated);
            if (loaded.Layout != RunVgaFontLayout.ExtendedCp852RunEga || loaded.SourceType != FontSourceType.RunEga)
                throw new InvalidDataException("Generated RUNEGA was not recognized as a supported RUNEGA font layout.");
            if (loaded.Game != ElviraGame.Elvira1 || loaded.LoadedGlyphCount != 256 ||
                loaded.FontOffset != RunEgaBootstrapService.FontBankPhysicalOffset ||
                loaded.FirstByteValue != 0 || loaded.LastByteValue != 0xFF)
                throw new InvalidDataException("Generated RUNEGA bank geometry diverged from the frozen profile.");
            for (int code = 0; code < 256; code++)
            {
                if (!loaded.Glyphs[code].IsLoadedFromSource)
                    throw new InvalidDataException($"A fallback bitmap was mistaken for loaded source data: 0x{code:X2}.");
            }
            foreach (int code in new[] { 0x41, 0x62, 0x94, 0xE1 })
            {
                byte[] expected = image.AsSpan(RunEgaBootstrapService.FontBankPhysicalOffset + code * 8, 8).ToArray();
                if (!loaded.Glyphs[code].Original.SequenceEqual(expected))
                    throw new InvalidDataException($"RUNEGA glyph 0x{code:X2} does not show actual EXE bank bytes.");
            }
            // Frozen RUNEGA 0x81 invariant is the historical original mask by
            // builder/validator/profile design (see the 0x81 invariant smoke);
            // the slot must still be reserved and never user-editable.
            if (!loaded.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(FontSlotMetadata.OriginalHudEraseGlyphBytes) ||
                !FontSlotMetadata.IsReserved(loaded, FontSlotMetadata.HudEraseGlyph))
                throw new InvalidDataException("Generated RUNEGA 0x81 diverged from its frozen invariant or protection.");
            if (loaded.CanApply)
                throw new InvalidDataException("Generated RUNEGA became a direct patch target; only CompositeBuild may persist EGA fonts.");
            // RUNEGA reserved-working-state paths must keep the original mask
            // and never substitute V5/RUNIT FCx8 bytes (centralized canonical).
            if (!FontSlotMetadata.GetCanonicalBytes(loaded, FontSlotMetadata.HudEraseGlyph).SequenceEqual(FontSlotMetadata.OriginalHudEraseGlyphBytes))
                throw new InvalidDataException("RUNEGA 0x81 canonical bytes are not the frozen original mask.");
            // The exact copy path the editor UI uses for reserved slots.
            loaded.Glyphs[FontSlotMetadata.HudEraseGlyph].ReplaceReservedCanonical(
                FontSlotMetadata.GetCanonicalBytes(loaded, FontSlotMetadata.HudEraseGlyph));
            if (!loaded.Glyphs[FontSlotMetadata.HudEraseGlyph].Edited.SequenceEqual(FontSlotMetadata.OriginalHudEraseGlyphBytes))
                throw new InvalidDataException("RUNEGA Reset-glyph path substituted non-canonical 0x81 bytes.");
            byte[] hostileImport = image.AsSpan(RunEgaBootstrapService.FontBankPhysicalOffset, RunVgaFontService.ExtendedFontBytes).ToArray();
            FontSlotMetadata.PatchedHudFullCellEraseGlyphBytes.CopyTo(hostileImport, FontSlotMetadata.HudEraseGlyph * 8);
            RunVgaFontService.ImportFontBytesIntoEdited(loaded, hostileImport, "hostile FCx8 probe");
            if (!loaded.Glyphs[FontSlotMetadata.HudEraseGlyph].Edited.SequenceEqual(FontSlotMetadata.OriginalHudEraseGlyphBytes))
                throw new InvalidDataException("RUNEGA font import replaced reserved 0x81 with foreign bytes.");

            // Same FontEditorForm path as manual Open game EXE.
            using var form = new FontEditorForm();
            form.LoadExecutableForTest(generated);
            if (form.SourceLoadCount != 1)
                throw new InvalidDataException("Font editor did not load the generated RUNEGA source exactly once.");
            if (form.IsApplyEnabledForTest)
                throw new InvalidDataException("Apply stays enabled for a RUNEGA source; direct patching must remain disabled.");
            if (!form.SelectGlyphForTest(0x41) || form.GlyphHasEditedForTest(0x41) || !form.EditedMatrixForTest.IsEditableForTest)
                throw new InvalidDataException("RUNEGA in-memory working-copy editing is not available for a normal glyph.");
            try
            {
                RunVgaFontService.SaveCopy(loaded, Path.Combine(root, "gen", "MUTANT.EXE"));
                throw new InvalidDataException("Direct RUNEGA patching was silently enabled.");
            }
            catch (InvalidOperationException) { }

            // Strictness: size alone never classifies an arbitrary MZ as a font.
            string bogus = Path.Combine(root, "bogus.exe");
            byte[] junk = new byte[0x1000];
            junk[0] = (byte)'M';
            junk[1] = (byte)'Z';
            File.WriteAllBytes(bogus, junk);
            FontLoadResult unknown = RunVgaFontService.LoadRunVga(bogus);
            if (unknown.Layout != RunVgaFontLayout.Unknown || unknown.LoadedGlyphCount != 0)
                throw new InvalidDataException("An arbitrary MZ executable was recognized as a font layout.");

            if (HashFile(packedRunEga) != packedHash || !File.ReadAllBytes(generated).SequenceEqual(image) ||
                Directory.Exists(Path.Combine(fixture.GameRoot, "VARIANTS")))
                throw new InvalidDataException("RUNEGA font loading touched game or variant state.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyHudErase081InvariantSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8.3: 0x81 is engine-owned HUD erase state, never user-editable.
        // Original sources carry 00FCFCFCFCFCFC00; generated V5/RUNIT-V2 carry
        // the patched full-cell FCFCFCFCFCFCFCFC; generated RUNEGA keeps the
        // original mask by frozen builder/validator/profile design.
        string root = Path.Combine(Path.GetTempPath(), "Pi1HudErase081", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            Directory.CreateDirectory(Path.Combine(root, "gen"));
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            ProjectContext e2 = CreateBuildFixtureProjectContext(root, "e2", elvira2Source, ElviraGameProfile.Elvira2);
            byte[] originalHud = Convert.FromHexString("00FCFCFCFCFCFC00");
            byte[] patchedHud = Convert.FromHexString("FCFCFCFCFCFCFCFC");
            var loadedResults = new List<FontLoadResult>();

            FontLoadResult packedVga = RunVgaFontService.LoadRunVga(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable));
            if (packedVga.Layout != RunVgaFontLayout.OriginalPackedAscii98 ||
                !packedVga.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(originalHud))
                throw new InvalidDataException("Original packed RUNVGA 0x81 diverged from the historical original invariant.");
            loadedResults.Add(packedVga);

            string v5Path = Path.Combine(root, "gen", "RUNVGA_V5.EXE");
            RunVgaBootstrapService.CreateExtendedCp852(
                Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable), v5Path, GlyphRepository.CreateAllCp852Slots());
            FontLoadResult genV5 = RunVgaFontService.LoadRunVga(v5Path);
            if (genV5.Layout != RunVgaFontLayout.ExtendedCp852V5 ||
                !genV5.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(patchedHud))
                throw new InvalidDataException("Generated VGA V5 0x81 diverged from the patched full-cell invariant.");
            byte[] v5Image = File.ReadAllBytes(v5Path);
            if (!genV5.Glyphs[0x41].Original.SequenceEqual(v5Image.AsSpan(0x3AE60 + 0x41 * 8, 8).ToArray()))
                throw new InvalidDataException("Generated V5 normal glyphs do not show actual EXE bank bytes.");
            loadedResults.Add(genV5);

            string egaPath = Path.Combine(root, "gen", Elvira1ProductionProfile.GeneratedSlovakEgaExecutable);
            RunEgaBootstrapService.CreateFrozenCp852(
                Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable), egaPath, GlyphRepository.CreateAllCp852Slots());
            FontLoadResult genEga = RunVgaFontService.LoadRunVga(egaPath);
            if (genEga.Layout != RunVgaFontLayout.ExtendedCp852RunEga ||
                !genEga.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(originalHud))
                throw new InvalidDataException("Generated RUNEGA 0x81 diverged from its frozen original-mask invariant.");
            loadedResults.Add(genEga);

            string packedItPath = Path.Combine(e2.GameRoot, "RUNIT.EXE");
            FontLoadResult packedIt = RunVgaFontService.LoadRunVga(packedItPath);
            if (packedIt.Layout != RunVgaFontLayout.OriginalPackedAscii98 ||
                !packedIt.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(originalHud))
                throw new InvalidDataException("Original packed RUNIT 0x81 diverged from the historical original invariant.");
            loadedResults.Add(packedIt);

            byte[] canonicalIt = RunItBootstrapService.UnpackCanonicalOriginal(File.ReadAllBytes(packedItPath));
            byte[] fontImage = RunItBootstrapService.CreateFontImage(canonicalIt, GlyphRepository.CreateAllCp852Slots(), out _);
            string v2Path = Path.Combine(root, "gen", "RUNIT_V2.EXE");
            File.WriteAllBytes(v2Path, RunItBootstrapService.BuildExtended(canonicalIt, fontImage));
            FontLoadResult genV2 = RunVgaFontService.LoadRunVga(v2Path);
            if (genV2.Layout != RunVgaFontLayout.ExtendedCp852RunIt ||
                !genV2.Glyphs[FontSlotMetadata.HudEraseGlyph].Original.SequenceEqual(patchedHud))
                throw new InvalidDataException("Generated RUNIT V2 0x81 diverged from the patched full-cell invariant.");
            loadedResults.Add(genV2);

            // Renderer invariant: 8 stored rows everywhere; 0x81 reserved in
            // every layout; the editor matrix refuses 0x81 gestures.
            foreach (FontLoadResult result in loadedResults)
            {
                foreach (GlyphModel glyph in result.Glyphs)
                {
                    if (glyph.Original.Length != 8)
                        throw new InvalidDataException("A glyph bitmap is not an 8-row cell.");
                }
                if (!FontSlotMetadata.IsReserved(result, FontSlotMetadata.HudEraseGlyph))
                    throw new InvalidDataException("0x81 lost its reserved status: " + result.Layout);
            }
            using var form = new FontEditorForm();
            form.LoadExecutableForTest(v5Path);
            if (!form.SelectGlyphForTest(FontSlotMetadata.HudEraseGlyph) || form.EditedMatrixForTest.IsEditableForTest)
                throw new InvalidDataException("0x81 became editable in the font editor.");
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyFontClearGlyphSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V8.3b: Clear glyph creates an explicit all-zero Edited working
        // copy (real zero HEX, not [EMPTY]). Lazy-clone behavior is intact:
        // later pixel edits modify the zero bitmap, and Reset still restores
        // exact Original bytes while retaining explicit-copy semantics.
        string root = Path.Combine(Path.GetTempPath(), "Pi1FontClearGlyph", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            string runVgaPath = Path.Combine(fixture.GameRoot, Elvira1ProductionProfile.ActiveVgaExecutable);
            string gamePcPath = Path.Combine(fixture.GameRoot, "GAMEPC");
            string runVgaHash = HashFile(runVgaPath);
            string gamePcHash = HashFile(gamePcPath);
            using var font = new FontEditorForm();
            font.LoadFromGameDirectory(fixture.GameRoot);
            if (font.SourceLoadCount != 1)
                throw new InvalidDataException("Font source did not load exactly once.");

            // 1/8. Fresh glyph starts without Edited state.
            if (!font.SelectGlyphForTest(0x42) || font.GlyphHasEditedForTest(0x42))
                throw new InvalidDataException("Glyph 0x42 is not in view-only state.");
            byte[] original42 = font.GlyphOriginalForTest(0x42);
            if (original42.Length != 8 || original42.All(value => value == 0))
                throw new InvalidDataException("Glyph 0x42 has no valid original bitmap.");
            if (!font.IsClearGlyphEnabledForTest)
                throw new InvalidDataException("Clear glyph is not available for a normal editable glyph.");

            // 2/3/4. Clear creates explicit zero state; Original untouched.
            font.ClearCurrentGlyphForTest();
            if (!font.GlyphHasEditedForTest(0x42))
                throw new InvalidDataException("Clear glyph did not create Edited state.");
            if (!font.GlyphEditedForTest(0x42).SequenceEqual(new byte[8]) ||
                !font.GlyphOriginalForTest(0x42).SequenceEqual(original42))
                throw new InvalidDataException("Clear glyph did not produce an exact zero bitmap or modified Original.");

            // 5. First pixel after Clear modifies the ZERO bitmap (mask 0x20 at row 1, col 2).
            font.ToggleCurrentPixelForTest(1, 2);
            byte[] afterPixel = font.GlyphEditedForTest(0x42);
            for (int row = 0; row < 8; row++)
            {
                byte expected = row == 1 ? (byte)0x20 : (byte)0x00;
                if (afterPixel[row] != expected)
                    throw new InvalidDataException("Pixel edit after Clear did not modify the zero bitmap.");
            }

            // 6/7. Reset restores exact Original bytes and retains explicit-copy semantics.
            font.ResetCurrentGlyphForTest();
            if (!font.GlyphHasEditedForTest(0x42) || !font.GlyphEditedForTest(0x42).SequenceEqual(original42))
                throw new InvalidDataException("Reset after Clear did not restore exact Original bytes as an explicit copy.");

            // 8. Viewing another fresh glyph creates nothing.
            if (!font.SelectGlyphForTest(0x43) || font.GlyphHasEditedForTest(0x43))
                throw new InvalidDataException("Selecting a fresh glyph created Edited state.");

            // 9. Reserved glyph rejects Clear (disabled + no state).
            if (!font.SelectGlyphForTest(FontSlotMetadata.HudEraseGlyph))
                throw new InvalidDataException("Reserved glyph 0x81 could not be selected.");
            if (font.IsClearGlyphEnabledForTest)
                throw new InvalidDataException("Clear glyph is available for reserved 0x81.");
            font.ClearCurrentGlyphForTest();
            if (font.GlyphHasEditedForTest(FontSlotMetadata.HudEraseGlyph))
                throw new InvalidDataException("Clear created working state for reserved 0x81.");

            // MODEL level: ClearToZero() itself fails closed for reserved
            // 0x81 even when invoked directly, bypassing all UI gates. A
            // normal model glyph still clears (positive control).
            var reservedModel = new GlyphModel(FontSlotMetadata.HudEraseGlyph, "model-probe", "model-probe");
            bool rejected = false;
            try { reservedModel.ClearToZero(); }
            catch (InvalidOperationException) { rejected = true; }
            if (!rejected || reservedModel.HasEdited || !reservedModel.Edited.SequenceEqual(new byte[8]))
                throw new InvalidDataException("Model-level ClearToZero accepted reserved 0x81 or mutated it.");
            var normalModel = new GlyphModel(0x42, "model-probe", "model-probe");
            normalModel.ClearToZero();
            if (!normalModel.HasEdited || !normalModel.Edited.SequenceEqual(new byte[8]))
                throw new InvalidDataException("Model-level ClearToZero regressed for a normal glyph.");

            // Unsupported slot: the packed 98-glyph table ends at 0x81, so
            // 0x82 has no source glyph in this layout.
            if (!font.SelectGlyphForTest(0x82))
                throw new InvalidDataException("Unsupported slot 0x82 could not be selected.");
            if (font.IsClearGlyphEnabledForTest)
                throw new InvalidDataException("Clear glyph is available for an unsupported slot.");
            font.ClearCurrentGlyphForTest();
            if (font.GlyphHasEditedForTest(0x82))
                throw new InvalidDataException("Clear created working state for an unsupported slot.");

            // 10/11/12. In-memory only: no executable/VARIANTS mutation.
            if (HashFile(runVgaPath) != runVgaHash || HashFile(gamePcPath) != gamePcHash ||
                Directory.Exists(Path.Combine(fixture.GameRoot, "VARIANTS")))
                throw new InvalidDataException("Clear-glyph editing wrote to game files.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRuntimeUiModsConsistencySmoke(string elvira1Source, string elvira2Source)
    {
        // R9F UI/Mods consistency: grid column UX + Pause-only Reset gating +
        // owned-variant availability for translated Mods entries.
        // A. Columns resizable with sane initials (Details is the fill share).
        // B. Manual widths survive refresh and language switches.
        // C. Details full text reachable via tooltip. D/E. Reset enabled only
        // for Pause+override. F. Reset restores blank/original fallback.
        // G. Pristine EN GAMEPC Available. H. Built SK GAMEPCSK Available from
        // the owned variant dir. I. Unbuilt S1 genuinely Missing. J. Launcher
        // target uses the same resolved path. K. No GameRoot writes.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiModsSmoke", Guid.NewGuid().ToString("N"));
        string priorLocale = UiText.LocaleId;
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            text = translations.Add(text, skCreated);
            TranslationProjectVariant s1Created = translations.Create(fixture, text, "Slovencina test", "S1", new Dictionary<int, string> { [393] = "Prirucna taska." });
            text = translations.Add(text, s1Created);
            translations.Save(fixture, text);
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Mods consistency fixture did not validate as an installation.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            if (!form.RuntimeUiGridColumnsResizableForTest)
                throw new InvalidDataException("Runtime UI columns are not all manually resizable.");
            if (form.RuntimeUiGridColumnWidthForTest("LogicalId") is < 140 or > 160 ||
                form.RuntimeUiGridColumnWidthForTest("Original") is < 180 or > 220)
                throw new InvalidDataException("Runtime UI initial Record/Original widths regressed.");
            if (form.RuntimeUiGridColumnWidthForTest("Override") is < 250 or > 350 ||
                form.RuntimeUiGridColumnWidthForTest("Status") is < 180 or > 240)
                throw new InvalidDataException("Runtime UI initial Override/Validation widths regressed.");
            // B. Manual widths survive refresh and language switches.
            form.SetRuntimeUiGridColumnWidthForTest("Override", 333);
            form.QueueRuntimeUiGridRefreshForTest();
            if (form.RuntimeUiGridColumnWidthForTest("Override") != 333)
                throw new InvalidDataException("Refresh reset manually adjusted Runtime UI widths.");
            UiText.SetLocale("sk");
            if (form.RuntimeUiGridColumnWidthForTest("Override") != 333)
                throw new InvalidDataException("Language switch reset Runtime UI widths.");
            UiText.SetLocale(priorLocale);
            // C. Details full text obtainable via tooltip.
            if (!form.RuntimeUiGridPauseDetailTooltipForTest.Equals(UiText.Get("RuntimeUi.Detail.Valid"), StringComparison.Ordinal))
                throw new InvalidDataException("Details tooltip does not carry the full diagnostic text.");
            form.SelectTranslationForTest("SK");
            _ = form.Handle;
            int baseline = form.RuntimeUiGridRefreshCountForTest;
            // D. Reset enabled for Pause.menu only when an override exists.
            // R9F V8: overrides arrive via the semantic editor, never inline.
            form.ApplySemanticEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "Testovacia");
            PumpRuntimeUiGrid(form, baseline);
            baseline = form.RuntimeUiGridRefreshCountForTest;
            if (!form.SelectRuntimeUiRowForTest(RuntimeUiLogicalRecordId.PauseMenu) || !form.RuntimeUiResetEnabledForTest)
                throw new InvalidDataException("Reset was not enabled for Pause.menu with an override.");
            // E. Reset disabled on rows without overrides (including the
            // still-unsupported five and the not-yet-overridden
            // proven-live rows).
            foreach (RuntimeUiLogicalRecordId id in new[] { RuntimeUiLogicalRecordId.ConfirmGeneric, RuntimeUiLogicalRecordId.SavePrompt, RuntimeUiLogicalRecordId.SaveFailure, RuntimeUiLogicalRecordId.LoadFailure, RuntimeUiLogicalRecordId.FileNotFound, RuntimeUiLogicalRecordId.TryAnotherDisk, RuntimeUiLogicalRecordId.SaveOverwrite })
            {
                if (!form.SelectRuntimeUiRowForTest(id))
                    throw new InvalidDataException("Runtime UI row is missing: " + id);
                if (form.RuntimeUiResetEnabledForTest)
                    throw new InvalidDataException("Reset was enabled on unsupported row: " + id);
            }
            // F. Reset clears the title override and restores the fallback.
            if (!form.SelectRuntimeUiRowForTest(RuntimeUiLogicalRecordId.PauseMenu))
                throw new InvalidDataException("Pause.menu row vanished.");
            form.ResetRuntimeUiOverrideForTest();
            PumpRuntimeUiGrid(form, baseline);
            if (form.RuntimeUiOverrideCountForTest != 0 ||
                !form.RuntimeUiGridPauseOverrideForTest.Equals(string.Empty, StringComparison.Ordinal) ||
                !form.RuntimeUiGridPauseOriginalForTest.Equals("Game Paused", StringComparison.Ordinal) ||
                !form.RuntimeUiDirtyForTest || !form.RuntimeUiSaveEnabledForTest)
                throw new InvalidDataException("Reset did not restore blank override with original fallback.");
            // G. Pristine EN baseline resolves to GameRoot.
            if (!form.IsVariantEntryAvailableForTest("GAMEPC"))
                throw new InvalidDataException("Pristine GAMEPC did not resolve Available.");
            if (!form.VariantEntryResolvedPathForTest("GAMEPC").Equals(Path.Combine(fixture.GameRoot, "GAMEPC"), StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("GAMEPC did not resolve to the pristine GameRoot path.");
            // H/I. Build SK only: SK becomes Available from the owned variant
            // dir, unbuilt S1 stays genuinely Missing.
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            TranslationProjectVariant sk = translations.Load(fixture).State!.Variants.Single(item => item.Code == "SK");
            var graphics = new GraphicsVariantService();
            var directories = new VariantDirectoryService();
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, vga, sk.Code), VariantDirectoryOperationStatus.Created, "mods fixture variant directory");
            CompositeBuildService skBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skBuild.Build(fixture, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("SK fixture build failed.");
            string expectedSk = Path.Combine(directories.GetVariantEditionDirectoryPath(fixture, vga, sk.Code), "GAMEPCSK");
            if (!form.IsVariantEntryAvailableForTest("GAMEPCSK") || !File.Exists(expectedSk))
                throw new InvalidDataException("Built SK GAMEPCSK did not resolve Available from the owned build location.");
            if (!form.VariantEntryResolvedPathForTest("GAMEPCSK").Equals(expectedSk, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("SK availability did not use the owned variant directory.");
            form.RefreshVariantGridForTest();
            if (!form.VariantGridRowStatusForTest("GAMEPCSK").Equals(UiText.Get("VariantAvailable"), StringComparison.Ordinal))
                throw new InvalidDataException("Mods grid does not show the built SK entry Available.");
            if (form.IsVariantEntryAvailableForTest("GAMEPCS1") ||
                !form.VariantGridRowStatusForTest("GAMEPCS1").Equals(UiText.Get("VariantMissing"), StringComparison.Ordinal))
                throw new InvalidDataException("Unbuilt S1 entry did not resolve genuinely Missing.");
            // J. Launcher/readiness target uses the same resolved artifact path.
            if (!form.VariantLaunchDataPathForTest().Equals(expectedSk, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Launcher target diverged from the availability path.");
            // K. Nothing translated was written to pristine GameRoot.
            if (File.Exists(Path.Combine(fixture.GameRoot, "GAMEPCSK")) || File.Exists(Path.Combine(fixture.GameRoot, "GAMEPCS1")))
                throw new InvalidDataException("Translated data reached pristine GameRoot.");
            _ = elvira2Source;
        }
        finally
        {
            UiText.SetLocale(priorLocale);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyVariantEditionIdentitySmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V3 runnable identity is Installation + Project + Edition +
        // Runtime. Owned outputs live in VARIANTS\<RuntimeKey>\<EditionCode>
        // (VARIANTS\E1VGA\SK): SK and S1 builds for the same runtime coexist
        // independently, and Variant Manager status never depends on which
        // runtime is selected. S1-never-built must read Missing (not corrupt).
        string root = Path.Combine(Path.GetTempPath(), "Pi1VariantEditionSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            text = translations.Add(text, skCreated);
            TranslationProjectVariant s1Created = translations.Create(fixture, text, "Slovencina test", "S1", new Dictionary<int, string> { [393] = "Prirucna taska." });
            text = translations.Add(text, s1Created);
            translations.Save(fixture, text);
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Edition identity fixture did not validate as an installation.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            if (form.ActiveVariantForTest?.VariantId != BuiltInVariantId.Elvira1Vga)
                throw new InvalidDataException("Default active runtime is not Elvira I VGA.");
            form.SelectTranslationForTest("SK");
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            // The five remaining VGA rows are genuinely non-editable at
            // every layer: locked cells, programmatic edits rejected without
            // touching state/dirty/actions. (Pause.menu, Confirm.generic and
            // Save.overwrite are proven-live editable variable-width rows.)
            foreach (RuntimeUiLogicalRecordId id in new[] { RuntimeUiLogicalRecordId.SavePrompt, RuntimeUiLogicalRecordId.SaveFailure, RuntimeUiLogicalRecordId.LoadFailure, RuntimeUiLogicalRecordId.FileNotFound, RuntimeUiLogicalRecordId.TryAnotherDisk })
            {
                if (form.RuntimeUiGridCellEditableForTest(id))
                    throw new InvalidDataException("Unsupported VGA row is editable: " + id);
                form.ApplyRuntimeUiGridEditForTest(id, "xxx");
                if (form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest || form.RuntimeUiSaveEnabledForTest || form.RuntimeUiResetEnabledForTest)
                    throw new InvalidDataException("Unsupported VGA edit altered state: " + id);
            }
            form.QueueRuntimeUiGridRefreshForTest();
            var graphics = new GraphicsVariantService();
            var directories = new VariantDirectoryService();
            TranslationProjectVariant sk = translations.Load(fixture).State!.Variants.Single(item => item.Code == "SK");
            TranslationProjectVariant s1 = translations.Load(fixture).State!.Variants.Single(item => item.Code == "S1");
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, vga, sk.Code), VariantDirectoryOperationStatus.Created, "edition fixture variant directory");
            // A. Build E1VGA/SK (edition identity recorded in the manifest).
            CompositeBuildService skBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skBuild.Build(fixture, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("SK fixture build failed.");
            string skRoot = directories.GetVariantEditionDirectoryPath(fixture, vga, sk.Code);
            string s1Root = directories.GetVariantEditionDirectoryPath(fixture, vga, s1.Code);
            string expectedSk = Path.Combine(skRoot, "GAMEPCSK");
            string expectedS1 = Path.Combine(s1Root, "GAMEPCS1");
            if (!File.Exists(expectedSk))
                throw new InvalidDataException("SK build did not materialize GAMEPCSK in the owned runtime+edition directory.");
            if (VariantManifestService.Read(skRoot).ProjectCode != "SK")
                throw new InvalidDataException("Manifest identity does not record the built edition.");
            // B. E1VGA reports Ready.
            if (!form.RuntimeVariantBuildStatusForTest(BuiltInVariantId.Elvira1Vga).Equals(VariantBuildStatus.Ready.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Built E1VGA/SK was not reported Ready.");
            // C/D. Switch to EGA: E1VGA must remain Ready (selection-independent).
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            if (!form.RuntimeVariantBuildStatusForTest(BuiltInVariantId.Elvira1Vga).Equals(VariantBuildStatus.Ready.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("E1VGA lost Ready status merely because EGA was selected.");
            if (!form.RuntimeVariantBuildStatusForTest(BuiltInVariantId.Elvira1Ega).Equals(VariantBuildStatus.Missing.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Unbuilt EGA row did not stay Missing.");
            // E/F. Switch back: still Ready, no physical change.
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            if (!form.RuntimeVariantBuildStatusForTest(BuiltInVariantId.Elvira1Vga).Equals(VariantBuildStatus.Ready.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("E1VGA did not stay Ready after switching back.");
            // G covered by D+F (both directions, same physical artifacts).
            // H. GAMEPCSK resolves to the owned artifact (not GameRoot, not metadata).
            if (!form.IsVariantEntryAvailableForTest("GAMEPCSK") || !File.Exists(expectedSk))
                throw new InvalidDataException("GAMEPCSK did not resolve to the owned artifact.");
            if (!form.VariantEntryResolvedPathForTest("GAMEPCSK").Equals(expectedSk, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("GAMEPCSK resolved to the wrong location.");
            // I/J. GAMEPCS1 resolves to its exact expected location and reads
            // Missing because it was never built — not corrupt, no error state.
            if (!form.VariantEntryResolvedPathForTest("GAMEPCS1").Equals(expectedS1, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("GAMEPCS1 did not resolve to its owned location.");
            if (form.IsVariantEntryAvailableForTest("GAMEPCS1") ||
                !form.VariantGridRowStatusForTest("GAMEPCS1").Equals(UiText.Get("VariantMissing"), StringComparison.Ordinal))
                throw new InvalidDataException("Never-built S1 was not cleanly Missing.");
            // M-positive: with SK built and selected, launcher/readiness uses
            // the same resolved artifact path as availability.
            if (!form.VariantLaunchDataPathForTest().Equals(expectedSk, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Launcher target diverged from the availability path.");
            // K/L. Building S1 coexists with SK: each edition owns an
            // independent runtime+edition directory, and neither build
            // deletes or replaces the other.
            string skHashBeforeS1 = HashFile(expectedSk);
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, vga, s1.Code), VariantDirectoryOperationStatus.Created, "S1 edition fixture directory");
            CompositeBuildService s1Build = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(s1, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, s1), s1.DataFile],
                projectVariantProvider: (_, _) => s1.Code);
            if (s1Build.Build(fixture, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("S1 fixture build failed.");
            if (!File.Exists(expectedS1))
                throw new InvalidDataException("S1 build did not materialize GAMEPCS1 in its owned runtime+edition directory.");
            if (!File.Exists(expectedSk) || !HashFile(expectedSk).Equals(skHashBeforeS1, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("S1 build deleted or replaced the SK output.");
            if (VariantManifestService.Read(s1Root).ProjectCode != "S1" || VariantManifestService.Read(skRoot).ProjectCode != "SK")
                throw new InvalidDataException("Edition manifests do not record their independent identities.");
            // M-coexistence: with SK still selected, SK stays Available and
            // the launcher still targets SK; selecting S1 targets S1.
            form.RefreshVariantGridForTest();
            if (!form.IsVariantEntryAvailableForTest("GAMEPCSK") ||
                !form.VariantGridRowStatusForTest("GAMEPCSK").Equals(UiText.Get("VariantAvailable"), StringComparison.Ordinal))
                throw new InvalidDataException("SK lost availability merely because S1 was built.");
            if (!form.IsVariantEntryAvailableForTest("GAMEPCS1") ||
                !form.VariantGridRowStatusForTest("GAMEPCS1").Equals(UiText.Get("VariantAvailable"), StringComparison.Ordinal))
                throw new InvalidDataException("Built S1 entry is not Available alongside SK.");
            if (!form.VariantLaunchDataPathForTest().Equals(expectedSk, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("SK launcher target diverged after the S1 build.");
            form.SelectTranslationForTest("S1");
            if (!form.VariantLaunchDataPathForTest().Equals(expectedS1, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("S1 launcher target did not resolve the S1 artifact.");
            form.SelectTranslationForTest("SK");
            // Cross-runtime isolation: building EGA/SK changes neither VGA artifact.
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, ega, sk.Code), VariantDirectoryOperationStatus.Created, "EGA edition fixture directory");
            CompositeBuildService egaBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (egaBuild.Build(fixture, ega, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("EGA/SK fixture build failed.");
            if (!HashFile(expectedSk).Equals(skHashBeforeS1, StringComparison.OrdinalIgnoreCase) || !File.Exists(expectedS1))
                throw new InvalidDataException("EGA build touched VGA edition artifacts.");
            if (!form.RuntimeVariantBuildStatusForTest(BuiltInVariantId.Elvira1Vga).Equals(VariantBuildStatus.Ready.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("VGA/SK lost Ready status after the EGA build.");
            // N. Pristine GameRoot never receives generated translations.
            if (File.Exists(Path.Combine(fixture.GameRoot, "GAMEPCSK")) || File.Exists(Path.Combine(fixture.GameRoot, "GAMEPCS1")))
                throw new InvalidDataException("Translated data reached pristine GameRoot.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunEgaUiSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F RUNEGA audit + materializer. A. Frozen descriptor cross-check.
        // B. Authoritative originals decode from the packed source. C. No
        // undecodable record (all eight decode; reasons N/A). D. Support
        // classifications pinned. E-Q per enabled field/record. R/S frozen
        // subfield protection. No RUNVGA/RUNIT behavior changes here.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunEgaUiSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var texts = new RuntimeUiTextService();
            var layouts = new RuntimeUiLayoutValidationService(texts);
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            RuntimeUiTextState empty = texts.Load(e1).State!;
            // A. Contract table matches the frozen bank descriptor + evidence.
            var expectedEvidence = new Dictionary<RuntimeUiLogicalRecordId, FrozenRuntimeEvidence>
            {
                [RuntimeUiLogicalRecordId.PauseMenu] = FrozenRuntimeEvidence.ProvenLive,
                [RuntimeUiLogicalRecordId.ConfirmGeneric] = FrozenRuntimeEvidence.ProvenByBinary,
                [RuntimeUiLogicalRecordId.SavePrompt] = FrozenRuntimeEvidence.ProvenLive,
                [RuntimeUiLogicalRecordId.SaveFailure] = FrozenRuntimeEvidence.ProvenByBinary,
                [RuntimeUiLogicalRecordId.LoadFailure] = FrozenRuntimeEvidence.ProvenByBinary,
                [RuntimeUiLogicalRecordId.FileNotFound] = FrozenRuntimeEvidence.ProvenByBinary,
                [RuntimeUiLogicalRecordId.TryAnotherDisk] = FrozenRuntimeEvidence.ProvenByBinary,
                [RuntimeUiLogicalRecordId.SaveOverwrite] = FrozenRuntimeEvidence.ProvenByBinary,
            };
            foreach (RunEgaUiFieldContract contract in RunEgaUiService.Contracts)
            {
                FrozenRuntimeUiRecord frozen = Elvira1ProductionProfile.RunEga.RuntimeUi.RuntimeRecords
                    .Single(record => record.Name.Equals(contract.DisplayName, StringComparison.Ordinal));
                if (frozen.BankOffset != contract.BankOffset || frozen.Length != contract.Length || frozen.Evidence != expectedEvidence[contract.Id])
                    throw new InvalidDataException("EGA contract diverges from the frozen descriptor: " + contract.DisplayName);
                RuntimeUiEvidenceStatus expectedStatus = frozen.Evidence == FrozenRuntimeEvidence.ProvenLive
                    ? RuntimeUiEvidenceStatus.ProvenLive : RuntimeUiEvidenceStatus.ProvenByBinary;
                if (texts.GetEffectiveRecords(ega, empty).Single(value => value.LogicalRecordId == contract.Id) is not { MappingReadiness: RuntimeUiMappingReadiness.SupportedAndMapped } projection ||
                    projection.EvidenceStatus != expectedStatus)
                    throw new InvalidDataException("EGA mapping/evidence regressed: " + contract.DisplayName);
            }
            // B. Authoritative originals (pinned binary facts, strict CP852).
            var expectedOriginals = new Dictionary<RuntimeUiLogicalRecordId, string>
            {
                [RuntimeUiLogicalRecordId.PauseMenu] = "     Game Paused\r\r\r Continue      Quit",
                [RuntimeUiLogicalRecordId.ConfirmGeneric] = "    Are you sure ?\r\r\r     Yes       No",
                [RuntimeUiLogicalRecordId.SavePrompt] = "\r Insert savegame data disk & enter filename:\r\r   ",
                [RuntimeUiLogicalRecordId.SaveFailure] = "\r    Save failed.",
                [RuntimeUiLogicalRecordId.LoadFailure] = "\r    Load failed.",
                [RuntimeUiLogicalRecordId.FileNotFound] = "\r  File not found.",
                [RuntimeUiLogicalRecordId.TryAnotherDisk] = "\r  Try another disk.",
                [RuntimeUiLogicalRecordId.SaveOverwrite] = "\r File already exists.\r\r    Overwrite it ?\r\r     Yes       No",
            };
            string packedRunEga = Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable);
            foreach ((RuntimeUiLogicalRecordId id, string expected) in expectedOriginals)
            {
                if (!RunEgaUiService.TryDecodeOriginal(packedRunEga, id, out string? decoded, out _) || !decoded!.Equals(expected, StringComparison.Ordinal))
                    throw new InvalidDataException("EGA original decode diverged: " + id);
            }
            // D. Classifications pinned (no D by audit evidence; mechanism kept data-driven).
            if (RunEgaUiService.Contract(RuntimeUiLogicalRecordId.PauseMenu).Kind != RunEgaUiContractKind.SafeStructuredText ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.ConfirmGeneric).Kind != RunEgaUiContractKind.SafeStructuredText ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SavePrompt).Kind != RunEgaUiContractKind.SafeStructuredText ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SaveFailure).Kind != RunEgaUiContractKind.SafeSimpleText ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SaveOverwrite).Kind != RunEgaUiContractKind.SafeStructuredText ||
                !RunEgaUiService.Contracts.All(contract => RunEgaUiService.IsEditable(contract.Id)))
                throw new InvalidDataException("EGA support classification regressed.");
            if (RunEgaUiService.Contract(RuntimeUiLogicalRecordId.PauseMenu).MaxFieldBytes != 14 ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.ConfirmGeneric).MaxFieldBytes != 14 ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SavePrompt).MaxFieldBytes != 43 ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SaveFailure).MaxFieldBytes != 12 ||
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SaveOverwrite).MaxFieldBytes != 21)
                throw new InvalidDataException("EGA field capacity regressed.");
            // E/F/H/I/J per enabled field (valid translated values).
            var validFields = new Dictionary<RuntimeUiLogicalRecordId, string>
            {
                [RuntimeUiLogicalRecordId.PauseMenu] = "Pozastavené",
                [RuntimeUiLogicalRecordId.ConfirmGeneric] = "Naozaj?",
                [RuntimeUiLogicalRecordId.SavePrompt] = "Vložte disk a zadajte meno:",
                [RuntimeUiLogicalRecordId.SaveFailure] = "Neuložené!",
                [RuntimeUiLogicalRecordId.LoadFailure] = "Chyba čít.",
                [RuntimeUiLogicalRecordId.FileNotFound] = "Súbor chýba.",
                [RuntimeUiLogicalRecordId.TryAnotherDisk] = "Skúste iný disk.",
                [RuntimeUiLogicalRecordId.SaveOverwrite] = "Súbor už existuje.",
            };
            foreach ((RuntimeUiLogicalRecordId id, string field) in validFields)
            {
                RunEgaUiFieldContract contract = RunEgaUiService.Contract(id);
                if (!RunEgaUiService.TryComposeField(id, field, out string full, out _, out _))
                    throw new InvalidDataException("Valid EGA field was rejected: " + id);
                if (!full.StartsWith(contract.Prefix, StringComparison.Ordinal) ||
                    (contract.Tail.Length > 0 && !full.EndsWith(contract.Tail, StringComparison.Ordinal)) || !full.Contains(field, StringComparison.Ordinal))
                    throw new InvalidDataException("EGA composition broke frozen formatting: " + id);
                RuntimeUiLayoutValidationResult validated = layouts.Validate(ega, texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Ega, id, full), id);
                if (validated.Status != RuntimeUiLayoutValidationStatus.Valid || validated.RecordCapacity != contract.Length)
                    throw new InvalidDataException("Valid EGA override did not validate: " + id + " / " + validated.Detail);
                if (!RunEgaUiService.TryExtractField(full, id, out string? back) || !back!.Equals(field, StringComparison.Ordinal))
                    throw new InvalidDataException("EGA field round-trip diverged: " + id);
            }
            // G. NUL/control rejection per field.
            foreach (RuntimeUiLogicalRecordId id in validFields.Keys)
            {
                if (RunEgaUiService.TryComposeField(id, "A\0B", out _, out _, out _) ||
                    RunEgaUiService.TryComposeField(id, "A\x01B", out _, out _, out _) ||
                    RunEgaUiService.TryComposeField(id, string.Empty, out _, out _, out _))
                    throw new InvalidDataException("EGA NUL/control/empty field was accepted: " + id);
            }
            // K/L. Save/load roundtrip + reset for one record.
            RuntimeUiTextState pauseState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.PauseMenu,
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.PauseMenu).Prefix + "Pozastavené" + "\r\r\r Continue      Quit");
            if (!texts.Save(e1, pauseState).Succeeded || !texts.Load(e1).IsSuccess)
                throw new InvalidDataException("EGA project save/load regressed.");
            RuntimeUiTextState pauseReset = texts.RemoveOverride(e1, texts.Load(e1).State!, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.PauseMenu);
            if (pauseReset.Overrides.Count != 0 || !texts.Save(e1, pauseReset).Succeeded)
                throw new InvalidDataException("EGA reset regressed.");
            // R/S. Frozen geometry is protected: shifted buttons fail with a
            // fixed-column violation; structurally damaged records without
            // intact keywords fail as hotspot violations; both block the build.
            // Translated button labels that preserve every column stay valid
            // (V4 hotspot-label contract).
            RuntimeUiTextState shifted = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.ConfirmGeneric, "    Are you sure ?\r\r\r      Yes     No");
            RuntimeUiLayoutValidationResult shiftedResult = layouts.Validate(ega, shifted, RuntimeUiLogicalRecordId.ConfirmGeneric);
            if (shiftedResult.Status != RuntimeUiLayoutValidationStatus.FixedColumnViolation)
                throw new InvalidDataException("EGA shifted buttons were accepted: " + shiftedResult.Status + " / " + shiftedResult.Detail);
            RuntimeUiTextState damaged = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.PauseMenu, "     Game Paused\r\rX Avanti Salir");
            if (layouts.Validate(ega, damaged, RuntimeUiLogicalRecordId.PauseMenu).Status != RuntimeUiLayoutValidationStatus.HotspotViolation)
                throw new InvalidDataException("EGA separator damage was accepted.");
            if (new RuntimeUiProjectBuildStep(shifted).Preflight(e1, ega) is null)
                throw new InvalidDataException("EGA subfield damage did not block the build.");
            // EN gate split: runtime-ui stage passes contract-valid state, the
            // executable stage demands a translated variant.
            RuntimeUiTextState oneValid = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    Uložené!");
            var egaDirectories = new VariantDirectoryService();
            var english = new TranslationProjectVariant("English", "EN", "GAMEPC", ega.SourceExecutableName, new Dictionary<int, string>());
            string? enGate = new TranslationAwareExecutableBuildStep(egaDirectories, english, oneValid).Preflight(e1, ega);
            if (enGate is null || !enGate.Contains("translated variant", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("EGA EN-gate did not fail closed.");
            if (new RuntimeUiProjectBuildStep(oneValid).Preflight(e1, ega) is not null)
                throw new InvalidDataException("Contract-valid EGA state falsely blocked the runtime-ui stage.");
            // Form-level EGA grid: field-only display, all cells editable,
            // compose-on-edit, row-specific reset.
            {
                var formTranslations = new TranslationProjectService();
                TranslationProjectState formText = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
                TranslationProjectVariant formSk = formTranslations.Create(e1, formText, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
                formTranslations.Save(e1, formTranslations.Add(formText, formSk));
                if (!GameInstallationValidator.TryValidate(e1.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1inst) || e1inst is null)
                    throw new InvalidDataException("EGA grid fixture did not validate as an installation.");
                using var form = new MainForm();
                form.InitializeInstallationStateForTest();
                form.ActivateInstallationForTest(e1inst);
                form.SelectTranslationForTest("SK");
                form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
                if (!form.RuntimeUiGridAllOverrideCellsReadOnlyForTest)
                    throw new InvalidDataException("R9F V8 EGA grid is not fully read-only.");
                if (!form.RuntimeUiGridPauseOriginalForTest.Equals("Game Paused", StringComparison.Ordinal))
                    throw new InvalidDataException("EGA Pause title display regressed.");
                // R9F V8: EGA edits also go through the semantic editor, never inline.
                form.ApplySemanticEditForTest(RuntimeUiLogicalRecordId.ConfirmGeneric, "Naozaj?");
                if (!form.RuntimeUiGridPauseOverrideForTest.Equals(string.Empty, StringComparison.Ordinal) ||
                    form.RuntimeUiOverrideCountForTest != 1 || !form.RuntimeUiDirtyForTest || !form.RuntimeUiSaveEnabledForTest)
                    throw new InvalidDataException("EGA grid edit diverged.");
                if (!form.SelectRuntimeUiRowForTest(RuntimeUiLogicalRecordId.ConfirmGeneric) || !form.RuntimeUiResetEnabledForTest)
                    throw new InvalidDataException("EGA Reset was not enabled for the edited row.");
                form.ResetRuntimeUiOverrideForTest();
                form.QueueRuntimeUiGridRefreshForTest();
                if (form.RuntimeUiOverrideCountForTest != 0 || !form.RuntimeUiDirtyForTest)
                    throw new InvalidDataException("EGA grid reset regressed.");
            }
            // M/N/O/P/Q. Materializer end-to-end with two overrides.
            var translations = new TranslationProjectService();
            TranslationProjectVariant sk = translations.Load(e1).State!.Variants.Single(item => item.Code == "SK");
            var graphics = new GraphicsVariantService();
            var directories = new VariantDirectoryService();
            RuntimeUiTextState uiState = texts.SetOverride(e1, empty, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.PauseMenu,
                RunEgaUiService.Contract(RuntimeUiLogicalRecordId.PauseMenu).Prefix + "Pozastavené" + "\r\r\r Continue      Quit");
            uiState = texts.SetOverride(e1, uiState, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    Neuložené!");
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, ega, sk.Code), VariantDirectoryOperationStatus.Created, "ega fixture variant directory");
            string pristineEgaHash = HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable));
            CompositeBuildService egaBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), uiState)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (egaBuild.Build(e1, ega, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("EGA composite build failed.");
            string variantRoot = directories.GetVariantEditionDirectoryPath(e1, ega, sk.Code);
            string patchedEga = Path.Combine(variantRoot, ActiveProjectBuildIdentity.ExecutableName(ega, sk));
            byte[] after = File.ReadAllBytes(patchedEga);
            string tmpSrc = Path.Combine(root, "tmp-pristine-runega.exe");
            string tmpOut = Path.Combine(root, "tmp-pristine-egaout.exe");
            File.Copy(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable), tmpSrc, true);
            RunEgaBootstrapService.CreateFrozenCp852(tmpSrc, tmpOut, GlyphRepository.CreateAllCp852Slots());
            byte[] beforeEga = File.ReadAllBytes(tmpOut);
            RunEgaUiFieldContract[] enabledSpans = [RunEgaUiService.Contract(RuntimeUiLogicalRecordId.PauseMenu), RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SaveFailure)];
            RunEgaUiService.VerifyOnlyEgaSpansChanged(beforeEga, after, enabledSpans);
            RunEgaBootstrapService.ValidateOutput(after);
            if (!RunEgaUiService.TryDecodeBankSpan(after, RuntimeUiLogicalRecordId.PauseMenu, out string? patchedPause, out _) ||
                !RunEgaUiService.TryDecodeBankSpan(after, RuntimeUiLogicalRecordId.SaveFailure, out string? patchedFailed, out _))
                throw new InvalidDataException("Patched EGA spans did not decode.");
            if (!patchedPause!.Contains("Pozastavené", StringComparison.Ordinal) || !patchedFailed!.Contains("Neuložené!", StringComparison.Ordinal))
                throw new InvalidDataException("Patched EGA output lost the translations.");
            if (HashFile(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable)) != pristineEgaHash)
                throw new InvalidDataException("EGA build modified the pristine source.");
            string hash1 = HashFile(patchedEga);
            if (egaBuild.Build(e1, ega, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success ||
                !HashFile(patchedEga).Equals(hash1, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("EGA rebuild was not deterministic.");
            // Q. No-op override (original full) applies byte-identically.
            if (!RunEgaUiService.TryDecodeOriginal(Path.Combine(e1.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable), RuntimeUiLogicalRecordId.SaveFailure, out string? originalFull, out _))
                throw new InvalidDataException("EGA no-op source did not decode.");
            byte[] noopAfter = (byte[])beforeEga.Clone();
            RunEgaUiService.ApplyOverridesToImage(noopAfter, [new RuntimeUiTextOverride(VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, originalFull!)]);
            RunEgaUiService.VerifyOnlyEgaSpansChanged(beforeEga, noopAfter,
                [RunEgaUiService.Contract(RuntimeUiLogicalRecordId.SaveFailure)]);
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void PumpRuntimeUiGrid(MainForm form, int baseline)
    {
        DateTime deadline = DateTime.UtcNow.AddSeconds(15);
        while (form.RuntimeUiGridRefreshCountForTest == baseline)
        {
            if (DateTime.UtcNow >= deadline)
                throw new InvalidDataException("Queued Runtime UI refresh did not complete on the STA loop.");
            Application.DoEvents();
            Thread.Sleep(5);
        }
    }

    private static void VerifyLegacyAdoptSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V4 §3: controlled, explicit adoption of a proven legacy flat
        // owned output into its runtime+edition directory. All-or-nothing:
        // aborts leave every byte untouched; success restores normal
        // ownership so Rebuild becomes available.
        string root = Path.Combine(Path.GetTempPath(), "Pi1LegacyAdoptSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            text = translations.Add(text, skCreated);
            translations.Save(fixture, text);
            TranslationProjectVariant sk = translations.Load(fixture).State!.Variants.Single(item => item.Code == "SK");
            var graphics = new GraphicsVariantService();
            var directories = new VariantDirectoryService();
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, vga, sk.Code), VariantDirectoryOperationStatus.Created, "adopt fixture edition directory");
            CompositeBuildService skBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skBuild.Build(fixture, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Adopt fixture SK build failed.");
            string editionRoot = directories.GetVariantEditionDirectoryPath(fixture, vga, sk.Code);
            string runtimeRoot = directories.GetVariantDirectoryPath(fixture, vga);
            var editionPayload = Directory.EnumerateFiles(editionRoot, "*", SearchOption.AllDirectories)
                .Where(path => !Path.GetFileName(path).Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase))
                .Select(path => (Relative: Path.GetRelativePath(editionRoot, path), Hash: HashFile(path))).ToArray();
            string legacyMarkerJson = "{\"SchemaVersion\":1,\"Product\":\"Pi1ElviraVariantDirectory\",\"GameId\":\"Elvira1\",\"VariantId\":\"Elvira1Vga\",\"DirectoryKey\":\"E1VGA\",\"BaselineFingerprint\":\"" + fixture.BaselineFingerprint + "\"}";

            void SimulateLegacy()
            {
                foreach (string file in Directory.EnumerateFiles(editionRoot, "*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(file).Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)) continue;
                    string destination = Path.Combine(runtimeRoot, Path.GetRelativePath(editionRoot, file));
                    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                    File.Move(file, destination);
                }
                File.Delete(Path.Combine(editionRoot, VariantDirectoryService.OwnershipMarkerFileName));
                if (Directory.EnumerateFiles(editionRoot, "*", SearchOption.AllDirectories).Any())
                    throw new InvalidDataException("Legacy simulation left files behind.");
                Directory.Delete(editionRoot, recursive: true);
                File.WriteAllText(Path.Combine(runtimeRoot, VariantDirectoryService.OwnershipMarkerFileName), legacyMarkerJson);
            }

            Dictionary<string, string> SnapshotRoot() => Directory.EnumerateFiles(runtimeRoot, "*", SearchOption.AllDirectories)
                .ToDictionary(path => Path.GetRelativePath(runtimeRoot, path), HashFile, StringComparer.OrdinalIgnoreCase);

            void RequireUntouched(Dictionary<string, string> before, string stage)
            {
                Dictionary<string, string> after = SnapshotRoot();
                if (!before.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase).SequenceEqual(after.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)))
                    throw new InvalidDataException("Aborted adoption touched legacy output: " + stage);
            }

            SimulateLegacy();
            var recovery = new RecoverySafetyService(directories, skBuild);
            // Form-level: the Adopt action is offered with an explanation.
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Adopt fixture did not validate as an installation.");
            using (var form = new MainForm())
            {
                form.InitializeInstallationStateForTest();
                form.ActivateInstallationForTest(e1);
                form.SelectTranslationForTest("SK");
                form.OpenModsForTest();
                if (!form.AdoptLegacyEnabledForTest)
                    throw new InvalidDataException("Adopt legacy output was not offered for proven legacy.");
                if (!form.RecoveryStatusForTest.Contains("E1VGA", StringComparison.Ordinal))
                    throw new InvalidDataException("Recovery status does not explain the legacy output.");
            }
            // Negatives: each aborts, then the mutation is reverted and the
            // snapshot proves everything was left untouched.
            Dictionary<string, string> pristine = SnapshotRoot();
            File.WriteAllText(Path.Combine(runtimeRoot, "FOREIGN.DAT"), "external");
            if (recovery.AdoptLegacyFlatVariant(fixture, vga, sk.Code).Succeeded)
                throw new InvalidDataException("Adoption accepted unknown files.");
            File.Delete(Path.Combine(runtimeRoot, "FOREIGN.DAT"));
            RequireUntouched(pristine, "unknown file");
            string gamepcsk = Path.Combine(runtimeRoot, "GAMEPCSK");
            byte[] gamepcskBytes = File.ReadAllBytes(gamepcsk);
            File.WriteAllBytes(gamepcsk, gamepcskBytes.Select((cell, index) => index == gamepcskBytes.Length / 2 ? (byte)(cell ^ 0x01) : cell).ToArray());
            if (recovery.AdoptLegacyFlatVariant(fixture, vga, sk.Code).Succeeded)
                throw new InvalidDataException("Adoption accepted a hash mismatch.");
            File.WriteAllBytes(gamepcsk, gamepcskBytes);
            RequireUntouched(pristine, "hash mismatch");
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, vga, sk.Code), VariantDirectoryOperationStatus.Created, "blocking edition directory");
            if (recovery.AdoptLegacyFlatVariant(fixture, vga, sk.Code).Succeeded)
                throw new InvalidDataException("Adoption accepted an existing edition output.");
            if (directories.DeleteVariantEditionDirectory(fixture, vga, sk.Code).Status != VariantDirectoryOperationStatus.Deleted)
                throw new InvalidDataException("Blocking edition directory was not removed.");
            RequireUntouched(pristine, "existing edition");
            File.Delete(Path.Combine(runtimeRoot, VariantDirectoryService.OwnershipMarkerFileName));
            if (recovery.AdoptLegacyFlatVariant(fixture, vga, sk.Code).Succeeded)
                throw new InvalidDataException("Adoption accepted unmarked (foreign) contents.");
            File.WriteAllText(Path.Combine(runtimeRoot, VariantDirectoryService.OwnershipMarkerFileName), legacyMarkerJson);
            RequireUntouched(pristine, "foreign marker");
            string manifestBackup = Path.Combine(root, "manifest-backup.json");
            File.Copy(Path.Combine(runtimeRoot, VariantManifestService.FileName), manifestBackup, overwrite: true);
            File.Delete(Path.Combine(runtimeRoot, VariantManifestService.FileName));
            if (recovery.AdoptLegacyFlatVariant(fixture, vga, sk.Code).Succeeded)
                throw new InvalidDataException("Adoption accepted a missing manifest.");
            File.Copy(manifestBackup, Path.Combine(runtimeRoot, VariantManifestService.FileName), overwrite: true);
            RequireUntouched(pristine, "missing manifest");
            // Positive: read-only census agrees, then adoption restores full
            // ownership byte-identically; launcher and rebuild follow.
            RecoverySafetyOperationResult census = recovery.VerifyLegacyAdoption(fixture, vga, sk.Code);
            if (!census.Succeeded)
                throw new InvalidDataException("Adoption census refused proven legacy: " + census.Detail);
            RecoverySafetyOperationResult adopted = recovery.AdoptLegacyFlatVariant(fixture, vga, sk.Code);
            if (!adopted.Succeeded)
                throw new InvalidDataException("Proven legacy adoption failed: " + adopted.Detail);
            if (directories.ValidateOwnedVariantEditionDirectory(fixture, vga, sk.Code).Status != VariantDirectoryOperationStatus.AlreadyValid)
                throw new InvalidDataException("Adopted edition output did not validate.");
            foreach ((string relative, string hash) in editionPayload)
            {
                string adoptedPath = Path.Combine(editionRoot, relative);
                if (!File.Exists(adoptedPath) || !HashFile(adoptedPath).Equals(hash, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Adopted payload diverged: " + relative);
            }
            if (VariantManifestService.Read(editionRoot).ProjectCode != "SK" || File.Exists(Path.Combine(runtimeRoot, VariantDirectoryService.OwnershipMarkerFileName)))
                throw new InvalidDataException("Adoption left the legacy root marker or manifest behind.");
            // The runtime root persists BY DESIGN as the container of the
            // adopted edition: no loose files may remain beside it.
            string[] rootFiles = Directory.EnumerateFiles(runtimeRoot, "*", SearchOption.TopDirectoryOnly).ToArray();
            string[] rootDirs = Directory.EnumerateDirectories(runtimeRoot, "*", SearchOption.TopDirectoryOnly).ToArray();
            if (rootFiles.Length != 0 || rootDirs.Length != 1 || !rootDirs[0].Equals(editionRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Adoption left loose files beside the adopted edition.");
            var launcher = new VariantLauncherService(directories, skBuild);
            if (launcher.Resolve(fixture, vga).Readiness != VariantLaunchReadiness.LaunchReady)
                throw new InvalidDataException("Adopted output is not launch-ready: " + launcher.Resolve(fixture, vga).Detail);
            if (recovery.RebuildOwnedVariant(fixture, vga, sk.Code).BuildResult?.Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Rebuild after adoption failed.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyLegacyAdoptRealSimSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V6: fixture reproducing the REAL flat layout (schema-1 root
        // marker + SK/Full manifest + payload + game-content subdirs, NO
        // edition subdir) for VGA and EGA, plus every §7 negative and the
        // Default-variant ComboBox UX assertions. Real directories untouched.
        string root = Path.Combine(Path.GetTempPath(), "Pi1LegacyAdoptRealSimSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            text = translations.Add(text, skCreated);
            TranslationProjectVariant s1Created = translations.Create(fixture, text, "Slovencina test", "S1", new Dictionary<int, string> { [393] = "Prirucna taska." });
            text = translations.Add(text, s1Created);
            translations.Save(fixture, text);
            TranslationProjectVariant sk = translations.Load(fixture).State!.Variants.Single(item => item.Code == "SK");
            var graphics = new GraphicsVariantService();
            var directories = new VariantDirectoryService();
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            CompositeBuildService BuildFor(TranslationProjectVariant translation, RuntimeUiTextState ui) => new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(translation, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), ui)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, translation), translation.DataFile],
                projectVariantProvider: (_, _) => translation.Code);
            var emptyUi = RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1);
            foreach (VariantContext runtime in new[] { vga, ega })
            {
                RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, runtime, sk.Code), VariantDirectoryOperationStatus.Created, "realsim edition directory");
                if (BuildFor(sk, emptyUi).Build(fixture, runtime, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                    throw new InvalidDataException("Realsim SK build failed for " + runtime.VariantId + ".");
            }
            // Simulate the real flat layout per runtime, including a
            // game-content subdirectory listed in the manifest.
            foreach (VariantContext runtime in new[] { vga, ega })
            {
                string editionRoot = directories.GetVariantEditionDirectoryPath(fixture, runtime, sk.Code);
                string runtimeRoot = directories.GetVariantDirectoryPath(fixture, runtime);
                foreach (string file in Directory.EnumerateFiles(editionRoot, "*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(file).Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)) continue;
                    string destination = Path.Combine(runtimeRoot, Path.GetRelativePath(editionRoot, file));
                    Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                    File.Move(file, destination);
                }
                File.Delete(Path.Combine(editionRoot, VariantDirectoryService.OwnershipMarkerFileName));
                if (Directory.EnumerateFiles(editionRoot, "*", SearchOption.AllDirectories).Any())
                    throw new InvalidDataException("Legacy simulation left files behind for " + runtime.VariantId + ".");
                Directory.Delete(editionRoot, recursive: true);
                string extraDir = Path.Combine(runtimeRoot, "DOSBOX");
                Directory.CreateDirectory(extraDir);
                File.WriteAllBytes(Path.Combine(extraDir, "EXTRA.DAT"), [0x44, 0x4F, 0x53]);
                VariantManifest manifest = VariantManifestService.Read(runtimeRoot);
                var extended = manifest.OutputArtifacts.Append(new VariantManifestArtifact(
                    Path.Combine("DOSBOX", "EXTRA.DAT").Replace('\\', '/'),
                    new FileInfo(Path.Combine(extraDir, "EXTRA.DAT")).Length,
                    HashFile(Path.Combine(extraDir, "EXTRA.DAT")))).OrderBy(a => a.RelativePath, StringComparer.Ordinal).ToArray();
                var rewritten = manifest with { OutputArtifacts = extended };
                File.WriteAllText(Path.Combine(runtimeRoot, VariantManifestService.FileName),
                    System.Text.Json.JsonSerializer.Serialize(rewritten, new System.Text.Json.JsonSerializerOptions { WriteIndented = true }));
                File.WriteAllText(Path.Combine(runtimeRoot, VariantDirectoryService.OwnershipMarkerFileName),
                    "{\"SchemaVersion\":1,\"Product\":\"Pi1ElviraVariantDirectory\",\"GameId\":\"Elvira1\",\"VariantId\":\"" + runtime.VariantId + "\",\"DirectoryKey\":\"" + runtime.DirectoryKey + "\",\"BaselineFingerprint\":\"" + fixture.BaselineFingerprint + "\"}");
                if (manifest.ProjectCode != "SK" || manifest.BuildState != "Full")
                    throw new InvalidDataException("Realsim manifest identity diverged for " + runtime.VariantId + ".");
            }
            var recovery = new RecoverySafetyService(directories, BuildFor(sk, emptyUi));
            // Form-level: Adopt offered with explanation for the selected SK
            // edition on both runtimes; withheld when S1 is selected.
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Realsim fixture did not validate as an installation.");
            var modCatalog = new VariantCatalog(fixture.GameRoot, ElviraGameProfile.Elvira1, [
                VariantNaming.Create("English", "EN", ElviraGameProfile.Elvira1, true, 1),
                VariantNaming.Create("Slovenčina", "SK", ElviraGameProfile.Elvira1, true, 2),
                VariantNaming.Create("Slovenčina test", "S1", ElviraGameProfile.Elvira1, true, 3),
            ], configurationExists: true);
            VariantConfigurationService.Save(modCatalog);
            using (var form = new MainForm())
            {
                form.InitializeInstallationStateForTest();
                form.ActivateInstallationForTest(e1);
                form.SelectTranslationForTest("SK");
                form.OpenModsForTest();
                if (!form.AdoptLegacyEnabledForTest)
                    throw new InvalidDataException("Adopt was not offered for the real-shape VGA legacy.");
                if (!form.RecoveryStatusForTest.Contains("SK", StringComparison.Ordinal))
                    throw new InvalidDataException("Recovery status does not name the legacy edition.");
                form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
                form.OpenModsForTest();
                if (!form.AdoptLegacyEnabledForTest)
                    throw new InvalidDataException("Adopt was not offered for the real-shape EGA legacy.");
                form.SelectTranslationForTest("S1");
                form.OpenModsForTest();
                if (form.AdoptLegacyEnabledForTest)
                    throw new InvalidDataException("Adopt was offered although the manifest code differs from the selected edition.");
                form.SelectTranslationForTest("SK");
                form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
                form.OpenModsForTest();
                // Default-variant ComboBox: human labels, identity, layout.
                IReadOnlyList<string> labels = form.DefaultVariantLabelsForTest;
                if (labels.Count != 3 || labels.Any(label => label.Contains("VariantEntry {", StringComparison.Ordinal)))
                    throw new InvalidDataException("Default variant ComboBox exposes record debug text.");
                string[] expectedLabels = ["English (EN) — RUNVGA.EXE", "Slovenčina (SK) — RUNVGASK.EXE", "Slovenčina test (S1) — RUNVGAS1.EXE"];
                if (!labels.SequenceEqual(expectedLabels, StringComparer.Ordinal))
                    throw new InvalidDataException("Default variant labels diverged: " + string.Join(" | ", labels));
                if (!form.SelectDefaultVariantForTest("GAMEPCS1") || !form.DefaultVariantSelectedDataFileForTest.Equals("GAMEPCS1", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Default variant selection identity diverged.");
                form.SimulateDefaultVariantLayoutForTest(600);
                if (form.DefaultVariantComboWidthForTest < 175 || form.DefaultVariantDropDownWidthForTest < form.DefaultVariantComboWidthForTest)
                    throw new InvalidDataException("Default variant dropdown does not fit its control.");
                if (form.DefaultVariantFlowWidthForTest - form.DefaultVariantComboRightForTest > 12)
                    throw new InvalidDataException("Default variant ComboBox does not fill the available row width.");
            }
            // Service-level per runtime: detect, adopt, rebuild, resolve.
            foreach (VariantContext runtime in new[] { vga, ega })
            {
                string editionRoot = directories.GetVariantEditionDirectoryPath(fixture, runtime, sk.Code);
                string runtimeRoot = directories.GetVariantDirectoryPath(fixture, runtime);
                Dictionary<string, string> Snapshot() => Directory.EnumerateFiles(runtimeRoot, "*", SearchOption.AllDirectories)
                    .ToDictionary(path => Path.GetRelativePath(runtimeRoot, path), HashFile, StringComparer.OrdinalIgnoreCase);
                void RequireUntouched(Dictionary<string, string> before, string stage)
                {
                    Dictionary<string, string> after = Snapshot();
                    if (!before.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase).SequenceEqual(after.OrderBy(pair => pair.Key, StringComparer.OrdinalIgnoreCase)))
                        throw new InvalidDataException("Aborted adoption touched legacy output (" + runtime.VariantId + "/" + stage + ").");
                }
                // 1-3. Candidate detected; edition path conflicting; Adopt offered
                // state verified above at UI level — here the path resolution.
                var editionProbe = new VariantLauncherService(directories, BuildFor(sk, emptyUi));
                if (editionProbe.Resolve(fixture, runtime).Readiness != VariantLaunchReadiness.ForeignOrInvalidVariant)
                    throw new InvalidDataException("Legacy parent layout did not report foreign/invalid for " + runtime.VariantId + ".");
                Dictionary<string, string> pristine = Snapshot();
                // §7 negatives: each aborts with everything untouched.
                string markerPath = Path.Combine(runtimeRoot, VariantDirectoryService.OwnershipMarkerFileName);
                string markerJson = File.ReadAllText(markerPath);
                foreach ((string name, Func<string, string> mutate) in new (string, Func<string, string>)[]
                {
                    ("product", json => json.Replace("Pi1ElviraVariantDirectory", "SomethingElse", StringComparison.Ordinal)),
                    ("game", json => json.Replace("\"GameId\":\"Elvira1\"", "\"GameId\":\"Elvira2\"", StringComparison.Ordinal)),
                    ("variant", json => json.Replace("\"VariantId\":\"" + runtime.VariantId + "\"", "\"VariantId\":\"" + (runtime.VariantId == BuiltInVariantId.Elvira1Vga ? "Elvira1Ega" : "Elvira1Vga") + "\"", StringComparison.Ordinal)),
                    ("key", json => json.Replace("\"DirectoryKey\":\"" + runtime.DirectoryKey + "\"", "\"DirectoryKey\":\"E9ZZZ\"", StringComparison.Ordinal)),
                    ("baseline", json => json.Replace(fixture.BaselineFingerprint, new string('0', fixture.BaselineFingerprint.Length), StringComparison.Ordinal)),
                })
                {
                    File.WriteAllText(markerPath, mutate(markerJson));
                    if (recovery.AdoptLegacyFlatVariant(fixture, runtime, sk.Code).Succeeded)
                        throw new InvalidDataException("Adoption accepted a bad marker (" + name + ").");
                    File.WriteAllText(markerPath, markerJson);
                    RequireUntouched(pristine, "marker-" + name);
                }
                string manifestPath = Path.Combine(runtimeRoot, VariantManifestService.FileName);
                string manifestJson = File.ReadAllText(manifestPath);
                string otherRuntimeKind = runtime.RuntimeKind == VariantRuntimeKind.Elvira1Vga ? "Elvira1Ega" : "Elvira1Vga";
                File.WriteAllText(manifestPath, manifestJson.Replace("\"RuntimeKind\": \"" + runtime.RuntimeKind + "\"", "\"RuntimeKind\": \"" + otherRuntimeKind + "\"", StringComparison.Ordinal));
                if (recovery.AdoptLegacyFlatVariant(fixture, runtime, sk.Code).Succeeded)
                    throw new InvalidDataException("Adoption accepted a runtime-kind mismatch.");
                File.WriteAllText(manifestPath, manifestJson);
                RequireUntouched(pristine, "runtime-kind");
                if (recovery.AdoptLegacyFlatVariant(fixture, runtime, "S1").Succeeded)
                    throw new InvalidDataException("Adoption accepted a project code different from the selected edition.");
                RequireUntouched(pristine, "code-mismatch");
                // 4-10. Explicit adoption succeeds; Rebuild + launcher follow.
                if (!recovery.VerifyLegacyAdoption(fixture, runtime, sk.Code).Succeeded)
                    throw new InvalidDataException("Adoption census refused real-shape legacy for " + runtime.VariantId + ".");
                RecoverySafetyOperationResult adopted = recovery.AdoptLegacyFlatVariant(fixture, runtime, sk.Code);
                if (!adopted.Succeeded)
                    throw new InvalidDataException("Real-shape adoption failed for " + runtime.VariantId + ": " + adopted.Detail);
                if (directories.ValidateOwnedVariantEditionDirectory(fixture, runtime, sk.Code).Status != VariantDirectoryOperationStatus.AlreadyValid)
                    throw new InvalidDataException("Adopted edition did not validate for " + runtime.VariantId + ".");
                foreach ((string relative, string hash) in pristine.Where(pair =>
                    !pair.Key.Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase) &&
                    !pair.Key.Equals(VariantManifestService.FileName, StringComparison.OrdinalIgnoreCase)))
                {
                    string adoptedPath = Path.Combine(editionRoot, relative);
                    if (!File.Exists(adoptedPath) || !HashFile(adoptedPath).Equals(hash, StringComparison.OrdinalIgnoreCase))
                        throw new InvalidDataException("Adopted payload diverged (" + runtime.VariantId + "/" + relative + ").");
                }
                if (VariantManifestService.Read(editionRoot).ProjectCode != "SK")
                    throw new InvalidDataException("Adopted manifest identity diverged for " + runtime.VariantId + ".");
                var launcher = new VariantLauncherService(directories, BuildFor(sk, emptyUi));
                if (launcher.Resolve(fixture, runtime).Readiness != VariantLaunchReadiness.LaunchReady)
                    throw new InvalidDataException("Adopted output is not launch-ready for " + runtime.VariantId + ".");
                if (recovery.RebuildOwnedVariant(fixture, runtime, sk.Code).BuildResult?.Status != CompositeBuildStatus.Success)
                    throw new InvalidDataException("Rebuild after adoption failed for " + runtime.VariantId + ".");
            }
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyModsPostAdoptionConsistencySmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V7 focused post-adoption fixture: VARIANTS\E1VGA\SK owned with
        // marker + manifest + GAMEPCSK + RUNVGASK.EXE, runtime parent as
        // container. Proves one authoritative runtime+edition identity across
        // manager, top summary, edition table, launcher, Run/Debug, rebuild,
        // plus Default-variant geometry. TEMP fixtures only; real user data
        // is never touched.
        string root = Path.Combine(Path.GetTempPath(), "Pi1ModsPostAdoptSmoke", Guid.NewGuid().ToString("N"));
        string priorLocale = UiText.LocaleId;
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            text = translations.Add(text, skCreated);
            TranslationProjectVariant s1Created = translations.Create(fixture, text, "Slovencina test", "S1", new Dictionary<int, string> { [393] = "Prirucna taska." });
            text = translations.Add(text, s1Created);
            translations.Save(fixture, text);
            var modCatalog = new VariantCatalog(fixture.GameRoot, ElviraGameProfile.Elvira1, [
                VariantNaming.Create("English", "EN", ElviraGameProfile.Elvira1, true, 1),
                VariantNaming.Create("Slovak", "SK", ElviraGameProfile.Elvira1, true, 2),
                VariantNaming.Create("Slovencina test", "S1", ElviraGameProfile.Elvira1, true, 3),
            ], configurationExists: true);
            VariantConfigurationService.Save(modCatalog);
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Post-adoption fixture did not validate as an installation.");
            var directories = new VariantDirectoryService();
            var graphics = new GraphicsVariantService();
            TranslationProjectVariant sk = translations.Load(fixture).State!.Variants.Single(item => item.Code == "SK");
            TranslationProjectVariant s1 = translations.Load(fixture).State!.Variants.Single(item => item.Code == "S1");
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, vga, sk.Code), VariantDirectoryOperationStatus.Created, "post-adoption fixture directory");
            CompositeBuildService skBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skBuild.Build(fixture, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Post-adoption SK fixture build failed.");
            string editionRoot = directories.GetVariantEditionDirectoryPath(fixture, vga, sk.Code);
            string runtimeRoot = directories.GetVariantDirectoryPath(fixture, vga);
            string expectedSk = Path.Combine(editionRoot, "GAMEPCSK");
            string expectedExe = Path.Combine(editionRoot, ActiveProjectBuildIdentity.ExecutableName(vga, sk));
            if (!File.Exists(expectedSk) || !File.Exists(expectedExe))
                throw new InvalidDataException("Post-adoption fixture did not materialize both artifacts in E1VGA\\SK.");
            if (File.Exists(Path.Combine(runtimeRoot, "GAMEPCSK")))
                throw new InvalidDataException("Runtime parent was treated as edition output.");
            // 1. Edition directory is editor-owned.
            if (directories.ValidateOwnedVariantEditionDirectory(fixture, vga, sk.Code).Status != VariantDirectoryOperationStatus.AlreadyValid)
                throw new InvalidDataException("Adopted edition directory is not editor-owned.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            form.SelectTranslationForTest("SK");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            form.OpenModsForTest();
            // 2. Variant Manager Ready.
            if (!form.RuntimeVariantBuildStatusForTest(BuiltInVariantId.Elvira1Vga).Equals(VariantBuildStatus.Ready.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Variant Manager did not report Ready for adopted E1VGA\\SK.");
            // 3-4. Top summary + launch readiness Ready, never foreign/incomplete.
            if (!form.VariantLaunchOwnershipForTest().Equals(VariantDirectoryOperationStatus.AlreadyValid.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Top Mods ownership diverged: " + form.ModsPresentationForTest);
            if (!form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Top Mods launch readiness diverged: " + form.ModsPresentationForTest);
            if (form.ModsPresentationForTest.Contains("ForeignDirectoryConflict", StringComparison.Ordinal) ||
                form.ModsPresentationForTest.Contains("ForeignOrInvalidVariant", StringComparison.Ordinal))
                throw new InvalidDataException("Top summary still reports the stale foreign state.");
            // 5-7. SK Available from the owned location; both artifacts resolve there.
            if (!form.IsVariantEntryAvailableForTest("GAMEPCSK"))
                throw new InvalidDataException("SK bottom row did not report Available.");
            if (!form.VariantEntryResolvedPathForTest("GAMEPCSK").Equals(expectedSk, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("GAMEPCSK did not resolve from E1VGA\\SK.");
            if (!form.VariantLaunchDataPathForTest().Equals(expectedSk, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Launcher data target diverged from availability path.");
            if (!form.VariantLaunchExecutablePathForTest().Equals(expectedExe, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("RUNVGASK.EXE did not resolve from E1VGA\\SK.");
            form.RefreshVariantGridForTest();
            if (!form.VariantGridRowStatusForTest("GAMEPCSK").Equals(UiText.Get("VariantAvailable"), StringComparison.Ordinal))
                throw new InvalidDataException("Mods grid does not show adopted SK Available.");
            // 8. Parent is container, never selected output.
            if (form.VariantLaunchDataPathForTest().StartsWith(runtimeRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
                !form.VariantLaunchDataPathForTest().StartsWith(editionRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Launcher treated the runtime parent as edition output.");
            // 9-10. Rebuild enabled; Run consistent with readiness.
            if (!form.RebuildEnabledForTest)
                throw new InvalidDataException("Rebuild is not enabled for the adopted edition.");
            if (!form.RunEnabledForTest)
                throw new InvalidDataException("Run is not enabled for the launch-ready target.");
            // 11. SK -> EN -> SK keeps the same correct state.
            form.SelectTranslationForTest("EN");
            form.OpenModsForTest();
            string enReadiness = form.VariantLaunchReadinessForTest();
            if (enReadiness.Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Unbuilt EN edition falsely reports LaunchReady.");
            form.SelectTranslationForTest("SK");
            form.OpenModsForTest();
            if (!form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal) ||
                !form.IsVariantEntryAvailableForTest("GAMEPCSK") ||
                !form.RuntimeVariantBuildStatusForTest(BuiltInVariantId.Elvira1Vga).Equals(VariantBuildStatus.Ready.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("SK -> EN -> SK did not preserve the adopted Ready state.");
            // 12. Reopening Mods keeps the same state.
            form.OpenModsForTest();
            if (!form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Reopening Mods lost the adopted Ready state.");
            // Equivalent EGA fixture behaves consistently.
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, ega, sk.Code), VariantDirectoryOperationStatus.Created, "EGA post-adoption directory");
            CompositeBuildService egaBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (egaBuild.Build(fixture, ega, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("EGA/SK fixture build failed.");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            form.OpenModsForTest();
            if (!form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("EGA/SK did not resolve Ready consistently.");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            form.OpenModsForTest();
            if (!form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("VGA/SK lost Ready after the EGA build.");
            // Never-built S1: genuinely Missing, never Foreign.
            form.SelectTranslationForTest("S1");
            form.OpenModsForTest();
            string s1Root = directories.GetVariantEditionDirectoryPath(fixture, vga, s1.Code);
            string expectedS1 = Path.Combine(s1Root, "GAMEPCS1");
            if (!form.VariantEntryResolvedPathForTest("GAMEPCS1").Equals(expectedS1, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("S1 did not resolve to its owned location.");
            if (form.IsVariantEntryAvailableForTest("GAMEPCS1"))
                throw new InvalidDataException("Never-built S1 falsely reports Available.");
            if (form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.ForeignOrInvalidVariant.ToString(), StringComparison.Ordinal))
                throw new InvalidDataException("Never-built S1 falsely reports Foreign.");
            form.SelectTranslationForTest("SK");
            form.OpenModsForTest();
            // Foreign negative: unowned S1 directory never looks Available.
            string foreignS1 = directories.GetVariantEditionDirectoryPath(fixture, vga, s1.Code);
            Directory.CreateDirectory(foreignS1);
            File.WriteAllBytes(Path.Combine(foreignS1, "GAMEPCS1"), new byte[] { 0x46 });
            try
            {
                if (directories.ValidateOwnedVariantEditionDirectory(fixture, vga, s1.Code).Status != VariantDirectoryOperationStatus.ForeignDirectoryConflict)
                    throw new InvalidDataException("Foreign S1 directory was not classified Foreign.");
                form.SelectTranslationForTest("S1");
                form.OpenModsForTest();
                if (form.IsVariantEntryAvailableForTest("GAMEPCS1"))
                    throw new InvalidDataException("Foreign S1 falsely reports Available.");
                if (!form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.ForeignOrInvalidVariant.ToString(), StringComparison.Ordinal))
                    throw new InvalidDataException("Foreign S1 did not report ForeignOrInvalidVariant.");
                form.SelectTranslationForTest("SK");
                form.OpenModsForTest();
            }
            finally { if (Directory.Exists(foreignS1)) Directory.Delete(foreignS1, true); }
            // Corrupt negative: owned marker but tampered payload fails closed.
            string skHash = HashFile(expectedSk);
            File.WriteAllBytes(expectedSk, new byte[] { 0x00, 0x01, 0x02 });
            try
            {
                form.OpenModsForTest();
                if (form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal))
                    throw new InvalidDataException("Corrupt SK falsely reports LaunchReady.");
                if (form.IsVariantEntryAvailableForTest("GAMEPCSK"))
                    throw new InvalidDataException("Corrupt SK falsely reports Available.");
            }
            finally
            {
                var rebuild = new RecoverySafetyService(directories, skBuild);
                if (rebuild.RebuildOwnedVariant(fixture, vga, sk.Code).BuildResult?.Status != CompositeBuildStatus.Success)
                    throw new InvalidDataException("Rebuild after corruption failed.");
                if (!HashFile(expectedSk).Equals(skHash, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Rebuilt SK payload diverged deterministically.");
                form.OpenModsForTest();
                if (!form.VariantLaunchReadinessForTest().Equals(VariantLaunchReadiness.LaunchReady.ToString(), StringComparison.Ordinal))
                    throw new InvalidDataException("Rebuilt SK did not return to LaunchReady.");
                if (!form.IsVariantEntryAvailableForTest("GAMEPCSK"))
                    throw new InvalidDataException("Rebuilt SK did not return to Available.");
            }
            form.SelectTranslationForTest("SK");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            form.OpenModsForTest();
            // 16. Default-variant identity: friendly labels, object identity, no string parsing.
            IReadOnlyList<string> labels = form.DefaultVariantLabelsForTest;
            if (labels.Any(label => label.Contains("VariantEntry {", StringComparison.Ordinal)))
                throw new InvalidDataException("Default variant ComboBox exposes record debug text.");
            if (!form.SelectDefaultVariantForTest("GAMEPCSK") || !form.DefaultVariantSelectedDataFileForTest.Equals("GAMEPCSK", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Default variant SK selection identity diverged.");
            if (!form.SelectDefaultVariantForTest("GAMEPCS1") || !form.DefaultVariantSelectedDataFileForTest.Equals("GAMEPCS1", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Default variant S1 selection identity diverged.");
            // 11-13. Default-variant geometry: visible control never exceeds
            // the shared header width (620); dropdown alone may grow wider.
            // Narrow rows still fill; wide rows cap instead of stretching.
            form.SimulateDefaultVariantLayoutForTest(600);
            if (form.DefaultVariantComboWidthForTest < 175 || form.DefaultVariantComboWidthForTest > form.SharedSelectorVisibleWidthForTest)
                throw new InvalidDataException("Default variant control width out of range (narrow): " + form.DefaultVariantComboWidthForTest);
            if (form.DefaultVariantDropDownWidthForTest < form.DefaultVariantComboWidthForTest)
                throw new InvalidDataException("Default variant dropdown does not fit its control (narrow).");
            if (form.DefaultVariantFlowWidthForTest - form.DefaultVariantComboRightForTest > 24)
                throw new InvalidDataException("Default variant ComboBox does not fill its narrow row.");
            form.SimulateDefaultVariantLayoutForTest(1400);
            if (form.DefaultVariantComboWidthForTest > form.SharedSelectorVisibleWidthForTest)
                throw new InvalidDataException("Default variant control stretched beyond the shared header width: " + form.DefaultVariantComboWidthForTest);
            if (form.DefaultVariantDropDownWidthForTest < form.DefaultVariantComboWidthForTest)
                throw new InvalidDataException("Default variant dropdown is narrower than its control (wide).");
            if (Math.Abs(form.HeaderInstallationComboWidthForTest - form.SharedSelectorVisibleWidthForTest) > 24)
                throw new InvalidDataException("Header ComboBox width diverged from the shared selector width.");
            // When the row is wide enough to cap, the visible widths match;
            // when headless/narrow, the control only fills what exists and
            // never exceeds the header width (proven above).
            if (form.DefaultVariantComboWidthForTest == form.SharedSelectorVisibleWidthForTest &&
                Math.Abs(form.DefaultVariantComboWidthForTest - form.HeaderInstallationComboWidthForTest) > 24)
                throw new InvalidDataException("Default variant right edge does not align with the header ComboBoxes.");
            if (File.Exists(Path.Combine(fixture.GameRoot, "GAMEPCSK")) || File.Exists(Path.Combine(fixture.GameRoot, "GAMEPCS1")))
                throw new InvalidDataException("Translated data reached pristine GameRoot.");
            _ = elvira2Source;
        }
        finally
        {
            UiText.SetLocale(priorLocale);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyRuntimeUiOriginalSmoke(string elvira1Source, string elvira2Source)
    {

        // R9F V4 §1: the Original edition stays immutable, but an explicit
        // edit attempt now explains itself (localized) instead of silently
        // doing nothing. Editable editions still open the editor normally.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiOriginalSmoke", Guid.NewGuid().ToString("N"));
        string priorLocale = UiText.LocaleId;
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            translations.Save(fixture, translations.Add(text, skCreated));
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Original feedback fixture did not validate as an installation.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            if (!form.ActiveProjectCodeForTest.Equals("EN", StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Fixture did not start on the Original edition.");
            // Double-click is blocked with an explicit reason on Original.
            if (form.OpenSemanticEditorBlockedReasonForTest(0) != "Original")
                throw new InvalidDataException("Original edit attempt was not reported as edition-blocked.");
            // The message is localized in all three locales.
            var expectedMessages = new Dictionary<string, string>
            {
                ["en"] = "The Original edition is read-only. Select or create an editable edition first.",
                ["sk"] = "Edícia Original je iba na čítanie. Najskôr vyberte alebo vytvorte upraviteľnú edíciu.",
                ["cs"] = "Edice Original je pouze pro čtení. Nejprve vyberte nebo vytvořte upravitelnou edici."
            };
            foreach ((string locale, string expected) in expectedMessages)
            {
                UiText.SetLocale(locale);
                if (!UiText.Get("RuntimeUi.Detail.OriginalEditionReadOnly").Equals(expected, StringComparison.Ordinal))
                    throw new InvalidDataException("Original read-only message diverged in locale " + locale + ".");
            }
            UiText.SetLocale(priorLocale);
            // In-cell edits on Original explain via the status notice and
            // store nothing.
            form.ApplyRuntimeUiGridEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "X");
            if (!form.RuntimeUiStatusLabelForTest.Contains("Original", StringComparison.Ordinal) ||
                form.RuntimeUiOverrideCountForTest != 0 || form.RuntimeUiDirtyForTest || form.RuntimeUiSaveEnabledForTest)
                throw new InvalidDataException("Original in-cell edit stored state or gave no feedback.");
            // Unsupported rows stay read-only-gated even on editable editions.
            form.SelectTranslationForTest("SK");
            if (form.OpenSemanticEditorBlockedReasonForTest(0) is not null)
                throw new InvalidDataException("Editable edition did not open the Pause.menu editor.");
            if (form.BuildSemanticEditorForTest(RuntimeUiLogicalRecordId.PauseMenu) is null)
                throw new InvalidDataException("Editable edition Pause.menu editor did not build.");
            if (form.OpenSemanticEditorBlockedReasonForTest(1) is not null)
                throw new InvalidDataException("Editable edition did not open the proven-live Confirm.generic editor.");
            if (form.BuildSemanticEditorForTest(RuntimeUiLogicalRecordId.ConfirmGeneric) is null)
                throw new InvalidDataException("Editable edition Confirm.generic editor did not build.");
            if (form.OpenSemanticEditorBlockedReasonForTest(2) != "ReadOnly")
                throw new InvalidDataException("Still-unsupported VGA row lost its read-only gate.");
            _ = elvira2Source;
        }
        finally
        {
            UiText.SetLocale(priorLocale);
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }
    }

    private static void VerifyRunEgaSelectorAuditSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V3 §3: audit of the already-generated RUNEGA Runtime UI
        // mechanism. Pins every frozen fact the variable-length decision
        // rests on: bank geometry, selector operands, bridge bytes,
        // relocation entry, record contiguity, and the current in-place
        // limits. Relocation is NOT proven safe (see assertions below).
        ProjectContext probe;
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunEgaSelectorAudit", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            probe = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            FrozenRuntimeUiDescriptor descriptor = Elvira1ProductionProfile.RunEga.RuntimeUi;
            if (!descriptor.IsFrozen || !descriptor.IsPersistentBank(0x31000, 0x317FF, 0x33400, 0x33BFF, 0x3100))
                throw new InvalidDataException("RUNEGA Runtime UI bank identity diverged.");
            if (descriptor.ModuleEndInclusive - descriptor.ModuleStart + 1 != 0x800)
                throw new InvalidDataException("RUNEGA Runtime UI bank is not 0x800 bytes.");
            if (descriptor.BridgeOffset != 0x00D4 || descriptor.BridgeRelocationOffset != 0x00DB)
                throw new InvalidDataException("RUNEGA bridge offsets diverged.");
            int[] selectors = descriptor.RuntimeRecords.Select(record => record.Selector).Order().ToArray();
            if (!selectors.SequenceEqual(Enumerable.Range(0xF000, 8)))
                throw new InvalidDataException("RUNEGA selectors are not unique/contiguous F000..F007.");
            // Record spans are contiguous and non-uniform: resolution cannot
            // be algorithmic (no stride), and no selector/offset table exists
            // anywhere in the 9-byte bridge.
            int[] bankOffsets = descriptor.RuntimeRecords.Select(record => record.BankOffset).ToArray();
            int[] lengths = descriptor.RuntimeRecords.Select(record => record.Length).ToArray();
            if (!bankOffsets.SequenceEqual(new[] { 0x00DD, 0x0107, 0x012E, 0x0161, 0x0173, 0x0185, 0x0198, 0x01AD }) ||
                !lengths.SequenceEqual(new[] { 42, 39, 51, 18, 18, 19, 21, 62 }))
                throw new InvalidDataException("RUNEGA bank map diverged from the frozen descriptor.");
            if (lengths.Distinct().Count() == 1)
                throw new InvalidDataException("RUNEGA record uniformity claim is false; audit premise broken.");
            for (int i = 0; i < bankOffsets.Length - 1; i++)
                if (bankOffsets[i] + lengths[i] != bankOffsets[i + 1])
                    throw new InvalidDataException("RUNEGA record spans are not contiguous.");
            // Pointer sites: six MOV DI (0xBF), two MOV BP (0xBD), with the
            // frozen original data offsets in the canonical image.
            (int Site, byte Op, int Original)[] sites =
            [
                (0xCDD8, 0xBF, 0x2D21), (0xCE0A, 0xBF, 0x2D48), (0xCBFC, 0xBF, 0x2D70), (0xCD03, 0xBF, 0x2DA4),
                (0xCD1A, 0xBF, 0x2DB6), (0xCD1D, 0xBD, 0x2DC8), (0xCD06, 0xBD, 0x2DDC), (0xCCDD, 0xBF, 0x2DF2)
            ];
            byte[] packed = File.ReadAllBytes(Path.Combine(probe.GameRoot, Elvira1ProductionProfile.ActiveEgaExecutable));
            byte[] canonical = RunEgaBootstrapService.CanonicalizePacked(packed);
            RunEgaBootstrapService.ValidateCanonical(canonical);
            static ushort U16(byte[] image, int offset) => (ushort)(image[offset] | (image[offset + 1] << 8));
            foreach ((int site, byte op, int original) in sites)
            {
                if (canonical[site] != op || U16(canonical, site + 1) != original)
                    throw new InvalidDataException($"RUNEGA pointer site 0x{site:X} diverged from the frozen route.");
            }
            // Generated output: bridge bytes, exactly one new relocation word
            // for the bridge segment operand, patched selectors, records at
            // their frozen offsets, and zeros elsewhere in the bank.
            byte[] output = RunEgaBootstrapService.Build(canonical, GlyphRepository.CreateAllCp852Slots());
            RunEgaBootstrapService.ValidateOutput(output);
            const int Ui = 0x33400;
            byte[] bridge = output.AsSpan(Ui + 0x00D4, 9).ToArray();
            if (!bridge.SequenceEqual(Convert.FromHexString("BBBE0253EABF02290D")))
                throw new InvalidDataException("RUNEGA bridge bytes diverged.");
            int headerParagraphs = U16(output, 8) * 16, relocCount = U16(output, 6), relocTable = U16(output, 0x18);
            bool bridgeRelocated = false;
            for (int i = 0; i < relocCount; i++)
            {
                int entry = relocTable + i * 4;
                if (headerParagraphs + U16(output, entry + 2) * 16 + U16(output, entry) == Ui + 0x00DB) bridgeRelocated = true;
            }
            if (!bridgeRelocated)
                throw new InvalidDataException("RUNEGA bridge relocation entry is missing.");
            ushort[] patched = [0xF000, 0xF001, 0xF002, 0xF003, 0xF004, 0xF005, 0xF006, 0xF007];
            for (int i = 0; i < sites.Length; i++)
            {
                if (output[sites[i].Site] != sites[i].Op || U16(output, sites[i].Site + 1) != patched[i])
                    throw new InvalidDataException($"RUNEGA site 0x{sites[i].Site:X} was not patched to selector 0x{patched[i]:X4}.");
            }
            for (int i = Ui; i < Ui + 0x00D4; i++)
                if (output[i] != 0x00)
                    throw new InvalidDataException($"RUNEGA bank pre-bridge byte 0x{i:X} is not zero in the generated baseline.");
            for (int i = Ui + 0x01EB; i < Ui + 0x0800; i++)
                if (output[i] != 0x00)
                    throw new InvalidDataException($"RUNEGA bank tail byte 0x{i:X} is not zero in the generated baseline.");
            // The zeros above are a GENERATOR fact, not a free-space proof:
            // no debugger/memory evidence shows the runtime never uses them
            // as scratch, the 9-byte bridge holds no selector/offset table,
            // and non-uniform spans rule out stride computation. Arbitrary
            // record relocation inside the bank is therefore NOT proven safe,
            // and the fixed English-envelope limits stay authoritative. A
            // 20-bytes-including-NUL translation into the 19-byte
            // FileNotFound envelope must still fail closed here.
            if (RunEgaUiService.TryComposeField(RuntimeUiLogicalRecordId.FileNotFound, "1234567890123456", out _, out _, out string detail))
                throw new InvalidDataException("Over-envelope RUNEGA field was accepted despite unproven relocation.");
            if (!detail.Contains("20", StringComparison.Ordinal) || !detail.Contains("19", StringComparison.Ordinal))
                throw new InvalidDataException("Over-envelope rejection did not report binary vs envelope capacity: " + detail);
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRunEgaSemanticSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V3 §§7-9,18: semantic contracts per structured record. Only the
        // proven message field is editable; buttons/questions/geometry stay
        // visibly read-only with reasons. Compose/decompose round-trips are
        // exact; Slovak boundary behavior is pinned.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RunEgaSemanticSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var originals = new Dictionary<RuntimeUiLogicalRecordId, string>
            {
                [RuntimeUiLogicalRecordId.PauseMenu] = "     Game Paused\r\r\r Continue      Quit",
                [RuntimeUiLogicalRecordId.ConfirmGeneric] = "    Are you sure ?\r\r\r     Yes       No",
                [RuntimeUiLogicalRecordId.SavePrompt] = "\r Insert savegame data disk & enter filename:\r\r   ",
                [RuntimeUiLogicalRecordId.SaveFailure] = "\r    Save failed.",
                [RuntimeUiLogicalRecordId.LoadFailure] = "\r    Load failed.",
                [RuntimeUiLogicalRecordId.FileNotFound] = "\r  File not found.",
                [RuntimeUiLogicalRecordId.TryAnotherDisk] = "\r  Try another disk.",
                [RuntimeUiLogicalRecordId.SaveOverwrite] = "\r File already exists.\r\r    Overwrite it ?\r\r     Yes       No",
            };
            var expectedKeys = new Dictionary<RuntimeUiLogicalRecordId, string[]>
            {
                [RuntimeUiLogicalRecordId.PauseMenu] = ["title", "continue", "quit"],
                [RuntimeUiLogicalRecordId.ConfirmGeneric] = ["prompt", "yes", "no"],
                [RuntimeUiLogicalRecordId.SavePrompt] = ["message", "input"],
                [RuntimeUiLogicalRecordId.SaveFailure] = ["message"],
                [RuntimeUiLogicalRecordId.LoadFailure] = ["message"],
                [RuntimeUiLogicalRecordId.FileNotFound] = ["message"],
                [RuntimeUiLogicalRecordId.TryAnotherDisk] = ["message"],
                [RuntimeUiLogicalRecordId.SaveOverwrite] = ["message", "question", "yes", "no"],
            };
            var expectedFields = new Dictionary<RuntimeUiLogicalRecordId, string>
            {
                [RuntimeUiLogicalRecordId.PauseMenu] = "Game Paused",
                [RuntimeUiLogicalRecordId.ConfirmGeneric] = "Are you sure ?",
                [RuntimeUiLogicalRecordId.SavePrompt] = "Insert savegame data disk & enter filename:",
                [RuntimeUiLogicalRecordId.SaveFailure] = "Save failed.",
                [RuntimeUiLogicalRecordId.LoadFailure] = "Load failed.",
                [RuntimeUiLogicalRecordId.FileNotFound] = "File not found.",
                [RuntimeUiLogicalRecordId.TryAnotherDisk] = "Try another disk.",
                [RuntimeUiLogicalRecordId.SaveOverwrite] = "File already exists.",
            };
            foreach ((RuntimeUiLogicalRecordId id, string full) in originals)
            {
                if (!RunEgaUiService.TryDecodeOriginalFromGameRoot(e1.GameRoot, id, out string? decoded, out _) || !decoded!.Equals(full, StringComparison.Ordinal))
                    throw new InvalidDataException("Authoritative RUNEGA original diverged: " + id);
                IReadOnlyList<RunEgaUiSemanticField> rows = RunEgaUiService.Decompose(full, id);
                if (!rows.Select(row => row.Key).SequenceEqual(expectedKeys[id]))
                    throw new InvalidDataException("Semantic subfield matrix diverged: " + id);
                RunEgaUiSemanticField editable = rows.Single(row => row.Role == RunEgaUiSemanticRole.EditableSafe);
                if (!editable.Value.Equals(expectedFields[id], StringComparison.Ordinal))
                    throw new InvalidDataException("Semantic editable field diverged: " + id);
                // V4 hotspot-label contract: button labels are editable
                // fixed-width slots (width hint carried as the reason);
                // questions/input geometry stay read-only with reasons.
                foreach (RunEgaUiSemanticField button in rows.Where(row => row.Role == RunEgaUiSemanticRole.EditableFixedWidth))
                {
                    RunEgaUiService.RunEgaUiButtonSlot slot = RunEgaUiService.RunEgaUiButtons.Slots(id).Single(item => item.Key.Equals(button.Key, StringComparison.Ordinal));
                    if (string.IsNullOrWhiteSpace(button.ReadOnlyReason) || !button.ReadOnlyReason.Contains(slot.Width.ToString(), StringComparison.Ordinal))
                        throw new InvalidDataException("Fixed-width button row lacks its width hint: " + id + "/" + button.Key);
                }
                string[] buttonKeys = rows.Where(row => row.Role == RunEgaUiSemanticRole.EditableFixedWidth).Select(row => row.Key).Order().ToArray();
                string[] expectedButtons = id switch
                {
                    RuntimeUiLogicalRecordId.PauseMenu => ["continue", "quit"],
                    RuntimeUiLogicalRecordId.ConfirmGeneric => ["no", "yes"],
                    RuntimeUiLogicalRecordId.SaveOverwrite => ["no", "yes"],
                    _ => []
                };
                if (!buttonKeys.SequenceEqual(expectedButtons))
                    throw new InvalidDataException("Semantic button matrix diverged: " + id);
                foreach (RunEgaUiSemanticField frozen in rows.Where(row => row.Role != RunEgaUiSemanticRole.EditableSafe && row.Role != RunEgaUiSemanticRole.EditableFixedWidth))
                {
                    if (string.IsNullOrWhiteSpace(frozen.ReadOnlyReason))
                        throw new InvalidDataException("Read-only subfield has no reason: " + id + "/" + frozen.Key);
                    if (frozen.Role != RunEgaUiSemanticRole.ReadOnlyGeometryUnproven && frozen.Role != RunEgaUiSemanticRole.ReadOnlyOther)
                        throw new InvalidDataException("Non-button frozen subfield has the wrong role: " + id + "/" + frozen.Key);
                }
                // Compose/decompose round-trip is exact for the original field.
                if (!RuntimeUiSemanticEditorForm.TryComposeSemanticField(VariantRuntimeKind.Elvira1Ega, id, editable.Value, out string recomposed, out _) ||
                    !recomposed.Equals(full, StringComparison.Ordinal))
                    throw new InvalidDataException("Semantic compose round-trip diverged: " + id);
            }
            // Slovak boundary pins (fixed English envelopes stay authoritative
            // while bank relocation is unproven): exact-boundary success,
            // boundary+1 failure, and the failure-A shape (fits the 19-byte
            // span, rejected by the 15-byte field/line rule).
            if (!RunEgaUiService.TryComposeField(RuntimeUiLogicalRecordId.SaveFailure, "Neuložené!XY", out _, out _, out _))
                throw new InvalidDataException("Exact-boundary Slovak Save.failed field was rejected.");
            if (RunEgaUiService.TryComposeField(RuntimeUiLogicalRecordId.SaveFailure, "Neuložené!XYZ", out _, out _, out _))
                throw new InvalidDataException("Boundary+1 Slovak Save.failed field was accepted.");
            if (RunEgaUiService.TryComposeField(RuntimeUiLogicalRecordId.FileNotFound, "Súbor sa nenašiel.", out _, out _, out string fieldDetail))
                throw new InvalidDataException("Failure-A probe composition unexpectedly changed; audit premise broken.");
            if (!fieldDetail.Contains("22", StringComparison.Ordinal) || !fieldDetail.Contains("19", StringComparison.Ordinal))
                throw new InvalidDataException("Failure-A rejection did not report composed vs envelope capacity: " + fieldDetail);
            // The modal semantic editor constructs headlessly over the same
            // contracts: valid field submits a composed record, overlong
            // field is refused with a validation message and no composition.
            IReadOnlyList<RunEgaUiSemanticField> pauseRows = RunEgaUiService.Decompose(originals[RuntimeUiLogicalRecordId.PauseMenu], RuntimeUiLogicalRecordId.PauseMenu);
            using (var editor = new RuntimeUiSemanticEditorForm("Pause.menu", RuntimeUiLogicalRecordId.PauseMenu, VariantRuntimeKind.Elvira1Ega, pauseRows, pauseRows))
            {
                editor.EditTextForTest = "Pozastavené";
                if (!editor.SubmitForTest() || editor.ComposedFullRecord is null ||
                    !editor.ComposedFullRecord.Equals("     Pozastavené\r\r\r Continue      Quit", StringComparison.Ordinal))
                    throw new InvalidDataException("Semantic editor did not compose the edited field.");
            }
            using (var rejected = new RuntimeUiSemanticEditorForm("Pause.menu", RuntimeUiLogicalRecordId.PauseMenu, VariantRuntimeKind.Elvira1Ega, pauseRows, pauseRows))
            {
                rejected.EditTextForTest = "This title is far too long for the envelope";
                if (rejected.SubmitForTest() || rejected.ComposedFullRecord is not null || string.IsNullOrWhiteSpace(rejected.ValidationForTest))
                    throw new InvalidDataException("Semantic editor accepted an overlong field.");
            }
            // V4 hotspot-label contract: TEXT MAY CHANGE, HOTSPOT GEOMETRY
            // MUST NOT. Exact-width labels compose; shorter labels pad with
            // spaces so following columns never move; overlong labels are
            // rejected, never truncated. Payload length is invariant.
            System.Text.Encoding cp852 = GamePcTextEditor.GetEncoding("CP852");
            string pauseOriginal = originals[RuntimeUiLogicalRecordId.PauseMenu];
            byte[] pauseOriginalPayload = cp852.GetBytes(pauseOriginal);
            if (!RunEgaUiService.TryComposeRecord(RuntimeUiLogicalRecordId.PauseMenu, "Game Paused",
                new Dictionary<string, string>(StringComparer.Ordinal) { ["continue"] = "Pokračuj", ["quit"] = "Quit" },
                out string pauseTranslated, out _, out _))
                throw new InvalidDataException("Exact-width hotspot labels were rejected.");
            if (!pauseTranslated.Equals("     Game Paused\r\r\r Pokračuj      Quit", StringComparison.Ordinal))
                throw new InvalidDataException("Hotspot label composition broke frozen formatting.");
            byte[] pauseTranslatedPayload = cp852.GetBytes(pauseTranslated);
            if (pauseTranslatedPayload.Length != pauseOriginalPayload.Length)
                throw new InvalidDataException("Translated hotspot labels changed the record length.");
            if (!RunEgaUiService.TryComposeRecord(RuntimeUiLogicalRecordId.PauseMenu, "Game Paused",
                new Dictionary<string, string>(StringComparer.Ordinal) { ["continue"] = "Hraj", ["quit"] = "Quit" },
                out string pausePadded, out _, out _))
                throw new InvalidDataException("Shorter hotspot label was rejected.");
            if (!pausePadded.Contains("Hraj    ", StringComparison.Ordinal))
                throw new InvalidDataException("Shorter hotspot label was not space-padded.");
            int quitOriginal = pauseOriginal.IndexOf("Quit", StringComparison.Ordinal);
            int quitTranslated = pausePadded.IndexOf("Quit", StringComparison.Ordinal);
            if (quitOriginal != quitTranslated || quitOriginal != 5 + 11 + 3 + 1 + 8 + 6)
                throw new InvalidDataException("Following hotspot field moved after padding.");
            for (int i = 0; i < pauseOriginalPayload.Length; i++)
            {
                bool inContinueSlot = i >= 20 && i < 28;
                if (!inContinueSlot && pauseOriginalPayload[i] != pauseTranslatedPayload[i])
                    throw new InvalidDataException($"Neighboring byte moved at 0x{i:X}.");
            }
            if (!RunEgaUiService.TryComposeRecord(RuntimeUiLogicalRecordId.ConfirmGeneric, "Are you sure ?",
                new Dictionary<string, string>(StringComparer.Ordinal) { ["yes"] = "Áno", ["no"] = "Ne" },
                out string confirmTranslated, out _, out _))
                throw new InvalidDataException("Exact-width Yes/No labels were rejected.");
            if (!confirmTranslated.Equals("    Are you sure ?\r\r\r     Áno       Ne", StringComparison.Ordinal))
                throw new InvalidDataException("Yes/No composition broke frozen columns.");
            // Overlong labels are rejected with the slot width, never cut.
            if (RunEgaUiService.TryComposeRecord(RuntimeUiLogicalRecordId.PauseMenu, "Game Paused",
                new Dictionary<string, string>(StringComparer.Ordinal) { ["quit"] = "Koniec" },
                out string quitCut, out RunEgaUiFailureKind quitFailure, out string quitDetail) || !string.IsNullOrEmpty(quitCut))
                throw new InvalidDataException("Overlong Quit label was accepted or truncated.");
            if (quitFailure != RunEgaUiFailureKind.FixedColumn || !quitDetail.Contains("Koniec", StringComparison.Ordinal) || !quitDetail.Contains("4", StringComparison.Ordinal))
                throw new InvalidDataException("Overlong Quit rejection lacks its fixed-column reason: " + quitDetail);
            if (RunEgaUiService.TryComposeRecord(RuntimeUiLogicalRecordId.ConfirmGeneric, "Are you sure ?",
                new Dictionary<string, string>(StringComparer.Ordinal) { ["no"] = "Nie" },
                out _, out _, out _))
                throw new InvalidDataException("Overlong No label was accepted.");
            if (RunEgaUiService.TryComposeRecord(RuntimeUiLogicalRecordId.PauseMenu, "Game Paused",
                new Dictionary<string, string>(StringComparer.Ordinal) { ["continue"] = "123456789" },
                out _, out _, out _))
                throw new InvalidDataException("Overlong Continue label was accepted.");
            // Round-trip: decomposition returns the translated labels; the
            // translated record validates, materializes and decodes back.
            if (!RunEgaUiService.TryExtractButtons(pauseTranslated, RuntimeUiLogicalRecordId.PauseMenu, out IReadOnlyDictionary<string, string>? back) ||
                !back!["continue"].Equals("Pokračuj", StringComparison.Ordinal) || !back["quit"].Equals("Quit", StringComparison.Ordinal))
                throw new InvalidDataException("Translated button round-trip diverged.");
            if (!RunEgaUiService.TryValidateStored(pauseTranslated, RuntimeUiLogicalRecordId.PauseMenu, out _, out string storedDetail))
                throw new InvalidDataException("Translated hotspot record did not validate: " + storedDetail);
            if (!RunEgaUiService.TryBuildSpan(RuntimeUiLogicalRecordId.PauseMenu, pauseTranslated, out byte[] span) || span.Length != 42)
                throw new InvalidDataException("Translated hotspot span did not materialize.");
            // In-cell field edits preserve already-translated buttons.
            if (!RunEgaUiService.TryExtractButtons(pauseTranslated, RuntimeUiLogicalRecordId.PauseMenu, out IReadOnlyDictionary<string, string>? kept) ||
                !RunEgaUiService.TryComposeRecord(RuntimeUiLogicalRecordId.PauseMenu, "Prestávka", kept, out string reframed, out _, out _) ||
                !reframed.Contains("Pokračuj", StringComparison.Ordinal))
                throw new InvalidDataException("Field recomposition reset translated buttons.");
            // The modal editor edits buttons through their own boxes.
            using (var buttonEditor = new RuntimeUiSemanticEditorForm("Pause.menu", RuntimeUiLogicalRecordId.PauseMenu, VariantRuntimeKind.Elvira1Ega, pauseRows, pauseRows))
            {
                buttonEditor.SetSemanticFieldForTest("continue", "Pokračuj");
                if (!buttonEditor.SubmitForTest() || buttonEditor.ComposedFullRecord is null ||
                    !buttonEditor.ComposedFullRecord.Equals("     Game Paused\r\r\r Pokračuj      Quit", StringComparison.Ordinal))
                    throw new InvalidDataException("Semantic editor did not compose the button label.");
            }
            // V5 VGA rule: Pause buttons are editable variable-width rows
            // with geometry hints (anchors frozen, lengths variable).
            IReadOnlyList<RunEgaUiSemanticField> vgaPause = RunVgaVariableUiService.TryDecompose(RuntimeUiLogicalRecordId.PauseMenu, originals[RuntimeUiLogicalRecordId.PauseMenu], out IReadOnlyList<RunEgaUiSemanticField>? vgaRows) && vgaRows is not null
                ? vgaRows
                : throw new InvalidDataException("VGA Pause decomposition failed.");
            if (vgaPause.Any(row => row.Key is "continue" or "quit" && row.Role != RunEgaUiSemanticRole.EditableSafe))
                throw new InvalidDataException("VGA Pause buttons are not editable variable-width rows.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRuntimeUiMigrationSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V3 §11: deterministic schema v1 -> v2 migration. Unambiguous
        // records are assigned to their only qualifying runtime; ambiguous
        // residue is preserved unassigned (surfaced, never built, never
        // silently duplicated); v1 bytes are backed up before the first
        // validated v2 write replaces them.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiMigrationSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext e1 = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var texts = new RuntimeUiTextService();
            string gameId = RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1).GameId;
            Directory.CreateDirectory(e1.ProjectRoot);
            string legacyJson = "{\"schemaVersion\":1,\"gameId\":\"" + gameId + "\",\"records\":["
                + "{\"logicalRecordId\":\"SaveFailure\",\"text\":\"\\r    Save failed.\"},"
                + "{\"logicalRecordId\":\"PauseMenu\",\"text\":\"     Hra\\r\\r\\r Continue      Quit\"},"
                + "{\"logicalRecordId\":\"ConfirmGeneric\",\"text\":\"raw unstructured legacy\"}]}";
            File.WriteAllText(texts.GetPath(e1), legacyJson);
            RuntimeUiTextLoadResult migrated = texts.Load(e1);
            if (!migrated.IsSuccess || migrated.State is null)
                throw new InvalidDataException("Legacy v1 state did not migrate: " + migrated.Status);
            if (migrated.State.Overrides.Count != 1 ||
                migrated.State.Overrides.Single() is not { Runtime: VariantRuntimeKind.Elvira1Ega, LogicalRecordId: RuntimeUiLogicalRecordId.SaveFailure })
                throw new InvalidDataException("Unambiguous v1 record was not assigned to its only runtime.");
            if (migrated.State.UnassignedLegacy.Count != 2 ||
                !migrated.State.UnassignedLegacy.Any(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu && value.ReasonCode == RuntimeUiTextService.ReasonAmbiguous && value.CandidateRuntimes.Count == 2) ||
                !migrated.State.UnassignedLegacy.Any(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.ConfirmGeneric && value.ReasonCode == RuntimeUiTextService.ReasonInvalidForAll))
                throw new InvalidDataException("Ambiguous v1 residue was not preserved unassigned with reasons.");
            if (string.IsNullOrWhiteSpace(migrated.Detail))
                throw new InvalidDataException("Migration did not surface a notice.");
            // Save writes validated v2 and preserves v1 bytes in the backup.
            RuntimeUiTextSaveResult saved = texts.Save(e1, migrated.State);
            if (!saved.Succeeded)
                throw new InvalidDataException("Migrated state did not save: " + saved.Detail);
            string backup = Path.Combine(e1.ProjectRoot, RuntimeUiTextService.LegacyBackupFileName);
            if (!File.Exists(backup) || File.ReadAllText(backup) != legacyJson)
                throw new InvalidDataException("Legacy v1 bytes were not backed up before the v2 write.");
            RuntimeUiTextLoadResult reloaded = texts.Load(e1);
            if (!reloaded.IsSuccess || reloaded.State!.Overrides.Count != 1 || reloaded.State.UnassignedLegacy.Count != 2)
                throw new InvalidDataException("Migrated v2 state did not reload deterministically.");
            // Explicit assignment moves residue to exactly one runtime: the
            // EGA view still shows no Pause override afterwards (no silent
            // duplication into both runtimes).
            VariantContext ega = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega);
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(e1).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            RuntimeUiTextState assigned = texts.AssignUnassigned(e1, reloaded.State!, RuntimeUiLogicalRecordId.PauseMenu, VariantRuntimeKind.Elvira1Vga);
            if (assigned.UnassignedLegacy.Count != 1 || !texts.GetEffectiveRecords(vga, assigned).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu).IsOverridden ||
                texts.GetEffectiveRecords(ega, assigned).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.PauseMenu).IsOverridden)
                throw new InvalidDataException("Legacy assignment duplicated or misattributed the override.");
            RuntimeUiTextState discarded = texts.DiscardUnassigned(e1, assigned, RuntimeUiLogicalRecordId.ConfirmGeneric);
            if (discarded.UnassignedLegacy.Count != 0 || !texts.Save(e1, discarded).Succeeded)
                throw new InvalidDataException("Legacy discard regressed.");
            if (File.ReadAllText(backup) != legacyJson)
                throw new InvalidDataException("Legacy backup was overwritten.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyRuntimeUiIsolationSmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V3 §§10,19: the 11-step cross-runtime isolation sequence at
        // the grid level, ending with a project reload that preserves both
        // runtime-specific values independently.
        string root = Path.Combine(Path.GetTempPath(), "Pi1RuntimeUiIsolationSmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            translations.Save(fixture, translations.Add(text, skCreated));
            if (!GameInstallationValidator.TryValidate(fixture.GameRoot, InstallationDiscoverySource.Manual, out GameInstallation? e1) || e1 is null)
                throw new InvalidDataException("Isolation fixture did not validate as an installation.");
            using var form = new MainForm();
            form.InitializeInstallationStateForTest();
            form.ActivateInstallationForTest(e1);
            form.SelectTranslationForTest("SK");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            // 1. Save EGA Save.failed override via the semantic editor.
            // (Variant switches reload project state from disk, so each phase
            // is saved explicitly.)
            form.ApplySemanticEditForTest(RuntimeUiLogicalRecordId.SaveFailure, "Neuložené!");
            form.SaveRuntimeUiForTest();
            // 2. Switch VGA. 3. VGA Save.failed Project override is blank and
            // read-only. 4. No EGA override enables a VGA Reset.
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            if (!form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.SaveFailure).Equals(string.Empty, StringComparison.Ordinal))
                throw new InvalidDataException("EGA Save.failed leaked into the VGA Project override column.");
            if (!form.SelectRuntimeUiRowForTest(RuntimeUiLogicalRecordId.SaveFailure) || form.RuntimeUiResetEnabledForTest)
                throw new InvalidDataException("VGA Save.failed row carries EGA override state.");
            if (form.RuntimeUiOverrideCountForTest != 1)
                throw new InvalidDataException("EGA override was lost or duplicated by the runtime switch.");
            // 5. Switch back EGA. 6. EGA override remains.
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            if (!form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.SaveFailure).Equals("Neuložené!", StringComparison.Ordinal))
                throw new InvalidDataException("EGA Save.failed override did not survive the runtime switch.");
            // 7. Save VGA Pause title. 8. Switch EGA. 9. EGA Pause independent.
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            form.ApplySemanticEditForTest(RuntimeUiLogicalRecordId.PauseMenu, "Prestávka");
            form.SaveRuntimeUiForTest();
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            if (!form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals(string.Empty, StringComparison.Ordinal))
                throw new InvalidDataException("VGA Pause title leaked into the EGA view.");
            // 10. Reload project. 11. Both runtime-specific values survive.
            form.ReloadRuntimeUiForTest();
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Ega);
            if (!form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.SaveFailure).Equals("Neuložené!", StringComparison.Ordinal) ||
                !form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals(string.Empty, StringComparison.Ordinal))
                throw new InvalidDataException("EGA values did not survive the project reload independently.");
            form.SetActiveVariantForTest(BuiltInVariantId.Elvira1Vga);
            if (!form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.PauseMenu).Equals("Prestávka", StringComparison.Ordinal) ||
                !form.RuntimeUiGridOverrideForTest(RuntimeUiLogicalRecordId.SaveFailure).Equals(string.Empty, StringComparison.Ordinal))
                throw new InvalidDataException("VGA values did not survive the project reload independently.");
            _ = elvira2Source;
        }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void VerifyLegacyEditionSafetySmoke(string elvira1Source, string elvira2Source)
    {
        // R9F V3 §14 (option A): legacy flat owned outputs are detected,
        // never reinterpreted, never silently deleted. New edition builds
        // coexist beside them; explicit removal is a deliberate action.
        string root = Path.Combine(Path.GetTempPath(), "Pi1LegacyEditionSafetySmoke", Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            ProjectContext fixture = CreateBuildFixtureProjectContext(root, "e1", elvira1Source, ElviraGameProfile.Elvira1);
            var translations = new TranslationProjectService();
            TranslationProjectState text = TranslationProjectState.Empty(ElviraGameProfile.Elvira1);
            TranslationProjectVariant skCreated = translations.Create(fixture, text, "Slovak", "SK", new Dictionary<int, string> { [393] = "Slovensky projektovy text." });
            text = translations.Add(text, skCreated);
            translations.Save(fixture, text);
            TranslationProjectVariant sk = translations.Load(fixture).State!.Variants.Single(item => item.Code == "SK");
            var graphics = new GraphicsVariantService();
            var directories = new VariantDirectoryService();
            VariantContext vga = VariantContextCatalog.CreateBuiltIns(fixture).Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
            // Build SK once, then mirror its outputs into the runtime root to
            // simulate a pre-R9F flat owned output (manifest included).
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(fixture, vga, sk.Code), VariantDirectoryOperationStatus.Created, "legacy fixture edition directory");
            CompositeBuildService skBuild = new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics,
                    new ActiveProjectBuildInput(sk, GraphicsProjectState.Empty(ElviraGameProfile.Elvira1), RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1))),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, sk), sk.DataFile],
                projectVariantProvider: (_, _) => sk.Code);
            if (skBuild.Build(fixture, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Legacy fixture SK build failed.");
            string editionRoot = directories.GetVariantEditionDirectoryPath(fixture, vga, sk.Code);
            string runtimeRoot = directories.GetVariantDirectoryPath(fixture, vga);
            foreach (string file in Directory.EnumerateFiles(editionRoot, "*", SearchOption.TopDirectoryOnly))
            {
                string name = Path.GetFileName(file);
                if (name.Equals(VariantDirectoryService.OwnershipMarkerFileName, StringComparison.OrdinalIgnoreCase)) continue;
                File.Copy(file, Path.Combine(runtimeRoot, name), overwrite: false);
            }
            // The legacy marker must be a runtime-level (schema 1) marker for
            // the simulation to be faithful: rewrite it without the edition binding.
            File.WriteAllText(Path.Combine(runtimeRoot, VariantDirectoryService.OwnershipMarkerFileName),
                "{\"SchemaVersion\":1,\"Product\":\"Pi1ElviraVariantDirectory\",\"GameId\":\"" + RuntimeUiTextState.Empty(ElviraGameProfile.Elvira1).GameId + "\",\"VariantId\":\"Elvira1Vga\",\"DirectoryKey\":\"E1VGA\",\"BaselineFingerprint\":\"" + fixture.BaselineFingerprint + "\"}");
            var legacy = directories.DetectLegacyFlatVariant(fixture, vga);
            if (!legacy.HasLegacyFiles || !legacy.HasMatchingMarker || legacy.ManifestProjectCode != "SK" || !legacy.FileNames.Contains("GAMEPCSK"))
                throw new InvalidDataException("Legacy flat owned output was not detected with its manifest identity.");
            if (!directories.Enumerate(fixture, [vga]).Any(item => item.Classification == VariantDirectoryClassification.LegacyFlatOwned))
                throw new InvalidDataException("Legacy flat output was not enumerated as legacy.");
            // Rebuilding SK leaves every legacy byte untouched and resolves
            // the launcher at the edition output, never at the legacy copy.
            var beforeLegacy = legacy.FileNames.ToDictionary(name => name, name => HashFile(Path.Combine(runtimeRoot, name)), StringComparer.OrdinalIgnoreCase);
            if (skBuild.Build(fixture, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("SK rebuild beside legacy output failed.");
            foreach ((string name, string hash) in beforeLegacy)
            {
                if (!File.Exists(Path.Combine(runtimeRoot, name)) || !HashFile(Path.Combine(runtimeRoot, name)).Equals(hash, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidDataException("Rebuild touched legacy flat output: " + name);
            }
            var launcher = new VariantLauncherService(directories, skBuild);
            VariantLaunchTarget target = launcher.Resolve(fixture, vga);
            if (target.Readiness != VariantLaunchReadiness.LaunchReady || !target.WorkingDirectory.Equals(editionRoot, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("Launcher did not resolve the edition output: " + target.Detail);
            var recovery = new RecoverySafetyService(directories, skBuild);
            RestorePlan plan = recovery.CreateRestorePlan(fixture);
            if (plan.Items.Any(item => item.Kind == RestorePlanItemKind.RemoveOwnedVariant && item.RelativePath.EndsWith("E1VGA", StringComparison.OrdinalIgnoreCase) && item.ProjectCode is null) ||
                !plan.Items.Any(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved && item.RelativePath.EndsWith("E1VGA", StringComparison.OrdinalIgnoreCase)))
                throw new InvalidDataException("Restore plan reinterpreted legacy output as a removable owned variant.");
            if (!plan.Items.Any(item => item.Kind == RestorePlanItemKind.RemoveOwnedVariant && item.ProjectCode == "SK"))
                throw new InvalidDataException("Restore plan lost the edition output entry.");
            // Explicit edition removal keeps legacy files; explicit legacy
            // removal then clears the root. Foreign contents stay untouched.
            RecoverySafetyOperationResult removedEdition = recovery.RemoveOwnedVariant(fixture, vga, sk.Code);
            if (!removedEdition.Succeeded || Directory.Exists(editionRoot))
                throw new InvalidDataException("Edition removal failed: " + removedEdition.Detail);
            foreach (string name in beforeLegacy.Keys)
            {
                if (!File.Exists(Path.Combine(runtimeRoot, name)))
                    throw new InvalidDataException("Edition removal touched legacy output: " + name);
            }
            RecoverySafetyOperationResult removedLegacy = recovery.RemoveOwnedVariant(fixture, vga, sk.Code);
            if (!removedLegacy.Succeeded || Directory.Exists(runtimeRoot))
                throw new InvalidDataException("Explicit legacy removal failed: " + removedLegacy.Detail);
            _ = elvira2Source;
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
            // R9F V3 isolation: an EGA-scoped override projects only into the
            // EGA view. The VGA view of the same logical record stays blank
            // (MappingIncomplete); it never inherits EGA project text.
            RuntimeUiTextState e1State = texts.SetOverride(e1, texts.Load(e1).State!, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    Save failed.");
            VariantContext[] e1Variants = VariantContextCatalog.CreateBuiltIns(e1).ToArray();
            string? vgaText = texts.GetEffectiveRecords(e1Variants[0], e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SaveFailure).EffectiveText;
            RuntimeUiRuntimeProjection egaView = texts.GetEffectiveRecords(e1Variants[1], e1State).Single(value => value.LogicalRecordId == RuntimeUiLogicalRecordId.SaveFailure);
            if (vgaText is not null || egaView is not { IsOverridden: true, EffectiveText: "\r    Save failed." })
                throw new InvalidDataException("EGA Runtime UI state leaked across runtime views.");
            if (layouts.Validate(e1Variants.Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Vga), e1State, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.MappingIncomplete ||
                layouts.Validate(e1Variants.Single(value => value.RuntimeKind == VariantRuntimeKind.Elvira1Ega), e1State, RuntimeUiLogicalRecordId.SaveFailure).Status != RuntimeUiLayoutValidationStatus.Valid)
                throw new InvalidDataException("E1 runtime status presentation diverged.");
            VariantContext runit = VariantContextCatalog.CreateBuiltIns(e2).Single();
            RuntimeUiTextState e2State = texts.SetOverride(e2, texts.Load(e2).State!, VariantRuntimeKind.Elvira2Vga, RuntimeUiLogicalRecordId.SaveFailure, "Save failed");
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
            RuntimeUiTextState reset = texts.RemoveOverride(e1, e1State, VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure);
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
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, vga, "SK"), VariantDirectoryOperationStatus.Created, "translation build fixture directory");
            ICompositeBuildStep[] buildSteps =
            [
                new TranslationProjectBuildStep(translations, directories, "SK"),
                new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations, "graphics", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations, "font", _ => true, _ => null, _ => null),
                new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations, "runtime-ui", _ => true, _ => null, _ => null),
                new RunVgaCompositeBuildStep(directories, "SK")
            ];
            CompositeBuildResult built = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, buildSteps,
                projectVariantProvider: (_, _) => "SK").Build(project, vga, CompositeBuildMode.Full);
            string output = directories.GetVariantEditionDirectoryPath(project, vga, "SK");
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
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, vga, "S1"), VariantDirectoryOperationStatus.Created, "active project variant directory");
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, vga, "SK"), VariantDirectoryOperationStatus.Created, "active project SK edition directory");
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, vga, "EN"), VariantDirectoryOperationStatus.Created, "active project EN edition directory");

            CompositeBuildService Create(TranslationProjectVariant selected) => new(new DisposableVariantBuildService(directories), directories,
                stepProvider: (_, _) => ActiveProjectCompositeBuildFactory.Create(directories, translations, graphics, new ActiveProjectBuildInput(selected, graphicsState, runtimeUi)),
                runtimeArtifactProvider: (_, runtime) => [ActiveProjectBuildIdentity.ExecutableName(runtime, selected), selected.DataFile],
                projectVariantProvider: (_, _) => selected.Code);
            string s1Root = directories.GetVariantEditionDirectoryPath(project, vga, "S1");
            string skRoot = directories.GetVariantEditionDirectoryPath(project, vga, "SK");
            string enRoot = directories.GetVariantEditionDirectoryPath(project, vga, "EN");
            CompositeBuildService s1Build = Create(s1);
            CompositeBuildResult s1Result = s1Build.Build(project, vga, CompositeBuildMode.Full);
            if (s1Result.Status != CompositeBuildStatus.Success) throw new InvalidDataException("S1 composite build failed: " + s1Result.Status + " / " + s1Result.Stages.Last().Detail);
            string s1Data = Path.Combine(s1Root, "GAMEPCS1"), s1Exe = Path.Combine(s1Root, "RUNVGAS1.EXE"), built382 = Path.Combine(s1Root, "382.VGA");
            VariantLaunchTarget s1Target = new VariantLauncherService(directories, s1Build).Resolve(project, vga);
            ParsedTable rebuiltTable = new VgaImageTableParser(File.ReadAllBytes(built382)).Parse();
            byte[] rebuiltPixels = ElviraImageDecoder.Decode(File.ReadAllBytes(built382), rebuiltTable.Entries.Single(entry => entry.ImageId == 1));
            if (!File.Exists(s1Data) || !File.Exists(s1Exe) || !GamePcTextEditor.GetEncoding("CP852").GetString(GamePcTextEditor.LoadEntries(s1Data, ElviraGameProfile.Elvira1).Single(entry => entry.Index == 393).OriginalBytes).Contains("Prirucna", StringComparison.Ordinal) ||
                HashFile(built382) == HashFile(source382) || rebuiltPixels[0] != ((pixels[0] + 1) & 0x0F) || s1Target is not { Readiness: VariantLaunchReadiness.LaunchReady, ExecutableFile: "RUNVGAS1.EXE", DataFile: "GAMEPCS1" })
                throw new InvalidDataException("S1 build did not materialize its selected text, executable, graphics, and Ready state.");
            string[] first = Snapshot(s1Root);
            if (s1Build.Build(project, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !first.SequenceEqual(Snapshot(s1Root), StringComparer.Ordinal))
                throw new InvalidDataException("Selected S1 composite rebuild was not reproducible.");

            // R9F V3 coexistence: the SK build owns its independent edition
            // directory and never deletes or replaces the S1 output.
            CompositeBuildService skBuild = Create(sk);
            if (skBuild.Build(project, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !File.Exists(Path.Combine(skRoot, "GAMEPCSK")) || !File.Exists(Path.Combine(skRoot, "RUNVGASK.EXE")) || !File.Exists(s1Data) || !File.Exists(s1Exe))
                throw new InvalidDataException("SK build did not coexist with the S1 output in its independent edition directory.");

            TranslationProjectVariant english = new("English", "EN", "GAMEPC", vga.SourceExecutableName, new Dictionary<int, string>());
            CompositeBuildService enBuild = Create(english);
            if (enBuild.Build(project, vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success || !File.Exists(Path.Combine(enRoot, "GAMEPC")) || !File.Exists(Path.Combine(enRoot, "RUNVGA.EXE")))
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
            RuntimeUiTextState s1Runtime = runtime.SetOverride(project, RuntimeUiTextState.Empty(project.GameProfile), VariantRuntimeKind.Elvira1Ega, RuntimeUiLogicalRecordId.SaveFailure, "\r    S1 only.");
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
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, e1Vga, "EN"), VariantDirectoryOperationStatus.Created, "E1VGA baseline edition");
            if (service.Resolve(e1, e1Vga) is not { Readiness: VariantLaunchReadiness.BuildIncomplete, Ownership: VariantDirectoryOperationStatus.AlreadyValid })
                throw new InvalidDataException("Owned pristine E1VGA variant was falsely launch-ready.");
            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign); File.WriteAllBytes(Path.Combine(foreign, "FOREIGN.DAT"), new byte[] { 0x46 });
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
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, e1Vga, "EN"), VariantDirectoryOperationStatus.Created, "E1VGA baseline edition");
            AssertBlocked(execution.Execute(launcher.Resolve(e1, e1Vga), VariantExecutionMode.Run), runner, runner.Starts.Count, "Incomplete variant");

            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign); File.WriteAllBytes(Path.Combine(foreign, "FOREIGN.DAT"), new byte[] { 0x46 });
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
            string e1Root = directories.GetVariantEditionDirectoryPath(e1, e1Vga, "EN");
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
            if (recovery.RemoveOwnedVariant(e1, e1Vga, "EN") is not { Succeeded: true, VariantResult.Status: VariantDirectoryOperationStatus.Deleted } || Directory.Exists(directories.GetVariantDirectoryPath(e1, e1Vga)))
                throw new InvalidDataException("Owned variant was not safely removed.");
            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign); File.WriteAllBytes(Path.Combine(foreign, "FOREIGN.DAT"), new byte[] { 0x46 });
            RecoverySafetyOperationResult foreignResult = recovery.RemoveOwnedVariant(e1, e1Ega, "EN");
            if (foreignResult.Succeeded || foreignResult.VariantResult?.Status != VariantDirectoryOperationStatus.ForeignDirectoryConflict || !Directory.Exists(foreign))
                throw new InvalidDataException("Foreign variant was removed or accepted.");
            Directory.Delete(foreign, true);

            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single();
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e2, e2Vga, "EN"), VariantDirectoryOperationStatus.Created, "owned rebuild fixture");
            RecoverySafetyOperationResult rebuilt = recovery.RebuildOwnedVariant(e2, e2Vga, "EN");
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
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, e1Vga, "EN"), VariantDirectoryOperationStatus.Created, "owned restore fixture");
            File.WriteAllBytes(Path.Combine(directories.GetVariantEditionDirectoryPath(e1, e1Vga, "EN"), "PAYLOAD.DAT"), [0x50, 0x49, 0x31]);
            string foreign = directories.GetVariantDirectoryPath(e1, e1Ega); Directory.CreateDirectory(foreign); File.WriteAllBytes(Path.Combine(foreign, "FOREIGN.DAT"), [0x46]);

            RestorePlan preview = recovery.CreateRestorePlan(e1);
            if (preview.IsBlocked || RestorePlanSignature(preview) != RestorePlanSignature(recovery.CreateRestorePlan(e1)) ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.RestoreMutable && item.RelativePath == "ELVIRA.BAT") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.RemoveOwnedVariant && item.VariantId == e1Vga.VariantId && item.ProjectCode == "EN") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.RemoveKnownEditorArtifact && item.RelativePath == "PI1MENU.COM") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved && item.RelativePath == "EXTERNAL.DAT") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved && item.RelativePath == "GAMEPC") ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved && item.RelativePath.EndsWith("E1EGA", StringComparison.OrdinalIgnoreCase)) ||
                !preview.Items.Any(item => item.Kind == RestorePlanItemKind.NoAction && item.RelativePath == "GAMEPCO"))
                throw new InvalidDataException("Restore preview classifications were not exact/narrow.");
            RestorePlanExecutionResult executed = recovery.ExecuteRestorePlan(e1, preview);
            if (!executed.Succeeded || File.ReadAllBytes(launcher).AsSpan().SequenceEqual(originalLauncher) is false || File.Exists(artifact) || Directory.Exists(directories.GetVariantEditionDirectoryPath(e1, e1Vga, "EN")) ||
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
        VariantDirectoryOperationResult ensured = directories.EnsureVariantEditionDirectory(project, variant, "EN");
        if (ensured.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid))
            throw new InvalidDataException("Unable to prepare owned fixture variant: " + ensured.Status);
        string root = directories.GetVariantEditionDirectoryPath(project, variant, "EN");
        // R9D-honest fixture identity: pristine packed bytes under the
        // generated/data names plus a matching manifest, so the launcher
        // verifies a supported binary that is byte-identical to its record.
        File.Copy(Path.Combine(project.GameRoot, variant.SourceExecutableName), Path.Combine(root, variant.GeneratedExecutableName), overwrite: true);
        File.Copy(Path.Combine(project.GameRoot, variant.LogicalDataFileName), Path.Combine(root, variant.LogicalDataFileName), overwrite: true);
        VariantManifestService.Write(project, variant, directories, CompositeBuildMode.PristineOnly,
            [variant.GeneratedExecutableName, variant.LogicalDataFileName], "EN");
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
                "Execution.Run", "Execution.Debug", "Graphics.SaveProject", "Error.Title",
                "RuntimeUi.Status.RouteProvenLayoutIncomplete", "RuntimeUi.Detail.RouteProvenLayoutIncomplete",
                "RuntimeUi.BuildNotMaterialized"
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
            if (UiText.Get("RuntimeUi.Status.RouteProvenLayoutIncomplete") == UiText.Get("en", "RuntimeUi.Status.RouteProvenLayoutIncomplete"))
                throw new InvalidDataException("Slovak Runtime UI status localization regressed to fallback.");
            UiText.SetLocale("cs");
            if (UiText.Get("RuntimeUi.Status.RouteProvenLayoutIncomplete") == UiText.Get("en", "RuntimeUi.Status.RouteProvenLayoutIncomplete"))
                throw new InvalidDataException("Czech Runtime UI status localization regressed to fallback.");
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
            !slovak.Contains('á') || !provider.TryGet("cs", UiLocalizationKeys.NoGameSelected, out string czech) || czech != "Není vybrána žádná hra.")
            throw new InvalidDataException("Initial JSON locale lookup did not retain the explicit en/sk/cs bootstrap contract.");
        // Fallback coverage must use a synthetic key that no production locale
        // defines, never the deliberate absence of a real production key.
        if (provider.TryGet("en", "Synthetic.Nonexistent.Locale.Key", out _) || provider.TryGet("cs", "Synthetic.Nonexistent.Locale.Key", out _) ||
            provider.Resolve("cs", "Synthetic.Nonexistent.Locale.Key") is not { Source: UiLocaleLookupSource.MissingKey })
            throw new InvalidDataException("JSON locale fallback did not report a synthetic missing key deterministically.");

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
            // The production neutral prompt joins with a space (see
            // MainForm.NeutralInstallationPrompt); the smoke must assert that
            // current contract, not a historical newline layout.
            string englishNeutral = UiText.Get(UiLocalizationKeys.NoGameSelected) + " " + UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder);
            if (!form.IsNeutralInstallationStateForTest || form.InstallationStatusForTest != englishNeutral)
                throw new InvalidDataException("MainForm did not expose the localized neutral startup state.");

            UiText.SetLanguage(UiLanguage.Slovak);
            string slovakNeutral = UiText.Get(UiLocalizationKeys.NoGameSelected) + " " + UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder);
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
            // Same production contract as the ui-localization smoke: the neutral
            // prompt joins with a space (MainForm.NeutralInstallationPrompt).
            if (!form.IsNeutralInstallationStateForTest || form.InstallationOptionCountForTest != 0 ||
                form.InstallationStatusForTest != "No game selected. Use Find games... or Browse folder...")
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
                UiText.Get("OpenGameExe") != "Otvoriť EXE hry..." ||
                UiText.Get("Synthetic.Nonexistent.Button.Key") != "Synthetic.Nonexistent.Button.Key")
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

            string readyRoot = directories.GetVariantEditionDirectoryPath(e1, e1Vga, "EN");
            // R9D-honest Ready fixture: pristine packed bytes under the
            // generated/data names plus a matching manifest.
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, e1Vga, "EN"), VariantDirectoryOperationStatus.Created, "E1 VGA ready edition");
            File.Copy(Path.Combine(e1.GameRoot, e1Vga.SourceExecutableName), Path.Combine(readyRoot, e1Vga.GeneratedExecutableName), overwrite: true);
            File.Copy(Path.Combine(e1.GameRoot, e1Vga.LogicalDataFileName), Path.Combine(readyRoot, e1Vga.LogicalDataFileName), overwrite: true);
            VariantManifestService.Write(e1, e1Vga, directories, CompositeBuildMode.PristineOnly,
                [e1Vga.GeneratedExecutableName, e1Vga.LogicalDataFileName], "EN");
            VariantBuildStatusProjection ready = status.Inspect(e1, e1Vga);
            if (ready.Status != VariantBuildStatus.Ready || ready.ConfiguredCapabilities != ready.CapabilityCount || ready.CapabilityCount == 0)
                throw new InvalidDataException("Owned E1 VGA output with configured plan was not reported Ready.");
            File.Delete(Path.Combine(readyRoot, e1Vga.GeneratedExecutableName));
            File.Delete(Path.Combine(readyRoot, e1Vga.LogicalDataFileName));
            File.Delete(Path.Combine(readyRoot, VariantManifestService.FileName));
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
                    // the generated name inside the owned runtime+edition output
                    // instead of arbitrary fake bytes.
                    variant => { string editionRoot = directories.GetVariantEditionDirectoryPath(variant.Project, variant, "EN"); File.Copy(Path.Combine(editionRoot, variant.SourceExecutableName), Path.Combine(editionRoot, variant.GeneratedExecutableName), true); return null; })).ToArray();
            var composite = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, steps,
                projectVariantProvider: (_, _) => "EN");
            var launcher = new VariantLauncherService(directories, composite);
            var status = new VariantBuildStatusService(composite, launcher);
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            VariantContext e2Vga = VariantContextCatalog.CreateBuiltIns(e2).Single(item => item.VariantId == BuiltInVariantId.Elvira2Vga);

            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e1, e1Vga, "EN"), VariantDirectoryOperationStatus.Created, "E1 manifest fixture directory");
            string e1Root = directories.GetVariantEditionDirectoryPath(e1, e1Vga, "EN");
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

            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(e2, e2Vga, "EN"), VariantDirectoryOperationStatus.Created, "E2 manifest fixture directory");
            if (composite.Build(e2, e2Vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("E2 manifest fixture build failed.");
            VerifyVariantManifest(VariantManifestService.Read(directories.GetVariantEditionDirectoryPath(e2, e2Vga, "EN")), e2, e2Vga, "Full");
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

            VerifyRepeatedBuild(e1, e1Vga, new RunVgaCompositeBuildStep(directories, "EN"));
            VerifyRepeatedBuild(e1, e1Ega, new RunEgaCompositeBuildStep(directories, "EN"));
            VerifyRepeatedBuild(e2, e2Vga, new RunItCompositeBuildStep(directories, "EN"));

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
                var builds = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, steps,
                    projectVariantProvider: (_, _) => "EN");
                VariantDirectoryOperationResult prepared = directories.EnsureVariantEditionDirectory(project, variant, "EN");
                if (prepared.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid))
                    throw new InvalidDataException($"{variant.VariantId} fixture directory could not be prepared: {prepared.Status}.");
                CompositeBuildResult firstBuild = builds.Build(project, variant, CompositeBuildMode.Full);
                if (firstBuild.Status != CompositeBuildStatus.Success)
                    throw new InvalidDataException($"First {variant.VariantId} build failed: {firstBuild.Status} / {firstBuild.Stages.Last().Detail}");

                string variantRoot = directories.GetVariantEditionDirectoryPath(project, variant, "EN");
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
            VerifyCompositeRejection(e1, BuiltInVariantId.Elvira1Vga, new RunVgaCompositeBuildStep(directories, "EN"), "RUNVGA.EXE", "RUNVGASK.EXE", "Elvira I VGA", directories, disposable);
            VerifyCompositeRejection(e1, BuiltInVariantId.Elvira1Ega, new RunEgaCompositeBuildStep(directories, "EN"), "RUNEGA.EXE", "RUNEGASK.EXE", "Elvira I EGA", directories, disposable);
            VerifyCompositeRejection(e2, BuiltInVariantId.Elvira2Vga, new RunItCompositeBuildStep(directories, "EN"), "RUNIT.EXE", "RUNITSK.EXE", "Elvira II VGA", directories, disposable);

            // Run/Debug readiness: supported full-build output stays LaunchReady; a mutated output does not.
            VariantContext e1Vga = VariantContextCatalog.CreateBuiltIns(e1).Single(item => item.VariantId == BuiltInVariantId.Elvira1Vga);
            ICompositeBuildStep[] stages = CompleteFixtureStages(null, null).Take(4).Append(new RunVgaCompositeBuildStep(directories, "EN")).ToArray();
            var builds = new CompositeBuildService(disposable, directories, stages, projectVariantProvider: (_, _) => "EN");
            if (builds.Build(e1, e1Vga, CompositeBuildMode.Full).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Supported RUNVGA full build failed; the acceptance path regressed.");
            var launcher = new VariantLauncherService(directories, builds);
            VariantLaunchTarget ready = launcher.Resolve(e1, e1Vga);
            if (ready.Readiness != VariantLaunchReadiness.LaunchReady)
                throw new InvalidDataException("Supported RUNVGA output was not launch-ready: " + ready.Detail);
            string generated = Path.Combine(directories.GetVariantEditionDirectoryPath(e1, e1Vga, "EN"), e1Vga.GeneratedExecutableName);
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
            string variantData = Path.Combine(directories.GetVariantEditionDirectoryPath(e1, e1Vga, "EN"), e1Vga.LogicalDataFileName);
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
                RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, variant, "EN"), VariantDirectoryOperationStatus.Created, runtimeDisplay + " fixture directory");
                RequireBuildStatus(disposable.Build(project, variant, "EN"), DisposableVariantBuildStatus.Success, runtimeDisplay + " pristine build");
                string variantRoot = directories.GetVariantEditionDirectoryPath(project, variant, "EN");
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
            VariantDirectoryOperationResult prepared = directories.EnsureVariantEditionDirectory(variant.Project, variant, "EN");
            if (prepared.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid)) throw new InvalidDataException("Real pristine edition directory could not be prepared.");
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
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, variant, "EN"), VariantDirectoryOperationStatus.Created, "composite fixture directory");
            string variantRoot = directories.GetVariantEditionDirectoryPath(project, variant, "EN");
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
            var step = new RunVgaCompositeBuildStep(directories, "EN");
            if (!step.AppliesTo(vga) || step.AppliesTo(ega)) throw new InvalidDataException("RUNVGA applicability is not runtime-specific.");
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project, vga, "EN"), VariantDirectoryOperationStatus.Created, "RUNVGA fixture directory");
            if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")))
                throw new InvalidDataException("Fixture root unexpectedly contains a builder artifact before execution.");
            var capabilityOnly = new CompositeBuildService(disposable, directories, [step]);
            if (!capabilityOnly.CreatePlan(project, vga, CompositeBuildMode.PristineOnly).Capabilities.Single().Configured)
                throw new InvalidDataException("RUNVGA patch capability was not reported as configured.");
            if (capabilityOnly.Build(project, vga, CompositeBuildMode.PristineOnly).Status != CompositeBuildStatus.Success)
                throw new InvalidDataException("Pristine-only capability inspection/rebuild failed.");
            string variantRoot = directories.GetVariantEditionDirectoryPath(project, vga, "EN"), source = Path.Combine(variantRoot, "RUNVGA.EXE"), output = Path.Combine(variantRoot, "RUNVGASK.EXE");
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

            var corruptSource = new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations, "corrupt-after-copy", _ => true, _ => null, variant => { File.WriteAllBytes(Path.Combine(directories.GetVariantEditionDirectoryPath(variant.Project, variant, "EN"), "RUNVGA.EXE"), [0]); return null; });
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
            var directories=new VariantDirectoryService();var disposable=new DisposableVariantBuildService(directories);VariantContext ega=VariantContextCatalog.CreateBuiltIns(project).Single(x=>x.RuntimeKind==VariantRuntimeKind.Elvira1Ega);var step=new RunEgaCompositeBuildStep(directories, "EN");
            if(!step.AppliesTo(ega)||step.AppliesTo(VariantContextCatalog.CreateBuiltIns(project).Single(x=>x.RuntimeKind==VariantRuntimeKind.Elvira1Vga)))throw new InvalidDataException("RUNEGA applicability is not runtime-specific.");
            RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project,ega,"EN"),VariantDirectoryOperationStatus.Created,"RUNEGA fixture directory");
            var capability=new CompositeBuildService(disposable,directories,[step]);if(!capability.CreatePlan(project,ega,CompositeBuildMode.PristineOnly).Capabilities.Single().Configured)throw new InvalidDataException("RUNEGA capability is not configured.");
            CompositeBuildResult pristine=capability.Build(project,ega,CompositeBuildMode.PristineOnly);if(pristine.Status!=CompositeBuildStatus.Success)throw new InvalidDataException("RUNEGA pristine-only failed: "+pristine.Status+" / "+pristine.Stages.Last().Detail);string vr=directories.GetVariantEditionDirectoryPath(project,ega,"EN"),outp=Path.Combine(vr,"RUNEGASK.EXE");
            if(File.Exists(outp)||capability.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(outp))throw new InvalidDataException("Incomplete build executed RUNEGA.");
            ICompositeBuildStep[] all=[new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations,"data",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations,"graphics",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations,"font",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations,"ui",_=>true,_=>null,_=>null),step];
            var full=new CompositeBuildService(disposable,directories,all);if(full.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!="C4028BAB9A35B247008197A5753C5C5185F2DEBFF54DCB099178E318830F90EE"||new FileInfo(outp).Length!=RunEgaBootstrapService.OutputSize)throw new InvalidDataException("RUNEGA fixture output diverged.");
            File.WriteAllBytes(outp,[0]);if(full.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!="C4028BAB9A35B247008197A5753C5C5185F2DEBFF54DCB099178E318830F90EE")throw new InvalidDataException("RUNEGA fixture rebuild was nondeterministic.");
        }
        finally { if(Directory.Exists(root))Directory.Delete(root,true); }
    }

    private static void VerifyRealRunEgaCompositeBuild(string elvira1Root)
    {
        ProjectContext project=RequireSuccess(new ProjectContextLoader().Open(elvira1Root),"real RUNEGA project");VariantContext ega=VariantContextCatalog.CreateBuiltIns(project).Single(x=>x.RuntimeKind==VariantRuntimeKind.Elvira1Ega);var directories=new VariantDirectoryService();var composite=new CompositeBuildService(new DisposableVariantBuildService(directories),directories,[new RunEgaCompositeBuildStep(directories, "EN")]);
        CompositeBuildPlan plan=composite.CreatePlan(project,ega,CompositeBuildMode.PristineOnly);if(plan.Capabilities is not [{Stage:CompositeBuildStage.ApplyExecutableTransformation,Configured:true}])throw new InvalidDataException("Real RUNEGA capability not configured.");
        VariantDirectoryOperationResult prepared=directories.EnsureVariantEditionDirectory(project,ega,"EN");if(prepared.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid))throw new InvalidDataException("Real RUNEGA edition directory could not be prepared.");
        if(composite.Build(project,ega,CompositeBuildMode.PristineOnly).Status!=CompositeBuildStatus.Success)throw new InvalidDataException("Real RUNEGA pristine-only failed.");string vr=directories.GetVariantEditionDirectoryPath(project,ega,"EN"),output=Path.Combine(vr,"RUNEGASK.EXE");
        if(File.Exists(output)||composite.Build(project,ega,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(output)||new PristineManifestService(project.StorageLayout,project.GameProfile).ValidateBaseline().Status!=BaselineValidationStatus.MatchesBaseline)throw new InvalidDataException("Real RUNEGA capability changed its pristine state.");
    }

    private static void VerifyRunItCompositeFixture(string elvira2Source)
    {
        string root=Path.Combine(Path.GetTempPath(),"Pi1RunItCompositeSmoke",Guid.NewGuid().ToString("N"));
        try{Directory.CreateDirectory(root);ProjectContext project=CreateBuildFixtureProjectContext(root,"e2",elvira2Source,ElviraGameProfile.Elvira2);var directories=new VariantDirectoryService();var disposable=new DisposableVariantBuildService(directories);VariantContext runit=VariantContextCatalog.CreateBuiltIns(project).Single();var step=new RunItCompositeBuildStep(directories, "EN");if(!step.AppliesTo(runit))throw new InvalidDataException("RUNIT applicability failed.");RequireDirectoryStatus(directories.EnsureVariantEditionDirectory(project,runit,"EN"),VariantDirectoryOperationStatus.Created,"RUNIT fixture directory");var cap=new CompositeBuildService(disposable,directories,[step]);if(!cap.CreatePlan(project,runit,CompositeBuildMode.PristineOnly).Capabilities.Single().Configured)throw new InvalidDataException("RUNIT capability not configured.");if(cap.Build(project,runit,CompositeBuildMode.PristineOnly).Status!=CompositeBuildStatus.Success)throw new InvalidDataException("RUNIT pristine-only failed.");string vr=directories.GetVariantEditionDirectoryPath(project,runit,"EN"),outp=Path.Combine(vr,"RUNITSK.EXE");if(File.Exists(outp)||cap.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(outp))throw new InvalidDataException("Incomplete RUNIT build executed.");ICompositeBuildStep[] all=[new FixtureCompositeStep(CompositeBuildStage.ApplyDataTransformations,"data",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyGraphicsTransformations,"graphics",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyFontTransformations,"font",_=>true,_=>null,_=>null),new FixtureCompositeStep(CompositeBuildStage.ApplyRuntimeUiTransformations,"ui",_=>true,_=>null,_=>null),step];var full=new CompositeBuildService(disposable,directories,all);if(full.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!=Elvira2ProductionProfile.DeterministicFontEnabled.Sha256||new FileInfo(outp).Length!=RunItBootstrapService.ExtendedSize)throw new InvalidDataException("RUNIT fixture output diverged.");File.WriteAllBytes(outp,[0]);if(full.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.Success||HashFile(outp)!=Elvira2ProductionProfile.DeterministicFontEnabled.Sha256)throw new InvalidDataException("RUNIT fixture rebuild nondeterministic.");}
        finally{if(Directory.Exists(root))Directory.Delete(root,true);}
    }

    private static void VerifyRealRunItCompositeBuild(string elvira2Root)
    {
        ProjectContext project=RequireSuccess(new ProjectContextLoader().Open(elvira2Root),"real RUNIT project");VariantContext runit=VariantContextCatalog.CreateBuiltIns(project).Single();var directories=new VariantDirectoryService();var composite=new CompositeBuildService(new DisposableVariantBuildService(directories),directories,[new RunItCompositeBuildStep(directories, "EN")]);if(composite.CreatePlan(project,runit,CompositeBuildMode.PristineOnly).Capabilities is not [{Configured:true}])throw new InvalidDataException("Real RUNIT capability not configured.");VariantDirectoryOperationResult prepared=directories.EnsureVariantEditionDirectory(project,runit,"EN");if(prepared.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid))throw new InvalidDataException("Real RUNIT edition directory could not be prepared.");if(composite.Build(project,runit,CompositeBuildMode.PristineOnly).Status!=CompositeBuildStatus.Success)throw new InvalidDataException("Real RUNIT pristine-only failed.");string output=Path.Combine(directories.GetVariantEditionDirectoryPath(project,runit,"EN"),"RUNITSK.EXE");if(File.Exists(output)||composite.Build(project,runit,CompositeBuildMode.Full).Status!=CompositeBuildStatus.NotFullyConfigured||File.Exists(output)||new PristineManifestService(project.StorageLayout,project.GameProfile).ValidateBaseline().Status!=BaselineValidationStatus.MatchesBaseline)throw new InvalidDataException("Real RUNIT capability changed pristine state.");
    }

    private static void VerifyRealRunVgaCompositeBuild(string elvira1Root)
    {
        ProjectContext project = RequireSuccess(new ProjectContextLoader().Open(elvira1Root), "real RUNVGA project");
        VariantContext variant = VariantContextCatalog.CreateBuiltIns(project).Single(item => item.RuntimeKind == VariantRuntimeKind.Elvira1Vga);
        var directories = new VariantDirectoryService(); var composite = new CompositeBuildService(new DisposableVariantBuildService(directories), directories, [new RunVgaCompositeBuildStep(directories, "EN")]);
        if (File.Exists(Path.Combine(project.GameRoot, "GAMEPCO")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGASK.EXE")) || File.Exists(Path.Combine(project.GameRoot, "RUNVGAO.EXE")))
            throw new InvalidDataException("Real pristine root already contains a forbidden RUNVGA builder artifact.");
        CompositeBuildPlan plan = composite.CreatePlan(project, variant, CompositeBuildMode.PristineOnly);
        if (plan.Capabilities is not [{ Stage: CompositeBuildStage.ApplyExecutableTransformation, Configured: true }])
            throw new InvalidDataException("Real RUNVGA capability is not read-only configured.");
        VariantDirectoryOperationResult prepared = directories.EnsureVariantEditionDirectory(project, variant, "EN");
        if (prepared.Status is not (VariantDirectoryOperationStatus.Created or VariantDirectoryOperationStatus.AlreadyValid)) throw new InvalidDataException("Real RUNVGA edition directory could not be prepared.");
        CompositeBuildResult first = composite.Build(project, variant, CompositeBuildMode.PristineOnly);
        if (first.Status != CompositeBuildStatus.Success) throw new InvalidDataException("Real pristine-only rebuild failed: " + first.Status);
        string variantRoot = directories.GetVariantEditionDirectoryPath(project, variant, "EN"), source = Path.Combine(variantRoot, "RUNVGA.EXE"), output = Path.Combine(variantRoot, "RUNVGASK.EXE");
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
