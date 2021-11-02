using System;
using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;
using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetOpportunitiesQuery : IQuery<IGetOpportunitiesQuery, IEnumerable<OpportunityModel>>,
                                              IHavePagination<IGetOpportunitiesQuery>,
                                              IFilterByLeads<IGetOpportunitiesQuery>,
                                              IHaveSorting<IGetOpportunitiesQuery, OpportunitySortField>,
                                              IHaveSearch<IGetOpportunitiesQuery>
    {
        IGetOpportunitiesQuery FindByLeadIds(IEnumerable<Guid> ids);

        IGetOpportunitiesQuery FindByCampaignIds(IEnumerable<Guid> ids);

        IGetOpportunitiesQuery FindByUserIds(IEnumerable<Guid> ids);

        IGetOpportunitiesQuery IncludeContact();

        PdfExportModel GetPdfToExport(string opportunityNo);
    }
}
