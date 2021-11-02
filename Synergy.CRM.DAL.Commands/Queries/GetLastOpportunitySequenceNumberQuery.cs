using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class GetLastOpportunitySequenceNumberQuery : SingleQuery<Guid?, int>
    {
        private readonly ISynergyContext _context;

        public GetLastOpportunitySequenceNumberQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<int> ExecuteAsync(Guid? dummyId, CancellationToken cancellationToken = default)
        {
            return await this._context.Opportunity.AsNoTracking().Where(x => x.CreatedOn.Month == DateTime.Now.Month && x.CreatedOn.Year == DateTime.Now.Year).CountAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}