namespace Synergy.CRM.Models.Commands
{
    using System;
    using System.Collections.Generic;
    using Synergy.ServiceBus.Abstracts;

    public class LeadsToCampaignUpdateCommand : Command
    {
        public Guid CampaignId { get; set; }

        public IEnumerable<Guid> CampaignLeadsIds { get; set; }

        public decimal? TotalAmountRecords { get; set; }
    }
}
