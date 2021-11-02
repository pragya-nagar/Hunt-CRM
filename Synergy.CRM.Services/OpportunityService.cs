using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Queries;
using Synergy.CRM.Models.Commands.Opportunity;
using Synergy.ServiceBus.Abstracts;
using Synergy.ServiceBus.Messages.Events;

namespace Synergy.CRM.Services
{
    public class OpportunityService :
        IMessageHandler<OpportunityCreateCommand>,
        IMessageHandler<OpportunityUpdateCommand>,
        IMessageHandler<OpportunitySensitiveDataUpdateCommand>
    {
        private readonly ILogger _logger;
        private readonly IPublishMessage _serviceBus;
        private readonly IMapper _mapper;
        private readonly ICreateOpportunityCommand _createOpportunityCommand;
        private readonly IUpdateOpportunityCommand _updateOpportunityCommand;
        private readonly IUpdateOpportunitySensitiveDataCommand _updateOpportunitySensitiveDataCommand;

        private readonly UserExistsQuery _userExistsQuery;
        private readonly GetOpportunityStageQuery _opportunityStageQuery;
        private readonly GetLastOpportunitySequenceNumberQuery _getLastOpportunitySequenceNumberQuery;
        private readonly OpportunityExistsQuery _opportunityExistsQuery;

        public OpportunityService(ILogger<OpportunityService> logger,
            IPublishMessage serviceBus,
            IMapper mapper,
            ICreateOpportunityCommand createOpportunityCommand,
            IUpdateOpportunityCommand updateOpportunityCommand,
            IUpdateOpportunitySensitiveDataCommand updateOpportunitySensitiveDataCommand,
            UserExistsQuery userExistsQuery,
            GetOpportunityStageQuery opportunityStageQuery,
            GetLastOpportunitySequenceNumberQuery getLastOpportunitySequenceNumberQuery,
            OpportunityExistsQuery opportunityExistsQuery)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._createOpportunityCommand = createOpportunityCommand ?? throw new ArgumentNullException(nameof(createOpportunityCommand));
            this._updateOpportunityCommand = updateOpportunityCommand ?? throw new ArgumentNullException(nameof(updateOpportunityCommand));
            this._updateOpportunitySensitiveDataCommand = updateOpportunitySensitiveDataCommand ?? throw new ArgumentNullException(nameof(updateOpportunitySensitiveDataCommand));

            this._userExistsQuery = userExistsQuery ?? throw new ArgumentNullException(nameof(userExistsQuery));
            this._opportunityStageQuery = opportunityStageQuery ?? throw new ArgumentNullException(nameof(opportunityStageQuery));
            this._getLastOpportunitySequenceNumberQuery = getLastOpportunitySequenceNumberQuery ?? throw new ArgumentNullException(nameof(getLastOpportunitySequenceNumberQuery));
            this._opportunityExistsQuery = opportunityExistsQuery ?? throw new ArgumentNullException(nameof(opportunityExistsQuery));
        }

        public void Handle(OpportunityCreateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(OpportunityCreateCommand message, CancellationToken cancellationToken = default)
        {
            var cmd = this._mapper.Map<DAL.Commands.Models.CreateOpportunityModel>(message);
            cmd.OpportunityNumber = await this.GenerateOpportunityNumber(cancellationToken).ConfigureAwait(false);
            await this._createOpportunityCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);

            var stageChangedEvent = Event.Create<OpportunityStageChangedEvent>(message.Id, message.CreatedBy);
            stageChangedEvent.OpportunityNumber = cmd.OpportunityNumber;
            stageChangedEvent.PreviousStageId = null;
            stageChangedEvent.CurrentStageId = message.StageId;
            await this._serviceBus.PublishAsync(stageChangedEvent, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(OpportunityUpdateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(OpportunityUpdateCommand message, CancellationToken cancellationToken = default)
        {
            var opportunity = await this._opportunityStageQuery
                            .ExecuteAsync(message.Id, cancellationToken)
                            .ConfigureAwait(false) ?? throw new NotFoundException($"Opportunity with id '{message.Id}' does not exist");

            var exists = await this._userExistsQuery.ExecuteAsync(message.UserId, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new ModelStateException(nameof(message.UserId), $"User with id '{message.UserId}' does not exist");
            }

            var cmd = this._mapper.Map<DAL.Commands.Models.UpdateOpportunityModel>(message);

            await this._updateOpportunityCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);

            if (opportunity.OpportunityStageId != message.StageId)
            {
                var stageChangedEvent = Event.Create<OpportunityStageChangedEvent>(message.Id, message.CreatedBy);
                stageChangedEvent.OpportunityNumber = opportunity.OpportunityNumber;
                stageChangedEvent.PreviousStageId = opportunity.OpportunityStageId;
                stageChangedEvent.CurrentStageId = message.StageId;
                await this._serviceBus.PublishAsync(stageChangedEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        public void Handle(OpportunitySensitiveDataUpdateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(OpportunitySensitiveDataUpdateCommand message, CancellationToken cancellationToken = default)
        {
            var opportunityExists = await this._opportunityExistsQuery
                            .ExecuteAsync(message.OpportunityId, cancellationToken)
                            .ConfigureAwait(false);

            if (!opportunityExists)
            {
                throw new ModelStateException(nameof(message.Id), $"Opportunity with id '{message.Id}' does not exist");
            }

            var cmd = this._mapper.Map<DAL.Commands.Models.Opportunity.UpdateOpportunitySensitiveDataModel>(message);

            await this._updateOpportunitySensitiveDataCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
        }

        private async Task<string> GenerateOpportunityNumber(CancellationToken cancellationToken = default)
        {
            var date = DateTime.Now.ToString("yyMM", CultureInfo.CurrentCulture);

            var sequenceNumber = await this._getLastOpportunitySequenceNumberQuery.ExecuteAsync(Guid.Empty, cancellationToken).ConfigureAwait(false);

            return $"{date}{sequenceNumber.ToString("D4", CultureInfo.InvariantCulture)}";
        }
    }
}
