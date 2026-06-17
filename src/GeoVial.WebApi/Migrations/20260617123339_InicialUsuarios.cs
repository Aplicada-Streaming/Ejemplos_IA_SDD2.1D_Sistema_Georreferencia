using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GeoVial.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class InicialUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NombreUsuario = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    HashContrasena = table.Column<string>(type: "TEXT", nullable: false),
                    Rol = table.Column<int>(type: "INTEGER", nullable: false),
                    IdAdministrador = table.Column<Guid>(type: "TEXT", nullable: true),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false),
                    FechaAlta = table.Column<DateTimeOffset>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_usuarios_usuarios_IdAdministrador",
                        column: x => x.IdAdministrador,
                        principalTable: "usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_IdAdministrador",
                table: "usuarios",
                column: "IdAdministrador");

            migrationBuilder.CreateIndex(
                name: "IX_usuarios_NombreUsuario",
                table: "usuarios",
                column: "NombreUsuario",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuarios");
        }
    }
}
