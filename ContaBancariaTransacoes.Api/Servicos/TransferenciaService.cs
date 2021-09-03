using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Servicos
{
    //public class TransferenciaService : TransacaoBaseService, ITransferenciaService
    //{
    //    public async Task<Transacao> ExecutaTransferencia(TransferenciaDto transferencia, bool isCobrancaTaxa)
    //    {
    //        var taxaTransferencia = 0m;

    //        try
    //        {

    //        }
    //        catch (Exception ex)
    //        {
    //            Console.WriteLine(ex);
    //            return new Transacao
    //            {
    //                IsInvalida = true,
    //                Mensagem = ex.Message
    //            };
    //        }
    //        finally
    //        {
    //            LiberaTransacoesDaConta(transferencia.NumeroContaOrigem, transferencia.NumeroContaDestino);
    //        }
    //    }
    //}
}
