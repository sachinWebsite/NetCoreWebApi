using Auth.Manager.Interfaces;
using Auth.Manager.Models;

namespace Auth.Manager.Services;

public sealed class RefreshTokenService
{
    private readonly IRefreshTokenService _refreshStore;
    private readonly ITokenIssuer _issuer;
    private readonly IUserContext _userContext;

    public RefreshTokenService(
        IRefreshTokenService refreshStore,
        ITokenIssuer issuer,
        IUserContext userContext)
    {
        _refreshStore = refreshStore;
        _issuer = issuer;
        _userContext = userContext;
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken)
    {
        var stored = await _refreshStore.ValidateAsync(refreshToken);

        await _refreshStore.RevokeAsync(refreshToken);

        var user = await _userContext.GetAsync(stored.UserId);

        var accessToken = _issuer.IssueAccessToken(user, out var expiresAt);
        var newRefresh = await _refreshStore.CreateAsync(user.UserId);

        return new AuthResult(accessToken, newRefresh.Token, expiresAt);
    }
}
