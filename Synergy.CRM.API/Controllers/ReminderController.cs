using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Synergy.Common.Abstracts;
using Synergy.Common.Domain.Models.Common;
using Synergy.Common.Security.Attributes;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands.Reminder;
using Synergy.CRM.Models.Reminder;
using Synergy.DataAccess.Enum;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class ReminderController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPublishMessage _serviceBus;
        private readonly ICurrentUserService _currentUserService;
        private readonly IReminderService _reminderService;

        public ReminderController(IMapper mapper, IPublishMessage serviceBus, ICurrentUserService currentUserService, IReminderService reminderService)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._reminderService = reminderService;
        }

        [CheckPermission("CRM.Reminder.Read")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<ReminderDetails>), 200)]
        public async Task<IActionResult> Get([FromQuery]SearchArgsModel<ReminderFilterArgs, ReminderSortField> args, CancellationToken cancellationToken = default)
        {
            var result = await this._reminderService.GetListAsync(args, cancellationToken).ConfigureAwait(false);
            return this.Ok(result);
        }

        [CheckPermission("CRM.Reminder.Read")]
        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(ReminderDetails), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._reminderService.FindAsync(id, cancellationToken).ConfigureAwait(false);
            return this.Ok(item);
        }

        [CheckPermission("CRM.Reminder.Write")]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 202)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Post([FromBody]ReminderCreateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<ReminderCreateCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id = command.Id }, command.Id);
        }

        [Route("{id:guid}")]
        [HttpPatch]
        [ProducesResponseType(typeof(Guid), 202)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> PatchNotification([FromRoute]Guid id, [FromBody] ReminderUpdateNotificationArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<ReminderUpdateNotificationCommand>(id, this._currentUserService.UserId);
            this._mapper.Map(args, command);
            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id });
        }

        [Route("{id:guid}")]
        [HttpPut]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody]ReminderUpdateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<ReminderUpdateCommand>(id, this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id });
        }
    }
}