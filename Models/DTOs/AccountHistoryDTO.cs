namespace SnackShackAPI.Models.DTOs
{
    public class AccountHistoryDTO
    {
        public DateTime ChangeDate { get; set; }
        public decimal NewAmount { get; set; }
        public decimal PreviousAmount { get; set; }
        public decimal TransactionAmount { get; set; }
        public string TransactionNotes { get; set; }
        public string CurrencyCode { get; set; }
    }
}
