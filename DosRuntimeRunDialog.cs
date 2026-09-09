namespace Pi1ElviraEditor;

/// <summary>R9F V8.6i effective Run candidate set. The bounded automatic
/// discovery list never contains a valid remembered/manual preferred host that
/// lives outside discovery, so the Run dialog merges one authoritative list:
/// all automatic candidates plus every still-runnable sticky candidate
/// (preferred, remembered, per-launch Browse) whose path is ABSENT from the
/// current automatic set. De-duplicated by normalized executable path
/// (case-insensitive). Fresh automatic results are authoritative for their own
/// paths: a sticky row never overwrites a same-path automatic row, so an
/// explicit Refresh that re-probes a path to Incompatible stays truthful
/// instead of showing stale probe-time Ready. Sticky rows are additionally
/// filtered live by IsRunnable (file must exist); disappeared paths are
/// dropped, never executed. Building the list never probes and never persists.</summary>
internal static class DosRuntimeRunCandidates
{
    internal static IReadOnlyList<DosRuntimeCandidate> BuildEffective(
        IReadOnlyList<DosRuntimeCandidate> automatic,
        IEnumerable<DosRuntimeCandidate?> sticky)
    {
        var merged = new Dictionary<string, DosRuntimeCandidate>(StringComparer.OrdinalIgnoreCase);
        foreach (DosRuntimeCandidate candidate in automatic ?? [])
        {
            if (candidate is null) continue;
            string key = Normalize(candidate.ExecutablePath);
            if (!merged.ContainsKey(key)) merged[key] = candidate;
        }
        foreach (DosRuntimeCandidate? candidate in sticky ?? [])
        {
            if (candidate is null || !candidate.IsRunnable) continue;
            string key = Normalize(candidate.ExecutablePath);
            if (!merged.ContainsKey(key)) merged[key] = candidate;
        }
        return merged.Values.ToArray();
    }

    internal static string Normalize(string? executablePath)
    {
        if (string.IsNullOrWhiteSpace(executablePath)) return string.Empty;
        try { return Path.GetFullPath(executablePath).ToUpperInvariant(); }
        catch { return executablePath.ToUpperInvariant(); }
    }
}

/// <summary>R9F V8.6h explicit per-launch DOS runtime selection. The outer
/// Run... action never executes directly: this modal dialog shows the
/// authoritative target plus detected compatible hosts, preselects the
/// preferred/remembered host without auto-running, and executes only after
/// the user explicitly clicks Run inside the dialog. Selection is per-launch;
/// it never overwrites DosRuntimeSettingsStore (Change... owns preference).
/// Host validation is never duplicated here: candidates come from the
/// session-validated discovery/probe path and Run enables only for runnable
/// DosRuntimeCandidate rows.</summary>
internal sealed class DosRuntimeRunDialog : Form
{
    private readonly DataGridView _grid = new();
    private readonly Label _lblTarget = new();
    private readonly Label _lblEmpty = new();
    private readonly Label _lblDetail = new();
    private readonly Button _btnRefresh = new();
    private readonly Button _btnBrowse = new();
    private readonly Button _btnRun = new();
    private readonly Button _btnCancel = new();
    private readonly Func<IReadOnlyList<DosRuntimeCandidate>> _refresh;
    private readonly Func<string, DosRuntimeCandidate> _probePath;
    /// <summary>R9F V8.6i sticky session-valid rows (preferred/remembered/
    /// per-launch Browse) preserved across explicit Refresh without re-probe.
    /// Disappeared paths are filtered live on every Fill.</summary>
    private readonly List<DosRuntimeCandidate> _sticky = [];

    internal DosRuntimeCandidate? Selected { get; private set; }

    internal IReadOnlyList<DosRuntimeCandidate> CurrentCandidates =>
        _grid.Rows.Cast<DataGridViewRow>()
            .Select(row => row.Tag as DosRuntimeCandidate)
            .Where(item => item is not null)
            .Cast<DosRuntimeCandidate>()
            .ToArray();

    /// <summary>R9F V8.6m minimal headless seam: the actually selected/current
    /// grid candidate backing Run/detail state (internal for tests only).</summary>
    internal DosRuntimeCandidate? CurrentForTest => _grid.CurrentRow?.Tag as DosRuntimeCandidate;

    /// <summary>R9F V8.6m minimal headless seam: the actual inner-Run button
    /// state (internal for tests only).</summary>
    internal bool RunEnabledForTest => _btnRun.Enabled;

    /// <summary>R9F V8.6m minimal headless seam: drives the real production
    /// SelectPath (including immediate state sync) without synthetic events.</summary>
    internal void SelectForTest(string? executablePath) => SelectPath(executablePath);

