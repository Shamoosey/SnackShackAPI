﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SnackShackAPI.Database;

#nullable disable

namespace SnackShackAPI.Migrations
{
    [DbContext(typeof(SnackShackContext))]
    partial class SnackShackContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SnackShackAPI.Database.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AccountName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("CurrencyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("CurrencyId");

                    b.HasIndex("UserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.AccountHistory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ChangeDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("NewAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("PreviousAmount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid?>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex("TransactionId");

                    b.ToTable("AccountHistories");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.Currency", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CurrencyCode")
                        .IsRequired()
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<string>("CurrencyName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Currencies");

                    b.HasData(
                        new
                        {
                            Id = new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                            CurrencyCode = "BWL",
                            CurrencyName = "Bowl"
                        },
                        new
                        {
                            Id = new Guid("26654d57-d733-4646-a7cd-78db9ba09a24"),
                            CurrencyCode = "MIL",
                            CurrencyName = "Million"
                        },
                        new
                        {
                            Id = new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                            CurrencyCode = "WOD",
                            CurrencyName = "Da Wood"
                        });
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.CurrencyExchangeRate", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("EffectiveDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("FromCurrencyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Rate")
                        .HasColumnType("float");

                    b.Property<Guid>("ToCurrencyId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("FromCurrencyId");

                    b.HasIndex("ToCurrencyId");

                    b.ToTable("CurrencyExchangeRates");

                    b.HasData(
                        new
                        {
                            Id = new Guid("3a30db39-1654-46a4-84fe-a60c49994ab0"),
                            EffectiveDate = new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                            FromCurrencyId = new Guid("26654d57-d733-4646-a7cd-78db9ba09a24"),
                            Rate = 0.20000000000000001,
                            ToCurrencyId = new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17")
                        },
                        new
                        {
                            Id = new Guid("c608df92-d128-4272-afc6-f0f685b3f277"),
                            EffectiveDate = new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                            FromCurrencyId = new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                            Rate = 5.0,
                            ToCurrencyId = new Guid("26654d57-d733-4646-a7cd-78db9ba09a24")
                        },
                        new
                        {
                            Id = new Guid("20527053-db1e-44b2-9234-5e205b599e87"),
                            EffectiveDate = new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                            FromCurrencyId = new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17"),
                            Rate = 10.0,
                            ToCurrencyId = new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073")
                        },
                        new
                        {
                            Id = new Guid("b4de049e-a4f7-4cdd-a696-e793990eef66"),
                            EffectiveDate = new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                            FromCurrencyId = new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                            Rate = 0.10000000000000001,
                            ToCurrencyId = new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17")
                        },
                        new
                        {
                            Id = new Guid("17b8fb92-98c3-401f-bf0e-ddde8450d72b"),
                            EffectiveDate = new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                            FromCurrencyId = new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073"),
                            Rate = 25.0,
                            ToCurrencyId = new Guid("26654d57-d733-4646-a7cd-78db9ba09a24")
                        },
                        new
                        {
                            Id = new Guid("fc2af90e-00f5-4f2c-9742-50b8132da6d8"),
                            EffectiveDate = new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc),
                            FromCurrencyId = new Guid("26654d57-d733-4646-a7cd-78db9ba09a24"),
                            Rate = 0.040000000000000001,
                            ToCurrencyId = new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073")
                        });
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.Transaction", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<Guid>("InitiatedByUserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Notes")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ReceiverAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("SenderAccountId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TransactionType")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InitiatedByUserId");

                    b.HasIndex("ReceiverAccountId");

                    b.HasIndex("SenderAccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.Account", b =>
                {
                    b.HasOne("SnackShackAPI.Database.Models.Currency", "Currency")
                        .WithMany("Accounts")
                        .HasForeignKey("CurrencyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SnackShackAPI.Database.Models.User", "User")
                        .WithMany("Accounts")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Currency");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.AccountHistory", b =>
                {
                    b.HasOne("SnackShackAPI.Database.Models.Account", "Account")
                        .WithMany("AccountHistories")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SnackShackAPI.Database.Models.Transaction", "Transaction")
                        .WithMany("AccountHistories")
                        .HasForeignKey("TransactionId");

                    b.Navigation("Account");

                    b.Navigation("Transaction");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.CurrencyExchangeRate", b =>
                {
                    b.HasOne("SnackShackAPI.Database.Models.Currency", "FromCurrency")
                        .WithMany("FromExchangeRates")
                        .HasForeignKey("FromCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SnackShackAPI.Database.Models.Currency", "ToCurrency")
                        .WithMany("ToExchangeRates")
                        .HasForeignKey("ToCurrencyId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("FromCurrency");

                    b.Navigation("ToCurrency");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.Transaction", b =>
                {
                    b.HasOne("SnackShackAPI.Database.Models.User", "InitiatedByUser")
                        .WithMany("InitiatedTransactions")
                        .HasForeignKey("InitiatedByUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SnackShackAPI.Database.Models.Account", "ReceiverAccount")
                        .WithMany("TransactionsAsReceiver")
                        .HasForeignKey("ReceiverAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SnackShackAPI.Database.Models.Account", "SenderAccount")
                        .WithMany("TransactionsAsSender")
                        .HasForeignKey("SenderAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("InitiatedByUser");

                    b.Navigation("ReceiverAccount");

                    b.Navigation("SenderAccount");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.Account", b =>
                {
                    b.Navigation("AccountHistories");

                    b.Navigation("TransactionsAsReceiver");

                    b.Navigation("TransactionsAsSender");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.Currency", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("FromExchangeRates");

                    b.Navigation("ToExchangeRates");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.Transaction", b =>
                {
                    b.Navigation("AccountHistories");
                });

            modelBuilder.Entity("SnackShackAPI.Database.Models.User", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("InitiatedTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}
