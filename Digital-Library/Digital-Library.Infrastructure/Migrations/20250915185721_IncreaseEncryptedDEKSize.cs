using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Digital_Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IncreaseEncryptedDEKSize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "EncryptedDEK",
                table: "UserBookAccesses",
                type: "varbinary(256)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(32)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "EncryptedDEK",
                table: "UserBookAccesses",
                type: "varbinary(32)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(256)",
                oldNullable: true);
        }
    }
}
