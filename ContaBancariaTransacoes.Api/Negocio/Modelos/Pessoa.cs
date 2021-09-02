using System;

namespace ContaBancaria.Transacoes.Api.Negocio.Modelos
{
    public class Pessoa
    {
        public long CPF { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
    }
}
