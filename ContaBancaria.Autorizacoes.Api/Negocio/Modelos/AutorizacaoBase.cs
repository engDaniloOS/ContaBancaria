using System;

namespace ContaBancaria.Autorizacoes.Api.Negocio.Modelos
{
    public abstract class AutorizacaoBase
    {
        public Guid Id { get; private set; }
        public Guid Token { get; set; }
        public bool IsNotValido { get; set; }
        public bool IsAtivo { get; set; }
        public DateTime DataCriacao { get; set; }
    }
}
