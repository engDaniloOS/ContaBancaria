using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using ContaBancaria.Autorizacoes.Api.Negocio.Servicos;
using ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura;
using System;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Servicos
{
    public class SessaoService : ISessaoService
    {
        private readonly IDispositivoRepository _dispositivoRepository;
        private readonly ISessaoRepository _sessaoRepository;

        public SessaoService(IDispositivoRepository dispositivoRepository, ISessaoRepository sessaoRepository)
        {
            _dispositivoRepository = dispositivoRepository;
            _sessaoRepository = sessaoRepository;
        }

        public async Task<Sessao> CriarSessao(string chaveDispositivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(chaveDispositivo))
                    throw new InvalidOperationException("Chave de dispositivo inválida");
                
                var dispositivo = await _dispositivoRepository.GetDispositivoAtivoByChave(chaveDispositivo);

                if (dispositivo == null)
                    return new Sessao();

                 return await _sessaoRepository.CriarNovaSessao(dispositivo);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Sessao { IsNotValido = true };
            }
        }

        public async Task<bool> DestruirSessao(string chaveSessao)
        {
            try
            {
                await _sessaoRepository.DestruirSessao(chaveSessao);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}
