using System;
using Amazon;
using Amazon.S3;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Synergy.Common.Aws.Extensions;
using Synergy.Common.DAL.Abstract;
using Synergy.Common.DAL.Access.PostgreSQL;
using Synergy.Common.FileStorage.Abstraction;
using Synergy.Common.FileStorage.AmazonS3;
using Synergy.CRM.DAL.Queries.Original.CRM;
using Synergy.CRM.Domain;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Domain.Validators;
using Synergy.CRM.Domain.Validators.Reminder;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Opportunity;
using Synergy.CRM.Models.Reminder;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Dictionaries.Queries;
using Synergy.ServiceBus.Amazon;
using Synergy.ServiceBus.Extensions.Configuration;
using Synergy.ServiceBus.RabbitMq;

namespace Synergy.CRM.API
{
    public static class ServiceRegistrations
    {
        public static IServiceCollection AddServiceBus(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (isDevelopment)
            {
                services.AddServiceBus<Synergy.ServiceBus.RabbitMq.MessageBus, RabbitMQConfig>(builder =>
                {
                    builder.Configure(configuration, "ServiceBus:RabbitMQ");
                });
            }
            else
            {
                services.AddServiceBus<Synergy.ServiceBus.Amazon.MessageBus, AWSMessageBusConfig>(builder =>
                {
                    builder.Configure(x =>
                    {
                        configuration.Bind("AwsMessageBus", x);

                        x.TopicName = configuration["TopicName"];
                        x.QueueName = configuration["crm:QueueName"];
                        x.Region = configuration.GetRegionEndPoint();
                    });
                });

                services.AddLargeMessageSerializer();
            }

            return services;
        }

        public static IServiceCollection AddFileStorage(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
        {
            if (isDevelopment)
            {
                var minioEndpointUrl = configuration["MinioEndpointUrl"];
                var minioAccessKey = configuration["MinioAccessKey"];
                var minioSecretKey = configuration["MinioSecretKey"];
                var bucketName = configuration["MinioBucketName"];

                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USEast1,
                    ServiceURL = minioEndpointUrl,
                    ForcePathStyle = true,
                };

                services.AddTransient<IAmazonS3>(_ => new AmazonS3Client(minioAccessKey, minioSecretKey, config));
                services.AddTransient<IFileStorage>(provider => new AmazonS3Storage(bucketName, provider.GetService<IAmazonS3>()));
            }
            else
            {
                var bucketName = configuration["BucketName"];

                services.AddTransient<IAmazonS3>(_ => new AmazonS3Client(configuration.GetRegionEndPoint()));
                services.AddTransient<IFileStorage>(provider => new AmazonS3Storage(bucketName, provider.GetService<IAmazonS3>()));
            }

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration, bool isDevelopment = false)
        {
            var connectionString = configuration.GetConnectionString("DB");
            var runMigrations = configuration["DB:RunMigrations"] == "true";

            services.RegisterSynergyEncriptionService(isDevelopment, configuration);
            services.RegisterCRMQueries(connectionString, isDevelopment && runMigrations);
            services.RegisterDictionariesQueries(connectionString, false);

            services.AddScoped<IDataAccess>(provider => new DAL.Queries.PostgreSQL.DataAccess(provider.GetService<ILoggerFactory>(), connectionString));
            services.AddTransient(typeof(IQueryProvider<>), typeof(QueryProvider<>));

            services.AddTransient<ILeadService, LeadService>();
            services.AddTransient<IOpportunityService, OpportunityService>();
            services.AddTransient<IContactService, ContactService>();
            services.AddTransient<IPropertyService, PropertyService>();
            services.AddTransient<ICampaignService, CampaignService>();
            services.AddTransient<IDictionaryService, DictionaryService>();
            services.AddTransient<IHistoryService, HistoryService>();
            services.AddTransient<IReminderService, ReminderService>();

            return services;
        }

        public static IServiceCollection AddDomainValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<OpportunityCreateArgs>, OpportunityCreateArgsValidator>();
            services.AddTransient<IValidator<OpportunityUpdateArgs>, OpportunityUpdateArgsValidator>();

            services.AddTransient<IValidator<ContactCreateArgs>, ContactCreateArgsValidator>();
            services.AddTransient<IValidator<ContactUpdateArgs>, ContactUpdateArgsValidator>();

            services.AddTransient<IValidator<CampaignCreateArgs>, CampaignCreateArgsValidator>();
            services.AddTransient<IValidator<CampaignUpdateArgs>, CampaignUpdateArgsValidator>();

            services.AddTransient<IValidator<LeadCommentCreateArgs>, LeadCommentCreateArgsValidator>();
            services.AddTransient<IValidator<CampaignCommentCreateArgs>, CampaignCommentCreateArgsValidator>();

            services.AddTransient<IValidator<ReminderCreateArgs>, ReminderCreateArgsValidator>();
            services.AddTransient<IValidator<ReminderUpdateArgs>, ReminderUpdateArgsValidator>();
            return services;
        }
    }
}