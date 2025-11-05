using FCG_MS_Users.Api.Middlewares;

namespace FCG_MS_Users.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseMiddlewareExtensions(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<RequestLoggingMiddleware>();
        builder.UseMiddleware<ExceptionHandlingMiddleware>();

        return builder;
    }
}
