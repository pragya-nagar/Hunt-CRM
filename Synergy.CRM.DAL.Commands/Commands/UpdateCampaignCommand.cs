using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class UpdateCampaignCommand : IUpdateCampaignCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public UpdateCampaignCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(UpdateCampaignModel entity, Guid userId)
        {
            this.UpdateCampaign(entity, userId);
            this._context.SaveChanges();
        }

        public Task<int> DispatchAsync(UpdateCampaignModel entity, Guid userId, CancellationToken cancellationToken = default)
        {
            this.UpdateCampaign(entity, userId);
            return this._context.SaveChangesAsync(cancellationToken);
        }

        private void UpdateCampaign(UpdateCampaignModel entity, Guid userId)
        {
            var campaign = this._context.Campaign.Include(c => c.CampaignCounty)
                .Single(x => x.Id == entity.Id).OnModifyAudit(userId);

            List<CampaignCounty> counties = campaign.CampaignCounty.ToList();

            foreach (var toRemove in counties.Where(cc => !entity.CountyIds.Contains(cc.CountyId)).ToList())
            {
                counties.Remove(toRemove);
            }

            foreach (var countyId in entity.CountyIds.Where(cc => !counties.Any(c => c.CountyId == cc)).ToList())
            {
                counties.Add(new CampaignCounty { Id = Guid.NewGuid(), CampaignId = campaign.Id, CountyId = countyId }.OnCreateAudit(userId));
            }

            this._mapper.Map(entity, campaign);
            campaign.CampaignCounty = counties;
            this._context.Campaign.Update(campaign);
        }
    }
}
