using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetContactsQuery : IQuery<IGetContactsQuery, IEnumerable<ContactModel>>,
                                         IHavePagination<IGetContactsQuery>,
                                         IFilterByLeads<IGetContactsQuery>,
                                         IHaveSorting<IGetContactsQuery, ContactSortField>,
                                         IHaveSearch<IGetContactsQuery>
    {
    }
}
