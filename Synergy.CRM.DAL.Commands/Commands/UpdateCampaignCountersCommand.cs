using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class UpdateCampaignCountersCommand : IUpdateCampaignCountersCommand
    {
        private readonly ISynergyContext _context;

        public UpdateCampaignCountersCommand(ISynergyContext context)
        {
            this._context = context;
        }

        public void Dispatch(UpdateCampaignCountersModel entity, Guid userId)
        {
            this.DispatchAsync(entity, userId).Wait();
        }

        public async Task<int> DispatchAsync(UpdateCampaignCountersModel entity, Guid userId, CancellationToken cancellationToken = default)
        {
            var campaign = await this._context.Campaign.SingleAsync(x => x.Id == entity.Id, cancellationToken).ConfigureAwait(false);
            campaign.OnModifyAudit(userId);
            campaign.TotalRecords = entity.TotalRecords;
            campaign.TotalAmountRecords = entity.TotalRecordsAmount;

            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
