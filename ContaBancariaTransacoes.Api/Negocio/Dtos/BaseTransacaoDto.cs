using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Dtos
{
    public class BaseTransacaoDto
    {
        public decimal Valor { get; set; }
        public int NumeroConta { get; set; }
        public short Agencia { get; set; }
        public Guid Sessao { get; set; }
    }
}
