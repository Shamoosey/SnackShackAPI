using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackShackAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedCurrencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("3229b017-7f9e-4578-abbc-e71f9dd0a1d3"));

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CurrencyCode", "CurrencyName" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), "BWL", "Bowl" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Currencies",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "CurrencyCode", "CurrencyName" },
                values: new object[] { new Guid("3229b017-7f9e-4578-abbc-e71f9dd0a1d3"), "BWL", "Bowl" });
        }
    }
}
