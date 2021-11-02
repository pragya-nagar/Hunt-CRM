using System.Collections.Generic;
using Synergy.CRM.DAL.Queries.Original.Models;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Interfaces
{
    public interface IGetPropertiesApplyRuleQuery : IQuery<IGetPropertiesApplyRuleQuery, IEnumerable<PropertyApplyRulesModel>>
    {
        IGetPropertiesApplyRuleQuery FilterByRules(IEnumerable<CampaignRulesModel> rules);

        IGetPropertiesApplyRuleQuery FilterByCounty(int countyId);
    }
}
