namespace ElviraVgaEditor;

internal sealed class GlyphMatrixControl : Control
{
    private byte[] _rows = new byte[8];
    private bool _editable;
    private bool _allowInactiveColumns;

    public event EventHandler<GlyphPixelToggleEventArgs>? PixelToggled;
    public event EventHandler<GlyphShiftEventArgs>? ShiftRequested;

    public GlyphMatrixControl()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        Cursor = Cursors.Default;
        TabStop = true;
    }

    public void SetGlyph(byte[]? rows, bool editable, bool allowInactiveColumns)
    {
        _rows = rows is { Length: 8 } ? rows : new byte[8];
        _editable = editable;
        _allowInactiveColumns = allowInactiveColumns;
        Cursor = editable ? Cursors.Hand : Cursors.Default;
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(SystemColors.Control);

        int cell = Math.Max(1, Math.Min(ClientSize.Width, ClientSize.Height) / 8);
        int gridW = cell * 8;
        int gridH = cell * 8;
        int x0 = (ClientSize.Width - gridW) / 2;
        int y0 = (ClientSize.Height - gridH) / 2;

        using var gridPen = new Pen(Color.Gray);
        using var activeOn = new SolidBrush(Color.Black);
        using var activeOff = new SolidBrush(Color.White);
        using var inactiveOn = new SolidBrush(Color.DarkGray);
        using var inactiveOff = new SolidBrush(Color.Gainsboro);
        using var textOn = new SolidBrush(Color.White);
        using var textOff = new SolidBrush(Color.Black);
        using var inactiveText = new SolidBrush(Color.DimGray);
        using var font = new Font("Consolas", Math.Max(8, cell * 0.35f), FontStyle.Bold, GraphicsUnit.Pixel);

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                bool inactive = col >= 6;
                bool on = (_rows[row] & (1 << (7 - col))) != 0;
                var rect = new Rectangle(x0 + col * cell, y0 + row * cell, cell, cell);
                e.Graphics.FillRectangle(inactive ? (on ? inactiveOn : inactiveOff) : (on ? activeOn : activeOff), rect);
                e.Graphics.DrawRectangle(gridPen, rect);

                string s = on ? "#" : ".";
                var size = e.Graphics.MeasureString(s, font);
                var brush = inactive ? inactiveText : (on ? textOn : textOff);
                e.Graphics.DrawString(s, font, brush,
                    rect.Left + (rect.Width - size.Width) / 2,
                    rect.Top + (rect.Height - size.Height) / 2);
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (_editable) Focus();
        if (!_editable || e.Button != MouseButtons.Left)
            return;

        int cell = Math.Max(1, Math.Min(ClientSize.Width, ClientSize.Height) / 8);
        int gridW = cell * 8;
        int gridH = cell * 8;
        int x0 = (ClientSize.Width - gridW) / 2;
        int y0 = (ClientSize.Height - gridH) / 2;
        int col = (e.X - x0) / cell;
        int row = (e.Y - y0) / cell;
        if (row < 0 || row > 7 || col < 0 || col > 7)
            return;
        if (col >= 6 && !_allowInactiveColumns)
            return;

        PixelToggled?.Invoke(this, new GlyphPixelToggleEventArgs(row, col));
    }
    protected override bool IsInputKey(Keys keyData)
    {
        Keys key = keyData & Keys.KeyCode;
        if (key is Keys.Left or Keys.Right or Keys.Up or Keys.Down)
            return true;
        return base.IsInputKey(keyData);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!_editable) return;

        int dx = 0, dy = 0;
        switch (e.KeyCode)
        {
            case Keys.Left: dx = -1; break;
            case Keys.Right: dx = 1; break;
            case Keys.Up: dy = -1; break;
            case Keys.Down: dy = 1; break;
            default: return;
        }

        ShiftRequested?.Invoke(this, new GlyphShiftEventArgs(dx, dy));
        e.Handled = true;
        e.SuppressKeyPress = true;
    }

}

internal sealed class GlyphPixelToggleEventArgs : EventArgs
{
    public int Row { get; }
    public int Column { get; }

    public GlyphPixelToggleEventArgs(int row, int column)
    {
        Row = row;
        Column = column;
    }
}


internal sealed class GlyphShiftEventArgs : EventArgs
{
    public int DeltaX { get; }
    public int DeltaY { get; }

    public GlyphShiftEventArgs(int deltaX, int deltaY)
    {
        DeltaX = deltaX;
        DeltaY = deltaY;
    }
}
