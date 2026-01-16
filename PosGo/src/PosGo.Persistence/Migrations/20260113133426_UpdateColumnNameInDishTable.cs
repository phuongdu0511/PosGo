using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateColumnNameInDishTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dishes_RestaurantId_Code",
                table: "Dishes");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Dishes",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "DishCategories",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DishVariantTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DishVariantOptionTranslations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "DishCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_RestaurantId_Name",
                table: "Dishes",
                columns: new[] { "RestaurantId", "Name" },
                unique: true,
                filter: "[Name] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Dishes_RestaurantId_Name",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DishVariantTranslations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DishVariantOptionTranslations");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "DishCategories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Dishes",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "DishCategories",
                newName: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_RestaurantId_Code",
                table: "Dishes",
                columns: new[] { "RestaurantId", "Code" },
                unique: true,
                filter: "[Code] IS NOT NULL");
        }
    }
}
