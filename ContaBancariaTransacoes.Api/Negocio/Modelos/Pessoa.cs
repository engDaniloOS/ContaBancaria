using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Modelos
{
    public class Pessoa
    {
        public long Id { get; set; }
        public long CPF { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public long ClienteId { get; set; }
    }
}
