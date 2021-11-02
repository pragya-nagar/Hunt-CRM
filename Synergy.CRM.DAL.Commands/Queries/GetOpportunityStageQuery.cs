using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Models.Results.Opportunity;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class GetOpportunityStageQuery : SingleQuery<Guid, OpportunityStageDetailsModel>
    {
        private readonly ISynergyContext _context;

        public GetOpportunityStageQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<OpportunityStageDetailsModel> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var opportunity = await _context.Opportunity.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedOn == null, cancellationToken)
                .ConfigureAwait(false);

            return new OpportunityStageDetailsModel
            {
                Id = opportunity.Id,
                OpportunityNumber = opportunity.OpportunityNumber,
                OpportunityStageId = opportunity.OpportunityStageId,
            };
        }
    }
}
