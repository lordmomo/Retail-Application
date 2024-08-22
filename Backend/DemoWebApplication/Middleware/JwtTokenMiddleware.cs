using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;

namespace DemoWebApplication.Middleware;

[ExcludeFromCodeCoverage]
public class JwtTokenMiddleware
{
    private readonly RequestDelegate _next;

    public JwtTokenMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext httpContext)
    {
        var jwtToken = httpContext.Request.Cookies["JwtToken"];
        if(jwtToken == null)
        {
            Log.Debug("NO cokie in auth header");
            //httpContext.Request.Headers.Append("Authorization", "Bearer " + jwtToken);
        }
        //if (httpContext.Request.Path.Value != "/login" && httpContext.Request.Path.Value != "/register")
        //{
        //    if (string.IsNullOrEmpty(jwtToken))
        //    {
        //        // Redirect to login if not logged in and not on registration or login page
        //        httpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
        //        httpContext.Response.Redirect("/login");
        //        return Task.CompletedTask;
        //    }
        //    else
        //    {
        //        httpContext.Request.Headers.Append("Authorization", "Bearer " + jwtToken);
        //    }
        //}

        //if (httpContext.Request.Path.Value == "/register")
        //{
        //    if (string.IsNullOrEmpty(jwtToken))
        //    {
        //        return _next(httpContext);

        //    }
        //}
        //else
        //if (httpContext.Request.Path.Value != "/login")
        //{
        //    //var jwtToken = httpContext.Request.Cookies["JwtToken"];
        //    if (!string.IsNullOrEmpty(jwtToken))
        //    {
        //        httpContext.Request.Headers.Append("Authorization", "Bearer " + jwtToken);
        //    }
        //}

        return _next(httpContext);
    }
}


[ExcludeFromCodeCoverage]
public static class JwtTokenMiddlewareExtensions
{
    public static IApplicationBuilder UseJwtTokenMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtTokenMiddleware>();
    }
}