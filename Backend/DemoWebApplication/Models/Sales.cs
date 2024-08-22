using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoWebApplication.Models;

public class Sales
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key, Column(Order = 0)]
    public int Id { get; set; }
    public int TransactionId { get; set; }

    [ForeignKey("Person")]
    public string UserId { get; set; }
    public virtual Person Person { get; set; }

    public DateTime date_of_sale { get; set; }


    [ForeignKey("Item")]
    public int ProductId { get; set; }
    public virtual Item Item { get; set; }
}
