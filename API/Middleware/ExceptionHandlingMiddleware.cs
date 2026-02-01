using API.Models;
using Auth.Manager.Common.Exceptions;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var (statusCode, message) = exception switch
            {
                //NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
                AppValidationException ex => (HttpStatusCode.BadRequest, ex.Message),
                SecurityTokenException ex => (HttpStatusCode.Unauthorized, ex.Message),

                _ => (HttpStatusCode.InternalServerError,
                      "Internal Server Error.")
            };

            var response = ApiResponse<object>.Fail(message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
