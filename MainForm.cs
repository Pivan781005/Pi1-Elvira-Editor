using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ElviraVgaEditor;

internal sealed class MainForm : Form
{
    // The path remains an internal state holder so existing editor backends need no path plumbing rewrite.
    // The user-facing control is cmbInstallations; its display text is never parsed.
    private readonly TextBox txtGameDir = new();
    private readonly ComboBox cmbInstallations = new();
    private readonly CenteredCaptionButton btnFindGames = new();
    private readonly CenteredCaptionButton btnBrowseGame = new();
    private readonly ComboBox cmbZone = new();
    private readonly Panel panelPaletteSelector = new();
    private readonly Panel panelPaletteSwatches = new();
    private readonly Label lblPaletteMode = new();
    private readonly ComboBox cmbPaletteManual = new();
    private readonly CenteredCaptionButton btnPaletteAdvanced = new();
    private readonly Label lblPaletteCaption = new();
    private readonly Label lblLanguageCaption = new();
    private readonly Label lblInstallationCaption = new();
    private readonly Label lblActiveVariantCaption = new();
    private readonly ComboBox cmbActiveVariant = new();
    private readonly Label lblActiveProjectCaption = new();
    private readonly ComboBox cmbActiveProject = new();
    private readonly CenteredCaptionButton btnBuildActiveVariant = new();
    private readonly Label lblVgaFileCaption = new();
    private readonly CenteredCaptionButton btnReload = new();
    private readonly DataGridView grid = new();
    private readonly PixelPerfectPictureBox preview = new();
    private readonly Panel previewScroll = new();
    private readonly Panel previewViewport = new();
    private readonly ComboBox cmbPreviewZoom = new();
    private readonly CenteredCaptionButton btnReloadPreview = new();
    private readonly Label lblPreviewZoom = new();
    private readonly Label lblMeta = new();
    private readonly StatusStrip statusBar = new();
    private readonly ToolStripStatusLabel lblStatus = new();
    private readonly CenteredCaptionButton btnAbout = new();
    private readonly CenteredCaptionButton btnHelp = new();
    private readonly CenteredCaptionButton btnSpriteEditor = new();
    private readonly CenteredCaptionButton btnTextEditor = new();
    private readonly CenteredCaptionButton btnFontEditor = new();
    private readonly CenteredCaptionButton btnModsLauncher = new();
    private readonly ComboBox cmbUiLanguage = new();
    private readonly Label lblDetectedGameCaption = new();
    private readonly Label lblDetectedGame = new();
    private readonly TabControl tabs = new();
    private readonly TabPage tabVga = new();
    private readonly TabPage tabText = new();
    private readonly TabPage tabFont = new();
    private readonly TabPage tabMods = new();
    private FontEditorForm? _embeddedFontEditor;
    private HelpViewerForm? _helpViewer;
    private AboutViewerForm? _aboutViewer;

    private readonly DataGridView textGrid = new();
    private readonly TextBox txtSearch = new();
    private readonly ComboBox cmbTextEncoding = new();
    private readonly CenteredCaptionButton btnOpenDataFile = new();
    private readonly CenteredCaptionButton btnReloadTexts = new();
    private readonly CenteredCaptionButton btnSaveTexts = new();
    private readonly CenteredCaptionButton btnSaveAsDataFile = new();
    private readonly CenteredCaptionButton btnCreateDataVariant = new();
    private readonly CenteredCaptionButton btnExportTranslations = new();
    private readonly CenteredCaptionButton btnImportTranslations = new();
    private readonly Label lblTextStatus = new();
    private readonly Label lblTextSource = new();
    private readonly Label lblTextOverview = new();
    private readonly ComboBox cmbTextContext = new();
    private readonly Label lblTranslationVariantCaption = new();
    private readonly ComboBox cmbTranslationVariant = new();
    private readonly Label lblTextValidation = new();
    private TextDiagnosticStore? _textDiagnosticStore;
    private readonly Label lblSearchCaption = new();
    private readonly Label lblEncodingCaption = new();
    private readonly Label lblTextContextCaption = new();
    private readonly ToolTip textToolTip = new();
    private readonly ToolTip installationToolTip = new();
    private readonly TabControl textDomainTabs = new();
    private readonly TabPage tabGameTextDomain = new();
    private readonly TabPage tabRuntimeUiDomain = new();
    private readonly DataGridView runtimeUiGrid = new();
    private readonly CenteredCaptionButton btnReloadRuntimeUi = new();
    private readonly CenteredCaptionButton btnSaveRuntimeUi = new();
    private readonly CenteredCaptionButton btnResetRuntimeUi = new();
    private readonly Label lblRuntimeUiStatus = new();
    private readonly Label lblRuntimeUiRuntime = new();
    private readonly RuntimeUiTextService _runtimeUiTexts = new();
    private readonly RuntimeUiLayoutValidationService _runtimeUiLayouts;
    private ProjectContext? _runtimeUiProject;
    private VariantContext? _runtimeUiVariant;
    private RuntimeUiTextState? _runtimeUiState;
    private bool _applyingUiLanguage;
    private bool _runtimeUiRefreshing;
    private bool _runtimeUiDirty;
    private bool _refreshingTextGrid;
    private bool _textGridRefreshQueued;
    private string? _currentDataFilePath;
    private GamePcOriginalReference? _gamePcOriginal;
    private VariantContext? _textVariant;
    private readonly TranslationProjectService _translationProjects = new();
    private TranslationProjectState? _translationProjectState;
    private string _activeTranslationCode = "EN";
    private bool _refreshingTranslationVariantSelector;
    private bool _refreshingActiveProjectSelector;
    private WorkflowStatus? _workflowStatus;

    private readonly DataGridView variantGrid = new();
    private readonly DataGridView variantManagerGrid = new();
    private readonly Label lblVariantManagerTitle = new();
    private readonly Label lblVariantManagerHint = new();
    private bool _refreshingVariantManager;
    private readonly CenteredCaptionButton btnVariantAdd = new();
    private readonly CenteredCaptionButton btnVariantEdit = new();
    private readonly CenteredCaptionButton btnVariantRemove = new();
    private readonly CenteredCaptionButton btnVariantMoveUp = new();
    private readonly CenteredCaptionButton btnVariantMoveDown = new();
    private readonly CenteredCaptionButton btnVariantToggleEnabled = new();
    private readonly CenteredCaptionButton btnVariantOpenDataFile = new();
    private readonly Label lblLauncherFile = new();
    private readonly TextBox txtLauncherFile = new();
    private readonly CenteredCaptionButton btnSelectLauncherFile = new();
    private readonly Label lblModderName = new();
    private readonly TextBox txtModderName = new();
    private readonly CenteredCaptionButton btnPreviewLauncher = new();
    private readonly CenteredCaptionButton btnGenerateLauncher = new();
    private readonly CenteredCaptionButton btnRestoreLauncher = new();
    private readonly Label lblDefaultVariant = new();
    private readonly Label lblLauncherReadiness = new();
    private readonly CenteredCaptionButton btnRunVariant = new();
    private readonly CenteredCaptionButton btnDebugVariant = new();
    private readonly Label lblRecoveryTitle = new();
    private readonly Label lblRecoveryStatus = new();
    private readonly CenteredCaptionButton btnVerifyPristine = new();
    private readonly CenteredCaptionButton btnRestoreBaselineLauncher = new();
    private readonly CenteredCaptionButton btnRebuildOwnedVariant = new();
    private readonly CenteredCaptionButton btnRemoveOwnedVariant = new();
    private readonly CenteredCaptionButton btnReverseAllPreview = new();
    private readonly ComboBox cmbDefaultVariant = new();
    private VariantCatalog? _variantCatalog;
    private VariantContext? _modsVariant;
    private readonly VariantDirectoryService _variantDirectories = new();
    private readonly CompositeBuildService _compositeBuilds;
    private readonly VariantLauncherService _variantLauncher;
    private readonly VariantBuildStatusService _variantBuildStatus;
    private readonly VariantExecutionService _variantExecution = new(debug: VariantDebugConfiguration.FromEnvironment());
    private readonly RecoverySafetyService _recoverySafety;
    // Set only by the explicit installation activation boundary.  Tabs consume
    // these immutable contexts; they never create an installation context.
    private ProjectContext? _activeProject;
    private VariantContext? _activeVariant;
    private IReadOnlyList<VariantContext> _availableActiveVariants = [];
    private readonly Dictionary<string, ProjectContext> _openedProjects = new(StringComparer.OrdinalIgnoreCase);

    private readonly List<GamePcStringEntry> _gamePcEntries = new();
    private readonly Dictionary<int, string> _gamePcEdits = new();
    // The working buffer intentionally contains saved translation edits so the
    // Text grid can render them.  Keep a separate saved snapshot: a difference
    // from Original is not, by itself, an unsaved editor change.
    private readonly Dictionary<int, string> _savedGamePcEdits = new();
    private readonly SplitContainer mainSplit = new();
    private TableLayoutPanel? _graphicsToolbar;
    private FlowLayoutPanel? _graphicsActionRow;
    private FlowLayoutPanel? _navigationRow;
    private TableLayoutPanel? _headerGrid;
    private readonly CenteredCaptionButton btnReplace = new();
    private readonly CenteredCaptionButton btnClearEdit = new();
    private readonly CenteredCaptionButton btnExport = new();
    private readonly CenteredCaptionButton btnRestore = new();
    private readonly CheckBox chkPixelPerfect = new();
    private readonly Label lblGraphicsVariant = new();
    private readonly ComboBox cmbGraphicsScope = new();
    private readonly CenteredCaptionButton btnSaveGraphicsProject = new();
    private readonly GraphicsVariantService _graphicsVariants = new();
    private ProjectContext? _graphicsProject;
    private VariantContext? _graphicsVariant;
    private GraphicsProjectState? _graphicsProjectState;
    private string _graphicsProjectCode = ProjectVariantOwnership.OriginalCode;
    private bool _graphicsProjectDirty;

    private string? _currentVga;
    private byte[]? _currentData;
    private ParsedTable? _table;
    private readonly Dictionary<int, string> _edits = new();
    private readonly List<ElviraPaletteBank> _paletteBanks = new();
    private readonly Elvira1PaletteResolver _elvira1PaletteResolver = new();
    private readonly Elvira2PaletteResolver _elvira2PaletteResolver = new();
    private int? _manualPaletteBank;
    private string? _pairedPaletteResource;
    private string? _pairedPalettePath;
    private bool _paletteAdvancedVisible;
    private bool _updatingPaletteControl;
    private bool _graphicsStatusIsError;
    private readonly ToolTip paletteToolTip = new();
    private ElviraGameProfile _detectedProfile = ElviraGameProfile.Unknown;
    private readonly InstallationSettingsStore _installationSettings = new();
    private readonly List<GameInstallation> _installations = new();
    private bool _refreshingInstallationSelector;
    private bool _refreshingActiveVariantSelector;
    private bool _applyingInstallation;
    private string? _lastInstallationActivationFailure;
    private bool _refreshingGraphicsGrid;
    private bool _applyingPreviewZoom;

    // These objects are reused for the lifetime of the form. Recreating GDI
    // fonts/bitmaps for every workflow or tab refresh caused cumulative GDI
    // pressure during long edition-switching sessions.
    private readonly Dictionary<Button, ModeButtonFonts> _modeButtonFonts = new();
    private Font? _statusRegularFont;
    private Font? _statusEmphasisFont;
    private Font? _variantMissingFont;
    private Bitmap? _statusInfoImage;
    private Bitmap? _statusSuccessImage;
    private Bitmap? _statusWarningImage;
    private Bitmap? _statusErrorImage;

    // Narrow context-synchronization diagnostics used only by the non-interactive
    // post-freeze regression. They do not participate in normal editor behavior.
    internal int ApplyInstallationRequestedCount { get; private set; }
    internal int ApplyInstallationEffectiveCount { get; private set; }
    internal int ApplyInstallationSuppressedCount { get; private set; }
    internal int GraphicsLoadCount { get; private set; }
    internal int TextLoadCount { get; private set; }
    internal int ModsRefreshCount { get; private set; }
    internal int RuntimeUiLoadCount { get; private set; }
    internal int FontBindCount { get; private set; }
    internal int WorkflowRefreshCount { get; private set; }
    internal int PreviewRenderCount { get; private set; }
    internal int ControlTreeCount => CountControls(this);
    internal bool HasTextDomainSplitForTest => textDomainTabs.TabPages.Contains(tabGameTextDomain) && textDomainTabs.TabPages.Contains(tabRuntimeUiDomain);
    internal bool HasRuntimeUiViewForTest => runtimeUiGrid.Columns.Contains("LogicalId") && runtimeUiGrid.Columns.Contains("Override") && btnSaveRuntimeUi.Parent is not null;
    internal bool HasGraphicsVariantPresentationForTest => lblGraphicsVariant.Parent is not null && cmbGraphicsScope.Items.Count == 3 && btnSaveGraphicsProject.Parent is not null;
    internal bool HasActiveInstallationForTest => _activeProject is not null && _activeVariant is not null;
    internal bool IsNeutralInstallationStateForTest => _activeProject is null && _activeVariant is null &&
        string.IsNullOrWhiteSpace(txtGameDir.Text) && cmbInstallations.SelectedIndex < 0 &&
        _gamePcEntries.Count == 0 && _runtimeUiProject is null && _graphicsProject is null;
    internal bool IsFontLoadedForTest => _embeddedFontEditor?.CurrentSourcePath is not null;
    internal string InstallationStatusForTest => lblStatus.Text ?? string.Empty;
    internal int InstallationOptionCountForTest => cmbInstallations.Items.Count;
    internal ElviraGameProfile ActiveGameForTest => _activeProject?.GameProfile ?? ElviraGameProfile.Unknown;
    internal bool HasLoadedGraphicsForTest => _graphicsProject is not null && cmbZone.Items.Count > 0;
    internal bool HasLoadedTextForTest => _gamePcEntries.Count > 0;
    internal bool HasLoadedRuntimeUiForTest => _runtimeUiProject is not null && _runtimeUiVariant is not null;
    internal string WindowTitleForTest => Text;
    internal bool HasNeutralTextActionStateForTest =>
        btnOpenDataFile.Enabled && !btnReloadTexts.Enabled && !btnSaveTexts.Enabled &&
        !btnSaveAsDataFile.Enabled && !btnCreateDataVariant.Enabled &&
        !btnExportTranslations.Enabled && !btnImportTranslations.Enabled;
    internal bool HasActiveTextActionStateForTest =>
        btnOpenDataFile.Enabled && btnReloadTexts.Enabled && btnSaveTexts.Enabled &&
        btnSaveAsDataFile.Enabled && btnCreateDataVariant.Enabled &&
        btnExportTranslations.Enabled && btnImportTranslations.Enabled;
    internal bool HasActiveOriginalTextActionStateForTest =>
        btnOpenDataFile.Enabled && btnReloadTexts.Enabled && !btnSaveTexts.Enabled &&
        btnSaveAsDataFile.Enabled && btnCreateDataVariant.Enabled &&
        btnExportTranslations.Enabled && btnImportTranslations.Enabled;
    internal bool HasNeutralGraphicsPresentationForTest =>
        _activeProject is null && _currentVga is null && _table is null &&
        !btnReload.Enabled && !btnReplace.Enabled && !btnExport.Enabled && !btnRestore.Enabled &&
        !btnReloadPreview.Enabled &&
        cmbGraphicsScope.SelectedIndex < 0 && !cmbGraphicsScope.Enabled &&
        lblGraphicsVariant.Text == UiText.Get("Graphics.ProjectUnavailable") + ": —" &&
        lblPaletteMode.Text == "—" && !panelPaletteSwatches.Enabled;
    internal bool HasActiveGraphicsPresentationForTest =>
        _activeProject is not null && _graphicsProject is not null && _currentVga is not null &&
        btnReload.Enabled && cmbGraphicsScope.SelectedIndex >= 0 &&
        lblGraphicsVariant.Text != UiText.Get("Graphics.ProjectUnavailable") + ": —";
    internal string TextNavigationCaptionForTest => btnTextEditor.Text;
    internal void ActivateTextModeForTest() => SwitchMode(tabText);
    internal void ActivateGraphicsModeForTest() => SwitchMode(tabVga);
    internal bool HasGraphicsValidationErrorForTest => _graphicsStatusIsError;
    internal void RecordGraphicsValidationErrorForTest() => SetGraphicsStatus(UiText.Get("Graphics.PngPaletteInvalid"), true);
    internal void ApplyRelevantGraphicsInputChangeForTest() => ClearStaleGraphicsError();
    internal void RecordSuccessfulReplacementForTest() => SetGraphicsStatus(UiText.Get("ReplacementLoaded"), false);
    internal bool IsVariantAddEnabledForTest => btnVariantAdd.Enabled;
    internal bool HasNeutralStartupPresentationForTest =>
        IsNeutralInstallationStateForTest &&
        lblDetectedGame.Text == UiText.Get(UiLocalizationKeys.NoGameSelected) &&
        lblStatus.Text == NeutralInstallationPrompt &&
        lblStatus.ForeColor == SystemColors.ControlText &&
        HasNeutralGraphicsPresentationForTest &&
        !lblTextOverview.Text.Contains(GamePcOriginalService.OriginalFileName, StringComparison.OrdinalIgnoreCase);
    internal string NeutralStartupDiagnosticForTest =>
        $"project={_activeProject is not null};variant={_activeVariant is not null};detected='{lblDetectedGame.Text}';status='{lblStatus.Text}';graphics='{lblGraphicsVariant.Text}';palette='{lblPaletteMode.Text}';actions={btnReload.Enabled}/{btnReplace.Enabled}/{btnExport.Enabled}/{btnRestore.Enabled}/{btnReloadPreview.Enabled};text='{lblTextOverview.Text}'";
    internal string UiStateDiagnosticForTest =>
        $"neutral={HasNeutralStartupPresentationForTest};text={HasNeutralTextActionStateForTest};variantAdd={btnVariantAdd.Enabled};" +
        $"textActions={btnOpenDataFile.Enabled}/{btnReloadTexts.Enabled}/{btnSaveTexts.Enabled}/{btnSaveAsDataFile.Enabled}/{btnCreateDataVariant.Enabled}/{btnExportTranslations.Enabled}/{btnImportTranslations.Enabled}; " +
        NeutralStartupDiagnosticForTest;
    internal string? FontSourceForTest => _embeddedFontEditor?.CurrentSourcePath;
    internal string? LastInstallationActivationFailureForTest => _lastInstallationActivationFailure;
    internal string ModsPresentationForTest => lblLauncherReadiness.Text;
    internal ProjectContext? ActiveProjectForTest => _activeProject;
    internal VariantContext? ActiveVariantForTest => _activeVariant;
    internal IReadOnlyList<VariantContext> AvailableActiveVariantsForTest => _availableActiveVariants;
    internal bool VariantAwareTabsUseActiveVariantForTest =>
        (_graphicsVariant is null || ReferenceEquals(_graphicsVariant, _activeVariant)) &&
        (_runtimeUiVariant is null || ReferenceEquals(_runtimeUiVariant, _activeVariant)) &&
        (_embeddedFontEditor is null || ReferenceEquals(_embeddedFontEditor.BoundProjectVariantForTest, _activeVariant)) &&
        (_textVariant is null || ReferenceEquals(_textVariant, _activeVariant)) &&
        (_modsVariant is null || ReferenceEquals(_modsVariant, _activeVariant));
    internal bool HasVariantManagerForTest => variantManagerGrid.Parent is not null && variantManagerGrid.Columns.Count == 7;
    internal IReadOnlyList<VariantManagerRow> VariantManagerRowsForTest => variantManagerGrid.Rows.Cast<DataGridViewRow>()
        .Select(row => row.Tag as VariantManagerRow).Where(row => row is not null).Cast<VariantManagerRow>().ToArray();
    internal IReadOnlyList<string> UiLocaleIdsForTest => cmbUiLanguage.Items.OfType<UiLocaleDescriptor>().Select(locale => locale.Id).ToArray();
    internal string TextOverviewForTest => lblTextOverview.Text;
    internal bool TextOverviewFitsForTest
    {
        get
        {
            Size measured = TextRenderer.MeasureText(lblTextOverview.Text, lblTextOverview.Font,
                new Size(lblTextOverview.ClientSize.Width, int.MaxValue), TextFormatFlags.WordBreak);
            return measured.Width <= lblTextOverview.ClientSize.Width && measured.Height <= lblTextOverview.ClientSize.Height;
        }
    }
    internal IReadOnlyList<string> TextTranslationCodesForTest => cmbTranslationVariant.Items.OfType<TextTranslationSelection>().Select(item => item.Code).ToArray();
    internal string ActiveProjectCodeForTest => _activeTranslationCode;
    internal IReadOnlyList<string> ActiveEditionCodesForTest => cmbActiveProject.Items.OfType<TextTranslationSelection>().Select(item => item.Code).ToArray();
    internal bool TextEditionSelectorVisibleForTest => cmbTranslationVariant.Parent?.Visible == true;
    internal string WorkflowStatusForTest => _workflowStatus?.Text ?? string.Empty;
    internal IReadOnlyDictionary<int, string> TextEditsForTest => new Dictionary<int, string>(_gamePcEdits);
    internal bool HasUnsavedTextChangesForTest => HasUnsavedTextChanges;
    internal int CurrentGraphicsEditCountForTest => _edits.Count;
    internal int GraphicsProjectStateEditCountForTest => _graphicsProjectState?.Edits.Count ?? 0;
    internal int AppliedGraphicsEditCountForTest => _edits.Count;
    internal int RuntimeUiOverrideCountForTest => _runtimeUiState?.Overrides.Count ?? 0;
    internal int EmbeddedFontProjectEditCountForTest => _embeddedFontEditor?.ProjectEditCountForTest ?? 0;
    internal IReadOnlyList<VariantEntry> TranslationCatalogEntriesForTest => variantGrid.Rows.Cast<DataGridViewRow>()
        .Select(row => row.Tag as VariantEntry).Where(entry => entry is not null).Cast<VariantEntry>().ToArray();

