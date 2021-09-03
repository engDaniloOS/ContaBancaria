using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Negocio.Servicos
{
    public interface ITransferenciaService
    {
        Task<Transacao> ExecutaTransferencia(TransferenciaDto transferencia, bool isCobrancaTaxa);
    }
}
