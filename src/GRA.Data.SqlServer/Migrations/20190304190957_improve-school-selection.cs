using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class improveschoolselection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schools_SchoolTypes_SchoolTypeId",
                table: "Schools");

            migrationBuilder.DropTable(
                name: "SchoolTypes");

            migrationBuilder.DropIndex(
                name: "IX_Schools_SchoolTypeId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "SchoolTypeId",
                table: "Schools");

            migrationBuilder.DropColumn(
                name: "IsCharter",
                table: "SchoolDistricts");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "SchoolDistricts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolTypeId",
                table: "Schools",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCharter",
                table: "SchoolDistricts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "SchoolDistricts",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "SchoolTypes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 255, nullable: false),
                    SiteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schools_SchoolTypeId",
                table: "Schools",
                column: "SchoolTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schools_SchoolTypes_SchoolTypeId",
                table: "Schools",
                column: "SchoolTypeId",
                principalTable: "SchoolTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
