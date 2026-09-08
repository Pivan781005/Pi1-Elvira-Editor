using System.Diagnostics;
using System.Drawing.Text;
using System.Security.Cryptography;
using System.Text;

namespace Pi1ElviraEditor;

internal sealed record VectorFontRasterOptions(string FamilyName, float PixelSize, int XOffset, int YOffset, bool ComposeDiacritics);

internal static class VectorFontRasterizer
{
    public const int GlyphCount = 256;
    public const int GlyphBytes = 8;
    public const int FontBytes = GlyphCount * GlyphBytes;

    public static IReadOnlyList<string> GetFamilyNames(string sourcePath)
    {
        using var prepared = PrepareFontFile(sourcePath);
        using var fonts = new PrivateFontCollection();
        fonts.AddFontFile(prepared.Path);
        return fonts.Families.Select(f => f.Name).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToArray();
    }

    public static byte[] RasterizeCp852(string sourcePath, VectorFontRasterOptions options)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding cp852 = Encoding.GetEncoding(852, EncoderFallback.ReplacementFallback, DecoderFallback.ReplacementFallback);

        using var prepared = PrepareFontFile(sourcePath);
        using var fonts = new PrivateFontCollection();
        fonts.AddFontFile(prepared.Path);
        FontFamily? family = fonts.Families.FirstOrDefault(f => f.Name.Equals(options.FamilyName, StringComparison.OrdinalIgnoreCase))
            ?? fonts.Families.FirstOrDefault();
        if (family is null)
            throw new InvalidDataException("No usable font family was found in the selected font file.");

        using var font = new Font(family, options.PixelSize, FontStyle.Regular, GraphicsUnit.Pixel);
        var result = new byte[FontBytes];
        using var format = (StringFormat)StringFormat.GenericTypographic.Clone();
        format.FormatFlags |= StringFormatFlags.NoClip | StringFormatFlags.MeasureTrailingSpaces;

        for (int code = 0; code < GlyphCount; code++)
        {
            // DOS control slots are deliberately blank. Printable CP852 bytes are mapped through Unicode.
            if (code < 0x20 || code == 0x7F)
                continue;

            string text = cp852.GetString(new[] { (byte)code });
            if (string.IsNullOrEmpty(text) || char.IsControl(text[0]))
                continue;

            using var bitmap = new Bitmap(8, 8);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.None;
                g.DrawString(text, font, Brushes.Black, new PointF(options.XOffset, options.YOffset), format);
            }

