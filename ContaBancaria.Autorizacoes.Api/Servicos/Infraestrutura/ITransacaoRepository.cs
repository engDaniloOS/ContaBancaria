using ContaBancaria.Autorizacoes.Api.Conectores.Controllers.Dtos;
using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura
{
    public interface ITransacaoRepository
    {
        Task<Transacao> CriarNovaTransacao(NovaTransacaoDto novaTransacaoDto);
        Task DestruirTransacao(string chaveTransacao);
    }
}
