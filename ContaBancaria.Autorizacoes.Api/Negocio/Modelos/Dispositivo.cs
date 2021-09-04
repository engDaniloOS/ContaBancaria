namespace ContaBancaria.Autorizacoes.Api.Negocio.Modelos
{
    public class Dispositivo
    {
        public long Id { get; set; }
        public string Chave { get; set; }
        public bool IsAtivo { get; set; }
    }
}
