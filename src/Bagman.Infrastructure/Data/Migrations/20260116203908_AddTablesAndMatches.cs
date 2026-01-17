using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bagman.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTablesAndMatches : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tables",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    max_players = table.Column<int>(type: "integer", nullable: false),
                    stake = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_secret_mode = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tables_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_tables_users_created_by",
                        column: x => x.created_by,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "matches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    country_1 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country_2 = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    match_datetime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    result = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "scheduled"),
                    started = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_matches_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_matches_tables_table_id",
                        column: x => x.table_id,
                        principalTable: "tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "table_members",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    joined_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_members", x => new { x.user_id, x.table_id });
                    table.ForeignKey(
                        name: "FK_table_members_tables_table_id",
                        column: x => x.table_id,
                        principalTable: "tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_table_members_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_stats",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    matches_played = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    bets_placed = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
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
                name: "bets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    prediction = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    edited_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bets_id", x => x.id);
                    table.ForeignKey(
                        name: "FK_bets_matches_match_id",
                        column: x => x.match_id,
                        principalTable: "matches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_bets_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pools",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    match_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false, defaultValue: 0m),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "active"),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "idx_bets_match_id",
                table: "bets",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "idx_bets_user_id",
                table: "bets",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "uk_bets_user_match",
                table: "bets",
                columns: new[] { "user_id", "match_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_matches_datetime",
                table: "matches",
                column: "match_datetime");

            migrationBuilder.CreateIndex(
                name: "idx_matches_table_id",
                table: "matches",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "IX_pool_winners_user_id",
                table: "pool_winners",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_pools_match_id",
                table: "pools",
                column: "match_id");

            migrationBuilder.CreateIndex(
                name: "idx_table_members_table_id",
                table: "table_members",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "idx_table_members_user_id",
                table: "table_members",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "idx_tables_created_by",
                table: "tables",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "idx_user_stats_table_id",
                table: "user_stats",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "idx_user_stats_user_id",
                table: "user_stats",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "bets");

            migrationBuilder.DropTable(
                name: "pool_winners");

            migrationBuilder.DropTable(
                name: "table_members");

            migrationBuilder.DropTable(
                name: "user_stats");

            migrationBuilder.DropTable(
                name: "pools");

            migrationBuilder.DropTable(
                name: "matches");

            migrationBuilder.DropTable(
                name: "tables");
        }
    }
}
