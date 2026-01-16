using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditSoftDeleteOnDishCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCategoryTranslations_DishCategories_CategoryId",
                table: "DishCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DishCategories");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "DishCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DishCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_DishCategoryTranslations_DishCategories_CategoryId",
                table: "DishCategoryTranslations",
                column: "CategoryId",
                principalTable: "DishCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishCategoryTranslations_DishCategories_CategoryId",
                table: "DishCategoryTranslations");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "DishCategories",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "DishCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DishCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_DishCategoryTranslations_DishCategories_CategoryId",
                table: "DishCategoryTranslations",
                column: "CategoryId",
                principalTable: "DishCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
