using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GRA.Data.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class packing_slip_alphanumeric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PackingSlip",
                table: "VendorCodes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "PackingSlip",
                table: "VendorCodePackingSlips",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.Sql("UPDATE [VendorCodes] SET [PackingSlip] = NULL WHERE [PackingSlip] = '0'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PackingSlip",
                table: "VendorCodes",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "PackingSlip",
                table: "VendorCodePackingSlips",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
