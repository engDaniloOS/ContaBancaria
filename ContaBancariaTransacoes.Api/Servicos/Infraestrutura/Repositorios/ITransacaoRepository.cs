using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Repositorios
{
    public interface ITransacaoRepository
    {
        Task<Transacao> ExecutaDeposito(Transacao transacao);
    }
}
