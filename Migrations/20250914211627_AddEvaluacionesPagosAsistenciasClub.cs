using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patinaje.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluacionesPagosAsistenciasClub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Profesores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Patinadores",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    AsistenciaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatinadorId = table.Column<int>(type: "int", nullable: false),
                    FechaClase = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Presente = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.AsistenciaId);
                    table.ForeignKey(
                        name: "FK_Asistencias_Patinadores_PatinadorId",
                        column: x => x.PatinadorId,
                        principalTable: "Patinadores",
                        principalColumn: "PatinadorId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clubes",
                columns: table => new
                {
                    ClubId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Direccion = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telefono = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubes", x => x.ClubId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    EvaluacionTecnicaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatinadorId = table.Column<int>(type: "int", nullable: false),
                    Elemento = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Puntaje = table.Column<int>(type: "int", nullable: false),
                    Observaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VideoUrl = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.EvaluacionTecnicaId);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Patinadores_PatinadorId",
                        column: x => x.PatinadorId,
                        principalTable: "Patinadores",
                        principalColumn: "PatinadorId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Pagos",
                columns: table => new
                {
                    PagoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PatinadorId = table.Column<int>(type: "int", nullable: false),
                    Concepto = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Monto = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Estado = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FechaVencimiento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FechaPago = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LinkComprobante = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pagos", x => x.PagoId);
                    table.ForeignKey(
                        name: "FK_Pagos_Patinadores_PatinadorId",
                        column: x => x.PatinadorId,
                        principalTable: "Patinadores",
                        principalColumn: "PatinadorId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_ClubId",
                table: "Profesores",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Patinadores_ClubId",
                table: "Patinadores",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_PatinadorId",
                table: "Asistencias",
                column: "PatinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_PatinadorId",
                table: "Evaluaciones",
                column: "PatinadorId");

            migrationBuilder.CreateIndex(
                name: "IX_Pagos_PatinadorId",
                table: "Pagos",
                column: "PatinadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patinadores_Clubes_ClubId",
                table: "Patinadores",
                column: "ClubId",
                principalTable: "Clubes",
                principalColumn: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Profesores_Clubes_ClubId",
                table: "Profesores",
                column: "ClubId",
                principalTable: "Clubes",
                principalColumn: "ClubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patinadores_Clubes_ClubId",
                table: "Patinadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Profesores_Clubes_ClubId",
                table: "Profesores");

            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "Clubes");

            migrationBuilder.DropTable(
                name: "Evaluaciones");

            migrationBuilder.DropTable(
                name: "Pagos");

            migrationBuilder.DropIndex(
                name: "IX_Profesores_ClubId",
                table: "Profesores");

            migrationBuilder.DropIndex(
                name: "IX_Patinadores_ClubId",
                table: "Patinadores");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Patinadores");
        }
    }
}
