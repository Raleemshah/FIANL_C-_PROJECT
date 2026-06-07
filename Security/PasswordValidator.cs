//passwordvalidator.cs
namespace PasswordResetSimulator.Security;

public class PasswordValidator
{
    public bool IsMatch(
        string candidate,
        string targetHash)
    {
        string hash =
            PasswordHasher.ComputeHash(candidate);

        return hash == targetHash;
    }
}