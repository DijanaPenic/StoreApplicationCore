using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class UpdatedUserLoginTableWithTokenAndIsConfirmedProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_confirmed",
                schema: "identity",
                table: "user_login",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "token",
                schema: "identity",
                table: "user_login",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_confirmed",
                schema: "identity",
                table: "user_login");

            migrationBuilder.DropColumn(
                name: "token",
                schema: "identity",
                table: "user_login");
        }
    }
}
