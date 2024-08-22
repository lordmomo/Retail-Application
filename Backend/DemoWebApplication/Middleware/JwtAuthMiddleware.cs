//using DemoWebApplication.Models;
//using DemoWebApplication.Service.ServiceImplementation;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Microsoft.IdentityModel.Tokens;
//using System.Diagnostics.CodeAnalysis;
//using System.Net;
//using System.Security.Claims;


//namespace DemoWebApplication.Middleware;

//[ExcludeFromCodeCoverage]
//public class JwtAuthMiddleware 
//{
//    private readonly RequestDelegate _next;
//    private readonly IAuthorizationService _authorizationService;

//    public JwtAuthMiddleware(RequestDelegate next,IAuthorizationService authorizationService)
//    {
//        _next = next;
//        _authorizationService = authorizationService;
//    }

//    public async Task Invoke(HttpContext context, AuthServiceImpl authServiceImpl)
//    {
//        Console.WriteLine(context.User.Identity.Name);
//        var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
//        try
//        {
//            if (!string.IsNullOrEmpty(token))
//            {
//                ClaimsPrincipal principal = null;

//                try
//                {
//                    principal = authServiceImpl.ValidateToken(token);

//                }
//                catch (SecurityTokenExpiredException)
//                {
//                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    await context.Response.WriteAsync("Token has expired. Please log in again.");
//                    return;
//                }
//                catch (SecurityTokenInvalidSignatureException)
//                {
//                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    await context.Response.WriteAsync("Invalid token signature.");
//                    return;
//                }
//                catch (Exception)
//                {
//                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                    await context.Response.WriteAsync("An error occurred while processing the token.");
//                    return;
//                }



//                if (principal != null)
//                {
//                    var usernameClaims = principal?.FindFirst("username")?.Value;
//                    if (usernameClaims != context.Request.Cookies["usernameCookie"])
//                    {
//                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                        return;
//                    }
//                    //var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
//                    var authorizationResult = await _authorizationService.AuthorizeAsync(principal, null, new RoleRequirement());
//                    if (authorizationResult.Succeeded)
//                    {
//                        // User is authorized, proceed with the request
//                        await _next(context);
//                        //return;
//                    }
//                }
                
//            }
//            //else if (string.IsNullOrWhiteSpace(context.User.Identity.Name) && context.Request.Path.Value == "/register")
//            //{
//            //    await _next(context);
//            //}
//            else if (string.IsNullOrWhiteSpace(context.User.Identity.Name) && context.Request.Path.Value != "/login")
//            {
//                if(context.Request.Path.Value == "/register")
//                {
//                    await _next(context);
//                }
//                context.Response.StatusCode = (int)HttpStatusCode.Redirect;
//                context.Response.Redirect("/login");
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine(ex);
//            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; 
//            return;
//        }

//        await _next(context);
//    }
//}
