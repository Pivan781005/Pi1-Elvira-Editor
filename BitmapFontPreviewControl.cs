using System.Text;

namespace Pi1ElviraEditor;

internal enum FontPreviewMode
{
    Original,
    Edited,
    SideBySide
}

internal sealed class BitmapFontPreviewControl : Control
{
    private IReadOnlyList<GlyphModel>? _glyphs;
    private int _selectedCode = 65;
    private FontPreviewMode _mode = FontPreviewMode.SideBySide;

    public BitmapFontPreviewControl()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        BackColor = SystemColors.Control;
        TabStop = false;
    }

    public void SetPreview(IReadOnlyList<GlyphModel>? glyphs, int selectedCode, FontPreviewMode mode)
    {
        _glyphs = glyphs;
        _selectedCode = Math.Clamp(selectedCode, 0, 255);
        _mode = mode;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(BackColor);
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

        using var titleFont = new Font(Font, FontStyle.Bold);
        using var smallFont = new Font(Font.FontFamily, 8.5f);
        using var borderPen = new Pen(Color.DarkGray);
        using var onBrush = new SolidBrush(Color.Black);
        using var offBrush = new SolidBrush(Color.White);

        int y = 8;
        e.Graphics.DrawString(UiText.Get("RealBitmapPreview"), titleFont, SystemBrushes.ControlText, 8, y);
        y += 27;

        if (_glyphs is null || _glyphs.Count < 256)
        {
            e.Graphics.DrawString(UiText.Get("LoadFontPreview"), Font, SystemBrushes.ControlText, 8, y);
            return;
        }

        GlyphModel glyph = _glyphs[_selectedCode];
        string slot = GlyphRepository.GetSlotDisplayText(_selectedCode);
        e.Graphics.DrawString($"{UiText.Get("Selected")}: 0x{_selectedCode:X2}  {slot}", Font, SystemBrushes.ControlText, 8, y);
        y += 26;

        byte[] primary = _mode == FontPreviewMode.Original ? glyph.Original : (glyph.HasEdited ? glyph.Edited : new byte[8]);
        e.Graphics.DrawString(_mode == FontPreviewMode.Original ? UiText.Get("Original") : UiText.Get("Edited"), smallFont, SystemBrushes.ControlText, 8, y);
        y += 18;

        int x = 8;
        foreach (int scale in new[] { 1, 2, 4, 8 })
        {
            e.Graphics.DrawString($"{scale}×", smallFont, SystemBrushes.ControlText, x, y);
            var glyphRect = new Rectangle(x, y + 18, 6 * scale, 8 * scale);
            DrawGlyph(e.Graphics, primary, glyphRect.Location, scale, onBrush, offBrush);
            e.Graphics.DrawRectangle(borderPen, glyphRect.X - 1, glyphRect.Y - 1, glyphRect.Width + 1, glyphRect.Height + 1);
            x += Math.Max(54, 6 * scale + 24);
        }
        y += 96;

        if (_mode == FontPreviewMode.SideBySide)
        {
            e.Graphics.DrawString(UiText.Get("OriginalVsEdited"), titleFont, SystemBrushes.ControlText, 8, y);
            y += 22;
            e.Graphics.DrawString(UiText.Get("Original"), smallFont, SystemBrushes.ControlText, 8, y + 16);
            DrawGlyph(e.Graphics, glyph.Original, new Point(74, y), 4, onBrush, offBrush);
            e.Graphics.DrawString(UiText.Get("Edited"), smallFont, SystemBrushes.ControlText, 122, y + 16);
            DrawGlyph(e.Graphics, glyph.HasEdited ? glyph.Edited : new byte[8], new Point(172, y), 4, onBrush, offBrush);
            y += 42;
        }

        y += 8;
        e.Graphics.DrawString(UiText.Get("FontInContext"), titleFont, SystemBrushes.ControlText, 8, y);
        y += 26;

        string[] samples =
        {
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "abcdefghijklmnopqrstuvwxyz",
            "0123456789 !?\"'.,-:;()",
            "ÁÄČĎÉÍĹĽŇÓÔŔŠŤÚÝŽ",
            "áäčďéíĺľňóôŕšťúýž"
        };

        if (_mode == FontPreviewMode.SideBySide)
        {
            e.Graphics.DrawString(UiText.Get("Original").ToUpperInvariant(), smallFont, SystemBrushes.ControlText, 8, y);
            y += 16;
            y = DrawSampleBlock(e.Graphics, samples, y, edited: false, onBrush, offBrush);
            y += 8;
            e.Graphics.DrawString(UiText.Get("EditedCaps"), smallFont, SystemBrushes.ControlText, 8, y);
            y += 16;
            y = DrawSampleBlock(e.Graphics, samples, y, edited: true, onBrush, offBrush);
        }
        else
        {
            bool edited = _mode == FontPreviewMode.Edited;
            y = DrawSampleBlock(e.Graphics, samples, y, edited, onBrush, offBrush);
        }

        y += 8;
        e.Graphics.DrawString(UiText.Get("PixelNote"), smallFont, SystemBrushes.ControlText, 8, y);
    }

    private int DrawSampleBlock(Graphics g, IEnumerable<string> lines, int y, bool edited, Brush onBrush, Brush offBrush)
    {
        const int contextScale = 2;
        foreach (string line in lines)
        {
            DrawBitmapText(g, line, new Point(8, y), contextScale, edited, onBrush, offBrush);
            y += 20;
        }
        return y;
    }

    private void DrawBitmapText(Graphics g, string text, Point origin, int scale, bool edited, Brush onBrush, Brush offBrush)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Encoding enc = Encoding.GetEncoding(852, EncoderFallback.ReplacementFallback, DecoderFallback.ReplacementFallback);
        byte[] codes = enc.GetBytes(text);
        int x = origin.X;
        foreach (byte code in codes)
        {
            if (_glyphs is null || code >= _glyphs.Count) continue;
            GlyphModel glyph = _glyphs[code];
            byte[] rows = edited ? (glyph.HasEdited ? glyph.Edited : new byte[8]) : glyph.Original;
            DrawGlyph(g, rows, new Point(x, origin.Y), scale, onBrush, offBrush);
            x += 6 * scale;
        }
    }

    private static void DrawGlyph(Graphics g, byte[] rows, Point origin, int scale, Brush onBrush, Brush offBrush)
    {
        if (rows.Length != 8) return;
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 6; col++)
            {
                bool on = (rows[row] & (1 << (7 - col))) != 0;
                var rect = new Rectangle(origin.X + col * scale, origin.Y + row * scale, scale, scale);
                g.FillRectangle(on ? onBrush : offBrush, rect);
            }
        }
    }
}
