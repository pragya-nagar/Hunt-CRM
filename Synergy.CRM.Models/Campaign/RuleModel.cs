using System;
using System.Collections.Generic;
using Synergy.Common.Domain.Models.Abstracts;

namespace Synergy.CRM.Models
{
    public class RuleModel : IResultModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsAttached { get; set; }

        public IEnumerable<CampaignRuleItemModel> CampaignRuleItems { get; set; }
    }
}
