//passwordhasher.cs
using System;
using System.Security.Cryptography;
using System.Text;

namespace PasswordResetSimulator.Security;

public static class PasswordHasher
{
    private const string Salt = "COMP123_STATIC_SALT";

    public static string ComputeHash(string password)
    {
        using SHA256 sha = SHA256.Create();

        string saltedPassword = password + Salt;

        byte[] bytes =
            sha.ComputeHash(
                Encoding.UTF8.GetBytes(saltedPassword));

        return Convert.ToHexString(bytes);
    }
}