            for (int row = 0; row < 8; row++)
            {
                byte packed = 0;
                for (int col = 0; col < 8; col++)
                {
                    Color px = bitmap.GetPixel(col, row);
                    bool on = px.R < 128 || px.G < 128 || px.B < 128;
                    if (on)
                        packed |= (byte)(1 << (7 - col));
                }
                result[code * GlyphBytes + row] = packed;
            }
        }

        if (options.ComposeDiacritics)
        {
            NormalizeLatinBaselines(result);
            ComposeCentralEuropeanDiacritics(result, cp852);
        }

        return result;
    }

    private enum AccentKind { Acute, Caron, Circumflex, Diaeresis, Ring, SpecialRightCaron }

    private static void ComposeCentralEuropeanDiacritics(byte[] result, Encoding cp852)
    {
        // At 6x8, precomposed vector glyphs often shrink/deform the base letter.  Build the
        // common Slovak/Czech CP852 letters from the already rasterized ASCII base instead.
        var map = new Dictionary<char, (char Base, AccentKind Accent)>
        {
            ['á']=('a',AccentKind.Acute), ['Á']=('A',AccentKind.Acute),
            ['ä']=('a',AccentKind.Diaeresis), ['Ä']=('A',AccentKind.Diaeresis),
            ['č']=('c',AccentKind.Caron), ['Č']=('C',AccentKind.Caron),
            ['ď']=('d',AccentKind.SpecialRightCaron), ['Ď']=('D',AccentKind.SpecialRightCaron),
            ['é']=('e',AccentKind.Acute), ['É']=('E',AccentKind.Acute),
            ['í']=('i',AccentKind.Acute), ['Í']=('I',AccentKind.Acute),
            ['ĺ']=('l',AccentKind.Acute), ['Ĺ']=('L',AccentKind.Acute),
            ['ľ']=('l',AccentKind.SpecialRightCaron), ['Ľ']=('L',AccentKind.SpecialRightCaron),
            ['ň']=('n',AccentKind.Caron), ['Ň']=('N',AccentKind.Caron),
            ['ó']=('o',AccentKind.Acute), ['Ó']=('O',AccentKind.Acute),
            ['ô']=('o',AccentKind.Circumflex), ['Ô']=('O',AccentKind.Circumflex),
            ['ŕ']=('r',AccentKind.Acute), ['Ŕ']=('R',AccentKind.Acute),
            ['š']=('s',AccentKind.Caron), ['Š']=('S',AccentKind.Caron),
            ['ť']=('t',AccentKind.SpecialRightCaron), ['Ť']=('T',AccentKind.SpecialRightCaron),
            ['ú']=('u',AccentKind.Acute), ['Ú']=('U',AccentKind.Acute),
            ['ů']=('u',AccentKind.Ring), ['Ů']=('U',AccentKind.Ring),
            ['ý']=('y',AccentKind.Acute), ['Ý']=('Y',AccentKind.Acute),
            ['ž']=('z',AccentKind.Caron), ['Ž']=('Z',AccentKind.Caron),
            ['ě']=('e',AccentKind.Caron), ['Ě']=('E',AccentKind.Caron),
            ['ř']=('r',AccentKind.Caron), ['Ř']=('R',AccentKind.Caron)
        };

        foreach (var item in map)
        {
            byte[] encoded;
            try { encoded = cp852.GetBytes(item.Key.ToString()); }
            catch { continue; }
            if (encoded.Length != 1) continue;
            int dst = encoded[0] * GlyphBytes;
            int src = (byte)item.Value.Base * GlyphBytes;
            Span<byte> glyph = result.AsSpan(dst, GlyphBytes);
            result.AsSpan(src, GlyphBytes).CopyTo(glyph);
            AddAccent(glyph, item.Key, item.Value.Base, item.Value.Accent);
        }
    }

    private static void AddAccent(Span<byte> glyph, char target, char baseChar, AccentKind accent)
    {
        // Renderer uses only bits 7..2 = six visible columns.  Placement is derived from the
        // actual 6x8 bitmap bounds of the rasterized base glyph instead of fixed columns.
        // This keeps narrow letters (i/l/r) and wide letters (A/M/W) visually balanced.
        if (accent == AccentKind.SpecialRightCaron)
        {
            ComposeRightCaron(glyph, target, baseChar);
            return;
        }

        int accentRows = accent == AccentKind.Diaeresis ? 1 : 2;
        PrepareTopAccentRoomBaselineAware(glyph, accentRows, baseChar);
        var b = GetBounds(glyph);
        if (!b.HasPixels) return;

        int center = Math.Clamp((b.MinCol + b.MaxCol) / 2, 0, 5);
        int left = Math.Max(0, center - 1);
        int right = Math.Min(5, center + 1);

        switch (accent)
        {
            case AccentKind.Acute:
                // Rising stroke, optically centered over the base glyph.
                glyph[0] |= Pixel(Math.Min(5, center + 1));
                glyph[1] |= Pixel(center);
                break;
            case AccentKind.Diaeresis:
                // Keep the dots symmetric around the actual glyph center.
                int dotL = Math.Max(0, center - 1);
                int dotR = Math.Min(5, center + 1);
                if (dotL == dotR) dotR = Math.Min(5, dotL + 1);
                glyph[0] |= (byte)(Pixel(dotL) | Pixel(dotR));
                break;
            case AccentKind.Caron:
                glyph[0] |= (byte)(Pixel(left) | Pixel(right));
                glyph[1] |= Pixel(center);
                break;
            case AccentKind.Circumflex:
                glyph[0] |= Pixel(center);
                glyph[1] |= (byte)(Pixel(left) | Pixel(right));
                break;
            case AccentKind.Ring:
                // A compact 2x2 ring is the most legible form at 6x8.
                int ringL = Math.Max(0, center - 1);
                int ringR = Math.Min(5, center);
                glyph[0] |= (byte)(Pixel(ringL) | Pixel(ringR));
                glyph[1] |= (byte)(Pixel(ringL) | Pixel(ringR));
                break;
        }
    }

    private readonly record struct GlyphBounds(bool HasPixels, int MinRow, int MaxRow, int MinCol, int MaxCol)
    {
        public int Height => HasPixels ? MaxRow - MinRow + 1 : 0;
        public int Width => HasPixels ? MaxCol - MinCol + 1 : 0;
    }

    private static GlyphBounds GetBounds(ReadOnlySpan<byte> glyph)
    {
        int minR = 8, maxR = -1, minC = 6, maxC = -1;
        for (int r = 0; r < 8; r++)
        {
            byte row = (byte)(glyph[r] & 0xFC);
            if (row == 0) continue;
            minR = Math.Min(minR, r); maxR = Math.Max(maxR, r);
            for (int c = 0; c < 6; c++)
            {
                if ((row & Pixel(c)) == 0) continue;
                minC = Math.Min(minC, c); maxC = Math.Max(maxC, c);
            }
        }
        return maxR < 0 ? new GlyphBounds(false, 0, 0, 0, 0) : new GlyphBounds(true, minR, maxR, minC, maxC);
    }

    private static readonly HashSet<char> Descenders = new("gjpqy".ToCharArray());

    private static void NormalizeLatinBaselines(byte[] result)
    {
        // Keep the font on a common writing line before any accent is added.  The previous
        // optical-only algorithm treated each glyph independently, which made letters jump
        // vertically.  In a bitmap font the baseline is a font-wide metric, not a per-glyph
        // optimization target.
        for (int code = 0x30; code <= 0x7A; code++)
        {
            char ch = (char)code;
            bool isLetter = (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
            bool isDigit = ch >= '0' && ch <= '9';
            if (!isLetter && !isDigit)
                continue;

            Span<byte> glyph = result.AsSpan(code * GlyphBytes, GlyphBytes);
            NormalizeGlyphToClassBaseline(glyph, ch);
        }
    }

    private static void NormalizeGlyphToClassBaseline(Span<byte> glyph, char ch)
    {
        var b = GetBounds(glyph);
        if (!b.HasPixels) return;

        // Row 6 is the common baseline row for normal capitals, lowercase letters and digits.
        // True descenders may use row 7. This leaves row 7 blank for most letters, matching the
        // visual rhythm of classic DOS 6x8 fonts and giving accents room without random jumps.
        int targetBottom = Descenders.Contains(ch) ? 7 : 6;
        int shift = targetBottom - b.MaxRow;
        ShiftGlyphVertical(glyph, shift);
    }

    private static void PrepareTopAccentRoomBaselineAware(Span<byte> glyph, int rowsNeeded, char baseChar)
    {
        var b = GetBounds(glyph);
        if (!b.HasPixels) return;

        int targetBottom = Descenders.Contains(baseChar) ? 7 : 6;
        int topAllowed = rowsNeeded;
        int targetHeight = targetBottom - topAllowed + 1;
        if (targetHeight <= 0) return;

        // First put the base letter on its class baseline. This is intentionally global and
        // deterministic: accented and non-accented forms of the same letter must sit on the
        // same writing line.
        ShiftGlyphVertical(glyph, targetBottom - b.MaxRow);
        b = GetBounds(glyph);
        if (!b.HasPixels) return;

        // If there is already enough room above the glyph, preserve its exact raster shape.
        if (b.MinRow >= topAllowed && b.MaxRow == targetBottom)
            return;

        Span<byte> source = stackalloc byte[8];
        glyph.CopyTo(source);
        glyph.Clear();

        // Preserve the baseline while fitting the occupied body into the rows below the accent.
        // If compression is necessary, resample only vertically; horizontal strokes remain
        // untouched. The bottom row always lands on targetBottom.
        int sourceHeight = b.Height;
        int outHeight = Math.Min(sourceHeight, targetHeight);
        int outTop = targetBottom - outHeight + 1;
        outTop = Math.Max(outTop, topAllowed);

        for (int dst = 0; dst < outHeight; dst++)
        {
            int srcRel = outHeight == 1 ? 0 : (int)Math.Round(dst * (sourceHeight - 1) / (double)(outHeight - 1));
            int srcRow = b.MinRow + srcRel;
            int dstRow = outTop + dst;
            if (dstRow >= 0 && dstRow < 8)
                glyph[dstRow] = source[srcRow];
        }
    }

    private static void ShiftGlyphVertical(Span<byte> glyph, int delta)
    {
        if (delta == 0) return;
        Span<byte> src = stackalloc byte[8];
        glyph.CopyTo(src);
        glyph.Clear();
        for (int r = 0; r < 8; r++)
        {
            int dst = r + delta;
            if (dst >= 0 && dst < 8)
                glyph[dst] = src[r];
        }
    }

    private static void ComposeRightCaron(Span<byte> glyph, char target, char baseChar)
    {
        var b = GetBounds(glyph);
        if (!b.HasPixels) return;

        // ď/ľ/ť and Ď/Ľ/Ť use a right-side apostrophe-like caron. Keep the full base letter
        // whenever possible; only shift it left when there is literally no right-side cell.
        if (b.MaxCol >= 5 && b.MinCol > 0)
        {
            ShiftGlyphLeft(glyph, 1);
            b = GetBounds(glyph);
        }

        int accentCol = Math.Min(5, b.MaxCol + 1);
        int upperCol = Math.Max(0, accentCol - 1);
        bool upper = char.IsUpper(target);

        // Lowercase ľ benefits from a tighter, almost vertical apostrophe. ď/ť use a more
        // diagonal form. Uppercase forms are kept slightly higher and shorter.
        if (char.ToLowerInvariant(baseChar) == 'l')
        {
            glyph[0] |= Pixel(accentCol);
            glyph[1] |= Pixel(accentCol);
        }
        else if (upper)
        {
            glyph[0] |= Pixel(accentCol);
            glyph[1] |= Pixel(upperCol);
        }
        else
        {
            glyph[0] |= Pixel(accentCol);
            glyph[1] |= Pixel(upperCol);
        }
    }

    private static void ShiftGlyphLeft(Span<byte> glyph, int count)
    {
        count = Math.Clamp(count, 0, 5);
        if (count == 0) return;
        for (int r = 0; r < 8; r++)
            glyph[r] = (byte)(glyph[r] << count);
    }

    private static byte Pixel(int col) => (byte)(1 << (7 - Math.Clamp(col, 0, 5)));

    public static string Describe(byte[] bytes, string sourcePath, VectorFontRasterOptions options)
    {
        string hash = Convert.ToHexString(SHA256.HashData(bytes));
        return $"Rasterized {Path.GetFileName(sourcePath)} → CP852 8×8 stored / 6×8 game-visible | family {options.FamilyName} | {options.PixelSize:0.##} px | offset X {options.XOffset}, Y {options.YOffset} | diacritics {(options.ComposeDiacritics ? "composed/baseline-aware" : "direct")} | 256 glyphs / 2048 bytes | SHA-256 {hash}";
    }

    private static PreparedFontFile PrepareFontFile(string sourcePath)
    {
        if (!File.Exists(sourcePath))
            throw new FileNotFoundException("Vector font file was not found.", sourcePath);

        string ext = Path.GetExtension(sourcePath);
        if (!ext.Equals(".sfd", StringComparison.OrdinalIgnoreCase))
            return new PreparedFontFile(sourcePath, null);

        string? fontForge = FindFontForge();
        if (fontForge is null)
            throw new InvalidOperationException(
                "Importing .SFD files requires FontForge. Install FontForge or convert the SFD to TTF/OTF first. " +
                "The editor automatically uses FontForge when fontforge.exe is available in PATH or a standard FontForge installation folder.");

        string tempDir = Path.Combine(Path.GetTempPath(), "Pi1ElviraFontImport");
        Directory.CreateDirectory(tempDir);
        string tempTtf = Path.Combine(tempDir, $"{Path.GetFileNameWithoutExtension(sourcePath)}_{Guid.NewGuid():N}.ttf");

        var psi = new ProcessStartInfo
        {
            FileName = fontForge,
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };
        psi.ArgumentList.Add("-lang=ff");
        psi.ArgumentList.Add("-c");
        psi.ArgumentList.Add("Open($1); Generate($2)");
        psi.ArgumentList.Add(sourcePath);
        psi.ArgumentList.Add(tempTtf);

        using Process? p = Process.Start(psi);
        if (p is null)
            throw new InvalidOperationException("FontForge could not be started.");
        string stdout = p.StandardOutput.ReadToEnd();
        string stderr = p.StandardError.ReadToEnd();
        p.WaitForExit();
        if (p.ExitCode != 0 || !File.Exists(tempTtf))
            throw new InvalidOperationException($"FontForge could not convert the SFD file. Exit code {p.ExitCode}.\n{stderr}\n{stdout}");

        return new PreparedFontFile(tempTtf, tempTtf);
    }

    private static string? FindFontForge()
    {
        string[] names = { "fontforge.exe", "fontforge-console.exe" };
        string? path = Environment.GetEnvironmentVariable("PATH");
        if (!string.IsNullOrWhiteSpace(path))
        {
            foreach (string dir in path.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                foreach (string name in names)
                {
                    string candidate = Path.Combine(dir, name);
                    if (File.Exists(candidate)) return candidate;
                }
        }

        string[] roots =
        {
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
        };
        foreach (string root in roots.Where(Directory.Exists))
        {
            foreach (string candidate in new[]
            {
                Path.Combine(root, "FontForgeBuilds", "bin", "fontforge.exe"),
                Path.Combine(root, "FontForgeBuilds", "bin", "fontforge-console.exe"),
                Path.Combine(root, "FontForge", "bin", "fontforge.exe"),
                Path.Combine(root, "FontForge", "bin", "fontforge-console.exe")
            })
                if (File.Exists(candidate)) return candidate;
        }
        return null;
    }

    private sealed class PreparedFontFile : IDisposable
    {
        public string Path { get; }
        private readonly string? _temporaryPath;

        public PreparedFontFile(string path, string? temporaryPath)
        {
            Path = path;
            _temporaryPath = temporaryPath;
        }

        public void Dispose()
        {
            if (_temporaryPath is null) return;
            try { File.Delete(_temporaryPath); } catch { }
        }
    }
}

internal sealed class VectorFontImportDialog : Form
{
    private readonly ComboBox _family = new();
    private readonly ComboBox _preset = new();
    private bool _applyingPreset;
    private readonly NumericUpDown _size = new();
    private readonly NumericUpDown _x = new();
    private readonly NumericUpDown _y = new();
    private readonly Label _info = new();
    private readonly CheckBox _compose = new();
    private readonly VectorFontSampleControl _sample = new();
    private readonly string _sourcePath;

    public VectorFontRasterOptions Options => new(
        _family.SelectedItem?.ToString() ?? string.Empty,
        (float)_size.Value,
        (int)_x.Value,
        (int)_y.Value,
        _compose.Checked);

    public VectorFontImportDialog(string sourcePath)
    {
        _sourcePath = sourcePath;
        Text = UiText.Get("VectorTitle");
        Width = 650;
        Height = 500;
        MinimumSize = new Size(620, 460);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;

        BuildUi();
        LoadFamilies();
        UpdatePreview();
    }

    private void BuildUi()
    {
        var top = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 212,
            ColumnCount = 2,
            RowCount = 7,
            Padding = new Padding(10)
        };
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        top.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        AddRow(top, 0, UiText.Get("VectorSource"), new Label { Text = Path.GetFileName(_sourcePath), AutoEllipsis = true, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft });

        _preset.DropDownStyle = ComboBoxStyle.DropDownList;
        _preset.Items.AddRange(new object[] { UiText.Get("PresetDefault"), UiText.Get("PresetLexis"), UiText.Get("PresetPixelOperator"), UiText.Get("PresetHpStyle") });
        _preset.SelectedIndex = 0;
        _preset.SelectedIndexChanged += (_, _) => ApplyPreset();
        AddRow(top, 1, UiText.Get("VectorPreset"), _preset);

        _family.DropDownStyle = ComboBoxStyle.DropDownList;
        _family.Dock = DockStyle.Fill;
        _family.SelectedIndexChanged += (_, _) => UpdatePreview();
        AddRow(top, 2, UiText.Get("VectorFamily"), _family);

        _size.Minimum = 4; _size.Maximum = 16; _size.DecimalPlaces = 1; _size.Increment = 0.5M; _size.Value = 8; _size.Width = 90;
        _size.ValueChanged += (_, _) => { if (!_applyingPreset) UpdatePreview(); };
        AddRow(top, 3, UiText.Get("VectorPixelSize"), _size);

        _x.Minimum = -2; _x.Maximum = 2; _x.Value = 1; _x.Width = 90;
        _x.ValueChanged += (_, _) => { if (!_applyingPreset) UpdatePreview(); };
        AddRow(top, 4, UiText.Get("VectorXOffset"), _x);

        _y.Minimum = -2; _y.Maximum = 2; _y.Value = 0; _y.Width = 90;
        _y.ValueChanged += (_, _) => { if (!_applyingPreset) UpdatePreview(); };
        AddRow(top, 5, UiText.Get("VectorYOffset"), _y);

        _compose.Text = UiText.Get("VectorCompose");
        _compose.Checked = true;
        _compose.AutoSize = true;
        _compose.CheckedChanged += (_, _) => { if (!_applyingPreset) UpdatePreview(); };
        AddRow(top, 6, UiText.Get("VectorDiacritics"), _compose);

        _info.Dock = DockStyle.Top;
        _info.Height = 70;
        _info.Padding = new Padding(10, 4, 10, 4);
        _info.Text = UiText.Get("VectorInfo");

        _sample.Dock = DockStyle.Fill;

        var bottom = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 52,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(8)
        };
        var ok = new Button { Text = UiText.Get("VectorImportEdited"), Width = 140, Height = 32, DialogResult = DialogResult.OK };
        var cancel = new Button { Text = UiText.Get("Cancel"), Width = 90, Height = 32, DialogResult = DialogResult.Cancel };
        bottom.Controls.Add(ok);
        bottom.Controls.Add(cancel);
        AcceptButton = ok;
        CancelButton = cancel;

        Controls.Add(_sample);
        Controls.Add(_info);
        Controls.Add(top);
        Controls.Add(bottom);
    }

    private void ApplyPreset()
    {
        if (_preset.SelectedIndex < 0) return;
        _applyingPreset = true;
        try
        {
            switch (_preset.SelectedIndex)
            {
                case 1: // Lexis
                    _size.Value = 8.0M; _x.Value = 1; _y.Value = 0; _compose.Checked = true;
                    break;
                case 2: // Pixel Operator
                    _size.Value = 8.0M; _x.Value = 1; _y.Value = 0; _compose.Checked = true;
                    break;
                case 3: // HP-style bitmap-like alignment
                    _size.Value = 8.0M; _x.Value = 0; _y.Value = 0; _compose.Checked = true;
                    break;
                default:
                    _size.Value = 8.0M; _x.Value = 1; _y.Value = 0; _compose.Checked = true;
                    break;
            }
        }
        finally { _applyingPreset = false; }
        UpdatePreview();
    }

    private static void AddRow(TableLayoutPanel table, int row, string label, Control control)
    {
        var l = new Label { Text = label, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        table.Controls.Add(l, 0, row);
        table.Controls.Add(control, 1, row);
    }

    private void LoadFamilies()
    {
        try
        {
            IReadOnlyList<string> families = VectorFontRasterizer.GetFamilyNames(_sourcePath);
            _family.Items.Clear();
            foreach (string family in families) _family.Items.Add(family);
            if (_family.Items.Count > 0) _family.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("VectorFontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            BeginInvoke(new Action(Close));
        }
    }

    private void UpdatePreview()
    {
        if (_family.SelectedItem is null) return;
        try
        {
            byte[] bytes = VectorFontRasterizer.RasterizeCp852(_sourcePath, Options);
            _sample.SetFont(bytes);
        }
        catch
        {
            _sample.SetFont(null);
        }
    }
}

internal sealed class VectorFontSampleControl : Control
{
    private byte[]? _fontBytes;

    public VectorFontSampleControl()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
    }

    public void SetFont(byte[]? bytes)
    {
        _fontBytes = bytes;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(BackColor);
        if (_fontBytes is null || _fontBytes.Length != VectorFontRasterizer.FontBytes)
            return;

        string[] lines =
        {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "abcdefghijklmnopqrstuvwxyz",
            "0123456789 !?\"'.,-:;()",
            "ÁÄČĎÉÍĹĽŇÓÔŔŠŤÚÝŽ",
            "áäčďéíĺľňóôŕšťúýž"
        };

        int y = 12;
        foreach (string line in lines)
        {
            DrawText(e.Graphics, line, 12, y, 2);
            y += 24;
        }
        using var labelFont = new Font(Font.FontFamily, 9f);
        e.Graphics.DrawString("Preview 2× — stored 8×8. Last two columns are preserved but may be invisible in Elvira.", labelFont, Brushes.DimGray, 12, y + 8);
    }

    private void DrawText(Graphics g, string text, int x, int y, int scale)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        byte[] codes = Encoding.GetEncoding(852).GetBytes(text);
        foreach (byte code in codes)
        {
            int baseOffset = code * 8;
            for (int row = 0; row < 8; row++)
            {
                byte packed = _fontBytes![baseOffset + row];
                for (int col = 0; col < 8; col++)
                {
                    bool on = (packed & (1 << (7 - col))) != 0;
                    if (col >= 6)
                        g.FillRectangle(Brushes.Gainsboro, x + col * scale, y + row * scale, scale, scale);
                    if (on)
                        g.FillRectangle(col >= 6 ? Brushes.DimGray : Brushes.Black, x + col * scale, y + row * scale, scale, scale);
                }
            }
            x += 8 * scale;
        }
    }
}
