using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramNewsAggregator.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUsingEFFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Channels_Name",
                table: "Channels");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_TelegramId",
                table: "Channels",
                column: "TelegramId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Channels_TelegramId",
                table: "Channels");

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Name",
                table: "Channels",
                column: "Name",
                unique: true);
        }
    }
}
