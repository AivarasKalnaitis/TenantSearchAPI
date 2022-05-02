using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class landlords_apartments_with_landlordId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Landlords_LandlordId",
                table: "Apartments");

            migrationBuilder.AlterColumn<Guid>(
                name: "LandlordId",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid(),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Landlords_LandlordId",
                table: "Apartments",
                column: "LandlordId",
                principalTable: "Landlords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Landlords_LandlordId",
                table: "Apartments");

            migrationBuilder.AlterColumn<Guid>(
                name: "LandlordId",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Landlords_LandlordId",
                table: "Apartments",
                column: "LandlordId",
                principalTable: "Landlords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
