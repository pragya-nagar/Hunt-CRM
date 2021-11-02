using System.Collections.Generic;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class CreateRuleCommand : Command
    {
        public string RuleName { get; set; }

        public IEnumerable<RuleItem> RuleItems { get; set; }
    }
}