namespace ContaBancaria.Autorizacoes.Api.Conectores.Controllers.Dtos
{
    public class NovaTransacaoDto
    {
        public string ChaveSessao { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
    }
}
