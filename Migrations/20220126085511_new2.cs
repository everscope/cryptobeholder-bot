using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoBeholderBot.Migrations
{
    public partial class new2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Persent",
                table: "TracesSettings",
                type: "decimal(3,2)",
                precision: 3,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,10)",
                oldPrecision: 10,
                oldScale: 10,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Persent",
                table: "TracesSettings",
                type: "decimal(10,10)",
                precision: 10,
                scale: 10,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldPrecision: 3,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
