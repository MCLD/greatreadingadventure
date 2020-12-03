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

            migrationBuilder.AddColumn<int>(
                name: "AvatarBundleId",
                table: "Notifications",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasBeenViewed",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "AvatarBundleId",
                table: "Notifications");
        }
    }
}
