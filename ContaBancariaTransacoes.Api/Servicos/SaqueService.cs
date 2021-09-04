using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Enumeradores;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Repositorios;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Servicos
{
    public class SaqueService : TransacaoBaseService, ISaqueService
    {
        public SaqueService(IConfiguration configuration,
                       IControleTransacaoService transacaoService,
                       IAutorizaTransacaoService autorizacaoService,
                       ITransacaoRepository transacaoRepository)
                       : base(configuration, transacaoService, autorizacaoService, transacaoRepository)
        {
        }

        public async Task<Transacao> ExecutaSaque(SaqueDto saque, bool isCobrancaTaxa)
        {
            var taxaSaque = 0m;

            try
            {
                var baseTransacao = saque.BaseTransacao;
                var taxaFixaSaque = decimal.Parse(_configuration["Saque:Taxa"]);

                IsParametrosTransacaoValidos(baseTransacao);
                IsCredenciaisValidas(saque.Usuario, saque.Senha);

                if (isCobrancaTaxa)
                    taxaSaque = taxaFixaSaque;

                await IsSaldoSuficiente(baseTransacao.NumeroConta, baseTransacao.Agencia, baseTransacao.Valor, taxaSaque);

                var maximoTentativasSaque = int.Parse(_configuration["Saque:MaximoTentativas"]);
                VerificaConcorrenciaTransacoes(baseTransacao.NumeroConta, maximoTentativasSaque);

                var transacaoToken = await _autorizacaoService.AutorizaTransacao(baseTransacao.Sessao.ToString(), saque.Usuario, saque.Senha);

                var transacao = ConstroiTransacao(baseTransacao, TipoTransacao.SAQUE, transacaoToken.ToString(), taxaSaque);

                return await _transacaoRepository.MovimentaSaldo(transacao);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Transacao
                {
                    IsInvalida = true,
                    Mensagem = ex.Message
                };
            }
            finally
            {
                LiberaTransacoesDaConta(saque.BaseTransacao.NumeroConta);
            }
        }
    }
}
