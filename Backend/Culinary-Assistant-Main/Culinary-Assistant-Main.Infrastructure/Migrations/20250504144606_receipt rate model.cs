using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class receiptratemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReceiptRates",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiptId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rate = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptRates_Receipts_ReceiptId",
                        column: x => x.ReceiptId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Receipts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptRates_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRates_ReceiptId",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                column: "ReceiptId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptRates_UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptRates",
                schema: "MainAppSchema");
        }
    }
}
