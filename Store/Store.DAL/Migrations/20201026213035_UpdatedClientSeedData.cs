using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Store.DAL.Migrations
{
    public partial class UpdatedClientSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "id",
                keyValue: new Guid("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"),
                column: "secret",
                value: "PX23zsV/7nm6+ZI9LmrKXSBf2O47cYtiJGk2WJ/G/PdU2eD7Y929MZeItkGpBY2v6a2tXhGINq8bAQYz1bQC6A==");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "client",
                keyColumn: "id",
                keyValue: new Guid("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"),
                column: "secret",
                value: "28IOjCR2kNUeT3dIoFLJpn7oAtLrpzofaSzlXi+dxG9cyFul0tiBJc3BWPWTDVkzoAkSkXsFZ8u7ON05wQ276w==");
        }
    }
}
