namespace SnackShackAPI.Models
{
    public class TransferNotification
    {
        public Guid TransactionId { get; set; }
        public Guid FromUserId { get; set; }
        public string FromAccountName { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
    }

}
