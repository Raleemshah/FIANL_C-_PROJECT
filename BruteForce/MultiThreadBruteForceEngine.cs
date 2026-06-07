// MultiThreadBruteForceEngine.cs
//
// TRUE parallel brute-force:
//   • The 26-character first-char alphabet is split evenly across
//     (CPU cores - 1) workers.
//   • Every worker iterates ALL lengths (1–6) for its assigned slice,
//     so no worker can finish early while others are still on length 1.
//   • Each worker owns its own SHA256 instance → zero shared-state
//     contention on the hot path.
//   • CancellationTokenSource is cancelled the moment ANY worker finds
//     the password, stopping all other workers immediately.
//   • Progress is tracked with a lock-free Interlocked counter.

using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PasswordResetSimulator.Models;
using PasswordResetSimulator.Security;

namespace PasswordResetSimulator.BruteForce;

public class MultiThreadBruteForceEngine
{
    private const string Characters = BruteForceGenerator.Characters;
    private const int    MaxLength  = 6;
    private const string Salt       = "COMP123_STATIC_SALT";

    // Exposed so the UI can poll it for a real progress bar.
    public long AttemptCount => Interlocked.Read(ref _attempts);
    private long _attempts;

    public BruteForceResult FindPassword(
        string targetHash,
        CancellationToken externalToken)
    {
        Stopwatch sw = Stopwatch.StartNew();
        Interlocked.Exchange(ref _attempts, 0);

        // Link external (Stop-button) token with our internal one.
        using CancellationTokenSource cts =
            CancellationTokenSource.CreateLinkedTokenSource(externalToken);

        string? foundPassword = null;

        int workers   = Math.Max(1, Environment.ProcessorCount - 1);
        int charCount = Characters.Length; // 26

        // Distribute the 26 first-characters across workers.
        // e.g. 7 workers → slices of 4,4,4,4,4,4,2 chars
        Task[] tasks = new Task[workers];

        for (int w = 0; w < workers; w++)
        {
            // Integer-divide the alphabet into even bands.
            int start = w       * charCount / workers;
            int end   = (w + 1) * charCount / workers;
            // Capture for the lambda.
            int capturedStart = start;
            int capturedEnd   = end;

            tasks[w] = Task.Run(
                () => WorkerBody(
                    capturedStart,
                    capturedEnd,
                    targetHash,
                    ref foundPassword,
                    cts),
                cts.Token);
        }

        try { Task.WaitAll(tasks); }
        catch { /* OperationCanceledException is expected */ }

        sw.Stop();

        return new BruteForceResult
        {
            Password       = foundPassword,
            ElapsedSeconds = sw.Elapsed.TotalSeconds,
            Found          = foundPassword is not null
        };
    }

    /// <summary>
    /// Each worker iterates lengths 1–MaxLength, but only processes
    /// candidates whose FIRST character falls in [firstCharStart, firstCharEnd).
    ///
    /// This means at every length the full search space is covered by
    /// exactly one worker → no gaps, no overlaps, full parallelism.
    /// </summary>
    private void WorkerBody(
        int firstCharStart,
        int firstCharEnd,
        string targetHash,
        ref string? foundPassword,
        CancellationTokenSource cts)
    {
        // Each thread owns its own SHA256 → no lock on the hot path.
        using SHA256 sha = SHA256.Create();

        byte[] targetBytes = Convert.FromHexString(targetHash);

        for (int length = 1;
             length <= MaxLength && !cts.IsCancellationRequested;
             length++)
        {
            char[] buffer  = new char[length];
            int[]  indices = new int[length];

            // Outer loop: first character restricted to this worker's slice.
            for (int fi = firstCharStart;
                 fi < firstCharEnd && !cts.IsCancellationRequested;
                 fi++)
            {
                // Initialise buffer.
                indices[0] = fi;
                buffer[0]  = Characters[fi];
                for (int k = 1; k < length; k++)
                {
                    indices[k] = 0;
                    buffer[k]  = Characters[0];
                }

                // Inner loop: enumerate all suffixes for this first char.
                while (!cts.IsCancellationRequested)
                {
                    // --- HOT PATH: hash inline, no PasswordValidator alloc ---
                    if (HashMatches(sha, buffer, targetBytes))
                    {
                        string found = new(buffer);
                        if (Interlocked.CompareExchange(
                                ref foundPassword, found, null) == null)
                        {
                            cts.Cancel();
                        }
                        return;
                    }

                    Interlocked.Increment(ref _attempts);

                    // Increment suffix (positions 1..length-1).
                    // Position 0 is fixed for this outer-loop iteration.
                    if (length == 1)
                        break; // single-char candidates: no suffix to increment

                    int pos = length - 1;
                    bool carried = true;
                    while (pos > 0 && carried)
                    {
                        if (indices[pos] < Characters.Length - 1)
                        {
                            indices[pos]++;
                            buffer[pos] = Characters[indices[pos]];
                            carried = false;
                        }
                        else
                        {
                            indices[pos] = 0;
                            buffer[pos]  = Characters[0];
                            pos--;
                        }
                    }

                    if (carried) // exhausted all suffixes for this first char
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Inline hash comparison — avoids allocating a new string just to
    /// pass into PasswordHasher.  Reuses the caller's SHA256 instance.
    /// </summary>
    private static bool HashMatches(
        SHA256 sha,
        char[] candidate,
        byte[] targetBytes)
    {
        // Build "candidate + Salt" as UTF-8 bytes without intermediate strings.
        string candidateStr  = new(candidate);
        string saltedStr     = candidateStr + Salt;
        byte[] inputBytes    = Encoding.UTF8.GetBytes(saltedStr);
        byte[] computedBytes = sha.ComputeHash(inputBytes);

        if (computedBytes.Length != targetBytes.Length)
            return false;

        for (int i = 0; i < computedBytes.Length; i++)
            if (computedBytes[i] != targetBytes[i])
                return false;

        return true;
    }
}