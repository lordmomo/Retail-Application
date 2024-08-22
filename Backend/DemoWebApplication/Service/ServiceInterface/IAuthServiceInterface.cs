using DemoWebApplication.Models;
using System.Security.Claims;

namespace DemoWebApplication.Service.ServiceInterface
{
    public interface IAuthServiceInterface
    {

        public Task<bool> CheckUserCredentials(Login user);

        public Task<string> GenerateTokenString(Person user);

        //public ClaimsPrincipal ValidateToken(string token);

        //public void GenerateNewToken(string? name, HttpContext httpContext);


    }
}
