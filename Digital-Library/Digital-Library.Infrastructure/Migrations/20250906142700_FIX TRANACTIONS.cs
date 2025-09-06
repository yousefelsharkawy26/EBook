using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Digital_Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FIXTRANACTIONS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Orders_OrderId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "Transactions",
                newName: "OrderHeaderId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_OrderId",
                table: "Transactions",
                newName: "IX_Transactions_OrderHeaderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_OrderHeader_OrderHeaderId",
                table: "Transactions",
                column: "OrderHeaderId",
                principalTable: "OrderHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_OrderHeader_OrderHeaderId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "OrderHeaderId",
                table: "Transactions",
                newName: "OrderId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_OrderHeaderId",
                table: "Transactions",
                newName: "IX_Transactions_OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Orders_OrderId",
                table: "Transactions",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
