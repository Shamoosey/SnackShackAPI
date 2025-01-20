using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnackShackAPI.Database.Models
{
    public class Currency
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
