using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ConflictosYCierre : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CerradoEn",
                table: "relevamientos",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "conflictos_marcadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    Resolucion = table.Column<int>(type: "INTEGER", nullable: true),
                    DetectadoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ResueltoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conflictos_marcadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_conflictos_marcadores_relevamientos_RelevamientoId",
                        column: x => x.RelevamientoId,
                        principalTable: "relevamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "conflictos_marcador_miembro",
                columns: table => new
                {
                    ConflictoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MarcadorId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_conflictos_marcador_miembro", x => new { x.ConflictoId, x.MarcadorId });
                    table.ForeignKey(
                        name: "FK_conflictos_marcador_miembro_conflictos_marcadores_ConflictoId",
                        column: x => x.ConflictoId,
                        principalTable: "conflictos_marcadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_conflictos_marcador_miembro_marcadores_MarcadorId",
                        column: x => x.MarcadorId,
                        principalTable: "marcadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_conflictos_marcador_miembro_MarcadorId",
                table: "conflictos_marcador_miembro",
                column: "MarcadorId");

            migrationBuilder.CreateIndex(
                name: "IX_conflictos_marcadores_RelevamientoId_Estado",
                table: "conflictos_marcadores",
                columns: new[] { "RelevamientoId", "Estado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "conflictos_marcador_miembro");

            migrationBuilder.DropTable(
                name: "conflictos_marcadores");

            migrationBuilder.DropColumn(
                name: "CerradoEn",
                table: "relevamientos");
        }
    }
}
