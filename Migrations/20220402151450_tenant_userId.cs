using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class tenant_userId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Tenants",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.NewGuid());
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Tenants");
        }
    }
}
