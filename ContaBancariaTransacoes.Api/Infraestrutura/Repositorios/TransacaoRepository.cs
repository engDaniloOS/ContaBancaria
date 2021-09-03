using ContaBancaria.Transacoes.Api.Infraestrutura.BancoDeDados;
using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Repositorios;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Infraestrutura.Repositorios
{
    public class TransacaoRepository : ITransacaoRepository
    {
        private readonly Contexto _contexto;
        public TransacaoRepository(Contexto contexto) => _contexto = contexto;

        public async Task<Transacao> ExecutaDeposito(Transacao transacao)
        {
            using (var dbTransaction = await _contexto.Database.BeginTransactionAsync())
            {
                try
                {
                    if (transacao.TaxaOrigem > 0)
                    {
                        var tarifa = new Tarifa
                        {
                            Transacao = transacao.Token,
                            Valor = transacao.Valor
                        };

                        await _contexto.Tarifas.AddAsync(tarifa);
                    }

                    var contaDestino = await _contexto.Contas.FirstOrDefaultAsync(conta =>
                                                        conta.Agencia == transacao.ContaDestino.Agencia &&
                                                        conta.Numero == transacao.ContaDestino.Numero &&
                                                        conta.IsAtiva);

                    contaDestino.Saldo += transacao.Valor;

                    transacao.SaldoDestino = contaDestino.Saldo;
                    await _contexto.Transacoes.AddAsync(transacao);

                    await _contexto.SaveChangesAsync();
                    dbTransaction.Commit();

                    return transacao;
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    throw;
                }
                finally
                {
                    dbTransaction.Dispose();
                }
            }

        }
    }
}
