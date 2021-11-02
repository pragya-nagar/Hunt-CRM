using System;
using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetPropertiesQuery : IQuery<IGetPropertiesQuery, IEnumerable<PropertyModel>>,
                                           IHavePagination<IGetPropertiesQuery>,
                                           IFilterByLeads<IGetPropertiesQuery>,
                                           IHaveSorting<IGetPropertiesQuery, PropertySortField>,
                                           IHaveSearch<IGetPropertiesQuery>
    {
        IGetPropertiesQuery IncludeContacts();

        IGetPropertiesQuery IncludeValuation();

        IGetPropertiesQuery IncludeDelinquency();

        IGetPropertiesQuery IncludeLead();

        IGetPropertiesQuery FilterByOpportunities(IEnumerable<Guid> ids);
    }
}
