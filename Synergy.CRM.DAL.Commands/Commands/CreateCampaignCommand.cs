using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class CreateCampaignCommand : ICreateCampaignCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public CreateCampaignCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(CreateCampaignModel campaignEntity, Guid userId)
        {
            var data = this._mapper.Map<Campaign>(campaignEntity).OnCreateAudit(userId);
            this._context.Campaign.Add(data);
            this._context.SaveChanges();
        }

        public async Task<int> DispatchAsync(CreateCampaignModel opportunityEntity, Guid userId, CancellationToken cancellationToken = default)
        {
            var data = this._mapper.Map<Campaign>(opportunityEntity).OnCreateAudit(userId);

            List<CampaignCounty> counties = new List<CampaignCounty>();
            foreach (var countyId in opportunityEntity.CountyIds)
            {
                counties.Add(new CampaignCounty { Id = Guid.NewGuid(), CampaignId = data.Id, CountyId = countyId }.OnCreateAudit(userId));
            }

            data.CampaignCounty = counties;

            this._context.Campaign.Add(data);
            return await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
