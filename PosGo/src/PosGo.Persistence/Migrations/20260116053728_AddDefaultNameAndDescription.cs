using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultNameAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Units",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_Units_RestaurantId_Code",
                table: "Units",
                newName: "IX_Units_RestaurantId_Name");

            migrationBuilder.AddColumn<string>(
                name: "Description",
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
                name: "Description",
                table: "DishCategories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DishCategories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DishCategories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DishCategories");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Units",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_Units_RestaurantId_Name",
                table: "Units",
                newName: "IX_Units_RestaurantId_Code");
        }
    }
}
