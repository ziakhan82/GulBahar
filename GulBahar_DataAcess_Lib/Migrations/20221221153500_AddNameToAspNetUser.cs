using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GulBahar_DataAcess_Lib.Migrations
{
    public partial class AddNameToAspNetUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "User",
                table: "AspNetUsers",
                newName: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "AspNetUsers",
                newName: "User");
        }
    }
}
