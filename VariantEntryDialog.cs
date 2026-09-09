namespace Pi1ElviraEditor;

/// <summary>Small metadata-only editor for a launcher variant. It never touches a data file.</summary>
internal sealed class VariantEntryDialog : Form
{
    private readonly TextBox txtDisplayName = new();
    private readonly TextBox txtDataFile = new();
    private readonly CheckBox chkEnabled = new();
    private readonly Label lblDisplayName = new();
    private readonly Label lblDataFile = new();
    private readonly Label lblExePreview = new();
    private readonly Label lblHint = new();
    private readonly Label lblMetadataOnly = new();
    private readonly Button btnOk = new();
    private readonly Button btnCancel = new();

    internal VariantEntryDialog(VariantEntry? entry = null, string? displayName = null, string? dataFile = null, bool enabled = true, string? exePreview = null)
    {
        Text = UiText.Get(entry is null ? "VariantDialogAddTitle" : "VariantDialogEditTitle");
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(470, 300);

        lblDisplayName.Text = UiText.Get("VariantDisplayName");
        lblDisplayName.SetBounds(12, 18, 130, 22);
        txtDisplayName.SetBounds(145, 14, 310, 26);

        lblDataFile.Text = UiText.Get("VariantDataFile") + ":";
        lblDataFile.SetBounds(12, 54, 130, 22);
        txtDataFile.SetBounds(145, 50, 310, 26);
        txtDataFile.CharacterCasing = CharacterCasing.Upper;

        chkEnabled.Text = UiText.Get("VariantEnabledField");
        chkEnabled.SetBounds(145, 84, 130, 24);

        lblExePreview.SetBounds(12, 114, 443, 22);
        lblExePreview.AutoSize = false;
        lblExePreview.ForeColor = SystemColors.GrayText;

        lblHint.Text = UiText.Get("VariantDialogHint");
        lblHint.SetBounds(12, 140, 443, 60);
        lblHint.AutoSize = false;
        lblHint.ForeColor = SystemColors.GrayText;

        lblMetadataOnly.Text = UiText.Get("VariantMetadataOnly");
        lblMetadataOnly.SetBounds(12, 204, 443, 40);
        lblMetadataOnly.AutoSize = false;
        lblMetadataOnly.ForeColor = SystemColors.GrayText;

        btnOk.Text = UiText.Get("Common.Ok");
        btnOk.SetBounds(280, 258, 82, 28);
        btnOk.DialogResult = DialogResult.OK;
        btnOk.Click += (_, _) => ValidateInput();
        btnCancel.Text = UiText.Get("Cancel");
        btnCancel.SetBounds(373, 258, 82, 28);
        btnCancel.DialogResult = DialogResult.Cancel;

        AcceptButton = btnOk;
        CancelButton = btnCancel;
        Controls.AddRange(new Control[] { lblDisplayName, txtDisplayName, lblDataFile, txtDataFile, chkEnabled, lblExePreview, lblHint, lblMetadataOnly, btnOk, btnCancel });

        txtDisplayName.Text = entry?.DisplayName ?? displayName ?? string.Empty;
        txtDataFile.Text = entry?.DataFile ?? dataFile ?? string.Empty;
        chkEnabled.Checked = entry?.Enabled ?? enabled;
        string preview = entry?.ExeFile ?? exePreview ?? string.Empty;
        lblExePreview.Text = string.IsNullOrWhiteSpace(preview)
            ? string.Empty
            : string.Format(UiText.Get("VariantDialogExePreview"), preview);
        var tips = new ToolTip();
        try
        {
            tips.SetToolTip(txtDataFile, UiText.Get("VariantDialogHint"));
            tips.SetToolTip(lblExePreview, UiText.Get("VariantDialogExePreview").Replace("{0}", preview));
        }
        catch { }
    }

    internal string DisplayName => txtDisplayName.Text.Trim();
    internal string DataFile => txtDataFile.Text.Trim();
    internal bool IsVariantEnabled => chkEnabled.Checked;

    private void ValidateInput()
    {
        if (DisplayName.Length == 0 || !VariantConfigurationService.IsValidDataFileName(DataFile))
        {
            DialogResult = DialogResult.None;
            MessageBox.Show(this, UiText.Get("VariantValidationError"), UiText.Get("Warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
