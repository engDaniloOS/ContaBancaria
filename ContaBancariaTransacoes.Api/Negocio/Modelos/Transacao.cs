using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Modelos
{
    public class Transacao
    {
        public Guid Id { get; set; }
        public Guid Token { get; set; }
        public Guid Sessao { get; set; }
        public DateTime Data { get; set; }
        public bool IsInvalida { get; set; }
        public ModalidadeTransacao Modalidade { get; set; }
        public Conta ContaOrigem { get; set; }
        public Conta ContaDestino { get; set; }
        public decimal SaldoOrigem { get; set; }
        public decimal SaldoDestino { get; set; }
        public decimal TaxaOrigem { get; set; }
        public decimal Valor { get; set; }
        public string Mensagem { get; set; }
    }
}
