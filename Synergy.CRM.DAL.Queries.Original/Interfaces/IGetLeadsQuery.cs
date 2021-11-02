using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetLeadsQuery : IQuery<IGetLeadsQuery, IEnumerable<LeadModel>>,
                                      IHavePagination<IGetLeadsQuery>,
                                      IHaveSearch<IGetLeadsQuery>,
                                      IHaveSorting<IGetLeadsQuery, LeadSortField>
    {
    }
}
