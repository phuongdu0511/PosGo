using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColumnNameAndImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dishes_RestaurantId_Name",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DishCategories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Dishes",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Dishes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DishCategories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_RestaurantId_Name",
                table: "Dishes",
                columns: new[] { "RestaurantId", "Name" },
                unique: true);
        }
    }
}
