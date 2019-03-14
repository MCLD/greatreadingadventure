using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addprizeredeemerinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TriggerList",
                table: "ReportCriteria",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedeemedByBranch",
                table: "PrizeWinners",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RedeemedBySystem",
                table: "PrizeWinners",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TriggerList",
                table: "ReportCriteria");

            migrationBuilder.DropColumn(
                name: "RedeemedByBranch",
                table: "PrizeWinners");

            migrationBuilder.DropColumn(
                name: "RedeemedBySystem",
                table: "PrizeWinners");
        }
    }
}
