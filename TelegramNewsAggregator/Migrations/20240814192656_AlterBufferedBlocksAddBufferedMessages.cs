using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramNewsAggregator.Migrations
{
    /// <inheritdoc />
    public partial class AlterBufferedBlocksAddBufferedMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BufferedBlocks_Messages_MessageId",
                table: "BufferedBlocks");

            migrationBuilder.DropIndex(
                name: "IX_BufferedBlocks_BlockId_MessageId",
                table: "BufferedBlocks");

            migrationBuilder.DropIndex(
                name: "IX_BufferedBlocks_MessageId",
                table: "BufferedBlocks");

            migrationBuilder.DropColumn(
                name: "BlockId",
                table: "BufferedBlocks");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "BufferedBlocks");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BufferedBlocks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BufferedMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BlockId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BufferedMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BufferedMessages_BufferedBlocks_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BufferedBlocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BufferedMessages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BufferedMessages_BlockId_MessageId",
                table: "BufferedMessages",
                columns: new[] { "BlockId", "MessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BufferedMessages_MessageId",
                table: "BufferedMessages",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BufferedMessages");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BufferedBlocks");

            migrationBuilder.AddColumn<Guid>(
                name: "BlockId",
                table: "BufferedBlocks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "MessageId",
                table: "BufferedBlocks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_BufferedBlocks_BlockId_MessageId",
                table: "BufferedBlocks",
                columns: new[] { "BlockId", "MessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BufferedBlocks_MessageId",
                table: "BufferedBlocks",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_BufferedBlocks_Messages_MessageId",
                table: "BufferedBlocks",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
