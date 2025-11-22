using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Electro.Reposatory.Migrations
{
    /// <inheritdoc />
    public partial class dsfvdssfvddيdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatAttachments");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatThreads");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatThreads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedAdminUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LastMessageAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    UnreadForAdmin = table.Column<int>(type: "int", nullable: false),
                    UnreadForCustomer = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatThreads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ThreadId = table.Column<int>(type: "int", nullable: false),
                    FromUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFromAdmin = table.Column<bool>(type: "bit", nullable: false),
                    SeenByAdminAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SeenByCustomerAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ToUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatThreads_ThreadId",
                        column: x => x.ThreadId,
                        principalTable: "ChatThreads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DurationMs = table.Column<int>(type: "int", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: true),
                    ThumbnailUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatAttachments_ChatMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatAttachments_MessageId",
                table: "ChatAttachments",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ThreadId_SentAtUtc",
                table: "ChatMessages",
                columns: new[] { "ThreadId", "SentAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatThreads_CustomerUserId",
                table: "ChatThreads",
                column: "CustomerUserId");
        }
    }
}
