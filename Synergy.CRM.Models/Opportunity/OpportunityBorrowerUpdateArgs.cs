using System;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityBorrowerUpdateArgs : OpportunityBorrowerCreateArgs
    {
        public Guid Id { get; set; }
    }
}
