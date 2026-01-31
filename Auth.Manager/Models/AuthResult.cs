using System;

namespace Auth.Manager.Models;

// public class AuthResult
// {
//     public string AccessToken { get; set; }
//     public string RefreshToken { get; set; }
//     public DateTime AccessTokenExpiresAt { get; set; }
//     // public AuthResult(string accessToken, string refreshToken)
//     // {
//     //     AccessToken = accessToken;
//     //     RefreshToken = refreshToken;
//     // }
// }

public sealed record AuthResult
(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);
