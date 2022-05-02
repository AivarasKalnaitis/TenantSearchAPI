using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class history : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApartmentId",
                table: "Tenants",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Apartments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());

            migrationBuilder.CreateTable(
                name: "TenantApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValue: Guid.NewGuid()),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValue: null),
                    SelectedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValue: null),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValue: null)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantApplications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantApplications");

            migrationBuilder.DropColumn(
                name: "ApartmentId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Apartments");
        }
    }
}
