using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnackShackAPI.Database.Models
{
    public partial class Account
    {
        public Account() 
        {
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string AccountName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Currency))]
        public Guid CurrencyId { get; set; }


        public virtual User User { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual ICollection<Transaction> TransactionsAsSender { get; set; }
        public virtual ICollection<Transaction> TransactionsAsReceiver { get; set; }
        public virtual ICollection<AccountHistory> AccountHistories { get; set; }
    }
}
