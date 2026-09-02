namespace ElviraVgaEditor;

/// <summary>
/// A compact presentation model for the active Runtime + Edition workflow.
/// It deliberately has no persistence and never decides build readiness: that
/// remains the responsibility of the existing composite/manifest services.
/// </summary>
internal enum WorkflowStatusSeverity { Info, Success, Warning, Error }

internal sealed record WorkflowStatus(WorkflowStatusSeverity Severity, string Text, string? RuntimeKey, string? EditionCode)
{
    internal bool AppliesTo(VariantContext? runtime, string? editionCode) =>
        string.Equals(RuntimeKey, runtime?.DirectoryKey, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(EditionCode, editionCode, StringComparison.OrdinalIgnoreCase);
}
