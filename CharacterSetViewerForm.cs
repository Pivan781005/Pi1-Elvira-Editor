namespace Pi1ElviraEditor;

internal sealed class CharacterSetViewerForm : Form
{
    private readonly IReadOnlyList<GlyphModel> _glyphs;
    private readonly CharacterSetGridControl _grid;
    private readonly TrackBar _zoom = new();
    private readonly Label _zoomLabel = new();
    private readonly ComboBox _mode = new();
    private readonly ComboBox _layout = new();
    private readonly Label _title = new();
    private readonly Label _layoutText = new();
    private readonly Label _zoomText = new();
    private readonly Label _hint = new();
    private readonly Label _warning = new();
    private readonly Label _selection = new();
    private readonly Label _status = new();
    private readonly Action<int>? _selectionChanged;
    internal string LocalizedTitleForTest => Text;
    internal string LocalizedHintForTest => _hint.Text;

    public CharacterSetViewerForm(IReadOnlyList<GlyphModel> glyphs, int selectedCode, FontPreviewMode initialMode, Action<int>? selectionChanged = null)
    {
        _glyphs = glyphs;
        _selectionChanged = selectionChanged;
        AutoScaleMode = AutoScaleMode.Dpi;
        Text = UiText.Get("FullSetTitle");
        Width = 1500;
        Height = 720;
        MinimumSize = new Size(900, 520);
        StartPosition = FormStartPosition.CenterParent;

        var top = new Panel { Dock = DockStyle.Top, Height = 82, Padding = new Padding(10, 8, 10, 4) };
        var topRow = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 30,
            ColumnCount = 7,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _title = new Label
        {
            Text = UiText.Get("FullSetHeading"),
            Font = new Font(Font, FontStyle.Bold),
            AutoSize = true,
            Anchor = AnchorStyles.Left
        };

        _grid = new CharacterSetGridControl
        {
            Dock = DockStyle.Fill,
            Glyphs = _glyphs,
            Zoom = 4,
            Columns = 32,
            ShowEdited = initialMode != FontPreviewMode.Original,
            SelectedCode = Math.Clamp(selectedCode, 0, 255)
        };
        _grid.SelectedCodeChanged += code =>
        {
            UpdateSelectionLabel(code);
            _selectionChanged?.Invoke(code);
        };

        _mode.DropDownStyle = ComboBoxStyle.DropDownList;
        _mode.Items.AddRange(new object[] { UiText.Get("Edited"), UiText.Get("Original") });
        _mode.Width = 112;
        _mode.Height = 28;
        _mode.Anchor = AnchorStyles.Left;
        _mode.Margin = new Padding(12, 1, 0, 1);
        _mode.SelectedIndex = initialMode == FontPreviewMode.Original ? 1 : 0;
        _mode.SelectedIndexChanged += (_, _) =>
        {
            _grid.ShowEdited = _mode.SelectedIndex == 0;
            _grid.Invalidate();
            UpdateSelectionLabel(_grid.SelectedCode);
        };

        var layoutGroup = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            Anchor = AnchorStyles.None
        };
        _layoutText = new Label
        {
            Text = UiText.Get("LayoutLabel"),
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 6, 8, 0)
        };
        _layout.DropDownStyle = ComboBoxStyle.DropDownList;
        _layout.Items.AddRange(new object[] { UiText.Get("LayoutWide"), UiText.Get("LayoutClassic"), UiText.Get("LayoutUltra") });
        _layout.Width = 180;
        _layout.Height = 28;
        _layout.Margin = Padding.Empty;
        _layout.SelectedIndex = 0;
        _layout.SelectedIndexChanged += (_, _) =>
        {
            _grid.Columns = _layout.SelectedIndex switch
            {
                1 => 16,
                2 => 64,
                _ => 32
            };
        };

        var zoomGroup = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            Anchor = AnchorStyles.Right
        };
        _zoomText = new Label { Text = UiText.Get("Zoom").TrimEnd(':'), AutoSize = true, Margin = new Padding(0, 6, 6, 0) };
        _zoom.Minimum = 1;
        _zoom.Maximum = 8;
        _zoom.Value = 4;
        _zoom.TickFrequency = 1;
        _zoom.SmallChange = 1;
        _zoom.LargeChange = 1;
        _zoom.Width = 180;
        _zoom.Margin = new Padding(0, 0, 6, 0);
        _zoom.ValueChanged += (_, _) =>
        {
            _zoomLabel.Text = $"{_zoom.Value}×";
            _grid.Zoom = _zoom.Value;
        };
        _zoomLabel.Text = "4×";
        _zoomLabel.AutoSize = true;
        _zoomLabel.Font = new Font(Font, FontStyle.Bold);
        _zoomLabel.Margin = new Padding(0, 6, 0, 0);

        _selection.AutoSize = true;
        _selection.Anchor = AnchorStyles.Right;
        _selection.Margin = new Padding(12, 6, 0, 0);

        _hint = new Label
        {
            Text = UiText.Get("ViewerHint"),
            AutoSize = true,
            Location = new Point(10, 40),
            ForeColor = Color.DimGray
        };

        _warning = new Label
        {
            Text = UiText.Get("ViewerWarning"),
            AutoSize = true,
            Location = new Point(10, 59),
            ForeColor = Color.DarkOrange
        };

        layoutGroup.Controls.AddRange(new Control[] { _layoutText, _layout });
        zoomGroup.Controls.AddRange(new Control[] { _zoomText, _zoom, _zoomLabel });
        topRow.Controls.Add(_title, 0, 0);
        topRow.Controls.Add(_mode, 1, 0);
        topRow.Controls.Add(layoutGroup, 3, 0);
        topRow.Controls.Add(zoomGroup, 5, 0);
        topRow.Controls.Add(_selection, 6, 0);
        top.Controls.AddRange(new Control[] { topRow, _hint, _warning });

        _status.Dock = DockStyle.Bottom;
        _status.Height = 28;
        _status.Padding = new Padding(8, 6, 0, 0);
        _status.BorderStyle = BorderStyle.Fixed3D;

        Controls.Add(_grid);
        Controls.Add(_status);
        Controls.Add(top);
        UiText.LocaleChanged += OnLocaleChanged;
        Disposed += (_, _) => UiText.LocaleChanged -= OnLocaleChanged;
        UpdateSelectionLabel(_grid.SelectedCode);
    }

    private void OnLocaleChanged(object? sender, UiLocaleChangedEventArgs e)
    {
        Text = UiText.Get("FullSetTitle");
        _title.Text = UiText.Get("FullSetHeading");
        _layoutText.Text = UiText.Get("LayoutLabel");
        _zoomText.Text = UiText.Get("Zoom").TrimEnd(':');
        _hint.Text = UiText.Get("ViewerHint");
        _warning.Text = UiText.Get("ViewerWarning");
        int mode = Math.Max(0, _mode.SelectedIndex);
        _mode.Items.Clear();
        _mode.Items.AddRange(new object[] { UiText.Get("Edited"), UiText.Get("Original") });
        _mode.SelectedIndex = Math.Min(mode, _mode.Items.Count - 1);
        int layout = Math.Max(0, _layout.SelectedIndex);
        _layout.Items.Clear();
        _layout.Items.AddRange(new object[] { UiText.Get("LayoutWide"), UiText.Get("LayoutClassic"), UiText.Get("LayoutUltra") });
        _layout.SelectedIndex = Math.Min(layout, _layout.Items.Count - 1);
        UpdateSelectionLabel(_grid.SelectedCode);
    }

    private void UpdateSelectionLabel(int code)
    {
        string slot = GlyphRepository.GetSlotDisplayText(code);
        _selection.Text = $"{UiText.Get("Selected")}: 0x{code:X2} / {code}  {slot}";
        byte[] rows = code >= 0 && code < _glyphs.Count
            ? (_grid.ShowEdited && _glyphs[code].HasEdited ? _glyphs[code].Edited : _glyphs[code].Original)
            : new byte[8];
        _status.Text = string.Format(UiText.Get("ViewerStatus"), code, Convert.ToHexString(rows), code * 8);
    }
}

