using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Dtos
{
    public class TransferenciaDto
    {
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public Guid Sessao { get; set; }
        public decimal Valor { get; set; }
        public int NumeroContaOrigem { get; set; }
        public short AgenciaOrigem { get; set; }
        public int NumeroContaDestino { get; set; }
        public short AgenciaDestino { get; set; }
        public long CNPJBancoDestino { get; set; }
    }
}
