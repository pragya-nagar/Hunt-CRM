﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions;
using Synergy.DataAccess.Context;
using OpportunityBorrower = Synergy.DataAccess.Entities.OpportunityEntities.OpportunityBorrower;

namespace Synergy.CRM.DAL.Queries.Original.Queries
{
    public class GetBorrowerSensitiveDataQuery : BaseQuery<OpportunityBorrower>, IGetBorrowerSensitiveDataQuery
    {
        private readonly DbSet<OpportunityBorrower> _entity;
        private readonly IMapper _mapper;

        public GetBorrowerSensitiveDataQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.OpportunityBorrower;
        }

        #region query builder
        public IGetBorrowerSensitiveDataQuery FindById(Guid id)
        {
            this.andAlsoPredicates.Add(lc => lc.Id == id);
            return this;
        }

        public IGetBorrowerSensitiveDataQuery FilterByOpportunity(Guid id)
        {
            this.andAlsoPredicates.Add(lc => lc.OpportunityId == id);
            return this;
        }
        #endregion

        public BorrowerSensetiveData Exeсute()
        {
            return this._mapper.Map<BorrowerSensetiveData>(BuildQuery().Single());
        }

        public async Task<BorrowerSensetiveData> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this._mapper.Map<BorrowerSensetiveData>(await BuildQuery().SingleAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<OpportunityBorrower> BuildQuery()
        {
            IQueryable<OpportunityBorrower> query = this._entity
                .Where(this.GetPredicate());

            return query;
        }
    }
}
