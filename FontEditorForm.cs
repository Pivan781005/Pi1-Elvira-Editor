namespace Pi1ElviraEditor;

internal sealed class FontEditorForm : Form
{
    // Used by the host only to prevent an installation switch from silently discarding work.
    internal bool HasPendingEditedGlyphs => _glyphs.Any(glyph => glyph.HasEdited);
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
    private readonly Label _variantInfo = new();
    private readonly Label _sourceInfo = new();
    private readonly Label _projectVariantInfo = new();
    private readonly CheckBox _advanced = new();
    private readonly Button _saveProject = new();
    private readonly Button _saveCopy = new();
    private readonly Button _createExtendedCp852 = new();
    private readonly Button _importFont = new();
    private readonly Button _exportFont = new();
    private readonly Button _copyOriginalToEdited = new();
    private readonly Button _reset = new();
    private readonly Button _clearGlyph = new();
    private readonly Button _copyHex = new();
    private readonly Button _shiftLeft = new();
    private readonly Button _shiftRight = new();
    private readonly Button _shiftUp = new();
    private readonly Button _shiftDown = new();
    private readonly Label _shiftInfo = new();
    private readonly ToolTip _toolTip = new();
    private readonly Button _open = new();
    private readonly BitmapFontPreviewControl _preview = new();
    private readonly Label _glyphHeading = new();
    private readonly ComboBox _previewMode = new();
    private readonly Button _fullCharacterSet = new();
    private GlyphModel? _current;
    private readonly string? _initialGameDirectory;
    private readonly Font _listFont = new("Consolas", 9.5f);
    private bool _isLoadingFont;
    private string? _lastAutoLoadedPath;
    private VariantEntry? _boundVariant;
    private string? _boundVariantPath;
    private bool _isManualSource;
    private readonly FontVariantService _fontVariants = new();
    private ProjectContext? _fontProject;
    private VariantContext? _fontVariant;
    private FontProjectState? _fontProjectState;
    private string _fontProjectCode = ProjectVariantOwnership.OriginalCode;

    // Narrow diagnostics used by the non-interactive regression smoke only.
    internal int SourceLoadCount { get; private set; }
    internal int PreviewRebuildCount { get; private set; }
    internal int ControlTreeCount => CountControls(this);
    internal string? CurrentSourcePath => _loaded?.SourcePath;
    internal string? BoundVariantExecutablePath => _boundVariantPath;
    internal bool IsManualSource => _isManualSource;
    internal bool IsSaveToProjectEnabledForTest => _saveProject.Enabled;
    internal bool HasFontVariantPresentationForTest => _projectVariantInfo.Parent is not null;
    internal VariantContext? BoundProjectVariantForTest => _fontVariant;
    internal int ProjectEditCountForTest => _fontProjectState?.Edits.Count ?? 0;
    internal int CurrentGlyphForTest => _current?.ByteValue ?? -1;
    internal GlyphMatrixControl EditedMatrixForTest => _editedMatrix;
    internal void LoadExecutableForTest(string path) => LoadRunVga(path, showErrors: false);
    internal bool GlyphHasEditedForTest(int byteValue) => _glyphs[byteValue].HasEdited;
    internal byte[] GlyphOriginalForTest(int byteValue) => _glyphs[byteValue].Original.ToArray();
    internal byte[] GlyphEditedForTest(int byteValue) => _glyphs[byteValue].Edited.ToArray();
    internal bool SelectGlyphForTest(int byteValue)
    {
        int index = _list.Items.IndexOf(byteValue);
        if (index < 0) return false;
        _list.SelectedIndex = index;
        return _current?.ByteValue == byteValue;
    }
    internal void ToggleCurrentPixelForTest(int row, int column) => EditedMatrixOnPixelToggled(this, new GlyphPixelToggleEventArgs(row, column));
    internal void ShiftCurrentGlyphForTest(int dx, int dy) => ShiftCurrentGlyph(dx, dy);
    // Headless-safe mirror of the Reset-glyph click path (PerformClick is a
    // no-op on an unshown form because the button cannot take selection).
    internal bool IsClearGlyphEnabledForTest => _clearGlyph.Enabled;
    internal void ClearCurrentGlyphForTest() => ClearCurrentGlyph();
    internal void ResetCurrentGlyphForTest()
    {
        if (_current is null || IsReservedHudEraseGlyph(_current)) return;
        _current.Reset();
        RefreshEditedActionState();
        UpdateMatricesOnly();
        RefreshListItem(_current.ByteValue);
    }

