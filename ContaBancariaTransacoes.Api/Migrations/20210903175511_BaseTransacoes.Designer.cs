// <auto-generated />
using System;
using ContaBancaria.Transacoes.Api.Infraestrutura.BancoDeDados;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ContaBancaria.Transacoes.Api.Migrations
{
    [DbContext(typeof(Contexto))]
    [Migration("20210903175511_BaseTransacoes")]
    partial class BaseTransacoes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Banco", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CNPJ");

                    b.Property<string>("Nome");

                    b.HasKey("Id");

                    b.ToTable("Bancos");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Cliente", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("Clientes");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Conta", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<short>("Agencia");

                    b.Property<int?>("BancoId");

                    b.Property<long>("ClienteId");

                    b.Property<bool>("IsAtiva");

                    b.Property<int?>("ModalidadeId");

                    b.Property<int>("Numero");

                    b.Property<decimal>("Saldo");

                    b.HasKey("Id");

                    b.HasIndex("BancoId");

                    b.HasIndex("ClienteId");

                    b.HasIndex("ModalidadeId");

                    b.ToTable("Contas");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Credenciais", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("ClienteId");

                    b.Property<string>("Senha");

                    b.Property<string>("Usuario");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId")
                        .IsUnique();

                    b.ToTable("Credenciais");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.ModalidadeConta", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nome");

                    b.HasKey("Id");

                    b.ToTable("ModalidadesConta");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.ModalidadeTransacao", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Nome");

                    b.HasKey("Id");

                    b.ToTable("ModalidadesTransacao");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Pessoa", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CPF");

                    b.Property<long>("ClienteId");

                    b.Property<DateTime>("DataNascimento");

                    b.Property<string>("Nome");

                    b.HasKey("Id");

                    b.HasIndex("ClienteId")
                        .IsUnique();

                    b.ToTable("Pessoas");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Tarifa", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("Transacao");

                    b.Property<decimal>("Valor");

                    b.HasKey("Id");

                    b.ToTable("Tarifas");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Transacao", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("ContaDestinoId");

                    b.Property<long?>("ContaOrigemId");

                    b.Property<DateTime>("Data");

                    b.Property<bool>("IsInvalida");

                    b.Property<string>("Mensagem");

                    b.Property<int?>("ModalidadeId");

                    b.Property<decimal>("SaldoDestino");

                    b.Property<decimal>("SaldoOrigem");

                    b.Property<Guid>("Sessao");

                    b.Property<decimal>("TaxaOrigem");

                    b.Property<Guid>("Token");

                    b.Property<decimal>("Valor");

                    b.HasKey("Id");

                    b.HasIndex("ContaDestinoId");

                    b.HasIndex("ContaOrigemId");

                    b.HasIndex("ModalidadeId");

                    b.ToTable("Transacoes");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Conta", b =>
                {
                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.Banco", "Banco")
                        .WithMany()
                        .HasForeignKey("BancoId");

                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.Cliente")
                        .WithMany("Contas")
                        .HasForeignKey("ClienteId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.ModalidadeConta", "Modalidade")
                        .WithMany()
                        .HasForeignKey("ModalidadeId");
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Credenciais", b =>
                {
                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.Cliente")
                        .WithOne("Credenciais")
                        .HasForeignKey("ContaBancaria.Transacoes.Api.Negocio.Modelos.Credenciais", "ClienteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Pessoa", b =>
                {
                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.Cliente")
                        .WithOne("Pessoa")
                        .HasForeignKey("ContaBancaria.Transacoes.Api.Negocio.Modelos.Pessoa", "ClienteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ContaBancaria.Transacoes.Api.Negocio.Modelos.Transacao", b =>
                {
                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.Conta", "ContaDestino")
                        .WithMany()
                        .HasForeignKey("ContaDestinoId");

                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.Conta", "ContaOrigem")
                        .WithMany()
                        .HasForeignKey("ContaOrigemId");

                    b.HasOne("ContaBancaria.Transacoes.Api.Negocio.Modelos.ModalidadeTransacao", "Modalidade")
                        .WithMany()
                        .HasForeignKey("ModalidadeId");
                });
#pragma warning restore 612, 618
        }
    }
}
