using System;
using System.Collections.Generic;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class ApplyRulesCommand : Command
    {
        public Guid CampaignId { get; set; }

        public IEnumerable<Guid> RuleIds { get; set; }
    }
}