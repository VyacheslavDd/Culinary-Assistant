﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Culinary_Assistant_Main.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class nullableuserid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptRates_Users_UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

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

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "MainAppSchema",
                table: "ReceiptRates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

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
    }
}
