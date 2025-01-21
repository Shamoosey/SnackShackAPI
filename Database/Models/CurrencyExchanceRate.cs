using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnackShackAPI.Database.Models
{
    public class CurrencyExchangeRate
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey(nameof(FromCurrency))]
        public Guid FromCurrencyId { get; set; }

        [ForeignKey(nameof(ToCurrency))]
        public Guid ToCurrencyId { get; set; }
        
        [Required]
        public double Rate { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        public virtual Currency FromCurrency { get; set; }

        public virtual Currency ToCurrency { get; set; }
    }
}
