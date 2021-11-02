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
    public class GetCampaignRulesQuery : BaseQuery<CampaignRuleCampaign>, IGetCampaignRulesQuery
    {
        private IMapper _mapper;
        private DbSet<CampaignRuleCampaign> _entity;

        public GetCampaignRulesQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.CampaignRuleCampaign;
        }

        #region query builder

        public IGetCampaignRulesQuery FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IGetCampaignRulesQuery FindByCampaignId(Guid campaignId)
        {
            this.andAlsoPredicates.Add(e => e.CampaignId == campaignId);
            return this;
        }

        #endregion

        public IEnumerable<CampaignRulesModel> Exeсute()
        {
            var data = this.BuildQuery();
            return this._mapper.Map<IEnumerable<CampaignRulesModel>>(data.ToList());
        }

        public async Task<IEnumerable<CampaignRulesModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = this.BuildQuery();
            return this._mapper.Map<IEnumerable<CampaignRulesModel>>(await data.ToListAsync(cancellationToken).ConfigureAwait(false));
        }

        private IQueryable<CampaignRuleCampaign> BuildQuery()
        {
            this.includes.Add(x => x.Campaign);
            IQueryable<CampaignRuleCampaign> query = this._entity
                .Include(e => e.CampaignRule).ThenInclude(x => x.CampaignRuleItems).ThenInclude(x => x.CampaignLogicType)
                .Include(e => e.CampaignRule).ThenInclude(x => x.CampaignRuleItems).ThenInclude(x => x.CampaignRuleField)
                .IncludeMultiple(this.includes.ToArray())
                .Where(this.GetPredicate());

            return query;
        }
    }
}
