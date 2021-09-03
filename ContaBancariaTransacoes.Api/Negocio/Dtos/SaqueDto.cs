namespace ContaBancaria.Transacoes.Api.Negocio.Dtos
{
    public class SaqueDto
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public BaseTransacaoDto BaseTransacao { get; set; }
    }
}
