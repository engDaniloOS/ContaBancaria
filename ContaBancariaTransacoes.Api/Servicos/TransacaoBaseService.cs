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
    public abstract class TransacaoBaseService
    {
        protected readonly IConfiguration _configuration;
        protected readonly IControleTransacaoService _transacaoService;
        protected readonly IAutorizaTransacaoService _autorizacaoService;
        protected readonly ITransacaoRepository _transacaoRepository;

        protected TransacaoBaseService(IConfiguration configuration,
                                       IControleTransacaoService transacaoService,
                                       IAutorizaTransacaoService autorizacaoService,
                                       ITransacaoRepository transacaoRepository)
        {
            _configuration = configuration;
            _transacaoService = transacaoService;
            _autorizacaoService = autorizacaoService;
            _transacaoRepository = transacaoRepository;
        }

        protected void IsParametrosTransacaoValidos(BaseTransacaoDto baseTransacao)
        {
            if (baseTransacao.Sessao.Equals(Guid.Empty))
                throw new InvalidOperationException("Sessão inválida. Tente novamente!");

            if (baseTransacao.Valor <= 0)
                throw new InvalidOperationException("Deposito com valor menor ou igual a zero");

            if (baseTransacao.Agencia == 0 || baseTransacao.NumeroConta == 0)
                throw new InvalidOperationException("Nº de conta e/ou agência inválido(s)");
        }

        protected void IsCredenciaisValidas(string usuario, string senha)
        {
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
                throw new InvalidOperationException("Credênciais informadas em formato inválido");
        }

        protected void VerificaConcorrenciaTransacoes(long numeroConta, int maximoTentativas = 1)
        {
            var tentativa = 0;

            do
            {
                var existsConcorrencia = _transacaoService.VerificaConcorrencia(numeroConta);

                if (!existsConcorrencia)
                {
                    _transacaoService.TravaContaParaTransacao(numeroConta);
                    return;
                }

                Task.Delay(1000);

                tentativa++;

            } while (tentativa < maximoTentativas);

            throw new TimeoutException("Existem transações ocorrendo em paralelo. Tente novamente mais tarde.");
        }

        protected Transacao ConstroiTransacao(BaseTransacaoDto baseTransacao, TipoTransacao tipoTransacao, string transacaoToken, decimal taxaDeposito = 0)
        {
            var cnpjBancoOrigem = long.Parse(_configuration["Banco:CNPJ"]);

            var contaTransacionado = new Conta
            {
                Agencia = baseTransacao.Agencia,
                Banco = new Banco { CNPJ = cnpjBancoOrigem },
                Numero = baseTransacao.NumeroConta
            };

            var valor = (tipoTransacao == TipoTransacao.SAQUE) ? baseTransacao.Valor * (-1) : baseTransacao.Valor;

            return new Transacao
            {
                Token = Guid.Parse(transacaoToken),
                ContaDestino = contaTransacionado,
                ContaOrigem = null,
                Data = DateTime.UtcNow,
                Modalidade = new ModalidadeTransacao { Id = (int) tipoTransacao },
                TaxaOrigem = taxaDeposito,
                Valor = valor,
                Sessao = baseTransacao.Sessao
            };
        }

        protected void LiberaTransacoesDaConta(int numeroConta) => _transacaoService.LiberaContaParaTransacao(numeroConta);
    }
}
