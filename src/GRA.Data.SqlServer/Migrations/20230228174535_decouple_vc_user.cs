using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class decouple_vc_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssociatedUserId",
                table: "VendorCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAssigned",
                table: "VendorCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForReassignment",
                table: "VendorCodes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReassignedAt",
                table: "VendorCodes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReassignedByUserId",
                table: "VendorCodes",
                type: "int",
                nullable: true);

            migrationBuilder.Sql("UPDATE [VendorCodes] SET [IsAssigned] = CASE WHEN [UserId] IS NULL THEN 0 ELSE 1 END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssociatedUserId",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "IsAssigned",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "ReasonForReassignment",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "ReassignedAt",
                table: "VendorCodes");

            migrationBuilder.DropColumn(
                name: "ReassignedByUserId",
                table: "VendorCodes");
        }
    }
}
