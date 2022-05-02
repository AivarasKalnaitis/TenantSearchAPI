using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class hobbies_tenants_mTm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hobbies_Tenants_TenantId",
                table: "Hobbies");

            migrationBuilder.DropIndex(
                name: "IX_Hobbies_TenantId",
                table: "Hobbies");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Hobbies");

            migrationBuilder.CreateTable(
                name: "HobbyTenant",
                columns: table => new
                {
                    HobbiesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HobbyTenant", x => new { x.HobbiesId, x.TenantsId });
                    table.ForeignKey(
                        name: "FK_HobbyTenant_Hobbies_HobbiesId",
                        column: x => x.HobbiesId,
                        principalTable: "Hobbies",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.Cascade,
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HobbyTenant_Tenants_TenantsId",
                        column: x => x.TenantsId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onUpdate: ReferentialAction.Cascade,
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HobbyTenant_TenantsId",
                table: "HobbyTenant",
                column: "TenantsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HobbyTenant");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Hobbies",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Hobbies_TenantId",
                table: "Hobbies",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hobbies_Tenants_TenantId",
                table: "Hobbies",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
