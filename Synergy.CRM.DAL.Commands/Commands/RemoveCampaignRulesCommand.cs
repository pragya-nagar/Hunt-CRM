using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class RemoveCampaignRulesCommand : IRemoveCampaignRulesCommand
    {
        private readonly ISynergyContext _context;

        public RemoveCampaignRulesCommand(ISynergyContext context)
        {
            this._context = context;
        }

        public void Dispatch(Guid campaignId, Guid userId)
        {
            this.DispatchAsync(campaignId, userId).Wait();
        }

        public async Task<int> DispatchAsync(Guid campaignId, Guid userId, CancellationToken cancellationToken = default)
        {
            var items = await this._context.CampaignRuleCampaign.Where(x => x.CampaignId == campaignId).ToListAsync(cancellationToken).ConfigureAwait(false);
            this._context.CampaignRuleCampaign.RemoveRange(items);

            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
