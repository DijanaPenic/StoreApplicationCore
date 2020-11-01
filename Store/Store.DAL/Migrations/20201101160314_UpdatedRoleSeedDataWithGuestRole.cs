using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class UpdatedRoleSeedDataWithGuestRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "role",
                columns: new[] { "id", "concurrency_stamp", "date_created_utc", "date_updated_utc", "name", "normalized_name", "stackable" },
                values: new object[] { new Guid("9621c09c-06b1-45fb-8baf-38e0757e2f59"), null, new DateTime(2020, 10, 30, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 30, 18, 44, 11, 0, DateTimeKind.Unspecified), "Guest", "GUEST", false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role",
                keyColumn: "id",
                keyValue: new Guid("9621c09c-06b1-45fb-8baf-38e0757e2f59"));
        }
    }
}
