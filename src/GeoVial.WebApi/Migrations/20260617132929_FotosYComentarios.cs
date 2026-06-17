using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class FotosYComentarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "fotos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ObservacionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ReferenciaAlmacen = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Latitud = table.Column<double>(type: "REAL", nullable: true),
                    Longitud = table.Column<double>(type: "REAL", nullable: true),
                    PendienteUbicacion = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fotos_observaciones_ObservacionId",
                        column: x => x.ObservacionId,
                        principalTable: "observaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comentarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FotoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Texto = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    FechaCreacion = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_comentarios_fotos_FotoId",
                        column: x => x.FotoId,
                        principalTable: "fotos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_FotoId",
                table: "comentarios",
                column: "FotoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_fotos_ObservacionId",
                table: "fotos",
                column: "ObservacionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comentarios");

            migrationBuilder.DropTable(
                name: "fotos");
        }
    }
}
