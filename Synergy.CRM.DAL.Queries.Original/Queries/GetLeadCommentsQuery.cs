using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.Queries
{
    public class GetLeadCommentsQuery : BaseQuery<LeadComment>, IGetLeadCommentsQuery
    {
        private IMapper _mapper;
        private DbSet<LeadComment> _entity;

        public GetLeadCommentsQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.LeadComment;
        }

        public int? TotalCount { get; private set; }

        #region query builder
        public IGetLeadCommentsQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(lc => lc.Id == id);
            return this;
        }

        public IGetLeadCommentsQuery FilterByLeads(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(lc => ids.Contains(lc.LeadId));
            return this;
        }

        public IGetLeadCommentsQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetLeadCommentsQuery Take(int take)
        {
            this._take = take;
            return this;
        }
        #endregion

        public IEnumerable<LeadCommentModel> Exeсute()
        {
            IQueryable<LeadComment> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            return this._mapper.Map<IEnumerable<LeadCommentModel>>(data.ToList());
        }

        public async Task<IEnumerable<LeadCommentModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<LeadComment> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            return this._mapper.Map<IEnumerable<LeadCommentModel>>(await data.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<LeadComment> BuildQuery()
        {
            this._sortSelector = e => e.CommentDate;
            this.includes.Add(c => c.Author);
            IQueryable<LeadComment> query = this._entity.IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate())
                .OrderBy(this._sortSelector, false)
                .ApplyPaging(this._skip, this._take);

            return query;
        }
    }
}
