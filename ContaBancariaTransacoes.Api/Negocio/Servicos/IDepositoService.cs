using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Negocio.Servicos
{
    public interface IDepositoService
    {
        Task<Transacao> ExecutaDeposito(DepositoDto deposito, bool isCobrancaTaxa);
    }
}
