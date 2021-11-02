namespace Synergy.CRM.Domain.Abstracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.CRM.Models;
    using Synergy.DataAccess.Enum;

    public interface IContactService
    {
        Task<SearchResultModel<ContactModel>> GetListAsync(SearchArgsModel<ContactFilterArgs, ContactSortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<ContactDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));
    }
}
