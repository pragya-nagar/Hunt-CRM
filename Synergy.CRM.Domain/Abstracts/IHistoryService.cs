using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Synergy.CRM.Models.Opportunity;

namespace Synergy.CRM.Domain.Abstracts
{
    public interface IHistoryService
    {
        Task<List<OpportunityHistoryModel>> GetOpportunityHistoryAsync(Guid id, OpportunityHistoryFilterArgs filterArgs, CancellationToken cancellationToken = default(CancellationToken));
    }
}
