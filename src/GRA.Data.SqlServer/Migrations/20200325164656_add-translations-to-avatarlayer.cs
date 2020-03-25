using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addtranslationstoavatarlayer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemoveLabel",
                table: "AvatarLayers",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpanishName",
                table: "AvatarLayers",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SpanishRemoveLabel",
                table: "AvatarLayers",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemoveLabel",
                table: "AvatarLayers");

            migrationBuilder.DropColumn(
                name: "SpanishName",
                table: "AvatarLayers");

            migrationBuilder.DropColumn(
                name: "SpanishRemoveLabel",
                table: "AvatarLayers");
        }
    }
}
