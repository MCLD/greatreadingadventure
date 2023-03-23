using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addvci18n : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorCodeTypeTexts");

            migrationBuilder.DropColumn(
                name: "DonationMail",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "DonationMessage",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "DonationSubject",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "EmailAwardMail",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "EmailAwardMessage",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "EmailAwardSubject",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "Mail",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "MailSubject",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "OptionMail",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "OptionSubject",
                table: "VendorCodeTypes");

            migrationBuilder.AddColumn<int>(
                name: "DonationMessageTemplateId",
                table: "VendorCodeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DonationSegmentId",
                table: "VendorCodeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailAwardMessageTemplateId",
                table: "VendorCodeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmailAwardSegmentId",
                table: "VendorCodeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MessageTemplateId",
                table: "VendorCodeTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OptionMessageTemplateId",
                table: "VendorCodeTypes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MessageTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageTemplateTexts",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    MessageTemplateId = table.Column<int>(type: "int", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageTemplateTexts", x => new { x.MessageTemplateId, x.LanguageId });
                });

            migrationBuilder.CreateTable(
                name: "Segments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Segments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SegmentTexts",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    SegmentId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SegmentTexts", x => new { x.LanguageId, x.SegmentId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageTemplates");

            migrationBuilder.DropTable(
                name: "MessageTemplateTexts");

            migrationBuilder.DropTable(
                name: "Segments");

            migrationBuilder.DropTable(
                name: "SegmentTexts");

            migrationBuilder.DropColumn(
                name: "DonationMessageTemplateId",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "DonationSegmentId",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "EmailAwardMessageTemplateId",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "EmailAwardSegmentId",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "MessageTemplateId",
                table: "VendorCodeTypes");

            migrationBuilder.DropColumn(
                name: "OptionMessageTemplateId",
                table: "VendorCodeTypes");

            migrationBuilder.AddColumn<string>(
                name: "DonationMail",
                table: "VendorCodeTypes",
                type: "nvarchar(1250)",
                maxLength: 1250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonationMessage",
                table: "VendorCodeTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DonationSubject",
                table: "VendorCodeTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAwardMail",
                table: "VendorCodeTypes",
                type: "nvarchar(1250)",
                maxLength: 1250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAwardMessage",
                table: "VendorCodeTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAwardSubject",
                table: "VendorCodeTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mail",
                table: "VendorCodeTypes",
                type: "nvarchar(1250)",
                maxLength: 1250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MailSubject",
                table: "VendorCodeTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OptionMail",
                table: "VendorCodeTypes",
                type: "nvarchar(1250)",
                maxLength: 1250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionSubject",
                table: "VendorCodeTypes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorCodeTypeTexts",
                columns: table => new
                {
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    VendorCodeTypeId = table.Column<int>(type: "int", nullable: false),
                    EmailAwardInstructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorCodeTypeTexts", x => new { x.LanguageId, x.VendorCodeTypeId });
                    table.ForeignKey(
                        name: "FK_VendorCodeTypeTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorCodeTypeTexts_VendorCodeTypes_VendorCodeTypeId",
                        column: x => x.VendorCodeTypeId,
                        principalTable: "VendorCodeTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendorCodeTypeTexts_VendorCodeTypeId",
                table: "VendorCodeTypeTexts",
                column: "VendorCodeTypeId");
        }
    }
}
