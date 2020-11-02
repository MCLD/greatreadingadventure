using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addvendorcoeemailaward : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DonationOptionSubject",
                table: "VendorCodeTypes",
                newName: "OptionSubject");

            migrationBuilder.RenameColumn(
                name: "DonationOptionMail",
                table: "VendorCodeTypes",
                newName: "OptionMail");

            migrationBuilder.AddColumn<string>(
                name: "EmailAwardMail",
                table: "VendorCodeTypes",
                maxLength: 1250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAwardMessage",
                table: "VendorCodeTypes",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAwardSubject",
                table: "VendorCodeTypes",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EmailAwardAddress",
                table: "VendorCodes",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailAwardReported",
                table: "VendorCodes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmailAwardSent",
                table: "VendorCodes",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailAward",
                table: "VendorCodes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorCodeTypeTexts",
                columns: table => new
                {
                    VendorCodeTypeId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    EmailAwardInstructions = table.Column<string>(maxLength: 1000, nullable: true)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendorCodeTypeTexts");

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
                name: "EmailAwardAddress",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "EmailAwardReported",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "EmailAwardSent",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "IsEmailAward",
                table: "VendorCodes");

            migrationBuilder.RenameColumn(
                name: "OptionSubject",
                table: "VendorCodeTypes",
                newName: "DonationOptionSubject");

            migrationBuilder.RenameColumn(
                name: "OptionMail",
                table: "VendorCodeTypes",
                newName: "DonationOptionMail");
        }
    }
}
