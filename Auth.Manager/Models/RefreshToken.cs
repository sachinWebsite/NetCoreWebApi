using System;

namespace Auth.Manager.Models;

// public class RefreshToken
// {
//     public string Token { get; set; }
//     public string UserId { get; set; }
//     public DateTime ExpiresAt { get; set; } 
//     public bool IsRevoked { get; set; }

// }

public sealed record RefreshToken
(
    string Token,
    string UserId,
    DateTime ExpiresAt,
    bool IsRevoked
);
