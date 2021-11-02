using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Commands.Interfaces;
using Synergy.CRM.DAL.Commands.Models.Opportunity;
using Synergy.DataAccess.Context;

namespace Synergy.CRM.DAL.Commands.Commands
{
    public class UpdateOpportunitySensitiveDataCommand : IUpdateOpportunitySensitiveDataCommand
    {
        private IMapper _mapper;
        private ISynergyContext _context;

        public UpdateOpportunitySensitiveDataCommand(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._context = context;
        }

        public void Dispatch(UpdateOpportunitySensitiveDataModel opportunityEntity, Guid userId)
        {
            DispatchAsync(opportunityEntity, userId).Wait();
        }

        public async Task<int> DispatchAsync(UpdateOpportunitySensitiveDataModel opportunityEntity, Guid userId, CancellationToken cancellationToken = default)
        {
            var opportunity = await this._context.Opportunity.Include(x => x.OpportunityBorrowers)
                                                             .Include(x => x.OpportunityCommercialBorrowers)
                                                             .FirstAsync(x => x.Id == opportunityEntity.Id)
                                                             .ConfigureAwait(false);

            foreach (var borrower in opportunityEntity.BorrowersSensitiveData)
            {
                if (borrower.IsDayOfBirthChanged)
                {
                    opportunity.OpportunityBorrowers.First(x => x.Id == borrower.Id).DateOfBirth = borrower.DayOfBirth.ToString();
                }

                if (borrower.IsSSNChanged)
                {
                    opportunity.OpportunityBorrowers.First(x => x.Id == borrower.Id).SSN = borrower.SSN;
                }

                if (borrower.IsTaxIdNumberChanged)
                {
                    opportunity.OpportunityCommercialBorrowers.First(x => x.Id == borrower.Id).TaxIdNumber = borrower.TaxIdNumber;
                }
            }

            this._context.OpportunityBorrower.UpdateRange(opportunity.OpportunityBorrowers);
            this._context.OpportunityCommercialBorrower.UpdateRange(opportunity.OpportunityCommercialBorrowers);

            return await this._context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
