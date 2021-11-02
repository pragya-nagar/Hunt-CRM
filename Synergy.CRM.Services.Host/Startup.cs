using System;
using System.Reflection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synergy.Common.AspNet;
using Synergy.Common.Logging.Configuration;
using Synergy.CRM.DAL.Commands;
using Synergy.CRM.Services.Host.AppStart;
using Synergy.DataAccess.Abstractions;
using Synergy.ServiceBus.Abstracts;
using Synergy.ServiceBus.Extensions.Configuration;

namespace Synergy.CRM.Services.Host
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            this._hostingEnvironment = env;
            this._configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = this._configuration.GetConnectionString("DB");

            var runMigrations = this._hostingEnvironment.IsDevelopment() && this._configuration["DB:RunMigrations"] == "true";
            services.RegisterSynergyEncriptionService(this._hostingEnvironment.IsDevelopment(), this._configuration);
            services.RegisterSynergyContext(connectionString, runMigrations);

            services.RegisterCRMCommands();

            services.AddHealthChecks(this._configuration.GetConnectionString("DB"), name: "Database");

            services.AddAutoMapper(new Assembly[]
            {
                Assembly.Load("Synergy.CRM.DAL.Commands"),
                Assembly.Load("Synergy.CRM.Services"),
            });

            this.RegisterServices(services);
        }

#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app,
#pragma warning restore CA1822 // Mark members as static
            IServiceProvider provider,
            IMessageBus messageBus)
        {
            app.UseStartupLogging();

            messageBus.UseServiceBusAsync(app.ApplicationServices).GetAwaiter().GetResult();

            app.UseHealthChecks("/api/health", new HealthCheckOptions
            {
                Predicate = _ => true,
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
            });

            provider.GetRequiredService<AutoMapper.IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
        }

        private void RegisterServices(IServiceCollection services)
        {
            var isDevelopment = this._hostingEnvironment.IsDevelopment();

            services.AddDefaultApiContext()
                .AddSerilogLogging(this._configuration)
                .AddServiceBus(this._configuration, isDevelopment)
                .AddFileStorage(this._configuration, isDevelopment);
        }
    }
}