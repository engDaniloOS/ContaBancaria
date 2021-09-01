using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Negocio.Servicos
{
    public interface ISessaoService
    {
        Task<Sessao> CriarSessao(string chaveDispositivo);
        Task<bool> DestruirSessao(string chaveSessao);
    }
}
