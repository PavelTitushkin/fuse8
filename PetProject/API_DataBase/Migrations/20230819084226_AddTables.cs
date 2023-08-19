using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cur");

            migrationBuilder.CreateTable(
                name: "currencies_list",
                schema: "cur",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies_list", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "currencies",
                schema: "cur",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<decimal>(type: "numeric", nullable: false),
                    currencies_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_currencies", x => x.id);
                    table.ForeignKey(
                        name: "fk_currencies_currencies_list_currencies_id",
                        column: x => x.currencies_id,
                        principalSchema: "cur",
                        principalTable: "currencies_list",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_currencies_currencies_id",
                schema: "cur",
                table: "currencies",
                column: "currencies_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currencies",
                schema: "cur");

            migrationBuilder.DropTable(
                name: "currencies_list",
                schema: "cur");
        }
    }
}
