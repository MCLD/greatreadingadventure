using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class add_auth_code_parameters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "AuthorizationCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "AuthorizationCodes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "SinglePageSignUp",
                table: "AuthorizationCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "AuthorizationCodes");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "AuthorizationCodes");

            migrationBuilder.DropColumn(
                name: "SinglePageSignUp",
                table: "AuthorizationCodes");
        }
    }
}
