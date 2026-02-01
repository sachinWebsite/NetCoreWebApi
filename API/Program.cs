// Clean Architecture: API layer depends only on Application and Infrastructure
// SOLID: Dependency Inversion via DI container
// Async everywhere for scalability
using API.Filters;
using API.Middleware;
using Application.Interfaces;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.RateLimiting;
using Polly;
using Polly.Extensions.Http;
using MediatR;
using Microsoft.AspNetCore.Mvc;
// using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.OpenApi.Models;
using Auth.Manager.DependencyResolver;

var builder = WebApplication.CreateBuilder(args);
// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NetCoreWebApi API",
        Version = "v1"
    });
});
builder.Configuration.AddEnvironmentVariables();

// Configure Serilog for structured logging and observability
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add controllers with global filters for exception handling and validation
builder.Services.AddControllers(options =>
{
    //options.Filters.Add<GlobalExceptionFilter>();
    //options.Filters.Add<ValidationFilter>();
    options.Filters.Add<ApiActionFilter>();
}).ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// API Versioning for backward compatibility
// builder.Services.AddApiVersioning(options =>
// {
//     options.AssumeDefaultVersionWhenUnspecified = true;
//     options.DefaultApiVersion = new ApiVersion(1, 0);
//     options.ReportApiVersions = true;
//     options.ApiVersionReader = new HeaderApiVersionReader("api-version");
// });

// FluentValidation for pipeline-based request validation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

// MediatR for CQRS pattern in Application layer
builder.Services.AddMediatR(typeof(Program).Assembly);

// Dependency Injection: Scoped lifetimes to avoid singleton-scoped issues
builder.Services.AddScoped<IUserRepository, UserRepository>();

// EF Core for data access in Infrastructure layer
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Custom Platform Identity Services as Dependency Injection
builder.Services.AddPlatformIdentity(builder.Configuration);
// JWT Authentication for security
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:Issuer"],
//             ValidAudience = builder.Configuration["Jwt:Audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
//                 builder.Configuration["JWT_KEY"] ?? throw new InvalidOperationException("JWT_KEY environment variable not set")))
//         };
//     });

// Policy-based authorization
// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
// });

// Data Protection API for secure key storage
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "keys")))
    .SetApplicationName("NetWebApiMasterFeatures");

// Health checks for production monitoring
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// CORS for cross-origin requests (restrict in production)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins("https://yourdomain.com") // TODO: Replace with your allowed origin
              .WithMethods("GET", "POST")
              .AllowAnyHeader();
    });
});

// Rate limiting for production readiness
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10);
        opt.PermitLimit = 5;
    });
});

// Response caching for performance
builder.Services.AddResponseCaching();

// HttpClient with Polly for resilience (retry & circuit breaker)
// builder.Services.AddHttpClient("PollyClient")
//     .AddPolicyHandler(HttpPolicyExtensions
//         .HandleTransientHttpError()
//         .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))))
//     .AddPolicyHandler(HttpPolicyExtensions
//         .HandleTransientHttpError()
//         .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();

    // Enable Swagger middleware & UI in Development only
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NetCoreWebApi API v1");
        c.RoutePrefix = "swagger";
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Custom middleware for correlation ID and logging
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowSpecific");

app.UseRateLimiter();

app.UseResponseCaching();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();