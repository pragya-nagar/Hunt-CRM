using System;
using System.Collections.Generic;
using System.Text;
using Synergy.ServiceBus.Abstracts;

namespace Synergy.CRM.Models.Commands
{
    public class MailMergeCommand : Command
    {
        public string PropertyPath { get; set; }

        public Guid TemplateId { get; set; }

        public Guid CampaignId { get; set; }

        public string ResultPath { get; set; }
    }
}
