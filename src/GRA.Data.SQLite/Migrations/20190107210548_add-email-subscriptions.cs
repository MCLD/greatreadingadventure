using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SQLite.Migrations
{
    public partial class addemailsubscriptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreregistrationReminderRequested",
                table: "Users",
                newName: "IsEmailSubscribed");

            migrationBuilder.CreateTable(
                name: "EmailSubscriptionAuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    Subscribed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailSubscriptionAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailSubscriptionAuditLogs_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailSubscriptionAuditLogs_CreatedBy",
                table: "EmailSubscriptionAuditLogs",
                column: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailSubscriptionAuditLogs");

            migrationBuilder.RenameColumn(
                name: "IsEmailSubscribed",
                table: "Users",
                newName: "PreregistrationReminderRequested");
        }
    }
}
