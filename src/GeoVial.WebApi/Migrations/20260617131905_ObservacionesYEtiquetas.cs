using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ObservacionesYEtiquetas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "etiquetas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etiquetas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_etiquetas_relevamientos_RelevamientoId",
                        column: x => x.RelevamientoId,
                        principalTable: "relevamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "observaciones",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    MarcadorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AutorId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nota = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_observaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_observaciones_marcadores_MarcadorId",
                        column: x => x.MarcadorId,
                        principalTable: "marcadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_observaciones_usuarios_AutorId",
                        column: x => x.AutorId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "etiquetas_marcador",
                columns: table => new
                {
                    EtiquetaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MarcadorId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_etiquetas_marcador", x => new { x.EtiquetaId, x.MarcadorId });
                    table.ForeignKey(
                        name: "FK_etiquetas_marcador_etiquetas_EtiquetaId",
                        column: x => x.EtiquetaId,
                        principalTable: "etiquetas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_etiquetas_marcador_marcadores_MarcadorId",
                        column: x => x.MarcadorId,
                        principalTable: "marcadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_etiquetas_RelevamientoId_Nombre",
                table: "etiquetas",
                columns: new[] { "RelevamientoId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_etiquetas_marcador_MarcadorId",
                table: "etiquetas_marcador",
                column: "MarcadorId");

            migrationBuilder.CreateIndex(
                name: "IX_observaciones_AutorId",
                table: "observaciones",
                column: "AutorId");

            migrationBuilder.CreateIndex(
                name: "IX_observaciones_MarcadorId",
                table: "observaciones",
                column: "MarcadorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "etiquetas_marcador");

            migrationBuilder.DropTable(
                name: "observaciones");

            migrationBuilder.DropTable(
                name: "etiquetas");
        }
    }
}
