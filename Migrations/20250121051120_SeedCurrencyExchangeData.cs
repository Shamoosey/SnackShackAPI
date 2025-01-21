using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SnackShackAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCurrencyExchangeData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.CreateTable(
                name: "CurrencyExchangeRates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyExchangeRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyExchangeRates_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CurrencyCode", "CurrencyName" },
                values: new object[,]
                {
                    { new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17"), "BWL", "Bowl" },
                    { new Guid("26654d57-d733-4646-a7cd-78db9ba09a24"), "MIL", "Million" },
                    { new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073"), "WOD", "Da Wood" }
                });

            migrationBuilder.InsertData(
                table: "CurrencyExchangeRates",
                columns: new[] { "Id", "EffectiveDate", "FromCurrencyId", "Rate", "ToCurrencyId" },
                values: new object[,]
                {
                    { new Guid("17b8fb92-98c3-401f-bf0e-ddde8450d72b"), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073"), 25.0, new Guid("26654d57-d733-4646-a7cd-78db9ba09a24") },
                    { new Guid("20527053-db1e-44b2-9234-5e205b599e87"), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17"), 10.0, new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073") },
                    { new Guid("3a30db39-1654-46a4-84fe-a60c49994ab0"), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("26654d57-d733-4646-a7cd-78db9ba09a24"), 0.20000000000000001, new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17") },
                    { new Guid("b4de049e-a4f7-4cdd-a696-e793990eef66"), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073"), 0.10000000000000001, new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17") },
                    { new Guid("c608df92-d128-4272-afc6-f0f685b3f277"), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17"), 5.0, new Guid("26654d57-d733-4646-a7cd-78db9ba09a24") },
                    { new Guid("fc2af90e-00f5-4f2c-9742-50b8132da6d8"), new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("26654d57-d733-4646-a7cd-78db9ba09a24"), 0.040000000000000001, new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_FromCurrencyId",
                table: "CurrencyExchangeRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyExchangeRates_ToCurrencyId",
                table: "CurrencyExchangeRates",
                column: "ToCurrencyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyExchangeRates");

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("18a8c6a6-18a1-4421-9bfd-5886a011be17"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("26654d57-d733-4646-a7cd-78db9ba09a24"));

            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("6922d54e-5509-4e5c-aeaa-4399a90f7073"));

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CurrencyCode", "CurrencyName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "BWL", "Bowl" });
        }
    }
}
