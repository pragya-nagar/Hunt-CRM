using System.Text;

namespace Synergy.CRM.Domain.Abstracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.CRM.Models.Opportunity;
    using Synergy.DataAccess.Enum;

    public interface IOpportunityService
    {
        Task<SearchResultModel<OpportunityModel>> GetListAsync(SearchArgsModel<OpportunityFilterArgs, OpportunitySortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<OpportunityDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        Task<string> GetSensitiveData(Guid opportunityId, Guid borrowerId, OpportunitySensitiveDataField field, CancellationToken cancellationToken = default(CancellationToken));

        StringBuilder GetPdfToExport(string opportunityNo, CancellationToken cancellationToken = default(CancellationToken));
    }
}
