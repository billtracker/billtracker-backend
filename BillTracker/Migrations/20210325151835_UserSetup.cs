using Microsoft.EntityFrameworkCore.Migrations;

namespace BillTracker.Migrations
{
    public partial class UserSetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasSetup",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WasSetup",
                table: "Users");
        }
    }
}
