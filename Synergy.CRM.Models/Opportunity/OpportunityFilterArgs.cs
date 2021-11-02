using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityFilterArgs
    {
        public IEnumerable<Guid> LeadIds { get; set; }

        public IEnumerable<Guid> CampaignIds { get; set; }

        public IEnumerable<Guid> UserIds { get; set; }
    }
}
