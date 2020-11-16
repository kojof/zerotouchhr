using Microsoft.EntityFrameworkCore.Migrations;

namespace ZeroTouchHR.Migrations
{
    public partial class AddedUserNameField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Employee",
                table: "Employee");

            migrationBuilder.RenameTable(
                name: "Employee",
                newName: "employee");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "employee",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_employee",
                table: "employee",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_employee",
                table: "employee");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "employee");

            migrationBuilder.RenameTable(
                name: "employee",
                newName: "Employee");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Employee",
                table: "Employee",
                column: "id");
        }
    }
}
