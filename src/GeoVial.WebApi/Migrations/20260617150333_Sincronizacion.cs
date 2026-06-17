using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Sincronizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_marcadores_RelevamientoId",
                table: "marcadores");

            migrationBuilder.AddColumn<string>(
                name: "IdOrigen",
                table: "observaciones",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ActualizadoEn",
                table: "marcadores",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<string>(
                name: "IdOrigen",
                table: "marcadores",
                type: "TEXT",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "marcas_sincronizacion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RelevamientoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Valor = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    SubidaConcluida = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActualizadoEn = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_marcas_sincronizacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_marcas_sincronizacion_relevamientos_RelevamientoId",
                        column: x => x.RelevamientoId,
                        principalTable: "relevamientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_marcas_sincronizacion_usuarios_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_marcadores_RelevamientoId_ActualizadoEn",
                table: "marcadores",
                columns: new[] { "RelevamientoId", "ActualizadoEn" });

            migrationBuilder.CreateIndex(
                name: "IX_marcadores_RelevamientoId_IdOrigen",
                table: "marcadores",
                columns: new[] { "RelevamientoId", "IdOrigen" });

            migrationBuilder.CreateIndex(
                name: "IX_marcas_sincronizacion_ClienteId",
                table: "marcas_sincronizacion",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_marcas_sincronizacion_RelevamientoId_ClienteId",
                table: "marcas_sincronizacion",
                columns: new[] { "RelevamientoId", "ClienteId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "marcas_sincronizacion");

            migrationBuilder.DropIndex(
                name: "IX_marcadores_RelevamientoId_ActualizadoEn",
                table: "marcadores");

            migrationBuilder.DropIndex(
                name: "IX_marcadores_RelevamientoId_IdOrigen",
                table: "marcadores");

            migrationBuilder.DropColumn(
                name: "IdOrigen",
                table: "observaciones");

            migrationBuilder.DropColumn(
                name: "ActualizadoEn",
                table: "marcadores");

            migrationBuilder.DropColumn(
                name: "IdOrigen",
                table: "marcadores");

            migrationBuilder.CreateIndex(
                name: "IX_marcadores_RelevamientoId",
                table: "marcadores",
                column: "RelevamientoId");
        }
    }
}
