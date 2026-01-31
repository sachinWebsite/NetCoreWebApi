using System;
using Auth.Manager.Models;

namespace Auth.Manager.Interfaces;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateAsync(string userId);
    Task<RefreshToken> ValidateAsync(string token);
    Task RevokeAsync(string token);
}
