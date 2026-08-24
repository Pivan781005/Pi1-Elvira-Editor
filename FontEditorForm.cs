namespace ElviraVgaEditor;

internal sealed class FontEditorForm : Form
{
    /// <summary>Raised after a supported EXE is opened so the host can switch the whole application context.</summary>
    public event Action<string, ElviraGame>? GameExecutableOpened;
    private List<GlyphModel> _glyphs = GlyphRepository.CreateAllCp852Slots().ToList();
    private FontLoadResult? _loaded;
    private readonly ListBox _list = new();
    private readonly GlyphMatrixControl _originalMatrix = new();
    private readonly GlyphMatrixControl _editedMatrix = new();
    private readonly Label _selected = new();
    private readonly Label _originalHex = new();
    private readonly Label _editedHex = new();
    private readonly RichTextBox _status = new();
    private readonly Label _footerStatus = new();
    private readonly Label _sourceInfo = new();
    private readonly CheckBox _advanced = new();
    private readonly Button _apply = new();
    private readonly Button _saveCopy = new();
    private readonly Button _createExtendedCp852 = new();
    private readonly Button _importFont = new();
    private readonly Button _exportFont = new();
    private readonly Button _copyOriginalToEdited = new();
    private readonly Button _reset = new();
    private readonly Button _copyHex = new();
    private readonly Button _shiftLeft = new();
    private readonly Button _shiftRight = new();
    private readonly Button _shiftUp = new();
    private readonly Button _shiftDown = new();
    private readonly Label _shiftInfo = new();
    private readonly ToolTip _toolTip = new();
    private readonly Button _open = new();
    private readonly BitmapFontPreviewControl _preview = new();
    private readonly ComboBox _previewMode = new();
    private readonly Button _fullCharacterSet = new();
    private GlyphModel? _current;
    private readonly string? _initialGameDirectory;
    private readonly Font _listFont = new("Consolas", 9.5f);

    public FontEditorForm(string? initialGameDirectory = null)
    {
        _initialGameDirectory = initialGameDirectory;
        Text = UiText.Get("FontTitle");
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
        Width = 1760;
        Height = 900;
        MinimumSize = new Size(1500, 780);
        StartPosition = FormStartPosition.CenterParent;

        BuildUi();
        ApplyLanguage();
        PopulateList();
        Shown += (_, _) => TryAutoLoadRunVga();
    }

    public void LoadFromGameDirectory(string? gameDirectory)
    {
        if (string.IsNullOrWhiteSpace(gameDirectory))
            return;

        string candidate = Path.Combine(gameDirectory, "RUNVGA.EXE");
        if (!File.Exists(candidate)) candidate = Path.Combine(gameDirectory, "RUNIT.EXE");
        if (!File.Exists(candidate))
            return;

        try { LoadRunVga(candidate, showErrors: false); }
        catch { }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _listFont.Dispose();
        base.Dispose(disposing);
    }

