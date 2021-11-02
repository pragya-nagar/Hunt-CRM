using System;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityCommercialBorrowerModel : OpportunityBorrowerBase
    {
        public Guid Id { get; set; }

        public string EntityName { get; set; }

        public string Title { get; set; }
    }
}
