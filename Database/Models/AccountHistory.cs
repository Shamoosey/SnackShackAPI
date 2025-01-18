using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SnackShackAPI.Database.Models
{
    public class AccountHistory
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public DateTime ChangeDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PreviousAmount { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal NewAmount { get; set; }

        [ForeignKey(nameof(Account))]
        public Guid AccountId { get; set; }

        [ForeignKey(nameof(Transaction))]
        public Guid? TransactionId { get; set; } // Nullable for manual adjustments

        // Navigation properties
        public virtual Account Account { get; set; }
        public virtual Transaction? Transaction { get; set; }
    }
}
