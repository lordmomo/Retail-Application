using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DemoWebApplication.Models
{
    public class FavouriteItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column(Order = 0)]
        public int FavouriteItemId { get; set; }

        [ForeignKey("Person")] 
        public string UserId { get; set; }
        public virtual Person User { get; set; }

        [ForeignKey("Item")]
        public int ProductId { get; set; }
        public virtual Item Product { get; set; }


    }
}
