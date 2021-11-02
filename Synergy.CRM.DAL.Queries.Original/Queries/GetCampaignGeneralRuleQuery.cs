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
    public class GetCampaignGeneralRuleQuery : BaseQuery<CampaignRule>, IGetCampaignGeneralRuleQuery
    {
        private readonly IMapper _mapper;
        private readonly DbSet<CampaignRule> _entity;

        public GetCampaignGeneralRuleQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.CampaignRule;
        }

        #region query builder
        public IGetCampaignGeneralRuleQuery FindById(Guid id)
        {
            throw new NotImplementedException();
        }
        #endregion

        public IEnumerable<CampaignGeneralRuleModel> Exeсute()
        {
            return this._mapper.Map<IEnumerable<CampaignGeneralRuleModel>>(this.BuildQuery());
        }

        public async Task<IEnumerable<CampaignGeneralRuleModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this._mapper.Map<IEnumerable<CampaignGeneralRuleModel>>(await BuildQuery().ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<CampaignRule> BuildQuery()
        {
            IQueryable<CampaignRule> query = this._entity.Include(c => c.CampaignRuleItems).ThenInclude(cl => cl.CampaignLogicType)
                .Include(c => c.CampaignRuleItems).ThenInclude(cf => cf.CampaignRuleField)
                .IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate());

            return query;
        }
    }
}
