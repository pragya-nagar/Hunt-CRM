using Synergy.Common.Domain.Models.Common;

namespace Synergy.CRM.Models
{
    public class CampaignRuleItemModel
    {
        public string Value { get; set; }

        public FastEntityModel<int> CampaignLogicType { get; set; }

        public FastEntityModel<int> CampaignRuleField { get; set; }
    }
}