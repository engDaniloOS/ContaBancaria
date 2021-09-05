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

        public async Task<decimal> GetSaldo(int numeroConta, short agencia)
        {
            return await (from conta in _contexto.Contas
                          where conta.Agencia == agencia && 
                          conta.Numero == numeroConta && 
                          conta.IsAtiva
                          select conta.Saldo).FirstOrDefaultAsync();
        }

        public async Task<Dictionary<long, List<Transacao>>> GetTransacoes(ExtratoRequestDto extratoDto)
        {
            var contaId = await (from conta in _contexto.Contas
                                 where conta.Numero == extratoDto.NumeroConta && 
                                 conta.Agencia == extratoDto.Agencia && 
                                 conta.IsAtiva
                                 select conta.Id).FirstOrDefaultAsync();

            var transacoes = await _contexto.Transacoes
                                        .Where(transacao => 
                                            ((transacao.ContaOrigem.Id == contaId) || (transacao.ContaDestino.Id == contaId)) &&
                                            ((transacao.Data >= extratoDto.DataInicio && transacao.Data <= extratoDto.DataFim)))
                                        .Include(transacao => transacao.ContaDestino)
                                        .Include(transacao => transacao.ContaOrigem)
                                        .Include(transacao => transacao.Modalidade)
                                        .ToListAsync();

            return new Dictionary<long, List<Transacao>>(){{ contaId, transacoes }};
        }

        public async Task<Transacao> MovimentaSaldo(Transacao transacao)
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
                            Valor = transacao.TaxaOrigem
                        };

                        await _contexto.Tarifas.AddAsync(tarifa);
                    }

                    var conta = await _contexto.Contas.FirstOrDefaultAsync(_conta =>
                                                        _conta.Agencia == transacao.ContaDestino.Agencia &&
                                                        _conta.Numero == transacao.ContaDestino.Numero &&
                                                        _conta.IsAtiva);

                    conta.Saldo += (transacao.Valor - transacao.TaxaOrigem);
                    _contexto.Contas.Update(conta);

                    var modalidade = await _contexto.ModalidadesTransacao.FindAsync(transacao.Modalidade.Id);

                    transacao.SaldoDestino = conta.Saldo;
                    transacao.ContaDestino = conta;
                    transacao.Modalidade = modalidade;
                    await _contexto.Transacoes.AddAsync(transacao);

                    await _contexto.SaveChangesAsync();
                    dbTransaction.Commit();

                    return transacao;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    dbTransaction.Rollback();
                    throw;
                }
                finally
                {
                    dbTransaction.Dispose();
                }
            }
        }

        public async Task<Transacao> RealizaTransferencia(Transacao transacao)
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
                            Valor = transacao.TaxaOrigem
                        };

                        await _contexto.Tarifas.AddAsync(tarifa);
                    }

                    var contaOrigem = await _contexto.Contas.FirstOrDefaultAsync(_conta =>
                                                        _conta.Agencia == transacao.ContaOrigem.Agencia &&
                                                        _conta.Numero == transacao.ContaOrigem.Numero &&
                                                        _conta.IsAtiva);


                    contaOrigem.Saldo -= (transacao.Valor + transacao.TaxaOrigem);
                    _contexto.Contas.Update(contaOrigem);


                    var bancoDestino = await _contexto.Bancos.FirstOrDefaultAsync(_banco =>
                                                        _banco.CNPJ == transacao.ContaDestino.Banco.CNPJ);

                    var contaDestino = await _contexto.Contas.FirstOrDefaultAsync(_conta =>
                                                        _conta.Agencia == transacao.ContaDestino.Agencia &&
                                                        _conta.Numero == transacao.ContaDestino.Numero &&
                                                        _conta.Banco.Id == bancoDestino.Id &&
                                                        _conta.IsAtiva);

                    contaDestino.Saldo += transacao.Valor;
                    _contexto.Contas.Update(contaDestino);

                    var modalidade = await _contexto.ModalidadesTransacao.FindAsync(transacao.Modalidade.Id);

                    transacao.SaldoOrigem = contaOrigem.Saldo;
                    transacao.SaldoDestino = contaDestino.Saldo;
                    transacao.ContaOrigem = contaOrigem;
                    transacao.ContaDestino = contaDestino;
                    transacao.Modalidade = modalidade;
                    await _contexto.Transacoes.AddAsync(transacao);

                    await _contexto.SaveChangesAsync();
                    dbTransaction.Commit();

                    return transacao;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
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
