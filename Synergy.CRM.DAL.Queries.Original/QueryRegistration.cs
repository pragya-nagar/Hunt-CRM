using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.DAL.Queries.Original.Queries;
using Synergy.DataAccess.Abstractions;

namespace Synergy.CRM.DAL.Queries.Original.CRM
{
    public static class QueryRegistration
    {
        public static void RegisterCRMQueries(this IServiceCollection serviceCollection, string connectionString, bool runMigration = true)
        {
            serviceCollection.RegisterSynergyContext(connectionString, runMigration);
            ValidateMapperConfigurations(serviceCollection);
            serviceCollection.AddTransient<IGetLeadsQuery, GetLeadsQuery>();
            serviceCollection.AddTransient<IGetPropertiesQuery, GetPropertiesQuery>();
            serviceCollection.AddTransient<IGetCampaignsQuery, GetCampaignsQuery>();
            serviceCollection.AddTransient<IGetOpportunitiesQuery, GetOpportunitiesQuery>();
            serviceCollection.AddTransient<IGetContactsQuery, GetContactsQuery>();
            serviceCollection.AddTransient<IGetCampaignLogicTypesQuery, GetCampaignLogicTypesQuery>();
            serviceCollection.AddTransient<IGetCampaignRuleFieldsQuery, GetCampaignRuleFieldsQuery>();
            serviceCollection.AddTransient<IGetCampaignGeneralRuleQuery, GetCampaignGeneralRuleQuery>();
            serviceCollection.AddTransient<IGetCampaignRulesQuery, GetCampaignRulesQuery>();
            serviceCollection.AddTransient<IGetPropertiesApplyRuleQuery, GetPropertiesApplyRuleQuery>();
            serviceCollection.AddTransient<IGetLeadCommentsQuery, GetLeadCommentsQuery>();
            serviceCollection.AddTransient<IGetCampaignCommentsQuery, GetCampaignCommentsQuery>();
            serviceCollection.AddTransient<IGetCountyQuery, GetCountyQuery>();
            serviceCollection.AddTransient<IGetBorrowerSensitiveDataQuery, GetBorrowerSensitiveDataQuery>();
            serviceCollection.AddTransient<IGetCommercialBorrowerSensitiveDataQuery, GetCommercialBorrowerSensitiveDataQuery>();
            serviceCollection.AddTransient<IGetReminderQuery, GetReminderQuery>();
        }

        private static void ValidateMapperConfigurations(IServiceCollection serviceCollection)
        {
            var mapper = serviceCollection.BuildServiceProvider().GetService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
