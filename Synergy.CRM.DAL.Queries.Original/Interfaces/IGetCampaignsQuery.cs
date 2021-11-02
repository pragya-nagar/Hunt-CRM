using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetCampaignsQuery : IQuery<IGetCampaignsQuery, IEnumerable<CampaignModel>>,
                                          IHavePagination<IGetCampaignsQuery>,
                                          IFilterByLeads<IGetCampaignsQuery>,
                                          IHaveSorting<IGetCampaignsQuery, CampaignSortField>,
                                          IHaveSearch<IGetCampaignsQuery>
    {
    }
}
