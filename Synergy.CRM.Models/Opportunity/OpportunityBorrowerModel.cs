using System;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityBorrowerModel : OpportunityBorrowerBase
    {
        public Guid Id { get; set; }

        public bool? IsMarried { get; set; }
    }
}
