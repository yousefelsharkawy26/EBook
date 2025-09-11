using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Digital_Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResolveCascadeCycles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartsDetails_Carts_CartId",
                table: "CartsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Books_BookId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderHeader_OrderHeaderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_userPdfBooks_AspNetUsers_UserId",
                table: "userPdfBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartsDetails_Carts_CartId",
                table: "CartsDetails",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Books_BookId",
                table: "OrderDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderHeader_OrderHeaderId",
                table: "OrderDetails",
                column: "OrderHeaderId",
                principalTable: "OrderHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userPdfBooks_AspNetUsers_UserId",
                table: "userPdfBooks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_CartsDetails_Carts_CartId",
                table: "CartsDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Books_BookId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderHeader_OrderHeaderId",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_userPdfBooks_AspNetUsers_UserId",
                table: "userPdfBooks");

            migrationBuilder.AddForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartsDetails_Carts_CartId",
                table: "CartsDetails",
                column: "CartId",
                principalTable: "Carts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Books_BookId",
                table: "OrderDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderHeader_OrderHeaderId",
                table: "OrderDetails",
                column: "OrderHeaderId",
                principalTable: "OrderHeader",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_userPdfBooks_AspNetUsers_UserId",
                table: "userPdfBooks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
