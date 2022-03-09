using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace peliculasApi.Migrations
{
    public partial class FixedPelicula : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaLanzamienti",
                table: "Peliculas");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLanzamiento",
                table: "Peliculas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaLanzamiento",
                table: "Peliculas");

            migrationBuilder.AddColumn<bool>(
                name: "FechaLanzamienti",
                table: "Peliculas",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
