namespace ContaBancaria.Transacoes.Api.Negocio.Modelos
{
    public class Conta
    {
        public long Id { get; set; }
        public int Numero { get; set; }
        public short Agencia { get; set; }
        public decimal Saldo { get; set; }
        public ModalidadeConta Modalidade { get; set; }
        public Banco Banco { get; set; }
    }
}
