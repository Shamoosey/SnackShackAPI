namespace SnackShackAPI.Models
{
    public class UserTransferFundsRequest
    {
        public Guid FromAccountId { get; set; }
        public Guid ToAccountId { get; set; }
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
    }

}
