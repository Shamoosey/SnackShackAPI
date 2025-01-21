namespace SnackShackAPI.Models
{
    public class ExchangeRateDTO
    {
        public Guid FromCurrencyId { get; set; }
        public Guid ToCurrencyId { get; set; }
        public double Rate { get; set; }
    }
}
