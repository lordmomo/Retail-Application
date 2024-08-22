using AutoMapper;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceImplementation;
using DemoWebTestAppication.Utils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NuGet.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebTestAppication.ShopApp.Tests.ServiceImplementation
{
    public class ShopInterfaceImplTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        //private readonly MockApplicationDb _mockApplicationDb;
        private readonly ApplicationDbContext _applicationDbContext;

        private readonly Mock<IMapper> _mapperMock;

        private readonly Mock<IMemoryCache> _memoryCacheMock;

        private ShopInterfaceImpl _shopInterfaceImpl;

        private readonly string _cacheKey = "ProductData";

        public ShopInterfaceImplTests()
        {
            _mapperMock = new Mock<IMapper>();
            _memoryCacheMock = new Mock<IMemoryCache>();

            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(databaseName: "TestDatabaseForShopImpl")
              .Options;

            //_mockApplicationDb = new MockApplicationDb();
            _applicationDbContext = new ApplicationDbContext(_dbContextOptions, null);
            //_applicationDbContext = _mockApplicationDb.GetApplicationDbContext();

            SeedDatabase();

            _shopInterfaceImpl = new ShopInterfaceImpl(_applicationDbContext, _mapperMock.Object, _memoryCacheMock.Object); ;

        }

        public void SeedDatabase()
        {
            using (var dbContext = new ApplicationDbContext(_dbContextOptions, null))
            {
                dbContext.Database.EnsureDeleted();
                dbContext.Users.Add(new Person
                {
                    UserName = "user1",
                    CustomerName = "user1 name",
                    Address = "user1 Address",
                    Password = "user1Password",
                    Balance = 100.0,
                    FilePath = "/path/user1"
                });

                dbContext.Users.Add(new Person
                {
                    UserName = "user2",
                    CustomerName = "user2 name",
                    Address = "user2 Address",
                    Password = "user2Password",
                    Balance = 100.0,
                    FilePath = "/path/user2"
                });

                dbContext.Items.Add(new Item
                {
                    ProductId = 1,
                    ProductName = "Widget",
                    ProductDescription = "A useful widget.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 19.99,
                    ProductQty = 100
                });
                dbContext.Items.Add(new Item
                {
                    ProductId = 2,
                    ProductName = "Gizmo",
                    ProductDescription = "An innovative gizmo.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 29.99,
                    ProductQty = 50
                });
                dbContext.Items.Add(new Item
                {
                    ProductId = 3,
                    ProductName = "Thingamajig",
                    ProductDescription = "A versatile thingamajig.",
                    ProductCategoryName = "Tools",
                    ProductPrice = 39.99,
                    ProductQty = 200
                });
                dbContext.SaveChanges();
                var user1 = dbContext.Users.First(u => u.UserName == "user1");
                var user2 = dbContext.Users.First(u => u.UserName == "user2");

                var item1 = dbContext.Items.First(i => i.ProductId == 1);
                var item2 = dbContext.Items.First(i => i.ProductId == 2);
                var item3 = dbContext.Items.First(i => i.ProductId == 3);

                dbContext.Sales.Add(new Sales
                {
                    TransactionId = 1,
                    UserId = user1.UserName,
                    date_of_sale = DateTime.Now,
                    ProductId = item1.ProductId
                });
                dbContext.Sales.Add(new Sales
                {
                    TransactionId = 2,
                    UserId = user2.UserName,
                    date_of_sale = DateTime.Now,
                    ProductId = item2.ProductId
                });
                dbContext.Sales.Add(new Sales
                {
                    TransactionId = 3,
                    UserId = user1.UserName,
                    date_of_sale = DateTime.Now,
                    ProductId = item3.ProductId
                });

                dbContext.SaveChanges();

            }
        }


        private List<Item> GetItems()
        {

            var itemList = _applicationDbContext.Items.ToList();

            var items = new List<Item>
            {
                new Item
                {
                    ProductId = 1,
                    ProductName = "Widget",
                    ProductDescription = "A useful widget.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 19.99,
                    ProductQty = 100
                },
                new Item
                {
                    ProductId = 2,
                    ProductName = "Gizmo",
                    ProductDescription = "An innovative gizmo.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 29.99,
                    ProductQty = 50
                },

                 new Item
                {
                    ProductId = 3,
                    ProductName = "Thingamajig",
                    ProductDescription = "A versatile thingamajig.",
                    ProductCategoryName = "Tools",
                    ProductPrice = 39.99,
                    ProductQty = 200
                }
            //new Item
            //{
            //    ProductId = 1,
            //    ProductName = "Sample Product 1",
            //    ProductDescription = "Description for Sample Product 1",
            //    ProductCategoryName = "Category 1",
            //    ProductPrice = 19.99,
            //    ProductQty = 50
            //},
            //new Item
            //{
            //    ProductId = 2,
            //    ProductName = "Sample Product 2",
            //    ProductDescription = "Description for Sample Product 2",
            //    ProductCategoryName = "Category 2",
            //    ProductPrice = 29.99,
            //    ProductQty = 75
            //}
            };
            return itemList;
        }

        [Fact]
        public void ShopInterfaceImpl_UpdateItemListCache_ReturnsBooleanTrue_WhenCacheItemListUpdated()
        {
            object cacheKeyObj = _cacheKey;
            var itemList = GetItems();
            object items = itemList;

            var cacheEntry = new Mock<ICacheEntry>();

            _memoryCacheMock.Setup(mc => mc.TryGetValue(cacheKeyObj, out items)).Returns(true);


            _memoryCacheMock.Setup(mc => mc.CreateEntry(cacheKeyObj)).Returns(cacheEntry.Object); ;

            var result = _shopInterfaceImpl.updateItemListCache(_cacheKey);

            Assert.NotNull(result);
            Assert.True(result);
        }


        [Fact]
        public void ShopInterfaceImpl_UpdateItemListCache_ReturnsBooleanFalse_WhenCacheItemListUpdated()
        {
            var itemList = GetItems();
            object items = itemList;

            var cacheEntry = new Mock<ICacheEntry>();

            _memoryCacheMock.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out items)).Returns(false);



            var result = _shopInterfaceImpl.updateItemListCache(_cacheKey);

            Assert.NotNull(result);
            Assert.False(result);
        }


        [Fact]
        public void ShopInterfaceImpl_DeductUserBalance_ReturnsBooleanFalse_WhenUserNotFound()
        {
            _mapperMock.Setup(m => m.Map<Person>(It.IsAny<PersonDto>())).Returns(null as Person);

            var result = _shopInterfaceImpl.DeductUserBalance(It.IsAny<PersonDto>(), It.IsAny<List<SelectedItem>>(), 10000);

            Assert.NotNull(result);
            Assert.False(result);
        }

        [Fact]
        public void ShopInterfaceImpl_DeductUserBalance_ReturnsBooleanFalse_WhenUserBalanceInsufficient()
        {

            var personDto = new PersonDto
            {
                Id = "user1-id",
                CustomerName = "user1 name",
                Address = "user1 Address",
                Username = "user1",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
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

            _mapperMock.Setup(m => m.Map<Person>(personDto)).Returns(person);

            var result = _shopInterfaceImpl.DeductUserBalance(personDto, It.IsAny<List<SelectedItem>>(), 10000);

            Assert.NotNull(result);
            Assert.False(result);
        }

        // try catch block of DeductUserBalance remaining.

        [Fact]
        public void ShopInterfaceImpl_DeductUserBalance_ResponseSuccess()
        {
            var person = _applicationDbContext.Users.FirstOrDefault(u => u.UserName == "user1");

            var personDto = new PersonDto
            {
                Id = person.Id,
                CustomerName = "user1 name",
                Address = "user1 Address",
                Username = "user1",
                Password = "user1Password",
                Balance = 100.0,
                FilePath = "/path/user1"
            };



            //var person = new Person
            //{
                
            //    CustomerName = "user1 name",
            //    Address = "user1 Address",
            //    UserName = "user1",
            //    Password = "user1Password",
            //    Balance = 100.0,
            //    FilePath = "/path/user1"
            //};

            _mapperMock.Setup(m => m.Map<Person>(personDto)).Returns(person);

            var selectedItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 2, selectedQuantity = 1 },
            };

            object cacheKeyObj = _cacheKey;
            var itemList = GetItems();
            object items = itemList;

            var cacheEntry = new Mock<ICacheEntry>();

            _memoryCacheMock.Setup(mc => mc.TryGetValue(cacheKeyObj, out items)).Returns(true);


            _memoryCacheMock.Setup(mc => mc.CreateEntry(cacheKeyObj)).Returns(cacheEntry.Object); ;



            var result = _shopInterfaceImpl.DeductUserBalance(personDto, selectedItems, 100);

            Assert.True(result);
            var updatedUser = _applicationDbContext.Users.First(u => u.UserName == "user1");
            var updatedItem = _applicationDbContext.Items.First(i => i.ProductId == 2);

            Assert.Equal(0, updatedUser.Balance);
            Assert.Equal(49, updatedItem.ProductQty);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var updatedUser = context.Users.First(u => u.UserName == "user1");
            //    var updatedItem = context.Items.First(i => i.ProductId == 1);

            //    Assert.Equal(0, updatedUser.Balance); 
            //    Assert.Equal(99, updatedItem.ProductQty);
            //}

        }

     // catch remaining


        [Fact]
        public void ShopInterfaceImpl_UpdatedItemsInDb_ReturnsBooleanTrue_WhenItemsUpdated()
        {

            var selectedItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 1, selectedQuantity = 10 },
                new SelectedItem { ProductId = 2, selectedQuantity = 5 }
            };

            var result = _shopInterfaceImpl.UpdatedItemsInDb(selectedItems);

            Assert.NotNull(result);
            Assert.True(result);

            var item1 = _applicationDbContext.Items.First(i => i.ProductId == 1);
            var item2 = _applicationDbContext.Items.First(i => i.ProductId == 2);

            Assert.Equal(90, item1.ProductQty);
            Assert.Equal(45, item2.ProductQty);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var item1 = context.Items.First(i => i.ProductId == 1);
            //    var item2 = context.Items.First(i => i.ProductId == 2);

            //    Assert.Equal(90, item1.ProductQty);
            //    Assert.Equal(0, item2.ProductQty);
            //}

        }

        [Fact]
        public void ShopInterfaceImpl_UpdatedItemsInDb_ShouldReturnFalse_WhenItemNotFound()
        {
            var selectedItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 999, selectedQuantity = 1 }
            };


            var result = _shopInterfaceImpl.UpdatedItemsInDb(selectedItems);

            Assert.NotNull(result);
            Assert.False(result);

        }

        [Fact]
        public void ShopInterfaceImpl_UpdatingItemsInDb_ShouldReturnAllItems()
        {
            var expectedResult = new List<Item>
            {
                new Item
                {
                    ProductId = 1,
                    ProductName = "Widget",
                    ProductDescription = "A useful widget.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 19.99,
                    ProductQty = 100
                },
                new Item
                {
                    ProductId = 2,
                    ProductName = "Gizmo",
                    ProductDescription = "An innovative gizmo.",
                    ProductCategoryName = "Gadgets",
                    ProductPrice = 29.99,
                    ProductQty = 50
                },
                new Item
                {
                    ProductId = 3,
                    ProductName = "Thingamajig",
                    ProductDescription = "A versatile thingamajig.",
                    ProductCategoryName = "Tools",
                    ProductPrice = 39.99,
                    ProductQty = 200
                }

            };
            var result = _shopInterfaceImpl.GetAllItems();

            Assert.NotNull(result);
            Assert.IsType<List<Item>>(result);

            Assert.Equal(3, result.Count);

            var itemList = result.ToList();

            foreach (var expectedItem in expectedResult)
            {
                Assert.Contains(result, item =>
                        item.ProductId == expectedItem.ProductId &&
                        item.ProductName == expectedItem.ProductName &&
                        item.ProductDescription == expectedItem.ProductDescription &&
                        item.ProductCategoryName == expectedItem.ProductCategoryName &&
                        item.ProductPrice == expectedItem.ProductPrice &&
                        item.ProductQty == expectedItem.ProductQty
                    );
            }
        }

        [Fact]
        public void ShopInterfaceImpl_GetItemById_ReturnItem()
        {
            var expectedItem = new Item
            {
                ProductId = 2,
                ProductName = "Gizmo",
                ProductDescription = "An innovative gizmo.",
                ProductCategoryName = "Gadgets",
                ProductPrice = 29.99,
                ProductQty = 50
            };

            int productId = 2;

            var result = _shopInterfaceImpl.GetItemById(productId);

            Assert.NotNull(result);
            Assert.IsType<Item>(result);
            Assert.Equal(expectedItem.ProductId, result.ProductId);
            Assert.Equal(expectedItem.ProductName, result.ProductName);
            Assert.Equal(expectedItem.ProductPrice, result.ProductPrice);
            Assert.Equal(expectedItem.ProductQty, result.ProductQty);

        }

        [Fact]
        public void ShopInterfaceImpl_GetSerachedItem_ReturnsItemList()
        {
            var expectedItem = new Item
            {
                ProductId = 1,
                ProductName = "Widget",
                ProductDescription = "A useful widget.",
                ProductCategoryName = "Gadgets",
                ProductPrice = 19.99,
                ProductQty = 100
            };


            var result = _shopInterfaceImpl.getSerachedItem(expectedItem.ProductName);
            //var resultList = result.ToList();
            Assert.NotNull(result);
            Assert.IsType<List<Item>>(result);
            //Assert.Equal(expectedItem.ProductId, result.);
            foreach (var item in result)
            {
                Assert.Equal(expectedItem.ProductId, item.ProductId);
            }

        }

        [Fact]
        public void ShopInterfaceImpl_GetSerachedItem_ReturnsNullList()
        {

            string productName = "noProduct";
            var result = _shopInterfaceImpl.getSerachedItem(productName);
            Assert.NotNull(result);
            Assert.IsType<List<Item>>(result);
            Assert.Empty(result);


        }


        [Fact]
        public void ShopInterfaceImpl_AddItemsToDb()
        {
            var addedItem = new Item
            {
                ProductId = 4,
                ProductName = "Hour-glass",
                ProductDescription = "One minute timer",
                ProductCategoryName = "Gadgets",
                ProductPrice = 39.99,
                ProductQty = 40
            };

            var result = _shopInterfaceImpl.AddItemsToDb(addedItem);

            Assert.NotNull(result);
            Assert.True(result);


            //var itemFromDb = _applicationDbContext.Items.First(i => i.ProductId == addedItem.ProductId);

            var itemFromDb = _applicationDbContext.Items.FirstOrDefault(i => i.ProductId == addedItem.ProductId);

            Assert.NotNull(itemFromDb);

            Assert.Equal(addedItem.ProductId, itemFromDb.ProductId);
            Assert.Equal(addedItem.ProductName, itemFromDb.ProductName);
            Assert.Equal(addedItem.ProductDescription, itemFromDb.ProductDescription);
            Assert.Equal(addedItem.ProductCategoryName, itemFromDb.ProductCategoryName);
            Assert.Equal(addedItem.ProductPrice, itemFromDb.ProductPrice);
            Assert.Equal(addedItem.ProductQty, itemFromDb.ProductQty);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var itemFromDb = context.Items.FirstOrDefault(i => i.ProductId == addedItem.ProductId);

            //    Assert.NotNull(itemFromDb);

            //    Assert.Equal(addedItem.ProductId, itemFromDb.ProductId);
            //    Assert.Equal(addedItem.ProductName, itemFromDb.ProductName);
            //    Assert.Equal(addedItem.ProductDescription, itemFromDb.ProductDescription);
            //    Assert.Equal(addedItem.ProductCategoryName, itemFromDb.ProductCategoryName);
            //    Assert.Equal(addedItem.ProductPrice, itemFromDb.ProductPrice);
            //    Assert.Equal(addedItem.ProductQty, itemFromDb.ProductQty);
            //}
        }


        [Fact]
        public void ShopInterfaceImpl_AddItemsToDb_ShouldReturnFalse_WhenExceptionOccurs()
        {
            //  var invalidItem = new Item
            //{
            //    ProductId = 4,
            //    //ProductName = "Unknown",
            //    ProductDescription = "One minute timer",
            //    ProductCategoryName = "Gadgets",
            //    ProductPrice = 39.99,
            //    ProductQty = 40
            //};

            
            var result = _shopInterfaceImpl.AddItemsToDb(null);

            Assert.NotNull(result);
            Assert.False(result); 

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var itemFromDb = context.Items.FirstOrDefault(i => i.ProductId == invalidItem.ProductId);
            //    Assert.Null(itemFromDb); 
            //}
        }

        [Fact]
        public void ShopInterfaceImpl_RemoveItemsDb_ShouldReturnTrue_WhenItemExists()
        {
            var itemToRemove = new Item
            {
                ProductId = 1,
                ProductName = "Widget",
                ProductDescription = "A useful widget.",
                ProductCategoryName = "Gadgets",
                ProductPrice = 19.99,
                ProductQty = 100
            };

            var result = _shopInterfaceImpl.RemoveItemsDb(itemToRemove.ProductId);
            Assert.True(result);

            //var itemFromDb = _applicationDbContext.Items.First(i => i.ProductId == itemToRemove.ProductId);
            var itemFromDb = _applicationDbContext.Items.FirstOrDefault(i => i.ProductId == itemToRemove.ProductId);

            Assert.Null(itemFromDb);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var itemFromDb = context.Items.FirstOrDefault(i => i.ProductId == itemToRemove.ProductId);
            //    Assert.Null(itemFromDb);
            //}
        }

        [Fact]
        public void ShopInterfaceImpl_RemoveItemsDb_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            int productId = 1999;
            var result = _shopInterfaceImpl.RemoveItemsDb(productId); 

            Assert.False(result);
        }

        [Fact]
        private void ShopInterfaceImpl_UpdateItemInDb_ShouldReturnTrue_WhenItemExists()
        {
            
            var originalItem = new Item
            {
                ProductId = 1,
                ProductName = "Widget",
                ProductDescription = "A useful widget.",
                ProductCategoryName = "Gadgets",
                ProductPrice = 19.99,
                ProductQty = 100
            };


            var updatedItem = new Item
            {
                ProductId = 1,
                ProductName = "Widget 2.0",
                ProductDescription = "A useful widget for everyone",
                ProductCategoryName = "Gadgets",
                ProductPrice = 59.99,
                ProductQty = 40
            };

            var result = _shopInterfaceImpl.UpdateItemInDb(updatedItem);

            Assert.True(result);

            //var itemFromDb = _applicationDbContext.Items.First(i => i.ProductId == updatedItem.ProductId);
            var itemFromDb = _applicationDbContext.Items.FirstOrDefault(i => i.ProductId == updatedItem.ProductId);

            Assert.NotNull(itemFromDb);
            Assert.Equal(updatedItem.ProductName, itemFromDb.ProductName);
            Assert.Equal(updatedItem.ProductDescription, itemFromDb.ProductDescription);
            Assert.Equal(updatedItem.ProductPrice, itemFromDb.ProductPrice);
            Assert.Equal(updatedItem.ProductQty, itemFromDb.ProductQty);


            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var itemFromDb = context.Items.FirstOrDefault(i => i.ProductId == updatedItem.ProductId);
            //    Assert.NotNull(itemFromDb);
            //    Assert.Equal(updatedItem.ProductName, itemFromDb.ProductName);
            //    Assert.Equal(updatedItem.ProductDescription, itemFromDb.ProductDescription);
            //    Assert.Equal(updatedItem.ProductPrice, itemFromDb.ProductPrice);
            //    Assert.Equal(updatedItem.ProductQty, itemFromDb.ProductQty);
            //}
        }

        [Fact]
        public void ShopInterfaceImpl_UpdateItemInDb_ShouldReturnFalse_WhenItemDoesNotExist()
        {
            var nonExistentItem = new Item
            {
                ProductId = 999,
                ProductName = "Non-existent Item",
                ProductDescription = "Description",
                ProductCategoryName = "Category",
                ProductPrice = 19.99,
                ProductQty = 10
            };

            var result = _shopInterfaceImpl.UpdateItemInDb(nonExistentItem);

            Assert.False(result);
        }

        [Fact]
        public void ShopInterfaceImpl_AddTransaction_ShouldAddSalesToDb_WhenValidDataIsProvided()
        {
            //var user = new Person
            //{
            //    Id = "user1",
            //    UserName = "user1",
            //    CustomerName = "User One",
            //    Address = "Address 1",
            //    Password = "password123",
            //    Balance = 100.0,
            //    FilePath = "/path/user1"
            //};

            var user = _applicationDbContext.Users.FirstOrDefault(u => u.UserName == "user1");


            //var item1 = new Item
            //{
            //    ProductId = 1,
            //    ProductName = "Widget",
            //    ProductDescription = "A useful widget.",
            //    ProductCategoryName = "Gadgets",
            //    ProductPrice = 19.99,
            //    ProductQty = 100
            //};
            //var item2 = new Item
            //{
            //    ProductId = 2,
            //    ProductName = "Gizmo",
            //    ProductDescription = "An innovative gizmo.",
            //    ProductCategoryName = "Gadgets",
            //    ProductPrice = 29.99,
            //    ProductQty = 50
            //};
            var cartItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 1111,ProductName = "Widget",
                ProductDescription = "A useful widget.",
                ProductCategoryName = "Gadgets",
                ProductPrice = 19.99,
                ProductQty = 100,
                    selectedQuantity = 2 },
                new SelectedItem { ProductId = 2111, ProductName = "Gizmo",
                ProductDescription = "An innovative gizmo.",
                ProductCategoryName = "Gadgets",
                ProductPrice = 29.99,
                ProductQty = 50,
                    selectedQuantity = 1 }
            };

            var result =_shopInterfaceImpl.AddTransaction(user, cartItems);

            Assert.True(result);


            var sales = _applicationDbContext.Sales.ToList();

            Assert.Equal(5, sales.Count);
            Assert.Contains(sales, s => s.UserId == user.Id && s.ProductId == 1111);
            Assert.Contains(sales, s => s.UserId == user.Id && s.ProductId == 2111);
           
            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var sales = context.Sales.ToList();

            //    Assert.Equal(5, sales.Count); 
            //    Assert.Contains(sales, s => s.UserId == user.Id && s.ProductId == 1);
            //    Assert.Contains(sales, s => s.UserId == user.Id && s.ProductId == 2);
            //}
        }

        [Fact]
        public void ShopInterfaceImpl_AddTransaction_ShouldNotAddSales_WhenCartItemsIsNull()
        {
            var user = new Person
            {
                Id = "user1",
                UserName = "user1",
                CustomerName = "User One",
                Address = "Address 1",
                Password = "password123",
                Balance = 100.0,
                FilePath = "/path/user1"
            };

            
            var result =_shopInterfaceImpl.AddTransaction(user, null);

            Assert.False(result);

        }

        [Fact]
        public void ShopInterfaceImpl_AddTransaction_ShouldGenerateUniqueTransactionId()
        {
            var user = new Person
            {
                UserName = "user2",
                CustomerName = "user2 name",
                Address = "user2 Address",
                Password = "user2Password",
                Balance = 100.0,
                FilePath = "/path/user2"
            };

            var cartItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 1, selectedQuantity = 2 }
            };

            var result = _shopInterfaceImpl.AddTransaction(user, cartItems);

            Assert.True(result);

            var sales = _applicationDbContext.Sales.Where(i => i.UserId == user.Id).ToList();
            Assert.Single(sales);
            var transactionId = sales[0].TransactionId;

            _shopInterfaceImpl.AddTransaction(user, cartItems);
            var updatedSales = _applicationDbContext.Sales.ToList();
            var newTransactionId = updatedSales[1].TransactionId;

            Assert.NotEqual(transactionId, newTransactionId);


            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var sales = context.Sales.Where(i => i.UserId == user.Id).ToList();

            //    Assert.Single(sales);
            //    var transactionId = sales[0].TransactionId;

            //    _shopInterfaceImpl.AddTransaction(user, cartItems);
            //    var updatedSales = context.Sales.ToList();
            //    var newTransactionId = updatedSales[1].TransactionId;

            //    Assert.NotEqual(transactionId, newTransactionId);
            //}
        }

        [Fact]
        public void ShopInterfaceImpl_AddTransaction_ShouldDetachExistingEntitiesWithSameTransactionId()
        {
            var user = new Person
            {
                UserName = "user2",
                CustomerName = "user2 name",
                Address = "user2 Address",
                Password = "user2Password",
                Balance = 100.0,
                FilePath = "/path/user2"
            };

            var cartItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 1, selectedQuantity = 2 }
            };

            _shopInterfaceImpl.AddTransaction(user, cartItems);

            // Add again to test detachment
            _shopInterfaceImpl.AddTransaction(user, cartItems);

            var sales = _applicationDbContext.Sales.Where(i => i.UserId == user.Id).ToList();
            var transactionIds = sales.Select(s => s.TransactionId).Distinct().ToList();

            Assert.Equal(2, transactionIds.Count);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var sales = context.Sales.Where(i => i.UserId == user.Id).ToList();
            //    var transactionIds = sales.Select(s => s.TransactionId).Distinct().ToList();
                
            //    // Ensure that duplicate transactions with the same ID were not added
            //    Assert.Equal(2, transactionIds.Count); 
            //}
        }
        
        [Fact]
        public void ShopInterfaceImpl_UpdatedItemsInDb_ShouldUpdateItems_WhenValidDataIsProvided()
        {
            
            var cartItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 1, selectedQuantity = 10 },
                new SelectedItem { ProductId = 2, selectedQuantity = 20 }
            };

            var result = _shopInterfaceImpl.UpdatedItemsInDb(cartItems);

            Assert.True(result);

            var updatedItems = _applicationDbContext.Items.ToList();
            var item1 = updatedItems.First(i => i.ProductId == 1);
            var item2 = updatedItems.First(i => i.ProductId == 2);

            Assert.Equal(90, item1.ProductQty);
            Assert.Equal(30, item2.ProductQty);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var updatedItems = context.Items.ToList();
            //    var item1 = updatedItems.First(i => i.ProductId == 1);
            //    var item2 = updatedItems.First(i => i.ProductId == 2);

            //    Assert.Equal(90, item1.ProductQty); 
            //    Assert.Equal(30, item2.ProductQty);
            //}
        }


        [Fact]
        public void ShopInterfaceImpl_UpdatedItemsInDb_ShouldReturnFalse_WhenItemDoesNotExist()
        {

            var cartItems = new List<SelectedItem>
            {
                // ProductId 222 does not exist
                new SelectedItem { ProductId = 222, selectedQuantity = 10 } 
            };

            var result = _shopInterfaceImpl.UpdatedItemsInDb(cartItems);

            Assert.False(result);
        }


        [Fact]
        public void ShopInterfaceImpl_UpdatedItemsInDb_ShouldSetQuantityToZero_WhenQuantityBecomesNegative()
        {
            
            var cartItems = new List<SelectedItem>
            {
                new SelectedItem { ProductId = 1, selectedQuantity = 110 } 
            };

            var result = _shopInterfaceImpl.UpdatedItemsInDb(cartItems);

            Assert.True(result);

            //var updatedItem = _applicationDbContext.Items.First(i => i.ProductId == 1);
            var updatedItem = _applicationDbContext.Items.FirstOrDefault(i => i.ProductId == 1);

            Assert.Equal(0, updatedItem.ProductQty);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var updatedItem = context.Items.First(i => i.ProductId == 1);
            //    Assert.Equal(0, updatedItem.ProductQty);
            //}
        }


        [Fact]
        public void ShopInterfaceImpl_UpdatedItemsInDb_ShouldNotChangeDatabase_WhenCartItemsIsEmpty()
        {
            
            var cartItems = new List<SelectedItem>(); 

            var result = _shopInterfaceImpl.UpdatedItemsInDb(cartItems);

            Assert.True(result);

            var item = _applicationDbContext.Items.First();

            Assert.Equal(100, item.ProductQty);

            //using (var context = new ApplicationDbContext(_dbContextOptions, null))
            //{
            //    var item = context.Items.First();

            //    // Ensure quantity hasn't changed
            //    Assert.Equal(100, item.ProductQty); 
            //}
        }


    }
}
