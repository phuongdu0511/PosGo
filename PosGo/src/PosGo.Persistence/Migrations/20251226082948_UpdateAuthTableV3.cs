using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PosGo.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAuthTableV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
