using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class UpdatedUserRefreshTokenPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_email_template_client_client_entity_id",
                table: "email_template");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_refresh_tokens",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.DropIndex(
                name: "ix_user_refresh_tokens_user_id",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.RenameIndex(
                name: "ix_email_template_client_id",
                table: "email_template",
                newName: "ix_email_templates_client_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_refresh_token",
                schema: "identity",
                table: "user_refresh_token",
                columns: new[] { "user_id", "client_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_email_templates_clients_client_id",
                table: "email_template",
                column: "client_id",
                principalSchema: "identity",
                principalTable: "client",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_email_templates_clients_client_id",
                table: "email_template");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_refresh_token",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.RenameIndex(
                name: "ix_email_templates_client_id",
                table: "email_template",
                newName: "ix_email_template_client_id");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "identity",
                table: "user_refresh_token",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_refresh_tokens",
                schema: "identity",
                table: "user_refresh_token",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "ix_user_refresh_tokens_user_id",
                schema: "identity",
                table: "user_refresh_token",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_email_template_client_client_entity_id",
                table: "email_template",
                column: "client_id",
                principalSchema: "identity",
                principalTable: "client",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
