using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class CreatedRefreshTokenAndClientTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_user_tokens",
                table: "user_token");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_roles",
                table: "user_role");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_logins",
                table: "user_login");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_token",
                table: "user_token",
                columns: new[] { "user_id", "login_provider", "name" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_role",
                table: "user_role",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_login",
                table: "user_login",
                columns: new[] { "login_provider", "provider_key" });

            migrationBuilder.CreateTable(
                name: "client",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    name = table.Column<string>(maxLength: 50, nullable: false),
                    secret = table.Column<string>(nullable: false),
                    description = table.Column<string>(maxLength: 100, nullable: true),
                    application_type = table.Column<int>(nullable: false),
                    active = table.Column<bool>(nullable: false),
                    access_token_life_time = table.Column<int>(nullable: false),
                    refresh_token_life_time = table.Column<int>(nullable: false),
                    allowed_origin = table.Column<string>(maxLength: 100, nullable: true),
                    date_created_utc = table.Column<DateTime>(nullable: false),
                    date_updated_utc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_refresh_token",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    value = table.Column<string>(maxLength: 256, nullable: false),
                    user_id = table.Column<Guid>(nullable: false),
                    client_id = table.Column<Guid>(nullable: false),
                    date_created_utc = table.Column<DateTime>(nullable: false),
                    date_updated_utc = table.Column<DateTime>(nullable: false),
                    expires_utc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_refresh_token_clients_client_entity_id",
                        column: x => x.client_id,
                        principalTable: "client",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_refresh_token_user_user_entity_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "NameIndex",
                table: "client",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_refresh_token_client_id",
                table: "user_refresh_token",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_refresh_token_user_id",
                table: "user_refresh_token",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_refresh_token");

            migrationBuilder.DropTable(
                name: "client");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_token",
                table: "user_token");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_role",
                table: "user_role");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_login",
                table: "user_login");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_tokens",
                table: "user_token",
                columns: new[] { "user_id", "login_provider", "name" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_roles",
                table: "user_role",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_logins",
                table: "user_login",
                columns: new[] { "login_provider", "provider_key" });
        }
    }
}
