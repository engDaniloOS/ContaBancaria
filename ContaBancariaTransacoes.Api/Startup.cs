using ContaBancaria.Transacoes.Api.Infraestrutura.BancoDeDados;
using ContaBancaria.Transacoes.Api.Infraestrutura.Repositorios;
using ContaBancaria.Transacoes.Api.Infraestrutura.Rest;
using ContaBancaria.Transacoes.Api.Negocio.Servicos;
using ContaBancaria.Transacoes.Api.Servicos;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Repositorios;
using ContaBancaria.Transacoes.Api.Servicos.Infraestrutura.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;

namespace ContaBancariaApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            #region Banco de dados
            services.AddDbContext<Contexto>(opcao => opcao.UseSqlServer(GetConnectionString()));
            #endregion

            #region Injeção de dependência
            services.AddTransient<IControleTransacaoService, ControleTransacaoService>();
            services.AddTransient<IDepositoService, DepositoService>();
            services.AddTransient<ITransacaoRepository, TransacaoRepository>();
            services.AddTransient<IAutorizaTransacaoService, AutorizaTransacaoService>();
            services.AddTransient<ISaqueService, SaqueService>();
            services.AddTransient<ITransferenciaService, TransferenciaService>();
            #endregion

            #region Swagger
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo { Title = "Conta Bancária - Transações", Version = "v1" });
            });
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSwagger();

            app.UseSwaggerUI(s =>
            {
                //s.RoutePrefix = string.Empty;
                s.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private string GetConnectionString()
            => Configuration.GetConnectionString("Default").Replace("?path?", Environment.CurrentDirectory);
    }
}
