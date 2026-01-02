using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeIndexRestaurantPlanTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantPlans_RestaurantId",
                table: "RestaurantPlans");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPlans_RestaurantId_PlanId",
                table: "RestaurantPlans",
                columns: new[] { "RestaurantId", "PlanId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RestaurantPlans_RestaurantId_PlanId",
                table: "RestaurantPlans");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantPlans_RestaurantId",
                table: "RestaurantPlans",
                column: "RestaurantId",
                unique: true,
                filter: "[IsActive] = 1");
        }
    }
}
