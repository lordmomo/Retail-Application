using AutoMapper;
using DemoWebApplication.Constants;
using DemoWebApplication.Controllers;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceInterface;
using DemoWebTestAppication.Models;
using DemoWebTestAppication.Utils;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Owin.BuilderProperties;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DemoWebTestAppication.ShopApp.Tests.Controller
{
    public class UserControllerTests
    {
        private Mock<IUserInterface> _userInterfaceMock;
        private Mock<UserManager<Person>> _userManagerMock;

        private DbContextOptions<ApplicationDbContext> _dbContextOptions;


        private readonly MockApplicationDb _mockApplicationDb;

        private ApplicationDbContext dbContext;

        private Mock<IMapper> _mapperMock;
        private readonly UserController _userController;

        private readonly string _testImagePath;
        //private Mock<DbSet<Person>> _dbSetMock;



        public UserControllerTests()
        {
            _userInterfaceMock = new Mock<IUserInterface>();
            _mapperMock = new Mock<IMapper>();

            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(databaseName: "TestDatabaseForUserController")
              .Options;

            //_mockApplicationDb = new MockApplicationDb();
            //_dbContext = _mockApplicationDb.GetApplicationDbContext();

            dbContext = new ApplicationDbContext(_dbContextOptions, null);

            SeedDatabase();

            var userStore = new Mock<IUserStore<Person>>();
            _userManagerMock = new Mock<UserManager<Person>>(userStore.Object, null, null, null, null, null, null, null, null);

            _userController = new UserController(_userInterfaceMock.Object, _userManagerMock.Object, dbContext, _mapperMock.Object);

            _testImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profile-Images/");
            
            if (!Directory.Exists(_testImagePath))
            {
                Directory.CreateDirectory(_testImagePath);
            }


        }

        private void SeedDatabase()
        {
            using (var context = new ApplicationDbContext(_dbContextOptions, null))
            {
                context.Users.Add(new Person
                {
                    UserName = "testuser",
                    CustomerName = "Old Name",
                    Address = "Old Address",
                    Password = "OldPassword",
                    Balance = 100.0,
                    FilePath = "/old/path"
                });


                dbContext.Database.EnsureDeleted();
                dbContext.Database.EnsureCreated();
                var user1 = new Person
                {
                    UserName = "user1",
                    CustomerName = "user1 name",
                    Address = "user1 Address",
                    Password = "user1Password",
                    Balance = 100.0,
                    FilePath = "/path/user1"
                };

                var user2 = new Person
                {
                    UserName = "user2",
                    CustomerName = "user2 name",
                    Address = "user2 Address",
                    Password = "user2Password",
                    Balance = 100.0,
                    FilePath = "/path/user2"
                };

                var user3 = new Person
                {
                    UserName = "user3",
                    CustomerName = "user3 name",
                    Address = "user3 Address",
                    Password = "user3Password",
                    Balance = 10000.0,
                    FilePath = "/path/user3"
                };

                dbContext.Users.AddRange(user1, user2, user3);


                var item1 = new Item
                {
                    ProductId = 1,
                    ProductName = "Widget",
                    ProductDescription = "A useful widget.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 19.99,
                    ProductQty = 100
                };
                var item2 = new Item
                {
                    ProductId = 2,
                    ProductName = "Gizmo",
                    ProductDescription = "An innovative gizmo.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 29.99,
                    ProductQty = 50
                };

                var item3 = new Item
                {
                    ProductId = 3,
                    ProductName = "Thingamajig",
                    ProductDescription = "A versatile thingamajig.",
                    ProductCategoryName = "Tools",
                    ProductPrice = 39.99,
                    ProductQty = 200
                };
                dbContext.Items.AddRange(item1, item2, item3);

                var adminRole = new RoleModel
                {
                    Id = "1",
                    Name = "Admin"
                };

                var user2Role = new RoleModel
                {
                    Id = "2",
                    Name = "User"
                };
                //var user3Role = new RoleModel
                //{
                //    Id = "3",
                //    Name = "User"
                //};

                dbContext.Roles.AddRange(adminRole, user2Role);

                dbContext.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = user1.Id,
                    RoleId = adminRole.Id
                });

                dbContext.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = user2.Id,
                    RoleId = user2Role.Id
                });


                dbContext.Sales.AddRange(
                    new Sales
                    {
                        TransactionId = 1,
                        UserId = user1.Id,
                        Person = null,
                        date_of_sale = DateTime.Now,
                        ProductId = 1,
                        Item = null
                    },
                    new Sales
                    {
                        TransactionId = 2,
                        UserId = user2.Id,
                        Person = null,
                        date_of_sale = DateTime.Now,
                        ProductId = 2,
                        Item = null
                    },
                    new Sales
                    {
                        TransactionId = 3,
                        UserId = user1.Id,
                        Person = null,
                        date_of_sale = DateTime.Now,
                        ProductId = 3,
                        Item = null
                    }); ;


                dbContext.FavouriteItems.Add(
                     new FavouriteItem
                     {
                         UserId = user1.Id,
                         User = null,
                         ProductId = item1.ProductId,
                         Product = null
                     }
                 );

                dbContext.Roles.AddRange(
                       new IdentityRole { Id = "qa1", Name = "QA", NormalizedName = "QA" },
                       new IdentityRole { Id = "mod2", Name = "Moderator", NormalizedName = "MODERATOR" }
                   );
                dbContext.SaveChanges();


            }
        }

        [Fact]
        public async Task UserController_RegisterUserForm_ReturnsJson()
        {
            var registerUser = new RegisterUser
            {
                CustomerName = "John Doe",
                Address = "123 Main St",
                Username = "johndoe",
                Password = "TestPassword123",
                Balance = 1000.0,
                FilePath = "/path/to/file"
            };

            var person = new Person
            {
                UserName = registerUser.Username,
                PasswordHash = "HashedPassword",
                CustomerName = registerUser.CustomerName,
                Address = registerUser.Address,
                Password = registerUser.Password,
                Balance = registerUser.Balance,
                FilePath = registerUser.FilePath
            };

            _mapperMock.Setup(m => m.Map<Person>(It.IsAny<RegisterUser>())).Returns(person);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<Person>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);


            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<Person>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);


            var result = await _userController.RegisterUserForm(registerUser) as ApiResponse;


            Assert.NotNull(result);
            Assert.IsType<ApiResponse>(result);

            Assert.True(result.success);
            Assert.Equal(Message.SuccessRegisteringUser, result.message);



        }



        [Fact]
        public async Task UserController_RegisterUserForm_ReturnsError_WhenUserRegistrationFails()
        {
            
            var registerUser = new RegisterUser
            {
                CustomerName = "Jane Doe",
                Address = "456 Elm St",
                Username = "janedoe",
                Password = "AnotherPassword123",
                Balance = 200.0,
                FilePath = "/path/to/anotherfile"
            };

            var person = new Person
            {
                UserName = registerUser.Username,
                PasswordHash = "HashedPassword",
                CustomerName = registerUser.CustomerName,
                Address = registerUser.Address,
                Password = registerUser.Password,
                Balance = registerUser.Balance,
                FilePath = registerUser.FilePath
            };

            _mapperMock.Setup(m => m.Map<Person>(It.IsAny<RegisterUser>())).Returns(person);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<Person>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<Person>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _userController.RegisterUserForm(registerUser);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ErrorRegisteringUser, result.message);
        }


        [Fact]
        public async Task UserController_RegisterUserForm_ReturnsError_WhenUserIsNull()
        {
            
            var result = await _userController.RegisterUserForm(null);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.UserNotFound, result.message);
        }


        [Fact]
        public async Task UserController_GetProfileImage_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var username = "doesNotExist";

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(null as Person);


            var result = await _userController.GetProfileImage(username) as NotFoundObjectResult;


            var response = result.Value;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(Message.UserNotFound, response);

        }

        [Fact]
        public async Task UserController_GetProfileImage_ReturnsNotFOund_WhenFileNotFound()
        {
            var username = "hari";

            var user = new Person
            {
                FilePath = "noFilePath.jpg"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(user);


            var result = await _userController.GetProfileImage(username) as NotFoundObjectResult;


            Assert.NotNull(result);

            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);

            Assert.Equal(Message.FileNotFound, result.Value);


        }

        [Fact]
        public async Task UserController_GetProfileImage_ReturnsFile_WhenFileExists()
        {
            string username = "harry";
            var fileName = "testImage.jpg";

            var filePath = Path.Combine(_testImagePath, fileName);

            var user = new Person { FilePath = filePath };


            await File.WriteAllBytesAsync(filePath, new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 });


            _userManagerMock.Setup(um => um.FindByNameAsync(username)).ReturnsAsync(user);


            var result = await _userController.GetProfileImage(username) as FileContentResult;


            Assert.NotNull(result);
            Assert.Equal("image/jpeg", result.ContentType);
            Assert.Equal(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }, result.FileContents);


        }


        [Fact]
        public async Task UserController_AddProfilePicture_ReturnsBadRequest_WhenNoFileUploaded()
        {
            var username = "testuser";
            IFormFile file = null;

            var result = await _userController.AddProfilePicture(file, username) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
            Assert.Equal(Message.NoFileUploadedMessage, result.Value);
        }


        [Fact]
        public async Task UserController_AddProfilePicture_ReturnsNotFound_WhenUserDoesNotExist()
        {
            var username = "testuser";
            var file = new FormFile(new MemoryStream(new byte[0]), 0, 2, "file", "test.jpg");

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(new PersonDto { Id = "userId" });
            _userManagerMock.Setup(um => um.FindByIdAsync("userId")).ReturnsAsync(null as Person);

            var result = await _userController.AddProfilePicture(file, username) as NotFoundObjectResult;


            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(Message.UserNotFound, result.Value);
        }

        [Fact]
        public async Task UserController_AddProfilePicture_ReturnsSuccess_WhenFileIsUploadedAndUserUpdated()
        {
            var username = "testuser";
            var fileName = "testimage.jpg";
            var uniqueFilename = "unique_" + fileName;
            var file = new FormFile(new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }), 0, 4, "file", fileName);
            var user = new Person { Id = "userId" };
            var userDto = new PersonDto { Id = user.Id };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(userDto);
            _userManagerMock.Setup(um => um.FindByIdAsync(userDto.Id)).ReturnsAsync(user);
            _userInterfaceMock.Setup(ui => ui.GetUniqueFilename(fileName)).Returns(uniqueFilename);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Success);

            var result = await _userController.AddProfilePicture(file, username) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(Message.ProfilePictureUpdatedMessage, result.Value);
            var filePath = Path.Combine(_testImagePath, uniqueFilename);
            Assert.True(File.Exists(filePath));
            File.Delete(filePath);
        }

        [Fact]
        public async Task UserController_AddProfilePicture_ReturnsFailed_WhenFileIsUploadedAndUserUpdated()
        {
            var username = "testuser";
            var fileName = "testimage.jpg";
            var uniqueFilename = "unique_" + fileName;
            var file = new FormFile(new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }), 0, 4, "file", fileName);
            var user = new Person { Id = "userId" };
            var userDto = new PersonDto { Id = user.Id };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(userDto);
            _userManagerMock.Setup(um => um.FindByIdAsync(userDto.Id)).ReturnsAsync(user);
            _userInterfaceMock.Setup(ui => ui.GetUniqueFilename(fileName)).Returns(uniqueFilename);
            _userManagerMock.Setup(um => um.UpdateAsync(user)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error" }));

            var result = await _userController.AddProfilePicture(file, username) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(Message.ErrorUpdatingProfilePicture, result.Value);
            var filePath = Path.Combine(_testImagePath, uniqueFilename);
            Assert.True(File.Exists(filePath));
            File.Delete(filePath);
        }

        [Fact]
        public async Task UserController_AddProfilePicture_ReturnsInternalServerError_WhenExceptionThrown()
        {
            var username = "testuser";
            var file = new FormFile(new MemoryStream(new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }), 0, 4, "file", "test.jpg");

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Throws(new Exception("Test exception"));


            var result = await _userController.AddProfilePicture(file, username) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
            Assert.Equal(Message.InternalServerError, result.Value);
        }


        [Fact]
        public void UserController_ShowUserData_ReturnsUser()
        {
            var username = "daniel";

            var user = new PersonDto
            {
                Username = "Danzel"
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(user);

            var result = _userController.ShowUserData(username) as JsonResult;

            var response = result.Value;

            Assert.NotNull(result);
            Assert.Equal(user, response);
        }


        [Fact]
        public void UserController_ShowUserData_ReturnsNotFoundStatusCode_WhenUserNotFound()
        {
            var username = "daniel";

            var user = new PersonDto
            {
                Username = "Danzel"
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(null as PersonDto);

            var result = _userController.ShowUserData(username) as ObjectResult;

            var response = result.Value;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(Message.UserNotFound, response);
        }

        [Fact]
        public void UserController_ViewAllUsers_ReturnsUserList()
        {
            List<PersonDto> userList = [
                 new PersonDto{Username = "Danzel"},
                 new PersonDto{Username = "Alex"}
            ];

            _userInterfaceMock.Setup(ui => ui.GetAlUsers()).Returns(userList);

            var result = _userController.ViewAllUsers() as JsonResult;
            Assert.NotNull(result);
            Assert.IsType<List<PersonDto>>(result.Value);

            var response = result.Value as List<PersonDto>;

            Assert.Equal(userList.Count, response.Count);
            Assert.Contains(userList, u => u.Username == "Alex");
            Assert.Contains(userList, u => u.Username == "Danzel");
        }

        [Fact]
        public void UserController_UserTransaction_ReturnTransactionHistory()
        {
            var username = "Alex";

            var transactionHistory = new List<SalesHistoryResponse>
            {
                new SalesHistoryResponse(1, 101, DateTime.Now),
                new SalesHistoryResponse(2, 102, DateTime.Now.AddDays(-1))
            };
            _userInterfaceMock.Setup(ui => ui.GetUserTransactionHistory(username)).Returns(transactionHistory);

            var result = _userController.UserTransaction(username) as JsonResult;

            Assert.NotNull(result);
            Assert.IsType<List<SalesHistoryResponse>>(result.Value);

            var response = result.Value as List<SalesHistoryResponse>;

            Assert.Equal(transactionHistory.Count, response.Count);

        }


        [Fact]
        public void UserController_UserTransaction_ReturnsNoUserTransactionMessage_WhenNotFound()
        {
            var username = "daniel";

            List<SalesHistoryResponse>? value = null;

            _userInterfaceMock.Setup(ui => ui.GetUserTransactionHistory(username))
                              .Returns(value);

            var result = _userController.UserTransaction(username) as JsonResult;


            Assert.NotNull(result);
            Assert.Equal(Message.NoUserTransactionMessage, result.Value);
        }

        [Fact]
        public void UserController_EditUserDetailsDto_ReturnsNotFound_WhenUserNotFound()
        {
            string username = "Alex";


            var editUserDetailsDto = new EditUserDetailsDto()
            {
                CustomerName = "dan",
                Address = "ktm"
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(null as PersonDto);

            //_mapperMock.Setup(m => m.Map<Person>(It.IsAny<RegisterUser>())).Returns(person);

            //_mapperMock.Setup(m => m.Map<Person>(person)).Returns(person);

            var result = _userController.EditUserDetails(editUserDetailsDto, username) as NotFoundObjectResult;
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(Message.UserNotFound, result.Value);
        }


        [Fact]
        public void UserController_EditUserDetails_ReturnsUserDetailsUpdatedMessage()
        {

            var username = "user1";
            var userDetails = new EditUserDetailsDto
            {
                CustomerName = "New Name user1",
                Address = "New Address user1"
            };

            var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);


            var person = new Person
            {
                UserName = "user1",
                CustomerName = "user1 name",
                Address = "user1 Address",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
            };
            var personDto = new PersonDto
            {
                Id = user.Id,
                Username = "user1",
                CustomerName = "user1 name",
                Address = "user1 Address",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(personDto);

            _mapperMock.Setup(m => m.Map<Person>(personDto)).Returns(person);

            var result = _userController.EditUserDetails(userDetails, username) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(Message.UserDetailsUpdatedMessage, result.Value);

            var dbUser = dbContext.Users.SingleOrDefault(u => u.UserName == username);
            Assert.NotNull(result);
            Assert.Equal(Message.UserDetailsUpdatedMessage, result.Value);
            Assert.Equal("New Name user1", dbUser.CustomerName);
            Assert.Equal("New Address user1", dbUser.Address);

            //using (var context = new ApplicationDbContext(_dbContext, null))
            //{
            //    var person = context.Users.SingleOrDefault(u => u.UserName == username);

            //    Assert.NotNull(result);
            //    Assert.Equal(Message.UserDetailsUpdatedMessage, result.Value);
            //    Assert.Equal("New Name", person.CustomerName);
            //    Assert.Equal("New Address", person.Address);

            //}
        }

        [Fact]
        public async Task UserController_EditPassword_ReturnJsonSuccess()
        {
            var username = "user1";
            var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "user1Password",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"
            };


            var person = new Person
            {
                UserName = "user1",
                CustomerName = "user1 name",
                Address = "user1 Address",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
            };
            var personDto = new PersonDto
            {
                Id = user.Id,
                Username = "user1",
                CustomerName = "user1 name",
                Address = "user1 Address",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(personDto);

            _userManagerMock
                .Setup(um => um.ChangePasswordAsync(It.IsAny<Person>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            //_userManagerMock
            //    .Setup(um => um.ChangePasswordAsync(person, changePasswordDto.OldPassword.ToString(), changePasswordDto.NewPassword.ToString()))
            //    .ReturnsAsync(IdentityResult.Success);

            var result = await _userController.EditPassword(username, changePasswordDto) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(Message.UserPasswordUpdatedMessage, result.Value);

            //var response = result.Value;

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var user = context.Users.SingleOrDefault(u => u.UserName == username);

            //    Assert.NotNull(result);
            //    //Assert.Equal("Old Name", user.CustomerName);
            //    Assert.Equal("newPassword", user.Password);

            //}

        }

        [Fact]
        public async Task UserController_EditPassword_ReturnsError_WhenFailedUserPasswordChange()
        {
            var username = "user1";
            var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "user1Password",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"
            };
           

            var person = new Person
            {
                UserName = "user1",
                CustomerName = "user1 name",
                Address = "user1 Address",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
            };
            var personDto = new PersonDto
            {
                    Id = user.Id,
                    Username = "user1",
                    CustomerName = "user1 name",
                    Address = "user1 Address",
                    Password = "user1Password",
                    Balance = 100.0,
                    FilePath = "/path/user1"
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(personDto);

            _userManagerMock
                .Setup(um => um.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to change password" }));

            var result = await _userController.EditPassword(username, changePasswordDto) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(Message.FailedUserPasswordChangeMessage, result.Value);

        }


        [Fact]
        public async Task UserController_EditPassword_InvalidOldPassword_ReturnsPasswordWrongMessage()
        {
            var username = "user1";
            var user = dbContext.Users.FirstOrDefault(u => u.UserName == username);
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "Password",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"
            };

            var personDto = new PersonDto
            {
                    Id = user.Id,
                    Username = "user1",
                    CustomerName = "user1 name",
                    Address = "user1 Address",
                    Password = "user1Password",
                    Balance = 100.0,
                    FilePath = "/path/user1"
                
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(personDto);
            //_userManagerMock.Setup(um => um.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
            //    .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = " error while changing password" }));

            var result = await _userController.EditPassword(username, changePasswordDto) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(Message.UserPasswordWrongMessage, result.Value);
        }

        [Fact]
        public async Task UserController_EditPassword_UserNotFound_ReturnsNotFound()
        {
            var username = "nonexistentuser";
            var changePasswordDto = new ChangePasswordDto
            {
                OldPassword = "OldPassword",
                NewPassword = "NewPassword",
                ConfirmPassword = "NewPassword"

            };

            _userInterfaceMock
                .Setup(ui => ui.GetUserByUsername(username))
                .Returns(null as PersonDto);

            var result = await _userController.EditPassword(username, changePasswordDto) as ObjectResult;

            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
            Assert.Equal(Message.UserNotFound, result.Value);
        }

    }


}
