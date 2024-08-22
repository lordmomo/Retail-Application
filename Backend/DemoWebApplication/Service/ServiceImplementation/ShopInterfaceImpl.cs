using DemoWebApplication.Models;
using DemoWebApplication.Service.ServiceInterface;
using DemoWebApplication.Utils;
using Microsoft.AspNetCore.Http;
using System.Security.Policy;
using Newtonsoft.Json;
using DemoWebApplication.Migrations;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Serilog;
using AutoMapper;
using System.Globalization;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Converters;

namespace DemoWebApplication.Service.ServiceImplementation;
public class ShopInterfaceImpl : IShopInterface
{
    public ApplicationDbContext db;
    public IMapper mapper;

    private readonly IMemoryCache _memoryCache;
    private readonly string _cacheKey = "ProductData";

    public ShopInterfaceImpl(ApplicationDbContext db,IMapper mapper, IMemoryCache memoryCache)
    {
        this.db = db;
        this.mapper = mapper;
        this._memoryCache = memoryCache;
    }



    public bool updateItemListCache(string _cacheKey)
    {
        if (_memoryCache.TryGetValue(_cacheKey, out List<Item> items))
        {
            items = GetAllItems();
            _memoryCache.Set(_cacheKey, items, TimeSpan.FromMinutes(10));
            return true;

        }
        return false;

    }

    public bool DeductUserBalance(PersonDto personDto, List<SelectedItem> cartItems, double totalAmount)
    {
        Person user = mapper.Map<Person>(personDto);

        if(user == null)
        {
            return false;
        }
        double newBalance = user.Balance - totalAmount;
        if (newBalance < 0)
        {
            return false;
        }
        user.Balance = newBalance;

        try
        {
            var evsr = db.Users.FirstOrDefault(x => x.UserName == personDto.Username);
            if(evsr == null)
            {
                //User not found
                return false; 
            }
            evsr.Balance = newBalance;

            bool transactionSuccess = AddTransaction(user, cartItems);

            bool itemsUpdated =  UpdatedItemsInDb(cartItems);

            if(transactionSuccess && itemsUpdated)
            {

                db.Update(evsr);

                db.SaveChanges();
            }
            else
            {
                return false;
            }
            //code changed 
            var updateFlg = updateItemListCache(_cacheKey);
            return updateFlg ? true  : false;

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;

        }
        //return true;
    }
        
    
    //requirement  unit test coverage
    //java jdk 11
    //    sonarQube scanner
    //    dot cover



    //    resharper
    //    sonarQube runner


    //public void UpdateItemListCache()
    //{
    //    if (_memoryCache.TryGetValue(_cacheKey, out List<Item> items))
    //    {
    //        items = db.Items.OrderBy(i => i.ProductId).ToList();
    //        _memoryCache.Set(_cacheKey, items, TimeSpan.FromMinutes(10));
    //    }
    //}

    public bool UpdatedItemsInDb(List<SelectedItem> cartItems)
    {
        foreach (var selectedItem in cartItems)
        {
            var item = db.Items.FirstOrDefault(x => x.ProductId == selectedItem.ProductId);

            if (item != null)
            {
                item.ProductQty -= selectedItem.selectedQuantity;

                if (item.ProductQty < 0)
                {
                   
                    item.ProductQty = 0;
                }
                db.Update(item);
            }
            else
            {
                Console.WriteLine($"Item with ProductId {selectedItem.ProductId} not found in database.");
                return false;
            }
        }

        db.SaveChanges();
        return true;

    }

    public bool AddTransaction(Person user, List<SelectedItem> cartItems)
    {
        DateTime currentDate = DateTime.UtcNow;
        int transactionId = Math.Abs(Guid.NewGuid().GetHashCode());
        List<Sales> salesList = new List<Sales>();
        if (cartItems == null)
        {
            return false;
        }
        foreach (SelectedItem item in cartItems)
        {
            Sales sale = new Sales
            {
                TransactionId = transactionId,
                UserId = user.Id,
                date_of_sale = currentDate,
                ProductId = item.ProductId
            };

            salesList.Add(sale);
        }

        foreach (var entity in salesList)
        {
            var existingEntity = db.ChangeTracker.Entries<Sales>()
                .FirstOrDefault(e => e.Entity.TransactionId == entity.TransactionId);

            if (existingEntity != null)
            {
                existingEntity.State = EntityState.Detached;
            }
        }
        db.Sales.AddRange(salesList);
        db.SaveChanges();
        return true;
    }

  

    public List<Item> GetAllItems()
    {
        return db.Items.OrderBy(i => i.ProductId).ToList();
    }

    public Item? GetItemById(int id)
    {
        return db.Items.Where(i => i.ProductId == id).FirstOrDefault();
    }

    //public ShoppingCart? GetOrCreateCart(ISession session)
    //{
    //    if (session.GetObjectFromJson<ShoppingCart>("ShoppingCart") == null)
    //    {
    //        var newCart = new ShoppingCart();
    //        session.SetObjectAsJson("ShoppingCart", newCart);
    //    }

    //    return session.GetObjectFromJson<ShoppingCart>("ShoppingCart");
    //}

    //public Person? GetUserByUsername(string username)
    //{
    //    var user = db.Users.Where(u => u.UserName == username).FirstOrDefault();
    //    if (user == null)
    //    {
    //        return null;
    //    }
    //    return user;
    //}

    public bool AddItemsToDb(Item item)
    {
        try
        {
            if (item == null)
            {
                throw new InvalidOperationException("Item is required.");
            }

            item.ProductName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(item.ProductName.ToLower());
            db.Items.Add(item);
            db.SaveChanges();
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return false;
        }
    }

    public bool RemoveItemsDb(int productId)
    {
        var itemToRemove = db.Items.Where(i => i.ProductId == productId).FirstOrDefault();
        if (itemToRemove != null)
        {
            db.Items.Remove(itemToRemove);
            db.SaveChanges();
            return true;
        }
        return false;
    }

    public bool UpdateItemInDb(Item item)
    {
        var itemToUpdate = db.Items.Where(i => i.ProductId == item.ProductId).FirstOrDefault();
        if (itemToUpdate != null)
        {
            itemToUpdate.ProductName = item.ProductName;
            itemToUpdate.ProductDescription = item.ProductDescription;
            itemToUpdate.ProductPrice = item.ProductPrice;
            itemToUpdate.ProductQty = item.ProductQty;

            db.SaveChanges();
            return true;
        }
        return false;
    }

    public List<Item> getSerachedItem(string shopItem)
    {
        var searchedItem = db.Items.Where(i => i.ProductName.Equals(shopItem)).FirstOrDefault();
        List<Item> result = new List<Item>();
        if (searchedItem != null)
        {
            result.Add(searchedItem);
        }
        return result;
       
    }
}
