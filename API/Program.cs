using API.Extensions;
using API.Filters;
using API.Middleware;
using Application.DependencyResolver;
using Application.Interfaces;
using Application.UseCases;
using Auth.Manager.DependencyResolver;
using FluentValidation;
using Infrastructure.Data;
using Infrastructure.DependencyResolver;
using Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddJwtSwagger("NetCoreWebApi API");
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
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
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
// Dependency Injection: Application layer
builder.Services.AddApplicationDependency();
// Dependency Injection: Infrastructure layer repositories
builder.Services.AddInfrastructureDependency(builder.Configuration);

// EF Core for data access in Infrastructure layer
builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
                    );

// Custom Platform Identity Services as Dependency Injection
builder.Services.AddPlatformIdentity(builder.Configuration);

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
builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();

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