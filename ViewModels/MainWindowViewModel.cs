using System;
using PasswordResetSimulator.Models;
using PasswordResetSimulator.BruteForce;

namespace PasswordResetSimulator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public string Greeting { get; }

    public MainWindowViewModel()
    {
        PasswordManager manager = new();

        string password =
            manager.GeneratePassword();

        string hash =
            manager.GenerateHash(password);

        BruteForceEngine engine = new();

        string? foundPassword =
            engine.FindPassword(
                hash,
                6,
                out TimeSpan elapsed);

        Greeting =
            $"Original Password: {password}\n\n" +
            $"Found Password: {foundPassword}\n\n" +
            $"Elapsed Time: {elapsed.TotalSeconds:F2} sec";
    }
}