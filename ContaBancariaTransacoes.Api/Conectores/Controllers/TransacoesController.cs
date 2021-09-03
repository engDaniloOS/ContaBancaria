using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Conectores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacoesController : ControllerBase
    {
        private readonly IDepositoService _depositoService;
        public TransacoesController(IDepositoService depositoService)
        {
            _depositoService = depositoService;
        }

        [HttpPost]
        [Route("deposito")]
        public async Task<IActionResult> ExecutaDeposito([FromBody] DepositoDto deposito)
        {
            var transacao = await _depositoService.ExecutaDeposito(deposito, true);

            if (transacao.IsInvalida)
                return BadRequest(transacao.Mensagem);

            return Ok(transacao);
        }
    }
}
