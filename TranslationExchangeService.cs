using System.Security.Cryptography;
using System.Text;
using ClosedXML.Excel;

namespace ElviraVgaEditor;

internal sealed record TranslationExchangeMetadata(string Game, int StringCount, string OriginalSha256, string VariantName, string VariantCode, string TranslationFile)
{
    internal const string Format = "Pi1Translation";
    internal const int FormatVersion = 1;
}

internal sealed record TranslationExchangeRow(int Index, string Original, string Translation, int OriginalBytes, int TranslationBytes, int DeltaBytes, string Runtime);

internal sealed record TranslationExchangeDocument(TranslationExchangeMetadata Metadata, IReadOnlyList<TranslationExchangeRow> Rows);

/// <summary>Strict, logical-index based interchange for Text Editor translations.  It never writes GAMEPC files.</summary>
internal static class TranslationExchangeService
{
    private static readonly UTF8Encoding CsvEncoding = new(true);
    private static readonly Encoding StrictCp852 = Encoding.GetEncoding(852, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
    private static readonly string[] Headers = ["Index", "Original text", "Translation", "Original B", "Translation B", "Δ B", "Runtime"];

    internal static TranslationExchangeDocument Create(
        ElviraGameProfile game, string originalPath, IReadOnlyList<GamePcStringEntry> originals,
        IReadOnlyList<GamePcStringEntry> translations, IReadOnlyDictionary<int, string> edits,
        VariantEntry? variant, Func<int, string, string> runtime)
    {
        if (originals.Count != translations.Count || !originals.Select(e => e.Index).SequenceEqual(translations.Select(e => e.Index)))
            throw new InvalidDataException("Original and translation logical string indices do not match.");
        var rows = new List<TranslationExchangeRow>(originals.Count);
        for (int i = 0; i < originals.Count; i++)
        {
            string original = GamePcTextEditor.ToEditableText(originals[i], originals[i], StrictCp852, game);
            string translation = edits.TryGetValue(translations[i].Index, out string? edit) ? edit : GamePcTextEditor.ToEditableText(translations[i], originals[i], StrictCp852, game);
            int originalBytes = StrictCp852.GetByteCount(original), translationBytes = StrictCp852.GetByteCount(translation);
            rows.Add(new(translations[i].Index, original, translation, originalBytes, translationBytes, translationBytes - originalBytes, runtime(translations[i].Index, translation)));
        }
        string gameName = game switch { ElviraGameProfile.Elvira1 => "Elvira1", ElviraGameProfile.Elvira2 => "Elvira2", _ => throw new InvalidDataException("A supported Elvira game is required.") };
        return new(new(gameName, rows.Count, Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(originalPath))),
            variant?.DisplayName ?? string.Empty, variant?.Code ?? string.Empty, variant?.DataFile ?? string.Empty), rows);
    }

    internal static void ExportCsv(string path, TranslationExchangeDocument document)
    {
        var lines = new List<string>
        {
            "# " + TranslationExchangeMetadata.Format,
            "# FormatVersion=" + TranslationExchangeMetadata.FormatVersion,
            "# Game=" + document.Metadata.Game,
            "# StringCount=" + document.Metadata.StringCount,
            "# OriginalSource=GAMEPCO",
            "# OriginalSHA256=" + document.Metadata.OriginalSha256,
            "# VariantName=" + document.Metadata.VariantName,
            "# VariantCode=" + document.Metadata.VariantCode,
            "# TranslationFile=" + document.Metadata.TranslationFile,
            Csv(Headers)
        };
        lines.AddRange(document.Rows.Select(row => Csv([row.Index.ToString(), row.Original, row.Translation, row.OriginalBytes.ToString(), row.TranslationBytes.ToString(), row.DeltaBytes.ToString(), row.Runtime])));
        File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n", CsvEncoding);
    }

    internal static void ExportXlsx(string path, TranslationExchangeDocument document)
    {
        using var workbook = new XLWorkbook();
        IXLWorksheet metadata = workbook.AddWorksheet("Metadata");
        string[,] values =
        {
            { "Format", TranslationExchangeMetadata.Format }, { "FormatVersion", "1" }, { "Game", document.Metadata.Game }, { "StringCount", document.Metadata.StringCount.ToString() },
            { "OriginalSource", "GAMEPCO" }, { "OriginalSHA256", document.Metadata.OriginalSha256 }, { "VariantName", document.Metadata.VariantName }, { "VariantCode", document.Metadata.VariantCode }, { "TranslationFile", document.Metadata.TranslationFile }
        };
        for (int row = 0; row < values.GetLength(0); row++) { metadata.Cell(row + 1, 1).Value = values[row, 0]; metadata.Cell(row + 1, 2).Value = values[row, 1]; }
        metadata.Column(1).Width = 22; metadata.Column(2).Width = 72; metadata.Column(1).Style.Font.Bold = true;
        IXLWorksheet sheet = workbook.AddWorksheet("Translation");
        for (int column = 0; column < Headers.Length; column++) sheet.Cell(1, column + 1).Value = Headers[column];
        sheet.Row(1).Style.Font.Bold = true; sheet.SheetView.FreezeRows(1); sheet.Range(1, 1, 1, Headers.Length).SetAutoFilter();
        for (int row = 0; row < document.Rows.Count; row++)
        {
            TranslationExchangeRow value = document.Rows[row]; int r = row + 2;
            sheet.Cell(r, 1).Value = value.Index; SetText(sheet.Cell(r, 2), value.Original); SetText(sheet.Cell(r, 3), value.Translation);
            sheet.Cell(r, 4).Value = value.OriginalBytes; sheet.Cell(r, 5).Value = value.TranslationBytes; sheet.Cell(r, 6).Value = value.DeltaBytes; SetText(sheet.Cell(r, 7), value.Runtime);
        }
        sheet.Column(1).Width = 10; sheet.Column(2).Width = 52; sheet.Column(3).Width = 52; sheet.Column(4).Width = 12; sheet.Column(5).Width = 14; sheet.Column(6).Width = 10; sheet.Column(7).Width = 20;
        sheet.Columns(2, 3).Style.Alignment.WrapText = true; sheet.Column(2).Style.Fill.BackgroundColor = XLColor.LightGray; sheet.Column(3).Style.Fill.BackgroundColor = XLColor.LightYellow;
        workbook.SaveAs(path);
    }

    internal static IReadOnlyDictionary<int, string> Import(string path, ElviraGameProfile game, string originalPath, IReadOnlyList<GamePcStringEntry> originals)
    {
        return ReadAndValidate(path, game, originalPath, originals).Rows.ToDictionary(row => row.Index, row => row.Translation);
    }

    internal static TranslationExchangeDocument ReadAndValidate(string path, ElviraGameProfile game, string originalPath, IReadOnlyList<GamePcStringEntry> originals)
    {
        TranslationExchangeDocument document = Path.GetExtension(path).Equals(".xlsx", StringComparison.OrdinalIgnoreCase) ? ReadXlsx(path) : ReadCsv(path);
        Validate(document, game, originalPath, originals);
        return document;
    }

    private static void Validate(TranslationExchangeDocument document, ElviraGameProfile game, string originalPath, IReadOnlyList<GamePcStringEntry> originals)
    {
        string expectedGame = game == ElviraGameProfile.Elvira1 ? "Elvira1" : game == ElviraGameProfile.Elvira2 ? "Elvira2" : throw new InvalidDataException("A supported Elvira game is required.");
        if (!document.Metadata.Game.Equals(expectedGame, StringComparison.Ordinal) || document.Metadata.StringCount != originals.Count ||
            !document.Metadata.OriginalSha256.Equals(Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(originalPath))), StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Translation import does not match this game or GAMEPCO original source.");
        if (document.Rows.Count != originals.Count || document.Rows.Select(row => row.Index).Distinct().Count() != originals.Count || document.Rows.Any(row => row.Index < 0 || row.Index >= originals.Count))
            throw new InvalidDataException("Translation import indices must be complete, unique, and in range.");
        foreach (TranslationExchangeRow row in document.Rows)
        {
            if (!row.Original.Equals(GamePcTextEditor.ToEditableText(originals[row.Index], originals[row.Index], StrictCp852, game), StringComparison.Ordinal))
                throw new InvalidDataException($"Translation import original text differs at index {row.Index}.");
            try { _ = StrictCp852.GetBytes(row.Translation); }
            catch (EncoderFallbackException) { throw new InvalidDataException($"Translation at index {row.Index} cannot be encoded as CP852."); }
        }
    }

    private static TranslationExchangeDocument ReadCsv(string path)
    {
        string text = File.ReadAllText(path, new UTF8Encoding(true));
        List<string> lines = text.Replace("\r\n", "\n").Split('\n').ToList(); var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); int line = 0;
        while (line < lines.Count && lines[line].StartsWith("#")) { string value = lines[line++].TrimStart('#', ' '); int equals = value.IndexOf('='); if (equals >= 0) metadata[value[..equals]] = value[(equals + 1)..]; else metadata["Format"] = value; }
        string csv = string.Join("\n", lines.Skip(line)); List<string[]> records = ParseCsv(csv);
        if (records.Count == 0 || !records[0].SequenceEqual(Headers)) throw new InvalidDataException("Translation CSV header is invalid.");
        return ReadDocument(metadata, records.Skip(1));
    }

    private static TranslationExchangeDocument ReadXlsx(string path)
    {
        using var workbook = new XLWorkbook(path); IXLWorksheet metadataSheet = workbook.Worksheet("Metadata"), translationSheet = workbook.Worksheet("Translation");
        var metadata = metadataSheet.RowsUsed().ToDictionary(row => row.Cell(1).GetString(), row => row.Cell(2).GetString(), StringComparer.OrdinalIgnoreCase);
        string[] headers = Enumerable.Range(1, Headers.Length).Select(c => translationSheet.Cell(1, c).GetString()).ToArray();
        if (!headers.SequenceEqual(Headers)) throw new InvalidDataException("Translation XLSX header is invalid.");
        return ReadDocument(metadata, translationSheet.RowsUsed().Skip(1).Select(row => Enumerable.Range(1, Headers.Length).Select(c => row.Cell(c).GetString()).ToArray()));
    }

    private static TranslationExchangeDocument ReadDocument(IReadOnlyDictionary<string, string> metadata, IEnumerable<string[]> rows)
    {
        if (!metadata.TryGetValue("Format", out string? format) || format != TranslationExchangeMetadata.Format || !metadata.TryGetValue("FormatVersion", out string? version) || version != "1" ||
            !metadata.TryGetValue("OriginalSource", out string? source) || source != "GAMEPCO" || !metadata.TryGetValue("Game", out string? game) || !metadata.TryGetValue("StringCount", out string? count) || !int.TryParse(count, out int stringCount) || !metadata.TryGetValue("OriginalSHA256", out string? hash))
            throw new InvalidDataException("Translation import metadata is invalid or unsupported.");
        var values = new List<TranslationExchangeRow>();
        foreach (string[] row in rows)
        {
            if (row.Length != Headers.Length || !int.TryParse(row[0], out int index)) throw new InvalidDataException("Translation import row has an invalid index.");
            values.Add(new(index, row[1], row[2], 0, 0, 0, row[6]));
        }
        return new(new(game, stringCount, hash, metadata.GetValueOrDefault("VariantName", ""), metadata.GetValueOrDefault("VariantCode", ""), metadata.GetValueOrDefault("TranslationFile", "")), values);
    }

    // ClosedXML writes a string through Value as a shared-string literal; formulas require FormulaA1.
    private static void SetText(IXLCell cell, string value) => cell.Value = value;
    private static string Csv(IEnumerable<string> fields) => string.Join(',', fields.Select(value => '"' + value.Replace("\"", "\"\"") + '"'));
    private static List<string[]> ParseCsv(string text)
    {
        var rows = new List<string[]>(); var row = new List<string>(); var field = new StringBuilder(); bool quote = false;
        for (int i = 0; i < text.Length; i++) { char c = text[i]; if (quote) { if (c == '"' && i + 1 < text.Length && text[i + 1] == '"') { field.Append(c); i++; } else if (c == '"') quote = false; else field.Append(c); } else if (c == '"') quote = true; else if (c == ',') { row.Add(field.ToString()); field.Clear(); } else if (c == '\r') { } else if (c == '\n') { row.Add(field.ToString()); field.Clear(); rows.Add(row.ToArray()); row.Clear(); } else field.Append(c); }
        if (quote) throw new InvalidDataException("Translation CSV contains an unterminated quoted field.");
        if (field.Length > 0 || row.Count > 0) { row.Add(field.ToString()); rows.Add(row.ToArray()); }
        return rows;
    }
}
