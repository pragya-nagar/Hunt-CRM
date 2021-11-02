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
    public class GetCampaignCommentsQuery : BaseQuery<CampaignComment>, IGetCampaignCommentsQuery
    {
        private readonly IMapper _mapper;
        private readonly DbSet<CampaignComment> _entity;

        public GetCampaignCommentsQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.CampaignComment;
        }

        public int? TotalCount { get; private set; }

        #region query builder
        public IGetCampaignCommentsQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(lc => lc.Id == id);
            return this;
        }

        public IGetCampaignCommentsQuery FilterByCampaign(IEnumerable<Guid> ids)
        {
            this.andAlsoPredicates.Add(lc => ids.Contains(lc.CampaignId));
            return this;
        }

        public IGetCampaignCommentsQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetCampaignCommentsQuery Take(int take)
        {
            this._take = take;
            return this;
        }
        #endregion

        public IEnumerable<CampaignCommentModel> Exeсute()
        {
            IQueryable<CampaignComment> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            return this._mapper.Map<IEnumerable<CampaignCommentModel>>(data.ToList());
        }

        public async Task<IEnumerable<CampaignCommentModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<CampaignComment> data = this.BuildQuery();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            return this._mapper.Map<IEnumerable<CampaignCommentModel>>(await data.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<CampaignComment> BuildQuery()
        {
            this._sortSelector = e => e.CommentDate;
            this.includes.Add(c => c.Author);
            IQueryable<CampaignComment> query = this._entity.IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate())
                .OrderBy(this._sortSelector, false)
                .ApplyPaging(this._skip, this._take);

            return query;
        }
    }
}
