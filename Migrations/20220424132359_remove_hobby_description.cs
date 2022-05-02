using Microsoft.EntityFrameworkCore.Migrations;

namespace TenantSearchAPI.Migrations
{
    public partial class remove_hobby_description : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Hobbies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Hobbies",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
