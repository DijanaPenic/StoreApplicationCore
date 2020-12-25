using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class CreateEmailTemplateTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "email_template",
                columns: table => new
                {
                    id = table.Column<Guid>(nullable: false),
                    date_created_utc = table.Column<DateTime>(nullable: false),
                    date_updated_utc = table.Column<DateTime>(nullable: false),
                    client_id = table.Column<Guid>(nullable: false),
                    type = table.Column<int>(nullable: false),
                    path = table.Column<string>(maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_email_templates", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "email_template");
        }
    }
}
