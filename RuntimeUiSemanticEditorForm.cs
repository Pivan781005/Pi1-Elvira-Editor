namespace Pi1ElviraEditor;

/// <summary>Modal semantic editor for one structured Runtime UI record.
/// Subfields are shown separately (original vs project); the message field
/// and hotspot button labels are editable (buttons as fixed-width slots:
/// text may change, hotspot geometry must not), every frozen component stays
/// visibly read-only with its reason. Raw CR separators, padding and NUL are
/// never exposed: the dialog composes the frozen full record through the
/// audited contracts on OK. Used by the Runtime UI grid double-click/Edit path.</summary>
internal sealed class RuntimeUiSemanticEditorForm : Form
{
    private readonly RuntimeUiLogicalRecordId _recordId;
    private readonly VariantRuntimeKind _runtime;
    private readonly Dictionary<string, TextBox> _editBoxes = new(StringComparer.Ordinal);
    private readonly string _mainKey;
    private readonly Label _validationLabel = new();
    private readonly Button _okButton = new();
    private readonly Button _cancelButton = new();

    public string? ComposedFullRecord { get; private set; }

    internal string EditTextForTest { get => _editBoxes[_mainKey].Text; set => _editBoxes[_mainKey].Text = value; }
    internal string ValidationForTest => _validationLabel.Text;

    internal void SetSemanticFieldForTest(string key, string text) => _editBoxes[key].Text = text;

    internal bool SubmitForTest()
    {
        OnOk();
        return DialogResult == DialogResult.OK;
    }

    public RuntimeUiSemanticEditorForm(
        string recordDisplayName,
        RuntimeUiLogicalRecordId recordId,
        VariantRuntimeKind runtime,
        IReadOnlyList<RunEgaUiSemanticField> originalFields,
        IReadOnlyList<RunEgaUiSemanticField> currentFields)
    {
        _recordId = recordId;
        _runtime = runtime;
        _mainKey = EditableKeyFor(recordId, runtime);
        Text = string.Format(UiText.Get("RuntimeUi.SemanticEditor.Title"), recordDisplayName);
        StartPosition = FormStartPosition.CenterParent;
        Size = new Size(640, 200 + Math.Max(originalFields.Count, 1) * 64);
        MinimumSize = new Size(560, 320);
        FormBorderStyle = FormBorderStyle.Sizable;

        var heading = new Label
        {
            Dock = DockStyle.Top,
            Height = 28,
            TextAlign = ContentAlignment.MiddleLeft,
            Text = string.Format(UiText.Get("RuntimeUi.SemanticEditor.Title"), recordDisplayName)
        };

        var table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            AutoScroll = true
        };
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        table.Controls.Add(new Label { Text = string.Empty }, 0, 0);
        table.Controls.Add(new Label { Text = UiText.Get("RuntimeUi.SemanticEditor.OriginalHeader"), AutoSize = true }, 1, 0);
        table.Controls.Add(new Label { Text = UiText.Get("RuntimeUi.SemanticEditor.ProjectHeader"), AutoSize = true }, 2, 0);

        var originalByKey = originalFields.ToDictionary(field => field.Key, StringComparer.Ordinal);
        int row = 1;
        foreach (RunEgaUiSemanticField current in currentFields)
        {
            string original = originalByKey.TryGetValue(current.Key, out RunEgaUiSemanticField? match) ? match.Value : string.Empty;
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var name = new Label { Text = current.Label, AutoSize = true, Anchor = AnchorStyles.Left };
            name.Margin = new Padding(3, 8, 3, 3);
            var originalBox = new TextBox
            {
                Text = original,
                ReadOnly = true,
                BackColor = SystemColors.Control,
                Dock = DockStyle.Fill,
                Multiline = true
            };
            table.Controls.Add(name, 0, row);
            table.Controls.Add(originalBox, 1, row);
            if (current.Role is RunEgaUiSemanticRole.EditableSafe or RunEgaUiSemanticRole.EditableFixedWidth)
            {
                var edit = new TextBox
                {
                    Text = current.Value,
                    Dock = DockStyle.Fill,
                    Multiline = true
                };
                if (!string.IsNullOrEmpty(current.ReadOnlyReason))
                {
                    var hint = new ToolTip();
                    hint.SetToolTip(edit, current.ReadOnlyReason);
                    hint.SetToolTip(name, current.ReadOnlyReason);
                }
                _editBoxes[current.Key] = edit;
                table.Controls.Add(edit, 2, row);
            }
            else
            {
                var frozen = new TextBox
                {
                    Text = current.Value,
                    ReadOnly = true,
                    BackColor = SystemColors.Control,
                    Dock = DockStyle.Fill,
                    Multiline = true
                };
                var tip = new ToolTip();
                tip.SetToolTip(frozen, current.ReadOnlyReason);
                tip.SetToolTip(name, current.ReadOnlyReason);
                table.Controls.Add(frozen, 2, row);
            }
            row++;
        }

