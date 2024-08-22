using DemoWebApplication.Constants;
using DemoWebApplication.Controllers;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceInterface;
using FakeItEasy;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NuGet.ContentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace DemoWebTestAppication.ShopApp.Tests.Controller
{
    public class ShopControllerTests
    {
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private readonly Mock<IShopInterface>_shopInterfaceMock;
        private readonly Mock<IUserInterface> _userInterfaceMock;
        private readonly ShopController _shopController;
        public ShopControllerTests()
        {
            _memoryCacheMock = new Mock<IMemoryCache>();
            _shopInterfaceMock = new Mock<IShopInterface>();
            _userInterfaceMock = new Mock<IUserInterface>();

            //SUT (what we are going to execute on) aka (System Under Test)
            _shopController = new ShopController(_memoryCacheMock.Object, _shopInterfaceMock.Object, _userInterfaceMock.Object);
        }

        [Fact]
        public void ShopController_ShowAllItemsInShop_RetunrsCahceData_WithoutAddingToCache()
        {
            var cacheKey = "ProductData";
            var items = GetItems();
            object itemList = items;

            _memoryCacheMock.Setup(mc => mc.TryGetValue(It.IsAny<object>(),out itemList)).Returns(true);

            var result = _shopController.ShowAllItemsInShop() as JsonResult;

            Assert.NotNull(result);

            var response = result.Value as List<Item>;
            Assert.Equal(items.Count, response.Count);

            Assert.NotNull(response);
            foreach (var item in items)
            {
                Assert.Contains(response, i =>
                    i.ProductId == item.ProductId &&
                    i.ProductName == item.ProductName &&
                    i.ProductDescription == item.ProductDescription &&
                    i.ProductCategoryName == item.ProductCategoryName &&
                    i.ProductPrice == item.ProductPrice &&
                    i.ProductQty == item.ProductQty);
            }

        }

        [Fact]
        public void ShopController_ShowAllItemsInShop_RetunrsCacheData_AfterAddingToCache()
        {
            var cacheKey = "ProductData";
            var items = new List<Item>
            {
                new Item
                {
                    ProductId = 1,
                    ProductName = "Sample Product 1",
                    ProductDescription = "Description for Sample Product 1",
                    ProductCategoryName = "Category 1",
                    ProductPrice = 19.99,
                    ProductQty = 50
                }
            };
            object itemList = items;

            var cacheEntry = new Mock<ICacheEntry>();

            _memoryCacheMock.Setup(mc => mc.TryGetValue(It.IsAny<object>(), out itemList)).Returns(false);

            _shopInterfaceMock.Setup( si => si.GetAllItems()).Returns(items);

            _memoryCacheMock.Setup(mc => mc.CreateEntry(It.IsAny<object>())).Returns(cacheEntry.Object);
            


            var result = _shopController.ShowAllItemsInShop() as JsonResult;

            Assert.NotNull(result);

            var response = result.Value as List<Item>;
            Assert.Equal(items.Count, response.Count);

            Assert.NotNull(response);
            foreach (var item in items)
            {
                Assert.Contains(response, i =>
                    i.ProductId == item.ProductId &&
                    i.ProductName == item.ProductName &&
                    i.ProductDescription == item.ProductDescription &&
                    i.ProductCategoryName == item.ProductCategoryName &&
                    i.ProductPrice == item.ProductPrice &&
                    i.ProductQty == item.ProductQty);
            }

        }

        [Fact]
        public void ShopController_GetUserFavouriteItem_ReturnError_WhenUserNotFound()
        {
            var username = "Ales";
            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(null as PersonDto);

            var result = _shopController.GetUserFavouriteItem(username) as JsonResult;

            Assert.NotNull(result);
            Assert.Equal(Message.UserNotFound, result.Value);
        
        }

        [Fact]
        public void SHopController_GetUserFavouriteItem_ReturnSuccess_WhenFavouriteItemAdded()
        {
            var username = "Alex";

            List<Item> listItem = GetItems();
            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(Mock.Of<PersonDto>);

            _userInterfaceMock.Setup(ui => ui.getUserFavouriteItems(It.IsAny<PersonDto>())).Returns(listItem);
        
            var result = _shopController.GetUserFavouriteItem(username);

            Assert.NotNull(result);
            //Assert.IsType<List<Item>>(result);
        
        }


        [Fact]
        public void ShopController_SaveFavouriteItem_ReturnActionResult()
        {
            var username = "alex";
            var selectedItem = new SelectedItem
            {
                ProductId = 1,
                ProductName = "Sample Product",
                ProductDescription = "This is a sample product description.",
                ProductCategoryName = "Sample Category",
                ProductPrice = 29.99,
                ProductQty = 100.0,
                selectedQuantity = 2.0
            };
            var user = A.Fake<PersonDto>();


            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(user);

            _userInterfaceMock.Setup(ui => ui.saveFavouriteItem(user, selectedItem)).Returns(Message.ItemAddedInFavourites);   


            var result = _shopController.SaveFavouriteItem(username,selectedItem);


            Assert.NotNull(result);
            Assert.True(result.success);
            Assert.Equal(Message.ItemAddedInFavourites, result.message);

        }

        [Fact]
        public void ShopController_RemoveItemsInShop_ReturnsSuccess_WhenItemRemovedAndCacheUpdated()
        {
            int productId = 1; 
            var cacheKey = "ProductData";
            var cacheUpdateSuccess = true;

            _shopInterfaceMock.Setup(si => si.RemoveItemsDb(productId)).Returns(true);
            _shopInterfaceMock.Setup(si => si.updateItemListCache(cacheKey)).Returns(true);


            var result = _shopController.RemoveItemsInShop(productId);

            Assert.NotNull(result);
            Assert.True(result.success);
            Assert.Equal(Message.ItemDeletedFromShop, result.message);
        }
        [Fact]
        public void ShopController_RemoveItemsInShop_ReturnsFailed_WhenItemRemovedAndCacheUpdated()
        {
            int productId = 1;
            var cacheKey = "ProductData";

            _shopInterfaceMock.Setup(si => si.RemoveItemsDb(productId)).Returns(true);
            _shopInterfaceMock.Setup(si => si.updateItemListCache(cacheKey)).Returns(false);

            var result = _shopController.RemoveItemsInShop(productId);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ErrorDeletingItemFromShop, result.message);
        }

        [Fact]
        public void ShopController_RemoveItemsInShop_ReturnsFailed_WhenItemNotFound()
        {
            int productId = 1;

            _shopInterfaceMock.Setup(si => si.RemoveItemsDb(productId)).Returns(false);

            var result = _shopController.RemoveItemsInShop(productId);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ItemNotFound, result.message);
        }




        [Fact]
        public void ShopController_UpdateItemsInShop_ReturnsSuceess_WhenItemsUpdated()
        {
            int productId = 1;
            var cacheKey = "ProductData";

            var item = new Item
            {
                ProductId = 1,
                ProductName = "Sample Product 1",
                ProductDescription = "Description for Sample Product 1",
                ProductCategoryName = "Category 1",
                ProductPrice = 19.99,
                ProductQty = 50
            };

            var updatedItem = new Item
            {
                ProductId = 1,
                ProductName = "New Sample Product 12",
                ProductDescription = "Description for Sample Product 12",
                ProductCategoryName = "Category 12",
                ProductPrice = 100,
                ProductQty = 1150
            };

            _shopInterfaceMock.Setup(si => si.GetItemById(productId)).Returns(item);
            _shopInterfaceMock.Setup(si => si.UpdateItemInDb(updatedItem)).Returns(true);
            _shopInterfaceMock.Setup(si => si.updateItemListCache(cacheKey)).Returns(true);

            var result = _shopController.UpdateItemsInShop(updatedItem);

            Assert.NotNull(result);
            Assert.True(result.success);
            Assert.Equal(Message.ItemEditedOfShop, result.message);

        }


        [Fact]
        public void ShopController_UpdateItemsInShop_ReturnsFailure_WhenUpdatingCacheList()
        {
            int productId = 1;
            var cacheKey = "ProductData";

            var item = new Item
            {
                ProductId = 1,
                ProductName = "Sample Product 1",
                ProductDescription = "Description for Sample Product 1",
                ProductCategoryName = "Category 1",
                ProductPrice = 19.99,
                ProductQty = 50
            };

            var updatedItem = new Item
            {
                ProductId = 1,
                ProductName = "New Sample Product 12",
                ProductDescription = "Description for Sample Product 12",
                ProductCategoryName = "Category 12",
                ProductPrice = 100,
                ProductQty = 1150
            };

            _shopInterfaceMock.Setup(si => si.GetItemById(productId)).Returns(item);
            _shopInterfaceMock.Setup(si => si.UpdateItemInDb(updatedItem)).Returns(true);
            _shopInterfaceMock.Setup(si => si.updateItemListCache(cacheKey)).Returns(false);

            var result = _shopController.UpdateItemsInShop(updatedItem);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ErrorWhileCacheUpdatedAfterDataUpdate, result.message);

        }


        [Fact]
        public void ShopController_UpdateItemsInShop_ReturnsFailure_WhenUpdatingItem()
        {
            int productId = 1;

            var item = new Item
            {
                ProductId = 1,
                ProductName = "Sample Product 1",
                ProductDescription = "Description for Sample Product 1",
                ProductCategoryName = "Category 1",
                ProductPrice = 19.99,
                ProductQty = 50
            };

            var updatedItem = new Item
            {
                ProductId = 1,
                ProductName = "New Sample Product 12",
                ProductDescription = "Description for Sample Product 12",
                ProductCategoryName = "Category 12",
                ProductPrice = 100,
                ProductQty = 1150
            };

            _shopInterfaceMock.Setup(si => si.GetItemById(productId)).Returns(item);
            _shopInterfaceMock.Setup(si => si.UpdateItemInDb(updatedItem)).Returns(false);

            var result = _shopController.UpdateItemsInShop(updatedItem);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ErrorEditingItemOfShop, result.message);

        }

        [Fact]
        public void ShopController_UpdateItemsInShop_ReturnsFailure_WhenItemNotFound()
        {
            int productId = 1;

            var item = new Item
            {
                ProductId = 1,
                ProductName = "Sample Product 1",
                ProductDescription = "Description for Sample Product 1",
                ProductCategoryName = "Category 1",
                ProductPrice = 19.99,
                ProductQty = 50
            };

            var updatedItem = new Item
            {
                ProductId = 1,
                ProductName = "New Sample Product 12",
                ProductDescription = "Description for Sample Product 12",
                ProductCategoryName = "Category 12",
                ProductPrice = 100,
                ProductQty = 1150
            };

            _shopInterfaceMock.Setup(si => si.GetItemById(productId)).Returns(null as Item);

            var result = _shopController.UpdateItemsInShop(updatedItem);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ItemNotFound, result.message);

        }

        [Fact]
        public void ShopController_DedutBalance_ReturnsError_WhenUserNotFound()
        {
            var username = "testUser";

            var requestModel = new DeduceBalanceRequestModel
            {
                Username = username,
                CartItems = new List<SelectedItem> { new SelectedItem() },
                TotalAmount = 100
            };
            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(null as PersonDto);

            var result = _shopController.DeductBalance(requestModel);

            Assert.NotNull(result);
            Assert.False(result.success);

            Assert.Equal(Message.UserNotFound,result.message);

        }


        [Fact]
        public void ShopController_DedutBalance_ReturnsError_WhenUserBalanceInsufficient()
        {

            var username = "testUser";
            var user = new PersonDto { Username = username }; 
            var requestModel = new DeduceBalanceRequestModel
            {
                Username = username,
                CartItems = new List<SelectedItem> { new SelectedItem() }, 
                TotalAmount = 100
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns( user);
            _shopInterfaceMock.Setup(si => si.DeductUserBalance(user,requestModel.CartItems, requestModel.TotalAmount)).Returns(false);
            var result = _shopController.DeductBalance(requestModel);


            
            Assert.NotNull(result);
            Assert.False(result.success);

            Assert.Equal(Message.FailedToDeduceBalanceMessage, result.message);

        }

        [Fact]
        public void ShopController_DedutBalance_ReturnsSuccess_WhenBalanceDeducted()
        {

            var username = "testUser";
            var user = new PersonDto { Username = username };
            var requestModel = new DeduceBalanceRequestModel
            {
                Username = username,
                CartItems = new List<SelectedItem> { new SelectedItem() },
                TotalAmount = 100
            };

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(user);
            _shopInterfaceMock.Setup(si => si.DeductUserBalance(user, requestModel.CartItems, requestModel.TotalAmount)).Returns(true);
            var result = _shopController.DeductBalance(requestModel);

            Assert.NotNull(result);
            Assert.True(result.success);

            Assert.Equal(Message.SuccessfulDeduceBalanceMessage, result.message);

        }

        [Fact]
        public void ShopController_AddItemsInShop_ReturnsSuccess_WhenItemAddedAndCacheUpdated()
        {
            var cacheKey = "ProductData";

            var item = new Item { };

            _shopInterfaceMock.Setup(si => si.AddItemsToDb(item)).Returns(true);
            _shopInterfaceMock.Setup(si => si.updateItemListCache(cacheKey)).Returns(true);


            var result = _shopController.AddItemsInShop(item);

            Assert.NotNull(result);
            Assert.True(result.success);
            Assert.Equal(Message.ItemAddedToShop, result.message);
        }

        [Fact]
        public void ShopController_AddItemsInShop_ReturnsFailure_WhenItemAddedAndCacheUpdated()
        {
            var cacheKey = "ProductData";

            var item = new Item { };

            _shopInterfaceMock.Setup(si => si.AddItemsToDb(item)).Returns(true);
            _shopInterfaceMock.Setup(si => si.updateItemListCache(cacheKey)).Returns(false);


            var result = _shopController.AddItemsInShop(item);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ErrorAddingItemToShop, result.message);
        }


        [Fact]
        public void ShopController_AddItemsInShop_ReturnsFailure_WhenItemNotAddedInDb()
        {

            var item = new Item { };

            _shopInterfaceMock.Setup(si => si.AddItemsToDb(item)).Returns(false);


            var result = _shopController.AddItemsInShop(item);

            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.ErrorWithAddingItemInDb, result.message);
        }
        [Fact]
        public void ShopController_SaveFavouriteItem_ReturnError_WhenUserNotFound()
        {
            var username = "alex";
      

            _userInterfaceMock.Setup(ui => ui.GetUserByUsername(username)).Returns(null as PersonDto);

            var result = _shopController.SaveFavouriteItem(username, It.IsAny<SelectedItem>());
            Assert.NotNull(result);
            Assert.False(result.success);
            Assert.Equal(Message.UserNotFound, result.message);

        }

        [Fact]
        public void ShopController_SearchedItem_ReturnActionResult()
        {
            var items = GetItems();
            string shopItem = "Sample Product 1";

            var filteredItems = items.Where(item => item.ProductName == shopItem).ToList();

            _shopInterfaceMock.Setup(si => si.getSerachedItem(shopItem)).Returns(filteredItems);

            var result = _shopController.SearchedItem(shopItem).Result as JsonResult;
            var resultItems = result?.Value as List<Item>;
            Console.WriteLine(result);
            Console.WriteLine(resultItems);
            Assert.NotNull(result);
            Assert.True(resultItems.Any(item => item.ProductName == shopItem));
            
        }


        public List<Item> GetItems()
        {
            var items = new List<Item>
        {
            new Item
            {
                ProductId = 1,
                ProductName = "Sample Product 1",
                ProductDescription = "Description for Sample Product 1",
                ProductCategoryName = "Category 1",
                ProductPrice = 19.99,
                ProductQty = 50
            },
            new Item
            {
                ProductId = 2,
                ProductName = "Sample Product 2",
                ProductDescription = "Description for Sample Product 2",
                ProductCategoryName = "Category 2",
                ProductPrice = 29.99,
                ProductQty = 75
            },
            new Item
            {
                ProductId = 3,
                ProductName = "Sample Product 3",
                ProductDescription = "Description for Sample Product 3",
                ProductCategoryName = "Category 3",
                ProductPrice = 39.99,
                ProductQty = 100
            }
        };

            return items;
        }
    }
}
