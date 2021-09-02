using ContaBancaria.Autorizacoes.Api.Infraestrutura.BancoDeDados;
using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Infraestrutura.Repositorios
{
    public class SessaoRepository : ISessaoRepository
    {
        private readonly Contexto _contexto;

        public SessaoRepository(Contexto contexto) => _contexto = contexto;

        public async Task<Sessao> CriarNovaSessao(Dispositivo dispositivo)
        {
            var novaSessao = new Sessao
            {
                DataCriacao = DateTime.UtcNow,
                IsAtivo = true,
                IsNotValido = false,
                Token = Guid.NewGuid(),
                Dispositivo = dispositivo
            };

            await _contexto.Sessoes.AddAsync(novaSessao);
            await _contexto.SaveChangesAsync();

            return novaSessao;
        }

        public async  Task DestruirSessao(string chaveSessao)
        {
            var tokenSessao = Guid.Parse(chaveSessao);

            var sessao = await _contexto.Sessoes.Where(
                                _sessao => _sessao.Token.Equals(tokenSessao))
                                .FirstOrDefaultAsync();

            sessao.IsAtivo = false;

            await _contexto.SaveChangesAsync();
        }
    }
}
