using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SQLite.Migrations
{
    public partial class revisemcnewscategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayInSidebar",
                table: "NewsCateories");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DisplayInSidebar",
                table: "NewsCateories",
                nullable: false,
                defaultValue: false);
        }
    }
}
