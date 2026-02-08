using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bagman.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSuperAdminFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_tables_table_id",
                table: "matches");

            migrationBuilder.DropPrimaryKey(
                name: "pk_table_members",
                table: "table_members");

            migrationBuilder.DropPrimaryKey(
                name: "pk_bets_id",
                table: "bets");

            migrationBuilder.RenameColumn(
                name: "table_id",
                table: "matches",
                newName: "event_type_id");

            migrationBuilder.RenameIndex(
                name: "idx_matches_table_id",
                table: "matches",
                newName: "idx_matches_event_type_id");

            migrationBuilder.AddColumn<bool>(
                name: "is_super_admin",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "event_type_id",
                table: "tables",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_table_members",
                table: "table_members",
                columns: new[] { "user_id", "table_id" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_bets",
                table: "bets",
                column: "id");

            migrationBuilder.CreateTable(
                name: "event_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event_types_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tables_event_type_id",
                table: "tables",
                column: "event_type_id");

            migrationBuilder.CreateIndex(
                name: "idx_event_types_is_active",
                table: "event_types",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "uk_event_types_code",
                table: "event_types",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_matches_event_types_event_type_id",
                table: "matches",
                column: "event_type_id",
                principalTable: "event_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tables_event_types_event_type_id",
                table: "tables",
                column: "event_type_id",
                principalTable: "event_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_matches_event_types_event_type_id",
                table: "matches");

            migrationBuilder.DropForeignKey(
                name: "FK_tables_event_types_event_type_id",
                table: "tables");

            migrationBuilder.DropTable(
                name: "event_types");

            migrationBuilder.DropIndex(
                name: "IX_tables_event_type_id",
                table: "tables");

            migrationBuilder.DropPrimaryKey(
                name: "PK_table_members",
                table: "table_members");

            migrationBuilder.DropPrimaryKey(
                name: "PK_bets",
                table: "bets");

            migrationBuilder.DropColumn(
                name: "is_super_admin",
                table: "users");

            migrationBuilder.DropColumn(
                name: "event_type_id",
                table: "tables");

            migrationBuilder.RenameColumn(
                name: "event_type_id",
                table: "matches",
                newName: "table_id");

            migrationBuilder.RenameIndex(
                name: "idx_matches_event_type_id",
                table: "matches",
                newName: "idx_matches_table_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_table_members",
                table: "table_members",
                columns: new[] { "user_id", "table_id" });

            migrationBuilder.AddPrimaryKey(
                name: "pk_bets_id",
                table: "bets",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_matches_tables_table_id",
                table: "matches",
                column: "table_id",
                principalTable: "tables",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
