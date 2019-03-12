using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SQLite.Migrations
{
    public partial class addbasei18n : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Culture",
                table: "Users",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSystemUser",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropColumn(
                name: "Culture",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSystemUser",
                table: "Users");
        }
    }
}
