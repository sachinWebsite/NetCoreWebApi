using Auth.Manager.Interfaces;
using Auth.Manager.Models;
using Auth.Manager.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Manager.AuthManagerExtensions;

public static class AuthManagerExtensions
{
    public static IServiceCollection AddAuthManager(
        this IServiceCollection services,
        Action<JwtOptions> jwtOptions)
    {
        var options = new JwtOptions();
        jwtOptions(options);

        services.AddSingleton(options);

        // Core services
        services.AddScoped<ITokenIssuer, TokenIssuer>();
        services.AddScoped<TokenExchangeService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<OneTimeTokenService>();

        return services;
    }
}
