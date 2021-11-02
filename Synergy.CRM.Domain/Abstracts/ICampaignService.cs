namespace Synergy.CRM.Domain.Abstracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.CRM.Models;
    using Synergy.DataAccess.Enum;

    public interface ICampaignService
    {
        Task<SearchResultModel<CampaignModel>> GetListAsync(SearchArgsModel<CampaignFilterArgs, CampaignSortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<CampaignDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<RuleModel>> GetRuleListAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken));

        Task<IEnumerable<string>> GetDumpFieldsAsync(Guid campaignId, CampaignDumpContext context, CancellationToken cancellationToken = default(CancellationToken));

        Task<SearchResultModel<CampaignCommentModel>> GetCommentsListAsync(SearchArgsModel<Guid, CommentSortField> args, CancellationToken cancellationToken = default(CancellationToken));

        Task<CampaignCommentModel> GetCommentAsync(Guid campaignId, Guid commentId, CancellationToken cancellationToken = default);
    }
}