        _validationLabel.Dock = DockStyle.Bottom;
        _validationLabel.Height = 26;
        _validationLabel.ForeColor = Color.DarkRed;

        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 40, FlowDirection = FlowDirection.RightToLeft };
        _okButton.Text = UiText.Get("Apply");
        _okButton.DialogResult = DialogResult.None;
        _okButton.Click += (_, _) => OnOk();
        _cancelButton.Text = UiText.Get("Cancel");
        _cancelButton.DialogResult = DialogResult.Cancel;
        buttons.Controls.Add(_cancelButton);
        buttons.Controls.Add(_okButton);
        AcceptButton = _okButton;
        CancelButton = _cancelButton;

        Controls.Add(table);
        Controls.Add(_validationLabel);
        Controls.Add(buttons);
        Controls.Add(heading);
    }

    private static string EditableKeyFor(RuntimeUiLogicalRecordId id, VariantRuntimeKind runtime)
    {
        if (runtime == VariantRuntimeKind.Elvira1Vga && id == RuntimeUiLogicalRecordId.PauseMenu) return "title";
        return id switch
        {
            RuntimeUiLogicalRecordId.PauseMenu => "title",
            RuntimeUiLogicalRecordId.ConfirmGeneric => "prompt",
            RuntimeUiLogicalRecordId.SavePrompt => "message",
            RuntimeUiLogicalRecordId.SaveOverwrite => "message",
            _ => "message"
        };
    }

    private void OnOk()
    {
        string detail;
        if (_runtime == VariantRuntimeKind.Elvira1Vga && RunVgaVariableUiService.IsVariableRecord(_recordId))
        {
            var edits = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach ((string key, TextBox box) in _editBoxes)
                edits[key] = box.Text;
            if (!RunVgaVariableUiService.TryCompose(_recordId, edits, out string? full, out _, out detail))
            {
                _validationLabel.Text = detail;
                return;
            }
            ComposedFullRecord = full;
        }
        else
        {
            var buttonEdits = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach ((string key, TextBox box) in _editBoxes)
                if (!key.Equals(_mainKey, StringComparison.Ordinal))
                    buttonEdits[key] = box.Text;
            if (!RunEgaUiService.TryComposeRecord(_recordId, _editBoxes[_mainKey].Text, buttonEdits, out string? full, out _, out detail))
            {
                _validationLabel.Text = detail;
                return;
            }
            ComposedFullRecord = full;
        }
        DialogResult = DialogResult.OK;
        Close();
    }

    /// <summary>Headless composition used by tests and by the grid edit path
    /// contract: validates one edited semantic field (plus optional hotspot
    /// button labels) into its frozen record.</summary>
    internal static bool TryComposeSemanticField(VariantRuntimeKind runtime, RuntimeUiLogicalRecordId id, string? editedField, out string fullRecord, out string detail)
        => TryComposeSemanticField(runtime, id, editedField, null, out fullRecord, out detail);

    internal static bool TryComposeSemanticField(VariantRuntimeKind runtime, RuntimeUiLogicalRecordId id, string? editedField, IReadOnlyDictionary<string, string>? buttons, out string fullRecord, out string detail)
    {
        fullRecord = string.Empty;
        detail = string.Empty;
        if (runtime == VariantRuntimeKind.Elvira1Vga && RunVgaVariableUiService.IsVariableRecord(id))
        {
            string mainKey = id switch
            {
                RuntimeUiLogicalRecordId.PauseMenu => "title",
                RuntimeUiLogicalRecordId.ConfirmGeneric => "prompt",
                RuntimeUiLogicalRecordId.SaveOverwrite => "message",
                _ => throw new ArgumentOutOfRangeException(nameof(id))
            };
            var edits = new Dictionary<string, string>(StringComparer.Ordinal) { [mainKey] = editedField ?? string.Empty };
            if (buttons is not null)
                foreach ((string key, string value) in buttons)
                    edits[key] = value;
            else
                foreach ((string key, string value) in VgaEnglishDefaults(id))
                    edits[key] = value;
            return RunVgaVariableUiService.TryCompose(id, edits, out fullRecord, out _, out detail);
        }
        return RunEgaUiService.TryComposeRecord(id, editedField, buttons, out fullRecord, out _, out detail);
    }

    internal static IReadOnlyDictionary<string, string> VgaEnglishDefaults(RuntimeUiLogicalRecordId id) => id switch
    {
        RuntimeUiLogicalRecordId.PauseMenu => new Dictionary<string, string>(StringComparer.Ordinal) { ["continue"] = "Continue", ["quit"] = "Quit" },
        RuntimeUiLogicalRecordId.ConfirmGeneric => new Dictionary<string, string>(StringComparer.Ordinal) { ["yes"] = "Yes", ["no"] = "No" },
        RuntimeUiLogicalRecordId.SaveOverwrite => new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["question"] = "Overwrite it ?",
            ["yes"] = "Yes",
            ["no"] = "No"
        },
        _ => throw new ArgumentOutOfRangeException(nameof(id))
    };
}
