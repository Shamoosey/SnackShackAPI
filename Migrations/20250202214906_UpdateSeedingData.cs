using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackShackAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CurrencyExchangeRates",
                keyColumn: "Id",
                keyValue: new Guid("20527053-db1e-44b2-9234-5e205b599e87"),
                column: "Rate",
                value: 0.10000000000000001);

            migrationBuilder.UpdateData(
                table: "CurrencyExchangeRates",
                keyColumn: "Id",
                keyValue: new Guid("b4de049e-a4f7-4cdd-a696-e793990eef66"),
                column: "Rate",
                value: 10.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "CurrencyExchangeRates",
                keyColumn: "Id",
                keyValue: new Guid("20527053-db1e-44b2-9234-5e205b599e87"),
                column: "Rate",
                value: 10.0);

            migrationBuilder.UpdateData(
                table: "CurrencyExchangeRates",
                keyColumn: "Id",
                keyValue: new Guid("b4de049e-a4f7-4cdd-a696-e793990eef66"),
                column: "Rate",
                value: 0.10000000000000001);
        }
    }
}
