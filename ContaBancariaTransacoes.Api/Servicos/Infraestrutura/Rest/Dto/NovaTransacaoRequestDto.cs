namespace ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest.Dto
{
    public class NovaTransacaoRequestDto
    {
        public string ChaveSessao { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}
