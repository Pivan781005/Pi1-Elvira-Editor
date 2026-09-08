namespace Pi1ElviraEditor;

internal sealed class HelpViewerForm : Form
{
    private readonly TreeView _topics = new();
    private readonly RichTextBox _content = new();
    private HelpDocument? _document;

    internal string FirstTopicTitleForTest => _document?.Topics.FirstOrDefault()?.Title ?? string.Empty;
    internal int GroupCountForTest => _topics.Nodes.Count;

    internal HelpViewerForm(UiLanguage language)
    {
        Text = string.Format(UiText.Get("Help.WindowTitle"), UiText.Get("Help"));
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 520);
        Size = new Size(1040, 720);
        AutoScaleMode = AutoScaleMode.Dpi;
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }

        // Do not set splitter constraints before the modeless form has a real client size.
        // WinForms validates those setters immediately and can reject the initial zero-width layout.
        var split = new SplitContainer { Dock = DockStyle.Fill };
        _topics.Dock = DockStyle.Fill;
        _topics.HideSelection = false;
        _topics.AfterSelect += (_, _) => ShowSelectedTopic();
        _content.Dock = DockStyle.Fill;
        _content.ReadOnly = true;
        _content.BorderStyle = BorderStyle.None;
        _content.BackColor = SystemColors.Window;
        _content.DetectUrls = true;
        _content.Font = new Font(SystemFonts.MessageBoxFont?.FontFamily ?? FontFamily.GenericSansSerif, 10f);
        split.Panel1.Padding = new Padding(8);
        split.Panel2.Padding = new Padding(12);
        split.Panel1.Controls.Add(_topics);
        split.Panel2.Controls.Add(_content);
        Controls.Add(split);
        SetLanguage(language);
    }

    internal void SetLanguage(UiLanguage language)
    {
        _document = HelpDocumentService.Load(language);
        Text = string.Format(UiText.Get("Help.WindowTitle"), UiText.Get("Help"));
        _topics.BeginUpdate();
        _topics.Nodes.Clear();
        foreach (IGrouping<string, HelpTopic> group in _document.Topics.GroupBy(topic => topic.Group, StringComparer.Ordinal))
        {
            var groupNode = new TreeNode(group.Key);
            foreach (HelpTopic topic in group)
                groupNode.Nodes.Add(new TreeNode(topic.Title) { Tag = topic });
            _topics.Nodes.Add(groupNode);
        }
        _topics.ExpandAll();
        _topics.EndUpdate();
        _topics.SelectedNode = _topics.Nodes.Cast<TreeNode>().FirstOrDefault()?.Nodes.Cast<TreeNode>().FirstOrDefault();
    }

    private void ShowSelectedTopic()
    {
        if (_topics.SelectedNode?.Tag is not HelpTopic topic) return;
        _content.Clear();
        _content.SelectionFont = new Font(_content.Font.FontFamily, 18f, FontStyle.Bold);
        _content.AppendText(topic.Title + Environment.NewLine + Environment.NewLine);
        _content.SelectionFont = _content.Font;
        foreach (string raw in topic.Body.Split('\n'))
        {
            string line = raw.TrimEnd('\r');
            if (line.StartsWith("### ", StringComparison.Ordinal))
            {
                _content.SelectionFont = new Font(_content.Font.FontFamily, 12f, FontStyle.Bold);
                _content.AppendText(line[4..] + Environment.NewLine);
                _content.SelectionFont = _content.Font;
            }
            else
            {
                string display = line.StartsWith("- ", StringComparison.Ordinal) ? "• " + line[2..] : line;
                AppendInline(display);
                _content.AppendText(Environment.NewLine);
            }
        }
        _content.SelectionStart = 0;
        _content.ScrollToCaret();
    }

    private void AppendInline(string text)
    {
        for (int i = 0; i < text.Length;)
        {
            bool bold = text.AsSpan(i).StartsWith("**", StringComparison.Ordinal);
            bool code = text[i] == '`';
            if (!bold && !code)
            {
                int nextBold = text.IndexOf("**", i, StringComparison.Ordinal);
                int nextCode = text.IndexOf('`', i);
                int next = new[] { nextBold, nextCode }.Where(n => n >= 0).DefaultIfEmpty(text.Length).Min();
                _content.SelectionFont = _content.Font;
                _content.AppendText(text[i..next]);
                i = next;
                continue;
            }

            int marker = bold ? 2 : 1;
            int end = bold ? text.IndexOf("**", i + marker, StringComparison.Ordinal) : text.IndexOf('`', i + marker);
            if (end < 0) { _content.AppendText(text[i..]); return; }
            _content.SelectionFont = code
                ? new Font(FontFamily.GenericMonospace, _content.Font.Size)
                : new Font(_content.Font, FontStyle.Bold);
            _content.AppendText(text[(i + marker)..end]);
            _content.SelectionFont = _content.Font;
            i = end + marker;
        }
    }
}
