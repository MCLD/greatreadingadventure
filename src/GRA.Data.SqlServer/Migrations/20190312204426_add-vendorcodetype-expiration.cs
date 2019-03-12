using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addvendorcodetypeexpiration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LimitToProgramId",
                table: "Challenges",
                newName: "AssociatedProgramId");

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpirationDate",
                table: "VendorCodeTypes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "VendorCodeTypes");

            migrationBuilder.RenameColumn(
                name: "AssociatedProgramId",
                table: "Challenges",
                newName: "LimitToProgramId");
        }
    }
}
