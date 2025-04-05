using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class valueobjectsforreceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Proteins",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Nutrients_Proteins");

            migrationBuilder.RenameColumn(
                name: "Fats",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Nutrients_Fats");

            migrationBuilder.RenameColumn(
                name: "Carbohydrates",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Nutrients_Carbohydrates");

            migrationBuilder.RenameColumn(
                name: "Calories",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Nutrients_Calories");

            migrationBuilder.RenameColumn(
                name: "Title",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Title_Value");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Description_Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nutrients_Proteins",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Proteins");

            migrationBuilder.RenameColumn(
                name: "Nutrients_Fats",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Fats");

            migrationBuilder.RenameColumn(
                name: "Nutrients_Carbohydrates",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Carbohydrates");

            migrationBuilder.RenameColumn(
                name: "Nutrients_Calories",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Calories");

            migrationBuilder.RenameColumn(
                name: "Title_Value",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Description_Value",
                schema: "MainAppSchema",
                table: "Receipts",
                newName: "Description");
        }
    }
}
