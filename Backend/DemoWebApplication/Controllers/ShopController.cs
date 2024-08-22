using DemoWebApplication.Constants;
using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceInterface;
using DemoWebApplication.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace DemoWebApplication.Controllers;

public class ShopController : Controller
{
    private readonly IMemoryCache _memoryCache;

    private readonly string _cacheKey = "ProductData";

    //private readonly IHttpContextAccessor _httpContextAccessor;

    private IShopInterface _shopInterface;

    private IUserInterface _userInterface;
    public ShopController(IMemoryCache memoryCache
        //, IHttpContextAccessor httpContextAccessor
        , IShopInterface shopInterface, IUserInterface userInterface)
    {
        _memoryCache = memoryCache;
        //_httpContextAccessor = httpContextAccessor;
        _shopInterface = shopInterface;
        _userInterface = userInterface;
    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User,Admin,Moderator")]
    [HttpGet]
    [Route(Urls.showAllItemsApi)]
    public ActionResult ShowAllItemsInShop()
    {
        if (!_memoryCache.TryGetValue(_cacheKey, out List<Item> items))
        {
            items = _shopInterface.GetAllItems();
            _memoryCache.Set(_cacheKey, items, TimeSpan.FromMinutes(10));
            Log.Debug(Message.CacheCreatedForListofItems);
        }
        return Json(items);

    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin,User,Moderator")]
    [HttpPost]
    [Route(Urls.deductUserBalanceApi)]
    public ApiResponse DeductBalance([FromBody] DeduceBalanceRequestModel model)
    {

        var user = _userInterface.GetUserByUsername(model.Username);
        if (user == null)
        {
            return new ApiResponse { success = false , message = Message.UserNotFound};
            //return Json(new { success = false, message = Message.UserNotFound });

        }
        var result = _shopInterface.DeductUserBalance(user, model.CartItems, model.TotalAmount);
        if (!result)
        {
            return new ApiResponse { success = false, message = Message.FailedToDeduceBalanceMessage };

            //return Json(new { success = false, message = Message.FailedToDeduceBalanceMessage });

        }
        else
        {
            return new ApiResponse { success = true, message = Message.SuccessfulDeduceBalanceMessage };

            //return Json(new { success = true, message = Message.SuccessfulDeduceBalanceMessage });

        }

    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    [Route(Urls.addItemsInShopApi)]
    public ApiResponse AddItemsInShop([FromBody] Item item)
    {
            //try
            //{
        var addedToDbFlg= _shopInterface.AddItemsToDb(item);
        if (addedToDbFlg)
        {
            var check = _shopInterface.updateItemListCache(_cacheKey);
            Log.Debug(Message.CacheUpdatedAfterDataAdded);
            if (check)
            {
                //return Json(new { success = true, message = Message.ItemAddedToShop });
                return new ApiResponse { success = true, message = Message.ItemAddedToShop };

            }
            else
            {
                //return Json(new { success = true, message = Message.ItemAddedToShop });
                return new ApiResponse { success = false, message = Message.ErrorAddingItemToShop };

            }
        }
        else
        {
            return new ApiResponse { success = false, message = Message.ErrorWithAddingItemInDb };
        }

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString());
            //return new ApiResponse { success = true, message = Message.SuccessfulDeduceBalanceMessage };

            //return Json(new { success = false, message = Message.ErrorAddingItemToShop });

            //}

    }


    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    [Route(Urls.removeItemsInShopApi)]

    public ApiResponse RemoveItemsInShop([FromBody]int productId)
    {
        //try
        //{
            var removeCheckFlg = _shopInterface.RemoveItemsDb(productId);
            if (removeCheckFlg)
            {
                //UpdateItemListCache();
                var check = _shopInterface.updateItemListCache(_cacheKey);
                if (check)
                {


                    Log.Debug(Message.CacheUpdatedAfterDataRemoval);
                    return new ApiResponse { success = true, message = Message.ItemDeletedFromShop };

                //return Json(new { success = true, message = Message.ItemDeletedFromShop });
                }
                else
                {
                return new ApiResponse { success = false, message = Message.ErrorDeletingItemFromShop };

                //return Json(new { success = false, message = Message.ErrorDeletingItemFromShop });

                }
            }
            else
            {
            return new ApiResponse { success = false, message = Message.ItemNotFound };

            //return Json(new { success = false, message = Message.ItemNotFound });

            }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine(ex.ToString());
        //    return Json(new { success = false, message = Message.ErrorDeletingItemFromShop });

        //}
    }



    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [HttpPost]
    [Route(Urls.updateItemsInShopApi)]
    public ApiResponse UpdateItemsInShop([FromBody] Item item)
    {
        //try
        //{
        var itemFlg = _shopInterface.GetItemById(item.ProductId);
        if (itemFlg != null)
        {


            var updateCheckFlg = _shopInterface.UpdateItemInDb(item);
            if (updateCheckFlg)
            {
                var check = _shopInterface.updateItemListCache(_cacheKey);
                if(check)
                {

                    Log.Debug(Message.CacheUpdatedAfterDataUpdate);
                    return new ApiResponse { success = true, message = Message.ItemEditedOfShop };
                }
                else
                {
                    return new ApiResponse { success = false, message = Message.ErrorWhileCacheUpdatedAfterDataUpdate };

                }

                //return Json(new { success = true, message = Message.ItemEditedOfShop });
            }
            else
            {
                return new ApiResponse { success = false, message = Message.ErrorEditingItemOfShop };

                //return Json(new { success = false, message = Message.ItemNotFound });

            }
        }
        else
        {
            return new ApiResponse { success = false, message = Message.ItemNotFound };

        }
        //}
        //catch (Exception ex)
        //{

        //    Console.WriteLine(ex.ToString());
        //    return Json(new { success = false, message = Message.ErrorEditingItemOfShop });
        //}
    }
    //public bool UpdateItemListCache()
    //{
    //    if (_memoryCache.TryGetValue(_cacheKey, out List<Item> items))
    //    {
    //        items = _shopInterface.GetAllItems();
    //        _memoryCache.Set(_cacheKey, items, TimeSpan.FromMinutes(10));
    //    }
    //    return true;
    //}

    [HttpGet]
    [Route(Urls.searchItemInShop)]
    public ActionResult<IEnumerable<Item>> SearchedItem(string shopItem)
    {
        //var list = _shopInterface.getSerachedItem(shopItem);
        //return Json(list);
        return Json(_shopInterface.getSerachedItem(shopItem));
    }

    [HttpGet]
    [Route(Urls.getUserFavouriteItems)]
    public IActionResult GetUserFavouriteItem(string username)
    {
        var user = _userInterface.GetUserByUsername(username);
        if (user == null)
        {
            //return new ApiResponse { success = false, message = Message.UserNotFound };

            return Json(Message.UserNotFound);
        }

        List<Item> favItems = _userInterface.getUserFavouriteItems(user);
        return Json(favItems);
    }

    [HttpPost]
    [Route(Urls.saveFavouriteItem)]
    public ApiResponse SaveFavouriteItem(string username, [FromBody] SelectedItem favouriteItem)
    {
        var user =  _userInterface.GetUserByUsername(username);
        if(user == null)
        {
            return new ApiResponse { success = false, message = Message.UserNotFound };

            //return Json(Message.UserNotFound);
        }

        string val = _userInterface.saveFavouriteItem(user, favouriteItem);
        return new ApiResponse { success = true, message = val };

        //return Json(val);

    }
}
