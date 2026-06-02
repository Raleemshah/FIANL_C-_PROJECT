using System.Collections.Generic;

namespace PasswordResetSimulator.BruteForce;

public class BruteForceGenerator
{
    private const string Characters =
        "abcdefghijklmnopqrstuvwxyz";

    public IEnumerable<string> Generate(int maxLength)
    {
        for (int length = 1; length <= maxLength; length++)
        {
            foreach (var value in GenerateRecursive("", length))
            {
                yield return value;
            }
        }
    }

    private IEnumerable<string> GenerateRecursive(
        string current,
        int remaining)
    {
        if (remaining == 0)
        {
            yield return current;
            yield break;
        }

        foreach (char c in Characters)
        {
            foreach (var result in GenerateRecursive(
                current + c,
                remaining - 1))
            {
                yield return result;
            }
        }
    }
}