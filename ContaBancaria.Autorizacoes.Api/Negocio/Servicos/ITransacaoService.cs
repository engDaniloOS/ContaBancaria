using ContaBancaria.Autorizacoes.Api.Conectores.Controllers.Dtos;
using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Negocio.Servicos
{
    public interface ITransacaoService
    {
        Task<Transacao> CriarTransacao(NovaTransacaoDto novaTransacaoDto);
        Task<bool> DestruirTransacao(string chaveTransacao);
    }
}
