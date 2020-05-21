using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addvendorcodeemailaward : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
