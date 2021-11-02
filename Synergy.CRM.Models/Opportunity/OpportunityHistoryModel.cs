using System;
using System.Collections.Generic;

namespace Synergy.CRM.Models.Opportunity
{
    public class OpportunityHistoryModel
    {
        public DateTime LastUpdate { get; set; }

        public Guid UpdatedBy { get; set; }

        public OpportunityHistoryFieldName Field { get; set; }

        public BorrowerHistoryModel Borrower { get; set; }

        public string PreviousValue { get; set; }

        public string NewValue { get; set; }

        public IEnumerable<string> PreviousValues { get; set; }

        public IEnumerable<string> NewValues { get; set; }
    }
}