    public MainForm()
    {
        _runtimeUiLayouts = new RuntimeUiLayoutValidationService(_runtimeUiTexts);
        _compositeBuilds = new CompositeBuildService(new DisposableVariantBuildService(_variantDirectories), _variantDirectories,
            stepProvider: CreateActiveProjectBuildSteps,
            runtimeArtifactProvider: GetActiveProjectRuntimeArtifacts,
            projectVariantProvider: (_, _) => _activeTranslationCode);
        _variantLauncher = new VariantLauncherService(_variantDirectories, _compositeBuilds);
        _variantBuildStatus = new VariantBuildStatusService(_compositeBuilds, _variantLauncher);
        _recoverySafety = new RecoverySafetyService(_variantDirectories, _compositeBuilds);
        AutoScaleMode = AutoScaleMode.Dpi;
        KeyPreview = true;
        Text = AppInfo.ProductTitle;
        Width = 1450;
        Height = 900;
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        MinimumSize = new Size(1100, 700);
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

        UiText.SetLanguage(UiLanguage.English);
        BuildUi();
        UiText.LocaleChanged += OnUiLocaleChanged;
        UiText.LocalesChanged += OnUiLocalesChanged;
        Disposed += (_, _) =>
        {
            UiText.LocaleChanged -= OnUiLocaleChanged;
            UiText.LocalesChanged -= OnUiLocalesChanged;
            DisposePresentationResources();
        };
        KeyDown += (_, e) =>
        {
            if (e.KeyCode != Keys.F1) return;
            OpenHelp();
            e.Handled = true;
            e.SuppressKeyPress = true;
        };

        Shown += (_, _) =>
        {
            // The editor is intended as a workspace application; always start maximized.
            WindowState = FormWindowState.Maximized;
            UiText.SetLanguage(UiLanguage.English);
            RefreshUiLocaleSelector();
            ApplyLanguage();
            InitializeInstallations();

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

    // One shared visual configuration for normal MainForm action buttons.
    // Fixed 32 px logical height; optical vertical-centering padding keeps the
    // visible caption mass centered (measured: Padding(6,1,6,1) renders the
    // caption ~1 px high, Padding(6,2,6,0) centers it, Padding(6,3,6,0)
    // already sits low; total vertical padding stays 2 px so rectangles are
    // unchanged). Width growth for localized captions is opt-in per container
    // so fixed toolbars keep deterministic geometry while wrapping flows may
    // reflow horizontally. Height never changes: preferred text height stays
    // below the 32 px MinimumSize floor at supported fonts/DPIs.
    private static void ConfigureCenteredButton(Button button, bool allowWidthGrowth = false)
    {
        button.AutoSize = false;
        button.Height = 32;
        button.MinimumSize = new Size(0, 32);
        button.Padding = new Padding(6, 2, 6, 0);
        button.TextAlign = ContentAlignment.MiddleCenter;
        button.UseCompatibleTextRendering = false;
        if (allowWidthGrowth)
        {
            // Floor preserves the EN minimum; only width may grow for SK/CZ.
            button.MinimumSize = new Size(button.Width, 32);
            button.AutoSize = true;
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        }
    }

    private static void ConfigureComboLabel(Label label, int width)
    {
        label.AutoSize = false;
        label.Size = new Size(width, 26);
        label.TextAlign = ContentAlignment.MiddleLeft;
        label.Anchor = AnchorStyles.Left;
    }

    private void OpenHelp()
    {
        try
        {
            if (_helpViewer is not null && !_helpViewer.IsDisposed)
            {
                _helpViewer.SetLanguage(UiText.Language);
                _helpViewer.Activate();
                return;
            }

            _helpViewer = new HelpViewerForm(UiText.Language);
            _helpViewer.FormClosed += (_, _) => _helpViewer = null;
            _helpViewer.Show(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("Help"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void OpenAbout()
    {
        try
        {
            if (_aboutViewer is not null && !_aboutViewer.IsDisposed)
            {
                _aboutViewer.SetLanguage(UiText.Language);
                _aboutViewer.Activate();
                return;
            }

            _aboutViewer = new AboutViewerForm(UiText.Language);
            _aboutViewer.FormClosed += (_, _) => _aboutViewer = null;
            _aboutViewer.Show(this);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("About"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BuildTextEditorUi()
    {
        // Stabilized layout: the previous 168 px fixed panel used absolute
        // SetBounds coordinates (buttons up to x=1008, context group at
        // x=1020) which clipped on narrow windows, overlapped under DPI
        // scaling, and overflowed with longer SK/CZ captions. Use stacked
        // Dock.Top AutoSize rows (header table + wrapping action/filter
        // flows) so resize/DPI/localization reflow instead of clipping.
        var topText = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(8)
        };

        var headerTable = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
        headerTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60));

        lblTextSource.AutoSize = true;
        lblTextSource.Dock = DockStyle.Fill;
        lblTextSource.TextAlign = ContentAlignment.MiddleLeft;
        lblTextSource.Font = new Font(Font, FontStyle.Bold);

        lblTextOverview.AutoSize = true;
        lblTextOverview.Dock = DockStyle.Fill;
        lblTextOverview.TextAlign = ContentAlignment.MiddleLeft;
        lblTextOverview.Font = new Font(Font, FontStyle.Bold);
        headerTable.Controls.Add(lblTextSource, 0, 0);
        headerTable.Controls.Add(lblTextOverview, 1, 0);

        var actionFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        btnOpenDataFile.Text = UiText.Get("OpenDataFile");
        btnOpenDataFile.Size = new Size(210, 32);
        btnOpenDataFile.Margin = new Padding(0, 4, 6, 0);
        btnOpenDataFile.Click += (_, _) => OpenTextDataFile();

        lblSearchCaption.Text = UiText.Get("Search");
        lblSearchCaption.AutoSize = true;
        lblSearchCaption.TextAlign = ContentAlignment.MiddleLeft;
        lblSearchCaption.Margin = new Padding(0, 5, 6, 0);

        txtSearch.Size = new Size(260, 26);
        txtSearch.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        txtSearch.Margin = new Padding(0, 4, 6, 0);
        txtSearch.TextChanged += (_, _) => RefreshTextGrid();

        lblEncodingCaption.Text = UiText.Get("Encoding");
        lblEncodingCaption.AutoSize = true;
        lblEncodingCaption.TextAlign = ContentAlignment.MiddleLeft;
        lblEncodingCaption.Margin = new Padding(6, 5, 6, 0);

        cmbTextEncoding.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTextEncoding.Items.AddRange(new object[] { "CP852", "Windows-1250", "Latin1/Raw" });
        cmbTextEncoding.SelectedIndex = 0;
        cmbTextEncoding.Size = new Size(130, 26);
        cmbTextEncoding.Margin = new Padding(0, 4, 6, 0);
        cmbTextEncoding.SelectedIndexChanged += (_, _) => RefreshTextGrid();

        btnReloadTexts.Text = UiText.Get("ReloadTexts");
        btnReloadTexts.Size = new Size(100, 32);
        btnReloadTexts.Margin = new Padding(0, 4, 6, 0);
        btnReloadTexts.Click += (_, _) => LoadGamePcTexts();

        btnSaveTexts.Text = UiText.Get("SaveToProject");
        btnSaveTexts.Size = new Size(145, 32);
        btnSaveTexts.Margin = new Padding(0, 4, 6, 0);
        btnSaveTexts.Click += (_, _) => SaveGamePcTexts();

        btnSaveAsDataFile.Text = UiText.Get("SaveAsDataFile");
        btnSaveAsDataFile.Size = new Size(125, 32);
        btnSaveAsDataFile.Margin = new Padding(0, 4, 6, 0);
        btnSaveAsDataFile.Click += (_, _) => SaveTextDataFileAs(createVariant: false);

        btnCreateDataVariant.Text = UiText.Get("CreateVariant");
        btnCreateDataVariant.Size = new Size(160, 32);
        btnCreateDataVariant.Margin = new Padding(0, 4, 6, 0);
        btnExportTranslations.Size = new Size(100, 32);
        btnExportTranslations.Margin = new Padding(0, 4, 6, 0);
        btnImportTranslations.Size = new Size(100, 32);
        btnImportTranslations.Margin = new Padding(0, 4, 0, 0);
        btnExportTranslations.Text = UiText.Get("ExportTranslation");
        btnImportTranslations.Text = UiText.Get("ImportTranslation");
        btnExportTranslations.Click += (_, _) => ExportTranslations();
        btnImportTranslations.Click += (_, _) => ImportTranslations();
        foreach (Button button in new[] { btnOpenDataFile, btnReloadTexts, btnSaveTexts, btnSaveAsDataFile, btnCreateDataVariant, btnExportTranslations, btnImportTranslations })
            ConfigureCenteredButton(button, allowWidthGrowth: true);
        btnCreateDataVariant.Click += (_, _) => SaveTextDataFileAs(createVariant: true);

        lblTextContextCaption.Text = UiText.Get("TextContext");
        lblTextContextCaption.AutoSize = true;
        lblTextContextCaption.TextAlign = ContentAlignment.MiddleLeft;
        lblTextContextCaption.Margin = new Padding(0, 5, 10, 0);

        cmbTextContext.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTextContext.Items.AddRange(new object[] { UiText.Get("ContextAuto"), UiText.Get("ContextGeneric"), UiText.Get("ContextNpc") });
        cmbTextContext.SelectedIndex = 0;
        cmbTextContext.Width = 230;
        cmbTextContext.Height = 28;
        cmbTextContext.Margin = Padding.Empty;
        cmbTextContext.SelectedIndexChanged += (_, _) =>
        {
            RefreshTextGrid();
            UpdateTextValidation();
            UpdateTextStatusSummary();
        };

        lblTextValidation.AutoSize = true;
        lblTextValidation.TextAlign = ContentAlignment.MiddleLeft;
        lblTextValidation.Margin = new Padding(6, 5, 0, 0);

        var textContextGroup = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(6, 4, 0, 0),
            Padding = Padding.Empty
        };
        textContextGroup.Controls.AddRange(new Control[] { lblTextContextCaption, cmbTextContext });

        lblTranslationVariantCaption.Text = UiText.Get("Edition") + ":";
        lblTranslationVariantCaption.AutoSize = true;
        lblTranslationVariantCaption.TextAlign = ContentAlignment.MiddleLeft;
        lblTranslationVariantCaption.Margin = new Padding(0, 5, 10, 0);

        cmbTranslationVariant.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTranslationVariant.Width = 230;
        cmbTranslationVariant.Height = 28;
        cmbTranslationVariant.Margin = Padding.Empty;
        cmbTranslationVariant.SelectedIndexChanged += (_, _) => SelectTranslationFromSelector();

        var filterFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        filterFlow.Controls.AddRange(new Control[]
        {
            lblSearchCaption, txtSearch, lblEncodingCaption, cmbTextEncoding,
            lblTextValidation
        });

        actionFlow.Controls.AddRange(new Control[]
        {
            btnOpenDataFile, btnReloadTexts, btnSaveTexts, btnSaveAsDataFile, btnCreateDataVariant,
            btnExportTranslations, btnImportTranslations, textContextGroup
        });

        topText.Controls.Add(filterFlow);
        topText.Controls.Add(actionFlow);
        topText.Controls.Add(headerTable);

        textGrid.Dock = DockStyle.Fill;
        textGrid.AllowUserToAddRows = false;
        textGrid.AllowUserToDeleteRows = false;
        textGrid.RowHeadersVisible = false;
        textGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        textGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        textGrid.ShowCellToolTips = true;
        textGrid.Columns.Add("Index", UiText.Get("TextIndex"));
        textGrid.Columns.Add("OriginalBytes", UiText.Get("OriginalBytes"));
        textGrid.Columns.Add("TranslationBytes", UiText.Get("TranslationBytes"));
        textGrid.Columns.Add("ByteDifference", UiText.Get("Difference"));
        textGrid.Columns.Add("DosLimit", UiText.Get("Runtime"));
        textGrid.Columns.Add("OriginalText", UiText.Get("OriginalText"));
        textGrid.Columns.Add("Translation", UiText.Get("Translation"));
        textGrid.Columns["Index"].Width = 70;
        textGrid.Columns["OriginalBytes"].Width = 85;
        textGrid.Columns["TranslationBytes"].Width = 95;
        textGrid.Columns["ByteDifference"].Width = 65;
        textGrid.Columns["DosLimit"].Width = 110;
        textGrid.Columns["DosLimit"].HeaderCell.ToolTipText = UiText.Get("RuntimeDiagnosticTooltip");
        textGrid.Columns["OriginalText"].ReadOnly = true;
        textGrid.Columns["OriginalText"].MinimumWidth = 320;
        textGrid.Columns["OriginalText"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        textGrid.Columns["Translation"].MinimumWidth = 320;
        textGrid.Columns["Translation"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        textGrid.CellEndEdit += (_, e) =>
        {
            if (_refreshingTextGrid || e.RowIndex < 0) return;
            if (ProjectVariantOwnership.IsOriginal(_activeTranslationCode)) return;
            var row = textGrid.Rows[e.RowIndex];
            if (row.Tag is not GamePcStringEntry entry) return;
            if (textGrid.Columns[e.ColumnIndex].Name != "Translation") return;
            string value = row.Cells["Translation"].Value?.ToString() ?? "";
            _gamePcEdits[entry.Index] = value;
            row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
            RefreshWorkflowStatus();

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
            if (textGrid.Columns[e.ColumnIndex].Name != "Translation" && textGrid.Columns[e.ColumnIndex].Name != "DosLimit") return;
            var row = textGrid.Rows[e.RowIndex];
            string value = row.Cells["Translation"].Value?.ToString() ?? string.Empty;
            if (row.Tag is GamePcStringEntry entry)
                e.ToolTipText = BuildDialogueTooltip(entry, value);
            else
                e.ToolTipText = value;
        };

        lblTextStatus.Dock = DockStyle.Bottom;
        lblTextStatus.AutoSize = true;
        lblTextStatus.MinimumSize = new Size(0, 28);
        lblTextStatus.Padding = new Padding(6, 6, 0, 0);

        // The main application toolbar overlays the top ~75 px of the tab area.
        // Use the same 75 px content offset as the VGA editor so the text-editor
        // toolbar (Back to VGA / data-file actions) stays visible.
        var textHost = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = Padding.Empty
        };

        textHost.Controls.Add(textGrid);
        textHost.Controls.Add(topText);
        textHost.Controls.Add(lblTextStatus);
        tabGameTextDomain.Controls.Add(textHost);
        BuildRuntimeUiTextUi();
        textDomainTabs.Dock = DockStyle.Fill;
        textDomainTabs.TabPages.Clear();
        textDomainTabs.TabPages.Add(tabGameTextDomain);
        textDomainTabs.TabPages.Add(tabRuntimeUiDomain);
        tabText.Controls.Add(textDomainTabs);
    }

    private void BuildRuntimeUiTextUi()
    {
        // Stabilized: previously a fixed 74 px panel with absolute positions;
        // longer SK/CZ button captions overflowed fixed 140 px slots. Use an
        // AutoSize header label plus a wrapping button flow.
        var toolbar = new Panel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8) };
        lblRuntimeUiRuntime.AutoSize = true;
        lblRuntimeUiRuntime.Dock = DockStyle.Top;
        lblRuntimeUiRuntime.Font = new Font(Font, FontStyle.Bold);
        lblRuntimeUiRuntime.TextAlign = ContentAlignment.MiddleLeft;
        var runtimeButtonFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        btnReloadRuntimeUi.Text = UiText.Get("RuntimeUi.Reload");
        btnReloadRuntimeUi.Size = new Size(140, 30);
        btnReloadRuntimeUi.Margin = new Padding(0, 4, 6, 0);
        btnReloadRuntimeUi.Click += (_, _) => LoadRuntimeUiText();
        btnSaveRuntimeUi.Text = UiText.Get("SaveToProject");
        btnSaveRuntimeUi.Size = new Size(140, 30);
        btnSaveRuntimeUi.Margin = new Padding(0, 4, 6, 0);
        btnSaveRuntimeUi.Click += (_, _) => SaveRuntimeUiText();
        btnResetRuntimeUi.Text = UiText.Get("RuntimeUi.ResetOverride");
        btnResetRuntimeUi.Size = new Size(140, 30);
        btnResetRuntimeUi.Margin = new Padding(0, 4, 0, 0);
        btnResetRuntimeUi.Click += (_, _) => ResetSelectedRuntimeUiOverride();
        foreach (Button button in new[] { btnReloadRuntimeUi, btnSaveRuntimeUi, btnResetRuntimeUi }) ConfigureCenteredButton(button, allowWidthGrowth: true);
        runtimeButtonFlow.Controls.AddRange(new Control[] { btnReloadRuntimeUi, btnSaveRuntimeUi, btnResetRuntimeUi });
        toolbar.Controls.Add(runtimeButtonFlow);
        toolbar.Controls.Add(lblRuntimeUiRuntime);

        runtimeUiGrid.Dock = DockStyle.Fill;
        runtimeUiGrid.AllowUserToAddRows = false;
        runtimeUiGrid.AllowUserToDeleteRows = false;
        runtimeUiGrid.RowHeadersVisible = false;
        runtimeUiGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        runtimeUiGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        runtimeUiGrid.Columns.Add("LogicalId", UiText.Get("RuntimeUi.Record"));
        runtimeUiGrid.Columns.Add("Original", UiText.Get("RuntimeUi.OriginalText"));
        runtimeUiGrid.Columns.Add("Override", UiText.Get("RuntimeUi.ProjectOverride"));
        runtimeUiGrid.Columns.Add("Status", UiText.Get("RuntimeUi.Validation"));
        runtimeUiGrid.Columns.Add("Detail", UiText.Get("RuntimeUi.Details"));
        runtimeUiGrid.Columns["LogicalId"].Width = 155;
        runtimeUiGrid.Columns["Original"].Width = 190;
        runtimeUiGrid.Columns["Override"].MinimumWidth = 260;
        runtimeUiGrid.Columns["Override"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        runtimeUiGrid.Columns["Status"].Width = 150;
        runtimeUiGrid.Columns["Detail"].Width = 360;
        runtimeUiGrid.Columns["LogicalId"].ReadOnly = true;
        runtimeUiGrid.Columns["Original"].ReadOnly = true;
        runtimeUiGrid.Columns["Status"].ReadOnly = true;
        runtimeUiGrid.Columns["Detail"].ReadOnly = true;
        runtimeUiGrid.CellEndEdit += (_, e) => ApplyRuntimeUiGridEdit(e.RowIndex, e.ColumnIndex);
        runtimeUiGrid.SelectionChanged += (_, _) => UpdateRuntimeUiActions();

        lblRuntimeUiStatus.Dock = DockStyle.Bottom;
        lblRuntimeUiStatus.AutoSize = true;
        lblRuntimeUiStatus.MinimumSize = new Size(0, 28);
        lblRuntimeUiStatus.Padding = new Padding(6, 6, 0, 0);
        var host = new Panel { Dock = DockStyle.Fill, Padding = Padding.Empty };
        host.Controls.Add(runtimeUiGrid);
        host.Controls.Add(toolbar);
        host.Controls.Add(lblRuntimeUiStatus);
        tabRuntimeUiDomain.Controls.Add(host);
    }

    private void BuildModsLauncherUi()
    {
        // Stabilized: previously a fixed 416 px panel with ~23 absolute
        // SetBounds positions (fixed 880 px labels/grid, buttons up to
        // x=880). Longer SK/CZ captions overflowed fixed slots and the grid
        // never stretched on resize/DPI. Use stacked Dock.Top AutoSize rows
        // with wrapping flows plus a Dock.Top grid that stretches width.
        var authoring = new Panel { Dock = DockStyle.Top, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, Padding = new Padding(8) };
        var fileFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        lblLauncherFile.AutoSize = true;
        lblLauncherFile.TextAlign = ContentAlignment.MiddleLeft;
        lblLauncherFile.Margin = new Padding(0, 7, 6, 0);
        txtLauncherFile.Size = new Size(175, 26);
        txtLauncherFile.Margin = new Padding(0, 4, 6, 0);
        btnSelectLauncherFile.Size = new Size(105, 32);
        btnSelectLauncherFile.Margin = new Padding(0, 4, 12, 0);
        lblModderName.AutoSize = true;
        lblModderName.TextAlign = ContentAlignment.MiddleLeft;
        lblModderName.Margin = new Padding(0, 7, 6, 0);
        txtModderName.Size = new Size(190, 26);
        txtModderName.Margin = new Padding(0, 4, 0, 0);
        fileFlow.Controls.AddRange(new Control[] { lblLauncherFile, txtLauncherFile, btnSelectLauncherFile, lblModderName, txtModderName });
        var defaultFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        lblDefaultVariant.AutoSize = true;
        lblDefaultVariant.TextAlign = ContentAlignment.MiddleLeft;
        lblDefaultVariant.Margin = new Padding(0, 7, 6, 0);
        cmbDefaultVariant.Size = new Size(175, 26);
        cmbDefaultVariant.Margin = new Padding(0, 4, 0, 0);
        defaultFlow.Controls.AddRange(new Control[] { lblDefaultVariant, cmbDefaultVariant });
        var launcherFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        btnPreviewLauncher.Size = new Size(145, 32);
        btnPreviewLauncher.Margin = new Padding(0, 4, 6, 0);
        btnGenerateLauncher.Size = new Size(275, 32);
        btnGenerateLauncher.Margin = new Padding(0, 4, 6, 0);
        btnRestoreLauncher.Size = new Size(245, 32);
        btnRestoreLauncher.Margin = new Padding(0, 4, 0, 0);
        launcherFlow.Controls.AddRange(new Control[] { btnPreviewLauncher, btnGenerateLauncher, btnRestoreLauncher });
        var runFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        btnRunVariant.Size = new Size(145, 30);
        btnRunVariant.Margin = new Padding(0, 4, 6, 0);
        btnDebugVariant.Size = new Size(145, 30);
        btnDebugVariant.Margin = new Padding(0, 4, 0, 0);
        runFlow.Controls.AddRange(new Control[] { btnRunVariant, btnDebugVariant });
        btnRunVariant.Text = UiText.Get("Execution.Run"); btnDebugVariant.Text = UiText.Get("Execution.Debug");
        btnRunVariant.Click += (_, _) => ExecuteActiveVariant(VariantExecutionMode.Run);
        btnDebugVariant.Click += (_, _) => ExecuteActiveVariant(VariantExecutionMode.Debug);
        lblLauncherReadiness.AutoSize = true;
        lblLauncherReadiness.Dock = DockStyle.Top;
        lblLauncherReadiness.TextAlign = ContentAlignment.MiddleLeft;
        lblLauncherReadiness.Margin = new Padding(0, 6, 0, 0);
        lblRecoveryTitle.AutoSize = true;
        lblRecoveryTitle.Dock = DockStyle.Top;
        lblRecoveryTitle.Text = UiText.Get("Recovery.Title"); lblRecoveryTitle.Font = new Font(Font, FontStyle.Bold);
        lblRecoveryTitle.Margin = new Padding(0, 8, 0, 0);
        var recoveryFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        btnVerifyPristine.Size = new Size(145, 30);
        btnVerifyPristine.Margin = new Padding(0, 4, 6, 0);
        btnRestoreBaselineLauncher.Size = new Size(210, 30);
        btnRestoreBaselineLauncher.Margin = new Padding(0, 4, 6, 0);
        btnRebuildOwnedVariant.Size = new Size(165, 30);
        btnRebuildOwnedVariant.Margin = new Padding(0, 4, 6, 0);
        btnRemoveOwnedVariant.Size = new Size(165, 30);
        btnRemoveOwnedVariant.Margin = new Padding(0, 4, 6, 0);
        btnReverseAllPreview.Size = new Size(155, 30);
        btnReverseAllPreview.Margin = new Padding(0, 4, 0, 0);
        recoveryFlow.Controls.AddRange(new Control[] { btnVerifyPristine, btnRestoreBaselineLauncher, btnRebuildOwnedVariant, btnRemoveOwnedVariant, btnReverseAllPreview });
        btnVerifyPristine.Text = UiText.Get("Recovery.VerifyPristine"); btnRestoreBaselineLauncher.Text = UiText.Get("Recovery.RestoreLauncher");
        btnRebuildOwnedVariant.Text = UiText.Get("Recovery.RebuildVariant"); btnRemoveOwnedVariant.Text = UiText.Get("Recovery.RemoveVariant"); btnReverseAllPreview.Text = UiText.Get("Recovery.ReverseAll");
        btnSaveGraphicsProject.Text = UiText.Get("SaveToProject");
        btnVerifyPristine.Click += (_, _) => VerifyPristineInstallation();
        btnRestoreBaselineLauncher.Click += (_, _) => RestoreBaselineLauncher();
        btnRebuildOwnedVariant.Click += (_, _) => RebuildOwnedVariant();
        btnRemoveOwnedVariant.Click += (_, _) => RemoveOwnedVariantDirectory();
        btnReverseAllPreview.Click += (_, _) => PreviewReverseAllChanges();
        lblRecoveryStatus.AutoSize = true;
        lblRecoveryStatus.Dock = DockStyle.Top;
        lblRecoveryStatus.TextAlign = ContentAlignment.MiddleLeft;
        lblRecoveryStatus.Margin = new Padding(0, 4, 0, 0);
        var managerHeaderFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        lblVariantManagerTitle.AutoSize = true;
        lblVariantManagerTitle.Font = new Font(Font, FontStyle.Bold);
        lblVariantManagerTitle.Margin = new Padding(0, 8, 8, 0);
        lblVariantManagerHint.AutoSize = true;
        lblVariantManagerHint.TextAlign = ContentAlignment.MiddleLeft;
        lblVariantManagerHint.Margin = new Padding(0, 10, 0, 0);
        lblVariantManagerTitle.Text = UiText.Get("VariantManager.Title");
        lblVariantManagerHint.Text = UiText.Get("VariantManager.SelectHint");
        managerHeaderFlow.Controls.AddRange(new Control[] { lblVariantManagerTitle, lblVariantManagerHint });
        variantManagerGrid.Dock = DockStyle.Top;
        variantManagerGrid.Height = 102;
        variantManagerGrid.Margin = new Padding(0, 4, 0, 0);
        variantManagerGrid.AllowUserToAddRows = false;
        variantManagerGrid.AllowUserToDeleteRows = false;
        variantManagerGrid.AllowUserToResizeRows = false;
        variantManagerGrid.RowHeadersVisible = false;
        variantManagerGrid.ReadOnly = true;
        variantManagerGrid.MultiSelect = false;
        variantManagerGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        variantManagerGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        variantManagerGrid.BackgroundColor = SystemColors.Window;
        variantManagerGrid.BorderStyle = BorderStyle.FixedSingle;
        variantManagerGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Active", HeaderText = UiText.Get("VariantManager.Active"), Width = 50 });
        variantManagerGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Variant", HeaderText = UiText.Get("VariantManager.Variant"), Width = 125 });
        variantManagerGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Runtime", HeaderText = UiText.Get("VariantManager.Runtime"), Width = 110 });
        variantManagerGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Directory", HeaderText = UiText.Get("VariantManager.Directory"), Width = 85 });
        variantManagerGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ownership", HeaderText = UiText.Get("VariantManager.Ownership"), Width = 145 });
        variantManagerGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "BuildStatus", HeaderText = UiText.Get("VariantManager.BuildStatus"), Width = 100 });
        variantManagerGrid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Readiness", HeaderText = UiText.Get("VariantManager.Readiness"), Width = 170 });
        variantManagerGrid.SelectionChanged += (_, _) => SelectVariantManagerRow();
        // One shared configuration: EN widths stay minima, longer SK/CZ
        // captions grow in width only; wrapping flows reflow the extra width.
        foreach (Button button in new[] { btnSelectLauncherFile, btnPreviewLauncher, btnGenerateLauncher, btnRestoreLauncher, btnRunVariant, btnDebugVariant, btnVerifyPristine, btnRestoreBaselineLauncher, btnRebuildOwnedVariant, btnRemoveOwnedVariant, btnReverseAllPreview })
            ConfigureCenteredButton(button, allowWidthGrowth: true);
        btnSelectLauncherFile.Click += (_, _) => SelectLauncherFile();
        btnPreviewLauncher.Click += (_, _) => PreviewLauncher();
        btnGenerateLauncher.Click += (_, _) => GenerateLauncher();
        btnRestoreLauncher.Click += (_, _) => RestoreLauncher();
        SetModsLauncherControls(active: false);
        // Dock.Top stacking: last added docks first at the very top, so add
        // bottom-up to obtain file/default/launcher/run/readiness/recovery/
        // status/manager-header/grid visual order.
        authoring.Controls.Add(variantManagerGrid);
        authoring.Controls.Add(managerHeaderFlow);
        authoring.Controls.Add(lblRecoveryStatus);
        authoring.Controls.Add(recoveryFlow);
        authoring.Controls.Add(lblRecoveryTitle);
        authoring.Controls.Add(lblLauncherReadiness);
        authoring.Controls.Add(runFlow);
        authoring.Controls.Add(launcherFlow);
        authoring.Controls.Add(defaultFlow);
        authoring.Controls.Add(fileFlow);
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(8),
            WrapContents = true
        };

        foreach (Button button in new[] { btnVariantAdd, btnVariantEdit, btnVariantRemove, btnVariantMoveUp, btnVariantMoveDown, btnVariantToggleEnabled, btnVariantOpenDataFile })
        {
            button.AutoSize = false;
            button.Width = button == btnVariantOpenDataFile ? 270 : button == btnVariantMoveDown ? 125 : button == btnVariantMoveUp ? 115 : 95;
            ConfigureCenteredButton(button, allowWidthGrowth: true);
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

        var host = new Panel { Dock = DockStyle.Fill, Padding = Padding.Empty };
        host.Controls.Add(variantGrid);
        host.Controls.Add(actions);
        host.Controls.Add(authoring);
        tabMods.Controls.Add(host);
    }

    private VariantCatalog EnsureVariantCatalog()
    {
        if (_activeProject is null)
            throw new InvalidOperationException("No game installation is active.");
        string directory = _activeProject.GameRoot;
        if (_variantCatalog is null || !_variantCatalog.InstallationDirectory.Equals(directory, StringComparison.OrdinalIgnoreCase))
            _variantCatalog = VariantConfigurationService.Load(directory, ActiveGameProfile);
        return _variantCatalog;
    }

    private VariantEntry? SelectedVariant => variantGrid.CurrentRow?.Tag as VariantEntry;

    private IEnumerable<VariantEntry> GetVariantGridEntries(VariantCatalog catalog)
    {
        var entries = catalog.Entries.ToDictionary(entry => entry.DataFile, StringComparer.OrdinalIgnoreCase);
        int nextOrder = entries.Count == 0 ? 1 : entries.Values.Max(entry => entry.Order) + 1;

        // text-translations.json is the authority for editable translation
        // projects.  A missing GAMEPCxx build artifact must not make the
        // project disappear from the launcher/catalog presentation.
        foreach (TranslationProjectVariant translation in (_translationProjectState?.Variants ?? []).OrderBy(item => item.Code, StringComparer.Ordinal))
        {
            int order = entries.TryGetValue(translation.DataFile, out VariantEntry? existing) ? existing.Order : nextOrder++;
            entries[translation.DataFile] = new VariantEntry(translation.DisplayName, translation.DataFile, true, order, translation.Code, translation.ExeFile);
        }

        return entries.Values.OrderBy(entry => entry.Order).ThenBy(entry => entry.DataFile, StringComparer.OrdinalIgnoreCase);
    }

    private bool TryGetProjectTranslation(VariantEntry? entry, out TranslationProjectVariant translation)
    {
        translation = null!;
        if (entry is null || _translationProjectState is null) return false;
        translation = _translationProjectState.Variants.SingleOrDefault(item =>
            item.Code.Equals(entry.Code, StringComparison.OrdinalIgnoreCase) ||
            item.DataFile.Equals(entry.DataFile, StringComparison.OrdinalIgnoreCase))!;
        return translation is not null;
    }

    private string CurrentTranslationDataFileForPresentation()
    {
        TranslationProjectVariant? selected = _translationProjectState?.Variants.SingleOrDefault(item =>
            item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase));
        return selected?.DataFile ?? Path.GetFileName(CurrentDataFilePath);
    }

    private void RefreshVariantGrid(string? selectDataFile = null)
    {
        VariantCatalog catalog = EnsureVariantCatalog();
        string? selected = selectDataFile ?? SelectedVariant?.DataFile;
        variantGrid.Rows.Clear();
        DataGridViewRow? selectedRow = null;
        foreach (VariantEntry entry in GetVariantGridEntries(catalog))
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
                _variantMissingFont ??= new Font(variantGrid.Font, FontStyle.Bold);
                row.Cells["Status"].Style.Font = _variantMissingFont;
            }
            if (!entry.Enabled)
                row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
            if (!string.IsNullOrWhiteSpace(selected) && entry.DataFile.Equals(selected, StringComparison.OrdinalIgnoreCase))
                selectedRow = row;
        }
        if (selectedRow is not null)
        {
            selectedRow.Selected = true;
            variantGrid.CurrentCell = selectedRow.Cells[0];
        }
        UpdateVariantActions();
    }

