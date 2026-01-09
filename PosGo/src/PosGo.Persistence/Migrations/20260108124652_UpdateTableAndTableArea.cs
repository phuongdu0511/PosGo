using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableAndTableArea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TableAreas");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "TableAreas");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "TableAreas");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Tables",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Tables_RestaurantId_Code",
                table: "Tables",
                newName: "IX_Tables_RestaurantId_Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Tables",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_Tables_RestaurantId_Name",
                table: "Tables",
                newName: "IX_Tables_RestaurantId_Code");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "Tables",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "Tables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Tables",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAt",
                table: "TableAreas",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeletedByUserId",
                table: "TableAreas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "TableAreas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
