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

            modelBuilder.Entity<Currency>().HasData(
               new Currency
               {
                   Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                   CurrencyCode = "BWL",
                   CurrencyName = "Bowl"
               }
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AccountHistory> AccountHistories { get; set; }
    }
}
