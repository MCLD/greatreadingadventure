using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addingpremadeavatartousers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PremadeAvatarId",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremadeAvatarId",
                table: "Users");
        }
    }
}
