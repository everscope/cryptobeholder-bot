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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    VsCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrackedCoins",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ChatId = table.Column<int>(type: "int", nullable: false),
                    Coin = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackedCoins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrackedCoins_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TracesSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    AbsoluteMax = table.Column<decimal>(type: "decimal(12,10)", precision: 12, scale: 10, nullable: true),
                    AbsoluteMin = table.Column<decimal>(type: "decimal(12,10)", precision: 12, scale: 10, nullable: true),
                    Persent = table.Column<decimal>(type: "decimal(2,2)", precision: 2, scale: 2, nullable: true),
                    TracingMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TracesSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TracesSettings_TrackedCoins_Id",
                        column: x => x.Id,
                        principalTable: "TrackedCoins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
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