    public FontEditorForm(string? initialGameDirectory = null)
    {
        AutoScaleMode = AutoScaleMode.Dpi;
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
    }

    public void LoadFromGameDirectory(string? gameDirectory)
    {
        if (string.IsNullOrWhiteSpace(gameDirectory))
            return;

        string candidate = Path.Combine(gameDirectory, "RUNVGA.EXE");
        if (!File.Exists(candidate)) candidate = Path.Combine(gameDirectory, "RUNIT.EXE");
        if (!File.Exists(candidate))
            return;

        if (_isLoadingFont || IsAlreadyAutoLoaded(candidate)) return;
        try { LoadRunVga(candidate, showErrors: false); }
        catch { }
    }

    /// <summary>
    /// Binds the normal Font Editor source to the already-active text variant.
    /// The explicit ExeFile stored in the entry is deliberately the only filename
    /// used here; data-file, language and code naming must not influence this target.
    /// </summary>
    internal bool BindVariant(VariantEntry variant, string installationDirectory, bool showErrors)
    {
        ArgumentNullException.ThrowIfNull(variant);
        if (string.IsNullOrWhiteSpace(installationDirectory)) return false;

        _boundVariant = variant;
        _boundVariantPath = Path.Combine(Path.GetFullPath(installationDirectory), variant.ExeFile);
        _isManualSource = false;
        UpdateVariantContext();
        if (!File.Exists(_boundVariantPath))
        {
            ClearLoadedFont();
            string message = string.Format(UiText.Get("VariantExecutableMissing"), variant.ExeFile);
            _sourceInfo.Text = message;
            SetStatusText(message);
            if (showErrors)
                MessageBox.Show(this, message, UiText.Get("FontLoadTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        if (_isLoadingFont || IsAlreadyAutoLoaded(_boundVariantPath)) return true;
        LoadRunVga(_boundVariantPath, showErrors);
        return _loaded is not null;
    }

    /// <summary>Shows the project/variant projection only. It does not apply
    /// glyph bytes, invoke a bootstrap service, or create project storage.</summary>
    internal void BindProjectVariant(ProjectContext? project, VariantContext? variant) =>
        BindProjectVariant(project, variant, ProjectVariantOwnership.OriginalCode);

    internal void BindProjectVariant(ProjectContext? project, VariantContext? variant, string projectVariantCode)
    {
        if (project is null || variant is null || !ReferenceEquals(project, variant.Project))
        {
            _fontProject = null; _fontVariant = null; _fontProjectState = null; _fontProjectCode = ProjectVariantOwnership.OriginalCode;
            UpdateFontProjectPresentation();
            RefreshEditedActionState();
            return;
        }
        _fontProject = project; _fontVariant = variant; _fontProjectCode = ProjectVariantOwnership.NormalizeCode(project, projectVariantCode);
        FontProjectLoadResult loaded = _fontVariants.Load(project, _fontProjectCode);
        _fontProjectState = loaded.IsSuccess ? loaded.State : null;
        UpdateFontProjectPresentation();
        RefreshEditedActionState();
    }

    /// <summary>Returns the embedded editor to the neutral no-installation
    /// presentation without writing a project or touching an executable.</summary>
    internal void ClearActiveProjectBinding()
    {
        _boundVariant = null;
        _boundVariantPath = null;
        _isManualSource = false;
        _fontProject = null;
        _fontVariant = null;
        _fontProjectState = null;
        _fontProjectCode = ProjectVariantOwnership.OriginalCode;
        ClearLoadedFont();
        UpdateVariantContext();
        UpdateFontProjectPresentation();
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

        var top = new Panel { Dock = DockStyle.Top, Height = 116, Padding = new Padding(10, 8, 10, 4) };
        _open.Text = UiText.Get("OpenGameExe");
        _open.SetBounds(10, 8, 170, 30);
        _open.Click += (_, _) => OpenGameExe();

        _copyOriginalToEdited.Text = UiText.Get("CopyOriginalEdited");
        _copyOriginalToEdited.SetBounds(190, 8, 250, 30);
        _copyOriginalToEdited.TextAlign = ContentAlignment.MiddleCenter;
        _copyOriginalToEdited.UseCompatibleTextRendering = false;
        _copyOriginalToEdited.Enabled = false;
        _copyOriginalToEdited.Click += (_, _) => CopyOriginalToEdited();

        _importFont.Text = UiText.Get("ImportFont");
        _importFont.SetBounds(450, 8, 150, 30);
        _importFont.Enabled = false;
        _importFont.Click += (_, _) => ImportFont();

        _exportFont.Text = UiText.Get("ExportFont");
        _exportFont.SetBounds(610, 8, 140, 30);
        _exportFont.Enabled = false;
        _exportFont.Click += (_, _) => ExportFont();

        _createExtendedCp852.Text = UiText.Get("Font.CreateExtendedExe");
        _createExtendedCp852.SetBounds(10, 45, 225, 30);
        _createExtendedCp852.Enabled = false;
        _createExtendedCp852.Click += (_, _) => CreateExtendedCp852();

        _saveProject.Text = UiText.Get("SaveToProject");
        _saveProject.SetBounds(245, 45, 145, 30);
        _saveProject.Enabled = false;
        _saveProject.Click += (_, _) => SaveFontProjectState();

        _saveCopy.Text = UiText.Get("SaveCopyAs");
        _saveCopy.SetBounds(400, 45, 170, 30);
        _saveCopy.Enabled = false;
        _saveCopy.Click += (_, _) => SaveCopy();

        _variantInfo.SetBounds(780, 5, 620, 20);
        _variantInfo.AutoEllipsis = true;
        _variantInfo.Font = new Font(Font, FontStyle.Bold);
        _variantInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        _sourceInfo.SetBounds(780, 27, 620, 55);
        _sourceInfo.Text = UiText.Get("NoRunVga");
        _sourceInfo.AutoEllipsis = true;
        _sourceInfo.Font = new Font(Font, FontStyle.Bold);
        _sourceInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _projectVariantInfo.SetBounds(780, 84, 620, 22);
        _projectVariantInfo.AutoEllipsis = true;
        _projectVariantInfo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        _projectVariantInfo.ForeColor = SystemColors.ControlText;
        foreach (Button button in new[] { _open, _copyOriginalToEdited, _importFont, _exportFont, _createExtendedCp852, _saveProject, _saveCopy })
            ConfigureCenteredButton(button);
        top.Controls.AddRange(new Control[] { _open, _saveProject, _saveCopy, _createExtendedCp852, _copyOriginalToEdited, _importFont, _exportFont, _variantInfo, _sourceInfo, _projectVariantInfo });

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
        // Leave enough room for localized glyph classifications instead of
        // truncating them behind the fixed thumbnail/code columns.
        contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 560));
        contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        contentGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 430));
        contentGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        _glyphHeading.Text = UiText.Get("AllByteSlots");
        _glyphHeading.Tag = "AllByteSlots";
        _glyphHeading.Dock = DockStyle.Top;
        _glyphHeading.Height = 30;
        _glyphHeading.Font = new Font(Font, FontStyle.Bold);

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
        left.Controls.Add(_glyphHeading);

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

