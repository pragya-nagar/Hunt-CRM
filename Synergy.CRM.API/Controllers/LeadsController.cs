using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synergy.Common.Abstracts;
using Synergy.Common.Domain.Models.Common;
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
    public class LeadsController : Controller
    {
        private readonly IPublishMessage _serviceBus;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILeadService _leadService;

        public LeadsController(IPublishMessage serviceBus, ICurrentUserService currentUserService, ILeadService leadService)
        {
            this._serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

            this._leadService = leadService;
        }

        [CheckPermission("CRM.Records.Read")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<LeadModel>), 200)]
        public async Task<IActionResult> Get([FromQuery]SearchArgsModel<LeadSortField> args, CancellationToken cancellationToken = default)
        {
            var result = await this._leadService.GetListAsync(args, cancellationToken).ConfigureAwait(false);
            return this.Ok(result);
        }

        [CheckPermission("CRM.Records.Read")]
        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(LeadDetailsModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._leadService.FindAsync(id, cancellationToken).ConfigureAwait(false);
            return this.Ok(item);
        }

        [CheckPermission("CRM.RecordComments.Read")]
        [HttpGet]
        [Route("{id:guid}/comments")]
        [ProducesResponseType(typeof(SearchResultModel<LeadCommentModel>), 200)]
        public async Task<IActionResult> GetComments([FromRoute] Guid id, [FromQuery] SearchArgsModel args, CancellationToken cancellationToken = default)
        {
            var searchArgs = new SearchArgsModel<Guid, CommentSortField> { Filter = id, Limit = args.Limit, Offset = args.Offset };
            var comments = await this._leadService.GetCommentsListAsync(searchArgs, cancellationToken).ConfigureAwait(false);
            return this.Ok(comments);
        }

        [CheckPermission("CRM.RecordComments.Write")]
        [HttpPost]
        [Route("{id:guid}/comments")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostComments([FromRoute]Guid id, [FromBody]string comment, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<LeadCommentCreateCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            command.LeadId = id;
            command.Comment = comment;

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction(nameof(this.GetComments), new { id }, command.Id);
        }

        [CheckPermission("CRM.RecordComments.Write")]
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

            var commentEntity = await this._leadService.GetCommentAsync(id, commentId, cancellationToken).ConfigureAwait(false);

            if (commentEntity == null)
            {
                return this.NotFound(new { commentId = "Comment is not found" });
            }

            if (commentEntity.Author.Id != this._currentUserService.UserId)
            {
                return this.Forbid();
            }

            var command = Command.Create<LeadCommentUpdateCommand>(commentId, this._currentUserService.UserId);
            command.Comment = comment;

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction(nameof(this.GetComments), new { id }, commentId);
        }

        [CheckPermission("CRM.RecordComments.Write")]
        [HttpDelete]
        [Route("{id:guid}/comments/{commentId:guid}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> DeleteComment([FromRoute]Guid id, [FromRoute]Guid commentId, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<LeadCommentDeleteCommand>(commentId, this._currentUserService.UserId);

            var commentEntity = await this._leadService.GetCommentAsync(id, commentId, cancellationToken).ConfigureAwait(false);

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
    }
}
