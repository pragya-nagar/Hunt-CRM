using System;
using System.Threading;
using System.Threading.Tasks;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.Models;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.Domain.Abstracts
{
    public interface IReminderService
    {
        Task<SearchResultModel<ReminderDetails>> GetListAsync(SearchArgsModel<ReminderFilterArgs, ReminderSortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<ReminderDetails> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
    }
}
