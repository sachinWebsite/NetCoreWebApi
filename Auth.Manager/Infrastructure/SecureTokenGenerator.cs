using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;

namespace Auth.Manager.Infrastructure;

internal static class SecureTokenGenerator
{
    public static string Generate(int size = 64)
    {
        var bytes = RandomNumberGenerator.GetBytes(size);
        return Base64UrlEncoder.Encode(bytes);
    }
}
