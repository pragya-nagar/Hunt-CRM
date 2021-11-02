using System;
using System.Threading;
using System.Threading.Tasks;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.Domain.Abstracts
{
    public interface IPropertyService
    {
        Task<SearchResultModel<PropertyModel>> GetListAsync(SearchArgsModel<PropertyFilterArgs, PropertySortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<PropertyDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
    }
}
