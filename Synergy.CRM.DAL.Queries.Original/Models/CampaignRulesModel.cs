using System;
using System.Collections.Generic;
using Synergy.DataAccess.Abstractions.Interfaces;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class CampaignRulesModel : IModel
    {
        public Guid CampaignId { get; set; }

        public Guid RuleId { get; set; }

        public IEnumerable<ApplyRulesPropertyItemModel> CampaignRuleItems { get; set; }
    }
}