using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using Synergy.Common.AspNet;
using Synergy.Common.AspNet.Middleware;
using Synergy.Common.Logging.Configuration;
using Synergy.Common.Security.Extensions;
using Synergy.CRM.Domain.Validators;

namespace Synergy.CRM.API
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
            services
                .AddMvc(options =>
                {
                    options.Filters.Add(new AuthorizeFilter());
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                })
                .AddFluentValidation(x => x.RegisterValidatorsFromAssemblyContaining<CampaignCreateArgsValidator>());

            services.AddCors("synergy");

            services.AddHealthChecks(this._configuration.GetConnectionString("DB"), "Database");

            services.AddSwagger("Synergy.CRM.API");

            services.AddAutoMapper(new Assembly[]
            {
                Assembly.Load("Synergy.CRM.API"),
                Assembly.Load("Synergy.CRM.DAL.Queries.Original"),
                Assembly.Load("Synergy.CRM.Domain"),
                Assembly.Load("Synergy.DataAccess.Abstractions"),
                Assembly.Load("Synergy.DataAccess.Dictionaries.Queries"),
            });

            this.RegisterServices(services);
        }

        public void Configure(IApplicationBuilder app, AutoMapper.IMapper mapper)
        {
            app.UseStartupLogging();
            app.UseVersion();

            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            if (this._hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseCorrelationLogging();
            app.UseCors("synergy");
            app.UseHealthCheck();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseOperationContext();
            app.UseMvc();
            app.UseSwagger("Synergy CRM API V1");
        }

        private void RegisterServices(IServiceCollection services)
        {
            var isDevelopment = this._hostingEnvironment.IsDevelopment();

            services.AddDefaultApiContext()
                .AddSerilogLogging(this._configuration)
                .AddAuth(this._configuration, isDevelopment)
                .AddServiceBus(this._configuration, isDevelopment)
                .AddFileStorage(this._configuration, isDevelopment)
                .AddDomainServices(this._configuration, isDevelopment)
                .AddDomainValidators();
        }

#pragma warning disable CA1812 // Startup.OperationIdFilter is an internal class that is apparently never instantiated
        private class OperationIdFilter : IOperationFilter
#pragma warning restore CA1812 // Startup.OperationIdFilter is an internal class that is apparently never instantiated
        {
            public void Apply(Operation operation, OperationFilterContext context)
            {
                operation.OperationId = context.MethodInfo.Name + context.MethodInfo.GetParameters()
                                            .Aggregate("_", (acc, cur) => cur.ParameterType == typeof(CancellationToken)
                                                ? acc
                                                : acc + "_" + cur.ParameterType.Name);
            }
        }
    }
}
