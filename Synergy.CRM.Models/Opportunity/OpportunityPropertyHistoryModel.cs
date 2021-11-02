using System;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityPropertyHistoryModel
    {
        public DateTime? ModifiedOn { get; set; }

        public Guid UpdatedBy { get; set; }

        public DateTime? DeletedOn { get; set; }

        public Guid PropertyId { get; set; }
    }
}
