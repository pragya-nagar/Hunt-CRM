using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Abstractions.Models;
using Synergy.DataAccess.Context;
using Synergy.DataAccess.Entities;

namespace Synergy.CRM.DAL.Queries.Original.Queries
{
    public class GetCountyQuery : BaseQuery<County>, IGetCountyQuery
    {
        private IMapper _mapper;
        private DbSet<County> _entity;
        private ISynergyContext _context;

        public GetCountyQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.County;
            this._context = context;
        }

        public int? TotalCount { get; private set; }

        #region build query

        public IGetCountyQuery FilterByState(int stateId)
        {
            this.andAlsoPredicates.Add(c => c.StateId == stateId);
            return this;
        }

        public IGetCountyQuery FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IGetCountyQuery Take(int take)
        {
            this._take = take;
            return this;
        }

        public IGetCountyQuery Skip(int skip)
        {
            this._skip = skip;
            return this;
        }

        public IGetCountyQuery Search(string search)
        {
            if (string.IsNullOrWhiteSpace(search))
            {
                return this;
            }

            search = search.Trim().ToLower();
            andAlsoPredicates.Add(x => x.Name.ToLower().StartsWith(search));

            return this;
        }
        #endregion

        public IEnumerable<FastEntityModel<int>> Exeсute()
        {
            var data = this.BuildQuery().ToList();

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = this._entity.Where(this.GetPredicate()).Count();
            }

            return this._mapper.Map<IEnumerable<FastEntityModel<int>>>(data);
        }

        public async Task<IEnumerable<FastEntityModel<int>>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = await BuildQuery().ToListAsync(cancellationToken).ConfigureAwait(false);

            if (this._skip != null || this._take != null)
            {
                this.TotalCount = await _entity.Where(GetPredicate()).CountAsync(cancellationToken).ConfigureAwait(false);
            }

            return this._mapper.Map<IEnumerable<FastEntityModel<int>>>(data);
        }

        private IQueryable<County> BuildQuery()
        {
            IQueryable<County> query = this._entity
                .Where(this.GetPredicate())
                .OrderBy(c => c.Name, true)
                .ApplyPaging(this._skip, this._take);

            return query;
        }
    }
}
