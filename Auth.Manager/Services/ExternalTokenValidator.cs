using Auth.Manager.Interfaces;
using Auth.Manager.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Auth.Manager.Services;

public sealed class ExternalTokenValidator : IExternalTokenValidator
{
    public Task<UserIdentity> ValidateAsync(string externalToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(externalToken);

        // In real impl → Validate signature & issuer
        return Task.FromResult(new UserIdentity(
            UserId: jwt.Claims.First(c => c.Type == "oid").Value,
            Email: jwt.Claims.First(c => c.Type == "preferred_username").Value,
            Provider: "AzureAD",
            Roles: jwt.Claims
                .Where(c => c.Type == "roles")
                .Select(c => c.Value)
                .ToList(),
            Claims: jwt.Claims.ToDictionary(c => c.Type, c => c.Value)
        ));
    }
}
