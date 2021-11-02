using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities.OpportunityEntities;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class CreateOpportunityCommand : ICreateOpportunityCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public CreateOpportunityCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(CreateOpportunityModel opportunityEntity, Guid userId)
        {
            this.DispatchAsync(opportunityEntity, userId).Wait();
        }

        public async Task<int> DispatchAsync(CreateOpportunityModel opportunityEntity, Guid userId, CancellationToken cancellationToken = default)
        {
            var data = this._mapper.Map<Opportunity>(opportunityEntity).OnCreateAudit(userId);
            data.OpportunityProperties.ToList().ForEach(x =>
            {
                x.OnCreateAudit(userId);
            });

            AddBorrowers(userId, data);
            this._context.Opportunity.Add(data);

            return await _context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private void AddBorrowers(Guid userId, Opportunity data)
        {
            if (data.OpportunityPropertyTypeId == (int)Models.OpportunityPropertyType.CommercialEntityOwned)
            {
                data.OpportunityCommercialBorrowers.ToList().ForEach(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.OnCreateAudit(userId);
                    x.OpportunityId = data.Id;
                });

                this._context.OpportunityCommercialBorrower.AddRange(data.OpportunityCommercialBorrowers);
            }
            else
            {
                data.OpportunityBorrowers.ToList().ForEach(x =>
                {
                    x.Id = Guid.NewGuid();
                    x.OnCreateAudit(userId);
                    x.OpportunityId = data.Id;
                });

                this._context.OpportunityBorrower.AddRange(data.OpportunityBorrowers);
            }
        }
    }
}
