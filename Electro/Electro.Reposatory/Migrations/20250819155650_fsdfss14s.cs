using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Electro.Reposatory.Migrations
{
    /// <inheritdoc />
    public partial class fsdfss14s : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Banners");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Banners",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
