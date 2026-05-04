// -----------------------------------------------------------------------------
// File: ExceptionHandlingMiddleware.cs
// Project: ArenaSync.Web
// Purpose: Batch 5 — Global unhandled-exception handler.
//          Logs the full exception, then either returns a JSON error response
//          (for API-style requests) or redirects to the /Error page.
// -----------------------------------------------------------------------------

using System.Net;
using System.Text.Json;

namespace ArenaSync.Web.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception for request {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Don't overwrite a response that has already started streaming
            if (context.Response.HasStarted)
            {
                _logger.LogWarning("Response already started — cannot rewrite error response.");
                return;
            }

            // If the caller expects JSON (e.g. an API consumer), return structured JSON
            bool wantsJson = context.Request.Headers.Accept
                .Any(a => a != null && a.Contains("application/json", StringComparison.OrdinalIgnoreCase));

            if (wantsJson)
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var payload = new
                {
                    status = 500,
                    message = "An unexpected error occurred. Please try again later.",
                    detail = _env.IsDevelopment() ? exception.ToString() : null
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
            else
            {
                // Redirect browser requests to the error page
                context.Response.Redirect("/Error");
            }
        }
    }
}
