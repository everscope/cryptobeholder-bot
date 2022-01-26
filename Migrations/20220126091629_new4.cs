using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBeholderBot.Migrations
{
    public partial class new4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TracesSettings_TrackedCoins_Id",
                table: "TracesSettings");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "TracesSettings",
                newName: "TrackedId");

            migrationBuilder.AddForeignKey(
                name: "FK_TracesSettings_TrackedCoins_TrackedId",
                table: "TracesSettings",
                column: "TrackedId",
                principalTable: "TrackedCoins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TracesSettings_TrackedCoins_TrackedId",
                table: "TracesSettings");

            migrationBuilder.RenameColumn(
                name: "TrackedId",
                table: "TracesSettings",
                newName: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TracesSettings_TrackedCoins_Id",
                table: "TracesSettings",
                column: "Id",
                principalTable: "TrackedCoins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
