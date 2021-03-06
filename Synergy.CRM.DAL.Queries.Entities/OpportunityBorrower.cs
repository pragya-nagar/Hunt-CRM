using System;

namespace Synergy.CRM.DAL.Queries.Entities
{
    public class OpportunityBorrower : OpportunityBorrowerBase
    {
        public string SSN { get; set; }

        public bool? IsMarried { get; set; }

        public string DateOfBirth { get; set; }

        public Opportunity Opportunity { get; set; }
    }
}
