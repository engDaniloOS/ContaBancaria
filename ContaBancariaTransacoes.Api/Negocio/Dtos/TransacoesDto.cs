using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Dtos
{
    public class TransacoesDto
    {
        public DateTime Data { get; set; }
        public string TipoTransacao { get; set; }
        public decimal Valor { get; set; }
        public decimal Taxa { get; set; }
        public decimal Saldo { get; set; }
        public int NumeroContaOrigem { get; set; }
        public short AgenciaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public short AgenciaDestino { get; set; }
    }
}
