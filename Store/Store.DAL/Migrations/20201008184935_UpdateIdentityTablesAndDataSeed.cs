using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Store.DAL.Migrations
{
    public partial class UpdateIdentityTablesAndDataSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Claim");

            migrationBuilder.DropTable(
                name: "ExternalLogin");

            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DropTable(
                name: "Client");

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

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreatedUtc",
                table: "UserRole",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DateUpdatedUtc",
                table: "UserRole",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "User",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "User",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "User",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LockoutEndDateUtc",
                table: "User",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "User",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "EmailConfirmed",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "User",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedUserName",
                table: "User",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PhoneNumberConfirmed",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TwoFactorEnabled",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Role",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar",
                oldMaxLength: 256);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Role",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                table: "Role",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    DateCreatedUtc = table.Column<DateTime>(nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    DateCreatedUtc = table.Column<DateTime>(nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<Guid>(nullable: false),
                    DateCreatedUtc = table.Column<DateTime>(nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LoginProvider = table.Column<string>(maxLength: 128, nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: false),
                    Value = table.Column<string>(nullable: true),
                    DateCreatedUtc = table.Column<DateTime>(nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Book",
                columns: new[] { "Id", "Author", "BookstoreId", "DateCreatedUtc", "DateUpdatedUtc", "Name" },
                values: new object[,]
                {
                    { new Guid("304e51fa-e2dc-4114-bf3f-08d86b04996d"), "J. R. R. Tolkien", new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), "The Lord of the Rings" },
                    { new Guid("2d59e1d6-05e8-47a4-bf40-08d86b04996d"), "Paulo Coelho", new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), "The Alchemist" },
                    { new Guid("53b9c986-1857-4878-bf41-08d86b04996d"), "Antoine de Saint-Exupéry", new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), "The Little Prince" }
                });

            migrationBuilder.UpdateData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                columns: new[] { "DateCreatedUtc", "DateUpdatedUtc" },
                values: new object[] { new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("a4b57c3c-4c09-4b8c-b2f8-e88ce049f30c"),
                columns: new[] { "DateCreatedUtc", "DateUpdatedUtc" },
                values: new object[] { new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("fa588725-0c60-4554-8eb9-20520038ee87"),
                columns: new[] { "DateCreatedUtc", "DateUpdatedUtc" },
                values: new object[] { new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "ConcurrencyStamp", "DateCreatedUtc", "DateUpdatedUtc", "Name", "NormalizedName", "Stackable" },
                values: new object[,]
                {
                    { new Guid("d72ef5e5-f08a-4173-b83a-74618893891b"), null, new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), "Admin", "ADMIN", false },
                    { new Guid("d82ef5e5-f08a-4173-b83a-74618893891b"), null, new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), "Customer", "CUSTOMER", true },
                    { new Guid("d92ef5e5-f08a-4173-b83a-74618893891b"), null, new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), new DateTime(2020, 10, 8, 18, 44, 11, 0, DateTimeKind.Unspecified), "Store Manager", "STORE MANAGER", true }
                });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "User",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "User",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Role",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaim_RoleId",
                table: "RoleClaim",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserId",
                table: "UserClaim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                table: "UserLogin",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleClaim");

            migrationBuilder.DropTable(
                name: "UserClaim");

            migrationBuilder.DropTable(
                name: "UserLogin");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropIndex(
                name: "EmailIndex",
                table: "User");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "User");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "Role");

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: new Guid("2d59e1d6-05e8-47a4-bf40-08d86b04996d"));

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: new Guid("304e51fa-e2dc-4114-bf3f-08d86b04996d"));

            migrationBuilder.DeleteData(
                table: "Book",
                keyColumn: "Id",
                keyValue: new Guid("53b9c986-1857-4878-bf41-08d86b04996d"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("d72ef5e5-f08a-4173-b83a-74618893891b"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("d82ef5e5-f08a-4173-b83a-74618893891b"));

            migrationBuilder.DeleteData(
                table: "Role",
                keyColumn: "Id",
                keyValue: new Guid("d92ef5e5-f08a-4173-b83a-74618893891b"));

            migrationBuilder.DropColumn(
                name: "DateCreatedUtc",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "DateUpdatedUtc",
                table: "UserRole");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "User");

            migrationBuilder.DropColumn(
                name: "EmailConfirmed",
                table: "User");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "User");

            migrationBuilder.DropColumn(
                name: "NormalizedUserName",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "User");

            migrationBuilder.DropColumn(
                name: "PhoneNumberConfirmed",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TwoFactorEnabled",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Role");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                table: "Role");

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "User",
                type: "varchar",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 256);

            migrationBuilder.AlterColumn<string>(
                name: "SecurityStamp",
                table: "User",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordHash",
                table: "User",
                type: "varchar",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LockoutEndDateUtc",
                table: "User",
                type: "timestamp without time zone",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Role",
                type: "varchar",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 256);

            migrationBuilder.CreateTable(
                name: "Claim",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ClaimType = table.Column<string>(type: "varchar", maxLength: 150, nullable: true),
                    ClaimValue = table.Column<string>(type: "varchar", nullable: true),
                    DateCreatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claim", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claim_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    AllowedOrigin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ApplicationType = table.Column<int>(type: "integer", nullable: false),
                    DateCreatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RefreshTokenLifeTime = table.Column<int>(type: "integer", nullable: false),
                    Secret = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalLogin",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "varchar", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "varchar", maxLength: 128, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalLogin", x => new { x.LoginProvider, x.ProviderKey, x.UserId });
                    table.ForeignKey(
                        name: "FK_ExternalLogin_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ClientId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DateUpdatedUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ExpiresUtc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProtectedTicket = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshToken_Client_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RefreshToken_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.UpdateData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("61c048ca-028d-4466-b7fd-4a05f0dad647"),
                columns: new[] { "DateCreatedUtc", "DateUpdatedUtc" },
                values: new object[] { new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(5457), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(5719) });

            migrationBuilder.UpdateData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("a4b57c3c-4c09-4b8c-b2f8-e88ce049f30c"),
                columns: new[] { "DateCreatedUtc", "DateUpdatedUtc" },
                values: new object[] { new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(5988), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(6000) });

            migrationBuilder.UpdateData(
                table: "Bookstore",
                keyColumn: "Id",
                keyValue: new Guid("fa588725-0c60-4554-8eb9-20520038ee87"),
                columns: new[] { "DateCreatedUtc", "DateUpdatedUtc" },
                values: new object[] { new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(6013), new DateTime(2020, 9, 24, 13, 19, 51, 210, DateTimeKind.Utc).AddTicks(6014) });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "DateCreatedUtc", "DateUpdatedUtc", "Name", "Stackable" },
                values: new object[,]
                {
                    { new Guid("7ae9e63d-8bfa-45d4-2f01-08d8608c84c3"), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(3769), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4166), "Admin", false },
                    { new Guid("85b336c4-f9ce-4480-2f02-08d8608c84c3"), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4450), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4459), "Customer", true },
                    { new Guid("9cf98591-311c-4e5e-2f03-08d8608c84c3"), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4475), new DateTime(2020, 9, 24, 13, 19, 51, 200, DateTimeKind.Utc).AddTicks(4475), "Store Manager", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Claim_UserId",
                table: "Claim",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalLogin_UserId",
                table: "ExternalLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_ClientId",
                table: "RefreshToken",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_UserId",
                table: "RefreshToken",
                column: "UserId");
        }
    }
}