    /// <summary>R9F V8.6m minimal headless seam: drives the real production
    /// Refresh path (fresh automatic merge + preserved selection + sync).</summary>
    internal void RefreshForTest() => RefreshAutomatic();

    internal DosRuntimeRunDialog(
        VariantLaunchTarget target,
        IReadOnlyList<DosRuntimeCandidate> candidates,
        DosRuntimeCandidate? preferred,
        Func<IReadOnlyList<DosRuntimeCandidate>> refresh,
        Func<string, DosRuntimeCandidate> probePath,
        IEnumerable<DosRuntimeCandidate?>? stickyExtras = null)
    {
        ArgumentNullException.ThrowIfNull(target);
        _refresh = refresh ?? throw new ArgumentNullException(nameof(refresh));
        _probePath = probePath ?? throw new ArgumentNullException(nameof(probePath));
        Text = UiText.Get("DosRuntime.RunDialogTitle");
        try { Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath); } catch { }
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(780, 480);
        Size = new Size(860, 520);

        string dosCommand = DosCommandPreview(target);
        _lblTarget.Dock = DockStyle.Top;
        _lblTarget.Height = 96;
        _lblTarget.Padding = new Padding(10, 8, 10, 4);
        _lblTarget.Text =
            UiText.Get("DosRuntime.RunDialogGame") + " " + target.GameId + "\n" +
            UiText.Get("DosRuntime.RunDialogRuntime") + " " + target.VariantId + "\n" +
            UiText.Get("DosRuntime.RunDialogEdition") + " " + EditionDisplay(target) + "\n" +
            UiText.Get("DosRuntime.RunDialogDirectory") + " " + target.WorkingDirectory + "\n" +
            UiText.Get("DosRuntime.RunDialogCommand") + " " + dosCommand;

        _lblEmpty.Dock = DockStyle.Top;
        _lblEmpty.Height = 24;
        _lblEmpty.Padding = new Padding(10, 0, 10, 0);
        _lblEmpty.ForeColor = SystemColors.GrayText;
        _lblEmpty.Visible = false;
        _lblEmpty.Text = UiText.Get("DosRuntime.RunDialogNoHost");

        // R9F V8.6j Invalid-host diagnostics: read-only reason for the selected
        // row (plus per-row ToolTipText). No modal popup. Status column stays
        // concise (Ready / Invalid) while the underlying Detail is inspectable.
        _lblDetail.Dock = DockStyle.Bottom;
        _lblDetail.Height = 26;
        _lblDetail.Padding = new Padding(10, 4, 10, 4);
        _lblDetail.ForeColor = SystemColors.GrayText;
        _lblDetail.AutoEllipsis = true;

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
        _grid.SelectionChanged += (_, _) => UpdateRunState();

