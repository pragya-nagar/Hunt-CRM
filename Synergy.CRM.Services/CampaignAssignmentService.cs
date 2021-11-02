using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.DAL.Commands.Models.Results;
using Synergy.CRM.DAL.Commands.Queries;
using Synergy.CRM.Models.Commands;
using Synergy.ServiceBus.Abstracts;
using Synergy.ServiceBus.Extensions.Progress;

namespace Synergy.CRM.Services
{
    public class CampaignAssignmentService : IMessageHandler<ApplyRulesCommand>, IMessageHandler<DeleteRulesCommand>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IProgressPublisher _progressPublisher;
        private readonly IAddRulesToCampaignCommand _addToCampaignCommand;
        private readonly IAddLeadsToCampaignCommand _addLeadsToCampaignCommand;
        private readonly IUpdateCampaignCountersCommand _updateCampaignCountersCommand;
        private readonly IRemoveCampaignLeadsCommand _removeCampaignLeadsCommand;
        private readonly IRemoveCampaignRulesCommand _removeCampaignRulesCommand;

        private readonly CampaignExistsQuery _campaignExistsQuery;
        private readonly LeadsDataCutQuery _leadsDataCutQuery;

        public CampaignAssignmentService(ILogger<CampaignService> logger,
            IMapper mapper,
            IProgressPublisher progressPublisher,
            IAddRulesToCampaignCommand addRulesToCampaignCommand,
            IAddLeadsToCampaignCommand addLeadsToCampaignCommand,
            IUpdateCampaignCountersCommand updateCampaignCountersCommand,
            IRemoveCampaignLeadsCommand removeCampaignLeadsCommand,
            IRemoveCampaignRulesCommand removeCampaignRulesCommand,
            CampaignExistsQuery campaignExistsQuery,
            LeadsDataCutQuery leadsDataCutQuery)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._progressPublisher = progressPublisher ?? throw new ArgumentNullException(nameof(progressPublisher));
            this._addToCampaignCommand = addRulesToCampaignCommand ?? throw new ArgumentNullException(nameof(addRulesToCampaignCommand));
            this._addLeadsToCampaignCommand = addLeadsToCampaignCommand ?? throw new ArgumentNullException(nameof(addLeadsToCampaignCommand));
            this._updateCampaignCountersCommand = updateCampaignCountersCommand ?? throw new ArgumentNullException(nameof(updateCampaignCountersCommand));
            this._removeCampaignLeadsCommand = removeCampaignLeadsCommand ?? throw new ArgumentNullException(nameof(removeCampaignLeadsCommand));
            this._removeCampaignRulesCommand = removeCampaignRulesCommand ?? throw new ArgumentNullException(nameof(removeCampaignRulesCommand));

            this._campaignExistsQuery = campaignExistsQuery ?? throw new ArgumentNullException(nameof(campaignExistsQuery));
            this._leadsDataCutQuery = leadsDataCutQuery ?? throw new ArgumentNullException(nameof(leadsDataCutQuery));
        }

        public void Handle(ApplyRulesCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(ApplyRulesCommand message, CancellationToken cancellationToken = default)
        {
            bool exists = await this._campaignExistsQuery.ExecuteAsync(message.CampaignId, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new NotFoundException($"Campaign with id '{message.CampaignId}' does not exist");
            }

            var ruleIds = message.RuleIds.ToList();

            await this._progressPublisher.PostProgressAsync(5, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(20), TransactionScopeAsyncFlowOption.Enabled))
            {
                // Uansign all previously assigned leads from campaign
                await this._removeCampaignLeadsCommand.DispatchAsync(message.CampaignId, message.CreatedBy, cancellationToken).ConfigureAwait(false);

                // Remove all previously assigned rules from campaign
                await this._removeCampaignRulesCommand.DispatchAsync(message.CampaignId, message.CreatedBy, cancellationToken).ConfigureAwait(false);

                // Set operation progres to 25 percent
                await this._progressPublisher.PostProgressAsync(25, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                // Attach assigned rules to campaign
                if (ruleIds.Any())
                {
                    await this._addToCampaignCommand.DispatchAsync(new AddRulesToCampaignModel
                    {
                        CampaignId = message.CampaignId,
                        CampaignRuleIds = ruleIds,
                    }, message.CreatedBy,
                       cancellationToken).ConfigureAwait(false);

                    this._logger.LogInformation("{RulesCount} rules have been assigned to the campaign '{CampaignId}'", ruleIds.Count, message.CampaignId);
                }

                // Set operation progres to 35 percent
                await this._progressPublisher.PostProgressAsync(35, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                // Get Lead`s that satisfied rule criterias
                IEnumerable<LeadsDataCutModel> leadsDataCut = await this._leadsDataCutQuery.ExecuteAsync(message.CampaignId, cancellationToken).ConfigureAwait(false);

                List<Guid> leadsIds = leadsDataCut.Select(x => x.Id).ToList();
                decimal totalAmount = leadsDataCut.Sum(x => x.TotalAmountDue) ?? 0;

                // Set operation progres to 70 percent
                await this._progressPublisher.PostProgressAsync(70, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                this._logger.LogInformation("{LeadsCount} leads have been discovered by data-cut algorithm for the campaign '{CampaignId}'", leadsIds.Count, message.CampaignId);

                // Attach leads to campaign
                if (leadsIds.Any())
                {
                    await this._addLeadsToCampaignCommand.DispatchAsync(new AddLeadsToCampaignModel
                    {
                        CampaignId = message.CampaignId,
                        CampaignLeadsIds = leadsIds,
                    }, message.CreatedBy,
                       cancellationToken).ConfigureAwait(false);

                    this._logger.LogInformation("{LeadsCount} leads have been assigned to the campaign '{CampaignId}' with total amount {TotalAmount}", leadsIds.Count, message.CampaignId, totalAmount);
                }

                // Set operation progres to 85 percent
                await this._progressPublisher.PostProgressAsync(85, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();

                // Updated campaign statistic information
                await this._updateCampaignCountersCommand.DispatchAsync(new UpdateCampaignCountersModel
                {
                    Id = message.CampaignId,
                    TotalRecords = leadsIds.Count,
                    TotalRecordsAmount = totalAmount,
                }, message.CreatedBy,
                   cancellationToken).ConfigureAwait(false);

                await this._progressPublisher.PostProgressAsync(100, cancellationToken).ConfigureAwait(false);
                scope.Complete();

                this._logger.LogInformation("An apply rules have finished for the campaign '{CampaignId}'", message.CampaignId);
            }
        }

        public void Handle(DeleteRulesCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(DeleteRulesCommand message, CancellationToken cancellationToken = default)
        {
            var exists = await this._campaignExistsQuery.ExecuteAsync(message.CampaignId, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new NotFoundException($"Campaign with id '{message.CampaignId}' does not exist");
            }

            using (var scope = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(20), TransactionScopeAsyncFlowOption.Enabled))
            {
                await this._removeCampaignLeadsCommand.DispatchAsync(message.CampaignId, message.CreatedBy, cancellationToken).ConfigureAwait(false);

                await this._progressPublisher.PostProgressAsync(50, cancellationToken).ConfigureAwait(false);

                await this._updateCampaignCountersCommand.DispatchAsync(new UpdateCampaignCountersModel
                {
                    Id = message.CampaignId,
                    TotalRecords = 0,
                    TotalRecordsAmount = 0,
                }, message.CreatedBy,
                   cancellationToken).ConfigureAwait(false);

                scope.Complete();
                this._logger.LogInformation("A delete rules have finished for the campaign '{CampaignId}'", message.CampaignId);
            }
        }
    }
}
