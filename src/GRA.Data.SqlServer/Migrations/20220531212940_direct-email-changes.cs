using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class directemailchanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailUserLogs");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.AddColumn<int>(
                name: "ReadyForPickupEmailTemplateId",
                table: "VendorCodeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDonationLocked",
                table: "VendorCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "EmailReminders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DirectEmailHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DirectEmailTemplateId = table.Column<int>(type: "int", nullable: false),
                    FromEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FromName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsBulk = table.Column<bool>(type: "bit", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    OverrideToEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SentResponse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Successful = table.Column<bool>(type: "bit", nullable: false),
                    ToEmailAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ToName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectEmailHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectEmailHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailBases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailBases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DirectEmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailBaseId = table.Column<int>(type: "int", nullable: false),
                    EmailsSent = table.Column<int>(type: "int", nullable: false),
                    SentBulk = table.Column<bool>(type: "bit", nullable: false),
                    SystemEmailId = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectEmailTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DirectEmailTemplates_EmailBases_EmailBaseId",
                        column: x => x.EmailBaseId,
                        principalTable: "EmailBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailBaseTexts",
                columns: table => new
                {
                    EmailBaseId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    TemplateHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateMjml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TemplateText = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailBaseTexts", x => new { x.EmailBaseId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_EmailBaseTexts_EmailBases_EmailBaseId",
                        column: x => x.EmailBaseId,
                        principalTable: "EmailBases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailBaseTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DirectEmailTemplateTexts",
                columns: table => new
                {
                    DirectEmailTemplateId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    BodyCommonMark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Footer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Preview = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectEmailTemplateTexts", x => new { x.DirectEmailTemplateId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_DirectEmailTemplateTexts_DirectEmailTemplates_DirectEmailTemplateId",
                        column: x => x.DirectEmailTemplateId,
                        principalTable: "DirectEmailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DirectEmailTemplateTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DirectEmailHistories_UserId",
                table: "DirectEmailHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectEmailTemplates_EmailBaseId",
                table: "DirectEmailTemplates",
                column: "EmailBaseId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectEmailTemplateTexts_LanguageId",
                table: "DirectEmailTemplateTexts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailBaseTexts_LanguageId",
                table: "EmailBaseTexts",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectEmailHistories");

            migrationBuilder.DropTable(
                name: "DirectEmailTemplateTexts");

            migrationBuilder.DropTable(
                name: "EmailBaseTexts");

            migrationBuilder.DropTable(
                name: "DirectEmailTemplates");

            migrationBuilder.DropTable(
                name: "EmailBases");

            migrationBuilder.DropColumn(
                name: "ReadyForPickupEmailTemplateId",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "IsDonationLocked",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "EmailReminders");

            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BodyHtml = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EmailsSent = table.Column<int>(type: "int", nullable: false),
                    FromAddress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FromName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmailUserLogs",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EmailTemplateId = table.Column<int>(type: "int", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailUserLogs", x => new { x.UserId, x.EmailTemplateId });
                    table.ForeignKey(
                        name: "FK_EmailUserLogs_EmailTemplates_EmailTemplateId",
                        column: x => x.EmailTemplateId,
                        principalTable: "EmailTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmailUserLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailUserLogs_EmailTemplateId_EmailAddress",
                table: "EmailUserLogs",
                columns: new[] { "EmailTemplateId", "EmailAddress" },
                unique: true);
        }
    }
}
