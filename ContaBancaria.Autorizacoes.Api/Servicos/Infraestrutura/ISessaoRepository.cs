using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura
{
    public interface ISessaoRepository
    {
        Task<Sessao> CriarNovaSessao(Dispositivo dispositivo);
        Task DestruirSessao(string chaveSessao);
    }
}
