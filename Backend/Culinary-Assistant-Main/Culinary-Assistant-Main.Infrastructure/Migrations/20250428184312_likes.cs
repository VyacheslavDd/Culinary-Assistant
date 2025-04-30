using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class likes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptCollectionLikes",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptCollectionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptCollectionLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptCollectionLikes_ReceiptCollections_ReceiptCollection~",
                        column: x => x.ReceiptCollectionId,
                        principalSchema: "MainAppSchema",
                        principalTable: "ReceiptCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptCollectionLikes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReceiptLikes",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptLikes_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptLikes_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCollectionLikes_ReceiptCollectionId",
                schema: "MainAppSchema",
                table: "ReceiptCollectionLikes",
                column: "ReceiptCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCollectionLikes_UserId",
                schema: "MainAppSchema",
                table: "ReceiptCollectionLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptLikes_ReceiptId",
                schema: "MainAppSchema",
                table: "ReceiptLikes",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptLikes_UserId",
                schema: "MainAppSchema",
                table: "ReceiptLikes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptCollectionLikes",
                schema: "MainAppSchema");

            migrationBuilder.DropTable(
                name: "ReceiptLikes",
                schema: "MainAppSchema");
        }
    }
}
