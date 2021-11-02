using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synergy.Common.Abstracts;
using Synergy.Common.Domain.Models.Common;
using Synergy.Common.Security.Attributes;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models.Commands.Opportunity;
using Synergy.CRM.Models.Opportunity;
using Synergy.DataAccess.Enum;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public class OpportunitiesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPublishMessage _serviceBus;
        private readonly ICurrentUserService _currentUserService;
        private readonly IOpportunityService _opportunityService;

        public OpportunitiesController(IMapper mapper, IPublishMessage serviceBus, ICurrentUserService currentUserService, IOpportunityService opportunityService)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._serviceBus = serviceBus ?? throw new ArgumentNullException(nameof(serviceBus));
            this._currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));

            this._opportunityService = opportunityService ?? throw new ArgumentNullException(nameof(opportunityService));
        }

        [CheckPermission("CRM.Opportunities.Read")]
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultModel<OpportunityModel>), 200)]
        public async Task<IActionResult> Get([FromQuery]SearchArgsModel<OpportunityFilterArgs, OpportunitySortField> args, CancellationToken cancellationToken = default)
        {
            var result = await this._opportunityService.GetListAsync(args, cancellationToken).ConfigureAwait(false);
            return this.Ok(result);
        }

        [CheckPermission("CRM.Opportunities.Read")]
        [Route("{id:guid}")]
        [HttpGet]
        [ProducesResponseType(typeof(OpportunityDetailsModel), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Get([FromRoute]Guid id, CancellationToken cancellationToken = default)
        {
            var item = await this._opportunityService.FindAsync(id, cancellationToken).ConfigureAwait(false);
            return this.Ok(item);
        }

        [CheckPermission("CRM.Opportunities.Write")]
        [HttpPost]
        [ProducesResponseType(typeof(Guid), 202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]OpportunityCreateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<OpportunityCreateCommand>(Guid.NewGuid(), this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id = command.Id }, command.Id);
        }

        [CheckPermission("CRM.Opportunities.Write")]
        [Route("{id:guid}")]
        [HttpPatch]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Patch([FromRoute]Guid id, [FromBody]OpportunityUpdateArgs args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<OpportunityUpdateCommand>(id, this._currentUserService.UserId);
            this._mapper.Map(args, command);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id });
        }

        [CheckPermission("CRM.OpportunitySensitiveData.Write")]
        [Route("{id:guid}/borrower/sensitivedata")]
        [HttpPatch]
        [ProducesResponseType(typeof(Guid), 202)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PatchSensitiveData([FromRoute]Guid id, [FromBody] List<Models.Opportunity.OpportunitySensitiveDataUpdateArgs> args, CancellationToken cancellationToken = default)
        {
            var command = Command.Create<OpportunitySensitiveDataUpdateCommand>(id, this._currentUserService.UserId);
            command.OpportunityId = id;
            command.BorrowersSensitiveData = new List<Models.Commands.Opportunity.OpportunitySensitiveDataUpdateArgs>();
            this._mapper.Map(args, command.BorrowersSensitiveData);

            await this._serviceBus.PublishAsync(command, cancellationToken).ConfigureAwait(false);

            return this.AcceptedAtAction("Get", new { id = command.Id }, command.Id);
        }

        [CheckPermission("CRM.OpportunitySensitiveData.Read")]
        [Route("{id:guid}/borrower/{borrowerId:guid}/sensitivedata")]
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetSensitiveData([FromRoute]Guid id, [FromRoute]Guid borrowerId, [FromQuery] OpportunitySensitiveDataField field, CancellationToken cancellationToken = default)
        {
            var item = await this._opportunityService.GetSensitiveData(id, borrowerId, field, cancellationToken).ConfigureAwait(false);
            return this.Ok(item);
        }

        [CheckPermission("CRM.Opportunities.Read")]
        [HttpGet("ExportToPdf")]
        [ProducesResponseType(200)]
        public FileResult GetPdf(string opportunityNo, CancellationToken cancellationToken = default)
        {
            var result = this._opportunityService.GetPdfToExport(opportunityNo, cancellationToken);

            StringReader stringBuilder = new StringReader(result.ToString());

            Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            HTMLWorker htmlParser = new HTMLWorker(pdfDoc);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                pdfDoc.Open();
                htmlParser.Parse(stringBuilder);
                pdfDoc.Close();
                byte[] bytes = memoryStream.ToArray();
                memoryStream.Close();
                stringBuilder.Dispose();
                Response.Clear();
                return File(bytes, "application/pdf");
            }
        }
    }
}
