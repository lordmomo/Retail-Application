using DemoWebApplication.Models;
using DemoWebTestAppication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebTestAppication.Utils
{
    public class MockApplicationDb
    {

        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private readonly ApplicationDbContext _applicationDbContext;


        public MockApplicationDb()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
              .UseInMemoryDatabase(databaseName: "TestDatabaseForMock")
              .Options;

            _applicationDbContext = new ApplicationDbContext(_dbContextOptions, null);

            SeedDatabase();
        }


        public ApplicationDbContext GetApplicationDbContext()
        {
            return _applicationDbContext;
        }

        private void SeedDatabase()
        {
            using (var dbContext = new ApplicationDbContext(_dbContextOptions, null))
            {
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
                dbContext.Items.AddRange(item1,item2,item3);

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




                //dbContext.SaveChanges();
                //var user1 = dbContext.Users.First(u => u.UserName == "user1");
                //var user2 = dbContext.Users.First(u => u.UserName == "user2");

                //var item1 = dbContext.Items.First(i => i.ProductId == 1);
                //var item2 = dbContext.Items.First(i => i.ProductId == 2);
                //var item3 = dbContext.Items.First(i => i.ProductId == 3);

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
    }
}
