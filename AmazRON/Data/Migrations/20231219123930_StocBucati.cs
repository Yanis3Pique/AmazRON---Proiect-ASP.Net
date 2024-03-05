using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazRON.Data.Migrations
{
    public partial class StocBucati : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bucati",
                table: "UserProducts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Stoc",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bucati",
                table: "UserProducts");

            migrationBuilder.DropColumn(
                name: "Stoc",
                table: "Products");
        }
    }
}
