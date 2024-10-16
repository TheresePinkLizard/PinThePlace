using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PinThePlace.Migrations
{
    /// <inheritdoc />
    public partial class ShopDbExpanded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Pins",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pins_UserId",
                table: "Pins",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pins_Users_UserId",
                table: "Pins",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pins_Users_UserId",
                table: "Pins");

            migrationBuilder.DropIndex(
                name: "IX_Pins_UserId",
                table: "Pins");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Pins");
        }
    }
}
