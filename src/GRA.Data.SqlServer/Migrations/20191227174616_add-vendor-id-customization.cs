using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addvendoridcustomization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BranchAvailabilitySuplimentalText",
                table: "PsSettings",
                newName: "VendorIdPrompt");

            migrationBuilder.AddColumn<string>(
                name: "BranchAvailabilitySupplementalText",
                table: "PsSettings",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchAvailabilitySupplementalText",
                table: "PsSettings");

            migrationBuilder.RenameColumn(
                name: "VendorIdPrompt",
                table: "PsSettings",
                newName: "BranchAvailabilitySuplimentalText");
        }
    }
}
