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
    private readonly Button btnSpriteEditor = new();
    private readonly Button btnTextEditor = new();
    private readonly Button btnFontEditor = new();
    private readonly ComboBox cmbUiLanguage = new();
    private readonly ComboBox cmbGameProfile = new();
    private readonly Label lblGameProfile = new();
    private readonly Label lblDetectedGame = new();
    private readonly TabControl tabs = new();
    private readonly TabPage tabVga = new();
    private readonly TabPage tabText = new();
    private readonly TabPage tabFont = new();
    private readonly TabPage tabMods = new();
    private FontEditorForm? _embeddedFontEditor;

    private readonly DataGridView textGrid = new();
    private readonly TextBox txtSearch = new();
    private readonly ComboBox cmbTextEncoding = new();
    private readonly Button btnOpenDataFile = new();
    private readonly Button btnReloadTexts = new();
    private readonly Button btnSaveTexts = new();
    private readonly Button btnSaveAsDataFile = new();
    private readonly Button btnCreateDataVariant = new();
    private readonly Label lblTextStatus = new();
    private readonly Label lblTextSource = new();
    private readonly ComboBox cmbTextContext = new();
    private readonly Label lblTextValidation = new();
    private readonly CheckBox chkOnlyTextRisks = new();
    private readonly CheckBox chkIgnoreTextWarning = new();
    private bool _updatingIgnoreCheck;
    private TextDiagnosticStore? _textDiagnosticStore;
    private readonly Label lblSearchCaption = new();
    private readonly Label lblEncodingCaption = new();
    private readonly Label lblTextContextCaption = new();
    private readonly ToolTip textToolTip = new();
    private bool _refreshingTextGrid;
    private bool _textGridRefreshQueued;
    private string? _currentDataFilePath;

    private readonly DataGridView variantGrid = new();
    private readonly Button btnVariantAdd = new();
    private readonly Button btnVariantEdit = new();
    private readonly Button btnVariantRemove = new();
    private readonly Button btnVariantMoveUp = new();
    private readonly Button btnVariantMoveDown = new();
    private readonly Button btnVariantToggleEnabled = new();
    private readonly Button btnVariantOpenDataFile = new();
    private readonly Label lblLauncherPlaceholder = new();
    private VariantCatalog? _variantCatalog;

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
    private ElviraGameProfile _detectedProfile = ElviraGameProfile.Unknown;

    public MainForm()
    {
        Text = UiText.Get("AppTitle");
        Width = 1450;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1100, 700);
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

        UiText.SetLanguage(UiLanguage.English);
        BuildUi();
        cmbGameProfile.SelectedIndex = 0;

        txtGameDir.Text = @"C:\Games\GOG\Elvira";
        Shown += (_, _) =>
        {
            // The editor is intended as a workspace application; always start maximized.
            WindowState = FormWindowState.Maximized;
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
            Height = 136,
            Padding = new Padding(6)
        };

        lblTextSource.AutoSize = false;
        lblTextSource.SetBounds(8, 8, 310, 28);
        lblTextSource.TextAlign = ContentAlignment.MiddleLeft;
        lblTextSource.Font = new Font(Font, FontStyle.Bold);

        btnOpenDataFile.Text = UiText.Get("OpenDataFile");
        btnOpenDataFile.SetBounds(325, 6, 135, 28);
        btnOpenDataFile.Click += (_, _) => OpenTextDataFile();

        lblSearchCaption.Text = UiText.Get("Search");
        lblSearchCaption.AutoSize = true;
        lblSearchCaption.SetBounds(8, 47, 55, 20);

        txtSearch.SetBounds(65, 42, 260, 26);
        txtSearch.TextChanged += (_, _) => RefreshTextGrid();

        lblEncodingCaption.Text = UiText.Get("Encoding");
        lblEncodingCaption.AutoSize = true;
        lblEncodingCaption.SetBounds(340, 47, 75, 20);

        cmbTextEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTextEncoding.Items.AddRange(new object[] { "CP852", "Windows-1250", "Latin1/Raw" });
        cmbTextEncoding.SelectedIndex = 0;
        cmbTextEncoding.SetBounds(420, 42, 130, 26);
        cmbTextEncoding.SelectedIndexChanged += (_, _) => RefreshTextGrid();

        btnReloadTexts.Text = UiText.Get("ReloadTexts");
        btnReloadTexts.SetBounds(470, 6, 90, 28);
        btnReloadTexts.Click += (_, _) => LoadGamePcTexts();

        btnSaveTexts.Text = UiText.Get("SaveTexts");
        btnSaveTexts.SetBounds(570, 6, 80, 28);
        btnSaveTexts.Click += (_, _) => SaveGamePcTexts();

        btnSaveAsDataFile.Text = UiText.Get("SaveAsDataFile");
        btnSaveAsDataFile.SetBounds(660, 6, 105, 28);
        btnSaveAsDataFile.Click += (_, _) => SaveTextDataFileAs(createVariant: false);

        btnCreateDataVariant.Text = UiText.Get("CreateVariant");
        btnCreateDataVariant.SetBounds(775, 6, 125, 28);
        btnCreateDataVariant.Click += (_, _) => SaveTextDataFileAs(createVariant: true);

        lblTextContextCaption.Text = UiText.Get("TextContext");
        chkOnlyTextRisks.Text = UiText.Get("OnlyRisks");
        chkIgnoreTextWarning.Text = UiText.Get("IgnoreWarning");
        lblTextContextCaption.AutoSize = true;
        lblTextContextCaption.SetBounds(8, 82, 90, 20);

        cmbTextContext.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTextContext.Items.AddRange(new object[] { UiText.Get("ContextAuto"), UiText.Get("ContextGeneric"), UiText.Get("ContextNpc") });
        cmbTextContext.SelectedIndex = 0;
        cmbTextContext.SetBounds(100, 77, 210, 26);
        cmbTextContext.SelectedIndexChanged += (_, _) =>
        {
            RefreshTextGrid();
            UpdateTextValidation();
            UpdateTextStatusSummary();
        };

        lblTextValidation.AutoSize = false;
        lblTextValidation.SetBounds(325, 80, 900, 22);
        lblTextValidation.TextAlign = ContentAlignment.MiddleLeft;

        chkOnlyTextRisks.Text = UiText.Get("OnlyRisks");
        chkOnlyTextRisks.AutoSize = true;
        chkOnlyTextRisks.SetBounds(8, 108, 210, 22);
        chkOnlyTextRisks.CheckedChanged += (_, _) => RefreshTextGrid();

        chkIgnoreTextWarning.Text = UiText.Get("IgnoreWarning");
        chkIgnoreTextWarning.AutoSize = true;
        chkIgnoreTextWarning.SetBounds(235, 108, 250, 22);
        chkIgnoreTextWarning.CheckedChanged += (_, _) =>
        {
            if (_updatingIgnoreCheck || _refreshingTextGrid || ActiveGameProfile != ElviraGameProfile.Elvira1) return;
            if (textGrid.CurrentRow?.Tag is not GamePcStringEntry selectedEntry) return;
            _textDiagnosticStore?.SetIgnored(selectedEntry.Index, chkIgnoreTextWarning.Checked);
            QueueTextGridRefresh(selectedEntry.Index);
        };

        topText.Controls.AddRange(new Control[]
        {
            lblTextSource, btnOpenDataFile, btnReloadTexts, btnSaveTexts, btnSaveAsDataFile, btnCreateDataVariant,
            lblSearchCaption, txtSearch, lblEncodingCaption, cmbTextEncoding,
            lblTextContextCaption, cmbTextContext, lblTextValidation, chkOnlyTextRisks, chkIgnoreTextWarning
        });

        textGrid.Dock = DockStyle.Fill;
        textGrid.AllowUserToAddRows = false;
        textGrid.AllowUserToDeleteRows = false;
        textGrid.RowHeadersVisible = false;
        textGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        textGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        textGrid.ShowCellToolTips = true;
        textGrid.Columns.Add("Index", UiText.Get("TextIndex"));
        textGrid.Columns.Add("Offset", UiText.Get("TextOffset"));
        textGrid.Columns.Add("Length", UiText.Get("Length"));
        textGrid.Columns.Add("Bytes", UiText.Get("Bytes"));
        textGrid.Columns.Add("DosLimit", UiText.Get("DosLimit"));
        textGrid.Columns.Add("Text", UiText.Get("Text"));
        textGrid.Columns["Index"].Width = 70;
        textGrid.Columns["Offset"].Width = 110;
        textGrid.Columns["Length"].Width = 80;
        textGrid.Columns["Bytes"].Width = 75;
        textGrid.Columns["DosLimit"].Width = 110;
        textGrid.Columns["Text"].MinimumWidth = 500;
        textGrid.Columns["Text"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        textGrid.CellEndEdit += (_, e) =>
        {
            if (_refreshingTextGrid || e.RowIndex < 0) return;
            var row = textGrid.Rows[e.RowIndex];
            if (row.Tag is not GamePcStringEntry entry) return;
            string value = row.Cells["Text"].Value?.ToString() ?? "";
            _gamePcEdits[entry.Index] = value;
            row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;

            // DataGridView is still finishing its current-cell transition while
            // CellEndEdit is raised. Rebuilding rows synchronously here can
            // re-enter SetCurrentCellAddressCore and crash WinForms. Queue the
            // rebuild until the current event has completely unwound.
            QueueTextGridRefresh(entry.Index);
        };
        textGrid.SelectionChanged += (_, _) =>
        {
            if (_refreshingTextGrid) return;
            UpdateTextValidation();
        };
        textGrid.CurrentCellChanged += (_, _) =>
        {
            if (_refreshingTextGrid) return;
            UpdateTextValidation();
        };
        textGrid.CellToolTipTextNeeded += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (textGrid.Columns[e.ColumnIndex].Name != "Text" && textGrid.Columns[e.ColumnIndex].Name != "DosLimit") return;
            var row = textGrid.Rows[e.RowIndex];
            string value = row.Cells["Text"].Value?.ToString() ?? string.Empty;
            if (row.Tag is GamePcStringEntry entry)
                e.ToolTipText = BuildDialogueTooltip(entry, value);
            else
                e.ToolTipText = value;
        };

        lblTextStatus.Dock = DockStyle.Bottom;
        lblTextStatus.Height = 28;
        lblTextStatus.Padding = new Padding(6, 6, 0, 0);

        // The main application toolbar overlays the top ~75 px of the tab area.
        // Use the same 75 px content offset as the VGA editor so the text-editor
        // toolbar (Back to VGA / data-file actions) stays visible.
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

    private void BuildModsLauncherUi()
    {
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 72,
            Padding = new Padding(8),
            WrapContents = true
        };

        foreach (Button button in new[] { btnVariantAdd, btnVariantEdit, btnVariantRemove, btnVariantMoveUp, btnVariantMoveDown, btnVariantToggleEnabled, btnVariantOpenDataFile })
        {
            button.AutoSize = true;
            button.Height = 28;
            actions.Controls.Add(button);
        }
        btnVariantAdd.Click += (_, _) => AddVariant();
        btnVariantEdit.Click += (_, _) => EditVariant();
        btnVariantRemove.Click += (_, _) => RemoveVariant();
        btnVariantMoveUp.Click += (_, _) => MoveVariant(-1);
        btnVariantMoveDown.Click += (_, _) => MoveVariant(1);
        btnVariantToggleEnabled.Click += (_, _) => ToggleVariantEnabled();
        btnVariantOpenDataFile.Click += (_, _) => OpenSelectedVariantDataFile();

        variantGrid.Dock = DockStyle.Fill;
        variantGrid.AllowUserToAddRows = false;
        variantGrid.AllowUserToDeleteRows = false;
        variantGrid.AllowUserToResizeRows = false;
        variantGrid.RowHeadersVisible = false;
        variantGrid.ReadOnly = true;
        variantGrid.MultiSelect = false;
        variantGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        variantGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        variantGrid.BackgroundColor = SystemColors.Window;
        variantGrid.BorderStyle = BorderStyle.FixedSingle;
        variantGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Order", Width = 50 });
        variantGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", Width = 200 });
        variantGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DataFile", Width = 130 });
        variantGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", Width = 110 });
        variantGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Enabled", Width = 85 });
        variantGrid.SelectionChanged += (_, _) => UpdateVariantActions();

        lblLauncherPlaceholder.Dock = DockStyle.Bottom;
        lblLauncherPlaceholder.Height = 34;
        lblLauncherPlaceholder.Padding = new Padding(8, 8, 0, 0);
        lblLauncherPlaceholder.ForeColor = SystemColors.GrayText;

        var host = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 75, 0, 0) };
        host.Controls.Add(variantGrid);
        host.Controls.Add(actions);
        host.Controls.Add(lblLauncherPlaceholder);
        tabMods.Controls.Add(host);
    }

    private VariantCatalog EnsureVariantCatalog()
    {
        string directory = Path.GetFullPath(txtGameDir.Text.Trim());
        if (_variantCatalog is null || !_variantCatalog.InstallationDirectory.Equals(directory, StringComparison.OrdinalIgnoreCase))
            _variantCatalog = VariantConfigurationService.Load(directory);
        return _variantCatalog;
    }

    private VariantEntry? SelectedVariant => variantGrid.CurrentRow?.Tag as VariantEntry;

    private void RefreshVariantGrid(string? selectDataFile = null)
    {
        VariantCatalog catalog = EnsureVariantCatalog();
        string? selected = selectDataFile ?? SelectedVariant?.DataFile;
        variantGrid.Rows.Clear();
        foreach (VariantEntry entry in catalog.Entries)
        {
            VariantEntryStatus status = catalog.GetStatus(entry);
            int rowIndex = variantGrid.Rows.Add(entry.Order, entry.DisplayName, entry.DataFile,
                status.IsAvailable ? UiText.Get("VariantAvailable") : UiText.Get("VariantMissing"),
                entry.Enabled ? UiText.Get("VariantYes") : UiText.Get("VariantNo"));
            DataGridViewRow row = variantGrid.Rows[rowIndex];
            row.Tag = entry;
            if (!status.IsAvailable)
            {
                row.DefaultCellStyle.BackColor = Color.LemonChiffon;
                row.Cells["Status"].Style.ForeColor = Color.DarkOrange;
                row.Cells["Status"].Style.Font = new Font(variantGrid.Font, FontStyle.Bold);
            }
            if (!entry.Enabled)
                row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
            if (!string.IsNullOrWhiteSpace(selected) && entry.DataFile.Equals(selected, StringComparison.OrdinalIgnoreCase))
                row.Selected = true;
        }
        UpdateVariantActions();
    }

    private void OpenModsLauncher()
    {
        EnsureVariantCatalog();
        RefreshVariantGrid(Path.GetFileName(CurrentDataFilePath));
    }

    private void PersistVariants(string? selectDataFile)
    {
        VariantCatalog catalog = EnsureVariantCatalog();
        VariantConfigurationService.Save(catalog);
        RefreshVariantGrid(selectDataFile);
    }

    private void AddVariant()
    {
        VariantCatalog catalog = EnsureVariantCatalog();
        using var dialog = new VariantEntryDialog();
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            VariantEntry created = catalog.Add(dialog.DisplayName, dialog.DataFile, dialog.IsVariantEnabled);
            PersistVariants(created.DataFile);
        }
        catch (Exception ex) { ShowVariantError(ex.Message); }
    }

    private void EditVariant()
    {
        VariantEntry? selected = SelectedVariant;
        if (selected is null) return;
        using var dialog = new VariantEntryDialog(selected);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            EnsureVariantCatalog().Edit(selected.DataFile, dialog.DisplayName, dialog.DataFile, dialog.IsVariantEnabled);
            PersistVariants(dialog.DataFile);
        }
        catch (Exception ex) { ShowVariantError(ex.Message); }
    }

    private void RemoveVariant()
    {
        VariantEntry? selected = SelectedVariant;
        if (selected is null) return;
        string message = string.Format(UiText.Get("VariantRemoveConfirm"), selected.DisplayName, selected.DataFile);
        if (MessageBox.Show(this, message, UiText.Get("VariantRemove"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            return;
        EnsureVariantCatalog().Remove(selected.DataFile);
        PersistVariants(null);
    }

    private void MoveVariant(int direction)
    {
        VariantEntry? selected = SelectedVariant;
        if (selected is null) return;
        VariantCatalog catalog = EnsureVariantCatalog();
        bool moved = direction < 0 ? catalog.MoveUp(selected.DataFile) : catalog.MoveDown(selected.DataFile);
        if (moved) PersistVariants(selected.DataFile);
    }

    private void ToggleVariantEnabled()
    {
        VariantEntry? selected = SelectedVariant;
        if (selected is null) return;
        EnsureVariantCatalog().SetEnabled(selected.DataFile, !selected.Enabled);
        PersistVariants(selected.DataFile);
    }

    private void OpenSelectedVariantDataFile()
    {
        VariantEntry? selected = SelectedVariant;
        if (selected is null) return;
        VariantCatalog catalog = EnsureVariantCatalog();
        VariantEntryStatus status = catalog.GetStatus(selected);
        if (!status.IsAvailable)
        {
            MessageBox.Show(this, string.Format(UiText.Get("VariantDataFileMissing"), selected.DataFile), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        OpenTextDataFile(Path.Combine(catalog.InstallationDirectory, selected.DataFile));
    }

    private void UpdateVariantActions()
    {
        VariantEntry? selected = SelectedVariant;
        bool hasSelection = selected is not null;
        btnVariantEdit.Enabled = hasSelection;
        btnVariantRemove.Enabled = hasSelection;
        btnVariantMoveUp.Enabled = hasSelection;
        btnVariantMoveDown.Enabled = hasSelection;
        btnVariantToggleEnabled.Enabled = hasSelection;
        btnVariantOpenDataFile.Enabled = hasSelection;
        btnVariantToggleEnabled.Text = UiText.Get(selected?.Enabled == false ? "VariantEnable" : "VariantDisable");
    }

    private void ShowVariantError(string message) =>
        MessageBox.Show(this, message, UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);

    private string CurrentDataFilePath
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_currentDataFilePath)) return _currentDataFilePath;
            _currentDataFilePath = Path.Combine(txtGameDir.Text.Trim(), "GAMEPC");
            return _currentDataFilePath;
        }
    }

    private void SetCurrentDataFile(string path)
    {
        _currentDataFilePath = Path.GetFullPath(path);
        lblTextSource.Text = string.Format(UiText.Get("DataFileSource"), Path.GetFileName(_currentDataFilePath));
    }

    private void LoadGamePcTexts()
    {
        try
        {
            _gamePcEntries.Clear();
            _gamePcEdits.Clear();
            _textDiagnosticStore = TextDiagnosticStore.Load(txtGameDir.Text.Trim());

            string dataFilePath = CurrentDataFilePath;
            SetCurrentDataFile(dataFilePath);
            if (!File.Exists(dataFilePath))
            {
                lblTextStatus.Text = string.Format(UiText.Get("NoDataFile"), Path.GetFileName(dataFilePath));
                textGrid.Rows.Clear();
                return;
            }

            _gamePcEntries.AddRange(GamePcTextEditor.LoadEntries(dataFilePath));
            RefreshTextGrid();
            UpdateTextStatusSummary();
            _detectedProfile = GameProfileDetector.Detect(txtGameDir.Text.Trim(), cmbZone.Items.Count, _gamePcEntries.Count);
            UpdateGameProfileDisplay();
        }
        catch (Exception ex)
        {
            lblTextStatus.Text = ex.Message;
        }
    }

    private void RefreshTextGrid()
    {
        if (_refreshingTextGrid || _gamePcEntries.Count == 0)
            return;

        int selectedEntryIndex = textGrid.CurrentRow?.Tag is GamePcStringEntry selected
            ? selected.Index
            : -1;
        int firstDisplayedRow = -1;
        try
        {
            if (textGrid.Rows.Count > 0)
                firstDisplayedRow = textGrid.FirstDisplayedScrollingRowIndex;
        }
        catch
        {
            firstDisplayedRow = -1;
        }

        try
        {
            _refreshingTextGrid = true;
            textGrid.SuspendLayout();

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

                int bytes = enc.GetByteCount(value);
                string dosStatus = string.Empty;
                Color? dosColor = null;
                Color? rowColor = null;
                TextDiagnosticResult? diagnostic = null;
                var profile = GameProfileInfo.For(ActiveGameProfile);

                if (ActiveGameProfile == ElviraGameProfile.Elvira1 && cmbTextContext.SelectedIndex == 0)
                {
                    diagnostic = Elvira1TextMetadata.Evaluate(entry.Index, value, enc, _textDiagnosticStore?.IsIgnored(entry.Index) == true);
                    switch (diagnostic.Kind)
                    {
                        case TextDiagnosticKind.ConfirmedRisk:
                            dosStatus = $"✖ +{diagnostic.OverBy}"; dosColor = Color.DarkRed; rowColor = Color.MistyRose; break;
                        case TextDiagnosticKind.PossibleRisk:
                            dosStatus = $"⚠ +{diagnostic.OverBy}"; dosColor = Color.DarkOrange; rowColor = Color.LemonChiffon; break;
                        case TextDiagnosticKind.ConfirmedSafe:
                            dosStatus = UiText.Get("SafeStatus"); dosColor = Color.DarkGreen; rowColor = Color.Honeydew; break;
                        case TextDiagnosticKind.Ignored:
                            dosStatus = UiText.Get("IgnoredStatus"); dosColor = Color.DimGray; rowColor = Color.Gainsboro; break;
                    }
                }
                else if (cmbTextContext.SelectedIndex == 2)
                {
                    if (profile.InteractiveDialogueByteLimit is int limit)
                    {
                        int over = Math.Max(0, bytes - limit);
                        dosStatus = over == 0 ? "OK" : $"✖ +{over}";
                        dosColor = over == 0 ? Color.DarkGreen : Color.DarkRed;
                        if (over > 0) rowColor = Color.MistyRose;
                    }
                    else
                    {
                        dosStatus = "N/A"; dosColor = Color.DimGray;
                    }
                }

                if (chkOnlyTextRisks.Checked && !(diagnostic?.IsRisk == true || (cmbTextContext.SelectedIndex == 2 && bytes > (profile.InteractiveDialogueByteLimit ?? int.MaxValue))))
                    continue;

                int rowIndex = textGrid.Rows.Add(
                    entry.Index,
                    $"0x{entry.Offset:X}",
                    entry.ByteLength,
                    bytes,
                    dosStatus,
                    value);

                var row = textGrid.Rows[rowIndex];
                row.Tag = entry;
                if (dosColor.HasValue)
                {
                    row.Cells["DosLimit"].Style.ForeColor = dosColor.Value;
                    if (dosStatus.StartsWith("⚠"))
                        row.Cells["DosLimit"].Style.Font = new Font(textGrid.Font, FontStyle.Bold);
                }

                if (rowColor.HasValue)
                    row.DefaultCellStyle.BackColor = rowColor.Value;
                else if (_gamePcEdits.ContainsKey(entry.Index))
                    row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            }

            // Restore selection only after the rebuild. Event handlers are
            // suppressed by _refreshingTextGrid while this happens.
            if (selectedEntryIndex >= 0)
                SelectTextEntry(selectedEntryIndex);

            if (firstDisplayedRow >= 0 && textGrid.Rows.Count > 0)
            {
                int target = Math.Min(firstDisplayedRow, textGrid.Rows.Count - 1);
                try { textGrid.FirstDisplayedScrollingRowIndex = target; } catch { }
            }
        }
        finally
        {
            textGrid.ResumeLayout();
            _refreshingTextGrid = false;
        }

        UpdateTextValidation();
    }

    private void QueueTextGridRefresh(int selectedEntryIndex)
    {
        if (_textGridRefreshQueued || IsDisposed || Disposing)
            return;

        _textGridRefreshQueued = true;
        BeginInvoke(new Action(() =>
        {
            _textGridRefreshQueued = false;
            if (IsDisposed || Disposing) return;

            RefreshTextGrid();
            SelectTextEntry(selectedEntryIndex);
            UpdateTextValidation();
            UpdateTextStatusSummary();
        }));
    }

    private void SaveGamePcTexts()
    {
        try
        {
            var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
            GamePcTextEditor.SaveInPlace(CurrentDataFilePath, _gamePcEdits, _gamePcEntries, enc);
            lblTextStatus.Text = string.Format(UiText.Get("TextSaved"), Path.GetFileName(CurrentDataFilePath));
            LoadGamePcTexts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, UiText.Get("AppTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblTextStatus.Text = ex.Message;
        }
    }

    private void OpenTextDataFile()
    {
        using var dialog = new OpenFileDialog
        {
            Title = UiText.Get("OpenDataFile"),
            InitialDirectory = Directory.Exists(Path.GetDirectoryName(CurrentDataFilePath)) ? Path.GetDirectoryName(CurrentDataFilePath) : txtGameDir.Text.Trim(),
            Filter = UiText.Get("DataFileFilter"),
            CheckFileExists = true,
            Multiselect = false
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        OpenTextDataFile(dialog.FileName);
    }

    private void OpenTextDataFile(string path)
    {
        SetCurrentDataFile(path);
        LoadGamePcTexts();
        SwitchMode(tabText);
    }

    private void OfferCreatedVariant(string createdPath)
    {
        if (MessageBox.Show(this, UiText.Get("AddCreatedVariantQuestion"), UiText.Get("ModsLauncherTab"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        using var dialog = new VariantEntryDialog(displayName: Path.GetFileName(createdPath), dataFile: Path.GetFileName(createdPath), enabled: true);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            VariantEntry created = EnsureVariantCatalog().Add(dialog.DisplayName, dialog.DataFile, dialog.IsVariantEnabled);
            PersistVariants(created.DataFile);
        }
        catch (Exception ex)
        {
            // The physical data file has already been created successfully. Metadata is deliberately independent.
            ShowVariantError(string.Format(UiText.Get("VariantMetadataNotAdded"), ex.Message));
        }
    }

    private void ApplyVariantLanguage()
    {
        btnVariantAdd.Text = UiText.Get("VariantAdd");
        btnVariantEdit.Text = UiText.Get("VariantEdit");
        btnVariantRemove.Text = UiText.Get("VariantRemove");
        btnVariantMoveUp.Text = UiText.Get("VariantMoveUp");
        btnVariantMoveDown.Text = UiText.Get("VariantMoveDown");
        btnVariantOpenDataFile.Text = UiText.Get("VariantOpenDataFile");
        lblLauncherPlaceholder.Text = UiText.Get("LauncherFutureNote");
        if (variantGrid.Columns.Count == 5)
        {
            variantGrid.Columns["Order"].HeaderText = UiText.Get("VariantOrder");
            variantGrid.Columns["Name"].HeaderText = UiText.Get("VariantName");
            variantGrid.Columns["DataFile"].HeaderText = UiText.Get("VariantDataFile");
            variantGrid.Columns["Status"].HeaderText = UiText.Get("VariantStatus");
            variantGrid.Columns["Enabled"].HeaderText = UiText.Get("VariantEnabled");
        }
        if (_variantCatalog is not null)
            RefreshVariantGrid(Path.GetFileName(CurrentDataFilePath));
        else
            UpdateVariantActions();
    }

    private void SaveTextDataFileAs(bool createVariant)
    {
        if (_gamePcEntries.Count == 0) return;
        using var dialog = new SaveFileDialog
        {
            Title = UiText.Get(createVariant ? "CreateVariant" : "SaveAsDataFile"),
            InitialDirectory = Path.GetDirectoryName(CurrentDataFilePath),
            FileName = Path.GetFileName(CurrentDataFilePath),
            Filter = UiText.Get("DataFileFilter"),
            OverwritePrompt = false
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
            string createdPath = GameDataFileService.SaveAsNew(CurrentDataFilePath, dialog.FileName, _gamePcEdits, _gamePcEntries, enc);
            SetCurrentDataFile(createdPath);
            LoadGamePcTexts();
            lblTextStatus.Text = string.Format(UiText.Get(createVariant ? "VariantCreated" : "DataFileSavedAs"), Path.GetFileName(CurrentDataFilePath));
            if (createVariant)
                OfferCreatedVariant(createdPath);
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
        tabFont.Text = UiText.Get("FontEditor");
        tabMods.Text = UiText.Get("ModsLauncherTab");
        btnBrowseGame.Text = UiText.Get("Browse");
        btnReload.Text = UiText.Get("Refresh");
        chkZoom.Text = UiText.Get("PixelZoom");
        btnReplace.Text = UiText.Get("ReplacePng");
        btnExport.Text = UiText.Get("ExportPng");
        btnRestore.Text = UiText.Get("RestoreOriginal");
        btnReloadTexts.Text = UiText.Get("ReloadTexts");
        btnSaveTexts.Text = UiText.Get("SaveTexts");
        btnOpenDataFile.Text = UiText.Get("OpenDataFile");
        btnSaveAsDataFile.Text = UiText.Get("SaveAsDataFile");
        btnCreateDataVariant.Text = UiText.Get("CreateVariant");
        SetCurrentDataFile(CurrentDataFilePath);
        lblSearchCaption.Text = UiText.Get("Search");
        lblEncodingCaption.Text = UiText.Get("Encoding");
        lblTextContextCaption.Text = UiText.Get("TextContext");
        chkOnlyTextRisks.Text = UiText.Get("OnlyRisks");
        chkIgnoreTextWarning.Text = UiText.Get("IgnoreWarning");
        int contextIndex = Math.Max(0, cmbTextContext.SelectedIndex);
        cmbTextContext.Items.Clear();
        cmbTextContext.Items.AddRange(new object[] { UiText.Get("ContextAuto"), UiText.Get("ContextGeneric"), UiText.Get("ContextNpc") });
        cmbTextContext.SelectedIndex = Math.Min(contextIndex, cmbTextContext.Items.Count - 1);

        int gameProfileIndex = Math.Max(0, cmbGameProfile.SelectedIndex);
        cmbGameProfile.Items.Clear();
        cmbGameProfile.Items.AddRange(new object[] { UiText.Get("AutoDetect"), "Elvira I", "Elvira II" });
        cmbGameProfile.SelectedIndex = Math.Min(gameProfileIndex, cmbGameProfile.Items.Count - 1);

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
            if (textGrid.Columns.Contains("Bytes")) textGrid.Columns["Bytes"].HeaderText = UiText.Get("Bytes");
            if (textGrid.Columns.Contains("DosLimit")) textGrid.Columns["DosLimit"].HeaderText = UiText.Get("DosLimit");
            textGrid.Columns["Text"].HeaderText = UiText.Get("Text");
        }
        if (cmbZone.Items.Count > 0)
            lblStatus.Text = string.Format(UiText.Get("ZonesFound"), cmbZone.Items.Count);
        if (_gamePcEntries.Count > 0)
            UpdateTextStatusSummary();
        ApplyVariantLanguage();
        btnAbout.Text = UiText.Get("About");
        btnSpriteEditor.Text = UiText.Get("SpriteEditor");
        btnClearEdit.Text = UiText.Get("CancelEdit");
        btnDeploy.Text = UiText.Get("ApplyGame");
        lblPaletteCaption.Text = UiText.Get("Palette");
        lblLanguageCaption.Text = UiText.Get("Language");
        RefreshPaletteDisplayLanguage();
        btnTextEditor.Text = UiText.Get("OpenTextEditor");
        btnFontEditor.Text = UiText.Get("FontEditor");
        lblGameProfile.Text = UiText.Get("Game");
        btnReloadPreview.Text = UiText.Get("ReloadPreview");
        if (_embeddedFontEditor is not null && !_embeddedFontEditor.IsDisposed)
            _embeddedFontEditor.ApplyLanguage();
        UpdateGameProfileDisplay();
        UpdateModeButtons();
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

        txtGameDir.SetBounds(8, 8, 520, 26);
        btnBrowseGame.Text = UiText.Get("Browse");
        btnBrowseGame.SetBounds(535, 7, 130, 28);
        btnBrowseGame.Click += (_, _) => BrowseGame();

        lblGameProfile.Text = UiText.Get("Game");
        lblGameProfile.AutoSize = true;
        lblGameProfile.SetBounds(680, 12, 45, 20);

        cmbGameProfile.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbGameProfile.SetBounds(725, 7, 190, 28);
        cmbGameProfile.Items.AddRange(new object[] { UiText.Get("AutoDetect"), "Elvira I", "Elvira II" });
        cmbGameProfile.SelectedIndexChanged += (_, _) => UpdateGameProfileDisplay();

        lblDetectedGame.AutoSize = true;
        lblDetectedGame.Font = new Font(Font, FontStyle.Bold);
        lblDetectedGame.SetBounds(925, 12, 470, 20);

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
        chkZoom.SetBounds(775, 43, 145, 24);
        chkZoom.CheckedChanged += (_, _) => ApplyPreviewZoom();

        // Permanent mode navigation. These buttons stay in the same place in every mode.
        ConfigureModeButton(btnSpriteEditor);
        ConfigureModeButton(btnTextEditor);
        ConfigureModeButton(btnFontEditor);

        btnSpriteEditor.Text = UiText.Get("SpriteEditor");
        btnSpriteEditor.SetBounds(925, 41, 115, 28);
        btnSpriteEditor.Click += (_, _) => SwitchMode(tabVga);

        btnTextEditor.Text = UiText.Get("OpenTextEditor");
        btnTextEditor.SetBounds(1045, 41, 110, 28);
        btnTextEditor.Click += (_, _) =>
        {
            LoadGamePcTexts();
            SwitchMode(tabText);
        };

        btnFontEditor.Text = UiText.Get("FontEditor");
        btnFontEditor.SetBounds(1160, 41, 105, 28);
        btnFontEditor.Click += (_, _) =>
        {
            EnsureEmbeddedFontEditor();
            SwitchMode(tabFont);
        };

        btnAbout.Text = UiText.Get("About");
        btnAbout.SetBounds(1270, 41, 90, 28);
        btnAbout.Click += (_, _) =>
            MessageBox.Show(
                UiText.Get("AboutText"),
                UiText.Get("AppTitle"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

        lblStatus.Dock = DockStyle.Bottom;
        lblStatus.Height = 28;
        lblStatus.Padding = new Padding(6, 6, 0, 0);
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;

        top.Controls.AddRange(new Control[] { txtGameDir, btnBrowseGame, lblGameProfile, cmbGameProfile, lblDetectedGame, zoneLabel, cmbZone, btnReload, lblPaletteCaption, cmbPalette, lblLanguageCaption, cmbUiLanguage, chkZoom, btnSpriteEditor, btnTextEditor, btnFontEditor, btnAbout });
        Controls.Add(top);
        top.BringToFront();

        mainSplit.Dock = DockStyle.Fill;
        mainSplit.Orientation = Orientation.Vertical;
        mainSplit.SplitterWidth = 6;
        mainSplit.Panel1MinSize = 0;
        mainSplit.Panel2MinSize = 0;
        tabVga.Text = UiText.Get("VgaTab");
        tabVga.Controls.Add(mainSplit);
        tabVga.Controls.Add(lblStatus);
        lblStatus.BringToFront();

        tabText.Text = UiText.Get("TextTab");
        BuildTextEditorUi();

        tabFont.Text = UiText.Get("FontEditor");
        tabMods.Text = UiText.Get("ModsLauncherTab");
        BuildModsLauncherUi();

        tabs.Dock = DockStyle.Fill;
        tabs.TabPages.Add(tabVga);
        tabs.TabPages.Add(tabText);
        tabs.TabPages.Add(tabFont);
        tabs.TabPages.Add(tabMods);
        tabs.SelectedIndexChanged += (_, _) =>
        {
            if (tabs.SelectedTab == tabMods) OpenModsLauncher();
            UpdateModeButtons();
        };
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

        lblPreviewZoom.Text = UiText.Get("Zoom");
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

        btnReplace.Text = UiText.Get("ReplacePng");
        btnClearEdit.Text = UiText.Get("CancelEdit");
        btnExport.Text = UiText.Get("ExportPng");
        btnDeploy.Text = UiText.Get("ApplyGame");
        btnRestore.Text = UiText.Get("RestoreOriginal");

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

    private static void ConfigureModeButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.UseVisualStyleBackColor = false;
    }

    private void SwitchMode(TabPage target)
    {
        tabs.SelectedTab = target;
        UpdateModeButtons();
    }

    private void UpdateModeButtons()
    {
        SetModeButtonState(btnSpriteEditor, tabs.SelectedTab == tabVga);
        SetModeButtonState(btnTextEditor, tabs.SelectedTab == tabText);
        SetModeButtonState(btnFontEditor, tabs.SelectedTab == tabFont);
    }

    private static void SetModeButtonState(Button button, bool active)
    {
        button.BackColor = active ? SystemColors.Highlight : SystemColors.Control;
        button.ForeColor = active ? SystemColors.HighlightText : SystemColors.ControlText;
        button.FlatAppearance.BorderColor = active ? SystemColors.Highlight : SystemColors.ControlDark;
        button.Font = new Font(button.Font, active ? FontStyle.Bold : FontStyle.Regular);
    }

    private void EnsureEmbeddedFontEditor()
    {
        if (_embeddedFontEditor is not null && !_embeddedFontEditor.IsDisposed)
            return;

        _embeddedFontEditor = new FontEditorForm(txtGameDir.Text)
        {
            TopLevel = false,
            FormBorderStyle = FormBorderStyle.None,
            Dock = DockStyle.Fill,
            MinimumSize = Size.Empty,
            StartPosition = FormStartPosition.Manual
        };
        _embeddedFontEditor.GameExecutableOpened += SynchronizeGameContextFromExecutable;

        // The application's permanent navigation/header overlays the first ~78 px of
        // the hidden TabControl page area.  VGA/Text already compensate for this.
        // Host the embedded font editor below the same header so its own Game EXE /
        // Apply / Import / Export toolbar remains fully visible.
        var fontHost = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 78, 0, 0)
        };
        fontHost.Controls.Add(_embeddedFontEditor);

        tabFont.Controls.Clear();
        tabFont.Controls.Add(fontHost);
        _embeddedFontEditor.Show();
    }

    private void SynchronizeGameContextFromExecutable(string executablePath, ElviraGame game)
    {
        string? directory = Path.GetDirectoryName(Path.GetFullPath(executablePath));
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) return;
        txtGameDir.Text = directory;
        _currentDataFilePath = null;
        _variantCatalog = null;
        _detectedProfile = game == ElviraGame.Elvira2 ? ElviraGameProfile.Elvira2 : ElviraGameProfile.Elvira1;
        // An EXE signature is stronger than a prior manual/folder heuristic, so return the UI to detected mode.
        cmbGameProfile.SelectedIndex = 0;
        ScanGameFolder();
        LoadGamePcTexts();
        UpdateGameProfileDisplay();
    }

    private ElviraGameProfile ActiveGameProfile
    {
        get
        {
            return cmbGameProfile.SelectedIndex switch
            {
                1 => ElviraGameProfile.Elvira1,
                2 => ElviraGameProfile.Elvira2,
                _ => _detectedProfile
            };
        }
    }

    private void UpdateGameProfileDisplay()
    {
        var active = ActiveGameProfile;
        var info = GameProfileInfo.For(active);
        string mode = cmbGameProfile.SelectedIndex == 0 ? UiText.Get("Detected") : UiText.Get("Selected");
        lblDetectedGame.Text = $"{mode}: {info.DisplayName}";
        lblDetectedGame.ForeColor = active == ElviraGameProfile.Unknown ? Color.DarkOrange : SystemColors.ControlText;
        UpdateTextValidation();
    }

    private void UpdateTextValidation()
    {
        if (lblTextValidation.IsDisposed) return;
        if (textGrid.CurrentRow?.Tag is not GamePcStringEntry entry)
        {
            lblTextValidation.Text = string.Empty;
            return;
        }

        string text = textGrid.CurrentRow.Cells["Text"].Value?.ToString() ?? string.Empty;
        var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
        int bytes = enc.GetByteCount(text);
        var profile = GameProfileInfo.For(ActiveGameProfile);

        _updatingIgnoreCheck = true;
        chkIgnoreTextWarning.Enabled = ActiveGameProfile == ElviraGameProfile.Elvira1 && cmbTextContext.SelectedIndex == 0;
        chkIgnoreTextWarning.Checked = _textDiagnosticStore?.IsIgnored(entry.Index) == true;
        _updatingIgnoreCheck = false;

        if (ActiveGameProfile == ElviraGameProfile.Elvira1 && cmbTextContext.SelectedIndex == 0)
        {
            var d = Elvira1TextMetadata.Evaluate(entry.Index, text, enc, _textDiagnosticStore?.IsIgnored(entry.Index) == true);
            lblTextValidation.ForeColor = d.Kind switch
            {
                TextDiagnosticKind.ConfirmedRisk => Color.DarkRed,
                TextDiagnosticKind.PossibleRisk => Color.DarkOrange,
                TextDiagnosticKind.ConfirmedSafe => Color.DarkGreen,
                TextDiagnosticKind.Ignored => Color.DimGray,
                _ => SystemColors.ControlText
            };
            lblTextValidation.Text = d.Kind switch
            {
                TextDiagnosticKind.ConfirmedRisk => string.Format(UiText.Get("ConfirmedRiskDetail"), d.Bytes, d.Limit, d.OverBy),
                TextDiagnosticKind.PossibleRisk => string.Format(UiText.Get("PossibleRiskDetail"), d.Bytes, d.Limit, d.OverBy),
                TextDiagnosticKind.ConfirmedSafe => string.Format(UiText.Get("ConfirmedSafeDetail"), d.Bytes, d.Context),
                TextDiagnosticKind.Ignored => string.Format(UiText.Get("IgnoredDetail"), d.Bytes),
                _ => string.Format(UiText.Get("BytesOnly"), d.Bytes)
            };
            textToolTip.SetToolTip(lblTextValidation, BuildDiagnosticTooltip(d, text));
            return;
        }

        if (cmbTextContext.SelectedIndex != 2)
        {
            lblTextValidation.ForeColor = SystemColors.ControlText;
            lblTextValidation.Text = string.Format(UiText.Get("BytesOnly"), bytes);
            return;
        }

        if (profile.InteractiveDialogueByteLimit is not int limit)
        {
            lblTextValidation.ForeColor = SystemColors.ControlText;
            lblTextValidation.Text = string.Format(UiText.Get("DialogueLimitUnknown"), profile.DisplayName);
            return;
        }

        var sim = DialogueLimitValidator.Simulate(text, enc, limit);
        if (bytes <= limit)
        {
            lblTextValidation.ForeColor = Color.DarkGreen;
            lblTextValidation.Text = string.Format(UiText.Get("DialogueLimitOk"), bytes, limit);
        }
        else
        {
            lblTextValidation.ForeColor = Color.DarkRed;
            lblTextValidation.Text = string.Format(UiText.Get("DialogueLimitExceeded"), bytes, limit, bytes - limit, sim.TruncatedText.Length);
            textToolTip.SetToolTip(lblTextValidation, $"{UiText.Get("DialogueVisible")}: {sim.VisibleText}\n\n{UiText.Get("DialogueTruncated")}: {sim.TruncatedText}");
        }
    }

    private static string BuildDiagnosticTooltip(TextDiagnosticResult d, string fullText)
    {
        if (d.Kind is TextDiagnosticKind.ConfirmedRisk or TextDiagnosticKind.PossibleRisk)
            return $"{d.Confidence} | {d.Context}\n{UiText.Get("DialogueVisible")}: {d.VisibleText}\n\n{UiText.Get("DialogueTruncated")}: {d.ClippedText}";
        return $"{d.Confidence} | {d.Context}\n{fullText}";
    }

    private string BuildDialogueTooltip(GamePcStringEntry entry, string text)
    {
        var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
        if (ActiveGameProfile == ElviraGameProfile.Elvira1 && cmbTextContext.SelectedIndex == 0)
        {
            var d = Elvira1TextMetadata.Evaluate(entry.Index, text, enc, _textDiagnosticStore?.IsIgnored(entry.Index) == true);
            return BuildDiagnosticTooltip(d, text);
        }

        int bytes = enc.GetByteCount(text);
        var profile = GameProfileInfo.For(ActiveGameProfile);
        if (cmbTextContext.SelectedIndex != 2 || profile.InteractiveDialogueByteLimit is not int limit)
            return $"{bytes} {UiText.Get("Bytes").ToLowerInvariant()} | {text}";

        var sim = DialogueLimitValidator.Simulate(text, enc, limit);
        if (bytes <= limit) return string.Format(UiText.Get("DialogueLimitOk"), bytes, limit) + $"\n\n{text}";
        return string.Format(UiText.Get("DialogueLimitExceeded"), bytes, limit, bytes - limit, sim.TruncatedText.Length) +
               $"\n\n{UiText.Get("DialogueVisible")}: {sim.VisibleText}\n\n{UiText.Get("DialogueTruncated")}: {sim.TruncatedText}";
    }

    private void UpdateTextStatusSummary()
    {
        if (_gamePcEntries.Count == 0) return;
        string baseText = string.Format(UiText.Get("StringsCount"), Path.GetFileName(CurrentDataFilePath), _gamePcEntries.Count);
        if (ActiveGameProfile == ElviraGameProfile.Elvira1 && cmbTextContext.SelectedIndex == 0)
        {
            var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
            int confirmed = 0, possible = 0;
            foreach (var entry in _gamePcEntries)
            {
                string value = _gamePcEdits.TryGetValue(entry.Index, out var edited) ? edited : entry.Decode(enc);
                var d = Elvira1TextMetadata.Evaluate(entry.Index, value, enc, _textDiagnosticStore?.IsIgnored(entry.Index) == true);
                if (d.Kind == TextDiagnosticKind.ConfirmedRisk) confirmed++;
                else if (d.Kind == TextDiagnosticKind.PossibleRisk) possible++;
            }
            baseText += " | " + string.Format(UiText.Get("RiskSummary"), confirmed, possible);
        }
        lblTextStatus.Text = baseText;
    }

    private void SelectTextEntry(int index)
    {
        if (textGrid.IsDisposed || index < 0)
            return;

        foreach (DataGridViewRow row in textGrid.Rows)
        {
            if (row.Tag is GamePcStringEntry e && e.Index == index)
            {
                textGrid.ClearSelection();
                row.Selected = true;
                if (!row.Cells["Text"].Selected)
                    textGrid.CurrentCell = row.Cells["Text"];
                break;
            }
        }
    }

    private static string LocalizedProfileNote(ElviraGameProfile profile)
    {
        return (UiText.Language, profile) switch
        {
            (UiLanguage.Slovak, ElviraGameProfile.Elvira1) => "Pôvodná DOS cesta interaktívneho NPC dialógu má pozorovaný 96-bajtový CP852 limit, ktorý rešpektuje hranice slov. Nie je to globálny limit GAMEPC.",
            (UiLanguage.Czech, ElviraGameProfile.Elvira1) => "Původní DOS cesta interaktivního NPC dialogu má pozorovaný 96bajtový CP852 limit respektující hranice slov. Nejde o globální limit GAMEPC.",
            (_, ElviraGameProfile.Elvira1) => "The original DOS interactive NPC dialogue path has an observed 96-byte CP852 word-aware limit. This is not a global GAMEPC limit.",
            (UiLanguage.Slovak, ElviraGameProfile.Elvira2) => "Pre Elviru II zatiaľ nepredpokladáme rovnaký limit ako v Elvire I; upozornenie zostáva informačné, kým sa limit experimentálne nepotvrdí.",
            (UiLanguage.Czech, ElviraGameProfile.Elvira2) => "Pro Elviru II zatím nepředpokládáme stejný limit jako v Elviře I; upozornění zůstává informační, dokud se limit experimentálně nepotvrdí.",
            (_, ElviraGameProfile.Elvira2) => "Elvira II is not assumed to share Elvira I's dialogue limit; validation remains informational until proven.",
            _ => string.Empty
        };
    }

    private void BrowseGame()
    {
        using var dlg = new FolderBrowserDialog { SelectedPath = txtGameDir.Text };
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            txtGameDir.Text = dlg.SelectedPath;
            _variantCatalog = null;
            ApplyLanguage();
            ScanGameFolder();
            LoadGamePcTexts();
            if (_embeddedFontEditor is not null && !_embeddedFontEditor.IsDisposed)
                _embeddedFontEditor.LoadFromGameDirectory(txtGameDir.Text);
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
            _detectedProfile = GameProfileDetector.Detect(dir, files.Count, _gamePcEntries.Count > 0 ? _gamePcEntries.Count : null);
            UpdateGameProfileDisplay();
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
            Filter = $"{UiText.Get("PngImage")}|*.png",
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
            Filter = $"{UiText.Get("PngImage")}|*.png",
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
            MessageBox.Show(this, UiText.Get("NoEditedPng"), UiText.Get("AppTitle"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var answer = MessageBox.Show(this,
            $"Automaticky aplikovať {_edits.Count} editov do {Path.GetFileName(_currentVga)}?\n\n" +
            "Program:\n" +
            "1. vytvorí nemennú O.VGA zálohu (iba pri prvom uložení),\n" +
            "2. zostaví a overí nový VGA do dočasného súboru,\n" +
            "3. nahradí iba aktívny VGA pod pôvodným názvom.",
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
                UiText.Get("AppTitle"),
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

        string originalBackup = SafeDeployer.OriginalBackupPath(_currentVga);

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
