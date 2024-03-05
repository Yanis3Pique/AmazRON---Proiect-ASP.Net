using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazRON.Data.Migrations
{
    public partial class Revelion_V2_AmazRON : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Products");
        }
    }
}
