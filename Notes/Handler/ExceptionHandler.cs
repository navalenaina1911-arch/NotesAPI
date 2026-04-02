using Microsoft.AspNetCore.Diagnostics;
using Notes.Exceptions;
using System.Net;

namespace Notes.Handler
{
 
public class ExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<ExceptionHandler> _logger;

        public ExceptionHandler(ILogger<ExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext context,
            Exception exception,
            CancellationToken cancellationToken)
        {
            // Handle your custom ApiException
            if (exception is ApiException api)
            {
                _logger.LogError(exception, "API exception occurred");
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    error = api.Message,
                    status = api.StatusCode,
                    details = api.Details
                };

                await context.Response.WriteAsJsonAsync(payload, cancellationToken);
                return true;
            }

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var error = new
            {
                error = "An unexpected error occurred",
                status = HttpStatusCode.InternalServerError
            };

            await context.Response.WriteAsJsonAsync(error, cancellationToken);
            return true;
        }
    }
}
