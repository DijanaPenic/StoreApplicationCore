using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class ChangedDatabaseSchemaForIdentityTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.RenameTable(
                name: "user_token",
                newName: "user_token",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "user_role",
                newName: "user_role",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "user_refresh_token",
                newName: "user_refresh_token",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "user_login",
                newName: "user_login",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "user_claim",
                newName: "user_claim",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "user",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "role_claim",
                newName: "role_claim",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "role",
                newName: "role",
                newSchema: "identity");

            migrationBuilder.RenameTable(
                name: "client",
                newName: "client",
                newSchema: "identity");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "user_token",
                schema: "identity",
                newName: "user_token");

            migrationBuilder.RenameTable(
                name: "user_role",
                schema: "identity",
                newName: "user_role");

            migrationBuilder.RenameTable(
                name: "user_refresh_token",
                schema: "identity",
                newName: "user_refresh_token");

            migrationBuilder.RenameTable(
                name: "user_login",
                schema: "identity",
                newName: "user_login");

            migrationBuilder.RenameTable(
                name: "user_claim",
                schema: "identity",
                newName: "user_claim");

            migrationBuilder.RenameTable(
                name: "user",
                schema: "identity",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "role_claim",
                schema: "identity",
                newName: "role_claim");

            migrationBuilder.RenameTable(
                name: "role",
                schema: "identity",
                newName: "role");

            migrationBuilder.RenameTable(
                name: "client",
                schema: "identity",
                newName: "client");
        }
    }
}
