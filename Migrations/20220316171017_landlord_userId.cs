using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class landlord_userId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Landlords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Landlords",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Landlords_UserId1",
                table: "Landlords",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Landlords_AspNetUsers_UserId1",
                table: "Landlords",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Landlords_AspNetUsers_UserId1",
                table: "Landlords");

            migrationBuilder.DropIndex(
                name: "IX_Landlords_UserId1",
                table: "Landlords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Landlords");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Landlords");
        }
    }
}
