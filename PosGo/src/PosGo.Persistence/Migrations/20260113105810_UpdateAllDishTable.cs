using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAllDishTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DishSkus_RestaurantId",
                table: "DishSkus");

            migrationBuilder.DropIndex(
                name: "IX_DishCategories_RestaurantId",
                table: "DishCategories");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "DishSkus",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "DishSkus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnMenu",
                table: "Dishes",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "Dishes",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowOnMenu",
                table: "DishCategories",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateTable(
                name: "DishComboItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComboDishId = table.Column<int>(type: "int", nullable: false),
                    ItemDishId = table.Column<int>(type: "int", nullable: false),
                    ItemDishSkuId = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DishId = table.Column<int>(type: "int", nullable: true),
                    DishId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishComboItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishComboItems_DishSkus_ItemDishSkuId",
                        column: x => x.ItemDishSkuId,
                        principalTable: "DishSkus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DishComboItems_Dishes_ComboDishId",
                        column: x => x.ComboDishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishComboItems_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DishComboItems_Dishes_DishId1",
                        column: x => x.DishId1,
                        principalTable: "Dishes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DishComboItems_Dishes_ItemDishId",
                        column: x => x.ItemDishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DishImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DishId = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    AltText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DishId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishImages_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishImages_Dishes_DishId1",
                        column: x => x.DishId1,
                        principalTable: "Dishes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishSkus_RestaurantId_IsActive_StockQuantity",
                table: "DishSkus",
                columns: new[] { "RestaurantId", "IsActive", "StockQuantity" });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_RestaurantId_CategoryId_SortOrder",
                table: "Dishes",
                columns: new[] { "RestaurantId", "CategoryId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_RestaurantId_DishTypeId",
                table: "Dishes",
                columns: new[] { "RestaurantId", "DishTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_RestaurantId_IsActive_IsAvailable",
                table: "Dishes",
                columns: new[] { "RestaurantId", "IsActive", "IsAvailable" });

            migrationBuilder.CreateIndex(
                name: "IX_DishCategories_RestaurantId_IsActive",
                table: "DishCategories",
                columns: new[] { "RestaurantId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DishCategories_RestaurantId_ParentCategoryId_SortOrder",
                table: "DishCategories",
                columns: new[] { "RestaurantId", "ParentCategoryId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DishComboItems_ComboDishId_DisplayOrder",
                table: "DishComboItems",
                columns: new[] { "ComboDishId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DishComboItems_ComboDishId_ItemDishId",
                table: "DishComboItems",
                columns: new[] { "ComboDishId", "ItemDishId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DishComboItems_DishId",
                table: "DishComboItems",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishComboItems_DishId1",
                table: "DishComboItems",
                column: "DishId1");

            migrationBuilder.CreateIndex(
                name: "IX_DishComboItems_ItemDishId",
                table: "DishComboItems",
                column: "ItemDishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishComboItems_ItemDishSkuId",
                table: "DishComboItems",
                column: "ItemDishSkuId");

            migrationBuilder.CreateIndex(
                name: "IX_DishImages_DishId_DisplayOrder",
                table: "DishImages",
                columns: new[] { "DishId", "DisplayOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_DishImages_DishId_IsPrimary",
                table: "DishImages",
                columns: new[] { "DishId", "IsPrimary" });

            migrationBuilder.CreateIndex(
                name: "IX_DishImages_DishId1",
                table: "DishImages",
                column: "DishId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishComboItems");

            migrationBuilder.DropTable(
                name: "DishImages");

            migrationBuilder.DropIndex(
                name: "IX_DishSkus_RestaurantId_IsActive_StockQuantity",
                table: "DishSkus");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_RestaurantId_CategoryId_SortOrder",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_RestaurantId_DishTypeId",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_RestaurantId_IsActive_IsAvailable",
                table: "Dishes");

            migrationBuilder.DropIndex(
                name: "IX_DishCategories_RestaurantId_IsActive",
                table: "DishCategories");

            migrationBuilder.DropIndex(
                name: "IX_DishCategories_RestaurantId_ParentCategoryId_SortOrder",
                table: "DishCategories");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "DishSkus");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "DishSkus");

            migrationBuilder.DropColumn(
                name: "ShowOnMenu",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "ShowOnMenu",
                table: "DishCategories");

            migrationBuilder.CreateIndex(
                name: "IX_DishSkus_RestaurantId",
                table: "DishSkus",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCategories_RestaurantId",
                table: "DishCategories",
                column: "RestaurantId");
        }
    }
}
