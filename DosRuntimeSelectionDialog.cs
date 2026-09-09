namespace Pi1ElviraEditor;

/// <summary>Modal DOS runtime host picker for Mods &amp; Launcher. It only
/// presents already-probed candidates plus an explicit Browse path; an
/// unrecognized executable never becomes selectable.</summary>
internal sealed class DosRuntimeSelectionDialog : Form
{
    private readonly DataGridView _grid = new();
    private readonly Button _btnRefresh = new();
    private readonly Button _btnBrowse = new();
    private readonly Button _btnOk = new();
    private readonly Button _btnCancel = new();
    private readonly Func<IReadOnlyList<DosRuntimeCandidate>> _refresh;
    private readonly Func<string, DosRuntimeCandidate> _probePath;

    internal DosRuntimeCandidate? Selected { get; private set; }

    internal DosRuntimeSelectionDialog(
        IReadOnlyList<DosRuntimeCandidate> candidates,
        DosRuntimeCandidate? selected,
        Func<IReadOnlyList<DosRuntimeCandidate>> refresh,
        Func<string, DosRuntimeCandidate> probePath)
    {
        _refresh = refresh ?? throw new ArgumentNullException(nameof(refresh));
        _probePath = probePath ?? throw new ArgumentNullException(nameof(probePath));
        Text = UiText.Get("DosRuntime.DialogTitle");
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 420);
        Size = new Size(820, 460);

        _grid.Dock = DockStyle.Fill;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.AllowUserToResizeRows = false;
        _grid.RowHeadersVisible = false;
        _grid.ReadOnly = true;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
        _grid.BackgroundColor = SystemColors.Window;
        _grid.BorderStyle = BorderStyle.FixedSingle;
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Host", HeaderText = UiText.Get("DosRuntime.ColHost"), Width = 150 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Version", HeaderText = UiText.Get("DosRuntime.ColVersion"), Width = 130 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Source", HeaderText = UiText.Get("DosRuntime.ColSource"), Width = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Path", HeaderText = UiText.Get("DosRuntime.ColPath"), Width = 260 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = UiText.Get("DosRuntime.ColStatus"), Width = 110 });
        _grid.CellDoubleClick += (_, e) =>
        {
            if (e.RowIndex >= 0) AcceptSelection();
        };
        _grid.SelectionChanged += (_, _) => UpdateOkState();

        var bottom = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(8)
        };
        _btnOk.Text = UiText.Get("DosRuntime.UseSelected");
        _btnOk.Size = new Size(130, 28);
        _btnOk.DialogResult = DialogResult.OK;
        _btnOk.Click += (_, _) => AcceptSelection();
        _btnCancel.Text = UiText.Get("Cancel");
        _btnCancel.Size = new Size(90, 28);
        _btnCancel.DialogResult = DialogResult.Cancel;
        _btnBrowse.Text = UiText.Get("DosRuntime.Browse");
        _btnBrowse.Size = new Size(110, 28);
        _btnBrowse.Click += (_, _) => BrowseHost();
        _btnRefresh.Text = UiText.Get("DosRuntime.Refresh");
        _btnRefresh.Size = new Size(110, 28);
        _btnRefresh.Click += (_, _) => Fill(_refresh());
        bottom.Controls.AddRange(new Control[] { _btnCancel, _btnOk, _btnBrowse, _btnRefresh });

        Controls.Add(_grid);
        Controls.Add(bottom);
        AcceptButton = _btnOk;
        CancelButton = _btnCancel;
        Fill(candidates);
        SelectPath(selected?.ExecutablePath);
    }

    private void Fill(IReadOnlyList<DosRuntimeCandidate> candidates)
    {
        string? keep = (_grid.CurrentRow?.Tag as DosRuntimeCandidate)?.ExecutablePath ?? Selected?.ExecutablePath;
        _grid.Rows.Clear();
        foreach (DosRuntimeCandidate candidate in candidates.OrderBy(item => item.Compatibility != DosRuntimeCompatibility.Compatible).ThenBy(item => item.DisplayName).ThenBy(item => item.ExecutablePath, StringComparer.OrdinalIgnoreCase))
        {
            int index = _grid.Rows.Add(candidate.DisplayName, candidate.Version,
                UiText.Get("DosRuntime.Source." + candidate.Source), candidate.ExecutablePath,
                candidate.Compatibility == DosRuntimeCompatibility.Compatible ? UiText.Get("DosRuntime.Ready") : UiText.Get("VariantInvalid"));
            DataGridViewRow row = _grid.Rows[index];
            row.Tag = candidate;
            if (candidate.Compatibility != DosRuntimeCompatibility.Compatible)
                row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        }
        if (keep is not null) SelectPath(keep);
        UpdateOkState();
    }

    private void SelectPath(string? executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath)) return;
        foreach (DataGridViewRow row in _grid.Rows)
        {
            if ((row.Tag as DosRuntimeCandidate)?.ExecutablePath.Equals(executablePath, StringComparison.OrdinalIgnoreCase) == true)
            {
                row.Selected = true;
                _grid.CurrentCell = row.Cells[0];
                return;
            }
        }
    }

    private void UpdateOkState()
    {
        _btnOk.Enabled = _grid.CurrentRow?.Tag is DosRuntimeCandidate candidate && candidate.IsRunnable;
    }

    private void AcceptSelection()
    {
        if (_grid.CurrentRow?.Tag is not DosRuntimeCandidate candidate || !candidate.IsRunnable)
        {
            DialogResult = DialogResult.None;
            return;
        }
        Selected = candidate;
    }

    private void BrowseHost()
    {
        using var dialog = new OpenFileDialog
        {
            Title = UiText.Get("DosRuntime.Browse"),
            Filter = "DOS runtime host (*.exe)|*.exe",
            CheckFileExists = true
        };
        if (dialog.ShowDialog(this) != DialogResult.OK) return;
        DosRuntimeCandidate probed;
        try { probed = _probePath(dialog.FileName); }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, UiText.Get("DosRuntime.DialogTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!probed.IsRunnable)
        {
            MessageBox.Show(this, UiText.Get("DosRuntime.HostInvalid"), UiText.Get("DosRuntime.DialogTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var merged = _grid.Rows.Cast<DataGridViewRow>()
            .Select(row => row.Tag as DosRuntimeCandidate)
            .Where(item => item is not null)
            .Cast<DosRuntimeCandidate>()
            .Where(item => !item.ExecutablePath.Equals(probed.ExecutablePath, StringComparison.OrdinalIgnoreCase))
            .Append(probed)
            .ToArray();
        Fill(merged);
        SelectPath(probed.ExecutablePath);
    }
}