    private void OpenModsLauncher()
    {
        ModsRefreshCount++;
        if (_activeProject is null || _activeVariant is null)
        {
            _modsVariant = null;
            RefreshVariantManager();
            variantGrid.Rows.Clear();
            cmbDefaultVariant.Items.Clear();
            txtLauncherFile.Text = string.Empty;
            txtModderName.Text = string.Empty;
            lblLauncherReadiness.Text = NeutralInstallationPrompt;
            SetModsLauncherControls(active: false);
            SetRecoverySafetyControls(active: false);
            UpdateVariantActions();
            return;
        }
        VariantCatalog catalog = EnsureVariantCatalog();
        _modsVariant = _activeVariant;
        RefreshVariantManager();
        SetModsLauncherControls(active: true);
        txtLauncherFile.Text = catalog.LauncherAuthoring.LauncherFile;
        txtModderName.Text = catalog.LauncherAuthoring.ModderName;
        cmbDefaultVariant.Items.Clear();
        foreach (VariantEntry entry in catalog.Entries) cmbDefaultVariant.Items.Add(entry);
        VariantEntry? defaultEntry = catalog.FindByDataFile(catalog.LauncherAuthoring.DefaultVariant);
        if (defaultEntry is not null) cmbDefaultVariant.SelectedItem = defaultEntry;
        RefreshVariantGrid(CurrentTranslationDataFileForPresentation());
        VariantLaunchTarget target = _variantLauncher.Resolve(_activeProject, _activeVariant);
        LauncherRedirectionPlan plan = _variantLauncher.CreateRedirectionPlan(_activeProject, _activeVariant);
        lblLauncherReadiness.Text = string.Format(UiText.Get("Launcher.Readiness"), target.GameId, target.VariantId, target.Ownership,
            target.BuildConfigured ? UiText.Get("Launcher.Configured") : UiText.Get("Launcher.Incomplete"), target.Readiness, plan.Detail);
        // Root launcher replacement remains a future authorized operation;
        // R6Q presents the plan but cannot write either real launcher.
        btnGenerateLauncher.Enabled = false;
        btnRestoreLauncher.Enabled = false;
        btnRunVariant.Enabled = _variantExecution.IsAvailable(target, VariantExecutionMode.Run);
        btnDebugVariant.Enabled = _variantExecution.IsAvailable(target, VariantExecutionMode.Debug);
        UpdateRecoverySafetyPresentation();
    }

    private void RefreshVariantManager()
    {
        _refreshingVariantManager = true;
        try
        {
            variantManagerGrid.Rows.Clear();
            if (_activeProject is null || _activeVariant is null)
            {
                variantManagerGrid.Enabled = false;
                return;
            }

            variantManagerGrid.Enabled = true;
            foreach (VariantContext variant in _availableActiveVariants)
            {
                VariantBuildStatusProjection status = _variantBuildStatus.Inspect(_activeProject, variant);
                VariantLaunchTarget target = status.Target;
                var row = new VariantManagerRow(variant, target.Ownership, target.Readiness, target.VariantRoot,
                    status.Status, status.ConfiguredCapabilities, status.CapabilityCount);
                int index = variantManagerGrid.Rows.Add(
                    ReferenceEquals(variant, _activeVariant) ? "✓" : string.Empty,
                    variant.DisplayName,
                    UiText.Get("VariantManager.Runtime." + variant.RuntimeKind),
                    variant.DirectoryKey,
                    UiText.Get("VariantManager.Ownership." + target.Ownership),
                    UiText.Get("VariantManager.BuildStatus." + status.Status),
                    UiText.Get("VariantManager.Readiness." + target.Readiness));
                DataGridViewRow gridRow = variantManagerGrid.Rows[index];
                gridRow.Tag = row;
                if (status.Status != VariantBuildStatus.Ready)
                    gridRow.DefaultCellStyle.ForeColor = status.Status == VariantBuildStatus.Missing ? SystemColors.GrayText : Color.DarkOrange;
                if (ReferenceEquals(variant, _activeVariant))
                {
                    gridRow.Selected = true;
                    variantManagerGrid.CurrentCell = gridRow.Cells[0];
                }
            }
        }
        finally { _refreshingVariantManager = false; }
    }

    private void SelectVariantManagerRow()
    {
        if (_refreshingVariantManager || variantManagerGrid.CurrentRow?.Tag is not VariantManagerRow row ||
            _activeProject is null || !ReferenceEquals(row.Variant.Project, _activeProject))
            return;
        SetActiveVariant(row.Variant);
    }

    private LauncherSettings CurrentLauncherSettings() => LauncherService.ValidateSettings(new LauncherSettings(txtLauncherFile.Text, txtModderName.Text, (cmbDefaultVariant.SelectedItem as VariantEntry)?.DataFile ?? string.Empty));

    private void SetModsLauncherControls(bool active)
    {
        btnVariantAdd.Enabled = active;
        txtLauncherFile.Enabled = active;
        txtModderName.Enabled = active;
        cmbDefaultVariant.Enabled = active;
        btnSelectLauncherFile.Enabled = active;
        btnPreviewLauncher.Enabled = active;
        // Root replacement and restoration are deliberately not R6Q actions.
        btnGenerateLauncher.Enabled = false;
        btnRestoreLauncher.Enabled = false;
        btnRunVariant.Enabled = false;
        btnDebugVariant.Enabled = false;
    }

    private void SetRecoverySafetyControls(bool active)
    {
        btnVerifyPristine.Enabled = active;
        btnRestoreBaselineLauncher.Enabled = false;
        btnRebuildOwnedVariant.Enabled = false;
        btnRemoveOwnedVariant.Enabled = false;
        btnReverseAllPreview.Enabled = active;
        lblRecoveryStatus.Text = active ? string.Empty : UiText.Get(UiLocalizationKeys.NoGameSelected) + " " + UiText.Get(UiLocalizationKeys.RecoveryActionsUnavailable);
    }

    private void UpdateRecoverySafetyPresentation()
    {
        if (_activeProject is null || _activeVariant is null) { SetRecoverySafetyControls(active: false); return; }
        RecoverySafetyInspection inspection = _recoverySafety.Inspect(_activeProject);
        VariantDirectoryOperationResult ownership = _variantDirectories.ValidateOwnedVariantDirectory(_activeProject, _activeVariant);
        bool owned = ownership.Status == VariantDirectoryOperationStatus.AlreadyValid;
        lblRecoveryStatus.Text = string.Format(UiText.Get("Recovery.Status"), inspection.Baseline.Status,
            inspection.ReversePreview.OwnedVariants.Count, inspection.ReversePreview.ForeignOrInvalidVariants.Count, inspection.ReversePreview.Detail);
        btnVerifyPristine.Enabled = true;
        btnRestoreBaselineLauncher.Enabled = inspection.ReversePreview.LauncherBackupAvailable;
        btnRebuildOwnedVariant.Enabled = owned && inspection.Baseline.Status == BaselineValidationStatus.MatchesBaseline;
        btnRemoveOwnedVariant.Enabled = owned;
        btnReverseAllPreview.Enabled = true;
    }

    private void VerifyPristineInstallation()
    {
        if (_activeProject is null) return;
        RecoverySafetyInspection inspection = _recoverySafety.Inspect(_activeProject);
        SetStatus(string.Format(UiText.Get("Recovery.PristineVerification"), inspection.Baseline.Status), inspection.Baseline.Status != BaselineValidationStatus.MatchesBaseline);
        UpdateRecoverySafetyPresentation();
    }

