using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Digital_Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PendingChangesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Vendors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Vendors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ImageBookCoverPath",
                table: "Books",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "Vendors");

            migrationBuilder.DropColumn(
                name: "ImageBookCoverPath",
                table: "Books");
        }
    }
}
