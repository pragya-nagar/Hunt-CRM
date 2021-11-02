using System;
using System.Collections.Generic;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class CampaignRulesModel
    {
        public Guid CampaignId { get; set; }

        public Guid RuleId { get; set; }

        public IEnumerable<ApplyRulesPropertyItemModel> CampaignRuleItems { get; set; }
    }
}
