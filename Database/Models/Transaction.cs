using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnackShackAPI.Database.Models
{
    public partial class Transaction
    {
        public Transaction()
        {
        }

        [Key]
        public Guid Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public TransactionType TransactionType { get; set; }

        public string? Notes { get; set; }


        [ForeignKey(nameof(InitiatedByUser))]
        public Guid InitiatedByUserId { get; set; }

        [ForeignKey(nameof(SenderAccount))]
        public Guid? SenderAccountId { get; set; }

        [ForeignKey(nameof(ReceiverAccount))]
        public Guid? ReceiverAccountId { get; set; }

        // Navigation properties
        public virtual User InitiatedByUser { get; set; }
        public virtual Account? SenderAccount { get; set; }
        public virtual Account? ReceiverAccount { get; set; }
        public virtual ICollection<AccountHistory> AccountHistories { get; set; }
    }
}
