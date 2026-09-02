using System.Drawing;

namespace ElviraVgaEditor;

/// <summary>Read-only explanation for an exact-palette PNG validation failure.</summary>
internal sealed class PaletteValidationDialog : Form
{
    private readonly List<Bitmap> _swatches = [];

    public PaletteValidationDialog(ReplacePngValidationResult validation)
    {
        if (validation.FailureKind != ReplacePngValidationFailureKind.PaletteMismatch)
            throw new ArgumentException("A palette mismatch result is required.", nameof(validation));

        AutoScaleMode = AutoScaleMode.Dpi;
        Text = UiText.Get("Graphics.ReplaceValidationTitle");
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(720, 590);

        var message = new Label
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(12, 10, 12, 4),
            Text = UiText.Get("Graphics.PngPaletteInvalid"),
            Font = new Font(Font, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft
        };
        var paletteLabel = SectionLabel(UiText.Get("Graphics.ActivePalette") + ": " + validation.PaletteIdentity);
        var paletteGrid = CreatePaletteGrid(validation.PaletteEntries);
        var invalidLabel = SectionLabel(UiText.Get("Graphics.InvalidColors"));
        var invalidGrid = CreateInvalidGrid(validation.InvalidColors);
        var summary = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 30,
            Padding = new Padding(12, 4, 12, 0),
            Text = string.Format(UiText.Get("Graphics.InvalidColorSummary"), validation.InvalidColors.Count, validation.InvalidPixelCount),
            TextAlign = ContentAlignment.MiddleLeft
        };
        var close = new Button { Text = UiText.Get("Common.Ok"), DialogResult = DialogResult.OK, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, MinimumSize = new Size(92, 32), TextAlign = ContentAlignment.MiddleCenter };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 46, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8, 6, 8, 6) };
        buttons.Controls.Add(close);

        var content = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4, Padding = new Padding(8) };
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        content.RowStyles.Add(new RowStyle(SizeType.Absolute, 25));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        content.Controls.Add(paletteLabel, 0, 0);
        content.Controls.Add(paletteGrid, 0, 1);
        content.Controls.Add(invalidLabel, 0, 2);
        content.Controls.Add(invalidGrid, 0, 3);

        Controls.Add(content);
        Controls.Add(summary);
        Controls.Add(buttons);
        Controls.Add(message);
        AcceptButton = close;
        FormClosed += (_, _) => { foreach (Bitmap swatch in _swatches) swatch.Dispose(); };
    }

    private static Label SectionLabel(string text) => new()
    {
        Dock = DockStyle.Fill,
        Text = text,
        Font = SystemFonts.MessageBoxFont,
        TextAlign = ContentAlignment.MiddleLeft
    };

    private DataGridView CreatePaletteGrid(IReadOnlyList<PaletteValidationEntry> palette)
    {
        var grid = CreateGrid();
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = UiText.Get("Graphics.PaletteIndex"), Width = 65 });
        grid.Columns.Add(new DataGridViewImageColumn { HeaderText = UiText.Get("Graphics.Preview"), Width = 72, ImageLayout = DataGridViewImageCellLayout.Normal });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = UiText.Get("Graphics.Hex"), Width = 100 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = UiText.Get("Graphics.Rgb"), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        foreach (PaletteValidationEntry entry in palette)
            grid.Rows.Add(entry.Index, CreateSwatch(entry.Color), entry.Hex, entry.Rgb);
        return grid;
    }

    private static DataGridView CreateInvalidGrid(IReadOnlyList<InvalidPngColor> invalidColors)
    {
        var grid = CreateGrid();
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = UiText.Get("Graphics.Hex"), Width = 110 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = UiText.Get("Graphics.Rgb"), Width = 130 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = UiText.Get("Graphics.PixelCount"), Width = 110 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = UiText.Get("Graphics.FirstOccurrence"), AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
        foreach (InvalidPngColor color in invalidColors)
            grid.Rows.Add(color.Hex, color.Rgb, color.PixelCount, $"({color.FirstOccurrence.X}, {color.FirstOccurrence.Y})");
        return grid;
    }

    private static DataGridView CreateGrid() => new()
    {
        Dock = DockStyle.Fill,
        AllowUserToAddRows = false,
        AllowUserToDeleteRows = false,
        AllowUserToResizeRows = false,
        ReadOnly = true,
        RowHeadersVisible = false,
        SelectionMode = DataGridViewSelectionMode.FullRowSelect,
        MultiSelect = false,
        AutoGenerateColumns = false,
        BackgroundColor = SystemColors.Window,
        BorderStyle = BorderStyle.FixedSingle,
        RowTemplate = { Height = 22 }
    };

    private Bitmap CreateSwatch(Color color)
    {
        var bitmap = new Bitmap(46, 16);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.Clear(color);
        graphics.DrawRectangle(Pens.Black, 0, 0, bitmap.Width - 1, bitmap.Height - 1);
        _swatches.Add(bitmap);
        return bitmap;
    }
}
