namespace ContaBancaria.Transacoes.Api.Negocio.Modelos
{
    public class Credenciais
    {
        public long Id { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public long ClienteId { get; set; }
    }
}
