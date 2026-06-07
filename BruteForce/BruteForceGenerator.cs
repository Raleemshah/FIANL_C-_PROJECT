// BruteForceGenerator.cs
// Generates all combinations from length 1 up to maxLength.
// Uses a char[] buffer instead of string concatenation to avoid
// allocating a new string on every recursive call (major GC fix).

using System.Collections.Generic;

namespace PasswordResetSimulator.BruteForce;

public class BruteForceGenerator
{
    public const string Characters = "abcdefghijklmnopqrstuvwxyz";

    /// <summary>
    /// Yields every combination from length 1 to maxLength,
    /// optionally restricted to candidates whose first character
    /// falls within [firstCharStart, firstCharEnd).
    /// </summary>
    public IEnumerable<string> Generate(
        int maxLength,
        int firstCharStart = 0,
        int firstCharEnd = -1)
    {
        if (firstCharEnd < 0)
            firstCharEnd = Characters.Length;

        // Reuse a single buffer for all candidates at each length.
        for (int length = 1; length <= maxLength; length++)
        {
            char[] buffer = new char[length];
            int[] indices = new int[length];

            // Set first character to the partition start.
            int startFirst = (length == 1) ? firstCharStart : 0;
            int endFirst   = (length == 1) ? firstCharEnd   : Characters.Length;

            for (int fi = startFirst; fi < endFirst; fi++)
            {
                indices[0] = fi;
                buffer[0]  = Characters[fi];

                // Reset the rest.
                for (int k = 1; k < length; k++)
                {
                    indices[k] = 0;
                    buffer[k]  = Characters[0];
                }

                // For length > 1, partition only the first character
                // when this is a partitioned call; inner positions
                // iterate over the full alphabet regardless.
                if (length > 1 && fi >= firstCharStart && fi < firstCharEnd)
                {
                    // Enumerate all suffixes for this first char.
                    while (true)
                    {
                        yield return new string(buffer);

                        // Increment from rightmost position.
                        int pos = length - 1;
                        while (pos > 0)
                        {
                            if (indices[pos] < Characters.Length - 1)
                            {
                                indices[pos]++;
                                buffer[pos] = Characters[indices[pos]];
                                break;
                            }
                            indices[pos] = 0;
                            buffer[pos]  = Characters[0];
                            pos--;
                        }

                        // If we wrapped back to pos 0 it means we
                        // exhausted all suffixes for this first char.
                        if (pos == 0)
                            break;
                    }
                }
                else if (length == 1)
                {
                    yield return new string(buffer);
                }
            }
        }
    }
}