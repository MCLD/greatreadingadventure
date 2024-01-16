using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addschedulingpointofcontact : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OnSiteContactEmail",
                table: "PsBranchSelections",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnSiteContactName",
                table: "PsBranchSelections",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OnSiteContactPhone",
                table: "PsBranchSelections",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnSiteContactEmail",
                table: "PsBranchSelections");

            migrationBuilder.DropColumn(
                name: "OnSiteContactName",
                table: "PsBranchSelections");

            migrationBuilder.DropColumn(
                name: "OnSiteContactPhone",
                table: "PsBranchSelections");
        }
    }
}
