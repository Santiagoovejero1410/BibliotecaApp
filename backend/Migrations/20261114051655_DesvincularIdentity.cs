
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaApi.Migrations
{
    public partial class DesvincularIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No eliminamos las tablas antiguas de ASP.NET Identity.
            // Se conservan para mantener sus datos históricos.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No hay cambios que revertir.
        }
    }
}
