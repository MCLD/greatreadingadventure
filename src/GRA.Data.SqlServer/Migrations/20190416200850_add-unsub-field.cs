using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addunsubfield : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnsubscribeToken",
                table: "Users",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "PsPrograms",
                type: "decimal(19,4)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AddColumn<bool>(
                name: "TokenUsed",
                table: "EmailSubscriptionAuditLogs",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnsubscribeToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TokenUsed",
                table: "EmailSubscriptionAuditLogs");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "PsPrograms",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,4)");
        }
    }
}
