using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            this._historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
        }

        [HttpGet("opportunity/{id:guid}")]
        [ProducesResponseType(typeof(List<OpportunityHistoryModel>), 200)]
        public async Task<IActionResult> GetOpportunityHistory(Guid id, [FromQuery]OpportunityHistoryFilterArgs filterArgs, CancellationToken cancellationToken = default)
        {
            var res = await this._historyService.GetOpportunityHistoryAsync(id, filterArgs, cancellationToken).ConfigureAwait(false);

            return this.Ok(res);
        }
    }
}