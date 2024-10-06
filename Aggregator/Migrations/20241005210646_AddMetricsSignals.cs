using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aggregator.Migrations
{
    /// <inheritdoc />
    public partial class AddMetricsSignals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MetricsSignals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserTelegramId = table.Column<long>(type: "bigint", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    ClickedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MetricsSignals", x => x.Id);
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MetricsSignals");
        }
    }
}
