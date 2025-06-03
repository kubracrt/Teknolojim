using System.Diagnostics;

namespace backend.Middlewares
{

    public class RequestLoggingMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown IP";
            var path = context.Request.Path;
            var method = context.Request.Method;

            await _next(context);

            stopwatch.Stop();
            var statusCode = context.Response.StatusCode;
            var elapsed = stopwatch.ElapsedMilliseconds;

            _logger.LogInformation("Request  : {Method} {Path} from {IP} responded with {StatusCode} in {Elapsed} ms", 
                method, path, ip, statusCode, elapsed);
        }
    }
}