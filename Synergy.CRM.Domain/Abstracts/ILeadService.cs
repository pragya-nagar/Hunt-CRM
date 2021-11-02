namespace Synergy.CRM.Domain.Abstracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.CRM.Models;
    using Synergy.DataAccess.Enum;

    public interface ILeadService
    {
        Task<SearchResultModel<LeadModel>> GetListAsync(SearchArgsModel<LeadSortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<LeadDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<LeadCommentModel>> GetCommentsListAsync(SearchArgsModel<Guid, CommentSortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<LeadCommentModel> GetCommentAsync(Guid leadId, Guid commentId, CancellationToken cancellationToken = default);
    }
}
