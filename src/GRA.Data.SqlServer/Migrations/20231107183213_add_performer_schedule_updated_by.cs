using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class add_performer_schedule_updated_by : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_CreatedBy",
                table: "PsBranchSelections",
                column: "CreatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_PsBranchSelections_Users_CreatedBy",
                table: "PsBranchSelections",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PsBranchSelections_Users_CreatedBy",
                table: "PsBranchSelections");

            migrationBuilder.DropIndex(
                name: "IX_PsBranchSelections_CreatedBy",
                table: "PsBranchSelections");
        }
    }
}
