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
    public class ExtratoService : IExtratoService
    {
        private readonly IAutorizaTransacaoService _autorizacaoService;
        private readonly ITransacaoRepository _transacaoRepository;
        private readonly IConfiguration _configuration;

        public ExtratoService(IAutorizaTransacaoService autorizacaoService, ITransacaoRepository transacaoRepository, IConfiguration configuration)
        {
            _autorizacaoService = autorizacaoService;
            _transacaoRepository = transacaoRepository;
            _configuration = configuration;
        }

        public async Task<ExtratoResponseDto> GetExtrato(ExtratoRequestDto extratoDto)
        {
            try
            {
                var quantidadeDias = (int) extratoDto.DataFim.Subtract(extratoDto.DataInicio).TotalDays;

                if (quantidadeDias > 90)
                    throw new InvalidOperationException("O intervalo para o extrato não pode ser superior a 90 dias.");

                var transacaoToken = await _autorizacaoService.AutorizaTransacao(extratoDto.Sessao.ToString(), _configuration["Banco:Usuario"], _configuration["Banco:Senha"]);

                Dictionary<long, List<Transacao>> contaTransacao = await _transacaoRepository.GetTransacoes(extratoDto);

                return OrganizaTransacoes(contaTransacao);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(ex);
                return new ExtratoResponseDto
                {
                    Extrato = new List<TransacoesDto>(),
                    IsInvalid = true,
                    Mensagem = ex.Message
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new ExtratoResponseDto
                {
                    Extrato = new List<TransacoesDto>(),
                    IsInvalid = true,
                    Mensagem = $"Erro ao processar a requisição: {ex.Message}"
                };
            }
        }

        private ExtratoResponseDto OrganizaTransacoes(Dictionary<long, List<Transacao>> contaTransacao)
        {
            var listaTransacoes = new List<TransacoesDto>();

            var id = contaTransacao.Keys.FirstOrDefault();
            var transacoes = contaTransacao.Values.FirstOrDefault();

            foreach (var transacao in transacoes)
            {
                var isNotTransferencia = transacao.Modalidade.Id != (int)TipoTransacao.TRANSFERENCIA;
                var isContaDestino = transacao.ContaDestino.Id == id;

                var saldo = isNotTransferencia || isContaDestino ? transacao.SaldoDestino : transacao.SaldoOrigem;
                var taxa = isNotTransferencia || !isContaDestino ? transacao.TaxaOrigem : 0;

                var transacaoDto = new TransacoesDto
                {
                    Data = transacao.Data,
                    TipoTransacao = transacao.Modalidade.Nome,
                    AgenciaDestino = transacao.ContaDestino.Agencia,
                    NumeroContaDestino = transacao.ContaDestino.Numero,
                    AgenciaOrigem = transacao.ContaOrigem?.Agencia ?? 0,
                    NumeroContaOrigem = transacao.ContaOrigem?.Numero ?? 0,
                    Saldo = saldo,
                    Taxa = taxa,
                    Valor = transacao.Valor
                };

                listaTransacoes.Add(transacaoDto);
            }

            return new ExtratoResponseDto 
            { 
                Extrato = listaTransacoes.OrderByDescending(transacao => transacao.Data).ToList() 
            };
        }
    }
}