    private void BuildUi()
    {
        SuspendLayout();

        var top = new Panel { Dock = DockStyle.Top, Height = 88, Padding = new Padding(10, 8, 10, 4) };
        _open.Text = UiText.Get("OpenGameExe");
        _open.SetBounds(10, 8, 150, 30);
        _open.Click += (_, _) => OpenGameExe();

        _apply.Text = UiText.Get("ApplyToExe");
        _apply.SetBounds(170, 8, 120, 30);
        _apply.Enabled = false;
        _apply.Click += (_, _) => ApplyToExe();

        _saveCopy.Text = UiText.Get("SaveCopyAs");
        _saveCopy.SetBounds(300, 8, 130, 30);
        _saveCopy.Enabled = false;
        _saveCopy.Click += (_, _) => SaveCopy();

        _createExtendedCp852.Text = "Activate CP852 Patch";
        _createExtendedCp852.SetBounds(440, 8, 190, 30);
        _createExtendedCp852.Enabled = false;
        _createExtendedCp852.Click += (_, _) => CreateExtendedCp852();

        _copyOriginalToEdited.Text = UiText.Get("CopyOriginalEdited");
        _copyOriginalToEdited.SetBounds(640, 8, 170, 30);
        _copyOriginalToEdited.TextAlign = ContentAlignment.MiddleCenter;
        _copyOriginalToEdited.UseCompatibleTextRendering = false;
        _copyOriginalToEdited.Enabled = false;
        _copyOriginalToEdited.Click += (_, _) => CopyOriginalToEdited();

        _importFont.Text = UiText.Get("ImportFont");
        _importFont.SetBounds(820, 8, 120, 30);
        _importFont.Enabled = false;
        _importFont.Click += (_, _) => ImportFont();

        _exportFont.Text = UiText.Get("ExportFont");
        _exportFont.SetBounds(950, 8, 120, 30);
        _exportFont.Enabled = false;
        _exportFont.Click += (_, _) => ExportFont();

        _sourceInfo.SetBounds(1080, 7, 550, 58);
        _sourceInfo.Text = UiText.Get("NoRunVga");
        _sourceInfo.AutoEllipsis = true;
        top.Controls.AddRange(new Control[] { _open, _apply, _saveCopy, _createExtendedCp852, _copyOriginalToEdited, _importFont, _exportFont, _sourceInfo });

        var left = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        var previewHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10, 8, 10, 10), BackColor = SystemColors.Control };
        var main = new Panel { Dock = DockStyle.Fill, Padding = new Padding(12) };
        var contentGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
        contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
        contentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var heading = new Label
        {
            Text = UiText.Get("AllByteSlots"),
            Tag = "AllByteSlots",
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font(Font, FontStyle.Bold)
        };

        var jumpBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 36,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 2, 0, 2)
        };
        var jumpDigits = new Button { Text = "0-9", Width = 62, Height = 28 };
        var jumpUpper = new Button { Text = "A-Z", Width = 62, Height = 28 };
        var jumpLower = new Button { Text = "a-z", Width = 62, Height = 28 };
        var jumpExtended = new Button { Text = "CP852", Width = 76, Height = 28 };
        jumpDigits.Click += (_, _) => JumpTo(0x30);
        jumpUpper.Click += (_, _) => JumpTo(0x41);
        jumpLower.Click += (_, _) => JumpTo(0x61);
        jumpExtended.Click += (_, _) => JumpTo(0x80);
        jumpBar.Controls.AddRange(new Control[] { jumpDigits, jumpUpper, jumpLower, jumpExtended });

        _list.Dock = DockStyle.Fill;
        _list.DrawMode = DrawMode.OwnerDrawFixed;
        _list.ItemHeight = 29;
        _list.IntegralHeight = false;
        _list.SelectedIndexChanged += (_, _) => SelectGlyph();
        _list.DrawItem += DrawGlyphListItem;
        left.Controls.Add(_list);
        left.Controls.Add(jumpBar);
        left.Controls.Add(heading);

        // Keep the full-character-set button visible even when the right preview pane is narrow.
        // The previous build used a fixed X coordinate (250), which could place the button
        // outside the clipped preview panel at common window sizes / DPI settings.
        var previewHeader = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 34,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        previewHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        previewHeader.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var previewTitle = new Label
        {
            Text = UiText.Get("RealBitmapPreview"),
            Tag = "RealBitmapPreview",
            Dock = DockStyle.Fill,
            Font = new Font(Font, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleLeft,
            AutoEllipsis = true,
            Margin = new Padding(0, 0, 6, 0)
        };
        _fullCharacterSet.Text = UiText.Get("FullCharacterSet");
        _fullCharacterSet.AutoSize = true;
        _fullCharacterSet.MinimumSize = new Size(150, 28);
        _fullCharacterSet.Height = 28;
        _fullCharacterSet.Margin = new Padding(0, 2, 0, 2);
        _fullCharacterSet.Click += (_, _) => OpenFullCharacterSet();
        previewHeader.Controls.Add(previewTitle, 0, 0);
        previewHeader.Controls.Add(_fullCharacterSet, 1, 0);

        _previewMode.DropDownStyle = ComboBoxStyle.DropDownList;
        _previewMode.Items.AddRange(new object[] { UiText.Get("SideBySide"), UiText.Get("Edited"), UiText.Get("Original") });
        _previewMode.SelectedIndex = 0;
        _previewMode.Dock = DockStyle.Top;
        _previewMode.Height = 30;
        _previewMode.SelectedIndexChanged += (_, _) => UpdatePreview();
        _preview.Dock = DockStyle.Fill;
        previewHost.Controls.Add(_preview);
        previewHost.Controls.Add(_previewMode);
        previewHost.Controls.Add(previewHeader);

        _selected.Dock = DockStyle.Top;
        _selected.Height = 34;
        _selected.Font = new Font(Font.FontFamily, 15f, FontStyle.Bold);

        var note = new Label
        {
            Dock = DockStyle.Top,
            Height = 52,
            Text = UiText.Get("GlyphNote"),
            Tag = "GlyphNote",
            AutoEllipsis = false
        };

        _advanced.Text = UiText.Get("AdvancedColumns");
        _advanced.Dock = DockStyle.Top;
        _advanced.Height = 28;
        _advanced.CheckedChanged += (_, _) => UpdateMatricesOnly();

        var labels = new Panel { Dock = DockStyle.Top, Height = 34 };
        var originalLabel = new Label
        {
            Text = UiText.Get("OriginalReadOnly"),
            Tag = "OriginalReadOnly",
            Font = new Font(Font, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        originalLabel.SetBounds(10, 4, 390, 26);
        var editedLabel = new Label
        {
            Text = UiText.Get("EditedCaps"),
            Tag = "EditedCaps",
            Font = new Font(Font, FontStyle.Bold),
            TextAlign = ContentAlignment.MiddleCenter
        };
        editedLabel.SetBounds(450, 4, 390, 26);
        labels.Controls.AddRange(new Control[] { originalLabel, editedLabel });

        _originalMatrix.SetBounds(25, 150, 368, 368);
        _editedMatrix.SetBounds(465, 150, 368, 368);
        _editedMatrix.PixelToggled += EditedMatrixOnPixelToggled;
        _editedMatrix.ShiftRequested += (_, e) => ShiftCurrentGlyph(e.DeltaX, e.DeltaY);

        _originalHex.SetBounds(25, 532, 390, 28);
        _originalHex.Font = new Font("Consolas", 10f);
        _editedHex.SetBounds(465, 532, 390, 28);
        _editedHex.Font = new Font("Consolas", 10f);

        _reset.Text = UiText.Get("ResetGlyph");
        _reset.SetBounds(465, 570, 110, 30);
        _reset.Enabled = false;
        _reset.Click += (_, _) =>
        {
            if (_current is null) return;
            if (IsReservedHudEraseGlyph(_current)) return;
            _current.Reset();
            RefreshEditedActionState();
            UpdateMatricesOnly();
            RefreshListItem(_current.ByteValue);
        };

        _copyHex.Text = UiText.Get("CopyHex");
        _copyHex.SetBounds(585, 570, 100, 30);
        _copyHex.Enabled = false;
        _copyHex.Click += (_, _) =>
        {
            if (_current is not null)
                Clipboard.SetText(ToHex(_current.Edited));
        };

        _shiftLeft.Text = "←";
        _shiftUp.Text = "↑";
        _shiftDown.Text = "↓";
        _shiftRight.Text = "→";
        _shiftLeft.SetBounds(700, 570, 34, 30);
        _shiftUp.SetBounds(736, 570, 34, 30);
        _shiftDown.SetBounds(772, 570, 34, 30);
        _shiftRight.SetBounds(808, 570, 34, 30);
        _shiftLeft.Click += (_, _) => ShiftCurrentGlyph(-1, 0);
        _shiftRight.Click += (_, _) => ShiftCurrentGlyph(1, 0);
        _shiftUp.Click += (_, _) => ShiftCurrentGlyph(0, -1);
        _shiftDown.Click += (_, _) => ShiftCurrentGlyph(0, 1);

        _shiftInfo.SetBounds(700, 604, 240, 24);
        _shiftInfo.AutoEllipsis = true;
        _shiftInfo.Text = string.Format(UiText.Get("GlyphShiftInfo"), 0, 0, 0);

        _status.SetBounds(25, 640, 900, 105);
        _status.ReadOnly = true;
        _status.BorderStyle = BorderStyle.None;
        _status.BackColor = SystemColors.Control;
        _status.ScrollBars = RichTextBoxScrollBars.None;
        _status.DetectUrls = false;
        _status.TabStop = false;
        SetStatusText(UiText.Get("FontReadyHint"));

        main.Controls.Add(_status);
        main.Controls.Add(_shiftInfo);
        main.Controls.Add(_shiftRight);
        main.Controls.Add(_shiftDown);
        main.Controls.Add(_shiftUp);
        main.Controls.Add(_shiftLeft);
        main.Controls.Add(_copyHex);
        main.Controls.Add(_reset);
        main.Controls.Add(_editedHex);
        main.Controls.Add(_originalHex);
        main.Controls.Add(_editedMatrix);
        main.Controls.Add(_originalMatrix);
        main.Controls.Add(labels);
        main.Controls.Add(_advanced);
        main.Controls.Add(note);
        main.Controls.Add(_selected);

        contentGrid.Controls.Add(left, 0, 0);
        contentGrid.Controls.Add(main, 1, 0);
        contentGrid.Controls.Add(previewHost, 2, 0);

        _footerStatus.Dock = DockStyle.Bottom;
        _footerStatus.Height = 28;
        _footerStatus.Padding = new Padding(8, 6, 0, 0);
        _footerStatus.BorderStyle = BorderStyle.Fixed3D;

        Controls.Add(contentGrid);
        Controls.Add(_footerStatus);
        Controls.Add(top);
        ResumeLayout(true);
    }

    private void DrawGlyphListItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= _list.Items.Count)
            return;
        var g = _glyphs[(int)_list.Items[e.Index]];
        e.DrawBackground();
        Color fg = (e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : SystemColors.ControlText;
        Color muted = (e.State & DrawItemState.Selected) != 0 ? SystemColors.HighlightText : Color.DimGray;

        int x = e.Bounds.Left + 4;
        int y = e.Bounds.Top + 3;
        int thumbX = x;
        int thumbY = y + 1;
        DrawGlyphThumbnail(e.Graphics, g, new Rectangle(thumbX, thumbY, 36, 22));

        bool reserved = IsReservedHudEraseGlyph(g);
        string state = reserved ? "!" : (g.HasEdited ? (g.IsModified ? "M" : "E") : (g.IsLoadedFromSource ? "*" : (g.HasKnownFallbackBitmap ? "R" : " ")));
        string printable = GlyphRepository.GetSlotDisplayText(g.ByteValue);
        string left = $"{state} 0x{g.ByteValue:X2} {g.ByteValue,3}";
        e.Graphics.DrawString(left, _listFont, new SolidBrush(fg), x + 46, y + 3);
        e.Graphics.DrawString(printable, _listFont, new SolidBrush(fg), x + 158, y + 3);
        e.Graphics.DrawString(reserved ? UiText.Get("ReservedHudEraseGlyphList") : UiText.GlyphDescription(g.Description), _listFont, new SolidBrush(muted), x + 220, y + 3);
        e.DrawFocusRectangle();
    }

    private static void DrawGlyphThumbnail(Graphics g, GlyphModel glyph, Rectangle bounds)
    {
        byte[] rows = glyph.HasEdited ? glyph.Edited : (glyph.IsLoadedFromSource || glyph.HasKnownFallbackBitmap ? glyph.Original : new byte[8]);
        int px = Math.Max(1, Math.Min(bounds.Width / 8, bounds.Height / 8));
        int w = px * 8;
        int h = px * 8;
        int x0 = bounds.Left + (bounds.Width - w) / 2;
        int y0 = bounds.Top + (bounds.Height - h) / 2;
        using var onBrush = new SolidBrush(glyph.IsLoadedFromSource ? Color.Black : Color.Gray);
        using var offBrush = new SolidBrush(Color.White);
        using var inactiveBrush = new SolidBrush(Color.Gainsboro);
        using var borderPen = new Pen(Color.DarkGray);

        for (int row = 0; row < 8; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                bool on = (rows[row] & (1 << (7 - col))) != 0;
                var r = new Rectangle(x0 + col * px, y0 + row * px, px, px);
                g.FillRectangle(col >= 6 ? inactiveBrush : (on ? onBrush : offBrush), r);
            }
        }
        g.DrawRectangle(borderPen, x0, y0, w, h);
    }

    private void TryAutoLoadRunVga()
    {
        if (string.IsNullOrWhiteSpace(_initialGameDirectory)) return;
        string candidate = Path.Combine(_initialGameDirectory, "RUNVGA.EXE");
        if (!File.Exists(candidate)) candidate = Path.Combine(_initialGameDirectory, "RUNIT.EXE");
        if (!File.Exists(candidate)) return;
        try { LoadRunVga(candidate, showErrors: false); }
        catch { }
    }

    private void OpenGameExe()
    {
        using var dlg = new OpenFileDialog
        {
            Title = UiText.Get("OpenGameExe"),
            Filter = $"Elvira game executables (*.EXE)|*.EXE|{UiText.Get("AllFiles")} (*.*)|*.*",
            FileName = "",
            InitialDirectory = Directory.Exists(_initialGameDirectory) ? _initialGameDirectory : null
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        LoadRunVga(dlg.FileName, showErrors: true);
    }

    private void LoadRunVga(string path, bool showErrors)
    {
        try
        {
            Cursor = Cursors.WaitCursor;
            _loaded = RunVgaFontService.LoadRunVga(path);
            _glyphs = _loaded.Glyphs;
            PopulateList();
            _apply.Enabled = false;
            _saveCopy.Enabled = false;
            RunVgaBootstrapState bootstrapState = RunVgaBootstrapService.DetectState(path);
            RunItBootstrapState runItState = RunItBootstrapService.DetectState(path);
            _createExtendedCp852.Enabled = bootstrapState is RunVgaBootstrapState.OriginalPacked or RunVgaBootstrapState.UnpackedBaseline ||
                runItState is RunItBootstrapState.OriginalPacked or RunItBootstrapState.CanonicalUnpackedAscii98;
            _copyOriginalToEdited.Enabled = _loaded.CanInitializeEdited;
            _importFont.Enabled = _loaded.HasFullCp852Font;
            _exportFont.Enabled = false;

            string layout = LocalizedLayout(_loaded.Layout);
            string nativeRange = LocalizedNativeRange(_loaded.Layout);
            string game = _loaded.Game == ElviraGame.Elvira2 ? "Elvira II" : _loaded.Game == ElviraGame.Elvira1 ? "Elvira I" : "Unknown";
            _sourceInfo.Text = $"{game}   |   {UiText.Get("Source")}: {Path.GetFileName(path)}   |   {UiText.Get("Layout")}: {layout}   |   Offset: {(_loaded.FontOffset >= 0 ? $"0x{_loaded.FontOffset:X}" : "n/a")}   |   {LocalizedSlotSummary(_loaded)}{nativeRange}";
            SetStatusText(BuildLocalizedDetectionSummary() + " " + UiText.Get("EditedEmptyHint"));
            if (_loaded.Game is ElviraGame.Elvira1 or ElviraGame.Elvira2)
                GameExecutableOpened?.Invoke(path, _loaded.Game);
        }
        catch (Exception ex)
        {
            _loaded = null;
            _apply.Enabled = false;
            _saveCopy.Enabled = false;
            _createExtendedCp852.Enabled = false;
            _copyOriginalToEdited.Enabled = false;
            _importFont.Enabled = false;
            _exportFont.Enabled = false;
            _sourceInfo.Text = UiText.Language == UiLanguage.Slovak ? "Načítanie herného EXE zlyhalo." : UiText.Language == UiLanguage.Czech ? "Načtení herního EXE selhalo." : "Game EXE load failed.";
            SetStatusText(ex.Message);
            if (showErrors)
                MessageBox.Show(this, ex.Message, UiText.Get("FontLoadTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void PopulateList()
    {
        int selectedByte = _list.SelectedIndex >= 0 ? (int)_list.Items[_list.SelectedIndex] : 0x20;
        _list.BeginUpdate();
        _list.Items.Clear();
        bool fullCp852 = _loaded?.HasFullCp852Font == true;
        for (int i = fullCp852 ? 0x20 : 0; i < _glyphs.Count; i++)
            if (!fullCp852 || i <= 0xFF) _list.Items.Add(i);
        _list.EndUpdate();

        if (_list.Items.Count > 0)
            _list.SelectedIndex = Math.Clamp(selectedByte - (fullCp852 ? 0x20 : 0), 0, _list.Items.Count - 1);
        _list.Invalidate();
    }

    private void JumpTo(int byteValue)
    {
        if (_list.Items.Count == 0) return;
        int first = _list.Items.Count > 0 ? (int)_list.Items[0] : 0;
        int index = Math.Clamp(byteValue - first, 0, _list.Items.Count - 1);
        _list.SelectedIndex = index;
        _list.TopIndex = Math.Max(0, index - 2);
        _list.Focus();
    }

    private void RefreshListItem(int byteValue)
    {
        int itemIndex = _list.Items.IndexOf(byteValue);
        if (itemIndex < 0) return;
        int y = _list.GetItemRectangle(itemIndex).Top;
        if (y + _list.ItemHeight >= 0 && y <= _list.ClientSize.Height)
            _list.Invalidate(_list.GetItemRectangle(itemIndex));
    }

    private void SelectGlyph()
    {
        if (_list.SelectedIndex < 0 || _list.SelectedIndex >= _list.Items.Count) return;
        _current = _glyphs[(int)_list.Items[_list.SelectedIndex]];
        string sourceState = _current.IsLoadedFromSource ? UiText.Get("LoadedFromRunVga") : (_current.HasKnownFallbackBitmap ? UiText.Get("HistoricalFallback") : UiText.Get("NotPresentLoaded"));
        bool reserved = IsReservedHudEraseGlyph(_current);
        string slot = GlyphRepository.GetSlotDisplayText(_current.ByteValue);
        _selected.Text = reserved
            ? UiText.Get("ReservedHudEraseGlyph")
            : $"0x{_current.ByteValue:X2} / {_current.ByteValue}   {slot}   {sourceState}";
        _toolTip.SetToolTip(_selected, reserved ? UiText.Get("ReservedHudEraseGlyphTip") : string.Empty);
        _toolTip.SetToolTip(_editedMatrix, reserved ? UiText.Get("ReservedHudEraseGlyphTip") : string.Empty);
        _reset.Enabled = _current.IsLoadedFromSource && !reserved;
        _copyHex.Enabled = _current.HasEdited;
        SetShiftButtonsEnabled(_current.HasEdited && !reserved);
        UpdateMatricesOnly();
    }

    private void UpdateMatricesOnly()
    {
        if (_current is null)
        {
            _originalMatrix.SetGlyph(null, false, false);
            _editedMatrix.SetGlyph(null, false, false);
            return;
        }

        _originalMatrix.SetGlyph(_current.Original, false, false);
        bool reserved = IsReservedHudEraseGlyph(_current);
        byte[] edited = reserved
            ? RunItBootstrapService.ReservedHudEraseGlyphBytes
            : (_current.HasEdited ? _current.Edited : new byte[8]);
        _editedMatrix.SetGlyph(edited, _current.HasEdited && !reserved, _advanced.Checked && !reserved);
        _originalHex.Text = UiText.Get("OriginalHex") + " " + ToHex(_current.Original);
        _editedHex.Text = _current.HasEdited ? UiText.Get("EditedHex") + " " + ToHex(_current.Edited) : UiText.Get("EditedHex") + " [EMPTY]";
        _shiftInfo.Text = _current.HasEdited
            ? string.Format(UiText.Get("GlyphShiftInfo"), _current.ShiftX, _current.ShiftY, _current.OutsidePixelCount)
            : string.Format(UiText.Get("GlyphShiftInfo"), 0, 0, 0);
        SetShiftButtonsEnabled(_current.HasEdited && !reserved);
        UpdatePreview();
    }

    private void EditedMatrixOnPixelToggled(object? sender, GlyphPixelToggleEventArgs e)
    {
        if (_current is null || !_current.HasEdited || IsReservedHudEraseGlyph(_current)) return;
        _current.ToggleEditedPixel(e.Row, e.Column);
        UpdateMatricesOnly();
        UpdateFontStatusSummary();
        RefreshListItem(_current.ByteValue);
    }

    private void ShiftCurrentGlyph(int dx, int dy)
    {
        if (_current is null || !_current.HasEdited || IsReservedHudEraseGlyph(_current)) return;
        _current.ShiftEdited(dx, dy);
        UpdateMatricesOnly();
        UpdateFontStatusSummary();
        RefreshListItem(_current.ByteValue);
        _editedMatrix.Focus();
    }

    private void SetShiftButtonsEnabled(bool enabled)
    {
        _shiftLeft.Enabled = enabled;
        _shiftRight.Enabled = enabled;
        _shiftUp.Enabled = enabled;
        _shiftDown.Enabled = enabled;
    }

    private bool IsReservedHudEraseGlyph(GlyphModel glyph) =>
        RunVgaFontService.IsReservedHudGlyph(_loaded, glyph.ByteValue);

    private bool ConfirmClipBeforeOutput(string operationName)
    {
        int glyphs = _glyphs.Count(g => g.HasEdited && g.HasPixelsOutside8x8);
        int pixels = _glyphs.Where(g => g.HasEdited).Sum(g => g.OutsidePixelCount);
        if (pixels == 0) return true;

        string message = string.Format(UiText.Get("GlyphClipWarning"), pixels, glyphs, operationName);
        var answer = MessageBox.Show(this, message, UiText.Get("GlyphClipTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        return answer == DialogResult.Yes;
    }

    private void ApplyToExe()
    {
        if (_loaded is null || !_loaded.CanApply || !_glyphs.Any(g => g.HasEdited)) return;
        int modified = _glyphs.Count(g => g.IsLoadedFromSource && g.IsModified);
        if (modified == 0)
        {
            MessageBox.Show(this, UiText.Get("NoGlyphChanges"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (!ConfirmClipBeforeOutput(UiText.Get("ApplyToExe"))) return;

        var answer = MessageBox.Show(this,
            string.Format(UiText.Get("ConfirmApplyFont"), modified, _loaded.SourcePath),
            UiText.Get("ApplyFontTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (answer != DialogResult.Yes) return;

        try
        {
            ProductionDeploymentResult result = GamePatchDeploymentService.Deploy(_loaded, _glyphs);
            LoadRunVga(result.ActiveExecutable, showErrors: true);
            MessageBox.Show(this, $"Patched active executable: {result.ActiveExecutable}\nOriginal backup: {result.OriginalExecutable}\nGAMEPC original backup: {result.OriginalGamePc}", UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("ApplyFontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SaveCopy()
    {
        if (_loaded is null || !_loaded.CanApply || !_glyphs.Any(g => g.HasEdited)) return;
        if (!ConfirmClipBeforeOutput(UiText.Get("SaveCopyAs"))) return;
        using var dlg = new SaveFileDialog
        {
            Title = UiText.Get("SaveModifiedRunVga"),
            Filter = $"{UiText.Get("DosExecutable")} (*.EXE)|*.EXE|{UiText.Get("AllFiles")} (*.*)|*.*",
            FileName = Path.GetFileNameWithoutExtension(_loaded.SourcePath) + "_FONT_EDITED.EXE",
            InitialDirectory = Path.GetDirectoryName(_loaded.SourcePath)
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            RunVgaFontService.SaveCopy(_loaded, dlg.FileName);
            MessageBox.Show(this, string.Format(UiText.Get("FontCopySaved"), dlg.FileName), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("SaveFontCopyTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CreateExtendedCp852()
    {
        if (_loaded is null) return;
        if (_loaded.Game is ElviraGame.Elvira1 or ElviraGame.Elvira2)
        {
            if (!ConfirmClipBeforeOutput("Activate CP852 Patch")) return;
            if (MessageBox.Show(this, "Create/replace the active CP852 executable from the verified original O-file source? Existing O-files are never overwritten.", UiText.Get("FontTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                ProductionDeploymentResult result = GamePatchDeploymentService.Deploy(_loaded, _glyphs);
                LoadRunVga(result.ActiveExecutable, showErrors: true);
                MessageBox.Show(this, $"Activated {(_loaded?.Game == ElviraGame.Elvira2 ? "Elvira II V2 split-font" : "Elvira I V5")} CP852 patch.\n\nOriginal EXE: {result.OriginalExecutable}\nOriginal GAMEPC: {result.OriginalGamePc}", UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            return;
        }
        RunItBootstrapState runItState = RunItBootstrapService.DetectState(_loaded.SourcePath);
        if (runItState != RunItBootstrapState.Unsupported)
        {
            CreateExtendedCp852RunIt(runItState);
            return;
        }
        RunVgaBootstrapState state = RunVgaBootstrapService.DetectState(_loaded.SourcePath);
        if (state == RunVgaBootstrapState.ExtendedCp852V5)
        {
            MessageBox.Show(this, "This executable is already Extended CP852 / V5.", UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (state is not (RunVgaBootstrapState.OriginalPacked or RunVgaBootstrapState.UnpackedBaseline))
        {
            MessageBox.Show(this, "This RUNVGA is not the exact supported English original or BAM/V3 baseline. No bytes were changed.", UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string sourceKind = state == RunVgaBootstrapState.OriginalPacked ? "the verified packed original" : "the verified unpacked BAM/V3 baseline";
        DialogResult answer = MessageBox.Show(this,
            $"Create a new Extended CP852 / V5 executable from {sourceKind}?\n\n" +
            "The source RUNVGA.EXE will not be modified. The new executable uses the currently edited 256-slot font when present; otherwise it uses the editor's current default font slots.",
            "Create Extended CP852 / V5", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes) return;

        using var dlg = new SaveFileDialog
        {
            Title = "Save Extended CP852 / V5 RUNVGA",
            Filter = $"{UiText.Get("DosExecutable")} (*.EXE)|*.EXE|{UiText.Get("AllFiles")} (*.*)|*.*",
            FileName = "RUNVGA_EXTENDED_CP852_V5.EXE",
            InitialDirectory = Path.GetDirectoryName(_loaded.SourcePath),
            OverwritePrompt = false
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            Cursor = Cursors.WaitCursor;
            RunVgaBootstrapResult result = RunVgaBootstrapService.CreateExtendedCp852(_loaded.SourcePath, dlg.FileName, _glyphs);
            LoadRunVga(result.OutputPath, showErrors: true);
            MessageBox.Show(this,
                $"Extended CP852 / V5 executable created safely:\n{result.OutputPath}\n\nSHA-256: {result.Sha256}\nFont source: {(result.UsedEditedFont ? "current edited glyphs" : "current default glyph slots")}.",
                "Create Extended CP852 / V5", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Create Extended CP852 / V5", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void CreateExtendedCp852RunIt(RunItBootstrapState state)
    {
        if (_loaded is null) return;
        if (state == RunItBootstrapState.ExtendedCp852)
        {
            MessageBox.Show(this, "This Elvira II RUNIT executable is already Extended CP852.", UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (state is not (RunItBootstrapState.OriginalPacked or RunItBootstrapState.CanonicalUnpackedAscii98))
        {
            MessageBox.Show(this, "Unsupported or modified RUNIT.EXE. No bytes were changed.", UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        string sourceKind = state == RunItBootstrapState.OriginalPacked ? "the verified packed Elvira II original" : "the canonical unpacked Elvira II baseline";
        if (MessageBox.Show(this, $"Create a new Elvira II Extended CP852 executable from {sourceKind}?\n\nThe source RUNIT.EXE will not be modified. The new executable uses current edited glyphs when present; otherwise it uses safe default slots.",
            "Create Elvira II Extended CP852", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        using var dlg = new SaveFileDialog
        {
            Title = "Save Elvira II Extended CP852 RUNIT",
            Filter = $"{UiText.Get("DosExecutable")} (*.EXE)|*.EXE|{UiText.Get("AllFiles")} (*.*)|*.*",
            FileName = "RUNIT_EXTENDED_CP852.EXE",
            InitialDirectory = Path.GetDirectoryName(_loaded.SourcePath),
            OverwritePrompt = false
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            Cursor = Cursors.WaitCursor;
            RunItBootstrapResult result = RunItBootstrapService.CreateExtendedCp852(_loaded.SourcePath, dlg.FileName, _glyphs);
            LoadRunVga(result.OutputPath, showErrors: true);
            MessageBox.Show(this, $"Elvira II Extended CP852 executable created safely:\n{result.OutputPath}\n\nSHA-256: {result.Sha256}\n\nIf saved under another name, DOSBox/GOG launch configuration must execute that filename.",
                "Create Elvira II Extended CP852", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Create Elvira II Extended CP852", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally { Cursor = Cursors.Default; }
    }



    private void CopyOriginalToEdited()
    {
        if (_loaded is null || !_loaded.CanInitializeEdited) return;

        int copied = 0;
        foreach (GlyphModel glyph in _glyphs)
        {
            if (!glyph.IsLoadedFromSource)
                continue;
            if (IsReservedHudEraseGlyph(glyph))
                glyph.ReplaceEdited(FontSlotMetadata.GetCanonicalBytes(_loaded!, glyph.ByteValue));
            else
                glyph.CopyOriginalToEdited();
            copied++;
        }

        SetStatusText(string.Format(UiText.Get("CopiedOriginal"), copied));
        RefreshEditedActionState();
        _list.Invalidate();
        SelectGlyph();
    }

    private void RefreshEditedActionState()
    {
        bool anyEdited = _glyphs.Any(g => g.HasEdited);
        bool fullEdited = _loaded?.HasFullCp852Font == true && _glyphs.All(g => g.HasEdited);
        _apply.Enabled = _loaded?.CanApply == true && anyEdited;
        _saveCopy.Enabled = _loaded?.CanApply == true && anyEdited;
        _exportFont.Enabled = fullEdited;
        UpdateFontStatusSummary();
    }

    private void ImportFont()
    {
        if (_loaded is null || !_loaded.HasFullCp852Font) return;
        using var dlg = new OpenFileDialog
        {
            Title = UiText.Get("ImportFontDialog"),
            Filter = $"{UiText.Get("SupportedFonts")} (*.bin;*.fnt;*.f08;*.ttf;*.otf;*.sfd)|*.bin;*.fnt;*.f08;*.ttf;*.otf;*.sfd|{UiText.Get("DosBitmapFont")} (*.f08)|*.f08|{UiText.Get("ElviraRawFont")} (*.bin;*.fnt)|*.bin;*.fnt|{UiText.Get("VectorFonts")} (*.ttf;*.otf;*.sfd)|*.ttf;*.otf;*.sfd|{UiText.Get("V5RunVgaExecutable")} (*.EXE)|*.EXE|{UiText.Get("AllFiles")} (*.*)|*.*",
            InitialDirectory = Path.GetDirectoryName(_loaded.SourcePath)
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            string extension = Path.GetExtension(dlg.FileName);
            string result;
            if (extension.Equals(".ttf", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".otf", StringComparison.OrdinalIgnoreCase) ||
                extension.Equals(".sfd", StringComparison.OrdinalIgnoreCase))
            {
                using var optionsDialog = new VectorFontImportDialog(dlg.FileName);
                if (optionsDialog.ShowDialog(this) != DialogResult.OK) return;
                byte[] bytes = VectorFontRasterizer.RasterizeCp852(dlg.FileName, optionsDialog.Options);
                result = RunVgaFontService.ImportFontBytesIntoEdited(_loaded, bytes,
                    VectorFontRasterizer.Describe(bytes, dlg.FileName, optionsDialog.Options));
            }
            else
            {
                result = RunVgaFontService.ImportFontIntoEdited(_loaded, dlg.FileName);
            }

            _glyphs = _loaded.Glyphs;
            SetStatusText(result + UiText.Get("ImportedSuffix"), Path.GetFileName(dlg.FileName));
            RefreshEditedActionState();
            _list.Invalidate();
            SelectGlyph();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("ImportFontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ExportFont()
    {
        if (_loaded is null || !_loaded.HasFullCp852Font || !_glyphs.All(g => g.HasEdited)) return;
        if (!ConfirmClipBeforeOutput(UiText.Get("ExportFont"))) return;
        using var dlg = new SaveFileDialog
        {
            Title = UiText.Get("ExportFontDialog"),
            Filter = $"{UiText.Get("ElviraRawFont")} (*.bin)|*.bin|{UiText.Get("AllFiles")} (*.*)|*.*",
            FileName = Path.GetFileNameWithoutExtension(_loaded.SourcePath) + "_FONT_256x8.bin",
            InitialDirectory = Path.GetDirectoryName(_loaded.SourcePath)
        };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            string hash = RunVgaFontService.ExportEditedFont(_loaded, dlg.FileName);
            SetStatusText(string.Format(UiText.Get("ExportedFontStatus"), dlg.FileName, hash), Path.GetFileName(dlg.FileName));
            MessageBox.Show(this, string.Format(UiText.Get("FontExported"), dlg.FileName, hash), UiText.Get("ExportFontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("ExportFontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenFullCharacterSet()
    {
        int code = _current?.ByteValue ?? 65;
        FontPreviewMode mode = _previewMode.SelectedIndex switch
        {
            2 => FontPreviewMode.Original,
            1 => FontPreviewMode.Edited,
            _ => FontPreviewMode.Edited
        };

        using var viewer = new CharacterSetViewerForm(_glyphs, code, mode, selectedCode =>
        {
            if (selectedCode >= 0 && selectedCode < _list.Items.Count)
            {
                _list.SelectedIndex = selectedCode;
                _list.TopIndex = Math.Max(0, selectedCode - 2);
            }
        });
        viewer.ShowDialog(this);
    }

    private void UpdatePreview()
    {
        int code = _current?.ByteValue ?? 65;
        FontPreviewMode mode = _previewMode.SelectedIndex switch
        {
            2 => FontPreviewMode.Original,
            1 => FontPreviewMode.Edited,
            _ => FontPreviewMode.SideBySide
        };
        _preview.SetPreview(_glyphs, code, mode);
    }

    public void ApplyLanguage()
    {
        Text = UiText.Get("FontTitle");
        _open.Text = UiText.Get("OpenGameExe");
        _apply.Text = UiText.Get("ApplyToExe");
        _saveCopy.Text = UiText.Get("SaveCopyAs");
        _copyOriginalToEdited.Text = UiText.Get("CopyOriginalEdited");
        _importFont.Text = UiText.Get("ImportFont");
        _exportFont.Text = UiText.Get("ExportFont");
        _fullCharacterSet.Text = UiText.Get("FullCharacterSet");
        _advanced.Text = UiText.Get("AdvancedColumns");
        _reset.Text = UiText.Get("ResetGlyph");
        _copyHex.Text = UiText.Get("CopyHex");
        _toolTip.SetToolTip(_shiftLeft, UiText.Get("GlyphShiftLeft"));
        _toolTip.SetToolTip(_shiftRight, UiText.Get("GlyphShiftRight"));
        _toolTip.SetToolTip(_shiftUp, UiText.Get("GlyphShiftUp"));
        _toolTip.SetToolTip(_shiftDown, UiText.Get("GlyphShiftDown"));

        ApplyTaggedLocalization(this);

        int previewIndex = Math.Max(0, _previewMode.SelectedIndex);
        _previewMode.Items.Clear();
        _previewMode.Items.AddRange(new object[] { UiText.Get("SideBySide"), UiText.Get("Edited"), UiText.Get("Original") });
        _previewMode.SelectedIndex = Math.Min(previewIndex, 2);

        if (_loaded is null)
        {
            _sourceInfo.Text = UiText.Get("NoRunVga");
            SetStatusText(UiText.Get("FontReadyHint"));
        }
        else
        {
            string layout = LocalizedLayout(_loaded.Layout);
            string nativeRange = LocalizedNativeRange(_loaded.Layout);
            string game = _loaded.Game == ElviraGame.Elvira2 ? "Elvira II" : _loaded.Game == ElviraGame.Elvira1 ? "Elvira I" : "Unknown";
            _sourceInfo.Text = $"{game}   |   {UiText.Get("Source")}: {Path.GetFileName(_loaded.SourcePath)}   |   {UiText.Get("Layout")}: {layout}   |   Offset: {(_loaded.FontOffset >= 0 ? $"0x{_loaded.FontOffset:X}" : "n/a")}   |   {LocalizedSlotSummary(_loaded)}{nativeRange}";
            if (!_glyphs.Any(g => g.HasEdited))
                SetStatusText(BuildLocalizedDetectionSummary() + " " + UiText.Get("EditedEmptyHint"));
        }

        _list.Invalidate();
        SelectGlyph();
        _preview.Invalidate();
        UpdateFontStatusSummary();
    }

    private static void ApplyTaggedLocalization(Control root)
    {
        foreach (Control c in root.Controls)
        {
            if (c.Tag is string key && !string.IsNullOrWhiteSpace(key))
                c.Text = UiText.Get(key);
            if (c.HasChildren)
                ApplyTaggedLocalization(c);
        }
    }

    private static string LocalizedLayout(RunVgaFontLayout layout) => (UiText.Language, layout) switch
    {
        (UiLanguage.Slovak, RunVgaFontLayout.OriginalPackedAscii80) => "Pôvodný PACKED ASCII font (80 znakov)",
        (UiLanguage.Slovak, RunVgaFontLayout.OriginalPackedAscii98) => "Pôvodný / PACKED font (98 znakov)",
        (UiLanguage.Slovak, RunVgaFontLayout.OriginalAscii98) => "Pôvodný ASCII font (98 znakov)",
        (UiLanguage.Slovak, RunVgaFontLayout.ExtendedCp852V5) => "V5 rozšírený CP852 font (224 slotov; 223 editovateľných; 0x81 rezervovaný)",
        (UiLanguage.Slovak, RunVgaFontLayout.ExtendedCp852RunIt) => "Elvira II split CP852 font (224 slotov; 223 editovateľných; 0x81 rezervovaný)",
        (UiLanguage.Czech, RunVgaFontLayout.OriginalPackedAscii80) => "Původní PACKED ASCII font (80 znaků)",
        (UiLanguage.Czech, RunVgaFontLayout.OriginalPackedAscii98) => "Původní / PACKED font (98 znaků)",
        (UiLanguage.Czech, RunVgaFontLayout.OriginalAscii98) => "Původní ASCII font (98 znaků)",
        (UiLanguage.Czech, RunVgaFontLayout.ExtendedCp852V5) => "V5 rozšířený CP852 font (224 slotů; 223 editovatelných; 0x81 rezervovaný)",
        (UiLanguage.Czech, RunVgaFontLayout.ExtendedCp852RunIt) => "Elvira II split CP852 font (224 slotů; 223 editovatelných; 0x81 rezervovaný)",
        (_, RunVgaFontLayout.OriginalPackedAscii80) => "Original PACKED ASCII 80-glyph",
        (_, RunVgaFontLayout.OriginalPackedAscii98) => "Original / Packed 98-glyph",
        (_, RunVgaFontLayout.OriginalAscii98) => "Original ASCII 98-glyph",
        (_, RunVgaFontLayout.ExtendedCp852V5) => "V5 extended CP852 (224 slots; 223 editable glyphs; 0x81 reserved)",
        (_, RunVgaFontLayout.ExtendedCp852RunIt) => "Elvira II split CP852 (224 slots; 223 editable glyphs; 0x81 reserved)",
        _ => UiText.Language == UiLanguage.Slovak ? "Neznáme / nepodporované" : UiText.Language == UiLanguage.Czech ? "Neznámé / nepodporované" : "Unknown / unsupported"
    };

    private static string LocalizedNativeRange(RunVgaFontLayout layout) => (UiText.Language, layout) switch
    {
        (UiLanguage.Slovak, RunVgaFontLayout.OriginalPackedAscii80) => " | Natívny packed rozsah: 0x2F-0x7E (vrátane 0-9/A-Z/a-z)",
        (UiLanguage.Slovak, RunVgaFontLayout.OriginalPackedAscii98) => " | Natívny rozsah: 0x20-0x81",
        (UiLanguage.Slovak, RunVgaFontLayout.OriginalAscii98) => " | Natívny rozsah: 0x20-0x81 (vrátane číslic/A-Z/a-z)",
        (UiLanguage.Slovak, RunVgaFontLayout.ExtendedCp852V5) or
        (UiLanguage.Slovak, RunVgaFontLayout.ExtendedCp852RunIt) => " | Editovateľné: 0x20-0x80, 0x82-0xFF; Rezervované: 0x81 (mazanie HUD)",
        (UiLanguage.Czech, RunVgaFontLayout.OriginalPackedAscii80) => " | Nativní packed rozsah: 0x2F-0x7E (včetně 0-9/A-Z/a-z)",
        (UiLanguage.Czech, RunVgaFontLayout.OriginalPackedAscii98) => " | Nativní rozsah: 0x20-0x81",
        (UiLanguage.Czech, RunVgaFontLayout.OriginalAscii98) => " | Nativní rozsah: 0x20-0x81 (včetně číslic/A-Z/a-z)",
        (UiLanguage.Czech, RunVgaFontLayout.ExtendedCp852V5) or
        (UiLanguage.Czech, RunVgaFontLayout.ExtendedCp852RunIt) => " | Editovatelné: 0x20-0x80, 0x82-0xFF; Rezervováno: 0x81 (mazání HUD)",
        (_, RunVgaFontLayout.OriginalPackedAscii80) => " | Native packed range: 0x2F-0x7E (0-9/A-Z/a-z included)",
        (_, RunVgaFontLayout.OriginalPackedAscii98) => " | Native range: 0x20-0x81",
        (_, RunVgaFontLayout.OriginalAscii98) => " | Native range: 0x20-0x81 (digits/A-Z/a-z included)",
        (_, RunVgaFontLayout.ExtendedCp852V5) or
        (_, RunVgaFontLayout.ExtendedCp852RunIt) => " | Editable: 0x20-0x80, 0x82-0xFF; Reserved: 0x81 (HUD erase)",
        _ => string.Empty
    };

    private static string LocalizedSlotSummary(FontLoadResult loaded)
    {
        if (loaded.ReservedGlyphSlots.Count == 0)
            return $"{UiText.Get("Loaded")}: {loaded.PhysicalSlotCount}";

        return UiText.Language switch
        {
            UiLanguage.Slovak => $"Fontové sloty: {loaded.PhysicalSlotCount}; editovateľné glyfy: {loaded.EditableGlyphCount}; rezervované: 0x81",
            UiLanguage.Czech => $"Fontové sloty: {loaded.PhysicalSlotCount}; editovatelné glyfy: {loaded.EditableGlyphCount}; rezervováno: 0x81",
            _ => $"Font slots: {loaded.PhysicalSlotCount}; editable glyphs: {loaded.EditableGlyphCount}; reserved: 0x81"
        };
    }

    private string BuildLocalizedDetectionSummary()
    {
        if (_loaded is null) return UiText.Get("FontReadyHint");
        return UiText.Language switch
        {
            UiLanguage.Slovak => $"Rozpoznaný font {_loaded.Game}: {_loaded.Layout}; {LocalizedSlotSummary(_loaded)}; offset {(_loaded.FontOffset >= 0 ? $"0x{_loaded.FontOffset:X}" : "n/a")}.",
            UiLanguage.Czech => $"Rozpoznaný font {_loaded.Game}: {_loaded.Layout}; {LocalizedSlotSummary(_loaded)}; offset {(_loaded.FontOffset >= 0 ? $"0x{_loaded.FontOffset:X}" : "n/a")}.",
            _ => $"Detected {_loaded.Game} font: {_loaded.Layout}; {LocalizedSlotSummary(_loaded)}; offset {(_loaded.FontOffset >= 0 ? $"0x{_loaded.FontOffset:X}" : "n/a")}.",
        };
    }

    private void UpdateFontStatusSummary()
    {
        int edited = _glyphs.Count(g => g.HasEdited && g.IsModified);
        int shifted = _glyphs.Count(g => g.HasEdited && (g.ShiftX != 0 || g.ShiftY != 0));
        _footerStatus.Text = string.Format(UiText.Get("FontStatusSummary"), _glyphs.Count, edited, shifted);
    }

    private void SetStatusText(string text, string? boldToken = null)
    {
        _status.SuspendLayout();
        _status.Clear();
        _status.SelectionFont = Font;
        _status.SelectionColor = SystemColors.ControlText;
        _status.AppendText(text);

        if (!string.IsNullOrWhiteSpace(boldToken))
        {
            int start = text.IndexOf(boldToken, StringComparison.OrdinalIgnoreCase);
            if (start >= 0)
            {
                _status.Select(start, boldToken.Length);
                _status.SelectionFont = new Font(Font, FontStyle.Bold);
                _status.Select(text.Length, 0);
                _status.SelectionFont = Font;
            }
        }

        _status.ResumeLayout();
    }

    private static string ToHex(byte[] bytes) => Convert.ToHexString(bytes);
}
