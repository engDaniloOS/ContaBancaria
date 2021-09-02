using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using ContaBancaria.Autorizacoes.Api.Servicos.Dtos;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Negocio.Servicos
{
    public interface ITransacaoService
    {
        Task<Transacao> CriarTransacao(NovaTransacaoDto novaTransacaoDto);
        Task<bool> DestruirTransacao(string chaveTransacao);
    }
}
