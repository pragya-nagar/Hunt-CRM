namespace Synergy.CRM.Services
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Synergy.Common.Exceptions;
    using Synergy.CRM.DAL.Commands.Interfaces;
    using Synergy.CRM.DAL.Commands.Queries;
    using Synergy.CRM.Models.Commands;
    using Synergy.ServiceBus.Abstracts;

    public class ContactService :
        IMessageHandler<ContactCreateCommand>,
        IMessageHandler<ContactUpdateCommand>
    {
        private readonly IMapper _mapper;
        private readonly ICreateContactCommand _createContactCommand;
        private readonly IUpdateContactCommand _updateContactCommand;
        private readonly ContactExistsQuery _contactExistsQuery;

        public ContactService(IMapper mapper,
            ICreateContactCommand createContactCommand,
            IUpdateContactCommand updateContactCommand,
            ContactExistsQuery contactExistsQuery)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._createContactCommand = createContactCommand ?? throw new ArgumentNullException(nameof(createContactCommand));
            this._updateContactCommand = updateContactCommand ?? throw new ArgumentNullException(nameof(updateContactCommand));
            this._contactExistsQuery = contactExistsQuery ?? throw new ArgumentNullException(nameof(contactExistsQuery));
        }

        public void Handle(ContactCreateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(ContactCreateCommand message, CancellationToken cancellationToken = default)
        {
            var cmd = this._mapper.Map<DAL.Commands.Models.CreateContactModel>(message);
            await this._createContactCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
        }

        public void Handle(ContactUpdateCommand message)
        {
            this.HandleAsync(message).Wait();
        }

        public async Task HandleAsync(ContactUpdateCommand message, CancellationToken cancellationToken = default)
        {
            var exists = await this._contactExistsQuery.ExecuteAsync(message.Id, cancellationToken).ConfigureAwait(false);
            if (exists == false)
            {
                throw new NotFoundException($"Contact with id '{message.Id}' does not exist");
            }

            var cmd = this._mapper.Map<DAL.Commands.Models.UpdateContactModel>(message);
            await this._updateContactCommand.DispatchAsync(cmd, message.CreatedBy, cancellationToken).ConfigureAwait(false);
        }
    }
}
