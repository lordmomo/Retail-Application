using DemoWebApplication.Constants;
using DemoWebApplication.Controllers;
using DemoWebApplication.Models;
using DemoWebTestAppication.Models;
using DemoWebTestAppication.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebTestAppication.ShopApp.Tests.Controller
{
    public class AdminControllerTests
    {

        private Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private Mock<UserManager<Person>> _userManagerMock;
        private AdminController _adminController;
        private MockApplicationDb _mockApplicationDb;

        private DbContextOptions<ApplicationDbContext> _dbContextOptions;


        private ApplicationDbContext _dbContext;
        //private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public AdminControllerTests()
        {
            var userStore = new Mock<IUserStore<Person>>();
            _userManagerMock = new Mock<UserManager<Person>>(userStore.Object, null, null, null, null, null, null, null, null);

            //_dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            //    .UseInMemoryDatabase(databaseName: "testDatabaseForAdminCOntroller")
            //    .Options;

            //_dbContext = new ApplicationDbContext(_dbContextOptions, null);
            //SeedDatabase(_dbContext);
            var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
            
            
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                roleStoreMock.Object,
                null,
                null,
                null,
                null
            );

            //_mockApplicationDb = new MockApplicationDb();
            //_dbContext = _mockApplicationDb.GetApplicationDbContext();


            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(databaseName: "TestDatabaseForAdminController")
              .Options;


            _dbContext = new ApplicationDbContext(_dbContextOptions, null);

            SeedDatabase();

            _roleManagerMock.Setup(rm => rm.Roles).Returns(_dbContext.Roles.AsQueryable());

            _adminController = new AdminController(_roleManagerMock.Object,_userManagerMock.Object);
        
        
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


                _dbContext.Database.EnsureDeleted();
                _dbContext.Database.EnsureCreated();
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

                _dbContext.Users.AddRange(user1, user2, user3);


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
                _dbContext.Items.AddRange(item1, item2, item3);

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

                _dbContext.Roles.AddRange(adminRole, user2Role);

                _dbContext.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = user1.Id,
                    RoleId = adminRole.Id
                });

                _dbContext.UserRoles.Add(new IdentityUserRole<string>
                {
                    UserId = user2.Id,
                    RoleId = user2Role.Id
                });


                _dbContext.Sales.AddRange(
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


                _dbContext.FavouriteItems.Add(
                     new FavouriteItem
                     {
                         UserId = user1.Id,
                         User = null,
                         ProductId = item1.ProductId,
                         Product = null
                     }
                 );

                _dbContext.Roles.AddRange(
                       new IdentityRole { Id = "qa1", Name = "QA", NormalizedName = "QA" },
                       new IdentityRole { Id = "mod2", Name = "Moderator", NormalizedName = "MODERATOR" }
                   );
                _dbContext.SaveChanges();


            }
        }

        //private void SeedDatabase(ApplicationDbContext context)
        //{
        //    context.Roles.AddRange(
        //        new IdentityRole { Id = "qa1", Name = "QA", NormalizedName = "QA" },
        //        new IdentityRole { Id = "mod2", Name = "Moderator", NormalizedName = "MODERATOR" }
        //    );
        //    context.SaveChanges();
        //}

        [Fact]
        public async Task AdminController_ListRoles_ReturnsListOfRoles()
        {

            var result = await _adminController.ListRoles() as JsonResult;

            Assert.NotNull(result);

            var roles = Assert.IsType<List<IdentityRole>>(result.Value);

            Assert.Equal(4, roles.Count);
            Assert.Contains(roles, r => r.Name == "QA");
            Assert.Contains(roles, r => r.Name == "Moderator");
        }


        [Fact]
        public async Task AdminController_CreateRole_ReturnsError_WhenBadRequest()
        {
            var roleDto = new RoleDto
            {
                rolename = null
            };

            var result = await _adminController.CreateRole(roleDto);

            Assert.NotNull(result);
            //var response = result.Value;

            //Assert.Equal(StatusCodes.Status400BadRequest, result.);
            Assert.Equal(Message.RoleNameBadRequestMessage, result.message);
        }

        [Fact]
        public async Task AdminController_CreateRole_ReturnsError_WhenRoleAlreadyExists()
        {
            var roleDto = new RoleDto
            {
                rolename = "admin"
            };

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleDto.rolename)).Returns(Task.FromResult(true));
            var result = await _adminController.CreateRole(roleDto);

            Assert.NotNull(result);

            Assert.Equal(Message.RoleAlreadyExists, result.message);
        }
        [Fact]
        public async Task AdminController_CreateRole_ReturnsError_WhileCreatingRole()
        {
            var roleDto = new RoleDto
            {
                rolename = "admin"
            };

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleDto.rolename)).Returns(Task.FromResult(false));

            _roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Failed( new IdentityError { Description = " Creation Failed!!"}));

            var result = await _adminController.CreateRole(roleDto);

            Assert.NotNull(result);

            Assert.False(result.success);
            Assert.Equal(Message.ErrorCreatingingRoleMessage, result.message);
        }

        [Fact]
        public async Task AdminController_CreateRole_ReturnsSuccess_WhileCreatingRole()
        {
            var roleDto = new RoleDto
            {
                rolename = "admin"
            };

            _roleManagerMock.Setup(rm => rm.RoleExistsAsync(roleDto.rolename)).Returns(Task.FromResult(false));

            _roleManagerMock.Setup(rm => rm.CreateAsync(It.IsAny<IdentityRole>())).ReturnsAsync(IdentityResult.Success);

            var result = await _adminController.CreateRole(roleDto);

            Assert.NotNull(result);

            Assert.True(result.success);
            Assert.Equal(Message.SuccessCreatingingRoleMessage, result.message);
        }

        [Fact]
        public async Task AdminController_EditRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            var model = new EditRoleDto
            {
                Id = "nonexistent-role-id",
                Name = "New Role Name",
                NormalizedName = "NEW ROLE NAME",
                concurrencyStamp = "ccNewRole23Rname"
            };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.Id))
                .ReturnsAsync(null as IdentityRole);

            var result = await _adminController.EditRole(model);

            Assert.NotNull(result);
            //var response = (dynamic)result.Value;
            Assert.False(result.success);
            Assert.Equal(Message.RoleNotFoundMessage, result.message);
        }

        [Fact]
        public async Task AdminController_EditRole_ReturnsSuccess_WhenRoleIsUpdated()
        {

            var role = _dbContext.Roles.FirstOrDefault(r => r.Name == "Admin");


            var model = new EditRoleDto
            {
                Id = role.Id,
                Name = "Updated Admin Name",
                NormalizedName = "UPDATED ROLE NAME",
                concurrencyStamp = "ccUpdatedAroleNamser21"
            };

            //var role = new IdentityRole { Id = model.Id, Name = "Old Role Name" };


            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.Id))
                .ReturnsAsync(role);

            _roleManagerMock.Setup(rm => rm.UpdateAsync(role))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _adminController.EditRole(model);

            Assert.NotNull(result);
            //var response = (dynamic)result.Value;
            Assert.True(result.success);
            Assert.Equal(Message.SuccessEditingRoleMessage, result.message);
        }

        [Fact]
        public async Task AdminController_EditRole_ReturnsError_WhenRoleUpdateFails()
        {
            var model = new EditRoleDto
            {
                Id = "existing-role-id",
                Name = "Updated Role Name",
                NormalizedName = "UPDATED ROLE NAME",
                concurrencyStamp = "ccstampexSitstingRole"
            };

            var role = new IdentityRole { Id = model.Id, Name = "Old Role Name" };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.Id))
                .ReturnsAsync(role);

            _roleManagerMock.Setup(rm => rm.UpdateAsync(role))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

            var result = await _adminController.EditRole(model) ;

            Assert.NotNull(result);
            //var response = (dynamic)result.Value;
            Assert.False(result.success);
            Assert.Equal(Message.ErrorEditingRoleMessage, result.message);
        }

        [Fact]
        public async Task AdminController_DeleteRole_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            var roleId = "nonexistent-role-id";

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(roleId))
                .ReturnsAsync((IdentityRole)null);

            var result = await _adminController.DeleteRole(roleId) ;

            Assert.NotNull(result);
            //var response = (dynamic)result.Value;
            Assert.False(result.success);
            Assert.Equal(Message.RoleNotFoundMessage, result.message);
        }

        [Fact]
        public async Task AdminController_DeleteRole_ReturnsSuccess_WhenRoleIsDeleted()
        {
            var roleId = "existing-role-id-1223";
            var role = new IdentityRole { Id = roleId, Name = "Role Name" };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _roleManagerMock.Setup(rm => rm.DeleteAsync(role))
                .ReturnsAsync(IdentityResult.Success);

            var result = await _adminController.DeleteRole(roleId);

            Assert.NotNull(result);
            //var response = (dynamic)result.Value;
            Assert.True(result.success);
            Assert.Equal(Message.SuccessDeletingRoleMessage, result.message);
        }

        [Fact]
        public async Task AdminController_DeleteRole_ReturnsError_WhenRoleDeletionFails()
        {
            var roleId = "existing-role-id-delete";
            var role = new IdentityRole { Id = roleId, Name = "Role Name" };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(roleId))
                .ReturnsAsync(role);

            _roleManagerMock.Setup(rm => rm.DeleteAsync(role))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Deletion failed" }));

            var result = await _adminController.DeleteRole(roleId);

            Assert.NotNull(result);
            //var response = (dynamic)result.Value;
            Assert.False(result.success);
            Assert.Equal(Message.ErrorDeletingRoleMessage, result.message);
        }



        [Fact]
        public async Task AdminController_AssignRoleToUser_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            var model = new UserRoleAssignmentModel
            {
                RoleId = "nonexistent-role-id",
                UserIds = new List<string> { "user1" },
                IsSelected = true
            };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.RoleId))
                .ReturnsAsync(null as IdentityRole);

            var result = await _adminController.AssignRoleToUser(model);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.RoleNotFoundMessage, result.message);
        }


        [Fact]
        public async Task AdminController_AssignRoleToUser_ReturnsNotFound_WhenUsersDoNotExist()
        {
            var role = new IdentityRole { Id = "role-id", Name = "RoleName" };
            var model = new UserRoleAssignmentModel
            {
                RoleId = role.Id,
                UserIds = new List<string> { "user1", "user2" },
                IsSelected = true
            };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.RoleId))
                .ReturnsAsync(role);

            _userManagerMock.Setup(um => um.FindByIdAsync("user4"))
                .ReturnsAsync(null as Person);
            _userManagerMock.Setup(um => um.FindByIdAsync("user3"))
               .ReturnsAsync(null as Person);
            //_userManagerMock.Setup(um => um.FindByIdAsync("user2"))
            //    .ReturnsAsync(new Person { UserName = "user2" });

            var result = await _adminController.AssignRoleToUser(model);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.AssignRoleBadRequestMessage, result.message);
            //var errors = result.Value as List<string>;
            //Assert.Contains("Failed to assign role 'RoleName' to user 'user2':", errors);
        }


        [Fact]
        public async Task AdminController_AssignRoleToUser_ReturnsSuccess_WhenRoleIsAssignedSuccessfully()
        {
            var role = new IdentityRole { Id = "role-id", Name = "RoleName" };
            var user = new Person { UserName = "user1" };

            var model = new UserRoleAssignmentModel
            {
                RoleId = role.Id,
                UserIds = new List<string> { "user1" },
                IsSelected = true
            };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.RoleId))
                .ReturnsAsync(role);

            _userManagerMock.Setup(um => um.FindByIdAsync("user1"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, role.Name))
                .ReturnsAsync(false);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, role.Name))
                .ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<Person>()))
       .ReturnsAsync(new List<string>());

            var result = await _adminController.AssignRoleToUser(model);

            Assert.NotNull(result);
            Assert.True(result.success);
            Assert.Equal(Message.RoleAssignedSuccessfullyMessage, result.message);
        }


        [Fact]
        public async Task AdminController_AssignRoleToUser_ReturnsBadRequest_WhenRoleAssignmentFails()
        {
            var role = new IdentityRole { Id = "101", Name = "nthRole" };

            
            var user = new Person { UserName = "user12" };

            var model = new UserRoleAssignmentModel
            {
                RoleId = role.Id,
                UserIds = new List<string> { "user12" },
                IsSelected = true
            };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.RoleId))
                .ReturnsAsync(role);

            _userManagerMock.Setup(um => um.FindByIdAsync("user12"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, role.Name))
                .ReturnsAsync(false);
            _userManagerMock.Setup(um => um.AddToRoleAsync(user, role.Name))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Error adding role" }));

            _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<Person>())).ReturnsAsync(new List<string>());
            var result = await _adminController.AssignRoleToUser(model);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.AssignRoleBadRequestMessage, result.message);
            //var errors = result.Value as List<string>;
            //Assert.Contains("Failed to assign role 'RoleName' to user 'user1': Error adding role", errors);
        }

        [Fact]
        public async Task AdminController_AssignRoleToUser_ReturnsSuccess_WhileRemovingOldRole()
        {
            var role = new IdentityRole { Id = "101", Name = "nthRole" };
           

            var user = new Person { UserName = "user1" };

            var model = new UserRoleAssignmentModel { 
                RoleId = role.Id, 
                UserIds = new List<string> { "user1"},
                IsSelected = true
            };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.RoleId)).ReturnsAsync(role);

            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);

            _userManagerMock.Setup(um => um.IsInRoleAsync(user, role.Name)).ReturnsAsync(true);

            _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, role.Name)).ReturnsAsync(IdentityResult.Success);


           // _userManagerMock.Setup(um => um.GetRolesAsync(user))
           //.ReturnsAsync(new List<string> { "OtherRole" }); 

           // _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, "OtherRole"))
           //     .ReturnsAsync(IdentityResult.Success);


            var result = await _adminController.AssignRoleToUser(model);

            Assert.NotNull(result);
            Assert.True(result.success);
            Assert.Equal(Message.RoleAssignedSuccessfullyMessage, result.message);

            //_userManagerMock.Verify(um => um.RemoveFromRoleAsync(user, "OtherRole"), Times.Once);

        }
        [Fact]
        public async Task AdminController_AssignRoleToUser_ReturnsFailure_WhileRemovingOldRole()
        {
            var role = new IdentityRole { Id = "1387", Name = "nthRoleFaile" };
           
            var user = new Person { UserName = "user4" };

            var model = new UserRoleAssignmentModel
            {
                RoleId = role.Id,
                UserIds = new List<string> { "user4" },
                IsSelected = true
            };

            _roleManagerMock.Setup(rm => rm.FindByIdAsync(model.RoleId)).ReturnsAsync(role);

            _userManagerMock.Setup(um => um.FindByIdAsync("user4")).ReturnsAsync(user);

            _userManagerMock.Setup(um => um.IsInRoleAsync(user, role.Name)).ReturnsAsync(true);

            _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, role.Name))
                .ReturnsAsync(IdentityResult.Failed( new IdentityError { Description = "Error removing role"}));


            // _userManagerMock.Setup(um => um.GetRolesAsync(user))
            //.ReturnsAsync(new List<string> { "OtherRole" }); 

            // _userManagerMock.Setup(um => um.RemoveFromRoleAsync(user, "OtherRole"))
            //     .ReturnsAsync(IdentityResult.Success);


            var result = await _adminController.AssignRoleToUser(model);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.AssignRoleBadRequestMessage, result.message);

            //_userManagerMock.Verify(um => um.RemoveFromRoleAsync(user, "OtherRole"), Times.Once);

        }
    }
}
