using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synergy.Common.Abstracts;
using Synergy.Common.Domain.Models.Common;
using Synergy.Common.FileStorage.Abstraction;
using Synergy.Common.Security.Attributes;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;
using Synergy.DataAccess.Enum;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class CampaignsController : Controller
    {
        private readonly IClockService _clockService;
        private readonly IMapper _mapper;
        private readonly IPublishMessage _serviceBus;
        private readonly IFileStorage _fileStorage;
        private readonly ICurrentUserService _currentUserService;

        private readonly ICampaignService _campaignService;

        public CampaignsController(IClockService clockService, IMapper mapper, IPublishMessage serviceBus, IFileStorage fileStorage, ICurrentUserService currentUserService, ICampaignService campaignService)
        {
            this._clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            this._fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

            this._campaignService = campaignService ?? throw new ArgumentNullException(nameof(campaignService));
        }

        [CheckPermission("CRM.Campaigns.Read")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<CampaignModel>), 200)]
        public async Task<IActionResult> Get([FromQuery]SearchArgsModel<CampaignFilterArgs, CampaignSortField> args, CancellationToken cancellationToken = default)
        {
            var result = await this._campaignService.GetListAsync(args, cancellationToken).ConfigureAwait(false);
            return this.Ok(result);
        }

        [CheckPermission("CRM.Campaigns.Read")]
        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(CampaignDetailsModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._campaignService.FindAsync(id, cancellationToken).ConfigureAwait(false);
            return this.Ok(item);
        }

        [CheckPermission("CRM.Campaigns.Write")]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 202)]
        public async Task<IActionResult> Post([FromBody]CampaignCreateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<CampaignCreateCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id = command.Id }, command.Id);
        }

        [CheckPermission("CRM.Campaigns.Write")]
        [Route("{id:guid}")]
        [HttpPut]
        [ProducesResponseType(202)]
        public async Task<IActionResult> Put([FromRoute]Guid id, [FromBody]CampaignUpdateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<CampaignUpdateCommand>(id, this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id });
        }

        [CheckPermission("CRM.CampaignComments.Read")]
        [HttpGet]
        [Route("{id:guid}/comments")]
        [ProducesResponseType(typeof(SearchResultModel<CampaignCommentModel>), 200)]
        public async Task<IActionResult> GetComments([FromRoute] Guid id, [FromQuery] SearchArgsModel args, CancellationToken cancellationToken = default)
        {
            var searchArgs = new SearchArgsModel<Guid, CommentSortField> { Filter = id, Limit = args.Limit, Offset = args.Offset };
            var comments = await this._campaignService.GetCommentsListAsync(searchArgs, cancellationToken).ConfigureAwait(false);
            return this.Ok(comments);
        }

        [CheckPermission("CRM.CampaignComments.Write")]
        [HttpPost]
        [Route("{id:guid}/comments")]
        [ProducesResponseType(202)]
        public async Task<IActionResult> PostComments([FromRoute]Guid id, [FromBody]string comment, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<CampaignCommentCreateCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            command.CampaignId = id;
            command.Comment = comment;

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction(nameof(this.GetComments), new { id }, command.Id);
        }

        [CheckPermission("CRM.CampaignComments.Write")]
        [HttpPut]
        [Route("{id:guid}/comments/{commentId:guid}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> PutComments([FromRoute]Guid id, [FromRoute]Guid commentId, [FromBody]string comment, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return this.BadRequest(new { comment = "Comment is required" });
            }

            var commentEntity = await this._campaignService.GetCommentAsync(id, commentId, cancellationToken).ConfigureAwait(false);

            if (commentEntity == null)
            {
                return this.NotFound(new { commentId = "Comment is not found" });
            }

            if (commentEntity.Author.Id != this._currentUserService.UserId)
            {
                return this.Forbid();
            }

            var command = Command.Create<CampaignCommentUpdateCommand>(commentId, this._currentUserService.UserId);
            command.Comment = comment;

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction(nameof(this.GetComments), new { id }, commentId);
        }

        [CheckPermission("CRM.CampaignComments.Write")]
        [HttpDelete]
        [Route("{id:guid}/comments/{commentId:guid}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> DeleteComment([FromRoute]Guid id, [FromRoute]Guid commentId, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<CampaignCommentDeleteCommand>(commentId, this._currentUserService.UserId);

            var commentEntity = await this._campaignService.GetCommentAsync(id, commentId, cancellationToken).ConfigureAwait(false);

            if (commentEntity == null)
            {
                return this.NotFound(new { commentId = "Comment is not found" });
            }

            if (commentEntity.Author.Id != this._currentUserService.UserId)
            {
                return this.Forbid();
            }

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.Accepted();
        }

        [CheckPermission("CRM.Campaigns.Write")]
        [HttpGet]
        [Route("{id:guid}/rules")]
        [ProducesResponseType(typeof(SearchResultModel<RuleModel>), 200)]
        public async Task<IActionResult> GetRules([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var items = await this._campaignService.GetRuleListAsync(id, cancellationToken).ConfigureAwait(false);
            return this.Ok(items);
        }

        [CheckPermission("CRM.Campaigns.Write")]
        [Route("rules")]
        [HttpPost]
        [ProducesResponseType(202)]
        public async Task<IActionResult> PostRules([FromBody]CreateRuleArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<CreateRuleCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            this._mapper.Map(args, command);
            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.Accepted(command.Id);
        }

        [CheckPermission("CRM.Campaigns.Write")]
        [Route("{id:guid}/rules")]
        [HttpPut]
        [ProducesResponseType(202)]
        public async Task<IActionResult> PutRules([FromRoute]Guid id, [FromBody]IEnumerable<Guid> args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<ApplyRulesCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            command.CampaignId = id;
            command.RuleIds = args;
            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.Accepted();
        }

        [CheckPermission("CRM.Campaigns.Write")]
        [Route("{id:guid}/rules")]
        [HttpDelete]
        [ProducesResponseType(202)]
        public async Task<IActionResult> DeleteRules([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<DeleteRulesCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            command.CampaignId = id;
            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.Accepted();
        }

        [CheckPermission("CRM.CampaignsDataDump.Read")]
        [Route("{id:guid}/dump/{context}/fields")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<string>), 200)]
        public async Task<IActionResult> GetDumpFields([FromRoute]Guid id, [FromRoute]CampaignDumpContext context, CancellationToken cancellationToken = default)
        {
            var res = await this._campaignService.GetDumpFieldsAsync(id, context, cancellationToken).ConfigureAwait(false);
            return this.Ok(res);
        }

        [CheckPermission("CRM.CampaignsDataDump.Read")]
        [HttpPost]
        [Route("{id:guid}/dump/{context}")]
        [ProducesResponseType(typeof(string), 202)]
        public async Task<IActionResult> PostDump([FromRoute]Guid id, [FromRoute]CampaignDumpContext context, [FromBody]IEnumerable<CampaignDumpArgs> args, CancellationToken cancellationToken = default)
        {
            var contextStr = $"{context:G}".ToLower(CultureInfo.InvariantCulture);

            var timestamp = this._clockService.UtcNow
                                .ToString("[yyyy-MM-dd_HH-mm-ss]", CultureInfo.InvariantCulture)
                                .ToLower(CultureInfo.InvariantCulture);

            var command = Command.Create<MakeCampaignDataDumpCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            command.CampaignId = id;
            command.Key = $"campaigns:{id}:dumps:{contextStr}:dump_campaign_{contextStr}_{timestamp}";
            command.Fields = args.Select(x => new CampaignField
            {
                Key = x.Key,
                Order = x.Order,
                Alias = x.Alias,
            });

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("GetDumpUrl", new { id, key = command.Key }, command.Key);
        }

        [CheckPermission("CRM.CampaignsDataDump.Read")]
        [Route("{id:guid}/dump/{key}")]
        [HttpGet]
        [ProducesResponseType(typeof(ObjectAccessModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDumpUrl([FromRoute]string key, CancellationToken cancellationToken = default)
        {
            var list = await this._fileStorage.GetAccessAsync(key.Replace(":", "/", StringComparison.OrdinalIgnoreCase), cancellationToken).ConfigureAwait(false);

            var item = list.FirstOrDefault();
            if (item == null)
            {
                return this.NotFound();
            }

            return this.Ok(item);
        }
    }
}
