//using Microsoft.IdentityModel.Tokens;
//using Serilog;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace DemoWebApplication.Service.ServiceImplementation;

//public class TokenServiceImpl
//{
//    private readonly IHttpContextAccessor _httpContextAccessor;
//    private readonly IConfiguration _configuration;

//    public TokenServiceImpl(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
//    {
//        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
//        _configuration = configuration;
//    }
//    public ClaimsPrincipal GetClaimsPrincipal(HttpContext httpContext)
//    {
//        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
//        Log.Debug("inside token");
//        Log.Debug(token);
//        if (string.IsNullOrEmpty(token))
//        {
//            return null;
//        }

//        var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidIssuer = _configuration["Jwt:Issuer"],
//            ValidAudience = _configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]))
//        }, out var validatedToken);

//        Console.WriteLine(claimsPrincipal);
//        Log.Debug("exit token");

//        return claimsPrincipal;
//    }
//}
