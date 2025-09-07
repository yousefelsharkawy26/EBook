using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Digital_Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalamountinorderheader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "OrderHeader",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "OrderHeader");
        }
    }
}
