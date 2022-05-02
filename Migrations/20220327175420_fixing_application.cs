using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class fixing_application : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LandlordId",
                table: "TenantApplications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "TenantApplications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandlordId",
                table: "TenantApplications");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "TenantApplications");
        }
    }
}
