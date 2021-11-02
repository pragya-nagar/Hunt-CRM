using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class AddRulesToCampaignCommand : IAddRulesToCampaignCommand
    {
        private readonly ISynergyContext _context;

        public AddRulesToCampaignCommand(ISynergyContext context)
        {
            this._context = context;
        }

        public void Dispatch(AddRulesToCampaignModel entity, Guid userId)
        {
            this.DispatchAsync(entity, userId).Wait();
        }

        public async Task<int> DispatchAsync(AddRulesToCampaignModel entity, Guid userId, CancellationToken cancellationToken = default)
        {
            var campaignId = entity.CampaignId;
            var list = entity.CampaignRuleIds.Select(x => new CampaignRuleCampaign
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                CampaignRuleId = x,
            }.OnCreateAudit(userId));

            this._context.CampaignRuleCampaign.AddRange(list);

            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
