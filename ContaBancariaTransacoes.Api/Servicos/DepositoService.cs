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
    public class DepositoService : TransacaoBaseService, IDepositoService
    {
        public DepositoService(IConfiguration configuration,
                               IControleTransacaoService transacaoService,
                               IAutorizaTransacaoService autorizacaoService,
                               ITransacaoRepository transacaoRepository)
                               : base(configuration, transacaoService, autorizacaoService, transacaoRepository)
        {
        }

        public async Task<Transacao> ExecutaDeposito(BaseTransacaoDto deposito, bool isCobrancaTaxa = false)
        {
            var taxaDeposito = 0m;
            
            try
            {
                var taxaProporcionalDeposito = decimal.Parse(_configuration["Deposito:Taxa"]);

                IsParametrosTransacaoValidos(deposito);

                if (isCobrancaTaxa)
                    taxaDeposito = deposito.Valor * taxaProporcionalDeposito;

                var maximoTentativasDeposito = int.Parse(_configuration["Deposito:MaximoTentativas"]);
                VerificaConcorrenciaTransacoes(deposito.NumeroConta, maximoTentativasDeposito);

                var transacaoToken = await _autorizacaoService.AutorizarTransacao(deposito.Sessao.ToString(), _configuration["Banco:Usuario"], _configuration["Banco:Senha"]);

                var transacao = ConstroiTransacao(deposito, TipoTransacao.DEPOSITO, transacaoToken.ToString(), taxaDeposito);

                return await _transacaoRepository.MovimentaSaldo(transacao);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex);
                return new Transacao
                {
                    IsInvalida = true,
                    Mensagem = $"Não foi possível encontrar a conta/Agência informada: {ex.Message}"
                };
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
                LiberaTransacoesDaConta(deposito.NumeroConta);
            }
        }
    }
}
