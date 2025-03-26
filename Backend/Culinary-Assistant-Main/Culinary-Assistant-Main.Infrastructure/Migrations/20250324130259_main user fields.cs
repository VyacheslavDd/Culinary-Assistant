using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mainuserfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Login",
                schema: "MainAppSchema",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.AddColumn<string>(
                name: "Email_Value",
                schema: "MainAppSchema",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Login_Value",
                schema: "MainAppSchema",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "Phone_Value",
                schema: "MainAppSchema",
                table: "Users",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePictureUrl",
                schema: "MainAppSchema",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email_Value",
                schema: "MainAppSchema",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Login_Value",
                schema: "MainAppSchema",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone_Value",
                schema: "MainAppSchema",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePictureUrl",
                schema: "MainAppSchema",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                schema: "MainAppSchema",
                table: "Users",
                newName: "Login");
        }
    }
}
