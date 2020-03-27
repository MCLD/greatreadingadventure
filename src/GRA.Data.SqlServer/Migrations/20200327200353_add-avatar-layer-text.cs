using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addavatarlayertext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "AvatarLayers");

            migrationBuilder.CreateTable(
                name: "AvatarLayerText",
                columns: table => new
                {
                    AvatarLayerId = table.Column<int>(nullable: false),
                    LanguageId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    RemoveLabel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarLayerText", x => new { x.AvatarLayerId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_AvatarLayerText_AvatarLayers_AvatarLayerId",
                        column: x => x.AvatarLayerId,
                        principalTable: "AvatarLayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvatarLayerText_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvatarLayerText_LanguageId",
                table: "AvatarLayerText",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvatarLayerText");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AvatarLayers",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
