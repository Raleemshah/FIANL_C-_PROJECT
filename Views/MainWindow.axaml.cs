using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using PasswordResetSimulator.Models;
using PasswordResetSimulator.BruteForce;

namespace PasswordResetSimulator.Views;

public partial class MainWindow : Window
{
    private readonly PasswordManager _manager = new();

    private string _password = "";
    private string _hash = "";

    public MainWindow()
    {
        InitializeComponent();
    }

    private void GeneratePassword_Click(
        object? sender,
        RoutedEventArgs e)
    {
        _password = _manager.GeneratePassword();

        _hash = _manager.GenerateHash(_password);

        OriginalPasswordText.Text =
            $"Original Password: {_password}";

        FoundPasswordText.Text =
            "Found Password: -";

        ElapsedTimeText.Text =
            "Elapsed Time: -";

        SingleThreadText.Text =
            "Single Thread: -";

        MultiThreadText.Text =
            "Multi Thread: Pending";

        SpeedupText.Text =
            "Speedup: Pending";

        AttackProgress.Value = 0;
    }

    private void StartAttack_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_hash))
        {
            FoundPasswordText.Text =
                "Generate a password first";
            return;
        }

        AttackProgress.Value = 25;

        BruteForceEngine engine = new();

        string? foundPassword =
            engine.FindPassword(
                _hash,
                6,
                out TimeSpan elapsed);

        AttackProgress.Value = 100;

        FoundPasswordText.Text =
            $"Found Password: {foundPassword}";

        ElapsedTimeText.Text =
            $"Elapsed Time: {elapsed.TotalSeconds:F2} sec";

        SingleThreadText.Text =
            $"Single Thread: {elapsed.TotalSeconds:F2} sec";
    }

    private void StopAttack_Click(
        object? sender,
        RoutedEventArgs e)
    {
        ElapsedTimeText.Text += " (Stopped)";
    }
}