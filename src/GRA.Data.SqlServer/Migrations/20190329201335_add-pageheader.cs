using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addpageheader : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pages_SiteId_Stub",
                table: "Pages");

            migrationBuilder.AlterColumn<int>(
                name: "SiteId",
                table: "Pages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PageHeaderId",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MetaDescription",
                table: "Pages",
                maxLength: 150,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PageHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    PageName = table.Column<string>(nullable: false),
                    Stub = table.Column<string>(maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageHeaders", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageHeaders_SiteId_Stub",
                table: "PageHeaders",
                columns: new[] { "SiteId", "Stub" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageHeaders");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "PageHeaderId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "MetaDescription",
                table: "Pages");

            migrationBuilder.AlterColumn<int>(
                name: "SiteId",
                table: "Pages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_SiteId_Stub",
                table: "Pages",
                columns: new[] { "SiteId", "Stub" },
                unique: true,
                filter: "[Stub] IS NOT NULL");
        }
    }
}