    private void RestoreBaselineLauncher()
    {
        if (_activeProject is null || MessageBox.Show(this, UiText.Get("Recovery.ConfirmRestoreLauncher"), UiText.Get("Recovery.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        RecoverySafetyOperationResult result = _recoverySafety.RestoreOriginalLauncher(_activeProject);
        SetStatus(result.Detail, !result.Succeeded); UpdateRecoverySafetyPresentation();
    }

    private void RebuildOwnedVariant()
    {
        if (_activeProject is null || _activeVariant is null || MessageBox.Show(this, UiText.Get("Recovery.ConfirmRebuildVariant"), UiText.Get("Recovery.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        RecoverySafetyOperationResult result = _recoverySafety.RebuildOwnedVariant(_activeProject, _activeVariant);
        if (result.Succeeded)
            SetWorkflowStatus(WorkflowStatusSeverity.Success, string.Format(UiText.Get("Workflow.BuiltEditionReady"), ActiveEditionDisplayName()));
        else
            SetStatus(result.Detail, error: true);
        RefreshVariantManager();
        UpdateRecoverySafetyPresentation();
    }

    /// <summary>Build input is deliberately taken from the active project
    /// editors, never from launcher-default selection or a DataFile-only row.</summary>
    private IReadOnlyList<ICompositeBuildStep> CreateActiveProjectBuildSteps(ProjectContext project, VariantContext variant)
    {
        if (!ReferenceEquals(project, _activeProject) || !ReferenceEquals(variant, _activeVariant)) return [];
        TranslationProjectVariant translation = GetActiveTranslationForBuild(project, variant);
        GraphicsProjectState graphics = ReferenceEquals(_graphicsProject, project) && _graphicsProjectState is not null
            ? _graphicsProjectState
            : RequireGraphicsState(project, _activeTranslationCode);
        RuntimeUiTextState runtimeUi = ReferenceEquals(_runtimeUiProject, project) && _runtimeUiState is not null
            ? _runtimeUiState
            : RequireRuntimeUiState(project, _activeTranslationCode);
        FontProjectState font = RequireFontState(project, _activeTranslationCode);
        return ActiveProjectCompositeBuildFactory.Create(_variantDirectories, _translationProjects, _graphicsVariants,
            new ActiveProjectBuildInput(translation, graphics, runtimeUi, font.Edits.Count != 0));
    }

    private IReadOnlyList<string> GetActiveProjectRuntimeArtifacts(ProjectContext project, VariantContext variant)
    {
        if (!ReferenceEquals(project, _activeProject) || !ReferenceEquals(variant, _activeVariant))
            return [variant.GeneratedExecutableName, variant.LogicalDataFileName];
        TranslationProjectVariant translation = GetActiveTranslationForBuild(project, variant);
        return [ActiveProjectBuildIdentity.ExecutableName(variant, translation), translation.DataFile];
    }

    private TranslationProjectVariant GetActiveTranslationForBuild(ProjectContext project, VariantContext variant)
    {
        TranslationProjectState state = _translationProjectState ?? RequireTranslationState(project);
        if (_activeTranslationCode.Equals("EN", StringComparison.OrdinalIgnoreCase))
            return new TranslationProjectVariant("English", "EN", variant.LogicalDataFileName, variant.SourceExecutableName, new Dictionary<int, string>());
        return state.Variants.SingleOrDefault(item => item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidDataException("The active translation selection is not present in the project.");
    }

    private GraphicsProjectState RequireGraphicsState(ProjectContext project, string projectVariantCode)
    {
        GraphicsProjectLoadResult loaded = _graphicsVariants.Load(project, projectVariantCode);
        if (!loaded.IsSuccess) throw new InvalidDataException(loaded.Detail ?? "Graphics project data is invalid.");
        return loaded.State!;
    }

    private RuntimeUiTextState RequireRuntimeUiState(ProjectContext project, string projectVariantCode)
    {
        RuntimeUiTextLoadResult loaded = _runtimeUiTexts.Load(project, projectVariantCode);
        if (!loaded.IsSuccess) throw new InvalidDataException(loaded.Detail ?? "Runtime UI project data is invalid.");
        return loaded.State!;
    }

    private FontProjectState RequireFontState(ProjectContext project, string projectVariantCode)
    {
        FontProjectLoadResult loaded = new FontVariantService().Load(project, projectVariantCode);
        if (!loaded.IsSuccess) throw new InvalidDataException(loaded.Detail ?? "Font project data is invalid.");
        return loaded.State!;
    }

    private TranslationProjectState RequireTranslationState(ProjectContext project)
    {
        TranslationProjectLoadResult loaded = _translationProjects.Load(project);
        if (!loaded.IsSuccess) throw new InvalidDataException(loaded.Detail ?? "Translation project data is invalid.");
        return loaded.State!;
    }

    private void RemoveOwnedVariantDirectory()
    {
        if (_activeProject is null || _activeVariant is null || MessageBox.Show(this, UiText.Get("Recovery.ConfirmRemoveVariant"), UiText.Get("Recovery.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        RecoverySafetyOperationResult result = _recoverySafety.RemoveOwnedVariant(_activeProject, _activeVariant);
        SetStatus(result.Detail, !result.Succeeded); UpdateRecoverySafetyPresentation();
    }

    private void PreviewReverseAllChanges()
    {
        if (_activeProject is null) return;
        RestorePlan plan = _recoverySafety.CreateRestorePlan(_activeProject);
        string summary = string.Format(UiText.Get("Recovery.PreviewSummary"),
            plan.Items.Count(item => item.Kind == RestorePlanItemKind.RestoreMutable),
            plan.Items.Count(item => item.Kind == RestorePlanItemKind.RemoveOwnedVariant),
            plan.Items.Count(item => item.Kind == RestorePlanItemKind.RemoveKnownEditorArtifact),
            plan.Items.Count(item => item.Kind == RestorePlanItemKind.ExternalDifferencePreserved),
            plan.Items.Count(item => item.Kind == RestorePlanItemKind.Blocked));
        if (plan.IsBlocked)
        {
            MessageBox.Show(this, summary + "\r\n" + UiText.Get("Recovery.BlockedNoAction"), UiText.Get("Recovery.Title"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (MessageBox.Show(this, summary + "\r\n\r\n" + UiText.Get("Recovery.ConfirmExecute"), UiText.Get("Recovery.Title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        RestorePlanExecutionResult result = _recoverySafety.ExecuteRestorePlan(_activeProject, plan);
        SetStatus(result.Detail, !result.Succeeded); UpdateRecoverySafetyPresentation();
    }

    private void ExecuteActiveVariant(VariantExecutionMode mode)
    {
        VariantLaunchTarget target = _variantLauncher.Resolve(_activeProject, _activeVariant);
        VariantExecutionResult result = _variantExecution.Execute(target, mode);
        if (!result.Started) SetStatus(result.Detail, true);
    }

    private ElviraGameProfile LauncherGameProfile()
    {
        ElviraGameProfile game = ActiveGameProfile;
        if (game is not (ElviraGameProfile.Elvira1 or ElviraGameProfile.Elvira2))
            throw new InvalidDataException(UiText.Get("LauncherGameUnknown"));
        return game;
    }

    private void SelectLauncherFile()
    {
        VariantCatalog catalog = EnsureVariantCatalog();
        using var dialog = new OpenFileDialog { Title = UiText.Get("SelectLauncher"), InitialDirectory = catalog.InstallationDirectory, Filter = "DOS batch files (*.BAT)|*.BAT", CheckFileExists = false, FileName = txtLauncherFile.Text };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        string directory = Path.GetDirectoryName(Path.GetFullPath(dialog.FileName)) ?? string.Empty;
        if (!directory.Equals(catalog.InstallationDirectory, StringComparison.OrdinalIgnoreCase) || !LauncherService.IsValidLauncherFileName(Path.GetFileName(dialog.FileName)))
        { ShowVariantError(UiText.Get("LauncherPathError")); return; }
        txtLauncherFile.Text = Path.GetFileName(dialog.FileName);
    }

    private void PreviewLauncher()
    {
        try
        {
            string text = LauncherService.BuildPreview(LauncherGameProfile(), EnsureVariantCatalog(), CurrentLauncherSettings(), UiText.Language, DateTime.Now);
            using var dialog = new Form { Text = UiText.Get("PreviewLauncher"), StartPosition = FormStartPosition.CenterParent, Size = new Size(850, 650) };
            dialog.Controls.Add(new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, Font = new Font(FontFamily.GenericMonospace, 9), Text = text });
            dialog.ShowDialog(this);
        }
        catch (Exception ex) { ShowVariantError(ex.Message); }
    }

    private void GenerateLauncher()
    {
        try
        {
            VariantCatalog catalog = EnsureVariantCatalog(); LauncherSettings settings = CurrentLauncherSettings();
            string active = Path.Combine(catalog.InstallationDirectory, settings.LauncherFile);
            bool external = LauncherService.ActiveLauncherIsExternallyModified(active);
            if (external && MessageBox.Show(this, UiText.Get("LauncherExternalConfirm"), UiText.Get("Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            LauncherService.Generate(LauncherGameProfile(), catalog, settings, UiText.Language, external);
            catalog.LauncherAuthoring = settings; VariantConfigurationService.Save(catalog);
            MessageBox.Show(this, UiText.Get("LauncherGenerated"), UiText.Get("ModsLauncherTab"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { ShowVariantError(ex.Message); }
    }

    private void RestoreLauncher()
    {
        try
        {
            VariantCatalog catalog = EnsureVariantCatalog(); LauncherSettings settings = CurrentLauncherSettings();
            if (MessageBox.Show(this, UiText.Get("LauncherRestoreConfirm"), UiText.Get("Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            LauncherService.RestoreOriginal(catalog.InstallationDirectory, settings);
            catalog.LauncherAuthoring = settings; VariantConfigurationService.Save(catalog);
        }
        catch (Exception ex) { ShowVariantError(ex.Message); }
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
        if (!moved) { UpdateVariantActions(); return; }
        try { PersistVariants(selected.DataFile); }
        catch (Exception ex)
        {
            _variantCatalog = VariantConfigurationService.Load(catalog.InstallationDirectory, ActiveGameProfile);
            RefreshVariantGrid(selected.DataFile);
            ShowVariantError(ex.Message);
        }
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
        if (TryGetProjectTranslation(selected, out TranslationProjectVariant projectTranslation))
        {
            // Project state remains editable before its disposable variant
            // output has been built.  Never materialize GAMEPCxx in GameRoot
            // just to satisfy this navigation action.
            SelectTranslationVariant(projectTranslation.Code);
            SwitchMode(tabText);
            return;
        }
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
        if (_activeProject is null || _activeVariant is null)
        {
            btnVariantAdd.Enabled = false;
            btnVariantEdit.Enabled = false;
            btnVariantRemove.Enabled = false;
            btnVariantMoveUp.Enabled = false;
            btnVariantMoveDown.Enabled = false;
            btnVariantToggleEnabled.Enabled = false;
            btnVariantOpenDataFile.Enabled = false;
            btnVariantToggleEnabled.Text = UiText.Get("VariantDisable");
            return;
        }
        VariantEntry? selected = SelectedVariant;
        bool hasSelection = selected is not null;
        btnVariantAdd.Enabled = true;
        bool projectTranslation = TryGetProjectTranslation(selected, out _);
        btnVariantEdit.Enabled = hasSelection && !projectTranslation;
        btnVariantRemove.Enabled = hasSelection && !projectTranslation;
        btnVariantMoveUp.Enabled = hasSelection && !projectTranslation && EnsureVariantCatalog().CanMoveUp(selected!.DataFile);
        btnVariantMoveDown.Enabled = hasSelection && !projectTranslation && EnsureVariantCatalog().CanMoveDown(selected!.DataFile);
        btnVariantToggleEnabled.Enabled = hasSelection && !projectTranslation;
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
            if (string.IsNullOrWhiteSpace(txtGameDir.Text)) return string.Empty;
            _currentDataFilePath = Path.Combine(txtGameDir.Text.Trim(), "GAMEPC");
            return _currentDataFilePath;
        }
    }

    private void SetCurrentDataFile(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            _currentDataFilePath = null;
            lblTextSource.Text = string.Empty;
            UpdateTextOverview();
            return;
        }
        _currentDataFilePath = Path.GetFullPath(path);
        lblTextSource.Text = string.Format(UiText.Get("DataFileSource"), Path.GetFileName(_currentDataFilePath));
        UpdateTextOverview();
    }

    private void RefreshTranslationVariantSelector()
    {
        _refreshingTranslationVariantSelector = true;
        try
        {
            cmbTranslationVariant.Items.Clear();
            cmbTranslationVariant.Items.Add(new TextTranslationSelection("EN", UiText.Get("Original"), "GAMEPC", _activeVariant?.SourceExecutableName ?? string.Empty));
            foreach (TranslationProjectVariant translation in (_translationProjectState?.Variants ?? [])
                .Where(item => !item.Code.Equals("EN", StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Code, StringComparer.Ordinal))
            {
                cmbTranslationVariant.Items.Add(new TextTranslationSelection(translation.Code, translation.DisplayName, translation.DataFile, translation.ExeFile));
            }

            TextTranslationSelection? selected = cmbTranslationVariant.Items.OfType<TextTranslationSelection>()
                .SingleOrDefault(item => item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase));
            if (selected is null)
            {
                _activeTranslationCode = "EN";
                selected = cmbTranslationVariant.Items.OfType<TextTranslationSelection>().FirstOrDefault();
            }
            cmbTranslationVariant.SelectedItem = selected;
            cmbTranslationVariant.Enabled = _activeProject is not null;
        }
        finally { _refreshingTranslationVariantSelector = false; }
        RefreshActiveProjectSelector();
    }

    /// <summary>The top-level project selector and the Text selector represent
    /// one shared project/mod choice. They never select a DOS runtime or a
    /// physical GAMEPCxx file independently.</summary>
    private void RefreshActiveProjectSelector()
    {
        _refreshingActiveProjectSelector = true;
        try
        {
            cmbActiveProject.Items.Clear();
            if (_activeProject is not null)
            {
                cmbActiveProject.Items.Add(new TextTranslationSelection("EN", UiText.Get("Original"), "GAMEPC", _activeVariant?.SourceExecutableName ?? string.Empty));
                foreach (TranslationProjectVariant translation in (_translationProjectState?.Variants ?? [])
                    .Where(item => !item.Code.Equals("EN", StringComparison.OrdinalIgnoreCase))
                    .OrderBy(item => item.Code, StringComparer.Ordinal))
                    cmbActiveProject.Items.Add(new TextTranslationSelection(translation.Code, translation.DisplayName, translation.DataFile, translation.ExeFile));
                cmbActiveProject.SelectedItem = cmbActiveProject.Items.OfType<TextTranslationSelection>()
                    .SingleOrDefault(item => item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase));
            }
            cmbActiveProject.Enabled = _activeProject is not null && cmbActiveProject.Items.Count != 0;
        }
        finally { _refreshingActiveProjectSelector = false; }
    }

    private void SelectTranslationFromSelector()
    {
        if (_refreshingTranslationVariantSelector || cmbTranslationVariant.SelectedItem is not TextTranslationSelection selection)
            return;
        SelectTranslationVariant(selection.Code);
    }

    private void SelectTranslationVariant(string code)
    {
        if (_activeProject is null || _translationProjectState is null) return;
        string selectedCode = code.Equals("EN", StringComparison.OrdinalIgnoreCase) ? "EN" : code;
        if (!selectedCode.Equals("EN", StringComparison.OrdinalIgnoreCase) && !_translationProjectState.Variants.Any(item => item.Code.Equals(selectedCode, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("The requested project translation does not exist.");
        if (selectedCode.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase)) return;

        // Persist the currently displayed project edits before changing the
        // project-state selection.  This is never a GAMEPC/GAMEPCxx write.
        if (!_activeTranslationCode.Equals("EN", StringComparison.OrdinalIgnoreCase) && HasUnsavedTextChanges)
        {
            _translationProjectState = _translationProjects.UpsertWorkingEdits(_activeProject, _translationProjectState, _activeTranslationCode, _gamePcEdits);
            _translationProjects.Save(_activeProject, _translationProjectState);
            CaptureSavedTextEdits();
        }

        _activeTranslationCode = selectedCode;
        RefreshTranslationVariantSelector();
        SetCurrentDataFile(Path.Combine(_activeProject.GameRoot, "GAMEPC"));
        LoadGamePcTexts();
        // LoadGamePcTexts refreshes Runtime UI. LoadSelectedZone refreshes the
        // graphics project state, so do not duplicate either synchronous path.
        if (cmbZone.SelectedIndex >= 0) LoadSelectedZone();
        else LoadGraphicsProjectState();
        BindFontEditorToActiveVariant(showMissingError: false);
        OpenModsLauncher();
        RefreshWorkflowStatus();
    }

    private void UpdateTextOverview()
    {
        if (lblTextOverview.IsDisposed) return;
        if (_activeProject is null || _activeVariant is null)
        {
            _textVariant = null;
            lblTextOverview.Text = $"{UiText.Get("ActiveVariant")} —   |   {UiText.Get("Original")}: —   |   {UiText.Get("Translation")}: —   |   EXE: —";
            return;
        }
        _textVariant = _activeVariant;
        bool baselineSelection = _activeTranslationCode.Equals("EN", StringComparison.OrdinalIgnoreCase);
        TranslationProjectVariant? translationVariant = baselineSelection ? null : _translationProjectState?.Variants
            .SingleOrDefault(item => item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase));
        // The baseline source remains GAMEPC.  A project translation describes
        // logical future output names only; rendering this header never probes
        // or creates root-side GAMEPCxx/RUNVGAxx files.
        string translation = baselineSelection ? UiText.Get("Original") : translationVariant?.DataFile ?? (string.IsNullOrWhiteSpace(_currentDataFilePath) ? "—" : Path.GetFileName(_currentDataFilePath));
        string exe = translationVariant?.ExeFile ?? _activeVariant?.SourceExecutableName ?? "—";
        string name = _activeVariant!.DisplayName;
        string original = _gamePcOriginal is null ? "GAMEPC" : Path.GetFileName(_gamePcOriginal.Path);
        string originalStatus = _gamePcOriginal?.Status == GamePcOriginalStatus.VerifiedOriginal ? UiText.Get("VerifiedOriginal") : UiText.Get("BaselineCopy");
        // Two explicit rows retain the original lifecycle context without
        // clipping logical output names at the end of a long single line.
        lblTextOverview.Text = $"{UiText.Get("ActiveVariant")} {name}   |   {UiText.Get("Original")}: {original} ({originalStatus})\r\n" +
            $"{UiText.Get("Translation")}: {translation}   |   EXE: {exe}";
    }

    private bool HasUnsavedTextChanges => _gamePcEdits.Count != _savedGamePcEdits.Count ||
        _gamePcEdits.Any(pair => !_savedGamePcEdits.TryGetValue(pair.Key, out string? saved) || !StringComparer.Ordinal.Equals(pair.Value, saved));

    private void CaptureSavedTextEdits()
    {
        _savedGamePcEdits.Clear();
        foreach ((int index, string text) in _gamePcEdits)
            _savedGamePcEdits[index] = text;
    }

    private void LoadGamePcTexts()
    {
        TextLoadCount++;
        try
        {
            _gamePcEntries.Clear();
            _gamePcEdits.Clear();
            _savedGamePcEdits.Clear();
            _gamePcOriginal = null;
            _translationProjectState = null;
            if (string.IsNullOrWhiteSpace(txtGameDir.Text))
            {
                _currentDataFilePath = null;
                textGrid.Rows.Clear();
                lblTextStatus.Text = UiText.Get("NoSupportedGameSelected");
                ClearRuntimeUiText();
                RefreshTranslationVariantSelector();
                UpdateTextOverview();
                return;
            }
            _textDiagnosticStore = TextDiagnosticStore.Load(txtGameDir.Text.Trim());

            string dataFilePath = CurrentDataFilePath;
            SetCurrentDataFile(dataFilePath);
            if (!File.Exists(dataFilePath))
            {
                lblTextStatus.Text = string.Format(UiText.Get("NoDataFile"), Path.GetFileName(dataFilePath));
                textGrid.Rows.Clear();
                return;
            }

            string directory = txtGameDir.Text.Trim();
            GameInstallationValidator.TryValidate(directory, InstallationDiscoverySource.Manual, out GameInstallation? parserInstallation);
            ElviraGameProfile parserProfile = parserInstallation?.Game ?? ActiveGameProfile;
            if (_activeProject is null || !_activeProject.GameRoot.Equals(directory, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Text project is unavailable until a supported installation is activated.");
            _gamePcOriginal = GamePcOriginalService.LoadProjectBaseline(_activeProject);
            TranslationProjectLoadResult projectText = _translationProjects.Load(_activeProject);
            if (!projectText.IsSuccess) throw new InvalidDataException(projectText.Detail ?? "Translation project data is invalid.");
            TranslationProjectState projectState = projectText.State!;
            _translationProjectState = projectState;
            RefreshTranslationVariantSelector();
            _gamePcEntries.AddRange(GamePcTextEditor.LoadEntries(dataFilePath, parserProfile));
            if (_gamePcOriginal.Entries.Count != _gamePcEntries.Count || !_gamePcOriginal.Entries.Select(entry => entry.Index).SequenceEqual(_gamePcEntries.Select(entry => entry.Index)))
                throw new InvalidDataException($"Original GAMEPC and translation {Path.GetFileName(dataFilePath)} do not have the same logical string indices.");
            if (Path.GetFullPath(dataFilePath).Equals(Path.Combine(_activeProject.GameRoot, "GAMEPC"), StringComparison.OrdinalIgnoreCase))
            {
                TranslationProjectVariant? active = projectState.Variants.SingleOrDefault(item => item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase));
                if (active is not null)
                    foreach ((int index, string text) in active.Edits) _gamePcEdits[index] = text;
            }
            CaptureSavedTextEdits();
            UpdateTextOverview();
            if (_variantCatalog is not null) RefreshVariantGrid(CurrentTranslationDataFileForPresentation());
            RefreshTextGrid();
            UpdateTextStatusSummary();
            _detectedProfile = GameInstallationValidator.TryValidate(directory, InstallationDiscoverySource.Manual, out GameInstallation? installation) && installation is not null
                ? installation.Game
                : GameProfileDetector.Detect(directory, cmbZone.Items.Count, _gamePcEntries.Count);
            UpdateGameProfileDisplay();
            LoadRuntimeUiText();
        }
        catch (Exception ex)
        {
            lblTextStatus.Text = ex.Message;
            LoadRuntimeUiText();
        }
        finally
        {
            UpdateTextActionState();
        }
    }

    /// <summary>Text actions are available only for the explicit active project.
    /// Opening a standalone data file remains an intentional read-only entry point.</summary>
    private void UpdateTextActionState()
    {
        bool projectActive = _activeProject is not null && _activeVariant is not null;
        bool textReady = projectActive && _gamePcEntries.Count > 0;
        btnOpenDataFile.Enabled = true;
        btnReloadTexts.Enabled = projectActive;
        bool editableProject = textReady && !ProjectVariantOwnership.IsOriginal(_activeTranslationCode);
        btnSaveTexts.Enabled = editableProject;
        btnSaveAsDataFile.Enabled = textReady;
        btnCreateDataVariant.Enabled = textReady;
        btnExportTranslations.Enabled = textReady;
        btnImportTranslations.Enabled = textReady;
        cmbTranslationVariant.Enabled = projectActive && _translationProjectState is not null;
        if (textGrid.Columns.Contains("Translation")) textGrid.Columns["Translation"].ReadOnly = !editableProject;
    }

    private void LoadRuntimeUiText()
    {
        RuntimeUiLoadCount++;
        _runtimeUiProject = null;
        _runtimeUiVariant = null;
        _runtimeUiState = null;
        _runtimeUiDirty = false;
        if (_activeProject is null || _activeVariant is null) { ClearRuntimeUiText(); return; }
        if (!_activeProject.GameRoot.Equals(txtGameDir.Text, StringComparison.OrdinalIgnoreCase))
        {
            lblRuntimeUiRuntime.Text = UiText.Get("RuntimeUi.Title");
            lblRuntimeUiStatus.Text = UiText.Get("RuntimeUi.ProjectUnavailable");
            runtimeUiGrid.Rows.Clear();
            UpdateRuntimeUiActions();
            return;
        }
        _runtimeUiProject = _activeProject;
        _runtimeUiVariant = _activeVariant;
        RuntimeUiTextLoadResult loaded = _runtimeUiTexts.Load(_runtimeUiProject, _activeTranslationCode);
        if (!loaded.IsSuccess)
        {
            lblRuntimeUiRuntime.Text = _runtimeUiVariant.DisplayName + " — " + UiText.Get("RuntimeUi.Title");
            lblRuntimeUiStatus.Text = UiText.Get("RuntimeUi.ProjectDataError") + ": " + (loaded.Detail ?? loaded.Status.ToString());
            runtimeUiGrid.Rows.Clear();
            UpdateRuntimeUiActions();
            return;
        }
        _runtimeUiState = loaded.State;
        RefreshRuntimeUiGrid();
    }

    private VariantContext ResolveRuntimeUiVariant(ProjectContext project)
    {
        if (ReferenceEquals(project, _activeProject) && _activeVariant is not null)
            return _activeVariant;
        VariantContext[] variants = VariantContextCatalog.CreateBuiltIns(project).ToArray();
        return variants[0];
    }

    private void ClearRuntimeUiText()
    {
        _runtimeUiProject = null; _runtimeUiVariant = null; _runtimeUiState = null; _runtimeUiDirty = false;
        runtimeUiGrid.Rows.Clear();
        lblRuntimeUiRuntime.Text = UiText.Get("RuntimeUi.Title");
        lblRuntimeUiStatus.Text = UiText.Get("NoSupportedGameSelected");
        UpdateRuntimeUiActions();
    }

    private void RefreshRuntimeUiGrid()
    {
        if (_runtimeUiProject is null || _runtimeUiVariant is null || _runtimeUiState is null) return;
        _runtimeUiRefreshing = true;
        try
        {
            runtimeUiGrid.Columns["Override"].ReadOnly = ProjectVariantOwnership.IsOriginal(_activeTranslationCode);
            runtimeUiGrid.Rows.Clear();
            lblRuntimeUiRuntime.Text = _runtimeUiVariant.DisplayName + " — " + UiText.Get("RuntimeUi.Title");
            foreach (RuntimeUiRuntimeProjection record in _runtimeUiTexts.GetEffectiveRecords(_runtimeUiVariant, _runtimeUiState))
            {
                RuntimeUiLayoutValidationResult validation = _runtimeUiLayouts.Validate(_runtimeUiVariant, _runtimeUiState, record.LogicalRecordId);
                string original = record.TextOrigin == RuntimeUiTextOrigin.FrozenDefaultUnavailable ? UiText.Get("RuntimeUi.OriginalUnavailable") : record.IsOverridden ? UiText.Get("RuntimeUi.OriginalUnavailable") : record.EffectiveText ?? UiText.Get("RuntimeUi.OriginalUnavailable");
                string overrideText = record.IsOverridden ? record.EffectiveText ?? string.Empty : string.Empty;
                int rowIndex = runtimeUiGrid.Rows.Add(record.DisplayName, original, overrideText, FriendlyRuntimeUiStatus(validation), validation.Detail);
                DataGridViewRow row = runtimeUiGrid.Rows[rowIndex];
                row.Tag = record.LogicalRecordId;
                if (record.IsOverridden) row.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                if (validation.Status == RuntimeUiLayoutValidationStatus.MappingIncomplete) row.DefaultCellStyle.ForeColor = Color.DarkOrange;
            }
            lblRuntimeUiStatus.Text = _runtimeUiDirty ? UiText.Get("RuntimeUi.ChangesNotSaved") : UiText.Get("RuntimeUi.ProjectStateLoaded");
        }
        finally { _runtimeUiRefreshing = false; }
        UpdateRuntimeUiActions();
    }

    private void ApplyRuntimeUiGridEdit(int rowIndex, int columnIndex)
    {
        if (_runtimeUiRefreshing || columnIndex < 0 || rowIndex < 0 || _runtimeUiProject is null || _runtimeUiState is null) return;
        if (runtimeUiGrid.Columns[columnIndex].Name != "Override" || runtimeUiGrid.Rows[rowIndex].Tag is not RuntimeUiLogicalRecordId id) return;
        string text = runtimeUiGrid.Rows[rowIndex].Cells["Override"].Value?.ToString() ?? string.Empty;
        if (ProjectVariantOwnership.IsOriginal(_activeTranslationCode)) return;
        _runtimeUiState = _runtimeUiTexts.SetOverride(_runtimeUiProject, _runtimeUiState, id, text);
        _runtimeUiDirty = true;
        RefreshRuntimeUiGrid();
        RefreshWorkflowStatus();
    }

    private void ResetSelectedRuntimeUiOverride()
    {
        if (_runtimeUiProject is null || _runtimeUiState is null || runtimeUiGrid.CurrentRow?.Tag is not RuntimeUiLogicalRecordId id) return;
        _runtimeUiState = _runtimeUiTexts.RemoveOverride(_runtimeUiProject, _runtimeUiState, id);
        _runtimeUiDirty = true;
        RefreshRuntimeUiGrid();
        RefreshWorkflowStatus();
    }

    private void SaveRuntimeUiText()
    {
        if (_runtimeUiProject is null || _runtimeUiState is null) return;
        RuntimeUiTextSaveResult saved = _runtimeUiTexts.Save(_runtimeUiProject, _activeTranslationCode, _runtimeUiState);
        if (!saved.Succeeded)
        {
            lblRuntimeUiStatus.Text = UiText.Get("RuntimeUi.SaveFailed") + ": " + saved.Detail;
            return;
        }
        _runtimeUiDirty = false;
        RefreshRuntimeUiGrid();
        lblRuntimeUiStatus.Text = UiText.Get("RuntimeUi.SavedProjectOnly");
        SetWorkflowStatus(WorkflowStatusSeverity.Warning, UiText.Get("Workflow.SavedBuildRequired"));
    }

    private void UpdateRuntimeUiActions()
    {
        bool ready = _runtimeUiProject is not null && _runtimeUiState is not null && !ProjectVariantOwnership.IsOriginal(_activeTranslationCode);
        btnReloadRuntimeUi.Enabled = !string.IsNullOrWhiteSpace(txtGameDir.Text);
        btnSaveRuntimeUi.Enabled = ready && _runtimeUiDirty;
        btnResetRuntimeUi.Enabled = ready && runtimeUiGrid.CurrentRow?.Tag is RuntimeUiLogicalRecordId;
    }

    private static string FriendlyRuntimeUiStatus(RuntimeUiLayoutValidationResult result) => result.Status switch
    {
        RuntimeUiLayoutValidationStatus.Valid => "Valid",
        RuntimeUiLayoutValidationStatus.MappingIncomplete => "Mapping incomplete",
        RuntimeUiLayoutValidationStatus.DefaultTextUnavailable => "Original text unavailable",
        RuntimeUiLayoutValidationStatus.EncodingFailure => "Unsupported character",
        RuntimeUiLayoutValidationStatus.UnsupportedGlyph => "Unsupported glyph",
        RuntimeUiLayoutValidationStatus.RecordCapacityExceeded or RuntimeUiLayoutValidationStatus.TextTooLong => "Text too long",
        RuntimeUiLayoutValidationStatus.BankCapacityExceeded => "Bank capacity exceeded",
        RuntimeUiLayoutValidationStatus.InvalidFrozenDescriptor => "Layout unavailable",
        _ => result.Status.ToString()
    };

    private void RefreshTextGrid()
    {
        if (_refreshingTextGrid || _gamePcEntries.Count == 0 || _gamePcOriginal is null)
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
            var cp852 = GamePcTextEditor.GetEncoding("CP852");
            string filter = txtSearch.Text.Trim();

            textGrid.Rows.Clear();

            foreach (var entry in _gamePcEntries)
            {
                GamePcStringEntry originalEntry = _gamePcOriginal.Entries[entry.Index];
                string originalText = GamePcTextEditor.ToEditableText(originalEntry, originalEntry, enc, ActiveGameProfile);
                string value = _gamePcEdits.TryGetValue(entry.Index, out var edited)
                    ? edited
                    : GamePcTextEditor.ToEditableText(entry, originalEntry, enc, ActiveGameProfile);

                if (!string.IsNullOrEmpty(filter) &&
                    value.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) < 0 &&
                    originalText.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) < 0 &&
                    entry.Index.ToString().IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                int bytes = cp852.GetByteCount(value);
                int originalBytes = cp852.GetByteCount(originalText);
                string dosStatus = string.Empty;

                int rowIndex = textGrid.Rows.Add(
                    entry.Index,
                    originalBytes,
                    bytes,
                    bytes - originalBytes,
                    dosStatus,
                    originalText,
                    value);

                var row = textGrid.Rows[rowIndex];
                row.Tag = entry;
                if (_gamePcEdits.ContainsKey(entry.Index))
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
            if (_activeProject is null || _translationProjectState is null) throw new InvalidOperationException("Text project is unavailable.");
            if (ProjectVariantOwnership.IsOriginal(_activeTranslationCode)) throw new InvalidOperationException("The Original project is read-only. Create or select an editable project variant first.");
            _translationProjectState = _translationProjects.UpsertWorkingEdits(_activeProject, _translationProjectState, _activeTranslationCode, _gamePcEdits);
            _translationProjects.Save(_activeProject, _translationProjectState);
            CaptureSavedTextEdits();
            lblTextStatus.Text = string.Format(UiText.Get("TextSaved"), TranslationProjectService.FileName);
            SetWorkflowStatus(WorkflowStatusSeverity.Warning, UiText.Get("Workflow.SavedBuildRequired"));
            LoadGamePcTexts();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, AppInfo.ProductTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblTextStatus.Text = ex.Message;
        }
    }

    private TranslationExchangeDocument CurrentTranslationExchange()
    {
        if (_gamePcOriginal is null || _gamePcEntries.Count == 0) throw new InvalidDataException("Open a translation data file first.");
        TranslationProjectVariant? projectVariant = _translationProjectState?.Variants.SingleOrDefault(item => item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase));
        VariantEntry? variant = projectVariant is null ? null : new VariantEntry(projectVariant.DisplayName, projectVariant.DataFile, true, 1, projectVariant.Code, projectVariant.ExeFile);
        return TranslationExchangeService.Create(ActiveGameProfile, _gamePcOriginal.Path, _gamePcOriginal.Entries, _gamePcEntries, _gamePcEdits, variant, RuntimeForExchange);
    }

    private string RuntimeForExchange(int index, string value)
    {
        return string.Empty;
    }

    private void ExportTranslations()
    {
        try
        {
            TranslationExchangeDocument document = CurrentTranslationExchange();
            string code = string.IsNullOrWhiteSpace(document.Metadata.VariantCode) ? "translation" : document.Metadata.VariantCode;
            using var dialog = new SaveFileDialog { Title = UiText.Get("ExportTranslation"), InitialDirectory = Path.GetDirectoryName(CurrentDataFilePath), FileName = $"{document.Metadata.Game}_{code}_translation.xlsx", Filter = "Excel Workbook (*.xlsx)|*.xlsx|CSV files (*.csv)|*.csv" };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            if (Path.GetExtension(dialog.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase)) TranslationExchangeService.ExportCsv(dialog.FileName, document);
            else TranslationExchangeService.ExportXlsx(dialog.FileName, document);
            lblTextStatus.Text = string.Format(UiText.Get("TranslationExported"), Path.GetFileName(dialog.FileName));
        }
        catch (Exception ex) { ShowVariantError(ex.Message); }
    }

    private void ImportTranslations()
    {
        try
        {
            if (_gamePcOriginal is null || _gamePcEntries.Count == 0) throw new InvalidDataException("Open a translation data file first.");
            if (_gamePcEdits.Count > 0 && MessageBox.Show(this, UiText.Get("TranslationImportDiscard"), UiText.Get("TranslationImport"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            using var dialog = new OpenFileDialog { Title = UiText.Get("ImportTranslation"), InitialDirectory = Path.GetDirectoryName(CurrentDataFilePath), Filter = "Translation files (*.csv;*.xlsx)|*.csv;*.xlsx|CSV files (*.csv)|*.csv|Excel Workbook (*.xlsx)|*.xlsx", CheckFileExists = true };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;
            TranslationExchangeDocument importedDocument = TranslationExchangeService.ReadAndValidate(dialog.FileName, ActiveGameProfile, _gamePcOriginal.Path, _gamePcOriginal.Entries);
            IReadOnlyDictionary<int, string> imported = importedDocument.Rows.ToDictionary(row => row.Index, row => row.Translation);
            TranslationExchangeDocument current = CurrentTranslationExchange();
            int changed = current.Rows.Count(row => !StringComparer.Ordinal.Equals(row.Translation, imported[row.Index]));
            VariantEntry? active = _variantCatalog?.FindByDataFile(Path.GetFileName(CurrentDataFilePath));
            // Variant fields are informational: warn, but deliberately do not switch the active variant.
            if (active is not null && (!importedDocument.Metadata.TranslationFile.Equals(active.DataFile, StringComparison.OrdinalIgnoreCase) ||
                !importedDocument.Metadata.VariantCode.Equals(active.Code, StringComparison.OrdinalIgnoreCase)) &&
                MessageBox.Show(this, UiText.Get("TranslationImportVariantMismatch"), UiText.Get("TranslationImport"), MessageBoxButtons.YesNo, MessageBoxIcon.Information) != DialogResult.Yes) return;
            if (MessageBox.Show(this, string.Format(UiText.Get("TranslationImportConfirm"), changed, current.Rows.Count - changed, active?.DisplayName ?? Path.GetFileName(CurrentDataFilePath)), UiText.Get("TranslationImport"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            var cp852 = GamePcTextEditor.GetEncoding("CP852");
            _gamePcEdits.Clear();
            foreach (GamePcStringEntry entry in _gamePcEntries)
            {
                GamePcStringEntry baseEntry = _gamePcOriginal.Entries[entry.Index];
                if (!StringComparer.Ordinal.Equals(GamePcTextEditor.ToEditableText(entry, baseEntry, cp852, ActiveGameProfile), imported[entry.Index])) _gamePcEdits[entry.Index] = imported[entry.Index];
            }
            RefreshTextGrid(); UpdateTextStatusSummary();
            RefreshWorkflowStatus();
        }
        catch (Exception ex) { ShowVariantError(ex.Message); }
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
        if (Path.GetFileName(path).Equals(GamePcOriginalService.OriginalFileName, StringComparison.OrdinalIgnoreCase))
        {
            ShowVariantError("GAMEPCO is the protected Original source and cannot be opened as the editable Translation file.");
            return;
        }
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
        lblLauncherFile.Text = UiText.Get("LauncherFile");
        btnSelectLauncherFile.Text = UiText.Get("SelectLauncher");
        lblModderName.Text = UiText.Get("ModderName");
        lblDefaultVariant.Text = UiText.Get("DefaultVariant");
        lblVariantManagerTitle.Text = UiText.Get("VariantManager.Title");
        lblVariantManagerHint.Text = UiText.Get("VariantManager.SelectHint");
        btnPreviewLauncher.Text = UiText.Get("PreviewLauncher");
        btnGenerateLauncher.Text = UiText.Get("GenerateLauncher");
        btnRestoreLauncher.Text = UiText.Get("RestoreOriginalLauncher");
        if (variantGrid.Columns.Count == 5)
        {
            variantGrid.Columns["Order"].HeaderText = UiText.Get("VariantOrder");
            variantGrid.Columns["Name"].HeaderText = UiText.Get("VariantName");
            variantGrid.Columns["DataFile"].HeaderText = UiText.Get("VariantDataFile");
            variantGrid.Columns["Status"].HeaderText = UiText.Get("VariantStatus");
            variantGrid.Columns["Enabled"].HeaderText = UiText.Get("VariantEnabled");
        }
        if (variantManagerGrid.Columns.Count == 7)
        {
            variantManagerGrid.Columns["Active"].HeaderText = UiText.Get("VariantManager.Active");
            variantManagerGrid.Columns["Variant"].HeaderText = UiText.Get("VariantManager.Variant");
            variantManagerGrid.Columns["Runtime"].HeaderText = UiText.Get("VariantManager.Runtime");
            variantManagerGrid.Columns["Directory"].HeaderText = UiText.Get("VariantManager.Directory");
            variantManagerGrid.Columns["Ownership"].HeaderText = UiText.Get("VariantManager.Ownership");
            variantManagerGrid.Columns["BuildStatus"].HeaderText = UiText.Get("VariantManager.BuildStatus");
            variantManagerGrid.Columns["Readiness"].HeaderText = UiText.Get("VariantManager.Readiness");
        }
        if (_variantCatalog is not null)
            RefreshVariantGrid(CurrentTranslationDataFileForPresentation());
        else
            UpdateVariantActions();
        RefreshVariantManager();
    }

    private void SaveTextDataFileAs(bool createVariant)
    {
        if (_gamePcEntries.Count == 0) return;
        if (createVariant)
        {
            CreateTranslationVariant();
            return;
        }
        using var dialog = new SaveFileDialog
        {
            Title = UiText.Get(createVariant ? "CreateVariant" : "SaveAsDataFile"),
            InitialDirectory = _activeProject?.ProjectRoot ?? Path.GetDirectoryName(CurrentDataFilePath),
            FileName = Path.GetFileName(CurrentDataFilePath),
            Filter = UiText.Get("DataFileFilter"),
            OverwritePrompt = false
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            if (_activeProject is null) throw new InvalidOperationException("Text project is unavailable.");
            string createdPath = _translationProjects.ExportProjectDataFile(_activeProject, Path.GetFileName(dialog.FileName), _gamePcEdits);
            lblTextStatus.Text = string.Format(UiText.Get("DataFileSavedAs"), Path.GetFileName(createdPath));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, AppInfo.ProductTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblTextStatus.Text = ex.Message;
        }
    }

    private void CreateTranslationVariant()
    {
        if (ActiveGameProfile is not (ElviraGameProfile.Elvira1 or ElviraGameProfile.Elvira2))
        {
            ShowVariantError(UiText.Get("LauncherGameUnknown"));
            return;
        }

        using var dialog = new TranslationVariantDialog(ActiveGameProfile);
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        try
        {
            if (_activeProject is null) throw new InvalidOperationException("Text project is unavailable.");
            TranslationProjectLoadResult loaded = _translationProjects.Load(_activeProject);
            if (!loaded.IsSuccess) throw new InvalidDataException(loaded.Detail ?? "Translation project data is invalid.");
            TranslationProjectVariant created = _translationProjects.Create(_activeProject, loaded.State!, dialog.NameValue, dialog.Code, _gamePcEdits);
            _translationProjectState = _translationProjects.Add(loaded.State!, created);
            _translationProjects.Save(_activeProject, _translationProjectState);
            _activeTranslationCode = created.Code;
            LoadGamePcTexts();
            lblTextStatus.Text = string.Format(UiText.Get("VariantCreated"), created.DisplayName);
        }
        catch (Exception ex)
        {
            ShowVariantError(ex.Message);
            lblTextStatus.Text = ex.Message;
        }
    }

    private void ApplyLanguage()
    {
        Text = AppInfo.ProductTitle;
        tabVga.Text = UiText.Get("Graphics");
        tabText.Text = UiText.Get("TextTab");
        tabFont.Text = UiText.Get("FontEditor");
        tabMods.Text = UiText.Get("ModsLauncherTab");
        tabGameTextDomain.Text = UiText.Get("RuntimeUi.GameTextTab");
        tabRuntimeUiDomain.Text = UiText.Get("RuntimeUi.Title");
        btnReloadRuntimeUi.Text = UiText.Get("RuntimeUi.Reload");
        btnSaveRuntimeUi.Text = UiText.Get("SaveToProject");
        btnResetRuntimeUi.Text = UiText.Get("RuntimeUi.ResetOverride");
        btnRunVariant.Text = UiText.Get("Execution.Run"); btnDebugVariant.Text = UiText.Get("Execution.Debug");
        lblRecoveryTitle.Text = UiText.Get("Recovery.Title");
        btnVerifyPristine.Text = UiText.Get("Recovery.VerifyPristine"); btnRestoreBaselineLauncher.Text = UiText.Get("Recovery.RestoreLauncher");
        btnRebuildOwnedVariant.Text = UiText.Get("Recovery.RebuildVariant"); btnRemoveOwnedVariant.Text = UiText.Get("Recovery.RemoveVariant"); btnReverseAllPreview.Text = UiText.Get("Recovery.ReverseAll");
        btnBrowseGame.Text = UiText.Get("Browse");
        btnFindGames.Text = UiText.Get("FindGames");
        lblActiveProjectCaption.Text = UiText.Get("Edition") + ":";
        btnBuildActiveVariant.Text = UiText.Get("BuildVariant");
        installationToolTip.SetToolTip(cmbInstallations, UiText.Get("InstallationPathTooltip"));
        btnReload.Text = UiText.Get("Refresh");
        btnReplace.Text = UiText.Get("ReplacePng");
        btnExport.Text = UiText.Get("ExportPng");
        btnRestore.Text = UiText.Get("RestoreOriginal");
        btnReloadTexts.Text = UiText.Get("ReloadTexts");
        btnSaveTexts.Text = UiText.Get("SaveToProject");
        btnOpenDataFile.Text = UiText.Get("OpenDataFile");
        btnExportTranslations.Text = UiText.Get("ExportTranslation");
        btnImportTranslations.Text = UiText.Get("ImportTranslation");
        btnSaveAsDataFile.Text = UiText.Get("SaveAsDataFile");
        btnCreateDataVariant.Text = UiText.Get("CreateVariant");
        if (!string.IsNullOrWhiteSpace(CurrentDataFilePath))
            SetCurrentDataFile(CurrentDataFilePath);
        lblSearchCaption.Text = UiText.Get("Search");
        lblEncodingCaption.Text = UiText.Get("Encoding");
        lblTextContextCaption.Text = UiText.Get("TextContext");
        lblTranslationVariantCaption.Text = UiText.Get("Edition") + ":";
        int contextIndex = Math.Max(0, cmbTextContext.SelectedIndex);
        cmbTextContext.Items.Clear();
        cmbTextContext.Items.AddRange(new object[] { UiText.Get("ContextAuto"), UiText.Get("ContextGeneric"), UiText.Get("ContextNpc") });
        cmbTextContext.SelectedIndex = Math.Min(contextIndex, cmbTextContext.Items.Count - 1);
        RefreshTranslationVariantSelector();

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
            textGrid.Columns["OriginalBytes"].HeaderText = UiText.Get("OriginalBytes");
            textGrid.Columns["TranslationBytes"].HeaderText = UiText.Get("TranslationBytes");
            textGrid.Columns["ByteDifference"].HeaderText = UiText.Get("Difference");
            if (textGrid.Columns.Contains("DosLimit"))
            {
                textGrid.Columns["DosLimit"].HeaderText = UiText.Get("DosLimit");
                textGrid.Columns["DosLimit"].HeaderCell.ToolTipText = UiText.Get("RuntimeDiagnosticTooltip");
            }
            textGrid.Columns["OriginalText"].HeaderText = UiText.Get("OriginalText");
            textGrid.Columns["Translation"].HeaderText = UiText.Get("Translation");
        }
        if (runtimeUiGrid.Columns.Contains("LogicalId"))
        {
            runtimeUiGrid.Columns["LogicalId"].HeaderText = UiText.Get("RuntimeUi.Record");
            runtimeUiGrid.Columns["Original"].HeaderText = UiText.Get("RuntimeUi.OriginalText");
            runtimeUiGrid.Columns["Override"].HeaderText = UiText.Get("RuntimeUi.ProjectOverride");
            runtimeUiGrid.Columns["Status"].HeaderText = UiText.Get("RuntimeUi.Validation");
            runtimeUiGrid.Columns["Detail"].HeaderText = UiText.Get("RuntimeUi.Details");
        }
        if (cmbZone.Items.Count > 0)
            lblStatus.Text = string.Format(UiText.Get("ZonesFound"), cmbZone.Items.Count);
        if (_gamePcEntries.Count > 0)
            UpdateTextStatusSummary();
        UpdateTextOverview();
        UpdateRuntimeColumnVisibility();
        ApplyVariantLanguage();
        btnAbout.Text = UiText.Get("About");
        btnHelp.Text = UiText.Get("Help");
        btnSpriteEditor.Text = UiText.Get("Graphics");
        btnClearEdit.Text = UiText.Get("CancelEdit");
        btnSaveGraphicsProject.Text = UiText.Get("SaveToProject");
        lblPaletteCaption.Text = UiText.Get("Palette");
        btnPaletteAdvanced.Text = _paletteAdvancedVisible ? UiText.Get("PaletteBasic") : UiText.Get("PaletteAdvanced");
        chkPixelPerfect.Text = UiText.Get("PixelPerfect");
        lblInstallationCaption.Text = UiText.Get("Installation");
        lblActiveVariantCaption.Text = UiText.Get("ActiveVariant");
        lblDetectedGameCaption.Text = UiText.Get("Detected") + ":";
        lblLanguageCaption.Text = UiText.Get("InterfaceLanguage");
        RefreshGraphicsScopeItems();
        RefreshPreviewZoomItems();
        UpdateGraphicsProjectPresentation();
        RefreshPaletteDisplayLanguage();
        btnTextEditor.Text = UiText.Get("OpenTextEditor");
        btnFontEditor.Text = UiText.Get("FontTab");
        btnModsLauncher.Text = UiText.Get("ModsLauncherTab");
        lblVgaFileCaption.Text = UiText.Get("VgaFile");
        btnReloadPreview.Text = UiText.Get("ReloadPreview");
        RefreshInstallationSelector(selectedPath: txtGameDir.Text);
        if (_activeProject is null)
        {
            SetStatusPresentation(WorkflowStatusSeverity.Info, NeutralInstallationPrompt);
            lblLauncherReadiness.Text = NeutralInstallationPrompt;
            SetRecoverySafetyControls(active: false);
        }
        if (_embeddedFontEditor is not null && !_embeddedFontEditor.IsDisposed)
            _embeddedFontEditor.ApplyLanguage();
        if (_helpViewer is not null && !_helpViewer.IsDisposed)
            _helpViewer.SetLanguage(UiText.Language);
        if (_aboutViewer is not null && !_aboutViewer.IsDisposed)
            _aboutViewer.SetLanguage(UiText.Language);
        UpdateGameProfileDisplay();
        UpdateModeButtons();
        // Reflow the stabilized AutoSize/FlowLayoutPanel rows after caption
        // lengths change (EN/SK/CZ). No semantics change; forces WinForms to
        // recompute wrapping toolbar heights.
        PerformLayout();
    }

    private string NeutralInstallationPrompt => UiText.Get(UiLocalizationKeys.NoGameSelected) + " " + UiText.Get(UiLocalizationKeys.FindGamesOrBrowseFolder);

    private void OnUiLocaleChanged(object? sender, UiLocaleChangedEventArgs e)
    {
        if (IsDisposed) return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => OnUiLocaleChanged(sender, e)));
            return;
        }

        _applyingUiLanguage = true;
        try
        {
            RefreshUiLocaleSelector();
            ApplyLanguage();
        }
        finally { _applyingUiLanguage = false; }
    }

    private void OnUiLocalesChanged(object? sender, EventArgs e)
    {
        if (IsDisposed) return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => OnUiLocalesChanged(sender, e)));
            return;
        }
        _applyingUiLanguage = true;
        try { RefreshUiLocaleSelector(); }
        finally { _applyingUiLanguage = false; }
    }

    private void RefreshUiLocaleSelector()
    {
        string selectedLocaleId = UiText.LocaleId;
        cmbUiLanguage.BeginUpdate();
        try
        {
            cmbUiLanguage.Items.Clear();
            foreach (UiLocaleDescriptor locale in UiText.AvailableLocales)
                cmbUiLanguage.Items.Add(locale);
            cmbUiLanguage.SelectedIndex = Enumerable.Range(0, cmbUiLanguage.Items.Count)
                .FirstOrDefault(index => cmbUiLanguage.Items[index] is UiLocaleDescriptor locale && locale.Id == selectedLocaleId, -1);
        }
        finally { cmbUiLanguage.EndUpdate(); }
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
        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 3,
            ColumnCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 172));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));

        var top = new Panel { Dock = DockStyle.Fill, Padding = new Padding(8) };
        // One structural grid: column 0 is the shared label column, column 1
        // the shared selector column with ONE fixed width (all three
        // ComboBoxes share X, width and right edge without each claiming the
        // whole remainder), column 2 the shared action column (Find games
        // above Build variant, Detected caption below), column 3 the value
        // column, column 4 the right area. A Percent selector column grew to
        // 1300+ px on wide desktops and pushed the right header out of the
        // visible area; the shared width matches the Installation dropdown
        // width (620) so closed and opened selectors look identical, while
        // long paths still truncate inside the box (full path in tooltip).
        // Deterministic absolute rows fit one 32 px action row or one native
        // ComboBox row.
        var headerGrid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 108,
            ColumnCount = 5,
            RowCount = 3,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };
        _headerGrid = headerGrid;
        headerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        // 628 = 620 px shared selector width + 8 px ComboBox side margins,
        // so closed width always equals DropDownWidth.
        headerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 628));
        headerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        headerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        headerGrid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        headerGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        headerGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        headerGrid.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));

        // AutoSize (not a measured Auto column): without it the flow reports
        // a small preferred size, its Auto column starves (~200 px) and the
        // language/help controls clip behind a scrollbar.
        var rightHeader = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, AutoScroll = true, Margin = Padding.Empty, Padding = Padding.Empty };

        lblInstallationCaption.Text = UiText.Get("Installation");
        lblInstallationCaption.AutoSize = true;
        lblInstallationCaption.Anchor = AnchorStyles.Left;
        cmbInstallations.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbInstallations.Dock = DockStyle.Fill;
        cmbInstallations.Margin = new Padding(6, 2, 2, 2);
        cmbInstallations.DropDownWidth = 620;
        cmbInstallations.SelectedIndexChanged += (_, _) => InstallationSelectionChanged();
        installationToolTip.SetToolTip(cmbInstallations, UiText.Get("InstallationPathTooltip"));

        btnFindGames.Text = UiText.Get("FindGames");
        btnFindGames.Width = 108;
        ConfigureCenteredButton(btnFindGames, allowWidthGrowth: true);
        btnFindGames.Margin = new Padding(0, 2, 4, 2);
        btnFindGames.Click += (_, _) => FindGames();

        btnBrowseGame.Text = UiText.Get("Browse");
        btnBrowseGame.Width = 128;
        ConfigureCenteredButton(btnBrowseGame, allowWidthGrowth: true);
        btnBrowseGame.Margin = new Padding(0, 2, 10, 2);
        btnBrowseGame.Click += (_, _) => BrowseGame();

        lblDetectedGameCaption.Text = UiText.Get("Detected") + ":";
        lblDetectedGameCaption.AutoSize = true;
        lblDetectedGameCaption.Anchor = AnchorStyles.Left;
        // Single-line by construction: an AutoSize label never word-wraps,
        // so E1/E2 display names cannot split across two rows. Anchored left
        // keeps it vertically centered in the 36 px row; the Auto value
        // column always allocates its full width.
        lblDetectedGame.AutoSize = true;
        lblDetectedGame.Anchor = AnchorStyles.Left;
        lblDetectedGame.TextAlign = ContentAlignment.MiddleLeft;
        lblDetectedGame.AutoEllipsis = true;
        installationToolTip.SetToolTip(lblDetectedGame, UiText.Get("Detected"));
        lblDetectedGame.Font = new Font(Font, FontStyle.Bold);

        lblActiveVariantCaption.Text = UiText.Get("ActiveVariant");
        lblActiveVariantCaption.AutoSize = true;
        lblActiveVariantCaption.Anchor = AnchorStyles.Left;
        cmbActiveVariant.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbActiveVariant.DisplayMember = nameof(VariantContext.DisplayName);
        cmbActiveVariant.Enabled = false;
        cmbActiveVariant.Dock = DockStyle.Fill;
        cmbActiveVariant.DropDownWidth = 620;
        cmbActiveVariant.Margin = new Padding(6, 2, 2, 2);
        cmbActiveVariant.SelectedIndexChanged += (_, _) =>
        {
            if (_refreshingActiveVariantSelector || cmbActiveVariant.SelectedItem is not VariantContext variant)
                return;
            if (!ReferenceEquals(variant.Project, _activeProject))
                return;
            SetActiveVariant(variant);
        };

        lblActiveProjectCaption.Text = UiText.Get("Edition") + ":";
        lblActiveProjectCaption.AutoSize = true;
        lblActiveProjectCaption.Anchor = AnchorStyles.Left;
        cmbActiveProject.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbActiveProject.Dock = DockStyle.Fill;
        cmbActiveProject.DropDownWidth = 620;
        cmbActiveProject.Enabled = false;
        cmbActiveProject.Margin = new Padding(6, 2, 2, 2);
        cmbActiveProject.SelectedIndexChanged += (_, _) =>
        {
            if (_refreshingActiveProjectSelector || cmbActiveProject.SelectedItem is not TextTranslationSelection selection) return;
            SelectTranslationVariant(selection.Code);
        };

        btnBuildActiveVariant.Text = UiText.Get("BuildVariant");
        btnBuildActiveVariant.Width = 128;
        btnBuildActiveVariant.Enabled = false;
        btnBuildActiveVariant.Margin = new Padding(0, 2, 8, 2);
        ConfigureCenteredButton(btnBuildActiveVariant, allowWidthGrowth: true);
        btnBuildActiveVariant.Click += (_, _) => NavigateToActiveBuildTarget();

        lblLanguageCaption.Text = UiText.Get("InterfaceLanguage");
        lblLanguageCaption.AutoSize = true;
        lblLanguageCaption.Anchor = AnchorStyles.Left;

        cmbUiLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbUiLanguage.DrawMode = DrawMode.OwnerDrawFixed;
        cmbUiLanguage.ItemHeight = 20;
        cmbUiLanguage.DrawItem += DrawCenteredLanguageItem;
        cmbUiLanguage.Width = 132;
        cmbUiLanguage.Margin = new Padding(6, 3, 10, 3);
        RefreshUiLocaleSelector();
        cmbUiLanguage.SelectedIndexChanged += (_, _) =>
        {
            if (_applyingUiLanguage) return;
            if (cmbUiLanguage.SelectedItem is UiLocaleDescriptor locale)
                UiText.SetLocale(locale.Id);
        };

        btnHelp.Text = UiText.Get("Help");
        btnHelp.Width = 90;
        ConfigureCenteredButton(btnHelp, allowWidthGrowth: true);
        btnHelp.Margin = new Padding(0, 2, 4, 2);
        btnHelp.Click += (_, _) => OpenHelp();

        btnAbout.Text = UiText.Get("About");
        btnAbout.Width = 120;
        ConfigureCenteredButton(btnAbout, allowWidthGrowth: true);
        btnAbout.Margin = new Padding(0, 2, 0, 2);
        btnAbout.Click += (_, _) => OpenAbout();

        // One structural grid: shared label column (0), shared selector
        // column (1), shared action column (2), value column (3), right
        // area (4). Rows: installation / active variant / edition.
        headerGrid.Controls.Add(lblInstallationCaption, 0, 0);
        headerGrid.Controls.Add(cmbInstallations, 1, 0);
        headerGrid.Controls.Add(btnFindGames, 2, 0);
        headerGrid.Controls.Add(btnBrowseGame, 3, 0);
        headerGrid.Controls.Add(lblActiveVariantCaption, 0, 1);
        headerGrid.Controls.Add(cmbActiveVariant, 1, 1);
        headerGrid.Controls.Add(btnBuildActiveVariant, 2, 1);
        headerGrid.Controls.Add(lblActiveProjectCaption, 0, 2);
        headerGrid.Controls.Add(cmbActiveProject, 1, 2);
        headerGrid.Controls.Add(lblDetectedGameCaption, 2, 2);
        headerGrid.Controls.Add(lblDetectedGame, 3, 2);
        rightHeader.Controls.AddRange(new Control[] { lblLanguageCaption, cmbUiLanguage, btnHelp, btnAbout });
        headerGrid.Controls.Add(rightHeader, 4, 0);

        // Fixed 48 px height with explicit bottom breathing room: content is
        // top padding (4) + button margin (3) + 32 px button + margin (3) =
        // 42, leaving 6 px of air below the buttons. Buttons keep y=7..39.
        var navigation = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 48,
            Padding = new Padding(0, 4, 0, 6),
            WrapContents = false,
            AutoScroll = true
        };
        _navigationRow = navigation;

        // Permanent mode navigation stays immediately below the global installation header.
        ConfigureModeButton(btnSpriteEditor);
        ConfigureModeButton(btnTextEditor);
        ConfigureModeButton(btnFontEditor);

        // One shared deterministic width for the four mode buttons: caption
        // length must never determine individual width. 140 px fits the
        // longest EN/SK/CS caption ("Mods & Launcher", 104 px bold) plus
        // padding, chrome and breathing room; Y/Height/spacing come from the
        // shared helper and the common flow margins.
        const int modeButtonWidth = 140;
        btnSpriteEditor.Text = UiText.Get("SpriteEditor");
        btnSpriteEditor.AutoSize = false;
        btnSpriteEditor.Width = modeButtonWidth;
        ConfigureCenteredButton(btnSpriteEditor);
        btnSpriteEditor.Click += (_, _) => SwitchMode(tabVga);

        btnTextEditor.Text = UiText.Get("OpenTextEditor");
        btnTextEditor.AutoSize = false;
        btnTextEditor.Width = modeButtonWidth;
        ConfigureCenteredButton(btnTextEditor);
        btnTextEditor.Click += (_, _) =>
        {
            LoadGamePcTexts();
            SwitchMode(tabText);
        };

        btnFontEditor.Text = UiText.Get("FontEditor");
        btnFontEditor.AutoSize = false;
        btnFontEditor.Width = modeButtonWidth;
        ConfigureCenteredButton(btnFontEditor);
        btnFontEditor.Click += (_, _) =>
        {
            EnsureEmbeddedFontEditor();
            BindFontEditorToActiveVariant(showMissingError: true);
            SwitchMode(tabFont);
        };

        ConfigureModeButton(btnModsLauncher);
        btnModsLauncher.Text = UiText.Get("ModsLauncherTab");
        btnModsLauncher.UseMnemonic = false;
        btnModsLauncher.AutoSize = false;
        btnModsLauncher.Width = modeButtonWidth;
        ConfigureCenteredButton(btnModsLauncher);
        btnModsLauncher.Click += (_, _) =>
        {
            OpenModsLauncher();
            SwitchMode(tabMods);
        };

        navigation.Controls.AddRange(new Control[] { btnSpriteEditor, btnTextEditor, btnFontEditor, btnModsLauncher });

        lblVgaFileCaption.Text = UiText.Get("VgaFile");
        ConfigureComboLabel(lblVgaFileCaption, 78);
        cmbZone.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbZone.Width = 160;
        cmbZone.SelectedIndexChanged += (_, _) => LoadSelectedZone();
        btnReload.Text = UiText.Get("Refresh");
        btnReload.Width = 100;
        ConfigureCenteredButton(btnReload);
        btnReload.Click += (_, _) => ScanGameFolder();
        lblPaletteCaption.Text = UiText.Get("Palette");
        ConfigureComboLabel(lblPaletteCaption, 58);
        lblPaletteMode.AutoSize = false;
        lblPaletteMode.Dock = DockStyle.Fill;
        lblPaletteMode.TextAlign = ContentAlignment.MiddleLeft;
        lblPaletteMode.BorderStyle = BorderStyle.Fixed3D;
        cmbPaletteManual.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPaletteManual.DrawMode = DrawMode.OwnerDrawFixed;
        cmbPaletteManual.ItemHeight = 22;
        cmbPaletteManual.DropDownWidth = 330;
        cmbPaletteManual.DrawItem += DrawPaletteChoice;
        cmbPaletteManual.Dock = DockStyle.Fill;
        cmbPaletteManual.Visible = false;
        cmbPaletteManual.SelectedIndexChanged += (_, _) =>
        {
            if (!_updatingPaletteControl && cmbPaletteManual.SelectedItem is PalettePreviewChoice choice)
            {
                ClearStaleGraphicsError();
                ApplyPaletteChoice(choice.BankIndex);
            }
        };
        panelPaletteSelector.Size = new Size(150, 28);
        panelPaletteSelector.Controls.Add(lblPaletteMode);
        panelPaletteSelector.Controls.Add(cmbPaletteManual);
        panelPaletteSwatches.Size = new Size(170, 22);
        panelPaletteSwatches.Paint += DrawEffectivePaletteSwatches;
        panelPaletteSwatches.MouseMove += ShowPaletteSwatchTooltip;
        btnPaletteAdvanced.Text = UiText.Get("PaletteAdvanced");
        btnPaletteAdvanced.Width = 105;
        ConfigureCenteredButton(btnPaletteAdvanced);
        btnPaletteAdvanced.Click += (_, _) => TogglePaletteAdvanced();
        chkPixelPerfect.Text = UiText.Get("PixelPerfect");
        chkPixelPerfect.AutoSize = true;
        chkPixelPerfect.TextAlign = ContentAlignment.MiddleLeft;
        chkPixelPerfect.Checked = true;
        chkPixelPerfect.CheckedChanged += (_, _) =>
        {
            preview.PixelPerfect = chkPixelPerfect.Checked;
            ApplyPreviewZoom();
            preview.Invalidate();
        };
        statusBar.Dock = DockStyle.Fill;
        statusBar.AutoSize = false;
        statusBar.Height = 30;
        statusBar.SizingGrip = false;
        statusBar.Items.Clear();
        lblStatus.Spring = true;
        lblStatus.AutoSize = false;
        lblStatus.AutoToolTip = true;
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        lblStatus.Padding = new Padding(6, 4, 6, 4);
        statusBar.Items.Add(lblStatus);

        top.Controls.Add(navigation);
        top.Controls.Add(headerGrid);
        mainSplit.Dock = DockStyle.Fill;
        mainSplit.Orientation = Orientation.Vertical;
        mainSplit.SplitterWidth = 6;
        mainSplit.Panel1MinSize = 0;
        mainSplit.Panel2MinSize = 0;
        tabVga.Text = UiText.Get("Graphics");
        var graphicsToolbar = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 38,
            Padding = new Padding(8, 0, 8, 0),
            ColumnCount = 11,
            RowCount = 1
        };
        _graphicsToolbar = graphicsToolbar;
        // Keep the edition-scope wording readable at common DPI settings while
        // preserving the compact, single-row graphics toolbar.
        foreach (int width in new[] { 78, 160, 112, 58, 140, 145, 109, 105, 110, 220 })
            graphicsToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width));
        graphicsToolbar.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 185));
        foreach (Control control in new Control[] { lblVgaFileCaption, cmbZone, btnReload, lblPaletteCaption, panelPaletteSelector, panelPaletteSwatches, btnPaletteAdvanced, chkPixelPerfect, lblGraphicsVariant, cmbGraphicsScope, btnSaveGraphicsProject })
            control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbZone.Margin = new Padding(0);
        btnReload.Margin = new Padding(4, 0, 8, 0);
        panelPaletteSelector.Margin = new Padding(0);
        btnPaletteAdvanced.Margin = new Padding(4, 0, 0, 0);
        chkPixelPerfect.Margin = new Padding(8, 0, 0, 0);
        lblGraphicsVariant.Text = "—";
        lblGraphicsVariant.AutoSize = false;
        lblGraphicsVariant.TextAlign = ContentAlignment.MiddleLeft;
        cmbGraphicsScope.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbGraphicsScope.DropDownWidth = 300;
        cmbGraphicsScope.Items.AddRange(new object[] { UiText.Get("Graphics.ScopeEdition"), UiText.Get("Graphics.CurrentRuntimeOnly"), UiText.Get("Graphics.ReadOnlyBaseline") });
        cmbGraphicsScope.SelectedIndex = -1;
        cmbGraphicsScope.SelectedIndexChanged += (_, _) => UpdateGraphicsProjectPresentation();
        btnSaveGraphicsProject.Text = UiText.Get("SaveToProject");
        btnSaveGraphicsProject.Anchor = AnchorStyles.Left;
        btnSaveGraphicsProject.Width = 175;
        btnSaveGraphicsProject.Margin = new Padding(4, 0, 0, 0);
        btnSaveGraphicsProject.Click += (_, _) => SaveGraphicsProjectState();
        ConfigureCenteredButton(btnSaveGraphicsProject);
        graphicsToolbar.Controls.Add(lblVgaFileCaption, 0, 0);
        graphicsToolbar.Controls.Add(cmbZone, 1, 0);
        graphicsToolbar.Controls.Add(btnReload, 2, 0);
        graphicsToolbar.Controls.Add(lblPaletteCaption, 3, 0);
        graphicsToolbar.Controls.Add(panelPaletteSelector, 4, 0);
        graphicsToolbar.Controls.Add(panelPaletteSwatches, 5, 0);
        graphicsToolbar.Controls.Add(btnPaletteAdvanced, 6, 0);
        graphicsToolbar.Controls.Add(chkPixelPerfect, 7, 0);
        graphicsToolbar.Controls.Add(lblGraphicsVariant, 8, 0);
        graphicsToolbar.Controls.Add(cmbGraphicsScope, 9, 0);
        graphicsToolbar.Controls.Add(btnSaveGraphicsProject, 10, 0);
        var vgaHost = new Panel { Dock = DockStyle.Fill, Padding = Padding.Empty };
        vgaHost.Controls.Add(mainSplit);
        vgaHost.Controls.Add(graphicsToolbar);
        tabVga.Controls.Add(vgaHost);

        tabText.Text = UiText.Get("TextTab");
        BuildTextEditorUi();

        tabFont.Text = UiText.Get("FontEditor");
        tabMods.Text = UiText.Get("ModsLauncherTab");
        BuildModsLauncherUi();

        tabs.Dock = DockStyle.Fill;
        // The application-level navigation is the only visible page selector.
        // Keep the TabControl strictly as the page host so it does not duplicate it.
        tabs.Appearance = TabAppearance.FlatButtons;
        tabs.ItemSize = new Size(0, 1);
        tabs.SizeMode = TabSizeMode.Fixed;
        tabs.TabPages.Add(tabVga);
        tabs.TabPages.Add(tabText);
        tabs.TabPages.Add(tabFont);
        tabs.TabPages.Add(tabMods);
        tabs.SelectedIndexChanged += (_, _) =>
        {
            if (tabs.SelectedTab == tabMods) OpenModsLauncher();
            UpdateModeButtons();
        };
        root.Controls.Add(top, 0, 0);
        root.Controls.Add(tabs, 0, 1);
        root.Controls.Add(statusBar, 0, 2);
        Controls.Add(root);

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

        grid.SelectionChanged += (_, _) =>
        {
            if (_refreshingGraphicsGrid) return;
            // A Basic/Automatic Elvira I preview is image-specific. Manual
            // selection remains intact and still has precedence.
            if (_manualPaletteBank is null) UpdatePalettePresentation();
            UpdateGraphicsProjectPresentation();
            UpdateGraphicsActionState();
            ShowSelectedPreview();
        };
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

        mainSplit.Panel1.Padding = new Padding(0, 0, 4, 0);
        mainSplit.Panel1.Controls.Add(grid);

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

        // Stabilized: previously absolute SetBounds (label 42 px wide,
        // combo at x=52, button at x=162) so longer SK/CZ "Zoom" captions
        // overlapped at DPI scaling. Use a wrapping flow with auto-sized label.
        var zoomBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(4)
        };

        lblPreviewZoom.Text = UiText.Get("Zoom");
        lblPreviewZoom.AutoSize = true;
        lblPreviewZoom.TextAlign = ContentAlignment.MiddleLeft;
        lblPreviewZoom.Margin = new Padding(0, 6, 6, 0);

        cmbPreviewZoom.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPreviewZoom.Size = new Size(100, 26);
        cmbPreviewZoom.Margin = new Padding(0, 2, 6, 0);
        cmbPreviewZoom.Anchor = AnchorStyles.Left | AnchorStyles.Right;
        cmbPreviewZoom.Items.AddRange(new object[]
        {
            UiText.Get("Graphics.Fit"), "100%", "200%", "300%", "400%", "600%", "800%"
        });
        cmbPreviewZoom.SelectedIndex = 0;
        cmbPreviewZoom.SelectedIndexChanged += (_, _) => ApplyPreviewZoom();
        btnReloadPreview.Text = UiText.Get("ReloadPreview");
        btnReloadPreview.Size = new Size(90, 26);
        btnReloadPreview.Margin = new Padding(0, 2, 0, 0);
        ConfigureCenteredButton(btnReloadPreview, allowWidthGrowth: true);
        btnReloadPreview.Click += (_, _) => ReloadCurrentPreview();

        previewScroll.Resize += (_, _) => ApplyPreviewZoom();

        zoomBar.Controls.Add(lblPreviewZoom);
        zoomBar.Controls.Add(cmbPreviewZoom);
        zoomBar.Controls.Add(btnReloadPreview);

        lblMeta.Dock = DockStyle.Bottom;
        lblMeta.Height = 48;
        lblMeta.TextAlign = ContentAlignment.MiddleLeft;

        // AutoSize with a 78 px floor: single-row content keeps the exact
        // established 78 px geometry (layout-stress snapshot stays stable),
        // while a wrapped second row at narrow widths grows the panel instead
        // of clipping the buttons' lower border.
        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(0, 84),
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(4)
        };
        _graphicsActionRow = buttons;

        btnReplace.Text = UiText.Get("ReplacePng");
        btnClearEdit.Text = UiText.Get("CancelEdit");
        btnExport.Text = UiText.Get("ExportPng");
        btnRestore.Text = UiText.Get("RestoreOriginal");

        btnReplace.Width = 120;
        btnClearEdit.Width = 100;
        btnExport.Width = 110;
        btnRestore.Width = 130;
        // Unified with the other action buttons: EN widths stay minima so the
        // SK captions ("Exportovať PNG...", "Obnoviť pôvodný obrázok") grow
        // in width instead of truncating; height stays pinned at 32 px.
        foreach (Button button in new[] { btnReplace, btnClearEdit, btnExport, btnRestore })
            ConfigureCenteredButton(button, allowWidthGrowth: true);

        btnReplace.Click += (_, _) => ReplaceSelected();
        btnClearEdit.Click += (_, _) => ClearSelectedEdit();
        btnExport.Click += (_, _) => ExportSelected();
        btnRestore.Click += (_, _) => RestoreOriginal();

        buttons.Controls.AddRange(new Control[] { btnReplace, btnClearEdit, btnExport, btnRestore });

        previewViewport.Dock = DockStyle.Fill;
        previewViewport.Padding = Padding.Empty;
        previewViewport.BackColor = Color.FromArgb(40, 40, 40);
        previewViewport.Controls.Add(previewScroll);

        right.Controls.Add(previewViewport);
        right.Controls.Add(zoomBar);
        right.Controls.Add(lblMeta);
        right.Controls.Add(buttons);
        UpdateGraphicsActionState();
    }

    private static void ConfigureModeButton(Button button)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 1;
        button.UseVisualStyleBackColor = false;
        button.TextAlign = ContentAlignment.MiddleCenter;
    }

    private void SwitchMode(TabPage target)
    {
        tabs.SelectedTab = target;
        // TabControl can retain stale pixels when a borderless embedded Form is
        // activated over a data grid. Force the newly active page to repaint.
        target.PerformLayout();
        target.Invalidate(true);
        target.Update();
        UpdateModeButtons();
    }

    private void UpdateModeButtons()
    {
        SetModeButtonState(btnSpriteEditor, tabs.SelectedTab == tabVga);
        SetModeButtonState(btnTextEditor, tabs.SelectedTab == tabText);
        SetModeButtonState(btnFontEditor, tabs.SelectedTab == tabFont);
        SetModeButtonState(btnModsLauncher, tabs.SelectedTab == tabMods);
    }

    private void SetModeButtonState(Button button, bool active)
    {
        button.BackColor = active ? SystemColors.Highlight : SystemColors.Control;
        button.ForeColor = active ? SystemColors.HighlightText : SystemColors.ControlText;
        button.FlatAppearance.BorderColor = active ? SystemColors.Highlight : SystemColors.ControlDark;
        if (!_modeButtonFonts.TryGetValue(button, out ModeButtonFonts? fonts))
        {
            FontStyle regularStyle = button.Font.Style & ~FontStyle.Bold;
            fonts = new ModeButtonFonts(new Font(button.Font, regularStyle), new Font(button.Font, regularStyle | FontStyle.Bold));
            _modeButtonFonts.Add(button, fonts);
        }
        button.Font = active ? fonts.Bold : fonts.Regular;
    }

    private void DisposePresentationResources()
    {
        foreach (ModeButtonFonts fonts in _modeButtonFonts.Values) fonts.Dispose();
        _modeButtonFonts.Clear();
        _statusRegularFont?.Dispose();
        _statusEmphasisFont?.Dispose();
        _variantMissingFont?.Dispose();
        _statusInfoImage?.Dispose();
        _statusSuccessImage?.Dispose();
        _statusWarningImage?.Dispose();
        _statusErrorImage?.Dispose();
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
        // The root layout gives the global header and the active page separate rows.
        // The embedded editor can therefore occupy the page row without compensating padding.
        var fontHost = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = Padding.Empty,
            BackColor = SystemColors.Control
        };
        fontHost.Controls.Add(_embeddedFontEditor);

        tabFont.Controls.Clear();
        tabFont.Controls.Add(fontHost);
        _embeddedFontEditor.Show();
        BindFontEditorToActiveVariant(showMissingError: false);
        tabFont.Invalidate(true);
    }

