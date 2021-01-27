using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addbadgealttext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltText",
                table: "Badges",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltText",
                table: "Badges");
        }
    }
}
