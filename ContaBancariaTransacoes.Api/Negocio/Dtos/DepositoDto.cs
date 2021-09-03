using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Dtos
{
    public class DepositoDto
    {
        public decimal Valor { get; set; }
        public long BancoCnpj { get; set; }
        public int NumeroConta { get; set; }
        public short Agencia { get; set; }
        public Guid Sessao { get; set; }
    }
}
