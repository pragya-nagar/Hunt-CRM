using System;
using Amazon;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Synergy.Common.Aws.Extensions;
using Synergy.Common.FileStorage.Abstraction;
using Synergy.Common.FileStorage.AmazonS3;
using Synergy.CRM.Models.Commands;
using Synergy.CRM.Models.Commands.Opportunity;
using Synergy.CRM.Models.Commands.Reminder;
using Synergy.ServiceBus.Abstracts;
using Synergy.ServiceBus.Abstracts.ServiceEvents;
using Synergy.ServiceBus.Amazon;
using Synergy.ServiceBus.Extensions.Configuration;
using Synergy.ServiceBus.Messages;
using Synergy.ServiceBus.RabbitMq;

namespace Synergy.CRM.Services.Host.AppStart
{
    public static class ServiceRegistration
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

                    AddSubscriptions(builder);
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

                    AddSubscriptions(builder);
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

        private static void AddSubscriptions(IHandlerRegistrationBuilder builder)
        {
            builder.Subscribe<OpportunityService, OpportunityCreateCommand>();
            builder.Subscribe<OpportunityService, OpportunityUpdateCommand>();
            builder.Subscribe<OpportunityService, OpportunitySensitiveDataUpdateCommand>();

            builder.Subscribe<ContactService, ContactCreateCommand>();
            builder.Subscribe<ContactService, ContactUpdateCommand>();

            builder.Subscribe<LeadService, LeadCommentCreateCommand>();
            builder.Subscribe<LeadService, LeadCommentUpdateCommand>();
            builder.Subscribe<LeadService, LeadCommentDeleteCommand>();

            builder.Subscribe<CampaignService, CampaignCreateCommand>();
            builder.Subscribe<CampaignService, CampaignUpdateCommand>();
            builder.Subscribe<CampaignService, MakeCampaignDataDumpCommand>(new HandleOptions { DisableParallelProcessing = true, ExecutionTimeout = TimeSpan.FromMinutes(10) });
            builder.Subscribe<CampaignService, CampaignCommentCreateCommand>();
            builder.Subscribe<CampaignService, CampaignCommentUpdateCommand>();
            builder.Subscribe<CampaignService, CampaignCommentDeleteCommand>();
            builder.Subscribe<CampaignAssignmentService, ApplyRulesCommand>(new HandleOptions { DisableParallelProcessing = true, ExecutionTimeout = TimeSpan.FromMinutes(10) });
            builder.Subscribe<CampaignAssignmentService, DeleteRulesCommand>(new HandleOptions { DisableParallelProcessing = true, ExecutionTimeout = TimeSpan.FromMinutes(10) });
            builder.Subscribe<CampaignService, CreateRuleCommand>();

            builder.Subscribe<MailMergeService, MailMergeCommand>(new HandleOptions { IsTerminal = false });
            builder.Subscribe<MailMergeService, MailMergeFinishedEvent>();

            builder.SubscribeToServiceEvent<NotificationService, HandlerStartedEvent>();
            builder.SubscribeToServiceEvent<NotificationService, HandlerSuccessEvent>();
            builder.SubscribeToServiceEvent<NotificationService, HandlerExceptionEvent>();
            builder.SubscribeToServiceEvent<NotificationService, DeadMessageEvent>();
            builder.SubscribeToServiceEvent<NotificationService, HandlerDiscardedEvent>();
            builder.SubscribeToServiceEvent<NotificationService, HandlerPostedForProcessingEvent>();
            builder.SubscribeToServiceEvent<NotificationService, MessageReceivedEvent>();

            builder.Subscribe<ReminderService, ReminderCreateCommand>();
            builder.Subscribe<ReminderService, ReminderUpdateNotificationCommand>();
            builder.Subscribe<ReminderService, ReminderUpdateCommand>();
        }
    }
}
