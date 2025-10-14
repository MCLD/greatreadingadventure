using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addachieverreportcriterion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeAchieverStatus",
                table: "ReportCriteria",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeAchieverStatus",
                table: "ReportCriteria");
        }
    }
}
