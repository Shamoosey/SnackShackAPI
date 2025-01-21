namespace SnackShackAPI.Models
{
    public class TransferFundsRequest
    {
        public Guid UserId { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
    }
}
