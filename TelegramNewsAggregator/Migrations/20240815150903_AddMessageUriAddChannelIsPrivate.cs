using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramNewsAggregator.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageUriAddChannelIsPrivate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Uri",
                table: "Messages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Channels",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Channels",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Uri",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Channels");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Channels",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }
    }
}
