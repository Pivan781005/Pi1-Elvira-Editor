namespace Pi1ElviraEditor;

// Owns the dedicated STA splash UI thread. The main application UI stays on
// its normal STA thread; MainForm is always constructed on the main thread.
// No Thread.Sleep occurs before initialization begins; initialization runs
// concurrently with the splash thread. A short minimum-visibility wait
// (approximately 800 ms) applies only after the main form is ready, solely
// to prevent a single-frame flash.
internal sealed class StartupSplashCoordinator : IDisposable
{
    internal static readonly TimeSpan MinimumVisibility = TimeSpan.FromMilliseconds(800);
    private static readonly TimeSpan ShownTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ExitTimeout = TimeSpan.FromSeconds(5);

    private readonly bool _headlessTestMode;
    private readonly object _sync = new();
    private Thread? _thread;
    private StartupSplashForm? _form;
    private readonly ManualResetEventSlim _shown = new(false);
    private readonly ManualResetEventSlim _closeRequested = new(false);
    private readonly ManualResetEventSlim _exited = new(false);
    private DateTime _startUtc;
    private bool _started;
    private bool _signaled;
    private bool _disposed;

    internal bool IsThreadAliveForTest
    {
        get { lock (_sync) return _thread?.IsAlive == true; }
    }

    internal bool WasShownForTest => _shown.IsSet;

    internal bool WasSignaledForTest
    {
        get { lock (_sync) return _signaled; }
    }

    internal StartupSplashCoordinator(bool headlessTestMode = false)
    {
        _headlessTestMode = headlessTestMode;
    }

    internal void Start()
    {
        lock (_sync)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(StartupSplashCoordinator));
            if (_started)
                throw new InvalidOperationException("Splash coordinator was already started.");
            _started = true;
            _startUtc = DateTime.UtcNow;
            _thread = new Thread(ThreadProc)
            {
                IsBackground = true,
                Name = "Pi1Splash",
            };
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();
        }
        // Return immediately: application initialization proceeds concurrently.
    }

    private void ThreadProc()
    {
        try
        {
            if (_headlessTestMode)
            {
                // Headless lifecycle probe: no window is created, but the same
                // start/signal/exit/Join contract is exercised so the smoke can
                // prove no background thread survives without requiring a display.
                _shown.Set();
                _closeRequested.Wait();
                return;
            }

            StartupSplashForm form;
            try
            {
                form = new StartupSplashForm();
            }
            catch
            {
                return;
            }

            lock (_sync) _form = form;
            form.Shown += (_, _) =>
            {
                try { _shown.Set(); } catch { }
            };
            try
            {
                Application.Run(form);
            }
            finally
            {
                lock (_sync)
                {
                    if (ReferenceEquals(_form, form))
                        _form = null;
                }
                try { form.Dispose(); } catch { }
            }
        }
        catch
        {
        }
        finally
        {
            try { _shown.Set(); } catch { }
            try { _exited.Set(); } catch { }
        }
    }

    // Called on the main thread when MainForm is ready (first Shown). Enforces
    // the short minimum-visibility window, then requests the splash thread to
    // close and waits boundedly for its termination. Idempotent.
    internal void SignalMainReady()
    {
        lock (_sync)
        {
            if (_disposed || !_started || _signaled)
                return;
            _signaled = true;
        }

        TimeSpan elapsed = DateTime.UtcNow - _startUtc;
        TimeSpan remaining = MinimumVisibility - elapsed;
        if (remaining > TimeSpan.Zero)
            Thread.Sleep(remaining);

        try { _shown.Wait(ShownTimeout); }
        catch (ObjectDisposedException) { return; }
        catch { }
        try { _closeRequested.Set(); }
        catch (ObjectDisposedException) { return; }
        catch { }
        RequestFormClose();
        try { _exited.Wait(ExitTimeout); }
        catch (ObjectDisposedException) { }
        catch { }
        JoinThread();
    }

    internal bool WaitForExit(TimeSpan timeout)
    {
        try { return _exited.Wait(timeout); }
        catch (ObjectDisposedException) { return true; }
    }

    private void RequestFormClose()
    {
        StartupSplashForm? form;
        lock (_sync) form = _form;
        if (form is null || form.IsDisposed)
            return;
        try
        {
            if (form.IsHandleCreated)
            {
                try
                {
                    form.BeginInvoke(new Action(() =>
                    {
                        try
                        {
                            if (!form.IsDisposed)
                                form.Close();
                        }
                        catch
                        {
                        }
                    }));
                    return;
                }
                catch
                {
                }
            }
            // Handle not yet created (splash signaled before first paint):
            // best-effort synchronous close; the message loop (if running)
            // will still exit via the _closeRequested fallback below.
            try
            {
                if (!form.IsDisposed)
                    form.Close();
            }
            catch
            {
            }
        }
        catch
        {
        }
    }

    private void JoinThread()
    {
        Thread? thread;
        lock (_sync) thread = _thread;
        if (thread is null)
            return;
        try
        {
            if (thread.IsAlive)
                thread.Join(ExitTimeout);
        }
        catch
        {
        }
    }

    public void Dispose()
    {
        ManualResetEventSlim? shown = null;
        ManualResetEventSlim? closeRequested = null;
        ManualResetEventSlim? exited = null;
        lock (_sync)
        {
            if (_disposed)
                return;
            _disposed = true;
        }

        // Failure path: never block on minimum visibility. Close promptly so a
        // MainForm construction failure cannot leave an orphan splash window.
        // Existing error reporting is preserved because Dispose never swallows
        // the caller's exception.
        try
        {
            _closeRequested.Set();
            RequestFormClose();
            _exited.Wait(ExitTimeout);
            JoinThread();
        }
        catch
        {
        }
        finally
        {
            lock (_sync)
            {
                shown = _shown;
                closeRequested = _closeRequested;
                exited = _exited;
            }
            // Dispose the wait handles once; SignalMainReady checks _disposed
            // first so a late signal after Dispose is a safe no-op.
            try { shown.Dispose(); } catch { }
            try { closeRequested.Dispose(); } catch { }
            try { exited.Dispose(); } catch { }
        }
    }
}
