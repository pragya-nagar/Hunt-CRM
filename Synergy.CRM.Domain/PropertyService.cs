namespace Synergy.CRM.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoMapper;
    using Synergy.Common.Domain.Models.Common;
    using Synergy.Common.Exceptions;
    using Synergy.CRM.DAL.Queries.Original.Interfaces;
    using Synergy.CRM.Domain.Abstracts;
    using Synergy.CRM.Models;
    using Synergy.DataAccess.Enum;

    public class PropertyService : IPropertyService
    {
        private readonly IGetPropertiesQuery _propertyQuery;
        private readonly IMapper _mapper;

        public PropertyService(IMapper mapper, IGetPropertiesQuery propertyQuery)
        {
            this._propertyQuery = propertyQuery ?? throw new ArgumentNullException(nameof(propertyQuery));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<SearchResultModel<PropertyModel>> GetListAsync(SearchArgsModel<PropertyFilterArgs, PropertySortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._propertyQuery
                .IncludeLead()
                .IncludeValuation()
                .IncludeDelinquency();

            if (string.IsNullOrWhiteSpace(args?.FullSearch) == false)
            {
                var val = args.FullSearch.Trim();
                query.Search(val);
            }

            if (args?.Filter?.LeadIds?.Any() == true)
            {
                var ids = args.Filter.LeadIds;
                query.FilterByLeads(ids);
            }

            if (args?.Filter?.OpportunityIds?.Any() == true)
            {
                var ids = args.Filter.LeadIds;
                query.FilterByOpportunities(ids);
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

            var properties = this._mapper.Map<IEnumerable<PropertyModel>>(items).ToList();

            return new SearchResultModel<PropertyModel>
            {
                TotalCount = count,
                List = properties,
            };
        }

        public async Task<PropertyDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await this._propertyQuery
                .FindById(id)
                .IncludeLead()
                .IncludeContacts()
                .IncludeValuation()
                .IncludeDelinquency()
                .ExeсuteAsync(cancellationToken)
                .ConfigureAwait(false);

            var item = list.FirstOrDefault();
            if (item == null)
            {
                throw new NotFoundException();
            }

            var result = this._mapper.Map<PropertyDetailsModel>(item);

            return result;
        }
    }
}
