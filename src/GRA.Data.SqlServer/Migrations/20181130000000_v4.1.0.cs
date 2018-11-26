using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class v410 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_SiteId_IsDeleted_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SiteId_Id_IsDeleted_HouseholdHeadUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PrizeWinners_DrawingId_UserId_RedeemedAt",
                table: "PrizeWinners");

            migrationBuilder.DropIndex(
                name: "IX_Pages_SiteId_Stub",
                table: "Pages");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteId_IsDeleted_Username",
                table: "Users",
                columns: new[] { "SiteId", "IsDeleted", "Username" },
                unique: true,
                filter: "[Username] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteId_Id_IsDeleted_HouseholdHeadUserId",
                table: "Users",
                columns: new[] { "SiteId", "Id", "IsDeleted", "HouseholdHeadUserId" },
                unique: true,
                filter: "[HouseholdHeadUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PrizeWinners_DrawingId_UserId_RedeemedAt",
                table: "PrizeWinners",
                columns: new[] { "DrawingId", "UserId", "RedeemedAt" },
                unique: true,
                filter: "[DrawingId] IS NOT NULL AND [RedeemedAt] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_SiteId_Stub",
                table: "Pages",
                columns: new[] { "SiteId", "Stub" },
                unique: true,
                filter: "[Stub] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_SiteId_IsDeleted_Username",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SiteId_Id_IsDeleted_HouseholdHeadUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_PrizeWinners_DrawingId_UserId_RedeemedAt",
                table: "PrizeWinners");

            migrationBuilder.DropIndex(
                name: "IX_Pages_SiteId_Stub",
                table: "Pages");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteId_IsDeleted_Username",
                table: "Users",
                columns: new[] { "SiteId", "IsDeleted", "Username" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_SiteId_Id_IsDeleted_HouseholdHeadUserId",
                table: "Users",
                columns: new[] { "SiteId", "Id", "IsDeleted", "HouseholdHeadUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrizeWinners_DrawingId_UserId_RedeemedAt",
                table: "PrizeWinners",
                columns: new[] { "DrawingId", "UserId", "RedeemedAt" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_SiteId_Stub",
                table: "Pages",
                columns: new[] { "SiteId", "Stub" },
                unique: true);
        }
    }
}
