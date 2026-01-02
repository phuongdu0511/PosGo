using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLogicRestaurantPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantPlans_RestaurantId",
                table: "RestaurantPlans");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "RestaurantPlans",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "RestaurantPlans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UpdatedAt",
                table: "RestaurantPlans",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedByUserId",
                table: "RestaurantPlans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPlans_RestaurantId",
                table: "RestaurantPlans",
                column: "RestaurantId",
                unique: true,
                filter: "[IsActive] = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantPlans_RestaurantId",
                table: "RestaurantPlans");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RestaurantPlans");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "RestaurantPlans");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "RestaurantPlans");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "RestaurantPlans");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPlans_RestaurantId",
                table: "RestaurantPlans",
                column: "RestaurantId",
                unique: true);
        }
    }
}
