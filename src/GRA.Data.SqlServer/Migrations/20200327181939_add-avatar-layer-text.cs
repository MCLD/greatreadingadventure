using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addavatarlayertext : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RemoveLabel",
                table: "AvatarLayers",
                maxLength: 255,
                nullable: true);

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
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvatarLayerText_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.DropColumn(
                name: "RemoveLabel",
                table: "AvatarLayers");
        }
    }
}
