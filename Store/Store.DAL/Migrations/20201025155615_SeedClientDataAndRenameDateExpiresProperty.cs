using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class SeedClientDataAndRenameDateExpiresProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expires_utc",
                table: "user_refresh_token");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_expires_utc",
                table: "user_refresh_token",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.InsertData(
                table: "client",
                columns: new[] { "id", "access_token_life_time", "active", "allowed_origin", "application_type", "date_created_utc", "date_updated_utc", "description", "name", "refresh_token_life_time", "secret" },
                values: new object[] { new Guid("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"), 20, true, "*", 1, new DateTime(2020, 10, 25, 16, 44, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 25, 16, 44, 0, 0, DateTimeKind.Unspecified), "Web API Application", "WebApiApplication", 60, "28IOjCR2kNUeT3dIoFLJpn7oAtLrpzofaSzlXi+dxG9cyFul0tiBJc3BWPWTDVkzoAkSkXsFZ8u7ON05wQ276w==" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "client",
                keyColumn: "id",
                keyValue: new Guid("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"));

            migrationBuilder.DropColumn(
                name: "date_expires_utc",
                table: "user_refresh_token");

            migrationBuilder.AddColumn<DateTime>(
                name: "expires_utc",
                table: "user_refresh_token",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
