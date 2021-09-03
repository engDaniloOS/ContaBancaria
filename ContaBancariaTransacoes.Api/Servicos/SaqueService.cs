using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Enumeradores;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Repositorios;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var taxaFixaSaque = decimal.Parse(_configuration["Saque:Taxa"]);

                IsParametrosTransacaoValidos(saque.BaseTransacao);
                IsCredenciaisValidas(saque.Usuario, saque.Senha);

                if (isCobrancaTaxa)
                    taxaSaque = taxaFixaSaque;

                await IsSaldoSuficiente(saque, taxaSaque);

                var maximoTentativasSaque = int.Parse(_configuration["Saque:MaximoTentativas"]);
                VerificaConcorrenciaTransacoes(saque.BaseTransacao.NumeroConta, maximoTentativasSaque);

                var transacaoToken = await _autorizacaoService.AutorizarTransacao(saque.BaseTransacao.Sessao.ToString(), saque.Usuario, saque.Senha);

                var transacao = ConstroiTransacao(saque.BaseTransacao, TipoTransacao.SAQUE, transacaoToken.ToString(), taxaSaque);

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

        private async Task IsSaldoSuficiente(SaqueDto saque, decimal taxaSaque)
        {
            try
            {
                var saldoContaAtual = await _transacaoRepository.GetSaldo(saque.BaseTransacao.NumeroConta, saque.BaseTransacao.Agencia);

                if (saldoContaAtual < (taxaSaque + saque.BaseTransacao.Valor))
                    throw new InvalidOperationException("Não há saldo suficiente para realizar a transação");
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex);
                throw new InvalidOperationException("Não foi possível encontrar a conta/Agência informada");
            }
        }
    }
}
