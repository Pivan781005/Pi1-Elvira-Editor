namespace Pi1ElviraEditor;

/// <summary>Small metadata-only editor for a launcher variant. It never touches a data file.</summary>
internal sealed class VariantEntryDialog : Form
{
    private readonly TextBox txtDisplayName = new();
    private readonly TextBox txtDataFile = new();
    private readonly CheckBox chkEnabled = new();
    private readonly Label lblDisplayName = new();
    private readonly Label lblDataFile = new();
    private readonly Label lblMetadataOnly = new();
    private readonly Button btnOk = new();
    private readonly Button btnCancel = new();

    internal VariantEntryDialog(VariantEntry? entry = null, string? displayName = null, string? dataFile = null, bool enabled = true)
    {
        Text = UiText.Get(entry is null ? "VariantDialogAddTitle" : "VariantDialogEditTitle");
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        MaximizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(470, 210);

        lblDisplayName.Text = UiText.Get("VariantDisplayName");
        lblDisplayName.SetBounds(12, 18, 130, 22);
        txtDisplayName.SetBounds(145, 14, 310, 26);

        lblDataFile.Text = UiText.Get("VariantDataFile") + ":";
        lblDataFile.SetBounds(12, 54, 130, 22);
        txtDataFile.SetBounds(145, 50, 310, 26);
        txtDataFile.CharacterCasing = CharacterCasing.Upper;

        chkEnabled.Text = UiText.Get("VariantEnabledField");
        chkEnabled.SetBounds(145, 84, 130, 24);

        lblMetadataOnly.Text = UiText.Get("VariantMetadataOnly");
        lblMetadataOnly.SetBounds(12, 116, 443, 40);
        lblMetadataOnly.AutoSize = false;
        lblMetadataOnly.ForeColor = SystemColors.GrayText;

        btnOk.Text = UiText.Get("Common.Ok");
        btnOk.SetBounds(280, 168, 82, 28);
        btnOk.DialogResult = DialogResult.OK;
        btnOk.Click += (_, _) => ValidateInput();
        btnCancel.Text = UiText.Get("Cancel");
        btnCancel.SetBounds(373, 168, 82, 28);
        btnCancel.DialogResult = DialogResult.Cancel;

        AcceptButton = btnOk;
        CancelButton = btnCancel;
        Controls.AddRange(new Control[] { lblDisplayName, txtDisplayName, lblDataFile, txtDataFile, chkEnabled, lblMetadataOnly, btnOk, btnCancel });

        txtDisplayName.Text = entry?.DisplayName ?? displayName ?? string.Empty;
        txtDataFile.Text = entry?.DataFile ?? dataFile ?? string.Empty;
        chkEnabled.Checked = entry?.Enabled ?? enabled;
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
