using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class v400beta2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ActivationDate",
                table: "Triggers",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TwitterCardImageUrl",
                table: "Sites",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SiteLogoUrl",
                table: "Sites",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FacebookImageUrl",
                table: "Sites",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Roles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "GroupInfoId",
                table: "ReportCriteria",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorCodeTypeId",
                table: "ReportCriteria",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivationDate",
                table: "Triggers");

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "GroupInfoId",
                table: "ReportCriteria");

            migrationBuilder.DropColumn(
                name: "VendorCodeTypeId",
                table: "ReportCriteria");

            migrationBuilder.AlterColumn<string>(
                name: "TwitterCardImageUrl",
                table: "Sites",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SiteLogoUrl",
                table: "Sites",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FacebookImageUrl",
                table: "Sites",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);
        }
    }
}
