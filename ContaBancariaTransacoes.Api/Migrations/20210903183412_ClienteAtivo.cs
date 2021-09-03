using Microsoft.EntityFrameworkCore.Migrations;

namespace ContaBancaria.Transacoes.Api.Migrations
{
    public partial class ClienteAtivo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAtivo",
                table: "Clientes",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAtivo",
                table: "Clientes");
        }
    }
}
