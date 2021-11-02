namespace Synergy.CRM.Models.Commands
{
    using System;
    using System.Collections.Generic;
    using Synergy.ServiceBus.Abstracts;

    public class MakeCampaignDataDumpCommand : Command
    {
        public Guid CampaignId { get; set; }

        public string Key { get; set; }

        public IEnumerable<CampaignField> Fields { get; set; }
    }
}
