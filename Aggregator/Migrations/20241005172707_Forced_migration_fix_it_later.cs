using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aggregator.Migrations
{
    /// <inheritdoc />
    public partial class Forced_migration_fix_it_later : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks",
                column: "MessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks");

            migrationBuilder.CreateIndex(
                name: "IX_SummaryBlocks_MessageId",
                table: "SummaryBlocks",
                column: "MessageId",
                unique: true);
        }
    }
}
