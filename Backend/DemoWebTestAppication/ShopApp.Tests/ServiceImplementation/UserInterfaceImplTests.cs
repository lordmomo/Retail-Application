using AutoMapper;
using DemoWebApplication.Constants;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceImplementation;
using DemoWebApplication.Service.ServiceInterface;
using DemoWebTestAppication.Utils;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebTestAppication.ShopApp.Tests.ServiceImplementation
{
    public class UserInterfaceImplTests
    {

        private readonly DbContextOptions<ApplicationDbContext> _dbOptions;
        private readonly ApplicationDbContext _applicationDbContext;

        private readonly MockApplicationDb _mockApplicationDb;
        private readonly ApplicationDbContext _dbContext;
        private readonly Mock<IMapper> _mapperMock;

        private UserIntefaceImpl _userInterfaceImpl;

        public UserInterfaceImplTests()
        {

            _mapperMock = new Mock<IMapper>();

            _mockApplicationDb = new MockApplicationDb();
            _dbContext = _mockApplicationDb.GetApplicationDbContext();  
            _userInterfaceImpl = new UserIntefaceImpl(_dbContext, _mapperMock.Object);

        }
        [Fact]
        public void UserIntefaceImpl_GetAlUsers_ReturnsUserList()
        {
            var users = _mockApplicationDb.GetApplicationDbContext().Users.ToList();

            var userDtos = users.Select(user => new PersonDto
            {
                Id = user.Id,
                Username = user.UserName,
                CustomerName = user.CustomerName,
                Address = user.Address,
                Password = user.Password,
                Balance = user.Balance,
                FilePath = user.FilePath
            }).ToList();

            _mapperMock.Setup(m => m.Map<PersonDto>(It.IsAny<Person>())).Returns(
                (Person person) => userDtos.First(dto => dto.Username == person.UserName)
                );
            var result = _userInterfaceImpl.GetAlUsers();
            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Count);

            for (int i = 0; i < users.Count; i++)
            {
                Assert.Equal(userDtos[i].Username, result[i].Username);
                Assert.Equal(userDtos[i].CustomerName, result[i].CustomerName);
                Assert.Equal(userDtos[i].Address, result[i].Address);
                Assert.Equal(userDtos[i].Password, result[i].Password);
                Assert.Equal(userDtos[i].Balance, result[i].Balance);
                Assert.Equal(userDtos[i].FilePath, result[i].FilePath);
            }
        }

        //[Fact]
        //public async Task UserIntefaceImpl_GetRoleOfUserAsync_UserHasNoRole_ReturnsUserNotAssignedWithRole()
        //{
        //    var userId = "userWithoutRole";

        //    var result = await _userInterfaceImpl.getRoleofUser(userId);

        //    Assert.Equal("User not assigned with role", result);
        //}

        //[Fact]
        //public async Task UserIntefaceImpl_GetRoleOfUserAsync_RoleDoesNotExist_ReturnsRoleNotFound()
        //{
        //    var userId = "userWithInvalidRole"; 

        //    var result = await _userInterfaceImpl.getRoleofUser(userId);

        //    Assert.Equal("Role not found", result);
        //}

        [Fact]
        public async Task UserIntefaceImpl_GetRoleOfUser_ReturnsRoleName()
        {
            var newUser = new Person
            {
                UserName = "user411",
                CustomerName = "user411 name",
                Address = "user411 Address",
                Password = "user411Password",
                Balance = 100.0,
                FilePath = "/path/user411"
            };

            _dbContext.Users.Add(newUser);

            _dbContext.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = newUser.Id,
                RoleId = "1"
            });

            _dbContext.SaveChanges();
            var userId = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user411")?.Id;

            var resultRolename = await _userInterfaceImpl.getRoleofUser(userId);

            Assert.NotNull(resultRolename);
            Assert.Equal("Admin", resultRolename);

        }

        [Fact]
        public async Task GetRoleofUser_UserHasNoRole_ReturnsUserNotAssignedWithRole()
        {
            // Arrange
            var userId = "user3"; // User3 has no role assigned

            // Act
            var result = await _userInterfaceImpl.getRoleofUser(userId);

            // Assert
            Assert.Equal("User not assigned with role", result);
        }


        [Fact]
        public async Task GetRoleofUser_RoleNotFound_ReturnsRoleNotFound()
        {
            // Arrange
            var userId = "user2"; 

            var role = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
            if (role != null)
            {
                _dbContext.Roles.Remove(role);
                await _dbContext.SaveChangesAsync();
            }

            // Act
            var result = await _userInterfaceImpl.getRoleofUser(userId);

            // Assert
            Assert.Equal("User not assigned with role", result);
        }


        [Fact]
        public void UserIntefaceImpl_GetUniqueFilename_ReturnsSuccess()
        {
            var fileName = "newFile";

            var result = _userInterfaceImpl.GetUniqueFilename(fileName);

            Assert.NotNull(result);
            Assert.StartsWith("newFile_", result);
            Assert.Equal(12, result.Length);
        }

        [Fact]
        public void UserIntefaceImpl_GetUserByUsername()
        {
            var person = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user1");

            var personDto = new PersonDto
            {
                Id = person.Id,
                Username = "user1",
                CustomerName = "user1 name",
                Address = "user1 Address",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
            };

            _mapperMock.Setup(m => m.Map<PersonDto>(person)).Returns(personDto);
            var result = _userInterfaceImpl.GetUserByUsername(person.UserName);
            Assert.NotNull(result);

            Assert.IsType<PersonDto>(result);
            Assert.Equal(personDto.Username, result.Username);

        }

        [Fact]
        public void UserIntefaceImpl_GetUserTransactionHistory_ReturnsNull_WhenUserNotFound()
        {

            var username = "nonexistentUser";
            var result = _userInterfaceImpl.GetUserTransactionHistory(username);

            Assert.Null(result);

        }

        [Fact]
        public void UserIntefaceImpl_GetUserTransactionHistory_ReturnsListOfTransaction()
        {
            var person = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user1");

            var expectedTransactions = _mockApplicationDb.GetApplicationDbContext().Sales.Where(u => u.UserId == person.Id).ToList();

            var result = _userInterfaceImpl.GetUserTransactionHistory(person.UserName);
            Assert.NotNull(result);

            Assert.IsType<List<SalesHistoryResponse>>(result);

            Assert.Equal(expectedTransactions.Count, result.Count);

            foreach (var expectedTransaction in expectedTransactions)
            {
                var actualResultList = result.FirstOrDefault(u => u.TransactionId == expectedTransaction.TransactionId);
                Assert.NotNull(actualResultList);
                Assert.Equal(expectedTransaction.ProductId, actualResultList.TransactionId);
                Assert.Equal(expectedTransaction.date_of_sale, actualResultList.Date_of_sale);

            }
        }

        [Fact]
        public void UserIntefaceImpl_GetUserTransactionHistory_ReturnsNull_WhenUserHasNoTransaction()
        {
            var person = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user3");

            var expectedTransactions = _mockApplicationDb.GetApplicationDbContext().Sales.Where(u => u.UserId == person.Id).ToList();

            var result = _userInterfaceImpl.GetUserTransactionHistory(person.UserName);
            Assert.Empty(result);
        }


        [Fact]
        public void UserIntefaceImpl_SaveFavouriteItem_ReturnsErrorMessage_WhenItemNotProvided()
        {
            var result = _userInterfaceImpl.saveFavouriteItem(Mock.Of<PersonDto>(), null);

            Assert.NotNull(result);
            Assert.Equal(Message.ItemNotFound, result);
        }

        [Fact]
        public void UserIntefaceImpl_SaveFavouriteItem_ReturnsErrorMessage_WhenUserNotProvided()
        {
            var result = _userInterfaceImpl.saveFavouriteItem(null, Mock.Of<SelectedItem>());

            Assert.NotNull(result);
            Assert.Equal(Message.UserNotFound, result);
        }

       

        [Fact]
        public void UserIntefaceImpl_SaveFavouriteItem_ReturnsSuccessMessage_WhenItemIsNotAlreadyInFavouritesOfUserButIsFavoriteInOthers()
        {
            var userId = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user2")?.Id;

            var personDto = new PersonDto { Id = userId };
            var selectedItem = new SelectedItem { ProductId = 1 };



            var favouriteItem = new FavouriteItem
            {
                FavouriteItemId = 15,
                UserId = personDto.Id,
                ProductId = 1
            };

            var favouriteItemDto = new FavouriteItemDto
            {
                FavouriteItemId = favouriteItem.FavouriteItemId,
                UserId = personDto.Id,
                ProductId = 1
            };


            _mapperMock.Setup(m => m.Map<FavouriteItem>(It.IsAny<FavouriteItemDto>()))
               .Returns(favouriteItem);

            var result = _userInterfaceImpl.saveFavouriteItem(personDto, selectedItem);

            Assert.Equal(Message.ItemAddedInFavourites, result);

            var favouriteItemCheck = _mockApplicationDb.GetApplicationDbContext().FavouriteItems
                .FirstOrDefault(fi => fi.UserId == personDto.Id && fi.ProductId == selectedItem.ProductId);
            Assert.NotNull(favouriteItemCheck);

        }


        [Fact]
        public void UserIntefaceImpl_SaveFavouriteItem_ReturnsSuccessMessage_WhenItemIsNotAlreadyInFavouritesListInDb()
        {
            var userId = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user2")?.Id;

            var personDto = new PersonDto { Id = userId };

            var selectedItem = new SelectedItem { ProductId = 123 };



            var favouriteItem = new FavouriteItem
            {
                FavouriteItemId = 13,
                UserId = personDto.Id,
                ProductId = 1
            };


            var favouriteItemDto = new FavouriteItemDto
            {
                FavouriteItemId = favouriteItem.FavouriteItemId,
                UserId = personDto.Id,
                ProductId = 1
            };


            _mapperMock.Setup(m => m.Map<FavouriteItem>(It.IsAny<FavouriteItemDto>()))
               .Returns(favouriteItem);

            var result = _userInterfaceImpl.saveFavouriteItem(personDto, selectedItem);

            Assert.Equal(Message.ItemAddedInFavourites, result);


        }

        [Fact]
        public void UserIntefaceImpl_SaveFavouriteItem_ReturnsErrorMessage_WhenItemAlreadyInFavourites()
        {
            var userId = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user1")?.Id;
            var personDto = new PersonDto { Id = userId };

            var selectedItem = new SelectedItem { ProductId = 1 };

            var result = _userInterfaceImpl.saveFavouriteItem(personDto, selectedItem);

            Assert.Equal(Message.ItemAlreadyInFavourites, result);
        }

        [Fact]
        public void UserIntefaceImpl_SaveFavouriteItem_ShouldReturnInternalServerError_WhenAddingNewItemNotInFavoriteListInDb()
        {
            var userId = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user1")?.Id;
            var personDto = new PersonDto { Id = userId };
            var selectedItem = new SelectedItem { ProductId = 999 };



            _mapperMock.Setup(m => m.Map<FavouriteItem>(It.IsAny<FavouriteItemDto>()))
                       .Throws(new Exception("Mapping error"));
           
            var result = _userInterfaceImpl.saveFavouriteItem(personDto, selectedItem);

            
            Assert.Equal(Message.InternalServerError, result);
        }

        [Fact]
        public void UserIntefaceImpl_SaveFavouriteItem_ShouldReturnInternalServerError_WhenAddingExistingItemInFavoriteListInDb()
        {
            var userId = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user2")?.Id;
            var personDto = new PersonDto { Id = userId };
            var selectedItem = new SelectedItem { ProductId = 1 };



            _mapperMock.Setup(m => m.Map<FavouriteItem>(It.IsAny<FavouriteItemDto>()))
                       .Throws(new Exception("Mapping error"));
           
            var result = _userInterfaceImpl.saveFavouriteItem(personDto, selectedItem);


            Assert.Equal(Message.InternalServerError, result);
        }

        [Fact]
        public void UserIntefaceImpl_AddFavoriteItem_ReturnsTrue() 
        {
            var userId = "user2";
            var productId = 3;
          

            var favouriteItem = new FavouriteItem
            {
                FavouriteItemId = 11,
                UserId = userId,
                ProductId = productId
            };

            var favouriteItemDto = new FavouriteItemDto
            {
                FavouriteItemId = favouriteItem.FavouriteItemId,
                UserId = userId,
                ProductId = productId
            };

            _mapperMock.Setup(m => m.Map<FavouriteItem>(It.IsAny<FavouriteItemDto>()))
                .Returns(favouriteItem);

            var result = _userInterfaceImpl.AddFavoriteItem(userId,productId);

            Assert.True(result);

            var addedItem = _mockApplicationDb.GetApplicationDbContext().FavouriteItems
                               .FirstOrDefault(fi => fi.UserId == userId && fi.ProductId == productId);
            Assert.NotNull(addedItem);
        }

        [Fact]
        public void UserIntefaceImpl_AddFavouriteItem_ReturnsFalse_WhenItemAlreadyExistsForUserAsFavorite()
        {
            var userId = _mockApplicationDb.GetApplicationDbContext().Users.FirstOrDefault(u => u.UserName == "user1")?.Id;
            var productId = 1;

            var result = _userInterfaceImpl.AddFavoriteItem(userId,productId);

            Assert.False(result);

        }

        [Fact]
        public void AddFavoriteItem_WhenUserDoesNotExist_ShouldReturnFalse()
        {
            var userId = "NonExistentUserId";
            var productId = 1;

            var result = _userInterfaceImpl.AddFavoriteItem(userId, productId);

            Assert.False(result);
        }

        [Fact]
        public void AddFavoriteItem_WhenProductDoesNotExist_ShouldReturnFalse()
        {
            
            var userId = "1";
            var productId = 999; 

            
            var result = _userInterfaceImpl.AddFavoriteItem(userId, productId);

            Assert.False(result);
        }

        [Fact]
        public void AddFavoriteItem_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            //var userId = "user1";
            //var productId = 2;

            //_mapperMock.Setup(m => m.Map<FavouriteItem>(It.IsAny<FavouriteItemDto>()))
            //           .Throws(new Exception("Mapping error"));

            //var result = _userInterfaceImpl.AddFavoriteItem(userId, productId);

            //Assert.False(result);

            // Arrange
            var userId = "1";
            var productId = 1;

            // Force a database exception by disposing the context
            _mockApplicationDb.GetApplicationDbContext().Dispose();

            // Act
            var result = _userInterfaceImpl.AddFavoriteItem(userId, productId);

            // Assert
            Assert.False(result);

        }

        [Fact]
        public void UserInterfaceImpl_GetUserFavoriteItems_ReturnSuccess()
        {
            var personDto = new PersonDto
            {
                Id = "user1"
            };

            var expectedItems = _mockApplicationDb.GetApplicationDbContext().Items.ToList();

            _mockApplicationDb.GetApplicationDbContext().FavouriteItems.AddRange(
                new FavouriteItem { UserId = personDto.Id, ProductId = 3 ,User = null, Product = null},
                new FavouriteItem { UserId = personDto.Id, ProductId = 2 , User = null, Product = null}
            );
            _mockApplicationDb.GetApplicationDbContext().SaveChanges();

            var result = _userInterfaceImpl.getUserFavouriteItems(personDto);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, i => i.ProductId == 3);
            Assert.Contains(result, i => i.ProductId == 2);


        }

        //[Fact]
        //public void UserIntefaceImpl_GetUserFavoriteItems_ReturnsCorrectListOfItem()
        //{
        //    var personDto = new PersonDto
        //    {
        //        Id = "user1"
        //    };

        //    var expectedItems = _mockApplicationDb.GetApplicationDbContext().Items.ToList();

        //    _mockApplicationDb.GetApplicationDbContext().FavouriteItems.AddRange(
        //        new FavouriteItem { UserId = personDto.Id, ProductId = 3 },
        //        new FavouriteItem { UserId = personDto.Id, ProductId = 2 }
        //    );
        //    _mockApplicationDb.GetApplicationDbContext().SaveChanges();

        //    var result = _userInterfaceImpl.getUserFavouriteItems(personDto);

        //    Assert.NotNull(result);
        //    Assert.Equal(2, result.Count);
        //    Assert.Contains(result, i => i.ProductId == 2);
        //    Assert.Contains(result, i => i.ProductId == 3);
        //}


        [Fact]
        public void UserInterfaceImpl_GetUserFavoriteItems_ReturnsEmpty_WhenNoFavorites()
        {
            // Arrange
            var personDto = new PersonDto { Id = "user2" };

            // Act
            var result = _userInterfaceImpl.getUserFavouriteItems(personDto);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void UserInterfaceImpl_GetUserFavoriteItems_DoesNotReturnNonExistentItems()
        {
            var personDto = new PersonDto { Id = "user1" };

            _mockApplicationDb.GetApplicationDbContext().FavouriteItems.AddRange(
                new FavouriteItem { UserId = personDto.Id,User = null, ProductId = 2, Product = null },
                new FavouriteItem { UserId = personDto.Id,User = null, ProductId = 999 ,Product = null}
            );
            _mockApplicationDb.GetApplicationDbContext().SaveChanges();

            var result = _userInterfaceImpl.getUserFavouriteItems(personDto);

            
            Assert.NotNull(result);
            Assert.Single(result); 
            Assert.Contains(result, i => i.ProductId == 2);
            Assert.DoesNotContain(result, i => i.ProductId == 999); 
        }

    }
}
