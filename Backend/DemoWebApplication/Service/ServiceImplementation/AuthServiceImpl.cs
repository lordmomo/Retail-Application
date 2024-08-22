using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceInterface;
using DemoWebApplication.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoWebApplication.Service.ServiceImplementation;

public class AuthServiceImpl : IAuthServiceInterface
{
    public ApplicationDbContext db;
    private readonly UserManager<Person> _userManager;
    public IConfiguration configuration;
    public AuthServiceImpl(ApplicationDbContext db
        , UserManager<Person> userManager, IConfiguration configuration)
    {
        this.db = db;
        this._userManager = userManager;
        this.configuration = configuration;
    }

    public async Task<bool> CheckUserCredentials(Login user)
    {
        var identityUser = await _userManager.FindByNameAsync(user.Username);
        if (identityUser is null)
        {
            return false;
        }
        return await _userManager.CheckPasswordAsync(identityUser, user.Password);
    }

    //public async Task<bool> Login(Login user)
    //{
    //    var result = await signInManager.PasswordSignInAsync(user.Username, user.Password, isPersistent: false, lockoutOnFailure: false);
    //    if(result.Succeeded)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    public async Task<string> GenerateTokenString(Person user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        byte[] key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
        var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
        var claims = new List<Claim>
        {
            new Claim("username",user.UserName),
        };
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        var expires = DateTime.UtcNow.AddMinutes(10);
        var securityToken= new JwtSecurityToken
        (
            claims : claims,
            expires : expires,
            issuer : issuer,
            audience : audience,
            signingCredentials : signingCredentials
        );
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.WriteToken(securityToken);
        return jwtToken;
    }

    //public ClaimsPrincipal ValidateToken(string token)
    //{
      
    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
    //    var tokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuerSigningKey = true,
    //        IssuerSigningKey = new SymmetricSecurityKey(key),
    //        ValidateIssuer = true,
    //        ValidateAudience = true,
    //        ValidateLifetime = true,
    //        ClockSkew = TimeSpan.Zero,
    //        ValidIssuer = configuration["Jwt:Issuer"],
    //        ValidAudience = configuration["Jwt:Audience"]
    //    };
    //    try
    //    {
    //        SecurityToken validatedToken;
    //        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
    //        return principal;
    //    }
    //    catch (SecurityTokenExpiredException ex)
    //    {
    //        throw; 
    //    }
    //    catch (SecurityTokenInvalidSignatureException)
    //    {
    //        throw; 
    //    }
    //    catch (Exception ex)
    //    {

    //        throw; 
    //    }

    //}

    //public void GenerateNewToken(string? name, HttpContext httpContext)
    //{
    //    var issuer = configuration["Jwt:Issuer"];
    //    var audience = configuration["Jwt:Audience"];
    //    byte[] key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
    //    var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
    //    var claims = new[]
    //    {
    //        new Claim("username",name)
    //    };
    //    var expires = DateTime.UtcNow.AddMinutes(2);
    //    var securityToken = new JwtSecurityToken
    //    (
    //        claims: claims,
    //        expires: expires,
    //        issuer: issuer,
    //        audience: audience,
    //        signingCredentials: signingCredentials
    //    );
    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var jwtToken = tokenHandler.WriteToken(securityToken);
    //    httpContext.Response.Cookies.Delete("JwtToken");
    //    httpContext.Response.Cookies.Append("JwtToken", jwtToken, new CookieOptions
    //    {
    //        HttpOnly = true,
    //        Expires = DateTimeOffset.UtcNow.AddMinutes(2) 
    //    });
        
    //}
}
