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
    public class GetCampaignRuleFieldsQuery : BaseQuery<CampaignRuleField>, IGetCampaignRuleFieldsQuery
    {
        private readonly IMapper _mapper;
        private readonly DbSet<CampaignRuleField> _entity;

        public GetCampaignRuleFieldsQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.CampaignRuleField;
        }

        public IGetCampaignRuleFieldsQuery FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CampaignRuleFieldModel> Exeсute()
        {
            return this._mapper.Map<IEnumerable<CampaignRuleFieldModel>>(this.BuildQuery());
        }

        public async Task<IEnumerable<CampaignRuleFieldModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IQueryable<CampaignRuleField> data = this.BuildQuery();
            return this._mapper.Map<IEnumerable<CampaignRuleFieldModel>>(await data.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<CampaignRuleField> BuildQuery()
        {
            this.includes.Add(e => e.CampaignFieldType.CampaignLogicTypes);
            IQueryable<CampaignRuleField> query = this._entity
                .IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate());

            return query;
        }
    }
}
