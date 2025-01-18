using System.ComponentModel.DataAnnotations;

namespace SnackShackAPI.Database.Models
{
    public class Currency
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(3)]
        public string CurrencyCode { get; set; }

        [Required]
        [MaxLength(50)]
        public string CurrencyName { get; set; }
       
        
        // Navigation properties
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
