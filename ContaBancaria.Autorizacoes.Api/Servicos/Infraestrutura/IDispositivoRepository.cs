using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura
{
    public interface IDispositivoRepository
    {
        Task<Dispositivo> GetDispositivoAtivoByChave(string chaveDispositivo);
    }
}
