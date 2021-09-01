namespace ContaBancaria.Autorizacoes.Api.Negocio.Modelos
{
    public class Transacao : AutorizacaoBase
    {
        public ClienteCredenciais ClienteCredenciais { get; set; }
        public Sessao Sessao { get; set; }
    }
}
