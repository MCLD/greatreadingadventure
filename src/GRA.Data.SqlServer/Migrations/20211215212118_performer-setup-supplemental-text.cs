using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class performersetupsupplementaltext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SetupSupplementalText",
                table: "PsSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "PsPrograms",
                keyColumn: "Setup",
                keyValue: null,
                column: "Setup",
                value: "N/A");

            migrationBuilder.AlterColumn<string>(
                name: "Setup",
                table: "PsPrograms",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: null,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SetupSupplementalText",
                table: "PsSettings");

            migrationBuilder.AlterColumn<string>(
                name: "Setup",
                table: "PsPrograms",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);
        }
    }
}
