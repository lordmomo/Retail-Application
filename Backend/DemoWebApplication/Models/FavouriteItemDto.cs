using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DemoWebApplication.Models
{
    public class FavouriteItemDto
    {
        public int FavouriteItemId { get; set; }

        public string UserId { get; set; }

        public int ProductId { get; set; }

    }
}
