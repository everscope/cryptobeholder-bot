using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBeholderBot.Migrations
{
    public partial class @new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Persent",
                table: "TracesSettings",
                type: "decimal(10,10)",
                precision: 10,
                scale: 10,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(2,2)",
                oldPrecision: 2,
                oldScale: 2,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Persent",
                table: "TracesSettings",
                type: "decimal(2,2)",
                precision: 2,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,10)",
                oldPrecision: 10,
                oldScale: 10,
                oldNullable: true);
        }
    }
}
