using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Idempotencia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "claves_idempotencia",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Clave = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    HuellaSolicitud = table.Column<string>(type: "TEXT", maxLength: 128, nullable: false),
                    Resultado = table.Column<string>(type: "TEXT", nullable: true),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    CreadoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_claves_idempotencia", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_claves_idempotencia_Clave",
                table: "claves_idempotencia",
                column: "Clave",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "claves_idempotencia");
        }
    }
}
