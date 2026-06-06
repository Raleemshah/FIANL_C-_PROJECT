using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PasswordResetSimulator.Models;
using PasswordResetSimulator.Security;

namespace PasswordResetSimulator.BruteForce;

public class MultiThreadBruteForceEngine
{
    private readonly PasswordValidator _validator = new();

    private const string Characters =
        "abcdefghijklmnopqrstuvwxyz";

    public BruteForceResult FindPassword(string targetHash)
    {
        CancellationTokenSource cts = new();

        object lockObj = new();

        string? foundPassword = null;

        long attempts = 0;

        Stopwatch sw = Stopwatch.StartNew();

        int workers =
            Math.Max(
                1,
                Environment.ProcessorCount - 1);

        Task[] tasks = new Task[workers];

        int lettersPerWorker =
            Characters.Length / workers;

        for (int worker = 0; worker < workers; worker++)
        {
            int start =
                worker * lettersPerWorker;

            int end =
                (worker == workers - 1)
                ? Characters.Length
                : start + lettersPerWorker;

            tasks[worker] = Task.Run(() =>
            {
                for (int i = start;
                     i < end &&
                     !cts.Token.IsCancellationRequested;
                     i++)
                {
                    Search(
                        Characters[i].ToString(),
                        targetHash,
                        ref foundPassword,
                        ref attempts,
                        lockObj,
                        cts);
                }
            });
        }

        Task.WaitAll(tasks);

        sw.Stop();

        return new BruteForceResult
        {
            Password = foundPassword,
            ElapsedSeconds = sw.Elapsed.TotalSeconds,
            Attempts = attempts,
            Found = foundPassword != null
        };
    }

    private void Search(
        string current,
        string targetHash,
        ref string? foundPassword,
        ref long attempts,
        object lockObj,
        CancellationTokenSource cts)
    {
        if (cts.Token.IsCancellationRequested)
            return;

        Interlocked.Increment(ref attempts);

        if (_validator.IsMatch(current, targetHash))
        {
            lock (lockObj)
            {
                foundPassword = current;
            }

            cts.Cancel();

            return;
        }

        if (current.Length >= 6)
            return;

        foreach (char c in Characters)
        {
            Search(
                current + c,
                targetHash,
                ref foundPassword,
                ref attempts,
                lockObj,
                cts);

            if (cts.Token.IsCancellationRequested)
                return;
        }
    }
}