internal sealed class CharacterSetGridControl : ScrollableControl
{
    private IReadOnlyList<GlyphModel>? _glyphs;
    private int _zoom = 4;
    private int _selectedCode = 65;
    private bool _showEdited = true;
    private int _columns = 32;

    public event Action<int>? SelectedCodeChanged;

    public CharacterSetGridControl()
    {
        DoubleBuffered = true;
        AutoScroll = true;
        BackColor = Color.White;
        ResizeRedraw = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
    }

    public IReadOnlyList<GlyphModel>? Glyphs
    {
        get => _glyphs;
        set { _glyphs = value; UpdateCanvasSize(); Invalidate(); }
    }

    public int Zoom
    {
        get => _zoom;
        set
        {
            int next = Math.Clamp(value, 1, 8);
            if (_zoom == next) return;
            _zoom = next;
            UpdateCanvasSize();
            EnsureSelectedVisible();
            Invalidate();
        }
    }

    public int Columns
    {
        get => _columns;
        set
        {
            int next = value is 16 or 32 or 64 ? value : 32;
            if (_columns == next) return;
            _columns = next;
            UpdateCanvasSize();
            EnsureSelectedVisible();
            Invalidate();
        }
    }

    private int Rows => 256 / _columns;

    public bool ShowEdited
    {
        get => _showEdited;
        set { _showEdited = value; Invalidate(); }
    }

