using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.Common.DAL.Abstract;
using Synergy.Common.Domain.Models.Common;
using Synergy.Common.Exceptions;
using Synergy.CRM.DAL.Queries.Entities;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.Domain
{
    public class LeadService : ILeadService
    {
        private readonly IGetLeadsQuery _leadQuery;
        private readonly IGetLeadCommentsQuery _leadCommentsQuery;

        private readonly IQueryProvider<LeadComment> _commentQueryProvider;

        private readonly IMapper _mapper;

        public LeadService(
            IGetLeadsQuery leadQuery,
            IGetLeadCommentsQuery leadCommentsQuery,
            IQueryProvider<LeadComment> commentQueryProvider,
            IMapper mapper)
        {
            this._leadQuery = leadQuery ?? throw new ArgumentNullException(nameof(leadQuery));
            this._leadCommentsQuery = leadCommentsQuery ?? throw new ArgumentNullException(nameof(leadCommentsQuery));

            this._commentQueryProvider = commentQueryProvider ?? throw new ArgumentNullException(nameof(commentQueryProvider));

            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SearchResultModel<LeadModel>> GetListAsync(SearchArgsModel<LeadSortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._leadQuery;

            if (string.IsNullOrWhiteSpace(args?.FullSearch) == false)
            {
                var val = args.FullSearch.Trim();
                query.Search(val);
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

            return new SearchResultModel<LeadModel>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<LeadModel>>(items),
            };
        }

        public async Task<LeadDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await this._leadQuery.FindById(id).ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            var item = list.FirstOrDefault();
            return item == null ? throw new NotFoundException() : this._mapper.Map<LeadDetailsModel>(item);
        }

        public async Task<SearchResultModel<LeadCommentModel>> GetCommentsListAsync(SearchArgsModel<Guid, CommentSortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._leadCommentsQuery.FilterByLeads(new List<Guid> { args.Filter });

            query = query.Skip(args.Offset ?? 0).Take(args?.Limit ?? 50);

            var items = await query.ExeсuteAsync(cancellationToken).ConfigureAwait(false);
            var count = query.TotalCount ?? 0;

            return new SearchResultModel<LeadCommentModel>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<LeadCommentModel>>(items),
            };
        }

        public async Task<LeadCommentModel> GetCommentAsync(Guid leadId, Guid commentId, CancellationToken cancellationToken = default)
        {
            return await this._commentQueryProvider.Query
                .Select(x => new LeadCommentModel
                {
                    Id = x.Id,
                    LeadId = x.LeadId,
                    Comment = x.Comment,
                    Author = new FastEntityModel<Guid>
                    {
                        Id = x.Author.Id,
                        Name = x.Author.FirstName + " " + x.Author.LastName,
                    },
                    CommentDate = x.CommentDate,
                    ModifiedOn = x.ModifiedOn,
                })
                .FirstOrDefaultAsync(x => x.LeadId == leadId && x.Id == commentId, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
