using System.Text;

namespace Pi1ElviraEditor;

/// <summary>Read-only original-text evidence for all eight RUNVGA records.
/// The three variable records delegate to the proven variable decoder;
/// the other five decode NUL-terminated CP852 strings at the frozen
/// OriginalBlockStart addresses from Elvira1ProductionProfile.RunVgaRoutes.
/// No editing, no materialization, no capacity inference: route evidence,
/// layout safety and build materialization stay separate. The grid shows
/// these originals; the five remain non-editable / non-materializable.</summary>
internal static class RunVgaOriginalTextService
{
    internal static bool TryGetFullOriginal(RuntimeUiLogicalRecordId id, string gameRoot, out string? full)
    {
        full = null;
        if (RunVgaVariableUiService.IsVariableRecord(id))
            return RunVgaVariableUiService.TryDecodeOriginalFromGameRoot(gameRoot, id, out full, out _);
        return TryDecodeRoutedOriginal(gameRoot, id, out full);
    }

    internal static bool TryGetShortOriginal(RuntimeUiLogicalRecordId id, string gameRoot, out string? shortText)
    {
        shortText = null;
        if (RunVgaVariableUiService.IsVariableRecord(id))
        {
            if (!RunVgaVariableUiService.TryDecodeOriginalFromGameRoot(gameRoot, id, out string? full, out _) || full is null)
                return false;
            if (!RunVgaVariableUiService.TryParse(id, full, out IReadOnlyDictionary<string, string>? fields) || fields is null)
                return false;
            shortText = id switch
            {
                RuntimeUiLogicalRecordId.PauseMenu => fields["title"],
                RuntimeUiLogicalRecordId.ConfirmGeneric => fields["prompt"],
                RuntimeUiLogicalRecordId.SaveOverwrite => fields["message"],
                _ => null
            };
            return shortText is not null;
        }
        if (!TryDecodeRoutedOriginal(gameRoot, id, out string? routed) || routed is null)
            return false;
        shortText = routed.Trim('\r', ' ', '\t');
        return true;
    }

    private static bool TryDecodeRoutedOriginal(string gameRoot, RuntimeUiLogicalRecordId id, out string? text)
    {
        text = null;
        string name = id switch
        {
            RuntimeUiLogicalRecordId.SavePrompt => "Save.prompt",
            RuntimeUiLogicalRecordId.SaveFailure => "Save.failed",
            RuntimeUiLogicalRecordId.LoadFailure => "Restore.loadFailed",
            RuntimeUiLogicalRecordId.FileNotFound => "Restore.fileNotFound",
            RuntimeUiLogicalRecordId.TryAnotherDisk => "Disk.retry",
            _ => string.Empty
        };
        if (string.IsNullOrEmpty(name))
            return false;
        try
        {
            string candidate = Path.Combine(gameRoot, Elvira1ProductionProfile.ActiveVgaExecutable);
            if (!File.Exists(candidate))
                return false;
            byte[] data = File.ReadAllBytes(candidate);
            RunVgaBootstrapState state = RunVgaBootstrapService.DetectState(candidate);
            byte[] image = state switch
            {
                RunVgaBootstrapState.UnpackedBaseline => data,
                RunVgaBootstrapState.ExtendedCp852V5 => data,
                RunVgaBootstrapState.OriginalPacked => RunVgaBootstrapService.UnpackVerifiedOriginal(data),
                _ => []
            };
            if (image.Length == 0)
                return false;
            FrozenRunVgaRouteEvidence routes = Elvira1ProductionProfile.RunVgaRoutes;
            FrozenRunVgaRouteRecord record = routes.Route(name);
            int offset = record.OriginalBlockStart;
            int length = RoutedLength(routes, name);
            if (offset < 0 || length <= 0 || image.Length < offset + length)
                return false;
            byte[] span = image.AsSpan(offset, length).ToArray();
            int nul = Array.IndexOf(span, (byte)0x00);
            if (nul < 0)
                return false;
            for (int i = nul + 1; i < span.Length; i++)
                if (span[i] != 0x00)
                    return false;
            Encoding source = GamePcTextEditor.GetEncoding("CP852");
            Encoding strict = Encoding.GetEncoding(source.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            string decoded = strict.GetString(span, 0, nul);
            if (!string.Equals(decoded, strict.GetString(strict.GetBytes(decoded)), StringComparison.Ordinal))
                return false;
            text = decoded;
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidDataException or ArgumentException or EncoderFallbackException)
        {
            text = null;
            return false;
        }
    }

    private static int RoutedLength(FrozenRunVgaRouteEvidence routes, string name)
    {
        IReadOnlyList<FrozenRunVgaRouteRecord> list = routes.Routes;
        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].Name.Equals(name, StringComparison.Ordinal))
                continue;
            if (i + 1 < list.Count)
                return list[i + 1].OriginalBlockStart - list[i].OriginalBlockStart;
            return RunVgaVariableUiService.SpanLength(RuntimeUiLogicalRecordId.SaveOverwrite);
        }
        return 0;
    }
}
