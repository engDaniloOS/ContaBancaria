using ContaBancaria.Autorizacoes.Api.Infraestrutura.BancoDeDados;
using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using ContaBancaria.Autorizacoes.Api.Servicos.Dtos;
using ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Infraestrutura.Repositorios
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly Contexto _contexto;

        public TransacaoRepository(Contexto contexto)
        {
            _contexto = contexto;                
        }

        public async Task<Transacao> CriarNovaTransacao(NovaTransacaoDto novaTransacaoDto)
        {
            var credenciais = await _contexto.Credenciais
                                        .FirstOrDefaultAsync(
                                            _credenciais => _credenciais.Usuario.Equals(novaTransacaoDto.Usuario) && 
                                                            _credenciais.IsAtivo && 
                                                            _credenciais.Senha.Equals(novaTransacaoDto.Senha));

            if (string.IsNullOrWhiteSpace(credenciais?.Usuario))
                throw new NotSupportedException("Credenciais inválidas");

            var tokenSessao = Guid.Parse(novaTransacaoDto.ChaveSessao);

            var sessao = await _contexto.Sessoes
                                .FirstOrDefaultAsync(_sessao => _sessao.Token.Equals(tokenSessao) && _sessao.IsAtivo);

            if (string.IsNullOrWhiteSpace(sessao.Token.ToString()))
                throw new Exception("Sessão inválida");

            var novaTransacao = new Transacao
            {
                DataCriacao = DateTime.UtcNow,
                IsAtivo = true,
                IsNotValido = false,
                Token = Guid.NewGuid(),
                Sessao = sessao,
                ClienteCredenciais = credenciais
            };

            await _contexto.Transacoes.AddAsync(novaTransacao);
            await _contexto.SaveChangesAsync();

            return novaTransacao;
        }

        public async Task DestruirTransacao(string chaveTransacao)
        {
            var tokenTransacao = Guid.Parse(chaveTransacao);

            var transacao = await _contexto.Transacoes
                                            .FirstOrDefaultAsync(_transacao => _transacao.Token.Equals(tokenTransacao));

            transacao.IsAtivo = false;

            await _contexto.SaveChangesAsync();

        }
    }
}
