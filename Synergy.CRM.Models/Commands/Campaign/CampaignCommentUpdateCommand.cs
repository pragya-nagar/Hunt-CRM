using System;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class CampaignCommentUpdateCommand : Command
    {
        public string Comment { get; set; }
    }
}