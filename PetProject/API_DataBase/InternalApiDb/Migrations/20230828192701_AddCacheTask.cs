using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataStore.InternalApiDb.Migrations
{
    /// <inheritdoc />
    public partial class AddCacheTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "cache_status",
                schema: "cur",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cache_tack_status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cache_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cache_tasks",
                schema: "cur",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cache_status_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cache_tasks", x => x.id);
                    table.ForeignKey(
                        name: "fk_cache_tasks_cache_status_cache_status_id",
                        column: x => x.cache_status_id,
                        principalSchema: "cur",
                        principalTable: "cache_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cache_tasks_cache_status_id",
                schema: "cur",
                table: "cache_tasks",
                column: "cache_status_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cache_tasks",
                schema: "cur");

            migrationBuilder.DropTable(
                name: "cache_status",
                schema: "cur");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
