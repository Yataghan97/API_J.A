using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleAcessoApi.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusCadastroToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StatusCadastro",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusCadastro",
                table: "Usuarios");
        }
    }
}
