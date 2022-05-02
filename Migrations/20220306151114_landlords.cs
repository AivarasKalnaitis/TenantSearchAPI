using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class landlords : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Apartments",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Area",
                table: "Apartments",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "LandlordId",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "Landlords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Landlords", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_LandlordId",
                table: "Apartments",
                column: "LandlordId");

            migrationBuilder.AddForeignKey(
                name: "FK_Apartments_Landlords_LandlordId",
                table: "Apartments",
                column: "LandlordId",
                principalTable: "Landlords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apartments_Landlords_LandlordId",
                table: "Apartments");

            migrationBuilder.DropTable(
                name: "Landlords");

            migrationBuilder.DropIndex(
                name: "IX_Apartments_LandlordId",
                table: "Apartments");

            migrationBuilder.DropColumn(
                name: "LandlordId",
                table: "Apartments");

            migrationBuilder.AlterColumn<string>(
                name: "Price",
                table: "Apartments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<decimal>(
                name: "Area",
                table: "Apartments",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
