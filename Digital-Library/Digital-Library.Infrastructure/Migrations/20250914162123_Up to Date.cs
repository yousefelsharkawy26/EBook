using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Digital_Library.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UptoDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Vendors_VendorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails");

            migrationBuilder.DropTable(
                name: "Borrowings");

            migrationBuilder.DropTable(
                name: "userPdfBooks");

            migrationBuilder.AddColumn<byte[]>(
                name: "PDFIV",
                table: "Books",
                type: "varbinary(16)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PDFTag",
                table: "Books",
                type: "varbinary(16)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PublicKey",
                table: "AspNetUsers",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserBookAccesses",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EncryptedDEK = table.Column<byte[]>(type: "varbinary(32)", nullable: true),
                    IV = table.Column<byte[]>(type: "varbinary(16)", nullable: true),
                    Tag = table.Column<byte[]>(type: "varbinary(16)", nullable: true),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBookAccesses", x => new { x.UserId, x.BookId });
                    table.ForeignKey(
                        name: "FK_UserBookAccesses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBookAccesses_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBookAccesses_BookId",
                table: "UserBookAccesses",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Vendors_VendorId",
                table: "Books",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Vendors_VendorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails");

            migrationBuilder.DropTable(
                name: "UserBookAccesses");

            migrationBuilder.DropColumn(
                name: "PDFIV",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PDFTag",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "PublicKey",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "Borrowings",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Borrowings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Borrowings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Borrowings_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "userPdfBooks",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userPdfBooks", x => new { x.UserId, x.BookId });
                    table.ForeignKey(
                        name: "FK_userPdfBooks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_userPdfBooks_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_BookId",
                table: "Borrowings",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_UserId",
                table: "Borrowings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_userPdfBooks_BookId",
                table: "userPdfBooks",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Vendors_VendorId",
                table: "Books",
                column: "VendorId",
                principalTable: "Vendors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartsDetails_Books_BookId",
                table: "CartsDetails",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
