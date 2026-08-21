using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace ElviraVgaEditor;

internal sealed class MainForm : Form
{
    private readonly TextBox txtGameDir = new();
    private readonly Button btnBrowseGame = new();
    private readonly ComboBox cmbZone = new();
    private readonly ComboBox cmbPalette = new();
    private readonly Label lblPaletteCaption = new();
    private readonly Label lblLanguageCaption = new();
    private readonly Button btnReload = new();
    private readonly DataGridView grid = new();
    private readonly PictureBox preview = new();
    private readonly Panel previewScroll = new();
    private readonly Panel previewViewport = new();
    private readonly ComboBox cmbPreviewZoom = new();
    private readonly Button btnReloadPreview = new();
    private readonly Label lblPreviewZoom = new();
    private readonly Label lblMeta = new();
    private readonly Label lblStatus = new();
    private readonly Button btnAbout = new();
    private readonly Button btnTextEditor = new();
    private readonly ComboBox cmbUiLanguage = new();
    private readonly TabControl tabs = new();
    private readonly TabPage tabVga = new();
    private readonly TabPage tabText = new();

    private readonly DataGridView textGrid = new();
    private readonly TextBox txtSearch = new();
    private readonly ComboBox cmbTextEncoding = new();
    private readonly Button btnReloadTexts = new();
    private readonly Button btnSaveTexts = new();
    private readonly Button btnBackToVga = new();
    private readonly Label lblTextStatus = new();

    private readonly List<GamePcStringEntry> _gamePcEntries = new();
    private readonly Dictionary<int, string> _gamePcEdits = new();
    private readonly SplitContainer mainSplit = new();
    private readonly Button btnReplace = new();
    private readonly Button btnClearEdit = new();
    private readonly Button btnExport = new();
    private readonly Button btnDeploy = new();
    private readonly Button btnRestore = new();
    private readonly CheckBox chkZoom = new();

    private string? _currentVga;
    private byte[]? _currentData;
    private ParsedTable? _table;
    private readonly Dictionary<int, string> _edits = new();
    private readonly List<ElviraPaletteBank> _paletteBanks = new();
    private Color[] _activePalette = ElviraPaletteLoader.DiagnosticPalette();

    public MainForm()
    {
        Text = UiText.Get("AppTitle");
        Width = 1450;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1100, 700);

        UiText.SetLanguage(UiLanguage.English);
        BuildUi();

