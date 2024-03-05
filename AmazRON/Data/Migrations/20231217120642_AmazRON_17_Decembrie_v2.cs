using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AmazRON.Data.Migrations
{
    public partial class AmazRON_17_Decembrie_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "AspNetUsers",
                newName: "Prenume");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "AspNetUsers",
                newName: "Nume");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Prenume",
                table: "AspNetUsers",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "Nume",
                table: "AspNetUsers",
                newName: "FirstName");
        }
    }
}
