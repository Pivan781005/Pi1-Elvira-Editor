namespace ElviraVgaEditor;

internal sealed class TranslationVariantDialog : Form
{
    private readonly TextBox txtName = new();
    private readonly TextBox txtCode = new();
    private readonly Label lblPreview = new();
    private readonly Button btnCreate = new();
    private readonly Button btnCancel = new();
    private readonly ElviraGameProfile _game;

    internal TranslationVariantDialog(ElviraGameProfile game)
    {
        _game = game;
        Text = UiText.Get("CreateVariant");
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(430, 170);
        var nameLabel = new Label { Text = UiText.Get("VariantName"), Location = new Point(12, 16), Size = new Size(105, 24) };
        var codeLabel = new Label { Text = UiText.Get("VariantCode"), Location = new Point(12, 51), Size = new Size(105, 24) };
        txtName.SetBounds(120, 13, 295, 26);
        txtCode.SetBounds(120, 48, 80, 26);
        txtCode.CharacterCasing = CharacterCasing.Upper;
        txtCode.MaxLength = TranslationProjectService.GetMaximumCodeLength(_game);
        txtName.TextChanged += (_, _) => UpdatePreview();
        txtCode.TextChanged += (_, _) => NormalizeCodeAndUpdatePreview();
        lblPreview.SetBounds(12, 82, 403, 38); lblPreview.ForeColor = SystemColors.GrayText;
        btnCreate.Text = UiText.Get("CreateVariant"); btnCreate.DialogResult = DialogResult.OK; btnCreate.Location = new Point(232, 130); btnCreate.Size = new Size(90, 30);
        btnCreate.AutoSize = false; btnCreate.Padding = Padding.Empty; btnCreate.TextAlign = ContentAlignment.MiddleCenter;
        btnCancel.Text = UiText.Get("Cancel"); btnCancel.DialogResult = DialogResult.Cancel; btnCancel.Location = new Point(330, 130); btnCancel.Size = new Size(85, 30);
        btnCancel.AutoSize = false; btnCancel.Padding = Padding.Empty; btnCancel.TextAlign = ContentAlignment.MiddleCenter;
        Controls.AddRange([nameLabel, codeLabel, txtName, txtCode, lblPreview, btnCreate, btnCancel]);
        AcceptButton = btnCreate; CancelButton = btnCancel; UpdatePreview();
    }

    internal string NameValue => txtName.Text.Trim();
    internal string Code => txtCode.Text.Trim();
    internal bool HasCenteredActionButtonsForTest =>
        btnCreate.Height == btnCancel.Height &&
        btnCreate.TextAlign == ContentAlignment.MiddleCenter &&
        btnCancel.TextAlign == ContentAlignment.MiddleCenter &&
        btnCreate.Right <= ClientSize.Width && btnCancel.Right <= ClientSize.Width;

    private void NormalizeCodeAndUpdatePreview()
    {
        string filtered = new string(txtCode.Text.Where(character => character is >= 'A' and <= 'Z' or >= 'a' and <= 'z' or >= '0' and <= '9').ToArray()).ToUpperInvariant();
        if (!txtCode.Text.Equals(filtered, StringComparison.Ordinal))
        {
            int selection = Math.Min(txtCode.SelectionStart, filtered.Length);
            txtCode.Text = filtered;
            txtCode.SelectionStart = selection;
            return;
        }
        UpdatePreview();
    }

    private void UpdatePreview()
    {
        try
        {
            string code = TranslationProjectService.ValidateProjectCode(Code, _game);
            VariantEntry entry = VariantNaming.Create(NameValue, code, _game, true, 1);
            lblPreview.Text = $"{UiText.Get("Translation")}: {entry.DataFile}    EXE: {entry.ExeFile}";
            btnCreate.Enabled = NameValue.Length > 0;
        }
        catch
        {
            lblPreview.Text = string.Empty;
            btnCreate.Enabled = false;
        }
    }
}
