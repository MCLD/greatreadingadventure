using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class performerschedulingimprovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferencesFilename",
                table: "PsPerformers");

            migrationBuilder.AddColumn<string>(
                name: "CoverSheetBranch",
                table: "PsSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CoverSheetContact",
                table: "PsSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "PsPrograms",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ContactName",
                table: "PsPerformers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "References",
                table: "PsPerformers",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE [PsPrograms] SET [IsApproved] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverSheetBranch",
                table: "PsSettings");

            migrationBuilder.DropColumn(
                name: "CoverSheetContact",
                table: "PsSettings");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "PsPrograms");

            migrationBuilder.DropColumn(
                name: "ContactName",
                table: "PsPerformers");

            migrationBuilder.DropColumn(
                name: "References",
                table: "PsPerformers");

            migrationBuilder.AddColumn<string>(
                name: "ReferencesFilename",
                table: "PsPerformers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
