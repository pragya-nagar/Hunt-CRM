using Synergy.DataAccess.Abstractions.Models;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class CampaignRuleItemModel : AuditModel
    {
        public string Value { get; set; }

        public FastEntityModel<int> CampaignLogicType { get; set; }

        public FastEntityModel<int> CampaignRuleField { get; set; }
    }
}
