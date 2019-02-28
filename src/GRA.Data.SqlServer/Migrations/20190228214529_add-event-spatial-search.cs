using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addeventspatialsearch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Locations",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Geolocation",
                table: "Locations",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Branches",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Geolocation",
                table: "Branches",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SpatialDistanceHeaders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedBy = table.Column<int>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    SiteId = table.Column<int>(nullable: false),
                    Geolocation = table.Column<string>(maxLength: 50, nullable: true),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpatialDistanceHeaders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpatialDistanceDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SpatialDistanceHeaderId = table.Column<int>(nullable: false),
                    BranchId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    Distance = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpatialDistanceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpatialDistanceDetails_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpatialDistanceDetails_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpatialDistanceDetails_SpatialDistanceHeaders_SpatialDistanceHeaderId",
                        column: x => x.SpatialDistanceHeaderId,
                        principalTable: "SpatialDistanceHeaders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpatialDistanceDetails_BranchId",
                table: "SpatialDistanceDetails",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_SpatialDistanceDetails_LocationId",
                table: "SpatialDistanceDetails",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_SpatialDistanceDetails_SpatialDistanceHeaderId",
                table: "SpatialDistanceDetails",
                column: "SpatialDistanceHeaderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpatialDistanceDetails");

            migrationBuilder.DropTable(
                name: "SpatialDistanceHeaders");

            migrationBuilder.DropColumn(
                name: "Geolocation",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "Geolocation",
                table: "Branches");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Locations",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Branches",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 255);
        }
    }
}
