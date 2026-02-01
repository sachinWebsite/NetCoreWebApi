using System.Text;
using Auth.Manager.Interfaces;
using Auth.Manager.Models;
using Auth.Manager.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;

namespace Auth.Manager.AuthManagerExtensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlatformIdentity(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1️⃣ Bind configuration INSIDE library
        var options = new IdentityOptions();
        configuration.GetSection("AzureAd").Bind(options);

        services.AddSingleton(options);
        services.AddSingleton(options.Jwt);

        // 2️⃣ Core services
        services.AddScoped<ITokenIssuer, TokenIssuer>();
        services.AddScoped<TokenExchangeService>();
        services.AddScoped<RefreshTokenService>();
        services.AddScoped<OneTimeTokenService>();

        // 3️⃣ External token validator (default)
        services.AddScoped<IExternalTokenValidator, ExternalTokenValidator>();

        // 4️⃣ Refresh token store (can be swapped)
        services.AddSingleton<IRefreshTokenService, InMemoryRefreshTokenService>();

        services.AddScoped<IUserContext, UserContext>();

        // 5️⃣ JWT Authentication (FULLY encapsulated)
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = options.Jwt.Issuer,
                    ValidAudience = options.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Jwt.SigningKey))
                };
            });

        return services;
    }
}
