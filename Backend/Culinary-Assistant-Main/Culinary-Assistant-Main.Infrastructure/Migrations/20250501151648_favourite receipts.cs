using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class favouritereceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FavouritedReceiptsInfo",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FavouritedReceiptsInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FavouritedReceiptsInfo_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FavouritedReceiptsInfo_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FavouritedReceiptsInfo_ReceiptId",
                schema: "MainAppSchema",
                table: "FavouritedReceiptsInfo",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_FavouritedReceiptsInfo_UserId",
                schema: "MainAppSchema",
                table: "FavouritedReceiptsInfo",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FavouritedReceiptsInfo",
                schema: "MainAppSchema");
        }
    }
}