    // Legacy VariantEntry metadata describes editable GAMEPC* translation files.
    // It is deliberately not the global runtime selection.
    private VariantEntry? ActiveDataFileVariant =>
        _variantCatalog?.FindByDataFile(Path.GetFileName(_currentDataFilePath ?? string.Empty));

    private VariantEntry? ActiveFontVariantEntry => _activeVariant is null
        ? null
        : new VariantEntry(_activeVariant.DisplayName, _activeVariant.LogicalDataFileName, true, 0,
            _activeVariant.DirectoryKey, _activeVariant.SourceExecutableName);

    private void BindFontEditorToActiveVariant(bool showMissingError)
    {
        if (_embeddedFontEditor is null || _embeddedFontEditor.IsDisposed || _variantCatalog is null || _activeProject is null || _activeVariant is null) return;
        FontBindCount++;
        VariantEntry? variant = ActiveFontVariantEntry;
        if (variant is null) return;
        _embeddedFontEditor.BindVariant(variant, _variantCatalog.InstallationDirectory, showMissingError);
        _embeddedFontEditor.BindProjectVariant(_activeProject, _activeVariant, _activeTranslationCode);
    }

    private bool ConfirmFontVariantTargetChange(VariantEntry? nextVariant)
    {
        if (!RequiresFontVariantDiscardConfirmation(ActiveDataFileVariant, nextVariant, _embeddedFontEditor?.HasPendingEditedGlyphs == true))
            return true;
        return MessageBox.Show(this,
            string.Format(UiText.Get("VariantFontChangeDiscardWarning"), nextVariant!.DisplayName),
            UiText.Get("Warning"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
    }

    internal static bool RequiresFontVariantDiscardConfirmation(VariantEntry? current, VariantEntry? next, bool hasPendingFontEdits) =>
        hasPendingFontEdits && current is not null && next is not null &&
        !current.ExeFile.Equals(next.ExeFile, StringComparison.OrdinalIgnoreCase);

    private void SynchronizeGameContextFromExecutable(string executablePath, ElviraGame game)
    {
        string? directory = Path.GetDirectoryName(Path.GetFullPath(executablePath));
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory)) return;
        if (_applyingInstallation || directory.Equals(txtGameDir.Text, StringComparison.OrdinalIgnoreCase))
        {
            _detectedProfile = game == ElviraGame.Elvira2 ? ElviraGameProfile.Elvira2 : ElviraGameProfile.Elvira1;
            UpdateGameProfileDisplay();
            return;
        }
        if (GameInstallationValidator.TryValidate(directory, InstallationDiscoverySource.Manual, out GameInstallation? installation) && installation is not null)
        {
            AddInstallation(installation);
            ApplyInstallation(installation, persist: true);
            return;
        }
        txtGameDir.Text = directory;
        _currentDataFilePath = null;
        _variantCatalog = null;
        _detectedProfile = game == ElviraGame.Elvira2 ? ElviraGameProfile.Elvira2 : ElviraGameProfile.Elvira1;
        ScanGameFolder();
        LoadGamePcTexts();
        UpdateGameProfileDisplay();
    }

