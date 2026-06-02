using PasswordResetSimulator.Models;
using PasswordResetSimulator.Security;
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

        PasswordValidator validator = new();
        BruteForceGenerator generator = new();

        string foundPassword = "Not Found";

        foreach (string candidate in generator.Generate(6))
        {
            if (validator.IsMatch(candidate, hash))
            {
                foundPassword = candidate;
                break;
            }
        }

        Greeting =
            $"Original Password: {password}\n\n" +
            $"Found Password: {foundPassword}\n\n" +
            $"Hash:\n{hash}";
    }
}