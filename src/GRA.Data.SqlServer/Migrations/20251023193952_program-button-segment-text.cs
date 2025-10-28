using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class programbuttonsegmenttext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ButtonSegmentId",
                table: "Programs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonSegmentId",
                table: "Programs");
        }
    }
}
