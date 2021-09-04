using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using Cache = ContaBancaria.Transacoes.Api.Infraestrutura.SimuladorCacheRepository;

namespace ContaBancaria.Transacoes.Api.Servicos
{
    public class ControleTransacaoService : IControleTransacaoService
    {
        public void LiberaContaParaTransacao(params long[] contas)
            => Cache.RemoveContasEmUso(contas);

        public void TravaContaParaTransacao(params long[] contas)
            => Cache.AdicionaContasEmUso(contas);

        public bool VerificaConcorrencia(params long[] contas)
            => Cache.VefiricaContaEmUso(contas);
    }
}
