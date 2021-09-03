using System;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest
{
    public interface IAutorizaTransacaoService
    {
        Task<Guid> AutorizarTransacao(string sessao, string usuario, string senha);
    }
}
