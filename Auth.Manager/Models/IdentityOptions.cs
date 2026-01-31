using System;

namespace Auth.Manager.Models;

public class IdentityOptions
{
    public JwtOptions Jwt { get; init; } = new();
}
