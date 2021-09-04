using ContaBancaria.Transacoes.Api.Negocio.Dtos;
using ContaBancaria.Transacoes.Api.Negocio.Enumeradores;
using ContaBancaria.Transacoes.Api.Negocio.Modelos;
using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using ContaBancaria.Transacoes.Api.Servicos;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Repositorios;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ContaBancaria.Transacoes.Api.Test.Servicos
{
    public class SaqueServiceTest
    {
        private Mock<IConfiguration> _configuration;
        private Mock<IControleTransacaoService> _transacaoService;
        private Mock<IAutorizaTransacaoService> _autorizacaoService;
        private Mock<ITransacaoRepository> _transacaoRepository;

        [SetUp]
        public void Setup()
        {
            _configuration = new Mock<IConfiguration>();
            _transacaoService = new Mock<IControleTransacaoService>();
            _autorizacaoService = new Mock<IAutorizaTransacaoService>();
            _transacaoRepository = new Mock<ITransacaoRepository>();

            _configuration.Setup(x => x["Saque:Taxa"]).Returns("4,00");
            _configuration.Setup(x => x["Saque:MaximoTentativas"]).Returns("1");
            _configuration.Setup(x => x["Banco:CNPJ"]).Returns("11122233344455");

            _transacaoRepository.Setup(x => x.GetSaldo(It.IsAny<int>(), It.IsAny<short>())).Returns(Task.FromResult(100m));

            _autorizacaoService.Setup(x => x.AutorizaTransacao(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(Guid.NewGuid()));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void ExecutaSaque(bool isCobrancaTaxa)
        {
            var valorSaque = 50m;
            var baseTransacao = ConstroiParametrosBaseTransacao(Guid.NewGuid(), valorSaque);
            var saque = ConstroiParametrosSaque(baseTransacao);
            var transacaoRetorno = ConstroiTransacao();

            _transacaoRepository.Setup(x => x.MovimentaSaldo(It.IsAny<Transacao>())).Returns(Task.FromResult(transacaoRetorno));

            var servico = new SaqueService(_configuration.Object, _transacaoService.Object, _autorizacaoService.Object, _transacaoRepository.Object);

            var transacao = servico.ExecutaSaque(saque, isCobrancaTaxa).Result;

            Assert.IsFalse(transacao.Token == Guid.Empty);
            Assert.IsFalse(transacao.IsInvalida);
            Assert.AreEqual(transacao.Modalidade.Id, (int) TipoTransacao.SAQUE);
            Assert.IsTrue(string.IsNullOrWhiteSpace(transacao.Mensagem));
        }

        [Test]
        public void TentaExecutarSaqueComSessaoInvalidaERetornaTransacaoInvalida()
        {
            var valorSaque = 50m;
            var baseTransacao = ConstroiParametrosBaseTransacao(Guid.Empty, valorSaque);
            var saque = ConstroiParametrosSaque(baseTransacao);

            var servico = new SaqueService(_configuration.Object, _transacaoService.Object, _autorizacaoService.Object, _transacaoRepository.Object);

            var transacao = servico.ExecutaSaque(saque, true).Result;

            Assert.IsTrue(transacao.IsInvalida);
            Assert.IsFalse(string.IsNullOrWhiteSpace(transacao.Mensagem));
        }

        [TestCase(0, 0, 0, "", "")]
        [TestCase(10, 123, 0, "", "")]
        [TestCase(10, 123, 123, "", "")]
        [TestCase(10, 123, 123, "usuario", "")]
        [TestCase(10, 123, 123, "", "senha")]
        public void TentaExecutarSaqueComParametrosInvalidosERetornaTransacaoInvalida(decimal valor, short agencia, int conta, string usuario, string senha)
        {
            var baseTransacao = ConstroiParametrosBaseTransacao(Guid.NewGuid(), valor, conta, agencia);
            var saque = ConstroiParametrosSaque(baseTransacao, usuario, senha);

            var servico = new SaqueService(_configuration.Object, _transacaoService.Object, _autorizacaoService.Object, _transacaoRepository.Object);

            var transacao = servico.ExecutaSaque(saque, true).Result;

            Assert.IsTrue(transacao.IsInvalida);
            Assert.IsFalse(string.IsNullOrWhiteSpace(transacao.Mensagem));
        }

        [Test]
        public void TentaExecutarSaqueComSaldoInsulficienteETaxaERetornaTransacaoInvalida()
        {
            var valorSaque = 50m;
            var baseTransacao = ConstroiParametrosBaseTransacao(Guid.NewGuid(), valorSaque);
            var saque = ConstroiParametrosSaque(baseTransacao);
            var transacaoRetorno = ConstroiTransacao();

            _transacaoRepository.Setup(x => x.GetSaldo(It.IsAny<int>(), It.IsAny<short>())).Returns(Task.FromResult(53m));

            var servico = new SaqueService(_configuration.Object, _transacaoService.Object, _autorizacaoService.Object, _transacaoRepository.Object);

            var transacao = servico.ExecutaSaque(saque, true).Result;

            Assert.IsTrue(transacao.IsInvalida);
            Assert.IsFalse(string.IsNullOrWhiteSpace(transacao.Mensagem));
        }

        [Test]
        public void ExecutaSaqueComSaldoIgualValorDeSaqueESemTaxa()
        {
            var valorSaque = 50m;
            var baseTransacao = ConstroiParametrosBaseTransacao(Guid.NewGuid(), valorSaque);
            var saque = ConstroiParametrosSaque(baseTransacao);
            var transacaoRetorno = ConstroiTransacao();

            _transacaoRepository.Setup(x => x.MovimentaSaldo(It.IsAny<Transacao>())).Returns(Task.FromResult(transacaoRetorno));
            _transacaoRepository.Setup(x => x.GetSaldo(It.IsAny<int>(), It.IsAny<short>())).Returns(Task.FromResult(50m));

            var servico = new SaqueService(_configuration.Object, _transacaoService.Object, _autorizacaoService.Object, _transacaoRepository.Object);

            var transacao = servico.ExecutaSaque(saque, false).Result;

            Assert.IsFalse(transacao.Token == Guid.Empty);
            Assert.IsFalse(transacao.IsInvalida);
            Assert.AreEqual(transacao.Modalidade.Id, (int)TipoTransacao.SAQUE);
            Assert.IsTrue(string.IsNullOrWhiteSpace(transacao.Mensagem));
        }

        [Test]
        public void TentaExecutarSaqueComFalhaNaAutorizacaoERetornaTransacaoInvalida()
        {
            var valorSaque = 50m;
            var baseTransacao = ConstroiParametrosBaseTransacao(Guid.NewGuid(), valorSaque);
            var saque = ConstroiParametrosSaque(baseTransacao);
            var transacaoRetorno = ConstroiTransacao();

            _transacaoRepository.Setup(x => x.MovimentaSaldo(It.IsAny<Transacao>())).Returns(Task.FromResult(transacaoRetorno));
            _autorizacaoService.Setup(x => x.AutorizaTransacao(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("Erro ao autorizar"));

            var servico = new SaqueService(_configuration.Object, _transacaoService.Object, _autorizacaoService.Object, _transacaoRepository.Object);

            var transacao = servico.ExecutaSaque(saque, true).Result;

            Assert.IsTrue(transacao.IsInvalida);
            Assert.IsFalse(string.IsNullOrWhiteSpace(transacao.Mensagem));
        }

        [Test]
        public void TentaExecutarSaqueComFalhaNoBancoDeDadosERetornaTransacaoInvalida()
        {
            var valorSaque = 50m;
            var baseTransacao = ConstroiParametrosBaseTransacao(Guid.NewGuid(), valorSaque);
            var saque = ConstroiParametrosSaque(baseTransacao);
            var transacaoRetorno = ConstroiTransacao();

            _transacaoRepository.Setup(x => x.MovimentaSaldo(It.IsAny<Transacao>())).Throws(new Exception("Erro ao executar operação no banco de dados"));

            var servico = new SaqueService(_configuration.Object, _transacaoService.Object, _autorizacaoService.Object, _transacaoRepository.Object);

            var transacao = servico.ExecutaSaque(saque, true).Result;

            Assert.IsTrue(transacao.IsInvalida);
            Assert.IsFalse(string.IsNullOrWhiteSpace(transacao.Mensagem));
        }

        private SaqueDto ConstroiParametrosSaque(BaseTransacaoDto baseTransacaoDto, string usuario = "Joao", string senha = "123") =>
            new SaqueDto { BaseTransacao = baseTransacaoDto, Senha = usuario, Usuario = senha };

        private BaseTransacaoDto ConstroiParametrosBaseTransacao(Guid sessao, decimal valor = 100m, int conta = 1234567, short agencia = 1) =>
            new BaseTransacaoDto { Valor = valor, NumeroConta = conta, Agencia = agencia, Sessao = sessao };

        private Transacao ConstroiTransacao() =>
            new Transacao { Token = Guid.NewGuid(), Modalidade = new ModalidadeTransacao { Id = (int)TipoTransacao.SAQUE } };
    }
}
