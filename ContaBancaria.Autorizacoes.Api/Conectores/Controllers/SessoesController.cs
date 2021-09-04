using ContaBancaria.Autorizacoes.Api.Negocio.Servicos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Conectores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessoesController : ControllerBase
    {
        private readonly string URL_TRANSACOES;
        private readonly ISessaoService _sessaoService;

        public SessoesController(ISessaoService sessaoService, IConfiguration configuration)
        {
            _sessaoService = sessaoService;
            URL_TRANSACOES = configuration["Transacoes.URL"];
        }

        [HttpPost]
        public async Task<IActionResult> CriarSessao(string chaveDispositivo)
        {
            var sessao = await _sessaoService.CriarSessao(chaveDispositivo);

            if (sessao.IsNotValido)
                return BadRequest("Erro ao gerar a sessão");

            else if (string.IsNullOrWhiteSpace(sessao.Dispositivo.Chave))
                return BadRequest("Dispositivo não cadastrado ou não autorizado para iniciar a sessão");

            return Ok(new
            {
                sessao.Token,
                sessao.Dispositivo,
            });
        }

        [HttpDelete]
        public async Task<IActionResult> FinalizarSessao(string chaveSessao)
        {
            if (await _sessaoService.DestruirSessao(chaveSessao))
                return Ok();

            return BadRequest("Não foi possível finalizar a sessão");
        }
    }
}
