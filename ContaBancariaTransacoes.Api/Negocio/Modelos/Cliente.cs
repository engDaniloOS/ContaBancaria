using System.Collections.Generic;

namespace ContaBancaria.Transacoes.Api.Negocio.Modelos
{
    public class Cliente
    {
        public long Registro { get; set; }
        public Pessoa Pessoa { get; set; }
        public Credenciais Credenciais { get; set; }
        public IList<Conta> Contas { get; set; }
    }
}
