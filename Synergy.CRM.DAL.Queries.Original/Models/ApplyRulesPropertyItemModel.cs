using Synergy.DataAccess.Enum;

namespace Synergy.CRM.DAL.Queries.Original.Models
{
    public class ApplyRulesPropertyItemModel
    {
        public string Value { get; set; }

        public LogicType LogicType { get; set; }

        public RuleField RuleField { get; set; }
    }
}