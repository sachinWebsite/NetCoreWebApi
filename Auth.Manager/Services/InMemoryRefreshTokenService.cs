using Auth.Manager.Infrastructure;
using Auth.Manager.Interfaces;
using Auth.Manager.Models;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;

namespace Auth.Manager.Services;

public class InMemoryRefreshTokenService: IRefreshTokenService
{
    private readonly ConcurrentDictionary<string, RefreshToken> _store = new();

    private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(14);

    public Task<RefreshToken> CreateAsync(string userId)
    {
        var rawToken = SecureTokenGenerator.Generate();
        var hashed = RefreshTokenHasher.Hash(rawToken);

        var refreshToken = new RefreshToken(
            Token: hashed,
            UserId: userId,
            ExpiresAt: DateTime.UtcNow.Add(TokenLifetime),
            IsRevoked: false
        );

        _store[hashed] = refreshToken;

        // ⚠️ IMPORTANT:
        // Return RAW token to client, not hashed
        return Task.FromResult(refreshToken with { Token = rawToken });
    }

    public Task<RefreshToken> ValidateAsync(string token)
    {
        var hashed = RefreshTokenHasher.Hash(token);
        if (!_store.TryGetValue(hashed, out var stored))
            throw new SecurityTokenException("Invalid refresh token");

        if (stored.IsRevoked)
            throw new SecurityTokenException("Refresh token revoked");

        if (stored.ExpiresAt < DateTime.UtcNow)
            throw new SecurityTokenException("Refresh token expired");

        return Task.FromResult(stored);
    }

    public Task RevokeAsync(string token)
    {
        var hashed = RefreshTokenHasher.Hash(token);

        if (_store.TryGetValue(hashed, out var stored))
        {
            _store[hashed] = stored with { IsRevoked = true };
        }

        return Task.CompletedTask;
    }
}