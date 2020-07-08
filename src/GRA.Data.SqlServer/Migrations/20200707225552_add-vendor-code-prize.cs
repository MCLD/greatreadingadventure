using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addvendorcodeprize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AwardPrizeOnShipDate",
                table: "VendorCodeTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "VendorCodes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorCodeId",
                table: "PrizeWinners",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwardPrizeOnShipDate",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "VendorCodeId",
                table: "PrizeWinners");
        }
    }
}
