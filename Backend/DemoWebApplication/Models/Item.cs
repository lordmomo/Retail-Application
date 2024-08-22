using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Models;
public class Item
{
    [Key]
    public int ProductId { get; set; }
    [Required]
    [NotNull]
    public string ProductName { get; set; }
    [Required]
    [NotNull]
    public string ProductDescription { get; set; }

    [Required]
    [NotNull]
    public string ProductCategoryName { get; set; }
    [Required]
    [NotNull]
    public double ProductPrice { get; set; }
    [Required]
    [NotNull]
    public double ProductQty { get; set; }
}
