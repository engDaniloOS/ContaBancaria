namespace ContaBancaria.Transacoes.Api.Negocio.Servicos
{
    public interface IControleTransacaoService
    {
        bool VerificaConcorrencia(params long[] contas);
        void TravaContaParaTransacao(params long[] contas);
        void LiberaContaParaTransacao(params long[] contas);
    }
}
