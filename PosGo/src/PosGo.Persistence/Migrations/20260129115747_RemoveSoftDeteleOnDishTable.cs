using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSoftDeteleOnDishTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DishVariants");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "DishVariants");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DishVariants");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DishVariantOptions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "DishVariantOptions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DishVariantOptions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DishSkus");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "DishSkus");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DishSkus");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "DishVariants",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "DishVariants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DishVariants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "DishVariantOptions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "DishVariantOptions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DishVariantOptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "DishSkus",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "DishSkus",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DishSkus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
