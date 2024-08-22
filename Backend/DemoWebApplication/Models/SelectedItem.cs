using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DemoWebApplication.Models
{
    public class SelectedItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public string ProductCategoryName { get; set; }

        public double ProductPrice { get; set; }

        public double ProductQty { get; set; }

        public double selectedQuantity { get; set; }
    }
}
