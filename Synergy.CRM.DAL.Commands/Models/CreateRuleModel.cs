using System;
using System.Collections.Generic;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class CreateRuleModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<RuleItemModel> CampaignRuleItems { get; set; }
    }
}
