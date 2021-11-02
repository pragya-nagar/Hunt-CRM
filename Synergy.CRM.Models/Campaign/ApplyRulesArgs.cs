using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models
{
    public class ApplyRulesArgs
    {
        public Guid RuleId { get; set; }

        public string RuleName { get; set; }

        public IEnumerable<RuleItemArgs> RuleItems { get; set; }
    }
}
