﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synergy.Common.Abstracts;
using Synergy.Common.FileStorage.Abstraction;
using Synergy.Common.Security.Attributes;
using Synergy.CRM.Models;
using Synergy.CRM.Models.Commands;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class MailMergeController : Controller
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IPublishMessage _publisher;
        private readonly IFileStorage _fileStorage;

        public MailMergeController(ICurrentUserService currentUserService, IPublishMessage publisher, IFileStorage fileStorage)
        {
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            this._publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
            this._fileStorage = fileStorage ?? throw new ArgumentNullException(nameof(fileStorage));
        }

        [Route("properties")]
        [HttpGet]
        [ProducesResponseType(typeof(ImportMetadataModel), 200)]
        [ProducesDefaultResponseType]
        [CheckPermission("CRM.MailMerge.Read")]
        public async Task<IActionResult> GetImportUrl([FromQuery] Guid campaignId, CancellationToken cancellationToken = default)
        {
            var uploadId = FileId.Generate(campaignId, "properties");

            var uploadUrl = await this._fileStorage.GetUploadUrlAsync(uploadId.FileName, cancellationToken).ConfigureAwait(false);

            return this.Ok(new ImportMetadataModel
            {
                Id = uploadId.Id,
                UploadUrl = uploadUrl,
            });
        }

        [Route("{fileId}")]
        [HttpGet]
        [ProducesResponseType(typeof(ImportMetadataModel), 200)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [CheckPermission("CRM.MailMerge.Read")]
        public async Task<IActionResult> GetUploadUrl(string fileId, CancellationToken cancellationToken = default)
        {
            var file = FileId.Parse(fileId);

            var access = await this._fileStorage.GetAccessAsync(file.FileName, cancellationToken).ConfigureAwait(false);

            var item = access.FirstOrDefault();
            if (item == null)
            {
                return this.NotFound();
            }

            return this.Ok(item);
        }

        [Route("{templateId:guid}")]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 202)]
        [ProducesResponseType(400)]
        [CheckPermission("CRM.MailMerge.Read")]
        public async Task<ActionResult> Merge(Guid templateId, string propertyFileId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(propertyFileId))
            {
                return this.BadRequest(new { propertyFileId = "PropertyFileId is required." });
            }

            var propertyFile = FileId.Parse(propertyFileId);
            var resultFile = FileId.Generate(propertyFile.CampaignId, "mergeresult");

            var command = Command.Create<MailMergeCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            command.CampaignId = propertyFile.CampaignId;
            command.PropertyPath = propertyFile.FileName;
            command.TemplateId = templateId;
            command.ResultPath = resultFile.FileName;

            await this._publisher.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction(nameof(this.GetUploadUrl), new { fileId = resultFile.Id }, resultFile.Id);
        }
    }
}