        var bottom = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 44,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(8)
        };
        _btnRun.Text = UiText.Get("DosRuntime.RunDialogRun");
        _btnRun.Size = new Size(110, 28);
        _btnRun.DialogResult = DialogResult.OK;
        _btnRun.Click += (_, _) => AcceptSelection();
        _btnCancel.Text = UiText.Get("Cancel");
        _btnCancel.Size = new Size(90, 28);
        _btnCancel.DialogResult = DialogResult.Cancel;
        _btnBrowse.Text = UiText.Get("DosRuntime.Browse");
        _btnBrowse.Size = new Size(110, 28);
        _btnBrowse.Click += (_, _) => BrowseHost();
        _btnRefresh.Text = UiText.Get("DosRuntime.Refresh");
        _btnRefresh.Size = new Size(110, 28);
        _btnRefresh.Click += (_, _) => RefreshAutomatic();
        bottom.Controls.AddRange(new Control[] { _btnCancel, _btnRun, _btnBrowse, _btnRefresh });

        Controls.Add(_grid);
        Controls.Add(_lblEmpty);
        Controls.Add(_lblTarget);
        Controls.Add(_lblDetail);
        Controls.Add(bottom);
        AcceptButton = _btnRun;
        CancelButton = _btnCancel;
        foreach (DosRuntimeCandidate? extra in stickyExtras ?? [])
        {
            if (extra is null || !extra.IsRunnable) continue;
            if (!_sticky.Any(item => DosRuntimeRunCandidates.Normalize(item.ExecutablePath).Equals(DosRuntimeRunCandidates.Normalize(extra.ExecutablePath), StringComparison.OrdinalIgnoreCase)))
                _sticky.Add(extra);
        }
        Fill(DosRuntimeRunCandidates.BuildEffective(candidates, _sticky));
        SelectPath(preferred?.ExecutablePath);
    }

    /// <summary>R9F V8.6i Refresh merges fresh automatic discovery with the
    /// sticky session-valid rows (preferred/remembered/Browse). Still-valid
    /// extras remain present and selected; disappeared paths drop out and can
    /// never execute. Per-launch Browse choices are not persisted.</summary>
    private void RefreshAutomatic()
    {
        IReadOnlyList<DosRuntimeCandidate> fresh;
        try { fresh = _refresh(); }
        catch { return; }
        Fill(DosRuntimeRunCandidates.BuildEffective(fresh, _sticky));
    }

    internal static string DosCommandPreview(VariantLaunchTarget target)
    {
        string exe = Path.GetFileNameWithoutExtension(target.ExecutableFile);
        if (string.IsNullOrWhiteSpace(exe)) exe = target.ExecutableFile;
        return (exe + " " + target.DataFile).Trim();
    }

    private static string EditionDisplay(VariantLaunchTarget target)
    {
        try
        {
            string root = Path.GetFullPath(target.WorkingDirectory);
            string edition = Path.GetFileName(root.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            return string.IsNullOrWhiteSpace(edition) ? target.DataFile : edition;
        }
        catch { return target.DataFile; }
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
            row.Cells[0].ToolTipText = candidate.Detail;
            row.Cells[1].ToolTipText = candidate.Detail;
            row.Cells[2].ToolTipText = candidate.Detail;
            row.Cells[3].ToolTipText = candidate.Detail;
            row.Cells[4].ToolTipText = candidate.Detail;
            if (candidate.Compatibility != DosRuntimeCompatibility.Compatible)
                row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
        }
        _lblEmpty.Visible = !candidates.Any(item => item.Compatibility == DosRuntimeCompatibility.Compatible);
        if (keep is not null) SelectPath(keep);
        UpdateRunState();
    }

    private void SelectPath(string? executablePath)
    {
        // R9F V8.6o fail-closed: programmatic selection is deterministic. A
        // found path becomes current/selected (Ready enables Run, Invalid
        // disables it). A requested path matching NO row must not leave a
        // stale/different row authoritative: explicitly clear current and
        // selection state so UpdateRunState sees no runnable candidate and
        // Run stays disabled until the user explicitly selects a remaining
        // host. A null/empty request selects nothing and preserves existing
        // constructor semantics. (R9F V8.6m: synchronize immediately here,
        // never via SelectionChanged timing, focus, Show/Shown, BeginInvoke,
        // DoEvents, timers, Sleep, or synthetic input.)
        bool found = false;
        if (!string.IsNullOrWhiteSpace(executablePath))
        {
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if ((row.Tag as DosRuntimeCandidate)?.ExecutablePath.Equals(executablePath, StringComparison.OrdinalIgnoreCase) == true)
                {
                    row.Selected = true;
                    _grid.CurrentCell = row.Cells[0];
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                _grid.ClearSelection();
                _grid.CurrentCell = null;
                Selected = null;
            }
        }
        UpdateRunState();
    }

    private void UpdateRunState()
    {
        DosRuntimeCandidate? current = _grid.CurrentRow?.Tag as DosRuntimeCandidate;
        _btnRun.Enabled = IsRunEnabledFor(current);
        _lblDetail.Text = current?.Detail ?? string.Empty;
    }

    /// <summary>R9F V8.6h/j inner-Run gate shared by the dialog and headless
    /// regressions: enabled only for a runnable/compatible candidate.</summary>
    internal static bool IsRunEnabledFor(DosRuntimeCandidate? candidate) =>
        candidate is not null && candidate.IsRunnable;

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
            MessageBox.Show(this, ex.Message, UiText.Get("DosRuntime.RunDialogTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!probed.IsRunnable)
        {
            MessageBox.Show(this, UiText.Get("DosRuntime.HostInvalid"), UiText.Get("DosRuntime.RunDialogTitle"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!_sticky.Any(item => DosRuntimeRunCandidates.Normalize(item.ExecutablePath).Equals(DosRuntimeRunCandidates.Normalize(probed.ExecutablePath), StringComparison.OrdinalIgnoreCase)))
            _sticky.Add(probed);
        // The fresh probe is authoritative for its own path: drop any same-path
        // grid row first so BuildEffective (fresh-automatic-wins) keeps probing truth.
        string probedKey = DosRuntimeRunCandidates.Normalize(probed.ExecutablePath);
        var merged = DosRuntimeRunCandidates.BuildEffective(
            _grid.Rows.Cast<DataGridViewRow>()
                .Select(row => row.Tag as DosRuntimeCandidate)
                .Where(item => item is not null)
                .Cast<DosRuntimeCandidate>()
                .Where(item => !DosRuntimeRunCandidates.Normalize(item.ExecutablePath).Equals(probedKey, StringComparison.OrdinalIgnoreCase))
                .ToArray(),
            [probed]);
        Fill(merged);
        SelectPath(probed.ExecutablePath);
    }
}
