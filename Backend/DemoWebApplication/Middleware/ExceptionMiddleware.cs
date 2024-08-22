using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace DemoWebApplication.Middleware;


[ExcludeFromCodeCoverage]
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured: {ex.Message}");
            await HandleExceptionAsync(httpContext, ex);
        }

    }
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        var errorResponse = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error from the custom exception middleware",
            ExceptionMessage = exception.Message, 
            ExceptionType = exception.GetType().ToString()
        };

        var errorJson = JsonConvert.SerializeObject(errorResponse);
        return context.Response.WriteAsync(errorJson);

    }
}

[ExcludeFromCodeCoverage]
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}
