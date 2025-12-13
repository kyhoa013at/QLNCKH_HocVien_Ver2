using System.Diagnostics;

namespace QLNCKH_HocVien.Middleware
{
    /// <summary>
    /// Middleware để log request/response cho API calls
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Chỉ log cho API requests
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            var stopwatch = Stopwatch.StartNew();
            var requestId = context.TraceIdentifier;

            // Log request
            _logger.LogInformation(
                "API Request: {RequestId} {Method} {Path} started",
                requestId,
                context.Request.Method,
                context.Request.Path);

            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                // Log response
                var level = context.Response.StatusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

                _logger.Log(level,
                    "API Response: {RequestId} {Method} {Path} responded {StatusCode} in {ElapsedMs}ms",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}

