using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class CampaignExistsQuery : SingleQuery<Guid, bool>
    {
        private readonly ISynergyContext _context;

        public CampaignExistsQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<bool> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Campaign.AsNoTracking().AnyAsync(x => x.Id == id && x.DeletedOn == null, cancellationToken).ConfigureAwait(false);
        }
    }
}
