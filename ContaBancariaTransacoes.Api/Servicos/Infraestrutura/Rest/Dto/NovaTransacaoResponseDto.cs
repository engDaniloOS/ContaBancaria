using System;

namespace ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest.Dto
{
    public class NovaTransacaoResponseDto
    {
        public Guid Token { get; set; }
        public DateTime DataCriacao { get; set; }
        public Guid Sessao { get; set; }
    }
}
