using Microsoft.EntityFrameworkCore;
using SnackShackAPI.Database.Models;

namespace SnackShackAPI.Database
{
    public class SnackShackContext : DbContext
    {
        public SnackShackContext(DbContextOptions<SnackShackContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
               .HasOne(t => t.SenderAccount)
               .WithMany(a => a.TransactionsAsSender)
               .HasForeignKey(t => t.SenderAccountId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.ReceiverAccount)
                .WithMany(a => a.TransactionsAsReceiver)
                .HasForeignKey(t => t.ReceiverAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AccountHistory> AccountHistories { get; set; }
    }
}