    public int SelectedCode
    {
        get => _selectedCode;
        set
        {
            _selectedCode = Math.Clamp(value, 0, 255);
            EnsureSelectedVisible();
            Invalidate();
        }
    }

    private int CellWidth => Math.Max(34, 8 * _zoom + 16);
    private int CellHeight => Math.Max(46, 8 * _zoom + 22);
    private int HeaderWidth => 54;
    private int HeaderHeight => 30;

    private void UpdateCanvasSize()
    {
        AutoScrollMinSize = new Size(HeaderWidth + _columns * CellWidth + 6, HeaderHeight + Rows * CellHeight + 6);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(Color.White);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
        e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
        e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
        e.Graphics.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);

        using var gridPen = new Pen(Color.FromArgb(165, 175, 190));
        using var headerBrush = new SolidBrush(Color.FromArgb(238, 241, 245));
        using var selectedBrush = new SolidBrush(Color.FromArgb(220, 235, 255));
        using var onBrush = new SolidBrush(Color.Black);
        using var offBrush = new SolidBrush(Color.White);
        using var inactiveOnBrush = new SolidBrush(Color.DimGray);
        using var inactiveOffBrush = new SolidBrush(Color.Gainsboro);
        using var labelFont = new Font("Consolas", 8f);
        using var codeFont = new Font("Consolas", 7f);
        using var headerFont = new Font("Consolas", 8f, FontStyle.Bold);

        // Wide layouts use relative hex-column headers (+00 .. +1F / +3F).
        for (int col = 0; col < _columns; col++)
        {
            int x = HeaderWidth + col * CellWidth;
            var rect = new Rectangle(x, 0, CellWidth, HeaderHeight);
            e.Graphics.FillRectangle(headerBrush, rect);
            e.Graphics.DrawRectangle(gridPen, rect);
            string text = _columns == 16 ? $".{col:X}" : $"+{col:X2}";
            DrawCentered(e.Graphics, text, headerFont, Brushes.Black, rect);
        }

        // Row header shows the base byte for the row in wide modes.
        for (int row = 0; row < Rows; row++)
        {
            int y = HeaderHeight + row * CellHeight;
            var rect = new Rectangle(0, y, HeaderWidth, CellHeight);
            e.Graphics.FillRectangle(headerBrush, rect);
            e.Graphics.DrawRectangle(gridPen, rect);
            string text = _columns == 16 ? $"{row:X}." : $"{row * _columns:X2}-";
            DrawCentered(e.Graphics, text, headerFont, Brushes.Black, rect);
        }

        if (_glyphs is null) return;

