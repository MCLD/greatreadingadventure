using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addtriggerattachments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AttachmentId",
                table: "UserLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AwardAttachmentId",
                table: "Triggers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCertificate = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Triggers_AwardAttachmentId",
                table: "Triggers",
                column: "AwardAttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Triggers_Attachments_AwardAttachmentId",
                table: "Triggers",
                column: "AwardAttachmentId",
                principalTable: "Attachments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Triggers_Attachments_AwardAttachmentId",
                table: "Triggers");

            migrationBuilder.DropTable(
                name: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Triggers_AwardAttachmentId",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "AttachmentId",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "AwardAttachmentId",
                table: "Triggers");
        }
    }
}
