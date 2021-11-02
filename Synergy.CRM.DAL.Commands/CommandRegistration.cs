using Microsoft.Extensions.DependencyInjection;
using Synergy.CRM.DAL.Commands.Commands;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Queries;

namespace Synergy.CRM.DAL.Commands
{
    public static class CommandRegistration
    {
        public static void RegisterCRMCommands(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ICreateOpportunityCommand, CreateOpportunityCommand>();
            serviceCollection.AddTransient<IUpdateOpportunityCommand, UpdateOpportunityCommand>();
            serviceCollection.AddTransient<IUpdateOpportunitySensitiveDataCommand, UpdateOpportunitySensitiveDataCommand>();

            serviceCollection.AddTransient<ICreateCampaignCommand, CreateCampaignCommand>();
            serviceCollection.AddTransient<IUpdateCampaignCommand, UpdateCampaignCommand>();
            serviceCollection.AddTransient<ICreateCampaignCommentCommand, CreateCampaignCommentCommand>();
            serviceCollection.AddTransient<IUpdateCampaignCommentCommand, UpdateCampaignCommentCommand>();
            serviceCollection.AddTransient<IDeleteCampaignCommentCommand, DeleteCampaignCommentCommand>();
            serviceCollection.AddTransient<IUpdateCampaignCountersCommand, UpdateCampaignCountersCommand>();

            serviceCollection.AddTransient<IAddLeadsToCampaignCommand, AddLeadsToCampaignCommand>();
            serviceCollection.AddTransient<IRemoveCampaignLeadsCommand, RemoveCampaignLeadsCommand>();
            serviceCollection.AddTransient<IRemoveCampaignRulesCommand, RemoveCampaignRulesCommand>();

            serviceCollection.AddTransient<ICreateCampaignGeneralRuleCommand, CreateCampaignGeneralRuleCommand>();
            serviceCollection.AddTransient<IAddRulesToCampaignCommand, AddRulesToCampaignCommand>();

            serviceCollection.AddTransient<ICreateContactCommand, CreateContactCommand>();
            serviceCollection.AddTransient<IUpdateContactCommand, UpdateContactCommand>();

            serviceCollection.AddTransient<ICreateLeadCommentCommand, CreateLeadCommentCommand>();
            serviceCollection.AddTransient<IUpdateLeadCommentCommand, UpdateLeadCommentCommand>();
            serviceCollection.AddTransient<IDeleteLeadCommentCommand, DeleteLeadCommentCommand>();

            serviceCollection.AddTransient<ICreateReminderCommand, CreateReminderCommand>();
            serviceCollection.AddTransient<IUpdateReminderNotificationCommand, UpdateReminderNotificationCommand>();
            serviceCollection.AddTransient<IUpdateReminderCommand, UpdateReminderCommand>();

            serviceCollection.AddTransient<GetMailMergePropertyFieldsQuery>();
            serviceCollection.AddTransient<GetMailMergeTemplateQuery>();

            serviceCollection.AddTransient<UserExistsQuery>();
            serviceCollection.AddTransient<CampaignExistsQuery>();
            serviceCollection.AddTransient<CampaignLeadDumpQuery>();
            serviceCollection.AddTransient<CampaignPropertyDumpQuery>();
            serviceCollection.AddTransient<CampaignQuery>();
            serviceCollection.AddTransient<GetCampaignRulesAndCountiesQuery>();
            serviceCollection.AddTransient<GetCampaignCommentAuthorIdQuery>();
            serviceCollection.AddTransient<ContactExistsQuery>();
            serviceCollection.AddTransient<OpportunityExistsQuery>();
            serviceCollection.AddTransient<LeadsDataCutQuery>();
            serviceCollection.AddTransient<GetLeadCommentAuthorIdQuery>();
            serviceCollection.AddTransient<GetLastOpportunitySequenceNumberQuery>();
            serviceCollection.AddTransient<GetOpportunityStageQuery>();
            serviceCollection.AddTransient<GetReminderQuery>();
            serviceCollection.AddTransient<ReminderExistQuery>();
        }
    }
}
