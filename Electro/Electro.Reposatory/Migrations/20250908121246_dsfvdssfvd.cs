using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Electro.Reposatory.Migrations
{
    /// <inheritdoc />
    public partial class dsfvdssfvd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlLocation",
                table: "CommunicationMethods",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlLocation",
                table: "CommunicationMethods");
        }
    }
}
