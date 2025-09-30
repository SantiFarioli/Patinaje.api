using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Patinaje.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDniDomicilioYFoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Dni",
                table: "Tutores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Domicilio",
                table: "Tutores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Dni",
                table: "Profesores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Domicilio",
                table: "Profesores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Dni",
                table: "Patinadores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Domicilio",
                table: "Patinadores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "Patinadores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "Domicilio",
                table: "Tutores");

            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "Domicilio",
                table: "Profesores");

            migrationBuilder.DropColumn(
                name: "Dni",
                table: "Patinadores");

            migrationBuilder.DropColumn(
                name: "Domicilio",
                table: "Patinadores");

            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "Patinadores");
        }
    }
}
