namespace SnackShackAPI.DTOs
{
    public class AccountDTO
    {
        public Guid AccountId { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; }

        public Guid CurrencyId { get; set; }

        public string AccountName { get; set; }
    }
}
