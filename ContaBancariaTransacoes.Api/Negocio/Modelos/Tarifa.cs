using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Modelos
{
    public class Tarifa
    {
        public long Id { get; set; }
        public decimal Valor { get; set; }
        public Guid Transacao { get; set; }
    }
}
