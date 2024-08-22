using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DemoWebApplication.Models
{
    public class SalesHistoryResponse
    {
        public int TransactionId { get; set; }

        public int ProductId { get; set; }

        public DateTime Date_of_sale { get; set; }


        public SalesHistoryResponse(int transactionId, int productId, DateTime date_of_sale)
        {
            TransactionId = transactionId;
            ProductId = productId;
            Date_of_sale = date_of_sale;
        }
    }
}
