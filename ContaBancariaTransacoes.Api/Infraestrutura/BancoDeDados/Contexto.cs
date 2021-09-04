using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ContaBancaria.Transacoes.Api.Infraestrutura.BancoDeDados
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions options) : base(options) { }

        public DbSet<Banco> Bancos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<Credenciais> Credenciais { get; set; }
        public DbSet<ModalidadeConta> ModalidadesConta { get; set; }
        public DbSet<ModalidadeTransacao> ModalidadesTransacao { get; set; }
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Tarifa> Tarifas { get; set; }
    }
}
