using System;
using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetCampaignCommentsQuery : IQuery<IGetCampaignCommentsQuery, IEnumerable<CampaignCommentModel>>,
                                         IHavePagination<IGetCampaignCommentsQuery>
    {
        IGetCampaignCommentsQuery FilterByCampaign(IEnumerable<Guid> ids);
    }
}
