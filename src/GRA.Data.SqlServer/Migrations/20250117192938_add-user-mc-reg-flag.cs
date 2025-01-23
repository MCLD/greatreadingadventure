using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addusermcregflag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsMcRegistered",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMcRegistered",
                table: "Users");
        }
    }
}
