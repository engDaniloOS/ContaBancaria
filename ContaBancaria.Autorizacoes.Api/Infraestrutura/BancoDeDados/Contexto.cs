using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ContaBancaria.Autorizacoes.Api.Infraestrutura.BancoDeDados
{
    public class Contexto : DbContext
    {
        public Contexto(DbContextOptions options) : base(options) { }

        public DbSet<Sessao> Sessoes { get; set; }
        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Dispositivo> Dispositivos { get; set; }
        public DbSet<ClienteCredenciais> Credenciais { get; set; }
    }
}
