using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTypeColumnRestaurantUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Bỏ khóa chính cũ
            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers");

            // 2. Xóa cột Id kiểu Guid
            migrationBuilder.DropColumn(
                name: "Id",
                table: "RestaurantUsers");

            // 3. Thêm lại cột Id kiểu int identity
            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "RestaurantUsers",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            // 4. Đặt lại khóa chính
            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse lại nếu rollback migration

            // 1. Bỏ PK int
            migrationBuilder.DropPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers");

            // 2. Xóa cột Id int
            migrationBuilder.DropColumn(
                name: "Id",
                table: "RestaurantUsers");

            // 3. Thêm lại cột Id kiểu Guid
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "RestaurantUsers",
                type: "uniqueidentifier",
                nullable: false);

            // 4. Đặt lại PK Guid
            migrationBuilder.AddPrimaryKey(
                name: "PK_RestaurantUsers",
                table: "RestaurantUsers",
                column: "Id");
        }
    }
}
