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
    public class GetCampaignLogicTypesQuery : BaseQuery<CampaignLogicType>, IGetCampaignLogicTypesQuery
    {
        private readonly IMapper _mapper;
        private readonly DbSet<CampaignLogicType> _entity;

        public GetCampaignLogicTypesQuery(ISynergyContext context, IMapper mapper)
        {
            this._mapper = mapper;
            this._entity = context.CampaignLogicType;
        }

        public IGetCampaignLogicTypesQuery FindById(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CampaignLogicTypeModel> Exeсute()
        {
            return this._mapper.Map<IEnumerable<CampaignLogicTypeModel>>(this._entity.Where(this.GetPredicate()));
        }

        public async Task<IEnumerable<CampaignLogicTypeModel>> ExeсuteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return this._mapper.Map<IEnumerable<CampaignLogicTypeModel>>(await _entity.Where(GetPredicate()).ToListAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}
