using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SnackShackAPI.Database.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string DiscordUserID { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        public bool IsAdmin { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Transaction> InitiatedTransactions { get; set; }
    }
}