        _clearGlyph.Text = UiText.Get("ClearGlyph");
        _clearGlyph.SetBounds(465, 604, 110, 28);
        _clearGlyph.Enabled = false;
        _clearGlyph.Click += (_, _) => ClearCurrentGlyph();

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
        foreach (Button button in new[] { _reset, _clearGlyph, _copyHex, _shiftLeft, _shiftUp, _shiftDown, _shiftRight, _fullCharacterSet })
            ConfigureCenteredButton(button);

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
        main.Controls.Add(_clearGlyph);
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

    private static void ConfigureCenteredButton(Button button)
    {
        button.AutoSize = false;
        button.Height = 32;
        button.MinimumSize = new Size(0, 32);
        button.Padding = new Padding(6, 1, 6, 1);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.UseCompatibleTextRendering = false;
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
        // This is an inspection/source override only. It never mutates the active
        // VariantEntry and opening it must not redirect the text/variant context.
        LoadRunVga(dlg.FileName, showErrors: true);
        if (_loaded is not null)
        {
            _isManualSource = true;
            UpdateVariantContext();
        }
    }

    private void LoadRunVga(string path, bool showErrors)
    {
        if (_isLoadingFont) return;
        _isLoadingFont = true;
        try
        {
            Cursor = Cursors.WaitCursor;
            SourceLoadCount++;
            _loaded = RunVgaFontService.LoadRunVga(path);
            _lastAutoLoadedPath = Path.GetFullPath(path);
            _glyphs = _loaded.Glyphs;
            PopulateList();
            _saveProject.Enabled = false;
            _saveCopy.Enabled = false;
            RunVgaBootstrapState bootstrapState = RunVgaBootstrapService.DetectState(path);
            RunItBootstrapState runItState = RunItBootstrapService.DetectState(path);
            _createExtendedCp852.Enabled = bootstrapState is RunVgaBootstrapState.OriginalPacked or RunVgaBootstrapState.UnpackedBaseline ||
                runItState is RunItBootstrapState.OriginalPacked or RunItBootstrapState.CanonicalUnpackedAscii98;
            _copyOriginalToEdited.Enabled = _loaded.CanInitializeEdited;
            _importFont.Enabled = _loaded.HasFullCp852Font;
            _exportFont.Enabled = false;

            _sourceInfo.Text = BuildSourceSummary(_loaded, path);
            SetStatusText(UiText.Get("EditedEmptyHint"));
        }
        catch (Exception ex)
        {
            _loaded = null;
            _saveProject.Enabled = false;
            _saveCopy.Enabled = false;
            _createExtendedCp852.Enabled = false;
            _copyOriginalToEdited.Enabled = false;
            _importFont.Enabled = false;
            _exportFont.Enabled = false;
            _sourceInfo.Text = UiText.Get("Font.LoadFailed");
            SetStatusText(ex.Message);
            if (showErrors)
                MessageBox.Show(this, ex.Message, UiText.Get("FontLoadTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            Cursor = Cursors.Default;
            _isLoadingFont = false;
        }
    }

    private bool IsAlreadyAutoLoaded(string path)
    {
        try
        {
            string full = Path.GetFullPath(path);
            return _lastAutoLoadedPath is not null && full.Equals(_lastAutoLoadedPath, StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    private void ClearLoadedFont()
    {
        _loaded = null;
        _lastAutoLoadedPath = null;
        _glyphs = GlyphRepository.CreateAllCp852Slots().ToList();
        _current = null;
        _saveProject.Enabled = false;
        _saveCopy.Enabled = false;
        _createExtendedCp852.Enabled = false;
        _copyOriginalToEdited.Enabled = false;
        _importFont.Enabled = false;
        _exportFont.Enabled = false;
        _glyphHeading.Text = UiText.Get("AllByteSlots");
        PopulateList();
        SelectGlyph();
        UpdatePreview();
    }

    private void UpdateVariantContext()
    {
        if (_boundVariant is null)
        {
            _variantInfo.Text = string.Empty;
            return;
        }

        string text = $"{UiText.Get("Variant")}: {_boundVariant.DisplayName} | {UiText.Get("Source")}: {_boundVariant.ExeFile}";
        _variantInfo.Text = _isManualSource ? text + $" ({UiText.Get("ManualSource")})" : text;
    }

    private void PopulateList()
    {
        int selectedByte = _list.SelectedIndex >= 0 ? (int)_list.Items[_list.SelectedIndex] : 0x20;
        _list.BeginUpdate();
        _list.Items.Clear();
        bool fullCp852 = _loaded?.HasFullCp852Font == true;
        if (_loaded is not null)
            _glyphHeading.Text = string.Format(UiText.Get("FontGlyphRange"), _loaded.FirstByteValue, _loaded.LastByteValue);
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
        _clearGlyph.Enabled = CanBeginEditingCurrentGlyph();
        _copyHex.Enabled = _current.HasEdited;
        SetShiftButtonsEnabled(CanBeginEditingCurrentGlyph());
        UpdateFontProjectPresentation();
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
            ? FontSlotMetadata.GetCanonicalBytes(_loaded!, _current.ByteValue)
            : (_current.HasEdited ? _current.Edited : new byte[8]);
        // V8.3: editability answers "may the user begin editing", not "does
        // a working copy already exist". A fresh supported glyph keeps its
        // matrix (and shift actions) live so the first real click reaches
        // the lazy working-copy handler instead of being swallowed.
        _editedMatrix.SetGlyph(edited, CanBeginEditingCurrentGlyph(), _advanced.Checked && !reserved);
        _originalHex.Text = UiText.Get("OriginalHex") + " " + ToHex(_current.Original);
        _editedHex.Text = _current.HasEdited ? UiText.Get("EditedHex") + " " + ToHex(_current.Edited) : UiText.Get("EditedHex") + " [EMPTY]";
        _shiftInfo.Text = _current.HasEdited
            ? string.Format(UiText.Get("GlyphShiftInfo"), _current.ShiftX, _current.ShiftY, _current.OutsidePixelCount)
            : string.Format(UiText.Get("GlyphShiftInfo"), 0, 0, 0);
        SetShiftButtonsEnabled(CanBeginEditingCurrentGlyph());
        _clearGlyph.Enabled = CanBeginEditingCurrentGlyph();
        UpdatePreview();
    }

    /// <summary>V8.3b Clear glyph: replaces the Edited working copy with an
    /// explicit all-zero 8-byte bitmap. Same mutation gates as pixel/shift
    /// editing (reserved/unsupported slots stay disabled); Reset semantics
    /// are untouched.</summary>
    private void ClearCurrentGlyph()
    {
        if (_current is null || !CanBeginEditingCurrentGlyph()) return;
        _current.ClearToZero();
        RefreshEditedActionState();
        UpdateMatricesOnly();
        UpdateFontStatusSummary();
        RefreshListItem(_current.ByteValue);
    }

    /// <summary>V8.3 editability model: the EDITED matrix accepts a first
    /// mutation when a source bitmap exists, the slot is supported and not
    /// reserved. HasEdited is deliberately not part of this predicate.</summary>
    private bool CanBeginEditingCurrentGlyph() =>
        _current is not null && !IsReservedHudEraseGlyph(_current) &&
        _loaded is not null && _loaded.CanInitializeEdited && _current.IsLoadedFromSource;

    private void UpdateFontProjectPresentation()
    {
        if (_fontProject is null || _fontVariant is null || _fontProjectState is null)
        {
            _projectVariantInfo.Text = UiText.Get("Font.ProjectUnavailable");
            return;
        }
        int code = _current?.ByteValue ?? 0x20;
        FontSlotProjection slot = _fontVariants.GetSlotProjection(_fontVariant, code);
        if (slot.Status == FontApplicabilityStatus.Protected)
        {
            _projectVariantInfo.Text = string.Format(UiText.Get("Font.ProtectedGlyph"), _fontVariant.DisplayName);
            return;
        }
        if (slot.Status == FontApplicabilityStatus.Unsupported)
        {
            _projectVariantInfo.Text = string.Format(UiText.Get("Font.UnsupportedGlyph"), _fontVariant.DisplayName);
            return;
        }
        FontProjectEdit? edit = _fontProjectState.Edits.SingleOrDefault(value => value.Identity.ByteValue == code);
        string scope = edit is null ? UiText.Get("Font.NoProjectEdit") : edit.Scope == FontEditScope.Shared ? UiText.Get("Font.SharedProjectGlyph") : UiText.Get("Font.CurrentRuntimeProjectGlyph");
        string bank = slot.Bank switch
        {
            FontRuntimeBank.Low => UiText.Get("Font.LowBank"),
            FontRuntimeBank.High => UiText.Get("Font.HighBank"),
            FontRuntimeBank.Full => UiText.Get("Font.FullBank"),
            _ => UiText.Get("Font.Bank")
        };
        _projectVariantInfo.Text = _fontVariant.DisplayName + " — " + scope + "; " + bank;
    }

    /// <summary>V8.2 lazy working copy: the first mutating action on a normal
    /// editable glyph clones ORIGINAL into the edited working copy, then the
    /// requested mutation applies in the same operation. Viewing/selecting
    /// never creates state; protected 0x81 never becomes editable.</summary>
    private bool EnsureEditedWorkingCopyForMutation()
    {
        // Same gate as the matrix editability model above, so every gesture
        // the control accepts also succeeds here in the same operation.
        if (_current is null || !CanBeginEditingCurrentGlyph()) return false;
        if (_current.HasEdited) return true;
        _current.CopyOriginalToEdited();
        RefreshEditedActionState();
        RefreshListItem(_current.ByteValue);
        return true;
    }

    private void EditedMatrixOnPixelToggled(object? sender, GlyphPixelToggleEventArgs e)
    {
        if (_current is null || IsReservedHudEraseGlyph(_current)) return;
        if (!EnsureEditedWorkingCopyForMutation()) return;
        _current.ToggleEditedPixel(e.Row, e.Column);
        UpdateMatricesOnly();
        UpdateFontStatusSummary();
        RefreshListItem(_current.ByteValue);
    }

    private void ShiftCurrentGlyph(int dx, int dy)
    {
        if (_current is null || IsReservedHudEraseGlyph(_current)) return;
        if (!EnsureEditedWorkingCopyForMutation()) return;
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

    /// <summary>Persists the in-memory edited glyphs as edition font project
    /// state. Original game executables are never modified: the state
    /// materializes into owned VARIANTS executables through Build Variant.
    /// Edits are scoped to the bound variant runtime so other runtimes keep
    /// building deterministically.</summary>
    private void SaveFontProjectState()
    {
        if (_fontProject is null || _fontVariant is null || _fontProjectState is null) return;
        if (ProjectVariantOwnership.IsOriginal(_fontProjectCode))
        {
            MessageBox.Show(this, UiText.Get("Workflow.OriginalReadOnly"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (_loaded is null) return;
        if (!IsLoadedGameForProject())
        {
            MessageBox.Show(this, UiText.Get("Font.GameMismatch"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            FontProjectState state = _fontProjectState;
            int saved = 0;
            foreach (GlyphModel glyph in _glyphs)
            {
                if (!glyph.HasEdited || !glyph.IsLoadedFromSource) continue;
                if (glyph.ByteValue == FontSlotMetadata.HudEraseGlyph) continue;
                var edit = FontProjectEdit.Create(new(glyph.ByteValue), glyph.Edited, FontEditScope.RuntimeSpecific, _fontVariant.RuntimeKind);
                state = _fontVariants.SetEdit(_fontProject, state, edit);
                saved++;
            }
            if (saved == 0)
            {
                MessageBox.Show(this, UiText.Get("NoGlyphChanges"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            FontProjectSaveResult result = _fontVariants.Save(_fontProject, _fontProjectCode, state);
            if (!result.Succeeded)
            {
                MessageBox.Show(this, result.Detail ?? UiText.Get("Font.ProjectSaveFailed"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _fontProjectState = state;
            SetStatusText(string.Format(UiText.Get("Font.ProjectSaved"), saved));
            MessageBox.Show(this, string.Format(UiText.Get("Font.ProjectSaved"), saved), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshEditedActionState();
            UpdateFontProjectPresentation();
            _list.Invalidate();
            SelectGlyph();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool IsLoadedGameForProject()
    {
        if (_loaded is null || _fontProject is null) return false;
        return (_loaded.Game, _fontProject.GameProfile) switch
        {
            (ElviraGame.Elvira1, ElviraGameProfile.Elvira1) => true,
            (ElviraGame.Elvira2, ElviraGameProfile.Elvira2) => true,
            _ => false
        };
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

    /// <summary>Creates an extended-CP852 executable as a NEW user-chosen
    /// file from a verified packed/baseline source. The pristine GameRoot
    /// source is only read; this never patches a game installation.
    /// Use Save to project + Build Variant for owned variant executables.</summary>
    private void CreateExtendedCp852()
    {
        if (_loaded is null) return;
        RunItBootstrapState runItState = RunItBootstrapService.DetectState(_loaded.SourcePath);
        if (runItState != RunItBootstrapState.Unsupported)
        {
            CreateExtendedCp852RunIt(runItState);
            return;
        }
        RunVgaBootstrapState state = RunVgaBootstrapService.DetectState(_loaded.SourcePath);
        if (state == RunVgaBootstrapState.ExtendedCp852V5)
        {
            MessageBox.Show(this, UiText.Get("Font.AlreadyV5"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (state is not (RunVgaBootstrapState.OriginalPacked or RunVgaBootstrapState.UnpackedBaseline))
        {
            MessageBox.Show(this, UiText.Get("Font.UnsupportedRunVga"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        string sourceKind = state == RunVgaBootstrapState.OriginalPacked ? UiText.Get("Font.VerifiedPackedOriginal") : UiText.Get("Font.VerifiedUnpackedBaseline");
        DialogResult answer = MessageBox.Show(this,
            string.Format(UiText.Get("Font.ConfirmCreateV5"), sourceKind),
            UiText.Get("Font.CreateV5Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (answer != DialogResult.Yes) return;

        using var dlg = new SaveFileDialog
        {
            Title = UiText.Get("Font.SaveV5Title"),
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
                string.Format(UiText.Get("Font.V5Created"), result.OutputPath, result.Sha256, result.UsedEditedFont ? UiText.Get("Font.CurrentEditedGlyphs") : UiText.Get("Font.DefaultGlyphSlots")),
                UiText.Get("Font.CreateV5Title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("Font.CreateV5Title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show(this, UiText.Get("Font.AlreadyRunIt"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        if (state is not (RunItBootstrapState.OriginalPacked or RunItBootstrapState.CanonicalUnpackedAscii98))
        {
            MessageBox.Show(this, UiText.Get("Font.UnsupportedRunIt"), UiText.Get("FontTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        string sourceKind = state == RunItBootstrapState.OriginalPacked ? UiText.Get("Font.VerifiedPackedRunIt") : UiText.Get("Font.CanonicalUnpackedRunIt");
        if (MessageBox.Show(this, string.Format(UiText.Get("Font.ConfirmCreateRunIt"), sourceKind),
            UiText.Get("Font.CreateRunItTitle"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
        using var dlg = new SaveFileDialog
        {
            Title = UiText.Get("Font.SaveRunItTitle"),
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
            MessageBox.Show(this, string.Format(UiText.Get("Font.RunItCreated"), result.OutputPath, result.Sha256),
                UiText.Get("Font.CreateRunItTitle"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("Font.CreateRunItTitle"), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                glyph.ReplaceReservedCanonical(FontSlotMetadata.GetCanonicalBytes(_loaded!, glyph.ByteValue));
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
        bool canSaveToProject = _fontProject is not null && _fontVariant is not null && _fontProjectState is not null
            && !ProjectVariantOwnership.IsOriginal(_fontProjectCode);
        _saveProject.Enabled = canSaveToProject && anyEdited;
        _toolTip.SetToolTip(_saveProject, _fontProject is null || _fontVariant is null || _fontProjectState is null
            ? UiText.Get("Font.ProjectUnavailable")
            : ProjectVariantOwnership.IsOriginal(_fontProjectCode) ? UiText.Get("Workflow.OriginalReadOnly") : string.Empty);
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
        PreviewRebuildCount++;
        int code = _current?.ByteValue ?? 65;
        FontPreviewMode mode = _previewMode.SelectedIndex switch
        {
            2 => FontPreviewMode.Original,
            1 => FontPreviewMode.Edited,
            _ => FontPreviewMode.SideBySide
        };
        _preview.SetPreview(_glyphs, code, mode);
    }

    private static int CountControls(Control root) =>
        1 + root.Controls.Cast<Control>().Sum(CountControls);

    public void ApplyLanguage()
    {
        Text = UiText.Get("FontTitle");
        _open.Text = UiText.Get("OpenGameExe");
        _saveProject.Text = UiText.Get("SaveToProject");
        _createExtendedCp852.Text = UiText.Get("Font.CreateExtendedExe");
        _saveCopy.Text = UiText.Get("SaveCopyAs");
        _copyOriginalToEdited.Text = UiText.Get("CopyOriginalEdited");
        _importFont.Text = UiText.Get("ImportFont");
        _exportFont.Text = UiText.Get("ExportFont");
        _fullCharacterSet.Text = UiText.Get("FullCharacterSet");
        _advanced.Text = UiText.Get("AdvancedColumns");
        _reset.Text = UiText.Get("ResetGlyph");
        _clearGlyph.Text = UiText.Get("ClearGlyph");
        _copyHex.Text = UiText.Get("CopyHex");
        _toolTip.SetToolTip(_reset, UiText.Get("ResetGlyphTip"));
        _toolTip.SetToolTip(_clearGlyph, UiText.Get("ClearGlyphTip"));
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
            _glyphHeading.Text = UiText.Get("AllByteSlots");
            if (_boundVariant is null)
                _sourceInfo.Text = UiText.Get("NoRunVga");
            UpdateVariantContext();
            SetStatusText(UiText.Get("FontReadyHint"));
        }
        else
        {
            _glyphHeading.Text = string.Format(UiText.Get("FontGlyphRange"), _loaded.FirstByteValue, _loaded.LastByteValue);
            _sourceInfo.Text = BuildSourceSummary(_loaded, _loaded.SourcePath);
            UpdateVariantContext();
            // A loaded V5/V2 source keeps the protected 0x81 glyph initialized
            // in-memory. That is not a user working copy and must not prevent
            // the localized empty-editor guidance from being refreshed.
            if (!_glyphs.Any(g => g.HasEdited && !IsReservedHudEraseGlyph(g)))
                SetStatusText(UiText.Get("EditedEmptyHint"));
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

    private static string LocalizedLayout(RunVgaFontLayout layout) => layout switch
    {
        RunVgaFontLayout.OriginalPackedAscii80 => UiText.Get("Font.Layout.OriginalPackedAscii80"),
        RunVgaFontLayout.OriginalPackedAscii98 => UiText.Get("Font.Layout.OriginalPackedAscii98"),
        RunVgaFontLayout.OriginalAscii98 => UiText.Get("Font.Layout.OriginalAscii98"),
        RunVgaFontLayout.ExtendedCp852V5 => UiText.Get("Font.Layout.ExtendedCp852V5"),
        RunVgaFontLayout.ExtendedCp852RunEga => UiText.Get("Font.Layout.ExtendedCp852RunEga"),
        RunVgaFontLayout.ExtendedCp852RunIt => UiText.Get("Font.Layout.ExtendedCp852RunIt"),
        _ => UiText.Get("Font.Layout.Unknown")
    };

    private static string LocalizedNativeRange(RunVgaFontLayout layout) => layout switch
    {
        RunVgaFontLayout.OriginalPackedAscii80 => UiText.Get("Font.Range.OriginalPackedAscii80"),
        RunVgaFontLayout.OriginalPackedAscii98 => UiText.Get("Font.Range.OriginalPackedAscii98"),
        RunVgaFontLayout.OriginalAscii98 => UiText.Get("Font.Range.OriginalAscii98"),
        RunVgaFontLayout.ExtendedCp852V5 or RunVgaFontLayout.ExtendedCp852RunEga or RunVgaFontLayout.ExtendedCp852RunIt => UiText.Get("Font.Range.ExtendedCp852"),
        _ => string.Empty
    };

    private static string LocalizedSlotSummary(FontLoadResult loaded)
    {
        if (loaded.ReservedGlyphSlots.Count == 0)
            return $"{UiText.Get("Loaded")}: {loaded.PhysicalSlotCount}";

        return string.Format(UiText.Get("Font.SlotSummary"), loaded.PhysicalSlotCount, loaded.EditableGlyphCount);
    }

    private static string BuildSourceSummary(FontLoadResult loaded, string path)
    {
        string game = loaded.Game == ElviraGame.Elvira2 ? "Elvira II" : loaded.Game == ElviraGame.Elvira1 ? "Elvira I" : "Unknown";
        return string.Format(
            UiText.Get("FontSourceSummary"),
            UiText.Get("Source"),
            Path.GetFileName(path),
            UiText.Get("FontMode"),
            $"{game} — {LocalizedLayout(loaded.Layout)}",
            LocalizedSlotSummary(loaded));
    }

    private string BuildLocalizedDetectionSummary()
    {
        if (_loaded is null) return UiText.Get("FontReadyHint");
        return string.Format(UiText.Get("Font.DetectedSummary"), _loaded.Game, LocalizedLayout(_loaded.Layout), LocalizedSlotSummary(_loaded));
    }

    private void UpdateFontStatusSummary()
    {
        if (_loaded is null)
        {
            _footerStatus.Text = string.Empty;
            return;
        }

        _footerStatus.Text = string.Format(
            UiText.Get("FontFooterSummary"),
            _loaded.PhysicalSlotCount,
            _loaded.EditableGlyphCount,
            _loaded.ReservedGlyphSlots.Count);
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
