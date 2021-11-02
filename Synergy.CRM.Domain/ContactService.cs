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

    public class ContactService : IContactService
    {
        private readonly IGetContactsQuery _contactsQuery;
        private readonly IMapper _mapper;

        public ContactService(IMapper mapper, IGetContactsQuery contactsQuery)
        {
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._contactsQuery = contactsQuery ?? throw new ArgumentNullException(nameof(contactsQuery));
        }

        public async Task<SearchResultModel<ContactModel>> GetListAsync(SearchArgsModel<ContactFilterArgs, ContactSortField> args, CancellationToken cancellationToken = default(CancellationToken))
        {
            var query = this._contactsQuery;

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

            if (args?.SortField != null)
            {
                query = (args.SortOrder ?? SortOrder.Asc) == SortOrder.Asc
                    ? query.OrderBy(args.SortField.Value)
                    : query.OrderByDescending(args.SortField.Value);
            }

            query.Skip(args?.Offset ?? 0).Take(args?.Limit ?? 50);

            var items = await query.ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            var count = query.TotalCount ?? 0;

            return new SearchResultModel<ContactModel>
            {
                TotalCount = count,
                List = this._mapper.Map<IEnumerable<ContactModel>>(items),
            };
        }

        public async Task<ContactDetailsModel> FindAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await this._contactsQuery.FindById(id).ExeсuteAsync(cancellationToken).ConfigureAwait(false);

            var item = list.FirstOrDefault();
            return item == null ? throw new NotFoundException() : this._mapper.Map<ContactDetailsModel>(item);
        }
    }
}
