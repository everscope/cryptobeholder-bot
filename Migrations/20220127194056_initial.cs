using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBeholderBot.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    VsCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "TrackedCoins",
                columns: table => new
                {
                    TrackedId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Coin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedCoins", x => x.TrackedId);
                    table.ForeignKey(
                        name: "FK_TrackedCoins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TracesSettings",
                columns: table => new
                {
                    TrackedCoinTrackedId = table.Column<int>(type: "int", nullable: false),
                    AbsoluteMax = table.Column<decimal>(type: "decimal(12,10)", precision: 12, scale: 10, nullable: true),
                    AbsoluteMin = table.Column<decimal>(type: "decimal(12,10)", precision: 12, scale: 10, nullable: true),
                    Persent = table.Column<decimal>(type: "decimal(3,2)", precision: 3, scale: 2, nullable: true),
                    TracingMode = table.Column<int>(type: "int", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TracesSettings", x => x.TrackedCoinTrackedId);
                    table.ForeignKey(
                        name: "FK_TracesSettings_TrackedCoins_TrackedCoinTrackedId",
                        column: x => x.TrackedCoinTrackedId,
                        principalTable: "TrackedCoins",
                        principalColumn: "TrackedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCoins_UserId",
                table: "TrackedCoins",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TracesSettings");

            migrationBuilder.DropTable(
                name: "TrackedCoins");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