    private ElviraGameProfile ActiveGameProfile => _activeProject?.GameProfile ?? ElviraGameProfile.Unknown;

    private void UpdateGameProfileDisplay()
    {
        if (_activeProject is null)
        {
            lblDetectedGame.Text = UiText.Get(UiLocalizationKeys.NoGameSelected);
            installationToolTip.SetToolTip(lblDetectedGame, lblDetectedGame.Text);
            lblDetectedGame.ForeColor = SystemColors.ControlText;
            UpdateTextOverview();
            UpdateRuntimeColumnVisibility();
            UpdateTextValidation();
            return;
        }
        // Authoritative identity: the validated active ProjectContext. An
        // internal detection result disagreeing with it is a diagnostic
        // invariant issue, never a user override (the manual Game selector
        // was removed).
        ElviraGameProfile profile = _activeProject.GameProfile;
        if (_detectedProfile != ElviraGameProfile.Unknown && _detectedProfile != profile)
            System.Diagnostics.Debug.WriteLine($"Detected profile {_detectedProfile} disagrees with validated project profile {profile}.");
        var info = GameProfileInfo.For(profile);
        lblDetectedGame.Text = info.DisplayName;
        installationToolTip.SetToolTip(lblDetectedGame, lblDetectedGame.Text);
        lblDetectedGame.ForeColor = profile == ElviraGameProfile.Unknown ? Color.DarkOrange : SystemColors.ControlText;
        UpdateTextOverview();
        UpdateRuntimeColumnVisibility();
        UpdateTextValidation();
    }

    private void UpdateRuntimeColumnVisibility()
    {
        if (!textGrid.Columns.Contains("DosLimit")) return;
        textGrid.Columns["DosLimit"].Visible = false;
    }

    private void UpdateTextValidation()
    {
        if (lblTextValidation.IsDisposed) return;
        if (textGrid.CurrentRow?.Tag is not GamePcStringEntry)
        {
            lblTextValidation.Text = string.Empty;
            return;
        }

        string text = textGrid.CurrentRow.Cells["Translation"].Value?.ToString() ?? string.Empty;
        var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
        int bytes = enc.GetByteCount(text);
        lblTextValidation.ForeColor = SystemColors.ControlText;
        lblTextValidation.Text = string.Format(UiText.Get("BytesOnly"), bytes);
        textToolTip.SetToolTip(lblTextValidation, text);
    }

    private string BuildDialogueTooltip(GamePcStringEntry entry, string text)
    {
        var enc = GamePcTextEditor.GetEncoding(cmbTextEncoding.SelectedItem?.ToString() ?? "CP852");
        int bytes = enc.GetByteCount(text);
        return $"{bytes} {UiText.Get("Bytes").ToLowerInvariant()} | {text}";
    }

