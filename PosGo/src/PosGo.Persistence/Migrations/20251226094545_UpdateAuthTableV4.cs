using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuthTableV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedByUserId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UpdatedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedByUserId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UpdatedByUserId",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedByUserId",
                table: "Users",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedByUserId",
                table: "Users",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedByUserId",
                table: "Users",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UpdatedByUserId",
                table: "Users",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
