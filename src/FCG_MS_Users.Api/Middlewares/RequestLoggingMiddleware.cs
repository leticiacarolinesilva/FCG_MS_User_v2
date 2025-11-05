using System.Diagnostics;

namespace FCG_MS_Users.Api.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        var request = context.Request;
        var method = request.Method;
        var path = request.Path;

        _logger.LogInformation("Starting request: {Method} {Path}", method, path);

        await _next(context);

        stopwatch.Stop();

        var statusCode = context.Response.StatusCode;

        _logger.LogInformation("Finished request: {Method} {Path} - Status {StatusCode} - Elapsed {Elapsed}ms",
            method, path, statusCode, stopwatch.ElapsedMilliseconds);
    }
}
