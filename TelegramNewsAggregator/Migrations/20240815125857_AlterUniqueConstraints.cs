using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramNewsAggregator.Migrations
{
    /// <inheritdoc />
    public partial class AlterUniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks");

            migrationBuilder.DropIndex(
                name: "IX_SummaryBlocks_SummaryId_MessageId",
                table: "SummaryBlocks");

            migrationBuilder.DropIndex(
                name: "IX_BufferedMessages_BlockId_MessageId",
                table: "BufferedMessages");

            migrationBuilder.DropIndex(
                name: "IX_BufferedMessages_MessageId",
                table: "BufferedMessages");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_SummaryId",
                table: "SummaryBlocks",
                column: "SummaryId");

            migrationBuilder.CreateIndex(
                name: "IX_BufferedMessages_BlockId",
                table: "BufferedMessages",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_BufferedMessages_MessageId",
                table: "BufferedMessages",
                column: "MessageId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks");

            migrationBuilder.DropIndex(
                name: "IX_SummaryBlocks_SummaryId",
                table: "SummaryBlocks");

            migrationBuilder.DropIndex(
                name: "IX_BufferedMessages_BlockId",
                table: "BufferedMessages");

            migrationBuilder.DropIndex(
                name: "IX_BufferedMessages_MessageId",
                table: "BufferedMessages");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_SummaryId_MessageId",
                table: "SummaryBlocks",
                columns: new[] { "SummaryId", "MessageId" },
                unique: true);

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
    }
}
