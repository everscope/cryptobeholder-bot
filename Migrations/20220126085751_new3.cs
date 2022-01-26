using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBeholderBot.Migrations
{
    public partial class new3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "TrackedCoins");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "TrackedCoins",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
