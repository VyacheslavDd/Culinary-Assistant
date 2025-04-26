using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ratingfieldforreceipt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Rating",
                schema: "MainAppSchema",
                table: "Receipts",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                schema: "MainAppSchema",
                table: "Receipts");
        }
    }
}
