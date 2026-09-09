using System.Security.Cryptography;
using System.Text;

namespace Pi1ElviraEditor;

/// <summary>R9F V8.6d content-based build freshness fingerprint. It captures
/// every material composite-build input for one explicit (runtime, edition)
/// target so a saved project change always invalidates a previously built
/// artifact without touching it. Deterministic canonical serialization;
/// no timestamps.</summary>
internal static class ProjectBuildFingerprintService
{
    internal const string Version = "PI1-BUILD-FINGERPRINT|v1";

    internal static string Compute(ProjectContext project, VariantContext variant, string editionCode)
    {
        using var _ = ModsRefreshDiagnostics.MeasureFingerprint();
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(variant);
        if (!ReferenceEquals(project, variant.Project))
            throw new ArgumentException("Variant must belong to the exact ProjectContext.", nameof(variant));
        string code = ProjectVariantOwnership.NormalizeCode(project, editionCode);

        var builder = new StringBuilder();
        builder.AppendLine(Version);
        builder.Append("game=").AppendLine(PristineManifestService.GameIdFor(project.GameProfile));
        builder.Append("variant=").AppendLine(variant.VariantId.ToString());
        builder.Append("runtime=").AppendLine(variant.RuntimeKind.ToString());
        builder.Append("dirkey=").AppendLine(variant.DirectoryKey ?? string.Empty);
        builder.Append("code=").AppendLine(code);
        builder.Append("baseline=").AppendLine(project.BaselineFingerprint ?? string.Empty);

        AppendTranslation(builder, project, variant, code);
        AppendFont(builder, project, variant, code);
        AppendGraphics(builder, project, variant, code);
        AppendRuntimeUi(builder, project, variant, code);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    private static void AppendTranslation(StringBuilder builder, ProjectContext project, VariantContext variant, string code)
    {
        try
        {
            if (ProjectVariantOwnership.IsOriginal(code))
            {
                builder.Append("translation=").Append("English|EN|")
                    .Append(variant.LogicalDataFileName ?? string.Empty).Append('|')
                    .AppendLine(variant.SourceExecutableName ?? string.Empty);
                return;
            }
            var service = new TranslationProjectService();
            TranslationProjectLoadResult loaded = service.Load(project);
            if (!loaded.IsSuccess || loaded.State is null)
            {
                builder.AppendLine("translation=INVALID");
                return;
            }
            TranslationProjectVariant? match = loaded.State.Variants
                .SingleOrDefault(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
            if (match is null)
            {
                builder.AppendLine("translation=MISSING");
                return;
            }
            builder.Append("translation=").Append(match.DisplayName ?? string.Empty).Append('|')
                .Append(match.Code ?? string.Empty).Append('|')
                .Append(match.DataFile ?? string.Empty).Append('|')
                .AppendLine(match.ExeFile ?? string.Empty);
            foreach ((int index, string text) in (match.Edits ?? new Dictionary<int, string>()).OrderBy(edit => edit.Key))
                builder.Append("tedit:").Append(index).Append(':').Append(text?.Length ?? 0).Append(':').AppendLine(text ?? string.Empty);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or InvalidDataException or IOException)
        {
            builder.Append("translation=INVALID:").AppendLine(ex.GetType().Name);
        }
    }

    private static void AppendFont(StringBuilder builder, ProjectContext project, VariantContext variant, string code)
    {
        try
        {
            var service = new FontVariantService();
            FontProjectLoadResult loaded = service.Load(project, code);
            if (!loaded.IsSuccess || loaded.State is null)
            {
                builder.AppendLine("font=INVALID");
                return;
            }
            IReadOnlyList<FontVariantProjection> applicable;
            try
            {
                applicable = service.GetFontEditsForVariant(project, loaded.State, variant);
            }
            catch (ArgumentException)
            {
                builder.AppendLine("font=INVALID");
                return;
            }
            foreach (FontVariantProjection projection in applicable.OrderBy(item => item.Edit.Identity.ByteValue))
            {
                FontProjectEdit edit = projection.Edit;
                builder.Append("font:").Append(edit.Identity.ByteValue).Append('|')
                    .Append(edit.BitmapBase64 ?? string.Empty).Append('|')
                    .Append(edit.Scope.ToString()).Append('|')
                    .AppendLine(edit.RuntimeKind?.ToString() ?? string.Empty);
            }
            builder.Append("font-count=").AppendLine(applicable.Count.ToString());
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or InvalidDataException or IOException)
        {
            builder.Append("font=INVALID:").AppendLine(ex.GetType().Name);
        }
    }

    private static void AppendGraphics(StringBuilder builder, ProjectContext project, VariantContext variant, string code)
    {
        try
        {
            var service = new GraphicsVariantService();
            GraphicsProjectLoadResult loaded = service.Load(project, code);
            if (!loaded.IsSuccess || loaded.State is null)
            {
                builder.AppendLine("graphics=INVALID");
                return;
            }
            IReadOnlyList<GraphicsVariantProjection> applicable;
            try
            {
                applicable = service.GetGraphicsEditsForVariant(project, loaded.State, variant);
            }
            catch (Exception ex) when (ex is ArgumentException or IOException)
            {
                builder.Append("graphics=INVALID:").AppendLine(ex.GetType().Name);
                return;
            }
            foreach (GraphicsVariantProjection projection in applicable
                .OrderBy(item => item.Edit.Identity.ResourceFileName, StringComparer.Ordinal)
                .ThenBy(item => item.Edit.Identity.ImageId))
            {
                GraphicsProjectEdit edit = projection.Edit;
                string pngIdentity;
                try
                {
                    pngIdentity = File.Exists(edit.ReplacementPngPath)
                        ? HashFile(edit.ReplacementPngPath) + "|" + new FileInfo(edit.ReplacementPngPath).Length
                        : "MISSING|" + (edit.ReplacementPngPath ?? string.Empty);
                }
                catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
                {
                    pngIdentity = "UNREADABLE";
                }
                builder.Append("graphics:").Append(edit.Identity.ResourceFileName ?? string.Empty).Append('|')
                    .Append(edit.Identity.ImageId).Append('|')
                    .Append(edit.Scope.ToString()).Append('|')
                    .Append(edit.RuntimeKind?.ToString() ?? string.Empty).Append('|')
                    .AppendLine(pngIdentity);
            }
            builder.Append("graphics-count=").AppendLine(applicable.Count.ToString());
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or InvalidDataException or IOException)
        {
            builder.Append("graphics=INVALID:").AppendLine(ex.GetType().Name);
        }
    }

    private static void AppendRuntimeUi(StringBuilder builder, ProjectContext project, VariantContext variant, string code)
    {
        try
        {
            var service = new RuntimeUiTextService();
            RuntimeUiTextLoadResult loaded = service.Load(project, code);
            if (!loaded.IsSuccess || loaded.State is null)
            {
                builder.AppendLine("runtimeui=INVALID");
                return;
            }
            IReadOnlyList<RuntimeUiTextOverride> scoped;
            try
            {
                scoped = service.GetOverridesForRuntime(loaded.State, variant.RuntimeKind);
            }
            catch (Exception ex) when (ex is ArgumentException)
            {
                builder.Append("runtimeui=INVALID:").AppendLine(ex.GetType().Name);
                return;
            }
            foreach (RuntimeUiTextOverride value in scoped.OrderBy(item => item.LogicalRecordId))
                builder.Append("runtimeui:").Append(value.Runtime.ToString()).Append('|')
                    .Append(value.LogicalRecordId.ToString()).Append(':')
                    .Append(value.Text?.Length ?? 0).Append(':').AppendLine(value.Text ?? string.Empty);
            builder.Append("runtimeui-count=").AppendLine(scoped.Count.ToString());
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException or InvalidDataException or IOException)
        {
            builder.Append("runtimeui=INVALID:").AppendLine(ex.GetType().Name);
        }
    }

    private static string HashFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
}
