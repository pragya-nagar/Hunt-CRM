using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.Common.DAL.Abstract;
using Synergy.Common.Domain.Models.Common;
using Synergy.Common.Domain.Models.Extensions;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Queries.Entities;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.Domain
{
    public class CampaignService : ICampaignService
    {
        private readonly IQueryProvider<CampaignLead> _campaignLeadQueryProvider;
        private readonly IQueryProvider<CampaignRuleCampaign> _ruleCampaignQueryProvider;
        private readonly IQueryProvider<CampaignRule> _ruleQueryProvider;
        private readonly IQueryProvider<CampaignComment> _commentQueryProvider;
        private readonly IGetCampaignsQuery _campaignQuery;
        private readonly IGetCampaignCommentsQuery _campaignCommentsQuery;

        private readonly IMapper _mapper;

        public CampaignService(IGetCampaignsQuery campaignQuery,
            IGetCampaignCommentsQuery campaignCommentsQuery,
            IMapper mapper,
            IQueryProvider<CampaignLead> campaignLeadQueryProvider,
            IQueryProvider<CampaignRuleCampaign> ruleCampaignQueryProvider,
            IQueryProvider<CampaignRule> ruleQueryProvider,
            IQueryProvider<CampaignComment> commentQueryProvider)
        {
            this._campaignQuery = campaignQuery ?? throw new ArgumentNullException(nameof(campaignQuery));
            this._campaignCommentsQuery = campaignCommentsQuery ?? throw new ArgumentNullException(nameof(campaignCommentsQuery));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._campaignLeadQueryProvider = campaignLeadQueryProvider ?? throw new ArgumentNullException(nameof(campaignLeadQueryProvider));
            this._ruleCampaignQueryProvider = ruleCampaignQueryProvider ?? throw new ArgumentNullException(nameof(ruleCampaignQueryProvider));
            this._ruleQueryProvider = ruleQueryProvider ?? throw new ArgumentNullException(nameof(ruleQueryProvider));
            this._commentQueryProvider = commentQueryProvider ?? throw new ArgumentNullException(nameof(commentQueryProvider));
        }

        public async Task<SearchResultModel<CampaignModel>> GetListAsync(SearchArgsModel<CampaignFilterArgs, CampaignSortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._campaignQuery;

            if (string.IsNullOrWhiteSpace(args?.FullSearch) == false)
            {
                var val = args.FullSearch.Trim();
                query.Search(val);
            }

            if (args?.Filter?.LeadIds?.Any() == true)
            {
                var ids = args.Filter.LeadIds;
                query.FilterByLeads(ids);
            }

            if (args?.SortField != null)
            {
                query = (args.SortOrder ?? SortOrder.Asc) == SortOrder.Asc
                    ? query.OrderBy(args.SortField.Value)
                    : query.OrderByDescending(args.SortField.Value);
            }

            query.Skip(args?.Offset ?? 0).Take(args?.Limit ?? 50);

            var items = await query.ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            var count = query.TotalCount ?? 0;

            return new SearchResultModel<CampaignModel>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<CampaignModel>>(items),
            };
        }

        public async Task<CampaignDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await this._campaignQuery.FindById(id).ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            var item = list.FirstOrDefault();
            return item == null ? throw new NotFoundException() : this._mapper.Map<CampaignDetailsModel>(item);
        }

        public async Task<SearchResultModel<CampaignCommentModel>> GetCommentsListAsync(SearchArgsModel<Guid, CommentSortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._campaignCommentsQuery.FilterByCampaign(new List<Guid> { args.Filter });

            query = query.Skip(args.Offset ?? 0).Take(args?.Limit ?? 50);

            var items = await query.ExeсuteAsync(cancellationToken).ConfigureAwait(false);
            var count = query.TotalCount ?? 0;

            return new SearchResultModel<CampaignCommentModel>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<CampaignCommentModel>>(items),
            };
        }

        public async Task<CampaignCommentModel> GetCommentAsync(Guid campaignId, Guid commentId, CancellationToken cancellationToken = default)
        {
            return await this._commentQueryProvider.Query
                .Select(x => new CampaignCommentModel
                {
                    Id = x.Id,
                    CampaignId = x.CampaignId,
                    Comment = x.Comment,
                    Author = new FastEntityModel<Guid>
                    {
                        Id = x.Author.Id,
                        Name = x.Author.FirstName + " " + x.Author.LastName,
                    },
                    CommentDate = x.CommentDate,
                    ModifiedOn = x.ModifiedOn,
                })
                .FirstOrDefaultAsync(x => x.CampaignId == campaignId && x.Id == commentId, cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<string>> GetDumpFieldsAsync(Guid campaignId, CampaignDumpContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var contactCount = await this._campaignLeadQueryProvider.Query
                .Where(x => x.DeletedOn == null && x.CampaignId == campaignId)
                .Select(x => x.Lead.Contacts.Count())
                .MaxAsync(cancellationToken)
                .ConfigureAwait(false);

            return context == CampaignDumpContext.Lead
                ? new CampaignLeadDumpModel(contactCount).ToDataDump().Keys.Distinct()
                : new CampaignPropertyDumpModel(contactCount).ToDataDump().Keys.Distinct();
        }

        public async Task<SearchResultModel<RuleModel>> GetRuleListAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = from r in this._ruleQueryProvider.Query
                        join rc in this._ruleCampaignQueryProvider.Query.Where(x => x.CampaignId == id) on r.Id equals rc.CampaignRuleId into left
                        from rc in left.DefaultIfEmpty()
                        select new RuleModel
                        {
                            Id = r.Id,
                            IsAttached = rc != null ? true : false,
                            Name = r.Name,
                            CampaignRuleItems = r.Items.Select(x => new CampaignRuleItemModel
                            {
                                CampaignLogicType = new FastEntityModel<int>
                                {
                                    Id = x.CampaignLogicTypeId,
                                    Name = x.CampaignLogicType.Description,
                                },
                                Value = x.Value,
                                CampaignRuleField = new FastEntityModel<int>
                                {
                                    Id = x.CampaignRuleFieldId,
                                    Name = x.CampaignRuleField.Description,
                                },
                            }),
                        };

            var list = await query.ToListAsync(cancellationToken).ConfigureAwait(false);
            return new SearchResultModel<RuleModel>
            {
                List = list,
                TotalCount = list.Count,
            };
        }
    }
}
