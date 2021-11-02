using System;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class DeleteRulesCommand : Command
    {
        public Guid CampaignId { get; set; }
    }
}