using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditSoftDeleteOnDish : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishTranslations_Dishes_DishId",
                table: "DishTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Dishes");

            migrationBuilder.AddForeignKey(
                name: "FK_DishTranslations_Dishes_DishId",
                table: "DishTranslations",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishTranslations_Dishes_DishId",
                table: "DishTranslations");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Dishes",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Dishes",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_DishTranslations_Dishes_DishId",
                table: "DishTranslations",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
