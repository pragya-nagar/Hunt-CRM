using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models;
using Synergy.DataAccess.Abstractions.Commands;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;
using Synergy.DataAccess.Entities.OpportunityEntities;
using Task = System.Threading.Tasks.Task;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class UpdateOpportunityCommand : IUpdateOpportunityCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public UpdateOpportunityCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(UpdateOpportunityModel opportunityEntity, Guid userId)
        {
            DispatchAsync(opportunityEntity, userId).Wait();
        }

        public async Task<int> DispatchAsync(UpdateOpportunityModel opportunityEntity, Guid userId, CancellationToken cancellationToken = default)
        {
            await this.UpdateOpportunity(opportunityEntity, userId, cancellationToken).ConfigureAwait(false);
            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }

        private async Task UpdateOpportunity(UpdateOpportunityModel opportunityEntity, Guid userId, CancellationToken cancellationToken = default)
        {
            Opportunity opportunity = await this._context.Opportunity
                                                        .Include(o => o.OpportunityProperties)
                                                        .Include(x => x.OpportunityBorrowers)
                                                        .Include(x => x.OpportunityCommercialBorrowers)
                                        .SingleAsync(x => x.Id == opportunityEntity.Id, cancellationToken).ConfigureAwait(false);

            this._mapper.Map(opportunityEntity, opportunity.OnModifyAudit(userId));

            List<OpportunityProperty> properties = opportunity.OpportunityProperties.ToList();

            List<OpportunityProperty> toRemove = properties.Where(o => !opportunityEntity.Properties.Contains(o.PropertyId)).ToList();
            toRemove.ForEach(r => properties.Remove(r));

            List<OpportunityProperty> toAdd = opportunityEntity.Properties
                                .Where(o => !properties.Select(p => p.PropertyId).Contains(o))
                                .Select(np => new OpportunityProperty { OpportunityId = opportunity.Id, PropertyId = np }).ToList();
            toAdd.ForEach(x =>
            {
                x.OnCreateAudit(userId);
                x.Id = Guid.NewGuid();
            });

            properties.AddRange(toAdd);

            opportunity.OpportunityProperties = properties;

            UpdateOpportunityBorrowers(userId, opportunity, opportunityEntity);

            this._context.Opportunity.Update(opportunity.OnModifyAudit(userId));
        }

        private void UpdateOpportunityBorrowers(Guid userId, Opportunity opportunity, UpdateOpportunityModel opportunityEntity)
        {
            switch (opportunity.OpportunityPropertyTypeId)
            {
                case (int)Models.OpportunityPropertyType.CommercialEntityOwned:
                    UpdateCommercialBorrowers(userId, opportunity, opportunityEntity);
                    break;

                case (int)Models.OpportunityPropertyType.CommercialIndividuallyOwned:
                case (int)Models.OpportunityPropertyType.CommercialLand:
                case (int)Models.OpportunityPropertyType.ResidentialNonOwnerOccupied:
                case (int)Models.OpportunityPropertyType.ResidentialOwnerOccupied:
                    UpdateBorrowers(userId, opportunity, opportunityEntity);
                    break;
                default:
                    throw new NotFoundException($"There is no OpportunityPropertyTypeId value {opportunity.OpportunityPropertyTypeId}");
            }
        }

        private void UpdateBorrowers(Guid userId, Opportunity opportunity, UpdateOpportunityModel opportunityEntity)
        {
            // If property type previously was Commercial Entity Owned - delete Opportunity Commercial Borrowers
            if (opportunity.OpportunityCommercialBorrowers.Any())
            {
                this._context.OpportunityCommercialBorrower.RemoveRange(opportunity.OpportunityCommercialBorrowers);
            }

            // delete borrowers that are not in list for update
            var toDelete = opportunity.OpportunityBorrowers.Where(x => !opportunityEntity.OpportunityBorrowers.Select(o => o.Id).Contains(x.Id));

            if (toDelete.Any())
            {
                this._context.OpportunityBorrower.RemoveRange(toDelete);
            }

            foreach (var item in opportunityEntity.OpportunityBorrowers)
            {
                // find entity that belongs to opportunity and update it
                var existingBorrower = opportunity.OpportunityBorrowers.Where(x => x.Id == item.Id && x.OpportunityId == opportunity.Id).FirstOrDefault();
                if (existingBorrower != null)
                {
                    this._mapper.Map(item, existingBorrower);
                    existingBorrower.OnModifyAudit(userId);

                    this._context.OpportunityBorrower.Update(existingBorrower);
                    continue;
                }

                // if Guid.Empty we need to add new entity
                if (item.Id == Guid.Empty)
                {
                    existingBorrower = new OpportunityBorrower
                    {
                        Id = Guid.NewGuid(),
                        OpportunityId = opportunity.Id,
                    }.OnCreateAudit(userId);
                }

                this._mapper.Map(item, existingBorrower);
                this._context.OpportunityBorrower.Add(existingBorrower);
            }
        }

        private void UpdateCommercialBorrowers(Guid userId, Opportunity opportunity, UpdateOpportunityModel opportunityEntity)
        {
            // If property type changed to Commercial Entity Owned - delete Opportunity Borrowers
            if (opportunity.OpportunityBorrowers.Any())
            {
                this._context.OpportunityBorrower.RemoveRange(opportunity.OpportunityBorrowers);
            }

            // delete commercial borrowers that are not in list for update
            var toDelete = opportunity.OpportunityCommercialBorrowers.Where(x => !opportunityEntity.OpportunityCommercialBorrowers.Select(o => o.Id).Contains(x.Id));

            if (toDelete.Any())
            {
                this._context.OpportunityCommercialBorrower.RemoveRange(toDelete);
            }

            foreach (var item in opportunityEntity.OpportunityCommercialBorrowers)
            {
                // find entity that belongs to opportunity and update it
                var existingBorrower = opportunity.OpportunityCommercialBorrowers.Where(x => x.Id == item.Id && x.OpportunityId == opportunity.Id).FirstOrDefault();
                if (existingBorrower != null)
                {
                    this._mapper.Map(item, existingBorrower);
                    existingBorrower.OnModifyAudit(userId);

                    this._context.OpportunityCommercialBorrower.Update(existingBorrower);
                    continue;
                }

                // if Guid.Empty we need to add new entity
                if (item.Id == Guid.Empty)
                {
                    existingBorrower = new OpportunityCommercialBorrower
                    {
                        Id = Guid.NewGuid(),
                        OpportunityId = opportunity.Id,
                    }.OnCreateAudit(userId);

                    this._mapper.Map(item, existingBorrower);
                    this._context.OpportunityCommercialBorrower.Add(existingBorrower);
                }
            }
        }
    }
}
