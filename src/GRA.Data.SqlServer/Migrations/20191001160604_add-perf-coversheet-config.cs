using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addperfcoversheetconfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FundingSource",
                table: "PsSettings",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LibraryBranch",
                table: "PsSettings",
                maxLength: 255,
                nullable: true);
            migrationBuilder.AddColumn<string>(
                name: "StaffContact",
                table: "PsSettings",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FundingSource",
                table: "PsSettings");

            migrationBuilder.DropColumn(
                name: "LibraryBranch",
                table: "PsSettings");

            migrationBuilder.DropColumn(
                name: "StaffContact",
                table: "PsSettings");
        }
    }
}
