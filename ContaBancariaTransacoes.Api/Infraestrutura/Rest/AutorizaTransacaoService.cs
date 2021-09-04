using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest.Dto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Infraestrutura.Rest
{
    public class AutorizaTransacaoService : IAutorizaTransacaoService
    {
        private readonly IConfiguration _configuration;
        public AutorizaTransacaoService(IConfiguration configuration) => _configuration = configuration;

        public async Task<Guid> AutorizaTransacao(string sessao, string usuario, string senha)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var novaTransacao = new NovaTransacaoRequestDto
                    {
                        Usuario = usuario,
                        Senha = senha,
                        ChaveSessao = sessao
                    };

                    var corpoRequisicao = JsonConvert.SerializeObject(novaTransacao);

                    StringContent dados = new StringContent(corpoRequisicao, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(_configuration["Autorizacao:tokenUrl"], dados);

                    if (!response.IsSuccessStatusCode)
                        throw new HttpRequestException($"Erro ao autorizar processo: Status: {response.StatusCode}. Mensagem: {response.Content}");

                    var responseContent = await response.Content.ReadAsStringAsync();
                    var transacao = JsonConvert.DeserializeObject<NovaTransacaoResponseDto>(responseContent);

                    return transacao.Token;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}
