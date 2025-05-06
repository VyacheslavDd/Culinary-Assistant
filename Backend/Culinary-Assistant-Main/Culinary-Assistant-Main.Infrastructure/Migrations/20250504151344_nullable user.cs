using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nullableuser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptRates_Users_UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptRates_Users_UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                column: "UserId",
                principalSchema: "MainAppSchema",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptRates_Users_UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptRates_Users_UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                column: "UserId",
                principalSchema: "MainAppSchema",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
