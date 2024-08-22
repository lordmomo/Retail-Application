using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace DemoWebApplication.Models;

public class Person : IdentityUser
{
    [Required(ErrorMessage = " Name is required")]
    [MaxLength(20, ErrorMessage = "Name cannot be over 20 characters")]
    [NotNull]
    public string? CustomerName { get; set; }

    public string? Address { get; set; }

    //[Required(ErrorMessage = " Username is required")]
    //[MaxLength(15, ErrorMessage = "Username can have at max 15 characters")]
    //[MinLength(8, ErrorMessage = "Username needs a minimum 8 characters")]
    //public string? Username { get; set; }

    [Required(ErrorMessage = " Password is required")]
    [MinLength(8, ErrorMessage = "Password requires a minimum 8 characters")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Required(ErrorMessage = " Balance is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Balance cannot be negative")]
    public double Balance { get; set; }

    public string? FilePath { get; set; }


    //public Person(string customerName, string address, string username, string password, double balance, string filePath)
    //{
    //    CustomerName = customerName;
    //    Address = address;
    //    UserName = username;
    //    Password = password;
    //    Balance = balance;
    //    FilePath = filePath;
    //}


}
