using System.Net;
using System.Text.Json;
using WebApplication2.DTOs.Common;
using WebApplication2.Exceptions;

namespace WebApplication2.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
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
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception);
            }
        }

        private async Task HandleExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var statusCode = exception switch
            {
                NotFoundException =>
                    HttpStatusCode.NotFound,

                BadRequestException =>
                    HttpStatusCode.BadRequest,

                ConflictException =>
                    HttpStatusCode.Conflict,

                _ =>
                    HttpStatusCode.InternalServerError
            };

            if (statusCode == HttpStatusCode.InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "An unhandled exception occurred.");
            }

            var response = new ErrorResponseDto
            {
                StatusCode = (int)statusCode,
                Message = statusCode ==
                          HttpStatusCode.InternalServerError
                    ? "An unexpected server error occurred."
                    : exception.Message
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(response);

            await context.Response.WriteAsync(json);
        }
    }
}
