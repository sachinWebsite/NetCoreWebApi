using System;

namespace Auth.Manager.Models;

// public record UserIdentity
// {
//     public string UserId { get; set; }
//     public string Email { get; set; }
//     public string Provider { get; set; } 
//     public List<string> Roles { get; set; }
//     public IDictionary<string, string> Claims { get; set; } 
// }

public sealed record UserIdentity
(
    string UserId,
    string Email,
    string Provider,
    IReadOnlyCollection<string> Roles,
    IReadOnlyDictionary<string, string> Claims
);
