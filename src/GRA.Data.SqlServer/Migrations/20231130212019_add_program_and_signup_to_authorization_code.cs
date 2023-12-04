using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class add_program_and_signup_to_authorization_code : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "ProgramId",
                table: "AuthorizationCodes");

            migrationBuilder.DropColumn(
                name: "SinglePageSignUp",
                table: "AuthorizationCodes");
        }
    }
}
