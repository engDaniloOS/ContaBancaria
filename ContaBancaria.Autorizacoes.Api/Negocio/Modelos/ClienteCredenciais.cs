namespace ContaBancaria.Autorizacoes.Api.Negocio.Modelos
{
    public class ClienteCredenciais
    {
        public long Id { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public bool IsAtivo { get; set; }
    }
}