    private void UpdateTextStatusSummary()
    {
        if (_gamePcEntries.Count == 0) return;
        lblTextStatus.Text = string.Format(UiText.Get("StringsCount"), Path.GetFileName(CurrentDataFilePath), _gamePcEntries.Count);
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
                if (!row.Cells["Translation"].Selected)
                    textGrid.CurrentCell = row.Cells["Translation"];
                break;
            }
        }
    }

    private static string LocalizedProfileNote(ElviraGameProfile profile) => profile switch
    {
        ElviraGameProfile.Elvira1 => UiText.Get("Text.ProfileNote.Elvira1"),
        ElviraGameProfile.Elvira2 => UiText.Get("Text.ProfileNote.Elvira2"),
        _ => string.Empty
    };

    private void BrowseGame()
    {
        using var dlg = new FolderBrowserDialog { SelectedPath = Directory.Exists(txtGameDir.Text) ? txtGameDir.Text : string.Empty };
        if (dlg.ShowDialog(this) != DialogResult.OK) return;
        if (!GameInstallationValidator.TryValidate(dlg.SelectedPath, InstallationDiscoverySource.Manual, out GameInstallation? installation) || installation is null)
        {
            MessageBox.Show(this, UiText.Get("InvalidGameFolder"), UiText.Get("UnsupportedInstallation"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!ConfirmInstallationChange(installation.InstallationPath)) return;
        AddInstallation(installation);
        ApplyInstallation(installation, persist: true);
    }

    private void InitializeInstallations()
    {
        // Discovery is intentionally an explicit user action.  Do not restore
        // the last installation, select a singleton result, or create context.
        _installations.Clear();
        RefreshInstallationSelector();
        ClearInstallationState();
    }

    private void FindGames()
    {
        Cursor? previous = Cursor.Current;
        try
        {
            Cursor.Current = Cursors.WaitCursor;
            IReadOnlyList<GameInstallation> found = InstallationDiscoveryService.DiscoverFast(_installations.Select(item => item.InstallationPath));
            foreach (GameInstallation installation in found) AddInstallation(installation, refresh: false);
            RefreshInstallationSelector();
            PersistInstallations();
            string message = found.Count switch
            {
                0 => UiText.Get("NoSupportedInstallationFound"),
                1 => UiText.Get("OneSupportedInstallationFound"),
                _ => string.Format(UiText.Get("ManySupportedInstallationsFound"), found.Count)
            };
            MessageBox.Show(this, message, UiText.Get("FindGames"), MessageBoxButtons.OK,
                found.Count == 0 ? MessageBoxIcon.Information : MessageBoxIcon.None);
        }
        finally { if (previous is not null) Cursor.Current = previous; }
    }

    private GameInstallation? SelectedInstallation => (cmbInstallations.SelectedItem as InstallationSelectionItem)?.Installation;

    private void AddInstallation(GameInstallation installation, bool refresh = true)
    {
        int existing = _installations.FindIndex(item => item.NormalizedPath.Equals(installation.NormalizedPath, StringComparison.OrdinalIgnoreCase));
        if (existing >= 0) _installations[existing] = installation;
        else _installations.Add(installation);
        if (refresh) RefreshInstallationSelector(txtGameDir.Text);
    }

    private void RefreshInstallationSelector(string? selectedPath = null)
    {
        _refreshingInstallationSelector = true;
        try
        {
            string? selected = string.IsNullOrWhiteSpace(selectedPath) ? null : InstallationPathNormalizer.Normalize(selectedPath);
            cmbInstallations.BeginUpdate();
            cmbInstallations.Items.Clear();
            foreach (GameInstallation installation in _installations.OrderBy(item => item.Game).ThenBy(item => item.InstallationPath, StringComparer.OrdinalIgnoreCase))
                cmbInstallations.Items.Add(new InstallationSelectionItem(installation));
            int selectedIndex = selected is null ? -1 : Enumerable.Range(0, cmbInstallations.Items.Count)
                .FirstOrDefault(index => cmbInstallations.Items[index] is InstallationSelectionItem { Installation: { } installation } &&
                    installation.NormalizedPath.Equals(selected, StringComparison.OrdinalIgnoreCase));
            if (selected is not null && selectedIndex == 0 && (cmbInstallations.Items.Count == 0 || cmbInstallations.Items[0] is not InstallationSelectionItem { Installation: { } first } || !first.NormalizedPath.Equals(selected, StringComparison.OrdinalIgnoreCase)))
                selectedIndex = -1;
            cmbInstallations.SelectedIndex = selectedIndex;
            cmbInstallations.EndUpdate();
            installationToolTip.SetToolTip(cmbInstallations, SelectedInstallation?.InstallationPath ?? UiText.Get("InstallationPathTooltip"));
        }
        finally { _refreshingInstallationSelector = false; }
    }

    private void InstallationSelectionChanged()
    {
        if (_refreshingInstallationSelector) return;
        ApplyInstallationSelection(SelectedInstallation);
    }

    private void ApplyInstallationSelection(GameInstallation? selected)
    {
        if (selected is null)
        {
            if (string.IsNullOrWhiteSpace(txtGameDir.Text)) return;
            if (!ConfirmInstallationChange(null)) { RefreshInstallationSelector(txtGameDir.Text); return; }
            ClearInstallationState();
            PersistInstallations();
            return;
        }
        // The sentinel clears txtGameDir. It is not a filesystem installation, so
        // selecting a real installation afterwards must apply it directly rather
        // than attempting to normalize that deliberately empty UI state.
        if (InstallationSelectionState.IsSameActiveInstallation(selected, txtGameDir.Text)) return;
        if (!ConfirmInstallationChange(selected.InstallationPath)) { RefreshInstallationSelector(txtGameDir.Text); return; }
        ApplyInstallation(selected, persist: true);
    }

    private bool ConfirmInstallationChange(string? destination)
    {
        bool fontEdits = _embeddedFontEditor?.HasPendingEditedGlyphs == true;
        // `_edits` is the currently displayed graphics projection. It includes
        // saved edition edits, so it is not a dirty-state source.
        if (!HasUnsavedTextChanges && !_runtimeUiDirty && !_graphicsProjectDirty && !fontEdits) return true;
        string target = destination ?? UiText.Get("NoSupportedGameSelected");
        return MessageBox.Show(this, string.Format(UiText.Get("InstallationChangeDiscardWarning"), target), UiText.Get("Warning"),
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes;
    }

    private void ApplyInstallation(GameInstallation installation, bool persist)
    {
        ApplyInstallationRequestedCount++;
        if (_applyingInstallation || IsActiveInstallation(installation))
        {
            ApplyInstallationSuppressedCount++;
            return;
        }
        _applyingInstallation = true;
        try
        {
            ApplyInstallationEffectiveCount++;
            if (!_openedProjects.TryGetValue(installation.NormalizedPath, out ProjectContext? project))
            {
                ProjectContextOpenResult opened = new ProjectContextLoader().Open(installation.InstallationPath);
                if (!opened.IsSuccess)
                {
                    RejectInstallationActivation(installation, opened.Detail ?? opened.Status.ToString());
                    return;
                }
                project = opened.Context!;
                _openedProjects.Add(installation.NormalizedPath, project);
            }
            _lastInstallationActivationFailure = null;
            txtGameDir.Text = installation.InstallationPath;
            _currentDataFilePath = null;
            _activeTranslationCode = "EN";
            _variantCatalog = null;
            _activeProject = project;
            _availableActiveVariants = VariantContextCatalog.CreateBuiltIns(_activeProject);
            _activeVariant = _availableActiveVariants.First();
            _modsVariant = _activeVariant;
            RefreshActiveVariantSelector();
            _variantCatalog = VariantConfigurationService.Load(_activeProject.GameRoot, _activeProject.GameProfile);
        SetCurrentDataFile(Path.Combine(_activeProject.GameRoot, _activeVariant.LogicalDataFileName));
        ScanGameFolder();
            LoadGamePcTexts();
            _detectedProfile = installation.Game;
            UpdateGameProfileDisplay();
            BindFontEditorToActiveVariant(showMissingError: false);
            RefreshInstallationSelector(installation.InstallationPath);
            if (persist) PersistInstallations();
            RefreshWorkflowStatus();
        }
        finally { _applyingInstallation = false; }
    }

    /// <summary>
    /// The only runtime-selection transition. It changes presentation/projection
    /// state only; ProjectContext and game/project files remain untouched.
    /// </summary>
    private void SetActiveVariant(VariantContext variant)
    {
        ArgumentNullException.ThrowIfNull(variant);
        if (_activeProject is null || !ReferenceEquals(variant.Project, _activeProject))
            throw new InvalidOperationException("The active variant must belong to the active ProjectContext.");
        if (ReferenceEquals(_activeVariant, variant))
            return;

        _activeVariant = variant;
        RefreshActiveVariantSelector();
        UpdateTextOverview();
        LoadRuntimeUiText();
        if (cmbZone.SelectedIndex >= 0)
            LoadSelectedZone();
        else
            LoadGraphicsProjectState();
        BindFontEditorToActiveVariant(showMissingError: false);
        OpenModsLauncher();
        UpdateGameProfileDisplay();
        RefreshWorkflowStatus();
    }

    private void RefreshActiveVariantSelector()
    {
        _refreshingActiveVariantSelector = true;
        try
        {
            cmbActiveVariant.Items.Clear();
            if (_activeProject is not null)
            {
                foreach (VariantContext variant in _availableActiveVariants)
                    cmbActiveVariant.Items.Add(variant);
                cmbActiveVariant.SelectedItem = _activeVariant;
            }
            cmbActiveVariant.Enabled = _activeVariant is not null;
        }
        finally { _refreshingActiveVariantSelector = false; }
    }

    private void RejectInstallationActivation(GameInstallation installation, string detail)
    {
        ClearInstallationState();
        _lastInstallationActivationFailure = detail;
        RefreshInstallationSelector();
        SetStatus($"Cannot activate {GameProfileInfo.For(installation.Game).DisplayName}: {detail}", true);
    }

    private void ClearInstallationState()
    {
        _lastInstallationActivationFailure = null;
        txtGameDir.Text = string.Empty;
        _currentDataFilePath = null;
        _activeTranslationCode = "EN";
        _translationProjectState = null;
        _gamePcEntries.Clear();
        _gamePcEdits.Clear();
        _savedGamePcEdits.Clear();
        _gamePcOriginal = null;
        _textDiagnosticStore = null;
        _variantCatalog = null;
        _activeProject = null;
        _activeVariant = null;
        _availableActiveVariants = [];
        _textVariant = null;
        _modsVariant = null;
        RefreshActiveVariantSelector();
        _openedProjects.Clear();
        _detectedProfile = ElviraGameProfile.Unknown;
        cmbZone.Items.Clear();
        _currentVga = null;
        _currentData = null;
        _table = null;
        _paletteBanks.Clear();
        _pairedPaletteResource = null;
        _pairedPalettePath = null;
        _manualPaletteBank = null;
        preview.Image?.Dispose();
        preview.Image = null;
        grid.Rows.Clear();
        textGrid.Rows.Clear();
        // A selected catalog row is meaningful only inside its ProjectContext.
        // Clear it before recalculating no-game action state.
        variantGrid.Rows.Clear();
        variantManagerGrid.Rows.Clear();
        RefreshTranslationVariantSelector();
        UpdateTextOverview();
        UpdateTextActionState();
        ClearRuntimeUiText();
        ClearGraphicsProjectState();
        _embeddedFontEditor?.ClearActiveProjectBinding();
        SetModsLauncherControls(active: false);
        UpdateVariantActions();
        _workflowStatus = null;
        btnBuildActiveVariant.Enabled = false;
        SetStatusPresentation(WorkflowStatusSeverity.Info, NeutralInstallationPrompt);
        UpdateGraphicsActionState();
        UpdateGameProfileDisplay();
    }

    private void PersistInstallations()
    {
        _installationSettings.Save(_installations, string.IsNullOrWhiteSpace(txtGameDir.Text) ? null : txtGameDir.Text);
    }

    private void ScanGameFolder()
    {
        try
        {
            ClearStaleGraphicsError();
            string dir = txtGameDir.Text.Trim();
            if (!Directory.Exists(dir))
            {
                SetGraphicsStatus(UiText.Get("Graphics.DirectoryMissing"), true);
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

            SetGraphicsStatus(string.Format(UiText.Get("ZonesFound"), files.Count), false);
            _detectedProfile = GameInstallationValidator.TryValidate(dir, InstallationDiscoverySource.Manual, out GameInstallation? installation) && installation is not null
                ? installation.Game
                : GameProfileDetector.Detect(dir, files.Count, _gamePcEntries.Count > 0 ? _gamePcEntries.Count : null);
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
            ClearStaleGraphicsError();
            GraphicsLoadCount++;
            _edits.Clear();

            _currentVga = Path.Combine(txtGameDir.Text.Trim(), cmbZone.SelectedItem.ToString()!);
            LoadGraphicsProjectState();
            _currentData = File.ReadAllBytes(_currentVga);
            _table = new VgaImageTableParser(_currentData).Parse();
            ApplyGraphicsProjectEditsForCurrentResource();

            LoadZonePalettes();

            _refreshingGraphicsGrid = true;
            grid.SuspendLayout();
            try
            {
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
            }
            finally
            {
                grid.ResumeLayout();
                _refreshingGraphicsGrid = false;
            }
            preview.Image?.Dispose();
            preview.Image = null;
            preview.Size = previewScroll.ClientSize;
            lblMeta.Text = "";
            SetGraphicsStatus($"{Path.GetFileName(_currentVga)} | entries: {_table.Entries.Count} | endian: {_table.Endian}", false);
            UpdateGraphicsProjectPresentation();

            if (grid.Rows.Count > 0)
            {
                _refreshingGraphicsGrid = true;
                try
                {
                    grid.ClearSelection();
                    grid.CurrentCell = grid.Rows[0].Cells[0];
                    grid.Rows[0].Selected = true;
                    grid.FirstDisplayedScrollingRowIndex = 0;
                }
                finally { _refreshingGraphicsGrid = false; }
                // Programmatic selection above is intentionally silent. Refresh
                // each dependent presentation exactly once for the final row.
                if (_manualPaletteBank is null) UpdatePalettePresentation();
                UpdateGraphicsProjectPresentation();
                ShowSelectedPreview();
            }
            UpdateGraphicsActionState();
        }
        catch (Exception ex)
        {
            Error(ex);
            UpdateGraphicsActionState();
        }
    }

    private void LoadGraphicsProjectState()
    {
        if (_activeProject is null || _activeVariant is null) { ClearGraphicsProjectState(); return; }
        if (!_activeProject.GameRoot.Equals(txtGameDir.Text, StringComparison.OrdinalIgnoreCase))
        {
            _graphicsProject = null; _graphicsVariant = null; _graphicsProjectState = null; _graphicsProjectDirty = false;
            return;
        }
        if (_graphicsProject is not null && _graphicsProject.GameRoot.Equals(_activeProject.GameRoot, StringComparison.OrdinalIgnoreCase) &&
            _graphicsProjectCode.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase) && _graphicsProjectState is not null)
        {
            _graphicsVariant = _activeVariant;
            UpdateGraphicsProjectPresentation();
            return;
        }
        _graphicsProject = _activeProject;
        _graphicsVariant = _activeVariant;
        _graphicsProjectCode = _activeTranslationCode;
        GraphicsProjectLoadResult loaded = _graphicsVariants.Load(_graphicsProject, _activeTranslationCode);
        if (!loaded.IsSuccess)
        {
            _graphicsProjectState = null; _graphicsProjectDirty = false;
            SetGraphicsStatus("Graphics project data error: " + (loaded.Detail ?? loaded.Status.ToString()), true);
            return;
        }
        _graphicsProjectState = loaded.State;
        _graphicsProjectDirty = false;
    }

    private VariantContext ResolveGraphicsVariant(ProjectContext project)
    {
        if (ReferenceEquals(project, _activeProject) && _activeVariant is not null)
            return _activeVariant;
        VariantContext[] variants = VariantContextCatalog.CreateBuiltIns(project).ToArray();
        return variants[0];
    }

    private void ApplyGraphicsProjectEditsForCurrentResource()
    {
        if (_graphicsProject is null || _graphicsVariant is null || _graphicsProjectState is null || _currentVga is null) return;
        string resource = Path.GetFileName(_currentVga).ToUpperInvariant();
        foreach (GraphicsVariantProjection projection in _graphicsVariants.GetGraphicsEditsForVariant(_graphicsProject, _graphicsProjectState, _graphicsVariant))
        {
            if (!projection.Edit.Identity.ResourceFileName.Equals(resource, StringComparison.OrdinalIgnoreCase)) continue;
            if (File.Exists(projection.Edit.ReplacementPngPath)) _edits[projection.Edit.Identity.ImageId] = projection.Edit.ReplacementPngPath;
        }
    }

    private void SaveGraphicsProjectState()
    {
        if (_graphicsProject is null || _graphicsProjectState is null) return;
        GraphicsProjectSaveResult saved = _graphicsVariants.Save(_graphicsProject, _activeTranslationCode, _graphicsProjectState);
        if (!saved.Succeeded) { SetGraphicsStatus("Graphics project save failed: " + saved.Detail, true); return; }
        _graphicsProjectDirty = false;
        UpdateGraphicsProjectPresentation();
        SetWorkflowStatus(WorkflowStatusSeverity.Warning, UiText.Get("Workflow.SavedBuildRequired"));
    }

    private void UpdateGraphicsProjectPresentation()
    {
        bool bound = _graphicsProject is not null && _graphicsVariant is not null && _graphicsProjectState is not null;
        bool readOnlyOriginal = bound && ProjectVariantOwnership.IsOriginal(_activeTranslationCode);
        bool ready = bound && !readOnlyOriginal;
        btnSaveGraphicsProject.Enabled = ready && _graphicsProjectDirty;
        cmbGraphicsScope.Enabled = ready;
        if (readOnlyOriginal)
        {
            // The scope selector describes an editable-project choice. Original
            // is immutable, so show an explicit neutral baseline state instead.
            if (cmbGraphicsScope.SelectedIndex != 2) cmbGraphicsScope.SelectedIndex = 2;
            lblGraphicsVariant.Text = _graphicsVariant!.DisplayName + " — " + UiText.Get("OriginalReadOnly");
            return;
        }
        if (!ready)
        {
            // "Shared" is an edit scope, not a neutral/no-project value.
            // Keep the selector visibly unselected until a graphics project is active.
            if (cmbGraphicsScope.SelectedIndex >= 0) cmbGraphicsScope.SelectedIndex = -1;
            lblGraphicsVariant.Text = UiText.Get("Graphics.ProjectUnavailable") + ": —";
            return;
        }
        if (cmbGraphicsScope.SelectedIndex is < 0 or 2)
        {
            cmbGraphicsScope.SelectedIndex = 0;
            return;
        }
        VgaImageEntry? selected = SelectedEntry();
        GraphicsProjectEdit? edit = selected is null || _currentVga is null ? null : _graphicsProjectState!.Edits
            .SingleOrDefault(value => value.Identity.ResourceFileName.Equals(Path.GetFileName(_currentVga), StringComparison.OrdinalIgnoreCase) && value.Identity.ImageId == selected.ImageId);
        if (edit is null)
        {
            lblGraphicsVariant.Text = _graphicsVariant!.DisplayName + " — " +
                (_graphicsProjectDirty ? UiText.Get("Graphics.UnsavedProjectEdits") : UiText.Get("Graphics.NewEdit")) + " " +
                (cmbGraphicsScope.SelectedIndex == 1 ? UiText.Get("Graphics.CurrentRuntimeOnly") : UiText.Get("Graphics.ScopeEdition"));
            return;
        }
        GraphicsApplicabilityStatus status = _graphicsVariants.GetApplicability(_graphicsProject!.GameProfile, edit, _graphicsVariant!.RuntimeKind);
        string scope = status switch
        {
            GraphicsApplicabilityStatus.Shared => UiText.Get("Graphics.ScopeEdition"),
            GraphicsApplicabilityStatus.RuntimeSpecific => UiText.Get("Graphics.CurrentRuntimeOnly"),
            _ => UiText.Get("Graphics.UnsupportedCurrentRuntime")
        };
        lblGraphicsVariant.Text = _graphicsVariant.DisplayName + " — " + scope + (_graphicsProjectDirty ? " " + UiText.Get("Graphics.Unsaved") : string.Empty);
    }

    private void ClearGraphicsProjectState()
    {
        _graphicsProject = null; _graphicsVariant = null; _graphicsProjectState = null; _graphicsProjectCode = ProjectVariantOwnership.OriginalCode; _graphicsProjectDirty = false;
        _edits.Clear();
        _graphicsStatusIsError = false;
        UpdateGraphicsProjectPresentation();
    }

    /// <summary>Presentation only. A control is enabled only when the active
    /// installation has supplied a concrete VGA image on which it can act.</summary>
    private void UpdateGraphicsActionState()
    {
        bool installationReady = _activeProject is not null;
        bool imageReady = installationReady && _currentVga is not null && _table is not null;
        bool selected = imageReady && SelectedEntry() is not null;
        bool editableProject = !ProjectVariantOwnership.IsOriginal(_activeTranslationCode);
        cmbZone.Enabled = installationReady && cmbZone.Items.Count > 0;
        btnReload.Enabled = installationReady;
        btnReloadPreview.Enabled = selected;
        btnReplace.Enabled = selected && editableProject;
        btnExport.Enabled = selected;
        btnRestore.Enabled = selected && _currentVga is not null && File.Exists(SafeDeployer.OriginalBackupPath(_currentVga));
        btnClearEdit.Enabled = selected && editableProject && _edits.ContainsKey(SelectedEntry()!.ImageId);
        if (!imageReady)
        {
            lblPaletteMode.Text = "—";
            lblPaletteMode.Visible = true;
            cmbPaletteManual.Visible = false;
            cmbPaletteManual.Enabled = false;
            btnPaletteAdvanced.Enabled = false;
            panelPaletteSwatches.Enabled = false;
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
        if (_applyingPreviewZoom) return;
        _applyingPreviewZoom = true;
        try
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

        int zoomIndex = Math.Max(0, cmbPreviewZoom.SelectedIndex);

        if (zoomIndex == 0)
        {
            // Manual Fit centering from the live viewport client area and the
            // image size only. Dock=Fill is deliberately not used: the
            // placement must not depend on stale AutoScrollPosition, previous
            // preview Location/Size, or previous zoom/edition state, so every
            // input here is freshly read and the result is idempotent.
            previewScroll.AutoScroll = false;
            previewScroll.AutoScrollMinSize = Size.Empty;
            preview.Dock = DockStyle.None;
            preview.SizeMode = PictureBoxSizeMode.Zoom;
            (Size fitSize, Point fitLocation) = ComputeFitPlacement(previewScroll.ClientSize, preview.Image.Size);
            preview.Size = fitSize;
            preview.Location = fitLocation;
            previewScroll.AutoScrollPosition = Point.Empty;
            return;
        }

        previewScroll.AutoScroll = true;
        preview.Dock = DockStyle.None;
        preview.SizeMode = PictureBoxSizeMode.StretchImage;

        int percent = zoomIndex switch
        {
            1 => 100,
            2 => 200,
            3 => 300,
            4 => 400,
            5 => 600,
            6 => 800,
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
        finally { _applyingPreviewZoom = false; }
    }

    // Pure Fit geometry used by ApplyPreviewZoom and by the regression
    // smoke. Same scale rounding as the PixelPerfectPictureBox paint path
    // (ZoomRectangle), so the placed control matches what is painted.
    // Depends only on the live viewport client area and image size:
    // idempotent by construction. Free-space balance is exact to 1 px.
    internal static (Size ScaledSize, Point Location) ComputeFitPlacement(Size viewportClientSize, Size imageSize)
    {
        if (viewportClientSize.Width <= 0 || viewportClientSize.Height <= 0 ||
            imageSize.Width <= 0 || imageSize.Height <= 0)
            return (Size.Empty, Point.Empty);
        float scale = Math.Min((float)viewportClientSize.Width / imageSize.Width, (float)viewportClientSize.Height / imageSize.Height);
        int width = Math.Max(1, (int)Math.Round(imageSize.Width * scale));
        int height = Math.Max(1, (int)Math.Round(imageSize.Height * scale));
        return (new Size(width, height),
            new Point((viewportClientSize.Width - width) / 2, (viewportClientSize.Height - height) / 2));
    }

    private void RefreshGraphicsScopeItems()
    {
        int selected = cmbGraphicsScope.SelectedIndex;
        cmbGraphicsScope.BeginUpdate();
        try
        {
            cmbGraphicsScope.Items.Clear();
            cmbGraphicsScope.Items.AddRange(new object[]
            {
                UiText.Get("Graphics.ScopeEdition"),
                UiText.Get("Graphics.CurrentRuntimeOnly"),
                UiText.Get("Graphics.ReadOnlyBaseline")
            });
            cmbGraphicsScope.SelectedIndex = selected < 0 ? -1 : Math.Min(selected, cmbGraphicsScope.Items.Count - 1);
        }
        finally { cmbGraphicsScope.EndUpdate(); }
    }

    private void RefreshPreviewZoomItems()
    {
        int selected = Math.Max(0, cmbPreviewZoom.SelectedIndex);
        cmbPreviewZoom.BeginUpdate();
        try
        {
            cmbPreviewZoom.Items.Clear();
            cmbPreviewZoom.Items.AddRange(new object[] { UiText.Get("Graphics.Fit"), "100%", "200%", "300%", "400%", "600%", "800%" });
            cmbPreviewZoom.SelectedIndex = Math.Min(selected, cmbPreviewZoom.Items.Count - 1);
        }
        finally { cmbPreviewZoom.EndUpdate(); }
    }


    private void RefreshPaletteDisplayLanguage()
    {
        UpdatePalettePresentation();
    }

    private void LoadZonePalettes()
    {
        string? previousPalettePath = _pairedPalettePath;
        int? previousManualBank = _manualPaletteBank;
        _paletteBanks.Clear();
        _pairedPaletteResource = null;
        _pairedPalettePath = null;

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
                        _pairedPaletteResource = vga1Name;
                        _pairedPalettePath = vga1Path;
                    }
                    catch (Exception ex)
                    {
                        SetGraphicsStatus(string.Format(UiText.Get("PaletteLoadError"), ex.Message), true);
                    }
                }
            }
        }

        // The only proven automatic behavior is the paired xNN1.VGA group
        // with its default bank 0. It is deliberately not presented as an
        // exact original-runtime palette-bank detection.
        bool samePaletteGroup = previousPalettePath is not null
            && _pairedPalettePath is not null
            && string.Equals(previousPalettePath, _pairedPalettePath, StringComparison.OrdinalIgnoreCase);
        ApplyPaletteChoice(samePaletteGroup ? previousManualBank : null, refreshPreview: false);
    }

    private void ApplyPaletteChoice(int? manualBank, bool refreshPreview = true)
    {
        _manualPaletteBank = manualBank.HasValue && manualBank.Value >= 0 && manualBank.Value < _paletteBanks.Count
            ? manualBank
            : null;

        UpdatePalettePresentation();
        if (refreshPreview)
            ShowSelectedPreview();
    }

    private void UpdatePalettePresentation()
    {
        if (_activeProject is null || _currentVga is null)
        {
            lblPaletteMode.Text = "—";
            lblPaletteMode.Visible = true;
            cmbPaletteManual.Visible = false;
            cmbPaletteManual.Enabled = false;
            btnPaletteAdvanced.Enabled = false;
            panelPaletteSwatches.Enabled = false;
            panelPaletteSwatches.Invalidate();
            return;
        }
        bool hasPairedBanks = _paletteBanks.Count > 0;
        panelPaletteSwatches.Enabled = hasPairedBanks;
        string effectiveLabel = PaletteDisplayLabel();
        lblPaletteMode.Text = effectiveLabel;
        lblPaletteMode.Visible = !_paletteAdvancedVisible;
        cmbPaletteManual.Visible = _paletteAdvancedVisible;
        btnPaletteAdvanced.Enabled = hasPairedBanks;
        btnPaletteAdvanced.Text = _paletteAdvancedVisible ? UiText.Get("PaletteBasic") : UiText.Get("PaletteAdvanced");

        _updatingPaletteControl = true;
        try
        {
            cmbPaletteManual.Items.Clear();
            cmbPaletteManual.Items.Add(new PalettePreviewChoice(null, PaletteAutomaticLabel()));
            foreach (ElviraPaletteBank paletteBank in _paletteBanks)
                cmbPaletteManual.Items.Add(new PalettePreviewChoice(paletteBank.Index, paletteBank.ToString()));
            cmbPaletteManual.SelectedIndex = _manualPaletteBank is int selected && selected >= 0 && selected < _paletteBanks.Count
                ? selected + 1
                : 0;
            cmbPaletteManual.Enabled = hasPairedBanks;
        }
        finally
        {
            _updatingPaletteControl = false;
        }

        string paired = _pairedPaletteResource ?? UiText.Get("PaletteUnavailable");
        PaletteResolution automatic = AutomaticPaletteResolution();
        string tip = automatic.Kind == PaletteResolutionKind.Unique
            ? string.Format(UiText.Get("PaletteAutomaticUniqueTooltip"), paired, automatic.PaletteId!.Value.ToString("D3"))
            : string.Format(UiText.Get("PaletteAutomaticTooltip"), paired);
        paletteToolTip.SetToolTip(lblPaletteMode, tip);
        paletteToolTip.SetToolTip(cmbPaletteManual, tip);
        paletteToolTip.SetToolTip(btnPaletteAdvanced, tip);
        panelPaletteSwatches.Invalidate();
    }

    private void DrawPaletteChoice(object? sender, DrawItemEventArgs e)
    {
        if (e.Index < 0 || e.Index >= cmbPaletteManual.Items.Count) return;
        e.DrawBackground();
        if (cmbPaletteManual.Items[e.Index] is not PalettePreviewChoice choice) return;
        Color[] colors = choice.BankIndex is int bank && bank >= 0 && bank < _paletteBanks.Count ? _paletteBanks[bank].Colors : EffectivePalette();
        TextRenderer.DrawText(e.Graphics, choice.Display, e.Font, new Rectangle(e.Bounds.Left + 3, e.Bounds.Top, 112, e.Bounds.Height), e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        DrawPaletteSwatches(e.Graphics, colors, new Rectangle(e.Bounds.Left + 118, e.Bounds.Top + 3, e.Bounds.Width - 122, e.Bounds.Height - 6));
        e.DrawFocusRectangle();
    }

    private void DrawEffectivePaletteSwatches(object? sender, PaintEventArgs e)
    {
        if (!panelPaletteSwatches.Enabled)
        {
            e.Graphics.Clear(SystemColors.Control);
            ControlPaint.DrawBorder(e.Graphics, panelPaletteSwatches.ClientRectangle, SystemColors.ControlDark, ButtonBorderStyle.Solid);
            return;
        }
        DrawPaletteSwatches(e.Graphics, EffectivePalette(), panelPaletteSwatches.ClientRectangle);
    }

    private static void DrawPaletteSwatches(Graphics graphics, IReadOnlyList<Color> colors, Rectangle bounds)
    {
        if (colors.Count == 0 || bounds.Width <= 0 || bounds.Height <= 0) return;
        using var border = new Pen(SystemColors.ControlDark);
        for (int i = 0; i < colors.Count; i++)
        {
            int left = bounds.Left + i * bounds.Width / colors.Count;
            int right = bounds.Left + (i + 1) * bounds.Width / colors.Count;
            Rectangle swatch = Rectangle.FromLTRB(left, bounds.Top, Math.Max(left + 1, right), bounds.Bottom);
            using var brush = new SolidBrush(colors[i]); graphics.FillRectangle(brush, swatch); graphics.DrawRectangle(border, swatch.X, swatch.Y, swatch.Width - 1, swatch.Height - 1);
        }
    }

    private void ShowPaletteSwatchTooltip(object? sender, MouseEventArgs e)
    {
        if (!panelPaletteSwatches.Enabled) return;
        Color[] colors = EffectivePalette();
        if (colors.Length == 0 || panelPaletteSwatches.ClientSize.Width <= 0) return;
        int index = Math.Clamp(e.X * colors.Length / panelPaletteSwatches.ClientSize.Width, 0, colors.Length - 1); Color color = colors[index];
        paletteToolTip.SetToolTip(panelPaletteSwatches,
            string.Format(UiText.Get("Palette.SwatchTooltip"), index, color.R, color.G, color.B, color.R, color.G, color.B));
    }

    private void TogglePaletteAdvanced()
    {
        if (_paletteAdvancedVisible)
        {
            // Basic is a semantic reset: return the current resource to its
            // proven automatic palette choice.
            _paletteAdvancedVisible = false;
            ApplyPaletteChoice(null);
            return;
        }

        _paletteAdvancedVisible = true;
        UpdatePalettePresentation();
    }

    private PaletteResolution AutomaticPaletteResolution()
    {
        VgaImageEntry? entry = SelectedEntry();
        if (entry is null || _pairedPalettePath is null || _currentVga is null || _paletteBanks.Count == 0)
            return PaletteResolution.Unresolved();
        return ActiveGameProfile switch
        {
            ElviraGameProfile.Elvira1 => _elvira1PaletteResolver.Resolve(_pairedPalettePath, _currentVga, entry.ImageId),
            ElviraGameProfile.Elvira2 => _elvira2PaletteResolver.Resolve(_pairedPalettePath, _currentVga, entry.ImageId),
            _ => PaletteResolution.Unresolved()
        };
    }

    private int EffectivePaletteBank
        => Elvira1PaletteResolver.EffectivePaletteBank(_manualPaletteBank, AutomaticPaletteResolution(), _paletteBanks.Count);

    private Color[] EffectivePalette()
        => _paletteBanks.Count == 0
            ? ElviraPaletteLoader.DiagnosticPalette()
            : _paletteBanks[EffectivePaletteBank].Colors;

    private bool TryGetActiveReplacementPalette(out Color[] palette, out string identity)
    {
        if (_paletteBanks.Count == 0)
        {
            palette = Array.Empty<Color>();
            identity = UiText.Get("PaletteUnavailable");
            return false;
        }

        int bank = EffectivePaletteBank;
        palette = _paletteBanks[bank].Colors;
        identity = $"{_pairedPaletteResource ?? UiText.Get("PaletteUnavailable")} / {UiText.Get("PaletteWord")} {bank:D3}";
        return true;
    }

    private string PaletteAutomaticLabel()
    {
        PaletteResolution automatic = AutomaticPaletteResolution();
        int bank = Elvira1PaletteResolver.EffectivePaletteBank(null, automatic, _paletteBanks.Count);
        return automatic.Kind == PaletteResolutionKind.Unique
            ? string.Format(UiText.Get("PaletteAutomaticResolved"), bank)
            : string.Format(UiText.Get("PaletteAutomaticFallback"), bank);
    }

    private string PaletteDisplayLabel()
        => _manualPaletteBank is int bank && bank >= 0 && bank < _paletteBanks.Count
            ? _paletteBanks[bank].ToString()
            : PaletteAutomaticLabel();

    private VgaImageEntry? SelectedEntry()
        => grid.CurrentRow?.Tag as VgaImageEntry;

    private void ShowSelectedPreview()
    {
        PreviewRenderCount++;
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
                preview.Image = PaletteTools.ToBitmap(e.PixelWidth, e.Height, px, EffectivePalette(), transparentZero: true);
                ApplyPreviewZoom();
            }

            lblMeta.Text = string.Format(UiText.Get("GraphicsMeta"), e.ImageId, e.DataOffset, e.Compressed ? "RLE" : "RAW", e.PixelWidth, e.Height, e.HeaderFlags,
                _edits.ContainsKey(e.ImageId) ? UiText.Get("GraphicsEdited") : string.Empty, PaletteDisplayLabel());
        }
        catch (Exception ex)
        {
            preview.Image?.Dispose();
            preview.Image = null;
            lblMeta.Text = string.Format(UiText.Get("GraphicsPreviewError"), e.ImageId, ex.Message);
        }
    }

    private void ReplaceSelected()
    {
        var e = SelectedEntry();
        if (e is null) return;

        using var dlg = new OpenFileDialog
        {
            Filter = $"{UiText.Get("PngImage")}|*.png",
            Title = string.Format(UiText.Get("ReplacePngTitle"), e.ImageId)
        };

        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            bool hasPalette = TryGetActiveReplacementPalette(out Color[] palette, out string paletteIdentity);
            ReplacePngValidationResult validation = ReplacePngValidator.Validate(
                dlg.FileName, e.PixelWidth, e.Height, paletteIdentity, hasPalette ? palette : null);
            if (!validation.IsValid)
            {
                ShowReplacePngValidation(validation);
                return;
            }

            // Commit only after every pixel has passed strict exact-palette validation.
            if (!ReplacePngValidator.TryCommitValidatedReplacement(_edits, e.ImageId, dlg.FileName, validation))
                return;
            TrackGraphicsProjectEdit(e.ImageId, dlg.FileName);
            RefreshEditMarker(e.ImageId);
            ShowSelectedPreview();
            SetGraphicsStatus(string.Format(UiText.Get("ReplacementLoaded"), e.ImageId), false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            SetGraphicsStatus(UiText.Get("Graphics.ReplaceUnexpected"), true);
            MessageBox.Show(this, UiText.Get("Graphics.ReplaceUnexpected"), UiText.Get("Graphics.ReplaceValidationTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void ShowReplacePngValidation(ReplacePngValidationResult validation)
    {
        string message = validation.FailureKind switch
        {
            ReplacePngValidationFailureKind.PaletteMismatch => UiText.Get("Graphics.PngPaletteInvalid"),
            ReplacePngValidationFailureKind.DimensionMismatch => string.Format(UiText.Get("Graphics.PngDimensionsMismatch"),
                validation.ExpectedWidth, validation.ExpectedHeight, validation.ActualWidth ?? 0, validation.ActualHeight ?? 0),
            ReplacePngValidationFailureKind.PaletteUnavailable => UiText.Get("Graphics.PaletteUnavailable"),
            _ => UiText.Get("Graphics.UnsupportedPng")
        };
        SetGraphicsStatus(message, true);

        if (validation.FailureKind == ReplacePngValidationFailureKind.PaletteMismatch)
        {
            using var dialog = new PaletteValidationDialog(validation);
            dialog.ShowDialog(this);
            return;
        }

        MessageBox.Show(this, message, UiText.Get("Graphics.ReplaceValidationTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void ClearSelectedEdit()
    {
        var e = SelectedEntry();
        if (e is null) return;
        _edits.Remove(e.ImageId);
        RemoveGraphicsProjectEdit(e.ImageId);
        RefreshEditMarker(e.ImageId);
        UpdateGraphicsActionState();
        ShowSelectedPreview();
    }

    private void TrackGraphicsProjectEdit(int imageId, string pngPath)
    {
        if (ProjectVariantOwnership.IsOriginal(_activeTranslationCode)) return;
        if (_graphicsProject is null || _graphicsVariant is null || _graphicsProjectState is null || _currentVga is null) return;
        GraphicsEditScope scope = cmbGraphicsScope.SelectedIndex == 1 ? GraphicsEditScope.RuntimeSpecific : GraphicsEditScope.Shared;
        var edit = new GraphicsProjectEdit(new GraphicsProjectIdentity(Path.GetFileName(_currentVga), imageId), pngPath, scope,
            scope == GraphicsEditScope.RuntimeSpecific ? _graphicsVariant.RuntimeKind : null);
        _graphicsProjectState = _graphicsVariants.SetEdit(_graphicsProject, _graphicsProjectState, edit);
        _graphicsProjectDirty = true;
        UpdateGraphicsProjectPresentation();
        RefreshWorkflowStatus();
    }

    private void RemoveGraphicsProjectEdit(int imageId)
    {
        if (ProjectVariantOwnership.IsOriginal(_activeTranslationCode)) return;
        if (_graphicsProject is null || _graphicsProjectState is null || _currentVga is null) return;
        _graphicsProjectState = _graphicsVariants.RemoveEdit(_graphicsProject, _graphicsProjectState, new GraphicsProjectIdentity(Path.GetFileName(_currentVga), imageId));
        _graphicsProjectDirty = true;
        UpdateGraphicsProjectPresentation();
        RefreshWorkflowStatus();
    }

    private void RefreshEditMarker(int id)
    {
        foreach (DataGridViewRow row in grid.Rows)
        {
            if (row.Tag is VgaImageEntry e && e.ImageId == id)
            {
                row.Cells["Edit"].Value = _edits.ContainsKey(id) ? UiText.Get("Common.Yes") : "";
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
                : PaletteTools.ToBitmap(e.PixelWidth, e.Height, ElviraImageDecoder.Decode(_currentData, e), EffectivePalette(), transparentZero: true);

            bmp.Save(dlg.FileName, ImageFormat.Png);
            SetGraphicsStatus(string.Format(UiText.Get("Graphics.Exported"), dlg.FileName), false);
        }
        catch (Exception ex)
        {
            Error(ex);
        }
    }

    private bool ValidatePendingGraphicsReplacements()
    {
        if (_table is null) return false;
        bool hasPalette = TryGetActiveReplacementPalette(out Color[] palette, out string paletteIdentity);
        foreach ((int imageId, string path) in _edits.OrderBy(item => item.Key))
        {
            VgaImageEntry? entry = _table.Entries.SingleOrDefault(item => item.ImageId == imageId);
            if (entry is null)
            {
                SetGraphicsStatus(UiText.Get("Graphics.ReplaceUnexpected"), true);
                MessageBox.Show(this, UiText.Get("Graphics.ReplaceUnexpected"), UiText.Get("Graphics.ReplaceValidationTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            ReplacePngValidationResult validation = ReplacePngValidator.Validate(path, entry.PixelWidth, entry.Height,
                paletteIdentity, hasPalette ? palette : null);
            if (validation.IsValid) continue;
            ShowReplacePngValidation(validation);
            return false;
        }
        return true;
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

    private void NavigateToActiveBuildTarget()
    {
        if (_activeProject is null || _activeVariant is null) return;
        OpenModsLauncher();
        SwitchMode(tabMods);
        if (btnRebuildOwnedVariant.Enabled) btnRebuildOwnedVariant.Focus();
        SetWorkflowStatus(WorkflowStatusSeverity.Info, string.Format(UiText.Get("Workflow.BuildTarget"), _activeVariant.DisplayName, ActiveEditionDisplayName()));
    }

    private string ActiveEditionDisplayName()
    {
        if (_activeTranslationCode.Equals(ProjectVariantOwnership.OriginalCode, StringComparison.OrdinalIgnoreCase))
            return UiText.Get("Original") + " (EN)";
        TranslationProjectVariant? edition = _translationProjectState?.Variants
            .FirstOrDefault(item => item.Code.Equals(_activeTranslationCode, StringComparison.OrdinalIgnoreCase));
        return edition is null ? _activeTranslationCode : edition.DisplayName + " (" + _activeTranslationCode + ")";
    }

    private void RefreshWorkflowStatus()
    {
        WorkflowRefreshCount++;
        if (_activeProject is null || _activeVariant is null)
        {
            _workflowStatus = null;
            SetStatusPresentation(WorkflowStatusSeverity.Info, NeutralInstallationPrompt);
            return;
        }

        btnBuildActiveVariant.Enabled = true;
        if (ProjectVariantOwnership.IsOriginal(_activeTranslationCode))
        {
            SetWorkflowStatus(WorkflowStatusSeverity.Info, UiText.Get("Workflow.OriginalReadOnly"));
            return;
        }
        if (HasUnsavedTextChanges || _graphicsProjectDirty || _runtimeUiDirty)
        {
            SetWorkflowStatus(WorkflowStatusSeverity.Warning, UiText.Get("Workflow.UnsavedChanges"));
            return;
        }

        VariantBuildStatusProjection status = _variantBuildStatus.Inspect(_activeProject, _activeVariant);
        if (status.Status == VariantBuildStatus.Ready)
            SetWorkflowStatus(WorkflowStatusSeverity.Success, UiText.Get("Workflow.BuiltReady"));
        else if (status.Status is VariantBuildStatus.Incomplete or VariantBuildStatus.Invalid)
            SetWorkflowStatus(WorkflowStatusSeverity.Error, UiText.Get("Workflow.BuildIncomplete"));
        else
            SetWorkflowStatus(WorkflowStatusSeverity.Warning, UiText.Get("Workflow.SavedBuildRequired"));
    }

    private void SetWorkflowStatus(WorkflowStatusSeverity severity, string text)
    {
        _workflowStatus = new WorkflowStatus(severity, text, _activeVariant?.DirectoryKey, _activeTranslationCode);
        SetStatusPresentation(severity, text);
    }

    private void SetStatus(string text, bool error)
    {
        _workflowStatus = null;
        SetStatusPresentation(error ? WorkflowStatusSeverity.Error : WorkflowStatusSeverity.Info, text);
    }

    private void SetStatusPresentation(WorkflowStatusSeverity severity, string text)
    {
        lblStatus.Text = text;
        lblStatus.ToolTipText = text;
        lblStatus.ImageAlign = ContentAlignment.MiddleLeft;
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        lblStatus.Padding = new Padding(30, 4, 6, 4);
        bool emphasis = severity is WorkflowStatusSeverity.Warning or WorkflowStatusSeverity.Error;
        _statusRegularFont ??= new Font(Font, FontStyle.Regular);
        _statusEmphasisFont ??= new Font(Font, FontStyle.Bold);
        lblStatus.Font = emphasis ? _statusEmphasisFont : _statusRegularFont;
        switch (severity)
        {
            case WorkflowStatusSeverity.Success:
                _statusSuccessImage ??= SystemIcons.Shield.ToBitmap();
                lblStatus.Image = _statusSuccessImage;
                lblStatus.ForeColor = Color.DarkGreen;
                break;
            case WorkflowStatusSeverity.Warning:
                _statusWarningImage ??= SystemIcons.Warning.ToBitmap();
                lblStatus.Image = _statusWarningImage;
                lblStatus.ForeColor = Color.DarkGoldenrod;
                break;
            case WorkflowStatusSeverity.Error:
                _statusErrorImage ??= SystemIcons.Error.ToBitmap();
                lblStatus.Image = _statusErrorImage;
                lblStatus.ForeColor = Color.DarkRed;
                break;
            default:
                _statusInfoImage ??= SystemIcons.Information.ToBitmap();
                lblStatus.Image = _statusInfoImage;
                lblStatus.ForeColor = SystemColors.ControlText;
                break;
        }
    }

    private void Error(Exception ex)
    {
        SetGraphicsStatus(ex.Message, true);
        MessageBox.Show(this, ex.ToString(), UiText.Get("Error.Title"), MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void SetGraphicsStatus(string text, bool error)
    {
        _graphicsStatusIsError = error;
        SetStatus(text, error);
    }

    private void ClearStaleGraphicsError()
    {
        if (!_graphicsStatusIsError) return;
        _graphicsStatusIsError = false;
        SetStatus(string.Empty, error: false);
    }

    private bool IsActiveInstallation(GameInstallation installation)
    {
        return InstallationSelectionState.IsSameActiveInstallation(installation, txtGameDir.Text) &&
               _detectedProfile == installation.Game;
    }

    private static int CountControls(Control root)
    {
        int count = 1;
        foreach (Control child in root.Controls)
            count += CountControls(child);
        return count;
    }

    // Non-interactive lifecycle harness hooks.  They deliberately avoid dialogs
    // and persistence so the smoke can prove activation separately from UI input.
    internal void InitializeInstallationStateForTest() => InitializeInstallations();

    internal void ClearInstallationStateForTest()
    {
        ClearInstallationState();
        // The real selector transition has already selected its neutral item
        // before it invokes ClearInstallationState. Reproduce that UI detail
        // for the direct non-interactive state harness.
        RefreshInstallationSelector();
    }

    internal void DiscoverInstallationsForTest(IEnumerable<GameInstallation> installations)
    {
        foreach (GameInstallation installation in installations) AddInstallation(installation, refresh: false);
        RefreshInstallationSelector();
    }

    internal void ActivateInstallationForTest(GameInstallation installation) => ApplyInstallation(installation, persist: false);

    internal void SetActiveVariantForTest(BuiltInVariantId variantId)
    {
        if (_activeProject is null) throw new InvalidOperationException("No installation is active.");
        VariantContext variant = _availableActiveVariants.Single(item => item.VariantId == variantId);
        SetActiveVariant(variant);
    }

    internal void SelectGraphicsZoneForTest(string fileName)
    {
        int index = cmbZone.Items.IndexOf(fileName);
        if (index < 0) throw new InvalidOperationException("The requested graphics resource is unavailable.");
        cmbZone.SelectedIndex = index;
    }

    internal void SetTextEditForTest(int index, string text)
    {
        _gamePcEdits[index] = text;
        RefreshWorkflowStatus();
    }

    internal void SaveTextProjectForTest() => SaveGamePcTexts();

    internal void MaterializeGraphicsLayoutForTest()
    {
        // Non-interactive layout fixture: do not show a window, but give the
        // docked graphics hierarchy a realistic client area for geometry tests.
        tabVga.Size = new Size(1400, 700);
        foreach (Control child in tabVga.Controls)
        {
            child.Bounds = tabVga.ClientRectangle;
            child.PerformLayout();
        }
        mainSplit.Bounds = new Rectangle(0, 38, 1400, 620);
        ApplySafeHalfSplit();
        mainSplit.PerformLayout();
        foreach (Control child in mainSplit.Panel2.Controls) child.PerformLayout();
        previewViewport.PerformLayout();
        previewScroll.PerformLayout();
    }

    internal void MaterializeTabLayoutsForTest()
    {
        // Extended fixture for locale-geometry tests: every tab page gets a
        // realistic client area so no flow wraps merely because its host was
        // never laid out. Wrapping that remains is genuine content reflow.
        foreach (TabPage page in new[] { tabVga, tabText, tabFont, tabMods })
        {
            page.Size = new Size(1400, 700);
            foreach (Control child in page.Controls)
            {
                child.Bounds = page.ClientRectangle;
                child.PerformLayout();
            }
            page.PerformLayout();
        }
        MaterializeGraphicsLayoutForTest();
    }

    internal EditionSwitchDiagnostics RunEditionSwitchStressForTest(int cycles)
    {
        if (cycles <= 0) throw new ArgumentOutOfRangeException(nameof(cycles));
        if (_activeProject is null || _translationProjectState is null ||
            !_translationProjectState.Variants.Any(item => item.Code.Equals("S1", StringComparison.OrdinalIgnoreCase)) ||
            !_translationProjectState.Variants.Any(item => item.Code.Equals("SK", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("The edition-switch fixture requires S1 and SK projects.");

        EnsureEmbeddedFontEditor();
        SelectGraphicsZoneForTest("382.VGA");
        Rectangle viewportBounds = previewViewport.Bounds;
        Rectangle previewBounds = previewScroll.Bounds;
        // Validity gate (stress-harness correction): stability alone would
        // accept a degenerate, never-laid-out geometry (e.g. negative
        // height on an unmaterialized form). Require positive area first;
        // the per-switch equality checks below then prove stability too.
        if (viewportBounds.Width <= 0 || viewportBounds.Height <= 0 ||
            previewBounds.Width <= 0 || previewBounds.Height <= 0)
            throw new InvalidOperationException("Graphics preview geometry is degenerate; materialize the form layout before stress validation.");
        int controls = ControlTreeCount;
        int graphics = GraphicsLoadCount;
        int text = TextLoadCount;
        int runtime = RuntimeUiLoadCount;
        int font = FontBindCount;
        int workflow = WorkflowRefreshCount;
        int previewRenders = PreviewRenderCount;

        string[] sequence = ["EN", "S1", "SK", "S1"];
        for (int cycle = 0; cycle < cycles; cycle++)
        {
            foreach (string code in sequence)
            {
                SelectTranslationVariant(code);
                if (previewViewport.Bounds != viewportBounds || previewScroll.Bounds != previewBounds)
                    throw new InvalidOperationException("Edition switching changed Graphics preview geometry.");
                if (code.Equals("S1", StringComparison.OrdinalIgnoreCase) && _edits.Count != 1)
                    throw new InvalidOperationException("S1 Graphics state was not restored during stress switching.");
                if (!code.Equals("S1", StringComparison.OrdinalIgnoreCase) && _edits.Count != 0)
                    throw new InvalidOperationException("Graphics state leaked outside S1 during stress switching.");
                if (HasUnsavedTextChanges)
                    throw new InvalidOperationException("Edition switching created a false Text dirty state.");
            }
        }

        int switchCount = cycles * sequence.Length;
        var result = new EditionSwitchDiagnostics(
            switchCount,
            GraphicsLoadCount - graphics,
            TextLoadCount - text,
            RuntimeUiLoadCount - runtime,
            FontBindCount - font,
            WorkflowRefreshCount - workflow,
            PreviewRenderCount - previewRenders,
            controls,
            ControlTreeCount,
            viewportBounds,
            previewViewport.Bounds,
            previewBounds,
            previewScroll.Bounds);

        if (result.GraphicsLoads != switchCount || result.TextLoads != switchCount || result.RuntimeUiLoads != switchCount ||
            result.FontBinds != switchCount || result.WorkflowRefreshes < switchCount || result.WorkflowRefreshes > switchCount * 3 ||
            result.PreviewRenders != switchCount || result.ControlsBefore != result.ControlsAfter)
            throw new InvalidOperationException("Edition refresh work was not bounded per switch.");
        return result;
    }

    internal MainFormLayoutDiagnostics RunMainFormLayoutStressForTest()
    {
        if (_activeProject is null) throw new InvalidOperationException("An active project is required for layout stress validation.");
        GraphicsLayoutSnapshot baseline = CaptureGraphicsLayoutSnapshot();
        // Validity gate (stress-harness correction): snapshot equality alone
        // would accept degenerate rectangles. Baseline validity plus
        // AssertStableGraphicsLayout equality together prove valid + stable.
        if (baseline.PreviewViewportBounds.Width <= 0 || baseline.PreviewViewportBounds.Height <= 0 ||
            baseline.PreviewScrollBounds.Width <= 0 || baseline.PreviewScrollBounds.Height <= 0)
            throw new InvalidOperationException("Graphics layout snapshot is degenerate; materialize the form layout before stress validation.");

        for (int cycle = 0; cycle < 25; cycle++)
        {
            foreach (string code in new[] { "EN", "S1", "SK", "S1" })
            {
                SelectTranslationVariant(code);
                AssertStableGraphicsLayout(baseline, "edition change");
            }
        }

        for (int index = 0; index < 20; index++)
        {
            SwitchMode(tabVga);
            SwitchMode(tabText);
            SwitchMode(tabFont);
            SwitchMode(tabMods);
            SwitchMode(tabVga);
            AssertStableGraphicsLayout(baseline, "tab change");
        }

        string priorLocale = UiText.LocaleId;
        try
        {
            foreach (string locale in new[] { "sk", "cs", "en", "sk", "cs", "en", "sk", "cs", "en", "sk" })
            {
                UiText.SetLocale(locale);
                AssertStableGraphicsLayout(baseline, "locale change");
            }
        }
        finally { UiText.SetLocale(priorLocale); }

        AssertStableGraphicsLayout(baseline, "locale restoration");
        return new MainFormLayoutDiagnostics(baseline, CaptureGraphicsLayoutSnapshot(), 100, 20, 10);
    }

    private GraphicsLayoutSnapshot CaptureGraphicsLayoutSnapshot() => new(
        mainSplit.Bounds,
        mainSplit.Panel1.Bounds,
        mainSplit.Panel2.Bounds,
        mainSplit.SplitterDistance,
        previewViewport.Bounds,
        previewScroll.Bounds,
        _graphicsToolbar?.Bounds ?? Rectangle.Empty,
        _graphicsActionRow?.Bounds ?? Rectangle.Empty);

    private void AssertStableGraphicsLayout(GraphicsLayoutSnapshot expected, string transition)
    {
        if (CaptureGraphicsLayoutSnapshot() != expected)
            throw new InvalidOperationException("Graphics layout changed after " + transition + ".");
    }

    internal IReadOnlyList<ActionButtonGeometry> CaptureActionButtonGeometryForTest()
    {
        (Button Button, string Name)[] buttons =
        [
            (btnReplace, nameof(btnReplace)), (btnClearEdit, nameof(btnClearEdit)),
            (btnExport, nameof(btnExport)), (btnRestore, nameof(btnRestore)),
            (btnVerifyPristine, nameof(btnVerifyPristine)),
            (btnRestoreBaselineLauncher, nameof(btnRestoreBaselineLauncher)),
            (btnRebuildOwnedVariant, nameof(btnRebuildOwnedVariant)),
            (btnRemoveOwnedVariant, nameof(btnRemoveOwnedVariant)),
            (btnReverseAllPreview, nameof(btnReverseAllPreview)),
            (btnRunVariant, nameof(btnRunVariant)), (btnDebugVariant, nameof(btnDebugVariant)),
            (btnOpenDataFile, nameof(btnOpenDataFile)), (btnReloadTexts, nameof(btnReloadTexts)),
            (btnSaveTexts, nameof(btnSaveTexts)), (btnSaveAsDataFile, nameof(btnSaveAsDataFile)),
            (btnCreateDataVariant, nameof(btnCreateDataVariant)),
            (btnExportTranslations, nameof(btnExportTranslations)),
            (btnImportTranslations, nameof(btnImportTranslations)),
            (btnReloadRuntimeUi, nameof(btnReloadRuntimeUi)),
            (btnSaveRuntimeUi, nameof(btnSaveRuntimeUi)),
            (btnResetRuntimeUi, nameof(btnResetRuntimeUi)),
            (btnSelectLauncherFile, nameof(btnSelectLauncherFile)),
            (btnPreviewLauncher, nameof(btnPreviewLauncher)),
            (btnGenerateLauncher, nameof(btnGenerateLauncher)),
            (btnRestoreLauncher, nameof(btnRestoreLauncher)),
            (btnVariantAdd, nameof(btnVariantAdd)), (btnVariantEdit, nameof(btnVariantEdit)),
            (btnVariantRemove, nameof(btnVariantRemove)), (btnVariantMoveUp, nameof(btnVariantMoveUp)),
            (btnVariantMoveDown, nameof(btnVariantMoveDown)),
            (btnVariantToggleEnabled, nameof(btnVariantToggleEnabled)),
            (btnVariantOpenDataFile, nameof(btnVariantOpenDataFile)),
            (btnFindGames, nameof(btnFindGames)), (btnBrowseGame, nameof(btnBrowseGame)),
            (btnBuildActiveVariant, nameof(btnBuildActiveVariant)),
            (btnHelp, nameof(btnHelp)), (btnAbout, nameof(btnAbout)),
            (btnSpriteEditor, nameof(btnSpriteEditor)), (btnTextEditor, nameof(btnTextEditor)),
            (btnFontEditor, nameof(btnFontEditor)), (btnModsLauncher, nameof(btnModsLauncher)),
            (btnReload, nameof(btnReload)), (btnPaletteAdvanced, nameof(btnPaletteAdvanced)),
            (btnSaveGraphicsProject, nameof(btnSaveGraphicsProject)),
            (btnReloadPreview, nameof(btnReloadPreview)),
        ];
        return buttons.Select(item => new ActionButtonGeometry(
            item.Name, item.Button.GetType().Name, item.Button.Bounds, item.Button.MinimumSize, item.Button.AutoSize,
            item.Button.Padding, item.Button.TextAlign, item.Button.UseCompatibleTextRendering, item.Button.Enabled,
            item.Button.Text ?? string.Empty)).ToArray();
    }

    internal void SetButtonEnabledForTest(string buttonFieldName, bool enabled)
    {
        if (GetType().GetField(buttonFieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this) is Button button)
            button.Enabled = enabled;
        else
            throw new InvalidOperationException($"Unknown action button '{buttonFieldName}'.");
    }

    internal Control FindControlForTest(string fieldName) =>
        GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this) as Control
        ?? throw new InvalidOperationException($"Unknown control '{fieldName}'.");

    internal IReadOnlyList<ContextComboGeometry> CaptureContextComboGeometryForTest() => [
        new(nameof(cmbInstallations), cmbInstallations.Bounds, cmbInstallations.Margin),
        new(nameof(cmbActiveVariant), cmbActiveVariant.Bounds, cmbActiveVariant.Margin),
        new(nameof(cmbActiveProject), cmbActiveProject.Bounds, cmbActiveProject.Margin)];

    internal IReadOnlyList<ContextCellGeometry> CaptureContextCellGeometryForTest()
    {
        if (_headerGrid is null)
            throw new InvalidOperationException("Header layout is not built.");
        int[] widths = _headerGrid.GetColumnWidths();
        int[] heights = _headerGrid.GetRowHeights();
        ContextCellGeometry cell(ComboBox combo, string name)
        {
            TableLayoutPanelCellPosition position = _headerGrid.GetCellPosition(combo);
            int x = widths.Take(position.Column).Sum();
            int y = heights.Take(position.Row).Sum();
            return new ContextCellGeometry(name, combo.Bounds,
                new Rectangle(x, y, widths[position.Column], heights[position.Row]),
                _headerGrid.ClientRectangle);
        }
        return [cell(cmbInstallations, nameof(cmbInstallations)), cell(cmbActiveVariant, nameof(cmbActiveVariant)), cell(cmbActiveProject, nameof(cmbActiveProject))];
    }

    internal TableLayoutPanelCellPosition HeaderCellForTest(string controlFieldName)
    {
        if (_headerGrid is null)
            throw new InvalidOperationException("Header layout is not built.");
        if (GetType().GetField(controlFieldName, BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(this) is not Control control)
            throw new InvalidOperationException($"Unknown header control '{controlFieldName}'.");
        return _headerGrid.GetCellPosition(control);
    }

    internal Rectangle HeaderGridClientForTest => _headerGrid is null
        ? Rectangle.Empty
        : new Rectangle(Point.Empty, _headerGrid.ClientSize);

    internal int[] HeaderRowHeightsForTest() =>
        _headerGrid?.GetRowHeights() ?? [];

    internal ElviraGameProfile ActiveGameProfileForTest => ActiveGameProfile;

    internal Rectangle NavigationRowBoundsForTest => _navigationRow?.Bounds ?? Rectangle.Empty;

    internal Rectangle NavigationRowClientForTest => _navigationRow is null
        ? Rectangle.Empty
        : new Rectangle(Point.Empty, _navigationRow.ClientSize);

    internal Rectangle GraphicsActionRowBoundsForTest => _graphicsActionRow?.Bounds ?? Rectangle.Empty;

    internal IReadOnlyList<Rectangle> GraphicsActionChildBoundsForTest() =>
        (_graphicsActionRow?.Controls.Cast<Control>() ?? Enumerable.Empty<Control>()).Select(child => child.Bounds).ToArray();

    internal int StatusStripItemCountForTest => statusBar.Items.Count;

    internal Rectangle StatusStripBoundsForTest => statusBar.Bounds;

    internal void SelectTranslationForTest(string code)
    {
        if (_activeProject is null) throw new InvalidOperationException("No installation is active.");
        TranslationProjectLoadResult loaded = _translationProjects.Load(_activeProject);
        if (!loaded.IsSuccess || (!code.Equals("EN", StringComparison.OrdinalIgnoreCase) && !loaded.State!.Variants.Any(item => item.Code.Equals(code, StringComparison.OrdinalIgnoreCase))))
            throw new InvalidOperationException("The requested project translation does not exist.");
        if (_translationProjectState is null) LoadGamePcTexts();
        SelectTranslationVariant(code);
    }

    internal void OpenTranslationFromCatalogForTest(string code)
    {
        OpenModsLauncher();
        DataGridViewRow? row = variantGrid.Rows.Cast<DataGridViewRow>().SingleOrDefault(item =>
            item.Tag is VariantEntry entry && entry.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
        if (row is null) throw new InvalidOperationException("The requested translation is not present in the project translation catalog.");
        row.Selected = true;
        variantGrid.CurrentCell = row.Cells[0];
        OpenSelectedVariantDataFile();
    }

    internal void BrowseValidatedInstallationForTest(GameInstallation installation)
    {
        AddInstallation(installation, refresh: false);
        ApplyInstallation(installation, persist: false);
    }

    internal void ActivateFontForTest()
    {
        EnsureEmbeddedFontEditor();
        BindFontEditorToActiveVariant(showMissingError: false);
    }

    internal void OpenModsForTest() => OpenModsLauncher();

    internal ContextSafetyDiagnostics RunContextSafetyStressForTest(GameInstallation elvira1, GameInstallation elvira2)
    {
        ApplyInstallation(elvira1, persist: false);
        EnsureEmbeddedFontEditor();
        _embeddedFontEditor!.LoadFromGameDirectory(elvira1.InstallationPath);
        int controlsBefore = ControlTreeCount;
        for (int activation = 0; activation < 20; activation++)
        {
            ScanGameFolder();
            LoadGamePcTexts();
            _embeddedFontEditor!.LoadFromGameDirectory(elvira1.InstallationPath);
            OpenModsLauncher();
            SwitchMode(tabVga);
            SwitchMode(tabText);
            SwitchMode(tabFont);
            SwitchMode(tabMods);
        }

        for (int switchIndex = 0; switchIndex < 10; switchIndex++)
        {
            ApplyInstallation(elvira2, persist: false);
            ScanGameFolder(); LoadGamePcTexts(); OpenModsLauncher();
            OpenFirstVariantInTextEditorForTest();
            EnsureEmbeddedFontEditor();
            _embeddedFontEditor!.LoadFromGameDirectory(elvira2.InstallationPath);

            ApplyInstallation(elvira1, persist: false);
            ScanGameFolder(); LoadGamePcTexts(); OpenModsLauncher();
            OpenFirstVariantInTextEditorForTest();
            _embeddedFontEditor!.LoadFromGameDirectory(elvira1.InstallationPath);
        }

        int effectiveBeforeRedundant = ApplyInstallationEffectiveCount;
        for (int repeat = 0; repeat < 5; repeat++) ApplyInstallation(elvira1, persist: false);
        if (ApplyInstallationEffectiveCount != effectiveBeforeRedundant)
            throw new InvalidOperationException("An identical active installation was applied again.");

        return new ContextSafetyDiagnostics(
            ApplyInstallationRequestedCount,
            ApplyInstallationEffectiveCount,
            ApplyInstallationSuppressedCount,
            GraphicsLoadCount,
            TextLoadCount,
            _embeddedFontEditor?.SourceLoadCount ?? 0,
            ModsRefreshCount,
            controlsBefore,
            ControlTreeCount);
    }

    private void OpenFirstVariantInTextEditorForTest()
    {
        if (_activeProject is null || _activeVariant is null)
            throw new InvalidOperationException("The active variant was not available for the context-safety test.");

        string logicalDataFile = Path.Combine(_activeProject.GameRoot, _activeVariant.LogicalDataFileName);
        if (!File.Exists(logicalDataFile))
            throw new InvalidOperationException("The active variant logical data file was not available for the context-safety test.");

        SetCurrentDataFile(logicalDataFile);
        LoadGamePcTexts();
        SwitchMode(tabText);
    }
}

internal sealed record ContextSafetyDiagnostics(
    int ApplyRequested,
    int ApplyEffective,
    int ApplySuppressed,
    int GraphicsLoads,
    int TextLoads,
    int FontLoads,
    int ModsRefreshes,
    int ControlsBefore,
    int ControlsAfter);

internal sealed record EditionSwitchDiagnostics(
    int SwitchCount,
    int GraphicsLoads,
    int TextLoads,
    int RuntimeUiLoads,
    int FontBinds,
    int WorkflowRefreshes,
    int PreviewRenders,
    int ControlsBefore,
    int ControlsAfter,
    Rectangle ViewportBefore,
    Rectangle ViewportAfter,
    Rectangle PreviewBefore,
    Rectangle PreviewAfter);

internal sealed record GraphicsLayoutSnapshot(
    Rectangle SplitBounds,
    Rectangle LeftPanelBounds,
    Rectangle RightPanelBounds,
    int SplitterDistance,
    Rectangle PreviewViewportBounds,
    Rectangle PreviewScrollBounds,
    Rectangle ToolbarBounds,
    Rectangle ActionRowBounds);

internal sealed record MainFormLayoutDiagnostics(
    GraphicsLayoutSnapshot Before,
    GraphicsLayoutSnapshot After,
    int EditionChanges,
    int TabChanges,
    int LocaleChanges);

internal sealed record ActionButtonGeometry(
    string Name,
    string Renderer,
    Rectangle Bounds,
    Size MinimumSize,
    bool AutoSize,
    Padding Padding,
    ContentAlignment TextAlign,
    bool UseCompatibleTextRendering,
    bool Enabled,
    string Text);

internal sealed record ContextComboGeometry(
    string Name,
    Rectangle Bounds,
    Padding Margin);

internal sealed record ContextCellGeometry(
    string Name,
    Rectangle ControlBounds,
    Rectangle CellBounds,
    Rectangle PanelClient);

internal sealed class ModeButtonFonts(Font regular, Font bold) : IDisposable
{
    public Font Regular { get; } = regular;
    public Font Bold { get; } = bold;
    public void Dispose()
    {
        Regular.Dispose();
        Bold.Dispose();
    }
}

/// <summary>Read-only Variant Manager projection. It never authorizes or
/// performs a build, run, directory creation, or other file operation.</summary>
internal sealed record VariantManagerRow(
    VariantContext Variant,
    VariantDirectoryOperationStatus Ownership,
    VariantLaunchReadiness Readiness,
    string VariantRoot,
    VariantBuildStatus BuildStatus,
    int ConfiguredCapabilities,
    int CapabilityCount);

internal sealed record PalettePreviewChoice(int? BankIndex, string Display)
{
    public override string ToString() => Display;
}

internal sealed record TextTranslationSelection(string Code, string DisplayName, string DataFile, string ExeFile)
{
    public override string ToString() => Code.Equals("EN", StringComparison.OrdinalIgnoreCase)
        ? $"{DisplayName} (EN)"
        : $"{DisplayName} ({Code})";
}

/// <summary>
/// One shared caption-placement path for normal MainForm action buttons.
/// The native framework paints the COMPLETE button (background, border and
/// all normal/hot/pressed/disabled/focused/default and FlatStyle visuals);
/// only the native caption is suppressed during that paint and drawn
/// explicitly afterwards with a single common optical vertical offset that
/// standard Button padding cannot reliably produce on the real themed
/// display. Geometry, measurement, AutoSize, fonts and event behavior are
/// inherited unchanged from Button. WinForms-level double buffering is
/// deliberately left at the stock Button default: enabling it on top of the
/// native themed paint pipeline produced incomplete first-paint chrome that
/// only repaired on the next invalidation.
/// </summary>
internal sealed class CenteredCaptionButton : Button
{
    // Common optical correction, logical px at 96 dpi, scaled by DeviceDpi.
    // Same implementation for EN/SK/CS and all states; no per-button or
    // per-language offsets.
    internal const int OpticalCaptionOffsetY = 1;

    private bool _suppressCaption;

    // Nullability note: base Control.Text accepts null (treated as empty);
    // this override forwards verbatim, so behavior is identical.
#pragma warning disable CS8765 // Nullability of type of parameter 'value' doesn't match overridden member
    public override string Text
    {
        // During the native paint below the caption must read empty so the
        // framework draws chrome without text. Outside paint the real caption
        // is visible, so measurement, layout, accessibility and data binding
        // all keep working on the true text.
        get => _suppressCaption ? "" : base.Text;
        set => base.Text = value;
    }
#pragma warning restore CS8765

    // The native focus rectangle would hug the suppressed (empty) caption;
    // hide it and draw our own around the real caption instead.
    protected override bool ShowFocusCues => false;

    protected override void OnPaint(PaintEventArgs e)
    {
        // Image-bearing buttons keep fully native painting (no MainForm
        // action button uses images; a future one must not lose content).
        if (Image is not null || BackgroundImage is not null)
        {
            base.OnPaint(e);
            return;
        }

        _suppressCaption = true;
        try
        {
            base.OnPaint(e);
        }
        finally
        {
            _suppressCaption = false;
        }

        if (Focused && base.ShowFocusCues)
            ControlPaint.DrawFocusRectangle(e.Graphics, CaptionRectangle());

        PaintCaption(e.Graphics);
    }

    private Rectangle CaptionRectangle()
    {
        Rectangle client = ClientRectangle;
        Rectangle text = new(
            client.X + Padding.Left,
            client.Y + Padding.Top,
            Math.Max(0, client.Width - Padding.Horizontal),
            Math.Max(0, client.Height - Padding.Vertical));
        text.Y += (int)Math.Round(OpticalCaptionOffsetY * DeviceDpi / 96f);
        return text;
    }

    private void PaintCaption(Graphics graphics)
    {
        Rectangle text = CaptionRectangle();
        if (text.Width <= 0 || text.Height <= 0)
            return;
        TextFormatFlags flags = TextAlign switch
        {
            ContentAlignment.TopLeft => TextFormatFlags.Left | TextFormatFlags.Top,
            ContentAlignment.TopCenter => TextFormatFlags.HorizontalCenter | TextFormatFlags.Top,
            ContentAlignment.TopRight => TextFormatFlags.Right | TextFormatFlags.Top,
            ContentAlignment.MiddleLeft => TextFormatFlags.Left | TextFormatFlags.VerticalCenter,
            ContentAlignment.MiddleRight => TextFormatFlags.Right | TextFormatFlags.VerticalCenter,
            ContentAlignment.BottomLeft => TextFormatFlags.Left | TextFormatFlags.Bottom,
            ContentAlignment.BottomCenter => TextFormatFlags.HorizontalCenter | TextFormatFlags.Bottom,
            ContentAlignment.BottomRight => TextFormatFlags.Right | TextFormatFlags.Bottom,
            _ => TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
        };
        flags |= TextFormatFlags.SingleLine;
        // Showing & mnemonics is the default; HidePrefix only while keyboard
        // cues are hidden, NoPrefix when mnemonics are disabled entirely.
        flags |= !UseMnemonic ? TextFormatFlags.NoPrefix
            : ShowKeyboardCues ? (TextFormatFlags)0 : TextFormatFlags.HidePrefix;
        if (RightToLeft == RightToLeft.Yes)
            flags |= TextFormatFlags.RightToLeft;
        Color color = Enabled ? ForeColor : SystemColors.GrayText;
        TextRenderer.DrawText(graphics, Text, Font, text, color, flags);
    }
}

internal sealed class PixelPerfectPictureBox : PictureBox
{
    public bool PixelPerfect { get; set; } = true;

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaintBackground(e);
        if (Image is null || ClientSize.Width <= 0 || ClientSize.Height <= 0)
            return;

        Rectangle destination = SizeMode switch
        {
            PictureBoxSizeMode.StretchImage => ClientRectangle,
            PictureBoxSizeMode.Zoom => ZoomRectangle(Image.Size, ClientSize),
            PictureBoxSizeMode.CenterImage => new Rectangle(
                Math.Max(0, (ClientSize.Width - Image.Width) / 2),
                Math.Max(0, (ClientSize.Height - Image.Height) / 2),
                Image.Width,
                Image.Height),
            _ => new Rectangle(0, 0, Image.Width, Image.Height)
        };

        e.Graphics.InterpolationMode = PixelPerfect ? InterpolationMode.NearestNeighbor : InterpolationMode.HighQualityBicubic;
        e.Graphics.PixelOffsetMode = PixelPerfect ? PixelOffsetMode.Half : PixelOffsetMode.HighQuality;
        e.Graphics.SmoothingMode = PixelPerfect ? SmoothingMode.None : SmoothingMode.HighQuality;
        e.Graphics.CompositingQuality = PixelPerfect ? CompositingQuality.HighSpeed : CompositingQuality.HighQuality;
        e.Graphics.DrawImage(Image, destination, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
    }

    private static Rectangle ZoomRectangle(Size imageSize, Size clientSize)
    {
        float scale = Math.Min((float)clientSize.Width / imageSize.Width, (float)clientSize.Height / imageSize.Height);
        int width = Math.Max(1, (int)Math.Round(imageSize.Width * scale));
        int height = Math.Max(1, (int)Math.Round(imageSize.Height * scale));
        return new Rectangle((clientSize.Width - width) / 2, (clientSize.Height - height) / 2, width, height);
    }
}
