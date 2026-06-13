using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using UnitConversionApi.Exceptions;

namespace UnitConversionApi.Middleware
{
    /// <summary>
    /// Global exception handling middleware to catch all unhandled exceptions and return standardized JSON error responses.
    /// </summary>
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionHandlingMiddleware"/> class.
        /// </summary>
        /// <param name="next">The next delegate in the HTTP request pipeline.</param>
        /// <param name="logger">The logger instance.</param>
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware logic to process the HTTP context and catch exceptions.
        /// </summary>
        /// <param name="context">The current HTTP context.</param>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidConversionException ex)
            {
                _logger.LogWarning(ex, "Validation failed during unit conversion request: {Message}", ex.Message);
                await WriteErrorResponseAsync(context, StatusCodes.Status400BadRequest, "Invalid Conversion", ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request.");
                await WriteErrorResponseAsync(
                    context, 
                    StatusCodes.Status500InternalServerError, 
                    "Unexpected Error", 
                    "An unexpected error occurred. Please contact system support if the problem persists.");
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, int statusCode, string title, string detail)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
