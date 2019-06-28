using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addviewavatarbundles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasBeenViewed",
                table: "UserLogs",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvatarBundle",
                table: "Notifications",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBeenViewed",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "IsAvatarBundle",
                table: "Notifications");
        }
    }
}
