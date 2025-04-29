using GRA.Domain.Model;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addsegmenttype : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SegmentType",
                table: "Segments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql($"UPDATE [Segments] SET [SegmentType] = {(int)SegmentType.VendorCode} WHERE [Name] IN ('{SegmentNames.VendorCodeDonation}', '{SegmentNames.VendorCodeEmailAward}')");
            migrationBuilder.Sql($"UPDATE [Segments] SET [SegmentType] = {(int)SegmentType.LandingPage} WHERE [Name] LIKE '%Landing % Message' OR [Name] LIKE '%Exit % Message'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SegmentType",
                table: "Segments");
        }
    }
}
