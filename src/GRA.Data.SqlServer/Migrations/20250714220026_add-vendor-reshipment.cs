using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addvendorreshipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReshipmentDetectedDate",
                table: "VendorCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReshipmentPriorPackingSlip",
                table: "VendorCodes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReshipmentDetectedDate",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "ReshipmentPriorPackingSlip",
                table: "VendorCodes");
        }
    }
}
