using System;

namespace Synergy.CRM.DAL.Commands.Models
{
    public class RuleItemModel
    {
        public Guid Id { get; set; }

        public string Value { get; set; }

        public int CampaignLogicTypeId { get; set; }

        public int CampaignRuleFieldId { get; set; }
    }
}