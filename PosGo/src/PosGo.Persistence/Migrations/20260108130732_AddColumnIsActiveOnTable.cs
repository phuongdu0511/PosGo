using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnIsActiveOnTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tables_CodeItems_StatusId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_RestaurantId_StatusId",
                table: "Tables");

            migrationBuilder.DropIndex(
                name: "IX_Tables_StatusId",
                table: "Tables");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "Tables");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Tables",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Tables");

            migrationBuilder.AddColumn<Guid>(
                name: "StatusId",
                table: "Tables",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tables_RestaurantId_StatusId",
                table: "Tables",
                columns: new[] { "RestaurantId", "StatusId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tables_StatusId",
                table: "Tables",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tables_CodeItems_StatusId",
                table: "Tables",
                column: "StatusId",
                principalTable: "CodeItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
