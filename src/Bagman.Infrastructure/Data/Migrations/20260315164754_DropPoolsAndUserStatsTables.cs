using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bagman.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class DropPoolsAndUserStatsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "pool_winners");

            migrationBuilder.DropTable(
                name: "user_stats");

            migrationBuilder.DropTable(
                name: "pools");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "pools",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pools_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_pools_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bets_placed = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    matches_played = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    pools_won = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_won = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_stats", x => new { x.user_id, x.table_id });
                    table.ForeignKey(
                        name: "FK_user_stats_tables_table_id",
                        column: x => x.table_id,
                        principalTable: "tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_stats_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pool_winners",
                columns: table => new
                {
                    pool_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount_won = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_pool_winners", x => new { x.pool_id, x.user_id });
                    table.ForeignKey(
                        name: "FK_pool_winners_pools_pool_id",
                        column: x => x.pool_id,
                        principalTable: "pools",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_pool_winners_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_pool_winners_user_id",
                table: "pool_winners",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_pools_match_id",
                table: "pools",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_stats_table_id",
                table: "user_stats",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_stats_user_id",
                table: "user_stats",
                column: "user_id");
        }
    }
}
