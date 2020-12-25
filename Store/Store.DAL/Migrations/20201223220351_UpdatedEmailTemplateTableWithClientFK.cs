using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class UpdatedEmailTemplateTableWithClientFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_email_template_client_id",
                table: "email_template",
                column: "client_id");

            migrationBuilder.AddForeignKey(
                name: "fk_email_template_client_client_entity_id",
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
                name: "fk_email_template_client_client_entity_id",
                table: "email_template");

            migrationBuilder.DropIndex(
                name: "ix_email_template_client_id",
                table: "email_template");
        }
    }
}
