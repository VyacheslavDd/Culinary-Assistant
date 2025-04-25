using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class receiptcollectionmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptCollections",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title_Value = table.Column<string>(type: "text", nullable: false),
                    ReceiptCovers = table.Column<string>(type: "text", nullable: false),
                    IsPrivate = table.Column<bool>(type: "boolean", nullable: false),
                    IsFavourite = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptCollections_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptReceiptCollection",
                schema: "MainAppSchema",
                columns: table => new
                {
                    ReceiptCollectionsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptReceiptCollection", x => new { x.ReceiptCollectionsId, x.ReceiptsId });
                    table.ForeignKey(
                        name: "FK_ReceiptReceiptCollection_ReceiptCollections_ReceiptCollecti~",
                        column: x => x.ReceiptCollectionsId,
                        principalSchema: "MainAppSchema",
                        principalTable: "ReceiptCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptReceiptCollection_Receipts_ReceiptsId",
                        column: x => x.ReceiptsId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCollections_UserId",
                schema: "MainAppSchema",
                table: "ReceiptCollections",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptReceiptCollection_ReceiptsId",
                schema: "MainAppSchema",
                table: "ReceiptReceiptCollection",
                column: "ReceiptsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptReceiptCollection",
                schema: "MainAppSchema");

            migrationBuilder.DropTable(
                name: "ReceiptCollections",
                schema: "MainAppSchema");
        }
    }
}
