using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Repositorios
{
    public interface ITransacaoRepository
    {
        Task<Transacao> MovimentaSaldo(Transacao transacao);

        Task<decimal> GetSaldo(int numeroConta, short agencia);
        Task<Transacao> RealizaTransferencia(Transacao transacao);
        Task<Dictionary<long, List<Transacao>>> GetTransacoes(ExtratoRequestDto extratoDto);
    }
}
