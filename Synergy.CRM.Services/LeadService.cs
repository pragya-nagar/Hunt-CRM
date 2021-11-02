using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.CRM.DAL.Commands.Queries;
using Synergy.CRM.Models.Commands;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Services
{
    public class LeadService :
        IMessageHandler<LeadCommentCreateCommand>,
        IMessageHandler<LeadCommentUpdateCommand>,
        IMessageHandler<LeadCommentDeleteCommand>
    {
        private readonly IMapper _mapper;
        private readonly ICreateLeadCommentCommand _createLeadCommentCommand;
        private readonly IUpdateLeadCommentCommand _updateLeadCommentCommand;
        private readonly IDeleteLeadCommentCommand _deleteLeadCommentCommand;
        private readonly GetLeadCommentAuthorIdQuery _getLeadCommentAuthorIdQuery;

        public LeadService(IMapper mapper,
                           ICreateLeadCommentCommand createLeadCommentCommand,
                           IUpdateLeadCommentCommand updateLeadCommentCommand,
                           IDeleteLeadCommentCommand deleteLeadCommentCommand,
                           GetLeadCommentAuthorIdQuery getLeadCommentAuthorIdQuery)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._createLeadCommentCommand = createLeadCommentCommand ?? throw new ArgumentNullException(nameof(createLeadCommentCommand));
            this._updateLeadCommentCommand = updateLeadCommentCommand ?? throw new ArgumentNullException(nameof(updateLeadCommentCommand));
            this._deleteLeadCommentCommand = deleteLeadCommentCommand ?? throw new ArgumentNullException(nameof(deleteLeadCommentCommand));
            this._getLeadCommentAuthorIdQuery = getLeadCommentAuthorIdQuery ?? throw new ArgumentNullException(nameof(getLeadCommentAuthorIdQuery));
        }

        public void Handle(LeadCommentCreateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(LeadCommentCreateCommand message, CancellationToken cancellationToken = default)
        {
            var cmd = this._mapper.Map<CreateLeadCommentModel>(message);

            await this._createLeadCommentCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(LeadCommentUpdateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(LeadCommentUpdateCommand message, CancellationToken cancellationToken = default)
        {
            var author = await this._getLeadCommentAuthorIdQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);

            if (author == null)
            {
                throw new NotFoundException();
            }

            if (author != message.CreatedBy)
            {
                throw new NotAcceptableException("Only author can alter comment.");
            }

            await this._updateLeadCommentCommand.DispatchAsync(new UpdateLeadCommentModel
                    {
                        Id = message.Id,
                        Comment = message.Comment,
                    }, message.CreatedBy,
                    cancellationToken)
                .ConfigureAwait(false);
        }

        public void Handle(LeadCommentDeleteCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(LeadCommentDeleteCommand message, CancellationToken cancellationToken = default)
        {
            var author = await this._getLeadCommentAuthorIdQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);

            if (author == null)
            {
                throw new NotFoundException();
            }

            if (author != message.CreatedBy)
            {
                throw new NotAcceptableException("Only author can delete comment.");
            }

            var command = new DeleteLeadCommentModel { Id = message.Id };
            await this._deleteLeadCommentCommand
                .DispatchAsync(command, message.CreatedBy, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}