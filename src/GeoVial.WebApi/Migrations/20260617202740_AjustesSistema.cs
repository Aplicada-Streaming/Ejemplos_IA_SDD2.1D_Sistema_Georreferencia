using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AjustesSistema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ajustes_sistema",
                columns: table => new
                {
                    Clave = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Valor = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    ActualizadoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ajustes_sistema", x => x.Clave);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ajustes_sistema");
        }
    }
}
