using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using ContaBancaria.Autorizacoes.Api.Servicos.Dtos;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura
{
    public interface ITransacaoRepository
    {
        Task<Transacao> CriarNovaTransacao(NovaTransacaoDto novaTransacaoDto);
        Task DestruirTransacao(string chaveTransacao);
    }
}
