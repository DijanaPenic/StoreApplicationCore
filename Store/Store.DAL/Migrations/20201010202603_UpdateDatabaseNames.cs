using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class UpdateDatabaseNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_Bookstore_BookstoreId",
                table: "Book");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                table: "RoleClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim");

            migrationBuilder.DropForeignKey(
                name: "FK_UserLogin_User_UserId",
                table: "UserLogin");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserToken_User_UserId",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bookstore",
                table: "Bookstore");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Book",
                table: "Book");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserToken",
                table: "UserToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogin",
                table: "UserLogin");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RoleClaim",
                table: "RoleClaim");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "user");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "role");

            migrationBuilder.RenameTable(
                name: "Bookstore",
                newName: "bookstore");

            migrationBuilder.RenameTable(
                name: "Book",
                newName: "book");

            migrationBuilder.RenameTable(
                name: "UserToken",
                newName: "user_token");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "user_role");

            migrationBuilder.RenameTable(
                name: "UserLogin",
                newName: "user_login");

            migrationBuilder.RenameTable(
                name: "UserClaim",
                newName: "user_claim");

            migrationBuilder.RenameTable(
                name: "RoleClaim",
                newName: "role_claim");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "user",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "user",
                newName: "user_name");

            migrationBuilder.RenameColumn(
                name: "TwoFactorEnabled",
                table: "user",
                newName: "two_factor_enabled");

            migrationBuilder.RenameColumn(
                name: "SecurityStamp",
                table: "user",
                newName: "security_stamp");

            migrationBuilder.RenameColumn(
                name: "PhoneNumberConfirmed",
                table: "user",
                newName: "phone_number_confirmed");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "user",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "user",
                newName: "password_hash");

            migrationBuilder.RenameColumn(
                name: "NormalizedUserName",
                table: "user",
                newName: "normalized_user_name");

            migrationBuilder.RenameColumn(
                name: "NormalizedEmail",
                table: "user",
                newName: "normalized_email");

            migrationBuilder.RenameColumn(
                name: "LockoutEndDateUtc",
                table: "user",
                newName: "lockout_end_date_utc");

            migrationBuilder.RenameColumn(
                name: "LockoutEnabled",
                table: "user",
                newName: "lockout_enabled");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "user",
                newName: "last_name");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "user",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsApproved",
                table: "user",
                newName: "is_approved");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "user",
                newName: "first_name");

            migrationBuilder.RenameColumn(
                name: "EmailConfirmed",
                table: "user",
                newName: "email_confirmed");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "user",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "user",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "user",
                newName: "concurrency_stamp");

            migrationBuilder.RenameColumn(
                name: "AccessFailedCount",
                table: "user",
                newName: "access_failed_count");

            migrationBuilder.RenameColumn(
                name: "Stackable",
                table: "role",
                newName: "stackable");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "role",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "role",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "NormalizedName",
                table: "role",
                newName: "normalized_name");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "role",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "role",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                table: "role",
                newName: "concurrency_stamp");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "bookstore",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "bookstore",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "bookstore",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "bookstore",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "bookstore",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "book",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "book",
                newName: "author");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "book",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "book",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "book",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "BookstoreId",
                table: "book",
                newName: "bookstore_id");

            migrationBuilder.RenameIndex(
                name: "IX_Book_BookstoreId",
                table: "book",
                newName: "ix_books_bookstore_id");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "user_token",
                newName: "value");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "user_token",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "user_token",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "user_token",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "user_token",
                newName: "login_provider");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_token",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "user_role",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "user_role",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_role",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserRole_RoleId",
                table: "user_role",
                newName: "ix_user_roles_role_id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_login",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "ProviderDisplayName",
                table: "user_login",
                newName: "provider_display_name");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "user_login",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "user_login",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "ProviderKey",
                table: "user_login",
                newName: "provider_key");

            migrationBuilder.RenameColumn(
                name: "LoginProvider",
                table: "user_login",
                newName: "login_provider");

            migrationBuilder.RenameIndex(
                name: "IX_UserLogin_UserId",
                table: "user_login",
                newName: "ix_user_logins_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "user_claim",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "user_claim",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "user_claim",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "user_claim",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "user_claim",
                newName: "claim_value");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "user_claim",
                newName: "claim_type");

            migrationBuilder.RenameIndex(
                name: "IX_UserClaim_UserId",
                table: "user_claim",
                newName: "ix_user_claim_user_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "role_claim",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "RoleId",
                table: "role_claim",
                newName: "role_id");

            migrationBuilder.RenameColumn(
                name: "DateUpdatedUtc",
                table: "role_claim",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "DateCreatedUtc",
                table: "role_claim",
                newName: "date_created_utc");

            migrationBuilder.RenameColumn(
                name: "ClaimValue",
                table: "role_claim",
                newName: "claim_value");

            migrationBuilder.RenameColumn(
                name: "ClaimType",
                table: "role_claim",
                newName: "claim_type");

            migrationBuilder.RenameIndex(
                name: "IX_RoleClaim_RoleId",
                table: "role_claim",
                newName: "ix_role_claim_role_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_users",
                table: "user",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_roles",
                table: "role",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_bookstores",
                table: "bookstore",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_books",
                table: "book",
                column: "id");

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

            migrationBuilder.AddPrimaryKey(
                name: "pk_user_claims",
                table: "user_claim",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_role_claims",
                table: "role_claim",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_books_bookstores_bookstore_id",
                table: "book",
                column: "bookstore_id",
                principalTable: "bookstore",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_role_claim_role_role_entity_id",
                table: "role_claim",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_claim_user_user_entity_id",
                table: "user_claim",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_logins_user_user_entity_id",
                table: "user_login",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_role_role_entity_id",
                table: "user_role",
                column: "role_id",
                principalTable: "role",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_roles_user_user_entity_id",
                table: "user_role",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_user_tokens_user_user_entity_id",
                table: "user_token",
                column: "user_id",
                principalTable: "user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_books_bookstores_bookstore_id",
                table: "book");

            migrationBuilder.DropForeignKey(
                name: "fk_role_claim_role_role_entity_id",
                table: "role_claim");

            migrationBuilder.DropForeignKey(
                name: "fk_user_claim_user_user_entity_id",
                table: "user_claim");

            migrationBuilder.DropForeignKey(
                name: "fk_user_logins_user_user_entity_id",
                table: "user_login");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_role_role_entity_id",
                table: "user_role");

            migrationBuilder.DropForeignKey(
                name: "fk_user_roles_user_user_entity_id",
                table: "user_role");

            migrationBuilder.DropForeignKey(
                name: "fk_user_tokens_user_user_entity_id",
                table: "user_token");

            migrationBuilder.DropPrimaryKey(
                name: "pk_users",
                table: "user");

            migrationBuilder.DropPrimaryKey(
                name: "pk_roles",
                table: "role");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bookstores",
                table: "bookstore");

            migrationBuilder.DropPrimaryKey(
                name: "pk_books",
                table: "book");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_tokens",
                table: "user_token");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_roles",
                table: "user_role");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_logins",
                table: "user_login");

            migrationBuilder.DropPrimaryKey(
                name: "pk_user_claims",
                table: "user_claim");

            migrationBuilder.DropPrimaryKey(
                name: "pk_role_claims",
                table: "role_claim");

            migrationBuilder.RenameTable(
                name: "user",
                newName: "User");

            migrationBuilder.RenameTable(
                name: "role",
                newName: "Role");

            migrationBuilder.RenameTable(
                name: "bookstore",
                newName: "Bookstore");

            migrationBuilder.RenameTable(
                name: "book",
                newName: "Book");

            migrationBuilder.RenameTable(
                name: "user_token",
                newName: "UserToken");

            migrationBuilder.RenameTable(
                name: "user_role",
                newName: "UserRole");

            migrationBuilder.RenameTable(
                name: "user_login",
                newName: "UserLogin");

            migrationBuilder.RenameTable(
                name: "user_claim",
                newName: "UserClaim");

            migrationBuilder.RenameTable(
                name: "role_claim",
                newName: "RoleClaim");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "User",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "User",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_name",
                table: "User",
                newName: "UserName");

            migrationBuilder.RenameColumn(
                name: "two_factor_enabled",
                table: "User",
                newName: "TwoFactorEnabled");

            migrationBuilder.RenameColumn(
                name: "security_stamp",
                table: "User",
                newName: "SecurityStamp");

            migrationBuilder.RenameColumn(
                name: "phone_number_confirmed",
                table: "User",
                newName: "PhoneNumberConfirmed");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "User",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "User",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "normalized_user_name",
                table: "User",
                newName: "NormalizedUserName");

            migrationBuilder.RenameColumn(
                name: "normalized_email",
                table: "User",
                newName: "NormalizedEmail");

            migrationBuilder.RenameColumn(
                name: "lockout_end_date_utc",
                table: "User",
                newName: "LockoutEndDateUtc");

            migrationBuilder.RenameColumn(
                name: "lockout_enabled",
                table: "User",
                newName: "LockoutEnabled");

            migrationBuilder.RenameColumn(
                name: "last_name",
                table: "User",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "User",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_approved",
                table: "User",
                newName: "IsApproved");

            migrationBuilder.RenameColumn(
                name: "first_name",
                table: "User",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "email_confirmed",
                table: "User",
                newName: "EmailConfirmed");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "User",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "User",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                table: "User",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "access_failed_count",
                table: "User",
                newName: "AccessFailedCount");

            migrationBuilder.RenameColumn(
                name: "stackable",
                table: "Role",
                newName: "Stackable");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Role",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Role",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "normalized_name",
                table: "Role",
                newName: "NormalizedName");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "Role",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "Role",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                table: "Role",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Bookstore",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "Bookstore",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Bookstore",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "Bookstore",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "Bookstore",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Book",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "author",
                table: "Book",
                newName: "Author");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Book",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "Book",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "Book",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "bookstore_id",
                table: "Book",
                newName: "BookstoreId");

            migrationBuilder.RenameIndex(
                name: "ix_books_bookstore_id",
                table: "Book",
                newName: "IX_Book_BookstoreId");

            migrationBuilder.RenameColumn(
                name: "value",
                table: "UserToken",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "UserToken",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "UserToken",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "UserToken",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "login_provider",
                table: "UserToken",
                newName: "LoginProvider");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserToken",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "UserRole",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "UserRole",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserRole",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "ix_user_roles_role_id",
                table: "UserRole",
                newName: "IX_UserRole_RoleId");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserLogin",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "provider_display_name",
                table: "UserLogin",
                newName: "ProviderDisplayName");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "UserLogin",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "UserLogin",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "provider_key",
                table: "UserLogin",
                newName: "ProviderKey");

            migrationBuilder.RenameColumn(
                name: "login_provider",
                table: "UserLogin",
                newName: "LoginProvider");

            migrationBuilder.RenameIndex(
                name: "ix_user_logins_user_id",
                table: "UserLogin",
                newName: "IX_UserLogin_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserClaim",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserClaim",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "UserClaim",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "UserClaim",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "claim_value",
                table: "UserClaim",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "claim_type",
                table: "UserClaim",
                newName: "ClaimType");

            migrationBuilder.RenameIndex(
                name: "ix_user_claim_user_id",
                table: "UserClaim",
                newName: "IX_UserClaim_UserId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RoleClaim",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "role_id",
                table: "RoleClaim",
                newName: "RoleId");

            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                table: "RoleClaim",
                newName: "DateUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                table: "RoleClaim",
                newName: "DateCreatedUtc");

            migrationBuilder.RenameColumn(
                name: "claim_value",
                table: "RoleClaim",
                newName: "ClaimValue");

            migrationBuilder.RenameColumn(
                name: "claim_type",
                table: "RoleClaim",
                newName: "ClaimType");

            migrationBuilder.RenameIndex(
                name: "ix_role_claim_role_id",
                table: "RoleClaim",
                newName: "IX_RoleClaim_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bookstore",
                table: "Bookstore",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Book",
                table: "Book",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserToken",
                table: "UserToken",
                columns: new[] { "UserId", "LoginProvider", "Name" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                columns: new[] { "UserId", "RoleId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogin",
                table: "UserLogin",
                columns: new[] { "LoginProvider", "ProviderKey" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserClaim",
                table: "UserClaim",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RoleClaim",
                table: "RoleClaim",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_Bookstore_BookstoreId",
                table: "Book",
                column: "BookstoreId",
                principalTable: "Bookstore",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleClaim_Role_RoleId",
                table: "RoleClaim",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaim_User_UserId",
                table: "UserClaim",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserLogin_User_UserId",
                table: "UserLogin",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_Role_RoleId",
                table: "UserRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRole_User_UserId",
                table: "UserRole",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserToken_User_UserId",
                table: "UserToken",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
