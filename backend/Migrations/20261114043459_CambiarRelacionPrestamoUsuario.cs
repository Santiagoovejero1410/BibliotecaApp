using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BibliotecaApi.Migrations
{
    /// <inheritdoc />
    public partial class CambiarRelacionPrestamoUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_AspNetUsers_UsuarioId",
                table: "Prestamos");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioId",
                table: "Prestamos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prestamos_Usuarios_UsuarioId",
                table: "Prestamos");

            migrationBuilder.AddForeignKey(
                name: "FK_Prestamos_AspNetUsers_UsuarioId",
                table: "Prestamos",
                column: "UsuarioId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
