using Auth.Manager.Interfaces;
using Auth.Manager.Models;

namespace Auth.Manager.Services;

public sealed class TokenExchangeService
{
    private readonly IExternalTokenValidator _validator;
    private readonly ITokenIssuer _issuer;
    private readonly IRefreshTokenService _refreshService;

    public TokenExchangeService(
        IExternalTokenValidator validator,
        ITokenIssuer issuer,
        IRefreshTokenService refreshService)
    {
        _validator = validator;
        _issuer = issuer;
        _refreshService = refreshService;
    }

    public async Task<AuthResult> ExchangeAsync(string externalToken)
    {
        var user = await _validator.ValidateAsync(externalToken);

        var accessToken = _issuer.IssueAccessToken(user, out var expiresAt);
        var refresh = await _refreshService.CreateAsync(user.UserId);

        return new AuthResult(accessToken, refresh.Token, expiresAt);
    }
}
