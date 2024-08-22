using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Models;

[ExcludeFromCodeCoverage]
public class ApplicationDbContext : IdentityDbContext<Person>
{
    protected readonly IConfiguration _configuration;

    // Production constructor

    //public ApplicationDbContext( IConfiguration configuration) 
    //{
    //    _configuration = configuration;
    //}


    // For testing purposes
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options 
        , IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;

    }

    //protected override void OnConfiguring(DbContextOptionsBuilder options)
    //{
    //    options.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));

    //}


    //public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    //    : base(options)
    //{
    //}
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if ( _configuration != null)
        {
            options.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
        }
    }

    public DbSet<Person> Users { get; set; }

    public DbSet<Item> Items { get; set; }

    public DbSet<Sales> Sales { get; set; }

    public DbSet<FavouriteItem> FavouriteItems { get; set; }

    //protected override void OnModelCreating(ModelBuilder builder)
    //{
    //    base.OnModelCreating(builder);
    //}
}
