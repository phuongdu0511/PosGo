using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ModifedOnUtc",
                table: "Product",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedOnUtc",
                table: "Product",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Product",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Product",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Product",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "Product",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Product",
                newName: "ModifedOnUtc");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Product",
                newName: "CreatedOnUtc");
        }
    }
}
