using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models
{
    public class PropertyFilterArgs
    {
        public IEnumerable<Guid> LeadIds { get; set; }

        public IEnumerable<Guid> OpportunityIds { get; set; }
    }
}