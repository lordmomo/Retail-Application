namespace DemoWebApplication.Models
{
    public class DeduceBalanceRequestModel
    {
        public string Username { get; set; }
        public List<SelectedItem> CartItems { get; set; }
        public double TotalAmount { get; set; }
    }
}
