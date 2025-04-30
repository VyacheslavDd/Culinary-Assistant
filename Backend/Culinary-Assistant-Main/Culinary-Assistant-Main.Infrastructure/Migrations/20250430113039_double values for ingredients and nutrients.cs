using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class doublevaluesforingredientsandnutrients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Nutrients_Proteins",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Nutrients_Fats",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Nutrients_Carbohydrates",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Nutrients_Calories",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Nutrients_Proteins",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Nutrients_Fats",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Nutrients_Carbohydrates",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Nutrients_Calories",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");
        }
    }
}
