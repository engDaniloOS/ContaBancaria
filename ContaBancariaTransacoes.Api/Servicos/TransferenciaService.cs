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
    public class TransferenciaService : TransacaoBaseService, ITransferenciaService
    {

        public TransferenciaService(IConfiguration configuration,
               IControleTransacaoService transacaoService,
               IAutorizaTransacaoService autorizacaoService,
               ITransacaoRepository transacaoRepository)
               : base(configuration, transacaoService, autorizacaoService, transacaoRepository)
        {
        }

        public async Task<Transacao> ExecutaTransferencia(TransferenciaDto transferencia, bool isCobrancaTaxa)
        {
            var taxaTransferencia = 0m;

            try
            {
                var taxaFixaTransferencia = decimal.Parse(_configuration["Transferencia:Taxa"]);

                ValidaParametros(transferencia);

                if (isCobrancaTaxa)
                    taxaTransferencia = taxaFixaTransferencia;

                await IsSaldoSuficiente(transferencia.NumeroContaOrigem, transferencia.AgenciaOrigem, transferencia.Valor, taxaTransferencia);

                var maximoTentativasSaque = int.Parse(_configuration["Transferencia:MaximoTentativas"]);
                VerificaConcorrenciaTransacoes(transferencia.NumeroContaOrigem, maximoTentativasSaque);
                VerificaConcorrenciaTransacoes(transferencia.NumeroContaDestino, maximoTentativasSaque);

                var transacaoToken = await _autorizacaoService.AutorizaTransacao(transferencia.Sessao.ToString(), transferencia.Usuario, transferencia.Senha);

                var transacao = ConstroiTransacao(transferencia, transacaoToken, taxaTransferencia);

                return await _transacaoRepository.RealizaTransferencia(transacao);
            }
            catch (NullReferenceException ex)
            {
                Console.WriteLine(ex);
                return new Transacao
                {
                    IsInvalida = true,
                    Mensagem = $"Não foi possível encontrar a(s) conta(s)/agência(s) informada(s): {ex.Message}"
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
                LiberaTransacoesDaConta(transferencia.NumeroContaOrigem);
                LiberaTransacoesDaConta(transferencia.NumeroContaDestino);
            }
        }

        private void ValidaParametros(TransferenciaDto transferencia)
        {
            var baseValidacao = new BaseTransacaoDto
            {
                Agencia = transferencia.AgenciaOrigem,
                NumeroConta = transferencia.NumeroContaOrigem,
                Sessao = transferencia.Sessao,
                Valor = transferencia.Valor
            };
            IsParametrosTransacaoValidos(baseValidacao);

            baseValidacao.Agencia = transferencia.AgenciaDestino;
            baseValidacao.NumeroConta = transferencia.NumeroContaDestino;
            IsParametrosTransacaoValidos(baseValidacao);

            IsCredenciaisValidas(transferencia.Usuario, transferencia.Senha);

            if (transferencia.CNPJBancoDestino == 0)
                throw new InvalidOperationException("O CNPJ do banco destino esta inválido");
        }

        private Transacao ConstroiTransacao(TransferenciaDto transferencia, Guid transacaoToken, decimal taxaTransferencia = 0)
        {
            var cnpjBancoOrigem = long.Parse(_configuration["Banco:CNPJ"]);

            var modalidade = new ModalidadeTransacao { Id = (int)TipoTransacao.TRANSFERENCIA };

            var contaOrigem = new Conta
            {
                Numero = transferencia.NumeroContaOrigem,
                Agencia = transferencia.AgenciaOrigem,
                Banco = new Banco { CNPJ = cnpjBancoOrigem }
            };

            var contaDestino = new Conta
            {
                Numero = transferencia.NumeroContaDestino,
                Agencia = transferencia.AgenciaDestino,
                Banco = new Banco { CNPJ = transferencia.CNPJBancoDestino }
            };

            return new Transacao
            {
                ContaOrigem = contaOrigem,
                ContaDestino = contaDestino,
                Data = DateTime.UtcNow,
                Modalidade = modalidade,
                Sessao = transferencia.Sessao,
                TaxaOrigem = taxaTransferencia,
                Valor = transferencia.Valor,
                Token = transacaoToken
            };
        }
    }
}
