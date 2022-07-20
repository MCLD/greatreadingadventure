using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class featuredchallengegroups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FeaturedChallengeGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteId = table.Column<int>(type: "int", nullable: false),
                    ChallengeGroupId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedChallengeGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeaturedChallengeGroups_ChallengeGroups_ChallengeGroupId",
                        column: x => x.ChallengeGroupId,
                        principalTable: "ChallengeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FeaturedChallengeGroupTexts",
                columns: table => new
                {
                    FeaturedChallengeGroupId = table.Column<int>(type: "int", nullable: false),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    Filename = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedChallengeGroupTexts", x => new { x.FeaturedChallengeGroupId, x.LanguageId });
                    table.ForeignKey(
                        name: "FK_FeaturedChallengeGroupTexts_FeaturedChallengeGroups_FeaturedChallengeGroupId",
                        column: x => x.FeaturedChallengeGroupId,
                        principalTable: "FeaturedChallengeGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeaturedChallengeGroupTexts_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedChallengeGroups_ChallengeGroupId",
                table: "FeaturedChallengeGroups",
                column: "ChallengeGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_FeaturedChallengeGroupTexts_LanguageId",
                table: "FeaturedChallengeGroupTexts",
                column: "LanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FeaturedChallengeGroupTexts");

            migrationBuilder.DropTable(
                name: "FeaturedChallengeGroups");
        }
    }
}
