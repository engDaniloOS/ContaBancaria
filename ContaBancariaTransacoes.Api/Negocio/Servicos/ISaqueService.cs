using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Negocio.Servicos
{
    public interface ISaqueService
    {
        Task<Transacao> ExecutaSaque(SaqueDto saque, bool isCobrancaTaxa);
    }
}
