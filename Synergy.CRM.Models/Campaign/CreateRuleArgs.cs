using System.Collections.Generic;

namespace Synergy.CRM.Models
{
    public class CreateRuleArgs
    {
        public string RuleName { get; set; }

        public IEnumerable<RuleItemArgs> RuleItems { get; set; }
    }
}
