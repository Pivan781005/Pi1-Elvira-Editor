namespace Pi1ElviraEditor;

internal sealed class AboutViewerForm : Form
{
    private readonly RichTextBox _content = new();
    internal string TitleForTest => Text;
    internal string BodyForTest => _content.Text;

    internal AboutViewerForm(UiLanguage language)
    {
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(560, 390);
        Size = new Size(720, 540);
        AutoScaleMode = AutoScaleMode.Dpi;
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
        _content.Dock = DockStyle.Fill;
        _content.ReadOnly = true;
        _content.BorderStyle = BorderStyle.None;
        _content.BackColor = SystemColors.Window;
        _content.DetectUrls = true;
        _content.Font = new Font(SystemFonts.MessageBoxFont?.FontFamily ?? FontFamily.GenericSansSerif, 10f);
        Controls.Add(_content);
        SetLanguage(language);
    }

    internal void SetLanguage(UiLanguage language)
    {
        AboutDocument document = AboutDocumentService.Load(language);
        // The surrounding caption may be localized, but product identity is
        // deliberately locale-invariant.
        Text = UiText.Get("About") + " — " + AppInfo.ProductTitle;
        _content.Text = document.Body;
        _content.SelectionStart = 0;
        _content.ScrollToCaret();
    }
}
