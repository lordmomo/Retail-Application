using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Models;

public class Login
{
    [Required]
    [NotNull]
    public string Username { get; set; }

    [Required]
    [MinLength(8, ErrorMessage = "Password needs a minimum 8 characters")]
    [NotNull]
    [DataType(DataType.Password)]
    public string Password { get; set; }


}
