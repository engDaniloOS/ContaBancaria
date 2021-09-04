using System.Collections.Generic;

namespace ContaBancaria.Transacoes.Api.Negocio.Dtos
{
    public class ExtratoResponseDto
    {
        public List<TransacoesDto> Extrato { get; set; }
        public string Mensagem { get; set; }
        public bool IsInvalid { get; set; }
    }
}
