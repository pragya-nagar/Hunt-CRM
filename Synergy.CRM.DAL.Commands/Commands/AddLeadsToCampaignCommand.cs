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
    public class AddLeadsToCampaignCommand : IAddLeadsToCampaignCommand
    {
        private readonly ISynergyContext _context;

        public AddLeadsToCampaignCommand(ISynergyContext context)
        {
            this._context = context;
        }

        public void Dispatch(AddLeadsToCampaignModel entity, Guid userId)
        {
            this.DispatchAsync(entity, userId).Wait();
        }

        public async Task<int> DispatchAsync(AddLeadsToCampaignModel entity, Guid userId, CancellationToken cancellationToken = default)
        {
            var campaignId = entity.CampaignId;
            var list = entity.CampaignLeadsIds.Select(x => new CampaignLead()
            {
                Id = Guid.NewGuid(),
                CampaignId = campaignId,
                LeadId = x,
            }.OnCreateAudit(userId));

            this._context.CampaignLead.AddRange(list);

            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
