using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models
{
    public class CampaignFilterArgs
    {
        public IEnumerable<Guid> LeadIds { get; set; }
    }
}
