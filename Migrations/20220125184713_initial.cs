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
                    ChatId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VsCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.ChatId);
                });

            migrationBuilder.CreateTable(
                name: "TrackedCoins",
                columns: table => new
                {
                    CoinId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    Coin = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedCoins", x => x.CoinId);
                    table.ForeignKey(
                        name: "FK_TrackedCoins_Users_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Users",
                        principalColumn: "ChatId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TracesSettings",
                columns: table => new
                {
                    CoinId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AbsoluteMax = table.Column<decimal>(type: "decimal(12,10)", precision: 12, scale: 10, nullable: true),
                    AbsoluteMin = table.Column<decimal>(type: "decimal(12,10)", precision: 12, scale: 10, nullable: true),
                    Persent = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    TracingMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TracesSettings", x => x.CoinId);
                    table.ForeignKey(
                        name: "FK_TracesSettings_TrackedCoins_CoinId",
                        column: x => x.CoinId,
                        principalTable: "TrackedCoins",
                        principalColumn: "CoinId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackedCoins_ChatId",
                table: "TrackedCoins",
                column: "ChatId");
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
