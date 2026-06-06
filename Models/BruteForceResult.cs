namespace PasswordResetSimulator.Models;

public class BruteForceResult
{
    public string? Password { get; set; }

    public double ElapsedSeconds { get; set; }

    public long Attempts { get; set; }

    public bool Found { get; set; }
}