using System;
using System.Security.Cryptography;

namespace Auth.Manager.Infrastructure;

internal static class SecureTokenGenerator
{
    public static string Generate(int size = 64)
    {
        var bytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(bytes);
    }
}
