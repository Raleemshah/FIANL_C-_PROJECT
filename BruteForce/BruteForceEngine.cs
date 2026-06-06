using System;
using System.Diagnostics;
using PasswordResetSimulator.Security;

namespace PasswordResetSimulator.BruteForce;

public class BruteForceEngine
{
    private readonly BruteForceGenerator _generator = new();
    private readonly PasswordValidator _validator = new();

    public string? FindPassword(
        string targetHash,
        int maxLength,
        out TimeSpan elapsedTime)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        foreach (string candidate in _generator.Generate(maxLength))
        {
            if (_validator.IsMatch(candidate, targetHash))
            {
                stopwatch.Stop();

                elapsedTime = stopwatch.Elapsed;

                return candidate;
            }
        }

        stopwatch.Stop();

        elapsedTime = stopwatch.Elapsed;

        return null;
    }
}