using System;
using System.Collections.Generic;

namespace Synergy.CRM.DAL.Commands.Models.Results
{
    public class CampaignRulesAndCountiesModel
    {
        public int StateId { get; set; }

        public List<int> CountyIds { get; set; }

        public List<Guid> RuleIds { get; set; }
    }
}
