using ContaBancaria.Autorizacoes.Api.Infraestrutura.BancoDeDados;
using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Infraestrutura.Repositorios
{
    public class DispositivoRepository : IDispositivoRepository
    {
        private readonly Contexto _contexto;

        public DispositivoRepository(Contexto contexto) => _contexto = contexto;
        
        public async Task<Dispositivo> GetDispositivoAtivoByChave(string chaveDispositivo)
            => await _contexto.Dispositivos.Where(
                        dispositivo => (dispositivo.Chave == chaveDispositivo && dispositivo.IsAtivo))
                        .FirstOrDefaultAsync();
    }
}
