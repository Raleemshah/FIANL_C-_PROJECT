//password manager.cs
using System;
using PasswordResetSimulator.Security;

namespace PasswordResetSimulator.Models;

public class PasswordManager
{
    private const string Characters =
        "abcdefghijklmnopqrstuvwxyz";

    private readonly Random _random = new();

    public string GeneratePassword()
    {
      int length = _random.Next(4, 6);
      //int length = 4;

        char[] password = new char[length];

        for (int i = 0; i < length; i++)
        {
            password[i] =
                Characters[
                    _random.Next(Characters.Length)];
        }

        return new string(password);
    }

    public string GenerateHash(string password)
    {
        return PasswordHasher.ComputeHash(password);
    }
}