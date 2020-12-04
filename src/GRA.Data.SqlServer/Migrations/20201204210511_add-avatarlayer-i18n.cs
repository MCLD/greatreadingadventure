using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addavatarlayeri18n : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvatarLayerTexts",
                columns: table => new
                {
                    AvatarLayerId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RemoveLabel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarLayerTexts", x => new { x.AvatarLayerId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_AvatarLayerTexts_AvatarLayers_AvatarLayerId",
                        column: x => x.AvatarLayerId,
                        principalTable: "AvatarLayers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvatarLayerTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvatarLayerTexts_LanguageId",
                table: "AvatarLayerTexts",
                column: "LanguageId");

            migrationBuilder.Sql("INSERT INTO [AvatarLayerTexts] SELECT [Id] [AvatarLayerId], 1 [LanguageId], [Name], 'Remove ' + [Name] [RemoveLabel] FROM [AvatarLayers] ORDER BY [Id];");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "AvatarLayers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "AvatarLayers",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE al SET al.[Name] = alt.[Name] FROM [AvatarLayers] al INNER JOIN [AvatarLayerTexts] alt ON alt.[AvatarLayerId] = al.[Id] WHERE alt.[LanguageId] = 1");

            migrationBuilder.DropTable(
                name: "AvatarLayerTexts");
        }
    }
}
