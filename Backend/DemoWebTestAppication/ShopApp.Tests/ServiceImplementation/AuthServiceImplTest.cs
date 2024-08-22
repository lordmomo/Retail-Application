using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceImplementation;
using DemoWebTestAppication.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebTestAppication.ShopApp.Tests.ServiceImplementation
{
    public class AuthServiceImplTest
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        //private readonly MockApplicationDb _mockApplicationDb;

        private readonly ApplicationDbContext _applicationDbContext;

        private readonly Mock<UserManager<Person>> _userManagerMock;


        private AuthServiceImpl _authServiceImpl;

        public AuthServiceImplTest()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabaseForAuth").Options;

            //_mockApplicationDb = new MockApplicationDb();
            _applicationDbContext = new ApplicationDbContext(_dbContextOptions, null);
            //_applicationDbContext = _mockApplicationDb.GetApplicationDbContext();

            SeedDatabse();

            var userStore = new Mock<IUserStore<Person>>();

            _userManagerMock = new Mock<UserManager<Person>>(userStore.Object, null, null, null, null, null, null, null, null);


            var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        {"Jwt:Issuer" , "testIssuer" },
                        {"Jwt:Audience","testAudience" },
                        {"Jwt:Key","thisIsAValidSecretKeyForTesting12345" }
                    }).Build();


            //var jwtSection = new Mock<IConfigurationSection>();
            //jwtSection.Setup(x => x[It.IsAny<string>()]).Returns<string>(key =>
            //{
            //    return key switch
            //    {
            //        "Jwt:Issuer" => "testIssuer",
            //        "Jwt:Audience" => "testAudience",
            //        "Jwt:Key" => "testSecretKey12345",
            //        _ => null
            //    };
            //});
            //_configurationMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSection.Object);


            _authServiceImpl = new AuthServiceImpl(_applicationDbContext, _userManagerMock.Object, configuration);
        }

        private void SeedDatabse()
        {
            using (var dbContext = new ApplicationDbContext(_dbContextOptions, null))
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Users.Add(new Person
                {
                    Id = "demo_auth_userABC",
                    UserName = "userABC",
                    CustomerName = "userABC name",
                    Address = "userABC Address",
                    Password = "userABCPassword",
                    Balance = 100.0,
                    FilePath = "/path/userABC"
                });

                dbContext.Users.Add(new Person
                {
                    Id = "demo_auth_userQWE",
                    UserName = "userQWE",
                    CustomerName = "userQWE name",
                    Address = "userQWE Address",
                    Password = "userQWEPassword",
                    Balance = 100.0,
                    FilePath = "/path/userQWE"
                });

                dbContext.SaveChanges();

            }
        }

        [Fact]
        public async Task AuthServiceImpl_CheckUserCredentials_ReturnFalse_WhenUserNotFound()
        {
            var loginDetails = new Login
            {
                Username = "notFoundUsername",
                Password = "noPassword"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginDetails.Username)).ReturnsAsync(null as Person);
             var result = await _authServiceImpl.CheckUserCredentials(loginDetails);

            Assert.False(result);
        
        }

        [Fact]
        public async Task AuthServiceImpl_CheckUserCredentials_ReturnFalse_WhenUserPasswordIsWrong()
        {
            var loginDetails = new Login
            {
                Username = "userQWE",
                Password = "wrongPassword"
            };

            var person = new Person
            {
                UserName = "userQWE",
                CustomerName = "userQWE name",
                Address = "userQWE Address",
                Password = "userQWEPassword",
                Balance = 100.0,
                FilePath = "/path/userQWE"

            };
            _userManagerMock.Setup(um => um.FindByNameAsync(loginDetails.Username)).ReturnsAsync(person);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(person, loginDetails.Password)).ReturnsAsync(false);
            var result = await _authServiceImpl.CheckUserCredentials(loginDetails);

            Assert.False(result);

        }

        [Fact]
        public async Task AuthServiceImpl_CheckUserCredentials_ReturnSucces_WhenUserCredentialsAreCorrect()
        {
            var loginDetails = new Login
            {
                Username = "userQWE",
                Password = "wrongPassword"
            };

            var person = new Person
            {
                UserName = "userQWE",
                CustomerName = "userQWE name",
                Address = "userQWE Address",
                Password = "userQWEPassword",
                Balance = 100.0,
                FilePath = "/path/userQWE"
            };
            _userManagerMock.Setup(um => um.FindByNameAsync(loginDetails.Username)).ReturnsAsync(person);
            _userManagerMock.Setup(um => um.CheckPasswordAsync(person, loginDetails.Password)).ReturnsAsync(true);
            var result = await _authServiceImpl.CheckUserCredentials(loginDetails);

            Assert.True(result);

        }

        [Fact]
        public async Task AuthServiceImpl_GenerateTokenString_ReturnSucces()
        {
            var person = _applicationDbContext.Users.FirstOrDefault(u => u.UserName == "userQWE");
           //var person = new Person
           // {
           //     UserName = "user2",
           //     CustomerName = "user2 name",
           //     Address = "user2 Address",
           //     Password = "user2Password",
           //     Balance = 100.0,
           //     FilePath = "/path/user2"

           // };

            //var roleList = _applicationDbContext.Roles.ToList();

            var roleList = new List<string> { "User" };

            _userManagerMock.Setup(um => um.GetRolesAsync(person)).ReturnsAsync(roleList);

            var resultToken = await _authServiceImpl.GenerateTokenString(person);

            Assert.NotNull(resultToken);

            Assert.IsType<string>(resultToken);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(resultToken);

            Assert.Equal("userQWE", jwtToken.Claims.First(c => c.Type == "username").Value);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "User");

        }

        //[Fact]
        //private void AuthServiceImpl_ValidateToken_ReturnsClaimsPrincipal()
        //{

        //    var token = GenerateExpiredToken();

        //    //var result = _authServiceImpl.ValidateToken(token);
        //    var exception = Assert.Throws<SecurityTokenExpiredException>(() => _authServiceImpl.ValidateToken(token));

        //    Assert.NotNull(exception);
        //}



        //private string GenerateExpiredToken()
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.UTF8.GetBytes("your_secret_key_here");
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new Claim[]
        //        {
        //            new Claim(ClaimTypes.Name, "testuser")
        //        }),
        //        Expires = DateTime.UtcNow.AddDays(-1), // Set the expiration to a past date
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        //        Issuer = "testIssuer",
        //        Audience = "testAudience"
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}
