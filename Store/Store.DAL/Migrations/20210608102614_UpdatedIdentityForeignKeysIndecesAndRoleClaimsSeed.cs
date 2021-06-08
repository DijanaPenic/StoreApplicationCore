using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class UpdatedIdentityForeignKeysIndecesAndRoleClaimsSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_role_claim_role_role_entity_id",
                schema: "identity",
                table: "role_claim");

            migrationBuilder.DropForeignKey(
                name: "fk_user_claim_user_user_entity_id",
                schema: "identity",
                table: "user_claim");

            migrationBuilder.DropForeignKey(
                name: "fk_user_logins_user_user_entity_id",
                schema: "identity",
                table: "user_login");

            migrationBuilder.DropForeignKey(
                name: "fk_user_refresh_token_clients_client_entity_id",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.DropForeignKey(
                name: "fk_user_refresh_token_user_user_entity_id",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_role_role_entity_id",
                schema: "identity",
                table: "user_role");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_user_user_entity_id",
                schema: "identity",
                table: "user_role");

            migrationBuilder.DropForeignKey(
                name: "fk_user_tokens_user_user_entity_id",
                schema: "identity",
                table: "user_token");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_role",
                schema: "identity",
                table: "user_role");

            migrationBuilder.DropIndex(
                name: "ix_user_roles_role_id",
                schema: "identity",
                table: "user_role");

            migrationBuilder.RenameIndex(
                name: "ix_user_refresh_token_user_id",
                schema: "identity",
                table: "user_refresh_token",
                newName: "ix_user_refresh_tokens_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_refresh_token_client_id",
                schema: "identity",
                table: "user_refresh_token",
                newName: "ix_user_refresh_tokens_client_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_claim_user_id",
                schema: "identity",
                table: "user_claim",
                newName: "ix_user_claims_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_role_claim_role_id",
                schema: "identity",
                table: "role_claim",
                newName: "ix_role_claims_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_role",
                schema: "identity",
                table: "user_role",
                columns: new[] { "role_id", "user_id" });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("0fd82be4-fad9-44a6-bf1e-5369a5f7ce0d"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("1633728a-d42a-4ee3-98af-74db1970224e"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("171c4701-98e8-4b74-af5e-eda937dbdb24"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("18a4fabe-2e70-4f51-a38a-30fa12332216"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("1d2674ad-f820-4222-897b-4e48bba43d3d"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("28fc2b87-eee6-4cbd-847b-91c0da1751a2"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("2cc7acb8-0b62-4cd4-9139-b0ad220678e4"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("31fc1523-8653-4bbc-acab-dee44688439a"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("399fc9fb-bd70-4bf9-8dd4-a45e25630d8a"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("3e7a5a9f-efd5-41c9-97bc-db0e438da4d5"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("3f3b40be-8f86-4e79-adf1-8ba429f73c89"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("43f588d9-5a6c-48c3-900b-5a9d6a643637"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("4cc53412-561a-4d29-ac1b-22dd3cf18c32"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("5bfb886c-da71-40ae-893e-aba0f5fca319"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("62589436-f138-4375-a4b0-fb19048efc6c"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("6775734b-677b-4430-bbb9-1495d8991419"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("71e14223-8ceb-4a37-8696-3dedc476fb8e"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("75a033b5-074d-4a50-b2dd-d934dfc35018"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("83cf48ff-6a60-4565-bf5c-6470870f7efd"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("9a72cefd-056b-4507-9130-06e680c6bbc7"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("a6bf0136-feb7-480b-b518-64f65ed70fa5"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b1e0cbc5-f04f-4a43-bab6-2448d144dbfc"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b236e988-e237-4367-823b-55d53e9d591e"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b5695386-1b56-4e77-87ab-6b58c16f5594"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c0aada02-252d-4231-ae31-4445736c39ea"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c2810def-d14f-451c-8dde-cc9a0e2ad04c"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c8ad649a-2019-43e7-8a33-786d2963c4cc"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("d7cca48d-8c8c-4e58-9354-526b1bbcd801"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ec34092b-38fa-4df7-a2e4-d313d79f0a3d"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("edf7e607-426d-4cc5-b6ac-321f11518520"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ee04a7d9-ad9c-4f4a-9820-d55fa540c632"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ee777566-7a40-4aa1-9923-2758a38e777a"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("f6534a40-cb9a-43a9-b00f-01471fff8c73"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("f753b22d-bcaf-4545-b9b4-37d1bdedde67"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("fd843c83-ddeb-4ca9-957f-333ee739402c"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2021, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "ix_user_role_user_id",
                schema: "identity",
                table: "user_role",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_role_claims_roles_role_id",
                schema: "identity",
                table: "role_claim",
                column: "role_id",
                principalSchema: "identity",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_claims_users_user_id",
                schema: "identity",
                table: "user_claim",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_logins_users_user_id",
                schema: "identity",
                table: "user_login",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_refresh_tokens_clients_client_id",
                schema: "identity",
                table: "user_refresh_token",
                column: "client_id",
                principalSchema: "identity",
                principalTable: "client",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_refresh_tokens_users_user_id",
                schema: "identity",
                table: "user_refresh_token",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_role_roles_role_id",
                schema: "identity",
                table: "user_role",
                column: "role_id",
                principalSchema: "identity",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_role_users_user_id",
                schema: "identity",
                table: "user_role",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_tokens_users_user_id",
                schema: "identity",
                table: "user_token",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_role_claims_roles_role_id",
                schema: "identity",
                table: "role_claim");

            migrationBuilder.DropForeignKey(
                name: "fk_user_claims_users_user_id",
                schema: "identity",
                table: "user_claim");

            migrationBuilder.DropForeignKey(
                name: "fk_user_logins_users_user_id",
                schema: "identity",
                table: "user_login");

            migrationBuilder.DropForeignKey(
                name: "fk_user_refresh_tokens_clients_client_id",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.DropForeignKey(
                name: "fk_user_refresh_tokens_users_user_id",
                schema: "identity",
                table: "user_refresh_token");

            migrationBuilder.DropForeignKey(
                name: "fk_user_role_roles_role_id",
                schema: "identity",
                table: "user_role");

            migrationBuilder.DropForeignKey(
                name: "fk_user_role_users_user_id",
                schema: "identity",
                table: "user_role");

            migrationBuilder.DropForeignKey(
                name: "fk_user_tokens_users_user_id",
                schema: "identity",
                table: "user_token");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_role",
                schema: "identity",
                table: "user_role");

            migrationBuilder.DropIndex(
                name: "ix_user_role_user_id",
                schema: "identity",
                table: "user_role");

            migrationBuilder.RenameIndex(
                name: "ix_user_refresh_tokens_user_id",
                schema: "identity",
                table: "user_refresh_token",
                newName: "ix_user_refresh_token_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_refresh_tokens_client_id",
                schema: "identity",
                table: "user_refresh_token",
                newName: "ix_user_refresh_token_client_id");

            migrationBuilder.RenameIndex(
                name: "ix_user_claims_user_id",
                schema: "identity",
                table: "user_claim",
                newName: "ix_user_claim_user_id");

            migrationBuilder.RenameIndex(
                name: "ix_role_claims_role_id",
                schema: "identity",
                table: "role_claim",
                newName: "ix_role_claim_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_role",
                schema: "identity",
                table: "user_role",
                columns: new[] { "user_id", "role_id" });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("0fd82be4-fad9-44a6-bf1e-5369a5f7ce0d"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("1633728a-d42a-4ee3-98af-74db1970224e"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("171c4701-98e8-4b74-af5e-eda937dbdb24"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("18a4fabe-2e70-4f51-a38a-30fa12332216"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("1d2674ad-f820-4222-897b-4e48bba43d3d"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("28fc2b87-eee6-4cbd-847b-91c0da1751a2"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("2cc7acb8-0b62-4cd4-9139-b0ad220678e4"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("31fc1523-8653-4bbc-acab-dee44688439a"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("399fc9fb-bd70-4bf9-8dd4-a45e25630d8a"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("3e7a5a9f-efd5-41c9-97bc-db0e438da4d5"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("3f3b40be-8f86-4e79-adf1-8ba429f73c89"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("43f588d9-5a6c-48c3-900b-5a9d6a643637"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("4cc53412-561a-4d29-ac1b-22dd3cf18c32"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("5bfb886c-da71-40ae-893e-aba0f5fca319"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("62589436-f138-4375-a4b0-fb19048efc6c"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("6775734b-677b-4430-bbb9-1495d8991419"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("71e14223-8ceb-4a37-8696-3dedc476fb8e"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("75a033b5-074d-4a50-b2dd-d934dfc35018"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("83cf48ff-6a60-4565-bf5c-6470870f7efd"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("9a72cefd-056b-4507-9130-06e680c6bbc7"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("a6bf0136-feb7-480b-b518-64f65ed70fa5"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b1e0cbc5-f04f-4a43-bab6-2448d144dbfc"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b236e988-e237-4367-823b-55d53e9d591e"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("b5695386-1b56-4e77-87ab-6b58c16f5594"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c0aada02-252d-4231-ae31-4445736c39ea"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c2810def-d14f-451c-8dde-cc9a0e2ad04c"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c8ad649a-2019-43e7-8a33-786d2963c4cc"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("d7cca48d-8c8c-4e58-9354-526b1bbcd801"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ec34092b-38fa-4df7-a2e4-d313d79f0a3d"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("edf7e607-426d-4cc5-b6ac-321f11518520"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ee04a7d9-ad9c-4f4a-9820-d55fa540c632"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("ee777566-7a40-4aa1-9923-2758a38e777a"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("f6534a40-cb9a-43a9-b00f-01471fff8c73"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("f753b22d-bcaf-4545-b9b4-37d1bdedde67"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("fd843c83-ddeb-4ca9-957f-333ee739402c"),
                columns: new[] { "date_created_utc", "date_updated_utc" },
                values: new object[] { new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified), new DateTime(2022, 3, 1, 20, 39, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                schema: "identity",
                table: "user_role",
                column: "role_id");

            migrationBuilder.AddForeignKey(
                name: "fk_role_claim_role_role_entity_id",
                schema: "identity",
                table: "role_claim",
                column: "role_id",
                principalSchema: "identity",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_claim_user_user_entity_id",
                schema: "identity",
                table: "user_claim",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_logins_user_user_entity_id",
                schema: "identity",
                table: "user_login",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_refresh_token_clients_client_entity_id",
                schema: "identity",
                table: "user_refresh_token",
                column: "client_id",
                principalSchema: "identity",
                principalTable: "client",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_refresh_token_user_user_entity_id",
                schema: "identity",
                table: "user_refresh_token",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_role_role_entity_id",
                schema: "identity",
                table: "user_role",
                column: "role_id",
                principalSchema: "identity",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_user_user_entity_id",
                schema: "identity",
                table: "user_role",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_tokens_user_user_entity_id",
                schema: "identity",
                table: "user_token",
                column: "user_id",
                principalSchema: "identity",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
