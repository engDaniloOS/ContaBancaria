using ContaBancaria.Autorizacoes.Api.Conectores.Controllers.Dtos;
using ContaBancaria.Autorizacoes.Api.Negocio.Servicos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Conectores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransacaoController : ControllerBase
    {
        private readonly ITransacaoService _transacaoService;

        public TransacaoController(ITransacaoService transacaoService) => _transacaoService = transacaoService;

        [HttpPost]
        public async Task<IActionResult> CriarTransacao([FromBody] NovaTransacaoDto novaTransacaoDto)
        {
            var transacao = await _transacaoService.CriarTransacao(novaTransacaoDto);

            if (transacao.IsNotValido)
                return BadRequest("Problemas ao autenticar o usuário. Verifique as credenciais e tente novamente!");

            else if (string.IsNullOrWhiteSpace(transacao.Sessao?.Token.ToString()))
                return BadRequest("Erro ao criar nova transação");

            return Ok(transacao);
        }

        [HttpDelete]
        public async Task<IActionResult> FinalizarTransacao(string chaveTransacao)
        {
            if (await _transacaoService.DestruirTransacao(chaveTransacao))
                return Ok();

            return BadRequest("Não foi possível finalizar a transação!");
        }
    }
}
