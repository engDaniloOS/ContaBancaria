using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Negocio.Servicos
{
    public interface IExtratoService
    {
        Task<ExtratoResponseDto> GetExtrato(ExtratoRequestDto extratoDto);
    }
}
