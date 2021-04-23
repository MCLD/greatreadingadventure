using Microsoft.EntityFrameworkCore.Migrations;

namespace GRA.Data.SqlServer.Migrations
{
    public partial class addpagei18n : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("SET IDENTITY_INSERT [PageHeaders] ON;");
            migrationBuilder.Sql("INSERT INTO [PageHeaders] ([Id], [CreatedBy], [CreatedAt], [SiteId], [PageName], [Stub]) SELECT [Id], [CreatedBy], [Createdat], [SiteId], [Title], [Stub] FROM [Pages];");
            migrationBuilder.Sql("SET IDENTITY_INSERT [PageHeaders] OFF;");
            migrationBuilder.Sql("UPDATE [Pages] SET [PageHeaderId] = [Id]");
            migrationBuilder.Sql("IF(NOT EXISTS(SELECT 1 FROM [Languages])) BEGIN INSERT INTO [Languages] ([CreatedBy], [CreatedAt], [Description], [IsActive], [IsDefault], [Name]) VALUES (1, GETDATE(), 'English (United States)', 1, 1, 'en-US') END");
            migrationBuilder.Sql("UPDATE [Pages] SET [LanguageId] = l.[Id] FROM (SELECT TOP 1 [Id] FROM [Languages] WHERE [IsDefault] = 1) AS l");

            migrationBuilder.DropColumn(
                name: "SiteId",
                table: "Pages");

            migrationBuilder.DropColumn(
                name: "Stub",
                table: "Pages");

            migrationBuilder.AlterColumn<int>(
                name: "PageHeaderId",
                table: "Pages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "Pages",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pages_LanguageId",
                table: "Pages",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Pages_PageHeaderId",
                table: "Pages",
                column: "PageHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_Languages_LanguageId",
                table: "Pages",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Pages_PageHeaders_PageHeaderId",
                table: "Pages",
                column: "PageHeaderId",
                principalTable: "PageHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pages_Languages_LanguageId",
                table: "Pages");

            migrationBuilder.DropForeignKey(
                name: "FK_Pages_PageHeaders_PageHeaderId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_LanguageId",
                table: "Pages");

            migrationBuilder.DropIndex(
                name: "IX_Pages_PageHeaderId",
                table: "Pages");

            migrationBuilder.AlterColumn<int>(
                name: "PageHeaderId",
                table: "Pages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "LanguageId",
                table: "Pages",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "SiteId",
                table: "Pages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Stub",
                table: "Pages",
                maxLength: 255,
                nullable: true);
        }
    }
}
