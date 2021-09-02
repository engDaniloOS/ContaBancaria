using ContaBancaria.Autorizacoes.Api.Negocio.Modelos;
using ContaBancaria.Autorizacoes.Api.Negocio.Servicos;
using ContaBancaria.Autorizacoes.Api.Servicos.Dtos;
using ContaBancaria.Autorizacoes.Api.Servicos.Infraestrutura;
using System;
using System.Threading.Tasks;

namespace ContaBancaria.Autorizacoes.Api.Servicos
{
    public class TransacaoService : ITransacaoService
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public TransacaoService(ITransacaoRepository transacaoRepository) => _transacaoRepository = transacaoRepository;

        public async Task<Transacao> CriarTransacao(NovaTransacaoDto novaTransacaoDto)
        {
            try
            {
                ValidarParametrosNovaTransacao(novaTransacaoDto);

                return await _transacaoRepository.CriarNovaTransacao(novaTransacaoDto);
            }
            catch (NotSupportedException ex)
            {
                Console.WriteLine(ex);
                return new Transacao { IsNotValido = true };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new Transacao();
            }
        }

        public async Task<bool> DestruirTransacao(string chaveTransacao)
        {
            try
            {
                await _transacaoRepository.DestruirTransacao(chaveTransacao);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        private void ValidarParametrosNovaTransacao(NovaTransacaoDto novaTransacaoDto)
        {
            if(string.IsNullOrWhiteSpace(novaTransacaoDto.ChaveSessao)
                || string.IsNullOrWhiteSpace(novaTransacaoDto.Usuario)
                || string.IsNullOrWhiteSpace(novaTransacaoDto.Senha))
                throw new InvalidOperationException("Paramentros para construção de nova sessão inválidos");

            return;
        }
    }
}
