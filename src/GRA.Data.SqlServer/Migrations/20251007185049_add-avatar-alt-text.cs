using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class addavataralttext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AvatarColorTexts",
                columns: table => new
                {
                    AvatarColorId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarColorTexts", x => new { x.AvatarColorId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_AvatarColorTexts_AvatarColors_AvatarColorId",
                        column: x => x.AvatarColorId,
                        principalTable: "AvatarColors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvatarColorTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AvatarItemTexts",
                columns: table => new
                {
                    AvatarItemId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(130)", maxLength: 130, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvatarItemTexts", x => new { x.AvatarItemId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_AvatarItemTexts_AvatarItems_AvatarItemId",
                        column: x => x.AvatarItemId,
                        principalTable: "AvatarItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvatarItemTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AvatarColorTexts_LanguageId",
                table: "AvatarColorTexts",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_AvatarItemTexts_LanguageId",
                table: "AvatarItemTexts",
                column: "LanguageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AvatarColorTexts");

            migrationBuilder.DropTable(
                name: "AvatarItemTexts");
        }
    }
}
