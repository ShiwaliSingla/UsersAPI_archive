using System.Net;
using System.Text.Json;

namespace UsersAPI.Middlewares
{
    using global::UsersAPI.Domain.Exceptions;
    using System.Net;
    using System.Text.Json;

    namespace UsersAPI.Middlewares
    {
        public class ExceptionHandlingMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionHandlingMiddleware> _logger;

            public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
            {
                _next = next;
                _logger = logger;
            }

            public async Task Invoke(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (NotFoundException notFoundEx)
                {
                    // Log the exception
                    _logger.LogWarning(notFoundEx, "Resource not found");

                    // Return a 404 Not Found response with the exception message
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.ContentType = "application/json";

                    var errorResponse = JsonSerializer.Serialize(new { error = notFoundEx.Message });
                    await context.Response.WriteAsync(errorResponse);
                }
                catch (Exception ex)
                {
                    // Log other unhandled exceptions
                    _logger.LogError(ex, "Unhandled exception");

                    // Return a 500 Internal Server Error response
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var errorResponse = JsonSerializer.Serialize(new { error = "An unexpected error occurred. shiwali" });
                    await context.Response.WriteAsync(errorResponse);
                }
            }
        }
    }

}
