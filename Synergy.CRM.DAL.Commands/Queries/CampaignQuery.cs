using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Models.Results;
using Synergy.DataAccess.Abstractions.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Queries
{
    public class CampaignQuery : SingleQuery<Guid, CampaignModel>
    {
        private readonly ISynergyContext _context;

        public CampaignQuery(ISynergyContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public override async Task<CampaignModel> ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Campaign.AsNoTracking()
                .Where(x => x.Id == id && x.DeletedOn == null)
                .Select(x => new CampaignModel
                {
                    Id = x.Id,
                    Name = x.CampaignName,
                })
                .SingleOrDefaultAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
