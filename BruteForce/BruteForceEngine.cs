// BruteForceEngine.cs
// Single-threaded brute-force engine.
// Iterates through the generator sequentially and stops on first match.

using System;
using System.Diagnostics;
using System.Threading;
using PasswordResetSimulator.Security;

namespace PasswordResetSimulator.BruteForce;

public class BruteForceEngine
{
    private readonly BruteForceGenerator _generator = new();
    private readonly PasswordValidator   _validator  = new();

    public string? FindPassword(
        string targetHash,
        int maxLength,
        out TimeSpan elapsedTime)
        => FindPassword(
            targetHash,
            maxLength,
            out elapsedTime,
            CancellationToken.None);

    public string? FindPassword(
        string targetHash,
        int maxLength,
        out TimeSpan elapsedTime,
        CancellationToken cancellationToken)
    {
        Stopwatch sw = Stopwatch.StartNew();

        foreach (string candidate in _generator.Generate(maxLength))
        {
            if (cancellationToken.IsCancellationRequested)
            {
                sw.Stop();
                elapsedTime = sw.Elapsed;
                return null;
            }

            if (_validator.IsMatch(candidate, targetHash))
            {
                sw.Stop();
                elapsedTime = sw.Elapsed;
                return candidate;
            }
        }

        sw.Stop();
        elapsedTime = sw.Elapsed;
        return null;
    }
}