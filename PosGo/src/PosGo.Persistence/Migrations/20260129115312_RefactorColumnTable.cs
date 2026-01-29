using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RefactorColumnTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DishSkus_DishId_IsDefault",
                table: "DishSkus");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "DishSkus");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "DishVariants",
                newName: "Name");

            migrationBuilder.RenameIndex(
                name: "IX_DishVariants_DishId_Code",
                table: "DishVariants",
                newName: "IX_DishVariants_DishId_Name");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "DishVariantOptions",
                newName: "Value");

            migrationBuilder.RenameIndex(
                name: "IX_DishVariantOptions_VariantId_Code",
                table: "DishVariantOptions",
                newName: "IX_DishVariantOptions_VariantId_Value");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "DishSkus",
                newName: "Sku");

            migrationBuilder.RenameIndex(
                name: "IX_DishSkus_DishId_Code",
                table: "DishSkus",
                newName: "IX_DishSkus_DishId_Sku");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "DishVariants",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_DishVariants_DishId_Name",
                table: "DishVariants",
                newName: "IX_DishVariants_DishId_Code");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "DishVariantOptions",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_DishVariantOptions_VariantId_Value",
                table: "DishVariantOptions",
                newName: "IX_DishVariantOptions_VariantId_Code");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "DishSkus",
                newName: "Code");

            migrationBuilder.RenameIndex(
                name: "IX_DishSkus_DishId_Sku",
                table: "DishSkus",
                newName: "IX_DishSkus_DishId_Code");

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "DishSkus",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_DishSkus_DishId_IsDefault",
                table: "DishSkus",
                columns: new[] { "DishId", "IsDefault" });
        }
    }
}
