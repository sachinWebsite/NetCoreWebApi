using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Manager.Interfaces;
using Auth.Manager.Models;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Manager.Services;

public sealed class TokenIssuer : ITokenIssuer
{
    private readonly JwtOptions _options;

    public TokenIssuer(JwtOptions options)
    {
        _options = options;
    }

    public string IssueAccessToken(UserIdentity user, out DateTime expiresAt)
    {
        expiresAt = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

        //var claims = new List<Claim>
        //{
        //    new(JwtRegisteredClaimNames.Sub, user.UserId),
        //    new(JwtRegisteredClaimNames.Email, user.Email),
        //    new("provider", user.Provider)
        //};

        var claims = new List<Claim>
        {
            new("oid", user.UserId),
            new("preferred_username", user.Email),
            new(JwtRegisteredClaimNames.Sub, user.UserId),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("provider", user.Provider)
        };


        claims.AddRange(user.Roles.Select(r => new Claim("role", r)));
        claims.AddRange(user.Claims.Select(c => new Claim(c.Key, c.Value)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
