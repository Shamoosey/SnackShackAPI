using SnackShackAPI.Database.Models;

namespace SnackShackAPI.Models
{
    public class UpdateAccountBalanceRequest
    {
        public Guid UserId { get; set; }
        public Guid AccountId { get; set; }
        public decimal Amount { get; set; }
        public string Notes { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}
