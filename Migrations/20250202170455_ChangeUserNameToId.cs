using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SnackShackAPI.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserNameToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Username",
                table: "Users",
                newName: "DiscordUserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DiscordUserID",
                table: "Users",
                newName: "Username");
        }
    }
}
