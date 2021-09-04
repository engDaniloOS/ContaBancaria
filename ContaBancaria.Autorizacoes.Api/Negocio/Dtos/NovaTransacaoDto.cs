namespace ContaBancaria.Autorizacoes.Api.Servicos.Dtos
{
    public class NovaTransacaoDto
    {
        public string ChaveSessao { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}