        txtGameDir.Text = @"C:\Games\GOG\Elvira";
        Shown += (_, _) =>
        {
            UiText.SetLanguage(UiLanguage.English);
            if (cmbUiLanguage.Items.Count > 1)
                cmbUiLanguage.SelectedIndex = 1;
            ApplyLanguage();
            ScanGameFolder();

LoadGamePcTexts();

            BeginInvoke(new Action(() =>
            {
                ApplySafeHalfSplit();
            }));
        };
    }






    private void DrawCenteredLanguageItem(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0)
            return;

        e.DrawBackground();

        string value = cmbUiLanguage.Items[e.Index]?.ToString() ?? string.Empty;

        TextRenderer.DrawText(
            e.Graphics,
            value,
            cmbUiLanguage.Font,
            e.Bounds,
            e.ForeColor,
            TextFormatFlags.HorizontalCenter |
            TextFormatFlags.VerticalCenter |
            TextFormatFlags.SingleLine |
            TextFormatFlags.NoPrefix);

        e.DrawFocusRectangle();
    }

    private void BuildTextEditorUi()
    {
        var topText = new Panel
        {
            Dock = DockStyle.Top,
            Height = 42,
            Padding = new Padding(6)
        };

        var lblSearch = new Label { Text = UiText.Get("Search"), AutoSize = true };
        lblSearch.SetBounds(8, 12, 55, 20);

        txtSearch.SetBounds(65, 7, 260, 26);
        txtSearch.TextChanged += (_, _) => RefreshTextGrid();

        var lblEnc = new Label { Text = UiText.Get("Encoding"), AutoSize = true };
        lblEnc.SetBounds(340, 12, 75, 20);

        cmbTextEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTextEncoding.Items.AddRange(new object[] { "CP852", "Windows-1250", "Latin1/Raw" });
        cmbTextEncoding.SelectedIndex = 0;
        cmbTextEncoding.SetBounds(420, 7, 130, 26);
        cmbTextEncoding.SelectedIndexChanged += (_, _) => RefreshTextGrid();

        btnReloadTexts.Text = UiText.Get("ReloadTexts");
        btnReloadTexts.SetBounds(695, 6, 130, 28);
        btnReloadTexts.Click += (_, _) => LoadGamePcTexts();

        btnBackToVga.Text = UiText.Get("BackToVga");
        btnBackToVga.SetBounds(565, 6, 120, 28);
        btnBackToVga.Click += (_, _) => tabs.SelectedTab = tabVga;

        btnSaveTexts.Text = UiText.Get("SaveTexts");
        btnSaveTexts.SetBounds(835, 6, 150, 28);
        btnSaveTexts.Click += (_, _) => SaveGamePcTexts();

        topText.Controls.AddRange(new Control[]
        {
            lblSearch, txtSearch, lblEnc, cmbTextEncoding, btnBackToVga, btnReloadTexts, btnSaveTexts
        });

        textGrid.Dock = DockStyle.Fill;
        textGrid.AllowUserToAddRows = false;
        textGrid.AllowUserToDeleteRows = false;
        textGrid.RowHeadersVisible = false;
        textGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        textGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        textGrid.Columns.Add("Index", UiText.Get("TextIndex"));
        textGrid.Columns.Add("Offset", UiText.Get("TextOffset"));
        textGrid.Columns.Add("Length", UiText.Get("Length"));
        textGrid.Columns.Add("Text", UiText.Get("Text"));
        textGrid.Columns["Index"].Width = 70;
        textGrid.Columns["Offset"].Width = 110;
        textGrid.Columns["Length"].Width = 80;
        textGrid.Columns["Text"].Width = 800;
        textGrid.CellEndEdit += (_, e) =>
        {
            if (e.RowIndex < 0) return;
            var row = textGrid.Rows[e.RowIndex];
            if (row.Tag is not GamePcStringEntry entry) return;
            string value = row.Cells["Text"].Value?.ToString() ?? "";
            _gamePcEdits[entry.Index] = value;
            row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
        };

        lblTextStatus.Dock = DockStyle.Bottom;
        lblTextStatus.Height = 28;
        lblTextStatus.Padding = new Padding(6, 6, 0, 0);

        // The main application toolbar overlays the top ~75 px of the tab area.
        // Use the same 75 px content offset as the VGA editor so the text-editor
        // toolbar (Back to VGA / Reload GAMEPC / Save GAMEPC) stays visible.
        var textHost = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 75, 0, 0)
        };

        textHost.Controls.Add(textGrid);
        textHost.Controls.Add(topText);
        textHost.Controls.Add(lblTextStatus);
        tabText.Controls.Add(textHost);
    }

    private string GamePcPath => Path.Combine(txtGameDir.Text.Trim(), "GAMEPC");

    private void LoadGamePcTexts()
    {
        try
        {
            _gamePcEntries.Clear();
            _gamePcEdits.Clear();

            if (!File.Exists(GamePcPath))
            {
                lblTextStatus.Text = UiText.Get("NoGamepc");
                textGrid.Rows.Clear();
                return;
            }

            _gamePcEntries.AddRange(GamePcTextEditor.LoadEntries(GamePcPath));
            RefreshTextGrid();
            lblTextStatus.Text = $"GAMEPC: {_gamePcEntries.Count} strings";
        }
        catch (Exception ex)
        {
            lblTextStatus.Text = ex.Message;
        }
    }

    private void RefreshTextGrid()
    {
        if (_gamePcEntries.Count == 0)
            return;

        var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
        string filter = txtSearch.Text.Trim();

        textGrid.Rows.Clear();

        foreach (var entry in _gamePcEntries)
        {
            string value = _gamePcEdits.TryGetValue(entry.Index, out var edited)
                ? edited
                : entry.Decode(enc);

            if (!string.IsNullOrEmpty(filter) &&
                value.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) < 0 &&
                entry.Index.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                continue;

            int rowIndex = textGrid.Rows.Add(
                entry.Index,
                $"0x{entry.Offset:X}",
                entry.ByteLength,
                value);

            textGrid.Rows[rowIndex].Tag = entry;

            if (_gamePcEdits.ContainsKey(entry.Index))
                textGrid.Rows[rowIndex].DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
        }
    }

    private void SaveGamePcTexts()
    {
        try
        {
            var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
            GamePcTextEditor.SaveInPlace(GamePcPath, _gamePcEdits, _gamePcEntries, enc);
            lblTextStatus.Text = UiText.Get("TextSaved");
            LoadGamePcTexts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, UiText.Get("AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblTextStatus.Text = ex.Message;
        }
    }

    private void ApplyLanguage()
    {
        Text = UiText.Get("AppTitle");
        tabVga.Text = UiText.Get("VgaTab");
        tabText.Text = UiText.Get("TextTab");
        btnBrowseGame.Text = UiText.Get("Browse");
        btnReload.Text = UiText.Get("Refresh");
        chkZoom.Text = UiText.Get("PixelZoom");
        btnReplace.Text = UiText.Get("ReplacePng");
        btnExport.Text = UiText.Get("ExportPng");
        btnRestore.Text = UiText.Get("RestoreOriginal");
        btnReloadTexts.Text = UiText.Get("ReloadTexts");
        btnSaveTexts.Text = UiText.Get("SaveTexts");

        if (grid.Columns.Count >= 6)
        {
            grid.Columns[0].HeaderText = UiText.Get("Id");
            grid.Columns[1].HeaderText = UiText.Get("Offset");
            grid.Columns[2].HeaderText = UiText.Get("Type");
            grid.Columns[3].HeaderText = UiText.Get("Resolution");
            grid.Columns[4].HeaderText = UiText.Get("Flags");
            grid.Columns[5].HeaderText = UiText.Get("Edit");
        }

        if (textGrid.Columns.Contains("Index"))
        {
            textGrid.Columns["Index"].HeaderText = UiText.Get("TextIndex");
            textGrid.Columns["Offset"].HeaderText = UiText.Get("TextOffset");
            textGrid.Columns["Length"].HeaderText = UiText.Get("Length");
            textGrid.Columns["Text"].HeaderText = UiText.Get("Text");
        }
        if (cmbZone.Items.Count > 0)
            lblStatus.Text = string.Format(UiText.Get("ZonesFound"), cmbZone.Items.Count);
        btnAbout.Text = UiText.Get("About");
        btnClearEdit.Text = UiText.Get("CancelEdit");
        btnDeploy.Text = UiText.Get("ApplyGame");
        lblPaletteCaption.Text = UiText.Get("Palette");
        lblLanguageCaption.Text = UiText.Get("Language");
        RefreshPaletteDisplayLanguage();
        btnTextEditor.Text = UiText.Get("OpenTextEditor");
        btnReloadPreview.Text = UiText.Get("ReloadPreview");
        btnBackToVga.Text = UiText.Get("BackToVga");
    }

    private void ApplySafeHalfSplit()
    {
        if (mainSplit.IsDisposed || mainSplit.ClientSize.Width <= 0)
            return;

        int width = mainSplit.ClientSize.Width;
        int splitter = Math.Max(1, mainSplit.SplitterWidth);

        // Min sizes are assigned only after the control has a real width.
        // Keep them conservative so even narrow windows remain valid.
        int desiredMin = 260;
        int maxPossibleMin = Math.Max(0, (width - splitter) / 3);
        int safeMin = Math.Min(desiredMin, maxPossibleMin);

        mainSplit.Panel1MinSize = safeMin;
        mainSplit.Panel2MinSize = safeMin;

        int minDistance = mainSplit.Panel1MinSize;
        int maxDistance = width - splitter - mainSplit.Panel2MinSize;

        if (maxDistance < minDistance)
        {
            mainSplit.Panel1MinSize = 0;
            mainSplit.Panel2MinSize = 0;
            minDistance = 0;
            maxDistance = Math.Max(0, width - splitter);
        }

        int desired = Math.Max(0, (width - splitter) / 2);
        int clamped = Math.Max(minDistance, Math.Min(desired, maxDistance));

        // Only assign if WinForms considers the interval valid.
        if (maxDistance >= minDistance)
        {
            try
            {
                mainSplit.SplitterDistance = clamped;
            }
            catch (InvalidOperationException)
            {
                // Transient WinForms layout state. Retry once after layout completes.
                BeginInvoke(new Action(() =>
                {
                    int w = mainSplit.ClientSize.Width;
                    int s = Math.Max(1, mainSplit.SplitterWidth);
                    int min = mainSplit.Panel1MinSize;
                    int max = w - s - mainSplit.Panel2MinSize;

                    if (max >= min)
                    {
                        int d = Math.Max(min, Math.Min((w - s) / 2, max));
                        mainSplit.SplitterDistance = d;
                    }
                }));
            }
        }
    }

    private void BuildUi()
    {
        var top = new Panel { Dock = DockStyle.Top, Height = 78, Padding = new Padding(8) };

        txtGameDir.SetBounds(8, 8, 650, 26);
        btnBrowseGame.Text = UiText.Get("Browse");
        btnBrowseGame.SetBounds(665, 7, 130, 28);
        btnBrowseGame.Click += (_, _) => BrowseGame();

        var zoneLabel = new Label { Text = "xNN2.VGA:", AutoSize = true };
        zoneLabel.SetBounds(8, 46, 70, 20);

        cmbZone.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbZone.SetBounds(80, 42, 160, 28);
        cmbZone.SelectedIndexChanged += (_, _) => LoadSelectedZone();

        btnReload.Text = UiText.Get("Refresh");
        btnReload.SetBounds(250, 41, 90, 28);
        btnReload.Click += (_, _) => ScanGameFolder();

        lblPaletteCaption.Text = UiText.Get("Palette");
        lblPaletteCaption.AutoSize = true;
        lblPaletteCaption.SetBounds(350, 46, 48, 20);

        cmbPalette.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPalette.SetBounds(400, 42, 130, 28);
        cmbPalette.SelectedIndexChanged += (_, _) => PaletteChanged();

        lblLanguageCaption.Text = UiText.Get("Language");
        lblLanguageCaption.AutoSize = true;
        lblLanguageCaption.SetBounds(545, 46, 52, 20);

        cmbUiLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUiLanguage.DrawMode = DrawMode.OwnerDrawFixed;
        cmbUiLanguage.ItemHeight = 20;
        cmbUiLanguage.DrawItem += DrawCenteredLanguageItem;
        cmbUiLanguage.SetBounds(615, 42, 145, 28);
        cmbUiLanguage.Items.AddRange(new object[] { "Slovenčina", "English", "Čeština" });
        cmbUiLanguage.SelectedIndex = 1;
        cmbUiLanguage.SelectedIndexChanged += (_, _) =>
        {
            UiText.SetLanguage(cmbUiLanguage.SelectedIndex switch
            {
                1 => UiLanguage.English,
                2 => UiLanguage.Czech,
                _ => UiLanguage.Slovak
            });
            ApplyLanguage();
        };

        chkZoom.Text = UiText.Get("PixelZoom");
        chkZoom.Checked = true;
        chkZoom.SetBounds(775, 43, 150, 24);
        chkZoom.CheckedChanged += (_, _) => ApplyPreviewZoom();
        btnAbout.Text = UiText.Get("About");
        btnAbout.SetBounds(955, 41, 90, 28);
        btnAbout.Click += (_, _) =>
            MessageBox.Show(
                UiText.Get("AboutText"),
                UiText.Get("AppTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        btnTextEditor.Text = UiText.Get("OpenTextEditor");
        btnTextEditor.SetBounds(1055, 41, 110, 28);
        btnTextEditor.Click += (_, _) =>
        {
            tabs.SelectedTab = tabText;
            LoadGamePcTexts();
        };


        lblStatus.AutoSize = true;
        lblStatus.SetBounds(1180, 46, 240, 20);

        top.Controls.AddRange(new Control[] { txtGameDir, btnBrowseGame, zoneLabel, cmbZone, btnReload, lblPaletteCaption, cmbPalette, lblLanguageCaption, cmbUiLanguage, chkZoom, btnAbout, btnTextEditor, lblStatus });
        Controls.Add(top);
        top.BringToFront();

        mainSplit.Dock = DockStyle.Fill;
        mainSplit.Orientation = Orientation.Vertical;
        mainSplit.SplitterWidth = 6;
        mainSplit.Panel1MinSize = 0;
        mainSplit.Panel2MinSize = 0;
        tabVga.Text = UiText.Get("VgaTab");
        tabVga.Controls.Add(mainSplit);

        tabText.Text = UiText.Get("TextTab");
        BuildTextEditorUi();

        tabs.Dock = DockStyle.Fill;
        tabs.TabPages.Add(tabVga);
        tabs.TabPages.Add(tabText);
        Controls.Add(tabs);

        grid.Dock = DockStyle.Fill;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.RowHeadersVisible = false;
        grid.ReadOnly = true;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.ScrollBars = ScrollBars.Both;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.RowTemplate.Height = 24;
        grid.BackgroundColor = SystemColors.Window;
        grid.BorderStyle = BorderStyle.FixedSingle;

        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ID", HeaderText = "ID", Width = 60 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Offset", HeaderText = "Offset", Width = 110 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Typ", Width = 70 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Size", HeaderText = "Rozmer", Width = 90 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Flags", HeaderText = "Flags", Width = 70 });
        grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Edit", HeaderText = "Edit", Width = 70 });

        grid.SelectionChanged += (_, _) => ShowSelectedPreview();
        grid.MouseEnter += (_, _) => grid.Focus();
        grid.KeyDown += (_, e) =>
        {
            if (grid.Rows.Count == 0) return;
            if (e.KeyCode == Keys.Home)
            {
                grid.FirstDisplayedScrollingRowIndex = 0;
                if (grid.Rows.Count > 0)
                {
                    grid.ClearSelection();
                    grid.CurrentCell = grid.Rows[0].Cells[0];
                    grid.Rows[0].Selected = true;
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.End)
            {
                int last = grid.Rows.Count - 1;
                grid.ClearSelection();
                grid.CurrentCell = grid.Rows[last].Cells[0];
                grid.Rows[last].Selected = true;
                grid.FirstDisplayedScrollingRowIndex = Math.Max(0, last - Math.Max(1, grid.DisplayedRowCount(false)) + 1);
                e.Handled = true;
            }
        };

        var gridTopHost = new Panel
        {
            Dock = DockStyle.Top,
            Height = 75,
            BackColor = SystemColors.Control
        };

mainSplit.Panel1.Padding = new Padding(0, 0, 4, 0);
        mainSplit.Panel1.Controls.Add(grid);
        mainSplit.Panel1.Controls.Add(gridTopHost);

        var right = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        mainSplit.Panel2.Controls.Add(right);

        previewScroll.Dock = DockStyle.Fill;
        previewScroll.AutoScroll = true;
        previewScroll.BackColor = Color.FromArgb(40, 40, 40);
        previewScroll.BorderStyle = BorderStyle.FixedSingle;

        preview.BackColor = Color.FromArgb(40, 40, 40);
        preview.SizeMode = PictureBoxSizeMode.Zoom;
        preview.Location = new Point(0, 0);
        previewScroll.Controls.Add(preview);

        var zoomBar = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 36,
            Padding = new Padding(4)
        };

        lblPreviewZoom.Text = "Zoom:";
        lblPreviewZoom.AutoSize = true;
        lblPreviewZoom.SetBounds(4, 9, 42, 20);

        cmbPreviewZoom.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPreviewZoom.SetBounds(52, 5, 100, 26);
        btnReloadPreview.SetBounds(162, 5, 90, 26);
        cmbPreviewZoom.Items.AddRange(new object[]
        {
            "Fit", "100%", "200%", "300%", "400%", "600%", "800%"
        });
        cmbPreviewZoom.SelectedIndex = 0;
        cmbPreviewZoom.SelectedIndexChanged += (_, _) => ApplyPreviewZoom();
        btnReloadPreview.Text = UiText.Get("ReloadPreview");
        btnReloadPreview.AutoSize = true;
        btnReloadPreview.Click += (_, _) => ReloadCurrentPreview();

        previewScroll.Resize += (_, _) => ApplyPreviewZoom();

        zoomBar.Controls.Add(lblPreviewZoom);
        zoomBar.Controls.Add(cmbPreviewZoom);
        zoomBar.Controls.Add(btnReloadPreview);

        lblMeta.Dock = DockStyle.Bottom;
        lblMeta.Height = 48;
        lblMeta.TextAlign = ContentAlignment.MiddleLeft;

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 78,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(4)
        };

        btnReplace.Text = "Nahradiť PNG...";
        btnClearEdit.Text = "Zrušiť edit";
        btnExport.Text = "Export PNG...";
        btnDeploy.Text = "APLIKOVAŤ DO HRY";
        btnRestore.Text = "Obnoviť originál";

        btnReplace.Width = 120;
        btnClearEdit.Width = 100;
        btnExport.Width = 110;
        btnDeploy.Width = 150;
        btnRestore.Width = 130;

        btnReplace.Click += (_, _) => ReplaceSelected();
        btnClearEdit.Click += (_, _) => ClearSelectedEdit();
        btnExport.Click += (_, _) => ExportSelected();
        btnDeploy.Click += (_, _) => Deploy();
        btnRestore.Click += (_, _) => RestoreOriginal();

        buttons.Controls.AddRange(new Control[] { btnReplace, btnClearEdit, btnExport, btnDeploy, btnRestore });

        previewViewport.Dock = DockStyle.Fill;
        previewViewport.Padding = new Padding(0, 75, 0, 0);
        previewViewport.BackColor = Color.FromArgb(40, 40, 40);
        previewViewport.Controls.Add(previewScroll);

        right.Controls.Add(previewViewport);
        right.Controls.Add(zoomBar);
        right.Controls.Add(lblMeta);
        right.Controls.Add(buttons);
    }

    private void BrowseGame()
    {
        using var dlg = new FolderBrowserDialog { SelectedPath = txtGameDir.Text };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            txtGameDir.Text = dlg.SelectedPath;
            ApplyLanguage();
            ScanGameFolder();
            LoadGamePcTexts();
        }
    }

    private void ScanGameFolder()
    {
        try
        {
            string dir = txtGameDir.Text.Trim();
            if (!Directory.Exists(dir))
            {
                SetStatus("Adresár neexistuje.", true);
                return;
            }

            string? old = cmbZone.SelectedItem?.ToString();

            var files = Directory.EnumerateFiles(dir, "*2.VGA", SearchOption.TopDirectoryOnly)
                .Where(p => Regex.IsMatch(Path.GetFileName(p), @"^\d{2}2\.VGA$", RegexOptions.IgnoreCase))
                .OrderBy(Path.GetFileName)
                .ToList();

            cmbZone.BeginUpdate();
            cmbZone.Items.Clear();
            foreach (var f in files)
                cmbZone.Items.Add(Path.GetFileName(f));
            cmbZone.EndUpdate();

            if (cmbZone.Items.Count > 0)
            {
                int idx = old is null ? 0 : cmbZone.Items.IndexOf(old);
                cmbZone.SelectedIndex = idx >= 0 ? idx : 0;
            }

            SetStatus(string.Format(UiText.Get("ZonesFound"), files.Count), false);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    private void LoadSelectedZone()
    {
        if (cmbZone.SelectedItem is null) return;

        try
        {
            _edits.Clear();

            _currentVga = Path.Combine(txtGameDir.Text.Trim(), cmbZone.SelectedItem.ToString()!);
            _currentData = File.ReadAllBytes(_currentVga);
            _table = new VgaImageTableParser(_currentData).Parse();

            LoadZonePalettes();

            grid.SuspendLayout();
            grid.Rows.Clear();

            foreach (var e in _table.Entries)
            {
                if (e.DataOffset == 0 || e.PixelWidth <= 0 || e.Height <= 0)
                    continue;

                int rowIndex = grid.Rows.Add(
                    e.ImageId.ToString("D4"),
                    $"0x{e.DataOffset:X8}",
                    e.Compressed ? "RLE" : "RAW",
                    $"{e.PixelWidth}x{e.Height}",
                    $"0x{e.HeaderFlags:X2}",
                    "");

                grid.Rows[rowIndex].Tag = e;
            }

            grid.ResumeLayout();
            preview.Image?.Dispose();
            preview.Image = null;
            preview.Size = previewScroll.ClientSize;
            lblMeta.Text = "";
            SetStatus($"{Path.GetFileName(_currentVga)} | entries: {_table.Entries.Count} | endian: {_table.Endian}", false);

            if (grid.Rows.Count > 0)
            {
                grid.ClearSelection();
                grid.CurrentCell = grid.Rows[0].Cells[0];
                grid.Rows[0].Selected = true;
                grid.FirstDisplayedScrollingRowIndex = 0;
            }
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }




    private void ReloadCurrentPreview()
    {
        if (_table is null || grid.CurrentRow?.Tag is not VgaImageEntry entry)
            return;

        try
        {
            // Re-decode the currently selected image and preserve the current zoom.
            // ShowSelectedPreview() already calls ApplyPreviewZoom().
            ShowSelectedPreview();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                UiText.Get("Warning"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void ApplyPreviewZoom()
    {
        if (preview.Image is null)
        {
            preview.Dock = DockStyle.None;
            preview.SizeMode = PictureBoxSizeMode.Zoom;
            preview.Location = Point.Empty;
            preview.Size = Size.Empty;
            previewScroll.AutoScrollMinSize = Size.Empty;
            return;
        }

        string mode = cmbPreviewZoom.SelectedItem?.ToString() ?? "Fit";

        if (mode == "Fit")
        {
            previewScroll.AutoScrollMinSize = Size.Empty;
            preview.Dock = DockStyle.Fill;
            preview.SizeMode = PictureBoxSizeMode.Zoom;
            preview.Location = Point.Empty;
            previewScroll.AutoScrollPosition = Point.Empty;
            return;
        }

        preview.Dock = DockStyle.None;
        preview.SizeMode = PictureBoxSizeMode.StretchImage;

        int percent = mode switch
        {
            "100%" => 100,
            "200%" => 200,
            "300%" => 300,
            "400%" => 400,
            "600%" => 600,
            "800%" => 800,
            _ => 100
        };

        int imageW = Math.Max(1, preview.Image.Width * percent / 100);
        int imageH = Math.Max(1, preview.Image.Height * percent / 100);
        preview.Size = new Size(imageW, imageH);

        int viewW = Math.Max(0, previewScroll.ClientSize.Width);
        int viewH = Math.Max(0, previewScroll.ClientSize.Height);

        bool needsHScroll = imageW > viewW;
        bool needsVScroll = imageH > viewH;

        if (!needsHScroll && !needsVScroll)
        {
            // Center the sprite when it fits into the renderer.
            int x = Math.Max(0, (viewW - imageW) / 2);
            int y = Math.Max(0, (viewH - imageH) / 2);
            preview.Location = new Point(x, y);
            previewScroll.AutoScrollMinSize = Size.Empty;
            previewScroll.AutoScrollPosition = Point.Empty;
        }
        else
        {
            // If only one dimension overflows, keep the other dimension centered.
            int x = needsHScroll ? 0 : Math.Max(0, (viewW - imageW) / 2);
            int y = needsVScroll ? 0 : Math.Max(0, (viewH - imageH) / 2);
            preview.Location = new Point(x, y);

            previewScroll.AutoScrollMinSize = new Size(
                Math.Max(imageW, viewW),
                Math.Max(imageH, viewH));
        }
    }


    private void RefreshPaletteDisplayLanguage()
    {
        if (cmbPalette.Items.Count == 0)
            return;

        int selected = cmbPalette.SelectedIndex;

        // Item 0 is the diagnostic palette string; replace it in the new language.
        if (cmbPalette.Items.Count > 0 && cmbPalette.Items[0] is string)
            cmbPalette.Items[0] = UiText.Get("DiagnosticPalette");

        // ElviraPaletteBank.ToString() uses the current UiText language.
        // Reinsert bank objects so the ComboBox recomputes their visible text.
        var banks = new List<ElviraPaletteBank>();
        for (int i = 1; i < cmbPalette.Items.Count; i++)
        {
            if (cmbPalette.Items[i] is ElviraPaletteBank bank)
                banks.Add(bank);
        }

        cmbPalette.BeginUpdate();
        while (cmbPalette.Items.Count > 1)
            cmbPalette.Items.RemoveAt(cmbPalette.Items.Count - 1);

        foreach (var bank in banks)
            cmbPalette.Items.Add(bank);

        if (selected >= 0 && selected < cmbPalette.Items.Count)
            cmbPalette.SelectedIndex = selected;

        cmbPalette.EndUpdate();
        cmbPalette.Refresh();
    }

    private void LoadZonePalettes()
    {
        _paletteBanks.Clear();
        cmbPalette.BeginUpdate();
        cmbPalette.Items.Clear();

        cmbPalette.Items.Add(UiText.Get("DiagnosticPalette"));

        if (_currentVga is not null)
        {
            string name = Path.GetFileName(_currentVga);
            if (name.Length >= 3)
            {
                // 092.VGA -> 091.VGA, 012.VGA -> 011.VGA
                string vga1Name = name[..2] + "1.VGA";
                string vga1Path = Path.Combine(Path.GetDirectoryName(_currentVga)!, vga1Name);

                if (File.Exists(vga1Path))
                {
                    try
                    {
                        _paletteBanks.AddRange(ElviraPaletteLoader.Load(vga1Path));
                        foreach (var bank in _paletteBanks)
                            cmbPalette.Items.Add(bank);
                    }
                    catch (Exception ex)
                    {
                        SetStatus($"Paleta: {ex.Message}", true);
                    }
                }
            }
        }

        cmbPalette.EndUpdate();

        // First real palette bank is a better default than the diagnostic colors.
        cmbPalette.SelectedIndex = _paletteBanks.Count > 0 ? 1 : 0;
    }

    private void PaletteChanged()
    {
        if (cmbPalette.SelectedIndex <= 0)
            _activePalette = ElviraPaletteLoader.DiagnosticPalette();
        else
        {
            int bankIndex = cmbPalette.SelectedIndex - 1;
            if (bankIndex >= 0 && bankIndex < _paletteBanks.Count)
                _activePalette = _paletteBanks[bankIndex].Colors;
        }

        ShowSelectedPreview();
    }

    private VgaImageEntry? SelectedEntry()
        => grid.CurrentRow?.Tag as VgaImageEntry;

    private void ShowSelectedPreview()
    {
        var e = SelectedEntry();
        if (e is null || _currentData is null) return;

        try
        {
            preview.Image?.Dispose();

            if (_edits.TryGetValue(e.ImageId, out string? png))
            {
                using var img = new Bitmap(png);
                preview.Image = new Bitmap(img);
                ApplyPreviewZoom();
            }
            else
            {
                byte[] px = ElviraImageDecoder.Decode(_currentData, e);
                preview.Image = PaletteTools.ToBitmap(e.PixelWidth, e.Height, px, _activePalette, transparentZero: true);
                ApplyPreviewZoom();
            }

            lblMeta.Text =
                $"Image {e.ImageId:D4} | Offset 0x{e.DataOffset:X8} | {(e.Compressed ? "RLE" : "RAW")} | " +
                $"{e.PixelWidth}x{e.Height} | flags 0x{e.HeaderFlags:X2}" +
                (_edits.ContainsKey(e.ImageId) ? " | EDITOVANÝ" : "") + $" | paleta: {cmbPalette.Text}";
        }
        catch (Exception ex)
        {
            preview.Image?.Dispose();
            preview.Image = null;
            lblMeta.Text = $"Image {e.ImageId:D4} | preview chyba: {ex.Message}";
        }
    }

    private void ReplaceSelected()
    {
        var e = SelectedEntry();
        if (e is null) return;

        using var dlg = new OpenFileDialog
        {
            Filter = "PNG image|*.png",
            Title = $"Náhradný PNG pre image {e.ImageId:D4}"
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            // Validate immediately.
            _ = PaletteTools.ReadIndices(dlg.FileName, e.PixelWidth, e.Height);
            _edits[e.ImageId] = dlg.FileName;
            RefreshEditMarker(e.ImageId);
            ShowSelectedPreview();
            SetStatus($"Image {e.ImageId:D4}: načítaný replacement PNG.", false);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    private void ClearSelectedEdit()
    {
        var e = SelectedEntry();
        if (e is null) return;
        _edits.Remove(e.ImageId);
        RefreshEditMarker(e.ImageId);
        ShowSelectedPreview();
    }

    private void RefreshEditMarker(int id)
    {
        foreach (DataGridViewRow row in grid.Rows)
        {
            if (row.Tag is VgaImageEntry e && e.ImageId == id)
            {
                row.Cells["Edit"].Value = _edits.ContainsKey(id) ? "ÁNO" : "";
                row.DefaultCellStyle.BackColor = _edits.ContainsKey(id)
                    ? Color.LightGoldenrodYellow
                    : SystemColors.Window;
                break;
            }
        }
    }

    private void ExportSelected()
    {
        var e = SelectedEntry();
        if (e is null || _currentData is null) return;

        using var dlg = new SaveFileDialog
        {
            Filter = "PNG image|*.png",
            FileName = $"{Path.GetFileNameWithoutExtension(_currentVga)}_{e.ImageId:D4}_off_{e.DataOffset:X8}_{(e.Compressed ? "RLE" : "RAW")}.png"
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            using Bitmap bmp = _edits.TryGetValue(e.ImageId, out string? replacement)
                ? new Bitmap(replacement)
                : PaletteTools.ToBitmap(e.PixelWidth, e.Height, ElviraImageDecoder.Decode(_currentData, e), _activePalette, transparentZero: true);

            bmp.Save(dlg.FileName, ImageFormat.Png);
            SetStatus($"Exportované: {dlg.FileName}", false);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    private void Deploy()
    {
        if (_currentVga is null) return;
        if (_edits.Count == 0)
        {
            MessageBox.Show(this, "Nie sú pripravené žiadne editované PNG.", "π1 Elvira I&II VGA Editor v1.0",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var answer = MessageBox.Show(this,
            $"Automaticky aplikovať {_edits.Count} editov do {Path.GetFileName(_currentVga)}?\n\n" +
            "Program:\n" +
            "1. vytvorí .bak_original (iba prvýkrát),\n" +
            "2. zostaví nový VGA,\n" +
            "3. aktuálny VGA premenuje na .bak_previous,\n" +
            "4. nový VGA automaticky premenuje na pôvodný názov.",
            "Aplikovať do hry",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (answer != DialogResult.Yes) return;

        try
        {
            SafeDeployer.Deploy(_currentVga, _edits);
            SetStatus("Patch úspešne aplikovaný do hry.", false);
            LoadSelectedZone();
            MessageBox.Show(this,
                "Hotovo. VGA je už pod pôvodným názvom a môžeš rovno spustiť hru.",
                "π1 Elvira I&II VGA Editor v1.0",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    private void RestoreOriginal()
    {
        if (string.IsNullOrWhiteSpace(_currentVga))
            return;

        string originalBackup = _currentVga + ".bak_original";

        if (!File.Exists(originalBackup))
        {
            MessageBox.Show(
                UiText.Get("OriginalBackupMissing"),
                UiText.Get("Warning"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            SafeDeployer.RestoreOriginal(_currentVga);
            _edits.Clear();
            LoadSelectedZone();
            ShowSelectedPreview();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.Message,
                UiText.Get("Warning"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }

    private void SetStatus(string text, bool error)
    {
        lblStatus.Text = text;
        lblStatus.ForeColor = error ? Color.DarkRed : SystemColors.ControlText;
    }

    private void Error(Exception ex)
    {
        SetStatus(ex.Message, true);
        MessageBox.Show(this, ex.ToString(), "Chyba", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
