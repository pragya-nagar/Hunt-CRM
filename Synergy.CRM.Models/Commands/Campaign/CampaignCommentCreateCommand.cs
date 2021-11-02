using System;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class CampaignCommentCreateCommand : Command
    {
        public Guid CampaignId { get; set; }

        public string Comment { get; set; }
    }
}