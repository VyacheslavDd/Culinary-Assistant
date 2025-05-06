using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class abstractedrateentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Rate",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                newName: "Rating");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                schema: "MainAppSchema",
                table: "ReceiptCollections",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "ReceiptCollectionRates",
                schema: "MainAppSchema",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReceiptCollectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptCollectionRates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReceiptCollectionRates_ReceiptCollections_ReceiptCollection~",
                        column: x => x.ReceiptCollectionId,
                        principalSchema: "MainAppSchema",
                        principalTable: "ReceiptCollections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReceiptCollectionRates_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "MainAppSchema",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCollectionRates_ReceiptCollectionId",
                schema: "MainAppSchema",
                table: "ReceiptCollectionRates",
                column: "ReceiptCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptCollectionRates_UserId",
                schema: "MainAppSchema",
                table: "ReceiptCollectionRates",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReceiptCollectionRates",
                schema: "MainAppSchema");

            migrationBuilder.DropColumn(
                name: "Rating",
                schema: "MainAppSchema",
                table: "ReceiptCollections");

            migrationBuilder.RenameColumn(
                name: "Rating",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                newName: "Rate");
        }
    }
}
