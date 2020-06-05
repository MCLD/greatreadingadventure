using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addingpremadeavatartouseravatar : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PremadeAatarId",
                table: "UserAvatars",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PremadeAatarId",
                table: "UserAvatars");
        }
    }
}
