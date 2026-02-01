using System.Text;
using Auth.Manager.Interfaces;
using Auth.Manager.Models;
using Auth.Manager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Auth.Manager.Common.Extensions;

namespace Auth.Manager.DependencyResolver;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new IdentityOptions();
        configuration.GetSection("AzureAd").Bind(options);

        services.AddSingleton(options);
        services.AddSingleton(options.Jwt);

        services.AddScoped<ITokenIssuer, TokenIssuer>();
        services.AddScoped<TokenExchangeService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<OneTimeTokenService>();

        services.AddScoped<IExternalTokenValidator, ExternalTokenValidator>();
        services.AddSingleton<IRefreshTokenService, InMemoryRefreshTokenService>();
        services.AddScoped<IUserContext, UserContext>();

        // JWT Authentication (FULLY encapsulated)
        services.AddCustomJwtAuthentication(options.Jwt);

        return services;
    }
}
