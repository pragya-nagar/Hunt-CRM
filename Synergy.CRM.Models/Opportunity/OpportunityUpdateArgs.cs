using System.Collections.Generic;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityUpdateArgs : OpportunityCreateArgs
    {
        public new List<OpportunityBorrowerUpdateArgs> Borrowers { get; set; }

        public new List<OpportunityCommercialBorrowerUpdateArgs> CommercialBorrowers { get; set; }
    }
}
