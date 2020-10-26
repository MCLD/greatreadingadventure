using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addstreamingevents : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStreaming",
                table: "Events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStreamingEmbed",
                table: "Events",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "StreamingAccessEnds",
                table: "Events",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StreamingLinkData",
                table: "Events",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStreaming",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "IsStreamingEmbed",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StreamingAccessEnds",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "StreamingLinkData",
                table: "Events");
        }
    }
}
