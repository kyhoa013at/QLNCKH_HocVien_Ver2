using Microsoft.AspNetCore.Mvc;
using QLNCKH_HocVien.Client.Models;
using System.Net;
using System.Text.Json;

namespace QLNCKH_HocVien.Middleware
{
    /// <summary>
    /// Global Exception Handler Middleware
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception
            _logger.LogError(exception, 
                "Unhandled exception occurred. TraceId: {TraceId}, Path: {Path}, Method: {Method}",
                context.TraceIdentifier,
                context.Request.Path,
                context.Request.Method);

            // Determine status code and message based on exception type
            var (statusCode, message) = exception switch
            {
                ArgumentException _ => (HttpStatusCode.BadRequest, exception.Message),
                KeyNotFoundException _ => (HttpStatusCode.NotFound, exception.Message),
                UnauthorizedAccessException _ => (HttpStatusCode.Unauthorized, "Không có quyền truy cập"),
                InvalidOperationException _ => (HttpStatusCode.BadRequest, exception.Message),
                _ => (HttpStatusCode.InternalServerError, "Đã xảy ra lỗi. Vui lòng thử lại sau.")
            };

            // Prepare response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var response = new ApiResult
            {
                Success = false,
                Message = message,
                Errors = _env.IsDevelopment() 
                    ? new List<string> { exception.ToString() } 
                    : new List<string>()
            };

            // Add TraceId for debugging
            context.Response.Headers["X-Trace-Id"] = context.TraceIdentifier;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }
    }

    /// <summary>
    /// Extension method để đăng ký middleware
    /// </summary>
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}

