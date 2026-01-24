using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bagman.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMatchStartedFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "started",
                table: "matches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "started",
                table: "matches",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
