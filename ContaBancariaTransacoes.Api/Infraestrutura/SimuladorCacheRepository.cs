using System.Collections.Generic;

namespace ContaBancaria.Transacoes.Api.Infraestrutura
{
    public static class SimuladorCacheRepository
    {

        public static List<long> ContasEmUso { get; set; }

        public static void AdicionaContasEmUso(params long[] contas)
        {
            if (ContasEmUso == null)
                ContasEmUso = new List<long>();

            foreach (var conta in contas)
            {
                if (!ContasEmUso.Contains(conta))
                    ContasEmUso.AddRange(contas);
            }
        }

        public static void RemoveContasEmUso(params long[] contas)
        {
            if (ContasEmUso == null) return;

            foreach (var conta in contas)
                ContasEmUso.Remove(conta);
        }

        public static bool VefiricaContaEmUso(params long[] contas)
        {
            if (ContasEmUso == null) return false;

            foreach (var conta in contas)
            {
                if (ContasEmUso.Contains(conta))
                    return true;
            }

            return false;
        }
    }
}
