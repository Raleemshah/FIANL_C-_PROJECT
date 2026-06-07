// MainWindow.axaml.cs
// Runs single-thread and multi-thread attacks independently so their
// times are comparable.  A live attempt counter drives the progress bar.

using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using PasswordResetSimulator.BruteForce;
using PasswordResetSimulator.Models;

namespace PasswordResetSimulator.Views;

public partial class MainWindow : Window
{
    private readonly PasswordManager _manager = new();

    private string _password = "";
    private string _hash     = "";

    private CancellationTokenSource? _cts;
    private bool _attackRunning;

    // Rough upper-bound of total candidates up to length 6 (26^1+…+26^6).
    private const long TotalCandidates = 26 + 676 + 17576 + 456976 + 11881376 + 308915776;

    public MainWindow() => InitializeComponent();

    // ------------------------------------------------------------------ //
    //  Generate password                                                   //
    // ------------------------------------------------------------------ //

    private void GeneratePassword_Click(object? sender, RoutedEventArgs e)
    {
        if (_attackRunning) return;

        _password = _manager.GeneratePassword();
        _hash     = _manager.GenerateHash(_password);

        OriginalPasswordText.Text = $"Original Password: {_password}";
        FoundPasswordText.Text    = "Found Password: -";
        ElapsedTimeText.Text      = "Elapsed Time: -";
        SingleThreadText.Text     = "Single Thread: -";
        MultiThreadText.Text      = "Multi Thread: -";
        SpeedupText.Text          = "Speedup: -";
        AttackProgress.Value      = 0;
    }

    // ------------------------------------------------------------------ //
    //  Start attack                                                        //
    // ------------------------------------------------------------------ //

    private async void StartAttack_Click(object? sender, RoutedEventArgs e)
    {
        if (_attackRunning) return;

        if (string.IsNullOrWhiteSpace(_hash))
        {
            FoundPasswordText.Text = "Generate a password first.";
            return;
        }

        _attackRunning        = true;
        _cts                  = new CancellationTokenSource();
        AttackProgress.Value  = 0;

        try
        {
            // ── 1. Single-threaded run ──────────────────────────────── //
            SingleThreadText.Text = "Single Thread: running…";

            TimeSpan singleElapsed = default;
            string?  foundSingle   = null;

            await Task.Run(() =>
            {
                BruteForceEngine engine = new();
                foundSingle = engine.FindPassword(
                    _hash, 6, out singleElapsed, _cts.Token);
            }, _cts.Token);

            if (_cts.IsCancellationRequested)
            {
                ShowCancelled();
                return;
            }

            SingleThreadText.Text = $"Single Thread: {singleElapsed.TotalSeconds:F3} sec";

            // ── 2. Multi-threaded run ───────────────────────────────── //
            MultiThreadText.Text = "Multi Thread: running…";

            MultiThreadBruteForceEngine multiEngine = new();

            // Live progress loop: polls AttemptCount while multi runs.
            using CancellationTokenSource progressCts = new();
            Task progressTask = RunProgressAsync(multiEngine, progressCts.Token);

            BruteForceResult multiResult = await Task.Run(() =>
                multiEngine.FindPassword(_hash, _cts.Token), _cts.Token);

            progressCts.Cancel();
            try { await progressTask; } catch { /* expected */ }

            if (_cts.IsCancellationRequested)
            {
                ShowCancelled();
                return;
            }

            // ── 3. Display results ──────────────────────────────────── //
            AttackProgress.Value   = 100;
            FoundPasswordText.Text = $"Found Password: {multiResult.Password ?? foundSingle}";
            ElapsedTimeText.Text   = $"Elapsed Time (multi): {multiResult.ElapsedSeconds:F3} sec";
            MultiThreadText.Text   = $"Multi Thread: {multiResult.ElapsedSeconds:F3} sec";

            double speedup = multiResult.ElapsedSeconds > 0
                ? singleElapsed.TotalSeconds / multiResult.ElapsedSeconds
                : 0;

            SpeedupText.Text = $"Speedup: {speedup:F2}x  " +
                               $"({Environment.ProcessorCount - 1} worker threads)";
        }
        catch (OperationCanceledException)
        {
            ShowCancelled();
        }
        finally
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts           = null;
            _attackRunning = false;
        }
    }

    // ------------------------------------------------------------------ //
    //  Live progress bar                                                   //
    // ------------------------------------------------------------------ //

    private async Task RunProgressAsync(
        MultiThreadBruteForceEngine engine,
        CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                long attempts = engine.AttemptCount;
                double pct    = Math.Min(99,
                    attempts / (double)TotalCandidates * 100.0);

                await Dispatcher.UIThread.InvokeAsync(() =>
                    AttackProgress.Value = pct);

                await Task.Delay(100, token);
            }
        }
        catch (OperationCanceledException) { }
    }

    // ------------------------------------------------------------------ //
    //  Stop                                                                //
    // ------------------------------------------------------------------ //

    private void StopAttack_Click(object? sender, RoutedEventArgs e)
    {
        _cts?.Cancel();
        ShowCancelled();
    }

    private void ShowCancelled()
    {
        FoundPasswordText.Text = "Attack Cancelled";
        MultiThreadText.Text   = "Multi Thread: Cancelled";
        SpeedupText.Text       = "Speedup: Cancelled";
        AttackProgress.Value   = 0;
    }
}