        for (int code = 0; code < 256; code++)
        {
            int row = code / _columns;
            int col = code % _columns;
            int x = HeaderWidth + col * CellWidth;
            int y = HeaderHeight + row * CellHeight;
            var cell = new Rectangle(x, y, CellWidth, CellHeight);
            e.Graphics.FillRectangle(code == _selectedCode ? selectedBrush : Brushes.White, cell);
            e.Graphics.DrawRectangle(gridPen, cell);

            if (code >= _glyphs.Count) continue;
            GlyphModel glyph = _glyphs[code];
            byte[] rows = _showEdited
                ? (glyph.HasEdited ? glyph.Edited : new byte[8])
                : glyph.Original;

            int glyphW = 8 * _zoom;
            int glyphH = 8 * _zoom;
            int gx = x + Math.Max(2, (CellWidth - glyphW) / 2);
            int gy = y + 3;
            DrawGlyph(e.Graphics, rows, new Point(gx, gy), _zoom, onBrush, offBrush, inactiveOnBrush, inactiveOffBrush);

            string slot = GlyphRepository.GetSlotDisplayText(code);
            string shortSlot = slot.Trim('\'', ' ');
            if (shortSlot.Length > 5) shortSlot = string.Empty;
            var labelRect = new Rectangle(x + 2, y + glyphH + 5, CellWidth - 4, 11);
            if (!string.IsNullOrWhiteSpace(shortSlot))
                DrawCentered(e.Graphics, shortSlot, labelFont, Brushes.Black, labelRect);

            var codeRect = new Rectangle(x + 2, y + CellHeight - 12, CellWidth - 4, 10);
            DrawCentered(e.Graphics, $"{code:X2}", codeFont, Brushes.DimGray, codeRect);
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        Point p = new(e.X - AutoScrollPosition.X, e.Y - AutoScrollPosition.Y);
        if (p.X < HeaderWidth || p.Y < HeaderHeight) return;
        int col = (p.X - HeaderWidth) / CellWidth;
        int row = (p.Y - HeaderHeight) / CellHeight;
        if (col < 0 || col >= _columns || row < 0 || row >= Rows) return;
        int code = row * _columns + col;
        if (code > 255) return;
        _selectedCode = code;
        Invalidate();
        SelectedCodeChanged?.Invoke(code);
    }

    private void EnsureSelectedVisible()
    {
        if (!IsHandleCreated) return;
        int row = _selectedCode / _columns;
        int col = _selectedCode % _columns;
        int left = HeaderWidth + col * CellWidth;
        int top = HeaderHeight + row * CellHeight;
        int curX = -AutoScrollPosition.X;
        int curY = -AutoScrollPosition.Y;
        int viewW = Math.Max(1, ClientSize.Width - SystemInformation.VerticalScrollBarWidth);
        int viewH = Math.Max(1, ClientSize.Height - SystemInformation.HorizontalScrollBarHeight);
        int nextX = curX;
        int nextY = curY;
        if (left < curX) nextX = left;
        else if (left + CellWidth > curX + viewW) nextX = left + CellWidth - viewW;
        if (top < curY) nextY = top;
        else if (top + CellHeight > curY + viewH) nextY = top + CellHeight - viewH;
        AutoScrollPosition = new Point(Math.Max(0, nextX), Math.Max(0, nextY));
    }

    private static void DrawGlyph(Graphics g, byte[] rows, Point origin, int scale,
        Brush onBrush, Brush offBrush, Brush inactiveOnBrush, Brush inactiveOffBrush)
    {
        if (rows.Length != 8) return;
        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                bool inactive = col >= 6;
                bool on = (rows[row] & (1 << (7 - col))) != 0;
                Brush brush = inactive
                    ? (on ? inactiveOnBrush : inactiveOffBrush)
                    : (on ? onBrush : offBrush);
                g.FillRectangle(brush,
                    origin.X + col * scale, origin.Y + row * scale, scale, scale);
            }
        }
    }

    private static void DrawCentered(Graphics g, string text, Font font, Brush brush, Rectangle rect)
    {
        var size = g.MeasureString(text, font);
        float x = rect.X + (rect.Width - size.Width) / 2f;
        float y = rect.Y + (rect.Height - size.Height) / 2f;
        g.DrawString(text, font, brush, x, y);
    }
}
