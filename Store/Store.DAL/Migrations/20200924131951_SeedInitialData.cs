using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class SeedInitialData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Bookstore",
                columns: new[] { "Id", "DateCreatedUtc", "DateUpdatedUtc", "Location", "Name" },
                values: new object[,]
                {
                    { new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(5457), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(5719), "2526 E Colfax Ave, Denver, WA", "Strand Book Store" },
                    { new Guid("a4b57c3c-4c09-4b8c-b2f8-e88ce049f30c"), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(5988), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(6000), "18325 Campus Way NE, Bothell, WA", "Powell's City of Books" },
                    { new Guid("fa588725-0c60-4554-8eb9-20520038ee87"), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(6013), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(6014), "3415 SW Cedar Hills Blvd, Beaverton, OR", "Shakespeare & Co" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "DateCreatedUtc", "DateUpdatedUtc", "Name", "Stackable" },
                values: new object[,]
                {
                    { new Guid("7ae9e63d-8bfa-45d4-2f01-08d8608c84c3"), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(3769), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4166), "Admin", false },
                    { new Guid("85b336c4-f9ce-4480-2f02-08d8608c84c3"), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4450), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4459), "Customer", true },
                    { new Guid("9cf98591-311c-4e5e-2f03-08d8608c84c3"), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4475), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4475), "Store Manager", true }
                });

            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Id", "Author", "BookstoreId", "DateCreatedUtc", "DateUpdatedUtc", "Name" },
                values: new object[,]
                {
                    { new Guid("6ee5782a-575a-476c-2f04-08d8608c84c3"), "J. R. R. Tolkien", new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"), new DateTime(2020, 9, 24, 13, 19, 51, 211, DateTimeKind.Utc).AddTicks(341), new DateTime(2020, 9, 24, 13, 19, 51, 211, DateTimeKind.Utc).AddTicks(626), "The Lord of the Rings" },
                    { new Guid("0563508d-f97c-44aa-2f05-08d8608c84c3"), "Paulo Coelho", new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"), new DateTime(2020, 9, 24, 13, 19, 51, 211, DateTimeKind.Utc).AddTicks(920), new DateTime(2020, 9, 24, 13, 19, 51, 211, DateTimeKind.Utc).AddTicks(929), "The Alchemist" },
                    { new Guid("cdccce7b-a0b9-4c03-2f06-08d8608c84c3"), "Antoine de Saint-Exupéry", new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"), new DateTime(2020, 9, 24, 13, 19, 51, 211, DateTimeKind.Utc).AddTicks(951), new DateTime(2020, 9, 24, 13, 19, 51, 211, DateTimeKind.Utc).AddTicks(952), "The Little Prince" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: new Guid("0563508d-f97c-44aa-2f05-08d8608c84c3"));

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: new Guid("6ee5782a-575a-476c-2f04-08d8608c84c3"));

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: new Guid("cdccce7b-a0b9-4c03-2f06-08d8608c84c3"));

            migrationBuilder.DeleteData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("a4b57c3c-4c09-4b8c-b2f8-e88ce049f30c"));

            migrationBuilder.DeleteData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("fa588725-0c60-4554-8eb9-20520038ee87"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("7ae9e63d-8bfa-45d4-2f01-08d8608c84c3"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("85b336c4-f9ce-4480-2f02-08d8608c84c3"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("9cf98591-311c-4e5e-2f03-08d8608c84c3"));

            migrationBuilder.DeleteData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"));
        }
    }
}
