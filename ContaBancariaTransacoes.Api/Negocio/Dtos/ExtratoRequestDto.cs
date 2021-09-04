using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Dtos
{
    public class ExtratoRequestDto
    {
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public Guid Sessao { get; set; }
        public int NumeroConta { get; set; }
        public short Agencia { get; set; }
    }
}
