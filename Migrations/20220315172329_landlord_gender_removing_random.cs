using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class landlord_gender_removing_random : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Random",
                table: "Landlords");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "Landlords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Landlords");

            migrationBuilder.AddColumn<string>(
                name: "Random",
                table: "Landlords",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
