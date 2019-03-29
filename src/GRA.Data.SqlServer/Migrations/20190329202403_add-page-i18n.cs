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
            migrationBuilder.Sql("UPDATE [Pages] SET [LanguageId] = 1");
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
