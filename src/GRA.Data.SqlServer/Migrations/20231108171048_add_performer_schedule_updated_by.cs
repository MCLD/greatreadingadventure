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
            migrationBuilder.DropForeignKey(
                name: "FK_PsBranchSelections_Users_UserId",
                table: "PsBranchSelections");

            migrationBuilder.DropIndex(
                name: "IX_PsBranchSelections_UserId",
                table: "PsBranchSelections");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PsBranchSelections");

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUserId",
                table: "PsBranchSelections",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_CreatedBy",
                table: "PsBranchSelections",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_UpdatedByUserId",
                table: "PsBranchSelections",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PsBranchSelections_Users_CreatedBy",
                table: "PsBranchSelections",
                column: "CreatedBy",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PsBranchSelections_Users_UpdatedByUserId",
                table: "PsBranchSelections",
                column: "UpdatedByUserId",
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

            migrationBuilder.DropForeignKey(
                name: "FK_PsBranchSelections_Users_UpdatedByUserId",
                table: "PsBranchSelections");

            migrationBuilder.DropIndex(
                name: "IX_PsBranchSelections_CreatedBy",
                table: "PsBranchSelections");

            migrationBuilder.DropIndex(
                name: "IX_PsBranchSelections_UpdatedByUserId",
                table: "PsBranchSelections");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "PsBranchSelections");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PsBranchSelections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PsBranchSelections_UserId",
                table: "PsBranchSelections",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PsBranchSelections_Users_UserId",
                table: "PsBranchSelections",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
