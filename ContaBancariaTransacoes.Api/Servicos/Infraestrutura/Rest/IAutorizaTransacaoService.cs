using System;

namespace ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest
{
    public interface IAutorizaTransacaoService
    {
        string AutorizarDeposito(Guid sessao);
    }
}
