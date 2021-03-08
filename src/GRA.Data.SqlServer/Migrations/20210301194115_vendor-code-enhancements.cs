using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class vendorcodeenhancements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OptionSubject",
                table: "VendorCodeTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AwardPrizeOnPackingSlip",
                table: "VendorCodeTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArrivalDate",
                table: "VendorCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PackingSlip",
                table: "VendorCodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "VendorCodes",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorCodePackingSlips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    PackingSlip = table.Column<long>(type: "bigint", nullable: false),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCodePackingSlips", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorCodePackingSlips");

            migrationBuilder.DropColumn(
                name: "AwardPrizeOnPackingSlip",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "ArrivalDate",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "PackingSlip",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "VendorCodes");

            migrationBuilder.AlterColumn<string>(
                name: "OptionSubject",
                table: "VendorCodeTypes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
