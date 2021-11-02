namespace Synergy.CRM.API.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Synergy.Common.Abstracts;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.Common.Security.Attributes;
    using Synergy.CRM.Domain.Abstracts;
    using Synergy.CRM.Models;
    using Synergy.CRM.Models.Commands;
    using Synergy.DataAccess.Enum;
    using Synergy.ServiceBus.Abstracts;

    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class ContactsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPublishMessage _serviceBus;
        private readonly ICurrentUserService _currentUserService;

        private readonly IContactService _contactService;

        public ContactsController(IMapper mapper, IPublishMessage serviceBus, ICurrentUserService currentUserService, IContactService contactService)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

            this._contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
        }

        [CheckPermission("CRM.Contacts.Read")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<ContactModel>), 200)]
        public async Task<IActionResult> Get([FromQuery]SearchArgsModel<ContactFilterArgs, ContactSortField> args, CancellationToken cancellationToken = default)
        {
            var result = await this._contactService.GetListAsync(args, cancellationToken).ConfigureAwait(false);
            return this.Ok(result);
        }

        [CheckPermission("CRM.Contacts.Read")]
        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ContactDetailsModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._contactService.FindAsync(id, cancellationToken).ConfigureAwait(false);
            return this.Ok(item);
        }

        [CheckPermission("CRM.Contacts.Write")]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]ContactCreateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<ContactCreateCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            var result = this.AcceptedAtAction("Get", new { id = command.Id }, command.Id);
            return result;
        }

        [CheckPermission("CRM.Contacts.Write")]
        [Route("{id:guid}")]
        [HttpPut]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody]ContactUpdateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<ContactUpdateCommand>(id, this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id });
        }
    }
}
