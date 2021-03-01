﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class SeedRoleClaimData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "role_claim",
                columns: new[] { "id", "claim_type", "claim_value", "date_created_utc", "date_updated_utc", "role_id" },
                values: new object[,]
                {
                    { new Guid("31fc1523-8653-4bbc-acab-dee44688439a"), "Permission", "Book.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("171c4701-98e8-4b74-af5e-eda937dbdb24"), "Permission", "Role.Full", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("c0aada02-252d-4231-ae31-4445736c39ea"), "Permission", "EmailTemplate.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("4cc53412-561a-4d29-ac1b-22dd3cf18c32"), "Permission", "EmailTemplate.Update", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("b5695386-1b56-4e77-87ab-6b58c16f5594"), "Permission", "EmailTemplate.Create", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("f6534a40-cb9a-43a9-b00f-01471fff8c73"), "Permission", "EmailTemplate.Delete", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("ee777566-7a40-4aa1-9923-2758a38e777a"), "Permission", "EmailTemplate.Full", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("fd843c83-ddeb-4ca9-957f-333ee739402c"), "Permission", "Role.Delete", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("3f3b40be-8f86-4e79-adf1-8ba429f73c89"), "Permission", "Book.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d82ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("c8ad649a-2019-43e7-8a33-786d2963c4cc"), "Permission", "Book.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("6775734b-677b-4430-bbb9-1495d8991419"), "Permission", "Book.Update", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("43f588d9-5a6c-48c3-900b-5a9d6a643637"), "Permission", "Book.Create", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("b1e0cbc5-f04f-4a43-bab6-2448d144dbfc"), "Permission", "Book.Delete", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("18a4fabe-2e70-4f51-a38a-30fa12332216"), "Permission", "Bookstore.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("f753b22d-bcaf-4545-b9b4-37d1bdedde67"), "Permission", "Bookstore.Update", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("399fc9fb-bd70-4bf9-8dd4-a45e25630d8a"), "Permission", "Bookstore.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d82ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("1633728a-d42a-4ee3-98af-74db1970224e"), "Permission", "Bookstore.Create", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("5bfb886c-da71-40ae-893e-aba0f5fca319"), "Permission", "Role.Create", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("ee04a7d9-ad9c-4f4a-9820-d55fa540c632"), "Permission", "Role.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("d7cca48d-8c8c-4e58-9354-526b1bbcd801"), "Permission", "Book.Update", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("75a033b5-074d-4a50-b2dd-d934dfc35018"), "Permission", "Book.Create", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("71e14223-8ceb-4a37-8696-3dedc476fb8e"), "Permission", "Book.Delete", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("2cc7acb8-0b62-4cd4-9139-b0ad220678e4"), "Permission", "Book.Full", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("28fc2b87-eee6-4cbd-847b-91c0da1751a2"), "Permission", "Bookstore.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("3e7a5a9f-efd5-41c9-97bc-db0e438da4d5"), "Permission", "Bookstore.Update", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("1d2674ad-f820-4222-897b-4e48bba43d3d"), "Permission", "Role.Update", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("62589436-f138-4375-a4b0-fb19048efc6c"), "Permission", "Bookstore.Create", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("9a72cefd-056b-4507-9130-06e680c6bbc7"), "Permission", "Bookstore.Full", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("83cf48ff-6a60-4565-bf5c-6470870f7efd"), "Permission", "User.Read", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("ec34092b-38fa-4df7-a2e4-d313d79f0a3d"), "Permission", "User.Update", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("a6bf0136-feb7-480b-b518-64f65ed70fa5"), "Permission", "User.Create", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("0fd82be4-fad9-44a6-bf1e-5369a5f7ce0d"), "Permission", "User.Delete", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("b236e988-e237-4367-823b-55d53e9d591e"), "Permission", "User.Full", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("c2810def-d14f-451c-8dde-cc9a0e2ad04c"), "Permission", "Bookstore.Delete", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("edf7e607-426d-4cc5-b6ac-321f11518520"), "Permission", "Bookstore.Delete", new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("0fd82be4-fad9-44a6-bf1e-5369a5f7ce0d"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("1633728a-d42a-4ee3-98af-74db1970224e"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("171c4701-98e8-4b74-af5e-eda937dbdb24"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("18a4fabe-2e70-4f51-a38a-30fa12332216"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("1d2674ad-f820-4222-897b-4e48bba43d3d"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("28fc2b87-eee6-4cbd-847b-91c0da1751a2"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("2cc7acb8-0b62-4cd4-9139-b0ad220678e4"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("31fc1523-8653-4bbc-acab-dee44688439a"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("399fc9fb-bd70-4bf9-8dd4-a45e25630d8a"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("3e7a5a9f-efd5-41c9-97bc-db0e438da4d5"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("3f3b40be-8f86-4e79-adf1-8ba429f73c89"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("43f588d9-5a6c-48c3-900b-5a9d6a643637"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("4cc53412-561a-4d29-ac1b-22dd3cf18c32"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("5bfb886c-da71-40ae-893e-aba0f5fca319"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("62589436-f138-4375-a4b0-fb19048efc6c"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("6775734b-677b-4430-bbb9-1495d8991419"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("71e14223-8ceb-4a37-8696-3dedc476fb8e"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("75a033b5-074d-4a50-b2dd-d934dfc35018"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("83cf48ff-6a60-4565-bf5c-6470870f7efd"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("9a72cefd-056b-4507-9130-06e680c6bbc7"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("a6bf0136-feb7-480b-b518-64f65ed70fa5"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b1e0cbc5-f04f-4a43-bab6-2448d144dbfc"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b236e988-e237-4367-823b-55d53e9d591e"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b5695386-1b56-4e77-87ab-6b58c16f5594"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c0aada02-252d-4231-ae31-4445736c39ea"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c2810def-d14f-451c-8dde-cc9a0e2ad04c"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c8ad649a-2019-43e7-8a33-786d2963c4cc"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("d7cca48d-8c8c-4e58-9354-526b1bbcd801"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ec34092b-38fa-4df7-a2e4-d313d79f0a3d"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("edf7e607-426d-4cc5-b6ac-321f11518520"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ee04a7d9-ad9c-4f4a-9820-d55fa540c632"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ee777566-7a40-4aa1-9923-2758a38e777a"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("f6534a40-cb9a-43a9-b00f-01471fff8c73"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("f753b22d-bcaf-4545-b9b4-37d1bdedde67"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("fd843c83-ddeb-4ca9-957f-333ee739402c"));
        }
    }
}
