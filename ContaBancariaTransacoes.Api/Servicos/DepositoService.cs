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
    public class DepositoService : IDepositoService
    {
        private readonly IConfiguration _configuration;
        private readonly IControleTransacaoService _transacaoService;
        private readonly IAutorizaTransacaoService _autorizacaoService;
        private readonly ITransacaoRepository _transacaoRepository;

        public DepositoService(IConfiguration configuration,
                               IControleTransacaoService transacaoService,
                               IAutorizaTransacaoService autorizacaoService,
                               ITransacaoRepository transacaoRepository)
        {
            _configuration = configuration;
            _transacaoService = transacaoService;
            _autorizacaoService = autorizacaoService;
            _transacaoRepository = transacaoRepository;
        }

        public async Task<Transacao> ExecutaDeposito(DepositoDto deposito, bool isCobrancaTaxa = false)
        {
            var taxaProporcionalDeposito = decimal.Parse(_configuration["Deposito.Taxa"]);
            var taxaDeposito = 0m;
            
            try
            {
                IsParametrosDepositoValidos(deposito);

                if (isCobrancaTaxa)
                    taxaDeposito = deposito.Valor * taxaProporcionalDeposito;

                deposito.Valor -= taxaDeposito;

                VerificaConcorrenciaTransacoes(deposito.NumeroConta);

                var transacaoToken = _autorizacaoService.AutorizarDeposito(deposito.Sessao);

                var transacao = ConstroiTransacao(deposito, taxaDeposito, transacaoToken);

                return await _transacaoRepository.ExecutaDeposito(transacao);
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
                _transacaoService.LiberaContaParaTransacao(deposito.NumeroConta);
            }
        }

        private void IsParametrosDepositoValidos(DepositoDto deposito)
        {
            if (deposito.Sessao.Equals(Guid.Empty))
                throw new InvalidOperationException("Sessão inválida. Tente novamente!");

            if (deposito.Valor <= 0)
                throw new InvalidOperationException("Deposito com valor menor ou igual a zero");

            if (deposito.BancoCnpj == 0)
                throw new InvalidOperationException("Banco informado inválid");

            if (deposito.Agencia == 0 || deposito.NumeroConta == 0)
                throw new InvalidOperationException("Nº de conta e/ou agência inválido(s)");
        }

        private void VerificaConcorrenciaTransacoes(long numeroConta)
        {
            var tentativa = 0;
            var maximoTentativasDeposito = int.Parse(_configuration["Deposito.MaximoTentativas"]);
            do
            {
                var existsConcorrencia = _transacaoService.VerificaConcorrencia(numeroConta);

                if (!existsConcorrencia)
                {
                    _transacaoService.TravaContaParaTransacao(numeroConta);
                    break;
                }

                Task.Delay(1000);

                tentativa++;

            } while (tentativa < maximoTentativasDeposito);

            throw new TimeoutException("Existem transações ocorrendo em paralelo. Tente novamente mais tarde.");
        }
    
        private Transacao ConstroiTransacao(DepositoDto deposito, decimal taxaDeposito, string transacaoToken)
        {
            var banco = new Banco { CNPJ = long.Parse(_configuration["Banco.CNPJ"])  };
            var modalidade = new ModalidadeTransacao { Id = (int) TipoTransacao.DEPOSITO };

            var contaDestino = new Conta
            {
                Agencia = deposito.Agencia,
                Banco = banco,
                Numero = deposito.NumeroConta,
            };

            return new Transacao
            {
                Token = Guid.Parse(transacaoToken),
                ContaDestino = contaDestino,
                ContaOrigem = null,
                Data = DateTime.UtcNow,
                Modalidade = modalidade,
                TaxaOrigem = taxaDeposito,
                Valor = deposito.Valor,
                Sessao = deposito.Sessao
            };
        }
    }
}
