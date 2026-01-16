using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAuditableOnTranslation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DishTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "DishTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DishTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "DishTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DishCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "DishCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DishCategoryTranslations");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "DishCategoryTranslations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "DishTranslations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "DishTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "DishTranslations",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "DishTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "DishCategoryTranslations",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "DishCategoryTranslations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "DishCategoryTranslations",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "DishCategoryTranslations",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
