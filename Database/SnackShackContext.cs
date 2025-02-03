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

            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasOne(t => t.FromCurrency)
                .WithMany(a => a.FromExchangeRates)
                .HasForeignKey(t => t.FromCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CurrencyExchangeRate>()
                .HasOne(t => t.ToCurrency)
                .WithMany(a => a.ToExchangeRates)
                .HasForeignKey(t => t.ToCurrencyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Currency>().HasData(
                new Currency
                {
                    Id = Guid.Parse("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                    CurrencyCode = "BWL",
                    CurrencyName = "Bowl"
                },
                new Currency
                {
                    Id = Guid.Parse("26654d57-d733-4646-a7cd-78db9ba09a24"),
                    CurrencyCode = "MIL",
                    CurrencyName = "Million"
                },
                new Currency
                {
                    Id = Guid.Parse("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                    CurrencyCode = "WOD",
                    CurrencyName = "Da Wood"
                }
            );

            modelBuilder.Entity<CurrencyExchangeRate>().HasData(
                new CurrencyExchangeRate //million to bowl
                {
                    Id = Guid.Parse("3a30db39-1654-46a4-84fe-a60c49994ab0"),
                    ToCurrencyId = Guid.Parse("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                    FromCurrencyId = Guid.Parse("26654d57-d733-4646-a7cd-78db9ba09a24"),
                    EffectiveDate = new DateTime(2025, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                    Rate = 0.2
                },
                new CurrencyExchangeRate //bowl to million
                {
                    Id = Guid.Parse("c608df92-d128-4272-afc6-f0f685b3f277"),
                    FromCurrencyId = Guid.Parse("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                    ToCurrencyId = Guid.Parse("26654d57-d733-4646-a7cd-78db9ba09a24"),
                    EffectiveDate = new DateTime(2025, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                    Rate = 5
                },
                new CurrencyExchangeRate //bowl to Da Wood
                {
                    Id = Guid.Parse("20527053-db1e-44b2-9234-5e205b599e87"),
                    FromCurrencyId = Guid.Parse("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                    ToCurrencyId = Guid.Parse("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                    EffectiveDate = new DateTime(2025, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                    Rate = .1
                },
                new CurrencyExchangeRate //Da Wood to bowl
                {
                    Id = Guid.Parse("b4de049e-a4f7-4cdd-a696-e793990eef66"),
                    FromCurrencyId = Guid.Parse("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                    ToCurrencyId = Guid.Parse("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                    EffectiveDate = new DateTime(2025, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                    Rate = 10
                },
                new CurrencyExchangeRate //Da Wood to Million
                {
                    Id = Guid.Parse("17b8fb92-98c3-401f-bf0e-ddde8450d72b"),
                    FromCurrencyId = Guid.Parse("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                    ToCurrencyId = Guid.Parse("26654d57-d733-4646-a7cd-78db9ba09a24"),
                    EffectiveDate = new DateTime(2025, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                    Rate = 25
                },
                new CurrencyExchangeRate //Million to Dawood
                {
                    Id = Guid.Parse("fc2af90e-00f5-4f2c-9742-50b8132da6d8"),
                    FromCurrencyId = Guid.Parse("26654d57-d733-4646-a7cd-78db9ba09a24"),
                    ToCurrencyId = Guid.Parse("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                    EffectiveDate = new DateTime(2025, 01, 20, 0, 0, 0, DateTimeKind.Utc),
                    Rate = 0.04
                }
            );
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<AccountHistory> AccountHistories { get; set; }
        public DbSet<CurrencyExchangeRate> CurrencyExchangeRates { get; set; }
    }
}
