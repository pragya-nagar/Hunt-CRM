using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Synergy.Common.DAL.Abstract;
using Synergy.Common.Domain.Models.Common;
using Synergy.CRM.DAL.Queries.Original.Interfaces;
using Synergy.CRM.Domain.Abstracts;
using Synergy.CRM.Models;
using Synergy.DataAccess.Dictionaries.Queries.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace Synergy.CRM.Domain
{
    public class DictionaryService : IDictionaryService
    {
        private readonly IMapper _mapper;
        private readonly IGetCampaignTypesQuery _getCampaignTypesQuery;
        private readonly IGetCampaignRuleFieldsQuery _getRuleFieldsQuery;
        private readonly IGetCountyQuery _countyQuery;
        private readonly IQueryProvider<DAL.Queries.Entities.OpportunityPropertyType> _opportunityPropertyTypeQueryProvider;
        private readonly IQueryProvider<DAL.Queries.Entities.OpportunityMonthlyPrepay> _opportunityMonthlyPrepayProvider;
        private readonly IQueryProvider<DAL.Queries.Entities.OpportunityPercentagePrepay> _opportunityPercentagePrepayProvider;
        private readonly IQueryProvider<DAL.Queries.Entities.CollectingEntityType> _collectingEntityTypeQueryProvider;

        public DictionaryService(IMapper mapper,
            IGetCampaignTypesQuery getCampaignTypesQuery,
            IGetCampaignRuleFieldsQuery getRuleFieldsQuery,
            IGetCountyQuery countyQuery,
            IQueryProvider<DAL.Queries.Entities.OpportunityPropertyType> opportunityPropertyTypeQueryProvider,
            IQueryProvider<DAL.Queries.Entities.OpportunityMonthlyPrepay> opportunityMonthlyPrepayProvider,
            IQueryProvider<DAL.Queries.Entities.OpportunityPercentagePrepay> opportunityPercentagePrepayProvider,
            IQueryProvider<DAL.Queries.Entities.CollectingEntityType> collectingEntityTypeQueryProvider)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._getCampaignTypesQuery = getCampaignTypesQuery ?? throw new ArgumentNullException(nameof(getCampaignTypesQuery));
            this._getRuleFieldsQuery = getRuleFieldsQuery ?? throw new ArgumentNullException(nameof(getRuleFieldsQuery));
            this._countyQuery = countyQuery ?? throw new ArgumentNullException(nameof(countyQuery));
            this._opportunityPropertyTypeQueryProvider = opportunityPropertyTypeQueryProvider ?? throw new ArgumentNullException(nameof(opportunityPropertyTypeQueryProvider));
            this._opportunityMonthlyPrepayProvider = opportunityMonthlyPrepayProvider ?? throw new ArgumentNullException(nameof(opportunityMonthlyPrepayProvider));
            this._opportunityPercentagePrepayProvider = opportunityPercentagePrepayProvider ?? throw new ArgumentNullException(nameof(opportunityPercentagePrepayProvider));
            this._collectingEntityTypeQueryProvider = collectingEntityTypeQueryProvider ?? throw new ArgumentNullException(nameof(collectingEntityTypeQueryProvider));
        }

        public async Task<SearchResultModel<FastEntityModel<int>>> GetEnumDictionaryAsync<TEnum>(CancellationToken cancellationToken = default(CancellationToken))
            where TEnum : Enum
        {
            var items = this._mapper.Map<IEnumerable<FastEntityModel<int>>>(Enum.GetValues(typeof(TEnum)).Cast<Enum>());
            return await Task.FromResult(new SearchResultModel<FastEntityModel<int>>
            {
                TotalCount = items.Count(),
                List = items,
            }).ConfigureAwait(false);
        }

        public async Task<SearchResultModel<CampaignTypeModel>> GetCampaignTypesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var items = await this._getCampaignTypesQuery.ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            return new SearchResultModel<CampaignTypeModel>
            {
                TotalCount = items.Count(),
                List = this._mapper.Map<IEnumerable<CampaignTypeModel>>(items),
            };
        }

        public async Task<SearchResultModel<CampaignRuleFieldModel>> GetRuleFieldsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var items = await this._getRuleFieldsQuery.ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            return new SearchResultModel<CampaignRuleFieldModel>
            {
                TotalCount = items.Count(),
                List = this._mapper.Map<IEnumerable<CampaignRuleFieldModel>>(items),
            };
        }

        public async Task<SearchResultModel<FastEntityModel<int>>> GetCountiesAsync(SearchArgsModel<CountySearchArgs, CountySortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._countyQuery;

            if (string.IsNullOrWhiteSpace(args?.FullSearch) == false)
            {
                var val = args.FullSearch.Trim();
                query.Search(val);
            }

            var statId = args?.Filter?.StateId;
            if (statId.HasValue == true)
            {
                query.FilterByState(statId.Value);
            }

            query.Skip(args?.Offset ?? 0).Take(args?.Limit ?? 50);

            var items = await query.ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            var count = query.TotalCount ?? 0;

            return new SearchResultModel<FastEntityModel<int>>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<FastEntityModel<int>>>(items),
            };
        }

        public async Task<SearchResultModel<FastEntityModel<int>>> GetPropertyTypesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var items = await this._opportunityPropertyTypeQueryProvider.Query.ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new SearchResultModel<FastEntityModel<int>>
            {
                TotalCount = items.Count(),
                List = this._mapper.Map<IEnumerable<FastEntityModel<int>>>(items),
            };
        }

        public async Task<SearchResultModel<FastEntityModel<int>>> GetMonthlyPrepayFieldsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var items = await this._opportunityMonthlyPrepayProvider.Query.ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new SearchResultModel<FastEntityModel<int>>
            {
                TotalCount = items.Count(),
                List = this._mapper.Map<IEnumerable<FastEntityModel<int>>>(items),
            };
        }

        public async Task<SearchResultModel<FastEntityModel<int>>> GetPercentagePrepayFieldsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var items = await this._opportunityPercentagePrepayProvider.Query.ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new SearchResultModel<FastEntityModel<int>>
            {
                TotalCount = items.Count(),
                List = this._mapper.Map<IEnumerable<FastEntityModel<int>>>(items),
            };
        }

        public async Task<SearchResultModel<FastEntityModel<int>>> GetCollectingEntityTypesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var items = await this._collectingEntityTypeQueryProvider.Query.ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            return new SearchResultModel<FastEntityModel<int>>
            {
                TotalCount = items.Count(),
                List = this._mapper.Map<IEnumerable<FastEntityModel<int>>>(items),
            };
        }
    }
}
