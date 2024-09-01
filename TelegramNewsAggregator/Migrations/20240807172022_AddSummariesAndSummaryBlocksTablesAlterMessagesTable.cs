using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramNewsAggregator.Migrations
{
    /// <inheritdoc />
    public partial class AddSummariesAndSummaryBlocksTablesAlterMessagesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SummariesMessages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_TelegramId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Channels_TelegramId",
                table: "Channels");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Messages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SummaryBlocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryId = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummaryBlocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummaryBlocks_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SummaryBlocks_Summaries_SummaryId",
                        column: x => x.SummaryId,
                        principalTable: "Summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_SummaryId_MessageId",
                table: "SummaryBlocks",
                columns: new[] { "SummaryId", "MessageId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SummaryBlocks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Messages");

            migrationBuilder.CreateTable(
                name: "SummariesMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageId = table.Column<Guid>(type: "uuid", nullable: false),
                    SummaryId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SummariesMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SummariesMessages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SummariesMessages_Summaries_SummaryId",
                        column: x => x.SummaryId,
                        principalTable: "Summaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_TelegramId",
                table: "Messages",
                column: "TelegramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_TelegramId",
                table: "Channels",
                column: "TelegramId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SummariesMessages_MessageId",
                table: "SummariesMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_SummariesMessages_SummaryId_MessageId",
                table: "SummariesMessages",
                columns: new[] { "SummaryId", "MessageId" },
                unique: true);
        }
    }
}
