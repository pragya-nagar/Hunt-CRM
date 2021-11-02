using System;

namespace Synergy.CRM.Models.Opportunity
{
    public class PrimaryContactHistoryModel
    {
        public Guid? CurrentId { get; set; }

        public Guid? PreviouseId { get; set; }

        public DateTime LastUpdate { get; set; }

        public Guid UpdatedBy { get; set; }
    }
}
