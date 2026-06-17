using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class RelevamientosYMarcadores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "relevamientos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TramoVial = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false),
                    IdJefeArea = table.Column<Guid>(type: "TEXT", nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_relevamientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_relevamientos_usuarios_IdJefeArea",
                        column: x => x.IdJefeArea,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "asignaciones_agente",
                columns: table => new
                {
                    RelevamientoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdAgente = table.Column<Guid>(type: "TEXT", nullable: false),
                    FechaAsignacion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_asignaciones_agente", x => new { x.RelevamientoId, x.IdAgente });
                    table.ForeignKey(
                        name: "FK_asignaciones_agente_relevamientos_RelevamientoId",
                        column: x => x.RelevamientoId,
                        principalTable: "relevamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_asignaciones_agente_usuarios_IdAgente",
                        column: x => x.IdAgente,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "marcadores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Latitud = table.Column<double>(type: "REAL", nullable: false),
                    Longitud = table.Column<double>(type: "REAL", nullable: false),
                    Descripcion = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marcadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marcadores_relevamientos_RelevamientoId",
                        column: x => x.RelevamientoId,
                        principalTable: "relevamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_asignaciones_agente_IdAgente",
                table: "asignaciones_agente",
                column: "IdAgente");

            migrationBuilder.CreateIndex(
                name: "IX_marcadores_RelevamientoId",
                table: "marcadores",
                column: "RelevamientoId");

            migrationBuilder.CreateIndex(
                name: "IX_relevamientos_IdJefeArea",
                table: "relevamientos",
                column: "IdJefeArea");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "asignaciones_agente");

            migrationBuilder.DropTable(
                name: "marcadores");

            migrationBuilder.DropTable(
                name: "relevamientos");
        }
    }
}
