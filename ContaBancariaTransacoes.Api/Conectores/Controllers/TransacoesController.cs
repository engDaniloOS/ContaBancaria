using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Conectores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacoesController : ControllerBase
    {
        private readonly IDepositoService _depositoService;
        private readonly ISaqueService _saqueService;
        private readonly ITransferenciaService _transferenciaService;
        private readonly IExtratoService _extratoService;

        public TransacoesController(IDepositoService depositoService,
                                    ISaqueService saqueService,
                                    ITransferenciaService transferenciaService,
                                    IExtratoService extratoService)
        {
            _depositoService = depositoService;
            _saqueService = saqueService;
            _transferenciaService = transferenciaService;
            _extratoService = extratoService;
        }

        [HttpGet]
        [Route("extrato")]
        public async Task<IActionResult> RecuperaExtrato([FromQuery] ExtratoRequestDto requisicaoExtrato)
        {
            var extrato = await _extratoService.GetExtrato(requisicaoExtrato);

            if (extrato.IsInvalid)
                return BadRequest(extrato.Mensagem);

            return Ok(extrato);
        }

        [HttpPost]
        [Route("deposito")]
        public async Task<IActionResult> ExecutaDeposito([FromBody] BaseTransacaoDto deposito)
        {
            var transacao = await _depositoService.ExecutaDeposito(deposito, true);

            if (transacao.IsInvalida)
                return BadRequest(transacao.Mensagem);

            return Ok(new
            {
                Conta = transacao.ContaDestino.Numero,
                transacao.ContaDestino.Agencia,
                transacao.Data,
                transacao.Modalidade,
                transacao.Valor,
                Taxa = transacao.TaxaOrigem,
                transacao.Sessao,
                Transacao = transacao.Token
            });
        }

        [HttpPost]
        [Route("saque")]
        public async Task<IActionResult> ExecutaSaque([FromBody] SaqueDto saque)
        {
            var transacao = await _saqueService.ExecutaSaque(saque, true);

            if (transacao.IsInvalida)
                return BadRequest(transacao.Mensagem);

            return Ok(new
            {
                Conta = transacao.ContaDestino.Numero,
                transacao.ContaDestino.Agencia,
                transacao.Data,
                transacao.Modalidade,
                transacao.Valor,
                transacao.SaldoDestino,
                Taxa = transacao.TaxaOrigem,
                transacao.Sessao,
                Transacao = transacao.Token
            });
        }

        [HttpPost]
        [Route("transferencia")]
        public async Task<IActionResult> ExecutaTransferencia([FromBody] TransferenciaDto transferencia)
        {
            var transacao = await _transferenciaService.ExecutaTransferencia(transferencia, true);

            if (transacao.IsInvalida)
                return BadRequest(transacao.Mensagem);

            return Ok(new
            {
                ContaOrigem = transacao.ContaOrigem.Numero,
                AgenciaOrigem = transacao.ContaOrigem.Agencia,
                ContaDestino = transacao.ContaDestino.Numero,
                AgenciaDestino = transacao.ContaDestino.Agencia,
                BancoDestino = transacao.ContaDestino.Banco.Nome,
                transacao.Valor,
                transacao.TaxaOrigem,
                transacao.Data,
                transacao.Sessao,
                Transacao = transacao.Token
            });
        }
    }
}
